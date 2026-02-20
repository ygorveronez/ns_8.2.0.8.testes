using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ContratoFinanciamento", "Financeiros/ContratoFinanciamentoParcelaValor")]
    public class ContratoFinanciamentoParcelaValorController : BaseController
    {
		#region Construtores

		public ContratoFinanciamentoParcelaValorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("CodigoContratoFinanciamentoParcela"), out int codigoParcela);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Movimento", "TipoMovimento", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor> contratosFinanciamentoParcelaValor = repContratoFinanciamentoParcelaValor.Consultar(codigoParcela, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContratoFinanciamentoParcelaValor.ContarConsulta(codigoParcela));

                var lista = (from p in contratosFinanciamentoParcelaValor
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 TipoMovimento = p.TipoMovimento != null ? p.TipoMovimento.Descricao : string.Empty,
                                 Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaHelper.ObterDescricao(p.Tipo),
                                 Valor = p.Valor.ToString("n2")
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

                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor contratoFinanciamentoParcelaValor = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor();

                PreencherContratoFinanciamentoParcelaValor(contratoFinanciamentoParcelaValor, unitOfWork);
                repContratoFinanciamentoParcelaValor.Inserir(contratoFinanciamentoParcelaValor);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFinanciamentoParcelaValor.ContratoFinanciamentoParcela.ContratoFinanciamento, null, "Adicionou Dados da Parcela Valor.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor contratoFinanciamentoParcelaValor = repContratoFinanciamentoParcelaValor.BuscarPorCodigo(codigo);

                PreencherContratoFinanciamentoParcelaValor(contratoFinanciamentoParcelaValor, unitOfWork);
                repContratoFinanciamentoParcelaValor.Atualizar(contratoFinanciamentoParcelaValor);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFinanciamentoParcelaValor.ContratoFinanciamentoParcela.ContratoFinanciamento, null, "Alterou Dados da Parcela Valor.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor contratoFinanciamentoParcelaValor = repContratoFinanciamentoParcelaValor.BuscarPorCodigo(codigo);

                var dynContratoFinanciamentoParcelaValor = new
                {
                    contratoFinanciamentoParcelaValor.Codigo,
                    contratoFinanciamentoParcelaValor.Descricao,
                    contratoFinanciamentoParcelaValor.Tipo,
                    TipoMovimento = contratoFinanciamentoParcelaValor.TipoMovimento != null ? new { contratoFinanciamentoParcelaValor.TipoMovimento.Codigo, contratoFinanciamentoParcelaValor.TipoMovimento.Descricao } : null,
                    Valor = contratoFinanciamentoParcelaValor.Valor.ToString("n2"),
                };

                return new JsonpResult(dynContratoFinanciamentoParcelaValor);
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
                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor contratoFinanciamentoParcelaValor = repContratoFinanciamentoParcelaValor.BuscarPorCodigo(codigo);

                if (contratoFinanciamentoParcelaValor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repContratoFinanciamentoParcelaValor.Deletar(contratoFinanciamentoParcelaValor);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFinanciamentoParcelaValor.ContratoFinanciamentoParcela.ContratoFinanciamento, null, "Excluiu Dados da Parcela Valor.", unitOfWork);

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

        private void PreencherContratoFinanciamentoParcelaValor(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor contratoFinanciamentoParcelaValor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unitOfWork);

            int.TryParse(Request.Params("CodigoContratoFinanciamentoParcela"), out int codigoParcela);
            int.TryParse(Request.Params("TipoMovimento"), out int codigoTipoMovimento);

            string descricao = Request.Params("Descricao");

            decimal.TryParse(Request.Params("Valor"), out decimal valor);

            Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipo);

            contratoFinanciamentoParcelaValor.Descricao = descricao;
            contratoFinanciamentoParcelaValor.Valor = valor;
            contratoFinanciamentoParcelaValor.Tipo = tipo;

            contratoFinanciamentoParcelaValor.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
            contratoFinanciamentoParcelaValor.ContratoFinanciamentoParcela = repContratoFinanciamentoParcela.BuscarPorCodigo(codigoParcela);
        }

        #endregion
    }
}
