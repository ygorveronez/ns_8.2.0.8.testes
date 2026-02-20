using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_INTEGRACAO_PEDIDO_ARQUIVO", EntityName = "PedidoIntegracaoArquivo", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo", NameType = typeof(PedidoIntegracaoArquivo))]
    public class PedidoIntegracaoArquivo : Integracao.IntegracaoArquivo
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PIA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public override DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "PIA_MENSAGEM", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public override string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PIA_TIPO", TypeType = typeof(TipoArquivoIntegracaoCTeCarga), NotNull = true)]
        public override TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

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