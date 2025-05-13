using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tokero_QA_Automated_Testing
{
    public class TestBase
    {
        protected IPlaywright _playwright;
        protected IBrowser _browser;
        protected IPage _page;
        protected IBrowserContext _context;

        [SetUp]
        public async Task SetupAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await PlaywrightConfig.GetBrowserAsync(_playwright);
            _context = await _browser.NewContextAsync();
            _page = await _browser.NewPageAsync();        
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            if (_page != null) await _page.CloseAsync();
            if (_context != null) await _context.CloseAsync();
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        protected async Task<IBrowser> GetBrowserInstance(string browserType)
        {
            return browserType switch
            {
                "chromium" => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }),
                "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }),
                "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }),
                _ => throw new ArgumentException("Invalid browser type")
            };
        }
    }
}