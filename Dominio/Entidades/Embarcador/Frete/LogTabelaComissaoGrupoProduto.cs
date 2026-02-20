using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_TABELA_FRETE_COMISSAO_GRUPO_PRODUTO", EntityName = "LogTabelaComissaoGrupoProduto", Name = "Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto", NameType = typeof(LogTabelaComissaoGrupoProduto))]
    public class LogTabelaComissaoGrupoProduto: EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoGrupoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LTG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualValorProduto", Column = "LTG_PERCENTUAL_VALOR_PRODUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualValorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "LTG_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRegistroLog", Column = "LTG_DATA_REGISTRO_LOG", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRegistroLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLog", Column = "LTG_TIPO_LOG", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLog), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLog TipoLog { get; set; }

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

        public virtual bool Equals(LogTabelaComissaoGrupoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual void SetarLog(Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto tabelaFreteComissaoGrupoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLog tipoLog, Dominio.Entidades.Usuario usuario)
        {
            this.Ativo = tabelaFreteComissaoGrupoProduto.Ativo;
            this.DataRegistroLog = DateTime.Now;
            this.ContratoFreteTransportador = tabelaFreteComissaoGrupoProduto.ContratoFreteTransportador;
            this.Pessoa = tabelaFreteComissaoGrupoProduto.Pessoa;
            this.GrupoPessoas = tabelaFreteComissaoGrupoProduto.GrupoPessoas;
            this.PercentualValorProduto = tabelaFreteComissaoGrupoProduto.PercentualValorProduto;
            this.GrupoProduto = tabelaFreteComissaoGrupoProduto.GrupoProduto;
            this.TipoLog = tipoLog;
            this.TabelaFrete = tabelaFreteComissaoGrupoProduto.TabelaFrete;
            this.Usuario = usuario;
        }
    }
}
