using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_TAKE_PAY_LINHA", EntityName = "ImportacaoTakePayLinha", Name = "Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha", NameType = typeof(ImportacaoTakePayLinha))]
    public class ImportacaoTakePayLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoTakePay", Column = "ITP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay ImportacaoTakePay { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ITL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "ITL_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Colunas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPORTACAO_TAKE_PAY_COLUNA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ITL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ImportacaoTakePayLinhaColuna", Column = "ITC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> Colunas { get; set; }


    }
}