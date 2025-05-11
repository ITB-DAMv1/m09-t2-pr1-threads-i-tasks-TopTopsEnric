Enunciat 1: Problema dels Filòsofs Xinesos
Estructura del Projecte

El projecte s'estructura en varias classes:

    Palet: Representa cada recurs compartit amb un ID i un objecte de bloqueig.
    Comensal: Implementa la lògica de cada filòsof com un fil d'execució.
    Registre: Centralitza la visualització i emmagatzematge d'estadístiques.
    Simulacio: Orquestra tot el procés i gestiona els fils.
    EstatComensal: Enum que defineix els possibles estats(esta dins del script de comensal).

Com s'ha evitat interbloquejos i que ningú passés fam?

He implementat dues estratègies fonamentals:

    Prevenció de Deadlock (Interbloqueig):
        Estratègia del Recurs Jeràrquic: Cada filòsof sempre intenta agafar primer el palet amb ID més baix, independentment de si és el de la dreta o esquerra:

    Aquest enfocament trenca el problema circular que em pasaba. Succeix si tots els filòsofs agafessin primer el palet esquerre i després el dret.
    Si tots els comensals segueixen el mateix ordre jeràrquic per adquirir recursos, no es pot formar un cicle d'espera.

    Prevenció de Fam (Starvation):
        Supervisió Activa: Un fil específic (SupervisorFam) monitoritza constantment el temps que porta cada comensal sense menjar.
        Si detecta que algun comensal porta més de 15 segons sense menjar, atura la simulació.
    

Dades Esperables

En una execució normal de 30 segons:

    Comensals que han passat fam:
        Cap hauria de passar més de 15 segons sense menjar, ja que la simulació s'aturaria.
        El temps mitjà de fam el stats actuals s'esperaria que fos entre 3-5 segons de mitjana.

    Vegades que ha menjat cada comensal (mitjana):
        Amb el stats realitzats al programa s' esperaria que cada comensal mengés unes 8-12 vegades durant una simulació de 30 segons.

    Record de vegades que ha menjat un mateix comensal:
        15 vegades segons un recompte de 15 intents.

    Record de menys vegades que ha menjat un comensal:
        Amb una volta de 15 proves el menor numero de vegades era 6.

Enunciat 2: Joc Asteroids per Consola
Estructura del Projecte

El projecte s'organitza en diverses classes:

    Game: Gestiona el bucle principal del joc i la coordinació de tasques.
    Ship: Representa la nau del jugador amb la seva posició i moviments.
    Asteroid: Modela els asteroides amb la seva posició i moviment.
    Program: Inicialitza el joc i mostra resultats.

Com s'han executat les tasques simultàniament

He implementat un sistema de tasques paral·leles asíncrones utilitzant la classe Task i el patró async/await:
C#

public async Task StartAsync()
{
    // Inicialització
    // ...

    // Inici de tasques paral·leles
    Task webEvalTask = WebEvaluation();
    Task updateTask = UpdateLoop();
    Task renderTask = RenderLoop();
    Task inputTask = HandleInput();

    // Esperar que totes acabin
    await Task.WhenAll(webEvalTask, updateTask, renderTask, inputTask);

    // Finalització
    // ...
}

Diferenciació entre programació paral·lela i asíncrona

He aplicat un model híbrid que combina amb dues tècniques:

    Programació Paral·lela:
        S'executen quatre tasques simultàniament que realitzen funcions diferents.
        Cada tasca opera independentment i en paral·lel amb les altres.
        La sincronització entre tasques es gestiona amb lock per protegir les dades compartides.

    Programació Asíncrona:
        Cada tasca utilitza await Task.Delay() per permetre que el thread torni al pool mentre espera:

    No es bloqueja cap thread, sinó que s'alliberen recursos mentre s'espera.
    L'ús de async/await permet que tot funcioni de manera fluida sense bloquejos.

Aquesta arquitectura garanteix que totes les operacions (pintar, calcular i escoltar el teclat) s'executin simultàniament però a les freqüències adequades per a cada tasca, optimitzant recursos i mantenint el joc fluid.
Justificació de les Decisions d'Implementació

    Ús de lock vs altres mecanismes:
        Vaig triar lock per la seva simplicitat i perque l'havia fet servir abans amb l'anterior projecte.
       

    Dificultat Progressiva:
        Implementar una dificultat creixent (augmentant la freqüència d'asteroides) millora l'experiència de joc, era masa facil:

    Estadístiques Persistents:
        Mantenir estadístiques inclús després de perdre vides permet veure el rendiment global de la sessió. Voldria haber ficat un maxim de vides pero tenia poc temps.

