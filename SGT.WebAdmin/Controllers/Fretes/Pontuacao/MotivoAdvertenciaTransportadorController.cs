using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/MotivoAdvertenciaTransportador")]
    public class MotivoAdvertenciaTransportadorController : BaseController
    {
		#region Construtores

		public MotivoAdvertenciaTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivoAdvertenciaTransportador = new Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador();

                PreencherMotivoAdvertenciaTransportador(motivoAdvertenciaTransportador);

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);

                repositorio.Inserir(motivoAdvertenciaTransportador, Auditado);

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
                Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivoAdvertenciaTransportador = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoAdvertenciaTransportador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivoAdvertenciaTransportador(motivoAdvertenciaTransportador);

                unitOfWork.Start();

                repositorio.Atualizar(motivoAdvertenciaTransportador, Auditado);

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
                Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivoAdvertenciaTransportador = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoAdvertenciaTransportador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoAdvertenciaTransportador.Codigo,
                    motivoAdvertenciaTransportador.Descricao,
                    Status = motivoAdvertenciaTransportador.Ativo,
                    motivoAdvertenciaTransportador.Observacao,
                    motivoAdvertenciaTransportador.Pontuacao
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
                Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivoAdvertenciaTransportador = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoAdvertenciaTransportador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivoAdvertenciaTransportador, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherMotivoAdvertenciaTransportador(Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivoAdvertenciaTransportador)
        {
            string descricao = Request.GetStringParam("Descricao");
            string observacao = Request.GetStringParam("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new ControllerException("Observação não pode passar de 2000 caracteres.");

            motivoAdvertenciaTransportador.Ativo = Request.GetBoolParam("Status");
            motivoAdvertenciaTransportador.Descricao = descricao;
            motivoAdvertenciaTransportador.Observacao = observacao;
            motivoAdvertenciaTransportador.Pontuacao = Request.GetIntParam("Pontuacao");
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
                grid.AdicionarCabecalho("Pontuação", "Pontuacao", 15, Models.Grid.Align.center, false);

                if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacaoAtivo);
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador> listaMotivoAdvertenciaTransportador = (totalRegistros > 0) ? repositorio.Consultar(descricao, situacaoAtivo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador>();

                var listaMotivoAdvertenciaTransportadorRetornar = (
                    from motivo in listaMotivoAdvertenciaTransportador
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo,
                        motivo.Pontuacao
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoAdvertenciaTransportadorRetornar);
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
