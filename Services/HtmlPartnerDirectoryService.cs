using Microsoft.Extensions.Logging;
using OpenTextPartnerScraper.Models;
using System.Text;
using System.Text.Json;

namespace OpenTextPartnerScraper.Services
{
    public class HtmlPartnerDirectoryService
    {
        private readonly ILogger<HtmlPartnerDirectoryService> _logger;

        public HtmlPartnerDirectoryService(ILogger<HtmlPartnerDirectoryService> logger)
        {
            _logger = logger;
        }

        public async Task GeneratePartnerDirectoryPage()
        {
            try
            {
                _logger.LogInformation("🎨 Starting Partner Directory HTML generation...");

                // Load partners with solutions data
                var partnersData = await LoadPartnersWithSolutions();
                
                if (!partnersData.Any())
                {
                    _logger.LogWarning("No partner data found to generate directory");
                    Console.WriteLine("❌ No partner data found to generate directory page");
                    return;
                }

                // Generate HTML content
                var htmlContent = GenerateHtmlContent(partnersData);

                // Save to output directory
                var outputDir = "output";
                Directory.CreateDirectory(outputDir);
                var htmlPath = Path.Combine(outputDir, "partner_directory.html");
                
                await File.WriteAllTextAsync(htmlPath, htmlContent, Encoding.UTF8);

                _logger.LogInformation($"✅ Partner Directory page generated successfully");
                Console.WriteLine($"🎯 Partner Directory page generated successfully!");
                Console.WriteLine($"💾 HTML file saved to:");
                Console.WriteLine($"   📄 HTML: {htmlPath}");
                Console.WriteLine($"📊 Directory Summary:");
                Console.WriteLine($"   👥 Total partners: {partnersData.Count}");
                Console.WriteLine($"   🔗 Partners with solutions: {partnersData.Count(p => p.SolutionCount > 0)}");
                Console.WriteLine($"   💡 Total solutions: {partnersData.Sum(p => p.SolutionCount)}");
                
                // Open HTML file in default browser
                try
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("🌐 Opening Partner Directory in your default browser...");
                    Console.ResetColor();
                    
                    var absolutePath = Path.GetFullPath(htmlPath);
                    var startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = absolutePath,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(startInfo);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Browser opened successfully!");
                    Console.ResetColor();
                }
                catch (Exception browserEx)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️  Could not auto-open browser: {browserEx.Message}");
                    Console.WriteLine($"📂 Please manually open: {Path.GetFullPath(htmlPath)}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Partner Directory HTML page");
                Console.WriteLine($"❌ Error generating Partner Directory: {ex.Message}");
            }
        }

