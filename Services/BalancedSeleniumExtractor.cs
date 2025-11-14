using OpenTextPartnerScraper.Models;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace OpenTextPartnerScraper.Services
{
    public class BalancedSeleniumExtractor
    {
        private readonly ILogger<BalancedSeleniumExtractor> _logger;

        public BalancedSeleniumExtractor(ILogger<BalancedSeleniumExtractor> logger)
        {
            _logger = logger;
        }

        public async Task<List<PartnerSolution>> ExtractAllSolutions()
        {
            var solutions = new List<PartnerSolution>();
            var processedSolutions = new HashSet<string>();
            // Estimate pages - will be adjusted based on actual data found
            var estimatedSolutions = 200; // Conservative estimate
            var solutionsPerPage = 5;
            var estimatedPages = (int)Math.Ceiling((double)estimatedSolutions / solutionsPerPage);
            var failedPages = new List<int>();
            
            _logger.LogInformation($"🚀 Starting BALANCED extraction of solutions...");
            _logger.LogInformation($"⚡ Optimized for speed with better error handling and retry logic");

            // Use a single Chrome instance but with optimized settings
            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            
            var options = new ChromeOptions();
            options.AddArguments("--headless", "--no-sandbox", "--disable-dev-shm-usage", 
                               "--disable-gpu", "--disable-logging", "--disable-extensions",
                               "--disable-background-timer-throttling", "--disable-renderer-backgrounding");
            // Comprehensive SSL and error suppression
            options.AddArguments("--ignore-ssl-errors", "--ignore-certificate-errors", "--ignore-certificate-errors-spki-list",
                               "--ignore-ssl-errors-spki-list", "--disable-web-security", "--allow-running-insecure-content");
            options.AddArguments("--silent", "--log-level=3", "--disable-logging", "--disable-dev-tools");
            options.AddArguments("--disable-background-networking", "--disable-default-apps", "--disable-sync");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            // Suppress Chrome console output completely
            options.AddExcludedArguments("enable-logging");
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-dev-shm-usage");
            
            using var driver = new ChromeDriver(service, options);
            // Increased timeouts to prevent timeout errors during assessment
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
            
            try
            {
                // Process pages until we find all solutions or reach reasonable limit
                for (int pageNum = 0; pageNum < estimatedPages; pageNum++)
                {
                    var startIndex = pageNum * solutionsPerPage;
                    var success = false;
                    var attempts = 0;
                    const int maxAttempts = 3;
                    
                    while (!success && attempts < maxAttempts)
                    {
                        attempts++;
                        try
                        {
                            _logger.LogInformation($"📄 Loading page {pageNum + 1} (start={startIndex}, attempt={attempts})...");
                            
                            var url = $"https://www.opentext.com/products-and-solutions/partners-and-alliances/partner-solutions-catalog?start={startIndex}&max={solutionsPerPage}";
                            driver.Navigate().GoToUrl(url);
                            
                            // Smart wait - check for content periodically
                            var contentFound = await WaitForContent(driver, TimeSpan.FromSeconds(6));
                            if (!contentFound)
                            {
                                _logger.LogWarning($"⚠️ Page {pageNum + 1}: Content not loaded properly, retrying...");
                                await Task.Delay(1000 * attempts); // Exponential backoff
                                continue;
                            }
                            
                            var pageSource = driver.PageSource;
                            var pageSolutions = ExtractSolutionsFromJson(pageSource);
                            
                            if (pageSolutions.Count > 0)
                            {
                                var newSolutions = pageSolutions
                                    .Where(s => !processedSolutions.Contains($"{s.SolutionName}|{s.PartnerName}"))
                                    .Where(s => IsValidSolution(s))
                                    .ToList();
                                
                                if (newSolutions.Count > 0)
                                {
                                    solutions.AddRange(newSolutions);
                                    foreach (var solution in newSolutions)
                                    {
                                        processedSolutions.Add($"{solution.SolutionName}|{solution.PartnerName}");
                                    }
                                    _logger.LogInformation($"✅ Page {pageNum + 1}: Found {newSolutions.Count} new solutions (total: {solutions.Count})");
                                }
                                else
                                {
                                    _logger.LogInformation($"⏭️ Page {pageNum + 1}: All {pageSolutions.Count} solutions were duplicates");
                                }
                                success = true;
                            }
                            else
                            {
                                // Check if this is a valid empty page
                                // Check if we've reached the end of available data
                                if (pageSource.Contains("No results found") || 
                                    pageSource.Contains("no partners found") ||
                                    !pageSource.Contains("partner-card"))
                                {
                                    _logger.LogInformation($"📄 Page {pageNum + 1}: Valid empty page");
                                    success = true;
                                }
                                else
                                {
                                    _logger.LogWarning($"⚠️ Page {pageNum + 1}: No solutions found, retrying...");
                                    await Task.Delay(1500 * attempts);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"❌ Error on page {pageNum + 1}, attempt {attempts}");
                            if (attempts < maxAttempts)
                            {
                                await Task.Delay(2000 * attempts);
                            }
                        }
                    }
                    
                    if (!success)
                    {
                        failedPages.Add(pageNum + 1);
                        _logger.LogError($"💥 Failed to extract page {pageNum + 1} after {maxAttempts} attempts");
                    }
                    
                    // Small delay between pages to be respectful
                    await Task.Delay(200);
                }
                
                _logger.LogInformation($"🎯 Extraction completed! Found {solutions.Count} total solutions");
                var processedPages = estimatedPages - failedPages.Count;
                _logger.LogInformation($"📊 Success rate: {processedPages}/{estimatedPages} pages processed successfully");
                
                if (failedPages.Count > 0)
                {
                    _logger.LogWarning($"⚠️ Failed pages: {string.Join(", ", failedPages)}");
                    
                    // One final retry for failed pages
                    _logger.LogInformation($"🔄 Final retry for {failedPages.Count} failed pages...");
                    foreach (var failedPage in failedPages.ToList())
                    {
                        try
                        {
                            var startIndex = (failedPage - 1) * solutionsPerPage;
                            _logger.LogInformation($"🔄 Final retry for page {failedPage} (start={startIndex})...");
                            
                            var url = $"https://www.opentext.com/products-and-solutions/partners-and-alliances/partner-solutions-catalog?start={startIndex}&max={solutionsPerPage}";
                            driver.Navigate().GoToUrl(url);
                            await Task.Delay(5000); // Longer wait for final retry
                            
                            var pageSource = driver.PageSource;
                            var pageSolutions = ExtractSolutionsFromJson(pageSource);
                            
                            if (pageSolutions.Count > 0)
                            {
                                var newSolutions = pageSolutions
                                    .Where(s => !processedSolutions.Contains($"{s.SolutionName}|{s.PartnerName}"))
                                    .Where(s => IsValidSolution(s))
                                    .ToList();
                                
                                if (newSolutions.Count > 0)
                                {
                                    solutions.AddRange(newSolutions);
                                    failedPages.Remove(failedPage);
                                    _logger.LogInformation($"✅ Retry success! Page {failedPage}: Found {newSolutions.Count} solutions");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"❌ Final retry failed for page {failedPage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Critical error during extraction");
            }
            finally
            {
                driver.Quit();
            }
            
            return solutions.OrderBy(s => s.SolutionName).ToList();
        }

        private async Task<bool> WaitForContent(IWebDriver driver, TimeSpan timeout)
        {
            var endTime = DateTime.Now.Add(timeout);
            while (DateTime.Now < endTime)
            {
                try
                {
                    var pageSource = driver.PageSource;
                    if (pageSource.Contains("jsonResponse") && pageSource.Contains("total"))
                    {
                        return true; // Content found
                    }
                }
                catch
                {
                    // Continue waiting
                }
                
                await Task.Delay(300);
            }
            return false;
        }

        private List<PartnerSolution> ExtractSolutionsFromJson(string pageSource)
        {
            var solutions = new List<PartnerSolution>();
            
            try
            {
                // Find JSON response in the page source
                var jsonPattern = @"var jsonResponse\s*=\s*JSON\.stringify\((\{.*?\})\);";
                var match = Regex.Match(pageSource, jsonPattern, RegexOptions.Singleline);
                
                if (match.Success)
                {
                    var jsonString = match.Groups[1].Value;
                    
                    using var document = JsonDocument.Parse(jsonString);
                    var root = document.RootElement;
                    
                    if (root.TryGetProperty("results", out var results) &&
                        results.TryGetProperty("assets", out var assets))
                    {
                        foreach (var asset in assets.EnumerateArray())
                        {
                            if (asset.TryGetProperty("metadata", out var metadata))
                            {
                                string solutionName = "";
                                string partnerName = "";
                                
                                if (metadata.TryGetProperty("TeamSite/Metadata/SolutionName", out var solutionNameElement))
                                {
                                    solutionName = solutionNameElement.GetString() ?? "";
                                }
                                
                                if (metadata.TryGetProperty("TeamSite/Metadata/SolutionPartnerName", out var partnerNameElement))
                                {
                                    partnerName = partnerNameElement.GetString() ?? "";
                                }
                                
                                if (!string.IsNullOrWhiteSpace(solutionName) && !string.IsNullOrWhiteSpace(partnerName))
                                {
                                    solutions.Add(new PartnerSolution
                                    {
                                        SolutionName = solutionName.Trim(),
                                        PartnerName = partnerName.Trim()
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Error extracting solutions from JSON");
            }
            
            return solutions;
        }

        private bool IsValidSolutionName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2)
                return false;
                
            var invalidTerms = new[] { "null", "undefined", "metadata", "teamsite" };
            return !invalidTerms.Any(term => name.ToLower().Contains(term));
        }

        private bool IsValidPartnerName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2)
                return false;
                
            var invalidTerms = new[] { "null", "undefined", "metadata", "teamsite" };
            return !invalidTerms.Any(term => name.ToLower().Contains(term));
        }
        
        private bool IsValidSolution(PartnerSolution solution)
        {
            return IsValidSolutionName(solution.SolutionName) && 
                   IsValidPartnerName(solution.PartnerName) &&
                   solution.SolutionName != solution.PartnerName;
        }
    }
}