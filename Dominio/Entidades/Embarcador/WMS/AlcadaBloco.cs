namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE_DEPOSITO_BLOCO", EntityName = "AlcadaBloco", Name = "Dominio.Entidades.Embarcador.WMS.AlcadaBloco", NameType = typeof(AlcadaBloco))]
    public class AlcadaBloco : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraDescarte RegraDescarte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoBloco", Column = "DEB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DepositoBloco Bloco { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Bloco?.Descricao ?? string.Empty;
            }
        }

        public virtual DepositoBloco PropriedadeAlcada
        {
            get
            {
                return this.Bloco;
            }
            set
            {
                this.Bloco = value;
            }
        }
    }
}