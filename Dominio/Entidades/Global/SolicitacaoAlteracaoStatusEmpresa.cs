using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_STATUS_EMPRESA", EntityName = "SolicitacaoAlteracaoStatusEmpresa", Name = "Dominio.Entidades.SolicitacaoAlteracaoStatusEmpresa", NameType = typeof(SolicitacaoAlteracaoStatusEmpresa))]
    public class SolicitacaoAlteracaoStatusEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SOL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_SOLICITANTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaSolicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_ALTERAR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaAlterar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "SOL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SOL_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }
    }
}
