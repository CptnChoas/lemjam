using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace LemJam
{
    public class MediaInfo
    {
        string store = System.Environment.CurrentDirectory;
        Movie movie;
        public string MediaId;
        public string MediaName;

        public MediaInfo(string imdbId) {
            MediaId = imdbId;
            Newtonsoft.Json.JsonConvert.DeserializeObject <Movie>(File.ReadAllText(store + "\\" + MediaId + ".json"));
        }

        public MediaInfo(string imdbId, Movie movie)
        {
            MediaId = imdbId;
            this.movie = movie;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(movie, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(store + "\\" + MediaId + ".json", json);
        }
    }
}
