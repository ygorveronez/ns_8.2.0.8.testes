using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "BuscarPorTipoAlerta" }, "Logistica/AlertaTratativaAcao", "Logistica/Monitoramento", "TorreControle/AcompanhamentoCarga")]
    public class AlertaTratativaAcaoController : BaseController
    {
		#region Construtores

		public AlertaTratativaAcaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao reg = new Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao();

                try
                {
                    PreencherDados(reg);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);

                repositorio.Inserir(reg, Auditado);

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
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao reg = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherDados(reg);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(reg, Auditado);

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
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao reg = repositorio.BuscarPorCodigo(codigo);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    reg.Codigo,
                    reg.Descricao,
                    reg.TipoAlerta,
                    reg.AcaoMonitorada,
                    Status = reg.Ativo,
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

        public async Task<IActionResult> BuscarPorTipoAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = repAlerta.BuscarPorCodigo(codigo);
                if (alerta == null) return new JsonpResult(false, true, $"Alerta não encontrado.");

                List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> tiposAcao = repositorio.BuscarPorTipoDeAlerta(alerta.TipoAlerta);
                if (tiposAcao == null || tiposAcao.Count == 0) return new JsonpResult(false, true, $"Nenhuma ação de tratativa encontrada para o alerta \"{Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(alerta.TipoAlerta)}\".");

                return new JsonpResult(new
                {
                    Codigo = alerta.Codigo,
                    DescricaoAlerta =  alerta.TipoAlerta.ObterDescricao()+ "." + " Carga: " + alerta.Carga?.CodigoCargaEmbarcador ?? "",
                    TiposAcao = from tipoAcao in tiposAcao
                                select new
                                {
                                    text = tipoAcao.Descricao,
                                    value = tipoAcao.Codigo
                                }
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
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao reg = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(reg, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                if ((excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException))))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;
                    if (excecaoSql.Number == 547)
                    {
                        return new JsonpResult(false, "Registro possui dependência e não pode ser excluído");
                    }
                }

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

        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao reg)
        {
            var descricao = Request.Params("Descricao");

            var tipoAlerta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>("tipoAlerta");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new Exception("Descrição é obrigatória.");

            if (descricao.Length > 100)
                throw new Exception("Descrição não pode passar de 100 caracteres.");

            if (tipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta)
                throw new Exception("Um tipo de alerta deve ser informado.");

            SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

            reg.Ativo = Request.GetBoolParam("Status");
            reg.AcaoMonitorada = Request.GetBoolParam("AcaoMonitorada");
            reg.Descricao = descricao;
            reg.TipoAlerta = tipoAlerta;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");
                SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de alerta", "Tipo", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "Ativo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> listaConsulta = repositorio.Consultar(descricao, situacaoAtivo, parametrosConsulta);

                int totalRegistros = repositorio.ContarConsulta(descricao, situacaoAtivo);
                var listaRetornar = (
                    from reg in listaConsulta
                    select new
                    {
                        reg.Codigo,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(reg.TipoAlerta),
                        reg.Descricao,
                        Ativo = reg.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
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
