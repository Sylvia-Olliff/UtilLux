using Microsoft.Extensions.Logging;
using System.IO.Pipes;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace UtilLux.Core.Infrastructure;

internal sealed class NamedPipeService(ILogger<NamedPipeService> logger) : Disposable, INamedPipeService
{
    private readonly Subject<string> receivedString = new();

    public IObservable<string> ReceivedString =>
        this.receivedString.AsObservable();

    public void StartServer(string pipeName) =>
        Task.Run(() => this.WaitForMessages(pipeName));

    public bool Write(string pipeName, string text, int connectTimeout = 300)
    {
        using var client = new NamedPipeClientStream(pipeName);

        try
        {
            client.Connect(connectTimeout);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Named pipe error");
            return false;
        }

        if (!client.IsConnected)
        {
            logger.LogError("The client is not connected");
            return false;
        }

        using var writer = new StreamWriter(client);
        writer.Write(text);
        writer.Flush();

        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.receivedString.Dispose();
        }
    }

    private void WaitForMessages(string pipeName)
    {
        using var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);

        while (!this.Disposed)
        {
            server.WaitForConnection();

            using var reader = new StreamReader(server);
            var message = reader.ReadToEnd();

            this.receivedString.OnNext(message);
        }
    }
}
