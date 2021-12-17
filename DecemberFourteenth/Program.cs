using System.Collections.Concurrent;

var input = File.ReadLines("input2").GetEnumerator();

input.MoveNext();

var chain = new List<char>();
chain.AddRange(input.Current);

input.MoveNext();
// separator

var rules = new Dictionary<string, char>();
while (input.MoveNext())
{
  // rules look like "CH -> B"
  var rule = input.Current;
  rules.Add(rule.Substring(0, 2), rule.Substring(6)[0]);
}

var uniqueLetters = rules.Values.ToHashSet();
chain.ForEach(c => uniqueLetters.Add(c));

var uniqueLettersList = uniqueLetters.ToList();

var lookup = new Dictionary<string, List<char>>();

// for every possible combination, see what the value would look like after 10 rounds
for (var i = 0; i < uniqueLettersList.Count; i++)
{
  for (var j = 0; j < uniqueLettersList.Count; j++)
  {
    var pair = "" + uniqueLettersList[i] + uniqueLettersList[j];
    var endValue = ProcessPair(pair, rules);
    lookup[pair] = endValue;
  }
}

var counts = new Dictionary<char, long>();
uniqueLettersList.ForEach(letter => counts[letter] = 0);
counts[chain[0]] = 1;

for (var i = 0; i < chain.Count - 1; i++)
{
  var pair = "" + chain[i] + chain[i + 1];
  var expanded = lookup[pair];
  foreach (var c in expanded)
  {
    counts[c] += 1;
  }
}

Console.WriteLine(new { part1Answer = counts.Values.Max() - counts.Values.Min() });

List<char> ProcessPair(string pair, Dictionary<string, char> rules)
{
  var answer = new List<char>(pair);

  for (var step = 0; step < 10; step++)
  {
    var newChain = new List<char>();
    char? lastChar = null;
    foreach (var character in answer)
    {
      if (lastChar != null)
      {
        var key = "" + lastChar + character;
        if (rules.ContainsKey(key))
        {
          newChain.Add(rules[key]);
        }
      }
      newChain.Add(character);
      lastChar = character;
    }
    answer = newChain;
  }
  // remove the first char, so ABC -> A + PP(AB) + PP(BC)
  answer.RemoveAt(0);
  return answer;
}

// var totalCounts = new ConcurrentDictionary<char, long>();
// var lookup = new ConcurrentDictionary<string, (List<char> characters, Dictionary<char, long> score)>();
// // to prevent dupes, lookup doesn't include the first character in the pair. I think that means i just have to 
// // include the original chain[0]
// for (var i = 0; i < chain.Count - 1; i++)
// {
//   var pair = "" + chain[i] + chain[i + 1];
//   if (!lookup.ContainsKey(pair))
//   {
//     ProcessPair(pair, rules, lookup);
//   }
// }
// // now the string is...
// // chain[n] + lookup[n concat n + 1]
// for (var i = 0; i < chain.Count - 1; i++)
// {
//   var pair = "" + chain[i] + chain[i + 1];
//   var series = lookup[pair].characters;
//   Console.WriteLine(series.Count);
//   var firstSet = "" + pair[0] + series[0];
//   var lastSet = "" + series[series.Count - 1] + pair[1];
//     if (!lookup.ContainsKey(firstSet))
//     {
//       ProcessPair(firstSet, rules, lookup);
//     }
//     if (!lookup.ContainsKey(lastSet))
//     {
//       ProcessPair(lastSet, rules, lookup);
//     }
//   ScorePair(firstSet, totalCounts, lookup);
//   ScorePair(lastSet, totalCounts, lookup);
//   for (var j = 0; j < series.Count - 1; j++)
//   {
//     var newPair = "" + series[j] + series[j + 1];
//     if (!lookup.ContainsKey(newPair))
//     {
//       ProcessPair(newPair, rules, lookup);
//     }
//     ScorePair(newPair, totalCounts, lookup);
//   }
// }

// totalCounts[chain[0]] += 1;

// var part2Answer = totalCounts.Values.Max() - totalCounts.Values.Min();

// Console.WriteLine( new { part2Answer });

// void ScorePair(string pair, ConcurrentDictionary<char, long> scoreBoard, 
//   ConcurrentDictionary<string, (List<char> characters, Dictionary<char, long> score)> lookup)
// {
//   var counts = lookup[pair].score;
//   Console.WriteLine( new { counts });
//   foreach (var entry in counts)
//   {
//     scoreBoard.AddOrUpdate(entry.Key, c => entry.Value, (c, current) => current + entry.Value);
//   }
// }

// void ProcessPair(string pair, 
//   Dictionary<string, char> rules, 
//   ConcurrentDictionary<string, (List<char> characters, Dictionary<char, long> score)> lookup)
// {
//   var chain = new List<char>(pair);
//   var lookupKey = "" + chain[0] + chain[1];
//   for (var step = 0; step < 20; step++)
//   {
//     var newChain = new List<char>();
//     char? lastChar = null;
//     foreach(var character in chain)
//     {
//       if (lastChar != null)
//       {
//         var key = "" + lastChar + character;
//         if (rules.ContainsKey(key))
//         {
//           Console.WriteLine("Found rule");
//           newChain.Add(rules[key]);
//           Console.WriteLine(newChain.Count);
//         }
//         newChain.Add(character);
//       }
//       lastChar = character;
//     }
//     chain = newChain;
//   }

//   var counts = new Dictionary<char, long>();
//   foreach(char c in chain)
//   {
//     if (counts.ContainsKey(c))
//     {
//       counts[c] += 1;
//     }
//     else
//     {
//       counts[c] = 1;
//     }
//   }
//   Console.WriteLine(chain.Count);
//   lookup[lookupKey] = (chain, counts);
// }
// // var part1Answer = counts.Values.Max() - counts.Values.Min();

// Console.WriteLine(new { part1Answer });
