Console.WriteLine("Hello, World!");

string? theStupidestPerson = Console.ReadLine(); 

bool reachedFailLine = false;
if (theStupidestPerson is null)
reachedFailLine = true;
if (theStupidestPerson == string.Empty)
reachedFailLine = true;


if (reachedFailLine)
Console.WriteLine("im feeling facty today.");
else
Console.WriteLine("wow! i HATE "+theStupidestPerson);


Console.ReadKey(); 