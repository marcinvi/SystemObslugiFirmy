namespace ReklamacjeAPI.DTOs
{
    public class AdminUserListDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string NazwaWyswietlana { get; set; }
        public string Rola { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminCreateUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string NazwaWyswietlana { get; set; }
        public string Rola { get; set; }
    }

    public class AdminUpdateUserDto
    {
        public string NazwaWyswietlana { get; set; }
        public string Rola { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminResetPasswordDto
    {
        public string NewPassword { get; set; }
    }
}