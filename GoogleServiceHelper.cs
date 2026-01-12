using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.IO;

public static class GoogleServiceHelper
{
    private static readonly string ApplicationName = "TwojaAplikacja"; // Nazwa aplikacji
    private static readonly string SheetsCredentialPath = "credentials_sheets.json"; // Ścieżka do pliku JSON dla Google Sheets
    private static readonly string DriveCredentialPath = "credentials_drive.json"; // Ścieżka do pliku JSON dla Google Drive

    private static SheetsService _sheetsService;
    private static DriveService _driveService;

    /// <summary>
    /// Inicjalizuje usługę Google Sheets
    /// </summary>
    /// <returns>Instancja SheetsService</returns>
    public static SheetsService GetSheetsService()
    {
        if (_sheetsService == null)
        {
            try
            {
                // Wczytanie pliku JSON z poświadczeniami dla Google Sheets
                using (var stream = new FileStream(SheetsCredentialPath, FileMode.Open, FileAccess.Read))
                {
                    var credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);

                    // Tworzenie instancji SheetsService
                    _sheetsService = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = ApplicationName,
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas inicjalizacji Google Sheets API: {ex.Message}");
                throw;
            }
        }

        return _sheetsService;
    }

    /// <summary>
    /// Inicjalizuje usługę Google Drive
    /// </summary>
    /// <returns>Instancja DriveService</returns>
    public static DriveService GetDriveService()
    {
        if (_driveService == null)
        {
            try
            {
                // Wczytanie pliku JSON z poświadczeniami dla Google Drive
                using (var stream = new FileStream(DriveCredentialPath, FileMode.Open, FileAccess.Read))
                {
                    var credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(DriveService.Scope.Drive);

                    // Tworzenie instancji DriveService
                    _driveService = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = ApplicationName,
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas inicjalizacji Google Drive API: {ex.Message}");
                throw;
            }
        }

        return _driveService;
    }
}
