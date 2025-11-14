using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTextPartnerScraper.Services;
using OpenTextPartnerScraper.Models;
using Serilog;
using System.Threading;
using System;

namespace OpenTextPartnerScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configure Serilog for clean, user-friendly output
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}")
                .CreateLogger();

            try
            {
                bool continueRunning = true;
                
                while (continueRunning)
                {
                    Log.Information("OpenText Scraper - Interactive Menu Starting...");

                    // Display attractive header
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                        🚀 OPENTEXT Coding Test 🚀                           ║");
                Console.WriteLine("║                    Advanced Data Extraction & Analysis Tool                 ║");
                Console.WriteLine("║                           Developed by Dennis Htut                          ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    // Menu options with icons and descriptions
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("📋 AVAILABLE OPERATIONS:");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   2. ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("🔺 Question 2: ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Display Triangle Patterns (Mathematical algorithms)");
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   3. ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("⚡ Question 3: ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("OpenText Scraper (Sequential Processing - Partners + Solutions)");
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   4. ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("🌐 Question 4: ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Generate Partner Directory HTML Page (Interactive web interface)");
                    
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    // Separator line
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("────────────────────────────────────────────────────────────────────────────────");
                    Console.ResetColor();
                    
                    // Input prompt
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("💡 Enter your choice (2, 3, 4) or press any other key to exit: ");
                    Console.ForegroundColor = ConsoleColor.White;
                
                var input = Console.ReadLine();

                // Build the host with dependency injection
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        // Register only the optimized services we need
                        services.AddTransient<BalancedSeleniumExtractor>();
                        services.AddTransient<SmartOptimizedPartnerExtractor>();
                        services.AddTransient<JsonJoinService>();
                        services.AddTransient<HtmlPartnerDirectoryService>();
                    })
                    .UseSerilog()
                    .Build();

                // Get the services (only the optimized ones we need)
                var balancedExtractor = host.Services.GetRequiredService<BalancedSeleniumExtractor>();
                var smartOptimizedPartnerExtractor = host.Services.GetRequiredService<SmartOptimizedPartnerExtractor>();
                var joinService = host.Services.GetRequiredService<JsonJoinService>();
                var htmlService = host.Services.GetRequiredService<HtmlPartnerDirectoryService>();
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                if (input?.Trim() == "2")
                {
                    // Display Triangle Patterns (Question 2)
                    Console.ResetColor();
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║                    🔺 QUESTION 2: TRIANGLE PATTERNS 🔺                      ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("📐 Mathematical Triangle Pattern Generation:");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("🔹 Fixed 3×4 Triangle Pattern:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    TriangleWithDimension.DisplayTriangle3x4();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("🔹 Custom 5×7 Triangle Pattern:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    TriangleWithDimension.DisplayCustomTriangle(5, 7);
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Triangle patterns displayed successfully!");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    // Return to menu prompt
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("⬅️ Press any key to return to main menu...");
                    Console.ResetColor();
                    try
                    {
                        Console.ReadKey(true);
                    }
                    catch (InvalidOperationException)
                    {
                        // Handle case when console input is redirected
                        Console.WriteLine("\n✅ Returning to main menu...");
                        Thread.Sleep(1000);
                    }
                }
                else if (input?.Trim() == "3")
                {
                    // Sequential processing with enhanced UI
                    Console.ResetColor();
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║                  ⚡ QUESTION 3: OPENTEXT SCRAPER ⚡                         ║");
                    Console.WriteLine("║                      Sequential Data Processing                              ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🚀 Starting automated sequential processing...");
                    Console.ResetColor();
                    Console.WriteLine();

                    // Step 1 with enhanced formatting
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("═══ STEP 1: EXTRACTING PARTNER SOLUTIONS ═══");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🔍 We are pulling the solution data. Please be patient...");
                    Console.Write("📊 Analyzing solutions: 0 extracted");
                    Console.ResetColor();
                    
                    // Start progress tracking task
                    var solutionProgressCancellation = new CancellationTokenSource();
                    var solutionProgressTask = Task.Run(async () =>
                    {
                        var dots = 0;
                        while (!solutionProgressCancellation.Token.IsCancellationRequested)
                        {
                            dots = (dots + 1) % 4;
                            var dotString = new string('.', dots).PadRight(3);
                            Console.Write($"\r📊 Analyzing solutions: Processing{dotString}");
                            await Task.Delay(800);
                        }
                    });
                    
                    var solutions = await balancedExtractor.ExtractAllSolutions();
                    
                    // Stop progress tracking
                    solutionProgressCancellation.Cancel();
                    await solutionProgressTask;
                    
                    Console.Write($" \r📊 Analyzing solutions: {solutions.Count} extracted... ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Complete!");
                    Console.ResetColor();
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("💾 Saving solution data to JSON...");
                    Console.ResetColor();
                    
                    // Save JSON manually
                    var outputDir = "output";
                    Directory.CreateDirectory(outputDir);
                    var jsonPath = Path.Combine(outputDir, "solutions.json");
                    var jsonOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    var json = System.Text.Json.JsonSerializer.Serialize(solutions, jsonOptions);
                    await File.WriteAllTextAsync(jsonPath, json);
                    
                    Console.Write(" \r💾 Saving solution data to JSON... ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Complete!");
                    Console.ResetColor();
                    
                    var solutionsJsonPath = Path.Combine("output", "solutions.json");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"📁 File saved: {Path.GetFullPath(solutionsJsonPath)} ({solutions.Count} solutions)");
                    Console.ResetColor();
                    Console.WriteLine();

                    // Step 2 with enhanced formatting
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("─── STEP 2: EXTRACTING ALL PARTNERS ───");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🔍 We are pulling the partner data. Please be patient...");
                    Console.Write("🔍 Scraping directory: Starting extraction...");
                    Console.ResetColor();
                    
                    // Create a progress tracking system with real data
                    var progressCancellation = new CancellationTokenSource();
                    var currentProgress = (current: 0, total: 600);
                    
                    var progressTask = Task.Run(async () =>
                    {
                        var dots = 0;
                        while (!progressCancellation.Token.IsCancellationRequested)
                        {
                            dots = (dots + 1) % 4;
                            var dotString = new string('.', dots).PadRight(3);
                            if (currentProgress.current > 0)
                            {
                                Console.Write($"\r🔍 Scraping directory: {currentProgress.current} partners found{dotString}");
                            }
                            else
                            {
                                Console.Write($"\r🔍 Scraping directory: Processing pages{dotString}");
                            }
                            await Task.Delay(800);
                        }
                    });
                    
                    var progress = new Progress<(int current, int total)>(p => currentProgress = p);
                    var partners = await smartOptimizedPartnerExtractor.ExtractAllPartnersAsync(progress);
                    
                    // Stop progress tracking
                    progressCancellation.Cancel();
                    await progressTask;
                    
                    Console.Write($" \r🔍 Scraping directory: {partners.Count} partners found... ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Complete!");
                    Console.ResetColor();
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("💾 Saving partner data to JSON...");
                    Console.ResetColor();
                    
                    // Save JSON manually
                    var partnersOutputDir = "output";
                    Directory.CreateDirectory(partnersOutputDir);
                    var partnersJsonPath = Path.Combine(partnersOutputDir, "partners.json");
                    var partnersJsonOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    var partnersJson = System.Text.Json.JsonSerializer.Serialize(partners, partnersJsonOptions);
                    await File.WriteAllTextAsync(partnersJsonPath, partnersJson);
                    
                    Console.Write(" \r💾 Saving partner data to JSON... ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Complete!");
                    Console.ResetColor();
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"📁 File saved: {Path.GetFullPath(partnersJsonPath)} ({partners.Count} partners)");
                    Console.ResetColor();
                    Console.WriteLine();

                    // Step 3 with enhanced formatting
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("═══ STEP 3: JOINING PARTNER-SOLUTION DATA ═══");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🔗 Processing and merging datasets. Please be patient...");
                    Console.Write("🔗 Merging datasets: analyzing relationships...");
                    Console.ResetColor();
                    
                    try
                    {
                        var partnersWithSolutions = await joinService.JoinPartnerSolutionFiles(
                            Path.Combine("output", "partners.json"),
                            Path.Combine("output", "solutions.json"));
                        await joinService.SaveJoinedData(partnersWithSolutions, Path.Combine("output", "partners_with_solutions.json"));
                        
                        // Small delay to ensure file is written
                        await Task.Delay(500);
                        
                        Console.Write($" \r🔗 Merging datasets: {partnersWithSolutions.Count} records processed... ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✅ Complete!");
                        Console.ResetColor();
                        
                        var mergedJsonPath = Path.Combine("output", "partners_with_solutions.json");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"📁 File saved: {Path.GetFullPath(mergedJsonPath)} ({partnersWithSolutions.Count} records)");
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    catch (Exception mergeEx)
                    {
                        Console.Write(" \r🔗 Merging datasets: ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Error occurred during merge!");
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"⚠️ Error details: {mergeEx.Message}");
                        Console.ResetColor();
                        
                        // Create an empty file as fallback
                        var emptyList = new List<PartnerWithSolutions>();
                        await joinService.SaveJoinedData(emptyList, Path.Combine("output", "partners_with_solutions.json"));
                        
                        var mergedJsonPath = Path.Combine("output", "partners_with_solutions.json");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"📁 Fallback file saved: {Path.GetFullPath(mergedJsonPath)} (0 records)");
                        Console.ResetColor();
                        Console.WriteLine();
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("🎯 Sequential processing completed successfully!");
                    Console.WriteLine("✅ All three steps have been executed automatically");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    // Return to menu prompt
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("⬅️ Press any key to return to main menu...");
                    Console.ResetColor();
                    try
                    {
                        Console.ReadKey(true);
                    }
                    catch (InvalidOperationException)
                    {
                        // Handle case when console input is redirected
                        Console.WriteLine("\n✅ Returning to main menu...");
                        Thread.Sleep(1000);
                    }
                }
                else if (input?.Trim() == "4")
                {
                    // Generate HTML Partner Directory page with enhanced UI
                    Console.ResetColor();
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║              🌐 QUESTION 4: HTML PARTNER DIRECTORY 🌐                       ║");
                    Console.WriteLine("║                    Interactive Web Interface Generator                       ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🎨 Generating Partner Directory HTML page...");
                    Console.ResetColor();
                    await htmlService.GeneratePartnerDirectoryPage();
                    Console.WriteLine();
                    
                    // Return to menu prompt
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("⬅️ Press any key to return to main menu...");
                    Console.ResetColor();
                    try
                    {
                        Console.ReadKey(true);
                    }
                    catch (InvalidOperationException)
                    {
                        // Handle case when console input is redirected
                        Console.WriteLine("\n✅ Returning to main menu...");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    // Exit application with enhanced UI
                    Console.ResetColor();
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║                           👋 THANK YOU! 👋                                   ║");
                    Console.WriteLine("║                 Thank you for checking my coding assignment!                 ║");
                    Console.WriteLine("║                           Developed by Dennis Htut                          ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("🔧 Application Features Used:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("   • Triangle Pattern Generation (Question 2)");
                    Console.WriteLine("   • OpenText Data Scraping (Question 3)");
                    Console.WriteLine("   • HTML Directory Generation (Question 4)");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✨ Have a great day!");
                    Console.ResetColor();
                    continueRunning = false;
                }

                logger.LogInformation("All processing completed successfully");
                }
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Log.Fatal(ex, "Application terminated unexpectedly");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
                Environment.ExitCode = 1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("🎯 Application terminated.");
            Console.ResetColor();
            try
            {
                Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
                // Handle case where console input is redirected or not available
                Console.WriteLine("Application completed.");
            }
        }

        private static async Task JoinPartnerSolutionFiles(ILogger<Program> logger, JsonJoinService joinService)
        {
            try
            {
                Console.WriteLine("=== STEP 3: JOINING PARTNER AND SOLUTION FILES ===");
                logger.LogInformation("🔗 Starting to join partner and solution JSON files...");

                var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
                
                // Use fixed filenames instead of searching for timestamped files
                var partnerFile = Path.Combine(outputDir, "partners.json");
                var solutionFile = Path.Combine(outputDir, "solutions.json");

                if (!File.Exists(partnerFile))
                {
                    Console.WriteLine("❌ partners.json file not found. This should not happen in sequential processing.");
                    logger.LogError("No partners.json file found after extraction step");
                    return;
                }

                if (!File.Exists(solutionFile))
                {
                    Console.WriteLine("❌ solutions.json file not found. This should not happen in sequential processing.");
                    logger.LogError("No solutions.json file found after extraction step");
                    return;
                }

                Console.WriteLine($"📁 Using partner file: partners.json");
                Console.WriteLine($"📁 Using solution file: solutions.json");

                // Join the files
                var joinedData = await joinService.JoinPartnerSolutionFiles(partnerFile, solutionFile);

                if (joinedData.Any())
                {
                    // Save the joined data with fixed filename
                    var outputPath = Path.Combine(outputDir, "partners_with_solutions.json");
                    
                    await joinService.SaveJoinedData(joinedData, outputPath);

                    Console.WriteLine($"🎯 Successfully joined partner and solution data!");
                    Console.WriteLine($"💾 Final unified dataset saved to:");
                    Console.WriteLine($"   📄 JSON: {outputPath}");
                    
                    logger.LogInformation($"🎯 Successfully joined and exported unified partner-solution dataset");

                    // Show comprehensive summary
                    var partnersWithSolutions = joinedData.Count(p => p.SolutionCount > 0);
                    var totalSolutions = joinedData.Sum(p => p.SolutionCount);
                    
                    Console.WriteLine($"\n📊 Final Dataset Summary:");
                    Console.WriteLine($"   👥 Total partners: {joinedData.Count}");
                    Console.WriteLine($"   🔗 Partners with solutions: {partnersWithSolutions}");
                    Console.WriteLine($"   💡 Total solutions linked: {totalSolutions}");
                    Console.WriteLine($"   📈 Coverage: {(partnersWithSolutions * 100.0 / joinedData.Count):F1}% of partners have solutions");

                    // Show top partners with most solutions
                    Console.WriteLine($"\n🏆 Top Partners by Solution Count:");
                    foreach (var partner in joinedData.Where(p => p.SolutionCount > 0).Take(5))
                    {
                        Console.WriteLine($"   • {partner.PartnerName}: {partner.SolutionCount} solution(s)");
                    }
                    
                    var morePartnersWithSolutions = joinedData.Count(p => p.SolutionCount > 0) - 5;
                    if (morePartnersWithSolutions > 0)
                    {
                        Console.WriteLine($"   ... and {morePartnersWithSolutions} more partners with solutions");
                    }
                }
                else
                {
                    Console.WriteLine("❌ No data was joined. Please check the logs for details.");
                    logger.LogWarning("No data was successfully joined during sequential processing.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during sequential join operation");
                Console.WriteLine($"❌ Error during join operation: {ex.Message}");
            }
        }
    }
}
