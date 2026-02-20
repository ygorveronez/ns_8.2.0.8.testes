using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/MotoristaCampoObrigatorio")]
    public class MotoristaCampoObrigatorioController : BaseController
    {
		#region Construtores

		public MotoristaCampoObrigatorioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarMotoristaCampoObrigatorioPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.MotoristaCampoObrigatorio repMotoristaCampoObrigatorio = new Repositorio.MotoristaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio = repMotoristaCampoObrigatorio.ObterMotoristaCampoObrigatorio();

                if (motoristaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Codigo = motoristaCampoObrigatorio.Codigo,
                    Campos = motoristaCampoObrigatorio.Campos.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Campo
                    }).ToList()
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.MotoristaCampoObrigatorio repMotoristaCampoObrigatorio = new Repositorio.MotoristaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio = new Dominio.Entidades.MotoristaCampoObrigatorio();

                PreencherEntidade(motoristaCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repMotoristaCampoObrigatorio.Inserir(motoristaCampoObrigatorio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
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

                Repositorio.MotoristaCampoObrigatorio repMotoristaCampoObrigatorio = new Repositorio.MotoristaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio = repMotoristaCampoObrigatorio.BuscarPorCodigo(codigo, true);

                if (motoristaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(motoristaCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repMotoristaCampoObrigatorio.Atualizar(motoristaCampoObrigatorio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
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
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.MotoristaCampoObrigatorio repMotoristaCampoObrigatorio = new Repositorio.MotoristaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio = repMotoristaCampoObrigatorio.BuscarPorCodigo(codigo, false);

                if (motoristaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Codigo = duplicar ? 0 : motoristaCampoObrigatorio.Codigo,
                    Campos = motoristaCampoObrigatorio.Campos.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao
                    }).ToList()
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

                Repositorio.MotoristaCampoObrigatorio repMotoristaCampoObrigatorio = new Repositorio.MotoristaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio = repMotoristaCampoObrigatorio.BuscarPorCodigo(codigo, true);

                if (motoristaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                motoristaCampoObrigatorio.Campos = null;

                repMotoristaCampoObrigatorio.Deletar(motoristaCampoObrigatorio, Auditado);

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
                Models.Grid.Grid grid = ObterGridPesquisa();

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

        private void PreencherEntidade(Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio, Repositorio.UnitOfWork unitOfWork)
        {
            PreencherCamposEntidade(motoristaCampoObrigatorio, unitOfWork);
        }

        private void PreencherCamposEntidade(Dominio.Entidades.MotoristaCampoObrigatorio motoristaCampoObrigatorio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.MotoristaCampo repPedidoCampo = new Repositorio.Embarcador.Transportadores.MotoristaCampo(unitOfWork);

            dynamic campos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Campos"));

            if (motoristaCampoObrigatorio.Campos == null)
            {
                motoristaCampoObrigatorio.Campos = new List<Dominio.Entidades.MotoristaCampo>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic campo in campos)
                    codigos.Add((int)campo.Codigo);

                List<Dominio.Entidades.MotoristaCampo> camposDeletar = motoristaCampoObrigatorio.Campos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.MotoristaCampo campoDeletar in camposDeletar)
                    motoristaCampoObrigatorio.Campos.Remove(campoDeletar);
            }

            foreach (dynamic campo in campos)
            {
                Dominio.Entidades.MotoristaCampo pedidoCampo = repPedidoCampo.BuscarPorCodigo((int)campo.Codigo, false);
                motoristaCampoObrigatorio.Campos.Add(pedidoCampo);
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                bool? ativo = Request.GetNullableBoolParam("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 80, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Ativo", 10, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.MotoristaCampoObrigatorio repMotoristaCampoObrigatorio = new Repositorio.MotoristaCampoObrigatorio(unitOfWork);

                List<Dominio.Entidades.MotoristaCampoObrigatorio> listaPedidoCamposObrigatorios = repMotoristaCampoObrigatorio.Consultar(codigoTipoOperacao, ativo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repMotoristaCampoObrigatorio.ContarConsulta(codigoTipoOperacao, ativo);

                var retorno = listaPedidoCamposObrigatorios.Select(o => new
                {
                    o.Codigo,
                }).ToList();

                grid.AdicionaRows(retorno);
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
            return propriedadeOrdenar;
        }

        #endregion
    }
}
