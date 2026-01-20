using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    public class QrPairingServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _isDisposed;

        public string Token { get; }
        public string ListenPrefix { get; }

        public event Action<QrPairingRequest> PairingReceived;

        public QrPairingServer(string ipAddress, int port)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentException("Brak adresu IP do nasłuchu.", nameof(ipAddress));
            }

            Token = Guid.NewGuid().ToString("N");
            ListenPrefix = $"http://{ipAddress}:{port}/";
            _listener = new HttpListener();
            _listener.Prefixes.Add(ListenPrefix);
        }

        public void Start()
        {
            _listener.Start();
            _ = Task.Run(() => ListenAsync(_cts.Token));
        }

        private async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context = null;
                try
                {
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch when (token.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                    continue;
                }

                if (context == null)
                {
                    continue;
                }

                _ = Task.Run(() => HandleRequest(context), token);
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "POST" || context.Request.Url?.AbsolutePath != "/pair")
            {
                WriteResponse(context, HttpStatusCode.NotFound, "Not found");
                return;
            }

            try
            {
                string body;
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    body = reader.ReadToEnd();
                }

                var payload = JsonConvert.DeserializeObject<QrPairingRequest>(body);

                if (payload == null || string.IsNullOrWhiteSpace(payload.Token) || payload.Token != Token)
                {
                    WriteResponse(context, HttpStatusCode.Forbidden, "Niepoprawny token parowania");
                    return;
                }

                if (string.IsNullOrWhiteSpace(payload.PhoneIp) || string.IsNullOrWhiteSpace(payload.PairingCode))
                {
                    WriteResponse(context, HttpStatusCode.BadRequest, "Brak danych parowania");
                    return;
                }

                WriteResponse(context, HttpStatusCode.OK, "OK");
                PairingReceived?.Invoke(payload);
            }
            catch (Exception ex)
            {
                WriteResponse(context, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private static void WriteResponse(HttpListenerContext context, HttpStatusCode statusCode, string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message ?? string.Empty);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                // Ignoruj błąd zapisu odpowiedzi.
            }
            finally
            {
                try
                {
                    context.Response.OutputStream.Close();
                }
                catch
                {
                    // Ignore dispose errors.
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _cts.Cancel();
            try
            {
                if (_listener.IsListening)
                {
                    _listener.Stop();
                }
            }
            catch
            {
                // Ignore shutdown errors.
            }
            finally
            {
                _listener.Close();
                _cts.Dispose();
            }
        }
    }
}
