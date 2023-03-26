namespace ApiFileUpload
{
    public class BlobResponceDto
    {
        public BlobResponceDto()
        {
            Blob= new BlobDto();
        }
        public string? Status { get; set; } 
        public bool Error { get; set; }
        public BlobDto Blob { get; set; }
    }
}
