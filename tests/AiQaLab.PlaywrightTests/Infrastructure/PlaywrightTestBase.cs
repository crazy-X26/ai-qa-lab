using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AiQaLab.PlaywrightTests.Infrastructure
{
    public abstract class PlaywrightTestBase : IAsyncLifetime
    {
        protected IPlaywright PlayWright { get; private set; } = null!;
        protected IBrowser Browser { get; private set; } = null!;
        protected IBrowserContext BrowserContext { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;
        protected DemoAppHost DemoApp { get; private set; } = null!;
        protected Uri BaseUrl { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            DemoApp = new DemoAppHost();
            await DemoApp.StartHostAsync();

            BaseUrl = DemoApp.BaseUrl;

            PlayWright = await Playwright.CreateAsync();
            
            Browser = await PlayWright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            BrowserContext = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true
            });

            Page = await BrowserContext.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await BrowserContext.CloseAsync();
            await Browser.CloseAsync();

            PlayWright.Dispose();

            await DemoApp.DisposeAsync();
        }
    }
}
