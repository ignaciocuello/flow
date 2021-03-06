# flow

**Flow** is a deep, fast-paced game that allows players freedom of movement as far as their skill is able to take them.

- The premise is simple, just hit the key into the lock to progress onto the next level. :key: :unlock:

![](lvl1.gif)

- However you must not let the key touch the *orange stuff™*, or else the key will despawn. :x:

![](keydie.gif)

- Things get a bit more interesting once you start adding more keys to the mix. :key::key:

![](lvl3.gif)

- ...you can also wall jump by the way.

![](walljump.gif)

- Once we add that, we can start to have a little more fun. Complete levels as fast as possible to earn more cash. :dollar: 

![](lvl5.gif)

- Levels flow seemlessly into one another while progressively increasing in difficulty.

![](hidden.gif)

- Once you're finished you must hide away your cash ... and you're off to your next heist. :money_with_wings:

![](endscreen.gif)

- The only thing limitting your speed is yourself, go back to older levels and blaze through them with your improved skills. :clock1:

![](fast.gif)

## A Note on difficulty tuning.

One of the biggest challanges when developing a game is finding the sweet-spot for difficulty. Make the game too hard and you risk frustrating your players resulting in them likely quitting your game. Make the game too easy and it becomes trivial and boring resulting in your players again likely quitting your game.<p>In order to tackle this issue, I decided the game would automatically save a replay of the player's playthrough. With each player's consent the replay (along with inputs and other stats) was uploaded to an **Azure** blob storage, where I could take a look at it and notice any common issues players were having.<p>In addition to explicit player feedback, the replays helped immensely in fine tuning the game and overall making it more enjoyable to play. Countless levels and mechanics were discarded or reworked as a result of this, but the game turned out for the better because of it.

Flow was originally made in 2018.

note: I've only included the C# files in the repository as the project takes too long otherwise to upload. The assets I've left out are not programming related (mainly art assets).
