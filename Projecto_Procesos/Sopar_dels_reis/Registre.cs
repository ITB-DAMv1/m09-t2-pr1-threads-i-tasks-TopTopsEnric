using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sopar_dels_reis
{
    class Registre
    {
        private readonly object lockConsole = new object();
        private readonly Stopwatch stopwatch;
        private readonly int numComensals;

        // Colors per als comensals
        private readonly ConsoleColor[] colors = {
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Red
        };

        // Colors per als estats
        private readonly ConsoleColor[] backColors = {
            ConsoleColor.DarkBlue,     // Pensant
            ConsoleColor.DarkRed,      // Agafant palet
            ConsoleColor.DarkGreen,    // Menjant
            ConsoleColor.DarkYellow    // Deixant palet
        };

        public Registre(int numComensals, Stopwatch stopwatch)
        {
            this.numComensals = numComensals;
            this.stopwatch = stopwatch;
        }

        public void RegistrarEstat(int idComensal, string missatge, EstatComensal estat)
        {
            lock (lockConsole)
            {
                Console.BackgroundColor = backColors[(int)estat];
                Console.ForegroundColor = colors[idComensal % colors.Length];
                Console.WriteLine($"[{stopwatch.Elapsed:mm\\:ss\\.fff}] {missatge}");
                Console.ResetColor();
            }
        }

        public void RegistrarAlerta(int idComensal, double tempsSenseMenjar)
        {
            lock (lockConsole)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n[{stopwatch.Elapsed:mm\\:ss\\.fff}] ALERTA: Comensal {idComensal} porta més de 15 segons sense menjar! ({tempsSenseMenjar:F2}s)");
                Console.ResetColor();
            }
        }

        public void MostrarEstadistiques(Dictionary<int, (int vegadesMenjat, double tempsMaximFam)> estadistiques)
        {
            Console.WriteLine("\n--- ESTADÍSTIQUES FINALS ---");
            Console.WriteLine("Comensal\tVegades Menjat\tTemps Màxim de Fam (s)");

            foreach (var entrada in estadistiques)
            {
                int id = entrada.Key;
                var (vegadesMenjat, tempsMaximFam) = entrada.Value;

                Console.ForegroundColor = colors[id % colors.Length];
                Console.WriteLine($"{id}\t\t{vegadesMenjat}\t\t{tempsMaximFam:F2}");
            }
            Console.ResetColor();
        }

        public void GuardarEstadistiquesCSV(Dictionary<int, (int vegadesMenjat, double tempsMaximFam)> estadistiques)
        {
            string ruta = "estadistiques_sopar.csv";
            using (StreamWriter sw = new StreamWriter(ruta))
            {
                sw.WriteLine("Numero_Comensal,Temps_Maxim_Fam,Vegades_Menjat");
                foreach (var entrada in estadistiques)
                {
                    int id = entrada.Key;
                    var (vegadesMenjat, tempsMaximFam) = entrada.Value;
                    sw.WriteLine($"{id},{tempsMaximFam:F2},{vegadesMenjat}");
                }
            }
            Console.WriteLine($"\nEstadístiques guardades a: {Path.GetFullPath(ruta)}");
        }
    }
}
