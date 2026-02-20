using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Repositorio.Embarcador.Patrimonio;
using QRCoder.Extensions;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.Relatorios.Embarcador.DataSource.PedidosVendas;

namespace SGT.WebAdmin.Controllers.PedidosVendas
{
    [CustomAuthorize("PedidosVendas/OrdemServicoPet")]
    public class OrdemServicoPetController : BaseController
    {
        #region Construtores

        public OrdemServicoPetController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pet", "Pet", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cor", "Cor", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Raça", "Raca", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data de chegada", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data de entrega", "DataEntrega", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tutor", "Tutor", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ValorTotal", false);

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> listaPedidoVenda = repPedidoVenda.Consulta(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repPedidoVenda.ContaConsulta(filtrosPesquisa));

                var lista = (from p in listaPedidoVenda
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 Pet = p.Pet != null ? p.Pet?.Nome : string.Empty,
                                 Cor = p.Pet != null && p.Pet?.Cor != null ? p.Pet?.Cor?.Descricao : string.Empty,
                                 Raca = p.Pet != null && p.Pet?.Raca != null ? p.Pet?.Raca?.Descricao : string.Empty,
                                 DataEmissao = p.DataEmissao.Value.ToDateTimeString(),
                                 DataEntrega = p.DataEntrega.Value.ToDateTimeString(),
                                 DescricaoStatus = p.StatusOrdemServicoPet.ObterDescricao(),
                                 Tutor = p.Pet != null && p.Pet?.Tutor != null ? p.Pet?.Tutor?.Descricao : string.Empty,
                                 ValorTotal = p.ValorTotal.ToString("n2"),
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
                int funcionario = Request.GetIntParam("Funcionario");
                int petCodigo = Request.GetIntParam("Pet");
                double pessoa = Request.GetDoubleParam("Pessoa");
                decimal peso = Request.GetDecimalParam("Peso");
                decimal valorProdutos = Request.GetDecimalParam("ValorProdutos");
                decimal valorServicos = Request.GetDecimalParam("ValorServicos");
                decimal valorTotal = Request.GetDecimalParam("ValorTotal");

                DateTime dataEntrega = Request.GetDateTimeParam("DataEntrega");
                StatusOrdemServicoPet statusOrdemServicoPet = Request.GetEnumParam<StatusOrdemServicoPet>("Status");

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);
                PetRepositorio petRepositorio = new(unitOfWork);

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda;
                if (codigo > 0)
                    pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigo, true);
                else
                {
                    pedidoVenda = new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda();
                    pedidoVenda.Numero = repPedidoVenda.BuscarUltimoNumero(this.Usuario.Empresa.Codigo) + 1;
                    pedidoVenda.Empresa = this.Usuario.Empresa;
                }

                if (dataEntrega.Date != DateTime.Now.Date && statusOrdemServicoPet == StatusOrdemServicoPet.Finalizado && pedidoVenda.Empresa.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual)
                    throw new ControllerException("Só é permitido finalizar o Pedido se a data de entrega for a data atual");

                pedidoVenda.StatusOrdemServicoPet = statusOrdemServicoPet;

                pedidoVenda.DataEmissao = Request.GetDateTimeParam("DataEmissao");
                pedidoVenda.DataEntrega = dataEntrega;
                pedidoVenda.Tipo = TipoPedidoVenda.OrdemServicoPet;

                pedidoVenda.Observacao = Request.GetStringParam("Observacao");
                pedidoVenda.Referencia = Request.GetStringParam("Referencia");
                pedidoVenda.ValorProdutos = valorProdutos;
                pedidoVenda.ValorServicos = valorServicos;
                pedidoVenda.ValorTotal = valorTotal;

                pedidoVenda.Cliente = repCliente.BuscarPorCPFCNPJ(pessoa);
                pedidoVenda.Funcionario = repUsuario.BuscarPorCodigo(funcionario);
                pedidoVenda.Pet = petCodigo > 0 ? petRepositorio.BuscarPorCodigo(petCodigo, false) : null;
                pedidoVenda.Peso = peso;

