using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TABELA_FRETE_ROTA", EntityName = "CargaTabelaFreteRota", Name = "Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota", NameType = typeof(CargaTabelaFreteRota))]
    public class CargaTabelaFreteRota : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteRota", Column = "TFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteRota TabelaFreteRota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteRotaTipoCargaModeloVeicularCarga", Column = "TTM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga TabelaFreteRotaTipoCargaModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaFreteFilialEmissora", Column = "TTM_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFreteFilialEmissora { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaTabelaFreteRota other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
