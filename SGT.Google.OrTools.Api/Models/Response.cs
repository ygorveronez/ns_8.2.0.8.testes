namespace Google.OrTools.Api.Models
{
    public class Response<T>
    {
        public bool status { get; set; }
        public string msg { get; set; }
        public int qtde { get; set; }
        public T result { get; set; }
    }
}