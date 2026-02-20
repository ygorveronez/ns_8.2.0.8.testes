using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_VALE_PEDAGIO_INTEGRACAO_ARQUIVO", EntityName = "CargaValePedagioIntegracaoArquivo", Name = "Dominio.Entidades.Embarcador.Cargas.ValePedagio", NameType = typeof(CargaValePedagioIntegracaoArquivo))]
    public class CargaValePedagioIntegracaoArquivo : Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CVI_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CVI_MENSAGEM", Type = "StringClob", NotNull = true)]
        public override string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CVI_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public override TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }
    }
}
