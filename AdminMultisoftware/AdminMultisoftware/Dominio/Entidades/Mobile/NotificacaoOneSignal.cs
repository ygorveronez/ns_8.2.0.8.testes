using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Mobile
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO_ONE_SIGNAL", EntityName = "NotificacaoOneSignal", Name = "AdminMultisoftware.Dominio.Entidades.Mobile.NotificacaoOneSignal", NameType = typeof(NotificacaoOneSignal))]
    public class NotificacaoOneSignal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UsuarioMobile", Column = "UMB_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UsuarioMobile Motorista { get; set; }

        /// <summary>
        /// Título da notificação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Headings", Column = "NOS_HEADINGS", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Headings { get; set; }

        /// <summary>
        /// Descrição da notificação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Contents", Column = "NOS_CONTENTS", TypeType = typeof(string), NotNull = true)]
        public virtual string Contents { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NOS_TIPO", TypeType = typeof(MobileHubs), NotNull = true)]
        public virtual MobileHubs Tipo { get; set; }

        /// <summary>
        /// JSON arbritrário que vai junto para enviar informações que depende do Tipo da notificação. Por exemplo, código
        /// da carga, código da cargaEntrega, mensagem, etc.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "NOS_DATA", TypeType = typeof(string), NotNull = true)]
        public virtual string Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "NOS_DATA_CRIACO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        /// <summary>
        /// Id que o OneSignal retorna quando a notificação foi enviada com sucesso
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IdOneSignal", Column = "NOS_ID_ONE_SIGNAL", TypeType = typeof(string), NotNull = true)]
        public virtual string IdOneSignal { get; set; }

        /// <summary>
        /// Se a notificação já foi lida ou não
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Lida", Column = "NOS_LIDA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Lida { get; set; }

    }
}
