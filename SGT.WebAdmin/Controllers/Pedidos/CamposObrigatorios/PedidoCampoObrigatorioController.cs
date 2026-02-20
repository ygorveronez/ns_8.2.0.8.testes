using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.CamposObrigatorios
{
    [CustomAuthorize("Pedidos/PedidoCampoObrigatorio")]
    public class PedidoCampoObrigatorioController : BaseController
    {
		#region Construtores

		public PedidoCampoObrigatorioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio = new Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio();

                PreencherEntidade(pedidoCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repPedidoCampoObrigatorio.Inserir(pedidoCampoObrigatorio, Auditado);

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

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio = repPedidoCampoObrigatorio.BuscarPorCodigo(codigo, true);

                if (pedidoCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(pedidoCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repPedidoCampoObrigatorio.Atualizar(pedidoCampoObrigatorio, Auditado);

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

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio = repPedidoCampoObrigatorio.BuscarPorCodigo(codigo, false);

                if (pedidoCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Codigo = duplicar ? 0 : pedidoCampoObrigatorio.Codigo,
                    Situacao = duplicar ? true : pedidoCampoObrigatorio.Ativo,
                    TipoOperacao = new
                    {
                        Descricao = pedidoCampoObrigatorio.TipoOperacao?.Descricao ?? string.Empty,
                        Codigo = pedidoCampoObrigatorio.TipoOperacao?.Codigo ?? 0
                    },
                    Campos = pedidoCampoObrigatorio.Campos.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao
                    }).ToList(),
                    pedidoCampoObrigatorio.ObrigatorioInformarProdutoPedido
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

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio = repPedidoCampoObrigatorio.BuscarPorCodigo(codigo, true);

                if (pedidoCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                pedidoCampoObrigatorio.Campos = null;

                repPedidoCampoObrigatorio.Deletar(pedidoCampoObrigatorio, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarParaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio = repPedidoCampoObrigatorio.BuscarParaPedido(codigoTipoOperacao);

                if (pedidoCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pedidoCampoObrigatorio.Codigo,
                    TipoOperacao = new
                    {
                        Descricao = pedidoCampoObrigatorio.TipoOperacao?.Descricao ?? string.Empty,
                        Codigo = pedidoCampoObrigatorio.TipoOperacao?.Codigo ?? 0
                    },
                    Campos = pedidoCampoObrigatorio.Campos.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Campo
                    }).ToList(),
                    pedidoCampoObrigatorio.ObrigatorioInformarProdutoPedido
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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

            pedidoCampoObrigatorio.TipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
            pedidoCampoObrigatorio.Ativo = Request.GetBoolParam("Situacao");
            pedidoCampoObrigatorio.ObrigatorioInformarProdutoPedido = Request.GetBoolParam("ObrigatorioInformarProdutoPedido");

            if (repPedidoCampoObrigatorio.ExistePorTipoOperacao(pedidoCampoObrigatorio.Codigo, pedidoCampoObrigatorio.TipoOperacao?.Codigo ?? 0))
                throw new ControllerException("Já existe uma configuração para este tipo de operação.");

            PreencherCamposEntidade(pedidoCampoObrigatorio, unitOfWork);
        }

        private void PreencherCamposEntidade(Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio pedidoCampoObrigatorio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo repPedidoCampo = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo(unitOfWork);

            dynamic campos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Campos"));

            if (pedidoCampoObrigatorio.Campos == null)
            {
                pedidoCampoObrigatorio.Campos = new List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic campo in campos)
                    codigos.Add((int)campo.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo> camposDeletar = pedidoCampoObrigatorio.Campos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo campoDeletar in camposDeletar)
                    pedidoCampoObrigatorio.Campos.Remove(campoDeletar);
            }

            foreach (dynamic campo in campos)
            {
                Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo pedidoCampo = repPedidoCampo.BuscarPorCodigo((int)campo.Codigo, false);
                pedidoCampoObrigatorio.Campos.Add(pedidoCampo);
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

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio repPedidoCampoObrigatorio = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> listaPedidoCamposObrigatorios = repPedidoCampoObrigatorio.Consultar(codigoTipoOperacao, ativo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPedidoCampoObrigatorio.ContarConsulta(codigoTipoOperacao, ativo);

                var retorno = listaPedidoCamposObrigatorios.Select(o => new
                {
                    o.Codigo,
                    TipoOperacao = o.TipoOperacao?.Descricao,
                    Ativo = o.DescricaoAtivo
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
