using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [Class(0, Table = "T_GRUPO_MOTORISTAS", EntityName = "GrupoMotoristas", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas", NameType = typeof(GrupoMotoristas))]
    public class GrupoMotoristas : EntidadeBase
    {
        [Id(Name = "Codigo", Type = "int", Column = "GMO_CODIGO")]
        [Generator(Class = "native")]
        public virtual int Codigo { get; set; }

        [Property(Name = "CodigoIntegracao", Column = "GMO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [Property(Name = "Situacao", Column = "GMO_SITUACAO", TypeType = typeof(int), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristas Situacao { get; set; }

        [Property(Name = "Descricao", Column = "GMO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [Property(Name = "Observacoes", Column = "GMO_OBSERVACOES", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [Property(Name = "Ativo", Column = "GMO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

    }
}