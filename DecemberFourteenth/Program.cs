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

for (var step = 0; step < 10; step++ )
{
  var newChain = new List<char>();
  char? lastChar = null;
  foreach(var character in chain)
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
  chain = newChain;
}

var counts = new Dictionary<char, long>();
foreach(char c in chain)
{
  if (counts.ContainsKey(c))
  {
    counts[c] += 1;
  }
  else
  {
    counts[c] = 1;
  }
}

var part1Answer = counts.Values.Max() - counts.Values.Min();

Console.WriteLine(new { part1Answer });
