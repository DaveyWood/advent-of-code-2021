

var data = File.ReadAllLines("input").Select(line => line.Split(' '))
  .Select(pair => new { direction = pair[0], distance = int.Parse(pair[1])});

var depth = 0;
var distance = 0;
var aim = 0;

foreach (var datom in data)
{
  switch(datom.direction)
  {
    case "down":
    {
      aim += datom.distance;
      break;
    }
    case "up":
    {
      aim -= datom.distance;
      break;
    }
    case "forward":
    {
      distance += datom.distance;
      depth += (datom.distance * aim);
      break;
    }
    default:
    {
      throw new Exception("unknown direction " + datom.distance);
    }
  }
}

Console.WriteLine(depth * distance);

