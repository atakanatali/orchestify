namespace Orchestify.Shared.Errors;

/// <summary>
/// Workspace domain error codes for the Orchestify platform.
/// Covers workspace management, membership, and access control errors.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Workspace domain identifier for error code formatting.
    /// </summary>
    private const string WorkspaceDomain = "WS";

    /// <summary>
    /// Error codes and messages for workspace operations.
    /// </summary>
    public static class Workspaces
    {
        /// <summary>
        /// Workspace not found.
        /// </summary>
        public static string NotFound => FormatErrorCode(WorkspaceDomain, "NOT_FOUND");

        /// <summary>
        /// Workspace name already exists.
        /// </summary>
        public static string NameAlreadyExists => FormatErrorCode(WorkspaceDomain, "NAME_EXISTS");

        /// <summary>
        /// Workspace slug already exists.
        /// </summary>
        public static string SlugAlreadyExists => FormatErrorCode(WorkspaceDomain, "SLUG_EXISTS");

        /// <summary>
        /// Workspace name is invalid.
        /// </summary>
        public static string InvalidName => FormatErrorCode(WorkspaceDomain, "INVALID_NAME");

        /// <summary>
        /// Workspace slug is invalid.
        /// </summary>
        public static string InvalidSlug => FormatErrorCode(WorkspaceDomain, "INVALID_SLUG");

        /// <summary>
        /// User is not a member of the workspace.
        /// </summary>
        public static string NotAMember => FormatErrorCode(WorkspaceDomain, "NOT_MEMBER");

        /// <summary>
        /// User is already a member of the workspace.
        /// </summary>
        public static string AlreadyMember => FormatErrorCode(WorkspaceDomain, "ALREADY_MEMBER");

        /// <summary>
        /// User does not have required permission in workspace.
        /// </summary>
        public static string InsufficientPermissions => FormatErrorCode(WorkspaceDomain, "INSUFFICIENT_PERMISSIONS");

        /// <summary>
        /// Workspace has reached maximum member limit.
        /// </summary>
        public static string MemberLimitExceeded => FormatErrorCode(WorkspaceDomain, "MEMBER_LIMIT_EXCEEDED");

        /// <summary>
        /// Cannot remove the last workspace owner.
        /// </summary>
        public static string CannotRemoveLastOwner => FormatErrorCode(WorkspaceDomain, "CANNOT_REMOVE_LAST_OWNER");

        /// <summary>
        /// Cannot delete workspace with existing boards.
        /// </summary>
        public static string CannotDeleteHasBoards => FormatErrorCode(WorkspaceDomain, "CANNOT_DELETE_HAS_BOARDS");

        /// <summary>
        /// Workspace is archived and read-only.
        /// </summary>
        public static string IsArchived => FormatErrorCode(WorkspaceDomain, "IS_ARCHIVED");

        /// <summary>
        /// Invalid workspace role specified.
        /// </summary>
        public static string InvalidRole => FormatErrorCode(WorkspaceDomain, "INVALID_ROLE");

        /// <summary>
        /// Workspace invitation not found or expired.
        /// </summary>
        public static string InvitationNotFound => FormatErrorCode(WorkspaceDomain, "INVITATION_NOT_FOUND");

        /// <summary>
        /// Workspace invitation already accepted.
        /// </summary>
        public static string InvitationAlreadyAccepted => FormatErrorCode(WorkspaceDomain, "INVITATION_ALREADY_ACCEPTED");
    }
}
