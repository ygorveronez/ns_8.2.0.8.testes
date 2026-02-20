namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAutenticacao
    {
        Token = 1,
        UsuarioESenha = 2
    }

    public static class TipoAutenticacaoHelper
    {
        public static string ObterDescricao(this TipoAutenticacao tipoAutenticacao)
        {
            switch (tipoAutenticacao)
            {
                case TipoAutenticacao.Token: return "Token";
                case TipoAutenticacao.UsuarioESenha: return "Usuário e Senha";
                default: return string.Empty;
            }
        }
    }
}
