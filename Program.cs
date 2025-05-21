using SkillApp;


SQLConnector connector = new SQLConnector();

connector.MakeConnection();
connector.CreateTable();

// The while loop controls the ui

// Page determines what page of the ui the user is on
var page = "home";

// startMessage is the message displayed at the top of the "page"
var startMessage = "Welcome to Skill Manager!\n";
while (true)
{

    // Clearing the console for 30 lines
    for (var i = 0; i < 20; i++)
    {
        Console.WriteLine(new string(' ', Console.WindowWidth));
    }
    Console.SetCursorPosition(0, Console.CursorTop - 20);

    Console.WriteLine(startMessage);

    if (page == "home")
    {
        Console.WriteLine("Home\n");

        Console.WriteLine("Options:\n - Add skill \t\t(a)\n - View skills \t\t(v)\n - Quit \t\t(q)\n");
        Console.Write("Enter your choice: ");
        var input = Console.ReadLine();
        if (input != null)
        {
            input = input.ToLower();
            if (input == "a" || input == "add")
            {
                page = "add";
                startMessage = "Skill Manager\n";
            }
            else if (input == "v" || input == "view")
            {
                page = "view";
                startMessage = "Skill Manager\n";
            }
            else if (input == "q" || input == "quit")
            {
                break;
            }
        }

        Console.SetCursorPosition(0, Console.CursorTop - 10);
    }
    else if (page == "add")
    {
        Console.WriteLine("Add Skill\n");

        Console.Write("Enter your skill you would like to maintain: ");
        var skill = Console.ReadLine();
        Console.Write("Enter amount of time between reminders (format: unit amount - e.g 1 year or 5 minutes): ");
        var time = Console.ReadLine();

        var timeNumber = 0;

        if (time == null || skill == null)
        {
            startMessage = "Nothing entered. Please try again.\n";
        }
        else
        {
            var timeSplit = time.Split(" ");

            if (timeSplit.Length != 2)
            {
                startMessage = "Time entered was not in correct format. Please try again.\n";
            }

            try
            {
                timeNumber = int.Parse(timeSplit[0]);
            }
            catch
            {
                startMessage = "Time entered was not in correct format. Please try again.\n";
            }

            if (timeNumber != 0)
            {
                if (timeSplit[1] == "y" || timeSplit[1] == "year" || timeSplit[1] == "years")
                {
                    timeNumber *= 60 * 60 * 24 * 365;
                }
                else if (timeSplit[1] == "m" || timeSplit[1] == "month" || timeSplit[1] == "months")
                {
                    timeNumber *= 60 * 60 * 24 * 30;
                }
                else if (timeSplit[1] == "d" || timeSplit[1] == "day" || timeSplit[1] == "days")
                {
                    timeNumber *= 60 * 60 * 24;
                }
                else if (timeSplit[1] == "h" || timeSplit[1] == "hour" || timeSplit[1] == "hours")
                {
                    timeNumber *= 60 * 60;
                }
                else if (timeSplit[1] == "min" || timeSplit[1] == "minute" || timeSplit[1] == "minutes")
                {
                    timeNumber *= 60;
                }
                else if (timeSplit[1] == "s" || timeSplit[1] == "second" || timeSplit[1] == "seconds")
                {
                    timeNumber *= 1;
                }
                else
                {
                    timeNumber = 0;
                    startMessage = "Incorrect time format. Try again\n";
                }
            }

            if (skill != "" && timeNumber != 0)
            {
                connector.InsertSkill(skill, timeNumber);
                page = "home";
                startMessage = "Skill \"" + skill + "\" successfully added!\n";
            }

        }


        Console.SetCursorPosition(0, Console.CursorTop - 6);
    }
    else if (page == "view")
    {
        Console.WriteLine("View\n");
        var skills = connector.GetSkills();

        var counter = 1;
        foreach (var skill in skills)
        {
            var timeLeft = TimeSpan.FromSeconds(skill.Item2);
            Console.WriteLine("Skill " + counter.ToString() + ": " + skill.Item1 + "\t\tTime left until notification: " + timeLeft.ToString());
            counter++;
        }

        Console.WriteLine("\nOptions:");
        Console.WriteLine(" - back\t\t(b)");
        Console.WriteLine(" - delete skill\t(delete #)");
        Console.WriteLine(" - quit\t\t(q)");
        Console.Write("Enter your choice: ");
        var input = Console.ReadLine();

        if (input == "b" || input == "back")
        {
            page = "home";
            startMessage = "Skill Manager\n";
        }
        else if (input == null || input == "")
        {

        }
        else if (input[0] == 'd')
        {
            var splitInput = input.Split(" ");
            if (splitInput.Length == 2)
            {
                
                try
                {
                    var indexToDelete = int.Parse(splitInput[1]);
                    if (indexToDelete > 0 && indexToDelete <= skills.Count)
                    {
                        var skillToDelete = skills[indexToDelete - 1].Item1;
                        connector.DeleteSkill(skillToDelete);
                        startMessage = "Successfully deleted \"" + skillToDelete + "\"\n";
                    }
                    else
                    {
                        startMessage = "Enter a number shown to delete\n";
                    }
                }
                catch
                {
                    startMessage = "Enter a proper delete command\n";
                }
                
            }
        }
        else if (input == "quit" || input == "q")
        {
            break;
        }
        
        Console.SetCursorPosition(0, Console.CursorTop - (9 + counter));
    }
    

}

