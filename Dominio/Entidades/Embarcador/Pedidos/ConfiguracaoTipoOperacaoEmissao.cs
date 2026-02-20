namespace Dominio.Entidades.Embarcador.Pedidos
{
    /// <summary>
    /// Da aba Configuração Emissão, que é um componente com o cadastro do Grupo de Pessoas e Pessoas
    /// </summary>
    /// 
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_EMISSAO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoEmissao", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissao", NameType = typeof(ConfiguracaoTipoOperacaoEmissao))]
    public class ConfiguracaoTipoOperacaoEmissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTE_UTILIZAR_PRIMEIRA_UNIDADE_MEDIDA_PESO_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarSequencialObservacaoCTe", Column = "CTE_GERAR_SEQUENCIAL_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarSequencialObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTE_PREFIXO_OBSERVACAO_SEQUENCIAL_CTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PrefixoObservacaoSequencialCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarDocumentosEmitidosQuandoEntregaForConfirmada", Column = "TOP_LIBERAR_DOCUMENTOS_EMITIDOS_QUANTO_ENTREGA_FOR_CONFIRMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarDocumentosEmitidosQuandoEntregaForConfirmada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarComposicaoRateioCarga", Column = "CTE_DISPONIBILIZAR_COMPOSICAO_RATEIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarComposicaoRateioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbarCTeImportadoDoEmbarcador", Column = "CTE_AVERBAR_CTE_IMPORTADO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarCTeImportadoDoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTE_FATOR_CUBAGEM_RATEIO_FORMULA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal? FatorCubagemRateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTE_TIPO_USO_FATOR_CUBAGEM_RATEIO_FORMULA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem? TipoUsoFatorCubagemRateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoReceita", Column = "CTE_TIPO_RECEITA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoReceita), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoReceita? TipoReceita { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Emissão"; }
        }
    }
}
