using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EXPEDICAO_ESCALA", EntityName = "ExpedicaoEscala", Name = "Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala", NameType = typeof(ExpedicaoEscala))]
    public class ExpedicaoEscala : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EXE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GeracaoEscala", Column = "GES_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escalas.GeracaoEscala GeracaoEscala { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO_PRODUTO_EMBARCADOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "EXE_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOriginal", Column = "EXE_QUANTIDADE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal QuantidadeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ExpedicaoEscalaDestinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EXPEDICAO_ESCALA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EXE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExpedicaoEscalaDestino", Column = "EXD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino> ExpedicaoEscalaDestinos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.GeracaoEscala?.Descricao;
            }
        }
        public virtual bool Equals(ExpedicaoEscala other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
