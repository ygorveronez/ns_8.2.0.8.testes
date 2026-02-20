using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO", EntityName = "Produto", Name = "Dominio.Entidades.Produto", NameType = typeof(Produto))]
    public class Produto : EntidadeBase, IEqualityComparer<Produto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeMedidaGeral", Column = "UMG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeMedidaGeral UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NCM", Column = "NCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NCM NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PRO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProduto", Column = "PRO_COD_PRODUTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNCM", Column = "PRO_COD_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string CodigoNCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PRO_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeDeMedida", Column = "PRO_UNIDADE_MEDIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida? UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoNotaFiscal", Column = "PRO_DESCRICAO_NOTA_FISCAL", TypeType = typeof(string), Length = 120, NotNull = false)]
        public virtual string DescricaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemMercadoria", Column = "PRO_ORIGEM_MERCADORIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria? OrigemMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoCusto", Column = "PRO_ULTIMO_CUSTO", TypeType = typeof(decimal), Scale = 10, Precision = 18, NotNull = false)]
        public virtual decimal UltimoCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoMedio", Column = "PRO_CUSTO_MEDIO", TypeType = typeof(decimal), Scale = 10, Precision = 18, NotNull = false)]
        public virtual decimal CustoMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MargemLucro", Column = "PRO_MARGEM_LUCRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MargemLucro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorVenda", Column = "PRO_VALOR_VENDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "PRO_PESO_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "PRO_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEAN", Column = "PRO_CODIGO_EAN", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CodigoEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEnquadramentoIPI", Column = "PRO_CODIGO_ENQUADRAMENTO_IPI", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoEnquadramentoIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIPIVenda", Column = "PRO_CST_IPI_VENDA", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI CSTIPIVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIPIVenda", Column = "PRO_ALIQUOTA_IPI_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIPIVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIPICompra", Column = "PRO_CST_IPI_COMPRA", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI CSTIPICompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIPICompra", Column = "PRO_ALIQUOTA_IPI_COMPRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIPICompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaProduto", Column = "PRO_CATEGORIA_PRODUTO", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto CategoriaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeneroProduto", Column = "PRO_GENERO_PRODUTO", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeneroProduto GeneroProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CEST", Column = "CES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CEST CEST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida Unidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoImposto", Column = "GRI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto GrupoImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCEST", Column = "PRO_COD_CEST", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string CodigoCEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarrasEAN", Column = "PRO_CODIGO_BARRAS_EAN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoBarrasEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalculoCustoProduto", Column = "PRO_CALCULO_CUSTO_PRODUTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CalculoCustoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoEmEBS", Column = "PRO_PRODUTO_EM_EBS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProdutoEmEBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAnvisa", Column = "PRO_CODIGO_ANVISA", TypeType = typeof(string), Length = 13, NotNull = false)]
        public virtual string CodigoAnvisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoANP", Column = "PRO_CODIGO_ANP", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string CodigoANP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGLP", Column = "PRO_PERCENTUAL_GLP", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal PercentualGLP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGNN", Column = "PRO_PERCENTUAL_GNN", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal PercentualGNN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGNI", Column = "PRO_PERCENTUAL_GNI", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal PercentualGNI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualOrigemCombustivel", Column = "PRO_PERCENTUAL_ORIGEM_COMBUSTIVEL", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal PercentualOrigemCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMisturaBiodiesel", Column = "PRO_PERCENTUAL_MISTURA_BIODIESEL", TypeType = typeof(decimal), Scale = 4, Precision = 7, NotNull = false)]
        public virtual decimal PercentualMisturaBiodiesel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPartidaANP", Column = "PRO_VALOR_PARTIDA_ANP", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorPartidaANP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorEscalaRelevante", Column = "PRO_INDICADOR_ESCALA_RELEVANTE", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante IndicadorEscalaRelevante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorImportacaoCombustivel", Column = "PRO_INDICADOR_IMPORTACAO_COMBUSTIVEL", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorImportacaoCombustivel IndicadorImportacaoCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFabricante", Column = "PRO_CNPJ_FABRICANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJFabricante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBeneficioFiscal", Column = "PRO_CODIGO_BENEFICIO_FISCAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoBeneficioFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProdutoTMS", Column = "GPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS GrupoProdutoTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPneuAutomatico", Column = "PRO_GERAR_PNEU_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarPneuAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.ModeloPneu", Column = "PML_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.ModeloPneu Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.BandaRodagemPneu", Column = "PBR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu BandaRodagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoEPI", Column = "PRO_PRODUTO_EPI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProdutoEPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCA", Column = "PRO_NUMERO_CA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NumeroCA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoCombustivel", Column = "PRO_PRODUTO_COMBUSTIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProdutoCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaEstoqueCombustivel", Column = "PRO_CONTROLA_ESTOQUE_COMBUSTIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ControlaEstoqueCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoKIT", Column = "PRO_PRODUTO_KIT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProdutoKIT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoBem", Column = "PRO_PRODUTO_BEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProdutoBem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Almoxarifado", Column = "AMX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frota.Almoxarifado Almoxarifado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoVenda", Column = "PRO_VALOR_MINIMO_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.LocalArmazenamentoProduto LocalArmazenamentoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaProduto", Column = "MAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.MarcaProduto MarcaProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FinalidadeProdutoOrdemServico", Column = "FPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frota.FinalidadeProdutoOrdemServico FinalidadeProdutoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoFornecedor", Column = "PFO_CODIGO")]
        public virtual IList<ProdutoFornecedor> Fornecedores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_FOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoFoto", Column = "PFT_CODIGO")]
        public virtual IList<ProdutoFoto> Foto { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_COMPOSICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoComposicao", Column = "PCO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Produtos.ProdutoComposicao> Composicoes { get; set; }

        public virtual string DescricaoIndicadorEscalaRelevante
        {
            get
            {
                switch (this.IndicadorEscalaRelevante)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.Nenhum:
                        return "Nenhum";
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.ProduzidoEscalaNaoRelevante:
                        return "N – Produzido em Escala NÃO Relevante";
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.ProduzidoEscalaRelevante:
                        return "S - Produzido em Escala Relevante";
                    default:
                        return "";
                }
            }
        }

        public virtual string CodigoCategoriaProduto
        {
            get
            {
                switch (this.CategoriaProduto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.AtivoImobilizado:
                        return "08";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Embalagem:
                        return "02";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.MaterialUsoConsumo:
                        return "07";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.MateriaPrima:
                        return "01";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.MercadoriaRevenda:
                        return "00";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Outras:
                        return "99";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.OutrosInsumos:
                        return "10";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.ProdutoAcabado:
                        return "04";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.ProdutoEmProcesso:
                        return "03";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.ProdutoIntermediario:
                        return "06";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Servicos:
                        return "09";
                    case ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Subproduto:
                        return "05";
                    default:
                        return "99";
                }
            }
        }

        public virtual string DescricaoCSTIPIVenda
        {
            get
            {
                switch (this.CSTIPIVenda)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST00:
                        return "000";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST01:
                        return "001";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST02:
                        return "002";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03:
                        return "003";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST04:
                        return "004";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST05:
                        return "005";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST49:
                        return "049";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST50:
                        return "050";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51:
                        return "051";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52:
                        return "051";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53:
                        return "053";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST54:
                        return "054";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST55:
                        return "055";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST99:
                        return "099";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCSTIPICompra
        {
            get
            {
                switch (this.CSTIPICompra)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST00:
                        return "000";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST01:
                        return "001";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST02:
                        return "002";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03:
                        return "003";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST04:
                        return "004";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST05:
                        return "005";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST49:
                        return "049";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST50:
                        return "050";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51:
                        return "051";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52:
                        return "051";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53:
                        return "053";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST54:
                        return "054";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST55:
                        return "055";
                    case ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST99:
                        return "099";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return Localization.Resources.Gerais.Geral.Ativo;
                    case "I":
                        return Localization.Resources.Gerais.Geral.Inativo;
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(Produto x, Produto y)
        {
            if (x.Codigo == y.Codigo)
                return true;

            return false;
        }

        public virtual int GetHashCode(Produto obj)
        {
            return obj.Codigo.GetHashCode();
        }
    }
}
