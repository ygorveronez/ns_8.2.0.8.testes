using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public sealed class RelatorioCondicoesPagamentoTransportador
    {
        public int CodigoCPT { get; set; }
        public string RazaoSocial { get; set; }
        public string CodigoIntegracao { get; set; }
        private string CNPJ { get; set; }
        public string SiglaUF { get; set; }
        private int DiaEmissaoLimite { get; set; }
        private int DiaMes { get; set; }
        public int DiasDePrazoPagamento { get; set; }
        private DiaSemana DiaSemana { get; set; }
        private TipoPrazoPagamento TipoPrazoPagamento { get; set; }
        private bool VencimentoForaMes { get; set; }
        public string TipoCargaDescricao { get; set; }
        public string TipoOperacaoDescricao { get; set; }


        public string CNPJFormatado
        {
            get { return CNPJ.ObterCnpjFormatado(); }
        }

        public string DiaMesFormatado
        {
            get { return this.DiaMes == 0 ? String.Empty : this.DiaMes.ToString(); }
        }

        public string DiaEmissaoLimiteFormatado
        {
            get {
                return this.DiaEmissaoLimite == 0 ? String.Empty : this.DiaEmissaoLimite.ToString(); 
            }
        }

        public string DiaSemanaDescricao
        {
            get { return this.DiaSemana.ObterDescricao(); }
        }

        public string TipoPrazoPagamentoDescricao
        {
            get { return this.TipoPrazoPagamento.ObterDescricao(); }
        } 

        public string VencimentoForaMesFormatado
        {
            get { return this.VencimentoForaMes ? "Sim" : "Não"; }
        }
    }
}
