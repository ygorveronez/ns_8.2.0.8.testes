using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO_RETIRADA_PRODUTO", EntityName = "NotificacaoRetiradaProduto", Name = "Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto", NameType = typeof(NotificacaoRetiradaProduto))]
    public class NotificacaoRetiradaProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NRP_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "NRP_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "NRP_EMAIL", Type = "StringClob", NotNull = true)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTIFICACAO_RETIRADA_PRODUTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Destinatarios { get; set; }

        public virtual string EmailFormatado
        {
            get { return System.Text.RegularExpressions.Regex.Replace(Email, "<.*?>", string.Empty); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao ? "Ativo" : "Inativo"; }
        }
    }
}
