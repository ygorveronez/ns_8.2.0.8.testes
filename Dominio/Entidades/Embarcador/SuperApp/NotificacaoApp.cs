using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.SuperApp
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_NOTIFICACAO_APP", EntityName = "NotificacaoApp", Name = "Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp", NameType = typeof(NotificacaoApp))]
    public class NotificacaoApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "NOT_TIPO", TypeType = typeof(TipoNotificacaoApp), NotNull = true)]
        public virtual TipoNotificacaoApp Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "NOT_TITULO", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NOT_MENSAGEM", TypeType = typeof(string), NotNull = true, Length = 4000)]
        public virtual string Mensagem { get; set; }

        public virtual string Descricao
        {
            get { return string.Empty; }
        }
    }
}
