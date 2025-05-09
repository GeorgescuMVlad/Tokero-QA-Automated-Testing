using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
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

            // Grab hrefs safely before navigating
            var hrefs = await _page.EvalOnSelectorAllAsync<string[]>(
                "a[href*='/policies/']",
                "els => els.map(el => el.href).filter(href => !href.endsWith('/policies/'))"
            );

            foreach (var href in hrefs)
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await _page.GotoAsync(href);
                stopwatch.Stop();

                Assert.That(response.Status, Is.EqualTo(200), $"Page {href} did not load successfully.");
                Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(3000), $"Page {href} loaded too slowly ({stopwatch.ElapsedMilliseconds}ms).");
            }
        }

        [Test]
        public async Task ValidatePolicyPages()
        {
            await _page.GotoAsync("https://tokero.dev/en/");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Attempt to remove the modal overlay (with retry)
            for (int i = 0; i < 3; i++)
            {
                await _page.EvaluateAsync(@"document.querySelector('div.modal_modalOverlay__pCvXk')?.remove()");
                await _page.WaitForTimeoutAsync(500);
                if (!await _page.IsVisibleAsync("div.modal_modalOverlay__pCvXk")) break;
            }

            // Accept cookie popup if it appears
            if (await _page.Locator("div.cookieConsentPopup_container__3").IsVisibleAsync())
            {
                await _page.ClickAsync("button:has-text('Accept')");
                await _page.WaitForTimeoutAsync(500);
            }

            // Scroll and click on the Privacy Policy link
            var privacyLink = _page.Locator("a[title='Privacy policy']");
            await privacyLink.ScrollIntoViewIfNeededAsync();
            await privacyLink.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Gather unique policy links
            var links = await _page.QuerySelectorAllAsync("footer a[href*='/policies/']");
            var hrefs = new List<string>();
            var seen = new HashSet<string>();

            foreach (var link in links)
            {
                var href = await link.GetAttributeAsync("href");
                if (!string.IsNullOrEmpty(href) && !href.EndsWith("/policies/"))
                {
                    var fullUrl = href.StartsWith("http") ? href : $"https://tokero.dev{href}";
                    if (seen.Add(fullUrl)) hrefs.Add(fullUrl);
                }
            }

            // Visit and verify each policy page
            foreach (var url in hrefs)
            {
                var response = await _page.GotoAsync(url);
                Assert.That(response.Ok, $"Policy page failed to load: {url}");

                await _page.WaitForTimeoutAsync(1000); // let dynamic content load

                var contentFound = await _page.Locator("main, article, .policy-container, .htmlComponent_bodyClass__r").IsVisibleAsync();
                var fallbackBodyLength = (await _page.InnerTextAsync("body")).Trim().Length;

                Assert.IsTrue(contentFound || fallbackBodyLength > 300, $"Policy page {url} seems empty or broken.");
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
                await _context.CloseAsync(); // Cleanup before browser switch
                _browser = await GetBrowserInstance(browser);
                _context = await _browser.NewContextAsync();
                _page = await _context.NewPageAsync();

                await ValidatePolicyPages();
            }
        }        
    }
}
