namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PAGAMENTO_AGREGADO_PESSOA", EntityName = "AlcadaPessoaPagamentoAgregado", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado", NameType = typeof(AlcadaPessoaPagamentoAgregado))]
    public class AlcadaPessoaPagamentoAgregado : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPagamentoAgregado", Column = "RPA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraPagamentoAgregado RegraPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Cliente?.Descricao ?? string.Empty;
            }
        }

        public virtual Cliente PropriedadeAlcada
        {
            get
            {
                return this.Cliente;
            }
            set
            {
                this.Cliente = value;
            }
        }
    }
}