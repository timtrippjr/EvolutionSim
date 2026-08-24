namespace EvolutionSim;

//generates kinda cool sounding names


public class NameGenerator
{
    private class Syllable
    {
        public string? Value { get; set; }
        public int Location { get; set; }
        public Syllable(string value, int location)
        {
            Value = value;
            Location = location;
        }
    }

    //end = 1, begin = 2, mid = 4
    private static Syllable[] _vowels =
    {
        new("a", 7), new("o", 7), new("i", 7), new("o", 7), new("u", 7), new("y", 1),
        
        // repeats
        new("a", 7), new("o", 7), new("i", 7), new("o", 7), new("u", 7), new("y", 1),
        new("a", 7), new("o", 7), new("i", 7), new("o", 7), new("u", 7), new("y", 1),
        new("a", 7), new("o", 7), new("i", 7), new("o", 7), new("u", 7), new("y", 1),
        new("a", 7), new("o", 7), new("i", 7), new("o", 7), new("u", 7), new("y", 1),

        // biletter
        new("oi", 5), new("ou", 5), 
        new("ie", 5), new("ai", 5),
    };
    private static Syllable[] _consonants =
    {
        new("c", 7), new("d", 7), new("b", 7), new("p", 7), new("m", 7),
        new("n", 7), new("l", 7), new("f", 7), new("t", 7), new("j", 7),
        new("r", 7), new("g", 7), new("v", 7), new("w", 7),
        
        //repeats
        new("c", 7), new("d", 7), new("b", 7), new("p", 7), new("m", 7),
        new("n", 7), new("l", 7), new("f", 7), new("t", 7), new("j", 7),
        new("r", 7), new("g", 7), new("v", 7), new("w", 7),
        new("c", 7), new("d", 7), new("b", 7), new("p", 7), new("m", 7),
        new("n", 7), new("l", 7), new("f", 7), new("t", 7), new("j", 7),
        new("r", 7), new("g", 7), new("v", 7), new("w", 7),
        /////
        


        new("sh", 7), new("ch", 7), 
        
        new("dr", 6), 
        new("sl", 6), new("br", 6), 

        new("nd", 5), new("lt", 5), new("mb", 5), new("nf", 5), new("mf", 5),

        new("zz", 4), new("ll", 4),

        new("k", 3), 
        new("th", 2), 
        
        new("hn", 1), new("ck", 1), new("nk", 1), new("x", 1), new("rd", 1)
    };

    public static string GetRandomName(int minsyl, int maxsyl)
    {
        string result = "";

        int leng = Rng.Next(minsyl, maxsyl);
        bool isvowel = Rng.Next(0, 1) != 0;

        Syllable syll;
        for (int i = 1; i <= leng; i++)
        {
            do
            {
                if (isvowel) 
                    syll = _vowels[Rng.Next(_vowels.Length)];
                else
                    syll = _consonants[Rng.Next(0, _consonants.Length)];
                
                int loc = syll.Location;
                if (
                    (i == 1 && (loc & 2) != 0) || 
                    (i == leng && (loc & 1) != 0) || 
                    (i > 1 && i < leng && (loc & 4) != 0)
                ) break;
            }while(true);

            result += syll.Value;
            isvowel = !isvowel;
        }

        return  char.ToUpper(result[0]) + result[1..];
    }

    public static string GetRandomName(int syll)
    {
        return GetRandomName(syll, syll);
    }
}