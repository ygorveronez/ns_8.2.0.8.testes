namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TRANSPORTADOR", EntityName = "AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Frete.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraContratoFreteTransportador", Column = "RCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraContratoFreteTransportador RegraContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Transportador?.Descricao ?? string.Empty;
            }
        }

        public virtual Empresa PropriedadeAlcada
        {
            get
            {
                return this.Transportador;
            }
            set
            {
                this.Transportador = value;
            }
        }
    }
}