using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_OCORRENCIA", EntityName = "RegrasAutorizacaoOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia", NameType = typeof(RegrasAutorizacaoOcorrencia))]
    public class RegrasAutorizacaoOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RAO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RAO_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReprovadores", Column = "RAO_NUMERO_REPROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroReprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AprovacaoAutomaticaAposDias", Column = "RAO_APROVACAO_AUTOMATICA_APOS_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int AprovacaoAutomaticaAposDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDiasAprovacao", Column = "RAO_TIPO_DIAS_APROVACAO", TypeType = typeof(TipoDiasAprovacao), NotNull = false)]
        public virtual TipoDiasAprovacao TipoDiasAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacao", Column = "RAO_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAO_DIAS_PRAZO_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasPrazoAprovacao { get; set; }

        [Obsolete("Não utilizar. Regra substituída pelo número de reprovadores (campo NumeroReprovadores)")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "RAO_REJEITAR_COM_UNICA_REPROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RejeitarComUnicaReprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RAO_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAutorizacaoOcorrencia", Column = "RAO_ETAPA_AUTORIZACAO", TypeType = typeof(EtapaAutorizacaoOcorrencia), NotNull = false)]
        public virtual EtapaAutorizacaoOcorrencia EtapaAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOcorrencia", Column = "RAO_TIPO_OCORRENCIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorComponenteFrete", Column = "RAO_COMPONENTE_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilialEmissao", Column = "RAO_FILIAL_EMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTomadorOcorrencia", Column = "RAO_TOMADOR_OCORRENCIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTomadorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RAO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorOcorrencia", Column = "RAO_VALOR_OCORRENCIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAO_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiasAbertura", Column = "RAO_DIAS_ABERTURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDiasAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorExpedidor", Column = "RAO_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorExpedidor { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RAO_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorCanalEntrega", Column = "RAO_CANAL_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorCanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAO_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_OCORRENCIA_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_TIPO_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaTipoOcorrencia", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia> RegrasTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasComponenteFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaComponenteFrete", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete> RegrasComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilialEmissao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_FILIAL_EMISSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaFilialEmissao", Column = "RFE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao> RegrasFilialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTomadorOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_TOMADOR_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaTomadorOcorrencia", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia> RegrasTomadorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasValorOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_VALOR_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaValorOcorrencia", Column = "RVO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia> RegrasValorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaTipoOperacao", Column = "RTP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao> RegrasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasDiasAbertura", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_DIAS_ABERTURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaDiasAbertura", Column = "RDA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura> RegrasDiasAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasExpedidor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_EXPEDIDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaExpedidor", Column = "RFX_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor> RegrasExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaTipoCarga", Column = "RFX_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga> RegrasTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasCanalEntrega", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_OCORRENCIA_CANAL_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOcorrenciaCanalEntrega", Column = "RFX_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega> RegrasCanalEntrega { get; set; }

        public virtual string DescricaoEtapaAutorizacaoOcorrencia {
            get { return EtapaAutorizacaoOcorrencia.ObterDescricao();  }
        }

        public virtual string DescricaoAtivo
        {
            get { return (this.Ativo) ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(RegrasAutorizacaoOcorrencia other)
        {
            return (this.Codigo == other.Codigo);
        }
    }

}
