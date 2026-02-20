namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_MOTIVO_DEFEITO", EntityName = "MotivoDefeito", Name = "Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito", NameType = typeof(MotivoDefeito))]
    public class MotivoDefeito: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BMD_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "BMD_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return string.Empty;
                }
            }
        }

    }
}
