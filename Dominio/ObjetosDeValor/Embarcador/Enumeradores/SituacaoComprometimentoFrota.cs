namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoComprometimentoFrota
    {
        VeiculoAlteradoDeTrechosAnteriores = 1
    }

    public static class SituacaoComprometimentoFrotaHelper
    {
        public static string ObterDescricao(this SituacaoComprometimentoFrota situacaoComprometimentoFrota)
        {
            switch (situacaoComprometimentoFrota)
            {
                case SituacaoComprometimentoFrota.VeiculoAlteradoDeTrechosAnteriores: return "Veículo alterado em trechos anteriores";
                default: return string.Empty;
            }
        }
    }
}