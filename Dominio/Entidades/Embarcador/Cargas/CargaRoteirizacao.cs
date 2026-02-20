using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ROTEIRIZACAO", EntityName = "CargaRoteirizacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao", NameType = typeof(CargaRoteirizacao))]
    public class CargaRoteirizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaKM", Column = "CRT_DISTANCIA_KM", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal DistanciaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CRT_TIPO_ROTA", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "CRT_TIPO_ULTIMO_PONTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }


        public virtual bool Equals(CargaRoteirizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
