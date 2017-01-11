using System.Collections.Generic;

namespace LemJam
{
    public interface IDatabase
    {

        string TableName { get; }
        Dictionary<string, object> Fields { get; }
        Dictionary<string, object> PrimaryKeys { get; }

        void Save();
        void Update();
        void Delete();
    }
}