using System;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRANSFERENCIA_PRODUTO_EMBARCADOR", EntityName = "TransferenciaProdutoEmbarcadorLote", Name = "Dominio.Entidades.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote", NameType = typeof(TransferenciaProdutoEmbarcadorLote))]
    public class TransferenciaProdutoEmbarcadorLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "TPE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "TPE_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoPosicao", Column = "DPO_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DepositoPosicao DepositoPosicaoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoPosicao", Column = "DPO_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DepositoPosicao DepositoPosicaoDestino { get; set; }
    }
}
