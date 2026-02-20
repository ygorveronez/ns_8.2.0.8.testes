namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AguardandoIntegracao
    {
        Confirmado = 0,
        Aguardando = 1
    }

    public static class AguardandoIntegracaoHelper
    {
        public static string ObterDescricao(this AguardandoIntegracao etapa)
        {
            switch (etapa)
            {
                case AguardandoIntegracao.Confirmado: return "Confirmado";
                case AguardandoIntegracao.Aguardando: return "Aguardando";
                default: return string.Empty;
            }
        }
    }
}

