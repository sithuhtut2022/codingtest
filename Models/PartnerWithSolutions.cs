namespace OpenTextPartnerScraper.Models
{
    public class PartnerWithSolutions
    {
        public string PartnerName { get; set; } = string.Empty;
        public List<PartnerSolution> Solutions { get; set; } = new List<PartnerSolution>();
        public int SolutionCount => Solutions.Count;
    }
}