namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PAGAMENTO_AGREGADO_VALOR", EntityName = "AlcadaValorPagamentoAgregado", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado", NameType = typeof(AlcadaValorPagamentoAgregado))]
    public class AlcadaValorPagamentoAgregado : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPagamentoAgregado", Column = "RPA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraPagamentoAgregado RegraPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPV_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Valor.ToString("n4");
            }
        }

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
    }
}