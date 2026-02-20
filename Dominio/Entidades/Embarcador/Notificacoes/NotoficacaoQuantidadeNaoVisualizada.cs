using System;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO_QUANTIDADE_NAO_VISUALIZADA", EntityName = "NotoficacaoQuantidadeNaoVisualizada", Name = "Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada", NameType = typeof(NotoficacaoQuantidadeNaoVisualizada))]
    public class NotoficacaoQuantidadeNaoVisualizada : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NQV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNaoVisualizada", Column = "NQV_QUANTIDADE_NAO_VISUALIZADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeNaoVisualizada { get; set; }

        public virtual bool Equals(NotoficacaoQuantidadeNaoVisualizada other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
