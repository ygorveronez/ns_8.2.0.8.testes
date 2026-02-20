namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL_DESCONTO_EXCECAO", EntityName = "FilialDescontoExcecao", Name = "Dominio.Entidades.Embarcador.Seguros.FilialDescontoExcecao", NameType = typeof(FilialDescontoExcecao))]
    public class FilialDescontoExcecao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "FDE_HORA_INICIO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFim", Column = "FDE_HORA_FIM", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string HoraFim { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
