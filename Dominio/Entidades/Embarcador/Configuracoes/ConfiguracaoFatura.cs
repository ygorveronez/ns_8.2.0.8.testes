namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_FATURA", EntityName = "ConfiguracaoFatura", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoFatura", NameType = typeof(ConfiguracaoFatura))]
    public class ConfiguracaoFatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITIR_VENCIMENTO_RETROATIVO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVencimentoRetroativoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PREENCHER_PERIODO_FATURA_COM_DATA_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherPeriodoFaturaComDataAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_INFORMAR_DATA_CANCELAMENTO_CANCELAMENTO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDataCancelamentoCancelamentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_DISPONIBILIZAR_PROVISAO_CONTRAPARTIDA_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponbilizarProvisaoContraPartidaParaCancelamento { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_HABILITAR_LAYOUT_FATURA_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarLayoutFaturaNFSManual { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para faturas";
            }
        }
    }
}
