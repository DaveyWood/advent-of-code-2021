
int MostCommonByPosition(IEnumerable<string> inputs, int index, int tie)
{
  var zeroes = 0;
  var ones = 0;
  foreach (var str in inputs)
  {
    if (str[index] == '1')
    {
      ones++;
    }
    else
    {
      zeroes++;
    }
  }

  if (ones == zeroes)
  {
    return tie;
  }
  return ones < zeroes ? 0 : 1;
}

var data = File.ReadAllLines("input");

var bits = new int[data[0].Length];

for (int i = 0; i < bits.Length; i++)
{
  bits[i] = MostCommonByPosition(data, i, 1);
}

var gamma = GetDecimal(bits);
Console.WriteLine("gamma " + gamma);

var epsilonBits = bits.Select(b => b == 1 ? 0 : 1).ToArray();

var epsilon = GetDecimal(epsilonBits);

var power = gamma * epsilon;

Console.WriteLine("power: " + power);
// 693486

int GetDecimal(int[] bits)
{
  var value = 0;
  for (var i = 0; i < bits.Length; i++)
  {
    var power = bits.Length - 1 - i;
    var mult = Math.Pow(2, power);
    value += bits[i] * (int)mult;
  }

  return value;
}

int GetDecimalFromString(string s)
{
  return GetDecimal(s.Select(c => int.Parse(c.ToString())).ToArray());
}


// oxygen rating
// keep most common value, defaulting to one

var values = new List<string>();
values.AddRange(data);

for (int i = 0; i < values[0].Length && values.Count > 1; i++)
{
  bits[i] = MostCommonByPosition(values, i, 1);
  values = values.Where(v => int.Parse(v[i].ToString()) == bits[i]).ToList();
}

var oxygen = GetDecimalFromString(values[0]);

values = new List<string>();
values.AddRange(data);

for (int i = 0; i < values[0].Length && values.Count > 1; i++)
{
  bits[i] = MostCommonByPosition(values, i, 1) == 1 ? 0 : 1;
  values = values.Where(v => int.Parse(v[i].ToString()) == bits[i]).ToList(); 
}

var co2 = GetDecimalFromString(values[0]);



Console.WriteLine(new { oxygen, co2 });

Console.WriteLine(oxygen * co2);
// 3379326

