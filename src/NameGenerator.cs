namespace EvolutionSim;

//generates kinda cool sounding names


public class NameGenerator
{
    //end = 1, begin = 2, mid = 4
    private static (string, int)[] _vowels =
    {
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        
        // repeats
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),
        ("a", 7), ("o", 7), ("i", 7), ("o", 7), ("u", 7), ("y", 1),

        // biletter
        ("oi", 5), ("ou", 5), 
        ("ie", 5), ("ai", 5),
    };
    private static (string, int)[] _consonants =
    {
        ("c", 7), ("d", 7), ("b", 7), ("p", 7), ("m", 7), ("n", 7), ("l", 7), 
        ("f", 7), ("t", 7), ("j", 7), ("r", 7), ("g", 7), ("v", 7), ("w", 7),
        
        //repeats
        ("c", 7), ("d", 7), ("b", 7), ("p", 7), ("m", 7), ("n", 7), ("l", 7), 
        ("f", 7), ("t", 7), ("j", 7), ("r", 7), ("g", 7), ("v", 7), ("w", 7),
        ("c", 7), ("d", 7), ("b", 7), ("p", 7), ("m", 7), ("n", 7), ("l", 7), 
        ("f", 7), ("t", 7), ("j", 7), ("r", 7), ("g", 7), ("v", 7), ("w", 7),
        ("c", 7), ("d", 7), ("b", 7), ("p", 7), ("m", 7), ("n", 7), ("l", 7), 
        ("f", 7), ("t", 7), ("j", 7), ("r", 7), ("g", 7), ("v", 7), ("w", 7),
        ("c", 7), ("d", 7), ("b", 7), ("p", 7), ("m", 7), ("n", 7), ("l", 7), 
        ("f", 7), ("t", 7), ("j", 7), ("r", 7), ("g", 7), ("v", 7), ("w", 7),
        /////


        ("sh", 7), ("ch", 7), ("ph", 7),
        
        ("dr", 6), ("qu", 6), ("pl", 6),
        ("sl", 6), ("br", 6), 

        ("nd", 5), ("lt", 5), ("mb", 5), ("nf", 5), ("mf", 5),
        ("lk", 5), ("lc", 5), ("sc", 5), ("nt", 5),

        ("zz", 4), ("ll", 4),

        ("k", 3), 
        ("th", 2), 
        
        ("hn", 1), ("ck", 1), ("nk", 1), ("x", 1), ("rd", 1)
    };

    public static string GetRandomName(int minsyl, int maxsyl)
    {
        string result = "";

        int leng = Rng.Next(minsyl, maxsyl);
        bool isVowel = Rng.Next(0, 1) != 0;

        (string Value, int Location) syll;
        for (int i = 1; i <= leng; i++)
        {
            do
            {
                if (isVowel) 
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
            isVowel = !isVowel;
        }

        return char.ToUpper(result[0]) + result[1..];
    }

    public static string GetRandomName(int syll)
    {
        return GetRandomName(syll, syll);
    }
}