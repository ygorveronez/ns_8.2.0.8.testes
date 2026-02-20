namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_FILIAL", EntityName = "ContratoFreteTransportadorFilial", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial", NameType = typeof(ContratoFreteTransportadorFilial))]
    public class ContratoFreteTransportadorFilial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }


        public virtual string Descricao
        {
            get
            {
                return Filial.Descricao;
            }
        }
    }
}
