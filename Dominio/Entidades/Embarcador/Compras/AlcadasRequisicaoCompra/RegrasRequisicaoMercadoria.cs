using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_REQUISICAO_MERCADORIA", EntityName = "RegrasRequisicaoMercadoria", Name = "Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria", NameType = typeof(RegrasRequisicaoMercadoria))]
    public class RegrasRequisicaoMercadoria : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RRM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRM_ATIVO_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRM_ATIVO_MOTIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_REQUISICAO_MERCADORIA_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_REQUISICAO_MERCADORIA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Compras.AlcadaFilial", Column = "ARF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadaFilial> AlcadasFilial { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasMotivo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_REQUISICAO_MERCADORIA_MOTIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaMotivo", Column = "ARM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadaMotivo> AlcadasMotivo { get; set; }
        // --------------------------------------

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }
    }
}
