namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaAgendamentoPallet
    {
        Agendamento = 0,
        NFe = 1,
        Acompanhamento = 2
    }

    public static class EtapaAgendamentoPalletHelper
    {
        public static string ObterDescricao(this EtapaAgendamentoPallet situacao)
        {
            return situacao switch
            {
                EtapaAgendamentoPallet.Agendamento => "Agendamento",
                EtapaAgendamentoPallet.NFe => "NFe",
                EtapaAgendamentoPallet.Acompanhamento => "Acompanhamento",
                _ => string.Empty,
            };
        }
    }
}
