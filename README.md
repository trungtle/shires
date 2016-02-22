## Synopsis

*Shires* is a two player digital board game. The game is inspired by traditional chess game mixed with capture the flag.

![Shires screen shot](https://s3-us-west-2.amazonaws.com/trle/shires/shires_screenshot.png)

## Download

The game can be downloaded [here](http://www.trungtuanle.com/files/shires.zip). The game currently only supported OSX and Windows.

## Link

[Website](http://www.trungtuanle.com/game/shires)

## Rules

### Board

There are five zones, or shires:
- Red shire: the starting shire for red player
- Blue shire: the starting shire for blue player
- Neutral shires:
	- Orange shire: this represents the Hill. Hill gives +1 Rook bonus.
	- Teal shire: this represents the River. River grants Lord the ability to attack.
	- Purple shire: this represents the Valley. Valley converts Lord's movement to a Knight.

At the center of each shire is the capture tile. In order to capture a shire, a Lord unit must land on this tile. Capturing netraul shires gives a player different bonuses, but capturing the opponent's shire is the final objective.
Neutral shire can be recaptured and the bonus would transfer to the player who currently controls that shire.
The board contains destructable rocks in grey. Destructable rocks can only be destroyed by a Rook. Once destroyed, destructable rocks will respawn after several turns.
To travel from one shire to the other, the unit needs to enter a portal. The links between portals are indicated by the text on the portal. Unit can't immediately re-enter portals, meaning the have to make a move within the same shire before reentering the portal again.

### Units

- Lord: the lord is the *ONLY* unit that can capture a shire. Lord moves 2 tiles in straight line. The Lord cannot attack another unit.
- Knight: the Knight can hop to exactly 3 tiles away. Knight can attack another unit.
- Rook: the Rook moves in straight lines across the length of a shire. Rook can attack another unit can destroy rocks.

If a unit is attacked, it will respawn at the starting location after several turns.

### Objective

The objective of the game is to capture the opponent's shire. The game is over once a red Lord lands on blue's capture tile, or vice versa.

## Development

*Shires* was created in [Unity3D](https://unity3d.com/) using C#.

## Credit

[Trung Le](http://www.trungtuanle.com/game/shires), creator, programmer, design.

## License

The MIT License (MIT)

Copyright (c) [2016] [Trung Le]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
