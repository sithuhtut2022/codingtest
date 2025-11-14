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
                
                // Check if files exist with full paths
                var fullPartnersPath = Path.GetFullPath(partnersJsonPath);
                var fullSolutionsPath = Path.GetFullPath(solutionsJsonPath);
                
                _logger.LogInformation($"📁 Looking for partners file: {fullPartnersPath}");
                _logger.LogInformation($"📁 Looking for solutions file: {fullSolutionsPath}");
                
                if (!File.Exists(fullPartnersPath))
                {
                    _logger.LogError($"❌ Partners file not found: {fullPartnersPath}");
                    return new List<PartnerWithSolutions>();
                }
                
                if (!File.Exists(fullSolutionsPath))
                {
                    _logger.LogError($"❌ Solutions file not found: {fullSolutionsPath}");
                    return new List<PartnerWithSolutions>();
                }

                _logger.LogInformation("📄 Reading partners file...");
                var partnersJson = await File.ReadAllTextAsync(fullPartnersPath);
                
                _logger.LogInformation("📄 Reading solutions file...");
                var solutionsJson = await File.ReadAllTextAsync(fullSolutionsPath);
                
                if (string.IsNullOrWhiteSpace(partnersJson))
                {
                    _logger.LogWarning("⚠️ Partners file is empty");
                    return new List<PartnerWithSolutions>();
                }
                
                if (string.IsNullOrWhiteSpace(solutionsJson))
                {
                    _logger.LogWarning("⚠️ Solutions file is empty");
                    return new List<PartnerWithSolutions>();
                }

                _logger.LogInformation("🔄 Parsing JSON data...");
                
                // Set up JSON options for case-insensitive property matching
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                // Handle partners file which might have wrapper structure
                List<Partner> partners;
                try
                {
                    // Try to parse as direct array first
                    partners = JsonSerializer.Deserialize<List<Partner>>(partnersJson, jsonOptions) ?? new List<Partner>();
                }
                catch (JsonException)
                {
                    // If that fails, try to parse as wrapper object
                    var partnersWrapper = JsonSerializer.Deserialize<JsonElement>(partnersJson, jsonOptions);
                    if (partnersWrapper.TryGetProperty("partners", out var partnersArray))
                    {
                        partners = JsonSerializer.Deserialize<List<Partner>>(partnersArray.GetRawText(), jsonOptions) ?? new List<Partner>();
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Could not find 'partners' property in partners file");
                        partners = new List<Partner>();
                    }
                }
                
                // Handle solutions file which might also have wrapper structure  
                List<PartnerSolution> solutions;
                try
                {
                    // Try to parse as direct array first
                    solutions = JsonSerializer.Deserialize<List<PartnerSolution>>(solutionsJson, jsonOptions) ?? new List<PartnerSolution>();
                }
                catch (JsonException)
                {
                    // If that fails, try to parse as wrapper object
                    var solutionsWrapper = JsonSerializer.Deserialize<JsonElement>(solutionsJson, jsonOptions);
                    if (solutionsWrapper.TryGetProperty("solutions", out var solutionsArray))
                    {
                        solutions = JsonSerializer.Deserialize<List<PartnerSolution>>(solutionsArray.GetRawText(), jsonOptions) ?? new List<PartnerSolution>();
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Could not find 'solutions' property in solutions file");
                        solutions = new List<PartnerSolution>();
                    }
                }

                _logger.LogInformation($"📊 Loaded {partners.Count} partners and {solutions.Count} solutions");

                // Debug: Show sample data to understand structure
                if (partners.Count > 0)
                {
                    _logger.LogInformation($"🔍 Sample partner: '{partners.First().Name}'");
                }
                if (solutions.Count > 0)
                {
                    _logger.LogInformation($"🔍 Sample solution: '{solutions.First().SolutionName}' -> '{solutions.First().PartnerName}'");
                }

                // Group solutions by partner name
                var solutionsByPartner = solutions
                    .Where(s => !string.IsNullOrEmpty(s.PartnerName))
                    .GroupBy(s => s.PartnerName.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
                
                _logger.LogInformation($"🔗 Grouped solutions into {solutionsByPartner.Count} partner groups");
                
                // Show top 5 solution partner names for debugging
                var topSolutionPartners = solutionsByPartner.Keys.Take(5).ToList();
                _logger.LogInformation($"🔍 Top solution partners: {string.Join(", ", topSolutionPartners.Select(p => $"'{p}'"))}");
                
                // Show top 5 partner names for debugging  
                var topPartners = partners.Take(5).Select(p => p.Name).ToList();
                _logger.LogInformation($"🔍 Top partners: {string.Join(", ", topPartners.Select(p => $"'{p}'"))}");

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
                
                // Find unmatched solutions
                var allMatchedSolutions = result.SelectMany(p => p.Solutions).ToList();
                var unmatchedSolutions = solutions.Except(allMatchedSolutions).ToList();
                var uniqueUnmatchedPartnerNames = unmatchedSolutions
                    .Select(s => s.PartnerName)
                    .Distinct()
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                _logger.LogInformation($"✅ Join completed!");
                _logger.LogInformation($"📈 {partnersWithSolutions}/{partners.Count} partners have solutions");
                _logger.LogInformation($"🔗 {totalSolutionsMatched}/{solutions.Count} solutions matched to partners");
                
                if (unmatchedSolutions.Count > 0)
                {
                    _logger.LogWarning($"⚠️  {unmatchedSolutions.Count} solutions could not be matched to partners");
                    _logger.LogWarning($"🔍 Unmatched partner names ({uniqueUnmatchedPartnerNames.Count}): {string.Join(", ", uniqueUnmatchedPartnerNames.Take(10).Select(n => $"'{n}'"))}");
                    
                    if (uniqueUnmatchedPartnerNames.Count > 10)
                    {
                        _logger.LogWarning($"    ... and {uniqueUnmatchedPartnerNames.Count - 10} more unmatched partner names");
                    }
                }

                return result;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "❌ JSON parsing error while joining files. Check file format.");
                _logger.LogError($"JSON Error Details: {jsonEx.Message}");
                return new List<PartnerWithSolutions>();
            }
            catch (FileNotFoundException fileEx)
            {
                _logger.LogError(fileEx, "❌ File not found while joining data");
                return new List<PartnerWithSolutions>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error joining partner and solution files");
                _logger.LogError($"Error Details: {ex.Message}");
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