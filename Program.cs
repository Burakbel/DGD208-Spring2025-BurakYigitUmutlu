using System;
using System.Threading.Tasks;

namespace GameProgramming
{
    class Program
    {
        private static Pet? currentPet;
        private static bool isGameRunning = true;

        static async Task Main(string[] args)
        {
            try
            {
                Console.Title = "Virtual Pet Simulator";
                Console.ForegroundColor = ConsoleColor.Cyan;
                
                while (isGameRunning)
                {
                    try
                    {
                        await ShowMainMenuAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nAn error occurred: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                
                Console.WriteLine("Thank you for playing! Come back soon!");
            }
            finally
            {
                Console.ResetColor();
            }
        }

        static async Task ShowMainMenuAsync()
        {
            Console.Clear();
            Console.WriteLine("225040052 - Burak Yiğit Umutlu");
            Console.WriteLine(new string('=', 30));
            Console.WriteLine("\n=== Virtual Pet Simulator ===\n");
            Console.WriteLine("1. Start Game");
            Console.WriteLine("2. Exit");
            Console.Write("\nSelect an option: ");
            
            var key = Console.ReadKey().Key;
            Console.WriteLine("\n");
            
            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    await ShowPetSelectionMenuAsync();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    isGameRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    await Task.Delay(2000);
                    break;
            }
        }

        static async Task ShowPetSelectionMenuAsync()
        {
            bool backToMain = false;
            
            while (!backToMain)
            {
                Console.Clear();
                Console.WriteLine("225040052 - Burak Yiğit Umutlu");
                Console.WriteLine(new string('=', 30));
                Console.WriteLine("\n=== Select Your Pet ===\n");
                
                var petTypes = Enum.GetValues<PetType>();
                
                for (int i = 0; i < petTypes.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {petTypes[i]}");
                    var tempPet = new Pet($"Preview{petTypes[i]}", petTypes[i]);
                    Console.WriteLine(tempPet.GetAsciiArt() + "\n");
                }
                
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("\nSelect a pet: ");
                
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice > 0 && choice <= petTypes.Length)
                    {
                        var selectedPet = petTypes[choice - 1];
                        Console.Clear();
                        var tempPet = new Pet($"Preview{selectedPet}", selectedPet);
                        Console.WriteLine($"You selected: {selectedPet}");
                        Console.WriteLine(tempPet.GetAsciiArt());
                        Console.Write("\nEnter a name for your pet: ");
                        string petName = Console.ReadLine()?.Trim() ?? "";
                        
                        if (string.IsNullOrEmpty(petName))
                        {
                            petName = $"My{selectedPet}";
                        }
                        
                        currentPet = new Pet(petName, selectedPet);
                        currentPet.StatusChanged += OnPetStatusChanged;
                        currentPet.ActivityPerformed += OnPetActivityPerformed;
                        
                        await StartGameAsync(currentPet);
                        backToMain = true;
                    }
                    else if (choice == 0)
                    {
                        backToMain = true;
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid selection. Please try again.");
                        await Task.Delay(2000);
                    }
                }
                else
                {
                    Console.WriteLine("\nPlease enter a valid number.");
                    await Task.Delay(2000);
                }
            }
        }

        static async Task StartGameAsync(Pet pet)
        {
            bool exitGame = false;
            
            while (!exitGame && isGameRunning)
            {
                Console.Clear();
                Console.WriteLine("225040052 - Burak Yiğit Umutlu");
                Console.WriteLine(new string('=', 30));
                Console.WriteLine($"\n=== {pet.Name}'s Home ===\n");
                Console.WriteLine(pet.GetAsciiArt());
                
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Sleep");
                Console.WriteLine("2. Take Some Time");
                Console.WriteLine("0. Exit to Main Menu");
                
                Console.Write("\nSelect an action: ");
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                
                try
                {
                    switch (key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            await pet.SleepAsync();
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            await ShowTakeTimeMenuAsync(pet);
                            break;
                        case ConsoleKey.D0:
                        case ConsoleKey.NumPad0:
                            exitGame = true;
                            continue;
                        default:
                            Console.WriteLine("\nInvalid option. Please try again.");
                            await Task.Delay(2000);
                            break;
                    }
                    
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    await Task.Delay(2000);
                }
            }
        }

        static async Task ShowTakeTimeMenuAsync(Pet pet)
        {
            Console.Write("\nHow many minutes should your pet spend on their own? (1-30): ");
            if (int.TryParse(Console.ReadLine(), out int minutes) && minutes >= 1 && minutes <= 30)
            {
                Console.WriteLine($"\n{pet.Name} will spend {minutes} minutes on their own...");
                await pet.TakeTimeAsync(minutes);
            }
            else
            {
                Console.WriteLine("\nPlease enter a valid number between 1 and 30.");
                await Task.Delay(2000);
            }
        }

        private static void OnPetStatusChanged(object? sender, PetStatusEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Message))
            {
                Console.WriteLine($"\n{e.Message}");
            }
        }

        private static void OnPetActivityPerformed(object? sender, string activity)
        {
            Console.WriteLine($"\n{activity}");
        }
    }
} 