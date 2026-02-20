namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_QUANTIDADE", EntityName = "AlcadasOrdemCompra.AlcadaQuantidade", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade", NameType = typeof(AlcadaQuantidade))]

    public class AlcadaQuantidade : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ARQ_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Quantidade.ToString("n2");
            }
        }

        public virtual decimal PropriedadeAlcada
        {
            get
            {
                return this.Quantidade;
            }
            set
            {
                this.Quantidade = value;
            }
        }
    }
}