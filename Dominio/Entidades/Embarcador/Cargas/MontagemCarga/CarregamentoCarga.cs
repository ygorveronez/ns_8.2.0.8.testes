using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_CARGA", EntityName = "CarregamentoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga", NameType = typeof(CarregamentoCarga))]
    public class CarregamentoCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carga.CodigoCargaEmbarcador;
            }
        }

        public virtual bool Equals(CarregamentoCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
