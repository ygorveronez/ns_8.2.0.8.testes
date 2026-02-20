namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_CARGA", EntityName = "FechamentoFreteCarga", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga", NameType = typeof(FechamentoFreteCarga))]
    public class FechamentoFreteCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete Fechamento { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Carga nº" + (this.Carga?.CodigoCargaEmbarcador ?? "");
            }
        }
    }

}
