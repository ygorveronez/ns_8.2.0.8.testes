using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_DBTRANS", EntityName = "IntegracaoDBTrans", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans", NameType = typeof(IntegracaoDBTrans))]
    public class IntegracaoDBTrans : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CID_URL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCliente", Column = "CID_CODIGO_CLIENTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CID_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CID_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdLocalImpressao", Column = "CID_ID_LOCAL_IMPRESSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int IdLocalImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CID_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaDBTrans), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaDBTrans TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRotaFrete", Column = "CID_TIPO_ROTA_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFreteDBTrans), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFreteDBTrans TipoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MeioPagamento", Column = "CID_MEIO_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MeioPagamentoDBTrans), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MeioPagamentoDBTrans MeioPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEnviarTransportadorNaIntegracao", Column = "CID_NAO_ENVIAR_TRANSPORTADOR_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarTransportadorNaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEnviarMotoristaNaIntegracao", Column = "CID_NAO_ENVIAR_MOTORISTA_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarMotoristaNaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "CID_TIPO_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador? TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VerificarVeiculoCompraPorTag", Column = "CID_VERIFICAR_VEICULO_COMPRA_POR_TAG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VerificarVeiculoCompraPorTag { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsultarValorPedagioParaRota", Column = "CID_CONSULTAR_VALOR_PEDAGIO_PARA_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarValorPedagioParaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarTransportadorAntesDaCompra", Column = "CID_CADASTRAR_TRANSPORTADOR_ANTES_DA_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarTransportadorAntesDaCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarMotoristaAntesDaCompra", Column = "CID_CADASTRAR_MOTORISTA_ANTES_DA_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarMotoristaAntesDaCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarVeiculoAntesDaCompra", Column = "CID_CADASTRAR_VEICULO_ANTES_DA_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarVeiculoAntesDaCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarDocumentoTransportadorAntesDaCompra", Column = "CID_CADASTRAR_DOCUMENTO_TRANSPORTADOR_ANTES_DA_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarDocumentoTransportadorAntesDaCompra { get; set; }

        [Obsolete("Configuração temporária, não será mais usada.", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsultarIdVpoDepoisDaCompra", Column = "CID_CONSULTAR_ID_VPO_DEPOIS_DA_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarIdVpoDepoisDaCompra { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração DBTrans"; }
        }
    }
}
