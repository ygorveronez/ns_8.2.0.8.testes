using Dominio.Interfaces.Embarcador.Entidade;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.ICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_ICMS", EntityName = "RegraICMS", Name = "Dominio.Entidades.Embarcador.ICMS.RegraICMS", NameType = typeof(RegraICMS))]
    public class RegraICMS : EntidadeBase, IEquatable<RegraICMS>, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RIC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "RIC_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRegraICMS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRegraICMS Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_REGRA_ORIGINARIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraICMS RegraOriginaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAlteracao", Column = "RIC_SITUACAO_ALTERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoRegraICMS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoRegraICMS SituacaoAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_EMITENTE_DEFERENTE_DE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFEmitenteDiferente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFEmitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeRemetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoRemetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [Obsolete("Utilizar a lista de tipos de operação.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [Obsolete("Utilizar a lista de tipos de operação.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aliquota", Column = "RIC_ALIQUOTA", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal? Aliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBC", Column = "RIC_PERCENTUAL_REDUCAO_BC", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal? PercentualReducaoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCreditoPresumido", Column = "RIC_PERCENTUAL_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal? PercentualCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "RIC_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRegra", Column = "RIC_DESCRICAO_REGRA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DescricaoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimeLeiNoCTe", Column = "RIC_IMPRIMME_LEI_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimeLeiNoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SetorEmpresa", Column = "RIC_SETOR_EMPRESA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SetorEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ZerarValorICMS", Column = "RIC_ZERAR_VALOR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ZerarValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RIC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoOrigemDiferente", Column = "RIC_UF_ORIGEM_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoOrigemDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoDestinoDiferente", Column = "RIC_UF_DESTINO_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoDestinoDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaSimples", Column = "RIC_ALIQUOTA_SIMPLES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? AliquotaSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteOptanteSimplesNacional", Column = "RIC_SOMENTE_OPTANTE_SIMPLES_NACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteOptanteSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_VIGENCIA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_VIGENCIA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeAnexo", Column = "RIC_NOME_ANEXO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoAnexo", Column = "RIC_CAMINHO_ANEXO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CaminhoAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_REGIME_TRIBUTARIO_TOMADOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario? RegimeTributarioTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_REGIME_TRIBUTARIO_TOMADOR_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegimeTributarioTomadorDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_ATIVIDADE_TOMADOR_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtividadeTomadorDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarICMSDoValorAReceber", Column = "RIC_DESCONTAR_ICMS_ST_QUANDO_ICMS_NAO_INCLUSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarICMSDoValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoReduzirRetencaoICMSDoValorDaPrestacao", Column = "RIC_NAO_REDUZIR_RETENCAO_ICMS_DO_VALOR_DA_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoReduzirRetencaoICMSDoValorDaPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_NAO_IMPRIMIR_IMPOSTOS_DACTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImprimirImpostosDACTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_NAO_ENVIAR_IMPOSTOS_ICMS_NA_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarImpostoICMSNaEmissaoCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_NAO_INCLUIR_ICMS_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoIncluirICMSValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModal", Column = "RIC_TIPO_MODAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal? TipoModal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_UF_TOMADOR_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EstadoTomadorDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "RIC_TIPO_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServico), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoServico? TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_UF_ORIGEM_IGUAL_UF_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UFOrigemIgualUFTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "RIC_PAGOAPAGAR", TypeType = typeof(Dominio.Enumeradores.TipoPagamentoRegraICMS), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamentoRegraICMS? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroProposta", Column = "RIC_NUMERO_PROPOSTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroProposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirPisConfisNaBC", Column = "RIC_INCLUIR_PIS_CONFIS_NA_BC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirPisConfisNaBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIncluirPisConfisNaBCParaComplementos", Column = "RIC_NAO_INCLUIR_PIS_CONFIS_NA_BC_PARA_COMPLEMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirPisConfisNaBCParaComplementos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoAlertaVencimento", Column = "RIC_DATA_ULTIMO_ALERTA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoAlertaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ProdutosEmbarcador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_REGRA_ICMS_PRODUTO_EMBARCADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO")]
        public virtual ICollection<Produtos.ProdutoEmbarcador> ProdutosEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_REGRA_ICMS_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposDeCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_REGRA_ICMS_TIPO_DE_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> TiposDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultisoftware", Column = "RIC_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = false)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber", Column = "RIC_NAO_CALCULAR_ICMS_REDUZIDO_PARA_TOTAL_PRESTACAO_VALORES_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalcularICMSReduzidoParaTotalPrestacaoValoresReceber { get; set; }

        public virtual bool Equals(RegraICMS other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual RegraICMS Clonar()
        {
            return (RegraICMS)this.MemberwiseClone();
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool CSTIsenta
        {
            get { return (this.CST == "40" || this.CST == "41" || this.CST == "51" || this.CST == ""); }
        }
    }
}
