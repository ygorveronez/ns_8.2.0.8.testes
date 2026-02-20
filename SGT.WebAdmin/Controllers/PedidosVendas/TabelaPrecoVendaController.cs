using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PedidosVendas
{
    [CustomAuthorize("PedidosVendas/TabelaPrecoVenda")]
    public class TabelaPrecoVendaController : BaseController
    {
		#region Construtores

		public TabelaPrecoVendaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("GrupoProduto"), out int grupoProduto);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa / Grupo Pessoa", "PessoaGrupo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Início Vigência", "DataInicioVigencia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim Vigência", "DataFimVigencia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unitOfWork);
                List<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda> tabelaPrecos = repTabelaPrecoVenda.Consultar(codigoEmpresa, descricao, grupoProduto, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaPrecoVenda.ContarConsulta(codigoEmpresa, descricao, grupoProduto, status));

                var lista = (from p in tabelaPrecos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 PessoaGrupo = p.Pessoa?.Nome ?? p.GrupoPessoas?.Descricao ?? string.Empty,
                                 GrupoProduto = p.GrupoProduto?.Descricao ?? string.Empty,
                                 DataInicioVigencia = p.DataInicioVigencia.ToString("dd/MM/yyyy"),
                                 DataFimVigencia = p.DataFimVigencia.ToString("dd/MM/yyyy"),
                                 Valor = p.Valor.ToString("n2"),
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

                Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda tabelaPrecoVenda = new Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda();

                PreencherTabelaPrecoVenda(tabelaPrecoVenda, unitOfWork);
                repTabelaPrecoVenda.Inserir(tabelaPrecoVenda, Auditado);

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

                Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda tabelaPrecoVenda = repTabelaPrecoVenda.BuscarPorCodigo(codigo, true);

                PreencherTabelaPrecoVenda(tabelaPrecoVenda, unitOfWork);
                repTabelaPrecoVenda.Atualizar(tabelaPrecoVenda, Auditado);

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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda tabelaPrecoVenda = repTabelaPrecoVenda.BuscarPorCodigo(codigo, false);

                var dynTabelaPrecoVenda = new
                {
                    tabelaPrecoVenda.Codigo,
                    tabelaPrecoVenda.Descricao,
                    tabelaPrecoVenda.Status,
                    tabelaPrecoVenda.TipoPessoa,
                    Pessoa = tabelaPrecoVenda.Pessoa != null ? new { tabelaPrecoVenda.Pessoa.Codigo, tabelaPrecoVenda.Pessoa.Descricao } : null,
                    GrupoPessoa = tabelaPrecoVenda.GrupoPessoas != null ? new { tabelaPrecoVenda.GrupoPessoas.Codigo, tabelaPrecoVenda.GrupoPessoas.Descricao } : null,
                    GrupoProduto = tabelaPrecoVenda.GrupoProduto != null ? new { tabelaPrecoVenda.GrupoProduto.Codigo, tabelaPrecoVenda.GrupoProduto.Descricao } : null,
                    DataInicioVigencia = tabelaPrecoVenda.DataInicioVigencia.ToString("dd/MM/yyyy"),
                    DataFimVigencia = tabelaPrecoVenda.DataFimVigencia.ToString("dd/MM/yyyy"),
                    Valor = tabelaPrecoVenda.Valor.ToString("n2")
                };

                return new JsonpResult(dynTabelaPrecoVenda);
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
                Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda tabelaPrecoVenda = repTabelaPrecoVenda.BuscarPorCodigo(codigo, true);

                if (tabelaPrecoVenda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTabelaPrecoVenda.Deletar(tabelaPrecoVenda, Auditado);
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

        private void PreencherTabelaPrecoVenda(Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda tabelaPrecoVenda, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
            int.TryParse(Request.Params("GrupoProduto"), out int grupoProduto);
            bool.TryParse(Request.Params("Status"), out bool status);
            double.TryParse(Request.Params("Pessoa"), out double pessoa);

            DateTime.TryParse(Request.Params("DataInicioVigencia"), out DateTime dataInicioVigencia);
            DateTime.TryParse(Request.Params("DataFimVigencia"), out DateTime dataFimVigencia);

            decimal.TryParse(Request.Params("Valor"), out decimal valor);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa;
            Enum.TryParse(Request.Params("TipoPessoa"), out tipoPessoa);

            string descricao = Request.Params("Descricao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            tabelaPrecoVenda.Descricao = descricao;
            tabelaPrecoVenda.DataInicioVigencia = dataInicioVigencia;
            tabelaPrecoVenda.DataFimVigencia = dataFimVigencia;
            tabelaPrecoVenda.Valor = valor;
            tabelaPrecoVenda.TipoPessoa = tipoPessoa;
            tabelaPrecoVenda.Status = status;

            tabelaPrecoVenda.GrupoProduto = grupoProduto > 0 ? repGrupoProduto.BuscarPorCodigo(grupoProduto) : null;
            tabelaPrecoVenda.Pessoa = pessoa > 0 ? repCliente.BuscarPorCPFCNPJ(pessoa) : null;
            tabelaPrecoVenda.GrupoPessoas = grupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(grupoPessoa) : null;
            tabelaPrecoVenda.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        #endregion
    }
}
