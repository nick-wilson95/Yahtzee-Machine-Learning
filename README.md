# Yahtzee Machine Learning

Welcome to Yahtzee Machine Learning.

### To Run:
- Ensure .Net 7 SDK is installed
- Run `dotnet build` at project root
- Cd to Yahtzee/bin/Debug/net7.0/
- Run `start Yahtzee.exe`

### Features:
- Play Yahtzee on the command line
- Configure and start a machine learning experiment
- Autosaving means you can continue the last experiment if the process was interrupted

### About:
- AIs are based on simple neural networks
  - Inputs are: dice rolls, number of rerolls used, scored categories, current upper score
  - Outputs are: dice to reroll, priority weight for each score category
  - Size of hidden layers and number of hidden layers are configurable
  - Can be mutated via a randomisation scheme run on all weights
- Each iteration:
  - Plays *n* games for each AI (where dice rolls are the same for all AIs each game)
  - Takes the top *m* AIs based on average results of *n* games - and puts them in the next generation along with *k* mutated offspring
  - Values for *n*, *m* and *k* are configurable
- Every 100 or so iterations (depending on configured complexity):
  - A summary will show average results since the previous summary (hopefully this value will increase pretty reliably)
  - The experiment will autosave
