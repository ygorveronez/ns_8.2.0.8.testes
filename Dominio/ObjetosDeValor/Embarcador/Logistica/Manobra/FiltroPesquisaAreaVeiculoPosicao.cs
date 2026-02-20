namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAreaVeiculoPosicao
    {
        public int CodigoAreaPosicao { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoTipoRetornoCarga { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.TipoAreaVeiculo? TipoAreaVeiculo { get; set; }
    }
}
