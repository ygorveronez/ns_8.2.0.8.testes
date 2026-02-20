namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class PacoteWebHook
    {
        public string jwt { get; set; }
    }

    public class RetornoWebHook
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
