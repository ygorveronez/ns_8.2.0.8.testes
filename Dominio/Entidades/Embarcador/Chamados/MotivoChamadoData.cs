namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_CHAMADO_DATA", EntityName = "MotivoChamadoData", Name = "Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData", NameType = typeof(MotivoChamadoData))]
    public class MotivoChamadoData : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCD_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCD_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Obrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCD_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }
    }
}
