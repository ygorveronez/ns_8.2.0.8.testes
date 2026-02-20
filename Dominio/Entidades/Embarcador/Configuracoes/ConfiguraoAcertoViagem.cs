namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ACERTO_VIAGEM", EntityName = "ConfiguraoAcertoViagem", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguraoAcertoViagem", NameType = typeof(ConfiguraoAcertoViagem))]

    public class ConfiguraoAcertoViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NAO_FECHAR_ACERTO_DE_VIAGEM_ATE_RECEBER_PALLETS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFecharAcertoViagemAteReceberPallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_VISUALIZAR_PALLETS_E_CANHOTOS_NAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarPalletsCanhotosNasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_HABILITAR_FORMA_RECEBIMENTO_TITULO_AO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFormaRecebimentoTituloAoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_HABILITAR_LANCAMENTO_TACOGRAFO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarLancamentoTacografo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_SEPARAR_VALORES_ADIANTAMENTO_MOTORISTA_POR_TIPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SepararValoresAdiantamentoMotoristaPorTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_HABILITAR_INFORMACAO_ACERTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarInformacaoAcertoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_HABILITAR_CONTROLAR_OUTRAS_DESPESAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarControlarOutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoRecibo", Column = "ACV_TEXTO_RECIBO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string TextoRecibo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para acerto de viagem";
            }
        }
    }
}
