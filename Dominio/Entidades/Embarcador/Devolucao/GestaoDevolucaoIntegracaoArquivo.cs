using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_INTEGRACAO_ARQUIVO", EntityName = "GestaoDevolucaoIntegracaoArquivo", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo", NameType = typeof(GestaoDevolucaoIntegracaoArquivo))]
    public class GestaoDevolucaoIntegracaoArquivo : Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "DIA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "DIA_MENSAGEM", Type = "StringClob", NotNull = false)]
        public override string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "DIA_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = false)]
        public override TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoResposta { get; set; }
    }
}
