using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.WebService
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRADORA_INTEGRACAO_RETORNO", EntityName = "IntegradoraIntegracaoRetorno", Name = "Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno", NameType = typeof(IntegradoraIntegracaoRetorno))]
    public class IntegradoraIntegracaoRetorno : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "IIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_ORIGEM", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Enumerador.OrigemAuditado Origem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_SUCESSO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Sucesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_MENSAGEM", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_NUMERO_IDENTIFICACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NumeroIdentificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_SITUACAO_RETORNO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRADORA_INTEGRACAO_RETORNO_ARQUIVOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IIR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IntegradoraIntegracaoRetornoArquivo", Column = "IIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo> ArquivosIntegracaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IIR_HASH", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string HashJson { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "IIR_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaTentativa", Column = "IIR_DATA_ULTIMA_TENTATIVA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaTentativa { get; set; }


        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoSituacaoRetorno
        {
            get { return this.SituacaoRetorno == SituacaoIntegracao.AgIntegracao ? "Sem integração retorno" : this.SituacaoRetorno.ObterDescricao(); }
        }

    }
}
