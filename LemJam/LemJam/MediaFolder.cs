using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LemJam
{
    public class MediaFolder : IDatabase
    {
        #region Fields & Properties
        string path;
        string displayName;
        MediaFolderType type;
        bool workerActive = false;

        Thread worker;
        Dictionary<string, MediaInfo> mediaInfo;
        Dictionary<string, Media> mediaFiles;

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

        public MediaFolderType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                Update();
            }
        }

        public string TableName
        {
            get
            {
                return "media_folder";
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
                    { "type", type },
                    { "workerActive", workerActive }
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

        public bool WorkerActive
        {
            get
            {
                return workerActive;
            }

            set
            {
                workerActive = value;
                Update();

                if (workerActive)
                    startWorker();
            }
        }
        #endregion

        public MediaFolder(string path, string displayName, MediaFolderType type, bool workerActive)
        {
            this.path = path;
            this.displayName = displayName;
            this.type = type;
            this.workerActive = workerActive;

            mediaInfo = new Dictionary<string, MediaInfo>();
        }

        public static MediaFolder FromDatabase(SQLiteDataReader reader)
        {

            bool workerActive = false;
            if (!(reader["workerActive"] is DBNull))
                workerActive = Convert.ToBoolean(reader["workerActive"]);

            MediaFolder pathItem = new MediaFolder((string)reader["path"], (string)reader["displayName"], (MediaFolderType)(int)reader["type"], workerActive);

            if (pathItem.workerActive)
                pathItem.startWorker();

            return pathItem;
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

        private void startWorker()
        {
            workerActive = true;

            if (worker == null || !worker.IsAlive)
            {
                worker = new Thread(work);
                worker.Start();
            }
        }

        private void work()
        {
            while(workerActive)
            {
                string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (!workerActive)
                        break;

                    string mediaName = System.IO.Path.GetFileNameWithoutExtension(file);

                    if (System.IO.Path.GetExtension(file) == ".nfo")
                    {
                        MediaInfo info = NfoParser.ParseNfo(file);
                        info.MediaName = mediaName;

                        if (info != null)
                            if (!mediaInfo.ContainsKey(info.MediaName))
                                mediaInfo.Add(info.MediaName, info);

                    }
                    else if (mediaName.StartsWith("thumbs"))
                        continue;
                    else
                    {
                        if(!mediaFiles.ContainsKey(mediaName)) {

                            Media media = new Media(0, file, mediaName);
                            media.Save();
                            mediaFiles.Add(mediaName, media);
                        }
                    }
                }

                Thread.Sleep(1500);
            }
        }
        public void ShutdownWorker()
        {
            workerActive = false;
        }
    }
}
