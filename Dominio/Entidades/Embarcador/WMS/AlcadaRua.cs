namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE_DEPOSITO_RUA", EntityName = "AlcadaRua", Name = "Dominio.Entidades.Embarcador.WMS.AlcadaRua", NameType = typeof(AlcadaRua))]
    public class AlcadaRua : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraDescarte RegraDescarte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoRua", Column = "DER_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DepositoRua Rua { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Rua?.Descricao ?? string.Empty;
            }
        }

        public virtual DepositoRua PropriedadeAlcada
        {
            get
            {
                return this.Rua;
            }
            set
            {
                this.Rua = value;
            }
        }
    }
}