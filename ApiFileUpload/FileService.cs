using Azure.Storage;
using Azure.Storage.Blobs;

namespace ApiFileUpload
{
    public class FileService
    {
        private readonly string _storageAccount = "mahmoudblob20023";
        private readonly string _Key = "oTd3W4xOVQNaZP1jhyEOD87JOlCILY9jvSmTefY5Be78uKzbujuF3zKS2KM+dV0ypOe1WlpIpQkm+AStlStKqA==";
        private readonly BlobContainerClient _fileContainer;

        public FileService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _Key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
           var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _fileContainer = blobServiceClient.GetBlobContainerClient("files");

        }

        public async Task<List<BlobDto>> ListAsync()
        {
            List<BlobDto> files =new List<BlobDto>();

            await foreach(var file in _fileContainer.GetBlobsAsync())
            {
                string uri =_fileContainer.Uri.ToString();
                var name =file.Name;
                var fullUri=$"{uri}/{name}";

                files.Add(new BlobDto
                {
                    Uri =fullUri,
                    Name=name,
                    ContentType=file.Properties.ContentType,
                    
                });
            }
            return files;
        }

        public async Task<BlobResponceDto> UploadAsync(IFormFile blob)
        {
            BlobResponceDto response = new();
            BlobClient client = _fileContainer.GetBlobClient(blob.FileName);


            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response;
        }


        public async Task<BlobDto> DownloadAsync(string blobFilename)
        {
            BlobClient file = _fileContainer.GetBlobClient(blobFilename);
            
            if(await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;

                var content = await file.DownloadContentAsync();
                string name = blobFilename;
                string contentType =content.Value.Details.ContentType;

                return new BlobDto
                {
                    Content = blobContent,
                    Name = name,
                    ContentType = contentType
                };
            }
            return null;
        }

        public async Task<BlobResponceDto> DeleteAsync(string blobFilename)
        {
            BlobClient file = _fileContainer.GetBlobClient(blobFilename);

           await file.DeleteAsync();
           
            return new BlobResponceDto { Error=false ,
            Status=$"File: {blobFilename} has been successfully deleted."};
        }
    }
}
