using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Usuarios.Colaborador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLABORADOR_SITUACAO_LANCAMENTO_INTEGRACAO_ARQUIVO", EntityName = "ColaboradorSituacaoLancamentoIntegracaoArquivo", Name = "Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo", NameType = typeof(ColaboradorSituacaoLancamentoIntegracaoArquivo))]
    public class ColaboradorSituacaoLancamentoIntegracaoArquivo : Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CLA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CLA_MENSAGEM", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public override string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CLA_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public override TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColaboradorSituacaoLancamentoIntegracao", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao ColaboradorSituacaoLancamentoIntegracao { get; set; }
    }
}
