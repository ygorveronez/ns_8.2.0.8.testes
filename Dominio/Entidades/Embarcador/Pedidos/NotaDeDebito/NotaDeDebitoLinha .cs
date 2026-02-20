using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_NOTA_DEBITO_LINHA", EntityName = "NotaDeDebitoLinha", Name = "Dominio.Entidades.Embarcador.Pedidos.NotaDebito.NotaDeDebitoLinha", NameType = typeof(NotaDeDebitoLinha))]
    public class NotaDeDebitoLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NDL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaDeDebito", Column = "NTD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito NotaDeDebito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NDL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "NDL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NDL_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Colunas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTA_DEBITO_COLUNA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NDL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotaDeDebitoLinhaColuna", Column = "NDC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebitoLinhaColuna> Colunas { get; set; }

    }
}

