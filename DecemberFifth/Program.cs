
Line ParseLine(string line)
{
  var points = line.Split(" -> ");
  var start = points[0].Split(',');
  var end = points[1].Split(',');

  return new Line { 
    start = (int.Parse(start[0]), int.Parse(start[1])), 
    end = (int.Parse(end[0]), int.Parse(end[1])) 
  };
}
var watch = new System.Diagnostics.Stopwatch();
for (var run = 0; run < 5; run++)
{
  watch.Restart();
  var data = File.ReadLines("input").Select(ParseLine);

  var ventsByPoint = new Dictionary<(int x, int y), int>();

  var dangerousCount = 0;

  foreach(Line line in data)
  {
    foreach((int x, int y) point in line.GetAllPoints())
    {
      if (ventsByPoint.ContainsKey(point))
      {
        if (ventsByPoint[point] == 1)
        {
          dangerousCount = dangerousCount + 1;
        }
        ventsByPoint[point] = ventsByPoint[point] + 1;
      }
      else
      {
        ventsByPoint[point] = 1;
      }
    }
  }

  var time = watch.Elapsed.TotalMilliseconds;
  Console.WriteLine( new { dangerousCount, time });
}
struct Line
{
  public (int x, int y) start;
  public (int x, int y) end;

  public IEnumerable<(int x, int y)> GetAllPoints()
  {
    if (start.x != end.x && start.y != end.y)
    {
      var xIncrement = start.x < end.x ? 1 : -1;
      var yIncrement = start.y < end.y ? 1 : -1;

      var x = start.x;
      var y = start.y;
      yield return (x, y);
      while (y != end.y && x != end.x)
      {
        y += yIncrement;
        x += xIncrement;
        yield return (x, y);
      }
    }
    else if (start.x < end.x)
    {
      for (int i = start.x; i <= end.x; i++)
      {
        yield return (i, start.y);
      }
    }
    else if (end.x < start.x)
    {
      for (int i = end.x; i <= start.x; i++)
      {
        yield return (i, start.y);
      }
    }
    else if (start.y < end.y)
    {
      for (int i = start.y; i <= end.y; i++)
      {
        yield return (start.x, i);
      }
    }
    else  // end.y < start.y
    {
      for (int i = end.y; i <= start.y; i++)
      {
        yield return (start.x, i);
      }
    }
  }
}