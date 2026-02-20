namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaCTeAgrupado
    {
        EmEmissao = 0,
        Rejeitado = 1,
        Finalizado = 2,
        Cancelado = 3,
        EmCancelamento = 4,
        AgIntegracao = 5,
        FalhaIntegracao = 6,
    }

    public static class SituacaoCargaCTeAgrupadoHelper
    {
        public static string ObterDescricao(this SituacaoCargaCTeAgrupado situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaCTeAgrupado.EmEmissao: return "Em Emissão";
                case SituacaoCargaCTeAgrupado.Rejeitado: return "Rejeitado";
                case SituacaoCargaCTeAgrupado.Finalizado: return "Finalizado";
                case SituacaoCargaCTeAgrupado.Cancelado: return "Cancelado";
                case SituacaoCargaCTeAgrupado.EmCancelamento: return "Em cancelamento";
                case SituacaoCargaCTeAgrupado.AgIntegracao: return "Aguardando integrações";
                case SituacaoCargaCTeAgrupado.FalhaIntegracao: return "Falha em integração";
                default: return "";
            }
        }
    }
}
