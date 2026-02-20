namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Traffilog
{
    public class RequestToken
    {
        public ActionRequestToken action { get; set; }
    }

    public class ActionRequestToken
    {
        public string name { get; set; }
        public ParametersRequestToken parameters { get; set; }
    }

    public class ParametersRequestToken
    {
        public string login_name { get; set; }
        public string password { get; set; }
    }
}