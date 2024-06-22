using Firebase.Storage;

namespace SU24_VMO_API.Services
{
    public class FirebaseService
    {
        IConfiguration config;
        private readonly string _storageBucket;
        public FirebaseService()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            _storageBucket = config["Firebase:StorageBucket"];
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            try
            {
                if (file != null && file.Length != 0)
                {

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                    var storageClient = new FirebaseStorage(_storageBucket);
                    using (var stream = file.OpenReadStream())
                    {
                        stream.Position = 0; // Reset the stream position

                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            memoryStream.Position = 0;
                            await storageClient.Child("images").Child(fileName).PutAsync(memoryStream);
                        }
                    }

                    FirebaseStorageReference starsRef = storageClient.Child("images/" + fileName);
                    string link = await starsRef.GetDownloadUrlAsync();
                    return link;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public async Task<byte[]> DownloadFiletest(string? fileName)
        {

            var storageClient = new FirebaseStorage(_storageBucket);
            FirebaseStorageReference starsRef;

            starsRef = storageClient.Child("images/" + fileName);
            //starsRef = storageClient.Child($"{projectId}/" + $"{fileType}/" + fileName);


            // Get the download URL
            string downloadUrl = await starsRef.GetDownloadUrlAsync();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(downloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    return content;
                }
            }
            return null;
        }
    }
}
