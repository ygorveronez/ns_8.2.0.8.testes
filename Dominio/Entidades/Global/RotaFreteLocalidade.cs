using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_LOCALIDADE", EntityName = "RotaFreteLocalidade", Name = "Dominio.Entidades.RotaFreteLocalidade", NameType = typeof(RotaFreteLocalidade))]
    public class RotaFreteLocalidade : EntidadeBase, IEquatable<RotaFreteLocalidade>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFL_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        public virtual bool Equals(RotaFreteLocalidade other)
        {
            return (Localidade.Codigo == other.Localidade.Codigo && Ordem == other.Ordem);
        }

        public override int GetHashCode()
        {
            int hashLocalidade = Localidade == null ? 0 : Localidade.Codigo.GetHashCode();
            int hashOrdem = Ordem.GetHashCode();

            return hashLocalidade ^ hashOrdem;
        }
    }
}
