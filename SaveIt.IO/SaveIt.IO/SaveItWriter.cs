using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaveIt.Common;

namespace SaveIt.IO
{
    public class SaveItWriter : IDisposable
    {
        private StreamWriter writer;

        public SaveItWriter(string filePath)
        {
            writer = File.AppendText(filePath);
        }

        public async Task WriteUserAsync(SaveItUser user)
        {
            string json = JsonConvert.SerializeObject(user);
            await writer.WriteLineAsync(
                string.Format("{0}\t{1}", typeof(SaveItUser), json));
        }

        public async Task WriteEntityAsync<T>(T entity)
            where T : SaveItEntityBase
        {
            string json = JsonConvert.SerializeObject(entity);
            await writer.WriteLineAsync(
                string.Format("{0}\t{1}", typeof(T), json));
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}