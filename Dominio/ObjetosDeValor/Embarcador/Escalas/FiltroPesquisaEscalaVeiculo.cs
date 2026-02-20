namespace Dominio.ObjetosDeValor.Embarcador.Escalas
{
    public sealed class FiltroPesquisaEscalaVeiculo
    {
        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoVeiculo { get; set; }

        public Enumeradores.SituacaoEscalaVeiculo? Situacao { get; set; }

        public bool SomenteVeiculosDataPrevisaoRetornoExcedida { get; set; }
    }
}
