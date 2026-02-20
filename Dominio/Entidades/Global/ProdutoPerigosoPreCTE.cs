namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_PRODUTO_PERIG", EntityName = "ProdutoPerigosoPreCTE", Name = "Dominio.Entidades.ProdutoPerigosoPreCTE", NameType = typeof(ProdutoPerigosoPreCTE))]
    public class ProdutoPerigosoPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseRisco", Column = "PCP_CLASSERISCO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string ClasseRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroONU", Column = "PCP_NUMERO_ONU", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeApropriado", Column = "PCP_NOMEAPRO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeApropriado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PCP_QUANTIDADE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "PCP_VOLUMES", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "PCP_GRUPO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string Grupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontoFulgor", Column = "PCP_PONTO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string PontoFulgor { get; set; }
    }
}
