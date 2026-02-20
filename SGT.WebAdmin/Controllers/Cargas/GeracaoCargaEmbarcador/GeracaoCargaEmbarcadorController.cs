using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.GeracaoCargaEmbarcador
{
    [CustomAuthorize("Cargas/GeracaoCargaEmbarcador")]
    public class GeracaoCargaEmbarcadorController : BaseController
    {
		#region Construtores

		public GeracaoCargaEmbarcadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos 

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaGeracaoEmbarcador filtros = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = ObterGridPesquisa();

                Repositorio.Embarcador.Cargas.CargaGeracaoEmbarcador repCargaGeracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaGeracaoEmbarcador(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                int countRegistros = repCargaGeracaoEmbarcador.ContarConsulta(filtros);

                List<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador> registros = new List<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador>();

                if (countRegistros > 0)
                    registros = repCargaGeracaoEmbarcador.Consultar(filtros, grid.ObterParametrosConsulta(ObterPropriedadeOrdenarPesquisa));

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       DataGeracao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       Carga = obj.Carga.CodigoCargaEmbarcador,
                                       Origem = svcCargaDadosSumarizados.ObterOrigem(obj.Carga),
                                       Destino = svcCargaDadosSumarizados.ObterDestino(obj.Carga),
                                       Veiculos = obj.Carga.PlacasVeiculos ?? string.Empty
                                   }).ToList());


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

        public async Task<IActionResult> PesquisaMDFesDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador filtros = ObterFiltrosPesquisaMDFesDisponiveis();
                Models.Grid.Grid grid = ObterGridPesquisaMDFesDisponiveis();

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                int countRegistros = repMDFe.ContarConsultaMDFesDisponiveisParaGerarCarga(filtros);

                IList<Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador> registros = new List<Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador>();

                if (countRegistros > 0)
                    registros = repMDFe.ConsultarMDFesDisponiveisParaGerarCarga(filtros, grid.ObterParametrosConsulta(ObterPropriedadeOrdenarPesquisa));

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(registros);

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

        public async Task<IActionResult> GerarCarga()
        {
            bool violacaoChaveUnica = false;
            int countExecucoes = 0;

            do
            {
                countExecucoes++;

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                try
                {
                    List<int> codigosMDFes = Request.GetListParam<int>("MDFes");

                    int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                    int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                    int codigoCentroResultados = Request.GetIntParam("CentroResultado");
                    int codigoVeiculo = Request.GetIntParam("Veiculo");

                    DateTime? dataPrevisaoSaida = Request.GetNullableDateTimeParam("DataPrevisaoSaida");
                    DateTime? dataPrevisaoEntrega = Request.GetNullableDateTimeParam("DataPrevisaoEntrega");

                    if (!Servicos.Embarcador.CTe.CTEsImportados.GerarCarga(out string mensagem, codigosMDFes, codigoTipoOperacao, codigoTipoCarga, codigoCentroResultados, dataPrevisaoSaida, dataPrevisaoEntrega, codigoVeiculo, unitOfWork, TipoServicoMultisoftware, Auditado))
                        return new JsonpResult(false, true, mensagem);

                    return new JsonpResult(true);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                    {
                        System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;

                        if (excecao.Number == 2601 || excecao.Number == 2627)
                        {
                            violacaoChaveUnica = true;

                            if (countExecucoes > 4)
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, "Ocorreu uma falha ao gerar a carga.");
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();

                            return new JsonpResult(false, "Ocorreu uma falha ao gerar a carga.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();

                        return new JsonpResult(false, "Ocorreu uma falha ao gerar a carga.");
                    }
                }
                finally
                {
                    unitOfWork.Dispose();
                }

            } while (violacaoChaveUnica && countExecucoes < 5);

            return new JsonpResult(true);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaGeracaoEmbarcador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaGeracaoEmbarcador()
            {
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                NumeroMDFe = Request.GetIntParam("NumeroMDFe")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data Geração", "DataGeracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Origem", "Origem", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destino", "Destino", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Veículos", "Veiculos", 15, Models.Grid.Align.left, false);

            return grid;
        }

        private string ObterPropriedadeOrdenarPesquisa(string prop)
        {
            if (prop == "DataGeracao")
                return "Carga.DataCriacaoCarga";

            return prop;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador ObterFiltrosPesquisaMDFesDisponiveis()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador()
            {
                NomeMotorista = Utilidades.String.Left(Utilidades.String.OnlyNumbersAndChars(Request.GetStringParam("NomeMotorista")),80),
                PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
                UFOrigem = Request.GetStringParam("UFOrigem"),
                UFDestino = Request.GetStringParam("UFDestino"),
                CodigoLocalidadeDestino = Request.GetIntParam("LocalidadeDestino"),
                CodigoLocalidadeOrigem = Request.GetIntParam("LocalidadeOrigem"),
                NumeroMDFe = Request.GetIntParam("NumeroMDFe"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                DataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetNullableDateTimeParam("DataEmissaoFinal"),
                Serie = Request.GetIntParam("Serie")
            };
        }

        private Models.Grid.Grid ObterGridPesquisaMDFesDisponiveis()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data de Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Estado Origem", "UFOrigem", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Origem", "Origem", 16, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Estado Destino", "UFDestino", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destino", "Destino", 16, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Veículos", "Veiculos", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Segmento", "Segmento", 10, Models.Grid.Align.left, false);

            return grid;
        }

        #endregion
    }
}
