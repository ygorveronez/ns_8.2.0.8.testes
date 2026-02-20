using System;

namespace Dominio.Entidades.Embarcador.Cargas.AlertaCarga
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_EVENTO", EntityName = "CargaEvento", Name = "Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento", NameType = typeof(CargaEvento))]
    public class CargaEvento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga), Column = "ALC_TIPO", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga TipoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEvento", Column = "ALC_DATA_EVENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "ALC_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus), Column = "ALC_STATUS", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALC_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaDescricao", Column = "ALC_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string AlertaDescricao { get; set; }

        public virtual string Descricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterDescricao(TipoAlerta); }
        }
    }
}
