using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Logistica/MonitoramentoGrupoStatusViagem")]
    public class MonitoramentoGrupoStatusViagemController : BaseController
    {
		#region Construtores

		public MonitoramentoGrupoStatusViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Ordem", "Ordem", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 35, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem> rows = repMonitoramentoGrupoStatusViagem.Consultar(descricao, -1, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMonitoramentoGrupoStatusViagem.ContarConsulta(descricao, -1, ativo));

                var lista = (from r in rows
                             select new
                             {
                                 r.Codigo,
                                 r.Ordem,
                                 r.Descricao,
                                 r.DescricaoAtivo
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);

                string descricao = Request.GetStringParam("Descricao");
                int ordem = Request.GetIntParam("Ordem");
                bool ativo = Request.GetBoolParam("Ativo");
                string cor = Request.GetStringParam("Cor");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra) Request.GetIntParam("TipoRegra");

                // Cria um novo status
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem monitoramentoGrupoStatusViagem = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem()
                {
                    Descricao = descricao,
                    Ordem = ordem,
                    Cor = cor,
                    Ativo = ativo,
                    DataCadastro = DateTime.Now
                };
                repMonitoramentoGrupoStatusViagem.Inserir(monitoramentoGrupoStatusViagem, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                string descricao = Request.GetStringParam("Descricao");
                int ordem = Request.GetIntParam("Ordem");
                string cor = Request.GetStringParam("Cor");
                bool ativo = Request.GetBoolParam("Ativo");

                Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem monitoramentoGrupoStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarPorCodigo(codigo, true);
                if (monitoramentoGrupoStatusViagem == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                monitoramentoGrupoStatusViagem.Descricao = descricao;
                monitoramentoGrupoStatusViagem.Ordem = ordem;
                monitoramentoGrupoStatusViagem.Cor = cor;
                monitoramentoGrupoStatusViagem.Ativo = ativo;
                repMonitoramentoGrupoStatusViagem.Atualizar(monitoramentoGrupoStatusViagem, Auditado);
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem> rows = repMonitoramentoGrupoStatusViagem.BuscarAtivos();

                var result = (from r in rows
                             select new 
                             {
                                 value = r.Codigo,
                                 text = r.Descricao,
                                 selected = "selected"
                             }).ToList();

                result.Add(new
                {
                    value = -1,
                    text = "Nenhum",
                    selected = "selected"
                });

                return new JsonpResult(new{
                    GrupoStatusViagem = result
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem monitoramentoGrupoStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarPorCodigo(codigo, false);
                return new JsonpResult(new
                {
                    monitoramentoGrupoStatusViagem.Codigo,
                    monitoramentoGrupoStatusViagem.Descricao,
                    monitoramentoGrupoStatusViagem.Ordem,
                    monitoramentoGrupoStatusViagem.Cor,
                    monitoramentoGrupoStatusViagem.Ativo,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem monitoramentoGrupoStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarPorCodigo(codigo, false);
                if (monitoramentoGrupoStatusViagem == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                repMonitoramentoGrupoStatusViagem.Deletar(monitoramentoGrupoStatusViagem, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

