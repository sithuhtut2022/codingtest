using Microsoft.Extensions.Logging;
using OpenTextPartnerScraper.Models;
using System.Text.Json;

namespace OpenTextPartnerScraper.Services
{
    public class JsonJoinService
    {
        private readonly ILogger<JsonJoinService> _logger;

        public JsonJoinService(ILogger<JsonJoinService> logger)
        {
            _logger = logger;
        }

        public async Task<List<PartnerWithSolutions>> JoinPartnerSolutionFiles(string partnersJsonPath, string solutionsJsonPath)
        {
            try
            {
                _logger.LogInformation("🔗 Starting to join partner and solution files...");
                
                // Read partners file
                if (!File.Exists(partnersJsonPath))
                {
                    _logger.LogError($"Partners file not found: {partnersJsonPath}");
                    return new List<PartnerWithSolutions>();
                }
                
                if (!File.Exists(solutionsJsonPath))
                {
                    _logger.LogError($"Solutions file not found: {solutionsJsonPath}");
                    return new List<PartnerWithSolutions>();
                }

                var partnersJson = await File.ReadAllTextAsync(partnersJsonPath);
                var solutionsJson = await File.ReadAllTextAsync(solutionsJsonPath);

                var partners = JsonSerializer.Deserialize<List<Partner>>(partnersJson) ?? new List<Partner>();
                var solutions = JsonSerializer.Deserialize<List<PartnerSolution>>(solutionsJson) ?? new List<PartnerSolution>();

                _logger.LogInformation($"📊 Loaded {partners.Count} partners and {solutions.Count} solutions");

                // Group solutions by partner name
                var solutionsByPartner = solutions
                    .Where(s => !string.IsNullOrEmpty(s.PartnerName))
                    .GroupBy(s => s.PartnerName.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

                // Create the combined result
                var result = new List<PartnerWithSolutions>();

                foreach (var partner in partners)
                {
                    var partnerWithSolutions = new PartnerWithSolutions
                    {
                        PartnerName = partner.Name
                    };

                    // Find solutions for this partner (fuzzy matching)
                    if (solutionsByPartner.ContainsKey(partner.Name))
                    {
                        partnerWithSolutions.Solutions = solutionsByPartner[partner.Name];
                    }
                    else
                    {
                        // Try fuzzy matching for slight name variations
                        var matchingKey = solutionsByPartner.Keys
                            .FirstOrDefault(key => 
                                string.Equals(key, partner.Name, StringComparison.OrdinalIgnoreCase) ||
                                key.Contains(partner.Name, StringComparison.OrdinalIgnoreCase) ||
                                partner.Name.Contains(key, StringComparison.OrdinalIgnoreCase));

                        if (matchingKey != null)
                        {
                            partnerWithSolutions.Solutions = solutionsByPartner[matchingKey];
                            _logger.LogDebug($"🔍 Fuzzy match: '{partner.Name}' -> '{matchingKey}'");
                        }
                    }

                    result.Add(partnerWithSolutions);
                }

                // Sort by partner name and then by solution count (partners with more solutions first)
                result = result
                    .OrderByDescending(p => p.SolutionCount)
                    .ThenBy(p => p.PartnerName)
                    .ToList();

                var partnersWithSolutions = result.Count(p => p.SolutionCount > 0);
                var totalSolutionsMatched = result.Sum(p => p.SolutionCount);

                _logger.LogInformation($"✅ Join completed!");
                _logger.LogInformation($"📈 {partnersWithSolutions}/{partners.Count} partners have solutions");
                _logger.LogInformation($"🔗 {totalSolutionsMatched}/{solutions.Count} solutions matched to partners");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error joining partner and solution files");
                return new List<PartnerWithSolutions>();
            }
        }

        public async Task SaveJoinedData(List<PartnerWithSolutions> data, string outputPath)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var json = JsonSerializer.Serialize(data, jsonOptions);
                await File.WriteAllTextAsync(outputPath, json);

                _logger.LogInformation($"💾 Joined data saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error saving joined data to {outputPath}");
            }
        }
    }
}