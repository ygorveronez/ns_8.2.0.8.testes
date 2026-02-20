using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_PATIO_ALERTA_SLN", EntityName = "GestaoPatioAlertaSla", Name = "Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla", NameType = typeof(GestaoPatioAlertaSla))]
    public class GestaoPatioAlertaSla : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoFaltante", Column = "GAS_TEMPO_FALTANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoFaltante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoFaltanteTransportador", Column = "GAS_TEMPO_FALTANTE_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoFaltanteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoExcedido", Column = "GAS_TEMPO_EXCEDIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExcedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoExcedidoTransportador", Column = "GAS_TEMPO_EXCEDIDO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExcedidoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorAlertaTempoFaltante", Column = "GAS_COR_ALERTA_TEMPO_FALTANTE", TypeType = typeof(string), NotNull = false, Length = 15)]
        public virtual string CorAlertaTempoFaltante { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CorAlertaTempoExcedido", Column = "GAS_COR_ALERTA_TEMPO_EXCEDIDO", TypeType = typeof(string), NotNull = false, Length = 15)]
        public virtual string CorAlertaTempoExcedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeAlerta", Column = "GAS_NOME_ALERTA", TypeType = typeof(string), NotNull = true, Length = 50)]
        public virtual string NomeAlerta { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "GAS_EMAILS", TypeType = typeof(string), NotNull = false, Length = 300)]
        public virtual string Emails { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertarTransportadorPorEmail", Column = "GAS_ALERTAR_TRANSPORTADOR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarTransportadorPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Etapas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALERTA_SLN_ETAPA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "GAS_ETAPA", TypeType = typeof(EtapaFluxoGestaoPatio), NotNull = false)]
        public virtual ICollection<EtapaFluxoGestaoPatio> Etapas { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposAlertaEmail", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALERTA_SLN_TIPO_ALERTA_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "GAS_TIPO_ALERTA", TypeType = typeof(TipoAlertaSlnEmail), NotNull = false)]
        public virtual ICollection<TipoAlertaSlnEmail> TiposAlertaEmail { get; set; }
    }
}
