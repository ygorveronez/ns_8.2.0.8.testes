using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO", EntityName = "CTeTerceiro", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiro", NameType = typeof(CTeTerceiro))]
    public class CTeTerceiro : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CPS_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "CPS_SERIE", TypeType = typeof(string), Length = 3, NotNull = true)]
        public virtual string Serie { get; set; }

        /// <summary>
        /// armazena quando o CT-e é gerado de uma carga CT-e
        /// </summary>
        [Obsolete("Não utilizar. Substituído pela lista CargaCTes.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveAcesso", Column = "CPS_CHAVE_ACESSO", TypeType = typeof(string), Length = 44, NotNull = true)]
        public virtual string ChaveAcesso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TRANSPORTADOR_TERCEIRO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente TransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CPS_PAGOAPAGAR", TypeType = typeof(Dominio.Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CPS_DATAHORAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTE", Column = "CPS_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "CPS_VERSAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTEReferenciado", Column = "CPS_CHAVE_CTE_REFERENCIADO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCTEReferenciado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "CPS_TIPO_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServico), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "CPS_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modal", Column = "CPS_MODAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal Modal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CPS_EMITENTE_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CPS_REMETENTE_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CPS_DESTINATARIO_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CPS_EXPEDIDOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CPS_RECEBEDOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CPS_TOMADOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe OutrosTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrestacaoServico", Column = "CPS_VALOR_PREST_SERVICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAReceber", Column = "CPS_VALOR_RECEBER", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "CPS_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "CPS_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "CPS_ALIQ_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CPS_VAL_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBaseCalculoICMS", Column = "CPS_PER_RED_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SimplesNacional", Column = "CPS_SIMPLES_NAC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "CPS_VALOR_TOTAL_MERC", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoriaOriginal", Column = "CPS_VALOR_TOTAL_MERC_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoriaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominante", Column = "CPS_PRODUTO_PRED", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lotacao", Column = "CPS_LOTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Lotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "CPS_LOCINICIOPRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeInicioPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "CPS_LOCTERMINOPRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeTerminoPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasCaracteristicasDaCarga", Column = "CPS_OUTRAS_CARAC_CARGA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string OutrasCaracteristicasDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoAdicionalFisco", Column = "CPS_INFORMACAO_ADICIONAL_FISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string InformacaoAdicionalFisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRomaneio", Column = "CPS_NUMERO_ROMANEIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroRomaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoAdicionalContribuinte", Column = "CPS_INFORMACAO_ADICIONAL_CONTRIBUINTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string InformacaoAdicionalContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CPS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_PROTOCOLO_CLIENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProtocoloCliente { get; set; }

        /// <summary>
        /// Informação provenientes do modal aéreo
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_NUMERO_OPERACIONAL_CONHECIMENTO_AEREO", TypeType = typeof(long), NotNull = false)]
        public virtual long? NumeroOperacionalConhecimentoAereo { get; set; }

        /// <summary>
        /// Informação provenientes do modal aéreo
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_NUMERO_MINUTA", TypeType = typeof(long), NotNull = false)]
        public virtual long? NumeroMinuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_DESCRICAO_ITEM_PESO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoItemPeso { get; set; }

        /// <summary>
        /// Peso em KG calculado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoRecebidoViaFTP", Column = "CPS_DOCUMENTO_RECEBIDO_VIA_FTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoRecebidoViaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoRecebidoViaEmail", Column = "CPS_DOCUMENTO_RECEBIDO_VIA_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoRecebidoViaEmail { get; set; }

        /// <summary>
        /// armazena quando o CT-e é gerado de uma carga CT-e
        /// </summary>
        [NHibernate.Mapping.Attributes.Set(0, Name = "CargaCTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_CARGA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTe", Column = "CCT_CODIGO")]
        public virtual ICollection<Cargas.CargaCTe> CargaCTes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ChavesNFe", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_PARA_SUBCONTRATACAO_CHAVENFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CPS_CHAVENFE", TypeType = typeof(string), Length = 44, NotNull = true)]
        [Obsolete("Não utilizar. Substituído pela bag CTeTerceiroNFes.")]
        public virtual ICollection<string> ChavesNFe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_NFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroNFe", Column = "CNE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> CTesTerceiroNFes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTesTerceiroNotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroNotaFiscal", Column = "CSN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal> CTesTerceiroNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTesTerceiroOutrosDocumentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_OUTROS_DOCUMENTOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroOutrosDocumentos", Column = "CSO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> CTesTerceiroOutrosDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTeTerceiroQuantidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_QUANTIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroQuantidade", Column = "CSQ_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> CTeTerceiroQuantidades { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTeTerceiroSeguros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_SEGURO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroSeguro", Column = "CSO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> CTeTerceiroSeguros { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTeTerceiroComponentesFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroComponenteFrete", Column = "CSC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> CTeTerceiroComponentesFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ocorrencias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaOcorrencia", Column = "COC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Ocorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_OBSERVACAO_GERAL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoGeral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_NUMERO_CARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTeTerceiroDimensoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_TERCEIRO_DIMENSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiroDimensao", Column = "CTD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao> CTeTerceiroDimensoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_METROS_CUBICOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetrosCubicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_FATOR_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_PESO_BASE_CALCULO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoBaseParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "CPS_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_SITUACAO_SEFAZ", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz SituacaoSEFAZ { get; set; } = ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentifacaoPacote", Column = "CPS_IDENTIFICACAO_PACOTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdentifacaoPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePaletes", Column = "CPS_QUANTIDADE_PALETES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadePaletes { get; set; }

        #endregion Propriedades

        #region Propriedades Virtuais

        public virtual decimal ValorFreteSemICMS
        {
            get
            {
                return this.ValorAReceber - this.ValorICMS;
            }
        }

        public virtual int NumeroTotalDocumentos
        {
            get
            {
                int numeroTotal = 0;

                if (CTesTerceiroNFes != null)
                    numeroTotal += CTesTerceiroNFes.Count();

                if (CTesTerceiroNotasFiscais != null)
                    numeroTotal += CTesTerceiroNotasFiscais.Count();

                if (CTesTerceiroOutrosDocumentos != null)
                    numeroTotal += CTesTerceiroOutrosDocumentos.Count();

                return numeroTotal;
            }
        }

        public virtual string DescricaoTipoServico
        {
            get
            {
                switch (TipoServico)
                {
                    case Enumeradores.TipoServico.Normal:
                        return "Normal";
                    case Enumeradores.TipoServico.Redespacho:
                        return "Redespacho";
                    case Enumeradores.TipoServico.RedIntermediario:
                        return "Red. Intermediário";
                    case Enumeradores.TipoServico.SubContratacao:
                        return "Subcontratação";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero + " - " + this.Serie;
            }
        }

        public virtual string DescricaoTipoCTE
        {
            get
            {
                switch (TipoCTE)
                {
                    case Enumeradores.TipoCTE.Anulacao:
                        return "Anulação";
                    case Enumeradores.TipoCTE.Complemento:
                        return "Complementar";
                    case Enumeradores.TipoCTE.Normal:
                        return "Normal";
                    case Enumeradores.TipoCTE.Substituto:
                        return "Substituição";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// Indicador CIF ou FOB, utilizado para o EDI
        /// </summary>
        public virtual string CondicaoPagamento
        {
            get
            {
                return this.TipoPagamento == Enumeradores.TipoPagamento.Pago ? "C" : "F";
            }
        }

        public virtual ParticipanteCTe Tomador
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case Enumeradores.TipoTomador.Destinatario:
                        return this.Destinatario;
                    case Enumeradores.TipoTomador.Expedidor:
                        return this.Expedidor;
                    case Enumeradores.TipoTomador.Outros:
                        return this.OutrosTomador;
                    case Enumeradores.TipoTomador.Recebedor:
                        return this.Recebedor;
                    case Enumeradores.TipoTomador.Remetente:
                        return this.Remetente;
                    default:
                        return null;
                }
            }
        }

        #endregion Propriedades Virtuais

        #region Métodos

        public virtual bool Equals(CTeTerceiro other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual CTeTerceiro Clonar()
        {
            return (CTeTerceiro)this.MemberwiseClone();
        }

        public virtual ParticipanteCTe ObterParticipante(Dominio.Enumeradores.TipoTomador tipoParticipante)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    return this.Destinatario;
                case Enumeradores.TipoTomador.Expedidor:
                    return this.Expedidor;
                case Enumeradores.TipoTomador.Outros:
                    return this.OutrosTomador;
                case Enumeradores.TipoTomador.Recebedor:
                    return this.Recebedor;
                case Enumeradores.TipoTomador.Remetente:
                    return this.Remetente;
                default:
                    return null;
            }
        }

        public virtual void SetarParticipante(Dominio.Entidades.Cliente cliente, Enumeradores.TipoTomador tipoParticipante, Dominio.ObjetosDeValor.Endereco endereco = null)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, endereco);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, endereco);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, endereco);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, endereco);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, endereco);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.Cliente cliente, Enumeradores.TipoTomador tipoParticipante, Dominio.Entidades.Pais pais)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, pais);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.CTe.Cliente cliente, Enumeradores.TipoTomador tipoParticipante, Dominio.Entidades.Pais pais)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, pais);
                    break;
                default:
                    break;
            }
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, Cliente cliente, Dominio.ObjetosDeValor.Endereco endereco)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = cliente.Atividade;
                participante.Cidade = null;

                participante.CPF_CNPJ = cliente.CPF_CNPJ_SemFormato;
                participante.Email = cliente.Email;
                participante.EmailContador = cliente.EmailContador;
                participante.EmailContadorStatus = cliente.EmailContadorStatus == "A" ? true : false;
                participante.EmailContato = cliente.EmailContato;
                participante.EmailContatoStatus = cliente.EmailContatoStatus == "A" ? true : false;
                participante.EmailStatus = cliente.EmailStatus == "A" ? true : false;
                participante.Exterior = false;
                participante.IE_RG = cliente.IE_RG;
                participante.InscricaoMunicipal = cliente.InscricaoMunicipal;
                participante.InscricaoSuframa = cliente.InscricaoSuframa;
                participante.Nome = cliente.Nome;
                participante.NomeFantasia = cliente.NomeFantasia;
                participante.Pais = null;
                participante.Telefone2 = cliente.Telefone2;
                participante.Tipo = cliente.Tipo == "J" ? Enumeradores.TipoPessoa.Juridica : Enumeradores.TipoPessoa.Fisica;

                if (endereco == null)
                {
                    participante.Bairro = cliente.Bairro;
                    participante.CEP = cliente.CEP;
                    participante.Complemento = cliente.Complemento;
                    participante.Endereco = cliente.Endereco;
                    participante.Localidade = cliente.Localidade;
                    participante.Numero = cliente.Numero;
                    participante.Telefone1 = cliente.Telefone1;
                    participante.SalvarEndereco = cliente.Localidade?.Estado?.Sigla == "EX" ? false : true;
                }
                else
                {
                    participante.Bairro = endereco.Bairro;
                    participante.CEP = endereco.CEP;
                    participante.Complemento = endereco.Complemento;
                    participante.Endereco = endereco.Logradouro;
                    participante.Localidade = endereco.Cidade;
                    participante.Numero = endereco.Numero;
                    participante.Telefone1 = endereco.Telefone;
                    participante.SalvarEndereco = false;
                }
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, ObjetosDeValor.Cliente cliente, Dominio.Entidades.Pais pais)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = null;
                participante.Bairro = cliente.Bairro;
                participante.CEP = null;
                participante.Cidade = cliente.Cidade;
                participante.Complemento = cliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = cliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = true;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = null;
                participante.Nome = cliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = cliente.Numero;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, ObjetosDeValor.CTe.Cliente cliente, Dominio.Entidades.Pais pais)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = null;
                participante.Bairro = cliente.Bairro;
                participante.CEP = null;
                participante.Cidade = cliente.Cidade;
                participante.Complemento = cliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = cliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = true;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = null;
                participante.Nome = cliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = cliente.Numero;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        public virtual void AdicionarCargaCTe(Cargas.CargaCTe cargaCTe)
        {
            if (cargaCTe == null)
                return;

            if (CargaCTes?.Any(cargaCTeExistente => cargaCTeExistente.Codigo == cargaCTe.Codigo) ?? false)
                return;

            if (CargaCTes == null)
                CargaCTes = new List<Cargas.CargaCTe>();

            CargaCTes.Add(cargaCTe);
        }

        public virtual Cargas.CargaCTe ObterCargaCTe(int codigoCarga)
        {
            Cargas.CargaCTe primeiroCargaCte = CargaCTes?.FirstOrDefault(cargaCte => cargaCte.Carga.Codigo == codigoCarga);

            if (primeiroCargaCte != null)
                return primeiroCargaCte;

            return (CargaCTe?.Carga.Codigo == codigoCarga) ? CargaCTe : null;
        }

        #endregion Métodos
    }
}
