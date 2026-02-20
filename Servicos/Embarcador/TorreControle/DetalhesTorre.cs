using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.TorreControle
{
    public class DetalhesTorre
    {
        #region Atributos Privados

        private CancellationToken _cancellationToken;
        private readonly Repositorio.UnitOfWork _unitOfWork;


        #endregion Atributos Privados

        #region Construtores
        public DetalhesTorre(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region Atributos protegidos

        protected const string MASCARA_DATA_HM = "dd/MM/yyyy HH:mm";
        protected const string VALOR_NULO = "-";

        #endregion

        #region Métodos públicos

        public async Task<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesTorre> ObterDadosPesquisaAsync(int codigo)
        {
            NumberFormatInfo nfid = new NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalSeparator = ",", NumberDecimalDigits = 1 };

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repositorioMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repositorioMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotoristas = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = await repositorioMonitoramento.BuscarUltimoPorCargaAsync(codigo);

            if (monitoramento == null)
                throw new ServicoException(Localization.Resources.Logistica.Monitoramento.MonitoramentoInexistente);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesMonitoramento = await repositorioPosicao.BuscarObjetoDeValorPorMonitoramentoAsync(monitoramento.Codigo, monitoramento.Veiculo.Codigo, null, null);
            List<CargaEntrega> listaCargaEntrega = await repositorioCargaEntrega.ConsultarParadasPorCargaAsync(monitoramento.Carga.Codigo, null);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicos = await repositorioMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramentoAsync(monitoramento);
            List<DetalhesTorreParadas> listaCargaEntregaRetornar = (from cargaEntrega in listaCargaEntrega select ObterDetalhesGridParadas(cargaEntrega)).ToList();
            List<DetalhesTorreHistoricoStatus> historicoStatusViagem = MapearHistoricoStatusViagem(historicos);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> statusViagem = await repositorioMonitoramentoStatusViagem.BuscarAtivosAsync();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristas = await repositorioCargaMotoristas.BuscarPorCargaAsync(monitoramento.Carga.Codigo);

            DetalhesTorreStatus statusAtual = new DetalhesTorreStatus();
            List<DetalhesTorreStatus> statusViagemRetorno = new List<DetalhesTorreStatus>();

            if (monitoramento.StatusViagem != null)
            {
                statusAtual = new DetalhesTorreStatus
                {
                    Codigo = monitoramento.StatusViagem.Codigo,
                    Descricao = monitoramento.StatusViagem.Descricao
                };

                statusViagemRetorno = statusViagem
                    .Select(statusViagem => new DetalhesTorreStatus
                    {
                        Codigo = statusViagem.Codigo,
                        Descricao = statusViagem.Descricao
                    })
                    .ToList();
            }

            Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesTorre detalhesTorre = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesTorre
            {
                Codigo = monitoramento.Codigo,
                MotoristaNome = motoristas.Count > 0 ? String.Join(", ", motoristas.Select(m => m.Motorista?.Nome ?? VALOR_NULO)) : VALOR_NULO,
                MotoristaCPF = motoristas.Count > 0 ? String.Join(", ", motoristas.Select(m => m.Motorista?.CPF_CNPJ_Formatado_Com_Asterisco ?? VALOR_NULO)) : VALOR_NULO,
                MotoristaTelefone = motoristas.Count > 0 ? String.Join(", ", motoristas.Select(m => m.Motorista?.Telefone ?? VALOR_NULO)) : VALOR_NULO,
                InicioMonitoramento = monitoramento.DataInicio?.ToString(MASCARA_DATA_HM) ?? VALOR_NULO,
                FimMonitoramento = monitoramento.DataFim?.ToString(MASCARA_DATA_HM) ?? VALOR_NULO,
                Veiculo = monitoramento.Veiculo?.Placa ?? VALOR_NULO,
                Tecnologia = monitoramento.Veiculo?.TecnologiaRastreador?.Descricao ?? VALOR_NULO,
                RastreadorStatus = monitoramento.UltimaPosicao?.DataVeiculo == null ? 1 : monitoramento.UltimaPosicao.DataVeiculo == DateTime.MinValue ? 1 : (DateTime.Now - monitoramento.UltimaPosicao.DataVeiculo).TotalMinutes <= (configuracao?.TempoSemPosicaoParaVeiculoPerderSinal ?? 0) ? 3 : 4,
                PrimeiraPosicao = posicoesMonitoramento != null && posicoesMonitoramento.Count > 0 ? posicoesMonitoramento.First().DataVeiculo.ToString(MASCARA_DATA_HM) : null,
                Velocidade = (monitoramento.UltimaPosicao != null) ? monitoramento.UltimaPosicao.Velocidade.ToString() + " Km/h" : VALOR_NULO,
                Localizacao = monitoramento.UltimaPosicao?.Descricao ?? VALOR_NULO,
                Cidade = VALOR_NULO,
                DataPosicao = monitoramento.UltimaPosicao?.DataVeiculo.ToString(MASCARA_DATA_HM) ?? null,
                Temperatura = (monitoramento.UltimaPosicao != null && monitoramento.UltimaPosicao?.Temperatura != null && monitoramento.UltimaPosicao?.Temperatura != 0) ? monitoramento.UltimaPosicao?.Temperatura.Value.ToString(nfid) + "º" : VALOR_NULO,
                Ignicao = (monitoramento.UltimaPosicao != null && monitoramento.UltimaPosicao.Ignicao > 0),
                Status = statusViagemRetorno,
                StatusAtual = statusAtual,
                DistanciaRealizada = String.Format("{0:n1} Km", monitoramento.DistanciaRealizada),
                DistanciaPrevista = monitoramento.DistanciaPrevista != null ? String.Format("{0:n1} Km", monitoramento.DistanciaPrevista) : VALOR_NULO,
                PrevisaoChegada = monitoramento.Carga.DataFimViagemReprogramada != null ? monitoramento.Carga.DataFimViagemReprogramada?.ToString(MASCARA_DATA_HM) : monitoramento.Carga.DataFimViagemPrevista?.ToString(MASCARA_DATA_HM) ?? VALOR_NULO,
                DistanciaDestino = monitoramento.DistanciaAteDestino != null ? monitoramento.DistanciaAteDestino.ToString() : VALOR_NULO,
                MonitoramentoCritico = monitoramento.Critico,
                Observacao = monitoramento.Observacao ?? string.Empty,

                Paradas = listaCargaEntregaRetornar,

                Velocidades = posicoesMonitoramento?.Where(p => p != null).Select(p => new DetalhesTorreVelocidade { Valor = p.Velocidade, Data = p.DataVeiculo.ToString(MASCARA_DATA_HM) }).ToList() ?? new List<DetalhesTorreVelocidade>(),
                Temperaturas = posicoesMonitoramento?.Where(p => p != null).Select(p => new DetalhesTorreTemperatura { Valor = p.Temperatura, Data = p.DataVeiculo.ToString(MASCARA_DATA_HM) }).ToList() ?? new List<DetalhesTorreTemperatura>(),

                Historico = historicoStatusViagem
            };

            return detalhesTorre;
        }

        public async Task AtualizarCamposDetalhesTorre(Dominio.ObjetosDeValor.Embarcador.TorreControle.DadosAtualizarDetalhesTorre dadosAtualizarDetalhesTorre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                await _unitOfWork.StartAsync();

                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repositorioMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork, _cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork, _cancellationToken).BuscarPrimeiroRegistroAsync();

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = await repositorioMonitoramento.BuscarPorCodigoAsync(dadosAtualizarDetalhesTorre.Codigo, true);
                if (monitoramento == null) throw new ServicoException(Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado) throw new ServicoException(Localization.Resources.Logistica.Monitoramento.MonitoramentoEncerrado);


                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = await repositorioMonitoramentoStatusViagem.BuscarPorCodigoAsync(dadosAtualizarDetalhesTorre.CodigoStatusViagem, false);
                if (dadosAtualizarDetalhesTorre.DataInicioStatus > DateTime.Now) throw new ServicoException(Localization.Resources.Logistica.Monitoramento.DataInicialStatusNaoPodeSerMaiorDataHoraAtuais);

                monitoramento.Initialize();
                monitoramento.Critico = dadosAtualizarDetalhesTorre.Critico;
                monitoramento.Observacao = dadosAtualizarDetalhesTorre.Observacao;

                if (dadosAtualizarDetalhesTorre.DataInicioStatus != DateTime.MinValue)
                    Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.AlterarStatusViagem(_unitOfWork, monitoramento, statusViagem, dadosAtualizarDetalhesTorre.DataInicioStatus, auditado, configuracao, tipoServicoMultisoftware, clienteMultisoftware, configuracaoControleEntrega);

                await repositorioMonitoramento.AtualizarAsync(monitoramento);
                await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, monitoramento, monitoramento.GetChanges(), Localization.Resources.Logistica.Monitoramento.MonitoramentoAtualizadoModalTorre, _unitOfWork);

                await _unitOfWork.CommitChangesAsync();
            }
            catch (Exception excecao)
            {
                await _unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                throw new ServicoException(Localization.Resources.Logistica.Monitoramento.FalhaAoAtualizar);
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos privados
        private DetalhesTorreParadas ObterDetalhesGridParadas(CargaEntrega cargaEntrega)
        {
            DetalhesTorreParadas objCargaEntrega = new DetalhesTorreParadas
            {
                Cliente = cargaEntrega.Cliente?.Nome,
                Cidade = cargaEntrega.Cliente?.Localidade.DescricaoCidadeEstado,
                Tipo = cargaEntrega.Coleta ? "C" : "E",
                Situacao = cargaEntrega.DescricaoSituacao,
                PesoKg = cargaEntrega.Pedidos.Sum(p => p.CargaPedido.Pedido.PesoTotal).ToString("n0"),
                ValorTotalProdutos = cargaEntrega.Pedidos.Sum(p => p.CargaPedido.Produtos.Sum(o => o.ValorTotal)).ToString("n2"),
                SequenciaPlanejada = cargaEntrega.Ordem.ToString(),
                SequenciaExecutada = cargaEntrega.OrdemRealizada.ToString(),
                PrevisaoChegada = cargaEntrega.DataPrevista?.ToDateTimeString() ?? string.Empty,
                PrevisaoChegadaReprogramada = cargaEntrega.DataReprogramada?.ToDateTimeString() ?? string.Empty,
                EntradaRaio = cargaEntrega.DataEntradaRaio?.ToDateTimeString() ?? string.Empty,
                InicioEntrega = cargaEntrega.DataInicio?.ToDateTimeString() ?? string.Empty,
                FimEntrega = cargaEntrega.DataFim?.ToDateTimeString() ?? string.Empty,
                SaidaRaio = cargaEntrega.DataSaidaRaio?.ToDateTimeString() ?? string.Empty,
                Endereco = cargaEntrega.Cliente?.EnderecoCompleto,
                Entrega = cargaEntrega.DescricaoEntregaNoPrazo
            };

            return objCargaEntrega;
        }
        private List<DetalhesTorreHistoricoStatus> MapearHistoricoStatusViagem(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicos)
        {
            List<DetalhesTorreHistoricoStatus> historicoRows = new List<DetalhesTorreHistoricoStatus>();
            if (historicos != null && historicos.Count > 0)
            {
                historicoRows = (
                    from row in historicos
                    select new DetalhesTorreHistoricoStatus
                    {
                        Status = row.StatusViagem?.Descricao,
                        Veiculo = row.Veiculo?.Placa,
                        DataInicio = row.DataInicio.ToString(MASCARA_DATA_HM),
                        DataFim = row.DataFim?.ToString(MASCARA_DATA_HM) ?? "",
                        Tempo = (row.TempoSegundos != null) ? row.Tempo.ToString() : ""
                    }).ToList();
            }
            return historicoRows;
        }
    }

    #endregion
}
