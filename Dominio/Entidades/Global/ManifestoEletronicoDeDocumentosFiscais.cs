using Dominio.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE", EntityName = "ManifestoEletronicoDeDocumentosFiscais", Name = "Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais", NameType = typeof(ManifestoEletronicoDeDocumentosFiscais))]
    public class ManifestoEletronicoDeDocumentosFiscais : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaConsultaSituacaoMDFe", Column = "MDF_DATA_ULTIMA_CONSULTA_SITUACAO_MDFE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsultaSituacaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MDF_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie Serie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalTransporte", Column = "MOA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModalTransporte Modal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_CARREGAMENTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_DESCARREGAMENTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "MDF_RNTRC", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIOT", Column = "MDF_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFisco", Column = "MDF_OBSERVACAO_FISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoFisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoContribuinte", Column = "MDF_OBSERVACAO_CONTRIBUINTE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedidaMercadoria", Column = "MDF_UNIDADE_MEDIDA", TypeType = typeof(UnidadeMedidaMDFe), NotNull = true)]
        public virtual UnidadeMedidaMDFe UnidadeMedidaMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "MDF_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBrutoMercadoria", Column = "MDF_PESO_BRUTO", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = true)]
        public virtual decimal PesoBrutoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "MDF_PROTOCOLO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloCancelamento", Column = "MDF_PROTOCOLO_CANCELAMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string ProtocoloCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloEncerramento", Column = "MDF_PROTOCOLO_ENCERRAMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string ProtocoloEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "MDF_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "MDF_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "MDF_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "MDF_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEncerramento", Column = "MDF_DATA_ENCERRAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "MDF_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioNotificacao", Column = "MDF_DATA_NOTIFICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegradorAutorizacao", Column = "MDF_CODIGO_INTEGRADOR_AUTORIZACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegradorAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegradorCancelamento", Column = "MDF_CODIGO_INTEGRADOR_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegradorCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegradorEncerramento", Column = "MDF_CODIGO_INTEGRADOR_ENCERRAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegradorEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "MDF_MUNICIPIO_ENCERRAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade MunicipioEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoSefaz", Column = "MDF_MENSAGEM_RETORNO_SEFAZ", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ErroSefaz", Column = "ERR_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual ErroSefaz MensagemStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "MDF_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MDF_STATUS", TypeType = typeof(StatusMDFe), NotNull = true)]
        public virtual StatusMDFe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "MDF_AMBIENTE", TypeType = typeof(TipoAmbiente), NotNull = true)]
        public virtual TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissao", Column = "MDF_TIPO_EMISSAO", TypeType = typeof(TipoEmissaoMDFe), NotNull = true)]
        public virtual TipoEmissaoMDFe TipoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmitente", Column = "MDF_TIPO_EMITENTE", TypeType = typeof(TipoEmitenteMDFe), NotNull = true)]
        public virtual TipoEmitenteMDFe TipoEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativaEncerramentoAutomatico", Column = "MDF_TENTATIVA_ENCERRAMENTO_AUTOMATICO", TypeType = typeof(int), NotNull = true)]
        public virtual int TentativaEncerramentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaCancelamento", Column = "MDF_JUSTIFICATIVA_CANCELAMENTO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "MDF_VERSAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDF_SEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MDFeSemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativaReenvio", Column = "MDF_TENTATIVA_REENVIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativaReenvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrarCancelamento", Column = "MDF_COBRAR_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrarCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDF_QRCODE", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string QRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDF_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEncerramento", Column = "MDF_DATA_PREVISAO_ENCERRAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCargaMDFe", Column = "MDF_TIPO_CARGA", TypeType = typeof(TipoCargaMDFe), NotNull = false)]
        public virtual TipoCargaMDFe TipoCargaMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteDescricao", Column = "MDF_PRODUTO_PREDEDOMINANTE_DESCRICAO", TypeType = typeof(string), Length = 120, NotNull = false)]
        public virtual string ProdutoPredominanteDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteCEAN", Column = "MDF_PRODUTO_PREDEDOMINANTE_CEAN", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string ProdutoPredominanteCEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteNCM", Column = "MDF_PRODUTO_PREDEDOMINANTE_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ProdutoPredominanteNCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEPCarregamentoLotacao", Column = "MDF_CEP_CARREGAMENTO_LOTACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEPCarregamentoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeCarregamentoLotacao", Column = "MDF_LATITUDE_CARREGAMENTO_LOTACAO", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LatitudeCarregamentoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeCarregamentoLotacao", Column = "MDF_LONGITUDE_CARREGAMENTO_LOTACAO", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LongitudeCarregamentoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEPDescarregamentoLotacao", Column = "MDF_CEP_DESCARREGAMENTO_LOTACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEPDescarregamentoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeDescarregamentoLotacao", Column = "MDF_LATITUDE_DESCARREGAMENTO_LOTACAO", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LatitudeDescarregamentoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeDescarregamentoLotacao", Column = "MDF_LONGITUDE_DESCARREGAMENTO_LOTACAO", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LongitudeDescarregamentoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDF_IMPORTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Importado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaEmissor", Column = "MDF_SISTEMA_EMISSOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento? SistemaEmissor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Percursos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_PERCURSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PercursoMDFe", Column = "MDP_CODIGO")]
        public virtual IList<Dominio.Entidades.PercursoMDFe> Percursos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "MunicipiosDescarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_MUNICIPIO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MunicipioDescarregamentoMDFe", Column = "MDD_CODIGO")]
        public virtual IList<Dominio.Entidades.MunicipioDescarregamentoMDFe> MunicipiosDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "MunicipiosCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_MUNICIPIO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MunicipioCarregamentoMDFe", Column = "MDC_CODIGO")]
        public virtual IList<Dominio.Entidades.MunicipioCarregamentoMDFe> MunicipiosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoMDFe", Column = "MDV_CODIGO")]
        public virtual IList<Dominio.Entidades.VeiculoMDFe> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Reboques", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ReboqueMDFe", Column = "MDR_CODIGO")]
        public virtual IList<Dominio.Entidades.ReboqueMDFe> Reboques { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Lacres", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_LACRE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LacreMDFe", Column = "MDL_CODIGO")]
        public virtual IList<Dominio.Entidades.LacreMDFe> Lacres { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Motoristas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaMDFe", Column = "MDM_CODIGO")]
        public virtual IList<Dominio.Entidades.MotoristaMDFe> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_TERMINAL_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> TerminalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_TERMINAL_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> TerminalDescarregamento { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaGeracaoCargaAutomaticamente", Column = "MDF_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaGeracaoCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TerminalOrigem", Formula = @"SUBSTRING((SELECT ', ' + CX.TTI_DESCRICAO
                                                                                    FROM T_MDFE_TERMINAL_CARREGAMENTO X
                                                                                    JOIN T_TIPO_TERMINAL_IMPORTACAO CX ON CX.TTI_CODIGO = X.TTI_CODIGO
                                                                                    WHERE X.MDF_CODIGO = MDF_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TerminalDestino", Formula = @"SUBSTRING((SELECT ', ' + CX.TTI_DESCRICAO
                                                                                    FROM T_MDFE_TERMINAL_DESCARREGAMENTO X
                                                                                    JOIN T_TIPO_TERMINAL_IMPORTACAO CX ON CX.TTI_CODIGO = X.TTI_CODIGO
                                                                                    WHERE X.MDF_CODIGO = MDF_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMDFeManual", Formula = @"(SELECT TOP(1) X.CMM_CODIGO
                                                                                                FROM T_CARGA_MDFE_MANUAL_MDFE X                                                                                                
                                                                                                WHERE X.MDF_CODIGO = MDF_CODIGO
		                                                                                        ORDER BY X.CMM_CODIGO DESC)", TypeType = typeof(int), Lazy = true)]
        public virtual int CodigoMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdTotalDocumentos", Formula = @"(select COUNT(1) from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC X
                                                                                            JOIN T_CTE C ON C.CON_CODIGO = X.CON_CODIGO
                                                                                            JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO M ON M.MDD_CODIGO = X.MDD_CODIGO
                                                                                            WHERE M.MDF_CODIGO = MDF_CODIGO)", TypeType = typeof(int), Lazy = true)]
        public virtual int QtdTotalDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdAAK", Formula = @"(select COUNT(1) from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC X
                                                                                            JOIN T_CTE C ON C.CON_CODIGO = X.CON_CODIGO AND C.CON_TIPO_SERVICO <> 4 
                                                                                            JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO M ON M.MDD_CODIGO = X.MDD_CODIGO
                                                                                            WHERE M.MDF_CODIGO = MDF_CODIGO)", TypeType = typeof(int), Lazy = true)]
        public virtual int QtdAAK { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdSVM", Formula = @"(select COUNT(1) from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC X
                                                                                            JOIN T_CTE C ON C.CON_CODIGO = X.CON_CODIGO AND C.CON_TIPO_SERVICO = 4 
                                                                                            JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO M ON M.MDD_CODIGO = X.MDD_CODIGO
                                                                                            WHERE M.MDF_CODIGO = MDF_CODIGO)", TypeType = typeof(int), Lazy = true)]
        public virtual int QtdSVM { get; set; }

        public virtual string DescricaoStatus
        {
            get { return this.Status.ObterDescricao(); }
        }

        public virtual string TipoMDFeSefazMT
        {
            get
            {
                if (this.EstadoCarregamento.Sigla == "MT" || this.EstadoDescarregamento.Sigla == "MT")
                    return "E"; //Manifesto de Entrada (E): Todas as UF's destinatárias dos registros 40 (Pos. 88 a 89) devem ser igual a MT e as dos remetentes(Pos. 70 a 71) diferentes de MT
                else
                    return "G"; //Manifesto de Trânsito (G): Todas as UF's destinatárias e remetentes nos registros 40 (Pos. 70 a 71 e 88 a 89) devem ser diferentes de MT.

            }
        }
        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString() + " - " + (this.Serie?.Numero ?? 0).ToString();
            }
        }
    }
}
