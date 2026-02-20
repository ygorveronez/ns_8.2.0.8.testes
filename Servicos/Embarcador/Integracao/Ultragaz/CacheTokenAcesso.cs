using System;

namespace Servicos.Embarcador.Integracao.Ultragaz
{
    public static class CacheTokenAcesso
    {
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.CacheTokenAcesso cacheTokenAutenticacao = null;

        public static void SetCache(string tokenAcesso, int tempoExpiracao)
        {
            cacheTokenAutenticacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.CacheTokenAcesso()
            {
                TokenAcesso = tokenAcesso,
                DataExpiracao = DateTime.Now.AddSeconds(tempoExpiracao)
            };
        }

        public static bool IsTokenCacheValido()
        {
            if (cacheTokenAutenticacao == null)
                return false;

            return cacheTokenAutenticacao.DataExpiracao >= DateTime.Now;
        }

        public static string ObterToken()
        {
            if (!IsTokenCacheValido())
                throw new Exception("É necessário autenticar antes de executar a integração.");

            return cacheTokenAutenticacao.TokenAcesso;
        }
    }
}
