using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.ProcessadorTarefas.Etapas
{
    public class GerarCarregamentoComRedespacho : EtapaState
    {
        private readonly ITenantService _tenantService;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5, 5);

        public GerarCarregamentoComRedespacho(ITenantService tenantService, IRequestSubtarefaRepository repositorioSubtarefa, IProcessamentoTarefaRepository repositorioTarefa)
        {
            _tenantService = tenantService;
            _repositorioSubtarefa = repositorioSubtarefa;
            _repositorioTarefa = repositorioTarefa;
        }

        public override async Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa
                .ObterPendentesPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

            if (!subtarefas.Any())
                return;

            var resultadosDoc = contexto.Tarefa.Resultado ?? new BsonDocument();
            var cargasArray = resultadosDoc.Contains("cargas")
                ? resultadosDoc["cargas"].AsBsonArray
                : new BsonArray();

            var resultadosConcurrent = new ConcurrentBag<BsonDocument>();

            var tasks = subtarefas.Select(async subtarefa =>
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    await ProcessarSubtarefaAsync(subtarefa, resultadosConcurrent, cargasArray, cancellationToken);
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            foreach (var resultado in resultadosConcurrent)
            {
                cargasArray.Add(resultado);
            }

            resultadosDoc["cargas"] = cargasArray;
            var update = Builders<ProcessamentoTarefa>.Update
                .Set(t => t.Resultado, resultadosDoc);
            await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);

            var todasSubtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);
            var subtarefasPendentes = todasSubtarefas.Where(s => s.Status == StatusTarefa.Aguardando || s.Status == StatusTarefa.EmProcessamento).ToList();

            if (subtarefasPendentes.Any() && contexto.Tarefa.Etapas[contexto.IndiceEtapa].Tentativas < 5)
            {
                throw new ServicoException("Ocorreu um erro ao processar Carregamentos.");
            }
        }

        private async Task ProcessarSubtarefaAsync(
            RequestSubtarefa subtarefa,
            ConcurrentBag<BsonDocument> resultadosConcurrent,
            BsonArray cargasArray,
            CancellationToken cancellationToken)
        {
            using (var unitOfWork = new Repositorio.UnitOfWork(
                _tenantService.StringConexao(),
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento =
                        subtarefa.Dados.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>();

                    string mensagem = string.Empty;
                    int? codigoCarga = null;
                    int? codigoCargaPrimeiroTrecho = null;

                    if (carregamento.CarregamentosRedespacho?.Count > 0)
                    {
                        Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = await new WebService.Carga.Carga(
                            unitOfWork,
                            _tenantService.ObterTipoServicoMultisoftware(),
                            _tenantService.ObterCliente(),
                            _tenantService.ObterAuditado(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos),
                            _tenantService.AdminStringConexao()
                        ).GerarCarregamentoNovoAsync(carregamento, cancellationToken, wsRest: true);

                        mensagem = retorno.Mensagem;
                        codigoCarga = retorno.Objeto;
                        codigoCargaPrimeiroTrecho = retorno.Objeto;
                    }
                    else
                    {
                        BsonDocument primeiroTrecho = null;

                        var primeiroTrechoEncontrado = resultadosConcurrent.FirstOrDefault(c =>
                            c.Contains("codigo_carga_primeiro_trecho") &&
                            c["codigo_carga_primeiro_trecho"].AsInt32 > 0);

                        if (primeiroTrechoEncontrado != null)
                        {
                            primeiroTrecho = primeiroTrechoEncontrado;
                        }
                        else
                        {
                            lock (cargasArray)
                            {
                                primeiroTrecho = cargasArray.Cast<BsonValue>()
                                    .Where(c => c.IsBsonDocument)
                                    .Select(c => c.AsBsonDocument)
                                    .FirstOrDefault(c =>
                                        c.Contains("codigo_carga_primeiro_trecho") &&
                                        c["codigo_carga_primeiro_trecho"].AsInt32 > 0);
                            }
                        }

                        if (primeiroTrecho == null)
                        {
                            throw new ServicoException("Não foi possível gerar o carregamento completo, pois não foi encontrado o carregamento do primeiro trecho.");
                        }

                        int codigoPrimeiroTrecho = primeiroTrecho["codigo_carga_primeiro_trecho"].AsInt32;
                        codigoCarga = await GerarCarregamentoSegundoTrecho(carregamento, codigoPrimeiroTrecho, unitOfWork);
                        mensagem = "Carregamento segundo trecho gerado com sucesso";
                    }

                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Concluida,
                        mensagem,
                        cancellationToken);

                    var cargaDoc = new BsonDocument
                    {
                        { "codigo", subtarefa.Id },
                        { "protocolo", codigoCarga?.ToString() ?? "" },
                        { "codigo_carga_primeiro_trecho", codigoCargaPrimeiroTrecho ?? 0 },
                        { "mensagem", mensagem ?? "" }
                    };
                    resultadosConcurrent.Add(cargaDoc);
                }
                catch (BaseException excecao)
                {
                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Falha,
                        excecao.Message,
                        cancellationToken);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao, "GerarCarregamentoComRedespacho");
                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Falha,
                        "Erro ao processar carregamento.",
                        cancellationToken);
                }
            }
        }

        private async Task<int> GerarCarregamentoSegundoTrecho(
            Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento,
            int codigoCargaPrimeiroTrecho,
            Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Carga.CargaDistribuidor servicoCargaDistribuidor =
                    new Servicos.Embarcador.Carga.CargaDistribuidor(unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();
                Dominio.ObjetosDeValor.WebService.Carga.GerarAgrupamento.DadosValidados dadosCarregamento = await new Servicos.WebService.Carga.Carga(unitOfWork).ValidarDadosParaGerarCarregamentoAsync(carregamento);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaPrimeiroTrecho = await repositorioCarga.BuscarPorCodigoAsync(codigoCargaPrimeiroTrecho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPrimeiroTrecho = await repositorioCargaPedido.BuscarPorCodigoCargaAsync(cargaPrimeiroTrecho.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaSegundoTrecho = await repositorioPedido.BuscarPorCodigosAsync(carregamento.ProtocolosPedidos);
                List<Dominio.Entidades.Cliente> recebedores = pedidosCargaSegundoTrecho.Select(pedido => pedido.Recebedor).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaPedidosPrimeiroTrecho.Where(cargaPedido => carregamento.ProtocolosPedidos.Contains(cargaPedido.Pedido.Protocolo)).ToList();

                if (recebedores.Count > 1)
                    throw new ServicoException($"Não é possível gerar o carregamento {carregamento.NumeroCarregamento}, pois existem mais de um recebedor.");

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDistribuidor cargaDistribuidor = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDistribuidor()
                {
                    CargaAntiga = cargaPrimeiroTrecho,
                    TipoOperacao = dadosCarregamento.TipoOperacao,
                    Distancia = carregamento.DistanciaEmKm,
                    UsarTipoOperacao = true,
                    Expedidor = recebedores.FirstOrDefault(),
                    CargaPedidos = cargaPedidos,
                    Empresa = dadosCarregamento.EmpresaIntegradora,
                    ConfiguracaoTMS = configuracaoEmbarcador,
                    VincularTrechos = true,
                    Redespacho = null,
                    Veiculo = dadosCarregamento.Veiculo,
                    RedespachoContainer = false,
                    ModeloVeicularCarga = dadosCarregamento.ModeloVeicularCarga,
                    Recebedor = null,
                    Motorista = dadosCarregamento.Motorista,
                    VeiculosVinculados = null,
                    CodigoCargaEmbarcador = carregamento.NumeroCarregamento
                };

                Dominio.Entidades.Embarcador.Cargas.Carga CargaGerada = servicoCargaDistribuidor.GerarCargaProximoTrecho(cargaDistribuidor);

                await unitOfWork.CommitChangesAsync();

                return CargaGerada.Codigo;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Log.TratarErro(ex, "ProcessadorTarefasGerarCargaRedespacho");

                throw new ServicoException("Erro ao gerar o carregamento de segundo trecho.");
            }
        }
    }
}
