using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace Reklamacje_Dane
{
    public static class GoogleSheetsDataService
    {
        public static async Task<IList<IList<object>>> GetSheetValuesAsync(string credentialsPath, string spreadsheetId, string range)
        {
            if (string.IsNullOrWhiteSpace(credentialsPath))
                throw new ArgumentException("Path to credentials file is required", nameof(credentialsPath));
            if (string.IsNullOrWhiteSpace(spreadsheetId))
                throw new ArgumentException("Spreadsheet ID is required", nameof(spreadsheetId));
            if (string.IsNullOrWhiteSpace(range))
                throw new ArgumentException("Range is required", nameof(range));

            try
            {
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    var credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
                    using (var service = new SheetsService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential
                    }))
                    {
                        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                        var response = await request.ExecuteAsync().ConfigureAwait(false);
                        return response.Values;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania danych z Google Sheets: {ex.Message}");
                throw;
            }
        }
    }
}
