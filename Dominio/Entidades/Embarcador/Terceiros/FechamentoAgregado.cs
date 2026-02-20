using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_AGREGADO", EntityName = "FechamentoAgregado", Name = "Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado", NameType = typeof(FechamentoAgregado))]
    public class FechamentoAgregado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>
    {
        public FechamentoAgregado() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FAG_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CRIACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FechamentoAgregadoCIOTs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_AGREGADO_CIOT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoAgregadoCIOT", Column = "FAC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> FechamentoAgregadoCIOTs { get; set; }

        // DESCONTINUADO - Não foi removido para compatibilidade com registros legados
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }

        // DESCONTINUADO - Não foi removido para compatibilidade com registros legados
        [NHibernate.Mapping.Attributes.Property(0, Column = "FAG_CONSOLIDADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Consolidado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAG_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        public virtual FechamentoAgregado Clonar()
        {
            return (FechamentoAgregado)this.MemberwiseClone();
        }

        public virtual bool Equals(FechamentoAgregado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}