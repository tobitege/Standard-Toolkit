using System.Collections.Generic;

namespace Classes
{
    /// <summary>
    /// Holds the outcome of comparing a palette's _schemeOfficeColors array against the SchemeOfficeColors enum list.
    /// </summary>
    internal sealed class PaletteArrayIssues
    {
        public int MissingCount { get; set; }
        public int UnlabelledCount { get; set; }
        public int OutOfOrderCount { get; set; }
        public int ExtraCount { get; set; }

        public List<int> MissingIndices { get; } = [];
        public List<int> UnlabelledIndices { get; } = [];
        public List<int> OutOfOrderIndices { get; } = [];
        public List<int> ExtraIndices { get; } = [];

        /// <summary>
        /// True when no discrepancies were found.
        /// </summary>
        public bool IsClean => MissingCount == 0 
            && UnlabelledCount == 0 
            && OutOfOrderCount == 0 
            && ExtraCount == 0;
    }
}