var watch = new System.Diagnostics.Stopwatch();

// run in loop to get times
var results = new List<dynamic>();
for (var runs = 0; runs < 20; runs++)
{  
  watch.Reset();
  watch.Start();
  var data = File.ReadLines("input").GetEnumerator();

  var readFileTime = watch.Elapsed.TotalMilliseconds;

  data.MoveNext();
  var numbersToCall = ParsedNumbers(data.Current);

  IEnumerable<int> ParsedNumbers(string commaSeperated)
  {
    int buffer = 0;
    for (int i = 0; i < commaSeperated.Length; i++)
    {
      if (commaSeperated[i] == ',')
      {
        yield return buffer;
        buffer = 0;
      }
      else
      { 
        buffer = buffer * 10 + commaSeperated[i] - '0';
      }
    }
  }

  var boards = new List<BingoBoard>();
  var listCreatedTime = watch.Elapsed.TotalMilliseconds;

  while(data.MoveNext())
  {
    if (data.Current == "")
    {
      continue;
    }
    var boardNumbers = new Space[5, 5];
    // the next five lines are the board
    for (var j = 0; j < 5; j++)
    {
      var lineNumbers = data.Current.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      for (var k = 0; k < 5; k++)
      {
        boardNumbers[j, k] = new Space { called = false, number = int.Parse(lineNumbers[k]) };
      }
      data.MoveNext();
    }
    boards.Add(new BingoBoard(boardNumbers));
  }

  var boardsBuiltTime = watch.Elapsed.TotalMilliseconds;

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
        }
        if (copyOfBoards.Count == 1)
        {
          lastBoardScore = board.SumOfUnmarked * number;
        }
        boardsToRemove.Add(board);
      }
    }
    boardsToRemove.ForEach(b => copyOfBoards.Remove(b));
    if (copyOfBoards.Count == 0)
    {
      break;
    }
  }
  var doneTime = watch.Elapsed.TotalMilliseconds;
  watch.Stop();

  if (runs > 1)
  {
    results.Add(new { winningScore, lastBoardScore, readFileTime, listCreatedTime, boardsBuiltTime, doneTime });
  }
}

// print results
var aggregates = new { results[0].winningScore, results[0].lastBoardScore, 
  readFileTime = new { min = results.Select(r => r.readFileTime).Min(),
                       max = results.Select(r => r.readFileTime).Max() }, 
  listCreatedTime = new { min = results.Select(r => r.listCreatedTime).Min(),
                       max = results.Select(r => r.listCreatedTime).Max() }, 
  boardsBuiltTime = new { min = results.Select(r => r.boardsBuiltTime).Min(),
                       max = results.Select(r => r.boardsBuiltTime).Max() }, 
  doneTime = new { min = results.Select(r => r.doneTime).Min(),
                       max = results.Select(r => r.doneTime).Max() }
 };

 Console.WriteLine(aggregates);
public struct Space
{
  public bool called;
  public int number;

  public override string ToString()
  {
    return number + " " + (called ? "True  " : "False ");
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
