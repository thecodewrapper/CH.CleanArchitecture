using System.Collections.Generic;

namespace CH.CleanArchitecture.Core.Application.DTOs
{
    /// <summary>
    /// Represents a request to add/remove a role to/from a user
    /// </summary>
    public class RoleAssignmentRequestDTO
    {
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}
