
var caves = new Dictionary<string, Cave>();

var input = File.ReadLines("input");

foreach (var line in input)
{
  var caveNames = line.Split('-');
  var firstCaveName = caveNames[0];
  var secondCaveName = caveNames[1];

  ConnectCave(firstCaveName, secondCaveName);
  ConnectCave(secondCaveName, firstCaveName);
}

var start = caves["start"];
var end = caves["end"];

int FindPath(Cave location, HashSet<string> visited)
{
  if (caves == null)
  {
    throw new Exception("");
  }
  if (location == end)
  {
    return 1;
  }

  var newVisited = new HashSet<string>(visited);
  newVisited.Add(location.Name);

  return location.Neighbors
    // lookup the cave for each neighbor
    .Select(n => caves[n])
    // we can go there if we've never been there or it's big
    .Where(n => !newVisited.Contains(n.Name) || n.Big)
    .Select(location => FindPath(location, new HashSet<string>(newVisited))).Sum();
}

int FindPathPart2(Cave location, HashSet<string> visited, bool visitedASmallCaveTwice)
{
  if (caves == null)
  {
    throw new Exception("");
  }
  if (location == end)
  {
    return 1;
  }

  var newVisited = new HashSet<string>(visited);
  newVisited.Add(location.Name);


  var newAndUnvisted = location.Neighbors
    // lookup the cave for each neighbor
    .Select(n => caves[n])
    // we can go there if we've never been there or it's big
    .Where(n => !newVisited.Contains(n.Name) || n.Big)
    .Select(location => FindPathPart2(location, new HashSet<string>(newVisited), visitedASmallCaveTwice)).Sum();

  if (visitedASmallCaveTwice)
  {
    return newAndUnvisted;
  }

  var repeats = location.Neighbors
    // lookup the cave
    .Select(n => caves[n])
    // find small caves that aren't start or end, and we've visited
    .Where(c => !c.Big && c != start && c != end && newVisited.Contains(c.Name))
    // visit them, but no more small caves
    .Select(location => FindPathPart2(location, new HashSet<string>(newVisited), true)).Sum();
  return newAndUnvisted + repeats;
}

var answer = FindPath(start, new HashSet<string> { "start" });
var part2Answer = FindPathPart2(start, new HashSet<string> { "start " }, false);

Console.WriteLine(new { answer, part2Answer });

void ConnectCave(string caveName, string partner)
{
  if (caves == null)
  {
    throw new Exception();
  }
  var cave = caves.ContainsKey(caveName) ? caves[caveName] : new Cave(caveName);
  caves[caveName] = cave;
  cave.AddNeighbor(partner);
}

class Cave
{
  public string Name { get; set; }

  private readonly HashSet<string> _neighbors = new HashSet<string>();

  public bool Big
  {
    get
    {
      return Name == Name.ToUpper();
    }
  }

  public void AddNeighbor(string neighbor)
  {
    _neighbors.Add(neighbor);
  }

  public IEnumerable<string> Neighbors
  {
    get
    {
      return _neighbors;
    }
  }
  public Cave(string name)
  {
    this.Name = name;
  }
}