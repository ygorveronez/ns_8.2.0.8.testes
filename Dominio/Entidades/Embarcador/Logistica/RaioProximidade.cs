namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RAIO_PROXIMIDADE", EntityName = "RaioProximidade", Name = "Dominio.Entidades.Embarcador.Logistica.RaioProximidade", NameType = typeof(RaioProximidade))]
    public class RaioProximidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Raio", Column = "RAP_RAIO", TypeType = typeof(int), NotNull = true)]
        public virtual int Raio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificacao", Column = "RAP_IDENTIFICACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Identificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "RAP_COR", TypeType = typeof(string), NotNull = false)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaAutomaticoPorPermanencia", Column = "RAP_GERAR_ALERTA_AUTOMATICO_POR_PERMANENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarAlertaAutomaticoPorPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "RAP_TEMPO", TypeType = typeof(int), NotNull = false)]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta), Column = "ALE_TIPO", NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Local", Class = "Locais", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Locais Local { get; set; }

        public virtual string Descricao { get { return Identificacao; } }
    }
}
