namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_EVENTO_CAUSA", EntityName = "MonitoramentoEventoCausa", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa", NameType = typeof(MonitoramentoEventoCausa))]
    public class MonitoramentoEventoCausa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", TypeType = typeof(string), Column = "MEC_DESCRICAO", NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MEC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta), Column = "MEC_TIPO_ALERTA", NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
