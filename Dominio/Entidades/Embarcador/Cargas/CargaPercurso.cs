using System;


namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PERCURSO", EntityName = "CargaPercurso", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPercurso", NameType = typeof(CargaPercurso))]
    public class CargaPercurso : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>
    {
        public CargaPercurso() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaKM", Column = "CPD_DISTANCIA_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "CPD_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CPD_TIPO_ROTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota TipoRota { get; set; }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPercurso Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPercurso)this.MemberwiseClone();
        }
        public virtual string DescricaoTipoRota
        {
            get
            {
                if (this.TipoRota == ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento)
                    return "Descarregamento";
                else
                {
                    if (this.TipoRota == ObjetosDeValor.Embarcador.Enumeradores.TipoRota.carregamento)
                        return "Carregamento";
                    else
                        return "Todos";
                }
                
            }
        }

        public virtual bool Equals(CargaPercurso other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
