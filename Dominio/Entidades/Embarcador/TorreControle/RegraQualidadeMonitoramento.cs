namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_REGRA_QUALIDADE_MONITORAMENTO", EntityName = "RegraQualidadeMonitoramento", Name = "Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento", NameType = typeof(RegraQualidadeMonitoramento))]
    public class RegraQualidadeMonitoramento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RQM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RQM_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RQM_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRegraQualidadeMonitoramento", Column = "RQM_TIPO_REGRA_QUALIDADE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraQualidadeMonitoramento), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraQualidadeMonitoramento TipoRegraQualidadeMonitoramento { get; set; }
    }
}
