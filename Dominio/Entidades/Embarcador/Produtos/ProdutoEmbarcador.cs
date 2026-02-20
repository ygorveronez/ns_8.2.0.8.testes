using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR", EntityName = "ProdutoEmbarcador", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoEmbarcador", NameType = typeof(ProdutoEmbarcador))]
    public class ProdutoEmbarcador : EntidadeBase, IEquatable<ProdutoEmbarcador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoEmbarcador", Column = "PRO_CODIGO_PRODUTO_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodigoProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumentacao", Column = "PRO_CODIGO_DOCUMENTACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GRP_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCEAN", Column = "PRO_CODIGO_CEAN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCEAN { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PRO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "PRO_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "PRO_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_PESSOA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo CheckList { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TemperaturaTransporte", Column = "PRO_TEMPERATURA_TRANSPORTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TemperaturaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "PRO_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FatorConversao", Column = "PRO_FATOR_CONVERSAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorConversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquidoUnitario", Column = "PRO_PESO_LIQUIDO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquidoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida Unidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaUnidade", Column = "PRO_SIGLA_UNIDADE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SiglaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirExpedicaoEmTempoReal", Column = "PRO_EXIBIR_EXPEDICAO_TEMPO_REAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirExpedicaoEmTempoReal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarPesoProdutoCalculoFrete", Column = "PRO_DESCONTAR_PESO_PRODUTO_CALCULO_FRETE", TypeType = typeof(bool), Scale = 6, Precision = 18, NotNull = false)]
        public virtual bool DescontarPesoProdutoCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorProdutoCalculoFrete", Column = "PRO_DESCONTAR_VALOR_PRODUTO_CALCULO_FRETE", TypeType = typeof(bool), Scale = 6, Precision = 18, NotNull = false)]
        public virtual bool DescontarValorProdutoCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_OBRIGATORIO_GTA", NotNull = false)]
        public virtual bool ObrigatorioGuiaTransporteAnimal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_OBRIGATORIO_NF_PRODUTOR", NotNull = false)]
        public virtual bool ObrigatorioNFProdutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixa", Column = "PRO_QUANTIDADE_CAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixaPorPallet", Column = "PRO_QUANTIDADE_CAIXA_POR_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixaPorPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdPalet", Column = "PRO_QUANTIDADE_PALET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QtdPalet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlturaCM", Column = "PRO_ALTURA_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AlturaCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LarguraCM", Column = "PRO_LARGURA_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal LarguraCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComprimentoCM", Column = "PRO_COMPRIMENTO_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ComprimentoCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubito", Column = "PRO_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClassificacaoRiscoONU", Column = "CRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.ClassificacaoRiscoONU ClassificacaoRiscoONU { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LinhaSeparacao", Column = "CLS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.LinhaSeparacao LinhaSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoEmbalagem", Column = "TE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.TipoEmbalagem TipoEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_POSSUI_INTEGRACAO_COLETA_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoColetaMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_OBRIGATORIO_INFORMAR_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNCM", Column = "PRO_CODIGO_NCM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoNCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_EXIGE_INFORMAR_CAIXAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeInformarCaixas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_EXIGE_INFORMAR_IMUNOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeInformarImunos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaProduto", Column = "MAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.MarcaProduto MarcaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEAN", Column = "PRO_CODIGO_EAN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "PRO_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)] 
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixaPorCamadaPallet", Column = "PRO_QUANTIDADE_CAIXA_POR_CAMADA_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixaPorCamadaPallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Palletizacao", Column = "PAL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.Palletizacao ConfiguracaoPalletizacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Lotes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_EMBARCADOR_LOTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcadorLote", Column = "PEL_CODIGO")]
        public virtual IList<ProdutoEmbarcadorLote> Lotes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_EMBARCADOR_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcadorCliente", Column = "PEC_CODIGO")]
        public virtual IList<ProdutoEmbarcadorCliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_EMBARCADOR_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcadorFilial", Column = "PEF_CODIGO")]
        public virtual IList<ProdutoEmbarcadorFilial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Organizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_EMBARCADOR_ORGANIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcadorOrganizacao", Column = "PEO_CODIGO")]
        public virtual IList<ProdutoEmbarcadorOrganizacao> Organizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TabelaConversao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_EMBARCADOR_TABELA_CONVERSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcadorTabelaConversao", Column = "PTC_CODIGO")]
        public virtual ICollection<ProdutoEmbarcadorTabelaConversao> TabelaConversao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FornecedorInternoProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_EMBARCADOR_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcadorFornecedor", Column = "PEF_CODIGO")]
        public virtual IList<ProdutoEmbarcadorFornecedor> FornecedorInternoProduto { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        public virtual bool Equals(ProdutoEmbarcador other)
        {
            return (other.Codigo == this.Codigo);
        }
        public virtual string UnidadeMedidaSuperApp
        {
            get
            {
                switch (this.SiglaUnidade)
                {
                    case "KG":
                        return "kg";
                    case "LT":
                        return "l";
                    case "M3":
                        return "m³";
                    case "TON":
                        return "t";
                    default:
                        return "";
                }
            }
        }
    }
}
