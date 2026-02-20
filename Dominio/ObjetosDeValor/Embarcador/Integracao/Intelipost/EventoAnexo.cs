namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost
{
    public class EventoAnexo
    {
        public string url { get; set; }
        public string content_in_base64 { get; set; }
        public string type { get; set; }
        public string file_name { get; set; }
        public string mime_type { get; set; }
        public EventoAnexoInformacaoAdicional additional_information { get; set; }
    }
}
