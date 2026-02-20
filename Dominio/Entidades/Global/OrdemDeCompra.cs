using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA", EntityName = "OrdemDeCompra", Name = "Dominio.Entidades.OrdemDeCompra", NameType = typeof(OrdemDeCompra))]
    public class OrdemDeCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ODC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ODC_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ODC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Servico", Column = "ODC_SERVICO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Servico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSolicitante", Column = "ODC_SOLICITANTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeSolicitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Setor", Column = "ODC_SETOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Setor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeiculo", Column = "VMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloVeiculo ModeloVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ODC_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOrdemDeCompra", Column = "ODC_TIPO", TypeType = typeof(Enumeradores.TipoOrdemDeCompra), NotNull = false)]
        public virtual Enumeradores.TipoOrdemDeCompra? TipoOrdemDeCompra { get; set; }
    }
}
