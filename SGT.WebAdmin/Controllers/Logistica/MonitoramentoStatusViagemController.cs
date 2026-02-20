using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Logistica/MonitoramentoStatusViagem")]
    public class MonitoramentoStatusViagemController : BaseController
    {
		#region Construtores

		public MonitoramentoStatusViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string sigla = Request.Params("Sigla");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Ordem", "Ordem", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sigla", "Sigla", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Grupo", "Grupo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo de regra", "TipoRegra", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("ValidarStatusCargaAoTrocarStatusViagem", false);
                grid.AdicionarCabecalho("StatusCargaAoTrocarStatusViagem", false);

                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> rows = repMonitoramentoStatusViagem.Consultar(descricao, sigla, -1, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMonitoramentoStatusViagem.ContarConsulta(descricao, sigla, -1, ativo));

                var lista = (from r in rows
                             select new
                             {
                                 r.Codigo,
                                 r.Ordem,
                                 r.Descricao,
                                 r.Sigla,
                                 Grupo = r.Grupo.Descricao,
                                 TipoRegra = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegraHelper.ObterDescricao(r.TipoRegra),
                                 r.DescricaoAtivo,
                                 r.ValidarStatusCargaAoTrocarStatusViagem,
                                 StatusCargaAoTrocarStatusViagem = (int) r.StatusCargaAoTrocarStatusViagem
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
                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);

                string descricao = Request.GetStringParam("Descricao");
                string sigla = Request.GetStringParam("Sigla");
                int grupo = Request.GetIntParam("Grupo");
                int ordem = Request.GetIntParam("Ordem");
                bool ativo = Request.GetBoolParam("Ativo");
                string cor = Request.GetStringParam("Cor");
                bool naoUtilizarStatusParaCalculoTemperaturaDentroFaixa = Request.GetBoolParam("NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra) Request.GetIntParam("TipoRegra");
                bool validarStatusCargaAoTrocarStatusViagem = Request.GetBoolParam("validarStatusCargaAoTrocarStatusViagem");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento statusCargaAoTrocarStatusViagem = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento)Request.GetIntParam("statusCargaAoTrocarStatusViagem");

                // Não pode existir dois status com a mesma regra
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagemExistente = repMonitoramentoStatusViagem.BuscarPorTipoRegra(tipoRegra);
                if (monitoramentoStatusViagemExistente != null)
                    return new JsonpResult(false, $"Já existe outro status cadastrado com a regra {Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegraHelper.ObterDescricao(tipoRegra)}");

                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem monitoramentoGrupoStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarPorCodigo(grupo, false);
                if (monitoramentoGrupoStatusViagem == null)
                    return new JsonpResult(false, $"Grupo não encontrado.");

                // Cria um novo status
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem()
                {
                    Descricao = descricao,
                    Sigla = sigla,
                    Grupo = monitoramentoGrupoStatusViagem,
                    Ordem = ordem,
                    TipoRegra = tipoRegra,
                    Cor = cor,
                    Ativo = ativo,
                    DataCadastro = DateTime.Now,
                    ValidarStatusCargaAoTrocarStatusViagem = validarStatusCargaAoTrocarStatusViagem,
                    StatusCargaAoTrocarStatusViagem = validarStatusCargaAoTrocarStatusViagem ? statusCargaAoTrocarStatusViagem : 0,
                    NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa = naoUtilizarStatusParaCalculoTemperaturaDentroFaixa
                };
                
                repMonitoramentoStatusViagem.Inserir(monitoramentoStatusViagem, Auditado);

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
                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                string descricao = Request.GetStringParam("Descricao");
                int grupo = Request.GetIntParam("Grupo");
                string sigla = Request.GetStringParam("Sigla");
                int ordem = Request.GetIntParam("Ordem");
                string cor = Request.GetStringParam("Cor");
                bool ativo = Request.GetBoolParam("Ativo");
                bool naoUtilizarStatusParaCalculoTemperaturaDentroFaixa = Request.GetBoolParam("NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra)Request.GetIntParam("TipoRegra");
                bool validarStatusCargaAoTrocarStatusViagem = Request.GetBoolParam("validarStatusCargaAoTrocarStatusViagem");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento statusCargaAoTrocarStatusViagem = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento)Request.GetIntParam("statusCargaAoTrocarStatusViagem");

                // Não pode existir dois status com a mesma regra
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagemExistente = repMonitoramentoStatusViagem.BuscarPorTipoRegra(tipoRegra);
                if (monitoramentoStatusViagemExistente != null && monitoramentoStatusViagemExistente.Codigo != codigo)
                    return new JsonpResult(false, $"Já existe outro status cadastrado com a regra {Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegraHelper.ObterDescricao(tipoRegra)}");

                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem = repMonitoramentoStatusViagem.BuscarPorCodigo(codigo, true);
                if (monitoramentoStatusViagem == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem monitoramentoGrupoStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarPorCodigo(grupo, false);
                if (monitoramentoGrupoStatusViagem == null)
                    return new JsonpResult(false, $"Grupo não encontrado.");

                monitoramentoStatusViagem.Descricao = descricao;
                monitoramentoStatusViagem.Sigla = sigla;
                monitoramentoStatusViagem.Grupo = monitoramentoGrupoStatusViagem;
                monitoramentoStatusViagem.Ordem = ordem;
                monitoramentoStatusViagem.TipoRegra = tipoRegra;
                monitoramentoStatusViagem.Cor = cor;
                monitoramentoStatusViagem.Ativo = ativo;
                monitoramentoStatusViagem.ValidarStatusCargaAoTrocarStatusViagem = validarStatusCargaAoTrocarStatusViagem;
                monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem = validarStatusCargaAoTrocarStatusViagem ? statusCargaAoTrocarStatusViagem : 0;
                monitoramentoStatusViagem.NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa = naoUtilizarStatusParaCalculoTemperaturaDentroFaixa;

                repMonitoramentoStatusViagem.Atualizar(monitoramentoStatusViagem, Auditado);

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
                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> rows = repMonitoramentoStatusViagem.BuscarAtivos();

                var result = (from r in rows
                             select new 
                             {
                                 value = r.Codigo,
                                 text = r.Descricao,
                                 selected = (r.TipoRegra != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando) ? "selected" : ""
                             }).ToList();
                
                result.Insert(0, new
                {
                    value = -1,
                    text = "Sem status",
                    selected = "selected"
                });

                return new JsonpResult(new{
                    StatusViagem = result
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
                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem = repMonitoramentoStatusViagem.BuscarPorCodigo(codigo, false);
                return new JsonpResult(new
                {
                    monitoramentoStatusViagem.Codigo,
                    monitoramentoStatusViagem.Descricao,
                    Grupo = monitoramentoStatusViagem.Grupo.Codigo,
                    monitoramentoStatusViagem.Sigla,
                    monitoramentoStatusViagem.Ordem,
                    monitoramentoStatusViagem.TipoRegra,
                    monitoramentoStatusViagem.Cor,
                    monitoramentoStatusViagem.Ativo,
                    monitoramentoStatusViagem.ValidarStatusCargaAoTrocarStatusViagem,
                    monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem,
                    monitoramentoStatusViagem.NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa
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
                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem = repMonitoramentoStatusViagem.BuscarPorCodigo(codigo, false);
                if (monitoramentoStatusViagem == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                repMonitoramentoStatusViagem.Deletar(monitoramentoStatusViagem, Auditado);
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
