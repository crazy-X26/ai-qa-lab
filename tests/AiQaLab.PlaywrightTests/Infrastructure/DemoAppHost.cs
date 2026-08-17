using AiQaLab.DemoApp;
using Microsoft.AspNetCore.Builder;

namespace AiQaLab.PlaywrightTests.Infrastructure
{
    public class DemoAppHost : IAsyncDisposable
    {
        private WebApplication? _app;

        // BaseUrl contains a trailing slash.
        public Uri BaseUrl { get; private set; } = null!;

        public async Task StartHostAsync()
        {
            _app = Program.CreateApp([]);
            await _app.StartAsync();

            var address = _app.Urls.FirstOrDefault();

            if (address is null)
            {
                throw new InvalidOperationException("Could not determine the DemoApp URL.");
            }

            BaseUrl = new Uri(address);
        }

        public async ValueTask DisposeAsync()
        {
            if(_app != null)
            {
                await _app.StopAsync();
                await _app.DisposeAsync();
            }
        }
    }
}
