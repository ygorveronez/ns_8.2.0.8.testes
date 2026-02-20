using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_NOTA_FISCAL", EntityName = "Canhoto", Name = "Dominio.Entidades.Embarcador.Canhotos.Canhoto", NameType = typeof(Canhoto))]
    public class Canhoto : EntidadeBase, IEquatable<Canhoto>, IEntidade
    {
        public Canhoto() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CNF_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_SERIE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCanhoto", Column = "CNF_TIPO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto TipoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "UDC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanhotoAvulso", Column = "CAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso CanhotoAvulso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiro CTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Malote", Column = "MCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.Malote Malote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "CNF_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CNF_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalidadeFrete", Column = "CNF_MODALIDADE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ModalidadeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemDigitalizacao", Column = "CNF_ORIGEM_DIGITALIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CanhotoOrigemDigitalizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CanhotoOrigemDigitalizacao? OrigemDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TerceiroResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidNomeArquivo", Column = "CNF_GUID_NOME_ARQUIVO", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string GuidNomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CNF_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CNF_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioCanhoto", Column = "CNF_DATA_ENVIO_CANHOTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvioCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_DATA_REVERTER_JUSTIFICATIVA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReverterJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaModificacao", Column = "CNF_DATA_ULTIMA_MODIFICACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataUltimaModificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CNF_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_OBSERVACAO_REVERTER_JUSTIFICATIVA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoReverterJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_OBSERVACAO_RECEBIMENTO_FISICO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoRecebimentoFisico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCanhoto", Column = "CNF_SITUACAO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoDigitalizacao", Column = "CNF_MOTIVO_REJEICAO_DIGITALIZACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MotivoRejeicaoDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDigitalizacaoCanhoto", Column = "CNF_SITUACAO_DIGITALIZACAO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_DATA_DIGITALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoCanhoto", Column = "LAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto LocalArmazenamentoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PacoteArmazenado", Column = "CNF_PACOTE_ARMAZENADO", TypeType = typeof(int), NotNull = false)]
        public virtual int PacoteArmazenado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoNoPacote", Column = "CNF_POSICAO_NO_PACOTE", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoNoPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CNF_LATITUDE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CNF_LONGITUDE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitalizacaoIntegrada", Column = "CNF_DIGITALIZACAO_INTEGRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DigitalizacaoIntegrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponivelParaConsulta", Column = "CNF_DISPONIVEL_PARA_CONSULTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelParaConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Oculto", Column = "CNF_OCULTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Oculto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoOriginario", Column = "CNF_NUMERO_DOCUMENTO_ORIGINARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDocumentoOriginario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPgtoCanhoto", Column = "CNF_SITUACAO_PGTO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto SituacaoPgtoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoPagamento", Column = "CNF_DATA_LIBERACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "ULP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioLiberacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntregaNotaCliente", Column = "CNF_DATA_ENTREGA_NOTA_CLIENTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntregaNotaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracaoEntrega", Column = "CNF_DATA_INTEGRACAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAuditoriaCanhoto", Column = "CNF_SITUACA_AUDITORIA", TypeType = typeof(ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto SituacaoAuditoriaCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRejeicaoAuditoria", Column = "MRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRejeicaoAuditoria MotivoRejeicaoAuditoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoSituacaoAuditoria", Column = "CNF_DESCRICAO_SITUACAO_AUDITORIA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string DescricaoSituacaoAuditoria { get; set; }

        //Somente é salva a data de de aprovacao do canhoto desconsiderar o da rejeicao
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovacaoRejeicaoAuditoria", Column = "CNF_DATA_APROVACAO_REJECIAO_AUDITORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacaoRejeicaoAuditoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRejeicaoAuditoria", Column = "CNF_DATA_REJECIAO_AUDITORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRejeicaoAuditoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "CNF_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroProtocolo", Column = "CNF_NUMERO_PROTOCOLO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroProtocolo { get; set; }

        //vai ser removido, estarei enviando email pra retirar da entidade
        //[NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioNotificacaoTransportadorDigitalizacaoRejeitada", Column = "CNF_DATA_ENVIO_NOTIFICACAO_TRANSPORTADOR_DIGITALIZACAO_REJEITADA", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? DataEnvioNotificacaoTransportadorDigitalizacaoRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReversao", Column = "CNF_DATA_REVERSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovacaoDigitalizacao", Column = "CNF_DATA_APROVACAO_DIGITALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacaoDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "CNF_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRastreio", Column = "CNF_CODIGO_RASTREIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoRastreio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_ID_TRIZY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IdTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoViaOCR", Column = "CNF_VALIDACAO_VIA_OCR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoViaOCR { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "MotoristasResponsaveis", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANHOTO_FRETE_MOTORISTAS_RESPONSAVEIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> MotoristasResponsaveis { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "InconsistenciasDigitacaoCanhoto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INCONSISTENCIA_DIGITACAO_CANHOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InconsistenciaDigitacaoCanhoto", Column = "CID_CODIGO")]
        public virtual IList<InconsistenciaDigitacaoCanhoto> InconsistenciasDigitacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoGeracaoCancelamentoAutomatico", Column = "CNF_SITUACAO_GERACAO_CANCELAMENTO_AUTOMATICO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoCancelamentoAutomatico), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoCancelamentoAutomatico SituacaoGeracaoCancelamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNF_DATA_GERACAO_CANCELAMENTO_AUTOMATICO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracaoCancelamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEnvioDigitalizacaoCanhoto", Column = "CNF_QUANTIDADE_ENVIO_DIGITALIZACAO_CANHOTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEnvioDigitalizacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemSituacaoDigitalizacaoCanhoto", Column = "CNF_ORIGEM_SITUACAO_DIGITALIZACAO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoDigitalizacaoCanhoto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoDigitalizacaoCanhoto? OrigemSituacaoDigitalizacaoCanhoto { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTe", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CTe.CON_CHAVECTE FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotasFiscais on CanhotoNotaFiscal.NFX_CODIGO = CTeXMLNotasFiscais.NFX_CODIGO
                                                                                    join T_CTE CTe on CTe.con_codigo = CTeXMLNotasFiscais.CON_CODIGO
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = CNF_CODIGO AND CTe.CON_STATUS NOT IN ('C', 'I', 'Z') FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ChaveCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTe", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), CTe.CON_NUM) FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotasFiscais on CanhotoNotaFiscal.NFX_CODIGO = CTeXMLNotasFiscais.NFX_CODIGO
                                                                                    join T_CTE CTe on CTe.con_codigo = CTeXMLNotasFiscais.CON_CODIGO
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = CNF_CODIGO AND CTe.CON_STATUS NOT IN ('C', 'I', 'Z') FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCTe", Formula = @"(SELECT TOP(1) CTe.CON_VALOR_RECEBER FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotasFiscais on CanhotoNotaFiscal.NFX_CODIGO = CTeXMLNotasFiscais.NFX_CODIGO
                                                                                    join T_CTE CTe on CTe.con_codigo = CTeXMLNotasFiscais.CON_CODIGO
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = CNF_CODIGO AND CTe.CON_STATUS NOT IN ('C', 'I', 'Z'))", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motorista", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + Motorista.FUN_NOME FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                    join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                                                                                    join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                                                                    WHERE CanhotoNotaFiscal.CNF_CODIGO = CNF_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelefoneMotorista", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + Motorista.FUN_FONE FROM T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                                                                                            join T_CARGA_MOTORISTA CargaMotorista on CargaMotorista.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                                                                                            join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                                                                            WHERE CanhotoNotaFiscal.CNF_CODIGO = CNF_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string TelefoneMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Veiculo", Formula = @"((select vei.VEI_PLACA from T_VEICULO vei inner join T_CARGA carga on carga.CAR_VEICULO = vei.VEI_CODIGO where carga.CAR_CODIGO = CAR_CODIGO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), ''))", TypeType = typeof(string), Lazy = true)]
        public virtual string Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Frota", Formula = @"((select vei.VEI_NUMERO_FROTA from T_VEICULO vei inner join T_CARGA carga on carga.CAR_VEICULO = vei.VEI_CODIGO where carga.CAR_CODIGO = CAR_CODIGO) + ISNULL((SELECT ', ' + veiculo1.VEI_NUMERO_FROTA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), ''))", TypeType = typeof(string), Lazy = true)]
        public virtual string Frota { get; set; }

        #region Validação Canhoto IA Comprovei Devops: #4487

        /// <summary>
        /// Possui integração com a Comprovei.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoComprovei", Column = "CNF_POSSUI_INTEGRACAO_COMPROVEI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoComprovei { get; set; }

        /// <summary>
        /// "neural_evaluation": "1.00", //se reconheceu como canhoto ou não
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoCanhoto", Column = "CNF_VALIDACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoCanhoto { get; set; }

        /// <summary>
        /// "number_evaluation": "0.00", //se reconheceu o número da nota
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoNumero", Column = "CNF_VALIDACAO_NUMERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoNumero { get; set; }

        /// <summary>
        /// "date_evaluation": "1.00", //se encontrou a data
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoEncontrouData", Column = "CNF_VALIDACAO_ENCONTROU_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoEncontrouData { get; set; }

        /// <summary>
        /// "signature_evaluation": "1.00" //se encontrou a assinatura
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoAssinatura", Column = "CNF_VALIDACAO_ASSINATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoAssinatura { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "HouveFalhaNaValidacao", Column = "CNF_HOUVE_FALHA_NA_VALIDACAO_IA_COMPROVEI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HouveFalhaNaValidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FalhaValidacaoCanhoto", Column = "CNF_FALHA_VALIDACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FalhaValidacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FalhaValidacaoNumero", Column = "CNF_FALHA_VALIDACAO_NUMERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FalhaValidacaoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FalhaValidacaoEncontrouData", Column = "CNF_FALHA_VALIDACAO_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FalhaValidacaoEncontrouData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FalhaValidacaoAssinatura", Column = "CNF_FALHA_VALIDACAO_ASSINATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FalhaValidacaoAssinatura { get; set; }
        #endregion


        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        private bool? _isPDF { get; set; }

        public virtual bool IsPDF()
        {
            if (_isPDF.HasValue) return _isPDF.Value;

            if (string.IsNullOrWhiteSpace(NomeArquivo))
                return false;

            string extensao = NomeArquivo.Split('.').LastOrDefault().ToLower();

            return (_isPDF = extensao == "pdf").Value;
        }

        public virtual bool Equals(Canhoto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoModalidadeFrete
        {
            get
            {
                string retorno = "";
                switch (this.ModalidadeFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago:
                        retorno = "Pago";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar:
                        retorno = "A pagar";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros:
                        retorno = "Outros";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido:
                        retorno = "Não Definido";
                        break;
                    default:
                        break;
                }

                return retorno;
            }
        }

        public virtual string DescricaoDigitalizacao
        {
            get
            {
                string retorno = "";
                switch (this.SituacaoDigitalizacaoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao:
                        retorno = "Ag. Aprovação";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada:
                        retorno = "Rejeitada";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado:
                        retorno = "Digitalizado";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao:
                        retorno = "Pendente";
                        break;
                    default:
                        break;
                }
                return retorno;
            }
        }

        public virtual string DescricaoTipoCanhoto
        {
            get
            {
                switch (this.TipoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe:
                        return "NF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso:
                        return "Avulso";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao:
                        return "CT-e Subcontratação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe:
                        return "CT-e";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                string retorno = "";
                switch (this.SituacaoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente:
                        retorno = "Pendente de Envio";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado:
                        retorno = "Justificado";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente:
                        retorno = "Recebido Fisicamente";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Extraviado:
                        retorno = "Extraviado";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EntregueMotorista:
                        retorno = "Entregue pelo Motorista";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EnviadoCliente:
                        retorno = "Enviado ao Cliente";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoCliente:
                        retorno = "Recebido pelo Cliente";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Cancelado:
                        retorno = "Cancelado";
                        break;
                    default:
                        break;
                }
                return retorno;
            }
        }

        public virtual string DescricaoSituacaoPgto
        {
            get
            {
                string retorno = "";
                switch (this.SituacaoPgtoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente:
                        retorno = "Pendente ";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Liberado:
                        retorno = "Liberado";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Rejeitado:
                        retorno = "Rejeitado";
                        break;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Todas:
                        retorno = "Todas";
                        break;
                    default:
                        break;
                }
                return retorno;
            }
        }

        public virtual InconsistenciaDigitacaoCanhoto UltimaInconsistencia
        {
            get { return InconsistenciasDigitacaoCanhoto.LastOrDefault(); }
        }

        public virtual string Identificacao
        {
            get
            {
                if (TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
                    return XMLNotaFiscal?.Chave ?? "";
                else if (TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
                    return CanhotoAvulso?.QRCode ?? "";
                else if (TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
                    return CTeSubcontratacao?.ChaveAcesso ?? "";
                else if (TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe)
                    return CargaCTe?.CTe?.ChaveAcesso ?? "";

                return "";
            }
        }

        public virtual Canhoto Clonar()
        {
            Canhoto canhoto = new Canhoto();

            canhoto.Numero = this.Numero;
            canhoto.Serie = this.Serie;
            canhoto.TipoCanhoto = this.TipoCanhoto;
            canhoto.XMLNotaFiscal = this.XMLNotaFiscal;
            canhoto.UsuarioDigitalizacao = this.UsuarioDigitalizacao;
            canhoto.CanhotoAvulso = this.CanhotoAvulso;
            canhoto.CTeSubcontratacao = this.CTeSubcontratacao;
            canhoto.CargaCTe = this.CargaCTe;
            canhoto.Pedido = this.Pedido;
            canhoto.Carga = this.Carga;
            canhoto.CargaPedido = this.CargaPedido;
            canhoto.Malote = this.Malote;
            canhoto.Empresa = this.Empresa;
            canhoto.EmpresaFilialEmissora = this.EmpresaFilialEmissora;
            canhoto.Emitente = this.Emitente;
            canhoto.Destinatario = this.Destinatario;
            canhoto.Filial = this.Filial;
            canhoto.Peso = this.Peso;
            canhoto.Valor = this.Valor;
            canhoto.ModalidadeFrete = this.ModalidadeFrete;
            canhoto.OrigemDigitalizacao = this.OrigemDigitalizacao;
            canhoto.TerceiroResponsavel = this.TerceiroResponsavel;
            canhoto.GuidNomeArquivo = this.GuidNomeArquivo;
            canhoto.NomeArquivo = this.NomeArquivo;
            canhoto.DataEmissao = this.DataEmissao;
            canhoto.DataEnvioCanhoto = this.DataEnvioCanhoto;
            canhoto.DataReverterJustificativa = this.DataReverterJustificativa;
            canhoto.DataUltimaModificacao = this.DataUltimaModificacao;
            canhoto.Observacao = this.Observacao;
            canhoto.ObservacaoReverterJustificativa = this.ObservacaoReverterJustificativa;
            canhoto.ObservacaoRecebimentoFisico = this.ObservacaoRecebimentoFisico;
            canhoto.SituacaoCanhoto = this.SituacaoCanhoto;
            canhoto.MotivoRejeicaoDigitalizacao = this.MotivoRejeicaoDigitalizacao;
            canhoto.SituacaoDigitalizacaoCanhoto = this.SituacaoDigitalizacaoCanhoto;
            canhoto.DataDigitalizacao = this.DataDigitalizacao;
            canhoto.LocalArmazenamentoCanhoto = this.LocalArmazenamentoCanhoto;
            canhoto.PacoteArmazenado = this.PacoteArmazenado;
            canhoto.PosicaoNoPacote = this.PosicaoNoPacote;
            canhoto.Latitude = this.Latitude;
            canhoto.Longitude = this.Longitude;
            canhoto.DigitalizacaoIntegrada = this.DigitalizacaoIntegrada;
            canhoto.NumeroDocumentoOriginario = this.NumeroDocumentoOriginario;
            canhoto.SituacaoPgtoCanhoto = this.SituacaoPgtoCanhoto;
            canhoto.DataLiberacaoPagamento = this.DataLiberacaoPagamento;
            canhoto.UsuarioLiberacaoPagamento = this.UsuarioLiberacaoPagamento;
            canhoto.Usuario = this.Usuario;
            canhoto.DataEntregaNotaCliente = this.DataEntregaNotaCliente;
            canhoto.DataIntegracaoEntrega = this.DataIntegracaoEntrega;

            return canhoto;
        }

        #endregion
    }
}