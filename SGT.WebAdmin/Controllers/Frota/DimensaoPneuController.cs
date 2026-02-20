using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/DimensaoPneu")]
    public class DimensaoPneuController : BaseController
    {
		#region Construtores

		public DimensaoPneuController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu = new Dominio.Entidades.Embarcador.Frota.DimensaoPneu();

                try
                {
                    PreencherDimensaoPneu(dimensaoPneu);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.DimensaoPneu repositorio = new Repositorio.Embarcador.Frota.DimensaoPneu(unitOfWork);

                repositorio.Inserir(dimensaoPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.DimensaoPneu repositorio = new Repositorio.Embarcador.Frota.DimensaoPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (dimensaoPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherDimensaoPneu(dimensaoPneu);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(dimensaoPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

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
                Repositorio.Embarcador.Frota.DimensaoPneu repositorio = new Repositorio.Embarcador.Frota.DimensaoPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu = repositorio.BuscarPorCodigo(codigo);

                if (dimensaoPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    dimensaoPneu.Codigo,
                    dimensaoPneu.Aplicacao,
                    dimensaoPneu.Aro,
                    dimensaoPneu.Largura,
                    dimensaoPneu.Perfil,
                    dimensaoPneu.Radial,
                    Status = dimensaoPneu.Ativo
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.DimensaoPneu repositorio = new Repositorio.Embarcador.Frota.DimensaoPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (dimensaoPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(dimensaoPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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

        private void PreencherDimensaoPneu(Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu)
        {
            string aplicacao = Request.GetStringParam("Aplicacao");

            if (string.IsNullOrWhiteSpace(aplicacao))
                throw new ControllerException("Aplicação é obrigatória.");

            if (aplicacao.Length > 50)
                throw new ControllerException("Aplicação não pode passar de 50 caracteres.");

            dimensaoPneu.Aplicacao = aplicacao;
            dimensaoPneu.Aro = Request.GetDecimalParam("Aro");
            dimensaoPneu.Ativo = Request.GetBoolParam("Status");
            dimensaoPneu.Largura = Request.GetIntParam("Largura");
            dimensaoPneu.Perfil = Request.GetIntParam("Perfil");
            dimensaoPneu.Radial = Request.GetBoolParam("Radial");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                dimensaoPneu.Empresa = this.Usuario.Empresa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDimensaoPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDimensaoPneu()
                {
                    Aplicacao = Request.GetStringParam("Aplicacao"),
                    SituacaoAtivo = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Aplicação", "Descricao", 50, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.DimensaoPneu repositorio = new Repositorio.Embarcador.Frota.DimensaoPneu(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.DimensaoPneu> listaDimensaoPneu = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaDimensaoPneuRetornar = (
                    from dimensaoPneu in listaDimensaoPneu 
                    select new
                    {
                        dimensaoPneu.Codigo,
                        Descricao = dimensaoPneu.Aplicacao,
                        dimensaoPneu.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaDimensaoPneuRetornar);
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
