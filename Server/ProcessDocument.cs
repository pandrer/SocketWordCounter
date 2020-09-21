using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Server
{
    class ProcessDocument
    {
        public string FieName { get; set; }

        private WordCounter WordCounter { get; set; }

        public ProcessDocument(string _filename)
        {
            this.FieName = _filename ?? throw new ArgumentException("filename is not empty");
            WordCounter = new WordCounter(_filename);
        }

        public StringBuilder SumaryFile()
        {
            StringBuilder summary = new StringBuilder();

            var stopWatch = Stopwatch.StartNew();
            var wordsResponse = WordCounter.GetTopWords();
            stopWatch.Stop();
            summary.AppendLine($"Word\t\tCount");
            foreach (var word in wordsResponse)
            {
                summary.AppendLine($"{word.Key}\t\t{word.Value}");
            }
            summary.AppendLine($"Words: {wordsResponse.Sum(pair => pair.Value)}");
            summary.AppendLine($"Size: {ReadableFileSize()}");
            summary.AppendLine($"Execution time:: {stopWatch.Elapsed}");
            return summary;
        }

        private string ReadableFileSize()
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            long len = WordCounter.FileSize();
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
