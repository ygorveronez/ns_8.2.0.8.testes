using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/ContratoFreteValorPadrao")]
    public class ContratoFreteValorPadraoController : BaseController
    {
		#region Construtores

		public ContratoFreteValorPadraoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao repContratoFreteValorPadrao = new Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao contratoFreteValorPadrao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao();

                PreencherEntidade(contratoFreteValorPadrao, unitOfWork);

                unitOfWork.Start();

                repContratoFreteValorPadrao.Inserir(contratoFreteValorPadrao, Auditado);

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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao repContratoFreteValorPadrao = new Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao contratoFreteValorPadrao = repContratoFreteValorPadrao.BuscarPorCodigo(codigo, true);

                if (contratoFreteValorPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(contratoFreteValorPadrao, unitOfWork);

                unitOfWork.Start();

                repContratoFreteValorPadrao.Atualizar(contratoFreteValorPadrao, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao repContratoFreteValorPadrao = new Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao contratoFreteValorPadrao = repContratoFreteValorPadrao.BuscarPorCodigo(codigo, false);

                if (contratoFreteValorPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    contratoFreteValorPadrao.Codigo,
                    contratoFreteValorPadrao.Descricao,
                    Situacao = contratoFreteValorPadrao.Ativo,
                    contratoFreteValorPadrao.Observacao,
                    contratoFreteValorPadrao.ApenasQuandoEmitirCIOT,
                    contratoFreteValorPadrao.Valor,
                    Justificativa = new { Codigo = contratoFreteValorPadrao.Justificativa?.Codigo, Descricao = contratoFreteValorPadrao.Justificativa?.Descricao },
                    TransportadorTerceiro = new { Codigo = contratoFreteValorPadrao.TransportadorTerceiro?.CPF_CNPJ, Descricao = contratoFreteValorPadrao.TransportadorTerceiro?.Descricao }
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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao repContratoFreteValorPadrao = new Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao contratoFreteValorPadrao = repContratoFreteValorPadrao.BuscarPorCodigo(codigo, true);

                if (contratoFreteValorPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repContratoFreteValorPadrao.Deletar(contratoFreteValorPadrao, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao contratoFreteValorPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            bool ativo = Request.GetBoolParam("Situacao");
            bool apenasQuandoEmitirCIOT = Request.GetBoolParam("ApenasQuandoEmitirCIOT");

            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            int codigoJustificativa = Request.GetIntParam("Justificativa");

            decimal valor = Request.GetDecimalParam("Valor");

            double cpfCnpjTransportadorTerceiro = Request.GetDoubleParam("TransportadorTerceiro");

            contratoFreteValorPadrao.Ativo = ativo;
            contratoFreteValorPadrao.Descricao = descricao;
            contratoFreteValorPadrao.Observacao = observacao;
            contratoFreteValorPadrao.ApenasQuandoEmitirCIOT = apenasQuandoEmitirCIOT;
            contratoFreteValorPadrao.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
            contratoFreteValorPadrao.Valor = valor;
            contratoFreteValorPadrao.TransportadorTerceiro = cpfCnpjTransportadorTerceiro > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportadorTerceiro) : null;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Terceiro", "TransportadorTerceiro", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao repContratoFreteValorPadrao = new Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> listaContratoFreteValorPadrao = repContratoFreteValorPadrao.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repContratoFreteValorPadrao.ContarConsulta(descricao, status);

                var retorno = (from obj in listaContratoFreteValorPadrao
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.DescricaoAtivo,
                                   Justificativa = obj.Justificativa?.Descricao,
                                   TransportadorTerceiro = obj.TransportadorTerceiro?.Descricao,
                                   Valor = obj.Valor.ToString("n2")
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
