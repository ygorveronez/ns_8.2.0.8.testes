using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/TaxaTerceiro")]
    public class TaxaTerceiroController : BaseController
    {
		#region Construtores

		public TaxaTerceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaTaxaTerceiro filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Terceiro", "Terceiro", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("CodigoJustificativa", false);
                grid.AdicionarCabecalho("Justificativa", false);

                Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro> taxaTerceiros = repTaxaTerceiro.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repTaxaTerceiro.ContarConsulta(filtrosPesquisa));

                var lista = (from p in taxaTerceiros
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Terceiro = p.Terceiro.Descricao,
                                 Valor = p.Valor.ToString("n2"),
                                 p.DescricaoAtivo,
                                 CodigoJustificativa = p.Justificativa?.Codigo ?? 0,
                                 Justificativa = p.Justificativa?.Descricao ?? string.Empty
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

                Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro taxaTerceiro = new Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro();

                PreencherTaxaTerceiro(taxaTerceiro, unitOfWork);

                repTaxaTerceiro.Inserir(taxaTerceiro, Auditado);

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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro taxaTerceiro = repTaxaTerceiro.BuscarPorCodigo(codigo, true);

                if (taxaTerceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTaxaTerceiro(taxaTerceiro, unitOfWork);

                repTaxaTerceiro.Atualizar(taxaTerceiro, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro taxaTerceiro = repTaxaTerceiro.BuscarPorCodigo(codigo, false);

                if (taxaTerceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynTaxaTerceiro = new
                {
                    taxaTerceiro.Codigo,
                    taxaTerceiro.Descricao,
                    taxaTerceiro.Ativo,
                    taxaTerceiro.TipoTaxaTerceiro,
                    Valor = taxaTerceiro.Valor.ToString("n2"),
                    VigenciaInicial = taxaTerceiro.VigenciaInicial?.ToDateString() ?? string.Empty,
                    VigenciaFinal = taxaTerceiro.VigenciaFinal?.ToDateString() ?? string.Empty,
                    Terceiro = new { taxaTerceiro.Terceiro.Codigo, taxaTerceiro.Terceiro.Descricao },
                    Veiculo = new { Codigo = taxaTerceiro.Veiculo?.Codigo ?? 0, Descricao = taxaTerceiro.Veiculo?.Descricao ?? string.Empty },
                    Justificativa = new { Codigo = taxaTerceiro.Justificativa?.Codigo ?? 0, Descricao = taxaTerceiro.Justificativa?.Descricao ?? string.Empty }
                };

                return new JsonpResult(dynTaxaTerceiro);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.TaxaTerceiro repTaxaTerceiro = new Repositorio.Embarcador.Terceiros.TaxaTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro taxaTerceiro = repTaxaTerceiro.BuscarPorCodigo(codigo, true);

                if (taxaTerceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTaxaTerceiro.Deletar(taxaTerceiro, Auditado);

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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherTaxaTerceiro(Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro taxaTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoJustificativa = Request.GetIntParam("Justificativa");
            double codigoTerceiro = Request.GetDoubleParam("Terceiro");

            taxaTerceiro.Descricao = Request.GetStringParam("Descricao");
            taxaTerceiro.Ativo = Request.GetBoolParam("Ativo");
            taxaTerceiro.Valor = Request.GetDecimalParam("Valor");
            taxaTerceiro.TipoTaxaTerceiro = Request.GetEnumParam<TipoTaxaTerceiro>("TipoTaxaTerceiro");
            taxaTerceiro.VigenciaInicial = Request.GetNullableDateTimeParam("VigenciaInicial");
            taxaTerceiro.VigenciaFinal = Request.GetNullableDateTimeParam("VigenciaFinal");

            taxaTerceiro.Terceiro = codigoTerceiro > 0 ? repCliente.BuscarPorCPFCNPJ(codigoTerceiro) : null;
            taxaTerceiro.Justificativa = codigoJustificativa > 0 ? repJustificativa.BuscarPorCodigo(codigoJustificativa) : null;
            taxaTerceiro.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaTaxaTerceiro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaTaxaTerceiro()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CnpjCpfTerceiro = Request.GetDoubleParam("Terceiro"),
                CodigoJustificativa = Request.GetIntParam("Justificativa"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoCarga = Request.GetIntParam("Carga")
            };
        }

        #endregion
    }
}
