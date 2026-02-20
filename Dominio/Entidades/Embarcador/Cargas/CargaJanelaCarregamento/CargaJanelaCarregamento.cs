using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO", EntityName = "CargaJanelaCarregamento", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento", NameType = typeof(CargaJanelaCarregamento))]
    public class CargaJanelaCarregamento : EntidadeCargaBase, IEntidade
    {
        public CargaJanelaCarregamento() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CJC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "CJC_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO_AGRUPADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento CargaJanelaCarregamentoAgrupador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_TRANSPORTADOR_EXCLUSIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa TransportadorExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_TRANSPORTADOR_COTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa TransportadorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_TRANSPORTADOR_ORIGINAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa TransportadorOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "CJC_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volume", Column = "CJC_VOLUME", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Volume { get; set; }

        [Obsolete("Não utilizar. Será removida. Utilizar Carga.Rota")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_ENCAIXE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoEncaixe { get; set; }

        /// <summary>
        /// É a data que inicialmente a carga deveria ser carregada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoProgramada", Column = "CJC_DATA_CARREGAMENTO_PROGRAMADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCarregamentoProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReservada", Column = "CJC_DATA_RESERVADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReservada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioCarregamento", Column = "CJC_INICIO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime InicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteInicioCarregamento", Column = "CJC_LIMITE_INICIO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? LimiteInicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TerminoCarregamento", Column = "CJC_TERMINO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime TerminoCarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_DATA_PREVISAO_CHEGADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegada { get; set; }

        /// <summary>
        /// Tempo em minutos que leva para a Carga ser carregada, tem impacto direto na visualização na Janela de Carregamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoCarregamento", Column = "CJC_TEMPO_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeAdicionalVagasOcupadas", Column = "CJC_QUANTIDADE_ADICIONAL_VAGAS_OCUPADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAdicionalVagasOcupadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberaradaParaFaturamentePeloTransportador", Column = "CJC_LIBERADA_PARA_FATURAMENTO_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaradaParaFaturamentePeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_HORARIO_ENCAIXADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioEncaixado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProximaSituacao", Column = "CJC_DATA_PROXIMA_SITUACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataProximaSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSituacaoAtual", Column = "CJC_DATA_SITUACAO_ATUAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSituacaoAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CJC_TIPO", TypeType = typeof(TipoCargaJanelaCarregamento), NotNull = true)]
        public virtual TipoCargaJanelaCarregamento Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CJC_SITUACAO", TypeType = typeof(SituacaoCargaJanelaCarregamento), NotNull = true)]
        public virtual SituacaoCargaJanelaCarregamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCotacao", Column = "CJC_SITUACAO_COTACAO", TypeType = typeof(SituacaoCargaJanelaCarregamentoCotacao), NotNull = false)]
        public virtual SituacaoCargaJanelaCarregamentoCotacao SituacaoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Excedente", Column = "CJC_EXCEDENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Excedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComparecido", Column = "CJC_NAO_COMPARECIDO", TypeType = typeof(TipoNaoComparecimento), NotNull = false)]
        public virtual TipoNaoComparecimento NaoComparecido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarregamentoReservado", Column = "CJC_CARREGAMENTO_RESERVADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CarregamentoReservado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportador", Column = "CJC_OBSERVACAO_TRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportadorInformadaManualmente", Column = "CJC_OBSERVACAO_TRANSPORTADOR_INFORMADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObservacaoTransportadorInformadaManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFluxoPatio", Column = "CJC_OBSERVACAO_FLUXO_PATIO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAtraso", Formula = @"DATEDIFF((DAY), CJC_DATA_CARREGAMENTO_PROGRAMADA, CJC_INICIO_CARREGAMENTO)", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAtraso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_CARGA_LIBERADA_FILA_CARREGAMENTO_MANUALMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CargaLiberadaFilaCarregamentoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_CARGA_BLOQUEADA_FILA_CARREGAMENTO_MANUALMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CargaBloqueadaFilaCarregamentoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_CARGA_LIBERADA_COTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaLiberadaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_CARGA_LIBERADA_COTACAO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaLiberadaCotacaoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PROCESSO_COTACAO_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessoCotacaoFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_CARGA_COTACAO_GANHA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaCotacaoGanhaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_SHARE_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ShareLiberado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoRotaFrete", Column = "CRF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoShare", Column = "CJC_DATA_LIBERACAO_SHARE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoShare { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTerminoCotacao", Column = "CJC_DATA_TERMINO_COTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTerminoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaida", Column = "CJC_DATA_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDisponibilizacaoTransportadores", Column = "CJC_DATA_DISPONIBILIZACAO_TRANSPORTADORES", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDisponibilizacaoTransportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rodada", Column = "CJC_NUMERO_RODADA", TypeType = typeof(int), NotNull = false)]
        public virtual int Rodada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DatasAgendadasDivergentes", Column = "CJC_DATAS_AGENDADAS_DIVERGENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DatasAgendadasDivergentes { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_QUANTIDADE_ALTERACOES_MANUAIS_HORARIO_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAlteracoesManuaisHorarioCarregamento { get; set; }

        /// <summary>
        /// Serve para indicar quando a carga teve um cancelamento de horário de Carregamento, se estiver como true quer dizer que em algum momento já ocorreu o cancelamento de algum horário.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_HORARIO_DESENCAIXADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioDesencaixado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAtrasoCarregamento", Column = "MAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento MotivoAtrasoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecomendacaoGR", Column = "CJC_RECOMENDACAO_GR", TypeType = typeof(RecomendacaoGR), NotNull = false)]
        public virtual RecomendacaoGR? RecomendacaoGR { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoChecklist", Column = "CHE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist CargaJanelaCarregamentoChecklist { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (this.CentroCarregamento != null && this.CentroDescarregamento != null)
                    return this.CentroCarregamento.Descricao + " - " + this.CentroDescarregamento.Descricao;
                else
                    return this.Carga?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            { return Situacao.ObterDescricao(); }
        }

        public virtual bool IsCargaDadosPermiteVincularFilaCarregamentoAutomaticamente()
        {
            return (
                !CargaBloqueadaFilaCarregamentoManualmente &&
                (
                    CargaLiberadaFilaCarregamentoManualmente ||
                    (
                        !(CentroCarregamento?.VincularMotoristaFilaCarregamentoManualmente ?? false) &&
                        !(CargaBase?.TipoOperacao?.VincularMotoristaFilaCarregamentoManualmente ?? false) &&
                        !(CargaBase?.Rota?.VincularMotoristaFilaCarregamentoManualmente ?? false) &&
                        (!LimiteInicioCarregamento.HasValue || (LimiteInicioCarregamento.Value > DateTime.Now)) &&
                        !(CargaBase?.IsPossuiRestricaoFilaCarregamentoPorDestinatario() ?? false) &&
                        !(CargaBase?.IsPossuiRestricaoFilaCarregamentoPorRemetente() ?? false)
                    )
                )
            );
        }

        public virtual bool IsSituacaoPermiteVincularFilaCarregamento()
        {
            return (Situacao == SituacaoCargaJanelaCarregamento.SemTransportador) && (Carga.SituacaoCarga == SituacaoCarga.AgTransportador);
        }

        public virtual bool IsTempoCarregamentoValido()
        {
            return TempoCarregamento > 0 || (CentroCarregamento != null && CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas);
        }
    }
}
