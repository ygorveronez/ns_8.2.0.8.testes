using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/MarcaPneu")]
    public class MarcaPneuController : BaseController
    {
		#region Construtores

		public MarcaPneuController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = new Dominio.Entidades.Embarcador.Frota.MarcaPneu();

                try
                {
                    PreencherMarcaPneu(marcaPneu);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);

                repositorio.Inserir(marcaPneu, Auditado);

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
                Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (marcaPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherMarcaPneu(marcaPneu);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(marcaPneu, Auditado);

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
                Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = repositorio.BuscarPorCodigo(codigo);

                if (marcaPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    marcaPneu.Codigo,
                    marcaPneu.Descricao,
                    Status = marcaPneu.Ativo
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
                Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (marcaPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(marcaPneu, Auditado);

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

        private void PreencherMarcaPneu(Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu)
        {
            string descricao = Request.GetStringParam("Descricao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            marcaPneu.Descricao = descricao;
            marcaPneu.Ativo = Request.GetBoolParam("Status");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                marcaPneu.Empresa = this.Usuario.Empresa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMarcaPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMarcaPneu()
                {
                    Descricao = Request.GetStringParam("Descricao"),
                    SituacaoAtivo = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.MarcaPneu> listaMarcaPneu = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaMarcaPneuRetornar = (
                    from marcaPneu in listaMarcaPneu
                    select new
                    {
                        marcaPneu.Codigo,
                        marcaPneu.Descricao,
                        marcaPneu.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMarcaPneuRetornar);
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
