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
}