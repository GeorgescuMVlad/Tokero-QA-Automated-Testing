# Tokero-QA-Automated-Testing
Automated testing sample with **Playwright** and **C#** on: https://tokero.dev/en/

This repository contains automated integration tests for the [Tokero staging environment](https://tokero.dev/en/), built using **Playwright with .NET**. The goal is to validate key website flows, including multilingual support, policy page accessibility, and cross-browser compatibility.

## Tech Stack

- **Language**: C# (.NET 6)
- **Framework**: Playwright
- **Browsers Tested**: Chromium, Firefox, WebKit

## Test Coverage

### 1. Policy Page Validation
- Navigates to all policy-related links from the website footer.
- Confirms each page loads correctly (HTTP 200) and contains expected content.

### 2. Language Switching
- Tests the language selector.
- Verifies content updates accordingly for all supported languages (`/en`, `/ro`, etc.).

### 3. Cross-Browser Testing
- Executes all tests across Chromium, Firefox, and WebKit to ensure consistent behavior.

## Notes

- Authentication flows were skipped due to reCAPTCHA restrictions.
- Performance testing was deprioritized in favor of functional coverage and fast delivery.
- The test suite prioritizes simplicity and clarity over full-blown architecture.

