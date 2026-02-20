namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_OCORRENCIA", EntityName = "FechamentoFreteOcorrencia", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia", NameType = typeof(FechamentoFreteOcorrencia))]
    public class FechamentoFreteOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete Fechamento { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia Ocorrencia { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Ocorrência nº" + (this.Ocorrencia?.NumeroOcorrencia ?? 0).ToString();
            }
        }
    }

}
