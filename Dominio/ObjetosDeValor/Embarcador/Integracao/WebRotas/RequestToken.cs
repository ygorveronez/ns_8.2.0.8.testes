namespace Dominio.ObjetosDeValor.Embarcador.Integracao.WebRotas
{
    public class RequestToken
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Realm { get; set; } = "G";

    }
}
