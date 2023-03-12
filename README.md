# UCL-VR-2022
UCL Virtual Environment 2022

## Git commit info.

[new] new files

[upd] update files

[dbg] debug codes

[del] delete files

## Requirments

1. Rule of the Game

+ Number of Players: 2-4

+ Communicate by speaking

+ 1-2 players (?) draw the given word; 1 player judge the if the guess is correct( or no judge at the start); the rest guess the word by robocalls

+ Steps:

    + Press the start button to start a new round;

    + Random choose 1-2 players (?), give them a word to draw, then random choose a referee;

    + The players have 120 seconds (?) to draw;

    + The other player can communicate with each other when drawing, and they have a button to get the chance of answering. 
    
    + The player guessing the word can only speak one word and the referee should judge if the answer is correct. 
    
    + If correct, the round finishes and the players of drawing and anwsering can get corresponding points.

    + If not, the round continues;

    + During the end phase of one round, the room owner could decide start a new round or end the game and calculate the final points.


2. Open Scene of the Game

+ A room (Additional: change the scene);

+ All of the players have their pannel, which can be called by a button;

+ Additional: Change the apperance of players.

3. Drawing

+ By hand or by the controllers?

+ Eraser and undo (redo);

+ Change the color;

+ Create some simple 3D objects 
    
    + Sphere, cyliner, cube 

4. Recording function (?)


+ Record a game;

+ Save a drawing;

+ move drawings by hand;

+ 3d eraser.

## Modules

Priority high to low: (1) (2) (3) (4)

Open Scene:

+ One room style (2)

Panel:

+ Drawer: Word (1), color(2), 3D object (line, cube, sphere; cylinder) (3), countdown (2)

+ Player: keyboard (1), submit (1), countdown (2), (hint) (4)

+ Room owner: Start new round (2), End game (2)

Drawing:

+ Test draw by Hand and controller (1)

+ Move objects (3)

+ Eraser (3) / Undo (Redo) (3)

+ Create line/cube/sphere (3)

Guessing:

+ Random pick module (1)

+ Word repo. (1)

+ countdown module (2)

+ judge module (1)

+ Ranking module (3)

Recording:

+ Save drawings (3)

+ Save game recording (3)

Game procedure:

+ Create/Enter a room (1)

+ Owner: Side pannel-Start/End (2)

+ Start a round: (1)
    + Random choose drawer
    + Random choose a word 
    + Players: show the panel, start countdown 
    + end a round:
        + countdown to 0;
        + all the players guess out the word;
    + end phase: calculate the points, clear the playground, save the drawing

+ End game: Calculate the total points and ranking, save the game (3)

TBD:

延迟半秒进rank

积分系统rank

显示正确答案

game入口切换到当前phase done

美化UI (panel,button，clicksound)

房间场景

保存画作

每轮round之后笔归位 done

笔的owner唯一 done

轮流artist