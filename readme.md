# PONG

A local two-player Pong built in Unity, with configurable match rules, paddles that move in two axes, and a ball that gets meaner the longer a rally lasts.

**[Play it on itch.io](#)** · *(replace with your build link)*

<!-- Drop a gameplay gif or screenshot here -->

---

## How to play

Two players, one keyboard, one screen.

| | Move |
|---|---|
| **Player One** | `W` `A` `S` `D` |
| **Player Two** | `↑` `←` `↓` `→` |

Unlike classic Pong, paddles move **horizontally as well as vertically** — but only inside their own half, and never across the strip down the middle of the court. Rushing the centre line to meet the ball early is a real option, and leaving your goal wide open is a real consequence.

## Match rules

Before each match you pick how the winner is decided:

**By rounds** — best of 1, 3, 5 or 7. First to take the majority wins it.

**By time** — 15, 30, 45 or 60 seconds. Whoever is ahead when the clock runs out takes the match. If the score is level, the ball decides: whichever half it's sitting in when time expires concedes the deciding goal.

## The 20-second rule

No stalling. Every round runs a countdown, shown as a bar in the HUD that fades from blue to amber to red. If nobody scores before it empties, the ball's current half concedes an automatic goal. Keep it on your opponent's side, or pay for it.

## The ball

The ball launches with an impulse at the start of every round and never loses energy to friction or bounce damping — what changes is the speed floor it's held to. Every few bounces (paddles and walls both count) it steps up by a set multiplier, up to a capped number of increases. Late rallies are noticeably faster than the serve.

Its direction is also clamped away from the axes, so a flat contact can't leave it bouncing endlessly in a straight horizontal or vertical line.

## Paddle feedback

Paddles flash and fade back, like a filter rather than a repaint:

- **Black** when you push against the edge of the court
- **A random colour** every time you connect with the ball

Neither one overwrites your configured colour — that's still yours, and the flash melts back into it.

## Player settings

From the main menu, each player can independently set their **speed**, **paddle width** and **colour**. Changes apply live: the runtime state broadcasts to whichever systems care, so a paddle already sitting against the wall when it grows will push itself back inside the court on its own.

---

## Built with

Unity · C# · 2D physics · TextMeshPro

## How it's put together

**ScriptableObjects for design data.** Starting values live in assets, not in the Inspector of individual objects — `PlayerDataSo` per player, `BallDataSo`, `CourtBounds`, `GameSettingsSo`. Runtime state is kept separate from them, so play-mode changes never leak back into the assets.

**Events over Inspector wiring for cross-cutting changes.** Settings panels, goal zones and the rules panel broadcast; whoever cares subscribes. Nothing downstream holds a reference to the manager above it.

**A state machine for the match loop.** Rule selection, serving, playing, round end, match end. Each state owns exactly which timers run, which is what keeps the serve countdown from eating into a timed match's clock.

**Physics where it fits, code where it doesn't.** The ball is a dynamic Rigidbody2D bouncing off real colliders. Paddles are kinematic and clamped in code, because "your own half, minus the centre strip" is a logical region that doesn't map cleanly onto fixed walls — and because paddles need to pass through things the ball can't.

## Running it locally

```
git clone <repo-url>
```

Open the project in Unity, load `MainMenuScene`, press Play.

> Starting from `GameScene` directly works, but player settings will fall back to empty defaults — the values are seeded from the main menu.

---

## Credits

Game Assets:

**UI & Fonts Bundle.**
**Rolling Ball Bundle.**
**Shape Characters Bundle.**

All courtesy from www.kenney.nl

---

Built for a game development course. Ported, broken, fixed and rebalanced more times than the commit history admits.