using System;
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
    }

    private void OnStatusChanged(string message = "")
    {
        StatusChanged?.Invoke(this, new PetStatusEventArgs(message));
    }

    private void OnActivityPerformed(string activity)
    {
        ActivityPerformed?.Invoke(this, activity);
    }

    public async Task SleepAsync()
    {
        await Task.Run(async () =>
        {
            OnActivityPerformed($"{Name} is going to sleep...");
            await Task.Delay(3000); // Sleeping animation time
            OnStatusChanged($"{Name} had a good sleep!");
        });
    }

    public async Task TakeTimeAsync(int minutes)
    {
        await Task.Run(async () =>
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
                await Task.Delay(2000);
            }
            
            // Final status update
            OnStatusChanged($"{Name} finished spending time on their own!");
        });
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