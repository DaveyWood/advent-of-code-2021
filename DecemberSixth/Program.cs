var watch = new System.Diagnostics.Stopwatch();
// multiple runs to measure times
for (var run = 0; run < 5; run++)
{
  watch.Restart();
  var school = File.ReadAllText("input").Split(',').Select(num => new LanternFish(int.Parse(num))).ToList();

  var days = 80;

  for (var i = 0; i < days; i++)
  {
    var newFish = new List<LanternFish>();
    foreach(var fish in school)
    {
      var result = fish.Tick();
      if (result != null)
      {
        newFish.Add(result);
      }
    }
    school.AddRange(newFish);
  }
  var part1Time = watch.Elapsed.TotalMilliseconds;
  Console.WriteLine(new { part1Count = school.Count, part1Time });


  watch.Restart();
  var initialTimerToFinalCountLookup = new Dictionary<int, long>();

  int part2Days = 256;
  long part2Count = 0;

  var newSchool = File.ReadAllText("input").Split(',').Select(int.Parse).ToList();

  // gets the number of fish you end up with if you create a fish with x days remaing
  long GetResultByDaysLeft(int days)
  {
    if (initialTimerToFinalCountLookup.ContainsKey(days))
    {
      return initialTimerToFinalCountLookup[days];
    }

    long count = 0;

    var list = new List<LanternFish>{new LanternFish(8)};
    for (var i = 0; i < days; i++)
    {
      var newFish = new List<LanternFish>();
      foreach(var fish in list)
      {
        var result = fish.Tick();
        if (result != null)
        {
          // newFish.Add(result);
          var remainingDays = days - i - 1;
          count += GetResultByDaysLeft(remainingDays);
        }
      }
      list.AddRange(newFish);
    }
    count += list.Count;
    initialTimerToFinalCountLookup[days] = count;
    return count;
  }

  // prime the cache
  for (var i = 0; i < part2Days; i++)
  {
    GetResultByDaysLeft(i);
  }

  foreach(var age in newSchool)
  {
    var firstCreateDay = part2Days + 8 - age;
    part2Count += GetResultByDaysLeft(firstCreateDay);
  }
  // part2Count += newSchool.Count;

  // foreach (var pair in initialTimerToFinalCountLookup)
  // {
  //   Console.WriteLine(new { pair.Key, pair.Value });
  // }


  // TOO SLOW
  // foreach (var age in newSchool)
  // {
  //   if (initialTimerToFinalCountLookup.ContainsKey(age))
  //   {
  //     part2Count += initialTimerToFinalCountLookup[age];
  //   }
  //   else
  //   {
  //     var tempSchool = new List<LanternFish> { new LanternFish(age) };
  //     for (var i = 0; i < part2Days; i++)
  //     {
  //       var newFish = new List<LanternFish>();
  //       foreach(var fish in tempSchool)
  //       {
  //         var result = fish.Tick();
  //         if (result != null)
  //         {
  //           newFish.Add(result);
  //         }
  //       }
  //       tempSchool.AddRange(newFish);
  //     }
  //     part2Count += tempSchool.Count;
  //     initialTimerToFinalCountLookup[age] = tempSchool.Count;
  //   }
  // }
  var part2Time = watch.Elapsed.TotalMilliseconds;
  Console.WriteLine(new { part2Count, part2Time });

}
class LanternFish
{
  private int timer;

  public LanternFish(int timer)
  {
    this.timer = timer;
  }

  public LanternFish? Tick()
  {
    if (timer > 0)
    {
      timer = timer - 1;
      return null;
    }
    timer = 6;
    return new LanternFish(8);
  }
}