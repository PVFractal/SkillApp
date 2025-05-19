namespace SkillApp;
using System.IO;
using Npgsql;

class SQLConnector
{
    public void openConnection()
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


        using NpgsqlConnection connection = new NpgsqlConnection(connStrBuilder.ConnectionString);;
        connection.Open();
    }
}