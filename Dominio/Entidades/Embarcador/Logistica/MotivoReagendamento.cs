namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_REAGENDAMENTO", EntityName = "MotivoReagendamento", Name = "Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento", NameType = typeof(MotivoReagendamento))]
    public class MotivoReagendamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MRE_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MRE_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MRE_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarOnTime", Column = "MRE_CONSIDERAR_ON_TIME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarOnTime { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoResponsavelAtrasoEntrega", Column = "TRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega TipoResponsavelAtrasoEntrega { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo; }
        }
    }
}
