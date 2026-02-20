using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/TaxaIncidenciaFrete")]
    public class TaxaIncidenciaFreteController : BaseController
    {
		#region Construtores

		public TaxaIncidenciaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R028_TaxaIncidenciaFrete;

        private decimal TamanhoColunaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Taxa de Incidência de Frete", "Cargas", "TaxaIncidenciaFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "DataCarregamento", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                int codigoTransportador, codigoFilial, codigoCentroCarregamento, codigoRota, codigoDestino, codigoTipoCarga, codigoModeloVeiculo;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                // TODO (ct-reports): Repassar CT
                List<int> codigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
                List<double> codigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
                List<int> codigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.TaxaIncidenciaFrete.TaxaIncidenciaFrete> listaTaxaIncidenciaFrete = await repCarga.ConsultarRelatorioTaxaIncidenciaFrete(agrupamentos, dataInicial, dataFinal, codigoTransportador, codigosFilial, codigosRecebedores, codigoCentroCarregamento, codigoRota, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjDestinatario, codigosTipoOperacao, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, cancellationToken);
                grid.setarQuantidadeTotal(listaTaxaIncidenciaFrete.Count());

                grid.AdicionaRows(listaTaxaIncidenciaFrete);

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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTransportador, codigoFilial, codigoCentroCarregamento, codigoRota, codigoDestino, codigoTipoCarga, codigoModeloVeiculo;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);
                int.TryParse(Request.Params("ModeloVeiculo"), out codigoModeloVeiculo);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                List<int> codigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
                List<double> codigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
                List<int> codigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioTaxaIncidenciaFrete(agrupamentos, dataInicial, dataFinal, codigoTransportador, codigosFilial, codigosRecebedores, codigoCentroCarregamento, codigoRota, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjDestinatario, codigosTipoOperacao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioTaxaIncidenciaFrete(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicial, DateTime dataFinal, int codigoTransportador, List<int> codigosFilial, List<double> codigosRecebedores,int codigoCentroCarregamento, int codigoRota, int codigoDestino, List<int> codigosTipoCarga, int codigoModeloVeiculo, double cpfCnpjDestinatario, List<int> codigosTipoOperacao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork, cancellationToken);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.TaxaIncidenciaFrete.TaxaIncidenciaFrete> listaTaxaIncidenciaFrete = await repCarga.ConsultarRelatorioTaxaIncidenciaFrete(propriedades, dataInicial, dataFinal, codigoTransportador, codigosFilial, codigosRecebedores,codigoCentroCarregamento, codigoRota, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjDestinatario, codigosTipoOperacao, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, cancellationToken);

                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo, cancellationToken);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", dataInicial, dataFinal));

                if (codigoTransportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(codigoTransportador, cancellationToken);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigoCentroCarregamento > 0)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = await repCentroCarregamento.BuscarPorCodigoAsync(codigoCentroCarregamento, cancellationToken);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", centroCarregamento.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroCarregamento", false));

                if (codigoDestino > 0)
                {
                    Dominio.Entidades.Localidade destino = await repLocalidade.BuscarPorCodigoAsync(codigoDestino);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

                if (cpfCnpjDestinatario > 0d)
                {
                    Dominio.Entidades.Cliente destinatario = await repCliente.BuscarPorCPFCNPJAsync(cpfCnpjDestinatario);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario.CPF_CNPJ_Formatado + " - " + destinatario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

                if (codigosFilial?.Count > 0)
                {
                    if (codigosFilial.Count == 1)
                    {
                        Dominio.Entidades.Embarcador.Filiais.Filial filial = await repFilial.BuscarPorCodigoAsync(codigosFilial.FirstOrDefault());

                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

                if (codigoModeloVeiculo > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = await repModeloVeiculo.BuscarPorCodigoAsync(codigoModeloVeiculo, cancellationToken);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", false));

                if (codigoRota > 0)
                {
                    Dominio.Entidades.Embarcador.Logistica.Rota rota = await repRota.BuscarPorCodigoAsync(codigoRota, cancellationToken);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", rota.Origem.DescricaoCidadeEstado + " -> " + rota.Destino.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Rota", false));

                if (codigosTipoCarga?.Count > 0)
                {
                    if (codigosTipoCarga.Count == 1)
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = await repTipoCarga.BuscarPorCodigoAsync(codigosTipoCarga.FirstOrDefault(), cancellationToken);

                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));
                
                serRelatorio.GerarRelatorioDinamico("Relatorios/Cargas/TaxaIncidenciaFrete", parametros, relatorioControleGeracao, relatorioTemp, listaTaxaIncidenciaFrete, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Numero da Carga", "NumeroCarga", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data de Carregamento", "DataCarregamento", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeiculo", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Pedido", "NumeroPedido", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculos", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Coletas", "NumeroColetas", TamanhoColunaExtraPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Entregas", "NumeroEntregas", TamanhoColunaExtraPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor NF", "ValorTotalNotaFiscal", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Taxa de Incidência", "TaxaIncidencia", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.media);

            return grid;
        }

        #endregion
    }
}
