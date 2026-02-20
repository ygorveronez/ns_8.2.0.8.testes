namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaPagamentoMotorista
    {
        Iniciada = 1,
        AgAutorizacao = 2,
        Integracao = 3
    }

    public static class EtapaPagamentoMotoristaHelper
    {
        public static string ObterDescricao(this EtapaPagamentoMotorista etapa)
        {
            switch (etapa)
            {
                case EtapaPagamentoMotorista.Iniciada: return "Iniciada";
                case EtapaPagamentoMotorista.AgAutorizacao: return "Aguardando Autorização";
                case EtapaPagamentoMotorista.Integracao: return "Integração";
                default: return string.Empty;
            }
        }
    }
}
