using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Creditos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Creditos/CargaComplementosFrete")]
    public class CargaComplementosFreteController : BaseController
    {
		#region Construtores

		public CargaComplementosFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R016_CargaComponenteFrete;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Operador", "Solicitante", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Creditor", "Solicitado", 8, Models.Grid.Align.left, true, true, false, false, false);
            grid.AdicionarCabecalho("Carga", "Carga", 5, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, true, true, false, true, true);

            grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false, true, false, false, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, false, true, false, false, false);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false, true, false, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", 11, Models.Grid.Align.left, false, true, false, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", 11, Models.Grid.Align.left, false, true, false, false, true);

            grid.AdicionarCabecalho("Valor Solicitado", "ValorSolicitado", (decimal)7, Models.Grid.Align.right, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum, false);
            grid.AdicionarCabecalho("Valor Liberado", "ValorLiberado", (decimal)7, Models.Grid.Align.right, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo", "MotivoAdicionalFrete", 15, Models.Grid.Align.left, false, true, false, true);

            grid.AdicionarCabecalho("Observação", "Motivo", 20, Models.Grid.Align.left, false, true, false, false, false);

            grid.AdicionarCabecalho("Retorno", "RetornoSolicitacao", 20, Models.Grid.Align.left, false, true, false, false, false);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de complementos de Frete", "Creditos", "CargaComplementosFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "Solicitante", "", Codigo, unitOfWork, true, false, 7);
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
                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                int codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Operador"), out operador);

                int.TryParse(Request.Params("MotivoAdicionalFrete"), out motivoAdicionalFrete);

                // TODO (ct-reports): Repassar CT
                List<int> codigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
                List<double> codigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete)int.Parse(Request.Params("SituacaoComplementoFrete"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Transportador")
                    propOrdena = "Carga.Empresa.RazaoSocial";
                else if (propOrdena == "MotivoAdicionalFrete")
                    propOrdena = "MotivoAdicionalFrete.Descricao";

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                if (propAgrupa == "Solicitante")
                    propAgrupa = "Usuario";
                else if (propAgrupa == "Transportador")
                    propAgrupa = "Carga.Empresa";
                else if (propAgrupa == "MotivoAdicionalFrete")
                    propAgrupa = "MotivoAdicionalFrete.Descricao";

                List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> listaCargaComplementoFrete = repCargaComplementoFrete.ConsultarRelatorioCargaComplementosFrete(dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor,codigosTipoCarga, codigosTipoOperacao, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaComplementoFrete.ContarConsultaRelatorioCargaComplementosFrete(dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor, codigosTipoCarga, codigosTipoOperacao));


                var lista = (from obj in listaCargaComplementoFrete
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Solicitante = obj.Usuario.Nome,
                                 Solicitado = obj.SolicitacaoCredito != null ? obj.SolicitacaoCredito.Solicitado.Nome : "",
                                 Carga = obj.Carga.CodigoCargaEmbarcador,
                                 Data = obj.DataAlteracao.ToString("dd/MM/yyyy"),
                                 Destinatario = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Destinatarios : "",
                                 Remetente = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Remetentes : "",
                                 Filial = obj.Carga.Filial != null ? obj.Carga.Filial.Descricao : "",
                                 Origem = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Origens : "",
                                 Destino = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Destinos : "",
                                 Transportador = obj.Carga.Empresa != null ? obj.Carga.Empresa.RazaoSocial : "CARGA DISPONÍVEL NO PORTAL",
                                 Veiculo = obj.Carga.Veiculo != null ? obj.Carga.Veiculo.Placa : "",
                                 ValorSolicitado = obj.ValorComplementoOriginal.ToString("n2"),
                                 ValorLiberado = obj.ValorComplemento.ToString("n2"),
                                 MotivoAdicionalFrete = obj.MotivoAdicionalFrete?.Descricao ?? "",
                                 Situacao = obj.DescricaoSituacao,
                                 Motivo = obj.Motivo,
                                 RetornoSolicitacao = obj.SolicitacaoCredito != null ? obj.SolicitacaoCredito.RetornoSolicitacao : ""
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
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                int codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Operador"), out operador);
                int.TryParse(Request.Params("MotivoAdicionalFrete"), out motivoAdicionalFrete);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete)int.Parse(Request.Params("SituacaoComplementoFrete"));

                List<int> codigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
                List<double> codigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);


                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);


                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioCargaComplementosFrete(dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor, codigosTipoCarga, codigosTipoOperacao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        private async Task GerarRelatorioCargaComplementosFrete(DateTime dataInicio, DateTime dataFim, int codigoTransportador, int codigoVeiculo, int operador, int motivoAdicionalFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete situacaoComplementoFrete, List<int> codigosFilial, List<double> codigosRecebedor, List<int> codigosTipoCarga, List<int> codigosTipoOperacao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                if (propAgrupa == "Solicitante")
                    propAgrupa = "Usuario";
                else if (propAgrupa == "Transportador")
                    propAgrupa = "Carga.Empresa";
                else if (propAgrupa == "MotivoAdicionalFrete")
                    propAgrupa = "MotivoAdicionalFrete.Descricao";

                List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> listaCargaComplementoFrete = repCargaComplementoFrete.ConsultarRelatorioCargaComplementosFrete(dataInicio, dataFim, codigoTransportador, codigoVeiculo, operador, motivoAdicionalFrete, situacaoComplementoFrete, codigosFilial, codigosRecebedor, codigosTipoCarga, codigosTipoOperacao, propAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao);
                List<Dominio.Relatorios.Embarcador.DataSource.Creditos.CargaComplementosFrete> lista = (from obj in listaCargaComplementoFrete
                                                                                                        select new Dominio.Relatorios.Embarcador.DataSource.Creditos.CargaComplementosFrete
                                                                                                        {
                                                                                                            Codigo = obj.Codigo,
                                                                                                            Solicitante = obj.Usuario.Nome,
                                                                                                            Solicitado = obj.SolicitacaoCredito != null ? obj.SolicitacaoCredito.Solicitado.Nome : "",
                                                                                                            Carga = obj.Carga.CodigoCargaEmbarcador,
                                                                                                            Data = obj.DataAlteracao,
                                                                                                            Destinatario = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Destinatarios : "",
                                                                                                            Remetente = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Remetentes : "",
                                                                                                            Filial = obj.Carga.Filial != null ? obj.Carga.Filial.Descricao : "",
                                                                                                            Origem = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Origens : "",
                                                                                                            Destino = obj.Carga.DadosSumarizados != null ? obj.Carga.DadosSumarizados.Destinos : "",
                                                                                                            Transportador = obj.Carga.Empresa != null ? obj.Carga.Empresa.RazaoSocial : "CARGA DISPONÍVEL NO PORTAL",
                                                                                                            Veiculo = obj.Carga.Veiculo != null ? obj.Carga.Veiculo.Placa : "",
                                                                                                            ValorSolicitado = obj.ValorComplementoOriginal,
                                                                                                            ValorLiberado = obj.ValorComplemento,
                                                                                                            Situacao = obj.DescricaoSituacao,
                                                                                                            MotivoAdicionalFrete = obj.MotivoAdicionalFrete?.Descricao ?? "",
                                                                                                            Motivo = obj.Motivo,
                                                                                                            RetornoSolicitacao = obj.SolicitacaoCredito != null ? obj.SolicitacaoCredito.RetornoSolicitacao : ""
                                                                                                        }).ToList();



                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataInicio != DateTime.MinValue || dataFim != DateTime.MinValue)
                {
                    string data = "";
                    if (dataInicio != DateTime.MinValue)
                        data = dataInicio.ToString("dd/MM/yyyy");
                    if (dataFim != DateTime.MinValue)
                        data += " até " + dataFim.ToString("dd/MM/yyyy");

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", false));

                if (codigoTransportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (motivoAdicionalFrete > 0)
                {
                    Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete entMotivoAdicionalFrete = repMotivoAdicionalFrete.BuscarPorCodigo(motivoAdicionalFrete);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MotivoAdicionalFrete", entMotivoAdicionalFrete.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MotivoAdicionalFrete", false));

                if (codigoVeiculo > 0)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

                if (operador > 0)
                {
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(operador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", usuario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", false));

                if (situacaoComplementoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Todas)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoComplementoFrete", situacaoComplementoFrete.ToString(), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoComplementoFrete", false));

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Creditos/CargaComplementosFrete", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Creditos/CargaComplementosFrete", parametros, relatorioControleGeracao, relatorioTemp, lista, unitOfWork);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }
    }
}
