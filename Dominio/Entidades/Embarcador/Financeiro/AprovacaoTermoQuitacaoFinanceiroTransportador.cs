using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APROVACAO_TERMO_QUITACAO_FINANCEIRO_TRANSPORTADOR", EntityName = "AprovacaoTermoQuitacaoFinanceiroTransportador", Name = "Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador", NameType = typeof(AprovacaoTermoQuitacaoFinanceiroTransportador))]
    public class AprovacaoTermoQuitacaoFinanceiroTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "ATQ_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; } 
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ATQ_SITUACAO", TypeType = typeof(SituacaoAprovacaoTermoQuitacaoTransportador), NotNull = false)]
        public virtual SituacaoAprovacaoTermoQuitacaoTransportador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro TermoQuitacaoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaNotificacaoEmail", Column = "ATQ_DATA_ULTIMA_NOTIFICACAO_EMAIL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaNotificacaoEmail { get; set; }

    }
}
