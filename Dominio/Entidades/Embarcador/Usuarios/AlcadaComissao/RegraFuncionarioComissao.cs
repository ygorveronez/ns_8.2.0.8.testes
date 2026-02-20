using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_FUNCIONARIO_COMISSAO", EntityName = "RegraFuncionarioComissao", Name = "Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao", NameType = typeof(RegraFuncionarioComissao))]
    public class RegraFuncionarioComissao : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFC_ATIVO_FUNCIONARIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFC_ATIVO_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_FUNCIONARIO_COMISSAO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFuncionario", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FUNCIONARIO_COMISSAO_FUNCIONARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaComissao.AlcadaFuncionario", Column = "AFF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario> AlcadasFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FUNCIONARIO_COMISSAO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaComissao.AlcadaValor", Column = "ACV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor> AlcadasValor { get; set; }
    }
}
