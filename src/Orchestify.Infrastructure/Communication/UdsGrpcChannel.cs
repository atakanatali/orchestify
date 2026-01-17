using System.Net.Http;
using System.Net.Sockets;
using Grpc.Net.Client;

namespace Orchestify.Infrastructure.Communication;

/// <summary>
/// Provides a gRPC channel that communicates over Unix Domain Sockets.
/// </summary>
public static class UdsGrpcChannel
{
    public static GrpcChannel Create(string socketPath)
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        
        var connectionFactory = new SocketsHttpHandler
        {
            ConnectCallback = async (context, cancellationToken) =>
            {
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                try
                {
                    await socket.ConnectAsync(udsEndPoint, cancellationToken).ConfigureAwait(false);
                    return new NetworkStream(socket, ownsSocket: true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        };

        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = connectionFactory
        });
    }
}
