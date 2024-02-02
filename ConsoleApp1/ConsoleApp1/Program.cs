using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


/*
    
Assignment 2: Video Game Sales
    
    Institution: Bow Valley College
    Program: Software Development
    Course: SODV2202: Object Oriented Programming
    Instructor: Dr. Sohaib Bajwa
    Student Name: Sasha Greene
    Student ID: 435097
*/






public class Game
{
    public readonly string Name;
    public readonly string Platform;
    public readonly int ReleaseYear;
    public readonly string Genre;
    public readonly string Publisher;
    public readonly double NASales;
    public readonly double EUSales;
    public readonly double JPSales;
    public readonly double OtherSales;
    //public readonly double GlobalSales;
    public readonly double CriticScore;
    public readonly int CriticCount;
    public readonly double UserScore;
    public readonly int UserCount;
    public readonly string Rating;

    private string NextValue(string csv, ref int index)
    {
        string result = "";
        if (index < csv.Length)
        {
            if (csv[index] == ',')
            {
                index++;
            }
            else if (csv[index] == '"')
            {
                int endIndex = csv.IndexOf('"', index + 1);
                result = csv.Substring(index + 1, endIndex - (index + 1));
                index = endIndex + 2;
            }
            else
            {
                int endIndex = csv.IndexOf(',', index);
                if (endIndex == -1)
                    result = csv.Substring(index);
                else
                    result = csv.Substring(index, endIndex - index);
                index = endIndex + 1;
            }
        }
        return result;
    }

    public Game(string csv)
    {
        int index = 0;
        Name = NextValue(csv, ref index);
        Platform = NextValue(csv, ref index);
        int.TryParse(NextValue(csv, ref index), out ReleaseYear);
        Genre = NextValue(csv, ref index);
        Publisher = NextValue(csv, ref index);
        double.TryParse(NextValue(csv, ref index), out NASales);
        double.TryParse(NextValue(csv, ref index), out EUSales);
        double.TryParse(NextValue(csv, ref index), out JPSales);
        double.TryParse(NextValue(csv, ref index), out OtherSales);
        NextValue(csv, ref index); //public readonly double GlobalSales;
        double.TryParse(NextValue(csv, ref index), out CriticScore);
        int.TryParse(NextValue(csv, ref index), out CriticCount);
        double.TryParse(NextValue(csv, ref index), out UserScore);
        int.TryParse(NextValue(csv, ref index), out UserCount);
        Rating = NextValue(csv, ref index);
    }
}

class Program
{
    static List<Game> Games = new List<Game>();

    static void BuildDB()
    {
        string input;
        while ((input = Console.ReadLine()) != "")
        {
            var game = new Game(input);
            Games.Add(game);
        }
    }

    static void BuildDBFromFile()
    {
        using (var reader = File.OpenText("Video Game Sales 2017.csv"))
        {
            string input = reader.ReadLine(); // Skip label row
            while ((input = reader.ReadLine()) != null)
            {
                var game = new Game(input);
                Games.Add(game);
            }
        }
    }

