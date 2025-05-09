# Tokero-QA-Automated-Testing
Automated testing sample with **Playwright** and **C#** on: https://tokero.dev/en/

This repository contains automated integration tests for the [Tokero staging environment](https://tokero.dev/en/), built using **Playwright with .NET**. The goal is to validate key website flows, including multilingual support, policy page accessibility, and cross-browser compatibility.

## Tech Stack

- **Language**: C#
- **Framework**: Playwright with .NET - NUnit Test Project (.NET Core) (Target framework: .NET 8.0)
- **Browsers Tested**: Chromium, Firefox, WebKit

## Test Coverage

### 1. Policy Page Validation
- Navigates to all policy-related links from the website footer.
- Confirms each page loads correctly (HTTP 200) and contains expected content.

### 2. Language Switching
- Tests the language selector.
- Verifies content updates accordingly for all supported languages (`/en`, `/fr`, `/de`).

### 3. Cross-Browser Testing
- Executes all tests across Chromium, Firefox, and WebKit to ensure consistent behavior.

### 4. Load Performance Measurement
- Measures page load times for each policy page, ensuring they load within acceptable performance thresholds (under 3 seconds).

## Notes

- Authentication flows were skipped due to reCAPTCHA restrictions.
- Performance testing was included, with page load times measured to ensure policy pages load within acceptable thresholds.
- The test suite prioritizes functional coverage, cross-browser compatibility, and multi-language validation over complex architecture or advanced features.

