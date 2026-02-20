namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPOSTO_CONTRATO_FRETE_INSS", EntityName = "INSSImpostoContratoFrete", Name = "Dominio.Entidades.INSSImpostoContratoFrete", NameType = typeof(INSSImpostoContratoFrete))]
    public class INSSImpostoContratoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImpostoContratoFrete", Column = "ICF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImpostoContratoFrete Imposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorInicial", Column = "ICI_VALOR_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFinal", Column = "ICI_VALOR_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAplicar", Column = "ICI_PERCENTUAL_APLICAR", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = true)]
        public virtual decimal PercentualAplicar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAplicarContratante", Column = "ICI_PERCENTUAL_APLICAR_CONTRATANTE", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualAplicarContratante { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Imposto?.Empresa?.Descricao ?? "") + " - Impostos INSS";
            }
        }

        public virtual string DescricaoFaixa
        {
            get
            {
                return ValorInicial.ToString("n2") + " até " + ValorFinal.ToString("n2");
            }
        }
    }
}
