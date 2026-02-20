namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitoramentoEventoModeloEmail
    {
        Padrao = 0,
        ModeloDPA = 1
    }

    public static class MonitoramentoEventoModeloEmailHelper
    {
        public static string ObterDescricao(this MonitoramentoEventoModeloEmail monitoramentoEventoModeloEmail)
        {
            switch (monitoramentoEventoModeloEmail)
            {
                case MonitoramentoEventoModeloEmail.Padrao: return "Padrão";
                case MonitoramentoEventoModeloEmail.ModeloDPA: return "Modelo DPA";
                default: return string.Empty;
            }
        }
    }
}
