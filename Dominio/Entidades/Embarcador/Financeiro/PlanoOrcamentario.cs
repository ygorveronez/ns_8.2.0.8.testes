using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_ORCAMENTARIO", EntityName = "PlanoOrcamentario", Name = "Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario", NameType = typeof(PlanoOrcamentario))]
    public class PlanoOrcamentario : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBase", Column = "POR_DATA_BASE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "POR_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "POR_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Repetir", Column = "POR_REPETIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Repetir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dividir", Column = "POR_DIVIDIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Dividir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Periodicidade", Column = "POR_PERIODICIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Periodicidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Periodicidade Periodicidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOcorrencia", Column = "POR_NUMERO_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Contas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PLANO_ORCAMENTARIO_CONTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "POR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PlanoOrcamentarioConta", Column = "POC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta> Contas { get; set; }

        public virtual bool Equals(PlanoOrcamentario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
