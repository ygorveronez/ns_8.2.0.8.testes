using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Data;


namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/Pedido", "Cargas/PedidoProdutosCarregamentos", "Logistica/AgendamentoColeta")]
    public class PedidoProdutoController : BaseController
    {
        #region Construtores

        public PedidoProdutoController(Conexao conexao) : base(conexao) { }

        #endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoPedido = 0;
                int.TryParse(Request.Params("Pedido"), out codigoPedido);

                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Produto, "Produto", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoUnitario, "PesoUnitario", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Quantidade, "Quantidade", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoTotal, "PesoTotal", 10, Models.Grid.Align.right, true);


                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProduto = repPedidoProduto.Consultar(codigoPedido, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoProduto.ContarConsulta(codigoPedido));

                var lista = (from obj in pedidoProduto
                             select new
                             {
                                 obj.Codigo,
                                 Produto = obj.Produto.CodigoProdutoEmbarcador + " - " + obj.Produto.Descricao,
                                 Quantidade = obj.Quantidade.ToString("n3"),
                                 QuantidadePlanejada = obj.QuantidadePlanejada.ToString("n3"),
                                 PesoUnitario = obj.PesoUnitario.ToString("n3"),
                                 PesoTotalEmbalagem = obj.PesoTotalEmbalagem.ToString("n3"),
                                 PesoTotal = ((obj.Quantidade * obj.PesoUnitario) + obj.PesoTotalEmbalagem).ToString("n3")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Carregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoPedido = 0;
                int.TryParse(Request.Params("Pedido"), out codigoPedido);
                int codigoCarregamento = 0;
                int.TryParse(Request.Params("Carregamento"), out codigoCarregamento);

                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Produto, "Produto", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoUnitario, "PesoUnitario", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PalletFechado, "PalletFechado", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Quantidade, "Quantidade", 10, Models.Grid.Align.right, false).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt));
                grid.AdicionarCabecalho("Quantidade em Sessão Roteirização", "QuantidadeProdutoSessao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoTotal, "PesoTotal", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoCarregado, "PesoTotalCarregado", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoCarregar, "PesoTotalCarregar", 10, Models.Grid.Align.right, false).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9));
                grid.AdicionarCabecalho("QuantidadePallet", false);
                grid.AdicionarCabecalho("MetroCubico", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProduto = repPedidoProduto.Consultar(codigoPedido, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoProduto.ContarConsulta(codigoPedido));

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCpp = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProdutos = repCpp.BuscarPorPedido(codigoPedido);
                if (carregamentosPedidoProdutos == null)
                    carregamentosPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto>();

                bool temControlePorEstoque = pedidoProduto.Exists(pp => pp.FilialArmazem != null);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProdutos = null;
                if (temControlePorEstoque)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork);
                    sessaoRoteirizadorPedidoProdutos = repositorioSessaoRoteirizadorPedidoProduto.BuscarSessaoRoteirizadorPorPedido(codigoPedido);
                    List<int> codigosSessaoRoteirizadorPedidoProdutos = (from obj in sessaoRoteirizadorPedidoProdutos select obj.PedidoProduto.Codigo).ToList();
                    pedidoProduto = pedidoProduto.FindAll(pp => codigosSessaoRoteirizadorPedidoProdutos.Contains(pp.Codigo));
                }


                var lista = (from obj in pedidoProduto
                             select new
                             {
                                 obj.Codigo,
                                 Produto = obj.Produto.CodigoProdutoEmbarcador + " - " + obj.Produto.Descricao,
                                 Quantidade = (int)obj.Quantidade,
                                 QuantidadeProdutoSessao = temControlePorEstoque ? (int)sessaoRoteirizadorPedidoProdutos.Find(sr => sr.PedidoProduto.Codigo == obj.Codigo).QuantidadeProdutoSessao : 0,
                                 PesoUnitario = obj.PesoUnitario.ToString("n3"),
                                 PalletFechado = (obj.PalletFechado ? "SIM" : "NÃO"),
                                 PesoTotalEmbalagem = obj.PesoTotalEmbalagem.ToString("n2"),
                                 PesoTotal = ((obj.Quantidade * obj.PesoUnitario) + obj.PesoTotalEmbalagem).ToString("n2"),
                                 PesoTotalCarregado = PesoPedidoCarregamento(obj.Codigo, codigoCarregamento, IfSum.Diferente, carregamentosPedidoProdutos).ToString("n2"),
                                 PesoTotalCarregar = PesoPedidoCarregamento(obj.Codigo, codigoCarregamento, IfSum.Igual, carregamentosPedidoProdutos).ToString("n2"),
                                 QuantidadePallet = obj.QuantidadePalet,
                                 MetroCubico = obj.MetroCubico,
                                 DT_RowColor = PesoPedidoCarregamento(obj.Codigo, codigoCarregamento, IfSum.Diferente, carregamentosPedidoProdutos) >= obj.PesoTotal ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                 PesoPedidoCarregamento(obj.Codigo, codigoCarregamento, IfSum.Diferente, carregamentosPedidoProdutos) > 0 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : ""
                                 //DT_FontColor = PesoPedidoCarregamento(obj.Codigo, codigoCarregamento, IfSum.Ambos, carregamentosPedidoProdutos) >= obj.PesoTotal ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : ""
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesPedidoProdutosCarregamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = 0;
                int.TryParse(Request.Params("Pedido"), out codigoPedido);
                string numeroPedido = Request.Params("NumeroPedido");
                int codigoFilial = 0;
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

                // Utilizando na tela de consulta Cargas/PedidoProdutosCarregamentos.
                if (codigoPedido == 0 && !string.IsNullOrEmpty(numeroPedido))
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).BuscarPorNumeroEmbarcador(numeroPedido, codigoFilial, false);
                    codigoPedido = pedido?.Codigo ?? 0;
                }

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidosProdutos = repCarregamentoPedidoProduto.BuscarPorPedido(codigoPedido);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarPorPedido(codigoPedido);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPedidoProduto", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Produto, "Produto", 25, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Quantidade, "Quantidade", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Peso, "Peso", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.QtdPalet, "QtdePallet", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho("M³", "MetroCubico", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Sessao, "CodigoSessaoRoteirizador", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Carregamento, "CodigoCarregamento", 7, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Carga, "CodigoCargas", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PerfilVeiculo, "ModeloVeicular", 8, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Placa, "Placa", 5, Models.Grid.Align.left);
                grid.AdicionarCabecalho("NF.", "NotaFiscal", 5, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Dt. NF.", "DataNotaFiscal", 10, Models.Grid.Align.center);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                grid.setarQuantidadeTotal(carregamentosPedidosProdutos.Count);

                var lista = (from obj in carregamentosPedidosProdutos
                             select new
                             {
                                 obj.Codigo,
                                 CodigoPedidoProduto = obj.PedidoProduto.Codigo,
                                 Produto = obj.PedidoProduto.Produto.CodigoProdutoEmbarcador + " - " + obj.PedidoProduto.Produto.Descricao,
                                 Quantidade = obj.Quantidade.ToString("n3"),
                                 Peso = obj.Peso.ToString("n3"),
                                 QtdePallet = obj.QuantidadePallet.ToString("n3"),
                                 MetroCubico = obj.MetroCubico.ToString("n3"),
                                 CodigoSessaoRoteirizador = obj.CarregamentoPedido?.Carregamento?.SessaoRoteirizador?.Codigo.ToString() ?? "",
                                 CodigoCarregamento = obj.CarregamentoPedido?.Carregamento?.NumeroCarregamento ?? "",
                                 CodigoCargas = string.Join(", ", (from car in obj.CarregamentoPedido?.Carregamento?.Cargas
                                                                   where car != null
                                                                   select car.Codigo).ToArray()),
                                 ModeloVeicular = obj.CarregamentoPedido?.Carregamento?.ModeloVeicularCarga?.Descricao ?? "",
                                 Placa = obj.CarregamentoPedido?.Carregamento?.Veiculo?.Placa ?? "",
                                 NotaFiscal = string.Join(", ", (from nf in pedidoXMLNotaFiscais
                                                                 where nf.CargaPedido.Carga.Carregamento.Codigo == obj.CarregamentoPedido.Carregamento.Codigo
                                                                 select nf.XMLNotaFiscal.Numero)),
                                 DataNotaFiscal = string.Join(", ", (from nf in pedidoXMLNotaFiscais
                                                                     where nf.CargaPedido.Carga.Carregamento.Codigo == obj.CarregamentoPedido.Carregamento.Codigo
                                                                     select nf.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss")))
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos = repPedidoProduto.BuscarPorPedidoComFetch(codigoPedido);

                if (listaProdutos == null || listaProdutos.Count == 0)
                    return new JsonpResult(false);

                var listaRetorno = (from obj in listaProdutos
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        CodigoProduto = obj.Produto.Codigo,
                                        CodigoEmbarcador = obj.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                                        Descricao = obj.Produto?.Descricao ?? string.Empty,
                                        Setor = obj.Produto.GrupoProduto.Descricao,
                                        Quantidade = obj.Quantidade,
                                        QuantidadeOriginal = obj.Quantidade,
                                        Removido = false,
                                        DT_Enable = true,
                                        DT_RowColor = "",
                                        DT_RowId = obj.Codigo
                                    });

                return new JsonpResult(listaRetorno);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um problema buscar os dados do produto");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesPedidoProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = 0;
                int.TryParse(Request.Params("Pedido"), out codigoPedido);

                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoArmazem", false);
                grid.AdicionarCabecalho("CodigoProduto", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Produto, "Produto", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoUnitario, "PesoUnitario", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Quantidade, "Quantidade", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PesoTotal, "PesoTotal", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.PalletFechado, "PalletFechado", 5, Models.Grid.Align.right, true);


                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProduto = repPedidoProduto.Consultar(codigoPedido, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoProduto.ContarConsulta(codigoPedido));

                var lista = (from obj in pedidoProduto
                             select new
                             {
                                 obj.Codigo,
                                 CodigoFilial = obj.Pedido.Filial?.Codigo,
                                 CodigoProduto = obj.Produto.Codigo,
                                 Produto = obj.Produto.CodigoProdutoEmbarcador + " - " + obj.Produto.Descricao,
                                 PesoUnitario = obj.PesoUnitario.ToString("n3"),
                                 Quantidade = obj.Quantidade.ToString("n3"),
                                 QuantidadePlanejada = obj.QuantidadePlanejada.ToString("n3"),
                                 PesoTotalEmbalagem = obj.PesoTotalEmbalagem.ToString("n3"),
                                 PesoTotal = ((obj.Quantidade * obj.PesoUnitario) + obj.PesoTotalEmbalagem).ToString("n3"),
                                 PalletFechado = obj.PalletFechado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorPedidoProdutoAlterado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");

                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento repAlteracaoPedidoProdutoAgendamento = new Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento = repAlteracaoPedidoProdutoAgendamento.BuscarPorPedidoNaoVinculado(codigoPedido);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos = repPedidoProduto.BuscarPorPedidoComFetch(codigoPedido);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = repAgendamentoColetaPedidoProduto.BuscarPorCodigoPedido(codigoPedido);

                if (listaProdutos == null || listaProdutos.Count == 0)
                    return new JsonpResult(false);

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in listaProdutos)
                {
                    decimal quantidadeOriginal = pedidoProduto.Quantidade;
                    decimal novaQuantidadeProdutoAgendamento = pedidoProduto.Quantidade;
                    bool pedidoProdutoRemovido = false;

                    List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listProdutosAgendadosDoPedido = listAgendamentoColetaPedidoProduto.Where(x => x.PedidoProduto.Codigo == pedidoProduto.Codigo && ValidaCanceladaDesagendada(x.AgendamentoColetaPedido.AgendamentoColeta, unitOfWork)).ToList();

                    List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamentoValido = listaPedidoProdutoAgendamento.Where(x => ValidaCanceladaDesagendada(x.AgendamentoColeta, unitOfWork)).ToList();

                    if (listaPedidoProdutoAgendamentoValido?.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento item in listaPedidoProdutoAgendamentoValido.Where(o => o.PedidoProduto.Codigo == pedidoProduto.Codigo && o.AgendamentoColeta != null).ToList())
                            quantidadeOriginal -= item.NovaQuantidadeProduto;

                        Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento pedidoProdutoAgendamento = listaPedidoProdutoAgendamentoValido.Find(o => o.PedidoProduto.Codigo == pedidoProduto.Codigo);

                        if (pedidoProduto != null && pedidoProdutoAgendamento != null)
                            novaQuantidadeProdutoAgendamento = pedidoProdutoAgendamento.NovaQuantidadeProduto;
                        else
                            pedidoProdutoRemovido = true;
                    }

                    if (listProdutosAgendadosDoPedido?.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto produtoAgendado in listProdutosAgendadosDoPedido)
                            quantidadeOriginal -= produtoAgendado.Quantidade;
                    }

                    listaRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos
                    {
                        Codigo = pedidoProduto.Codigo,
                        CodigoProduto = pedidoProduto.Produto.Codigo,
                        CodigoEmbarcador = pedidoProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                        Descricao = pedidoProduto.Produto?.Descricao ?? string.Empty,
                        Setor = pedidoProduto.Produto.GrupoProduto.Descricao,
                        Quantidade = novaQuantidadeProdutoAgendamento <= 0 ? 0 : (novaQuantidadeProdutoAgendamento > quantidadeOriginal ? quantidadeOriginal : novaQuantidadeProdutoAgendamento),
                        QuantidadeOriginal = quantidadeOriginal > 0 ? quantidadeOriginal : 0,
                        QuantidadeCaixas = (int)Math.Ceiling(novaQuantidadeProdutoAgendamento / Math.Max(pedidoProduto.Produto.QuantidadeCaixa, 1)),
                        Removido = pedidoProdutoRemovido,
                        DT_Enable = true,
                        DT_RowColor = pedidoProdutoRemovido ? "#ebc3c3" : "",
                        DT_RowId = pedidoProduto.Codigo
                    });
                }

                return new JsonpResult(listaRetorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um problema ao buscar os dados do produto");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool ValidaCanceladaDesagendada(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            if (agendamentoColeta == null) return true;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);

            if (repositorioCargaJanelaDescarregamento.ExisteJanelaDescarregamentoCanceladaPorCarga(agendamentoColeta.Carga.Codigo))
                return false;

            if (agendamentoColeta.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return false;

            if (agendamentoColeta.Carga?.CargaCancelamento != null)
                return false;

            if (agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.NaoComparecimento || agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.NaoComparecimentoConfirmadoPeloFornecedor || agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.CargaDevolvida)
                return false;

            return true;
        }


        private enum IfSum
        {
            Igual,
            Diferente,
            Ambos
        }

        private decimal PesoPedidoCarregamento(int codigoPedidoProduto, int codigoCarregamento, IfSum tipo, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProdutos)
        {
            if (tipo == IfSum.Igual)
            {
                return (from ic in carregamentosPedidoProdutos
                        where ic.PedidoProduto.Codigo == codigoPedidoProduto &&
                              ic.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento
                        select ic.Peso).Sum();
            }
            else if (tipo == IfSum.Diferente)
            {
                return (from ic in carregamentosPedidoProdutos
                        where ic.PedidoProduto.Codigo == codigoPedidoProduto &&
                              ic.CarregamentoPedido.Carregamento.Codigo != codigoCarregamento
                        select ic.Peso).Sum();
            }
            else
            {
                return (from ic in carregamentosPedidoProdutos
                        where ic.PedidoProduto.Codigo == codigoPedidoProduto
                        select ic.Peso).Sum();
            }
        }
    }
}
