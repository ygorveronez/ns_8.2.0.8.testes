using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Expedicao
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Expedicao/ExpedicaoProdutos")]
    public class ExpedicaoProdutosController : BaseController
    {
		#region Construtores

		public ExpedicaoProdutosController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R064_ExpedicaoProdutos;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código do Produto", "CodigoProduto", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Produto", "Produto", 30, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código do Grupo do Produto", "CodigoGrupoProduto", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo do Produto", "GrupoProduto", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Unidade de Medida", "UnidadeDeMedida", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 10, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Total", "PesoTotal", 10, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            return grid;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Produtos Expedidos", "Expedicao", "ExpedicaoProduto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", Codigo, unitOfWork, true, false, 10);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                int codigoTransportador, codigoFilial, codigoRota, codigoDestino, codigoTipoCarga, codigoOrigem, codigoTipoOperacao, produto, grupoProduto, unidadeMedida;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);
                int.TryParse(Request.Params("Produto"), out produto);
                int.TryParse(Request.Params("GrupoProduto"), out grupoProduto);
                int.TryParse(Request.Params("UnidadeDeMedida"), out unidadeMedida);

                double cpfCnpjDestinatario, cpfCnpjRemetente;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out cpfCnpjRemetente);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                List<int> codigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
                List<double> codigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
                List<int> codigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "CodigoProduto")
                    propOrdena = "ProdutoEmbarcador.CodigoProduto";

                if (propOrdena == "Produto")
                    propOrdena = "ProdutoEmbarcador.Produto";

                if (propOrdena == "GrupoProduto")
                    propOrdena = "GrupoProduto.GrupoProduto";

                if (propOrdena == "Filial")
                    propOrdena = "Carga.Filial";

                if (propOrdena == "UnidadeDeMedida")
                    propOrdena = "UnidadeDeMedida.UnidadeDeMedida";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                IList<Dominio.Relatorios.Embarcador.DataSource.Expedicao.ExpedicaoProduto> listaExpedicaoProduto = repCargaPedidoProduto.ConsultarRelatorioProdutosExpedidos(propriedades, dataInicial, dataFinal, produto, grupoProduto, unidadeMedida, codigoTransportador, codigosFilial, codigosRecebedor, codigoRota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaPedidoProduto.ContarConsultarRelatorioProdutosExpedidos(propriedades, dataInicial, dataFinal, produto, grupoProduto, unidadeMedida, codigoTransportador, codigosFilial, codigosRecebedor, codigoRota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite));

                grid.AdicionaRows(listaExpedicaoProduto);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransportador, codigoFilial, codigoRota, codigoDestino, codigoTipoCarga, codigoOrigem, codigoTipoOperacao, produto, grupoProduto, unidadeMedida;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);
                int.TryParse(Request.Params("Produto"), out produto);
                int.TryParse(Request.Params("GrupoProduto"), out grupoProduto);
                int.TryParse(Request.Params("UnidadeDeMedida"), out unidadeMedida);

                double cpfCnpjDestinatario, cpfCnpjRemetente;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out cpfCnpjRemetente);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                List<int> codigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
                List<double> codigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
                List<int> codigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioExpedicaoProdutos(propriedades, dataInicial, dataFinal, produto, grupoProduto, unidadeMedida, codigoTransportador, codigosFilial, codigosRecebedor, codigoRota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #region Métodos Privados
        private async Task GerarRelatorioExpedicaoProdutos(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicial, DateTime dataFinal, int codigoProduto, int codigoGrupoProduto, int codigoUnidadeMedida, int codigoEmpresa, List<int> codigosFilial, List<double> codigosRecebedor,int codigoRota, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, List<int> codigosTipoOperacao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.UnidadeDeMedida repUnidadeDeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Expedicao.ExpedicaoProduto> ExpedicaoProduto = repCargaPedidoProduto.ConsultarRelatorioProdutosExpedidos(propriedades, dataInicial, dataFinal, codigoProduto, codigoGrupoProduto, codigoUnidadeMedida, codigoEmpresa, codigosFilial, codigosRecebedor,codigoRota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, ExpedicaoProduto, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataInicial != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", dataInicial.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                if (dataFinal != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", dataFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));


                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigosFilial?.Count > 0)
                {
                    if (codigosFilial.Count == 1)
                    {
                        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigosFilial.FirstOrDefault());

                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

                if (cpfCnpjRemetente > 0d)
                {
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente.CPF_CNPJ_Formatado + " - " + remetente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", false));

                if (cpfCnpjDestinatario > 0d)
                {
                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario.CPF_CNPJ_Formatado + " - " + destinatario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

                if (codigoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", produto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

                if (codigoGrupoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

                if (codigoGrupoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

                if (codigoOrigem > 0)
                {
                    Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigo(codigoOrigem);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", false));

                if (codigoDestino > 0)
                {
                    Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigo(codigoDestino);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

                if (codigoRota > 0)
                {
                    Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarPorCodigo(codigoRota);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", rota.Origem.DescricaoCidadeEstado + " -> " + rota.Destino.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", false));

                if (codigosTipoOperacao?.Count > 0)
                {
                    if (codigosTipoOperacao.Count == 1)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigosTipoOperacao.FirstOrDefault());

                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", false));

                if (codigosTipoCarga?.Count > 0)
                {
                    if (codigosTipoCarga.Count == 1)
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigo(codigosTipoCarga.FirstOrDefault());

                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));

                if (codigoUnidadeMedida > 0)
                {
                    Dominio.Entidades.UnidadeDeMedida unidadeDeMedida = repUnidadeDeMedida.BuscarPorCodigo(codigoUnidadeMedida);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UnidadeDeMedida", unidadeDeMedida.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UnidadeDeMedida", false));

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Expedicao/ExpedicaoProdutos", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Expedicao/ExpedicaoProdutos", parametros, relatorioControleGeracao, relatorioTemp, ExpedicaoProduto, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}
