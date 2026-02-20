using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO_MOBILE", EntityName = "NotificacaoMobile", Name = "Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile", NameType = typeof(NotificacaoMobile))]
    public class NotificacaoMobile : EntidadeBase, IEquatable<NotificacaoMobile>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NML_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Assunto", Column = "NML_ASSUNTO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "NML_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NML_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NML_TIPO_LANCAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoNotificacaoMobile), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoNotificacaoMobile TipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Usuarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTIFICACAO_MOBILE_USUARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NML_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotificacaoMobileUsuario", Column = "NMU_CODIGO")]
        public virtual IList<NotificacaoMobileUsuario> Usuarios { get; set; }

        public virtual bool Equals(NotificacaoMobile other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
