using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ROTA_FRETE", EntityName = "CargaRotaFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete", NameType = typeof(CargaRotaFrete))]
    public class CargaRotaFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PolilinhaRota", Column = "CRF_POLINHA_ROTA", Type = "StringClob", NotNull = false)]
        public virtual string PolilinhaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeViagemEmMinutos", Column = "CRF_TEMPO_VIAGEM_EM_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDeViagemEmMinutos { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "CRF_TIPO_ULTIMO_PONTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaRotaFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
