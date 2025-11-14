using OpenTextPartnerScraper.Models;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;

namespace OpenTextPartnerScraper.Services
{
    public interface IDataExportService
    {
        Task ExportToCsvAsync(List<Partner> partners, string filePath);
        Task ExportToJsonAsync(List<Partner> partners, string filePath);
        Task ExportSolutionsToCsvAsync(List<PartnerSolution> solutions, string filePath);
        Task ExportSolutionsToJsonAsync(List<PartnerSolution> solutions, string filePath);
        Task ExportPartnersAsync(List<Partner> partners, string baseFileName);
        Task ExportSolutionsAsync(List<PartnerSolution> solutions, string baseFileName);
    }

    public class DataExportService : IDataExportService
    {
        private readonly ILogger<DataExportService> _logger;

        public DataExportService(ILogger<DataExportService> logger)
        {
            _logger = logger;
        }

        public async Task ExportToCsvAsync(List<Partner> partners, string filePath)
        {
            try
            {
                _logger.LogInformation($"Exporting {partners.Count} partners to CSV: {filePath}");

                var csv = new StringBuilder();
                
                // Add CSV header
                csv.AppendLine("Name");

                // Add partner data
                foreach (var partner in partners)
                {
                    csv.AppendLine(EscapeCsvField(partner.Name));
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
                _logger.LogInformation($"Successfully exported partners to CSV: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export partners to CSV: {filePath}");
                throw;
            }
        }

        public async Task ExportToJsonAsync(List<Partner> partners, string filePath)
        {
            try
            {
                _logger.LogInformation($"Exporting {partners.Count} partners to JSON: {filePath}");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var exportData = new
                {
                    TotalCount = partners.Count,
                    ExportDate = DateTime.UtcNow,
                    Partners = partners
                };

                var json = JsonSerializer.Serialize(exportData, options);
                await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
                
                _logger.LogInformation($"Successfully exported partners to JSON: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export partners to JSON: {filePath}");
                throw;
            }
        }

        public async Task ExportSolutionsToCsvAsync(List<PartnerSolution> solutions, string filePath)
        {
            try
            {
                _logger.LogInformation($"Exporting {solutions.Count} solutions to CSV: {filePath}");

                var csv = new StringBuilder();
                
                // Add CSV header
                csv.AppendLine("SolutionName,PartnerName");

                // Add solution data
                foreach (var solution in solutions)
                {
                    csv.AppendLine($"{EscapeCsvField(solution.SolutionName)},{EscapeCsvField(solution.PartnerName)}");
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
                _logger.LogInformation($"Successfully exported solutions to CSV: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export solutions to CSV: {filePath}");
                throw;
            }
        }

        public async Task ExportSolutionsToJsonAsync(List<PartnerSolution> solutions, string filePath)
        {
            try
            {
                _logger.LogInformation($"Exporting {solutions.Count} solutions to JSON: {filePath}");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var exportData = new
                {
                    TotalCount = solutions.Count,
                    ExportDate = DateTime.UtcNow,
                    Solutions = solutions
                };

                var json = JsonSerializer.Serialize(exportData, options);
                await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
                
                _logger.LogInformation($"Successfully exported solutions to JSON: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export solutions to JSON: {filePath}");
                throw;
            }
        }

        public async Task ExportPartnersAsync(List<Partner> partners, string baseFileName)
        {
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
            Directory.CreateDirectory(outputDir);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var jsonFile = Path.Combine(outputDir, $"{baseFileName}_{timestamp}.json");
            var csvFile = Path.Combine(outputDir, $"{baseFileName}_{timestamp}.csv");

            await ExportToJsonAsync(partners, jsonFile);
            await ExportToCsvAsync(partners, csvFile);

            _logger.LogInformation($"Partners exported to JSON and CSV files");
        }

        public async Task ExportSolutionsAsync(List<PartnerSolution> solutions, string baseFileName)
        {
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
            Directory.CreateDirectory(outputDir);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var jsonFile = Path.Combine(outputDir, $"{baseFileName}_{timestamp}.json");
            var csvFile = Path.Combine(outputDir, $"{baseFileName}_{timestamp}.csv");

            await ExportSolutionsToJsonAsync(solutions, jsonFile);
            await ExportSolutionsToCsvAsync(solutions, csvFile);

            _logger.LogInformation($"Solutions exported to JSON and CSV files");
        }

        private static string EscapeCsvField(string? field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // Escape quotes and wrap in quotes if field contains comma, quote, or newline
            if (field.Contains('"') || field.Contains(',') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}