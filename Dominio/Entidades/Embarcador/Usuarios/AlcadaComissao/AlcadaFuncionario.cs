namespace Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_FUNCIONARIO_COMISSAO_FUNCIONARIO", EntityName = "AlcadaComissao.AlcadaFuncionario", Name = "Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario", NameType = typeof(AlcadaFuncionario))]
    public class AlcadaFuncionario : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AFF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraFuncionarioComissao", Column = "RFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraFuncionarioComissao RegraFuncionarioComissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Funcionario?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Usuario PropriedadeAlcada
        {
            get
            {
                return this.Funcionario;
            }
            set
            {
                this.Funcionario = value;
            }
        }
    }
}
