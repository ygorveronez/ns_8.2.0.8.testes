namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class ConfiguracaoDescarregamento
    {
        public bool PermitirHorarioDescarregamentoComLimiteAtingido { get; set; }

        public bool PermitirHorarioDescarregamentoInferiorAoAtual { get; set; }

        public bool NaoPermitirBuscarOutroPeriodo { get; set; }
    }
}
