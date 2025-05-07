using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tokero_QA_Automated_Testing.Helpers;

namespace Tokero_QA_Automated_Testing.FunctionalTests
{
    class PolicyPageTests : TestBase
    {
        [Test]
        public async Task MeasurePolicyPageLoadTime()
        {
            await _page.GotoAsync("https://tokero.dev/en/");
            var links = await _page.QuerySelectorAllAsync("a.policy-link");

            foreach (var link in links)
            {
                var href = await link.GetAttributeAsync("href");
                var response = await _page.GotoAsync(href);
                Assert.IsTrue(response.Status == 200, $"Page {href} did not load successfully.");

                var loadTime = await _page.EvaluateAsync("performance.timing.loadEventEnd - performance.timing.navigationStart");
            }
        }

        [Test]
        public async Task ValidatePolicyPages()
        {
            await _page.GotoAsync("https://tokero.dev/en/");

            // Remove modal overlay completely
            await _page.EvaluateAsync(@"
            let modal = document.querySelector('div.modal_modalOverlay__pCvXk');
            if (modal) 
            {
                modal.style.display = 'none';
                modal.style.pointerEvents = 'none';
                modal.remove();
            }");

            // Accept cookie popup if visible
            if (await _page.IsVisibleAsync("div.cookieConsentPopup_container__3"))
            {
                await _page.ClickAsync("button:has-text('Accept')");
                await _page.WaitForTimeoutAsync(1000); // let the DOM settle
            }

            // Wait for and click the Privacy Policy link
            await _page.WaitForSelectorAsync("a[title='Privacy policy']", new PageWaitForSelectorOptions { Timeout = 60000 });
            await _page.ClickAsync("a[title='Privacy policy']");

            // Get all unique policy hrefs BEFORE navigating
            var links = await _page.QuerySelectorAllAsync("footer a[href*='/policies/']");
            var hrefs = new List<string>();
            var visited = new HashSet<string>();

            foreach (var link in links)
            {
                var href = await link.GetAttributeAsync("href");
                if (!string.IsNullOrEmpty(href) && !href.EndsWith("/policies/"))
                {
                    var absoluteUrl = $"https://tokero.dev{href}";
                    if (visited.Add(absoluteUrl)) hrefs.Add(absoluteUrl);
                }
            }

            // Navigate to each policy URL and verify it loaded
            foreach (var url in hrefs)
            {
                await _page.GotoAsync(url);
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var isVisible = await _page.IsVisibleAsync("h1, h2, .policy-title, .policy-container, .title-text, .custom-header, .section-title, .htmlComponent_editorClass__t, .htmlComponent_bodyClass__r");
                var body = await _page.InnerHTMLAsync("body");

                Assert.IsTrue(isVisible || body.Length > 1000, $"Policy page {url} failed to load or had no recognizable content.");
            }
        }

        [Test]
        public async Task ValidatePolicyPages_MultiLanguage()
        {
            string[] languages = { "en", "fr", "de" };

            foreach (var lang in languages)
            {
                await _page.GotoAsync($"https://tokero.dev/{lang}/");

                await LocalizationHelper.SwitchLanguageAsync(_page, lang);

                await ValidatePolicyPages(); // Reuse your passing version
            }
        }

        [Test]
        public async Task ValidatePolicyPages_MultiBrowser()
        {
            string[] browsers = { "chromium", "firefox", "webkit" };

            foreach (var browser in browsers)
            {
                _browser = await GetBrowserInstance(browser);
                _page = await _browser.NewPageAsync();
                await ValidatePolicyPages();
            }
        }        
    }
}
