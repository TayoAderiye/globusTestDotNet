using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusBank.Infrastructure.Helpers
{
    public class JsonHelper
    {
        public async static Task<T> LoadJson<T>(string path) where T : new()
        {
            T Config = new T();
            using (StreamReader r = new StreamReader(path))
            {
                string json = await r.ReadToEndAsync();
                Config = JsonConvert.DeserializeObject<T>(json);
            }
            return Config;
        }
    }
}
