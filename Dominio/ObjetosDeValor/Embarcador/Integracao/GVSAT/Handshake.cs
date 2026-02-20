namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    public class Handshake
    {
        public string username { get; set; }
        public string password { get; set; }
        public int appid { get; set; }
        public string token { get; set; }
        public long expiration { get; set; }
    }
}
