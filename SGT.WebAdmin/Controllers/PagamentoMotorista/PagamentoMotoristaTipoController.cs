using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTipo")]
    public class PagamentoMotoristaTipoController : BaseController
    {
		#region Construtores

		public PagamentoMotoristaTipoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                string descricao = Request.Params("Descricao");
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 80, Models.Grid.Align.left, true);
                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Ativo, "Ativo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CodigoPlanoDeContaDebito", false);
                grid.AdicionarCabecalho("DescricaoPlanoDeContaDebito", false);
                grid.AdicionarCabecalho("CodigoPlanoDeContaCredito", false);
                grid.AdicionarCabecalho("DescricaoPlanoDeContaCredito", false);
                grid.AdicionarCabecalho("GerarTituloPagar", false);
                grid.AdicionarCabecalho("NaoAssociarTipoPagamentoNoAcertoDeViagem", false);
                grid.AdicionarCabecalho("GerarMovimentoEntradaFixaMotorista", false);
                grid.AdicionarCabecalho("GerarTituloAPagarAoMotorista", false);
                grid.AdicionarCabecalho("PlanoContaCredito", false);
                grid.AdicionarCabecalho("PlanoContaDebito", false);
                grid.AdicionarCabecalho("DesabilitarAlteracaoDosPlanosDeContas", false);
                grid.AdicionarCabecalho("HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada", false);
                grid.AdicionarCabecalho("PessoaSeraInformadaGeracaoPagamento", false);
                grid.AdicionarCabecalho("CodigoFornecedor", false);
                grid.AdicionarCabecalho("NomeFornecedor", false);
                grid.AdicionarCabecalho("TipoPagamentoMotorista", false);
                grid.AdicionarCabecalho("GerarMovimentoAutomatico", false);
                grid.AdicionarCabecalho("PermitirMultiplosPagamentosAbertosParaMesmoMotorista", false);

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo> listaTipo = repPagamentoMotoristaTipo.Consultar(ativo, descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPagamentoMotoristaTipo.ContarConsulta(ativo, descricao));

                var lista = (from p in listaTipo
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Ativo = p.DescricaoAtivo,
                                 CodigoPlanoDeContaDebito = p.GerarMovimentoAutomatico && p.TipoMovimentoLancamento != null ? p.TipoMovimentoLancamento.PlanoDeContaDebito.Codigo : p.TipoMovimentoTituloMotorista?.PlanoDeContaDebito?.Codigo ?? 0,
                                 DescricaoPlanoDeContaDebito = p.GerarMovimentoAutomatico && p.TipoMovimentoLancamento != null ? p.TipoMovimentoLancamento.PlanoDeContaDebito.BuscarDescricao : p.TipoMovimentoTituloMotorista?.PlanoDeContaDebito?.BuscarDescricao ?? string.Empty,
                                 CodigoPlanoDeContaCredito = p.GerarMovimentoAutomatico && p.TipoMovimentoLancamento != null ? p.TipoMovimentoLancamento.PlanoDeContaCredito.Codigo : p.TipoMovimentoTituloMotorista?.PlanoDeContaCredito?.Codigo ?? 0,
                                 DescricaoPlanoDeContaCredito = p.GerarMovimentoAutomatico && p.TipoMovimentoLancamento != null ? p.TipoMovimentoLancamento.PlanoDeContaCredito.BuscarDescricao : p.TipoMovimentoTituloMotorista?.PlanoDeContaCredito?.BuscarDescricao ?? string.Empty,
                                 p.GerarTituloPagar,
                                 p.NaoAssociarTipoPagamentoNoAcertoDeViagem,
                                 p.GerarMovimentoEntradaFixaMotorista,
                                 p.GerarTituloAPagarAoMotorista,
                                 PlanoContaCredito = p?.TipoMovimentoTituloMotorista?.PlanoDeContaCredito.Codigo ?? 0,
                                 PlanoContaDebito = p?.TipoMovimentoTituloMotorista?.PlanoDeContaDebito.Codigo ?? 0,
                                 p.DesabilitarAlteracaoDosPlanosDeContas,
                                 p.HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada,
                                 p.PessoaSeraInformadaGeracaoPagamento,
                                 CodigoFornecedor = p.Pessoa?.CPF_CNPJ ?? 0d,
                                 NomeFornecedor = p.Pessoa?.Descricao ?? "",
                                 p.TipoPagamentoMotorista,
                                 p.GerarMovimentoAutomatico,
                                 p.PermitirMultiplosPagamentosAbertosParaMesmoMotorista,
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
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
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo();

                try
                {
                    PreencherPagamentoMotoristaTipo(pagamentoMotoristaTipo, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repPagamentoMotoristaTipo.Inserir(pagamentoMotoristaTipo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionar);
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

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo(codigo, true);

                try
                {
                    PreencherPagamentoMotoristaTipo(pagamentoMotoristaTipo, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repPagamentoMotoristaTipo.Atualizar(pagamentoMotoristaTipo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo(codigo);

                if (pagamentoMotoristaTipo == null)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelEncontrarRegistro);

                var dynPagamentoMotoristaTipo = new
                {
                    pagamentoMotoristaTipo.Codigo,
                    pagamentoMotoristaTipo.Descricao,
                    pagamentoMotoristaTipo.Ativo,
                    pagamentoMotoristaTipo.CodigoIntegracao,
                    pagamentoMotoristaTipo.CodigoIntegracaoTipo,
                    pagamentoMotoristaTipo.CodigoIntegracaoEfetivacaoAdiantamento,
                    pagamentoMotoristaTipo.CodigoIntegracaoTipoParcelaAdiantamento,
                    pagamentoMotoristaTipo.CodigoIntegracaoImportacao,
                    pagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista,
                    pagamentoMotoristaTipo.TipoPagamentoMotorista,
                    pagamentoMotoristaTipo.Observacao,
                    pagamentoMotoristaTipo.GerarMovimentoAutomatico,
                    pagamentoMotoristaTipo.NaoPermitirCancelamento,
                    pagamentoMotoristaTipo.PermitirMultiplosPagamentosAbertosParaMesmoMotorista,
                    TipoMovimentoReversaoLancamento = new { Descricao = pagamentoMotoristaTipo.TipoMovimentoReversaoLancamento?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoMovimentoReversaoLancamento?.Codigo ?? 0 },
                    TipoMovimentoLancamento = new { Descricao = pagamentoMotoristaTipo.TipoMovimentoLancamento?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoMovimentoLancamento?.Codigo ?? 0 },
                    pagamentoMotoristaTipo.GerarTituloPagar,
                    TipoMovimentoTituloPagar = new { Descricao = pagamentoMotoristaTipo.TipoMovimentoTituloPagar?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoMovimentoTituloPagar?.Codigo ?? 0 },
                    Pessoa = new { Descricao = pagamentoMotoristaTipo.Pessoa?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.Pessoa?.Codigo ?? 0 },
                    pagamentoMotoristaTipo.PessoaSeraInformadaGeracaoPagamento,
                    pagamentoMotoristaTipo.GerarTarifaAutomatica,
                    PercentualTarifa = pagamentoMotoristaTipo.PercentualTarifa > 0 ? pagamentoMotoristaTipo.PercentualTarifa.ToString("n2") : string.Empty,
                    TipoMovimentoTarifa = new { Descricao = pagamentoMotoristaTipo.TipoMovimentoTarifa?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoMovimentoTarifa?.Codigo ?? 0 },
                    TipoMovimentoReversaoTarifa = new { Descricao = pagamentoMotoristaTipo.TipoMovimentoReversaoTarifa?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoMovimentoReversaoTarifa?.Codigo ?? 0 },
                    pagamentoMotoristaTipo.NaoAssociarTipoPagamentoNoAcertoDeViagem,
                    pagamentoMotoristaTipo.GerarMovimentoEntradaFixaMotorista,
                    pagamentoMotoristaTipo.FormaPagamentoMotorista,
                    pagamentoMotoristaTipo.AssuntoEmail,
                    pagamentoMotoristaTipo.CorpoEmail,
                    pagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista,
                    pagamentoMotoristaTipo.GerarTituloAPagarAoMotorista,
                    TipoMovimentoTituloMotorista = new { Descricao = pagamentoMotoristaTipo.TipoMovimentoTituloMotorista?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoMovimentoTituloMotorista?.Codigo ?? 0 },
                    pagamentoMotoristaTipo.DesabilitarAlteracaoDosPlanosDeContas,
                    pagamentoMotoristaTipo.HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada,
                    pagamentoMotoristaTipo.PermitirLancarPagamentoContendoAcertoEmAndamento,
                    pagamentoMotoristaTipo.UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo,
                    pagamentoMotoristaTipo.PermitirLancarComDataRetroativa,
                    TipoDeDespesa = new { Descricao = pagamentoMotoristaTipo.TipoDespesa?.Descricao ?? string.Empty, Codigo = pagamentoMotoristaTipo.TipoDespesa?.Codigo ?? 0 },
                    pagamentoMotoristaTipo.NaoPermitirGerarPagamentoMotoristaTerceiro,
                    pagamentoMotoristaTipo.RealizarMovimentoFinanceiroPelaDataPagamento,
                    pagamentoMotoristaTipo.RealizarRateio,
                    pagamentoMotoristaTipo.GerarPendenciaAoMotorista,
                    pagamentoMotoristaTipo.ReterImpostoPagamentoMotorista
                };

                return new JsonpResult(dynPagamentoMotoristaTipo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
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
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo(codigo, true);
                repPagamentoMotoristaTipo.Deletar(pagamentoMotoristaTipo, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoEmOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherPagamentoMotoristaTipo(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repositorioTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double cnpjPessoa = Request.GetDoubleParam("Pessoa");

            int codigoTipoMovimentoUso = Request.GetIntParam("TipoMovimentoLancamento");
            int codigoTipoMovimentoReversao = Request.GetIntParam("TipoMovimentoReversaoLancamento");
            int codigoTipoMovimentoTituloPagar = Request.GetIntParam("TipoMovimentoTituloPagar");
            int codigoTipoMovimentoTarifa = Request.GetIntParam("TipoMovimentoTarifa");
            int codigoTipoMovimentoReversaoTarifa = Request.GetIntParam("TipoMovimentoReversaoTarifa");
            int codigoTipoMovimentoTituloMotorista = Request.GetIntParam("TipoMovimentoTituloMotorista");
            int codigoTipoDespesa = Request.GetIntParam("TipoDeDespesa");

            pagamentoMotoristaTipo.Descricao = Request.GetStringParam("Descricao");
            pagamentoMotoristaTipo.Observacao = Request.GetStringParam("Observacao");
            pagamentoMotoristaTipo.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            pagamentoMotoristaTipo.CodigoIntegracaoTipo = Request.GetStringParam("CodigoIntegracaoTipo");
            pagamentoMotoristaTipo.CodigoIntegracaoEfetivacaoAdiantamento = Request.GetStringParam("CodigoIntegracaoEfetivacaoAdiantamento");
            pagamentoMotoristaTipo.CodigoIntegracaoTipoParcelaAdiantamento = Request.GetStringParam("CodigoIntegracaoTipoParcelaAdiantamento");
            pagamentoMotoristaTipo.CodigoIntegracaoImportacao = Request.GetStringParam("CodigoIntegracaoImportacao");
            pagamentoMotoristaTipo.RealizarRateio = Request.GetBoolParam("RealizarRateio");
            pagamentoMotoristaTipo.TipoDespesa = codigoTipoDespesa > 0 ? repositorioTipoDespesa.BuscarPorCodigo(codigoTipoDespesa) : null;

            pagamentoMotoristaTipo.Ativo = Request.GetBoolParam("Ativo");
            pagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista = Request.GetEnumParam<TipoIntegracaoPagamentoMotorista>("TipoIntegracaoPagamentoMotorista");
            pagamentoMotoristaTipo.TipoPagamentoMotorista = Request.GetEnumParam<TipoPagamentoMotorista>("TipoPagamentoMotorista");
            pagamentoMotoristaTipo.CorpoEmail = Request.GetStringParam("CorpoEmail");
            pagamentoMotoristaTipo.AssuntoEmail = Request.GetStringParam("AssuntoEmail");
            pagamentoMotoristaTipo.FormaPagamentoMotorista = Request.GetEnumParam<FormaPagamentoMotorista>("FormaPagamentoMotorista");
            pagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista = Request.GetEnumParam<TipoMovimentoEntidade>("TipoMovimentoPagamentoMotorista");

            pagamentoMotoristaTipo.NaoPermitirCancelamento = Request.GetBoolParam("NaoPermitirCancelamento");
            pagamentoMotoristaTipo.PermitirMultiplosPagamentosAbertosParaMesmoMotorista = Request.GetBoolParam("PermitirMultiplosPagamentosAbertosParaMesmoMotorista");
            pagamentoMotoristaTipo.GerarMovimentoAutomatico = Request.GetBoolParam("GerarMovimentoAutomatico");
            pagamentoMotoristaTipo.TipoMovimentoReversaoLancamento = codigoTipoMovimentoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversao) : null;
            pagamentoMotoristaTipo.TipoMovimentoLancamento = codigoTipoMovimentoUso > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUso) : null;

            pagamentoMotoristaTipo.GerarTituloPagar = Request.GetBoolParam("GerarTituloPagar");
            pagamentoMotoristaTipo.TipoMovimentoTituloPagar = codigoTipoMovimentoTituloPagar > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTituloPagar) : null;
            pagamentoMotoristaTipo.Pessoa = cnpjPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjPessoa) : null;
            pagamentoMotoristaTipo.PessoaSeraInformadaGeracaoPagamento = Request.GetBoolParam("PessoaSeraInformadaGeracaoPagamento");

            pagamentoMotoristaTipo.GerarTarifaAutomatica = Request.GetBoolParam("GerarTarifaAutomatica");
            pagamentoMotoristaTipo.PercentualTarifa = Request.GetDecimalParam("PercentualTarifa");
            pagamentoMotoristaTipo.TipoMovimentoTarifa = codigoTipoMovimentoTarifa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTarifa) : null;
            pagamentoMotoristaTipo.TipoMovimentoReversaoTarifa = codigoTipoMovimentoReversaoTarifa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoTarifa) : null;

            pagamentoMotoristaTipo.UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo = Request.GetBoolParam("UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo");
            pagamentoMotoristaTipo.PermitirLancarComDataRetroativa = Request.GetBoolParam("PermitirLancarComDataRetroativa");
            pagamentoMotoristaTipo.PermitirLancarPagamentoContendoAcertoEmAndamento = Request.GetBoolParam("PermitirLancarPagamentoContendoAcertoEmAndamento");
            pagamentoMotoristaTipo.HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada = Request.GetBoolParam("HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada");
            pagamentoMotoristaTipo.DesabilitarAlteracaoDosPlanosDeContas = Request.GetBoolParam("DesabilitarAlteracaoDosPlanosDeContas");
            pagamentoMotoristaTipo.NaoAssociarTipoPagamentoNoAcertoDeViagem = Request.GetBoolParam("NaoAssociarTipoPagamentoNoAcertoDeViagem");
            pagamentoMotoristaTipo.GerarMovimentoEntradaFixaMotorista = Request.GetBoolParam("GerarMovimentoEntradaFixaMotorista");

            pagamentoMotoristaTipo.GerarTituloAPagarAoMotorista = Request.GetBoolParam("GerarTituloAPagarAoMotorista");
            pagamentoMotoristaTipo.TipoMovimentoTituloMotorista = codigoTipoMovimentoTituloMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTituloMotorista) : null;

            pagamentoMotoristaTipo.NaoPermitirGerarPagamentoMotoristaTerceiro = Request.GetBoolParam("NaoPermitirGerarPagamentoMotoristaTerceiro");
            pagamentoMotoristaTipo.RealizarMovimentoFinanceiroPelaDataPagamento = Request.GetBoolParam("RealizarMovimentoFinanceiroPelaDataPagamento");
            pagamentoMotoristaTipo.GerarPendenciaAoMotorista = Request.GetBoolParam("GerarPendenciaAoMotorista");
            pagamentoMotoristaTipo.ReterImpostoPagamentoMotorista = Request.GetBoolParam("ReterImpostoPagamentoMotorista");

            if (pagamentoMotoristaTipo.GerarMovimentoAutomatico)
            {
                if (pagamentoMotoristaTipo.TipoMovimentoLancamento == null || pagamentoMotoristaTipo.TipoMovimentoReversaoLancamento == null)
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão para este tipo de pagamento.");
            }

            if (pagamentoMotoristaTipo.GerarTituloPagar)
            {
                if (pagamentoMotoristaTipo.TipoMovimentoTituloPagar == null || (pagamentoMotoristaTipo.Pessoa == null && !pagamentoMotoristaTipo.PessoaSeraInformadaGeracaoPagamento))
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e o fornecedor para a geração do título a pagar.");
            }

            if (pagamentoMotoristaTipo.GerarTarifaAutomatica)
            {
                if (pagamentoMotoristaTipo.PercentualTarifa == 0 || pagamentoMotoristaTipo.TipoMovimentoTarifa == null || pagamentoMotoristaTipo.TipoMovimentoReversaoTarifa == null)
                    throw new ControllerException("É necessário configurar o tipo de movimento de uso e reversão e o percentual para a geração de tarifa automática.");
            }
        }

        #endregion
    }
}
