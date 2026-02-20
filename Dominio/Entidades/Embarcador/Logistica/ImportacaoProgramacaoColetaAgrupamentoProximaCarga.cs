namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTACAO_PROGRAMACAO_COLETA_AGRUPAMENTO_PROXIMA_CARGA", EntityName = "ImportacaoProgramacaoColetaAgrupamentoProximaCarga", Name = "Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamentoProximaCarga", NameType = typeof(ImportacaoProgramacaoColetaAgrupamentoProximaCarga))]
    public class ImportacaoProgramacaoColetaAgrupamentoProximaCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoProgramacaoColetaAgrupamento", Column = "IPA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImportacaoProgramacaoColetaAgrupamento ImportacaoProgramacaoColetaAgrupamento { get; set; }
    }
}
