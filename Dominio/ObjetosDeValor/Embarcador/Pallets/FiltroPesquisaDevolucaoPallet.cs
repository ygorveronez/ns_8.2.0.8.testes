using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public class FiltroPesquisaDevolucaoPallet
    {
        public int NumeroNotaFiscal { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroDevolucao { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoVeiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet? Situacao { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataBaixaInicial { get; set; }
        public DateTime DataBaixaFinal { get; set; }
        public double CpfCnpjRemetente { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public double CodigoTomador { get; set; }
        public bool NaoExibirSemNotaFiscal { get; set; }
    }
}
