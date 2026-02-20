using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/Provisao")]
    public class ProvisaoFechamentoController : BaseController
    {
		#region Construtores

		public ProvisaoFechamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Méodos Globais

        public async Task<IActionResult> DefirnirImpostoValorAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoProvisao = Request.GetIntParam("CodigoProvisao");
                int codigoStage = Request.GetIntParam("CodigoStage");
                int codigoImpostoValorAgregado = Request.GetIntParam("ImpostoValorAgregado");

                new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork).ReprocessarProvisaoPorStage(codigoProvisao, codigoStage, codigoImpostoValorAgregado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o IVA para a provisão");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioProvisaoProduto repositorioRateioProvisaoProduto = new Repositorio.Embarcador.Rateio.RateioProvisaoProduto(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repositorioProvisao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = repositorioDocumentoContabil.BuscarPorProvisao(codigo);
                DateTime dataLancamento = provisao.DataLancamento ?? new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork).ObterDataLancamento();
                bool possuiStage = documentosContabeis.Exists(documento => documento.Stage != null);

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentosContabeisAgrupados = documentosContabeis
                    .GroupBy(documento => new
                    {
                        CodigoPlanoConta = documento.PlanoConta.PlanoContabilidade,
                        DescricaoPlanoConta = documento.PlanoConta.Descricao,
                        CentroCusto = documento.CentroResultado.PlanoContabilidade,
                        TipoContabilizacao = documento.TipoContabilizacao
                    })
                    .Select(documentoAgrupado => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil
                    {
                        CodigoContaContabil = documentoAgrupado.Key.CodigoPlanoConta,
                        DescricaoContaContabil = documentoAgrupado.Key.DescricaoPlanoConta,
                        ValorContabilizacao = documentoAgrupado.Sum(dc => dc.ValorContabilizacao),
                        CodigoCentroResultado = documentoAgrupado.Key.CentroCusto,
                        TipoContabilizacao = documentoAgrupado.Key.TipoContabilizacao
                    })
                    .ToList();

                dynamic documentosContabeisPorStage = documentosContabeis
                    .GroupBy(documento => ValueTuple.Create(documento.Stage?.Codigo ?? 0, documento.Stage?.NumeroStage ?? string.Empty))
                    .Select(documentoAgrupado => ObterDocumentoContabelPorStage(documentoAgrupado, unitOfWork))
                    .ToList();

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DetalhamentoRateio> detalhamentoRateios = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DetalhamentoRateio>();

                foreach (Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto item in repositorioRateioProvisaoProduto.BuscarPorProvisao(provisao))
                {
                    detalhamentoRateios.Add(new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DetalhamentoRateio()
                    {
                        CodigoGrupoProduto = item.GrupoProduto.CodigoGrupoProdutoEmbarcador,
                        Descricao = item.GrupoProduto.Descricao,
                        ValorDetalhamentoFormatado = item.ValorTotalRateio
                    });
                }

                var retorno = new
                {
                    DocumentosContabeis = documentosContabeisAgrupados,
                    DocumentosContabeisPorStage = documentosContabeisPorStage,
                    DataLancamento = dataLancamento.ToString("dd/MM/yyyy"),
                    PossuiStage = possuiStage,
                    DetalhamentoRateio = detalhamentoRateios
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a provisão");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarFechamentoProvisao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repositorioProvisao.BuscarPorCodigo(codigo);

                if (provisao == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmFechamento)
                    throw new ControllerException($"Não é possivel efetuar essa ação na atual situação da provisão ({provisao.DescricaoSituacao})");

                string retorno = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork, Auditado).GerarFechamentoProvisaoIndividual(provisao);

                if (!string.IsNullOrEmpty(retorno))
                    throw new ControllerException(retorno);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a provisão");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarProvisao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);

                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);


                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigo);

                if (provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmFechamento && provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.PendenciaFechamento)
                    return new JsonpResult(false, true, "Não é possivel efetuar essa ação na atual situação da provisão (" + provisao.DescricaoSituacao + ")");

                unitOfWork.Start();

                if (provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.PendenciaFechamento)
                {
                    repDocumentoContabil.ExcluirTodosPorProvisao(provisao.Codigo);
                    repDocumentoProvisao.SetarDocumentosGerarMovimentoProvisionar(provisao.Codigo);
                }

                provisao.GerandoMovimentoFinanceiroProvisao = true;
                provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmFechamento;
                provisao.MotivoRejeicaoFechamentoProvisao = "";

                repProvisao.Atualizar(provisao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, provisao, null, "Solicitou o reprocessamento da provisão", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a provisão");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarProvisao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);

                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigo);

                if (provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmFechamento && provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.PendenciaFechamento)
                    return new JsonpResult(false, true, "Não é possivel efetuar essa ação na atual situação da provisão (" + provisao.DescricaoSituacao + ")");

                unitOfWork.Start();

                repDocumentoContabil.ExcluirTodosPorProvisao(provisao.Codigo);
                repDocumentoProvisao.SetarDocumentosLiberadosProvisionar(provisao.Codigo);
                repProvisao.Deletar(provisao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, provisao, null, "Solicitou a exclusão da provisão", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a provisão");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private dynamic ObterDocumentoContabelPorStage(IGrouping<(int CodigoStage, string NumeroStage), Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentoAgrupado, Repositorio.UnitOfWork unitOfWork)
        {
            (int CodigoStage, string NumeroStage) dadosStage = documentoAgrupado.Key;
            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = documentoAgrupado.ToList();
            List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> impostosValorAgregado = documentosContabeis.Select(documento => documento.ImpostoValorAgregado).Distinct().ToList();
            Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = (impostosValorAgregado.Count == 1) ? impostosValorAgregado.FirstOrDefault() : null;

            decimal aliquotaCofins = 0m;
            decimal aliquotaIcms = 0m;
            decimal aliquotaIss = 0m;
            decimal aliquotaPis = 0m;
            decimal valorTotalAdValorem = 0m;
            decimal valorTotalCofins = 0m;
            decimal valorTotalIcms = 0m;
            decimal valorTotalIcmsST = 0m;
            decimal valorTotalIss = 0m;
            decimal valorTotalIssRetido = 0m;
            decimal valorTotalFreteLiquido = 0m;
            decimal valorTotalGris = 0m;
            decimal valorTotalPedagio = 0m;
            decimal valorTotalPis = 0m;
            decimal valorTotalReceber = 0m;
            decimal valorTotalTaxaDescarga = 0m;
            decimal valorTotalTaxaEntrega = 0m;
            decimal valorTotalCustoFixo = 0m;
            decimal valorTotalFreteCaixa = 0m;
            decimal valorTotalFreteKM = 0m;
            decimal valorTotalFretePeso = 0m;
            decimal valorTotalFreteViagem = 0m;
            decimal valorTotalTaxa = 0m;
            decimal valorTotalPernoite = 0m;

            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil in documentosContabeis)
            {
                if (documentoContabil.TipoContaContabil == TipoContaContabil.AdValorem)
                    valorTotalAdValorem += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido || documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido2 || documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido9)
                    valorTotalFreteLiquido += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TotalReceber)
                    valorTotalReceber += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.Pedagio)
                    valorTotalPedagio += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaDescarga)
                    valorTotalTaxaDescarga += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaEntrega)
                    valorTotalTaxaEntrega += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ICMSST)
                    valorTotalIcmsST += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.GRIS)
                    valorTotalGris += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.CustoFixo)
                    valorTotalCustoFixo += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteCaixa)
                    valorTotalFreteCaixa += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteKM)
                    valorTotalFreteKM += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FretePeso)
                    valorTotalFretePeso += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteViagem)
                    valorTotalFreteViagem += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaTotal)
                    valorTotalTaxa += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.Pernoite)
                    valorTotalPernoite += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ISS)
                {
                    aliquotaIss = documentoContabil.AliquotaIss;
                    valorTotalIss += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ISSRetido)
                {
                    aliquotaIss = documentoContabil.AliquotaIss;
                    valorTotalIssRetido += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ICMS)
                {
                    aliquotaIcms = documentoContabil.DocumentoProvisao?.PercentualAliquota ?? 0m;
                    valorTotalIcms += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.PIS)
                {
                    aliquotaPis = documentoContabil.AliquotaPis;
                    valorTotalPis += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.COFINS)
                {
                    aliquotaCofins = documentoContabil.AliquotaCofins;
                    valorTotalCofins += documentoContabil.ValorContabilizacao;
                }
            }

            return new
            {
                CodigoStage = documentoAgrupado.Key.CodigoStage,
                NumeroStage = documentoAgrupado.Key.NumeroStage,
                IVA = impostoValorAgregado?.CodigoIVA ?? "",
                AdValorem = (valorTotalAdValorem > 0) ? valorTotalAdValorem.ToString("n2") : "",
                AliquotaCofins = (aliquotaCofins > 0) ? aliquotaCofins.ToString("n2") : "",
                AliquotaIcms = (aliquotaIcms > 0) ? aliquotaIcms.ToString("n2") : "",
                AliquotaIss = (aliquotaIss > 0) ? aliquotaIss.ToString("n2") : "",
                AliquotaPis = (aliquotaPis > 0) ? aliquotaPis.ToString("n2") : "",
                Cofins = (valorTotalCofins > 0) ? valorTotalCofins.ToString("n2") : "",
                Icms = (valorTotalIcms > 0) ? valorTotalIcms.ToString("n2") : "",
                IcmsST = (valorTotalIcmsST > 0) ? valorTotalIcmsST.ToString("n2") : "",
                Iss = (valorTotalIss > 0) ? valorTotalIss.ToString("n2") : "",
                IssRetido = (valorTotalIssRetido > 0) ? valorTotalIssRetido.ToString("n2") : "",
                CustoFixo = (valorTotalCustoFixo > 0) ? valorTotalCustoFixo.ToString("n2") : "",
                FreteCaixa = (valorTotalFreteCaixa > 0) ? valorTotalFreteCaixa.ToString("n2") : "",
                FreteKM = (valorTotalFreteKM > 0) ? valorTotalFreteKM.ToString("n2") : "",
                FretePeso = (valorTotalFretePeso > 0) ? valorTotalFretePeso.ToString("n2") : "",
                FreteViagem = (valorTotalFreteViagem > 0) ? valorTotalFreteViagem.ToString("n2") : "",
                FreteLiquido = (valorTotalFreteLiquido > 0) ? valorTotalFreteLiquido.ToString("n2") : "",
                FreteTotal = (valorTotalReceber > 0) ? valorTotalReceber.ToString("n2") : "",
                Gris = (valorTotalGris > 0) ? valorTotalGris.ToString("n2") : "",
                Pedagio = (valorTotalPedagio > 0) ? valorTotalPedagio.ToString("n2") : "",
                Pis = (valorTotalPis > 0) ? valorTotalPis.ToString("n2") : "",
                TaxaDescarga = (valorTotalTaxaDescarga > 0) ? valorTotalTaxaDescarga.ToString("n2") : "",
                TaxaEntrega = (valorTotalTaxaEntrega > 0) ? valorTotalTaxaEntrega.ToString("n2") : "",
                TaxaTotal = (valorTotalTaxa > 0) ? valorTotalTaxa.ToString("n2") : "",
                Pernoite = (valorTotalPernoite > 0) ? valorTotalPernoite.ToString("n2") : ""
            };
        }

        #endregion Métodos Privados
    }
}
