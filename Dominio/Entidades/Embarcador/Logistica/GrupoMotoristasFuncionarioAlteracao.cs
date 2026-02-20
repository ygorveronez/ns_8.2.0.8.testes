using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_GRUPO_MOTORISTAS_FUNCIONARIO_ALTERACAO", EntityName = "GrupoMotoristasFuncionarioAlteracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionarioAlteracao", NameType = typeof(GrupoMotoristasFuncionarioAlteracao))]
    public class GrupoMotoristasFuncionarioAlteracao : EntidadeBase
    {
        [Id(Name = "Codigo", Type = "int", Column = "GFA_CODIGO")]
        [Generator(Class = "native")]
        public virtual int Codigo { get; set; }

        [ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [ManyToOne(0, Class = "GrupoMotoristas", Column = "GMO_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual GrupoMotoristas GrupoMotoristas { get; set; }

        [Property(Name = "Acao", Column = "GFA_ACAO", TypeType = typeof(int), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GrupoMotoristaAtualizarAcao Acao { get; set; }

        public virtual string Descricao { get { return Funcionario?.Nome; } }
    }
}