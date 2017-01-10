using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemJam
{
    public class PathItem : IDatabase
    {
        #region Fields & Properties
        string path;
        string displayName;
        PathItemType type;

        public string Path
        {
            get
            {
                return path;
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public PathItemType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public string TableName
        {
            get
            {
                return "PathPool";
            }
        }

        public Dictionary<string, object> Fields
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "path" , path },
                    { "displayName", displayName },
                    { "type", type }
                };
            }
        }

        public Dictionary<string, object> PrimaryKeys
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "path" , path }
                };
            }
        }
        #endregion

        public PathItem(string path, string displayName, PathItemType type)
        {
            this.path = path;
            this.displayName = displayName;
            this.type = type;
        }

        public static PathItem FromDatabase(SQLiteDataReader reader)
        {
            PathItem pathItem = new PathItem(reader.GetString(0), reader.GetString(1), (PathItemType)reader.GetInt32(2));
            return pathItem;
        }

        public void Save(Database db)
        {
            db.Store(this);
        }

        public void Update(Database db)
        {
            db.Update(this);
        }

        internal void Delete(Database db)
        {
            db.Delete(this);
        }
    }
}
