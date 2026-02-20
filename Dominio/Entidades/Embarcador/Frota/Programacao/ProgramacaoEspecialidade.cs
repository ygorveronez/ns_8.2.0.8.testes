namespace Dominio.Entidades.Embarcador.Frota.Programacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROGRAMACAO_ESPECIALIDADE", EntityName = "ProgramacaoEspecialidade", Name = "Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoEspecialidade", NameType = typeof(ProgramacaoEspecialidade))]
    public class ProgramacaoEspecialidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PES_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEntidadeProgramacao", Column = "PES_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao TipoEntidadeProgramacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PES_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string DescricaoTipoEntidadeProgramacao
        {
            get
            {
                switch (this.TipoEntidadeProgramacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Veiculo:
                        return "Veículo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Motorista:
                        return "Motorista";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
