using Dominio.Entidades.Embarcador.Pedidos;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_MOTIVO_CHAMADO", EntityName = "GrupoMotivoChamado", Name = "Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado", NameType = typeof(GrupoMotivoChamado))]
    public class GrupoMotivoChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_GERA_OCORRENCIA_NORMAL", NotNull = true, TypeType = typeof(bool))]
        public virtual bool GeraOcorrenciaNormal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_GERA_CARGA_AVULSA", NotNull = true, TypeType = typeof(bool))]
        public virtual bool GeraCargaAvulsa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_NECESSARIO_APROVACAO_CRIACAO_CARGA_AVULSA", NotNull = true, TypeType = typeof(bool))]
        public virtual bool NecessarioAprovacaoCriacaoCargaAvulsa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_GERA_CARGA_REVERSA", NotNull = true, TypeType = typeof(bool))]
        public virtual bool GeraCargaReversa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_NAO_PERMITE_LANCAMENTO_MANUAL", NotNull = true, TypeType = typeof(bool))]
        public virtual bool NaoPermiteLancamentoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_SINISTRO", NotNull = true, TypeType = typeof(bool))]
        public virtual bool Sinistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMC_RECEBE_OCORRENCIA_ERP", NotNull = false, TypeType = typeof(bool))]
        public virtual bool RecebeOcorrenciaERP { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_MOTIVO_CHAMADO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GMC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<TipoOperacao> TiposOperacao { get; set; }

    }
}
