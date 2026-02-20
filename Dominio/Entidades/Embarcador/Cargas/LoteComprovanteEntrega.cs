using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_COMPROVANTE_ENTREGA", EntityName = "LoteComprovanteEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega", NameType = typeof(LoteComprovanteEntrega))]
    public class LoteComprovanteEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "LCE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(LoteComprovanteEntrega other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}