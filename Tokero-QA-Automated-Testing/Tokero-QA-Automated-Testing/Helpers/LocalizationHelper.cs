using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Tokero_QA_Automated_Testing.Helpers
{
    public static class LocalizationHelper
    {
        public static async Task SwitchLanguageAsync(IPage page, string languageCode)
        {
            // Only switch if we're not already in the target language
            var currentUrl = page.Url;
            if (currentUrl.Contains($"/{languageCode}/"))
            {
                return; 
            }

            // Open the language dropdown
            await page.ClickAsync("div[class~='dropdown']");
            await page.WaitForTimeoutAsync(500); // Let dropdown render

            // Click the appropriate language option
            await page.ClickAsync($"a[href='/{languageCode}/']"); // Real links like /en/, /fr/, etc.

            // Wait for navigation to complete
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}
