﻿namespace AuthService
{
    public class UserRegisterDto
    {
        public string? Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
