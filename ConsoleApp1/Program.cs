using System;
using System.Collections.Generic;
using System.IO;

namespace CinemaManager
{
    struct Movie
    {
        public int Id;
        public string Title;
        public string Genre;
        public int Year;
        public int TicketsSold;
    }

    class Program
    {
        static List<Movie> movies = new List<Movie>();
        static int nextId = 1;
        const string FileName = "movies_data.txt";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== MAIN MENU ===");
                Console.WriteLine("1. Show movies");
                Console.WriteLine("2. Add movie");
                Console.WriteLine("3. Find movie");
                Console.WriteLine("4. Edit movie");
                Console.WriteLine("5. Delete movie");
                Console.WriteLine("6. Statistics");
                Console.WriteLine("7. Genre analysis");
                Console.WriteLine("8. Filter by year");
                Console.WriteLine("9. Sorting");
                Console.WriteLine("10. Save data");
                Console.WriteLine("11. Load data");
                Console.WriteLine("0. Exit");
                Console.Write("\nChoose an action: ");

                string choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1": ShowMovies(movies); break;
                    case "2": AddMovie(); break;
                    case "3": FindMovie(); break;
                    case "4": EditMovie(); break;
                    case "5": DeleteMovie(); break;
                    case "6": ShowStatistics(); break;
                    case "7": AnalyzeGenre(); break;
                    case "8": FilterByYear(); break;
                    case "9": SortMovies(); break;
                    case "10": SaveData(); break;
                    case "11": LoadData(); break;
                    case "0": isRunning = false; break;
                    default: Console.WriteLine("Invalid choice. Please try again."); break;
                }

