namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class ConfiguracaoCarregamento
    {
        #region Propriedades

        public int DiasLimiteParaDefinicaoHorarioCarregamento { get; set; }

        public bool PermitirCapacidadeCarregamentoExcedida { get; set; }

        public bool PermitirHorarioCarregamentoComLimiteAtingido { get; set; }

        public bool PermitirHorarioCarregamentoInferiorAoAtual { get; set; }

        #endregion Propriedades
    }
}
