using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA", EntityName = "TituloBaixa", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixa", NameType = typeof(TituloBaixa))]
    public class TituloBaixa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TIB_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_PAGO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_TOTAL_A_PAGAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalAPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_PAGO_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPagoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_ACRESCIMO_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAcrescimoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_DESCONTO_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescontoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_VALOR_TOTAL_A_PAGAR_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalAPagarMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TIB_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        /// <summary>
        /// Maior data de emissão dos títulos da baixa
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaixa", Column = "TIB_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBase", Column = "TIB_DATA_BASE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOperacao", Column = "TIB_DATA_OPERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "TIB_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo TipoBaixaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoBaixaTitulo", Column = "TIB_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo SituacaoBaixaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArredondamento", Column = "TIB_TIPO_ARREDONDAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento? TipoArredondamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "TIB_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_MODELO_ANTIGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloAntigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bordero", Column = "BOR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Bordero Bordero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIB_GEROU_TITULOS_NEGOCIACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouTitulosNegociacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "TIB_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "TIB_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "TIB_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "TIB_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TitulosNegociacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_NEGOCIACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaNegociacao", Column = "TBN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> TitulosNegociacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConhecimentosRemovidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_CONHECIMENTO_REMOVIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaConhecimentoRemovido", Column = "TCR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido> ConhecimentosRemovidos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TitulosAgrupados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_AGRUPADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaAgrupado", Column = "TIA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> TitulosAgrupados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Acrescimos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_ACRESCIMO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaAcrescimo", Column = "TBA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> Acrescimos { get; set; }

        /*[NHibernate.Mapping.Attributes.Bag(0, Name = "Descontos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_ACRESCIMO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaDesconto", Column = "TBD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto> Descontos { get; set; }*/

        [NHibernate.Mapping.Attributes.Set(0, Name = "TitulosPendentesGeracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_TITULO_GERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Titulo", Column = "TIT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.Titulo> TitulosPendentesGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cheques", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO_BAIXA_CHEQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TituloBaixaCheque", Column = "TBC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> Cheques { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigosTitulos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(titulos.TIT_CODIGO AS NVARCHAR(20))
	                                                                                    FROM T_TITULO_BAIXA tituloBaixa
	                                                                                    inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
	                                                                                    WHERE tituloBaixa.TIB_CODIGO = TIB_CODIGO AND titulos.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string CodigosTitulos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Fornecedores", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + c.CLI_NOME
                                                                                        FROM T_TITULO_BAIXA tituloBaixa
                                                                                        inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                        inner join T_TITULO tt on tt.TIT_CODIGO = titulos.TIT_CODIGO
                                                                                        inner join T_CLIENTE c on c.CLI_CGCCPF = tt.CLI_CGCCPF
                                                                                        WHERE tituloBaixa.TIB_CODIGO = TIB_CODIGO  FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string Fornecedores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCargas", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CAR_CODIGO_CARGA_EMBARCADOR AS NVARCHAR(20))
	                                                                                    FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO BA ON B.TIB_CODIGO = BA.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON T.TIT_CODIGO = BA.TIT_CODIGO
                                                                                        JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
                                                                                        JOIN T_FATURA_CARGA D ON D.FAT_CODIGO = FP.FAT_CODIGO AND D.FAC_STATUS <> 3
                                                                                        JOIN T_CARGA C ON C.CAR_CODIGO = D.CAR_CODIGO
	                                                                                    WHERE BA.TIB_CODIGO = TIB_CODIGO AND BA.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPendente", Formula = @"(SELECT ISNULL(SUM(T.TIT_VALOR_ORIGINAL), 0) - ISNULL(SUM(T.TIT_DESCONTO), 0) + ISNULL(SUM(T.TIT_ACRESCIMO), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO AND T.TIT_STATUS <> 4
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorPendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPendenteMoedaEstrangeira", Formula = @"(SELECT ISNULL(SUM(T.TIT_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO AND T.TIT_STATUS <> 4
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorPendenteMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPendenteBaixa", Formula = @"(SELECT ISNULL(SUM(T.TIT_VALOR_ORIGINAL), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO AND T.TIT_STATUS <> 4
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorPendenteBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginal", Formula = @"(SELECT ISNULL(SUM(T.TIT_VALOR_ORIGINAL), 0) - ISNULL(SUM(T.TIT_DESCONTO), 0) + ISNULL(SUM(T.TIT_ACRESCIMO), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO AND T.TIT_STATUS <> 4
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFaturas", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST((CASE WHEN F.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR F.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN F.FAT_NUMERO ELSE F.FAT_NUMERO_FATURA_INTEGRACAO END) AS NVARCHAR(20))
                                                                                        FROM T_TITULO_BAIXA tituloBaixa
                                                                                        inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON T.TIT_CODIGO = titulos.TIT_CODIGO
                                                                                        JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
                                                                                        JOIN T_FATURA F ON F.FAT_CODIGO = FP.FAT_CODIGO
                                                                                        WHERE tituloBaixa.TIB_CODIGO = TIB_CODIGO AND titulos.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroFaturas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePessoa", Formula = @"(SELECT ISNULL(COUNT(DISTINCT T.CLI_CGCCPF), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO                                                                                                                                                                               
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(int), Lazy = true)]
        public virtual int QuantidadePessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeGrupoPessoa", Formula = @"(SELECT ISNULL(COUNT(DISTINCT C.GRP_CODIGO), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO
                                                                                        JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                                                                        LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = C.GRP_CODIGO
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(int), Lazy = true)]
        public virtual int QuantidadeGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoGrupoPessoa", Formula = @"(SELECT DISTINCT TOP 1 ISNULL(G.GRP_CODIGO, 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO
                                                                                        JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                                                                        JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = C.GRP_CODIGO
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(int), Lazy = true)]
        public virtual int CodigoGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorConhecimentosFaturamento", Formula = @"(SELECT ISNULL(SUM(C.CON_VALOR_RECEBER), 0)
                                                                                        FROM T_TITULO_BAIXA tituloBaixa
                                                                                        inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON T.TIT_CODIGO = titulos.TIT_CODIGO
                                                                                        JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
                                                                                        JOIN T_FATURA F ON F.FAT_CODIGO = FP.FAT_CODIGO
                                                                                        JOIN T_FATURA_CARGA_DOCUMENTO FC ON FC.FAT_CODIGO = F.FAT_CODIGO AND FC.FCD_STATUS_DOCUMENTO = 1
                                                                                        JOIN T_CTE C ON C.CON_CODIGO = FC.CON_CODIGO
                                                                                        WHERE tituloBaixa.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorConhecimentosFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTituloOriginal", Formula = @"(SELECT ISNULL(SUM(T.TIT_VALOR_TITULO_ORIGINAL), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO AND T.TIT_STATUS <> 4
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorTituloOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoTituloOriginal", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL AS NVARCHAR(20))
	                                                                                    FROM T_TITULO_BAIXA tituloBaixa
	                                                                                    inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON titulos.TIT_CODIGO = T.TIT_CODIGO        
	                                                                                    WHERE tituloBaixa.TIB_CODIGO = TIB_CODIGO AND titulos.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string TipoDocumentoTituloOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoTituloOriginal", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL AS NVARCHAR(2000))
	                                                                                    FROM T_TITULO_BAIXA tituloBaixa
	                                                                                    inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON titulos.TIT_CODIGO = T.TIT_CODIGO        
	                                                                                    WHERE tituloBaixa.TIB_CODIGO = TIB_CODIGO AND titulos.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 4000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroDocumentoTituloOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalAcrescimo", Formula = @"(SELECT ISNULL(SUM(T.TIT_ACRESCIMO), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorTotalAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDesconto", Formula = @"(SELECT ISNULL(SUM(T.TIT_DESCONTO), 0)
                                                                                        FROM T_TITULO_BAIXA B
                                                                                        JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                                                                                        JOIN T_TITULO T ON A.TIT_CODIGO = T.TIT_CODIGO
                                                                                        WHERE B.TIB_CODIGO = TIB_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorTotalDesconto { get; set; }



        public virtual decimal ValorTotalSaldo
        {
            get
            {
                return this.ValorPendenteBaixa + this.ValorTotalAcrescimo - this.ValorTotalDesconto;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual string DescricaoSituacaoBaixaTitulo
        {
            get
            {
                switch (this.SituacaoBaixaTitulo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada:
                        return "Cancelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao:
                        return "Em Negociação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada:
                        return "Iniciada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada:
                        return "Finalizada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmFinalizacao:
                        return "Em Finalização";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmGeracao:
                        return "Em Geração";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(TituloBaixa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
