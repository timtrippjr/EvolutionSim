namespace EvolutionSim;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        new Game().Begin(new TitleState());
    }
}