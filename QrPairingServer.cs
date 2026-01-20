using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    public class QrPairingServer : IDisposable
    {
        private readonly TcpListener _listener;
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

            if (port <= 0 || port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(port), "Niepoprawny port nasłuchu.");
            }

            if (!IPAddress.TryParse(ipAddress, out var parsedAddress))
            {
                throw new ArgumentException("Niepoprawny adres IP do nasłuchu.", nameof(ipAddress));
            }

            Token = Guid.NewGuid().ToString("N");
            ListenPrefix = $"http://{ipAddress}:{port}/";
            _listener = new TcpListener(parsedAddress, port);
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
                TcpClient client = null;
                try
                {
                    client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException)
                {
                    continue;
                }

                if (client == null)
                {
                    continue;
                }

                _ = Task.Run(() => HandleClientAsync(client), token);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, leaveOpen: true))
            {
                string requestLine = await reader.ReadLineAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(requestLine))
                {
                    return;
                }

                string[] requestParts = requestLine.Split(' ');
                if (requestParts.Length < 2)
                {
                    await WriteResponseAsync(stream, HttpStatusCode.BadRequest, "Bad request").ConfigureAwait(false);
                    return;
                }

                string method = requestParts[0];
                string path = requestParts[1];

                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string line;
                while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync().ConfigureAwait(false)))
                {
                    int separatorIndex = line.IndexOf(':');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }
                    string name = line.Substring(0, separatorIndex).Trim();
                    string value = line.Substring(separatorIndex + 1).Trim();
                    headers[name] = value;
                }

                if (!string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase) || path != "/pair")
                {
                    await WriteResponseAsync(stream, HttpStatusCode.NotFound, "Not found").ConfigureAwait(false);
                    return;
                }

                int contentLength = 0;
                if (headers.TryGetValue("Content-Length", out var lengthValue))
                {
                    int.TryParse(lengthValue, out contentLength);
                }

                if (contentLength <= 0)
                {
                    await WriteResponseAsync(stream, HttpStatusCode.BadRequest, "Brak danych parowania").ConfigureAwait(false);
                    return;
                }

                char[] buffer = new char[contentLength];
                int read = 0;
                while (read < contentLength)
                {
                    int chunk = await reader.ReadAsync(buffer, read, contentLength - read).ConfigureAwait(false);
                    if (chunk <= 0)
                    {
                        break;
                    }
                    read += chunk;
                }

                string body = new string(buffer, 0, read);

                try
                {
                    var payload = JsonConvert.DeserializeObject<QrPairingRequest>(body);

                    if (payload == null || string.IsNullOrWhiteSpace(payload.Token) || payload.Token != Token)
                    {
                        await WriteResponseAsync(stream, HttpStatusCode.Forbidden, "Niepoprawny token parowania").ConfigureAwait(false);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(payload.PhoneIp) || string.IsNullOrWhiteSpace(payload.PairingCode))
                    {
                        await WriteResponseAsync(stream, HttpStatusCode.BadRequest, "Brak danych parowania").ConfigureAwait(false);
                        return;
                    }

                    await WriteResponseAsync(stream, HttpStatusCode.OK, "OK").ConfigureAwait(false);
                    PairingReceived?.Invoke(payload);
                }
                catch (Exception ex)
                {
                    await WriteResponseAsync(stream, HttpStatusCode.InternalServerError, ex.Message).ConfigureAwait(false);
                }
            }
        }

        private static async Task WriteResponseAsync(NetworkStream stream, HttpStatusCode statusCode, string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message ?? string.Empty);
                string header = $"HTTP/1.1 {(int)statusCode} {statusCode}\r\n" +
                                "Content-Type: text/plain; charset=utf-8\r\n" +
                                $"Content-Length: {buffer.Length}\r\n" +
                                "Connection: close\r\n\r\n";
                byte[] headerBytes = Encoding.ASCII.GetBytes(header);
                await stream.WriteAsync(headerBytes, 0, headerBytes.Length).ConfigureAwait(false);
                await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            }
            catch
            {
                // Ignoruj błąd zapisu odpowiedzi.
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
                _listener.Stop();
            }
            catch
            {
                // Ignore shutdown errors.
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}
