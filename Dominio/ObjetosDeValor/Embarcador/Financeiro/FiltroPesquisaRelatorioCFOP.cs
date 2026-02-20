using Dominio.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioCFOP
    {
        public int CodigoEmpresa { get; set; }
        public int CodigoCFOP { get; set; }
        public string Extensao { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public TipoCFOP? TipoCFOP { get; set; }
        public bool GerarEstoque { get; set; }
        public bool RealizaRateioDespesaVeiculo { get; set; }

    }
}
