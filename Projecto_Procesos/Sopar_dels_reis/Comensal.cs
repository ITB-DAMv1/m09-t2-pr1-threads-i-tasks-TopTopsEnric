using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sopar_dels_reis
{
    enum EstatComensal
    {
        Pensant,
        AgafantPalet,
        Menjant,
        DeixantPalet
    }
    class Comensal
    {
        private readonly int id;
        private readonly Palet paletEsquerra;
        private readonly Palet paletDreta;
        private readonly Registre registre;
        private readonly Random random;
        private DateTime ultimCopMenjat;
        private int vegadesMenjat;
        private double tempsMaximFam;

        public Comensal(int id, Palet paletEsquerra, Palet paletDreta, Registre registre)
        {
            this.id = id;
            this.paletEsquerra = paletEsquerra;
            this.paletDreta = paletDreta;
            this.registre = registre;
            this.random = new Random(id * 100 + Environment.TickCount);
            this.ultimCopMenjat = DateTime.Now;
            this.vegadesMenjat = 0;
            this.tempsMaximFam = 0;
        }

        public void Executar(CancellationToken token, ref bool simulacioActiva)
        {
            // Estratègia per evitar deadlock: agafar sempre el palet de menor índex primer
            Palet primerPalet = paletEsquerra.Id < paletDreta.Id ? paletEsquerra : paletDreta;
            Palet segonPalet = paletEsquerra.Id < paletDreta.Id ? paletDreta : paletEsquerra;

            while (simulacioActiva && !token.IsCancellationRequested)
            {
                try
                {
                    // Pensar
                    registre.RegistrarEstat(id, $"Comensal {id} està pensant...", EstatComensal.Pensant);
                    Thread.Sleep(random.Next(500, 2001));

                    DateTime iniciGana = DateTime.Now;

                    // Intentar agafar el primer palet
                    registre.RegistrarEstat(id, $"Comensal {id} intenta agafar palet {primerPalet.Id}...", EstatComensal.AgafantPalet);
                    lock (primerPalet.Bloqueig)
                    {
                        registre.RegistrarEstat(id, $"Comensal {id} ha agafat palet {primerPalet.Id}", EstatComensal.AgafantPalet);

                        // Intentar agafar el segon palet
                        registre.RegistrarEstat(id, $"Comensal {id} intenta agafar palet {segonPalet.Id}...", EstatComensal.AgafantPalet);
                        lock (segonPalet.Bloqueig)
                        {
                            registre.RegistrarEstat(id, $"Comensal {id} ha agafat palet {segonPalet.Id}", EstatComensal.AgafantPalet);

                            // Calcular temps de fam
                            TimeSpan tempsSenseMenjar = DateTime.Now - ultimCopMenjat;
                            double tempsFam = tempsSenseMenjar.TotalSeconds;
                            tempsMaximFam = Math.Max(tempsMaximFam, tempsFam);

                            // Menjar
                            registre.RegistrarEstat(id, $"Comensal {id} està menjant", EstatComensal.Menjant);
                            Thread.Sleep(random.Next(500, 1001));
                            vegadesMenjat++;
                            ultimCopMenjat = DateTime.Now;

                            // Deixar el segon palet
                            registre.RegistrarEstat(id, $"Comensal {id} deixa palet {segonPalet.Id}", EstatComensal.DeixantPalet);
                        }

                        // Deixar el primer palet
                        registre.RegistrarEstat(id, $"Comensal {id} deixa palet {primerPalet.Id}", EstatComensal.DeixantPalet);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public double TempsSenseMenjar()
        {
            return (DateTime.Now - ultimCopMenjat).TotalSeconds;
        }

        public int VegadesMenjat => vegadesMenjat;
        public double TempsMaximFam => tempsMaximFam;
        public int Id => id;
    }
}