                if (isRunning)
                {
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                }
            }
        }

        static void ShowMovies(List<Movie> listToShow)
        {
            if (listToShow.Count == 0)
            {
                Console.WriteLine("The movie list is empty.");
                return;
            }

            Console.WriteLine(new string('-', 75));
            Console.WriteLine($"| {"Id",-4} | {"Title",-20} | {"Genre",-15} | {"Year",-6} | {"Tickets Sold",-15} |");
            Console.WriteLine(new string('-', 75));

            for (int i = 0; i < listToShow.Count; i++)
            {
                Console.WriteLine($"| {listToShow[i].Id,-4} | {listToShow[i].Title,-20} | {listToShow[i].Genre,-15} | {listToShow[i].Year,-6} | {listToShow[i].TicketsSold,-15} |");
            }
            Console.WriteLine(new string('-', 75));
        }

        static void AddMovie()
        {
            Movie newMovie = new Movie();
            newMovie.Id = nextId++;

            newMovie.Title = ReadString("Enter movie title: ");
            newMovie.Genre = ReadString("Enter movie genre: ");
            newMovie.Year = ReadInt("Enter release year: ", 1895, DateTime.Now.Year);
            newMovie.TicketsSold = ReadInt("Enter number of tickets sold: ", 0, int.MaxValue);

            movies.Add(newMovie);
            Console.WriteLine("Movie added successfully!");
        }

        static void FindMovie()
        {
            Console.Write("Enter title or genre to search: ");
            string query = Console.ReadLine().ToLower();

            List<Movie> results = new List<Movie>();
            for (int i = 0; i < movies.Count; i++)
            {
                if (movies[i].Title.ToLower().Contains(query) || movies[i].Genre.ToLower().Contains(query))
                {
                    results.Add(movies[i]);
                }
            }

            Console.WriteLine("\nSearch results:");
            ShowMovies(results);
        }

        static void EditMovie()
        {
            int idToEdit = ReadInt("Enter movie Id to edit: ", 1, int.MaxValue);
            int index = FindIndexById(idToEdit);

            if (index == -1)
            {
                Console.WriteLine("Movie with this Id not found.");
                return;
            }

            Movie m = movies[index];
            Console.WriteLine($"Editing movie: {m.Title} (leave blank to keep unchanged)");

            Console.Write($"New title ({m.Title}): ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTitle)) m.Title = newTitle.Trim();

            Console.Write($"New genre ({m.Genre}): ");
            string newGenre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newGenre)) m.Genre = newGenre.Trim();

            Console.Write($"New release year ({m.Year}): ");
            string newYearStr = Console.ReadLine();
            if (int.TryParse(newYearStr, out int y) && y >= 1895 && y <= DateTime.Now.Year) m.Year = y;

            Console.Write($"New number of tickets ({m.TicketsSold}): ");
            string newTicketsStr = Console.ReadLine();
            if (int.TryParse(newTicketsStr, out int t) && t >= 0) m.TicketsSold = t;

            movies[index] = m;
            Console.WriteLine("Movie data updated.");
        }

        static void DeleteMovie()
        {
            int idToDelete = ReadInt("Enter movie Id to delete: ", 1, int.MaxValue);
            int index = FindIndexById(idToDelete);

            if (index != -1)
            {
                movies.RemoveAt(index);
                Console.WriteLine("Movie deleted.");
            }
            else
            {
                Console.WriteLine("Movie with this Id not found.");
            }
        }

        static void ShowStatistics()
        {
            if (movies.Count == 0)
            {
                Console.WriteLine("No data for statistics.");
                return;
            }

            int totalMovies = movies.Count;
            long totalTickets = 0;
            int maxTickets = -1;
            string mostPopularTitle = "";

            List<string> uniqueGenres = new List<string>();
            List<int> genreTickets = new List<int>();

            for (int i = 0; i < movies.Count; i++)
            {
                totalTickets += movies[i].TicketsSold;

                if (movies[i].TicketsSold > maxTickets)
                {
                    maxTickets = movies[i].TicketsSold;
                    mostPopularTitle = movies[i].Title;
                }

                string g = movies[i].Genre.ToLower();
                int gIndex = -1;
                for (int j = 0; j < uniqueGenres.Count; j++)
                {
                    if (uniqueGenres[j] == g) { gIndex = j; break; }
                }

                if (gIndex == -1)
                {
                    uniqueGenres.Add(g);
                    genreTickets.Add(movies[i].TicketsSold);
                }
                else
                {
                    genreTickets[gIndex] += movies[i].TicketsSold;
                }
            }

            double averageTickets = (double)totalTickets / totalMovies;

            int maxGenreTickets = -1;
            string bestGenre = "";
            for (int i = 0; i < uniqueGenres.Count; i++)
            {
                if (genreTickets[i] > maxGenreTickets)
                {
                    maxGenreTickets = genreTickets[i];
                    bestGenre = uniqueGenres[i];
                }
            }

            Console.WriteLine("=== STATISTICS ===");
            Console.WriteLine($"Total number of movies: {totalMovies}");
            Console.WriteLine($"Total tickets sold: {totalTickets}");
            Console.WriteLine($"Average tickets per movie: {Math.Round(averageTickets, 2)}");
            Console.WriteLine($"Most popular movie: {mostPopularTitle} ({maxTickets} tickets)");
            Console.WriteLine($"Most popular genre: {bestGenre} ({maxGenreTickets} tickets in total)");
        }

        static void AnalyzeGenre()
        {
            Console.Write("Enter genre for analysis: ");
            string targetGenre = Console.ReadLine().ToLower();

            List<Movie> genreMovies = new List<Movie>();
            long totalTickets = 0;
            int maxTickets = -1;
            string bestMovie = "";

            for (int i = 0; i < movies.Count; i++)
            {
                if (movies[i].Genre.ToLower() == targetGenre)
                {
                    genreMovies.Add(movies[i]);
                    totalTickets += movies[i].TicketsSold;
                    if (movies[i].TicketsSold > maxTickets)
                    {
                        maxTickets = movies[i].TicketsSold;
                        bestMovie = movies[i].Title;
                    }
                }
            }

            if (genreMovies.Count == 0)
            {
                Console.WriteLine("No movies found for the specified genre.");
            }
            else
            {
                Console.WriteLine($"\nMovies in genre '{targetGenre}':");
                ShowMovies(genreMovies);
                Console.WriteLine($"Number of movies: {genreMovies.Count}");
                Console.WriteLine($"Total tickets sold: {totalTickets}");
                Console.WriteLine($"Most popular movie in this genre: {bestMovie} ({maxTickets} tickets)");
            }
        }

        static void FilterByYear()
        {
            int startYear = ReadInt("Start year: ", 1895, DateTime.Now.Year);
            int endYear = ReadInt("End year: ", startYear, DateTime.Now.Year);

            List<Movie> filtered = new List<Movie>();
            for (int i = 0; i < movies.Count; i++)
            {
                if (movies[i].Year >= startYear && movies[i].Year <= endYear)
                {
                    filtered.Add(movies[i]);
                }
            }

            Console.WriteLine($"\nMovies from {startYear} to {endYear}:");
            ShowMovies(filtered);
            Console.WriteLine($"Number of movies in range: {filtered.Count}");
        }

        static void SortMovies()
        {
            if (movies.Count < 2)
            {
                Console.WriteLine("Not enough movies to sort.");
                return;
            }

            Console.WriteLine("1. From least to most popular (ascending)");
            Console.WriteLine("2. From most to least popular (descending)");
            Console.Write("Choose sorting type: ");
            string sortChoice = Console.ReadLine();

            bool ascending = sortChoice == "1";

            for (int i = 0; i < movies.Count - 1; i++)
            {
                for (int j = 0; j < movies.Count - i - 1; j++)
                {
                    bool needSwap = ascending ?
                        movies[j].TicketsSold > movies[j + 1].TicketsSold :
                        movies[j].TicketsSold < movies[j + 1].TicketsSold;

                    if (needSwap)
                    {
                        Movie temp = movies[j];
                        movies[j] = movies[j + 1];
                        movies[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine("\nList sorted!");
            ShowMovies(movies);
        }

        static void SaveData()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileName))
                {
                    for (int i = 0; i < movies.Count; i++)
                    {
                        sw.WriteLine($"{movies[i].Id}|{movies[i].Title}|{movies[i].Genre}|{movies[i].Year}|{movies[i].TicketsSold}");
                    }
                }
                Console.WriteLine("Data successfully saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving: {ex.Message}");
            }
        }

        static void LoadData()
        {
            if (!File.Exists(FileName))
            {
                Console.WriteLine("Data file not found.");
                return;
            }

            try
            {
                movies.Clear();
                int maxId = 0;

                using (StreamReader sr = new StreamReader(FileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 5)
                        {
                            Movie m = new Movie();
                            if (int.TryParse(parts[0], out m.Id) &&
                                int.TryParse(parts[3], out m.Year) &&
                                int.TryParse(parts[4], out m.TicketsSold))
                            {
                                m.Title = parts[1];
                                m.Genre = parts[2];

                                if (!string.IsNullOrWhiteSpace(m.Title) && !string.IsNullOrWhiteSpace(m.Genre) && m.Year > 0 && m.TicketsSold >= 0)
                                {
                                    movies.Add(m);
                                    if (m.Id > maxId) maxId = m.Id;
                                }
                            }
                        }
                    }
                }
                nextId = maxId + 1;
                Console.WriteLine("Data successfully loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading: {ex.Message}");
            }
        }

        static string ReadString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Error: field cannot be empty.");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input.Trim();
        }

        static int ReadInt(string prompt, int min, int max)
        {
            int result;
            bool valid;
            do
            {
                Console.Write(prompt);
                valid = int.TryParse(Console.ReadLine(), out result);
                if (!valid || result < min || result > max)
                {
                    Console.WriteLine($"Error: enter an integer from {min} to {max}.");
                    valid = false;
                }
            } while (!valid);
            return result;
        }

        static int FindIndexById(int id)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if (movies[i].Id == id) return i;
            }
            return -1;
        }
    }
}