namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_CONTRATO_FRETE_VALOR_CONTRATO", EntityName = "RegrasContratoValorContrato", Name = "Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato", NameType = typeof(RegrasContratoValorContrato))]
    public class RegrasContratoValorContrato : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraContratoFreteTransportador", Column = "RCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraContratoFreteTransportador RegraContratoFreteTransportador { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "RVC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual decimal PropriedadeAlcada
        {
            get
            {
                return this.Valor;
            }
            set
            {
                this.Valor = value;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Valor.ToString("n2");
            }
        }

    }

}
