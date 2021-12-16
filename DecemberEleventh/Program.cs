
var data = File.ReadLines("input").GetEnumerator();

var grid = new Octopus[10, 10];

for (var row = 0; row < 10; row++)
{
  data.MoveNext();
  var line = data.Current;
  for (var column = 0; column < 10 && line != null; column++)
  {
    grid[row, column] = new Octopus { energy = line[column] - '0' };
  }
}

var answer = 0;
var allFlashedTurn = -1;
for (var run = 1; run <= 100 || allFlashedTurn == -1; run++)
{
  var newFlashes = RunTurn();
  if (run <= 100)
  {
    answer += newFlashes;
  }
  if (allFlashedTurn == -1 && newFlashes == 100)
  {
    allFlashedTurn = run;
  }
  WriteGrid(grid);
}

Console.WriteLine(new { answer, allFlashedTurn });


void WriteGrid(Octopus[,] grid)
{
  Console.WriteLine("");
  for (var row = 0; row < 10; row++)
  {
    for (var column = 0; column < 10; column++)
    {
      Console.Write(grid[row, column].energy % 10);
    }
    Console.WriteLine("");
  }
  Console.WriteLine("");
}


int RunTurn()
{
  if (grid == null)
  {
    throw new Exception("");
  }
  var flashes = 0;
  // increment everything
  for (var row = 0; row < 10; row++)
  {
    for (var column = 0; column < 10; column++)
    {
      var octopus = grid[row, column];
      octopus.energy += 1;
      grid[row, column] = octopus;
    }
  }

  // flash as needed
  for (var row = 0; row < 10; row++)
  {
    for (var column = 0; column < 10; column++)
    {
      Flash(row, column);
    }
  }

  // count flashes and reset
  for (var row = 0; row < 10; row++)
  {
    for (var column = 0; column < 10; column++)
    {
      var octopus = grid[row, column];
      if (octopus.flashed)
      {
        flashes += 1;
        grid[row, column] = new Octopus { energy = 0, flashed = false };
      }
    }
  }

  return flashes;
}

void Flash(int row, int column)
{
  if (grid == null)
  {
    throw new Exception("");
  }

  var octopus = grid[row, column];
  if (octopus.flashed || octopus.energy <= 9)
  {
    return;
  }

  octopus.flashed = true;
  grid[row, column] = octopus;

  var rowStart = row == 0 ? 0 : row - 1;
  var rowEnd = row == 9 ? 9 : row + 1;

  var columnStart = column == 0 ? 0 : column - 1;
  var columnEnd = column == 9 ? 9 : column + 1;

  for (var r = rowStart; r <= rowEnd; r++)
  {
    for (var c = columnStart; c <= columnEnd; c++)
    {
      if (row == r && c == column)
      {
        continue;
      }
      var neighbor = grid[r, c];
      neighbor.energy += 1;
      grid[r, c] = neighbor;
      Flash(r, c);
    }
  }
}

struct Octopus
{
  // the current energy
  public int energy;
  
  // whether this octopus flashed this turn
  // it's probably possible to solve the problem without tracking this
  public bool flashed;
}