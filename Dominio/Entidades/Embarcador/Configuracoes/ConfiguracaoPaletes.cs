namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PALETES", EntityName = "ConfiguracaoPaletes", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes", NameType = typeof(ConfiguracaoPaletes))]
    public class ConfiguracaoPaletes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_UTILIZAR_CONTROLE_PALETES_POR_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControlePaletesPorModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_LIQUIDAR_PALLET_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiquidarPalletAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_QUANTIDADE_DIAS_PARA_LIQUIDAR_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int QteDiasParaLiquidarPallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_NAO_EXIBIR_DEVOLUCAO_PALETES_SEM_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirDevolucaoPaletesSemNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_UTILIZAR_CONTROLE_PALETES_POR_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControlePaletesPorCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_LIMITE_DIAS_PARA_DEVOLUCAO_DE_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteDiasParaDevolucaoDePallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_NOTIFICAR_PALETES_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarPaletesPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPL_DIA_SEMANA_NOTIFICAR_PALETES_PENDENTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemanaNotificarPaletesPendentes { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração de paletes"; }
        }
    }
}
