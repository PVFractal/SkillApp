namespace SkillApp;
using System.IO;
using Npgsql;
using System.Collections.Generic;

class SQLConnector
{

    NpgsqlConnection connection;

    /// <summary>
    /// Constructor that initializes the connection field
    /// </summary>
    public SQLConnector()
    {
        connection = new NpgsqlConnection();
    }

    /// <summary>
    /// Gives the connection field all it needs to connect to the database
    /// </summary>
    public void MakeConnection()
    {
        StreamReader reader = new StreamReader(".env");

        var nextLine = "";
        do
        {
            nextLine = reader.ReadLine();
            if (nextLine == null)
            {
                break;
            }
            var kvPair = nextLine.Split(": ");
            if (kvPair.Length > 1)
            {
                Environment.SetEnvironmentVariable(kvPair[0], kvPair[1]);
            }
        } while (nextLine != null);



        var host = Environment.GetEnvironmentVariable("host");
        var port = Environment.GetEnvironmentVariable("port");
        var database = Environment.GetEnvironmentVariable("database");
        var username = Environment.GetEnvironmentVariable("username");
        var password = Environment.GetEnvironmentVariable("password");

        var connStrBuilder = new NpgsqlConnectionStringBuilder();
        connStrBuilder.Host = host;
        connStrBuilder.Username = username;
        connStrBuilder.Password = password;
        connStrBuilder.Database = database;
        try
        {
            if (port != null)
            {
                connStrBuilder.Port = int.Parse(port);
            }
        }
        catch
        {
            Console.WriteLine(".evn variable port could not be converted to int: port=" + port);
        }


        connection.ConnectionString = connStrBuilder.ConnectionString;
    }

    /// <summary>
    /// Creates the table skills if it does not exist
    /// </summary>
    public void CreateTable()
    {
        connection.Open();
        // First, checking to see if the table already exists
        string commandString = "CREATE TABLE IF NOT EXISTS skills (skill TEXT, reminder_time INTEGER, start_time INTEGER)"; // Make the table if it does not exist

        using NpgsqlCommand makeTable = new NpgsqlCommand(commandString, connection);

        makeTable.ExecuteNonQuery();

        connection.Close();

    }


    /// <summary>
    /// Inserts a new given skill into the table
    /// </summary>
    /// <param name="skill">The skill to add</param>
    /// <param name="time">The amouont of time in seconds between reminders</param>
    public void InsertSkill(string skill, int time)
    {
        connection.Open();
        var unixTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO skills (skill, reminder_time, start_time) VALUES (@skill, @reminder_time, @start_time)", connection);
        cmd.Parameters.AddWithValue("@skill", skill);
        cmd.Parameters.AddWithValue("@reminder_time", time);
        cmd.Parameters.AddWithValue("@start_time", unixTimeStamp);

        int rowsAffected = cmd.ExecuteNonQuery();
        connection.Close();
    }

    /// <summary>
    /// Deletes a skill from the table
    /// </summary>
    /// <param name="skill">The skill to delete</param>
    public void DeleteSkill(string skill)
    {
        connection.Open();
        using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM skills WHERE skill = @skill", connection);
        cmd.Parameters.AddWithValue("@skill", skill);

        int rowsAffected = cmd.ExecuteNonQuery();
        connection.Close();
    }

    /// <summary>
    /// Gets the rows from the skills table, in the form of (<skill>, <time left before notification>)
    /// </summary>
    /// <returns>Returns the skills and their time left before notification</returns>
    public List<(string, long)> GetSkills()
    {
        var skillsList = new List<(string, long)>();

        connection.Open();
        var unixTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM skills", connection);
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var skill = reader.GetString(0);
            var timeElapsed = unixTimeStamp - reader.GetInt32(2);
            var timeLeft = reader.GetInt32(1) - timeElapsed;
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }
            skillsList.Add((skill, timeLeft));
        }


        connection.Close();

        return skillsList;
    }
}