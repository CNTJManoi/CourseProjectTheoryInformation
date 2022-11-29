using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSender.Algorithms
{
    public static class InfoAboutCode
    {
        public static float FindEntropy(string message)
        {
            float entropy = 0;
            var messageNotDuplicate = new string(message.Distinct().ToArray());
            var sortMessage = string.Concat(messageNotDuplicate.OrderBy(x => x).ToList());
            for (var i = 0; i < sortMessage.Length; i++)
            {
                var count = message.Where(x => x == sortMessage[i]).Count();
                entropy += (float)Math.Round((double)count / message.Length * Math.Log((double)count / message.Length, 2),
                    3);
            }

            entropy *= -1;
            return entropy;
        }

        public static float FindMaxEntropy(string message)
        {
            var messageNotDuplicate = new string(message.Distinct().ToArray());
            return (float)Math.Round(Math.Log(messageNotDuplicate.Length, 2), 3);
        }

        public static float FindWeightedAverageLength(List<float> Probabilities, List<float> Lm)
        {
            float weightedAverageLength = 0;
            for (var i = 0; i < Probabilities.Count; i++) weightedAverageLength += Probabilities[i] * Lm[i];
            return weightedAverageLength;
        }

        public static string FindCodeOptimality(List<float> Probabilities, List<float> Lm, string message)
        {
            var result = "";
            var entropy = FindEntropy(message);
            var weightedAverage = FindWeightedAverageLength(Probabilities, Lm);
            result = weightedAverage + ">=" + entropy + " + 1 ; ";
            if (weightedAverage >= entropy + 1) result += "Код неоптимальный";
            else result += "Код оптимальный";
            return result;
        }

        public static float FindRelativeRedundancy(List<float> Probabilities, List<float> Lm, string message)
        {
            if (Probabilities.Count == 1) return 0;
            var weightedAverageLength = FindWeightedAverageLength(Probabilities, Lm);
            var entropy = FindEntropy(message);
            return 1.0f - entropy / weightedAverageLength;
        }

        public static List<float> CalculateProbabilities(string message)
        {
            var Probabilities = new List<float>();
            var messageNotDuplicate = new string(message.Distinct().ToArray());
            var sortMessage = string.Concat(messageNotDuplicate.OrderBy(x => x).ToList());
            for (var i = 0; i < sortMessage.Length; i++)
            {
                var count = message.Where(x => x == sortMessage[i]).Count();
                Probabilities.Add((float)Math.Round((double)count / message.Length, 2));
            }

            return Probabilities;
        }

        public static List<char> FindAlphabet(string message)
        {
            var Alphabit = new List<char>();
            var messageNotDuplicate = new string(message.Distinct().ToArray());
            var sortMessage = string.Concat(messageNotDuplicate.OrderBy(x => x).ToList());
            foreach (var letter in sortMessage) Alphabit.Add(letter);
            return Alphabit;
        }
    }
}
