using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.Mia.InputAdder
{
    public class Inputting
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<bool> MoveAsync(bool[] movements)
        {
            await LoadMoveFileAsync();
            List<string> movementsCorresponding = new List<string>() { "L", "R", "U", "D", "G", "X", "J" };
            StringBuilder movingTextBuilder = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                if (movements[i]) movingTextBuilder.Append(movementsCorresponding[i]).Append(i != 6 ? "," : "");
            }
            string movingText = movingTextBuilder.ToString();
            string newPath = Path.Combine(Environment.CurrentDirectory, "Mia");
            string filePath = Path.Combine(newPath, "moving.tas");
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    await writer.WriteAsync("1," + movingText);
                }
                string request = "http://localhost:32270/tas/playtas?filePath=" + filePath;
                await SendHttpRequestAsync(request);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static async Task LoadMoveFileAsync()
        {
            string newPath = Path.Combine(Environment.CurrentDirectory, "Mia");
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string filePath = Path.Combine(newPath, "moving.tas");
            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                    // No asynchronous disposal available, so just close the stream synchronously
                    fs.Close();
                }
            }
        }

        private static async Task SendHttpRequestAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed: " + ex.Message);
            }
        }
    }
}
