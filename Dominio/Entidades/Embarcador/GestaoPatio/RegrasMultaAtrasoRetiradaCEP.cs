namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_CEP", EntityName = "RegrasMultaAtrasoRetiradaCEP", Name = "Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP", NameType = typeof(RegrasMultaAtrasoRetiradaCEP))]
    public class RegrasMultaAtrasoRetiradaCEP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasMultaAtrasoRetirada", Column = "RMA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasMultaAtrasoRetirada RegrasMultaAtrasoRetirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RMC_CEP_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RMC_CEP_FINAL", TypeType = typeof(int), NotNull = true)]
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
