using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Obsolete] // Depreciado em 2021-08-18. Usar agora a entidade Cliente com flag Fronteira = true
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRONTEIRA", EntityName = "Fronteira", Name = "Dominio.Entidades.Embarcador.Logistica.Fronteira", NameType = typeof(Fronteira))]
    public class Fronteira : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Logistica.Fronteira>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FRO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFronteiraEmbarcador", Column = "FRO_CODIGO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoFronteiraEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fronteira", Column = "FRO_CODIGO_FRONTEIRA_OUTRO_LADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Fronteira FronteiraOutroLado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FRO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        /*
         * Clientes com a flag "Fronteira" são fronteiras. Essa propriedade faz a ligação das duas fronteiras.
         */
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAduanaDestino", Column = "FRO_CODIGO_ADUANEIRO_DESTINO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoAduanaDestino { get; set; }

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

        public virtual bool Equals(Fronteira other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
