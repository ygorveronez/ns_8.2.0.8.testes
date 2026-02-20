using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_IMPORTACAO_PLANILHA", EntityName = "PedidoImportacaoPlanilha", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoImportacaoPlanilha", NameType = typeof(PedidoImportacaoPlanilha))]
    public class PedidoImportacaoPlanilha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIP_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIP_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIP_DATA_FINALIZACAO_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacaoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIP_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoImportacaoPlanilha), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoImportacaoPlanilha Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIP_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }
    }
}
