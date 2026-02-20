namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTACAO_PROGRAMACAO_COLETA_AGRUPAMENTO", EntityName = "ImportacaoProgramacaoColetaAgrupamento", Name = "Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento", NameType = typeof(ImportacaoProgramacaoColetaAgrupamento))]
    public class ImportacaoProgramacaoColetaAgrupamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAgrupamento", Column = "IPA_NUMERO_AGRUPAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IPA_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoProgramacaoColeta", Column = "IPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImportacaoProgramacaoColeta ImportacaoProgramacaoColeta { get; set; }

        /// <summary>
        /// Para não gerar mais a programação ativa do veículo, pois irá continuar na nova
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoProgramacaoColetaAgrupamento", Column = "IPA_CODIGO_AGRUPAMENTO_NOVA_PROGRAMACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImportacaoProgramacaoColetaAgrupamento AgrupamentoNovaProgramacao { get; set; }
    }
}
