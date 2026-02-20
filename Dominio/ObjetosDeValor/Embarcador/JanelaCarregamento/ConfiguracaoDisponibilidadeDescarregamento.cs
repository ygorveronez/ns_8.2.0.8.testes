namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class ConfiguracaoDisponibilidadeDescarregamento
    {
        #region Propriedades

        public int DiasLimiteParaDefinicaoHorarioDescarregamento { get; set; }

        public bool PermitirHorarioDescarregamentoComLimiteAtingido { get; set; }

        public bool PermitirHorarioDescarregamentoInferiorAoAtual { get; set; }

        public bool NaoPermitirBuscarOutroPeriodo { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public double CodigoRemetente { get; set; }

        public int CodigoTipoCarga { get; set; }

        #endregion Propriedades

        #region Construtores

        public ConfiguracaoDisponibilidadeDescarregamento()
        {
            DiasLimiteParaDefinicaoHorarioDescarregamento = 2;
        }

        #endregion Construtores
    }
}
