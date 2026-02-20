using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.SuperApp
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_INTEGRACAO_SUPER_APP", EntityName = "IntegracaoSuperApp", Name = "Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp", NameType = typeof(IntegracaoSuperApp))]
    public class IntegracaoSuperApp : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ISA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "ISA_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoProcessamento", Column = "ISA_SITUACAO_PROCESSAMENTO", TypeType = typeof(SituacaoProcessamentoIntegracao), NotNull = true)]
        public virtual SituacaoProcessamentoIntegracao SituacaoProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalhesProcessamento", Column = "ISA_DETALHES_PROCESSAMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string DetalhesProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEvento", Column = "ISA_TIPO_EVENTO", TypeType = typeof(TipoEventoApp), NotNull = true)]
        public virtual TipoEventoApp TipoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "INT_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ISA_JSON_REQUEST", Type = "StringClob", NotNull = false)]
        public virtual string StringJsonRequest { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ISA_JSON_RESPONSE", Type = "StringClob", NotNull = false)]
        public virtual string StringJsonResponse { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "PAT_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "PAT_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        public virtual string Descricao
        {
            get { return TipoEvento.ObterDescricao(); }
        }
    }
}
