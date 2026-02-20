namespace Dominio.ObjetosDeValor.NovoApp.Cargas
{
    public class RequestSalvarAnexo
    {
        public int  CodigoCarga { get; set; }
        public string ArquivoBase64 { get; set; }
        public int ClienteMultisoftware { get; set; }
        public string Extensao { get; set; }
    }
}
