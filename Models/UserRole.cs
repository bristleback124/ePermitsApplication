using System;
using System.Collections.Generic;
using ePermitsApp.Entities;

namespace ePermits.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string UserRoleDesc { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<User>? Users { get; set; }
    }
}