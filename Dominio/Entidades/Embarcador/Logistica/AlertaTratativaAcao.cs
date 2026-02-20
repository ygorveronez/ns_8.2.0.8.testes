namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_ALERTA_TRATATIVA_ACAO", EntityName = "AlertaTratativaAcao", Name = "Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao", NameType = typeof(AlertaTratativaAcao))]
    public class AlertaTratativaAcao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ATC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcaoMonitorada", Column = "ATC_ACAO_MONITORADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcaoMonitorada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ATC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; } = true;

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLocal), Column = "ATC_TIPO_ALERTA", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

    }
}
