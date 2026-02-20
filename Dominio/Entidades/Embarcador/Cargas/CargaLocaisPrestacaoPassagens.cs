using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_LOCAIS_PRESTACAO_PASSAGENS", EntityName = "CargaLocaisPrestacaoPassagens", Name = "Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens", NameType = typeof(CargaLocaisPrestacaoPassagens))]

    public class CargaLocaisPrestacaoPassagens : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>
    {
        public CargaLocaisPrestacaoPassagens() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaLocaisPrestacao", Column = "CLP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaLocaisPrestacao CargaLocaisPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_PASSAGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDePassagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "CPP_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens)this.MemberwiseClone();
        }
        public virtual bool Equals(CargaLocaisPrestacaoPassagens other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
