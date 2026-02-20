namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_BIDDING", EntityName = "ConfiguracaoBidding", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding", NameType = typeof(ConfiguracaoBidding))]
    public class ConfiguracaoBidding : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_CALCULAR_KM_MEDIO_ROTA_POR_ORIGEM_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularKMMedioRotaPorOrigemDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_PERMITE_ADICIONAR_ROTA_SEM_INFORMAR_KM_MEDIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarRotaSemInformarKMMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_PERMITE_SELECIONAR_MAIS_DE_UMA_OFERTA_POR_BIDDING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarMaisDeUmaOfertaPorBidding { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_PERMITE_REMOVER_OBRIGATORIEDADE_DATAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteRemoverObrigatoriedadeDatas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_TRANSPORTADOR_UTILIZA_PROCESSO_AUTOMATIZADO_AVANÇADO_ETAPAS_BIDDING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_PERMITE_OFERTAR_QUANDO_ACEITACAO_IND_FOR_MENOR_QUE_CEM_PORCENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCB_INFORME_PORCENTAGEM_ACEITACAO_IND", TypeType = typeof(int), NotNull = false)]
        public virtual int InformePorcentagemAceitacaoInd { get; set; }
    }
}