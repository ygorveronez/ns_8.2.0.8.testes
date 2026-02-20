namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE_DEPOSITO_POSICAO", EntityName = "AlcadaPosicao", Name = "Dominio.Entidades.Embarcador.WMS.AlcadaPosicao", NameType = typeof(AlcadaPosicao))]
    public class AlcadaPosicao : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraDescarte", Column = "RED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraDescarte RegraDescarte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoPosicao", Column = "DEP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DepositoPosicao Posicao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Posicao?.Descricao ?? string.Empty;
            }
        }

        public virtual DepositoPosicao PropriedadeAlcada
        {
            get
            {
                return this.Posicao;
            }
            set
            {
                this.Posicao = value;
            }
        }
    }
}