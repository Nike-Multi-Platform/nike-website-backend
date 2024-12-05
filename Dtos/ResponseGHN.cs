namespace nike_website_backend.Dtos
{
    public class ResponseGHN<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
