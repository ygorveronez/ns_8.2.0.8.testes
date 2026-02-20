using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Servicos.Embarcador.Monitoramento
{
    public class MonitoramentoDadosSumarizados : ServicoBase
    {
        #region Atributos
        private readonly string _OK = "OK";
        private readonly string _NOK = "NOK";
        private readonly string _NAN = "NAN";
        
        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega _repCargaEntrega;
        private readonly Repositorio.Embarcador.Logistica.Posicao _repPosicao;
        private readonly Repositorio.Embarcador.Logistica.PosicaoAlvo _repPosicaoAlvo;
        private readonly Repositorio.Embarcador.Logistica.PermanenciaCliente _repPermanenciaCliente;
        private readonly Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem _repMonitoramentoHistoricoStatusViagem;

        private Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosSumarizados _resultado;

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> _posicoes;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> _posicoesSumarizado;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> _posicoesAlvo;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> _posicoesAlvoSumarizado;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaCliente> _permanenciaClientes;
        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> _cargaEntregas;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> _historicos;
        #endregion

        #region Construtores
        public MonitoramentoDadosSumarizados(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {            
            _repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);
            _repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(_unitOfWork);
            _repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(_unitOfWork);
            _repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            _repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(_unitOfWork);
        }
        #endregion

        #region Métodos Públicos
        public void PrepararDadosMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            DateTime dataInicio = monitoramento.DataInicio.Value;
            DateTime dataFim = monitoramento.DataFim.Value;
            // TODO: ToList precisa fazer um cast
            _historicos = _repMonitoramentoHistoricoStatusViagem.BuscarObjetoDeValorPorMonitoramento(monitoramento.Codigo).ToList();

            if (_historicos.Count > 0)
            {
                DateTime dataInicioHistorico = _historicos.Min(status => status.DataInicio);
                if (dataInicio > dataInicioHistorico)
                    dataInicio = dataInicioHistorico;

                DateTime? dataFimHistorico = _historicos.Where(status => status.DataFim.HasValue).Select(status => (DateTime?)status.DataFim.Value).Max();

                if (dataFimHistorico.HasValue && dataFim < dataFimHistorico.Value)
                    dataFim = dataFimHistorico.Value;
            }

            _posicoes = _repPosicao.BuscarObjetoDeValorPorMonitoramento(monitoramento.Codigo, monitoramento.Veiculo?.Codigo, dataInicio, dataFim).ToList();

            _posicoesAlvo = _repPosicaoAlvo.BuscarObjetoDeValorPorPosicoes(_posicoes.Select(pos => pos.ID).ToList());
            // TODO: ToList precisa fazer um cast
            _cargaEntregas = _repCargaEntrega.BuscarObjetoDeValorPorCarga(monitoramento.Carga?.Codigo ?? 0).ToList();
            
            _permanenciaClientes = _repPermanenciaCliente.BuscarObjetoDeValorPorCarga(monitoramento.Carga?.Codigo ?? 0).ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosSumarizados SumarizarPorRegra(TipoRegraQualidadeMonitoramento regra)
        {
            _resultado = new Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosSumarizados()
            {
                Resultado = _NAN,
                Posicoes = null
            };

            switch (regra)
            {
                case TipoRegraQualidadeMonitoramento.PreEmbarque:
                    SumarizarPreEmbarque();
                    break;
                case TipoRegraQualidadeMonitoramento.DeslocamentoParaOrigem:
                    SumarizarDeslocamentoParaOrigem();
                    break;
                case TipoRegraQualidadeMonitoramento.PreCheckin:
                    SumarizarPreCheckin();
                    break;
                case TipoRegraQualidadeMonitoramento.EmCarregamento:
                    SumarizarEmCarregamento();
                    break;
                case TipoRegraQualidadeMonitoramento.SaidaOrigem:
                    SumarizarSaidaOrigem();
                    break;
                case TipoRegraQualidadeMonitoramento.EmViagem:
                    SumarizarEmViagem();
                    break;
                case TipoRegraQualidadeMonitoramento.ChegadaDestino:
                    SumarizarChegadaDestino();
                    break;
                case TipoRegraQualidadeMonitoramento.Descarga:
                    SumarizarDescarga();
                    break;
                case TipoRegraQualidadeMonitoramento.SaidaDestino:
                    SumarizarSaidaDestino();
                    break;
                default:
                    break;
            }
            return _resultado;
        }
        #endregion

        #region Métodos Privados
        /// <summary>
        /// Pré Embarque – Deve ser considerado como OK e NOK conforme regra: 
        ///     OK – Cargas que tiveram pelo menos 1 posição 4 horas antes de adentrar ao raio e/ou polígono de carregamento;
        ///     NOK - Cargas que não tiveram nenhuma posição 4 horas antes de adentrar ao raio e/ou polígono de carregamento;
        /// Validar em cima dos status viagem(DeslocamentoParaPlanta)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarPreEmbarque()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoPreEmbarque();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega coleta = _cargaEntregas.Find(ce => ce.Coleta);
            if (coleta == null || !coleta.DataEntradaRaio.HasValue) return;

            _posicoesSumarizado = _posicoes.Where(pos => pos.DataVeiculo < coleta.DataEntradaRaio.Value).ToList();
            bool existePosicao = _posicoesSumarizado.Count > 0;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesSumarizado);
        }

        /// <summary>
        /// Deslocamento para Origem – Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 3 posições antes de adentrar ao raio e/ou polígono de carregamento;
        ///     NOK - Cargas que tiveram menos de 3 posições antes de adentrar ao raio e/ou polígono de carregamento;
        /// Validar em cima dos status viagem(DeslocamentoParaPlanta)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarDeslocamentoParaOrigem()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoDeslocamentoParaOrigem();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega coleta = _cargaEntregas.Find(ce => ce.Coleta);
            if (coleta == null || !coleta.DataEntradaRaio.HasValue) return;

            _posicoesSumarizado = _posicoes.Where(pos => pos.DataVeiculo < coleta.DataEntradaRaio.Value).ToList();
            bool existePosicao = _posicoesSumarizado.Count >= 3;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesSumarizado);
        }

        /// <summary>
        /// Pré Check In - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 1 posição no raio e/ou polígono de carregamento pai;
        ///     NOK - Cargas que tiveram não tiveram nenhuma posição antes no raio e/ou polígono de carregamento pai; 
        /// Validar em cima dos status viagem(DeslocamentoParaPlanta , AguardandoHorarioCarregamento, AguardandoCarregamento ou EmCarregamento)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarPreCheckin()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoPreCheckIn();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega coleta = _cargaEntregas.Find(ce => ce.Coleta);
            if (coleta == null || !coleta.DataEntradaRaio.HasValue) return;

            DateTime dataInicio = statusViagem.Min(status => status.DataInicio);
            DateTime dataFim = statusViagem.Max(status => status.DataFim.Value);
            List<long> codigoPosicoes = _posicoes.Where(pos => pos.DataVeiculo >= dataInicio && pos.DataVeiculo <= dataFim).Select(pos => pos.ID).ToList();

            _posicoesAlvoSumarizado = _posicoesAlvo.Where(pAlvo => codigoPosicoes.Contains(pAlvo.CodigoPosicao)).ToList();

            bool existePosicao = _posicoesAlvoSumarizado.Exists(pos => pos.CodigoCliente == (coleta.CodigoCliente ?? 0d));

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesAlvoSumarizado.Select(pos => new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao() { ID = pos.CodigoPosicao }).ToList());
        }

        /// <summary>
        /// Em Carregamento - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 3 posições no raio e/ou polígono de carregamento pai; 
        ///     NOK - Cargas que tiveram menos de 3 posições dentro do raio e/ou polígono de carregamento;
        /// Validar em cima dos status viagem(AguardandoHorarioCarregamento, AguardandoCarregamento ou EmCarregamento)
        /// Posições dentro das Coletas (validar pelo Permanência cliente)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarEmCarregamento()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoEmCarregamento();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega coleta = _cargaEntregas.Find(ce => ce.Coleta);
            if (coleta == null || !coleta.DataEntradaRaio.HasValue) return;

            DateTime dataInicio = statusViagem.Min(status => status.DataInicio);
            DateTime dataFim = statusViagem.Max(status => status.DataFim.Value);
            List<long> codigoPosicoes = _posicoes.Where(pos => pos.DataVeiculo >= dataInicio && pos.DataVeiculo <= dataFim).Select(pos => pos.ID).ToList();

            _posicoesAlvoSumarizado = _posicoesAlvo.Where(pAlvo => codigoPosicoes.Contains(pAlvo.CodigoPosicao) && pAlvo.CodigoCliente == (coleta.CodigoCliente ?? 0d)).ToList();

            bool existePosicao = _posicoesAlvoSumarizado.Count >= 3;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesAlvoSumarizado.Select(pos => new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao() { ID = pos.CodigoPosicao }).ToList());
        }

        /// <summary>
        /// Saída da Origem - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 1 posição fora no raio e/ou polígono de carregamento pai; 
        ///     NOK - Cargas que tiveram não tiveram nenhuma posição antes no raio e/ou polígono de carregamento pai;
        /// Validar em cima do status viagem(Transito, se tem uma posição no status de transito OK)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarSaidaOrigem()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoSaidaDaOrigem();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            DateTime dataInicio = statusViagem.Min(status => status.DataInicio);
            DateTime dataFim = statusViagem.Max(status => status.DataFim.Value);
            _posicoesSumarizado = _posicoes.Where(pos => pos.DataVeiculo >= dataInicio && pos.DataVeiculo <= dataFim).ToList();
            bool existePosicao = _posicoesSumarizado.Count > 0;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesSumarizado);
        }

        /// <summary>
        /// Em viagem - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que possuem pelo menos 10 posições fora do local de origem, ou seja, cargas com status em trânsito;
        ///     NOK - Cargas que tiveram menos de 10 posições antes de adentrar ao raio e/ou polígono de descarga;
        /// Validar em cima do status viagem(Transito, se tem 10 posições no status de transito OK)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarEmViagem()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoEmViagem();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            DateTime dataInicio = statusViagem.Min(status => status.DataInicio);
            DateTime dataFim = statusViagem.Max(status => status.DataFim.Value);
            _posicoesSumarizado = _posicoes.Where(pos => pos.DataVeiculo >= dataInicio && pos.DataVeiculo <= dataFim).ToList();
            bool existePosicao = _posicoesSumarizado.Count >= 10;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesSumarizado);
        }

        /// <summary>
        /// Chegada Destino - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 1 posição no raio e/ou polígono de descarga pai; 
        ///     NOK - Cargas que tiveram não tiveram nenhuma posição antes no raio e/ou polígono de descarga pai; 
        /// Validar em cima dos status viagem(AguardandoHorarioDescarga, AguardandoDescarga, Descarga)
        /// Posições dentro das ENTREGAS(validar pelo Posicao_Alvo, se tem pelo menos uma posicao_alvo para o cnpj da entrega, validar todas as entregas)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarChegadaDestino()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoChegadaDestino();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entrega = _cargaEntregas.Find(ce => !ce.Coleta);
            if (entrega == null || !entrega.DataEntradaRaio.HasValue) return;

            DateTime dataInicio = statusViagem.Min(status => status.DataInicio);
            DateTime dataFim = statusViagem.Max(status => status.DataFim.Value);
            List<long> codigoPosicoes = _posicoes.Where(pos => pos.DataVeiculo >= dataInicio && pos.DataVeiculo <= dataFim).Select(pos => pos.ID).ToList();

            _posicoesAlvoSumarizado = _posicoesAlvo.Where(pAlvo => codigoPosicoes.Contains(pAlvo.CodigoPosicao) && pAlvo.CodigoCliente == (entrega.CodigoCliente ?? 0d)).ToList();
            bool existePosicao = _posicoesAlvoSumarizado.Count > 0;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesAlvoSumarizado.Select(pos => new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao() { ID = pos.CodigoPosicao }).ToList());
        }

        /// <summary>
        /// Descarga - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 3 posições no raio e/ou polígono de descarga pai; 
        ///     NOK - Cargas que tiveram menos de 3 posições dentro do raio e/ou polígono de descarga;
        /// Validar em cima dos status viagem(AguardandoHorarioDescarga, AguardandoDescarga, Descarga)
        /// Posições dentro das ENTREGAS(validar pelo Posicao_Alvo, se tem pelo menos 3 posicao_alvo para o cnpj da entrega, validar todas as entregas)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarDescarga()
        {
            List<MonitoramentoStatusViagemTipoRegra> listaStatusViagem = MonitoramentoStatusViagemTipoRegraHelper.ObterStatusSumarizacaoDescarga();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> statusViagem = _historicos.Where(hist => listaStatusViagem.Contains(hist.TipoRegra) && hist.DataFim.HasValue).ToList();
            if (statusViagem.Count == 0) return;

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega entrega = _cargaEntregas.Find(ce => !ce.Coleta);
            if (entrega == null || !entrega.DataEntradaRaio.HasValue) return;

            DateTime dataInicio = statusViagem.Min(status => status.DataInicio);
            DateTime dataFim = statusViagem.Max(status => status.DataFim.Value);
            List<long> codigoPosicoes = _posicoes.Where(pos => pos.DataVeiculo >= dataInicio && pos.DataVeiculo <= dataFim).Select(pos => pos.ID).ToList();

            _posicoesAlvoSumarizado = _posicoesAlvo.Where(pAlvo => codigoPosicoes.Contains(pAlvo.CodigoPosicao) && pAlvo.CodigoCliente == (entrega.CodigoCliente ?? 0d)).ToList();
            bool existePosicao = _posicoesAlvoSumarizado.Count >= 3;

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesAlvoSumarizado.Select(pos => new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao() { ID = pos.CodigoPosicao }).ToList());
        }

        /// <summary>
        /// Saída Destino - Deve ser considerado como OK e NOK conforme regra: 
        ///     Ok – Cargas que tiveram pelo menos 1 posição fora no raio e/ou polígono de carregamento pai; 
        ///     NOK - Cargas que tiveram não tiveram nenhuma posição antes no raio e/ou polígono de carregamento pai; 
        /// Validar em cima da permanência cliente ou saída raio.., se todos finalizados ok(validar como esta isso no banco)
        /// </summary>
        /// <returns> MonitoramentoDadosSumarizados = {String: OK ou NOK, Lista: Posicoes Utilizadas na Sumarização} </returns>
        private void SumarizarSaidaDestino()
        {
            List<DateTime> datasFimPermanencias = _permanenciaClientes.Where(p => p.DataFim.HasValue).Select(p => p.DataFim.Value).ToList();
            _posicoesSumarizado = _posicoes.Where(pos => datasFimPermanencias.Contains(pos.DataVeiculo)).ToList();

            bool existePosicao = _permanenciaClientes.TrueForAll(permanencia => permanencia.DataFim.HasValue);

            MontarRetornoSumarizacao(existePosicao ? _OK : _NOK, _posicoesSumarizado);
        }

        private void MontarRetornoSumarizacao(string resultado, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            _resultado = new Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosSumarizados()
            {
                Resultado = resultado,
                Posicoes = posicoes?.DistinctBy(pos => pos.ID).ToList(),
            };
        }
        #endregion
    }
}