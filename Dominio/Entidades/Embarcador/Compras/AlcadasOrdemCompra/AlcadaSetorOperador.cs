namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_SETOR_OPERADOR", EntityName = "AlcadasOrdemCompra.AlcadaSetorOperador", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador", NameType = typeof(AlcadaSetorOperador))]
    public class AlcadaSetorOperador : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Setor Setor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Setor?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Setor PropriedadeAlcada
        {
            get
            {
                return this.Setor;
            }
            set
            {
                this.Setor = value;
            }
        }
    }
}