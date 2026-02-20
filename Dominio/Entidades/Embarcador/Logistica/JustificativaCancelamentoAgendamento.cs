namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_JUSTIFICATIVA_CANCELAMENTO_AGENDAMENTO", EntityName = "JustificativaCancelamentoAgendamento", Name = "Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento", NameType = typeof(JustificativaCancelamentoAgendamento))]
    public class JustificativaCancelamentoAgendamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCA_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCA_ATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCA_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

    }
}
