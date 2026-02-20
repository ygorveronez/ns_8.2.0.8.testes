using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_GRUPO_MOTORISTAS_FUNCIONARIO", EntityName = "GrupoMotoristasFuncionario", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario", NameType = typeof(GrupoMotoristasFuncionario))]
    public class GrupoMotoristasFuncionario : EntidadeBase, Interfaces.Embarcador.Logistica.GrupoMotoristas.IEntidadeRelacionamentoGrupoMotoristas
    {
        [Id(Name = "Codigo", Type = "int", Column = "GMF_CODIGO")]
        [Generator(Class = "native")]
        public virtual int Codigo { get; set; }

        [ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [ManyToOne(0, Class = "GrupoMotoristas", Column = "GMO_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual GrupoMotoristas GrupoMotoristas { get; set; }

        public virtual string Descricao { get { return Funcionario?.Nome; } }

    }
}