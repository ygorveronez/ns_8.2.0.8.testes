namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Klios
{
    public class RequestSendDoc
    {
        public string id_KliosAnalise { get; set; }
        public string tipo_doc { get; set; }
        public string file { get; set; }
        public string file_base64 { get; set; }
    }
}
