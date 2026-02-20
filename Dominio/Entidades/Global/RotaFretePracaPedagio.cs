using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_PRACAS_PEDAGIO", EntityName = "RotaFretePracaPedagio", Name = "Dominio.Entidades.RotaFretePracaPedagio", NameType = typeof(RotaFretePracaPedagio))]
    public class RotaFretePracaPedagio : EntidadeBase, IEquatable<RotaFretePracaPedagio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PracaPedagio", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PracaPedagio PracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EixosSuspenso", Column = "RFP_EIXOS_SUSPENSO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso EixosSuspenso { get; set; }

        public virtual bool Equals(RotaFretePracaPedagio other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
