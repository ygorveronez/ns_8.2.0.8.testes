namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_PRODUTO_PERIG", EntityName = "ProdutoPerigosoCTE", Name = "Dominio.Entidades.ProdutoPerigosoCTE", NameType = typeof(ProdutoPerigosoCTE))]
    public class ProdutoPerigosoCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseRisco", Column = "PRD_CLASSERISCO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string ClasseRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroONU", Column = "PRD_NUMERO_ONU", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeApropriado", Column = "PRD_NOMEAPRO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeApropriado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PRD_QUANTIDADE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "PRD_VOLUMES", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "PRD_GRUPO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string Grupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontoFulgor", Column = "PRD_PONTO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string PontoFulgor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTotal", Column = "PRD_QUANTIDADE_TOTAL", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal? QuantidadeTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeDeMedida", Column = "PRD_UNIDADE_MEDIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaCTeAereo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaCTeAereo? UnidadeDeMedida { get; set; }

        public virtual ProdutoPerigosoCTE Clonar()
        {
            return (ProdutoPerigosoCTE)this.MemberwiseClone();
        }
    }
}
