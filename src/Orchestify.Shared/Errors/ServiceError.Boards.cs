namespace Orchestify.Shared.Errors;

/// <summary>
/// Board domain error codes for the Orchestify platform.
/// Covers board management, columns, and board-specific operations.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Board domain identifier for error code formatting.
    /// </summary>
    private const string BoardDomain = "BOARD";

    /// <summary>
    /// Error codes and messages for board operations.
    /// </summary>
    public static class Boards
    {
        /// <summary>
        /// Board not found.
        /// </summary>
        public static string NotFound => FormatErrorCode(BoardDomain, "NOT_FOUND");

        /// <summary>
        /// Board name already exists in workspace.
        /// </summary>
        public static string NameAlreadyExists => FormatErrorCode(BoardDomain, "NAME_EXISTS");

        /// <summary>
        /// Board name is invalid.
        /// </summary>
        public static string InvalidName => FormatErrorCode(BoardDomain, "INVALID_NAME");

        /// <summary>
        /// Board description is invalid.
        /// </summary>
        public static string InvalidDescription => FormatErrorCode(BoardDomain, "INVALID_DESCRIPTION");

        /// <summary>
        /// Invalid board type specified.
        /// </summary>
        public static string InvalidType => FormatErrorCode(BoardDomain, "INVALID_TYPE");

        /// <summary>
        /// Board has reached maximum column limit.
        /// </summary>
        public static string ColumnLimitExceeded => FormatErrorCode(BoardDomain, "COLUMN_LIMIT_EXCEEDED");

        /// <summary>
        /// Column not found in board.
        /// </summary>
        public static string ColumnNotFound => FormatErrorCode(BoardDomain, "COLUMN_NOT_FOUND");

        /// <summary>
        /// Column name already exists in board.
        /// </summary>
        public static string ColumnNameExists => FormatErrorCode(BoardDomain, "COLUMN_NAME_EXISTS");

        /// <summary>
        /// Invalid column position specified.
        /// </summary>
        public static string InvalidColumnPosition => FormatErrorCode(BoardDomain, "INVALID_COLUMN_POSITION");

        /// <summary>
        /// Cannot delete board with existing tasks.
        /// </summary>
        public static string CannotDeleteHasTasks => FormatErrorCode(BoardDomain, "CANNOT_DELETE_HAS_TASKS");

        /// <summary>
        /// Cannot delete column with existing tasks.
        /// </summary>
        public static string CannotDeleteColumnHasTasks => FormatErrorCode(BoardDomain, "CANNOT_DELETE_COLUMN_HAS_TASKS");

        /// <summary>
        /// Board is archived and read-only.
        /// </summary>
        public static string IsArchived => FormatErrorCode(BoardDomain, "IS_ARCHIVED");

        /// <summary>
        /// Task not found in board.
        /// </summary>
        public static string TaskNotFound => FormatErrorCode(BoardDomain, "TASK_NOT_FOUND");

        /// <summary>
        /// Task does not belong to specified board.
        /// </summary>
        public static string TaskNotInBoard => FormatErrorCode(BoardDomain, "TASK_NOT_IN_BOARD");

        /// <summary>
        /// Invalid task position in column.
        /// </summary>
        public static string InvalidTaskPosition => FormatErrorCode(BoardDomain, "INVALID_TASK_POSITION");

        /// <summary>
        /// Cannot move task to same column.
        /// </summary>
        public static string CannotMoveToSameColumn => FormatErrorCode(BoardDomain, "CANNOT_MOVE_TO_SAME_COLUMN");

        /// <summary>
        /// Board template not found.
        /// </summary>
        public static string TemplateNotFound => FormatErrorCode(BoardDomain, "TEMPLATE_NOT_FOUND");
    }
}
