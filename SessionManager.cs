namespace Reklamacje_Dane
{
    public static class SessionManager
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserLogin { get; set; }

        // Ta metoda jest potrzebna, aby zapisać dane po zalogowaniu
        // Twój formularz LoginForm.cs wywołuje ją po pomyślnej weryfikacji hasła.
        public static void Login(int userId, string userLogin)
        {
            CurrentUserId = userId;
            CurrentUserLogin = userLogin;
        }
    }
}