using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_NATUREZA_OPERACAO", EntityName = "ConfiguracaoNaturezaOperacao", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao", NameType = typeof(ConfiguracaoNaturezaOperacao))]
    public class ConfiguracaoNaturezaOperacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNP_CODIGO")]
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

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeRemetente { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeDestinatario { get; set; }

        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoEmissorDiferenteUFOrigem", Column = "CNP_ESTADO_EMISSOR_DIFERENTE_UF_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoEmissorDiferenteUFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoOrigemDiferente", Column = "CNP_UF_ORIGEM_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoOrigemDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoDestinoDiferente", Column = "CNP_UF_DESTINO_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoDestinoDiferente { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoOrigemDiferenteUFDestino", Column = "CNP_UF_ORIGEM_DIFERENTE_UF_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoOrigemDiferenteUFDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoOrigemIgualUFDestino", Column = "CNP_UF_ORIGEM_IGUAL_UF_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoOrigemIgualUFDestino { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CNP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO_ESCRITURACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NaturezaDaOperacao NaturezaDaOperacaoCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO_CONTABILIZACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NaturezaDaOperacao NaturezaDaOperacaoVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracaoNaturezaOperacaoContabilizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NATUREZA_OPERACAO_CONTABILIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoNaturezaOperacaoContabilizacao", Column = "CCT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao> ConfiguracaoNaturezaOperacaoContabilizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracaoNaturezaOperacaoEscrituracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NATUREZA_OPERACAO_ESCRITURACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoNaturezaOperacaoEscrituracao", Column = "CCE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao> ConfiguracaoNaturezaOperacaoEscrituracoes { get; set; }

        public virtual bool Equals(ConfiguracaoNaturezaOperacao other)
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
