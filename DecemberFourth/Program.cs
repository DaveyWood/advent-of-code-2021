var watch = new System.Diagnostics.Stopwatch();
watch.Start();
var data = File.ReadAllLines("input");
Console.WriteLine($"File read at {watch.ElapsedMilliseconds}ms");

var numbersToCall = data[0].Split(',').Select(int.Parse).ToArray();

var boards = new List<BingoBoard>();

for (var i = 1; i < data.Length; i += 6)
{
  var boardNumbers = new Space[5, 5];
  // i is a blank line
  // the next five lines are the board
  for (var j = 0; j < 5; j++)
  {
    var lineNumbers = data[i + j + 1].Split(' ').Where(s => s.Length > 0).Select(int.Parse).ToArray();
    for (var k = 0; k < 5; k++)
    {
      boardNumbers[j, k] = new Space { called = false, number = lineNumbers[k] };
    }
  }

  boards.Add(new BingoBoard(boardNumbers));
}
Console.WriteLine($"Boards built at {watch.ElapsedMilliseconds}");

var winningScore = 0;
var lastBoardScore = 0;

var copyOfBoards = new List<BingoBoard>();
copyOfBoards.AddRange(boards);
foreach (var number in numbersToCall)
{
  var boardsToRemove = new List<BingoBoard>();
  foreach (var board in copyOfBoards)
  {
    if (board.CallNumber(number))
    {
      if (winningScore == 0)
      {
        winningScore = board.SumOfUnmarked * number;
        Console.WriteLine("winningScore: " + winningScore);
      }
      if (copyOfBoards.Count == 1)
      {
        lastBoardScore = board.SumOfUnmarked * number;
        Console.WriteLine("lastBoardScore: " + lastBoardScore);
      }
      boardsToRemove.Add(board);
      // Console.WriteLine("some board score: " + board.SumOfUnmarked * number);
      // board.Print();
    }
  }
  boardsToRemove.ForEach(b => copyOfBoards.Remove(b));
  if (copyOfBoards.Count == 0)
  {
    break;
  }
}

// boards.ForEach(b => b.Print());

Console.WriteLine($"Done at {watch.ElapsedMilliseconds}");

public struct Space
{
  public bool called;
  public int number;

  public override string ToString()
  {
    return number + " " + called + " ";
  }
}

public struct BingoBoard
{
  private readonly Space[,] _spaces;

  // look up a bingo number's index
  private readonly Dictionary<int, (int, int)> _lookup = new();
  public BingoBoard(Space[,] spaces)
  {
    _spaces = spaces;
    for (var i = 0; i < 5; i++)
    {
      for (var j = 0; j < 5; j++)
      {
        _lookup[_spaces[i, j].number] = (i, j);
      }
    }
  }

  private bool CheckVictory(int x, int y)
  {
    var allX = true;
    var allY = true;
    for (int i = 0; i < 5; i++)
    {
      allX = allX && _spaces[x, i].called;
      allY = allY && _spaces[i, y].called;
    }
    return allX || allY;
  }

  // returns whether or not the card wins
  public bool CallNumber(int number)
  {
    if (!_lookup.ContainsKey(number))
    {
      return false;
    }

    var index = _lookup[number];
    var space = _spaces[index.Item1, index.Item2];
    if (space.number != number)
    {
      throw new Exception();
    }
    // Console.WriteLine("found number " + space.number);
    space.called = true;
    _spaces[index.Item1, index.Item2] = space;

    return CheckVictory(index.Item1, index.Item2);
  }

  public int SumOfUnmarked
  {
    get
    {
      var sum = 0;
      for (var i = 0; i < 5; i++)
      {
        for (var j = 0; j < 5; j++)
        {
          var space = _spaces[i, j];
          if (!space.called)
          {
            sum += space.number;
          }
        }
      }
      // Console.WriteLine("sum: " + sum);
      return sum;
    }
  }

  public void Print()
  {
    var message = "";
    for (var i = 0; i < 5; i++)
    {
      var line = "";
      for (var j = 0; j < 5; j++)
      {
        line += _spaces[i, j].ToString();
      }
      message += line + Environment.NewLine;
    }

    Console.WriteLine(message);
  }

}
