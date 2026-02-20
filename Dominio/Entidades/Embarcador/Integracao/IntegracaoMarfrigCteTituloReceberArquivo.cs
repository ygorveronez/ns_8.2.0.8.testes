using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_MARFRIG_CTE_TITULO_RECEBER_ARQUIVO", EntityName = "IntegracaoMarfrigCteTituloReceberArquivo", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo", NameType = typeof(IntegracaoMarfrigCteTituloReceberArquivo))]
    public class IntegracaoMarfrigCteTituloReceberArquivo : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "IMA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IMA_MENSAGEM", TypeType = typeof(string), NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

    }
}

