using System;

namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CIOT", EntityName = "ConfiguracaoCIOT", Name = "Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT", NameType = typeof(ConfiguracaoCIOT))]
    public class ConfiguracaoCIOT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CCT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_ABRIR_CIOT_ANTES_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AbrirCIOTAntesEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GERAR_UM_CIOT_POR_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarUmCIOTPorViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_CONSULTAR_FATURAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarFaturas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_EXIGE_ROTA_CADASTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeRotaCadastrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALOR_PEDAGIO_RETORNADO_INTEGRADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorPedagioRetornadoIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_INTEGRAR_MOTORISTA_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarMotoristaNoCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_INTEGRAR_VEICULO_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarVeiculoNoCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CCT_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperadoraCIOT", Column = "CCT_OPERADORA_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT OperadoraCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_TARIFA_TRANSFERENCIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TarifaTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_TARIFA_SAQUE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TarifaSaque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_ENCERRAR_CIOT_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarCIOTManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_HABILITAR_CONCILIACAO_FINANCEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarConciliacaoFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GERAR_TITULOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTitulosContratoFrete { get; set; }

        [Obsolete("Propriedade criada erroneamente. Remover.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_UTILIZAR_CNPJ_CONTRATANTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCnpjContratanteIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITE_VARIOS_CIOTS_ABERTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteVariosCIOTsAbertos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITE_VARIOS_CIOTS_ABERTOS_TIPO_TERCEIRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo? PermiteVariosCIOTsAbertosTipoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_UTILIZAR_DATA_ATUAL_COMO_DATA_INICIO_TERMINO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataAtualComoInicioTerminoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_DIAS_TERMINO_CIOT", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasTerminoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_CONFIGURACAO_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfiguracaoMovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_HABILITAR_DATA_FIXA_VENCIMENTO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarDataFixaVencimentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_HABILITAR_QUITACAO_AUTOMATICA_PAGAMENTOS_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarQuitacaoAutomaticaPagamentosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_CNPJ_OPERADORA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJOperadora { get; set; }


        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
