namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ROTEIRIZACAO", EntityName = "ConfiguracaoRoteirizacao", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao", NameType = typeof(ConfiguracaoRoteirizacao))]
    public class ConfiguracaoRoteirizacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_ORDENAR_LOCALIDADES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrdenarLocalidades { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_NAO_CALCULAR_TEMPO_VIAGEM_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalcularTempoDeViagemAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_COLETAS_SEMPRE_INICIO_ROTA_ORDENADA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetasSempreInicioRotaOrdenadaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_CADASTRAR_NOVA_ROTA_DEVE_SER_PARA_TIPO_OPERACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarNovaRotaDeveSerParaTipoOperacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_NUMERO_DIAS_PARA_CONSULTA_PRACA_PEDAGIO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDiasParaConsultaPracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_SEMPRE_UTILIZAR_ROTA_BUSCAR_PRACAS_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreUtilizarRotaParaBuscarPracasPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRT_IGNORAR_OUTRO_ENDERECO_PEDIDO_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarOutroEnderecoPedidoComRecebedor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para Roteirização";
            }
        }
    }
}
