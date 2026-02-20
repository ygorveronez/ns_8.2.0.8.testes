using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_DESTINADO_EMPRESA", EntityName = "DocumentoDestinadoEmpresa", Name = "Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa", NameType = typeof(DocumentoDestinadoEmpresa))]
    public class DocumentoDestinadoEmpresa : EntidadeBase
    {
        public DocumentoDestinadoEmpresa() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloDocumento", Column = "DDE_MODELO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado ModeloDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "DDE_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialUnico", Column = "DDE_NUMERO_SEQUENCIAL_UNICO", TypeType = typeof(long), NotNull = true)]
        public virtual long NumeroSequencialUnico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DDE_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "DDE_SERIE", TypeType = typeof(int), NotNull = false)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "DDE_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFCNPJEmitente", Column = "DDE_CPF_CNPJ_EMITENTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPFCNPJEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IEEmitente", Column = "DDE_IE_EMITENTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IEEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeEmitente", Column = "DDE_NOME_EMITENTE", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string NomeEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "DDE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "DDE_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "DDE_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        /// <summary>
        /// Data em que o documento foi recebido pela integração.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DDE_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "DDE_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "DDE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacao", Column = "DDE_TIPO_OPERACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DDE_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DDE_ENCERRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Encerrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoEvento", Column = "DDE_DESCRICAO_EVENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "DDE_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialEvento", Column = "DDE_NUMERO_SEQUENCIAL_EVENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencialEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFCNPJRemetente", Column = "DDE_CPF_CNPJ_REMETENTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPFCNPJRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeRemetente", Column = "DDE_NOME_REMETENTE", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string NomeRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFCNPJDestinatario", Column = "DDE_CPF_CNPJ_DESTINATARIO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPFCNPJDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeDestinatario", Column = "DDE_NOME_DESTINATARIO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string NomeDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFDestinatario", Column = "DDE_UF_DESTINATARIO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFCNPJTomador", Column = "DDE_CPF_CNPJ_TOMADOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPFCNPJTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTomador", Column = "DDE_NOME_TOMADOR", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string NomeTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouArquivoIntegracao", Column = "DDE_GEROU_ARQUIVO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouArquivoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouXMLIntegracao", Column = "DDE_ENVIOU_XML_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouXMLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouXMLImputIntegracao", Column = "DDE_ENVIOU_XML_IMPUT_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouXMLImputIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativasEnvioImputIntegracao", Column = "DDE_TENTATIVAS_ENVIO_IMPUT_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativasEnvioImputIntegracao { get; set; }

        /// <summary>
        /// Aplica-se à carta de correção.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Correcao", Column = "DDE_CORRECAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Correcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoManifestacaoDestinatario", Column = "DDE_SITUACAO_MANIFESTACAO_DESTINATARIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario SituacaoManifestacaoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "DDE_OBSERVACAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Manifestacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_DESTINADO_EMPRESA_MANIFESTACAO_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DDE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ManifestacaoDestinatario", Column = "MDE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario> Manifestacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotasCTe", Formula = @"SUBSTRING((SELECT ', ' + CAST(DocumentoDestinadoEmpresaNotasCTe.DDN_NUMERO AS NVARCHAR(20))
                                                                                    FROM T_DOCUMENTO_DESTINADO_EMPRESA_NOTAS_CTE DocumentoDestinadoEmpresaNotasCTe
                                                                                    WHERE DocumentoDestinadoEmpresaNotasCTe.DDE_CODIGO = DDE_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroNotasCTe { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (TipoDocumento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoCTe:
                        return "Autorização de CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoMDFe:
                        return "Autorização de MDF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe:
                        return "Cancelamento de CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFe:
                        return "Cancelamento de MDF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe:
                        return "Cancelamento de NF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CCe:
                        return "CC-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada:
                        return "NF-e Destinada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte:
                        return "NF-e para Transporte";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario:
                        return "CT-e Destinatário";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente:
                        return "CT-e Emitente";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor:
                        return "CT-e Expedidor";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor:
                        return "CT-e Recebedor";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente:
                        return "CT-e Remetente";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro:
                        return "CT-e Terceiro";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador:
                        return "CT-e Tomador";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload:
                        return "Autorizado Download XML";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoMDFeComCTe:
                        return "Autorizado MDF-e com CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFeAutorizadoComCTe:
                        return "Cancelamento de MDF-e com CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoPassagemNFe:
                        return "Cancelamento de Passagem de NF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDe:
                        return "Manifestação do Destinatário";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemAutomaticoMDFe:
                        return "Passagem Automática do MDF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFe:
                        return "Passagem NF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFeAutomaticoPeloMDFeOuCTe:
                        return "Passagem NF-e Automático pelo MDF-e ou CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFePropagadoPeloMDFeOuCTe:
                        return "Passagem NF-e Propagado pelo MDF-e ou CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFeRFID:
                        return "Passagem NF-e RFID";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe:
                        return "Encerramento de MDF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFeDestinado:
                        return "MDF-e Destinado";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFSeDestinada:
                        return "NFS-e Destinada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador:
                        return "CT-e OS Destinado Tomador";                    
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoSituacaoManifestacaoDestinatario
        {
            get
            {
                switch (SituacaoManifestacaoDestinatario)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.CienciaOperacao:
                        return "Ciência da operação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.ConfirmadaOperacao:
                        return "Confirmada a operação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.Desconhecida:
                        return "Desconhecida";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.OperacaoNaoRealizada:
                        return "Operação não realizada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao:
                        return "Sem manifestação";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipoOperacao
        {
            get
            {
                switch (TipoOperacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada:
                        return "Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Saida:
                        return "Saída";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string CPFCNPJEmitente_Formatado
        {
            get
            {
                long cpf_cnpj = 0L;

                if (long.TryParse(this.CPFCNPJEmitente, out cpf_cnpj))
                {
                    return this.CPFCNPJEmitente.Length > 11 ? string.Format(@"{0:00\.000\.000\/0000\-00}", cpf_cnpj) : string.Format(@"{0:000\.000\.000\-00}", cpf_cnpj);
                }
                else
                {
                    return this.CPFCNPJEmitente;
                }
            }
        }
    }
}
