namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE_DEPOSITO", EntityName = "AlcadaDeposito", Name = "Dominio.Entidades.Embarcador.WMS.AlcadaDeposito", NameType = typeof(AlcadaDeposito))]
    public class AlcadaDeposito : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraDescarte RegraDescarte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Deposito", Column = "DEP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Deposito Deposito { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Deposito?.Descricao ?? string.Empty;
            }
        }

        public virtual Deposito PropriedadeAlcada
        {
            get
            {
                return this.Deposito;
            }
            set
            {
                this.Deposito = value;
            }
        }
    }
}