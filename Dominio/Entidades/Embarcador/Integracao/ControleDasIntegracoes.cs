using Dominio.Entidades.WebService;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;


namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_DAS_INTEGRACOES", EntityName = "ControleDasIntegracoes", Name = "Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes", NameType = typeof(ControleDasIntegracoes))]
    public class ControleDasIntegracoes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRequisicao", Column = "CDI_DATA_REQUISICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origem", Column = "CDI_ORIGEM", TypeType = typeof(OrigemAuditado), NotNull = false)]
        public virtual OrigemAuditado Origem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMetodo", Column = "CDI_NOME_METODO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeMetodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CDI_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "CDI_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracao), NotNull = false)]
        public virtual SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CDI_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Expiou", Column = "CDI_EXPIOU", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Expiou { get; set; }

        public virtual string DescricaoSituacao { get { return this.Situacao ? "Processado" : "Não Processado"; } }

    }
}
