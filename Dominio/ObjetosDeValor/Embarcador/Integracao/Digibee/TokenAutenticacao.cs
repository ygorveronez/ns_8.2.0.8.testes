namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class TokenAutenticacao
    {
        public bool FalhaAutenticacao { get; set; }

        public string Token { get; set; }

        public string MensagemErro { get; set; }

        public string JsonRequisicao { get; set; }

        public string JsonRetorno { get; set; }

        public bool Jwt { get; set; }
    }
}
