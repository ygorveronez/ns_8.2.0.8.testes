using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_ADIANTAMENTO", EntityName = "AcertoAdiantamento", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento", NameType = typeof(AcertoAdiantamento))]
    public class AcertoAdiantamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ADI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }        

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(AcertoAdiantamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}