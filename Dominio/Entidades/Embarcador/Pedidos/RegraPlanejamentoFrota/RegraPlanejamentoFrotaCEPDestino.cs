namespace Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PLANEJAMENTO_FROTA_CEP_DESTINO", EntityName = "RegraPlanejamentoFrotaCEPDestino", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrotaDestino", NameType = typeof(RegraPlanejamentoFrotaCEPDestino))]
    public class RegraPlanejamentoFrotaCEPDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPlanejamentoFrota", Column = "RPF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraPlanejamentoFrota RegraPlanejamentoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_CEP_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_CEP_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCD_DIAS_UTEIS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasUteis { get; set; }

        public virtual string Descricao
        {
            get
            {
                return string.Format(@"{0:00\.000\-000}", CEPInicial) + " à " + string.Format(@"{0:00\.000\-000}", CEPFinal);
            }
        }
    }
}