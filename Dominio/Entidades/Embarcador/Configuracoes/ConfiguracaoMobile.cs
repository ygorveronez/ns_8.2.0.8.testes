namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MOBILE", EntityName = "ConfiguracaoMobile", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile", NameType = typeof(ConfiguracaoMobile))]
    public class ConfiguracaoMobile : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_VALIDAR_RAIO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarRaioCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_RETORNAR_MULTIPLAS_CARGAS_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarMultiplasCargasApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_DIAS_LIMITE_RETORNAR_MULTIPLAS_CARGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasLimiteRetornarMultiplasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_SERVICOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuServicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_EXTRATO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuExtratoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_PONTOS_PARADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuPontosParada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_SERVICOS_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuServicosViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENU_RH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MenuRH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReferenciaBusca", Column = "CMO_DATA_REFERENCIA_BUSCA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DataReferenciaBusca), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DataReferenciaBusca? DataReferenciaBusca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntervaloInicial", Column = "CMO_INTERVALO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntervaloFinal", Column = "CMO_INTERVALO_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_HABILITAR_ALERTA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarAlertaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MinutosNotificarAlertaMotoristaViagem", Column = "CMO_MINUTOS_NOTIFICAR_ALERTA_MOTORISTA_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosNotificarAlertaMotoristaViagem { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para mobile"; }
        }
    }
}