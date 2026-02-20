using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaRelatorioAbastecimento
    {
        public List<TipoRecebimentoAbastecimento> TiposRecebimento { get; set; }
        public int CodigoEquipamento { get; set; }
        public List<int> CodigosProdutos { get; set; }
        public Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem SituacaoAcerto { get; set; }
        public double CodigoProprietario { get; set; }
        public string TipoPropriedade { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }    
        public DateTime DataBaseCRTInicial { get; set; }
        public DateTime DataBaseCRTFinal { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoVeiculo { get; set; }
        public string StatusAbastecimento { get; set; }
        public int TipoAbastecimentoInternoExterno { get; set; }
        public int CodigoSegmento { get; set; }
        public double Fornecedor { get; set; }
        public int CodigoMotorista { get; set; }
        public TipoAbastecimento TipoAbastecimento { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public int CodigoCentroResultado { get; set; }
        public string UFFornecedor { get; set; }
        public int ModeloVeicularCarga { get; set; }
        public List<int> Paises { get; set; }
        public List<MoedaCotacaoBancoCentral> Moedas { get; set; }
        public int CodigoLocalArmazenamento { get; set; }
        public int CodigoOrdemCompra { get; set; }
    }
}
