using System;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABASTECIMENTO_BOMBA", EntityName = "BombaAbastecimento", Name = "Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento", NameType = typeof(BombaAbastecimento))]
    public class BombaAbastecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "ABB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ABB_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOleo", Column = "TOL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOleo TipoOleo { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ABB_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoBombaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ABB_CODIGO_INTEGRACAO_BICO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoBicoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.LocalArmazenamentoProduto LocalArmazenamentoProduto { get; set; }
    }
}