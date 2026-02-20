using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.GestaoCarga
{
    [CustomAuthorize("Cargas/ServicoInspecaoFederal")]
    public class ServicoInspecaoFederalController : BaseController
    {
		#region Construtores

		public ServicoInspecaoFederalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ServicoInspecaoFederal repositorioSIF = new Repositorio.Embarcador.Cargas.ServicoInspecaoFederal(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal servicoInspecaoFederal = new Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal();

                PreencherDados(servicoInspecaoFederal);
                if (repositorioSIF.VerificaCodigoSIFCodigoIntegracao(servicoInspecaoFederal))
                    return new JsonpResult(true, "Já existe um registro com este Código SIF/Código de Integração.");

                repositorioSIF.Inserir(servicoInspecaoFederal);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ServicoInspecaoFederal repositorioSIF = new Repositorio.Embarcador.Cargas.ServicoInspecaoFederal(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal servicoInspecaoFederal = repositorioSIF.BuscarPorCodigo(codigo, false);

                if (servicoInspecaoFederal == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                PreencherDados(servicoInspecaoFederal);
                if (repositorioSIF.VerificaCodigoSIFCodigoIntegracao(servicoInspecaoFederal))
                    return new JsonpResult(true, "Já existe um registro com este Código SIF/Código de Integração.");

                repositorioSIF.Atualizar(servicoInspecaoFederal);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Cargas.ServicoInspecaoFederal repositorioSIF = new Repositorio.Embarcador.Cargas.ServicoInspecaoFederal(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal servicoInspecaoFederal = repositorioSIF.BuscarPorCodigo(codigo, false);

                if (servicoInspecaoFederal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    servicoInspecaoFederal.CodigoSIF,
                    Situacao = servicoInspecaoFederal.Ativo,
                    servicoInspecaoFederal.CodigoIntegracao,
                    servicoInspecaoFederal.Descricao

                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.ServicoInspecaoFederal repositorioSIF = new Repositorio.Embarcador.Cargas.ServicoInspecaoFederal(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal servicoInspecaoFederal = repositorioSIF.BuscarPorCodigo(codigo, false);

                repositorioSIF.Deletar(servicoInspecaoFederal);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possivel excluir registro!");
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código SIF", "CodigoSIF", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, true);

            Repositorio.Embarcador.Cargas.ServicoInspecaoFederal repositorioSIF = new Repositorio.Embarcador.Cargas.ServicoInspecaoFederal(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSIF filtrosPesquisa = ObterFiltrosPesquisa();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repositorioSIF.ContarConsulta(filtrosPesquisa);

            List<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal> listaSIF = totalRegistros > 0 ? repositorioSIF.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal>();

            var lista = (from p in listaSIF
                         select new
                         {
                             p.Codigo,
                             p.CodigoSIF,
                             p.Descricao,
                             Situacao = p.Ativo ? "Ativo" : "Inativo",
                             p.CodigoIntegracao
                         }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal servicoInspecaoFederal)
        {
            servicoInspecaoFederal.CodigoIntegracao = Request.GetStringParam("Codigointegracao");
            servicoInspecaoFederal.Descricao = Request.GetStringParam("Descricao");
            servicoInspecaoFederal.CodigoSIF = Request.GetStringParam("CodigoSIF");
            servicoInspecaoFederal.Ativo = Request.GetBoolParam("Situacao");
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSIF ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSIF()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Status"),
                CodigoSIF = Request.GetStringParam("CodigoSIF")
            };
        }
        #endregion
    }
}
