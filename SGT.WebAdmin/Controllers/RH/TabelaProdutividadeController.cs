using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/TabelaProdutividade")]
    public class TabelaProdutividadeController : BaseController
    {
		#region Construtores

		public TabelaProdutividadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.RH.TabelaProdutividade repTabelaProdutividade = new Repositorio.Embarcador.RH.TabelaProdutividade(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.RH.TabelaProdutividade> listaTabelaProdutividade = repTabelaProdutividade.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaProdutividade.ContarConsulta(descricao, ativo));
                var lista = from p in listaTabelaProdutividade
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

                Repositorio.Embarcador.RH.TabelaProdutividade repTabelaProdutividade = new Repositorio.Embarcador.RH.TabelaProdutividade(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.RH.TabelaProdutividade tabelaProdutividade = new Dominio.Entidades.Embarcador.RH.TabelaProdutividade();
                bool.TryParse(Request.Params("Ativo"), out bool ativo);

                tabelaProdutividade.Descricao = Request.Params("Descricao");
                tabelaProdutividade.Ativo = ativo;

                repTabelaProdutividade.Inserir(tabelaProdutividade, Auditado);

                SalvarTabelaPeriodo(ref tabelaProdutividade, unitOfWork);

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
                Repositorio.Embarcador.RH.TabelaProdutividade repTabelaProdutividade = new Repositorio.Embarcador.RH.TabelaProdutividade(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.RH.TabelaProdutividade tabelaProdutividade = repTabelaProdutividade.BuscarPorCodigo(codigo, true);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                tabelaProdutividade.Descricao = Request.Params("Descricao");
                tabelaProdutividade.Ativo = ativo;

                repTabelaProdutividade.Atualizar(tabelaProdutividade, Auditado);

                SalvarTabelaPeriodo(ref tabelaProdutividade, unitOfWork);

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
                Repositorio.Embarcador.RH.TabelaProdutividade repTabelaProdutividade = new Repositorio.Embarcador.RH.TabelaProdutividade(unitOfWork);
                Repositorio.Embarcador.RH.TabelaProdutividadeValores repTabelaProdutividadeValores = new Repositorio.Embarcador.RH.TabelaProdutividadeValores(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaProdutividade tabelaProdutividade = repTabelaProdutividade.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores> valores = repTabelaProdutividadeValores.BuscarPorTabela(codigo);
                var dynProcessoMovimento = new
                {
                    tabelaProdutividade.Codigo,
                    tabelaProdutividade.Descricao,
                    tabelaProdutividade.Ativo,
                    Valores = valores != null && valores.Count > 0 ? (from o in valores
                                                                      select new
                                                                      {
                                                                          o.Codigo,
                                                                          ValorInicial = o.ValorInicial.ToString("n2"),
                                                                          ValorFinal = o.ValorFinal.ToString("n2"),
                                                                          Valor = o.Valor.ToString("n2")
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
                Repositorio.Embarcador.RH.TabelaProdutividade reTabelaProdutividade = new Repositorio.Embarcador.RH.TabelaProdutividade(unitOfWork);
                Dominio.Entidades.Embarcador.RH.TabelaProdutividade tabelaProdutividade = reTabelaProdutividade.BuscarPorCodigo(codigo);
                reTabelaProdutividade.Deletar(tabelaProdutividade, Auditado);
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

        private void SalvarTabelaPeriodo(ref Dominio.Entidades.Embarcador.RH.TabelaProdutividade tabelaProdutividade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.TabelaProdutividadeValores repTabelaProdutividadeValores = new Repositorio.Embarcador.RH.TabelaProdutividadeValores(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            List<dynamic> valores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Valores"));
            if (valores == null) return;

            List<int> codigosRegistros = new List<int>();
            foreach (dynamic codigo in valores)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosRegistros.Add(intcodigo);
            }
            codigosRegistros = codigosRegistros.Where(o => o > 0).Distinct().ToList();

            List<int> registrosParaExcluir = repTabelaProdutividadeValores.BuscarItensNaoPesentesNaLista(tabelaProdutividade.Codigo, codigosRegistros);

            foreach (dynamic dynVeiculo in valores)
            {
                int.TryParse((string)dynVeiculo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores tabelaProdutividadeValores = repTabelaProdutividadeValores.BuscarPorParametroHoraExtraECodigo(tabelaProdutividade.Codigo, codigo);

                if (tabelaProdutividadeValores == null)
                    tabelaProdutividadeValores = new Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores();
                else
                    tabelaProdutividadeValores.Initialize();

                decimal valor = Utilidades.Decimal.Converter((string)dynVeiculo.Valor);
                decimal valorInicial = Utilidades.Decimal.Converter((string)dynVeiculo.ValorInicial);
                decimal valorFinal = Utilidades.Decimal.Converter((string)dynVeiculo.ValorFinal);

                tabelaProdutividadeValores.TabelaProdutividade = tabelaProdutividade;
                tabelaProdutividadeValores.Valor = valor;
                tabelaProdutividadeValores.ValorInicial = valorInicial;
                tabelaProdutividadeValores.ValorFinal = valorFinal;

                if (tabelaProdutividadeValores.Codigo == 0)
                    repTabelaProdutividadeValores.Inserir(tabelaProdutividadeValores);
                else
                    repTabelaProdutividadeValores.Atualizar(tabelaProdutividadeValores, Auditado);
            }

            foreach (int codigoRegistro in registrosParaExcluir)
            {
                Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores registro = repTabelaProdutividadeValores.BuscarPorParametroHoraExtraECodigo(tabelaProdutividade.Codigo, codigoRegistro);
                if (registro != null) repTabelaProdutividadeValores.Deletar(registro, Auditado);
            }
        }

        #endregion
    }
}
