using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_EVENTO_TRATATIVA", EntityName = "MonitoramentoEventoTratativa", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa", NameType = typeof(MonitoramentoEventoTratativa))]
    public class MonitoramentoEventoTratativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MET_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Type = "System.Int32", Column = "MET_SEQUENCIA", NotNull = true)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEmMinutos", Type = "System.Int32", Column = "MET_TEMPO_MINUTOS", NotNull = true)]
        public virtual int TempoEmMinutos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaResponsavel", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel CategoriaResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioEmail", Column = "MET_ENVIO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioEmailTransportador", Column = "MET_ENVIO_EMAIL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioEmailTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioEmailCliente", Column = "MET_ENVIO_EMAIL_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioEmailCliente { get; set; }

        //foi criado para utilizar apenas no tipo Atraso na descarga para DPA (ESTA SETADO DIRETO NO BANCO POR ENQUANTO)
        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloEmailPadrao", Column = "MET_ENVIO_MODELO_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoModeloEmail), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoModeloEmail ModeloEmailPadrao { get; set; }

        public virtual String Descricao { get { return Codigo.ToString(); } }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoEvento", Column = "MEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento MonitoramentoEvento { get; set; }

    }
}
