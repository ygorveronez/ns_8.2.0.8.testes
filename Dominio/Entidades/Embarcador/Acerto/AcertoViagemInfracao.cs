using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VEICULO_TABELA_INFRACAO", EntityName = "AcertoViagemInfracao", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao", NameType = typeof(AcertoViagemInfracao))]
    public class AcertoViagemInfracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.Infracao Infracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InfracaoAssinada", Column = "AVI_INFRACAO_ASSINADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InfracaoAssinada { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(AcertoViagemInfracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
