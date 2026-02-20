using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Produtos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Produtos/ProdutoEmbarcador")]
    public class ProdutoEmbarcadorController : BaseController
    {
		#region Construtores

		public ProdutoEmbarcadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R230_ProdutoEmbarcador;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Produtos Embarcador", "Produtos", "ProdutoEmbarcador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorio = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorioProdutoEmbarcador(filtrosPesquisa, agrupamentos);
                IList<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador> listaProdutos = (totalRegistros > 0) ? repositorio.ConsultarRelatorioProdutoEmbarcador(filtrosPesquisa, agrupamentos, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador>();

                bool temMontagemCarregamentoPedidoProduto = listaProdutos.Any(o => o.MontagemCarregamentoPedidoProduto == true);
                int contadorTecnico = 0;
                if(temMontagemCarregamentoPedidoProduto)
                {
                    //POG...vai lascar com a paginação mas se faz necessário...
                    for (int i = 0; i < listaProdutos.Count; i++)
                    {
                        if (listaProdutos[i].MontagemCarregamentoPedidoProduto && listaProdutos[i].Saldo > 0 && listaProdutos[i].StatusPedido.ToUpper() != "Em aberto".ToUpper())
                        {
                            Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador produtoClonado = listaProdutos[i].Clonar();
                            listaProdutos[i].Quantidade -= listaProdutos[i].Saldo;
                            if (listaProdutos[i].Quantidade > 0)
                            {
                                if (i < listaProdutos.Count - 1)
                                    listaProdutos.Insert(i + 1, produtoClonado);
                                else
                                    listaProdutos.Add(produtoClonado);
                                contadorTecnico++;
                            }
                        }
                    }
                }

                grid.setarQuantidadeTotal(totalRegistros + contadorTecnico);
                grid.AdicionaRows(listaProdutos);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                _ = Task.Factory.StartNew(() => GerarRelatorio(agrupamentos, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorio = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador> dataSource = repositorio.ConsultarRelatorioProdutoEmbarcador(filtrosPesquisa, agrupamentos, parametrosConsulta);

                //POG...vai lascar com a paginação mas se faz necessário...
                for (int i = 0; i < dataSource.Count; i++)
                {
                    if (dataSource[i].Saldo > 0 && dataSource[i].StatusPedido.ToUpper() != "Em aberto".ToUpper())
                    {
                        Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador produtoClonado = dataSource[i].Clonar();
                        dataSource[i].Quantidade -= dataSource[i].Saldo;
                        if (dataSource[i].Quantidade > 0)
                        {
                            if (i < dataSource.Count - 1)
                                dataSource.Insert(i + 1, produtoClonado);
                            else
                                dataSource.Add(produtoClonado);
                        }
                    }
                }

                List<Parametro> parametros = ObterParametros(filtrosPesquisa, unitOfWork, parametrosConsulta);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Produtos/ProdutoEmbarcador",parametros,relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador()
            {
                DataPedidoInicial = Request.GetNullableDateTimeParam("DataPedidoInicial"),
                DataPedidoFinal = Request.GetNullableDateTimeParam("DataPedidoFinal"),
                CodigosFiliais = Request.GetListParam<int>("Filial").Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : Request.GetListParam<int>("Filial"),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigosCanaisEntrega = Request.GetListParam<int>("CanalEntrega"),
                CodigosTiposCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosGrupoProduto = Request.GetListParam<int>("GrupoProduto"),
                CodigosDestinatario = Request.GetListParam<double>("Destinatario"),
                PedidosEmbarcador = Request.GetListParam<int>("Pedido"),
                StatusPedido = Request.GetListEnumParam<StatusPedidoEmbarcadorAssai>("StatusPedido"),
                TipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigosGrupoPessoa = Request.GetListParam<int>("GrupoPessoa"),
                CodigosProduto = Request.GetListParam<int>("Produto")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("MontagemCarregamentoPedidoProduto", false);
            grid.AdicionarCabecalho("Código", "Codigo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Pedido Embarcador", "PedidoEmbarcador", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Filial Integração", "FilialIntegracao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data/Hora Pedido", "DataHoraPedidoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Remetente Integração", "RemetenteIntegracao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário Integração", "DestinatarioIntegracao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Canal Entrega", "CanalEntrega", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operaçao", "TipoOperacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Produto Integração", "ProdutoIntegracao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Produto", "Produto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Quantidade Embalagem", "QuantidadeEmbalagem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade Pallet", "QuantidadePallet", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Unitário", "PesoUnitario", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Pallet Fechado", "PalletFechado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Metro Cúbico", "MetroCubico", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Total Toneladas", "PesoTotalToneladas", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso Total Quilogramas", "PesoTotalQuilogramas", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Status Produto", "StatusPedido", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Carga/Pré-Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Linha Separação", "LinhaSeparacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Sessão Roteirizador", "SessaoRoteirizador", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Grupo Pessoas", "GrupoPessoa", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade Caixas Por Palete", "QtdCaixasPorPalete", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("ID Demanda", "IDDemanda", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Endereço Produto", "EnderecoProduto", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Quantidade Carregada", "QuantidadeCarregada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Saldo", "Saldo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Placa", "Placa", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtrosPesquisa, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.TipoCarga repositorioTipoCarga = new Repositorio.TipoCarga(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPesosa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canaisEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();
            List<Dominio.Entidades.TipoCarga> tiposCarga = new List<Dominio.Entidades.TipoCarga>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            if (filtrosPesquisa.CodigosFiliais.Count > 0)
                filiais = repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais);

            if (filtrosPesquisa.CodigosCanaisEntrega.Count > 0)
                canaisEntrega = repositorioCanalEntrega.BuscarPorCodigos(filtrosPesquisa.CodigosCanaisEntrega);

            if (filtrosPesquisa.CodigosTiposCarga.Count > 0)
                tiposCarga = repositorioTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTiposCarga);

            if (filtrosPesquisa.CodigosDestinatario.Count > 0)
                destinatarios = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CodigosDestinatario);

            if (filtrosPesquisa.CodigosGrupoProduto.Count > 0)
                gruposProduto = repositorioGrupoProduto.BuscarPorCodigos(filtrosPesquisa.CodigosGrupoProduto);

            if (filtrosPesquisa.PedidosEmbarcador.Count > 0)
                pedidos = repositorioPedido.BuscarPorCodigos(filtrosPesquisa.PedidosEmbarcador);

            if (filtrosPesquisa.CodigosGrupoPessoa.Count > 0)
                grupoPessoas = repositorioGrupoPesosa.BuscarPorCodigo(filtrosPesquisa.CodigosGrupoPessoa.ToArray());

            if (filtrosPesquisa.CodigosProduto.Count > 0)
                produtos = repositorioProdutoEmbarcador.BuscarPorCodigo(filtrosPesquisa.CodigosProduto.ToArray());

            if (filtrosPesquisa.TipoOperacao.Count > 0)
                tiposOperacao = repositorioTipoOperacao.BuscarPorCodigos(filtrosPesquisa.TipoOperacao);

            parametros.Add(new Parametro("DataPedido", filtrosPesquisa.DataPedidoInicial, filtrosPesquisa.DataPedidoFinal));
            parametros.Add(new Parametro("Filial", string.Join(", ", filiais.Select(o => o.Descricao))));
            parametros.Add(new Parametro("CanalEntrega", string.Join(", ", canaisEntrega.Select(o => o.Descricao))));
            parametros.Add(new Parametro("TipoCarga", string.Join(", ", tiposCarga.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Destinatario", string.Join(", ", destinatarios.Select(o => o.Descricao))));
            parametros.Add(new Parametro("GrupoProduto", string.Join(", ", gruposProduto.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Pedido", string.Join(", ", pedidos.Select(o => o.Descricao))));

            parametros.Add(new Parametro("StatusPedido", string.Join(", ", filtrosPesquisa.StatusPedido.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("TipoOperacao", string.Join(", ", tiposOperacao.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Produto", string.Join(", ", produtos.Select(o => o.Descricao))));
            parametros.Add(new Parametro("GrupoPessoa", string.Join(", ", grupoPessoas.Select(o => o.Descricao))));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}
