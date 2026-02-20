namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRoteirizadorIntegracao
    {
        NaoIntegrado = 0,
        Integrado = 1,
        Cancelado = 2,
        FalhaIntegrar = 3
    }

    public static class SituacaoRoteirizadorIntegracaoHelper
    {
        public static string ObterDescricao(this SituacaoRoteirizadorIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoRoteirizadorIntegracao.Cancelado: return "Cancelado";
                case SituacaoRoteirizadorIntegracao.Integrado: return "Integrado";
                case SituacaoRoteirizadorIntegracao.FalhaIntegrar: return "FalhaIntegrar";
                default: return "Não Integrado";
            }
        }

        public static string ObterCorLinha(this SituacaoRoteirizadorIntegracao situacao)
        {
            return situacao switch
            {
                SituacaoRoteirizadorIntegracao.Cancelado => Utilidades.Cores.ObterCorPorPencentual(Cores.Vermelho.Descricao(), percentual: 200),
                SituacaoRoteirizadorIntegracao.Integrado => Utilidades.Cores.ObterCorPorPencentual(Cores.Verde.Descricao(), percentual: 90),
                _ => string.Empty,
            };
        }
    }
}
