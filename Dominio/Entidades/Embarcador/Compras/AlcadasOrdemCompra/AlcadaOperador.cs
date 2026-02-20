namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_COMPRA_OPERADOR", EntityName = "AlcadasOrdemCompra.AlcadaOperador", Name = "Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador", NameType = typeof(AlcadaOperador))]
    public class AlcadaOperador : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasOrdemCompra RegrasOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Operador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Operador?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Usuario PropriedadeAlcada
        {
            get
            {
                return this.Operador;
            }
            set
            {
                this.Operador = value;
            }
        }
    }
}