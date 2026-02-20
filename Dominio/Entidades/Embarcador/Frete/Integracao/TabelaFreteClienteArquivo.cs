using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_ARQUIVO", EntityName = "TabelaFreteClienteArquivo", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo", NameType = typeof(TabelaFreteClienteArquivo))]
    public class TabelaFreteClienteArquivo : Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "TFA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "TFA_MENSAGEM", Type = "StringClob", NotNull = false)]
        public override string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFA_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public override TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }  
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInternoLBC", Column = "TCA_CODIGO_INTERNO_LBC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoInternoLBC { get; set; }
    }
}