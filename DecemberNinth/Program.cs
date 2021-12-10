
var stopwatch = new System.Diagnostics.Stopwatch();


for(var run = 0; run < 5; run++)
{
  var data = File.ReadAllLines("input");
  stopwatch.Restart();

  var parsedData = new (int height, bool lowPoint)[data.Length, data[0].Length];
  var tasks = new Task<int>[data.Length];
  for (var i = 0; i < data.Length; i++)
  {
    for (var j = 0; j < data[i].Length; j++)
    {
      parsedData[i, j] = (data[i][j] - '0', false);
    }
    if (i > 0)
    {
      var x = i - 1;
      tasks[x] = Task.Run(() => Processor.ProcessLine(parsedData, x));
    }
    if (i == data.Length - 1)
    {
      var x = i;
      tasks[x] = Task.Run(() => Processor.ProcessLine(parsedData, x));
    }
  }
  Task.WaitAll(tasks);

  var score = tasks.Sum(t => t.Result);
  var time = stopwatch.Elapsed.TotalMilliseconds;

  Console.WriteLine(new { score, time });

  // part 2
  stopwatch.Restart();

  parsedData = new (int height, bool lowPoint)[data.Length, data[0].Length];
  var part2tasks = new Task<IEnumerable<(int x, int y)>>[data.Length];
  for (var i = 0; i < data.Length; i++)
  {
    for (var j = 0; j < data[i].Length; j++)
    {
      parsedData[i, j] = (data[i][j] - '0', false);
    }
    if (i > 0)
    {
      var x = i - 1;
      part2tasks[x] = Task.Run(() => Part2Processor.ProcessLine(parsedData, x));
    }
    if (i == data.Length - 1)
    {
      var x = i;
      part2tasks[x] = Task.Run(() => Part2Processor.ProcessLine(parsedData, x));
    }
  }
  Task.WaitAll(part2tasks);

  var part2Scores = part2tasks.SelectMany(t => t.Result)
    // calculate scores for all low points
    .Select(i => Part2Processor.Score(i, parsedData))
    .OrderByDescending(score => score);
    // part2Scores.ToList().ForEach(s => Console.WriteLine("Score: " + s));
    // multiply the three highest
    var part2Score = part2Scores.Take(3)
    .Aggregate((product, score) => product * score);
  var part2Time = stopwatch.Elapsed.TotalMilliseconds;

  Console.WriteLine(new { part2Score, part2Time });
}

public class Processor {
  
private static bool CheckSpace((int height, bool lowPoint)[,] grid, int index, int secondIndex)
{
  var centerLevel = grid[index, secondIndex].height;
  var checks = new (int, int)[] {
    (index, secondIndex - 1), 
    (index, secondIndex + 1), 
    (index - 1, secondIndex), 
    (index + 1, secondIndex)
  };
  
  foreach(var pair in checks)
  {
    var i = pair.Item1;
    var j = pair.Item2;
    if (i < 0 || j < 0 || i > grid.GetUpperBound(0) || j > grid.GetUpperBound(1))
    {
      // point is either our point or outside of the array
      continue;
    }
    var level = grid[i, j].height;
    if (level <= centerLevel)
    {
      // not a lowpoint
      return false;
    }
  }
  
  return true;
}
  public static int ProcessLine((int height, bool lowPoint)[,] grid, int index)
  {
    var width = grid.GetUpperBound(1);
    var dangerLevel = 0;
    for (var i = 0; i <= width; i++)
    {
      // working with grid[index, i]
      // if we can find one neighbor with a height greater than ours, we are safe
      var isLowPoint = CheckSpace(grid, index, i);
      grid[index, i] = (grid[index, i].height, isLowPoint);
      // otherwise, it's dangerous
      if (isLowPoint)
      {
        dangerLevel += grid[index, i].height + 1;
      }
    }

    return dangerLevel;
  }
}
public class Part2Processor {


  public static int Score((int x, int y) lowPoint, (int height, bool lowPoint)[,] grid)
  {
    // reuse the lowPoint as visited
    var score = 1;
    var x = lowPoint.x;
    var y = lowPoint.y;
    var neighbors = new (int x, int y)[] { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) };

    foreach (var neighbor in neighbors)
    {
      if (neighbor.x < 0 || neighbor.y < 0 
        || neighbor.x > grid.GetUpperBound(0) || neighbor.y > grid.GetUpperBound(1))
    {
      // point is either our point or outside of the array
      continue;
    }
      var location = grid[neighbor.x, neighbor.y];
      if (location.lowPoint)
      {
        // already visited
        continue;
      }
      if (location.height == 9)
      {
        // not part of a basin
        continue;
      }
      // this spot is not yet visited or a 9.
      grid[neighbor.x, neighbor.y] = (location.height, true);
      score += Score(neighbor, grid);
    }
    return score;
  }
  private static bool CheckSpace((int height, bool lowPoint)[,] grid, int index, int secondIndex)
  {
    var centerLevel = grid[index, secondIndex].height;
    var checks = new (int, int)[] {
      (index, secondIndex - 1), 
      (index, secondIndex + 1), 
      (index - 1, secondIndex), 
      (index + 1, secondIndex)
    };
    
    foreach(var pair in checks)
    {
      var i = pair.Item1;
      var j = pair.Item2;
      if (i < 0 || j < 0 || i > grid.GetUpperBound(0) || j > grid.GetUpperBound(1))
      {
        // point is either our point or outside of the array
        continue;
      }
      var level = grid[i, j].height;
      if (level <= centerLevel)
      {
        // not a lowpoint
        return false;
      }
    }
    
    return true;
  }
  public static IEnumerable<(int x, int y)> ProcessLine((int height, bool lowPoint)[,] grid, int index)
  {
    var width = grid.GetUpperBound(1);
    for (var i = 0; i <= width; i++)
    {
      // working with grid[index, i]
      // if we can find one neighbor with a height greater than ours, we are safe
      var isLowPoint = CheckSpace(grid, index, i);
      grid[index, i] = (grid[index, i].height, isLowPoint);
      // otherwise, it's dangerous
      if (isLowPoint)
      {
        yield return (index, i);
      }
    }
  }
}
     