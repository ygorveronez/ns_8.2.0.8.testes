using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT", EntityName = "CIOT", Name = "Dominio.Entidades.Embarcador.Documentos.CIOT", NameType = typeof(CIOT))]
    public class CIOT : EntidadeBase
    {
        public CIOT() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Contratante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAbertura", Column = "CIO_DATA_ABERTURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEncerramento", Column = "CIO_DATA_ENCERRAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloEncerramento", Column = "CIO_PROTOCOLO_ENCERRAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "CIO_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloCancelamento", Column = "CIO_PROTOCOLO_CANCELAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalViagem", Column = "CIO_DATA_FINAL_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinalViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CIO_MENSAGEM", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CIO_NUMERO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoContratoAgregado", Column = "CIO_CODIGO_CONTRATO_AGREGADO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoContratoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoVerificador", Column = "CIO_CODIGO_VERIFICADOR", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoVerificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "CIO_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Operadora", Column = "CIO_OPERADORA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT Operadora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CIO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_PROTOCOLO_ABERTURA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_PROTOCOLO_AUTORIZACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DIGITO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Digito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_AUTORIZACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_LIBERACAO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_VENCIMENTO_ADIANTAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_VENCIMENTO_ABASTECIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_VENCIMENTO_SALDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_SAQUE_ADIANTAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaqueAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSaqueAdiantamento", Column = "CIO_VALOR_SAQUE_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorSaqueAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChegada", Column = "CIO_DATA_CHEGADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoChegada", Column = "CIO_PESO_CHEGADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAvaria", Column = "CIO_VALOR_AVARIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDiferencaFrete", Column = "CIO_VALOR_DIFERENCA_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiferencaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorQuebra", Column = "CIO_VALOR_QUEBRA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuebra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_CIOT_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CIOTPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_CIOT_GERADO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CIOTGeradoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_ADIANTAMENTO_PAGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdiantamentoPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_ABASTECIMENTO_PAGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AbastecimentoPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_SALDO_PAGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SaldoPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_AUTORIZACAO_PAGAMENTO_ADIANTAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamentoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_AUTORIZACAO_PAGAMENTO_ABASTECIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamentoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_AUTORIZACAO_PAGAMENTO_SALDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_TIPO_EFETIVACAO_ADIANTAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_TIPO_EFETIVACAO_ABASTECIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_TIPO_EFETIVACAO_SALDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao? EfetivacaoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CIOTCTe", Column = "CIC_CODIGO")]
        public virtual IList<CIOTCTe> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaCIOT", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CIOT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCIOT", Column = "CCO_CODIGO")]
        public virtual IList<Cargas.CargaCIOT> CargaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CIOTIntegracaoArquivo", Column = "CIT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamentoCIOT", Column = "CIO_TIPO_PAGAMENTO_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_PROTOCOLO_ABASTECIMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_PROTOCOLO_ADIANTAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_PROTOCOLO_SALDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_SITUACAO_INTEGRACAO_PAGAMENTO_BBC", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoPagamentoBBC), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoPagamentoBBC? SituacaoIntegracaoPagamentoBBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_DATA_INTEGRACAO_PAGAMENTO_BBC", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracaoPagamentoBBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataParaFechamento", Column = "CIO_DATA_PARA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataParaFechamento { get; set; }

        public virtual string Descricao
        {
            get { return Numero; }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado:
                        return "Encerrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao:
                        return "Ag. Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia:
                        return "Pendência Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado:
                        return "Pagamento autorizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem:
                        return "Ag. Liberar Viagem";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
