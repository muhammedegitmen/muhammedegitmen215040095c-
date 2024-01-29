using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Game game = new Game(5, 5); // Room of size 5x5
        game.Start();
    }
}

class Game
{
    private Room room;
    private Player player;
    private NPC npc;
    private bool isGameOver;

    public Game(int width, int height)
    {
        room = new Room(width, height);
        player = new Player();
        npc = new NPC();
        isGameOver = false;
    }

    public void Start()
    {
        Console.WriteLine("welcome to game you are in a room because you need to find 2 object. objects are toy car and a red book ");
        room.PlaceItems();
        room.PlaceNPC();

        while (!isGameOver)
        {
            Console.WriteLine("What do you want to do? (m: move)");
            string command = Console.ReadLine().ToLower();

            switch (command)
            {
                case "m":
                    MovePlayer();
                    break;
                case "i":
                    room.Inspect(player);
                    break;
                case "t":
                    npc.Talk(player);
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }

    private void MovePlayer()
    {
        Console.WriteLine("Where do you want to move? (u: up, d: down, l: left, r: right)");
        string direction = Console.ReadLine().ToLower();
        player.Move(direction, room.Width, room.Height);

        // Automatically inspect the place after moving
        room.Inspect(player);
    }
}

class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public List<Item> Items { get; private set; }

    public Player()
    {
        X = 0; // Starting in the middle of the room
        Y = 0; // Starting in the middle of the room
        Items = new List<Item>();
    }

    public void CollectItem(Item item)
    {
    Console.WriteLine($"You found a {item.Name}: {item.Clue}");
    Items.Add(item);
    }

    public bool HasAllClues()
    {
        // Assuming there are a specific number of clues to collect
        // Update this number based on your game design
        const int totalCluesRequired = 2;
        return Items.Count >= totalCluesRequired;
    }

    public void Move(string direction, int width, int height)
    {
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        switch (direction)
        {
            case "u":
                if (Y < halfHeight) Y++;
                else Console.WriteLine("You've reached the end in this direction. Change direction.");
                break;
            case "d":
                if (Y > -halfHeight) Y--;
                else Console.WriteLine("You've reached the end in this direction. Change direction.");
                break;
            case "l":
                if (X > -halfWidth) X--;
                else Console.WriteLine("You've reached the end in this direction. Change direction.");
                break;
            case "r":
                if (X < halfWidth) X++;
                else Console.WriteLine("You've reached the end in this direction. Change direction.");
                break;
            default:
                Console.WriteLine("Invalid direction.");
                return;
        }
        Console.WriteLine($"Moved {direction}. Current position: ({X}, {Y})");
    }
}

class Room
{
    public int Width { get; }
    public int Height { get; }
    private List<Item> items;
    private NPC npc;

    public Room(int width, int height)
    {
        Width = width;
        Height = height;
        items = new List<Item>();
        npc = new NPC();
    }

    public void PlaceItems()
    {
        // Adjust item placements according to the new coordinate system
        items.Add(new Item("Toy car", "Found in the sky, often taking different shapes", 1, -2)); // Example placement
        items.Add(new Item("Red book", "Its presence can lead to the creation of rain", -1, -1)); // Example placement
        // Add more items as needed
    }

    public void PlaceNPC()
    {
        // Adjust NPC placement according to the new coordinate system
        npc.X = 2;
        npc.Y = 2;
    }

    public void Inspect(Player player)
    {
        foreach (var item in items)
        {
            if (item.X == player.X && item.Y == player.Y && !player.Items.Contains(item))
            {
                player.CollectItem(item);
                return;
            }
        }

        if (npc.X == player.X && npc.Y == player.Y)
        {
            Console.WriteLine("You see a mysterious figure in the shadows.");
            npc.Talk(player); // Automatically trigger NPC's talk method
            return;
        }

        Console.WriteLine("There is nothing of interest here.");
    }

}

class Item
{
    public string Name { get; }
    public string Clue { get; }
    public int X { get; }
    public int Y { get; }

    public Item(string name, string clue, int x, int y)
    {
        Name = name;
        Clue = clue;
        X = x;
        Y = y;
    }
}

class NPC
{
    public int X { get; set; }
    public int Y { get; set; }
    private string riddle = "I fly without wings. I cry without eyes. Wherever I go, darkness follows me. What am I?";
    private Dictionary<string, string> answers;

    public bool CheckAnswer(string answer)
    {
        // This now checks if the answer is one of the valid answers
        return answers.ContainsKey(answer);
    }

    public NPC()
    {
        answers = new Dictionary<string, string>
        {
            { "wind", "A gentle breeze." },
            { "time", "The unstoppable march." },
            { "sun", "The light of day." },
            { "Cloud", "like a cotton" },
            { "star", "A distant spark." }
        };
    }

    public string GetRiddle()
    {
        return riddle;
    }

    public void ShowAnswers(List<Item> items)
    {
        Console.WriteLine("Choose an answer:");
        foreach (var answer in answers)
        {
            Console.WriteLine($"- {answer.Key}");
        }
        // If the player has a specific item, show the correct answer
        // This can be modified based on game logic
        if (items.Exists(i => i.Name == "Book"))
        {
            Console.WriteLine("- The correct answer based on your clues");
        }
    }
/*
    public bool CheckAnswer(string answer)
    {
        // Check if the answer is correct
        // This logic can be customized to handle the special item case
        return answer == "the correct answer based on your clues";
    }*/

    public void Talk(Player player)
    {
        if (X == player.X && Y == player.Y)
        {
            if (player.HasAllClues())
            {
                Console.WriteLine("NPC: I see you have gathered all the clues. Are you ready to answer my riddle?");
                Console.WriteLine(GetRiddle());
                ShowAnswers(player.Items);

                string playerAnswer = Console.ReadLine().ToLower();
                if (CheckAnswer(playerAnswer))
                {
                    Console.WriteLine("NPC: Your answer is correct. Congratulations!");
                    // Implement game completion or next steps here
                }
                else
                {
                    Console.WriteLine("NPC: That is not the correct answer. Try again later.");
                }
            }
            else
            {
                Console.WriteLine("NPC: You have found me, but you are not yet ready to face my challenge. Find more clues.");
            }
        }
        else
        {
            Console.WriteLine("NPC: There is no one to talk to here.");
        }
    }

}