using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/OcorrenciaPatio")]
    public class OcorrenciaPatioController : BaseController
    {
		#region Construtores

		public OcorrenciaPatioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new Servicos.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioDados dados = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioDados()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoTipo = Request.GetIntParam("Tipo"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    Descricao = Request.GetStringParam("Descricao"),
                    TipoLancamento = TipoLancamento.Manual
                };

                servicoOcorrenciaPatio.Adicionar(dados);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a ocorrência de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrenciaPatio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new Servicos.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);

                servicoOcorrenciaPatio.Aprovar(codigoOcorrenciaPatio);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a ocorrência de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrenciaPatio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new Servicos.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);

                servicoOcorrenciaPatio.Excluir(codigoOcorrenciaPatio);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir a ocorrência de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrenciaPatio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new Servicos.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);

                servicoOcorrenciaPatio.Reprovar(codigoOcorrenciaPatio);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar a ocorrência de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatio ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatio()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacoes = Request.GetListEnumParam<SituacaoOcorrenciaPatio>("Situacao"),
                TipoLancamento = Request.GetNullableEnumParam<TipoLancamento>("TipoLancamento")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("TipoLancamento", false);
                grid.AdicionarCabecalho("Data da Geração", "DataGeracao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tração", "Tracao", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Reboques", "Reboques", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio repositorioOcorrenciaPatio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);
                int totalRegistros = repositorioOcorrenciaPatio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio> listaOcorrenciaPatio = totalRegistros > 0 ? repositorioOcorrenciaPatio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio>();

                var listaOcorrenciaPatioRetornar = (
                    from ocorrenciaPatio in listaOcorrenciaPatio
                    select new
                    {
                        ocorrenciaPatio.Codigo,
                        ocorrenciaPatio.Situacao,
                        ocorrenciaPatio.TipoLancamento,
                        DataGeracao = ocorrenciaPatio.DataGeracao.ToString("dd/MM/yyyy HH:mm"),
                        ocorrenciaPatio.Descricao,
                        Tracao = ocorrenciaPatio.Tracao?.Placa_Formatada,
                        Reboques = string.Join(", ", (from reboque in ocorrenciaPatio.Reboques select reboque.Placa_Formatada)),
                        Tipo = ocorrenciaPatio.OcorrenciaPatioTipo?.Descricao,
                        DescricaoSituacao = ocorrenciaPatio.Situacao.ObterDescricao(),
                        DT_RowColor = ocorrenciaPatio.Situacao.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaOcorrenciaPatioRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "SituacaoDescricao")
                return "Situacao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
