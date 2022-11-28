using Yahtzee.Agent;
using Yahtzee.Utils;

namespace Yahtzee;

public static class Terminal
{
    private static string SaveFilePath => FileHelper.ProjectPath + "/savedExperiment.json";
    
    public static void Start()
    {
        while (true)
        {
            Console.Clear();
            
            var options = new List<string> { "Play Yahtzee", "Start Experiment" };
            if (File.Exists(SaveFilePath)) options.Add("Continue Experiment");
        
            switch (UserSelect(options.ToArray()))
            {
                case "Play Yahtzee":
                    PlayYahtzee();
                    break;
                case "Start Experiment":
                    StartExperiment();
                    break;
                case "Continue Experiment":
                    ContinueExperiment();
                    break;
            }
        }
    }

    private static void ContinueExperiment()
    {
        var serialisedExperiment = File.ReadAllText(SaveFilePath);
        var experiment = Experiment.Deserialise(serialisedExperiment);
        RunExperiment(experiment);
    }

    private static void StartExperiment()
    {
        Console.Clear();
        
        var experiment = new Experiment(
            UserSupplyPositiveInt("the number of reproducing AIs in each generation (guide 20)"),
            UserSupplyPositiveInt("the number of offspring per reproducing AI (guide 2)"),
            UserSupplyPositiveInt("the number of games per generation (guide 500)"),
            UserSupplyPositiveInt("the number of hidden layers in the AIs (guide 2)"),
            UserSupplyPositiveInt("the number of nodes in AI hidden layers (guide 50)")
        );
        
        RunExperiment(experiment);
    }

    private static void RunExperiment(Experiment experiment)
    {
        Console.Clear();
        
        var reportTotals = new List<double>();
        var reportFrequency = 120 / (int)experiment.IterationComplexity;
        var reportSum = 0d;

        var iteration = 1;

        while (true)
        {
            var result = experiment.Iterate().OrderBy(x => x);

            reportSum += result.Average();
            
            Console.WriteLine($"Iteration {iteration}: {(int)result.First()} - {(int)result.Average()} - {(int)result.Last()}");

            if (iteration % reportFrequency == 0)
            {
                var serialised = experiment.Serialise();
                File.WriteAllTextAsync(SaveFilePath, serialised);
         
                reportTotals.Add(reportSum / reportFrequency);

                var reportTotalString = string.Join(", ", reportTotals.Select(x => $"{x:F2}"));
         
                Console.WriteLine($"{reportFrequency} iteration averages: {reportTotalString}");
         
                reportSum = 0;
            }

            iteration++;
        }
    }

    private static void PlayYahtzee()
    {
        var player = new HumanPlayer();
        var game = new Game.Game(player);
        var score = game.Play();
        
        Console.WriteLine($"Final score: {score} - press any key to continue");
        Console.ReadKey();
    }

    private static string UserSelect(params string[] options)
    {
        var optionCount = options.Length;
        
        if (optionCount == 0)
        {
            throw new ArgumentException("Can't select from 0 options");
        }

        var optionsPart = String.Join(", ", options.Select((x, i) => $"{i} - {x}"));

        while (true)
        {
            Console.WriteLine($"Select option: {optionsPart}");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var value) && value >= 0 && value < options.Length)
            {
                return options[value];
            }
            Console.Clear();
            Console.WriteLine("Invalid input");
        }
    }

    private static int UserSupplyPositiveInt(string purpose)
    {
        while (true)
        {
            Console.WriteLine($"Enter a value for {purpose}");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var value) && value > 0)
            {
                return value;
            }
            Console.Clear();
            Console.WriteLine("Invalid input");
        }
    }
}