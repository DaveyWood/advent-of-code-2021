using System.Collections.Concurrent;

var input = File.ReadLines("input").GetEnumerator();

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

var counts = new ConcurrentDictionary<char, long>();
uniqueLettersList.ForEach(letter => counts[letter] = 0);
counts[chain[0]] = 1;

var scoreCache = new ConcurrentDictionary<List<char>, Dictionary<char, long>>();

for (var i = 0; i < chain.Count - 1; i++)
{
  var pair = "" + chain[i] + chain[i + 1];
  var expanded = lookup[pair];

  var firstKey = "" + pair[0] + expanded[0];
  var firstScore = lookup[firstKey];
  foreach (var c in firstScore)
  {
    counts[c] += 1;
  }

  var tasks = new List<Task>();
  for (var j = 0; j < expanded.Count - 1; j++)
  {
    var exp = expanded;
    var lu = lookup;
    var counter = j;
    var key = "" + exp[j] + exp[j + 1];
    var toScore = lu[key];
    var v = j;
    var z = expanded.Count;
    var t = Task.Run(() =>
    {
      Console.WriteLine($"Iteration {v + 1} of {z}");
      var localScores = scoreCache.GetOrAdd(toScore, toScore => {
        var newScores = new Dictionary<char, long>();
        foreach (var c in toScore)
        {
          if (newScores.ContainsKey(c))
          {
            newScores[c] += 1;
          }
          else
          {
            newScores[c] = 1;
          }
        }
        return newScores;
      });
      foreach (var characterCount in localScores)
      {
        counts.AddOrUpdate(characterCount.Key, 
          c => characterCount.Value, 
          (c, currentCount) => currentCount + characterCount.Value);
      }
    });
    tasks.Add(t);
  }
  Console.WriteLine(tasks.Count);
  Task.WaitAll(tasks.ToArray());

  // foreach (var c in expanded)
  // {
  //   counts[c] += 1;
  // }
}

Console.WriteLine(new { part1Answer = counts.Values.Max() - counts.Values.Min() });

List<char> ProcessPair(string pair, Dictionary<string, char> rules)
{
  var answer = new List<char>(pair);

  for (var step = 0; step < 20; step++)
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
