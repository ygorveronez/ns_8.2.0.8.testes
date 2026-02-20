namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SitaucaoTermoQuitacao
    {
        Todas = 0,
        AguardandoAprovacaoTransportador = 1,
        AprovadoTransportador = 2,
        RejeitadoTransportador = 3,
        AguardandoAprovacaoProvisao = 4,
        AprovadoProvisao = 5,
        RejeitadoProvisao = 6,
        SemRegraProvisao = 7,
        Finalizada = 9,
    }

    public static class SitacaoTermoQuitacaoHelper
    {
        public static string ObterDescricao(this SitaucaoTermoQuitacao situacao)
        {
            switch (situacao)
            {
                case SitaucaoTermoQuitacao.AguardandoAprovacaoTransportador:
                    return "Aguardando Aprovação Transportador";
                case SitaucaoTermoQuitacao.AprovadoTransportador:
                    return "Aprovado Transportador";
                case SitaucaoTermoQuitacao.RejeitadoTransportador:
                    return "Rejeitado Transportador";
                case SitaucaoTermoQuitacao.AguardandoAprovacaoProvisao:
                    return "Aguardando Aprovação Provisão";
                case SitaucaoTermoQuitacao.AprovadoProvisao:
                    return "Aprovado Provisão";
                case SitaucaoTermoQuitacao.RejeitadoProvisao:
                    return "Rejeitado Provisão";
                case SitaucaoTermoQuitacao.SemRegraProvisao:
                    return "Sem Regra Provisão";
                default:
                    return string.Empty;
            }
        }
    }
}
