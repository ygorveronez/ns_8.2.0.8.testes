
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_MONITORAR_POSICOES", EntityName = "ConfiguracaoMonitoramentoMonitorarPosicoes", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoMonitorarPosicoes", NameType = typeof(ConfiguracaoMonitoramentoMonitorarPosicoes))]
    public class ConfiguracaoMonitoramentoMonitorarPosicoes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_LIMITE_REGISTROS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRegistros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_MINUTOS_CLIENTES_CACHE", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosClientesCache { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_HORAS_POSICOES_CACHE", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasPosicoesCache { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_HORAS_EXPIRAR_POSICOES", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasParaExpirarPosicoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_DISTANCIA_ALVOS_METROS", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaAlvoMetros { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_CONFIRMAR_VALIDADE_DA_POSICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfirmarValidadeDaPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_REGISTRAR_POSICAO_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegistrarPosicaoInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_EXCLUIR_POSICAO_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExcluirPosicaoInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_CLIENTES_SECUNDARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClientesSecundarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_GEOLOCALIZACAO_APENAS_JURIDICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeolocalizacaoApenasJuridico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_PROCESSAR_POSICOES_DEMAIS_PLACAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessarPosicoesDemaisPlacas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_ARQUIVO_NIVEL_LOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ArquivoNivelLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_ESPELHAR_POSICAO_REBOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EspelharPosicaoReboque { get; set; }
    }
}
