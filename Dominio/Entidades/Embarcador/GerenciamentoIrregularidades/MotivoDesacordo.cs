namespace Dominio.Entidades.Embarcador.GerenciamentoIrregularidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_DESACORDO", EntityName = "MotivoDesacordo", Name = "Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo", NameType = typeof(MotivoDesacordo))]
    public class MotivoDesacordo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTD_MOTIVO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTD_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTD_SUBSTITUI_CTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool  SubstituiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTD_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Irregularidade", Column = "IRR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade { get; set; }

    }
}
