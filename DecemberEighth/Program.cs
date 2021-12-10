
var timer = new System.Diagnostics.Stopwatch();

var data = File.ReadLines("input2");

for (var run = 0; run < 1; run ++)
{
  timer.Restart();

  var answer = 0;
  foreach (var line in data)
  {
    var halves = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
    var outputs = halves[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

    answer += outputs.Where(o => o.Length == 2 || o.Length == 3 || o.Length == 4 || o.Length == 7).Count();
  }

  var time = timer.Elapsed.TotalMilliseconds;

  Console.WriteLine(new { answer, time });

  var data2 = File.ReadLines("input");
  timer.Restart();
  var part2Answer = 0;

  string SortString(string s)
  {
    var chars = s.OrderBy(s => s);
    return new String(chars.ToArray());
  }

  foreach (var line in data2)
  {
    // Console.WriteLine(line);
    var halves = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
    var inputs = halves[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var outputs = halves[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var found = new bool[10];
    var lookup = new Dictionary<string, int>();
    // the three letter value is 7
    var seven = SortString(inputs.First(i => i.Length == 3));
    lookup[seven] = 7;
    found[7] = true;
    // the two letter value is 1
    var one = SortString(inputs.First(i => i.Length == 2));
    lookup[one] = 1;
    found[1] = true;
    // the seven letter value is 8
    var eight = SortString(inputs.First(i => i.Length == 7));
    lookup[eight] = 8;
    found[8] = true;
    // the four letter value is 4
    var four = SortString(inputs.First(i => i.Length == 4));
    lookup[four] = 4;
    found[4] = true;
    // the value that appears in 7 but not in 1 is the top line
    var topline = seven.First(letter => !one.Contains(letter));
    // the six letter value that doesn't overlap entirely with one is 6
    var six = SortString(inputs.First(code => code.Length == 6 
      && (!code.Contains(one[0]) || !code.Contains(one[1]))));
    lookup[six] = 6;
    found[6] = true;
    // we can use that to determine which letter is which in 1
    var topRight = one.First(letter => !six.Contains(letter));
    var bottomRight = one.First(letter => letter != topRight);
    // 2 is the only number with 5 parts, the upper right, but not the lower right
    var two = SortString(inputs.First(code => code.Length == 5 && code.Contains(topRight) 
      && !code.Contains(bottomRight)));
    lookup[two] = 2;
    found[2] = true;
    // 0 has six letters
    // I know 1, 2, 4, 6, 7, 8
    // the middle is in 2 and 4. It's the character that's in four and 2, but not in 1
    var middle = four.First(letter => two.Contains(letter) && !one.Contains(letter));
    var zero = SortString(inputs.First(code => code.Length == 6 && !code.Contains(middle)));
    lookup[zero] = 0;
    found[0] = true;
    // at this point we should know 0, 1, 2, 4, 6, 7, 8
    // find 3
    // 3 is not already in my values, and has top right and bottom right
    // probably not the best method, but should work
    var three = SortString(inputs.First(i => i.Length == 5 && !lookup.ContainsKey(SortString(i)) 
      && i.Contains(topRight) && i.Contains(bottomRight)));
    lookup[three] = 3;
    found[3] = true;
    // find 5
    var five = SortString(inputs.First(i => i.Length == 5 && !lookup.ContainsKey(SortString(i))));
    lookup[five] = 5;
    found[5] = true;
    // find 9
    var nine = SortString(inputs.First(i => !lookup.ContainsKey(SortString(i))));
    lookup[nine] = 9;
    found[9] = true;

    // Console.WriteLine(new { zero, one, two, three, four, five, six, seven, eight, nine });

    part2Answer += outputs.Aggregate("", (accumulate, value) => accumulate + lookup[SortString(value)], int.Parse);
  }


  time = timer.Elapsed.TotalMilliseconds;

  Console.WriteLine(new { part2Answer, time });
}