using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGIAO", EntityName = "Regiao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Localidades.Regiao", NameType = typeof(Regiao))]
    public class Regiao : EntidadeBase, IEquatable<Regiao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "REG_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "REG_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "REG_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasPrazoEntrega", Column = "REG_DIAS_PRAZO_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasPrazoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Localidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOCALIDADES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "REG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual IList<Localidade> Localidades { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegiaoPrazoEntrega", Column = "RPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega RegiaoPrazoEntrega { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(Regiao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
