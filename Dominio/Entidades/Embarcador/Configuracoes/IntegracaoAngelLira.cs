using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ANGELLIRA", EntityName = "IntegracaoAngelLira", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira", NameType = typeof(IntegracaoAngelLira))]
    public class IntegracaoAngelLira : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIA_USUARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIA_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Homologacao", Column = "CIA_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Homologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoTemperatura", Column = "CIA_INTEGRACAO_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_OBTER_ROTAS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObterRotasAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAcesso", Column = "CIE_URL_ACESSO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_UTILIZAR_DATA_AGENDAMENTO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataAgendamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_ENVIAR_DADOS_FORMATADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosFormatados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_NAO_ENVIAR_ROTA_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarRotaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_GERAR_VIAGENS_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarViagensPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_CONSULTAR_POSICAO_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarPosicaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAcessoPedido", Column = "CIE_URL_ACESSO_PEDIDO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAcessoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAcessoPedido", Column = "CIE_TOKEN_ACESSO_PEDIDO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioAcessoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaAcessoPedido", Column = "CIE_SENHA_ACESSO_PEDIDO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaAcessoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_UTILIZAR_DATA_ATUAL_TEMPO_ROTA_PARA_INICIO_FIM_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataAtualETempoRotaParaInicioEFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraCodigoIdentificacaoViagem", Column = "CIE_REGRA_CODIGO_IDENTIFICACAO_VIAGEM", TypeType = typeof(AngelLiraRegraCodigoIdentificacaoViagem), NotNull = false)]
        public virtual AngelLiraRegraCodigoIdentificacaoViagem RegraCodigoIdentificacaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_IGNORAR_VALIDACAO_CARGA_AGRUPADA_REGRA_CODIGO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarValidacaoCargaAgrupadaRegraCodigoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_APLICAR_REGRA_lOCAL_PALLETIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AplicarRegraLocalPalletizacao { get; set; }
        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração AngelLira";
            }
        }

    }
}
