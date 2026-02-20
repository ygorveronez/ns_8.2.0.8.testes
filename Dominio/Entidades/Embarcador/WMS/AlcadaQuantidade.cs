namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE_DEPOSITO_QUANTIDADE", EntityName = "AlcadaQuantidade", Name = "Dominio.Entidades.Embarcador.WMS.AlcadaQuantidade", NameType = typeof(AlcadaQuantidade))]
    public class AlcadaQuantidade : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraDescarte RegraDescarte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RDQ_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Quantidade.ToString("n4");
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