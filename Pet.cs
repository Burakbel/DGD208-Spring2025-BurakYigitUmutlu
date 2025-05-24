using System;
using System.Threading;
using System.Threading.Tasks;

public class PetStatusEventArgs : EventArgs
{
    public string Message { get; }

    public PetStatusEventArgs(string message)
    {
        Message = message;
    }
}

public class Pet
{
    private Random random = new Random();
    public string Name { get; private set; }
    public PetType Type { get; private set; }

    // Stats
    public int Hunger { get; private set; }
    public int Sleep { get; private set; }
    public int Fun { get; private set; }
    
    public bool IsHungry => Hunger < 50;
    public bool IsTired => Sleep < 50;
    public bool IsBored => Fun < 50;
    public bool IsDead => Hunger <= 0 || Sleep <= 0 || Fun <= 0;

    private readonly int StatDecreaseRate = 1; // Amount of stats decrease per update
    private readonly int MaxStat = 100;
    private readonly int InitialStat = 50;
    
    public event EventHandler<PetStatusEventArgs>? PetDied;

    public event EventHandler<PetStatusEventArgs>? StatusChanged;
    public event EventHandler<string>? ActivityPerformed;

    private readonly (string activity, string message)[] possibleActivities = new[]
    {
        ("found a sunny spot to rest in", "They seem very relaxed!"),
        ("played with a butterfly", "They're having fun!"),
        ("took a short nap", "They look refreshed!"),
        ("explored the surroundings", "They discovered new things!"),
        ("practiced some tricks", "They're getting better at it!"),
        ("made a new friend", "They look very happy!"),
        ("found something interesting", "They seem excited!")
    };

    public Pet(string name, PetType type)
    {
        Name = name;
        Type = type;
        
        // Initialize stats to 50
        Hunger = InitialStat;
        Sleep = InitialStat;
        Fun = InitialStat;
    }

    private void OnStatusChanged(string message = "")
    {
        StatusChanged?.Invoke(this, new PetStatusEventArgs(message));
    }

    private void OnActivityPerformed(string activity)
    {
        ActivityPerformed?.Invoke(this, activity);
    }

    public void UseItem(Item item)
    {
        try
        {
            if (!item.CompatibleWith.Contains(Type))
            {
                OnStatusChanged($"{Name} can't use {item.Name}!");
                return;
            }

            switch (item.AffectedStat)
            {
                case PetStat.Hunger:
                    if (Hunger >= MaxStat)
                    {
                        OnStatusChanged($"{Name} is not hungry right now!");
                        return;
                    }
                    OnActivityPerformed($"{Name} is eating {item.Name}...");
                    Thread.Sleep((int)(item.Duration * 1000));
                    Hunger = Math.Min(MaxStat, Hunger + item.EffectAmount);
                    OnStatusChanged($"{Name} enjoyed their meal! Hunger is now at {Hunger}%");
                    break;

                case PetStat.Fun:
                    if (Fun >= MaxStat)
                    {
                        OnStatusChanged($"{Name} doesn't want to play right now!");
                        return;
                    }
                    OnActivityPerformed($"{Name} is playing with {item.Name}...");
                    Thread.Sleep((int)(item.Duration * 1000));
                    Fun = Math.Min(MaxStat, Fun + item.EffectAmount);
                    OnStatusChanged($"{Name} had fun playing! Fun level is now at {Fun}%");
                    break;

                case PetStat.Sleep:
                    if (Sleep >= MaxStat)
                    {
                        OnStatusChanged($"{Name} is not tired right now!");
                        return;
                    }
                    OnActivityPerformed($"{Name} is getting cozy with {item.Name}...");
                    Thread.Sleep((int)(item.Duration * 1000));
                    Sleep = Math.Min(MaxStat, Sleep + item.EffectAmount);
                    OnStatusChanged($"{Name} feels relaxed! Energy level is now at {Sleep}%");
                    break;
            }
        }
        catch (Exception)
        {
            // Silently handle any exceptions during item usage
        }
    }

    public void UpdateStats()
    {
        // Decrease all stats
        Hunger = Math.Max(0, Hunger - StatDecreaseRate);
        Sleep = Math.Max(0, Sleep - StatDecreaseRate);
        Fun = Math.Max(0, Fun - StatDecreaseRate);
        
        // Check for warnings
        if (IsHungry)
        {
            OnStatusChanged($"{Name} is getting hungry! Current hunger: {Hunger}%");
        }
        if (IsTired)
        {
            OnStatusChanged($"{Name} is getting tired! Current energy: {Sleep}%");
        }
        if (IsBored)
        {
            OnStatusChanged($"{Name} is getting bored! Current fun: {Fun}%");
        }
        
        // Check for death condition
        if (IsDead)
        {
            string cause = Hunger <= 0 ? "hunger" : (Sleep <= 0 ? "exhaustion" : "depression");
            string deathMessage = $"{Name} has died from {cause}... 😢";
            OnStatusChanged(deathMessage);
            PetDied?.Invoke(this, new PetStatusEventArgs(deathMessage));
        }
    }

    public void SleepAction()
    {
        try
        {
            OnActivityPerformed($"{Name} is going to sleep...");
            Thread.Sleep(3000); // Sleeping animation time
            OnStatusChanged($"{Name} had a good sleep!");
        }
        catch (Exception)
        {
            // Silently handle any exceptions during the operation
        }
    }

    public void TakeTime(int minutes)
    {
        try
        {
            OnActivityPerformed($"{Name} is going to spend some time on their own...");
            
            int eventCount = random.Next(1, Math.Max(2, minutes / 5)); // One event every ~5 minutes
            
            for (int i = 0; i < eventCount; i++)
            {
                var (activity, message) = possibleActivities[random.Next(possibleActivities.Length)];
                
                string eventMessage = $"{Name} {activity}! {message}";
                OnActivityPerformed(eventMessage);
                OnStatusChanged(eventMessage);
                
                // Add some delay between events
                Thread.Sleep(2000);
            }
            
            // Final status update
            OnStatusChanged($"{Name} finished spending time on their own!");
        }
        catch (Exception)
        {
            // Silently handle any errors during the operation
        }
    }

    public string GetAsciiArt()
    {
        switch (Type)
        {
            case PetType.Dog:
                return
@"  ∩━━━∩
  (◐ᴥ◐)
  / ▽ \
 /|___|\
  |  |";
            
            case PetType.Cat:
                return
@"  /\___/\
 (  o o  )
 (  =^=  )
  --^--";
            
            case PetType.Bird:
                return
@"    A_
   (°v°)
   (   )
   -″-″-";
            
            case PetType.Rabbit:
                return
@"   /\ /\
  ((.Y.))
   ()~()
   (-)-";
            
            case PetType.Fish:
                return
@"    o
  ><(((°>
    o";
            
            default:
                return "";
        }
    }
} 