namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_REPOMFRETE", EntityName = "CIOTRepomFrete", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete", NameType = typeof(CIOTRepomFrete))]
    public class CIOTRepomFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_URL", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLRepomFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioRepomFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaRepomFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_PARTNER", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PartnerRepomFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_UTILIZAR_METODOS_VALIDACAO_CADASTROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMetodosValidacaoCadastros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_REALIZAR_ENCERRAMENTO_AUTORIZACAO_PAGAMENTO_SEPARADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarEncerramentoAutorizacaoPagamentoSeparado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_REALIZAR_COMPRA_VALE_PEDAGIO_INTEGRACAO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarCompraValePedagioIntegracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_ENVIAR_QUANTIDADES_MAIORES_QUE_ZERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarQuantidadesMaioresQueZero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_USAR_DATA_PAGAMENTO_TRANSPORTADOR_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarDataPagamentoTransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_UTILIZAR_DATA_PREVISAO_ENTREGA_PEDIDO_PARA_EXPECTATIVA_PAGAMENTO_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
