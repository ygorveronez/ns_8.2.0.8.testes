using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class TokenJWT
    {
        public string AccessToken { get; set; }
        public DateTime DataExpiracaoAccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DataExpiracaoRefreshToken { get; set; }
    }
}
