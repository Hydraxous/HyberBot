using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.DataPersistence
{
    internal static class TokenRetriever
    {

        private const string TOKEN_FILENAME = "token.txt";

        public static async Task<AsyncTokenOperationResult> RetrieveTokenAsync()
        {
            AsyncTokenOperationResult result = new AsyncTokenOperationResult();
            result.success = false;
            result.token = string.Empty;

            string folder = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(folder, TOKEN_FILENAME);
            if(File.Exists(filePath))
            {
                using (StreamReader streamreader = new StreamReader(filePath))
                {
                    result.token = await streamreader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(result.token))
                    {
                        result.success = true;
                    }
                }
            }

            return result;
        }
    }

    public struct AsyncTokenOperationResult
    {
        public string token;
        public bool success;
    }
}
