namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_CEP_DESTINO", EntityName = "TabelaFreteClienteCEPDestino", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino", NameType = typeof(TabelaFreteClienteCEPDestino))]
    public class TabelaFreteClienteCEPDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCD_CEP_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCD_CEP_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCD_DIAS_UTEIS", TypeType = typeof(int), NotNull = false)]
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
