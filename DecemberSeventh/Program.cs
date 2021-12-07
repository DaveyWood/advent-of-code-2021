var watch = new System.Diagnostics.Stopwatch();
for (var run = 0; run < 5; run++)
{
  // initial approach, with sampling
  var distances = File.ReadAllText("input").Split(',').Select(int.Parse).ToArray();
  watch.Restart();

  var min = distances.Min();
  var max = distances.Max();
  var middle = (min + max)/2;
  
  var sampleCount = Convert.ToInt32(Math.Sqrt(distances.Length));

  IEnumerable<int> GetSamples(int total, int sampleCount)
  {
    var increment = total/sampleCount;
    for (var i = 1; i < sampleCount; i++)
    {
      yield return i * increment;
    }
  }

  var samples = GetSamples(distances.Length, sampleCount);

  int GetTotalFuel(int position, int target)
  {
    var distance = Math.Abs(position - target);
    var cost = (distance * (distance + 1)) / 2;
    return cost;
  }

  int GetAnswer(int startPosition, int currentFuel, bool goingUp)
  {
    if (null == distances)
    {
      throw new Exception("distances cannot be null");
    }
    var nextIndex = goingUp ? startPosition + 1 : startPosition - 1;

    var nextFuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, nextIndex));

    if (nextFuel > currentFuel)
    {
      return currentFuel;
    }
    return GetAnswer(nextIndex, nextFuel, goingUp);
  }

  // find start
  // because values closest to the minimum are lowest
  // we can save time by starting at the lowest sample
  // but any start point can get us the correct answer
  var startIndex = samples.Select( s => 
    new { fuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, s)), s }).OrderBy(s => s.fuel)
    .First().s;

  var primeTime = watch.Elapsed.TotalMilliseconds;
  var middleFuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, startIndex));
  var upFuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, startIndex + 1));
  var setupTime = watch.Elapsed.TotalMilliseconds;
  var answer = upFuel < middleFuel ? GetAnswer(startIndex + 1, upFuel, true) : GetAnswer(startIndex, middleFuel, false);
  var time = watch.Elapsed.TotalMilliseconds;
  Console.WriteLine(new { approach = "Sampling", answer, time, size = distances.Length });

  // binary search
  var distances2 = File.ReadAllText("input").Split(',').Select(int.Parse).ToArray();
  watch.Restart();

  var min2 = distances.Min();
  var max2 = distances.Max();
  var middle2 = (min + max)/2;
  
  int SearchForAnswer(int[] distances, int startIndex, int endIndex)
  {
    if (endIndex - startIndex < 3)
    {
      var min = int.MaxValue;
      for (var i = startIndex; i <= endIndex; i++)
      {
        var fuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, i));
        if (fuel < min)
        {
          min = fuel;
        }
      }
      return min;
    }

    var middleIndex = (startIndex + endIndex)/2;
    var middleFuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, middleIndex));
    var nextFuel = distances.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, middleIndex + 1));

    if (nextFuel > middleFuel)
    {
      return SearchForAnswer(distances, startIndex, middleIndex);
    }
    else
    {
      return SearchForAnswer(distances, middleIndex + 1, endIndex);
    }
  }

  time = watch.Elapsed.TotalMilliseconds;
  answer = SearchForAnswer(distances2, min2, max2);
  Console.WriteLine(new { approach = "Binary Search", answer, time, size = distances.Length });

  // solution from https://github.com/jjcomer/advent-2021/blob/main/src/day07.rs to compare speed
  // pub fn solve_part2_stats(input: &Vec<i32>) -> i32 {
  //     let distance = input.iter().sum::<i32>().div(input.len() as i32);
  //     input
  //         .iter()
  //         .map(|d| compute_fuel((distance - d).abs()))
  //         .sum()
  // }

  var distances3 = File.ReadAllText("input").Split(',').Select(int.Parse).ToArray();
  watch.Restart();

  var averageDistance = distances3.Sum() / distances3.Length;
  time = watch.Elapsed.TotalMilliseconds;
  answer = distances3.Aggregate(0, (fuel, distance) => fuel + GetTotalFuel(distance, averageDistance));
  Console.WriteLine(new { approach = "Stats", answer, time, size = distances.Length });
}