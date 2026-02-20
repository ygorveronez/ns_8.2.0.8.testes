using System;

namespace Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_EMBARQUE_SITUACAO_HISTORICO", EntityName = "OrdemEmbarqueSituacaoHistorico", Name = "Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico", NameType = typeof(OrdemEmbarqueSituacaoHistorico))]
    public class OrdemEmbarqueSituacaoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OSH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemEmbarque", Column = "OEM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemEmbarque OrdemEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "OSH_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "OSH_DATA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemEmbarqueSituacao", Column = "OES_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemEmbarqueSituacao Situacao { get; set; }
    }
}
