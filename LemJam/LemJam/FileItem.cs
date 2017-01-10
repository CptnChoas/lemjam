using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemJam
{
    public class FileItem : IDatabase
    {

        #region Fields & Properties
        int imdbId;
        string path;
        string title;

        public string TableName
        {
            get
            {
                return "Items";
            }
        }
        public Dictionary<string, object> Fields
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "imdbId",  ImdbId},
                    { "path", Path },
                    { "title", Title }
                };
            }
        }

        public Dictionary<string, object> PrimaryKeys
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "imdbId",  ImdbId},
                    { "path", Path }
                };
            }
        }

        public int ImdbId
        {
            get
            {
                return imdbId;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }
        public string Title
        {
            get
            {
                return title;
            }
        }

        #endregion

        public FileItem(int imdbId, string path, string title)
        {
            this.imdbId = imdbId;
            this.path = path;
            this.title = title;
        }

        public static FileItem FromDatabase(SQLiteDataReader reader)
        {
            return new FileItem(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        }

        public void Save(Database db)
        {

        }

        public void Update(Database db)
        {

        }
    }
}
