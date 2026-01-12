// Plik: TextCheckResult.cs
using System.Collections.Generic;

namespace Reklamacje_Dane.Models
{
    // Klasa do przechowywania zagregowanych wyników sprawdzania
    public class TextCheckResult
    {
        public string OriginalText { get; set; }
        public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
        public Dictionary<string, int> WordRepetitions { get; set; } = new Dictionary<string, int>();
        public ReadabilityMetrics Readability { get; set; } = new ReadabilityMetrics();
    }

    public class ErrorDetail
    {
        public string Message { get; set; }
        public string ShortMessage { get; set; }
        public List<string> Suggestions { get; set; } = new List<string>();
        public int Offset { get; set; }
        public int Length { get; set; }
        public string Type { get; set; } // "Spelling", "Grammar", "Style", "Other"
        public string RuleId { get; set; }
    }

    public class ReadabilityMetrics
    {
        public int WordCount { get; set; }
        public int SentenceCount { get; set; }
        public double AverageWordsPerSentence { get; set; }
        public double AverageSyllablesPerWord { get; set; }
        public double FleschReadingEaseScore { get; set; } // Note: Primarily for English, may need adaptation for Polish
    }
}