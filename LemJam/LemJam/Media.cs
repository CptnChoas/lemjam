using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemJam
{
    public class Media : IDatabase
    {

        #region Fields & Properties
        int mediaId;
        string path;
        string title;

        public string TableName
        {
            get
            {
                return "media";
            }
        }
        public Dictionary<string, object> Fields
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "mediaId",  MediaId},
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
                    { "mediaId",  MediaId},
                    { "path", Path }
                };
            }
        }

        public int MediaId
        {
            get
            {
                return mediaId;
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

        public Media(int mediaId, string path, string title)
        {
            this.mediaId = mediaId;
            this.path = path;
            this.title = title;
        }

        public static Media FromDatabase(SQLiteDataReader reader)
        {
            return new Media((int)reader["mediaId"], (string)reader["path"], (string)reader["title"]);
        }

        public void Save()
        {
            Program.db.Store(this);
        }

        public void Update()
        {
            Program.db.Update(this);
        }

        public void Delete()
        {
            Program.db.Delete(this);
        }
    }
}
