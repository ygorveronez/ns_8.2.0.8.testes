using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioTituloAcrescimoDesconto
    {
        public int CodigoFatura { get; set; }
        public int CodigoBordero { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public List<int> CodigosJustificativa { get; set; }
        public EnumTipoAcrescimoDescontoTituloDocumento? Tipo { get; set; }
        public TipoJustificativa? TipoJustificativa { get; set; }
        public StatusTitulo SituacaoTitulo { get; set; }
        public DateTime DataBaseLiquidacaoInicial { get; set; }
        public DateTime DataBaseLiquidacaoFinal { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataLiquidacaoInicial { get; set; }
        public DateTime DataLiquidacaoFinal { get; set; }
        public int CodigoCTe { get; set; }
        public TipoTitulo TipoDeTitulo { get; set; }
    }
}
