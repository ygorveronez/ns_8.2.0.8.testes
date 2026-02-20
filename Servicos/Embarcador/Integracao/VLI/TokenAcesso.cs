using System;

namespace Servicos.Embarcador.Integracao.VLI
{
    public class TokenAcesso
    {
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.TokenAcesso cacheTokenAutenticacao = null;

        public static void SetCacheToken(string tokenAcesso, int tempoExpiracao)
        {
            cacheTokenAutenticacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.TokenAcesso()
            {
                Token = tokenAcesso,
                DataExpiracao = DateTime.Now.AddSeconds(tempoExpiracao)
            };
        }

        public static bool TokenValido()
        {
            if (cacheTokenAutenticacao == null)
                return false;

            return cacheTokenAutenticacao.DataExpiracao >= DateTime.Now;
        }

        public static string ObterToken()
        {
            if (!TokenValido())
                throw new Exception("É necessário autenticar antes de executar a integração.");

            return cacheTokenAutenticacao.Token;
        }
    }

}

