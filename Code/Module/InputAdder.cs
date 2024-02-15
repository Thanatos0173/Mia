using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;


namespace Celeste.Mod.Mia.InputAdder
{
    public class Inputting
    {
        
        public static bool Move(bool[] movements)
        {
            LoadMoveFile();
            List<String> movementsCorresponding = new List<string>() { "L", "R", "U", "D", "G", "X", "J" };
            string movingText = "";
            for(int i = 0;i < 7;i++)
            {
                if (movements[i]) movingText += movementsCorresponding[i] + (i != 6? ",": "");
            }
            string newPath = Environment.CurrentDirectory + @"\Mia";
            string filePath = newPath + @"\moving.tas";
            try {
                File.WriteAllText(filePath, "1,"+movingText);
                string request = "http://localhost:32270/tas/playtas?filePath=" + filePath;
                SendHttpRequest(request);
            }
            catch (NullReferenceException) { return false; }
            return true;
        }
        


        private static void LoadMoveFile()
        {
            string newPath = Environment.CurrentDirectory + @"\Mia";
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string filePath = newPath + @"\moving.tas";
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { }
            
        }
        static async void SendHttpRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Request failed: " + ex.Message);
                }
            }
        }
    }
}
