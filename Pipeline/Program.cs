using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pipeline
{
    public class Frequency
    {
        public string term { get; set; }
        public int frequency { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PrintAll(Sort(RemoveStopWords(Frequencies(Tokenize(ReadFile(args[0]))))));
        }

        public static void PrintAll(Dictionary<string, Frequency> input)
        {
            int counter = 0;
            foreach (KeyValuePair<string, Frequency> pair in input)
            {
                if (counter != 25)
                {
                    Console.WriteLine("{0}  -  {1}", pair.Value.term, pair.Value.frequency);
                    counter++;
                }
            }
        }

        public static Dictionary<string, Frequency> RemoveStopWords(Dictionary<string, Frequency> input)
        {
            HashSet<string> stopwords = new HashSet<string>();
            using (StreamReader sr = new StreamReader("stop_words.txt"))
            {
                string file_data = sr.ReadToEnd().Replace(",\n\n", "");
                string[] temp = Regex.Split(file_data, "\\W+");

                foreach (string stopword in temp)
                {
                    stopwords.Add(stopword);
                }
            }

            foreach (string word in stopwords)
            {
                input.Remove(word);
            }

            return input;
        }

        public static Dictionary<string, Frequency> Sort(Dictionary<string, Frequency> input)
        {
            return input.OrderByDescending(x => x.Value.frequency).ToDictionary(x => x.Key, x => x.Value);
        }

        public static Dictionary<string, Frequency> Frequencies(List<string> words)
        {
            Hashtable duplicates = new Hashtable();

            foreach (string word in words)
            {
                Frequency temp = new Frequency { term = word, frequency = 1 };
                if (duplicates[word] == null)
                {
                    duplicates[word] = temp;
                }
                else
                {
                    Frequency update = duplicates[word] as Frequency;
                    update.frequency += 1;
                }
            }

            Dictionary<string, Frequency> freq = new Dictionary<string, Frequency>();
            foreach (DictionaryEntry entry in duplicates)
            {
                freq.Add(entry.Key as string, entry.Value as Frequency);
            }

            return freq;
        }

        static Regex regex_token = new Regex("(\\w+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static List<string> Tokenize(string input)
        {
            input += ".";
            MatchCollection matches = regex_token.Matches(input);
            List<string> result = new List<string>();

            foreach (Match m in matches)
            {
                if (m.Groups[1].Value.ToLower() != "s")
                {
                    result.Add(m.Groups[1].Value.ToLower());
                }
            }

            return result;
        }

        public static string ReadFile(string file)
        {
            var request = WebRequest.Create(file);
            using (var response = request.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var text = reader.ReadToEnd();
                return text.ToString().Replace('_', ' ');
            }
        }
    }
}
