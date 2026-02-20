namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama
{
    public class ResponseToken
    {
        public int cli_id { get; set; }
        public int usr_id { get; set; }
        public string usr_name { get; set; }
        public string usr_email { get; set; }
        public int usr_timezone { get; set; }
        public string usr_login { get; set; }
        public string usr_client_name { get; set; }
        public string usr_client_login { get; set; }
        public string token { get; set; }
        public bool send_block { get; set; }
    }
}