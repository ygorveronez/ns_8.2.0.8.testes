namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_FREE_TIME", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoFreeTime", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFreeTime", NameType = typeof(ConfiguracaoTipoOperacaoFreeTime))]
    public class ConfiguracaoTipoOperacaoFreeTime : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TIPO_FREE_TIME", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreeTime), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFreeTime? TipoFreeTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoColetas", Column = "CFT_TEMPO_COLETAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoFronteiras", Column = "CFT_TEMPO_FRONTEIRAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoFronteiras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEntregas", Column = "CFT_TEMPO_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoTotalViagem", Column = "CFT_TEMPO_TOTAL_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoTotalViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarDatasDePrevisaoDoPedidoParaEstadia", Column = "CFT_CONSIDERAR_DATAS_PREVISAO_DO_PEDIDO_PARA_ESTADIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDatasDePrevisaoDoPedidoParaEstadia { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações de Free Time";
            }
        }
    }
}
