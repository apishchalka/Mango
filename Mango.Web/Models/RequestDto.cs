namespace Mango.Web.Models
{
    public class RequestDto
    {
        public  string Url { get; set; }
        public object Data { get; set; }

        public required HttpMethod Method { get; set; }
    }
}
