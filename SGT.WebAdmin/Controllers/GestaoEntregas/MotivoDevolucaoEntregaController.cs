using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    [CustomAuthorize("GestaoEntregas/MotivoDevolucaoEntrega")]
    public class MotivoDevolucaoEntregaController : BaseController
    {
		#region Construtores

		public MotivoDevolucaoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo = repMotivoDevolucaoEntrega.BuscarPorCodigo(codigo);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    motivo.Codigo,
                    motivo.Ativo,
                    MotivoChamado = new { Codigo = motivo.MotivoChamado?.Codigo ?? 0, Descricao = motivo.MotivoChamado?.Descricao ?? "" },
                    motivo.Descricao,
                    motivo.ObrigarFoto,
                    motivo.Observacao,
                    ChecklistSuperApp = new { Codigo = motivo.ChecklistSuperApp?.Codigo ?? 0, Descricao = motivo.ChecklistSuperApp?.Descricao ?? "" },
                    TipoOcorrencia = new { Codigo = motivo.TipoOcorrencia?.Codigo ?? 0, Descricao = motivo.TipoOcorrencia?.Descricao ?? "" },
                    motivo.EntregaParcialSuperAppId,
                    motivo.NaoEntregaSuperAppId,
                };

                return new JsonpResult(retorno);
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
                unitOfWork.Start();

                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo = new Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega();
                PreencheEntidade(ref motivo, unitOfWork);

                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                repMotivoDevolucaoEntrega.Inserir(motivo, Auditado);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
                if (configuracaoIntegracaoTrizy != null && configuracaoIntegracaoTrizy.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3)
                {
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarMotivoDevolucaoEntregaEntregaParcial(motivo, unitOfWork);
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarMotivoDevolucaoEntregaNaoEntregue(motivo, unitOfWork);
                }

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
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo = repMotivoDevolucaoEntrega.BuscarPorCodigo(codigo, true);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(ref motivo, unitOfWork);

                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                repMotivoDevolucaoEntrega.Atualizar(motivo, Auditado);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
                if (configuracaoIntegracaoTrizy != null && configuracaoIntegracaoTrizy.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3)
                {
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarMotivoDevolucaoEntregaEntregaParcial(motivo, unitOfWork);
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarMotivoDevolucaoEntregaNaoEntregue(motivo, unitOfWork);
                }

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo = repMotivoDevolucaoEntrega.BuscarPorCodigo(codigo);

                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMotivoDevolucaoEntrega.Deletar(motivo, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("MotivoChamado").Nome("Chamado").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoAtivo").Nome("Status").Tamanho(5).Align(Models.Grid.Align.left);

            return grid;
        }


        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> listaGrid = repMotivoDevolucaoEntrega.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);

            totalRegistros = repMotivoDevolucaoEntrega.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            MotivoChamado = obj.MotivoChamado?.Descricao ?? "",
                            obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.SuperApp.ChecklistSuperApp repChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            motivo.Descricao = Request.GetStringParam("Descricao");
            motivo.Ativo = Request.GetBoolParam("Status");
            motivo.ObrigarFoto = Request.GetBoolParam("ObrigarFoto");
            motivo.Observacao = Request.GetStringParam("Observacao");
            
            int checklistSuperAppId = Request.GetIntParam("ChecklistSuperApp");
            motivo.ChecklistSuperApp = checklistSuperAppId > 0? repChecklistSuperApp.BuscarPorCodigo(checklistSuperAppId, false) : null;

            int tipoDeOcorrenciaId = Request.GetIntParam("TipoOcorrencia");
            motivo.TipoOcorrencia = tipoDeOcorrenciaId > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoDeOcorrenciaId, false) : null;

            int motivoChamado = Request.GetIntParam("MotivoChamado");
            if (motivoChamado > 0)
                motivo.MotivoChamado = repMotivoChamado.BuscarPorCodigo(motivoChamado);
            else
                motivo.MotivoChamado = null;
        }


        private bool ValidaEntidade(Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo, out string msgErro)
        {
            msgErro = "";


            if (string.IsNullOrWhiteSpace(motivo.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (motivo.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            if (motivo.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode passar de 2000 caracteres.";
                return false;
            }

            return true;
        }


        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }
        #endregion
    }
}
