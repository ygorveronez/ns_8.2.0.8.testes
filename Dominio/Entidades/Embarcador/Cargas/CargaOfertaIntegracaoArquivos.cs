using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OFERTA_INTEGRACAO_ARQUIVOS", EntityName = "CargaOfertaIntegracaoArquivos", Name = "Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos", NameType = typeof(CargaOfertaIntegracaoArquivos))]
    public class CargaOfertaIntegracaoArquivos : Dominio.Entidades.Embarcador.Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        override public int Codigo { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOfertaIntegracao", Column = "COI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao CargaOfertaIntegracao { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CIA_DATA_HORA", TypeType = typeof(DateTime), NotNull = true)]
        override public DateTime Data { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CIA_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        override public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga Tipo { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "CIA_ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        override public Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "CIA_ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        override public Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CIA_MENSAGEM", Length = 255, TypeType = typeof(string), NotNull = true)]
        override public string Mensagem { get; set; }
    }
}