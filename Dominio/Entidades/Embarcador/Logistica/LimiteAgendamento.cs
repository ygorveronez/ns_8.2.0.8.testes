namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_LIMITE_AGENDAMENTO", EntityName = "LimiteAgendamento", Name = "Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento", NameType = typeof(LimiteAgendamento))]
    public class LimiteAgendamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAD_LIMITE", TypeType = typeof(int), NotNull = true)]
        public virtual int Limite { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "LAD_PERMITE_ULTRAPASSAR_LIMITE_VOLUME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteUltrapassarLimiteVolume { get; set; }
        
        public virtual string Descricao
        {
            get
            { return $"{this.GrupoPessoa.Descricao} - {this.Limite} por dia"; }
        }
    }
}
