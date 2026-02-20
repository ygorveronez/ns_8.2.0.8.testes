using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_INTEGRACAO_ARQUIVO", EntityName = "CargaCTeIntegracaoArquivo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo", NameType = typeof(CargaCTeIntegracaoArquivo))]
    public class CargaCTeIntegracaoArquivo : Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CCA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CCA_MENSAGEM", Type = "StringClob", NotNull = false)]
        public override string Mensagem { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CCA_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public override TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoResposta { get; set; }
    }
}
