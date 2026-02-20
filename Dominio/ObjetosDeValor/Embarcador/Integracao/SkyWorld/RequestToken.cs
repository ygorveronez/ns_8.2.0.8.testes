namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld
{
    public class RequestToken
    {
        public string usuario { get; set; }

        public string password { get; set; }
    }

    public class RequestPosicoes
    {
        public string id { get; set; }

        public string token { get; set; }
    }
}
