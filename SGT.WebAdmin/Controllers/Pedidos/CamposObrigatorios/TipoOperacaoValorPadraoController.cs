using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.CamposObrigatorios
{
    [CustomAuthorize("Pedidos/TipoOperacaoValorPadrao")]
    public class TipoOperacaoValorPadraoController : BaseController
    {
		#region Construtores

		public TipoOperacaoValorPadraoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao repTipoOperacaoValorPadrao = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao tipoOperacaoValorPadrao = new Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao();


                PreencherEntidade(tipoOperacaoValorPadrao, unitOfWork);
                
                unitOfWork.Start();

                repTipoOperacaoValorPadrao.Inserir(tipoOperacaoValorPadrao, Auditado);

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

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao repTipoOperacaoValorPadrao = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao tipoOperacaoValorPadrao = repTipoOperacaoValorPadrao.BuscarPorCodigo(codigo, false);
                tipoOperacaoValorPadrao.Habilitar = Request.GetBoolParam("Habilitar");
                if (tipoOperacaoValorPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

              //  PreencherEntidade(pedidoCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repTipoOperacaoValorPadrao.Atualizar(tipoOperacaoValorPadrao, Auditado);

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
            using var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao repTipoOperacaoValorPadrao = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao tipoOperacaoValorPadrao = repTipoOperacaoValorPadrao.BuscarPorCodigo(codigo, false);



                if (tipoOperacaoValorPadrao == null)
                {
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                }

                return new JsonpResult(new
                {
                    Codigo = tipoOperacaoValorPadrao.Codigo,
                    Habilitar = tipoOperacaoValorPadrao.Habilitar,
                    TipoOperacao = new
                    {
                        Descricao = tipoOperacaoValorPadrao.Campo.Descricao,
                        Codigo = tipoOperacaoValorPadrao.Codigo
                    },

                }); 
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }


        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");


                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao repTipoOperacaoValorPadrao = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao tipoOperacaoValorPadrao = repTipoOperacaoValorPadrao.BuscarPorCodigo(codigo, false);

                if (tipoOperacaoValorPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                //tipoOperacaoValorPadrao.Campo = null;

                repTipoOperacaoValorPadrao.Deletar(tipoOperacaoValorPadrao, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao tipoOperacaoValorPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo repTipoOperacaoCampo = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo(unitOfWork);

            var codigo = Request.GetIntParam("TipoOperacao");
            var habilitar = Request.GetBoolParam("Habilitar");
            codigo = Convert.ToInt32(codigo);
            tipoOperacaoValorPadrao.Habilitar = habilitar;
            tipoOperacaoValorPadrao.Campo = repTipoOperacaoCampo.BuscarPorCodigo(codigo, false);
            
          
        }
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTipoOperacao = Request.GetIntParam("Descricao");
                bool? habilitado = Request.GetNullableBoolParam("Habilitar");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 80, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Padrão", "Habilitar", 10, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao repTipoOperacaoValorPadrao = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao> listaTipooperacaoValorPadraoCampos = repTipoOperacaoValorPadrao.Consultar(codigoTipoOperacao, habilitado, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTipoOperacaoValorPadrao.ContarConsulta(codigoTipoOperacao, habilitado);

                var retorno = listaTipooperacaoValorPadraoCampos.Select(o => new
                {
                    o.Codigo,
                    Descricao = o.Campo?.Descricao,
                    Habilitar = o.Habilitar?"SIM":"NÃO"
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
