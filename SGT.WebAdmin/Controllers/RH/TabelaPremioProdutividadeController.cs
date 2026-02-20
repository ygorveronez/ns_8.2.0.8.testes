using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/TabelaPremioProdutividade")]
    public class TabelaPremioProdutividadeController : BaseController
    {
		#region Construtores

		public TabelaPremioProdutividadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.RH.TabelaPremioProdutividade repTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);

                int codigoGrupo = Request.GetIntParam("GrupoPessoas");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade> listaTabelaPremioProdutividade = repTabelaPremioProdutividade.Consultar(codigoGrupo, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaPremioProdutividade.ContarConsulta(codigoGrupo, ativo));
                var lista = from p in listaTabelaPremioProdutividade
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                p.DescricaoAtivo
                            };
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

                Repositorio.Embarcador.RH.TabelaPremioProdutividade repTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade tabelaPremioProdutividade = new Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade();
                bool.TryParse(Request.Params("Status"), out bool ativo);

                tabelaPremioProdutividade.Percentual = Request.GetDecimalParam("Percentual");
                tabelaPremioProdutividade.DataInicio = Request.GetDateTimeParam("DataInicio");
                tabelaPremioProdutividade.DataFim = Request.GetDateTimeParam("DataFim");
                tabelaPremioProdutividade.Ativo = ativo;

                SalvarTabelaGrupoPessoas(ref tabelaPremioProdutividade, unitOfWork);

                repTabelaPremioProdutividade.Inserir(tabelaPremioProdutividade, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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
                Repositorio.Embarcador.RH.TabelaPremioProdutividade repTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade tabelaPremioProdutividade = repTabelaPremioProdutividade.BuscarPorCodigo(codigo, true);

                bool ativo;
                bool.TryParse(Request.Params("Status"), out ativo);

                tabelaPremioProdutividade.Percentual = Request.GetDecimalParam("Percentual");
                tabelaPremioProdutividade.DataInicio = Request.GetDateTimeParam("DataInicio");
                tabelaPremioProdutividade.DataFim = Request.GetDateTimeParam("DataFim");
                tabelaPremioProdutividade.Ativo = ativo;

                SalvarTabelaGrupoPessoas(ref tabelaPremioProdutividade, unitOfWork);

                repTabelaPremioProdutividade.Atualizar(tabelaPremioProdutividade, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.RH.TabelaPremioProdutividade repTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade tabelaPremioProdutividade = repTabelaPremioProdutividade.BuscarPorCodigo(codigo);

                var dynProcessoMovimento = new
                {
                    tabelaPremioProdutividade.Codigo,
                    Percentual = tabelaPremioProdutividade.Percentual.ToString("n2"),
                    DataInicio = tabelaPremioProdutividade.DataInicio.ToString("dd/MM/yyyy"),
                    DataFim = tabelaPremioProdutividade.DataFim.ToString("dd/MM/yyyy"),
                    tabelaPremioProdutividade.Ativo,
                    GruposPessoas = tabelaPremioProdutividade.GrupoPessoas != null && tabelaPremioProdutividade.GrupoPessoas.Count > 0 ? (from o in tabelaPremioProdutividade.GrupoPessoas
                                                                                                                                          select new
                                                                                                                                          {
                                                                                                                                              o.Codigo,
                                                                                                                                              o.Descricao
                                                                                                                                          }).ToList() : null
                };
                return new JsonpResult(dynProcessoMovimento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.RH.TabelaPremioProdutividade reTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade tabelaPremioProdutividade = reTabelaPremioProdutividade.BuscarPorCodigo(codigo);
                tabelaPremioProdutividade.GrupoPessoas.Clear();
                reTabelaPremioProdutividade.Deletar(tabelaPremioProdutividade, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void SalvarTabelaGrupoPessoas(ref Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade tabelaPremioProdutividade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.TabelaPremioProdutividade repTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            List<dynamic> gruposPessoas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("GruposPessoas"));
            if (gruposPessoas == null) return;

            if (tabelaPremioProdutividade.GrupoPessoas == null)
                tabelaPremioProdutividade.GrupoPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            else
                tabelaPremioProdutividade.GrupoPessoas.Clear();

            foreach (dynamic dynGrupoPessoa in gruposPessoas)
            {
                int.TryParse((string)dynGrupoPessoa.Codigo, out int codigo);
                tabelaPremioProdutividade.GrupoPessoas.Add(repGrupoPessoas.BuscarPorCodigo(codigo));
            }

        }

        #endregion
    }
}
