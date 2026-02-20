using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.DespesaMensal
{
    [CustomAuthorize("Financeiros/DespesaMensal")]
    public class DespesaMensalController : BaseController
    {
		#region Construtores

		public DespesaMensalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Dia Provisão", "DiaProvisao", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor Provisão", "ValorProvisao", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal> despesaMensais = repDespesaMensal.Consultar(codigoEmpresa, descricao, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repDespesaMensal.ContarConsulta(codigoEmpresa, descricao, status));

                var lista = (from p in despesaMensais
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DiaProvisao,
                                 Data = p.Data.ToString("dd/MM/yyyy"),
                                 ValorProvisao = p.ValorProvisao.ToString("n2")
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
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal despesaMensal = new Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal();

                try
                {
                    PreencherDespesaMensal(despesaMensal, unitOfWork);
                }
                catch (ControllerException ex)
                {
                    return new JsonpResult(false, true, ex.Message);
                }

                repDespesaMensal.Inserir(despesaMensal, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal despesaMensal = repDespesaMensal.BuscarPorCodigo(codigo, true);

                try
                {
                    PreencherDespesaMensal(despesaMensal, unitOfWork);
                }
                catch (ControllerException ex)
                {
                    return new JsonpResult(false, true, ex.Message);
                }

                repDespesaMensal.Atualizar(despesaMensal, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal despesaMensal = repDespesaMensal.BuscarPorCodigo(codigo, false);

                var dynDespesaMensal = new
                {
                    despesaMensal.Codigo,
                    despesaMensal.Descricao,
                    despesaMensal.DiaProvisao,
                    Data = despesaMensal.Data.ToString("dd/MM/yyyy"),
                    ValorProvisao = despesaMensal.ValorProvisao.ToString("n2"),
                    despesaMensal.Situacao,
                    Pessoa = despesaMensal.Pessoa != null ? new { despesaMensal.Pessoa.Codigo, despesaMensal.Pessoa.Descricao } : null,
                    TipoDespesa = despesaMensal.TipoDespesaFinanceira != null ? new { despesaMensal.TipoDespesaFinanceira.Codigo, despesaMensal.TipoDespesaFinanceira.Descricao } : null,
                    TipoMovimento = despesaMensal.TipoMovimento != null ? new { despesaMensal.TipoMovimento.Codigo, despesaMensal.TipoMovimento.Descricao } : null,
                    TipoPagamentoRecebimento = despesaMensal.TipoPagamentoRecebimento != null ? new { despesaMensal.TipoPagamentoRecebimento.Codigo, despesaMensal.TipoPagamentoRecebimento.Descricao } : null
                };

                return new JsonpResult(dynDespesaMensal);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal despesaMensal = repDespesaMensal.BuscarPorCodigo(codigo, true);

                if (despesaMensal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repDespesaMensal.Deletar(despesaMensal, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDespesaMensal(Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal despesaMensal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("DiaProvisao"), out int diaProvisao);
            int.TryParse(Request.Params("TipoPagamentoRecebimento"), out int codigoTipoPagamentoRecebimento);
            int.TryParse(Request.Params("TipoMovimento"), out int codigoTipoMovimento);
            int.TryParse(Request.Params("TipoDespesa"), out int codigoTipoDespesaFinanceira);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            bool.TryParse(Request.Params("Situacao"), out bool situacao);
            double.TryParse(Request.Params("Pessoa"), out double pessoa);

            string descricao = Request.Params("Descricao");

            decimal.TryParse(Request.Params("ValorProvisao"), out decimal valorProvisao);

            DateTime.TryParse(Request.Params("Data"), out DateTime data);

            if (diaProvisao < 1 || diaProvisao > 31)
                throw new ControllerException("O Dia da Provisão está inválido.");

            despesaMensal.Descricao = descricao;
            despesaMensal.DiaProvisao = diaProvisao;
            despesaMensal.ValorProvisao = valorProvisao;
            despesaMensal.Data = data;
            despesaMensal.Situacao = situacao;

            despesaMensal.TipoDespesaFinanceira = repTipoDespesaFinanceira.BuscarPorCodigo(codigoTipoDespesaFinanceira);
            despesaMensal.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
            despesaMensal.TipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoPagamentoRecebimento);
            despesaMensal.Pessoa = pessoa > 0 ? repCliente.BuscarPorCPFCNPJ(pessoa) : null;
            despesaMensal.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        #endregion
    }
}
