
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ABASTECIMENTOS", EntityName = "ConfiguracaoAbastecimentos", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos", NameType = typeof(ConfiguracaoAbastecimentos))]
    public class ConfiguracaoAbastecimentos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_GERAR_REQUISICAO_AUTOMATICA_PARA_VEICULOS_VINCULADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRequisicaoAutomaticaParaVeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_UTILIZAR_CUSTO_MEDIO_PARA_LANCAMENTO_ABASTECIMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCustoMedioParaLancamentoAbastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCompra", Column = "MCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.MotivoCompra MotivoCompraAbastecimento { get; set; }
    }
}
