var input = File.ReadAllLines("input");

var dots = new HashSet<(int x, int y)>();

var populatingDots = true;
var part1Answer = 0;

foreach(var line in input)
{
  if (line == "")
  {
    populatingDots = false;
    continue;
  }
  if (populatingDots)
  {
    var newDot = line.Split(',').Select(int.Parse).ToArray();
    dots.Add((newDot[0], newDot[1]));
  }
  else
  {
    // it's a move like "fold along y=7"
    var move = line.Split(' ')[2];
    var subMove = move.Split('=');
      var foldIndex = int.Parse(subMove[1]);
    if (subMove[0] == "y")
    {
      dots = dots.Select(d => d.y < foldIndex ? d : (d.x, foldIndex - (d.y - foldIndex))).ToHashSet();
    }
    else
    {
      dots = dots.Select(d => d.x < foldIndex ? d : (foldIndex - (d.x - foldIndex), d.y)).ToHashSet();
    }
    if (part1Answer == 0)
    {
      part1Answer = dots.Count;
    }
  }
}

// find part2 answer
var width = dots.Select(d => d.x).Max();
var height = dots.Select(d => d.y).Max();

var text = new bool[width + 1, height + 1];

foreach(var dot in dots)
{
  text[dot.x, dot.y] = true;
}

for (var y = 0; y <= height; y++)
{
  for (var x = 0; x <= width; x++)
  {
    Console.Write(text[x, y] ? "*" : " ");
  }
  Console.WriteLine();
}


Console.WriteLine(new { part1Answer });