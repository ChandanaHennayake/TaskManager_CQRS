using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Features.Auth.Queries.Login
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
