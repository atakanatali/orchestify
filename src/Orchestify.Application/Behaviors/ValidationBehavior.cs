using FluentValidation;
using MediatR;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Behaviors;

/// <summary>
/// Pipeline behavior for automatic validation of requests.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
            
            // Try to return ServiceResult.Failure if TResponse is ServiceResult
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ServiceResult<>))
            {
                var method = typeof(ServiceResult<>).MakeGenericType(typeof(TResponse).GetGenericArguments())
                    .GetMethod("Failure", new[] { typeof(string), typeof(string) });
                    
                if (method != null)
                {
                    return (TResponse)method.Invoke(null, new object[] { "VALIDATION_ERROR", errorMessage })!;
                }
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}
