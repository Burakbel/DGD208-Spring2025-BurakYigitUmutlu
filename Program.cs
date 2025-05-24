using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace GameProgramming
{
    class Program
    {
        private static Pet? currentPet;
        private static List<Pet> adoptedPets = new List<Pet>();
        private static bool isGameRunning = true;
        private static readonly object consoleLock = new object();

        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.Title = "Virtual Pet Simulator";
                Console.ForegroundColor = ConsoleColor.Cyan;
                
                while (isGameRunning)
                {
                    try
                    {
                        // Run the menu synchronously to avoid console handle issues
                        ShowMainMenu();
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("handle is invalid"))
                    {
                        // Handle the specific console error gracefully
                        Thread.Sleep(1000);
                        SafeClear();
                        continue;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            SafeWriteLine($"\nAn error occurred: {ex.Message}");
                            SafeWriteLine("Press any key to continue...");
                            
                            try
                            {
                                Console.ReadKey(true);
                            }
                            catch
                            {
                                Thread.Sleep(3000);
                            }
                        }
                        catch
                        {
                            // If even the error handling fails, just wait and continue
                            Thread.Sleep(2000);
                        }
                    }
                }
                
                Console.WriteLine("Thank you for playing! Come back soon!");
            }
            finally
            {
                Console.ResetColor();
            }
        }

        // Safe console methods to prevent handle is invalid errors
        private static void SafeWriteLine(string text)
        {
            lock (consoleLock)
            {
                try
                {
                    Console.WriteLine(text);
                }
                catch
                {
                    // Ignore console errors
                }
            }
        }
        
        private static void SafeWrite(string text)
        {
            lock (consoleLock)
            {
                try
                {
                    Console.Write(text);
                }
                catch
                {
                    // Ignore console errors
                }
            }
        }
        
        private static string SafeReadLine()
        {
            lock (consoleLock)
            {
                try
                {
                    return Console.ReadLine()?.Trim() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        
        private static ConsoleKeyInfo SafeReadKey(bool intercept = false)
        {
            lock (consoleLock)
            {
                try
                {
                    return Console.ReadKey(intercept);
                }
                catch
                {
                    return new ConsoleKeyInfo();
                }
            }
        }
        
        private static void SafeClear()
        {
            lock (consoleLock)
            {
                try
                {
                    Console.Clear();
                }
                catch
                {
                    // Ignore console errors
                }
            }
        }
        
        static void ShowMainMenu()
        {
            try
            {
                SafeClear();
                SafeWriteLine("225040052 - Burak Yiğit Umutlu");
                SafeWriteLine(new string('=', 30));
                SafeWriteLine("\n=== Virtual Pet Simulator ===\n");
                SafeWriteLine("1. Adopt a New Pet");
                SafeWriteLine("2. View My Pets");
                SafeWriteLine("3. Exit");
                
                SafeWrite("Press 1-3: ");
                
                // Safer way to read input
                string input = SafeReadLine();
                
                var key = input == "1" ? ConsoleKey.D1 : (input == "2" ? ConsoleKey.D2 : ConsoleKey.NoName);
                
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        ShowPetSelectionMenu();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        ShowAdoptedPetsMenu();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        isGameRunning = false;
                        break;
                    default:
                        SafeWriteLine("Invalid option. Please try again.");
                        Thread.Sleep(2000);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle any console errors that might occur
                SafeWriteLine($"Menu error: {ex.Message}");
                Thread.Sleep(2000);
            }
        }

        static void ShowPetSelectionMenu()
        {
            bool backToMain = false;
            
            while (!backToMain)
            {
                SafeClear();
                SafeWriteLine("225040052 - Burak Yiğit Umutlu");
                SafeWriteLine(new string('=', 30));
                SafeWriteLine("\n=== Select Your Pet ===\n");
                
                var petTypes = Enum.GetValues<PetType>();
                
                for (int i = 0; i < petTypes.Length; i++)
                {
                    SafeWriteLine($"{i + 1}. {petTypes[i]}");
                }
                
                SafeWriteLine("0. Back to Main Menu");
                SafeWrite("\nSelect a pet type: ");
                
                string input = SafeReadLine();
                if (int.TryParse(input, out int choice))
                {
                    if (choice == 0)
                    {
                        backToMain = true;
                        continue;
                    }
                    
                    if (choice > 0 && choice <= petTypes.Length)
                    {
                        var selectedPetType = petTypes[choice - 1];
                        
                        SafeWrite("\nEnter a name for your pet: ");
                        var petName = SafeReadLine();
                        
                        if (string.IsNullOrWhiteSpace(petName))
                        {
                            SafeWriteLine("\nYou must enter a name for your pet.");
                            Thread.Sleep(2000);
                            continue;
                        }
                        
                        currentPet = new Pet(petName, selectedPetType);
                        currentPet.StatusChanged += OnPetStatusChanged;
                        currentPet.ActivityPerformed += OnPetActivityPerformed;
                        currentPet.PetDied += OnPetDied;
                        
                        // Add the pet to our collection of adopted pets
                        adoptedPets.Add(currentPet);
                        
                        StartGame(currentPet);
                        backToMain = true;
                    }
                    else
                    {
                        SafeWriteLine("\nInvalid selection. Please try again.");
                        Thread.Sleep(2000);
                    }
                }
                else
                {
                    SafeWriteLine("\nPlease enter a valid number.");
                    Thread.Sleep(2000);
                }
            }
        }

        static void StartGame(Pet pet)
        {
            bool exitGame = false;
            
            pet.UpdateStats();
            
            while (!exitGame && isGameRunning)
            {
                SafeClear();
                SafeWriteLine("225040052 - Burak Yiğit Umutlu");
                SafeWriteLine(new string('=', 30));
                SafeWriteLine($"\n=== {pet.Name}'s Home ===\n");
                SafeWriteLine(pet.GetAsciiArt());
                
                // Display pet stats
                SafeWriteLine($"\nStats:");
                SafeWriteLine($"Hunger: {pet.Hunger}%");
                SafeWriteLine($"Sleep: {pet.Sleep}%");
                SafeWriteLine($"Fun: {pet.Fun}%");
                
                SafeWriteLine("\nWhat would you like to do?");
                SafeWriteLine("1. Feed Pet");
                SafeWriteLine("2. Play with Pet");
                SafeWriteLine("3. Put Pet to Sleep");
                SafeWriteLine("4. Take Some Time");
                SafeWriteLine("0. Exit to Main Menu");
                
                SafeWrite("\nSelect an action (0-4): ");
                var input = SafeReadLine();
                var key = input switch
                {
                    "0" => ConsoleKey.D0,
                    "1" => ConsoleKey.D1,
                    "2" => ConsoleKey.D2,
                    "3" => ConsoleKey.D3,
                    "4" => ConsoleKey.D4,
                    _ => ConsoleKey.NoName
                };
                
                try
                {
                    switch (key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            ShowFoodMenu(pet);
                            pet.UpdateStats();
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            ShowToyMenu(pet);
                            pet.UpdateStats();
                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            ShowSleepItemMenu(pet);
                            pet.UpdateStats();
                            break;
                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            ShowTakeTimeMenu(pet);
                            pet.UpdateStats();
                            break;
                        case ConsoleKey.D0:
                        case ConsoleKey.NumPad0:
                            exitGame = true;
                            continue;
                        default:
                            SafeWriteLine("\nInvalid option. Please try again.");
                            Thread.Sleep(2000);
                            break;
                    }
                    
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    SafeWriteLine($"\nError: {ex.Message}");
                    Thread.Sleep(2000);
                }
            }
        }

        static void ShowTakeTimeMenu(Pet pet)
        {
            try
            {
                SafeWrite("\nHow many minutes should your pet spend on their own? (1-30): ");
                string input = SafeReadLine();
                
                if (int.TryParse(input, out int minutes) && minutes >= 1 && minutes <= 30)
                {
                    SafeWriteLine($"\n{pet.Name} will spend {minutes} minutes on their own...");
                    pet.TakeTime(minutes);
                }
                else
                {
                    SafeWriteLine("\nPlease enter a valid number between 1 and 30.");
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                SafeWriteLine($"Error: {ex.Message}");
                Thread.Sleep(2000);
            }
        }

        private static void OnPetStatusChanged(object? sender, PetStatusEventArgs e)
        {
            SafeWriteLine($"\n{e.Message}");
        }

        private static void OnPetActivityPerformed(object? sender, string activity)
        {
            SafeWriteLine($"\n{activity}");
        }
        
        private static void OnPetDied(object? sender, PetStatusEventArgs e)
        {
            if (sender is Pet pet)
            {
                try
                {
                    SafeWriteLine($"\n{e.Message}");
                    SafeWriteLine("Press any key to continue...");
                    
                    SafeReadKey(true);
                    
                    // Remove the pet from the adopted pets list
                    if (adoptedPets.Contains(pet))
                    {
                        adoptedPets.Remove(pet);
                    }
                    
                    currentPet = null;
                }
                catch
                {
                    // If console operations fail, just set the game state
                    if (adoptedPets.Contains(pet))
                    {
                        adoptedPets.Remove(pet);
                    }
                    currentPet = null;
                }
            }
        }
        
        private static void ShowFoodMenu(Pet pet)
        {
            var foodItems = ItemDatabase.AllItems.Where(i => i.Type == ItemType.Food && i.CompatibleWith.Contains(pet.Type)).ToList();
            ShowItemMenu(pet, foodItems, "Food");
        }
        
        private static void ShowToyMenu(Pet pet)
        {
            var toys = ItemDatabase.AllItems.Where(i => i.Type == ItemType.Toy && i.CompatibleWith.Contains(pet.Type)).ToList();
            ShowItemMenu(pet, toys, "Toys");
        }
        
        private static void ShowSleepItemMenu(Pet pet)
        {
            var sleepItems = ItemDatabase.AllItems.Where(i => i.AffectedStat == PetStat.Sleep && i.CompatibleWith.Contains(pet.Type)).ToList();
            ShowItemMenu(pet, sleepItems, "Sleep Items");
        }
        
        // New method to show the adopted pets menu
        private static void ShowAdoptedPetsMenu()
        {
            bool backToMain = false;
            
            while (!backToMain)
            {
                SafeClear();
                SafeWriteLine("225040052 - Burak Yiğit Umutlu");
                SafeWriteLine(new string('=', 30));
                SafeWriteLine("\n=== My Adopted Pets ===\n");
                
                if (adoptedPets.Count == 0)
                {
                    SafeWriteLine("You haven't adopted any pets yet!");
                    SafeWriteLine("\nPress any key to return to the main menu...");
                    SafeReadKey(true);
                    backToMain = true;
                    continue;
                }
                
                for (int i = 0; i < adoptedPets.Count; i++)
                {
                    Pet pet = adoptedPets[i];
                    SafeWriteLine($"{i + 1}. {pet.Name} the {pet.Type}");
                    SafeWriteLine($"   Hunger: {pet.Hunger}%, Energy: {pet.Sleep}%, Fun: {pet.Fun}%");
                }
                
                SafeWriteLine("\n0. Back to Main Menu");
                SafeWrite("\nSelect a pet to interact with: ");
                
                string input = SafeReadLine();
                if (int.TryParse(input, out int choice))
                {
                    if (choice == 0)
                    {
                        backToMain = true;
                    }
                    else if (choice > 0 && choice <= adoptedPets.Count)
                    {
                        currentPet = adoptedPets[choice - 1];
                        StartGame(currentPet);
                    }
                    else
                    {
                        SafeWriteLine("\nInvalid selection. Please try again.");
                        Thread.Sleep(2000);
                    }
                }
                else
                {
                    SafeWriteLine("\nInvalid input. Please enter a number.");
                    Thread.Sleep(2000);
                }
            }
        }
        
        private static void ShowItemMenu(Pet pet, List<Item> items, string category)
        {
            try
            {
                SafeClear();
                SafeWriteLine($"\n=== Available {category} for {pet.Name} ===\n");
                
                for (int i = 0; i < items.Count; i++)
                {
                    SafeWriteLine($"{i + 1}. {items[i].Name}");
                }
                
                SafeWriteLine("0. Back");
                SafeWrite("\nSelect an item: ");
                
                string input = SafeReadLine();
                
                if (int.TryParse(input, out int choice) && choice > 0 && choice <= items.Count)
                {
                    pet.UseItem(items[choice - 1]);
                    Thread.Sleep(1000);
                }
                else if (choice != 0)
                {
                    SafeWriteLine("\nInvalid selection.");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                SafeWriteLine($"Menu error: {ex.Message}");
                Thread.Sleep(1000);
            }
        }
    }
}