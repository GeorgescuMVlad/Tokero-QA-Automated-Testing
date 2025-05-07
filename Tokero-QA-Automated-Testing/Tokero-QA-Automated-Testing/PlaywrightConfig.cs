using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Tokero_QA_Automated_Testing
{
    public static class PlaywrightConfig
    {
        public static async Task<IBrowser> GetBrowserAsync(IPlaywright playwright)
        {
            return await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }
    }
}
