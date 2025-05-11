using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sopar_dels_reis
{
    class Simulacio
    {
        private readonly int numComensals;
        private readonly int tempsSimulacio;
        private readonly Palet[] palets;
        private readonly Comensal[] comensals;
        private readonly Registre registre;
        private readonly Stopwatch stopwatch;
        private CancellationTokenSource cts;
        private bool simulacioActiva;

        public Simulacio(int numComensals, int tempsSimulacio)
        {
            this.numComensals = numComensals;
            this.tempsSimulacio = tempsSimulacio;
            palets = new Palet[numComensals];
            stopwatch = new Stopwatch();
            registre = new Registre(numComensals, stopwatch);
            simulacioActiva = true;

            // Inicialitzar palets
            for (int i = 0; i < numComensals; i++)
            {
                palets[i] = new Palet(i);
            }

            // Inicialitzar comensals
            comensals = new Comensal[numComensals];
            for (int i = 0; i < numComensals; i++)
            {
                int paletEsquerra = i;
                int paletDreta = (i + 1) % numComensals;
                comensals[i] = new Comensal(i, palets[paletEsquerra], palets[paletDreta], registre);
            }
        }

        public void Iniciar()
        {
            stopwatch.Start();
            cts = new CancellationTokenSource();

            // Iniciar els fils dels comensals
            Task[] tasques = new Task[numComensals];
            for (int i = 0; i < numComensals; i++)
            {
                int id = i;
                tasques[i] = Task.Run(() => comensals[id].Executar(cts.Token, ref simulacioActiva));
            }

            // Tasca per comprovar si algun comensal passa massa temps sense menjar
            Task supervisorFam = Task.Run(() => SupervisorFam());

            // Aturar simulació després del temps especificat
            Task.Delay(tempsSimulacio).Wait();

            simulacioActiva = false;
            cts.Cancel();

            try
            {
                Task.WaitAll(tasques);
            }
            catch (AggregateException)
            {
                // Ignorar excepcions de cancel·lació
            }

            stopwatch.Stop();

            // Recollir i mostrar estadístiques
            Dictionary<int, (int vegadesMenjat, double tempsMaximFam)> estadistiques = new Dictionary<int, (int, double)>();
            for (int i = 0; i < numComensals; i++)
            {
                estadistiques.Add(i, (comensals[i].VegadesMenjat, comensals[i].TempsMaximFam));
            }

            registre.MostrarEstadistiques(estadistiques);
            registre.GuardarEstadistiquesCSV(estadistiques);
        }

        private void SupervisorFam()
        {
            while (simulacioActiva)
            {
                for (int i = 0; i < numComensals; i++)
                {
                    double tempsSenseMenjar = comensals[i].TempsSenseMenjar();
                    if (tempsSenseMenjar > 15)
                    {
                        registre.RegistrarAlerta(i, tempsSenseMenjar);
                        simulacioActiva = false;
                        cts.Cancel();
                        return;
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
