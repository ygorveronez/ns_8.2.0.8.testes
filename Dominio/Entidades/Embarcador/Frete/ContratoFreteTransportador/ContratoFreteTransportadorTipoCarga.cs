namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_TIPO_CARGA", EntityName = "ContratoFreteTransportadorTipoCarga", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga", NameType = typeof(ContratoFreteTransportadorTipoCarga))]
    public class ContratoFreteTransportadorTipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoDeCarga { get; set; }


        public virtual string Descricao
        {
            get
            {
                return ContratoFrete.Descricao + " - " + TipoDeCarga.Descricao;
            }
        }
    }
}
