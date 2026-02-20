using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MOTORISTA", EntityName = "ConfiguracaoMotorista", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista", NameType = typeof(ConfiguracaoMotorista))]
    public class ConfiguracaoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_NAO_PERMITIR_TRANSPORTADOR_ALTERAR_DATA_VALIDADE_SEGURADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirTransportadoAlterarDataValidadeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_BLOQUEAR_CAMPOS_MOTORISTA_LGPD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCamposMotoristaLGPD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_EXIBIR_CAMPOS_SUSPENSAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCamposSuspensaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_BLOQUEAR_CHECKLIST_MOTORISTA_SEM_LICENCA_VINCULADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearChecklistMotoristaSemLicencaVinculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_NAO_PERMITIR_INATIVAR_MOTORISTA_COM_SALDO_NO_EXTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInativarMotoristaComSaldoNoExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_PERMITIR_CADASTRAR_MOTORISTA_ESTRANGEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCadastrarMotoristaEstrangeiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_NAO_VALIDAR_HORA_NO_PAGAMENTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarHoraNoPagamentoMotorista { get; set; }

        /// <summary>
        /// Quando ativada, a foto do motorista que aparece na tela de Motoristas é a que ele mesmo escolhe no app
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MOTORISTA_USAR_FOTO_DO_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaUsarFotoDoApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_EXIBIR_CONFIGURACOES_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirConfiguracoesPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_NAO_PERMITIR_REALIZAR_CADASTRO_MOTORISTA_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirRealizarCadastroMotoristaBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_PERMITE_DUPLICAR_CADASTRO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteDuplicarCadastroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_DIAS_ANTECIDENCIA_PARA_COMUNICAR_MOTORISTA_VENCIMENTO_LICENCA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_NAO_GERAR_PRE_TRIP_MOTORISTAS_IGNORADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarPreTripMotoristasIgnorados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_HABILITAR_USO_CENTRO_RESULTADO_COMISSAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarUsoCentroResultadoComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "MotoristasIgnorados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_MOTORISTA_IGNORADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CMO_MOTORISTAS", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual ICollection<string> MotoristasIgnorados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_HABILITAR_CONTROLE_SITUACAO_COLABORADOR_PARA_MOTORISTAS_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarControleSituacaoColaboradorParaMotoristasTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMO_MENSAGEM_PERSONALIZADA_MOTORISTA_BLOQUEADO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MensagemPersonalizadaMotoristaBloqueado { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para motorista"; }
        }
    }
}
