using Asteroide_per_consola;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.CursorVisible = false;
            Console.Clear();

            Console.WriteLine("Benvingut a Asteroids Console Game!");
            Console.WriteLine("Controls: A = Esquerra, D = Dreta, Q = Sortir");
            Console.WriteLine("Pressiona qualsevol tecla per començar...");
            Console.ReadKey(true);

            Console.Clear();

            // Iniciar el joc
            Game game = new Game();
            await game.StartAsync();

            // Mostrar resultats finals
            Console.Clear();
            Console.WriteLine("┌─────────────────────────────────────────┐");
            Console.WriteLine("│            RESULTATS FINALS             │");
            Console.WriteLine("├─────────────────────────────────────────┤");
            Console.WriteLine($"│ Temps total: {game.TotalTime.TotalSeconds:F2} segons{new string(' ', 18 - game.TotalTime.TotalSeconds.ToString("F2").Length)}│");
            Console.WriteLine($"│ Asteroides esquivats: {game.AsteroidsDodged}{new string(' ', 19 - game.AsteroidsDodged.ToString().Length)}│");
            Console.WriteLine($"│ Vides gastades: {game.LivesLost}{new string(' ', 23 - game.LivesLost.ToString().Length)}│");
            Console.WriteLine("└─────────────────────────────────────────┘");

            Console.WriteLine("\nPressiona qualsevol tecla per sortir...");
            Console.ReadKey(true);  // True fa que no es mostri la tecla premuda
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine($"Error en el joc: {ex.Message}");
            Console.WriteLine("\nPressiona qualsevol tecla per sortir...");
            Console.ReadKey(true);
        }
    }
}