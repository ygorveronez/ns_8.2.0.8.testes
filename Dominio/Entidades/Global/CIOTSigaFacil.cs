using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SIGA_FACIL_CIOT", EntityName = "CIOTSigaFacil", Name = "Dominio.Entidades.CIOTSigaFacil", NameType = typeof(CIOTSigaFacil))]
    public class CIOTSigaFacil : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao => this.Numero.ToString();

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaCargaANTT", Column = "NCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaCargaANTT NaturezaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "SFC_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NSU", Column = "SFC_NSU", TypeType = typeof(int), NotNull = true)]
        public virtual int NSU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "SFC_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioViagem", Column = "SFC_DATA_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTerminoViagem", Column = "SFC_DATA_TERMINO_VIAGEM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataTerminoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCartaoMotorista", Column = "SFC_NUMERO_CARTAO_MOTORISTA", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string NumeroCartaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraAdiantamento", Column = "SFC_REGRA_ADIANTAMENTO", TypeType = typeof(Enumeradores.RegraQuitacaoAdiantamento), NotNull = true)]
        public virtual Enumeradores.RegraQuitacaoAdiantamento RegraAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraQuitacao", Column = "SFC_REGRA_QUITACAO", TypeType = typeof(Enumeradores.RegraQuitacaoQuitacao), NotNull = true)]
        public virtual Enumeradores.RegraQuitacaoQuitacao RegraQuitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoViagem", Column = "SFC_TIPO_VIAGEM", TypeType = typeof(Enumeradores.TipoViagem), NotNull = true)]
        public virtual Enumeradores.TipoViagem TipoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaTransportador", Column = "SFC_CATEGORIA_TRANSPORTADOR", TypeType = typeof(Enumeradores.CategoriaTransportadorANTT), NotNull = true)]
        public virtual Enumeradores.CategoriaTransportadorANTT CategoriaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentosObrigatorios", Column = "SFC_DOCUMENTOS_OBRIGATORIOS", TypeType = typeof(Enumeradores.DocumentosObrigatorios), NotNull = true)]
        public virtual Enumeradores.DocumentosObrigatorios DocumentosObrigatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "SFC_NUMERO_CONTRATO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContratoCancelamento", Column = "SFC_NUMERO_CONTRATO_CANCELAMENTO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroContratoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "SFC_CODIGO_RETORNO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "_MensagemRetorno", Column = "SFC_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string _MensagemRetorno { get; set; }

        public virtual string MensagemRetorno {
            get
            {
                return this._MensagemRetorno;
            }
            set
            {
                this._MensagemRetorno = value + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") ;
            }
        }

        public virtual string CodigoMensagemRetorno
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.CodigoRetorno) && string.IsNullOrWhiteSpace(this.MensagemRetorno))
                    return this.CodigoRetorno;
                else if (!string.IsNullOrWhiteSpace(this.CodigoRetorno) && !string.IsNullOrWhiteSpace(this.MensagemRetorno))
                    return this.CodigoRetorno + " - " + this.MensagemRetorno;
                else if(!string.IsNullOrWhiteSpace(this.MensagemRetorno))
                    return this.MensagemRetorno;
                else
                    return string.Empty;
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetornoCancelamento", Column = "SFC_CODIGO_RETORNO_CANCELAMENTO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CodigoRetornoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "_MensagemRetornoCancelamento", Column = "SFC_MENSAGEM_RETORNO_CANCELAMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string _MensagemRetornoCancelamento { get; set; }

        public virtual string MensagemRetornoCancelamento
        {
            get
            {
                return this._MensagemRetornoCancelamento;
            }
            set
            {
                this._MensagemRetornoCancelamento = value + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public virtual string CodigoMensagemRetornoCancelamento
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.CodigoRetornoCancelamento) && string.IsNullOrWhiteSpace(this.MensagemRetornoCancelamento))
                    return this.CodigoRetornoCancelamento;
                else if (!string.IsNullOrWhiteSpace(this.CodigoRetornoCancelamento) && !string.IsNullOrWhiteSpace(this.MensagemRetornoCancelamento))
                    return this.CodigoRetornoCancelamento + " - " + this.MensagemRetornoCancelamento;
                else if (!string.IsNullOrWhiteSpace(this.MensagemRetornoCancelamento))
                    return this.MensagemRetornoCancelamento;
                else
                    return string.Empty;
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "SFC_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCIOT", Column = "SFC_NUMERO_CIOT", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoVerificadorCIOT", Column = "SFC_CODIGO_VERIFICADOR_CIOT", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoVerificadorCIOT { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegradora", Column = "SFC_TIPO_INTEGRADORA", TypeType = typeof(ObjetosDeValor.Enumerador.TipoIntegradoraCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoIntegradoraCIOT TipoIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "SFC_TIPO_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoPagamentoCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoPagamentoCIOT TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SFC_STATUS", TypeType = typeof(ObjetosDeValor.Enumerador.StatusCIOT), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusCIOT Status { get; set; }
        public virtual string DescricaoStatus {
            get
            {
                if (this.Status == ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    return "Evento Rejeitado";
                else
                    return this.Status.ToString("G");
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCIOTIntegradora", Column = "SFC_CODIGO_CIOT_INTEGRADORA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCIOTIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCartaoTransportador", Column = "SFC_NUMERO_CARTAO_TRANSPORTADOR", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string NumeroCartaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFavorecido", Column = "SFC_TIPO_FAVORECIDO", TypeType = typeof(Enumeradores.TipoFavorecido), NotNull = false)]
        public virtual Enumeradores.TipoFavorecido TipoFavorecido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedagioIdaVolta", Column = "SFC_PEDAGIO_IDA_VOLTA", TypeType = typeof(Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Enumeradores.OpcaoSimNao PedagioIdaVolta { get; set; }



        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "SFC_PESO_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoriaKG", Column = "SFC_VALOR_MERCADORIA_KG", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoriaKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "SFC_VALOR_TOTAL_MERCADORIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTarifaFrete", Column = "SFC_VALOR_TARIFA_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTarifaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "SFC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorEstimado", Column = "SFC_VALOR_ESTIMADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamentoAbertura", Column = "SFC_VALOR_ADIANTAMENTO_ABERTURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamentoAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiAdiantamentoAbertura", Column = "SFC_POSSUI_ADIANTAMENTO_ABERTURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiAdiantamentoAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoQuebra", Column = "SFC_TIPO_QUEBRA", TypeType = typeof(Enumeradores.TipoQuebra), NotNull = false)]
        public virtual Enumeradores.TipoQuebra TipoQuebra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTolerancia", Column = "SFC_TIPO_TOLERANCIA", TypeType = typeof(Enumeradores.TipoTolerancia), NotNull = false)]
        public virtual Enumeradores.TipoTolerancia TipoTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualTolerancia", Column = "SFC_PERCENTUAL_TOLERANCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaSuperior", Column = "SFC_PERCENTUAL_TOLERANCIA_SUPERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualToleranciaSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "SFC_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "SFC_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "SFC_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIRRF", Column = "SFC_VALOR_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSS", Column = "SFC_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSENAT", Column = "SFC_VALOR_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSEST", Column = "SFC_VALOR_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOperacao", Column = "SFC_VALOR_OPERACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorQuitacao", Column = "SFC_VALOR_QUITACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAbastecimento", Column = "SFC_VALOR_ABASTECIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBruto", Column = "SFC_VALOR_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBruto { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SIGA_FACIL_CIOT_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeCIOTSigaFacil", Column = "SFT_CODIGO")]
        public virtual IList<CTeCIOTSigaFacil> CTes { get; set; }

        public virtual string StatusDescricao
        {
            get
            {
                switch(this.Status)
                {

                    case ObjetosDeValor.Enumerador.StatusCIOT.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Autorizado:
                        return "Autorizado";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Encerrado:
                        return "Encerrado";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Salvo:
                        return "Salvo";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado:
                        return "Rejeitado";
                    case ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento:
                        return "Evento Rejeitado";

                    default:
                        return "";
                }
            }
        }
    }
}
