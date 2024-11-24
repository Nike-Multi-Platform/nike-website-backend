using System.IO;
using System.Text.Json;

public class FirebaseConfig
{
    public string API_KEY { get; set; }
}

namespace nike_website_backend.Helpers
{
    public class ConfigHelper
    {
        public static FirebaseConfig LoadFirebaseConfig()
        {
            // Đường dẫn tới file JSON
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "Configs", "nike-d3392-firebase-adminsdk-t6ndk-364532f7b5.json");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Firebase config file not found!");
            }

            // Đọc nội dung file JSON
            string json = File.ReadAllText(configPath);

            // Parse JSON thành object
            return JsonSerializer.Deserialize<FirebaseConfig>(json);
        }
    }
}
