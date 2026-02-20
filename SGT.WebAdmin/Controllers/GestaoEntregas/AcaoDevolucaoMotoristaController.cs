using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    [CustomAuthorize("GestaoEntregas/AcaoDevolucaoMotorista")]
    public class AcaoDevolucaoMotoristaController : BaseController
    {
		#region Construtores

		public AcaoDevolucaoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista acaoDevolucaoMotorista = new Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista();

                PreencherAcaoDevolucaoMotorista(acaoDevolucaoMotorista);

                unitOfWork.Start();

                Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista repositorio = new Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista(unitOfWork);

                repositorio.Inserir(acaoDevolucaoMotorista, Auditado);

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
                Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista repositorio = new Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista acaoDevolucaoMotorista = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (acaoDevolucaoMotorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherAcaoDevolucaoMotorista(acaoDevolucaoMotorista);

                unitOfWork.Start();

                repositorio.Atualizar(acaoDevolucaoMotorista, Auditado);

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
                Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista repositorio = new Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista acaoDevolucaoMotorista = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (acaoDevolucaoMotorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    acaoDevolucaoMotorista.Codigo,
                    acaoDevolucaoMotorista.Descricao,
                    Status = acaoDevolucaoMotorista.Ativo,
                    acaoDevolucaoMotorista.Observacao
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
                Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista repositorio = new Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista acaoDevolucaoMotorista = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (acaoDevolucaoMotorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(acaoDevolucaoMotorista, Auditado);

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

        private void PreencherAcaoDevolucaoMotorista(Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista acaoDevolucaoMotorista)
        {
            string descricao = Request.GetStringParam("Descricao");
            string observacao = Request.GetStringParam("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new ControllerException("Observação não pode passar de 2000 caracteres.");

            acaoDevolucaoMotorista.Ativo = Request.GetBoolParam("Status");
            acaoDevolucaoMotorista.Descricao = descricao;
            acaoDevolucaoMotorista.Observacao = observacao;
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
                Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista repositorio = new Repositorio.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacaoAtivo);
                List<Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista> listaAcaoDevolucaoMotorista = (totalRegistros > 0) ? repositorio.Consultar(descricao, situacaoAtivo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista>();

                var listaAcaoDevolucaoMotoristaRetornar = (
                    from motivo in listaAcaoDevolucaoMotorista
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaAcaoDevolucaoMotoristaRetornar);
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
