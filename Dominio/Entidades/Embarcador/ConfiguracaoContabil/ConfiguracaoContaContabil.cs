using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONTA_CONTABIL", EntityName = "ConfiguracaoContaContabil", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil", NameType = typeof(ConfiguracaoContaContabil))]
    public class ConfiguracaoContaContabil : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaPessoa", Column = "CTP_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.CategoriaPessoa CategoriaRemetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaPessoa", Column = "CTP_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.CategoriaPessoa CategoriaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaPessoa", Column = "CTP_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.CategoriaPessoa CategoriaTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoRemetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CCC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDocumentoTransporte", Column = "TDT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte TipoDT { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracaoContaContabilContabilizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_CONTA_CONTABIL_CONTABILIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoContaContabilContabilizacao", Column = "CCT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> ConfiguracaoContaContabilContabilizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracaoContaContabilEscrituracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_CONTA_CONTABIL_ESCRITURACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoContaContabilEscrituracao", Column = "CCE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> ConfiguracaoContaContabilEscrituracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracaoContaContabilProvisoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_CONTA_CONTABIL_PROVISAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoContaContabilProvisao", Column = "CCP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao> ConfiguracaoContaContabilProvisoes { get; set; }

        public virtual bool Equals(ConfiguracaoContaContabil other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.TipoOperacao?.Descricao.ToString() ?? string.Empty;
            }
        }
    }

}
