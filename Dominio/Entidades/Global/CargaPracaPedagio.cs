using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRACAS_PEDAGIO", EntityName = "CargaPracaPedagio", Name = "Dominio.Entidades.CargaPracaPedagio", NameType = typeof(CargaPracaPedagio))]
    public class CargaPracaPedagio : EntidadeBase, IEquatable<CargaPracaPedagio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PracaPedagio", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PracaPedagio PracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EixosSuspenso", Column = "RFP_EIXOS_SUSPENSO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso EixosSuspenso { get; set; }

        public virtual bool Equals(CargaPracaPedagio other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
