using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.CRM
{
    public class Prospeccao
    {
        #region Propriedades

        private DateTime DataLancamento { get; set; }
        private DateTime DataRetorno { get; set; }
        public string Usuario { get; set; }
        public string Produto { get; set; }
        public string Cliente { get; set; }
        public string CNPJ { get; set; }
        public string Contato { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cidade { get; set; }
        public decimal Valor { get; set; }
        public TipoContatoAtendimento TipoContato { get; set; }
        public string OrigemContato { get; set; }
        public NivelSatisfacao Satisfacao { get; set; }
        public SituacaoProspeccao Situacao { get; set; }
        public bool Faturado { get; set; }

        #endregion

        #region  Propriedades com Regras

        public virtual string DescricaoFaturado
        {
            get { return Faturado ? "Sim" : "Não"; }
        }

        public virtual string DescricaoSatisfacao
        {
            get { return Satisfacao.ObterDescricao(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoTipoContato
        {
            get { return TipoContato.ObterDescricao(); }
        }

        public virtual string CNPJFormatado
        {
            get { return !string.IsNullOrWhiteSpace(this.CNPJ) ? String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ)) : string.Empty; }
        }
        public string DataLancamentoFormatada
        {
            get { return DataLancamento != DateTime.MinValue ? DataLancamento.ToString("dd/MM/yyyy") : ""; }
        }
        public string DataRetornoFormatada
        {
            get { return DataRetorno != DateTime.MinValue ? DataRetorno.ToString("dd/MM/yyyy") : ""; }
        }

        #endregion
    }
}
