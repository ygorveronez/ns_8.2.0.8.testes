namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class PropriedadesGeracaoCarga
    {
        public int DiasLimiteParaDefinicaoHorarioCarregamento { get; set; }

        public bool GeradoViaWs { get; set; }

        public MontagemCarga.MontagemCargaPedidoProduto MontagemCargaPedidoProduto { get; set; }

        public bool PermitirGerarCargaSemJanelaDescarregamento { get; set; }

        public bool PermitirHorarioCarregamentoComLimiteAtingido { get; set; }

        public bool PermitirHorarioCarregamentoInferiorAoAtual { get; set; }

        public bool PermitirHorarioDescarregamentoComLimiteAtingido { get; set; }

        public bool PermitirHorarioDescarregamentoInferiorAoAtual { get; set; }

        public Entidades.Usuario Usuario { get; set; }
    }
}
