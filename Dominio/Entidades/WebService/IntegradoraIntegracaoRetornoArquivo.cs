using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.WebService
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRADORA_INTEGRACAO_RETORNO_ARQUIVO", EntityName = "IntegradoraIntegracaoRetornoArquivo", Name = "Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo", NameType = typeof(IntegradoraIntegracaoRetornoArquivo))]
    public class IntegradoraIntegracaoRetornoArquivo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "IIA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IIA_MENSAGEM", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "IIA_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public virtual TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ArquivoRequisicao?.NomeArquivo;
            }
        }

        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }
    }
}