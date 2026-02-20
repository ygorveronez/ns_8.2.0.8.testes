namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_LANCES", EntityName = "LancesCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.LancesCarregamento", NameType = typeof(LancesCarregamento))]
    public class LancesCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLanceDe", Column = "LAC_NUMERO_LANCE_DE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroLanceDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLanceAte", Column = "LAC_NUMERO_LANCE_ATE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroLanceAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PorcentagemLance", Column = "LAC_PORCENTAGEM_LANCE", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal PorcentagemLance { get; set; }
    }
}
