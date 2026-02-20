using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_COMPLEMENTO_INFO_PRODUTO", DynamicUpdate = true, EntityName = "CargaCTeComplementoInfoProduto", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoProduto", NameType = typeof(CargaCTeComplementoInfoProduto))]
    public class CargaCTeComplementoInfoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CARGA_CTE_COMPLEMENTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTeComplementado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CTeProdutos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_COMPLEMENTO_INFO_PRODUTO_CTE_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeProduto", Column = "CTP_CODIGO")]
        public virtual ICollection<CTeProduto> CTeProdutos { get; set; }

        public virtual bool Equals(CargaCTeComplementoInfoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
