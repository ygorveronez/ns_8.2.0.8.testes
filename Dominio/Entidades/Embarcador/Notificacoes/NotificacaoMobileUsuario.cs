using System;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO_MOBILE_USUARIO", EntityName = "NotificacaoMobileUsuario", Name = "Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario", NameType = typeof(NotificacaoMobileUsuario))]
    public class NotificacaoMobileUsuario : EntidadeBase, IEquatable<NotificacaoMobileUsuario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NMU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLeitura", Column = "NMU_DATA_LEITURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Enviada", Column = "NMU_ENVIADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Enviada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotificacaoMobile", Column = "NML_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotificacaoMobile Notificacao { get; set; }

        public virtual string DescricaoEnviada
        {
            get { return Enviada ? "Sim" : "Não"; }
        }

        public virtual bool Equals(NotificacaoMobileUsuario other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
