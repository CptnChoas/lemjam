using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace LemJam
{
    class NfoParser
    {
        private static string apiKey = "4aca0dd71a38f28ece5ca242f76672e1";
        private static TMDbClient client = new TMDbLib.Client.TMDbClient(apiKey);

        public static MediaInfo ParseNfo(string nfo)
        {
            string imdbId;

            Match match = Regex.Match(nfo, @"imdb.com/title/(tt\d?)/");

            if(match.Groups.Count != 0)
            {
                imdbId = match.Groups[1].Value;

                Task<Movie> movieTask = client.GetMovieAsync(imdbId, "de_DE", TMDbLib.Objects.Movies.MovieMethods.AccountStates | TMDbLib.Objects.Movies.MovieMethods.AlternativeTitles | TMDbLib.Objects.Movies.MovieMethods.Releases | TMDbLib.Objects.Movies.MovieMethods.Videos | TMDbLib.Objects.Movies.MovieMethods.Images);
                movieTask.Wait();
                Movie movie = movieTask.Result;

                Program.Logger.LogMessage("New MediaInfo created: " + imdbId);

                return new MediaInfo(imdbId, movie);
            }

            return null;            
        }
    }
}
