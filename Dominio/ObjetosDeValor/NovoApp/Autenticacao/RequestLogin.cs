namespace Dominio.ObjetosDeValor.NovoApp.Autenticacao
{
    public class RequestLogin
    {
        public string CPF { get; set; }
        public string Senha { get; set; }
        public string UniqueID { get; set; }
        public string VersaoApp { get; set; }
        public string OneSignalPlayerId { get; set; }
    }
}
