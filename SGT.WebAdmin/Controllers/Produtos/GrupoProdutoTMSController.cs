using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/GrupoProdutoTMS")]
    public class GrupoProdutoTMSController : BaseController
    {
		#region Construtores

		public GrupoProdutoTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 80, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
                List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS> produtos = repGrupoProduto.Consultar(descricao, codigoEmpresa, 0, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repGrupoProduto.ContarConsulta(descricao, codigoEmpresa, 0, ativo));

                var lista = (from p in produtos select new { p.Codigo, p.Descricao, p.DescricaoAtivo }).ToList();

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

                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProdutoTMS = new Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS();

                int codigoGrupoPai = 0;
                int.TryParse(Request.Params("GrupoProdutoTMSPai"), out codigoGrupoPai);

                grupoProdutoTMS.Ativo = bool.Parse(Request.Params("Ativo"));
                grupoProdutoTMS.Descricao = Request.Params("Descricao");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grupoProdutoTMS.Empresa = this.Usuario.Empresa;
                if (codigoGrupoPai > 0)
                    grupoProdutoTMS.GrupoProdutoTMSPai = repGrupoProduto.BuscarPorCodigo(codigoGrupoPai);
                else
                    grupoProdutoTMS.GrupoProdutoTMSPai = null;

                SalvarDepreciacao(grupoProdutoTMS, unitOfWork);
                repGrupoProduto.Inserir(grupoProdutoTMS, Auditado);

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
                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProdutoTMS = repGrupoProduto.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int codigoGrupoPai = 0;
                int.TryParse(Request.Params("GrupoProdutoTMSPai"), out codigoGrupoPai);

                grupoProdutoTMS.Ativo = bool.Parse(Request.Params("Ativo"));
                grupoProdutoTMS.Descricao = Request.Params("Descricao");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grupoProdutoTMS.Empresa = this.Usuario.Empresa;
                if (codigoGrupoPai > 0)
                    grupoProdutoTMS.GrupoProdutoTMSPai = repGrupoProduto.BuscarPorCodigo(codigoGrupoPai);
                else
                    grupoProdutoTMS.GrupoProdutoTMSPai = null;

                SalvarDepreciacao(grupoProdutoTMS, unitOfWork);
                repGrupoProduto.Atualizar(grupoProdutoTMS, Auditado);

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
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProdutoTMS = repGrupoProduto.BuscarPorCodigo(codigo);

                var dynGrupoProduto = new
                {
                    grupoProdutoTMS.Ativo,
                    grupoProdutoTMS.Codigo,
                    grupoProdutoTMS.Descricao,
                    GrupoProdutoTMSPai = grupoProdutoTMS.GrupoProdutoTMSPai != null ? new { Codigo = grupoProdutoTMS.GrupoProdutoTMSPai.Codigo, Descricao = grupoProdutoTMS.GrupoProdutoTMSPai.Descricao } : null,
                    Depreciacao = new
                    {
                        grupoProdutoTMS.GerarDepreciacao,
                        PercentualDepreciacao = grupoProdutoTMS.PercentualDepreciacao.ToString("n2"),
                        TipoMovimentoBaixa = grupoProdutoTMS.TipoMovimentoBaixa != null ? new { grupoProdutoTMS.TipoMovimentoBaixa.Codigo, grupoProdutoTMS.TipoMovimentoBaixa.Descricao } : null,
                        TipoMovimentoDepreciacao = grupoProdutoTMS.TipoMovimentoDepreciacao != null ? new { grupoProdutoTMS.TipoMovimentoDepreciacao.Codigo, grupoProdutoTMS.TipoMovimentoDepreciacao.Descricao } : null,
                        TipoMovimentoDepreciacaoAcumulada = grupoProdutoTMS.TipoMovimentoDepreciacaoAcumulada != null ? new { grupoProdutoTMS.TipoMovimentoDepreciacaoAcumulada.Codigo, grupoProdutoTMS.TipoMovimentoDepreciacaoAcumulada.Descricao } : null
                    }
                };

                return new JsonpResult(dynGrupoProduto);
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

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProdutoTMS = repGrupoProduto.BuscarPorCodigo(codigo);
                repGrupoProduto.Deletar(grupoProdutoTMS, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarDepreciacao(Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProdutoTMS, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);

            dynamic depreciacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Depreciacao"));

            bool.TryParse((string)depreciacao.GerarDepreciacao, out bool gerarDepreciacao);

            int codigoTipoMovimentoBaixa, codigoTipoMovimentoDepreciacao, codigoTipoMovimentoDepreciacaoAcumulada;
            int.TryParse((string)depreciacao.TipoMovimentoBaixa, out codigoTipoMovimentoBaixa);
            int.TryParse((string)depreciacao.TipoMovimentoDepreciacao, out codigoTipoMovimentoDepreciacao);
            int.TryParse((string)depreciacao.TipoMovimentoDepreciacaoAcumulada, out codigoTipoMovimentoDepreciacaoAcumulada);

            grupoProdutoTMS.PercentualDepreciacao = Utilidades.Decimal.Converter((string)depreciacao.PercentualDepreciacao);
            grupoProdutoTMS.GerarDepreciacao = gerarDepreciacao;
            if (codigoTipoMovimentoBaixa > 0)
                grupoProdutoTMS.TipoMovimentoBaixa = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaixa);
            else
                grupoProdutoTMS.TipoMovimentoBaixa = null;
            if (codigoTipoMovimentoDepreciacao > 0)
                grupoProdutoTMS.TipoMovimentoDepreciacao = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoDepreciacao);
            else
                grupoProdutoTMS.TipoMovimentoDepreciacao = null;
            if (codigoTipoMovimentoDepreciacaoAcumulada > 0)
                grupoProdutoTMS.TipoMovimentoDepreciacaoAcumulada = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoDepreciacaoAcumulada);
            else
                grupoProdutoTMS.TipoMovimentoDepreciacaoAcumulada = null;
        }

        #endregion
    }
}
