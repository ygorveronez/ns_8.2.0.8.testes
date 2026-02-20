using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Retornos
{
    [CustomAuthorize("Cargas/MotivoCancelamentoRetornoCargaColetaBackhaul")]
    public class MotivoCancelamentoRetornoCargaColetaBackhaulController : BaseController
    {
		#region Construtores

		public MotivoCancelamentoRetornoCargaColetaBackhaulController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul motivoCancelamento = new Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul();

                PreencherMotivoCancelamento(motivoCancelamento);

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul repositorio = new Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul(unitOfWork);

                repositorio.Inserir(motivoCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch(ControllerException excecao)
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
                Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul repositorio = new Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul motivoCancelamento = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivoCancelamento(motivoCancelamento);

                unitOfWork.Start();

                repositorio.Atualizar(motivoCancelamento, Auditado);

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
                Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul repositorio = new Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul motivoCancelamento = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoCancelamento.Codigo,
                    motivoCancelamento.Descricao,
                    Status = motivoCancelamento.Ativo,
                    motivoCancelamento.GerarCargaColeta,
                    motivoCancelamento.Observacao
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
                Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul repositorio = new Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul motivoCancelamento = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivoCancelamento, Auditado);

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

        private void PreencherMotivoCancelamento(Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul motivoCancelamento)
        {
            var descricao = Request.Params("Descricao");
            var observacao = Request.Params("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new ControllerException("Observação não pode passar de 2000 caracteres.");

            motivoCancelamento.Ativo = Request.GetBoolParam("Status");
            motivoCancelamento.Descricao = descricao;
            motivoCancelamento.GerarCargaColeta = Request.GetBoolParam("GerarCargaColeta");
            motivoCancelamento.Observacao = observacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Gerar Carga", "DescricaoGerarCargaColeta", 15, Models.Grid.Align.left, true);

                if (status == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul repositorio = new Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, status);
                List<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul> listaMotivoCancelamento = totalRegistros > 0 ? repositorio.Consultar(descricao, status, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul>();
                
                var listaMotivoCancelamentoRetornar = (
                    from motivo in listaMotivoCancelamento
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo,
                        motivo.DescricaoGerarCargaColeta
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoCancelamentoRetornar);
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

            if (propriedadeOrdenar == "DescricaoGerarCargaColeta")
                return "GerarCargaColeta";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
