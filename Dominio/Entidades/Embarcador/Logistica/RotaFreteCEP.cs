namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_CEP", EntityName = "RotaFreteCEP", Name = "Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP", NameType = typeof(RotaFreteCEP))]
    public class RotaFreteCEP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ROC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROC_CEP_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROC_CEP_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int CEPFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROC_LEAD_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int? LeadTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROC_PERCENTUAL_ADVALOREM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? PercentualADValorem { get; set; }

        public virtual string CEPInicialFormatado
        {
            get
            {
                return CEPInicial.ToString(@"00\.000\-000");
            }
        }

        public virtual string CEPFinalFormatado
        {
            get
            {
                return CEPFinal.ToString(@"00\.000\-000");
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
