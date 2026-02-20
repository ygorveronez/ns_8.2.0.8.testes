using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/TipoInfracao")]
    public class TipoInfracaoController : BaseController
    {
		#region Construtores

		public TipoInfracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frota.TipoInfracao repositorio = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = new Dominio.Entidades.Embarcador.Frota.TipoInfracao();

                PreencherTipoInfracao(tipoInfracao, unitOfWork);

                repositorio.Inserir(tipoInfracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.Frota.TipoInfracao repositorio = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (tipoInfracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                PreencherTipoInfracao(tipoInfracao, unitOfWork);

                repositorio.Atualizar(tipoInfracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.Frota.TipoInfracao repositorio = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorio.BuscarPorCodigo(codigo);

                if (tipoInfracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoInfracao.Codigo,
                    tipoInfracao.CodigoCTB,
                    tipoInfracao.Descricao,
                    tipoInfracao.Nivel,
                    tipoInfracao.Pontos,
                    tipoInfracao.Tipo,
                    tipoInfracao.Valor,
                    tipoInfracao.Ativo,
                    tipoInfracao.GerarMovimentoFichaMotorista,
                    tipoInfracao.LancarDescontoMotorista,
                    tipoInfracao.DescontoComissaoMotorista,
                    tipoInfracao.ReduzirPercentualComissaoMotorista,
                    tipoInfracao.PercentualReducaoComissaoMotorista,
                    tipoInfracao.PermitirReplicarInformacao,
                    JustificativaDesconto = tipoInfracao.JustificativaDesconto != null ? new { tipoInfracao.JustificativaDesconto.Codigo, tipoInfracao.JustificativaDesconto.Descricao } : null,
                    TipoMovimentoFichaMotorista = tipoInfracao.TipoMovimentoFichaMotorista != null ? new { tipoInfracao.TipoMovimentoFichaMotorista.Codigo, tipoInfracao.TipoMovimentoFichaMotorista.Descricao } : null,
                    TipoMovimentoTituloEmpresa = tipoInfracao.TipoMovimentoTituloEmpresa != null ? new { tipoInfracao.TipoMovimentoTituloEmpresa.Codigo, tipoInfracao.TipoMovimentoTituloEmpresa.Descricao } : null,
                    tipoInfracao.NaoGerarTituloFinanceiro,
                    tipoInfracao.AdicionarRenavamVeiculoObservacao,
                    tipoInfracao.NaoObrigarInformarCidade,
                    tipoInfracao.NaoObrigarInformarLocal
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

                Repositorio.Embarcador.Frota.TipoInfracao repositorio = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (tipoInfracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(tipoInfracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(excecao);
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }
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

        private void PreencherTipoInfracao(Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            tipoInfracao.Ativo = Request.GetBoolParam("Ativo");
            tipoInfracao.CodigoCTB = Request.GetNullableStringParam("CodigoCTB");
            tipoInfracao.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new Exception("Descrição é obrigatória.");
            tipoInfracao.Nivel = Request.GetEnumParam<NivelInfracaoTransito>("Nivel");
            tipoInfracao.Pontos = Request.GetIntParam("Pontos");
            tipoInfracao.Tipo = Request.GetEnumParam<TipoInfracaoTransito>("Tipo");
            tipoInfracao.Valor = Request.GetDecimalParam("Valor");

            tipoInfracao.GerarMovimentoFichaMotorista = Request.GetBoolParam("GerarMovimentoFichaMotorista");
            tipoInfracao.PermitirReplicarInformacao = Request.GetBoolParam("PermitirReplicarInformacao");
            tipoInfracao.LancarDescontoMotorista = Request.GetBoolParam("LancarDescontoMotorista");
            tipoInfracao.DescontoComissaoMotorista = Request.GetDecimalParam("DescontoComissaoMotorista");

            int codigoJustificativa = Request.GetIntParam("JustificativaDesconto");
            tipoInfracao.JustificativaDesconto = codigoJustificativa > 0 ? repJustificativa.BuscarPorCodigo(codigoJustificativa) : null;

            int codigoTipoMovimentoFichaMotorista = Request.GetIntParam("TipoMovimentoFichaMotorista");
            tipoInfracao.TipoMovimentoFichaMotorista = codigoTipoMovimentoFichaMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoFichaMotorista) : null;

            int codigoTipoMovimentoTituloEmpresa = Request.GetIntParam("TipoMovimentoTituloEmpresa");
            tipoInfracao.TipoMovimentoTituloEmpresa = codigoTipoMovimentoTituloEmpresa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTituloEmpresa) : null;

            tipoInfracao.ReduzirPercentualComissaoMotorista = Request.GetBoolParam("ReduzirPercentualComissaoMotorista");
            tipoInfracao.PercentualReducaoComissaoMotorista = Request.GetDecimalParam("PercentualReducaoComissaoMotorista");
            tipoInfracao.NaoGerarTituloFinanceiro = Request.GetBoolParam("NaoGerarTituloFinanceiro");
            tipoInfracao.AdicionarRenavamVeiculoObservacao = Request.GetBoolParam("AdicionarRenavamVeiculoObservacao");
            tipoInfracao.NaoObrigarInformarLocal = Request.GetBoolParam("NaoObrigarInformarLocal");
            tipoInfracao.NaoObrigarInformarCidade = Request.GetBoolParam("NaoObrigarInformarCidade");

            if (tipoInfracao.GerarMovimentoFichaMotorista && tipoInfracao.TipoMovimentoFichaMotorista == null)
                throw new ControllerException("É obrigatório informar o tipo de movimento para a ficha do motorista.");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                string descricao = Request.GetStringParam("Descricao");
                string codigoCTB = Request.GetStringParam("CodigoCTB");
                SituacaoAtivoPesquisa status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código C.T.B", "CodigoCTB", 25, Models.Grid.Align.left, true);

                if (status == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("PermitirReplicarInformacao", false);
                grid.AdicionarCabecalho("GerarMovimentoFichaMotorista", false);
                grid.AdicionarCabecalho("CodigoTipoMovimentoTituloEmpresa", false);
                grid.AdicionarCabecalho("TipoMovimentoTituloEmpresa", false);
                grid.AdicionarCabecalho("CodigoTipoMovimentoFichaMotorista", false);
                grid.AdicionarCabecalho("TipoMovimentoFichaMotorista", false);
                grid.AdicionarCabecalho("AdicionarRenavamVeiculoObservacao", false);
                grid.AdicionarCabecalho("NaoObrigarInformarCidade", false);
                grid.AdicionarCabecalho("NaoObrigarInformarLocal", false);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                Repositorio.Embarcador.Frota.TipoInfracao repositorio = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.TipoInfracao> listaTipoInfracao = repositorio.Consultar(descricao, codigoCTB, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repositorio.ContarConsulta(descricao, codigoCTB, status);

                var listaTipoInfracaoRetornar = (
                    from tipoInfracao in listaTipoInfracao
                    select new
                    {
                        tipoInfracao.Codigo,
                        tipoInfracao.Descricao,
                        tipoInfracao.CodigoCTB,
                        tipoInfracao.DescricaoAtivo,
                        tipoInfracao.GerarMovimentoFichaMotorista,
                        tipoInfracao.Tipo,
                        tipoInfracao.PermitirReplicarInformacao,
                        CodigoTipoMovimentoTituloEmpresa = tipoInfracao.TipoMovimentoTituloEmpresa?.Codigo ?? 0,
                        TipoMovimentoTituloEmpresa = tipoInfracao.TipoMovimentoTituloEmpresa?.Descricao ?? "",
                        CodigoTipoMovimentoFichaMotorista = tipoInfracao.TipoMovimentoFichaMotorista?.Codigo ?? 0,
                        TipoMovimentoFichaMotorista = tipoInfracao.TipoMovimentoFichaMotorista?.Descricao ?? "",
                        tipoInfracao.AdicionarRenavamVeiculoObservacao,
                        tipoInfracao.NaoObrigarInformarCidade,
                        tipoInfracao.NaoObrigarInformarLocal
                    }
                ).ToList();

                grid.AdicionaRows(listaTipoInfracaoRetornar);
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
