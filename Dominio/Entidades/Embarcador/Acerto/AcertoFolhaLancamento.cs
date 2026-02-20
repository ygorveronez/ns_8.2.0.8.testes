using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VEICULO_FOLHA_LANCAMENTO", EntityName = "AcertoFolhaLancamento", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento", NameType = typeof(AcertoFolhaLancamento))]
    public class AcertoFolhaLancamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AFL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FolhaLancamento", Column = "FOL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RH.FolhaLancamento FolhaLancamento { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(AcertoFolhaLancamento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
