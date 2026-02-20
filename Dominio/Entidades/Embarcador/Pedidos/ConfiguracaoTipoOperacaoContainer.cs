using Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CONTAINER", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoContainer", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoContainer", NameType = typeof(ConfiguracaoTipoOperacaoContainer))]
    public class ConfiguracaoTipoOperacaoContainer : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_GESTAO_VIAGEM_CONTAINER_FLUXO_UNICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GestaoViagemContainerFluxoUnico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_NAO_PERMITIR_ALTERAR_MOTORISTA_APOS_AVERBACAO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAlterarMotoristaAposAverbacaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_EXIGIR_COMPROVANTE_COLETA_CONTAINER_PARA_SEGUIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirComprovanteColetaContainerParaSeguir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_GERAR_PAGAMENTO_MOTORISTA_AUTOMATICAMENTE_COM_VALOR_ADIANTAMENTO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotorista.PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_COMPRAR_VALE_PEDAGIO_ETAPA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComprarValePedagioEtapaContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_CONTAINER", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoComprovante", Column = "CTC_CODIGO_CONTAINER", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoComprovante TipoComprovanteColetaContainer { get; set; }
        public virtual string Descricao
        {
            get { return "Configurações Tipo Operação para Container."; }
        }
    }
}