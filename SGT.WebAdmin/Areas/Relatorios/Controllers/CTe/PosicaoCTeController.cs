using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/PosicaoCTe")]
    public class PosicaoCTeController : BaseController
    {
		#region Construtores

		public PosicaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R056_PosicaoCTe;

        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasLocalidades = 3;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "NumeroCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "SerieCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Status", "StatusCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Anulação", "DataAnulacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Importação", "DataImportacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Vínculo Carga", "DataVinculoCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Fatura", "DataFatura", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatario", "CPFCNPJDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Inicio da Prestação", "InicioPrestacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Inicio", "UFInicioPrestacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Fim da Prestação", "FimPrestacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Fim", "UFFimPrestacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ICMS", "ValorICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório Posição de CT-es", "CTe", "PosicaoCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "NumeroCTe", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                DateTime dataPosicao;
                DateTime.TryParseExact(Request.Params("DataPosicao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPosicao);
                if (dataPosicao == DateTime.MinValue)
                    return new JsonpResult(false, "Favor selecione a data de posição");

                int codigoTransportador = 0;
                int.TryParse(Request.Params("Transportadora"), out codigoTransportador);

                string statusCTe = Request.Params("StatusCTe");

                bool somenteCTesFaturados, somenteAvon, somenteDiaInformado;
                bool.TryParse(Request.Params("SomenteCTesFaturados"), out somenteCTesFaturados);
                bool.TryParse(Request.Params("SomenteAvon"), out somenteAvon);
                bool.TryParse(Request.Params("SomenteDiaInformado"), out somenteDiaInformado);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.PosicaoCTe> listaPosicaoCTe = repTitulo.RelatorioPosicaoCTe(somenteDiaInformado, somenteAvon, statusCTe, somenteCTesFaturados, dataPosicao, codigoTransportador, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarRelatorioPosicaoCTe(somenteDiaInformado, somenteAvon, statusCTe, somenteCTesFaturados, dataPosicao, codigoTransportador));

                var lista = (from obj in listaPosicaoCTe
                             select new
                             {
                                 obj.Codigo,
                                 obj.NumeroCTe,
                                 obj.SerieCTe,
                                 obj.StatusCTe,
                                 obj.NumeroCarga,
                                 DataEmissao = obj.DataEmissao > DateTime.MinValue ? obj.DataEmissao.ToString("dd/MM/yyyy") : string.Empty,
                                 DataAutorizacao = obj.DataAutorizacao > DateTime.MinValue ? obj.DataAutorizacao.ToString("dd/MM/yyyy") : string.Empty,
                                 DataCancelamento = obj.DataCancelamento > DateTime.MinValue ? obj.DataCancelamento.ToString("dd/MM/yyyy") : string.Empty,
                                 DataAnulacao = obj.DataAnulacao > DateTime.MinValue ? obj.DataAnulacao.ToString("dd/MM/yyyy") : string.Empty,
                                 DataImportacao = obj.DataImportacao > DateTime.MinValue ? obj.DataImportacao.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVinculoCarga = obj.DataVinculoCarga > DateTime.MinValue ? obj.DataVinculoCarga.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.CPFCNPJRemetente,
                                 obj.Remetente,
                                 obj.CPFCNPJDestinatario,
                                 obj.Destinatario,
                                 obj.CPFCNPJTomador,
                                 obj.Tomador,
                                 obj.InicioPrestacao,
                                 obj.UFInicioPrestacao,
                                 obj.FimPrestacao,
                                 obj.UFFimPrestacao,
                                 obj.Transportador,
                                 obj.AliquotaICMS,
                                 obj.ValorICMS,
                                 obj.ValorFrete,
                                 obj.ValorReceber,
                                 obj.ValorMercadoria,
                                 obj.ChaveCTe,
                                 DataFatura = obj.DataFatura > DateTime.MinValue ? obj.DataFatura.ToString("dd/MM/yyyy") : string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataPosicao;
                DateTime.TryParseExact(Request.Params("DataPosicao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPosicao);

                if (dataPosicao == DateTime.MinValue)
                    return new JsonpResult(false, "Favor selecione a data de posição");

                int codigoTransportador = 0;
                int.TryParse(Request.Params("Transportadora"), out codigoTransportador);

                string statusCTe = Request.Params("StatusCTe");

                bool somenteCTesFaturados, somenteAvon, somenteDiaInformado;
                bool.TryParse(Request.Params("SomenteCTesFaturados"), out somenteCTesFaturados);
                bool.TryParse(Request.Params("SomenteDiaInformado"), out somenteDiaInformado);
                bool.TryParse(Request.Params("SomenteAvon"), out somenteAvon);

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

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioPosicaoCTe(somenteDiaInformado, somenteAvon, statusCTe, somenteCTesFaturados, dataPosicao, codigoTransportador, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioPosicaoCTe(bool somenteDiaInformado, bool somenteAvon, string statusCTe, bool somenteCTesFaturados, DateTime dataPosicao, int codigoTransportador, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.PosicaoCTe> listaPosicaoCTe = repTitulo.RelatorioPosicaoCTe(somenteDiaInformado, somenteAvon, statusCTe, somenteCTesFaturados, dataPosicao, codigoTransportador, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);
                //var lista = (from obj in listaPosicaoCTe
                //             select new
                //             {
                //                 obj.Codigo,
                //                 obj.NumeroCTe,
                //                 obj.SerieCTe,
                //                 obj.StatusCTe,
                //                 obj.NumeroCarga,
                //                 DataEmissao = obj.DataEmissao > DateTime.MinValue ? obj.DataEmissao.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataAutorizacao = obj.DataAutorizacao > DateTime.MinValue ? obj.DataAutorizacao.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataCancelamento = obj.DataCancelamento > DateTime.MinValue ? obj.DataCancelamento.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataAnulacao = obj.DataAnulacao > DateTime.MinValue ? obj.DataAnulacao.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataImportacao = obj.DataImportacao > DateTime.MinValue ? obj.DataImportacao.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataVinculoCarga = obj.DataVinculoCarga > DateTime.MinValue ? obj.DataVinculoCarga.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.CPFCNPJRemetente,
                //                 obj.Remetente,
                //                 obj.CPFCNPJDestinatario,
                //                 obj.Destinatario,
                //                 obj.CPFCNPJTomador,
                //                 obj.Tomador,
                //                 obj.InicioPrestacao,
                //                 obj.UFInicioPrestacao,
                //                 obj.FimPrestacao,
                //                 obj.UFFimPrestacao,
                //                 obj.Transportador,
                //                 obj.AliquotaICMS,
                //                 obj.ValorICMS,
                //                 obj.ValorFrete,
                //                 obj.ValorReceber,
                //                 obj.ValorMercadoria,
                //                 obj.ChaveCTe,
                //                 DataFatura = obj.DataFatura > DateTime.MinValue ? obj.DataFatura.ToString("dd/MM/yyyy") : string.Empty
                //             }).ToList();

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataPosicao != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPosicao", dataPosicao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPosicao", false));

                if (codigoTransportador > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", repEmpresa.BuscarPorCodigo(codigoTransportador).RazaoSocial, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (statusCTe == "0")
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Todos", true));
                else if (statusCTe == "A")
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Autorizados", true));
                else if (statusCTe == "C")
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Cancelados", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Anulados", true));

                if (somenteCTesFaturados)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteCTesFaturados", "Sim", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteCTesFaturados", "Não", true));

                if (somenteAvon)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteAVON", "Sim", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteAVON", "Não", true));

                // serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);
                // serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/CTe/PosicaoCTe", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/CTe/PosicaoCTe", parametros, relatorioControleGeracao, relatorioTemp, listaPosicaoCTe, unitOfWork);
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
    }
}
