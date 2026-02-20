using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize("Compras/RegrasOrdemCompra")]
    public class RegrasOrdemCompraController : BaseController
    {
		#region Construtores

		public RegrasOrdemCompraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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

                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra repRegrasOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor repAlcadaFornecedor = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador repAlcadaOperador = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador repAlcadaSetorOperador = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto repAlcadaProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor repAlcadaValor = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade repAlcadaQuantidade = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto repAlcadaGrupoProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto repAlcadaPercentualDiferencaValorCustoProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra = new Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor> regraFornecedor = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador> regraOperador = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador> regraSetorOperador = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto> regraProduto = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor> regraValor = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade> regraQuantidade = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto> regraGrupoProduto = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto>();
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto> regraPercentualDiferencaValorCustoProduto = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>();

                PreencherEntidade(ref regrasOrdemCompra, unitOfWork);

                List<string> erros = new List<string>();
                if (!ValidarEntidade(regrasOrdemCompra, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, string.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasOrdemCompra, ref regraFornecedor, ref regraOperador, ref regraSetorOperador, ref regraProduto, ref regraValor, ref regraQuantidade, ref regraGrupoProduto, ref regraPercentualDiferencaValorCustoProduto, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                repRegrasOrdemCompra.Inserir(regrasOrdemCompra, Auditado);

                for (var i = 0; i < regraFornecedor.Count(); i++) repAlcadaFornecedor.Inserir(regraFornecedor[i], Auditado);
                for (var i = 0; i < regraOperador.Count(); i++) repAlcadaOperador.Inserir(regraOperador[i], Auditado);
                for (var i = 0; i < regraSetorOperador.Count(); i++) repAlcadaSetorOperador.Inserir(regraSetorOperador[i], Auditado);
                for (var i = 0; i < regraProduto.Count(); i++) repAlcadaProduto.Inserir(regraProduto[i], Auditado);
                for (var i = 0; i < regraValor.Count(); i++) repAlcadaValor.Inserir(regraValor[i], Auditado);
                for (var i = 0; i < regraQuantidade.Count(); i++) repAlcadaQuantidade.Inserir(regraQuantidade[i], Auditado);
                for (var i = 0; i < regraGrupoProduto.Count(); i++) repAlcadaGrupoProduto.Inserir(regraGrupoProduto[i], Auditado);
                for (var i = 0; i < regraPercentualDiferencaValorCustoProduto.Count(); i++) repAlcadaPercentualDiferencaValorCustoProduto.Inserir(regraPercentualDiferencaValorCustoProduto[i], Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                unitOfWork.Start();

                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra repRegrasOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor repAlcadaFornecedor = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador repAlcadaOperador = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador repAlcadaSetorOperador = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto repAlcadaProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor repAlcadaValor = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade repAlcadaQuantidade = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto repAlcadaGrupoProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto repAlcadaPercentualDiferencaValorCustoProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto(unitOfWork);

                int codigoRegra = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra = repRegrasOrdemCompra.BuscarPorCodigo(codigoRegra, true);

                if (regrasOrdemCompra == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor> regraFornecedor = repAlcadaFornecedor.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador> regraOperador = repAlcadaOperador.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador> regraSetorOperador = repAlcadaSetorOperador.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto> regraProduto = repAlcadaProduto.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor> regraValor = repAlcadaValor.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade> regraQuantidade = repAlcadaQuantidade.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto> regraGrupoProduto = repAlcadaGrupoProduto.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto> regraPercentualDiferencaValorCustoProduto = repAlcadaPercentualDiferencaValorCustoProduto.BuscarPorRegra(codigoRegra);
                #endregion

                PreencherEntidade(ref regrasOrdemCompra, unitOfWork);

                List<string> erros = new List<string>();
                if (!ValidarEntidade(regrasOrdemCompra, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, string.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasOrdemCompra, ref regraFornecedor, ref regraOperador, ref regraSetorOperador, ref regraProduto, ref regraValor, ref regraQuantidade, ref regraGrupoProduto, ref regraPercentualDiferencaValorCustoProduto, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraFornecedor, repAlcadaFornecedor, "Fornecedor", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraOperador, repAlcadaOperador, "Operador", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraSetorOperador, repAlcadaSetorOperador, "Setor", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraProduto, repAlcadaProduto, "Produto", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraValor, repAlcadaValor, "Valor", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraQuantidade, repAlcadaQuantidade, "Quantidade", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraGrupoProduto, repAlcadaGrupoProduto, "GrupoProduto", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regrasOrdemCompra, regraPercentualDiferencaValorCustoProduto, repAlcadaPercentualDiferencaValorCustoProduto, "PercentualDiferencaValorCustoProduto", ref alteracoes, unitOfWork);

                repRegrasOrdemCompra.Atualizar(regrasOrdemCompra, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasOrdemCompra, alteracoes, "Alterou os critérios da regra.", unitOfWork);
                #endregion

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra repRegrasOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor repAlcadaFornecedor = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador repAlcadaOperador = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador repAlcadaSetorOperador = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto repAlcadaProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor repAlcadaValor = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade repAlcadaQuantidade = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto repAlcadaGrupoProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto repAlcadaPercentualDiferencaValorCustoProduto = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto(unitOfWork);

                int codigoRegra = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra = repRegrasOrdemCompra.BuscarPorCodigo(codigoRegra);

                if (regrasOrdemCompra == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor> regraFornecedor = repAlcadaFornecedor.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador> regraOperador = repAlcadaOperador.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador> regraSetorOperador = repAlcadaSetorOperador.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto> regraProduto = repAlcadaProduto.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor> regraValor = repAlcadaValor.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade> regraQuantidade = repAlcadaQuantidade.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto> regraGrupoProduto = repAlcadaGrupoProduto.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto> regraPercentualDiferencaValorCustoProduto = repAlcadaPercentualDiferencaValorCustoProduto.BuscarPorRegra(codigoRegra);
                #endregion

                var dynRegra = new
                {
                    regrasOrdemCompra.Codigo,
                    regrasOrdemCompra.NumeroAprovadores,
                    Vigencia = regrasOrdemCompra.Vigencia.HasValue ? regrasOrdemCompra.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasOrdemCompra.Descricao) ? regrasOrdemCompra.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasOrdemCompra.Observacoes) ? regrasOrdemCompra.Observacoes : string.Empty,

                    Aprovadores = (from o in regrasOrdemCompra.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorFornecedor = regrasOrdemCompra.RegraPorFornecedor,
                    AlcadasFornecedor = (from obj in regraFornecedor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>(obj)).ToList(),

                    UsarRegraPorOperador = regrasOrdemCompra.RegraPorOperador,
                    AlcadasOperador = (from obj in regraOperador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador>(obj)).ToList(),

                    UsarRegraPorSetorOperador = regrasOrdemCompra.RegraPorSetorOperador,
                    AlcadasSetorOperador = (from obj in regraSetorOperador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>(obj)).ToList(),

                    UsarRegraPorProduto = regrasOrdemCompra.RegraPorProduto,
                    AlcadasProduto = (from obj in regraProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto>(obj)).ToList(),

                    UsarRegraPorValor = regrasOrdemCompra.RegraPorValor,
                    AlcadasValor = (from obj in regraValor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>(obj, true)).ToList(),

                    UsarRegraPorQuantidade = regrasOrdemCompra.RegraPorQuantidade,
                    AlcadasQuantidade = (from obj in regraQuantidade select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade>(obj, true)).ToList(),

                    UsarRegraPorGrupoProduto = regrasOrdemCompra.RegraPorGrupoProduto,
                    AlcadasGrupoProduto = (from obj in regraGrupoProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto>(obj)).ToList(),

                    UsarRegraPorPercentualDiferencaValorCustoProduto = regrasOrdemCompra.RegraPorPercentualDiferencaValorCustoProduto,
                    AlcadasPercentualDiferencaValorCustoProduto = (from obj in regraPercentualDiferencaValorCustoProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>(obj, true)).ToList(),
                };

                return new JsonpResult(dynRegra);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
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
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra repRegrasOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra = repRegrasOrdemCompra.BuscarPorCodigo(codigo);

                if (regrasOrdemCompra == null)
                    return new JsonpResult(false, true, "Não foi possível buscar a regra.");

                unitOfWork.Start();

                regrasOrdemCompra.Aprovadores.Clear();
                regrasOrdemCompra.AlcadasFornecedor.Clear();
                regrasOrdemCompra.AlcadasOperador.Clear();
                regrasOrdemCompra.AlcadasSetorOperador.Clear();
                regrasOrdemCompra.AlcadasProduto.Clear();
                regrasOrdemCompra.AlcadasValor.Clear();
                regrasOrdemCompra.AlcadasQuantidade.Clear();
                regrasOrdemCompra.AlcadasGrupoProduto.Clear();
                regrasOrdemCompra.AlcadasPercentualDiferencaValorCustoProduto.Clear();

                repRegrasOrdemCompra.Deletar(regrasOrdemCompra);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem informações vinculadas à regra.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
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
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);

            return grid;
        }

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            int.TryParse(Request.Params("NumeroAprovadores"), out int numeroAprovadores);

            bool.TryParse(Request.Params("UsarRegraPorFornecedor"), out bool usarRegraPorFornecedor);
            bool.TryParse(Request.Params("UsarRegraPorOperador"), out bool usarRegraPorOperador);
            bool.TryParse(Request.Params("UsarRegraPorSetorOperador"), out bool usarRegraPorSetorOperador);
            bool.TryParse(Request.Params("UsarRegraPorProduto"), out bool usarRegraPorProduto);
            bool.TryParse(Request.Params("UsarRegraPorValor"), out bool usarRegraPorValor);
            bool.TryParse(Request.Params("UsarRegraPorQuantidade"), out bool usarRegraPorQuantidade);
            bool.TryParse(Request.Params("UsarRegraPorGrupoProduto"), out bool usarRegraPorGrupoProduto);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            regrasOrdemCompra.Descricao = descricao;
            regrasOrdemCompra.Observacoes = observacao;
            regrasOrdemCompra.Vigencia = dataVigencia;
            regrasOrdemCompra.NumeroAprovadores = numeroAprovadores;
            regrasOrdemCompra.Aprovadores = listaAprovadores;

            regrasOrdemCompra.RegraPorFornecedor = usarRegraPorFornecedor;
            regrasOrdemCompra.RegraPorOperador = usarRegraPorOperador;
            regrasOrdemCompra.RegraPorSetorOperador = usarRegraPorSetorOperador;
            regrasOrdemCompra.RegraPorProduto = usarRegraPorProduto;
            regrasOrdemCompra.RegraPorValor = usarRegraPorValor;
            regrasOrdemCompra.RegraPorQuantidade = usarRegraPorQuantidade;
            regrasOrdemCompra.RegraPorGrupoProduto = usarRegraPorGrupoProduto;
            regrasOrdemCompra.RegraPorPercentualDiferencaValorCustoProduto = Request.GetBoolParam("UsarRegraPorPercentualDiferencaValorCustoProduto");
            regrasOrdemCompra.Empresa = this.Usuario.Empresa;
        }

        private void PreencherEntidadeRegra<T>(string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
        {
            /* Descricao
             * RegrasAutorizacao é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - Regra (Entidade Pai)
             * - Ordem
             * - Codicao
             * - Juncao
             * - TIPO
             * 
             * Esse último, é instanciado com o retorno do callback, já que é o único parametro que é modificado
             * Mas quando não for uma enteidade, mas um valor simples, basta usar a flag usarDynamic = true,
             * Fazendo isso é setado o valor que vem no RegrasPorTipo.Valor
             */

            // Converte json (com o parametro get)
            List<Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo>>(Request.Params(parametroJson));

            if (dynRegras == null)
                throw new Exception("Erro ao converter os dados recebidos.");

            // Variavel auxiliar
            PropertyInfo prop;

            // Itera retornos
            for (var i = 0; i < dynRegras.Count(); i++)
            {
                int.TryParse(dynRegras[i].Codigo.ToString(), out int codigoRegra);
                int indexRegraNaLista = -1;

                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                if (codigoRegra > 0)
                {
                    for (int j = 0; j < regrasPorTipo.Count; j++)
                        if ((int)((dynamic)regrasPorTipo[j]).Codigo == codigoRegra)
                        {
                            indexRegraNaLista = j;
                            break;
                        }
                }

                if (indexRegraNaLista >= 0)
                {
                    regra = regrasPorTipo[indexRegraNaLista];
                    regra.Initialize();
                }
                else
                    regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                prop = regra.GetType().GetProperty("RegrasOrdemCompra", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasOrdemCompra, null);

                regra.Ordem = dynRegras[i].Ordem;
                regra.Condicao = dynRegras[i].Condicao;
                regra.Juncao = dynRegras[i].Juncao;


                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegras[i].Entidade != null ? lambda(dynRegras[i].Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                    if (prop.PropertyType.Name.Equals("Decimal"))
                    {
                        decimal.TryParse(dynRegras[i].Valor.ToString(), out decimal valorDecimal);

                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                    }
                }

                // Adiciona lista de retorno
                if (indexRegraNaLista >= 0)
                    regrasPorTipo[indexRegraNaLista] = regra;
                else
                    regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasOrdemCompra.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasOrdemCompra.Aprovadores.Count() < regrasOrdemCompra.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regrasOrdemCompra.NumeroAprovadores.ToString());

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, List<T> regrasPorTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasPorTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");
            else
            {
                // Variavel auxiliar
                PropertyInfo prop;

                // Itera validacao
                for (var i = 0; i < regrasPorTipo.Count(); i++)
                {
                    var regra = regrasPorTipo[i];
                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);

                    if (prop.GetValue(regra) == null)
                        erros.Add(nomeRegra + " da regra é obrigatório.");
                }
            }

            return erros.Count() == 0;
        }

        private Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo RetornaRegraPorTipoDyn<T>(T obj, bool usarValor = false) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
        {
            // Variavel auxiliar
            PropertyInfo prop;

            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int.TryParse(prop.GetValue(obj).ToString(), out int codigo);

            Dominio.ObjetosDeValor.Embarcador.Alcada.Entidade objetoEntidade = null;
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty("Descricao", BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

                objetoEntidade = new Dominio.ObjetosDeValor.Embarcador.Alcada.Entidade
                {
                    Codigo = codigoEntidade,
                    Descricao = descricaoEntidade
                };
            }
            else
            {
                prop = obj.GetType().GetProperty("Descricao", BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo retorno = new Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = obj.Ordem,
                Juncao = obj.Juncao,
                Condicao = obj.Condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return retorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor> regraFornecedor, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador> regraOperador, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador> regraSetorOperador, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto> regraProduto, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor> regraValor, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade> regraQuantidade, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto> regraGrupoProduto, ref List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto> regraPercentualDiferencaValorCustoProduto, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region Fornecedor
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorFornecedor)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasFornecedor", false, ref regraFornecedor, ref regrasOrdemCompra, ((codigo) =>
                    {
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        double.TryParse(codigo.ToString(), out double cnpj);

                        return repCliente.BuscarPorCPFCNPJ(cnpj);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Fornecedor");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Fornecedor", regraFornecedor, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraFornecedor = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>();
            }
            #endregion

            #region Operador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorOperador)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasOperador", false, ref regraOperador, ref regrasOrdemCompra, ((codigo) =>
                    {
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoInt);

                        return repUsuario.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Operador");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Operador", regraOperador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraOperador = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador>();
            }
            #endregion

            #region SetorOperador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorSetorOperador)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasSetorOperador", false, ref regraSetorOperador, ref regrasOrdemCompra, ((codigo) =>
                    {
                        Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoInt);

                        return repSetor.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("SetorOperador");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("SetorOperador", regraSetorOperador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraSetorOperador = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>();
            }
            #endregion

            #region Produto
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorProduto)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasProduto", false, ref regraProduto, ref regrasOrdemCompra, ((codigo) =>
                    {
                        Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoInt);

                        return repProduto.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Produto");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Produto", regraProduto, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraProduto = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto>();
            }
            #endregion

            #region Valor
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorValor)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasValor", true, ref regraValor, ref regrasOrdemCompra);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor", regraValor, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraValor = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>();
            }
            #endregion

            #region Quantidade
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorQuantidade)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasQuantidade", true, ref regraQuantidade, ref regrasOrdemCompra);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Quantidade");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Quantidade", regraQuantidade, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraQuantidade = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade>();
            }
            #endregion

            #region GrupoProduto
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorGrupoProduto)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasGrupoProduto", false, ref regraGrupoProduto, ref regrasOrdemCompra, ((codigo) =>
                    {
                        Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProdutoTMS = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoInt);

                        return repGrupoProdutoTMS.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("GrupoProduto");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("GrupoProduto", regraGrupoProduto, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraGrupoProduto = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto>();
            }
            #endregion

            #region PercentualDiferencaValorCustoProduto
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasOrdemCompra.RegraPorPercentualDiferencaValorCustoProduto)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasPercentualDiferencaValorCustoProduto", true, ref regraPercentualDiferencaValorCustoProduto, ref regrasOrdemCompra);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("PercentualDiferencaValorCustoProduto");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("PercentualDiferencaValorCustoProduto", regraPercentualDiferencaValorCustoProduto, out erros))
                    throw new Exception(string.Join("<br>", erros));
            }
            else
            {
                regraPercentualDiferencaValorCustoProduto = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>();
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra repRegrasOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Aprovador"), out int codigoAprovador);

            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> listaGrid = repRegrasOrdemCompra.ConsultarRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegrasOrdemCompra.ContarConsultaRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void SalvarAlteracaoCriterioDaRegra<T, R>(Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regrasOrdemCompra, List<T> criterios, R repositorio, string descricao, ref List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase where R : Repositorio.RepositorioBase<T>
        {
            bool inseriuCriterio = false;
            bool removeuCriterio = false;
            for (var i = 0; i < criterios.Count(); i++)
            {
                PropertyInfo prop;

                prop = criterios[i].GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                int.TryParse(prop.GetValue(criterios[i]).ToString(), out int codigo);

                if (codigo == 0)
                {
                    repositorio.Inserir(criterios[i]);
                    inseriuCriterio = true;
                }
                else
                {
                    // Criterio ja existente
                    var changes = criterios[i].GetChanges();
                    if (changes == null)
                    {
                        // Caso GetChanges seja null, quer dizer que o criterio existia mas foi removido
                        repositorio.Deletar(criterios[i]);
                        removeuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(changes);
                        repositorio.Atualizar(criterios[i]);
                    }
                }
            }
            if (inseriuCriterio)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasOrdemCompra, null, "Adicionou um critério de " + descricao + ".", unitOfWork);
            if (removeuCriterio)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasOrdemCompra, null, "Removeu um critério de " + descricao + ".", unitOfWork);
        }

        #endregion
    }
}

