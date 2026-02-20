namespace Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PLANEJAMENTO_FROTA_CEP_ORIGEM", EntityName = "RegraPlanejamentoFrotaCEPOrigem", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrotaCEPOrigem", NameType = typeof(RegraPlanejamentoFrotaCEPOrigem))]
    public class RegraPlanejamentoFrotaCEPOrigem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPlanejamentoFrota", Column = "RPF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraPlanejamentoFrota RegraPlanejamentoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_CEP_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_CEP_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPFinal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return string.Format(@"{0:00\.000\-000}", CEPInicial) + " à " + string.Format(@"{0:00\.000\-000}", CEPFinal);
            }
        }
    }
}