var stopwatch = new System.Diagnostics.Stopwatch();

for (var run = 0; run < 5; run++)
{
  stopwatch.Restart();

  var input = File.ReadLines("input");

  var openers = new HashSet<char>{ '(', '{', '[', '<' };
  var closers = new HashSet<char>{ ')', '}', ']', '>' };

  var scores = new Dictionary<char, int> {
    { ')', 3 },
    { ']', 57 },
    { '}', 1197 },
    { '>', 25137 }
  };

  bool Matches(char opener, char closer)
  {
    return (opener == '(' && closer == ')')
      || (opener == '[' && closer == ']')
      || (opener == '{' && closer == '}')
      || (opener == '<' && closer == '>');
  }

  char CheckLine(string line)
  {
    if (openers == null || closers == null)
    {
      throw new Exception("What the fuck?");
    }
    var stuff = new Stack<char>();
    foreach(var character in line)
    {
      if (openers.Contains(character))
      {
        stuff.Push(character);
      }
      else if (closers.Contains(character))
      {
        var opener = stuff.Pop();
        if (!Matches(opener, character))
        {
          return character;
        }
      }
    }
    return 'x';
  }
  // fastest time in series was 0.5655 milliseconds
  // var score = input.Select(CheckLine).Where(c => c != 'x')
  //   .Select(c => scores[c]).Sum();

  // fastest async time was 0.2285 milliseconds
  var tasks = input.Select(c => Task.Run<char>(() => CheckLine(c))).ToArray();
  Task.WaitAll(tasks);

  var score = tasks.Select(t => t.Result).Where(c => c != 'x')
    .Select(c => scores[c]).Sum();

  var time = stopwatch.Elapsed.TotalMilliseconds;

  Console.WriteLine(new { score, time });

  // part 2
  stopwatch.Restart();

  input = File.ReadLines("input");

  var incompleteScores = new Dictionary<char, long> {
    { '(', 1 },
    { '[', 2 },
    { '{', 3 },
    { '<', 4 }
  };

  long CheckLine2(string line)
  {
    if (openers == null || closers == null || incompleteScores == null)
    {
      throw new Exception("What the fuck?");
    }
    var stuff = new Stack<char>();
    foreach(var character in line)
    {
      if (openers.Contains(character))
      {
        stuff.Push(character);
      }
      else if (closers.Contains(character))
      {
        var opener = stuff.Pop();
        if (!Matches(opener, character))
        {
          // don't care about corrupt lines
          return 0;
        }
      }
    }
    if (stuff.Count == 0)
    {
      return 0;
    }
    var score = incompleteScores[stuff.Pop()];
    while (stuff.Count > 0)
    {
      score = score * 5;
      score += incompleteScores[stuff.Pop()];
    }
    return score;
  }
  // fastest time in series was 0.5216 milliseconds
  // var results = input.Select(CheckLine2).Where(s => s > 0).OrderBy(s => s).ToList();
  // var answer = results[results.Count / 2];

  // fastest async time was 0.2492 milliseconds
  var part2Tasks = input.Select(c => Task.Run<long>(() => CheckLine2(c))).ToArray();
  Task.WaitAll(part2Tasks);
  var results = part2Tasks.Select(t => t.Result).Where(s => s > 0).OrderBy(s => s).ToList();
  var answer = results[results.Count / 2];

  time = stopwatch.Elapsed.TotalMilliseconds;

  Console.WriteLine(new { part2Score = answer, time });
}
