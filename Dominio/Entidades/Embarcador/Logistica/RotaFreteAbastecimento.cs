using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_ABASTECIMENTO", EntityName = "RotaFreteAbastecimento", Name = "Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento", NameType = typeof(RotaFreteAbastecimento))]
    public class RotaFreteAbastecimento : EntidadeBase, IEquatable<RotaFreteAbastecimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RFA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PreAbastecimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_ABASTECIMENTO_PRE_ABASTECIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RFA_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RotaFreteAbastecimentoPreAbastecimento")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento> PreAbastecimentos { get; set; }

        public virtual bool Equals(RotaFreteAbastecimento other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
