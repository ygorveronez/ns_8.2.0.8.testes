using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_TABELA_FRETE_COMISSAO_PRODUTO", EntityName = "LogAlteracaoTabelaComissaoProduto", Name = "Dominio.Entidades.Embarcador.Frete.LogAlteracaoTabelaComissaoProduto", NameType = typeof(LogTabelaComissaoProduto))]
    public class LogTabelaComissaoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualValorProduto", Column = "LTC_PERCENTUAL_VALOR_PRODUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualValorProduto { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "LTC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRegistroLog", Column = "LTC_DATA_REGISTRO_LOG", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRegistroLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLog", Column = "LTC_TIPO_LOG", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLog), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLog TipoLog { get; set; }

        #region Obsoletos

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimitePercentual", Column = "LTC_VALOR_LIMITE_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLimitePercentual { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorToneladaEntregue", Column = "LTC_VALOR_TONELADA_ENTREGUE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorToneladaEntregue { get; set; }

        #endregion

        public virtual string DescricaoTipoLog
        {
            get
            {
                if (this.TipoLog == ObjetosDeValor.Embarcador.Enumeradores.TipoLog.insert)
                    return "Inserido";
                if (this.TipoLog == ObjetosDeValor.Embarcador.Enumeradores.TipoLog.update)
                    return "Atualizado";
                if (this.TipoLog == ObjetosDeValor.Embarcador.Enumeradores.TipoLog.delete)
                    return "Excluído";
                return "";
            }
        }

        public virtual bool Equals(LogTabelaComissaoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual void SetarLog(Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto tabelaFreteComissaoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog tipoLog, Dominio.Entidades.Usuario usuario)
        {
            this.Ativo = tabelaFreteComissaoProduto.Ativo;
            this.DataRegistroLog = DateTime.Now;
            this.ContratoFreteTransportador = tabelaFreteComissaoProduto.ContratoFreteTransportador;
            this.Pessoa = tabelaFreteComissaoProduto.Pessoa;
            this.GrupoPessoas = tabelaFreteComissaoProduto.GrupoPessoas;
            this.PercentualValorProduto = tabelaFreteComissaoProduto.PercentualValorProduto;
            this.ProdutoEmbarcador = tabelaFreteComissaoProduto.ProdutoEmbarcador;
            this.TipoLog = tipoLog;
            this.TabelaFrete = tabelaFreteComissaoProduto.TabelaFrete;
            this.Usuario = usuario;
        }
    }
}
