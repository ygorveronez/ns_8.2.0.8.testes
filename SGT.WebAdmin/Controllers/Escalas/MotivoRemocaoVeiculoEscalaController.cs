using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escalas
{
    [CustomAuthorize("Escalas/MotivoRemocaoVeiculoEscala")]
    public class MotivoRemocaoVeiculoEscalaController : BaseController
    {
		#region Construtores

		public MotivoRemocaoVeiculoEscalaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala motivoRemocaoVeiculoEscala = new Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala();

                PreencherMotivoRemocaoVeiculoEscala(motivoRemocaoVeiculoEscala);

                unitOfWork.Start();

                Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala repositorio = new Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala(unitOfWork);

                repositorio.Inserir(motivoRemocaoVeiculoEscala, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
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
                Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala repositorio = new Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala motivoRemocaoVeiculoEscala = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoRemocaoVeiculoEscala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivoRemocaoVeiculoEscala(motivoRemocaoVeiculoEscala);

                unitOfWork.Start();

                repositorio.Atualizar(motivoRemocaoVeiculoEscala, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
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
                Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala repositorio = new Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala motivoRemocaoVeiculoEscala = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoRemocaoVeiculoEscala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoRemocaoVeiculoEscala.Codigo,
                    motivoRemocaoVeiculoEscala.Descricao,
                    Status = motivoRemocaoVeiculoEscala.Ativo,
                    motivoRemocaoVeiculoEscala.Observacao
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
                Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala repositorio = new Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala motivoRemocaoVeiculoEscala = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoRemocaoVeiculoEscala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivoRemocaoVeiculoEscala, Auditado);

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

        private void PreencherMotivoRemocaoVeiculoEscala(Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala motivoRemocaoVeiculoEscala)
        {
            string descricao = Request.GetStringParam("Descricao");
            string observacao = Request.GetStringParam("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new ControllerException("Observação não pode passar de 2000 caracteres.");

            motivoRemocaoVeiculoEscala.Ativo = Request.GetBoolParam("Status");
            motivoRemocaoVeiculoEscala.Descricao = descricao;
            motivoRemocaoVeiculoEscala.Observacao = observacao;
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
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);

                if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala repositorio = new Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacaoAtivo);
                List<Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala> listaMotivoRemocaoVeiculoEscala = (totalRegistros > 0) ? repositorio.Consultar(descricao, situacaoAtivo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala>();

                var listaMotivoRemocaoVeiculoEscalaRetornar = (
                    from motivo in listaMotivoRemocaoVeiculoEscala
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoRemocaoVeiculoEscalaRetornar);
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
