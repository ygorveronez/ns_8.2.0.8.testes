using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroPesquisaPagamento
    {
        public int Filial { get; set; }
        public int Carga { get; set; }
        public int Ocorrencia { get; set; }
        public int Numero { get; set; }
        public int NumeroDOC { get; set; }
        public int LocalidadePrestacao { get; set; }
        public double Tomador { get; set; }
        public int ModeloDocumentoFiscal { get; set; }
        public bool PagamentoLiberado { get; set; }
        public int TipoOperacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento? SituacaoPagamento { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public int Empresa { get; set; }
    }
}
