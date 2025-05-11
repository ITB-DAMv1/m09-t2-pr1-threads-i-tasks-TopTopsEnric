namespace Sopar_dels_reis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Simulació del sopar dels filòsofs xinesos");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Prem qualsevol tecla per començar...");
            Console.ReadKey();
            Console.Clear();

            // Crear i iniciar la simulació
            Simulacio simulacio = new Simulacio(5, 30000);
            simulacio.Iniciar();

            Console.WriteLine("\nPrem qualsevol tecla per sortir...");
            Console.ReadKey();
        }
    }
}