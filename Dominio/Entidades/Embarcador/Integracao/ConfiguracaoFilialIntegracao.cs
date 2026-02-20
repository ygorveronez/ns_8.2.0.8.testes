using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FILIAL_INTEGRACAO", EntityName = "ConfiguracaoFilialIntegracao", Name = "Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao", NameType = typeof(ConfiguracaoFilialIntegracao))]
    public class ConfiguracaoFilialIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposIntegracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_FILIAL_INTEGRACAO_TIPO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TiposIntegracao { get; set; }

        public virtual string DescricaoSituacao => Ativo ? "Ativo" : "Inativo";
    }
}