                if (codigo > 0)
                    repPedidoVenda.Atualizar(pedidoVenda, Auditado);
                else
                    repPedidoVenda.Inserir(pedidoVenda, Auditado);

                SalvarListaItens(pedidoVenda, unitOfWork);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    pedidoVenda.Codigo,
                    pedidoVenda.Status
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigo);

                if (pedidoVenda == null)
                    return new JsonpResult(false, true, "Ordem de Serviço do Pet não encontrado.");

                object retorno = new
                {
                    pedidoVenda.Codigo,
                    Numero = pedidoVenda.Numero.ToString("n0"),
                    pedidoVenda.Tipo,
                    Status = pedidoVenda.StatusOrdemServicoPet,
                    Pessoa = pedidoVenda.Cliente != null
                        ? new { Codigo = pedidoVenda.Cliente.CPF_CNPJ, Descricao = pedidoVenda.Cliente.Nome }
                        : null,
                    Funcionario = pedidoVenda.Funcionario != null
                        ? new { Codigo = pedidoVenda.Funcionario.Codigo, Descricao = pedidoVenda.Funcionario.Nome }
                        : null,
                    PetCodigo = pedidoVenda.Pet != null ? pedidoVenda.Pet?.Codigo : 0,
                    Peso = pedidoVenda.Peso,
                    DataEmissao = pedidoVenda.DataEmissao.Value.ToDateTimeString(),
                    DataEntrega = pedidoVenda.DataEntrega.Value.ToDateTimeString(),
                    pedidoVenda.Observacao,
                    pedidoVenda.Referencia,
                    pedidoVenda.ValorProdutos,
                    pedidoVenda.ValorServicos,
                    pedidoVenda.ValorTotal,
                    ListaItens = pedidoVenda.Itens != null
                        ? (from obj in pedidoVenda.Itens
                           select new
                           {
                               Codigo = obj.Codigo,
                               Produto = obj.Produto != null ? obj.Produto?.Codigo : 0,
                               Servico = obj.Servico != null ? obj.Servico?.Codigo : 0,
                               CodigoPedidoVenda = obj.PedidoVenda.Codigo,
                               obj.CodigoItem,
                               obj.DescricaoItem,
                               obj.NumeroItemOrdemCompra,
                               obj.NumeroOrdemCompra,
                               Quantidade = obj.Quantidade.ToString("n" + pedidoVenda.Empresa.CasasQuantidadeProdutoNFe.ToString()),
                               ValorUnitario = obj.ValorUnitario.ToString("n" + pedidoVenda.Empresa.CasasValorProdutoNFe.ToString()),
                               ValorTotalItem = obj.ValorTotal.ToString("n2"),
                               CodigoNCM = obj.Produto != null ? obj.Produto?.CodigoNCM : string.Empty,
                               Observacao = obj.Observacao ?? string.Empty
                           }).ToList()
                        : null
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
                int codigo = Request.GetIntParam("Codigo");
                string nomeEmpresa = Cliente.NomeFantasia;
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio relatorioRepositorio = new(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda pedidoVendaRepositorio = new(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio relatorioServico = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = relatorioRepositorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R340_OrdemServicoPet, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = relatorioServico.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R340_OrdemServicoPet, TipoServicoMultisoftware, "Relatorio de Ordem de Serviço de Pet", "PedidoVenda", "OrdemServicoPet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = relatorioServico.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                // Dados que vão para o relatório (tipo um DTO)
                IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet> dadosPedidoVenda = pedidoVendaRepositorio.RelatorioOrdemServicoPet(codigo);

                if (dadosPedidoVenda.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorio(codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPedidoVenda));
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
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFuncionarioLogado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                object retorno = null;

                retorno = new
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarListaItens(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repItensPedidoVenda = new(unidadeDeTrabalho);

            repItensPedidoVenda.DeletarPorPedido(pedidoVenda.Codigo);

            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var itemPedido in listaItens)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens itensPedidoVenda = new();
                    itensPedidoVenda.CodigoItem = (string)itemPedido.CodigoItem;
                    itensPedidoVenda.DescricaoItem = (string)itemPedido.DescricaoItem;
                    itensPedidoVenda.NumeroOrdemCompra = (string)itemPedido.NumeroOrdemCompra;
                    itensPedidoVenda.NumeroItemOrdemCompra = (string)itemPedido.NumeroItemOrdemCompra;
                    itensPedidoVenda.Quantidade = Utilidades.Decimal.Converter((string)itemPedido.Quantidade);
                    itensPedidoVenda.ValorUnitario = Utilidades.Decimal.Converter((string)itemPedido.ValorUnitario);
                    itensPedidoVenda.ValorTotal = Utilidades.Decimal.Converter((string)itemPedido.ValorTotalItem);
                    itensPedidoVenda.Observacao = (string)itemPedido.Observacao;

                    if (itemPedido.Produto != null && (int)itemPedido.Produto > 0)
                    {
                        itensPedidoVenda.Produto = new Dominio.Entidades.Produto()
                        {
                            Codigo = (int)itemPedido.Produto
                        };
                    }

                    if (itemPedido.Servico != null && (int)itemPedido.Servico > 0)
                    {
                        itensPedidoVenda.Servico = new Dominio.Entidades.Embarcador.NotaFiscal.Servico()
                        {
                            Codigo = (int)itemPedido.Servico
                        };
                    }

                    itensPedidoVenda.PedidoVenda = pedidoVenda;

                    repItensPedidoVenda.Inserir(itensPedidoVenda);
                }
            }
        }

        private void GerarRelatorio(int codigoOrdemServicoPet, string nomeEmpresa, string stringConexao, RelatorioControleGeracao relatorioControleGeracao, IList<RelatorioOrdemServicoPet> dadosOrdemServicoPet)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.PedidosVendas.PedidosVendas serPedidoVenda = new Servicos.Embarcador.PedidosVendas.PedidosVendas(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ProdutoFoto repProdutoFoto = new Repositorio.ProdutoFoto(unitOfWork);

            try
            {
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);
                string mensagem = "";

                var report = ReportRequest.WithType(ReportType.OrdemServicoPet)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoOrdemServicoPet", codigoOrdemServicoPet)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("dadosOrdemServicoPet", dadosOrdemServicoPet.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CaminhoLogoDacte", empresaRelatorio.CaminhoLogoDacte)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(report.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, report.ErrorMessage);
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

        private Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa =
                new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda()
                {
                    CodigoFuncionario = Request.GetIntParam("Funcionario"),
                    CodigoPet = Request.GetIntParam("Pet"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    NumeroInicial = Request.GetIntParam("NumeroInicial"),
                    NumeroFinal = Request.GetIntParam("NumeroFinal"),
                    CnpjCpfCliente = Request.GetDoubleParam("Pessoa"),
                    DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                    DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                    DataEntregaInicial = Request.GetDateTimeParam("DataEntregaInicial"),
                    DataEntregaFinal = Request.GetDateTimeParam("DataEntregaFinal"),
                    StatusPedidoVenda = Request.GetNullableEnumParam<StatusPedidoVenda>("Status"),
                    TipoPedidoVenda = Request.GetEnumParam<TipoPedidoVenda>("Tipo"),
                    CodigoEmpresa = Empresa.Codigo
                };

            if (filtrosPesquisa.StatusPedidoVenda == StatusPedidoVenda.AbertaFinalizada &&
                Usuario.Empresa.PermitirImportarApenasPedidoVendaFinalizado)
                filtrosPesquisa.StatusPedidoVenda = StatusPedidoVenda.Finalizada;

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("DescricaoTipo"))
                return "Tipo";
            else if (propriedadeOrdenar.Equals("DescricaoStatus"))
                return "Status";

            return propriedadeOrdenar;
        }

        private decimal RetornarValorTotalProdutos(List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda> itens)
        {
            decimal valorTotal = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda item in itens)
                valorTotal = valorTotal + (item.Quantidade * item.ValorUnitario);

            return valorTotal;
        }

        #endregion
    }
}
