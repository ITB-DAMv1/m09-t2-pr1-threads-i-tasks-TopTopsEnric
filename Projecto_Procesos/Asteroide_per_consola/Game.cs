using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroide_per_consola
{
    class Game
    {
        // Configuració del joc
        private const int UpdateFrequency = 50;  // 50Hz per actualitzar la lògica
        private const int RenderFrequency = 20;  // 20Hz per repintar la consola

        // Elements del joc
        private Ship ship;
        private List<Asteroid> asteroids;

        // Estat del joc
        private bool isRunning;
        private bool webEvaluationFinished;
        private readonly Random random;
        private readonly object stateLock = new object();
        private readonly object renderLock = new object();

        // Estadístiques
        public int AsteroidsDodged { get; private set; }
        public int LivesLost { get; private set; }
        public TimeSpan TotalTime { get; private set; }

        // Temporitzadors i control de temps
        private Stopwatch gameTimer;
        private DateTime lastAsteroidSpawn;
        private double asteroidSpawnRate;

        public Game()
        {
            random = new Random();
            asteroids = new List<Asteroid>();
            ship = new Ship();
            gameTimer = new Stopwatch();
            lastAsteroidSpawn = DateTime.Now;
            asteroidSpawnRate = 0.3; // Segons entre generació d'asteroides
        }

        public async Task StartAsync()
        {
            Reset();

            isRunning = true;
            gameTimer.Start();

            // Tasques principals
            Task webEvalTask = WebEvaluation();
            Task updateTask = UpdateLoop();
            Task renderTask = RenderLoop();
            Task inputTask = HandleInput();

            // Esperar que totes les tasques finalitzin
            await Task.WhenAll(webEvalTask, updateTask, renderTask, inputTask);

            gameTimer.Stop();
            TotalTime = gameTimer.Elapsed;
        }

        private void Reset()
        {
            lock (stateLock)
            {
                ship = new Ship();
                asteroids.Clear();
                isRunning = true;
                webEvaluationFinished = false;
            }
        }

        private async Task WebEvaluation()
        {
            // Simular l'avaluació de webs (duració aleatòria entre 30s i 1min)
            int evaluationTime = random.Next(30000, 60001);
            await Task.Delay(evaluationTime);

            lock (stateLock)
            {
                webEvaluationFinished = true;
                isRunning = false;
            }
        }

        private async Task UpdateLoop()
        {
            int updateDelay = 1000 / UpdateFrequency;

            while (true)
            {
                lock (stateLock)
                {
                    if (!isRunning)
                        break;
                }

                Update();
                await Task.Delay(updateDelay);
            }
        }

        private async Task RenderLoop()
        {
            int renderDelay = 1000 / RenderFrequency;

            while (true)
            {
                lock (stateLock)
                {
                    if (!isRunning)
                        break;
                }

                Render();
                await Task.Delay(renderDelay);
            }
        }

        private void Update()
        {
            lock (stateLock)
            {
                // Generar nous asteroides
                if ((DateTime.Now - lastAsteroidSpawn).TotalSeconds >= asteroidSpawnRate)
                {
                    lastAsteroidSpawn = DateTime.Now;

                    // Ajustar la dificultat amb el temps
                    asteroidSpawnRate = Math.Max(0.2, 0.7 - (gameTimer.Elapsed.TotalSeconds * 0.001));

                    int x = random.Next(0, Console.WindowWidth);
                    asteroids.Add(new Asteroid(x, 0));
                }

                // Actualitzar posicions dels asteroides
                List<Asteroid> asteroidsToRemove = new List<Asteroid>();

                foreach (var asteroid in asteroids)
                {
                    asteroid.Update();

                    // Comprovar si l'asteroide ha sortit de la pantalla
                    if (asteroid.Y >= Console.WindowHeight)
                    {
                        asteroidsToRemove.Add(asteroid);
                        AsteroidsDodged++;
                    }

                    // Comprovar col·lisió amb la nau
                    if (asteroid.X == ship.X && asteroid.Y == ship.Y)
                    {
                        LivesLost++;
                        Reset();
                        return;
                    }
                }

                // Eliminar asteroides que han sortit de la pantalla
                foreach (var asteroid in asteroidsToRemove)
                {
                    asteroids.Remove(asteroid);
                }
            }
        }

        private void Render()
        {
            lock (renderLock)
            {
                Console.Clear();

                // Mostrar informació
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Temps: {gameTimer.Elapsed.TotalSeconds:F1}s | Esquivats: {AsteroidsDodged} | Vides: {LivesLost}");

                lock (stateLock)
                {
                    // Dibuixar la nau
                    Console.SetCursorPosition(ship.X, ship.Y);
                    Console.Write("^");

                    // Dibuixar asteroides
                    foreach (var asteroid in asteroids)
                    {
                        if (asteroid.Y < Console.WindowHeight && asteroid.X < Console.WindowWidth)
                        {
                            Console.SetCursorPosition(asteroid.X, asteroid.Y);
                            Console.Write("*");
                        }
                    }
                }
            }
        }

        private async Task HandleInput()
        {
            while (true)
            {
                lock (stateLock)
                {
                    if (!isRunning)
                        break;
                }

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    lock (stateLock)
                    {
                        switch (key)
                        {
                            case ConsoleKey.A:
                                ship.MoveLeft();
                                break;
                            case ConsoleKey.D:
                                ship.MoveRight();
                                break;
                            case ConsoleKey.Q:
                                isRunning = false;
                                break;
                        }
                    }
                }

                await Task.Delay(10); 
            }
        }
    }

}
