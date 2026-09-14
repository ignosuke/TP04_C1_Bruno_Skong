# SKONG

**[English](#english) · [Español](#español)**

**[Play it on itch.io](#)** · *https://ignosuke.itch.io/skong*

<!-- Drop a gameplay gif or screenshot here -->

---

## English

A local two-player Pong built in Unity, with configurable match rules, paddles that move in two axes, and a ball that gets meaner the longer a rally lasts.

### How to play

Two players, one keyboard, one screen.

| | Move |
|---|---|
| **Player One** | `W` `A` `S` `D` |
| **Player Two** | `↑` `←` `↓` `→` |

Unlike classic Pong, paddles move **horizontally as well as vertically** — but only inside their own half, and never across the strip down the middle of the court. Rushing the centre line to meet the ball early is a real option, and leaving your goal wide open is a real consequence.

### Match rules

Before each match you pick how the winner is decided:

**By rounds** — best of 1, 3, 5 or 7. First to take the majority wins it.

**By time** — 15, 30, 45 or 60 seconds. Whoever is ahead when the clock runs out takes the match. If the score is level, the ball decides: whichever half it's sitting in when time expires concedes the deciding goal.

### The 20-second rule

No stalling. Every round runs a countdown, shown as a bar in the HUD that fades from blue to amber to red. If nobody scores before it empties, the ball's current half concedes an automatic goal. Keep it on your opponent's side, or pay for it.

### The ball

The ball launches with an impulse at the start of every round and never loses energy to friction or bounce damping — what changes is the speed floor it's held to. Every few bounces (paddles and walls both count) it steps up by a set multiplier, up to a capped number of increases. Late rallies are noticeably faster than the serve.

Its direction is also clamped away from the axes, so a flat contact can't leave it bouncing endlessly in a straight horizontal or vertical line.

### Paddle feedback

Paddles flash and fade back, like a filter rather than a repaint:

- **Black** when you push against the edge of the court
- **A random colour** every time you connect with the ball

Neither one overwrites your configured colour — that's still yours, and the flash melts back into it.

### Player settings

From the main menu, each player can independently set their **speed**, **paddle width** and **colour**. Changes apply live: the runtime state broadcasts to whichever systems care, so a paddle already sitting against the wall when it grows will push itself back inside the court on its own.

### Built with

Unity · C# · 2D physics · TextMeshPro

### How it's put together

**ScriptableObjects for design data.** Starting values live in assets, not in the Inspector of individual objects — `PlayerDataSo` per player, `BallDataSo`, `CourtBounds`, `GameSettingsSo`. Runtime state is kept separate from them, so play-mode changes never leak back into the assets.

**Events over Inspector wiring for cross-cutting changes.** Settings panels, goal zones and the rules panel broadcast; whoever cares subscribes. Nothing downstream holds a reference to the manager above it.

**A state machine for the match loop.** Rule selection, serving, playing, round end, match end. Each state owns exactly which timers run, which is what keeps the serve countdown from eating into a timed match's clock.

**Physics where it fits, code where it doesn't.** The ball is a dynamic Rigidbody2D bouncing off real colliders. Paddles are kinematic and clamped in code, because "your own half, minus the centre strip" is a logical region that doesn't map cleanly onto fixed walls — and because paddles need to pass through things the ball can't.

### Running it locally

```
git clone <repo-url>
```

Open the project in Unity, load `MainMenuScene`, press Play.

> Starting from `GameScene` directly works, but player settings will fall back to empty defaults — the values are seeded from the main menu.

---

## Español

Un Pong local para dos jugadores hecho en Unity, con reglas de partida configurables, paletas que se mueven en dos ejes y una pelota que se pone más brava cuanto más dura el peloteo.

### Cómo se juega

Dos jugadores, un teclado, una pantalla.

| | Movimiento |
|---|---|
| **Player One** | `W` `A` `S` `D` |
| **Player Two** | `↑` `←` `↓` `→` |

A diferencia del Pong clásico, las paletas se mueven **en horizontal además de en vertical**, pero solo dentro de su propia mitad y sin poder cruzar la franja del medio de la cancha. Adelantarse hasta la línea central para salir a buscar la pelota es una opción real, y dejar el arco abierto es una consecuencia real.

### Reglas de partida

Antes de cada partida elegís cómo se define el ganador:

**Por rondas** — mejor de 1, 3, 5 o 7. Gana el primero que se lleva la mayoría.

**Por tiempo** — 15, 30, 45 o 60 segundos. Gana el que esté arriba cuando se acabe el reloj. Si están empatados, decide la pelota: el lado de cancha en el que esté al terminar el tiempo se come el gol decisivo.

### La regla de los 20 segundos

No se puede especular. Cada ronda corre una cuenta regresiva, visible en el HUD como una barra que pasa de azul a ámbar a rojo. Si nadie convierte antes de que se vacíe, el lado donde esté la pelota recibe un gol automático. Mantenela del lado del rival, o pagás.

### La pelota

La pelota sale con un impulso al comienzo de cada ronda y nunca pierde energía por fricción ni por amortiguación del rebote: lo que cambia es la velocidad a la que se la mantiene. Cada cierta cantidad de rebotes (cuentan tanto las paletas como las paredes) sube según un multiplicador, hasta un tope de aumentos. Los peloteos largos son notoriamente más rápidos que el saque.

Su dirección además se mantiene alejada de los ejes, así un contacto plano no la deja rebotando eternamente en línea recta horizontal o vertical.

### Feedback de las paletas

Las paletas destellan y vuelven a su color, como un filtro y no como un repintado:

- **Negro** al empujar contra el límite de la cancha
- **Un color aleatorio** cada vez que le pegan a la pelota

Ninguno de los dos pisa el color configurado: ese sigue siendo tuyo, y el destello se funde de vuelta hacia él.

### Configuración de jugador

Desde el menú principal, cada jugador puede ajustar por separado su **velocidad**, **ancho de paleta** y **color**. Los cambios se aplican en vivo: el estado de runtime avisa a los sistemas que le interesan, así que una paleta que ya estaba contra la pared cuando crece se reacomoda sola dentro de la cancha.

### Hecho con

Unity · C# · físicas 2D · TextMeshPro

### Cómo está armado

**ScriptableObjects para los datos de diseño.** Los valores iniciales viven en assets y no en el Inspector de cada objeto: `PlayerDataSo` por jugador, `BallDataSo`, `CourtBounds`, `GameSettingsSo`. El estado de runtime se mantiene aparte, así los cambios en play mode nunca se filtran de vuelta a los assets.

**Eventos en lugar de referencias del Inspector para lo que cruza sistemas.** Los paneles de settings, las zonas de gol y el panel de reglas avisan; se suscribe el que le interesa. Ningún objeto de abajo guarda una referencia al manager de arriba.

**Una máquina de estados para el loop de partida.** Selección de reglas, saque, juego, fin de ronda, fin de partida. Cada estado define exactamente qué timers corren, que es lo que evita que la espera del saque se coma el reloj de una partida por tiempo.

**Física donde corresponde, código donde no.** La pelota es un Rigidbody2D dinámico que rebota contra colliders reales. Las paletas son kinemáticas y se clampean por código, porque "tu propia mitad, menos la franja del centro" es una zona lógica que no mapea bien a paredes fijas, y porque las paletas tienen que poder atravesar cosas que la pelota no.

### Correrlo localmente

```
git clone <repo-url>
```

Abrí el proyecto en Unity, cargá `MainMenuScene` y dale Play.

> Arrancar directo desde `GameScene` funciona, pero la configuración de los jugadores va a caer en valores default vacíos: se siembra desde el menú principal.

---

## Credits · Créditos

Game assets:

- **UI & Fonts Bundle**
- **Rolling Ball Bundle**
- **Shape Characters Bundle**

All courtesy of [kenney.nl](https://www.kenney.nl)

---

Built for a game development course. Ported, broken, fixed and rebalanced more times than the commit history admits.