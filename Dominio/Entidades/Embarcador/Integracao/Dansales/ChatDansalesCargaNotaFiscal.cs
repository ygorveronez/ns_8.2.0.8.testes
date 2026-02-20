using System;

namespace Dominio.Entidades.Embarcador.Integracao.Dansales
{
    /// <summary>
    /// Essa entidade é uma ligação do chat entre cargas e notas fiscais.
    /// Quando uma carga e uma nota fiscal estão ligadas por essa entidade, todas as mensagens do chat da carga
    /// vão ser também integradas com a Dansales (que usa como ID do chat uma nota fiscal).
    /// 
    /// Essa entidade é criada e deletada dinamicamente quando uma nota é incluída/excluida de um chat pelo operador da carga.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAT_CARGA_NOTA_FISCAL", EntityName = "ChatCargaNotaFiscal", Name = "Dominio.Entidades.Embarcador.Integracao.Dansales.ChatCargaNotaFiscal", NameType = typeof(ChatDansalesCargaNotaFiscal))]
    public class ChatDansalesCargaNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "CCN_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChatMobileMensagem", Column = "CMM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
