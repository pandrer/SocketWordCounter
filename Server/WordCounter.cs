using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class WordCounter
    {

        private const uint TopCount = 100;
        private readonly FileInfo InputFile;
        private static readonly char[] Separators = { ' ', '.', ',' };
        private static readonly HashSet<char> InvalidCharacters = new HashSet<char>(new[] { '®', '"', '\'', '!', '(', ')', '{', '}', '<', '>', '|', '?', '-', '_', '&' });

        public WordCounter(string fileName)
        {
            fileName = !string.IsNullOrEmpty(fileName) ? @"C:\Users\pandr\source\repos\WordCounter\Server\" + fileName : throw new ArgumentException("filename is not empty");
            this.InputFile = new FileInfo(fileName);
            if (!this.InputFile.Exists)
            {
                throw new Exception($"{fileName} not exists");
            }
        }


        public IDictionary<string, uint> GetTopWords()
        {
            Console.WriteLine(nameof(GetTopWords) + "...");

            var result = new Dictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);
            Parallel.ForEach(
                File.ReadLines(InputFile.FullName),
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                () => new Dictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase),
                (line, state, index, localDic) =>
                {
                    foreach (var word in line.Split(Separators, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!IsValidWord(word)) { continue; }
                        TrackWordsOccurrence(localDic, word);
                    }
                    return localDic;
                },
                localDic =>
                {
                    lock (result)
                    {
                        foreach (var pair in localDic)
                        {
                            var key = pair.Key;
                            if (result.ContainsKey(key))
                            {
                                result[key] += pair.Value;
                            }
                            else
                            {
                                result[key] = pair.Value;
                            }
                        }
                    }
                }
            );

            return result
                .OrderByDescending(kv => kv.Value)
                .Take((int)TopCount)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public long FileSize() => this.InputFile.Length;
        private bool IsValidWord(string word) => !InvalidCharacters.Contains(word[0]);

        private void TrackWordsOccurrence(IDictionary<string, uint> wordCounts, string word)
        {
            if (wordCounts.TryGetValue(word, out uint count))
            {
                wordCounts[word] = count + 1;
            }
            else
            {
                wordCounts[word] = 1;
            }
        }
    }
}