    static void Main(string[] args)
    {
        BuildDBFromFile();

        // Part 1
        //Basic Query
        {

            Console.WriteLine("Query 1: ");
            Console.WriteLine("What are the names of all the games that were released after 2013 as well as their release year.");
            Console.WriteLine();

            var gamesAfter2013 = from game in Games where game.ReleaseYear > 2013 orderby game.ReleaseYear select game;
            
            
            foreach (Game game in gamesAfter2013)
            {
                string output = "Name: "+game.Name+", Year: "+game.ReleaseYear;
                Console.WriteLine(output);
            }
            
            Console.WriteLine();
        }

        // Part 2
        //Fluent Syntax
        {

            Console.WriteLine("Query 2: ");
            Console.WriteLine("What are the names and critic score of the 10 lowest rated games by critics that have a critic score greater than 0");
            Console.WriteLine();
            var bottom3GamesByRating = Games
                                            .Where(game=>game.CriticScore>0)
                                            .OrderBy(game => game.CriticScore) 
                                            .Take(10);

            foreach(Game game in bottom3GamesByRating)
            {
                Console.WriteLine("Name: "+game.Name+", Critic Rating: "+game.CriticScore);
            }

            Console.WriteLine();

        }

        // Part 3
        {
            //projection

            Console.WriteLine();
            Console.WriteLine("Query 3: ");
            Console.WriteLine("what are the top 5 selling games with the word king in the title and what are the total sales?");
            Console.WriteLine();




            var gamesWithKing = Games.Where(game => game.Name.Contains("King"));
            gamesWithKing = gamesWithKing.Take(5);


            var gamesWithKingAndTotalSales = gamesWithKing.Select(game => new
            {
               TotalSales = game.JPSales+game.NASales+game.EUSales+game.OtherSales,
               Title = game.Name
            });

            foreach(var game in gamesWithKingAndTotalSales)
            {
                Console.WriteLine("Title: "+game.Title+", Total Sales: "+game.TotalSales);
            }
        }

        // Part 4
        {

            Console.WriteLine();

            Console.WriteLine("Query 4: ");
            Console.WriteLine("What are the 10 lowest selling games in japan with a rating of M where sales are greater than 0 and the user score is greater than the critic score, and what is the critic and user score?");
            Console.WriteLine();


            var LowestSellingMatureGamesJapan = from game in Games
                                                 where game.Rating == "M" && game.JPSales > 0 && game.CriticScore > 0 && game.UserScore > 0
                                                 orderby game.JPSales
                                                 select game;


            foreach (Game game in LowestSellingMatureGamesJapan.Take(10))
            {
                string output = "Name: " + game.Name + ", Total sales in japan: " + game.JPSales+", Critic Score: "+game.CriticScore+", User Score: "+game.UserScore;
                Console.WriteLine(output);
            }


        }

        // Part 5
        {
            Console.WriteLine();
            Console.WriteLine("Query 5: ");
            Console.WriteLine("What are all of the games published by Sony Computer Entertainment with Total Sales Less than 50k?");
            Console.WriteLine();
            //Query syntax:
            var TotalSalesLessThan1HundredThousand =
                from game in Games
                where game.EUSales+game.JPSales+game.NASales+game.OtherSales < 0.05
                where game.Publisher== "Sony Computer Entertainment"
                select game;

            TotalSalesLessThan1HundredThousand = TotalSalesLessThan1HundredThousand.OrderByDescending(game => game.OtherSales + game.JPSales + game.NASales + game.EUSales);

            
            foreach (Game game in TotalSalesLessThan1HundredThousand)
            {

                double totalSales = game.EUSales + game.JPSales + game.NASales + game.OtherSales;

                Console.WriteLine("Name: "+game.Name+", Total Sales: "+totalSales);
            }
        }




        Console.WriteLine();

        Console.WriteLine("Query 6: ");
        Console.WriteLine("What is the Average User Score of nintendo games where user score is greater than 0? what are the top 3 user rated nintendo games?");
        Console.WriteLine();
        var nintendoGames = Games.Where(game => game.UserScore > 0);

        double totalScore=0;
        int gameCount=0;
        foreach(Game game in nintendoGames)
        {
            totalScore += game.UserScore;
            gameCount++;
        }

        
        double averageScore = totalScore / gameCount;

        Console.WriteLine("The average user score of nintendo games is: " + averageScore+". The Top 3 rated games are: ");
        Console.WriteLine();

        nintendoGames = nintendoGames.OrderByDescending(game=>game.UserScore);
        nintendoGames = nintendoGames.Take(3);

        foreach (Game game in nintendoGames)
        {
            Console.WriteLine("Name: " + game.Name + ", User score: " + game.UserScore);
        }
        Console.WriteLine();

       Console.ReadLine();


    }
}
