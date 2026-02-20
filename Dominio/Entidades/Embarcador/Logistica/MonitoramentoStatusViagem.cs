using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_STATUS_VIAGEM", EntityName = "MonitoramentoStatusViagem", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem", NameType = typeof(MonitoramentoStatusViagem))]
    public class MonitoramentoStatusViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MSV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MSV_DESCRICAO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "MSV_SIGLA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "MSV_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRegra", Column = "MSV_TIPO_REGRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra TipoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "MSV_COR", TypeType = typeof(string), NotNull = true)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MSV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "MSV_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoGrupoStatusViagem", Column = "MGV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem Grupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarStatusCargaAoTrocarStatusViagem", Column = "MGV_VALIDAR_STATUS_CARGA_AO_TROCAR_STATUS_DA_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarStatusCargaAoTrocarStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusCargaAoTrocarStatusViagem", Column = "MGV_STATUS_CARGA_AO_TROCAR_STATUS_DA_VIAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento StatusCargaAoTrocarStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa", Column = "MSV_NAO_UTILIZAR_STATUS_PARA_CALCULO_TEMPERATURA_DENTRO_FAIXA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa { get; set; }

        public virtual string DescricaoAtivo { get { return (this.Ativo) ? "Ativo" : "Inativo"; } }

    }
}
