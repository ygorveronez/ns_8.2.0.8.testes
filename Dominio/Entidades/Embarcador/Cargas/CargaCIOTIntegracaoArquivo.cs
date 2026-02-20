using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CIOT_INTEGRACAO_ARQUIVO", EntityName = "CargaCIOTIntegracaoArquivo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo", NameType = typeof(CargaCIOTIntegracaoArquivo))]
    public class CargaCIOTIntegracaoArquivo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CCI_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CCI_MENSAGEM", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CCI_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public virtual TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }
    }
}
