namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_ALERTA", EntityName = "EmailAlerta", Name = "Dominio.Entidades.Embarcador.Email.EmailAlerta", NameType = typeof(EmailAlerta))]
    public class EmailAlerta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", Column = "TEA_TIPO_ALERTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaEmail), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaEmail TipoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailPrincipal", Column = "TEA_CONTA_EMAIL_PRINCIPAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EmailPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsCopia", Column = "TEA_CONTAS_DE_EMAIL_COPIA", Type = "StringClob", NotNull = false)]
        public virtual string EmailsCopia { get; set; }
    }
}
