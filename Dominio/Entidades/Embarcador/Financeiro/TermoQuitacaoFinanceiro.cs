using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO_FINANCEIRO", EntityName = "TermoQuitacaoFinanceiro", Name = "Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro", NameType = typeof(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro))]
    public class TermoQuitacaoFinanceiro : EntidadeBase,IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TQU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalGeralPagamento", Column = "TQU_TOTAL_GERAL_PAGAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalGeralPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTermo", Column = "TQU_NUMERO_TERMO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTermo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSaldoEmAberto", Column = "TQU_TOTAL_SALDO_EM_ABERTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalSaldoEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "TQU_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "TQU_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "TQU_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTermoQuitacao", Column = "TQU_SITUACAO_TERMO_QUITACAO", TypeType = typeof(SitaucaoTermoQuitacao), NotNull = false)]
        public virtual SituacaoTermoQuitacaoFinanceiro SituacaoTermoQuitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAprovacaoTransportador", Column = "TQU_SITUACAO_APROVACAO_TRANSPORTADOR", TypeType = typeof(SituacaoAprovacaoTermoQuitacaoTransportador), NotNull = false)]
        public virtual SituacaoAprovacaoTermoQuitacaoTransportador SituacaoAprovacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentosEDescontosViaCreditoEmConta", Column = "TQU_PAGAMENTO_DESCONTO_VIA_CREDITO_EM_CONTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PagamentosEDescontosViaCreditoEmConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentosEDescontosViaConfiming", Column = "TQU_PAGAMENTO_DESCONTO_VIA_CONFORMING", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PagamentosEDescontosViaConfiming { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalAdiantamento", Column = "TQU_TOTAL_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TotalAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CreditoEmConta", Column = "TQU_CREDITO_EM_CONTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CreditoEmConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotasCompensadasAdiantamentos", Column = "TQU_NOTAS_COMPENSADAS_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal NotasCompensadasAdiantamentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaRejeicao", Column = "TQU_JUSTIFICATIVA_REJEICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string JustificativaRejeicao { get; set; }

        public virtual string Descricao { get { return this.Transportador.Descricao + " - " + this.TotalGeralPagamento.ToString("n2"); } }
    }
}
