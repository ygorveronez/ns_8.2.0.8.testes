using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Net.Mail;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PlanejamentoPedido")]
    public class PlanejamentoPedidoController : BaseController
    {
        #region Construtores

        public PlanejamentoPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> DuplicarPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int quantidadeDuplicar = Request.GetIntParam("QuantidadeDuplicar");
                DateTime dataPedido = Request.GetDateTimeParam("DataPedido");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                if (pedido.Cotacao)
                    return new JsonpResult(false, true, "Não é possível duplicar um pedido de cotação.");

                if (pedido.ColetaEmProdutorRural)
                    return new JsonpResult(false, true, "Não é possível duplicar um pedido de coleta em produtor rural.");

                unitOfWork.Start();

                string retorno = DuplicarPedido(pedido, dataPedido, quantidadeDuplicar, unitOfWork);

                if (!string.IsNullOrEmpty(retorno))
                    throw new ControllerException(retorno);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao duplicar o pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo, auditavel: true);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                pedido.Ordem = Request.GetNullableStringParam("Ordem");
                pedido.ObservacaoInterna = Request.GetNullableStringParam("ObservacaoInterna");
                string numeroFrota = Request.GetNullableStringParam("NumeroFrota");

                List<Dominio.Entidades.Veiculo> veiculos = !string.IsNullOrWhiteSpace(numeroFrota) ? repositorioVeiculo.BuscarPorNumeroDaFrota(numeroFrota) : null;

                string numeroFrotaPedido = ObterFrota(pedido);
                string nomeMotoristaPedido = ObterNomeMotorista(pedido);

                bool alterouFrota = numeroFrotaPedido != numeroFrota;

                unitOfWork.Start();

                repositorioPedido.Atualizar(pedido, Auditado);

                if (alterouFrota)
                {
                    if (veiculos?.FirstOrDefault() != null)
                        SalvarVeiculoNoPedido(veiculos?.FirstOrDefault(), pedido, unitOfWork);
                    else
                    {
                        RemoverVeiculoDoPedido(pedido, unitOfWork);
                        RemoverMotoristaDoPedido(pedido, unitOfWork);
                    }

                    string nomeMotorista = ObterNomeMotorista(pedido);

                    AtualizarDisponibilidade(pedido.DataCarregamentoPedido ?? DateTime.MinValue, numeroFrotaPedido, numeroFrota, unitOfWork);
                    AtualizarDisponibilidadeMotorista(pedido.DataCarregamentoPedido ?? DateTime.MinValue, nomeMotoristaPedido, nomeMotorista, unitOfWork);
                }

                unitOfWork.CommitChanges();

                bool obterFrotaDuplicada = ObterFrotaDuplicada(pedido.DataCarregamentoPedido ?? DateTime.MinValue, numeroFrota, unitOfWork);

                return new JsonpResult(ObterPedido(pedido, repositorioCargaPedido, repositorioPedido), true, obterFrotaDuplicada ? "Frota utilizada em mais de uma viagem." : string.Empty);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosDisponibilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade repPlanejamentoPedidoDisponibilidade = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade planejamentoDisponibilidade = repPlanejamentoPedidoDisponibilidade.BuscarPorCodigo(codigo);

                if (planejamentoDisponibilidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a disponibilidade.");

                planejamentoDisponibilidade.Observacao = Request.GetStringParam("Observacao");

                repPlanejamentoPedidoDisponibilidade.Atualizar(planejamentoDisponibilidade);

                return new JsonpResult(ObterNovaDisponibilidade(planejamentoDisponibilidade));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados da disponibilidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosDisponibilidadeMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista repPlanejamentoPedidoDisponibilidadeMotorista = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista planejamentoDisponibilidadeMotorista = repPlanejamentoPedidoDisponibilidadeMotorista.BuscarPorCodigo(codigo);

                if (planejamentoDisponibilidadeMotorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a disponibilidade do motorista.");

                planejamentoDisponibilidadeMotorista.Observacao = Request.GetStringParam("Observacao");

                repPlanejamentoPedidoDisponibilidadeMotorista.Atualizar(planejamentoDisponibilidadeMotorista);

                return new JsonpResult(ObterNovaDisponibilidadeMotorista(planejamentoDisponibilidadeMotorista));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados da disponibilidade do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotalVeiculosAlocados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime data = Request.GetDateTimeParam("Data");

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade repPlanejamentoPedidoDisponibilidade = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade(unitOfWork);
                int Quantidade = repPlanejamentoPedidoDisponibilidade.ContarFrotaEntreData(data.Date, data.Date.AddDays(1));

                return new JsonpResult(new { Quantidade });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o total de veículos alocados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotalMotoristasAlocados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime data = Request.GetDateTimeParam("Data");

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista repPlanejamentoPedidoDisponibilidadeMotorista = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista(unitOfWork);
                int Quantidade = repPlanejamentoPedidoDisponibilidadeMotorista.ContarMotoristaEntreData(data.Date, data.Date.AddDays(1));

                return new JsonpResult(new { Quantidade });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o total de motoristas alocados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DuplicarPedidosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPlanejamentoPedido(unitOfWork);

                int quantidadeDuplicar = Request.GetIntParam("QuantidadeDuplicar");
                DateTime dataPedido = Request.GetDateTimeParam("DataPedido");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    if (pedido == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                    if (pedido.Cotacao)
                        return new JsonpResult(false, true, "Não é possível duplicar um pedido de cotação.");

                    if (pedido.ColetaEmProdutorRural)
                        return new JsonpResult(false, true, "Não é possível duplicar um pedido de coleta em produtor rural.");
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    unitOfWork.Start();

                    string retorno = DuplicarPedido(pedido, dataPedido, quantidadeDuplicar, unitOfWork);

                    if (!string.IsNullOrEmpty(retorno))
                        throw new ControllerException(retorno);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao duplicar os pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterVeiculosVinculados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                return new JsonpResult(ObterGridPlacaCarregamento(pedido));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> ObterVeiculosVinculadosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                var lista = (from obj in pedido.VeiculosCarregamento
                             select new
                             {
                                 obj.Codigo,

                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> SalvarPlacasCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoPedido");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> ListaCarga = repCarga.BuscarCargasPorPedido(codigo);

                pedido.VeiculosCarregamento.Clear();
                List<dynamic> PlacasCarregamento = Request.GetListParam<dynamic>("ItensSelecionados");

                var codigosSelecionados = PlacasCarregamento.Select(p => ((string)p.Codigo).ToInt()).ToList();

                foreach (var placa in PlacasCarregamento)
                {
                    int codigoVeiculo = ((string)placa.Codigo).ToInt();
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    if (veiculo != null)
                    {
                        pedido.VeiculosCarregamento.Add(veiculo);
                        repPedido.Atualizar(pedido);
                    }

                    if (ListaCarga != null && ListaCarga.Count > 0)
                    {
                        foreach (var carga in ListaCarga)
                        {
                            foreach (var v in carga.VeiculosCarregamento.Where(v => !codigosSelecionados.Contains(v.Codigo)).ToList())
                                carga.VeiculosCarregamento.Remove(v);

                            if (!carga.VeiculosCarregamento.Any(x => x.Codigo == codigoVeiculo))
                            {
                                carga.VeiculosCarregamento.Add(veiculo);
                            }
                            repCarga.Atualizar(carga);
                        }
                    }
                }

                return new JsonpResult(true);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados da disponibilidade do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarPedidosAurora()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string email = Request.GetStringParam("Email");
                string arquivo = GerarRelatorioPedidosSelecionados(unitOfWork);

                string ret = EnviarEmail(email, "Relatório em anexo", "Placas", arquivo, _conexao.StringConexao, unitOfWork);

                if (!string.IsNullOrEmpty(ret))
                    return new JsonpResult(false, true, ret);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterListaDisponibilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicial = Request.GetDateTimeParam("DataInicio");
                DateTime dataFinal = Request.GetDateTimeParam("DataFim");

                List<dynamic> listaDisponibilidade = new List<dynamic>();

                IEnumerable<DateTime> AllDatesBetween(DateTime startDate, DateTime endDate)
                {
                    for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1))
                        yield return date;
                }

                int codigo = 1;
                foreach (DateTime day in AllDatesBetween(dataInicial, dataFinal))
                {
                    var retorno = new
                    {
                        Codigo = codigo++,
                        Data = day.ToDateString()
                    };

                    listaDisponibilidade.Add(retorno);
                }

                return new JsonpResult(listaDisponibilidade.ToList());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a lista de disponibilidades");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDisponibilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime data = Request.GetDateTimeParam("Data");
                string filtroPlanejamentoDisponibilidade = Request.GetStringParam("FiltroPlanejamentoDisponibilidade");

                if (data == DateTime.MinValue)
                    data = DateTime.Now.Date;

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade repPlanejamentoPedidoDisponibilidade = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade> planejamentosDisponibilidade = repPlanejamentoPedidoDisponibilidade.BuscarDisponivelPorData(data);

                if (planejamentosDisponibilidade.Count == 0)
                {
                    GeraPlanejamentoDisponibilidade(data, unitOfWork);
                    planejamentosDisponibilidade = repPlanejamentoPedidoDisponibilidade.BuscarDisponivelPorData(data);
                }

                if (!string.IsNullOrEmpty(filtroPlanejamentoDisponibilidade))
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade> planejamentosDisponibilidadeFiltro = new List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade>();

                    string[] listaFiltros = filtroPlanejamentoDisponibilidade.Split(' ');

                    foreach (var filtro in listaFiltros)
                    {
                        planejamentosDisponibilidadeFiltro.AddRange(planejamentosDisponibilidade.Where(o => (o.Observacao != null && o.Observacao.ToUpper().Contains(filtro.ToUpper()))).ToList());
                    }

                    planejamentosDisponibilidade = planejamentosDisponibilidadeFiltro.Distinct().ToList();
                }

                return new JsonpResult(ObterGridDisponibilidade(planejamentosDisponibilidade));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter disponibilidade");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDisponibilidadeMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime data = Request.GetDateTimeParam("Data");
                string filtroPlanejamentoDisponibilidadeMotorista = Request.GetStringParam("FiltroPlanejamentoDisponibilidadeMotorista");

                if (data == DateTime.MinValue)
                    data = DateTime.Now.Date;

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista repPlanejamentoPedidoDisponibilidadeMotorista = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista> planejamentosDisponibilidadeMotorista = repPlanejamentoPedidoDisponibilidadeMotorista.BuscarDisponivelPorData(data);

                if (planejamentosDisponibilidadeMotorista.Count == 0)
                {
                    GeraPlanejamentoDisponibilidadeMotorista(data, unitOfWork);
                    planejamentosDisponibilidadeMotorista = repPlanejamentoPedidoDisponibilidadeMotorista.BuscarDisponivelPorData(data);
                }

                if (!string.IsNullOrEmpty(filtroPlanejamentoDisponibilidadeMotorista))
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista> planejamentosDisponibilidadeFiltro = new List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista>();

                    string[] listaFiltros = filtroPlanejamentoDisponibilidadeMotorista.Split(' ');

                    foreach (var filtro in listaFiltros)
                    {
                        planejamentosDisponibilidadeFiltro.AddRange(planejamentosDisponibilidadeMotorista.Where(o => (o.Observacao != null && o.Observacao.ToUpper().Contains(filtro.ToUpper()))).ToList());
                    }

                    planejamentosDisponibilidadeMotorista = planejamentosDisponibilidadeFiltro.Distinct().ToList();
                }

                return new JsonpResult(ObterGridDisponibilidadeMotorista(planejamentosDisponibilidadeMotorista));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter disponibilidade do motorista");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarSituacaoPlanejamentoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo, auditavel: true);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                SituacaoPlanejamentoPedido? situacaoPlanejamentoPedido = Request.GetNullableEnumParam<SituacaoPlanejamentoPedido>("SituacaoPlanejamentoPedido");

                if (!situacaoPlanejamentoPedido.HasValue)
                    return new JsonpResult(false, true, "Situação de planejamento do pedido não informada.");

                if (situacaoPlanejamentoPedido.Value == pedido.SituacaoPlanejamentoPedido)
                    return new JsonpResult(false, true, "A situação de planejamento do pedido informada é igual a atual. Não é possível realizar a alteração.");

                pedido.SituacaoPlanejamentoPedido = situacaoPlanejamentoPedido.Value;

                repositorioPedido.Atualizar(pedido, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o veículo.");

                unitOfWork.Start();

                string numeroFrotaPedido = ObterFrota(pedido);
                string nomeMotoristaPedido = ObterNomeMotorista(pedido);

                SalvarVeiculoNoPedido(veiculo, pedido, unitOfWork);

                string numeroFrota = ObterFrota(pedido);
                string nomeMotorista = ObterNomeMotorista(pedido);

                AtualizarDisponibilidade(pedido.DataCarregamentoPedido ?? DateTime.MinValue, numeroFrotaPedido, numeroFrota, unitOfWork);
                AtualizarDisponibilidadeMotorista(pedido.DataCarregamentoPedido ?? DateTime.MinValue, nomeMotoristaPedido, nomeMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao definir o veículo do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                int codigoMotorista = Request.GetIntParam("Motorista");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (motorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o motorista.");

                unitOfWork.Start();

                string nomeMotoristaPedido = ObterNomeMotorista(pedido);

                if (pedido.Motoristas == null)
                    pedido.Motoristas = new List<Dominio.Entidades.Usuario>();
                else
                    pedido.Motoristas.Clear();

                pedido.Motoristas.Add(motorista);

                AtualizarCargaPorPedido(pedido, unitOfWork);

                repositorioPedido.Atualizar(pedido);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o motorista {motorista.Nome} para o pedido.", unitOfWork);

                string nomeMotorista = ObterNomeMotorista(pedido);
                AtualizarDisponibilidadeMotorista(pedido.DataCarregamentoPedido ?? DateTime.MinValue, nomeMotoristaPedido, nomeMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao definir o motorista do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                unitOfWork.Start();

                string numeroFrotaPedido = ObterFrota(pedido);

                RemoverVeiculoDoPedido(pedido, unitOfWork);

                string numeroFrota = ObterFrota(pedido);
                AtualizarDisponibilidade(pedido.DataCarregamentoPedido ?? DateTime.MinValue, numeroFrotaPedido, numeroFrota, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o veículo do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedido.Codigo);

                unitOfWork.Start();

                string nomeMotoristaPedido = ObterNomeMotorista(pedido);

                RemoverMotoristaDoPedido(pedido, unitOfWork);

                string nomeMotorista = ObterNomeMotorista(pedido);
                AtualizarDisponibilidadeMotorista(pedido.DataCarregamentoPedido ?? DateTime.MinValue, nomeMotoristaPedido, nomeMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o motorista do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoPlanejamentoPedido", false);
                grid.AdicionarCabecalho("PossuiVeiculo", false);
                grid.AdicionarCabecalho("NecessitaInformarPlacaCarregamento", false);
                grid.AdicionarCabecalho("PossuiMotorista", false);
                grid.AdicionarCabecalho("DataCarregamentoPedido", false);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Ord/Qnt", "Ordem", 8, Models.Grid.Align.left, true, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 150));
                grid.AdicionarCabecalho("Observação", "ObservacaoInterna", 8, Models.Grid.Align.left, true, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 150));
                grid.AdicionarCabecalho("Frota", "NumeroFrota", 8, Models.Grid.Align.left, false, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 10));
                grid.AdicionarCabecalho("Motorista", "Motorista", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, false);

                if (configuracaoGeral.HabilitarFuncionalidadesProjetoGollum)
                {
                    grid.AdicionarCabecalho("Categoria OS", "CategoriaOS", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Tipo OS Convertido", "TipoOSConvertido", 8, Models.Grid.Align.left, false);
                }

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                if (parametrosConsulta.PropriedadeOrdenar == "Codigo")
                    parametrosConsulta.DirecaoOrdenar = "asc";

                parametrosConsulta.PropriedadeOrdenar = $"DataCarregamentoPedido, {parametrosConsulta.PropriedadeOrdenar}";


                int totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                var listaPedidoRetornar = (
                    from pedido in listaPedido
                    select ObterPedido(pedido, repositorioCargaPedido, repositorioPedido)
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterCorLinha(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.SituacaoPedido == SituacaoPedido.Finalizado)
                return "#0070C0";

            if (pedido.SituacaoPedido == SituacaoPedido.Cancelado)
                return "#ff0000";

            return pedido.SituacaoPlanejamentoPedido.ObterCorLinha();
        }

        private string ObterCorFonte(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.SituacaoPedido == SituacaoPedido.Cancelado ||
                pedido.SituacaoPedido == SituacaoPedido.Finalizado)
                return CorGrid.Branco;

            return CorGrid.Black;
        }

        private dynamic ObterPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido, Repositorio.Embarcador.Pedidos.Pedido repositorioPedido)
        {
            return new
            {
                pedido.Codigo,
                NumeroCarga = string.Join(", ", repositorioCargaPedido.BuscarNumeroCargasPorPedido(pedido.Codigo)),
                Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? string.Empty,
                Motorista = string.Join(", ", repositorioPedido.BuscarMotoristas(pedido.Codigo)),
                PossuiMotorista = pedido.Motoristas?.Count > 0,
                NumeroFrota = ObterFrota(pedido),
                Veiculo = ObterPlacas(pedido),
                PossuiVeiculo = pedido.Veiculos?.Count > 0,
                pedido.SituacaoPlanejamentoPedido,
                pedido.Ordem,
                pedido.ObservacaoInterna,
                ModeloVeicularCarga = pedido.ModeloVeicularCarga?.Descricao,
                Tomador = pedido.ObterTomador()?.Descricao ?? string.Empty,
                CategoriaOS = pedido.CategoriaOS?.ObterDescricao() ?? string.Empty,
                TipoOSConvertido = pedido.TipoOSConvertido?.ObterDescricao() ?? string.Empty,
                NecessitaInformarPlacaCarregamento = (pedido.TipoOperacao?.ConfiguracaoCarga?.NecessitaInformarPlacaCarregamento ?? false) && repositorioCargaPedido.BuscarPorPedido(pedido.Codigo).Count > 0 ? true : false,
                DT_RowColor = ObterCorLinha(pedido),
                DT_FontColor = ObterCorFonte(pedido),
                DT_Enable = true
            };
        }

        private dynamic ObterNovaDisponibilidade(Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade planejamentoDisponibilidade)
        {
            return new
            {
                planejamentoDisponibilidade.Codigo,
                planejamentoDisponibilidade.Data,
                planejamentoDisponibilidade.NumeroFrota,
                planejamentoDisponibilidade.Observacao,
                planejamentoDisponibilidade.NumeroFrotaNumero,

                DT_Enable = true
            };
        }

        private dynamic ObterNovaDisponibilidadeMotorista(Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista planejamentoDisponibilidadeMotorista)
        {
            return new
            {
                planejamentoDisponibilidadeMotorista.Codigo,
                planejamentoDisponibilidadeMotorista.Data,
                planejamentoDisponibilidadeMotorista.Nome,
                planejamentoDisponibilidadeMotorista.Observacao,

                DT_Enable = true
            };
        }

        private void RemoverMotoristaDoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedido.Codigo);
            Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

            pedido.Motoristas.Clear();

            repositorioPedido.Atualizar(pedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Nova && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    throw new ControllerException($"Não é possível remover o veículo pois a carga já está na etapa {cargaPedido.Carga.SituacaoCarga.ObterDescricao()}.");

                cargaPedido.Carga.Motoristas.Clear();

                repCarga.Atualizar(cargaPedido.Carga);

                try
                {
                    string erros = string.Empty;
                    serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Planejamento, cargaPedido.Carga.Veiculo, cargaPedido.Carga.VeiculosVinculados, null, null, DateTime.Now, pedido, cargaPedido.Carga, null, "Removido o motorista! ");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Removeu o motorista do pedido.", unitOfWork);
        }

        private void RemoverVeiculoDoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedido.Codigo);
            Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

            pedido.Veiculos.Clear();

            repositorioPedido.Atualizar(pedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Nova && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    throw new ControllerException($"Não é possível remover o veículo pois a carga já está na etapa {cargaPedido.Carga.SituacaoCarga.ObterDescricao()}.");

                cargaPedido.Carga.Veiculo = null;
                cargaPedido.Carga.VeiculosVinculados.Clear();

                repCarga.Atualizar(cargaPedido.Carga);

                try
                {
                    string erros = string.Empty;
                    serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Planejamento, cargaPedido.Carga.Veiculo, cargaPedido.Carga.VeiculosVinculados, cargaPedido.Carga.Motoristas, null, DateTime.Now, cargaPedido.Pedido, cargaPedido.Carga, null, "Removido o veículo! ");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Removeu o veículo do pedido.", unitOfWork);
        }

        private void SalvarVeiculoNoPedido(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
            Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

            if (pedido.Veiculos == null)
                pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
                pedido.Veiculos.Clear();

            if (pedido.Motoristas == null)
                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();
            else
                pedido.Motoristas.Clear();

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                if (veiculo.TipoVeiculo == "0")
                    pedido.VeiculoTracao = veiculo;
                else
                {
                    if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                        pedido.VeiculoTracao = veiculo.VeiculosTracao.FirstOrDefault();
                    else
                        pedido.VeiculoTracao = veiculo;
                }

                if (pedido.VeiculoTracao?.VeiculosVinculados != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in pedido.VeiculoTracao.VeiculosVinculados)
                        pedido.Veiculos.Add(reboque);
                }
            }
            else
            {
                pedido.Veiculos.Add(veiculo);
            }

            if (veiculoMotorista != null)
                pedido.Motoristas.Add(veiculoMotorista);

            if (veiculo.ModeloVeicularCarga != null)
                pedido.ModeloVeicularCarga = veiculo?.VeiculosVinculados?.FirstOrDefault()?.ModeloVeicularCarga ?? null;


            AtualizarCargaPorPedido(pedido, unitOfWork);

            repositorioPedido.Atualizar(pedido);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o veículo {veiculo.Placa_Formatada} para o pedido", unitOfWork);

            try
            {
                string erros = string.Empty;
                serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Planejamento, pedido.VeiculoTracao, pedido.Veiculos, pedido.Motoristas, DateTime.Now, null, pedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private string ObterPlacas(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.Veiculos.Count == 1)
            {
                Dominio.Entidades.Veiculo veiculo = pedido.Veiculos.FirstOrDefault();
                string placa = veiculo.Placa;

                if (veiculo.VeiculosVinculados.Count > 0)
                    placa += ", " + string.Join(", ", veiculo.VeiculosVinculados.Select(o => o.Placa));

                return placa;
            }
            else
            {
                return string.Join(", ", pedido.Veiculos.Select(o => o.Placa));
            }
        }

        private string ObterFrota(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.Veiculos.Count == 1)
            {
                Dominio.Entidades.Veiculo veiculo = pedido.Veiculos.FirstOrDefault();
                List<string> numeroFrota = new List<string>() { veiculo.NumeroFrota };

                numeroFrota.AddRange(veiculo.VeiculosVinculados.Select(o => o.NumeroFrota));

                return string.Join(", ", numeroFrota.Where(o => !string.IsNullOrWhiteSpace(o)));
            }
            else
            {
                return string.Join(", ", pedido.Veiculos.Where(o => !string.IsNullOrWhiteSpace(o.NumeroFrota)).OrderBy(o => o.TipoVeiculo).Select(o => o.NumeroFrota));
            }
        }

        private string ObterNomeMotorista(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.Motoristas == null)
                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

            if (pedido.Motoristas.Count == 1)
            {
                Dominio.Entidades.Usuario motorista = pedido.Motoristas.FirstOrDefault();
                return !string.IsNullOrWhiteSpace(motorista.Apelido) ? motorista.Apelido : motorista.Nome;
            }
            else
                return string.Join(", ", pedido.Motoristas.Where(o => !string.IsNullOrWhiteSpace(o.Nome)).Select(o => !string.IsNullOrWhiteSpace(o.Apelido) ? o.Apelido : o.Nome));
        }

        private void AtualizarCargaPorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

            servicoCarga.AtualizarCargaPorPedido(pedido, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador, ClienteMultisoftware: Cliente, Auditado, true);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorPedido(pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargasPedido.FirstOrDefault()?.Carga;

                string erros = string.Empty;
                serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Planejamento, carga.Veiculo, carga.VeiculosVinculados, carga.Motoristas, DateTime.Now, null, pedido, carga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial },
                CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga },
                CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao },
                CidadePoloDestino = Request.GetIntParam("CidadePoloDestino"),
                CidadePoloOrigem = Request.GetIntParam("CidadePoloOrigem"),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                Destino = Request.GetIntParam("Destino"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroNotaFiscal = Request.GetIntParam("NotaFiscal"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                Origem = Request.GetIntParam("Origem"),
                PaisDestino = Request.GetIntParam("PaisDestino"),
                PaisOrigem = Request.GetIntParam("PaisOrigem"),
                ProvedorOS = Request.GetDoubleParam("ProvedorOS"),
                Remetente = Request.GetDoubleParam("Remetente"),
                Situacao = Request.GetNullableEnumParam<SituacaoPedido>("Situacao"),
                SituacaoPlanejamentoPedido = Request.GetNullableEnumParam<SituacaoPlanejamentoPedido>("SituacaoPlanejamentoPedido"),
                Tomador = Request.GetDoubleParam("Tomador"),
                CodigosMotorista = codigoMotorista > 0 ? new List<int>() { codigoMotorista } : null,
                CodigosVeiculo = codigoVeiculo > 0 ? new List<int>() { codigoVeiculo } : null,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CategoriaOS = Request.GetListEnumParam<CategoriaOS>("CategoriaOS"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                UsuarioUtilizaSegregacaoPorProvedor = Usuario.UsuarioUtilizaSegregacaoPorProvedor,
                CodigosProvedores = Usuario.UsuarioUtilizaSegregacaoPorProvedor ? Usuario.ClientesProvedores.Select(o => o.CPF_CNPJ).ToList() : new List<double>()
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente")
                return "Remetente.Nome";

            if (propriedadeOrdenar == "Destinatario")
                return "Destinatario.Nome";

            return propriedadeOrdenar;
        }

        private void GeraPlanejamentoDisponibilidade(DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade repPlanejamentoPedidoDisponibilidade = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade(unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<string> frotas = repositorioVeiculo.BuscarFotasVeiculosDisponiblidade();

            unitOfWork.Start();

            try
            {
                foreach (string frota in frotas)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAnterior = repPedidos.BuscarPorFrotaEDataCarregamentoMenor(frota, data);

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedidos.BuscarPorFrotaEData(frota, data);

                    Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade disponibilidade = new Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade
                    {
                        Data = data,
                        NumeroFrota = frota,
                        StatusMotorista = StatusDisponibilidadePlanejamentoPedido.Disponivel,
                        StatusViagem = StatusDisponibilidadePlanejamentoPedido.Disponivel,
                        StatusVeiculo = StatusDisponibilidadePlanejamentoPedido.Disponivel,
                        DataCadastro = DateTime.Now,
                        Localizacao = pedidoAnterior?.Destino?.DescricaoCidadeEstado ?? string.Empty,
                        Disponivel = pedido == null ? true : false
                    };

                    repPlanejamentoPedidoDisponibilidade.Inserir(disponibilidade);
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw;
            }

        }

        private void GeraPlanejamentoDisponibilidadeMotorista(DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista repPlanejamentoPedidoDisponibilidadeMotorista = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista(unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<string> nomesMotorista = repositorioUsuario.BuscarNomesMotoristasDisponibilidade();

            unitOfWork.Start();

            try
            {
                foreach (string nomeMotorista in nomesMotorista)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAnterior = repPedidos.BuscarPorNomeMotoristaEDataCarregamentoMenor(nomeMotorista, data);

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedidos.BuscarPorNomeMotoristaEData(nomeMotorista, data);

                    Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista disponibilidade = new Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista
                    {
                        Data = data,
                        Nome = nomeMotorista,
                        DataCadastro = DateTime.Now,
                        Disponivel = pedido == null ? true : false
                    };

                    repPlanejamentoPedidoDisponibilidadeMotorista.Inserir(disponibilidade);
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPlanejamentoPedido(Repositorio.UnitOfWork unitOfWork)
        {
            var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

            List<int> codigos = new List<int>();

            foreach (var item in listaItensSelecionados)
            {
                codigos.Add((int)item.Codigo);
            }

            Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            return repPedidos.BuscarPorCodigos(codigos);
        }

        private string EnviarEmail(string email, string mensagem, string assunto, string caminhoArquivoAnexo, string stringConexao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (string.IsNullOrEmpty(email))
                return "e-mail não configurado";

            try
            {
                List<Attachment> anexos = new List<Attachment>();

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoAnexo))
                {
                    Attachment anexo = new Attachment(caminhoArquivoAnexo);
                    anexo.Name = System.IO.Path.GetFileName(caminhoArquivoAnexo);
                    anexos.Add(anexo);
                }

                Servicos.Email serEmail = new Servicos.Email(unidadeTrabalho);

                serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, "", "", assunto, mensagem, string.Empty, anexos, "", true, string.Empty, 0, unidadeTrabalho);


                foreach (var anexo in anexos)
                    anexo.Dispose();

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoAnexo))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivoAnexo);


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return "Erro ao enviar e-mail";
            }

            return "";
        }

        private string DuplicarPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime dataPedido, int quantidadeDuplicar, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);

            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido();
            Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

            for (var i = 0; i < quantidadeDuplicar; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoNovo = pedido.Clonar();
                pedido.ControleNumeracao = pedido.Codigo;
                repPedido.Atualizar(pedido);

                Utilidades.Object.DefinirListasGenericasComoNulas(pedidoNovo);

                pedidoNovo.SituacaoPlanejamentoPedido = SituacaoPlanejamentoPedido.Pendente;
                pedidoNovo.SituacaoPedido = SituacaoPedido.Aberto;
                pedidoNovo.Numero = repPedido.BuscarProximoNumero();
                pedidoNovo.DataCarregamentoPedido = dataPedido;
                pedidoNovo.ObservacaoInterna = null;
                pedidoNovo.Ordem = null;
                pedidoNovo.NumeroPedidoEmbarcador = null;
                pedidoNovo.CodigoCargaEmbarcador = null;
                pedidoNovo.DataCriacao = DateTime.Now;
                pedidoNovo.DataFinalColeta = null;
                pedidoNovo.DataFinalViagemExecutada = null;
                pedidoNovo.DataFinalViagemFaturada = null;
                pedidoNovo.DataInicialColeta = null;
                pedidoNovo.DataInicialViagemExecutada = null;
                pedidoNovo.DataInicialViagemFaturada = null;
                pedidoNovo.DataPrevisaoChegadaDestinatario = null;
                pedidoNovo.DataPrevisaoSaida = null;
                pedidoNovo.DataPrevisaoSaidaDestinatario = null;
                pedidoNovo.DataVencimentoArmazenamentoImportacao = null;
                pedidoNovo.PrevisaoEntrega = null;
                pedidoNovo.EnderecoDestino = null;
                pedidoNovo.EnderecoOrigem = null;
                pedidoNovo.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                pedidoNovo.DataCarregamentoPedido = dataPedido;
                pedidoNovo.DataInicialColeta = dataPedido;

                svcPedido.PreencherCodigoCargaEmbarcador(pedidoNovo, ConfiguracaoEmbarcador, unitOfWork);

                repPedido.Inserir(pedidoNovo);

                if (pedido.EnderecoOrigem != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = pedido.EnderecoOrigem.Clonar();

                    Utilidades.Object.DefinirListasGenericasComoNulas(enderecoOrigem);

                    repPedidoEndereco.Inserir(enderecoOrigem);

                    pedidoNovo.EnderecoOrigem = enderecoOrigem;
                }

                if (pedido.EnderecoDestino != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = pedido.EnderecoDestino.Clonar();

                    Utilidades.Object.DefinirListasGenericasComoNulas(enderecoDestino);

                    repPedidoEndereco.Inserir(enderecoDestino);

                    pedidoNovo.EnderecoDestino = enderecoDestino;
                }

                if (!ConfiguracaoEmbarcador.UtilizarIntegracaoPedido)
                {
                    unitOfWork.Clear(pedidoNovo);

                    pedidoNovo = repPedido.BuscarPorCodigo(pedidoNovo.Codigo);

                    pedidoNovo.PedidoIntegradoEmbarcador = true;

                    string retorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedidoNovo, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador);

                    if (string.IsNullOrWhiteSpace(retorno))
                        repPedido.Atualizar(pedidoNovo);
                    else
                    {
                        return retorno;
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoNovo, "Criou o pedido à partir da duplicação pela tela de planejamento de pedidos.", unitOfWork);

                try
                {
                    string erros = string.Empty;
                    serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Planejamento, pedidoNovo.VeiculoTracao, pedidoNovo.Veiculos, pedidoNovo.Motoristas, DateTime.Now, null, pedidoNovo, null);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }

            return string.Empty;
        }

        private string GetSubstring(string text, string stopAt = ",")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation).Trim();
                }
            }

            return text;
        }

        private bool ObterFrotaDuplicada(DateTime data, string numeroFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade repPlanejamentoPedidoDisponibilidade = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade(unitOfWork);
            return repPlanejamentoPedidoDisponibilidade.ContarPorFrotaEntreData(numeroFrota, data.Date, data.Date.AddDays(1)) > 1;
        }

        private void AtualizarDisponibilidade(DateTime data, string numeroFrotaPedido, string numeroFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade repPlanejamentoPedidoDisponibilidade = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade(unitOfWork);

            numeroFrotaPedido = GetSubstring(numeroFrotaPedido);
            numeroFrota = GetSubstring(numeroFrota);

            var disponibilidadeFrotaPedido = repPlanejamentoPedidoDisponibilidade.BuscarPorDataeFrota(data.Date, numeroFrotaPedido);
            var disponibilidadeFrota = repPlanejamentoPedidoDisponibilidade.BuscarPorDataeFrota(data.Date, numeroFrota);


            if (disponibilidadeFrotaPedido != null)
            {
                DateTime dataIni = data.Date;
                DateTime dataFim = data.Date.AddDays(1);
                Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedidos.BuscarPorFrotaEntreData(numeroFrotaPedido, dataIni, dataFim);
                if (pedido == null)
                {
                    disponibilidadeFrotaPedido.Disponivel = true;
                    repPlanejamentoPedidoDisponibilidade.Atualizar(disponibilidadeFrotaPedido);
                }
            }

            if (disponibilidadeFrota != null)
            {

                disponibilidadeFrota.Disponivel = false;
                repPlanejamentoPedidoDisponibilidade.Atualizar(disponibilidadeFrota);
            }
        }

        private void AtualizarDisponibilidadeMotorista(DateTime data, string nomeMotoristaPedido, string nomeMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista repPlanejamentoPedidoDisponibilidadeMotorista = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista(unitOfWork);

            nomeMotoristaPedido = GetSubstring(nomeMotoristaPedido);
            nomeMotorista = GetSubstring(nomeMotorista);

            Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista disponibilidadeMotoristaPedido = !string.IsNullOrWhiteSpace(nomeMotoristaPedido) ? repPlanejamentoPedidoDisponibilidadeMotorista.BuscarPorDataeNomeMotorista(data.Date, nomeMotoristaPedido) : null;
            Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista disponibilidadeMotorista = !string.IsNullOrWhiteSpace(nomeMotorista) ? repPlanejamentoPedidoDisponibilidadeMotorista.BuscarPorDataeNomeMotorista(data.Date, nomeMotorista) : null;

            if (disponibilidadeMotoristaPedido != null)
            {
                DateTime dataIni = data.Date;
                DateTime dataFim = data.Date.AddDays(1);
                Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedidos.BuscarPorNomeMotoristaEntreData(nomeMotoristaPedido, dataIni, dataFim);
                if (pedido == null)
                {
                    disponibilidadeMotoristaPedido.Disponivel = true;
                    repPlanejamentoPedidoDisponibilidadeMotorista.Atualizar(disponibilidadeMotoristaPedido);
                }
            }

            if (disponibilidadeMotorista != null)
            {
                disponibilidadeMotorista.Disponivel = false;
                repPlanejamentoPedidoDisponibilidadeMotorista.Atualizar(disponibilidadeMotorista);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido ObterStatusDisponibilidadePlanejamentoPedido(int StatusDisponibilidadePlanejamentoPedido)
        {
            if (StatusDisponibilidadePlanejamentoPedido > 3 || StatusDisponibilidadePlanejamentoPedido < 1)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido.Disponivel;

            return (Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido)StatusDisponibilidadePlanejamentoPedido;
        }

        private Models.Grid.Grid ObterGridDisponibilidade(List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade> planejamentosDisponibilidade)
        {
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Frota", "NumeroFrota", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 12, Models.Grid.Align.left, false, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 12));

                List<dynamic> listaDisponiblidadeRetornar = (
                   from disponiblidade in planejamentosDisponibilidade
                   select ObterNovaDisponibilidade(disponiblidade)).OrderBy(o => o.NumeroFrotaNumero).ToList();

                int totalRegistros = listaDisponiblidadeRetornar.Count();

                grid.AdicionaRows(listaDisponiblidadeRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private Models.Grid.Grid ObterGridDisponibilidadeMotorista(List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista> planejamentosDisponibilidadeMotorista)
        {
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Motorista", "Nome", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 10, Models.Grid.Align.left, false, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 12));

                List<dynamic> listaDisponiblidadeMotoristaRetornar = (
                   from disponibilidade in planejamentosDisponibilidadeMotorista
                   select ObterNovaDisponibilidadeMotorista(disponibilidade)).OrderBy(o => o.Nome).ToList();

                int totalRegistros = listaDisponiblidadeMotoristaRetornar.Count();

                grid.AdicionaRows(listaDisponiblidadeMotoristaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private Models.Grid.Grid ObterGridPlacaCarregamento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Placa", "Descricao", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("N° Frota", "NumeroFrota", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo de Carga", "ModeloCarga", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de veículo", "TipoVeiculo", 6, Models.Grid.Align.left, false);

                int totalRegistros = pedido.VeiculoTracao?.VeiculosVinculados?.Count() ?? 0;

                var lista = (from obj in pedido.VeiculoTracao?.VeiculosVinculados
                             select new
                             {

                                 obj.Codigo,
                                 obj.Descricao,
                                 NumeroFrota = obj.NumeroFrota ?? "",
                                 ModeloCarga = obj.Modelo?.Descricao ?? "",
                                 TipoVeiculo = obj.TipoDoVeiculo?.Descricao ?? "",

                             }).ToList();



                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private string GerarRelatorioPedidosSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido> dsPlanejamentoPedido = new List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido>();
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);


            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPlanejamentoPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenados = pedidos.OrderBy(o => o.DataCarregamentoPedido).ToList();

            foreach (var pedido in pedidosOrdenados)
            {
                var dataFormatada = pedido?.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? string.Empty;
                var planejamento = new Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido
                {
                    Data = dataFormatada,
                    Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                    Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                    ModeloVeicular = pedido.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Quantidade = pedido.Ordem,
                    Motorista = string.Join(", ", repositorioPedido.BuscarMotoristas(pedido.Codigo)),
                    Veiculo = ObterPlacas(pedido),
                    NumeroCarga = pedido.CodigoCargaEmbarcador
                };

                dsPlanejamentoPedido.Add(planejamento);

            }

            if (dsPlanejamentoPedido.Count == 0)
                throw new Exception("Nenhum pedido selecionado");

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsPlanejamentoPedido,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                }
            };

            var relatorio = ReportRequest.WithType(ReportType.PlanejamentoPedido)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("dataSet", dataSet.ToJson())
                .CallReport()
                .GetContentFile();

            string pastaRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, $"Pedidos_{DateTime.Now.ToString("dd-MM-yyyy_h-mm")}.pdf");

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(nomeArquivo, relatorio);

            return nomeArquivo;
        }

        #endregion
    }
}
