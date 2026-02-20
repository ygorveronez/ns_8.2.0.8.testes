namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_TERCEIROS", EntityName = "AlcadaContratoFreteTerceiroTerceiros", Name = "Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros", NameType = typeof(AlcadaContratoFreteTerceiroTerceiros))]
    public class AlcadaContratoFreteTerceiroTerceiros : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ART_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraContratoFreteTerceiro", Column = "RCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraContratoFreteTerceiro RegraContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Terceiro { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Terceiro?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Cliente PropriedadeAlcada
        {
            get
            {
                return this.Terceiro;
            }
            set
            {
                this.Terceiro = value;
            }
        }
    }
}