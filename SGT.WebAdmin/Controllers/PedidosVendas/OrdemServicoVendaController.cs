using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.PedidosVendas
{
    [CustomAuthorize("PedidosVendas/OrdemServicoVenda")]
    public class OrdemServicoVendaController : BaseController
    {
		#region Construtores

		public OrdemServicoVendaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                if (Empresa.HabilitarNumeroInternoOrdemServicoVenda)
                    grid.AdicionarCabecalho("Número Interno", "NumeroInterno", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pessoa", "Cliente", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 20, Models.Grid.Align.left, true);
                if (!filtrosPesquisa.StatusPedidoVenda.HasValue)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorTotal", 15, Models.Grid.Align.right, true);

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> listaPedidoVenda = repPedidoVenda.Consulta(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repPedidoVenda.ContaConsulta(filtrosPesquisa));

                var lista = (from p in listaPedidoVenda
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 p.NumeroInterno,
                                 DataEmissao = p.DataEmissao.Value.ToDateTimeString(),
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 Veiculo = p.Veiculo?.Placa ?? string.Empty,
                                 p.DescricaoStatus,
                                 ValorTotal = p.ValorTotal.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                int codigoFuncionario = Request.GetIntParam("Funcionario");
                int codigoFuncionarioSolicitante = Request.GetIntParam("FuncionarioSolicitante");
                string pessoaSolicitante = Request.GetStringParam("PessoaSolicitante");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoEmpresa = Request.GetIntParam("Empresa");

                double pessoa = Request.GetDoubleParam("Pessoa");

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda;
                bool alterouEmpresa = false;
                if (codigo > 0)
                {
                    pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigo, true);
                    if (pedidoVenda.Empresa.Codigo != codigoEmpresa)
                    {
                        pedidoVenda.Numero = repPedidoVenda.BuscarUltimoNumero(codigoEmpresa) + 1;
                        pedidoVenda.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                        alterouEmpresa = true;
                    }
                }
                else
                {
                    pedidoVenda = new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda();
                    pedidoVenda.Numero = repPedidoVenda.BuscarUltimoNumero(this.Usuario.Empresa.Codigo) + 1;
                    pedidoVenda.Empresa = this.Usuario.Empresa;
                }

                pedidoVenda.NumeroInterno = Request.GetIntParam("NumeroInterno");
                pedidoVenda.DataEmissao = Request.GetDateTimeParam("DataEmissao");
                pedidoVenda.DataEntrega = Request.GetDateTimeParam("DataEntrega");
                pedidoVenda.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVenda.OrdemServico;
                pedidoVenda.Status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda>("Status");
                pedidoVenda.Observacao = Request.GetStringParam("Observacao");
                pedidoVenda.Referencia = Request.GetStringParam("Referencia");
                pedidoVenda.ValorProdutos = Request.GetDecimalParam("ValorProdutos");
                pedidoVenda.ValorServicos = Request.GetDecimalParam("ValorServicos");
                pedidoVenda.ValorTotal = Request.GetDecimalParam("ValorTotal");
                pedidoVenda.ValorDesconto = Request.GetDecimalParam("ValorDesconto");
                pedidoVenda.PercentualDesconto = Request.GetDecimalParam("PercentualDesconto");
                pedidoVenda.KM = Request.GetIntParam("KM");

                pedidoVenda.Cliente = repCliente.BuscarPorCPFCNPJ(pessoa);

                Dominio.Entidades.Usuario funcionario = repUsuario.BuscarPorCodigo(codigoFuncionario);
                Dominio.Entidades.Usuario funcionarioSolicitante = repUsuario.BuscarPorCodigo(codigoFuncionarioSolicitante);
                Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                if (alterouEmpresa)
                {
                    Dominio.Entidades.Usuario funcionarioNovaEmpresa = repUsuario.BuscarPorCPF(pedidoVenda.Empresa.Codigo, funcionario.CPF, funcionario.Tipo);
                    if (funcionarioNovaEmpresa == null)
                        throw new ControllerException($"O funcionário {funcionario.Descricao} não existe na empresa que está tentando alterar, favor verificar antes de prosseguir.");
                    funcionario = funcionarioNovaEmpresa;

                    if (veiculo != null)
                    {
                        Dominio.Entidades.Veiculo veiculoNovaEmpresa = repVeiculo.BuscarPorPlaca(pedidoVenda.Empresa.Codigo, veiculo.Placa);
                        if (veiculoNovaEmpresa == null)
                            throw new ControllerException($"O veículo {veiculo.Placa_Formatada} não existe na empresa que está tentando alterar, favor verificar antes de prosseguir.");
                        veiculo = veiculoNovaEmpresa;
                    }
                }

                pedidoVenda.Funcionario = funcionario;
                pedidoVenda.Veiculo = veiculo;
                pedidoVenda.FuncionarioSolicitante = funcionarioSolicitante;
                pedidoVenda.PessoaSolicitante = pessoaSolicitante;

                if (codigo > 0)
                    repPedidoVenda.Atualizar(pedidoVenda, Auditado);
                else
                    repPedidoVenda.Inserir(pedidoVenda, Auditado);

                SalvarListaItens(pedidoVenda, alterouEmpresa, unitOfWork);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    pedidoVenda.Codigo,
                    pedidoVenda.Status,
                    RecarregarOrdem = !alterouEmpresa
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Salvar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigo);

                if (pedidoVenda == null)
                    return new JsonpResult(false, true, "Ordem de Serviço não encontrada.");

                object retorno = new
                {
                    pedidoVenda.Codigo,
                    Numero = pedidoVenda.Numero.ToString("n0"),
                    pedidoVenda.NumeroInterno,
                    pedidoVenda.Tipo,
                    pedidoVenda.Status,
                    Pessoa = pedidoVenda.Cliente != null ? new { Codigo = pedidoVenda.Cliente.CPF_CNPJ, Descricao = pedidoVenda.Cliente.Nome } : null,
                    Funcionario = pedidoVenda.Funcionario != null ? new { Codigo = pedidoVenda.Funcionario.Codigo, Descricao = pedidoVenda.Funcionario.Nome } : null,
                    FuncionarioSolicitante = pedidoVenda.FuncionarioSolicitante != null ? new { Codigo = pedidoVenda.FuncionarioSolicitante.Codigo, Descricao = pedidoVenda.FuncionarioSolicitante.Nome } : null,
                    PessoaSolicitante = pedidoVenda.PessoaSolicitante,
                    Veiculo = pedidoVenda.Veiculo != null ? new { Codigo = pedidoVenda.Veiculo.Codigo, Descricao = pedidoVenda.Veiculo.DescricaoComMarcaModelo } : null,
                    Empresa = pedidoVenda.Empresa != null ? new { Codigo = pedidoVenda.Empresa.Codigo, Descricao = pedidoVenda.Empresa.RazaoSocial } : null,
                    DataEmissao = pedidoVenda.DataEmissao.Value.ToDateTimeString(),
                    DataEntrega = pedidoVenda.DataEntrega.Value.ToDateTimeString(),
                    pedidoVenda.Observacao,
                    pedidoVenda.ValorProdutos,
                    pedidoVenda.ValorServicos,
                    pedidoVenda.ValorTotal,
                    pedidoVenda.ValorDesconto,
                    pedidoVenda.PercentualDesconto,
                    pedidoVenda.KM,
                    pedidoVenda.Referencia,
                    ListaItens = pedidoVenda.Itens != null ? (from obj in pedidoVenda.Itens
                                                              where obj.Produto != null
                                                              select new
                                                              {
                                                                  Codigo = obj.Codigo,
                                                                  Produto = obj.Produto != null ? obj.Produto.Codigo : 0,
                                                                  CodigoPedidoVenda = obj.PedidoVenda.Codigo,
                                                                  obj.CodigoItem,
                                                                  obj.DescricaoItem,
                                                                  Quantidade = obj.Quantidade.ToString("n2"),
                                                                  ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                                                  ValorTotalItem = obj.ValorTotal.ToString("n2"),
                                                                  ValorDesconto = obj.ValorDesconto.ToString("n2")
                                                              }).ToList() : null,
                    ListaMaoObras = pedidoVenda.Itens != null ? (from obj in pedidoVenda.Itens
                                                                 where obj.Servico != null
                                                                 select new
                                                                 {
                                                                     Codigo = obj.Codigo,
                                                                     Servico = obj.Servico != null ? obj.Servico.Codigo : 0,
                                                                     CodigoPedidoVenda = obj.PedidoVenda.Codigo,
                                                                     obj.CodigoItem,
                                                                     obj.DescricaoItem,
                                                                     Quantidade = obj.Quantidade.ToString("n2"),
                                                                     ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                                                     ValorTotalItem = obj.ValorTotal.ToString("n2"),
                                                                     ValorDesconto = obj.ValorDesconto.ToString("n2"),
                                                                     obj.TipoServico,
                                                                     Funcionario = obj.Funcionario != null ? obj.Funcionario.Codigo : 0,
                                                                     NomeFuncionario = obj.Funcionario != null ? obj.Funcionario.Nome : string.Empty,
                                                                     FuncionarioAuxiliar = obj.FuncionarioAuxiliar != null ? obj.FuncionarioAuxiliar.Codigo : 0,
                                                                     NomeFuncionarioAuxiliar = obj.FuncionarioAuxiliar != null ? obj.FuncionarioAuxiliar.Nome : string.Empty,
                                                                     Pessoa = obj.Cliente != null ? obj.Cliente.Codigo : 0,
                                                                     NomePessoa = obj.Cliente != null ? obj.Cliente.Nome : string.Empty,
                                                                     TipoOrdemServicoVenda = obj.TipoOrdemServicoVenda?.ToString("d") ?? string.Empty,
                                                                     KMInicial = obj.KMInicial > 0 ? obj.KMInicial.ToString("n0") : string.Empty,
                                                                     KMFinal = obj.KMFinal > 0 ? obj.KMFinal.ToString("n0") : string.Empty,
                                                                     KMTotal = obj.KMTotal > 0 ? obj.KMTotal.ToString("n0") : string.Empty,
                                                                     KMTotalUnidade = obj.KMTotal > 0 ? obj.KMTotal.ToString("n0") : string.Empty,
                                                                     HoraInicial = obj.HoraInicial.HasValue ? obj.HoraInicial.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     HoraFinal = obj.HoraFinal.HasValue ? obj.HoraFinal.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     HoraTotal = obj.HoraTotal.HasValue ? obj.HoraTotal.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     HoraTotalUnidade = obj.HoraTotal.HasValue ? obj.HoraTotal.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     ValorKM = obj.ValorKM.ToString("n2"),
                                                                     ValorTotalKM = obj.ValorTotalKM.ToString("n2"),
                                                                     ValorHora = obj.ValorHora.ToString("n2"),
                                                                     ValorTotalHora = obj.ValorTotalHora.ToString("n2"),
                                                                     HoraInicial2 = obj.HoraInicial2.HasValue ? obj.HoraInicial2.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     HoraFinal2 = obj.HoraFinal2.HasValue ? obj.HoraFinal2.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     HoraTotal2 = obj.HoraTotal2.HasValue ? obj.HoraTotal2.Value.ToString(@"hh\:mm") : string.Empty,
                                                                     KMInicial2 = obj.KMInicial > 0 ? obj.KMInicial2.ToString("n0") : string.Empty,
                                                                     KMFinal2 = obj.KMFinal2 > 0 ? obj.KMFinal2.ToString("n0") : string.Empty,
                                                                     KMTotal2 = obj.KMTotal2 > 0 ? obj.KMTotal2.ToString("n0") : string.Empty

                                                                 }).ToList() : null
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R061_OrdemServicoVenda, TipoServicoMultisoftware);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R061_OrdemServicoVenda, TipoServicoMultisoftware, "Relatório de Ordem de Serviço", "OrdemServicoVenda", "OrdemServicoVenda.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                int totalRegistros = repPedidoVenda.ContarRelatorioOrdemServicoVenda(codigo);
                if (totalRegistros > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioPedidoVenda(codigo, nomeCliente, stringConexao, relatorioControleGeracao));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de ordem de serviço para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R061_OrdemServicoVenda, TipoServicoMultisoftware);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R061_OrdemServicoVenda, TipoServicoMultisoftware, "Relatório de Ordem de Serviço", "OrdemServicoVenda", "OrdemServicoVenda.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                int totalRegistros = repPedidoVenda.ContarRelatorioOrdemServicoVenda(codigo);
                if (totalRegistros > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioPedidoVenda(codigo, nomeCliente, stringConexao, relatorioControleGeracao));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de pedido de vendas para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar por e-mail. " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFuncionarioLogado()
        {
            try
            {
                object retorno = new
                {
                    Usuario.Codigo,
                    Usuario.Nome
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o Funcionário Logado.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracoesEmpresa()
        {
            try
            {
                object retorno = new
                {
                    HabilitarTabelaValorOrdemServicoVenda = Empresa?.HabilitarTabelaValorOrdemServicoVenda ?? false,
                    PermiteAlterarEmpresaOrdemServicoVenda = Empresa?.PermiteAlterarEmpresaOrdemServicoVenda ?? false,
                    HabilitarNumeroInternoOrdemServicoVenda = Empresa?.HabilitarNumeroInternoOrdemServicoVenda ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as configurações da empresa.");
            }
        }

        public async Task<IActionResult> VerificaClienteTemEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(this.Usuario.Empresa.Codigo);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");
                else
                {
                    double pessoa;
                    double.TryParse(Request.Params("Pessoa"), out pessoa);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(pessoa);

                    if (cliente != null)
                    {
                        List<string> emails = new List<string>();
                        if (!string.IsNullOrWhiteSpace(cliente.Email))
                            emails.Add(cliente.Email);

                        for (int a = 0; a < cliente.Emails.Count; a++)
                        {
                            if (!string.IsNullOrWhiteSpace(cliente.Emails[a].Email) && cliente.Emails[a].EmailStatus == "A")
                                emails.Add(cliente.Emails[a].Email);
                        }

                        if (emails.Count > 0)
                            return new JsonpResult(true);
                        else
                            return new JsonpResult(false, "Não há e-mail cadastrado no cliente.");
                    }
                    else
                        return new JsonpResult(false, "Cliente não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar se o Cliente possui e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarListaItens(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda, bool alterouEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repItensPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unidadeDeTrabalho);

            repItensPedidoVenda.DeletarPorPedido(pedidoVenda.Codigo);

            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var itemPedido in listaItens)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens itensPedidoVenda = new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens();
                    itensPedidoVenda.CodigoItem = (string)itemPedido.CodigoItem;
                    itensPedidoVenda.DescricaoItem = (string)itemPedido.DescricaoItem;

                    itensPedidoVenda.Quantidade = Utilidades.Decimal.Converter((string)itemPedido.Quantidade);
                    itensPedidoVenda.ValorUnitario = Utilidades.Decimal.Converter((string)itemPedido.ValorUnitario);
                    itensPedidoVenda.ValorTotal = Utilidades.Decimal.Converter((string)itemPedido.ValorTotalItem);
                    itensPedidoVenda.ValorDesconto = Utilidades.Decimal.Converter((string)itemPedido.ValorDesconto);

                    Dominio.Entidades.Produto produto = null;
                    if ((int)itemPedido.Produto > 0)
                        produto = repProduto.BuscarPorCodigo((int)itemPedido.Produto);
                    if (alterouEmpresa && produto != null)
                    {
                        if (string.IsNullOrWhiteSpace(produto.CodigoProduto))
                            throw new ControllerException($"O produto {produto.Descricao} não possui Cód. Produto configurado, favor configurar no cadastro para prosseguir.");

                        Dominio.Entidades.Produto produtoNovaEmpresa = repProduto.BuscarPorCodigoIntegracao(pedidoVenda.Empresa.Codigo, produto.CodigoProduto);
                        if (produtoNovaEmpresa == null)
                            throw new ControllerException($"O produto {produto.Descricao} de Cód. Produto {produto.CodigoProduto} não existe na empresa que está tentando alterar, favor verificar antes de prosseguir.");

                        produto = produtoNovaEmpresa;
                    }

                    itensPedidoVenda.Produto = produto;
                    itensPedidoVenda.PedidoVenda = pedidoVenda;
                    repItensPedidoVenda.Inserir(itensPedidoVenda);
                }
            }

            dynamic listaMaoObras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaMaoObras"));
            if (listaMaoObras != null)
            {
                foreach (var maoObraPedido in listaMaoObras)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens itensPedidoVenda = new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens();
                    itensPedidoVenda.CodigoItem = (string)maoObraPedido.CodigoItem;
                    itensPedidoVenda.DescricaoItem = (string)maoObraPedido.DescricaoItem;

                    itensPedidoVenda.KMInicial = Utilidades.String.OnlyNumbers((string)maoObraPedido.KMInicial).ToInt();
                    itensPedidoVenda.KMFinal = Utilidades.String.OnlyNumbers((string)maoObraPedido.KMFinal).ToInt();
                    itensPedidoVenda.KMTotal = Utilidades.String.OnlyNumbers((string)maoObraPedido.KMTotal).ToInt();

                    itensPedidoVenda.KMInicial2 = Utilidades.String.OnlyNumbers((string)maoObraPedido.KMInicial2).ToInt();
                    itensPedidoVenda.KMFinal2 = Utilidades.String.OnlyNumbers((string)maoObraPedido.KMFinal2).ToInt();
                    itensPedidoVenda.KMTotal2 = Utilidades.String.OnlyNumbers((string)maoObraPedido.KMTotal2).ToInt();

                    itensPedidoVenda.Quantidade = Utilidades.Decimal.Converter((string)maoObraPedido.Quantidade);
                    itensPedidoVenda.ValorUnitario = Utilidades.Decimal.Converter((string)maoObraPedido.ValorUnitario);
                    itensPedidoVenda.ValorTotal = Utilidades.Decimal.Converter((string)maoObraPedido.ValorTotalItem);
                    itensPedidoVenda.ValorDesconto = Utilidades.Decimal.Converter((string)maoObraPedido.ValorDesconto);

                    itensPedidoVenda.ValorKM = Utilidades.Decimal.Converter((string)maoObraPedido.ValorKM);
                    itensPedidoVenda.ValorTotalKM = Utilidades.Decimal.Converter((string)maoObraPedido.ValorTotalKM);
                    itensPedidoVenda.ValorHora = Utilidades.Decimal.Converter((string)maoObraPedido.ValorHora);
                    itensPedidoVenda.ValorTotalHora = Utilidades.Decimal.Converter((string)maoObraPedido.ValorTotalHora);

                    Enum.TryParse((string)maoObraPedido.TipoServico, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServico tipoServico);
                    itensPedidoVenda.TipoServico = tipoServico;

                    TimeSpan horaInicial = TimeSpan.MinValue, horaFinal = TimeSpan.MinValue, horaTotal = TimeSpan.MinValue;
                    if (!string.IsNullOrWhiteSpace((string)maoObraPedido.HoraInicial))
                        TimeSpan.TryParse((string)maoObraPedido.HoraInicial, out horaInicial);
                    if (!string.IsNullOrWhiteSpace((string)maoObraPedido.HoraFinal))
                        TimeSpan.TryParse((string)maoObraPedido.HoraFinal, out horaFinal);
                    if (!string.IsNullOrWhiteSpace((string)maoObraPedido.HoraTotal))
                        TimeSpan.TryParse((string)maoObraPedido.HoraTotal, out horaTotal);
                    if (horaInicial > TimeSpan.MinValue)
                        itensPedidoVenda.HoraInicial = horaInicial;
                    else
                        itensPedidoVenda.HoraInicial = null;
                    if (horaFinal > TimeSpan.MinValue)
                        itensPedidoVenda.HoraFinal = horaFinal;
                    else
                        itensPedidoVenda.HoraFinal = null;
                    if (horaTotal > TimeSpan.MinValue)
                        itensPedidoVenda.HoraTotal = horaTotal;
                    else
                        itensPedidoVenda.HoraTotal = null;

                    TimeSpan horaInicial2 = TimeSpan.MinValue, horaFinal2 = TimeSpan.MinValue, horaTotal2 = TimeSpan.MinValue;
                    if (!string.IsNullOrWhiteSpace((string)maoObraPedido.HoraInicial2))
                        TimeSpan.TryParse((string)maoObraPedido.HoraInicial2, out horaInicial2);
                    if (!string.IsNullOrWhiteSpace((string)maoObraPedido.HoraFinal2))
                        TimeSpan.TryParse((string)maoObraPedido.HoraFinal2, out horaFinal2);
                    if (!string.IsNullOrWhiteSpace((string)maoObraPedido.HoraTotal2))
                        TimeSpan.TryParse((string)maoObraPedido.HoraTotal2, out horaTotal2);
                    if (horaInicial2 > TimeSpan.MinValue)
                        itensPedidoVenda.HoraInicial2 = horaInicial2;
                    else
                        itensPedidoVenda.HoraInicial2 = null;
                    if (horaFinal2 > TimeSpan.MinValue)
                        itensPedidoVenda.HoraFinal2 = horaFinal2;
                    else
                        itensPedidoVenda.HoraFinal2 = null;
                    if (horaTotal2 > TimeSpan.MinValue)
                        itensPedidoVenda.HoraTotal2 = horaTotal2;
                    else
                        itensPedidoVenda.HoraTotal2 = null;

                    Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = null;
                    if (maoObraPedido.Servico != null && (int)maoObraPedido.Servico > 0)
                        servico = repServico.BuscarPorCodigo((int)maoObraPedido.Servico);

                    if (alterouEmpresa && servico != null)
                    {
                        if (string.IsNullOrWhiteSpace(servico.CodigoIntegracao))
                            throw new ControllerException($"O serviço {servico.Descricao} não possui Código Integração configurado, favor configurar no cadastro para prosseguir.");

                        Dominio.Entidades.Embarcador.NotaFiscal.Servico servicoNovaEmpresa = repServico.BuscarPorCodigoIntegracao(servico.CodigoIntegracao, pedidoVenda.Empresa.Codigo);
                        if (servicoNovaEmpresa == null)
                            throw new ControllerException($"O serviço {servico.Descricao} de Código Integração {servico.CodigoIntegracao} não existe na empresa que está tentando alterar, favor verificar antes de prosseguir.");

                        servico = servicoNovaEmpresa;
                    }

                    itensPedidoVenda.Servico = servico;
                    itensPedidoVenda.TipoOrdemServicoVenda = ((string)maoObraPedido.TipoOrdemServicoVenda).ToNullableEnum<TipoOrdemServicoVenda>();

                    if (maoObraPedido.Funcionario != null)
                        if ((int)maoObraPedido.Funcionario > 0)
                            itensPedidoVenda.Funcionario = new Dominio.Entidades.Usuario() { Codigo = (int)maoObraPedido.Funcionario };

                    if (maoObraPedido.Pessoa != null)
                        if ((double)maoObraPedido.Pessoa > 0)
                            itensPedidoVenda.Cliente = new Dominio.Entidades.Cliente() { CPF_CNPJ = (double)maoObraPedido.Pessoa };

                    if ((int)maoObraPedido.FuncionarioAuxiliar > 0)
                        itensPedidoVenda.FuncionarioAuxiliar = new Dominio.Entidades.Usuario() { Codigo = (int)maoObraPedido.FuncionarioAuxiliar };

                    itensPedidoVenda.PedidoVenda = pedidoVenda;

                    repItensPedidoVenda.Inserir(itensPedidoVenda);
                }
            }
        }

        private void GerarRelatorioPedidoVenda(int codigoPedidoVenda, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, bool enviarEmail = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            try
            {
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);
                var report = ReportRequest.WithType(ReportType.OrdemServicoVenda)
                     .WithExecutionType(ExecutionType.Sync)
                     .AddExtraData("codigoPedidoVenda", codigoPedidoVenda)
                     .AddExtraData("nomeEmpresa", nomeEmpresa)
                     .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                     .AddExtraData("CaminhoLogoDacte", empresaRelatorio.CaminhoLogoDacte)
                     .CallReport();

                if (!string.IsNullOrWhiteSpace(report.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, report.ErrorMessage);
                else if (enviarEmail)
                    EnviarEmailPedido(unitOfWork, codigoPedidoVenda, report.FullPath);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void EnviarEmailPedido(Repositorio.UnitOfWork unitOfWork, int codigoPedido, string caminhoRelatorio)
        {
            Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;
            Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa.Codigo);

            if (email == null)
                throw new Exception("Não há um e-mail configurado para realizar o envio.");

            Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedido = repPedidoVenda.BuscarPorCodigo(codigoPedido);
            if (pedido != null)
            {
                string assunto = "Ordem de Serviço " + pedido.Empresa.NomeFantasia;
                string mensagemEmail = "Olá,<br/><br/>Segue em anexo a Ordem de Serviço da Empresa: " + pedido.Empresa.NomeFantasia + ".<br/><br/>";
                mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

                if (!string.IsNullOrWhiteSpace(caminhoRelatorio) && Utilidades.IO.FileStorageService.Storage.Exists(caminhoRelatorio))
                {
                    List<string> emails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(pedido.Cliente.Email))
                        emails.AddRange(pedido.Cliente.Email.Split(';').ToList());

                    for (int a = 0; a < pedido.Cliente.Emails.Count; a++)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = pedido.Cliente.Emails[a];
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                            emails.Add(outroEmail.Email);
                    }

                    if (!string.IsNullOrWhiteSpace(empresa.Email) && empresa.StatusEmail == "A")
                        emails.AddRange(empresa.Email.Split(';').ToList());

                    if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) && empresa.StatusEmailAdministrativo == "A")
                        emails.AddRange(empresa.EmailAdministrativo.Split(';').ToList());

                    emails = emails.Distinct().ToList();
                    if (emails.Count > 0)
                    {
                        byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoRelatorio);
                        bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, pedido.Cliente.Email, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdf), System.IO.Path.GetFileName(caminhoRelatorio), "application/pdf") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, empresa.Codigo);
                        if (!sucesso)
                            throw new Exception("Problemas ao enviar a ordem de serviço por e-mail: " + mensagemErro);
                    }
                    else
                        throw new Exception("Cliente da ordem de serviço não possui e-mail cadastrado.");
                }
                else
                    throw new Exception("Não foi possível localizar o arquivo PDF da ordem de serviço.");
            }
            else
                throw new Exception("Ordem de Serviço não localizado para enviar e-mail.");
        }

        private Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda()
            {
                CodigoFuncionario = Request.GetIntParam("Funcionario"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CnpjCpfCliente = Request.GetDoubleParam("Pessoa"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                StatusPedidoVenda = Request.GetNullableEnumParam<StatusPedidoVenda>("Status"),
                TipoPedidoVenda = TipoPedidoVenda.OrdemServico,
                CodigoEmpresa = Empresa.Codigo,
                NumeroInternoInicial = Request.GetIntParam("NumeroInternoInicial"),
                NumeroInternoFinal = Request.GetIntParam("NumeroInternoFinal")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("DescricaoTipo"))
                return "Tipo";
            else if (propriedadeOrdenar.Equals("DescricaoStatus"))
                return "Status";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
