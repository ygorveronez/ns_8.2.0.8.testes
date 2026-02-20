using NHibernate.Mapping.Attributes;
using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [Class(0, Table = "T_ACERTO_ABASTECIMENTO", EntityName = "AcertoAbastecimento", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento", NameType = typeof(AcertoAbastecimento))]
    public class AcertoAbastecimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>
    {

        [Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACB_CODIGO")]
        [Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Dominio.Entidades.Abastecimento Abastecimento { get; set; }

        [Property(0, Name = "LancadoManualmente", Column = "ACB_LANCADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancadoManualmente { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Abastecimento.Descricao;
            }
        }

        public virtual bool Equals(AcertoAbastecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}