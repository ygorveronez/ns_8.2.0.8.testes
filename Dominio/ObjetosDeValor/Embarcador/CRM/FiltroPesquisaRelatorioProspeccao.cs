using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.CRM
{
    public class FiltroPesquisaRelatorioProspeccao
    {
        public DateTime DataLancamentoInicial { get; set; }
        public DateTime DataLancamentoFinal { get; set; }
        public DateTime DataRetornoInicial { get; set; }
        public DateTime DataRetornoFinal { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoCidade { get; set; }
        public int CodigoCliente { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoOrigemContato { get; set; }
        public string CNPJ { get; set; }
        public bool? Faturado { get; set; }
        public TipoContatoAtendimento? TipoContato { get; set; }
        public NivelSatisfacao? Satisfacao { get; set; }
        public SituacaoProspeccao? Situacao { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}