        private async Task<List<PartnerWithSolutions>> LoadPartnersWithSolutions()
        {
            try
            {
                // Try main output file first
                var primaryPath = Path.Combine("output", "partners_with_solutions.json");
                var samplePath = Path.Combine("output", "sample", "partners_with_solutions.json");

                string jsonContent = null;
                bool usingSampleData = false;
                
                if (File.Exists(primaryPath))
                {
                    _logger.LogInformation($"📁 Loading partners data from: {primaryPath}");
                    Console.WriteLine($"📁 Loading partners data from: partners_with_solutions.json");
                    
                    var content = await File.ReadAllTextAsync(primaryPath);
                    
                    // Check if the content is valid and not empty
                    if (!string.IsNullOrWhiteSpace(content) && content.Trim() != "[]")
                    {
                        var testData = JsonSerializer.Deserialize<List<PartnerWithSolutions>>(content);
                        if (testData != null && testData.Count > 0)
                        {
                            jsonContent = content;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("⚠️  Main partners_with_solutions.json file is empty or contains no valid data");
                            Console.WriteLine("📄 Falling back to sample data...");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️  Main partners_with_solutions.json file is empty");
                        Console.WriteLine("📄 Falling back to sample data...");
                        Console.ResetColor();
                    }
                }
                
                // If main file doesn't work, try sample
                if (jsonContent == null && File.Exists(samplePath))
                {
                    _logger.LogInformation($"📁 Loading partners data from sample: {samplePath}");
                    Console.WriteLine($"📁 Loading partners data from sample: sample\\partners_with_solutions.json");
                    jsonContent = await File.ReadAllTextAsync(samplePath);
                    usingSampleData = true;
                }
                
                if (jsonContent == null)
                {
                    _logger.LogError("No valid partners_with_solutions.json file found");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ No valid partners_with_solutions.json file found!");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🔄 Please run Choice 3 (OpenText Scraper) first to generate the data");
                    Console.ResetColor();
                    return new List<PartnerWithSolutions>();
                }

                var jsonOptions = new JsonSerializerOptions 
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var partners = JsonSerializer.Deserialize<List<PartnerWithSolutions>>(jsonContent, jsonOptions) ?? new List<PartnerWithSolutions>();
                _logger.LogInformation($"📊 Loaded {partners.Count} partners from JSON file");
                
                // Show user feedback about data source
                if (usingSampleData)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("ℹ️  Note: Using sample data. For fresh data, run Choice 3 (OpenText Scraper)");
                    Console.ResetColor();
                }
                
                return partners;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading partners with solutions data");
                return new List<PartnerWithSolutions>();
            }
        }

        private string GenerateHtmlContent(List<PartnerWithSolutions> partnersData)
        {
            var html = new StringBuilder();

            // Generate complete HTML document with OpenText styling
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>OpenText Partner Directory</title>");
            
            // Add OpenText-style CSS
            html.AppendLine("    <style>");
            html.AppendLine(GenerateOpenTextStyles());
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header section
            html.AppendLine("    <header class=\"ot-header\">");
            html.AppendLine("        <div class=\"ot-container\">");
            html.AppendLine("            <h1 class=\"ot-logo\">OpenText Partner Directory</h1>");
            html.AppendLine("            <p class=\"ot-subtitle\">Discover our global network of trusted partners and their innovative solutions</p>");
            html.AppendLine("        </div>");
            html.AppendLine("    </header>");

            // Filters section
            html.AppendLine("    <section class=\"ot-filters\">");
            html.AppendLine("        <div class=\"ot-container\">");
            html.AppendLine("            <h2>Filter Partners</h2>");
            html.AppendLine("            <div class=\"ot-filter-controls\">");
            html.AppendLine("                <input type=\"text\" id=\"searchInput\" placeholder=\"Search partners...\" class=\"ot-search-input\">");
            html.AppendLine("                <select id=\"solutionFilter\" class=\"ot-select\">");
            html.AppendLine("                    <option value=\"all\">All Partners</option>");
            html.AppendLine("                    <option value=\"with-solutions\">Listed Solutions</option>");
            html.AppendLine("                </select>");
            html.AppendLine("                <button onclick=\"clearFilters()\" class=\"ot-btn ot-btn-secondary\">Clear Filters</button>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class=\"ot-stats\">");
            html.AppendLine($"                <span class=\"ot-stat\">Total Partners: <strong>{partnersData.Count}</strong></span>");
            html.AppendLine($"                <span class=\"ot-stat\">Partners with Solutions: <strong>{partnersData.Count(p => p.SolutionCount > 0)}</strong></span>");
            html.AppendLine($"                <span class=\"ot-stat\">Total Solutions: <strong>{partnersData.Sum(p => p.SolutionCount)}</strong></span>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </section>");

            // Partners section
            html.AppendLine("    <main class=\"ot-main\">");
            html.AppendLine("        <div class=\"ot-container\">");
            html.AppendLine("            <div class=\"ot-partners-grid\" id=\"partnersGrid\">");

            // Generate partner cards
            foreach (var partner in partnersData.OrderByDescending(p => p.SolutionCount).ThenBy(p => p.PartnerName))
            {
                html.AppendLine(GeneratePartnerCard(partner));
            }

            html.AppendLine("            </div>");
            html.AppendLine("            <div id=\"noResults\" class=\"ot-no-results\" style=\"display: none;\">");
            html.AppendLine("                <p>No partners found matching your criteria.</p>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </main>");

            // Footer
            html.AppendLine("    <footer class=\"ot-footer\">");
            html.AppendLine("        <div class=\"ot-container\">");
            html.AppendLine($"            <p>Generated on {DateTime.Now:MMMM dd, yyyy} | OpenText Partner Directory</p>");
            html.AppendLine("        </div>");
            html.AppendLine("    </footer>");

            // JavaScript for filtering
            html.AppendLine("    <script>");
            html.AppendLine(GenerateJavaScript());
            html.AppendLine("    </script>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string GeneratePartnerCard(PartnerWithSolutions partner)
        {
            var card = new StringBuilder();
            var hasSolutions = partner.SolutionCount > 0;
            var cardClass = hasSolutions ? "ot-partner-card ot-has-solutions" : "ot-partner-card";

            card.AppendLine($"                <div class=\"{cardClass}\" data-partner=\"{EscapeHtml(partner.PartnerName.ToLower())}\" data-has-solutions=\"{hasSolutions.ToString().ToLower()}\">");
            card.AppendLine("                    <div class=\"ot-partner-header\">");
            card.AppendLine($"                        <h3 class=\"ot-partner-name\">{EscapeHtml(partner.PartnerName)}</h3>");
            
            if (hasSolutions)
            {
                card.AppendLine($"                        <span class=\"ot-solution-count\">{partner.SolutionCount} Solution{(partner.SolutionCount == 1 ? "" : "s")}</span>");
            }
            else
            {
                card.AppendLine("                        <span class=\"ot-no-solutions\">No Listed Solutions</span>");
            }
            
            card.AppendLine("                    </div>");

            if (hasSolutions)
            {
                card.AppendLine("                    <div class=\"ot-solutions\">");
                card.AppendLine("                        <h4>Solutions:</h4>");
                card.AppendLine("                        <ul class=\"ot-solution-list\">");
                
                foreach (var solution in partner.Solutions.Take(10)) // Show max 10 solutions
                {
                    card.AppendLine($"                            <li>{EscapeHtml(solution.SolutionName)}</li>");
                }
                
                if (partner.Solutions.Count > 10)
                {
                    card.AppendLine($"                            <li class=\"ot-more-solutions\">... and {partner.Solutions.Count - 10} more solutions</li>");
                }
                
                card.AppendLine("                        </ul>");
                card.AppendLine("                    </div>");
            }

            card.AppendLine("                </div>");
            return card.ToString();
        }

        private string GenerateOpenTextStyles()
        {
            return @"
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background-color: #f8f9fa;
        }

        .ot-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 20px;
        }

        /* Header */
        .ot-header {
            background: linear-gradient(135deg, #0066cc 0%, #004499 100%);
            color: white;
            padding: 2rem 0;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .ot-logo {
            font-size: 2.5rem;
            font-weight: 300;
            margin-bottom: 0.5rem;
        }

        .ot-subtitle {
            font-size: 1.1rem;
            opacity: 0.9;
        }

        /* Filters */
        .ot-filters {
            background: white;
            padding: 2rem 0;
            border-bottom: 1px solid #e9ecef;
        }

        .ot-filters h2 {
            color: #0066cc;
            margin-bottom: 1rem;
            font-size: 1.5rem;
        }

        .ot-filter-controls {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
            margin-bottom: 1rem;
        }

        .ot-search-input, .ot-select {
            padding: 0.75rem;
            border: 2px solid #e9ecef;
            border-radius: 5px;
            font-size: 1rem;
            transition: border-color 0.3s ease;
        }

        .ot-search-input:focus, .ot-select:focus {
            outline: none;
            border-color: #0066cc;
        }

        .ot-search-input {
            flex: 1;
            min-width: 300px;
        }

        .ot-btn {
            padding: 0.75rem 1.5rem;
            border: none;
            border-radius: 5px;
            font-size: 1rem;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .ot-btn-secondary {
            background-color: #6c757d;
            color: white;
        }

        .ot-btn-secondary:hover {
            background-color: #545b62;
        }

        .ot-stats {
            display: flex;
            gap: 2rem;
            flex-wrap: wrap;
        }

        .ot-stat {
            color: #666;
            font-size: 0.9rem;
        }

        /* Main content */
        .ot-main {
            padding: 2rem 0;
            min-height: 60vh;
        }

        .ot-partners-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 1.5rem;
        }

        /* Partner cards */
        .ot-partner-card {
            background: white;
            border-radius: 8px;
            padding: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            border-left: 4px solid #e9ecef;
        }

        .ot-partner-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 15px rgba(0,0,0,0.15);
        }

        .ot-has-solutions {
            border-left-color: #0066cc;
        }

        .ot-partner-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 1rem;
        }

        .ot-partner-name {
            color: #0066cc;
            font-size: 1.2rem;
            font-weight: 600;
            flex: 1;
            margin-right: 1rem;
        }

        .ot-solution-count {
            background: #0066cc;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            white-space: nowrap;
        }

        .ot-no-solutions {
            background: #6c757d;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.8rem;
            white-space: nowrap;
        }

        .ot-solutions h4 {
            color: #333;
            margin-bottom: 0.5rem;
            font-size: 1rem;
        }

        .ot-solution-list {
            list-style: none;
            padding: 0;
        }

        .ot-solution-list li {
            padding: 0.25rem 0;
            border-bottom: 1px solid #f8f9fa;
            color: #666;
            font-size: 0.9rem;
        }

        .ot-solution-list li:last-child {
            border-bottom: none;
        }

        .ot-more-solutions {
            font-style: italic;
            color: #0066cc !important;
        }

        .ot-no-results {
            text-align: center;
            padding: 3rem;
            color: #666;
            font-size: 1.2rem;
        }

        /* Footer */
        .ot-footer {
            background: #333;
            color: white;
            text-align: center;
            padding: 1.5rem 0;
            margin-top: 3rem;
        }

        /* Responsive design */
        @media (max-width: 768px) {
            .ot-partners-grid {
                grid-template-columns: 1fr;
            }
            
            .ot-filter-controls {
                flex-direction: column;
            }
            
            .ot-search-input {
                min-width: auto;
            }
            
            .ot-stats {
                flex-direction: column;
                gap: 0.5rem;
            }
            
            .ot-partner-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 0.5rem;
            }
        }";
        }

        private string GenerateJavaScript()
        {
            return @"
        function filterPartners() {
            const searchTerm = document.getElementById('searchInput').value.toLowerCase();
            const solutionFilter = document.getElementById('solutionFilter').value;
            const partnerCards = document.querySelectorAll('.ot-partner-card');
            const noResults = document.getElementById('noResults');
            let visibleCount = 0;

            partnerCards.forEach(card => {
                const partnerName = card.getAttribute('data-partner');
                const hasSolutions = card.getAttribute('data-has-solutions') === 'true';
                
                let showCard = true;
                
                // Search filter
                if (searchTerm && !partnerName.includes(searchTerm)) {
                    showCard = false;
                }
                
                // Solution filter
                if (solutionFilter === 'with-solutions' && !hasSolutions) {
                    showCard = false;
                }
                
                if (showCard) {
                    card.style.display = 'block';
                    visibleCount++;
                } else {
                    card.style.display = 'none';
                }
            });

            // Show/hide no results message
            if (visibleCount === 0) {
                noResults.style.display = 'block';
            } else {
                noResults.style.display = 'none';
            }
        }

        function clearFilters() {
            document.getElementById('searchInput').value = '';
            document.getElementById('solutionFilter').value = 'all';
            filterPartners();
        }

        // Event listeners
        document.getElementById('searchInput').addEventListener('input', filterPartners);
        document.getElementById('solutionFilter').addEventListener('change', filterPartners);

        // Initialize
        document.addEventListener('DOMContentLoaded', function() {
            filterPartners();
        });";
        }

        private string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}