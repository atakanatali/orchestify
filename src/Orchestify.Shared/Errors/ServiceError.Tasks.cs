namespace Orchestify.Shared.Errors;

/// <summary>
/// Task domain error codes for the Orchestify platform.
/// Covers task management, assignment, dependencies, and task lifecycle operations.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Task domain identifier for error code formatting.
    /// </summary>
    private const string TaskDomain = "TASK";

    /// <summary>
    /// Error codes and messages for task operations.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Task not found.
        /// </summary>
        public static string NotFound => FormatErrorCode(TaskDomain, "NOT_FOUND");

        /// <summary>
        /// Task title is invalid or empty.
        /// </summary>
        public static string InvalidTitle => FormatErrorCode(TaskDomain, "INVALID_TITLE");

        /// <summary>
        /// Task description is invalid.
        /// </summary>
        public static string InvalidDescription => FormatErrorCode(TaskDomain, "INVALID_DESCRIPTION");

        /// <summary>
        /// Task priority is invalid.
        /// </summary>
        public static string InvalidPriority => FormatErrorCode(TaskDomain, "INVALID_PRIORITY");

        /// <summary>
        /// Task status is invalid for the operation.
        /// </summary>
        public static string InvalidStatus => FormatErrorCode(TaskDomain, "INVALID_STATUS");

        /// <summary>
        /// Task due date is in the past.
        /// </summary>
        public static string InvalidDueDate => FormatErrorCode(TaskDomain, "INVALID_DUE_DATE");

        /// <summary>
        /// Cannot assign task to non-member.
        /// </summary>
        public static string InvalidAssignee => FormatErrorCode(TaskDomain, "INVALID_ASSIGNEE");

        /// <summary>
        /// Task is already assigned.
        /// </summary>
        public static string AlreadyAssigned => FormatErrorCode(TaskDomain, "ALREADY_ASSIGNED");

        /// <summary>
        /// Task dependency not found.
        /// </summary>
        public static string DependencyNotFound => FormatErrorCode(TaskDomain, "DEPENDENCY_NOT_FOUND");

        /// <summary>
        /// Circular dependency detected.
        /// </summary>
        public static string CircularDependency => FormatErrorCode(TaskDomain, "CIRCULAR_DEPENDENCY");

        /// <summary>
        /// Cannot add task as dependency of itself.
        /// </summary>
        public static string SelfDependency => FormatErrorCode(TaskDomain, "SELF_DEPENDENCY");

        /// <summary>
        /// Task has unmet dependencies.
        /// </summary>
        public static string HasUnmetDependencies => FormatErrorCode(TaskDomain, "HAS_UNMET_DEPENDENCIES");

        /// <summary>
        /// Task is already completed.
        /// </summary>
        public static string AlreadyCompleted => FormatErrorCode(TaskDomain, "ALREADY_COMPLETED");

        /// <summary>
        /// Task is already cancelled.
        /// </summary>
        public static string AlreadyCancelled => FormatErrorCode(TaskDomain, "ALREADY_CANCELLED");

        /// <summary>
        /// Task is locked and cannot be modified.
        /// </summary>
        public static string IsLocked => FormatErrorCode(TaskDomain, "IS_LOCKED");

        /// <summary>
        /// Task label not found.
        /// </summary>
        public static string LabelNotFound => FormatErrorCode(TaskDomain, "LABEL_NOT_FOUND");

        /// <summary>
        /// Task comment not found.
        /// </summary>
        public static string CommentNotFound => FormatErrorCode(TaskDomain, "COMMENT_NOT_FOUND");

        /// <summary>
        /// Task attachment not found.
        /// </summary>
        public static string AttachmentNotFound => FormatErrorCode(TaskDomain, "ATTACHMENT_NOT_FOUND");

        /// <summary>
        /// Task has reached maximum attachment limit.
        /// </summary>
        public static string AttachmentLimitExceeded => FormatErrorCode(TaskDomain, "ATTACHMENT_LIMIT_EXCEEDED");

        /// <summary>
        /// Invalid attachment file type.
        /// </summary>
        public static string InvalidAttachmentType => FormatErrorCode(TaskDomain, "INVALID_ATTACHMENT_TYPE");

        /// <summary>
        /// Task has no parent.
        /// </summary>
        public static string NoParent => FormatErrorCode(TaskDomain, "NO_PARENT");

        /// <summary>
        /// Cannot create subtask depth limit exceeded.
        /// </summary>
        public static string SubtaskDepthExceeded => FormatErrorCode(TaskDomain, "SUBTASK_DEPTH_EXCEEDED");

        /// <summary>
        /// Cannot send messages while task is in progress.
        /// </summary>
        public static string CannotMessageWhileRunning => FormatErrorCode(TaskDomain, "CANNOT_MESSAGE_WHILE_RUNNING");
    }
}
