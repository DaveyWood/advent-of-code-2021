
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
  
// private static bool CheckSpace((int height, bool lowPoint)[,] grid, int index, int secondIndex)
// {
//   try
//   {
//     var centerLevel = grid[index, secondIndex].height;
//     for (int i = index - 1; i <= index + 1; i++)
//     {
//       for (int j = secondIndex - 1; j <= secondIndex + 1; j++)
//       {
//         if (i < 0 || j < 0 || i > grid.GetUpperBound(0) || j > grid.GetUpperBound(1) || (i == index && j == secondIndex))
//         {
//           // point is either our point or outside of the array
//           continue;
//         }
//         var level = grid[i, j].height;
//         if (level < centerLevel)
//         {
//           // not a lowpoint
//           return false;
//         }
//       }
//     }
//     // Console.WriteLine(new { index, secondIndex });
//     return true;
//   }
//   catch
//   {
//     Console.WriteLine(new { index, secondIndex });
//     throw;
//   }
// }
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
     