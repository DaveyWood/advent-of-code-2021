// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var numbers = File.ReadAllLines("./input").Select(num => int.Parse(num)).ToArray();

var answer = 0;

Console.WriteLine(numbers.Length);

for(var i = 3; i < numbers.Length; i++)
{
  if (numbers[i] > numbers[i-3])
  {
    answer++;
  }
}

Console.WriteLine(answer);
