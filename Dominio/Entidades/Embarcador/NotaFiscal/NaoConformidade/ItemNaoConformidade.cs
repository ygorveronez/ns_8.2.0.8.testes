using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ITEM_NAO_CONFORMIDADE", EntityName = "ItemNaoConformidade", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade", NameType = typeof(ItemNaoConformidade))]
    public class ItemNaoConformidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "INC_DESCRICAO", TypeType = typeof(string), NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "INC_CODIGO_INTEGRACAO", TypeType = typeof(string), NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaFiscal", Column = "INC_NOTA_FISCAL", TypeType = typeof(string), NotNull = true)]
        public virtual string NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "INC_GRUPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GrupoNC), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GrupoNC Grupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubGrupo", Column = "INC_SUBGRUPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SubGrupoNC), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SubGrupoNC SubGrupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "INC_AREA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AreaNC), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AreaNC Area { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IrrelevanteParaNC", Column = "INC_IRRELEVANTE_PARA_NC", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IrrelevanteParaNC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteContingencia", Column = "INC_PERMITE_CONTINGENCIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteContingencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRegra", Column = "INC_TIPO_REGRA", TypeType = typeof(TipoRegraNaoConformidade), NotNull = true)]
        public virtual TipoRegraNaoConformidade TipoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "INC_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoParticipante", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ITEM_NAO_CONFORMIDADE_PARTICIPANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemNaoConformidadeParticipantes", Column = "INP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes> TipoParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ITEM_NAO_CONFORMIDADE_TIPOOPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemNaoConformidadeTipoOperacao", Column = "INO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao> TipoOperacao { get; set; }
    }
}
