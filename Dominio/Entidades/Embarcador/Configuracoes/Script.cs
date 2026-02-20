namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SCRIPT", EntityName = "Script", Name = "Dominio.Entidades.Embarcador.Configuracoes.Script", NameType = typeof(Script))]
    public class Script : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SCR_DESCRICAO", TypeType = typeof(string), Length = 200,NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SCR_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ScriptSQL", Column = "SCR_SCRIPT_SQL", Type = "StringClob", NotNull = true)]
        public virtual string ScriptSQL { get; set; }

        public virtual bool Equals(Script other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
