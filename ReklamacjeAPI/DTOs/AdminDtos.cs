using System.Collections.Generic;

﻿namespace ReklamacjeAPI.DTOs
{
    public class AdminUserListDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string NazwaWyswietlana { get; set; }
        public string Rola { get; set; }
        public bool IsActive { get; set; }
        public List<int> ModuleIds { get; set; } = new();
    }

    public class AdminModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AdminCreateUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string NazwaWyswietlana { get; set; }
        public string Rola { get; set; }
        public List<int>? ModuleIds { get; set; }
    }

    public class AdminUpdateUserDto
    {
        public string NazwaWyswietlana { get; set; }
        public string Rola { get; set; }
        public bool IsActive { get; set; }
        public List<int>? ModuleIds { get; set; }
    }

    public class AdminResetPasswordDto
    {
        public string NewPassword { get; set; }
    }
}
