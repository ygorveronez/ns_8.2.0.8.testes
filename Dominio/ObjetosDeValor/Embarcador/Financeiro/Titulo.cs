using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class Titulo
    {
        public int Protocolo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public Pessoas.Pessoa Pessoa { get; set; }
        public Pessoas.Empresa Empresa { get; set; }
        public TipoMovimento TipoMovimento { get; set; }
        public MoedaCotacaoBancoCentral Moeda { get; set; }
        public FormaTitulo FormaTitulo { get; set; }
        public StatusTitulo Situacao { get; set; }
        public decimal ValorOriginal { get; set; }
        public string Observacao { get; set; }
        public string Referencia { get; set; }
        public string Parcelas { get; set; }
        public string CodigoIntegracaoPagamento { get; set; }
        public int CodigoAbastecimento { get; set; }
    }
}
