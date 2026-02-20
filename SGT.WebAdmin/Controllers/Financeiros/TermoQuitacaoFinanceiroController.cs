using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using Repositorio;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TermoQuitacao", "Financeiros/TermoQuitacaoDocumento")]
    public class TermoQuitacaoFinanceiroController : BaseController
    {
		#region Construtores

		public TermoQuitacaoFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);
               
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro exiteTermoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo, false);

                if (exiteTermoQuitacao == null)
                    return new JsonpResult(false, "Termo de Quitação não encontrado.");

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiroAnexo, Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro> repAnexosTermo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiroAnexo, Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado, Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro> repDocumentoAssinado = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado, Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiroAnexo> anexos = repAnexosTermo.BuscarPorEntidade(exiteTermoQuitacao.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacoes = repositorioContaPagar.BuscarPorTermoQuitacao(exiteTermoQuitacao.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado> documentosAssinados = repDocumentoAssinado.BuscarPorEntidade(exiteTermoQuitacao.Codigo);

                decimal valorViaCreditoEmConta = exiteTermoQuitacao.PagamentosEDescontosViaCreditoEmConta;
                decimal valorViaConfirming = exiteTermoQuitacao.PagamentosEDescontosViaConfiming;
                decimal valorTotalAdiantamento = exiteTermoQuitacao.TotalAdiantamento;
                decimal valorCompensado = exiteTermoQuitacao.NotasCompensadasAdiantamentos;

                var retorno = new
                {
                    NumeroTermo = exiteTermoQuitacao.NumeroTermo,
                    Codigo = exiteTermoQuitacao.Codigo,
                    Situacao = exiteTermoQuitacao.SituacaoTermoQuitacao,
                    PossuiRegrasAprovacao = repositorioAprovacao.PossuiRegrasCadastradas(),
                    Transportador = exiteTermoQuitacao?.Transportador != null ? new { Codigo = exiteTermoQuitacao?.Transportador?.Codigo ?? 0, Descricao = exiteTermoQuitacao?.Transportador?.Descricao ?? "" } : null,
                    Filiais = (from obj in exiteTermoQuitacao?.Transportador?.Filiais
                               select new
                               {
                                   CodigoIntegracao = obj?.CodigoIntegracao ?? string.Empty,
                                   CNPJ = obj?.CNPJ ?? "",
                                   Cidade = obj?.Localidade?.Descricao ?? string.Empty,
                                   UF = obj?.LocalidadeUF ?? string.Empty,
                               }).ToList(),
                    DataInicial = exiteTermoQuitacao.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = exiteTermoQuitacao.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    PagamentosEDescontosViaCreditoEmConta = $"R$ {valorViaCreditoEmConta.ToString("n2")}",
                    PagamentosEDescontosViaConfiming = $"R$ {valorViaConfirming.ToString("n2")}",
                    CreditoEmConta = $"R$ {exiteTermoQuitacao.CreditoEmConta.ToString("n2")}",
                    TotalAdiantamento = $"R$ {valorTotalAdiantamento.ToString("n2")}",
                    NotasCompensadasAdiantamentos = $"R$ {valorCompensado.ToString("n2")}",
                    SaldoAdiantamentoEmAberto = $"R$ {exiteTermoQuitacao.TotalSaldoEmAberto.ToString("n2")}",
                    TotalGeralPagamento = $"R$ {exiteTermoQuitacao.TotalGeralPagamento.ToString("n2")}",
                    DocumentosAssinados = (
                    from documentoAssinado in documentosAssinados
                    select new
                    {
                        documentoAssinado.Codigo,
                        documentoAssinado.Descricao,
                        documentoAssinado.NomeArquivo,
                    }).ToList(),
                    Anexos = (
                                from anexo in documentosAssinados
                                select new
                                {
                                    anexo.Codigo,
                                    anexo.Descricao,
                                    anexo.NomeArquivo
                                }
                            ).ToList(),
                    Movimentacoes = (from obj in movimentacoes
                                     select new
                                     {
                                         obj.Codigo,
                                         DataDocumento = obj.DataDocumento.HasValue ? obj.DataDocumento.Value.ToString("dd/MM/yyyy") : "",
                                         TipoDocumento = obj?.TipoDocumento ?? "",
                                         ValorTotal = obj?.ValorMonetario.ToString("n2") ?? "",
                                         TaxCode = obj?.CodigoTaxa ?? "",
                                         NumeroDocumento = obj?.NumeroDocumento ?? "",
                                         TermoPagamento = obj?.TermoPagamento ?? "",
                                         NumeroCte = obj?.NumeroCte ?? ""
                                     }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (System.Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Erro ao Tentar Obter Dados");
            }
        }

        public async Task<IActionResult> AtualizarMovimentacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");
                dynamic codigoMovimentacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Movimentacoes"));
                bool avisarTransportador = Request.GetBoolParam("AvisarTransportador");

                Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro existeTermoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo, false);

                if (existeTermoQuitacao == null)
                    throw new ControllerException("Termo Quitação não encontrado");

                if (existeTermoQuitacao.SituacaoTermoQuitacao != SituacaoTermoQuitacaoFinanceiro.RejeitadoTransportador)
                    throw new ControllerException("Situação termo não permite atualizar movimentação");

                Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacoes = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
                Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro servicoTermoFinanceiro = new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacaoContaPagar = repositorioMovimentacoes.BuscarPorTermoQuitacao(codigo);

                List<int> codigoAdicionarVinculo = new List<int>();

                foreach (var codigoMovimentacao in codigoMovimentacoes)
                    codigoAdicionarVinculo.Add((int)codigoMovimentacao);

                List<int> codigoAtuaisVinculo = movimentacaoContaPagar.Select(x => x.Codigo).ToList();
                List<int> codigoRemoverVinculo = codigoAtuaisVinculo.Where(x => !codigoAdicionarVinculo.Contains(x)).ToList();

                if (codigoRemoverVinculo.Count > 0)
                    repositorioMovimentacoes.RemoverVinculoTermoQuitacao(codigoRemoverVinculo);

                servicoTermoFinanceiro.ProcessarMovimentacaoFinanceiraTermo(existeTermoQuitacao, repositorioMovimentacoes.BuscarPorCodigos(codigoAdicionarVinculo));

                if (avisarTransportador)
                {
                    Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repositorioAprovacaoTermoQuitacao = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(unitOfWork);
                    Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador existeAprovacao = repositorioAprovacaoTermoQuitacao.BuscarPrincipalPorTermoQuitacao(existeTermoQuitacao.Codigo);

                    if (existeAprovacao == null)
                        throw new ControllerException("Não foi possivel encontrar Aprovação do termo de quitação");

                    existeAprovacao.Situacao = SituacaoAprovacaoTermoQuitacaoTransportador.Pendente;
                    existeTermoQuitacao.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador;
                    existeTermoQuitacao.SituacaoAprovacaoTransportador = SituacaoAprovacaoTermoQuitacaoTransportador.Pendente;

                    repositorioAprovacaoTermoQuitacao.Atualizar(existeAprovacao);
                    repositorioTermoQuitacao.Atualizar(existeTermoQuitacao);

                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterPreviaTermo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Financeiro.TermoQuitacao repTermo = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
            try
            {
                ObterDadosPreviaTermo(unitOfWork,
                    out int codigoTransportador,
                    out DateTime? dataInicial,
                    out DateTime? dataFinal,
                    ref repTermo,
                    out Dominio.Entidades.Empresa existeTransportador,
                    out decimal valorViaCreditoEmConta,
                    out decimal valorViaConfirming,
                    out decimal valorTotalAdiantamento,
                    out decimal valorCompensado,
                    out decimal valorCreditoEmConta);

                var retorno = new
                {
                    NumeroTermo = repTermo.ObterUltimoNumero() + 1,
                    Codigo = 0,
                    Situacao = SituacaoTermoQuitacaoFinanceiro.Novo,
                    PossuiRegrasAprovacao = false,
                    Transportador = existeTransportador != null ? new { Codigo = existeTransportador?.Codigo ?? 0, Descricao = existeTransportador?.Descricao ?? "" } : null,
                    Filiais = (from obj in existeTransportador?.Filiais
                               select new
                               {
                                   CodigoIntegracao = obj?.CodigoIntegracao ?? string.Empty,
                                   CNPJ = obj?.CNPJ ?? "",
                                   Cidade = obj?.Localidade?.Descricao ?? string.Empty,
                                   UF = obj?.LocalidadeUF ?? string.Empty,
                               }).ToList(),
                    DataInicial = dataInicial?.ToString("dd/MM/yyyyy") ?? "",
                    DataFinal = dataFinal?.ToString("dd/MM/yyyy") ?? "",
                    PagamentosEDescontosViaCreditoEmConta = $"R$ {valorViaCreditoEmConta.ToString("n2")}",
                    PagamentosEDescontosViaConfiming = $"R$ {valorViaConfirming.ToString("n2")}",
                    CreditoEmConta = $"R$ {valorCreditoEmConta.ToString("n2")}",
                    TotalAdiantamento = $"R$ {valorTotalAdiantamento.ToString("n2")}",
                    NotasCompensadasAdiantamentos = $"R$ {valorCompensado.ToString("n2")}",
                    SaldoAdiantamentoEmAberto = $"R$ {(valorTotalAdiantamento - valorCompensado).ToString("n2")}",
                    TotalGeralPagamento = $"R$ {(valorCreditoEmConta + valorTotalAdiantamento).ToString("n2")}"
                };

                return new JsonpResult(retorno);

            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar Obter Previa");
            }
        }

        public async Task<IActionResult> GerarTermoManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoTransportador = Request.GetIntParam("Transportador");
                DateTime? DataInicial = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? DataFinal = Request.GetNullableDateTimeParam("DataFinal");

                if (!DataInicial.HasValue || !DataInicial.HasValue)
                    throw new ControllerException("Precisa informar datas de inicio e fim valida");

                if (codigoTransportador == 0)
                    throw new ControllerException("Transportador não informador");

                Repositorio.Empresa repostiorioTransportador = new Repositorio.Empresa(unitOfWork);
                Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro servicoTermoFinanceiro = new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork);

                bool sucesso = servicoTermoFinanceiro.ProcessarCriacaoNovoTermo(repostiorioTransportador.BuscarPorCodigo(codigoTransportador), out int codigoTermoGerado, DataInicial, DataFinal);

                if (!sucesso)
                    throw new ControllerException("Não foi possivel gerar termo de quitação");

                unitOfWork.CommitChanges();

                return new JsonpResult(new { Codigo = codigoTermoGerado });
            }
            catch (ServicoException exe)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, exe.Message);
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Problema ao tentar gerar o termo manualmente");
            }
        }

        public async Task<IActionResult> ExportarTermo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTermo = Request.GetIntParam("Codigo");

                if (codigoTermo == 0)
                    return new JsonpResult(false, "Necessário selecionar um termo de quitação!");

                byte[] pdf = ReportRequest.WithType(ReportType.TermoQuitacao)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoTermoQuitacao", codigoTermo.ToString())
                    .CallReport()
                    .GetContentFile();
                
                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o termo de quitação!");

                return Arquivo(pdf, "application/pdf", "Termo de quitação.pdf");

            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesGeraisAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro exiteTermoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo, false);

                if (exiteTermoQuitacao == null)
                    return new JsonpResult(false, true, "Termo Quitação Financeiro não encontrada.");

                return new JsonpResult(new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork).ObterDetalhesAprovacao(exiteTermoQuitacao));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da aprovação da Fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro exiteTermoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo, false);

                if (exiteTermoQuitacao == null)
                    return new JsonpResult(false, false, "Termo Quitação Financeiro não encontrada.");

                if (exiteTermoQuitacao.SituacaoTermoQuitacao != SituacaoTermoQuitacaoFinanceiro.SemRegraProvisao)
                    return new JsonpResult(false, false, "A situação não permite esta operação.");

                Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro servicoTermo = new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork);

                servicoTermo.ValidarProvisaoPendente(exiteTermoQuitacao);

                repositorioTermoQuitacao.Atualizar(exiteTermoQuitacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork).ObterDetalhesAprovacao(exiteTermoQuitacao));
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarRegrasCadastradas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro repositorioAprovacaoAlcadaProvisao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro(unitOfWork);

                bool possuiRegrasAprovacao = repositorioAprovacaoAlcadaProvisao.PossuiRegrasCadastradas();

                return new JsonpResult(new
                {
                    PossuiRegrasAprovacao = possuiRegrasAprovacao
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioTermoQuitacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao autorizacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioTermoQuitacaoFinanceiro = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> listaAutorizacao = repositorioTermoQuitacaoFinanceiro.BuscarAutorizacoesPorTermoQuitacaoFinanceiro(codigo, parametrosConsulta);
                int totalRegistros = listaAutorizacao.Count();

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = ObterGridPesquisa(unitOfWork);

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
        public async Task<IActionResult> PesquisaAprovacaoPendenteTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Codigo Integração", "CodigoIntegracao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repositorioAprovaoTermoQuitacao = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador> aprovacoesPendenteTransportador = repositorioAprovaoTermoQuitacao.BuscarPorTermoQuitacao(codigo);

                int totalRegistros = aprovacoesPendenteTransportador.Count;

                var lista = (
                    from autorizacao in aprovacoesPendenteTransportador
                    select new
                    {
                        Codigo = autorizacao.Codigo,
                        CodigoIntegracao = autorizacao?.Transportador?.CodigoIntegracao ?? "",
                        Transportador = autorizacao?.Transportador?.Descricao ?? "",
                        Situacao = autorizacao.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ExportarResumo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTermo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao = new Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro();
                Repositorio.Embarcador.Financeiro.TermoQuitacao repTermo = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
                byte[] arquivo = null;

                if (codigoTermo > 0)
                {
                    termoQuitacao = repTermo.BuscarPorCodigo(codigoTermo);
                }
                else
                {
                    ObterDadosPreviaTermo(unitOfWork,
                        out int codigoTransportador,
                        out DateTime? dataInicial,
                        out DateTime? dataFinal,
                        ref repTermo,
                        out Dominio.Entidades.Empresa existeTransportador,
                        out decimal valorViaCreditoEmConta,
                        out decimal valorViaConfirming,
                        out decimal valorTotalAdiantamento,
                        out decimal valorCompensado,
                        out decimal valorCreditoEmConta);

                    termoQuitacao.Codigo = Request.GetIntParam("NumeroTermo");
                    termoQuitacao.DataInicial = dataInicial;
                    termoQuitacao.DataFinal = dataFinal;
                    termoQuitacao.Transportador = existeTransportador;
                    termoQuitacao.CreditoEmConta = valorViaCreditoEmConta;
                    termoQuitacao.NotasCompensadasAdiantamentos = valorCompensado;
                    termoQuitacao.PagamentosEDescontosViaConfiming = valorViaConfirming;
                    termoQuitacao.PagamentosEDescontosViaCreditoEmConta = valorViaCreditoEmConta;
                    termoQuitacao.TotalSaldoEmAberto = valorTotalAdiantamento - valorCompensado;
                    termoQuitacao.TotalAdiantamento = valorTotalAdiantamento;
                    termoQuitacao.TotalGeralPagamento = valorCreditoEmConta + valorTotalAdiantamento;
                }

                arquivo = new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork).ExportarExcelResumoTermo(termoQuitacao);
                return Arquivo(arquivo, "application/xls", $"Resumo do Termo de Quitação {termoQuitacao.Codigo}.xls");
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o Resumo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private void ObterDadosPreviaTermo(UnitOfWork unitOfWork, out int codigoTransportador, out DateTime? dataInicial, out DateTime? dataFinal, ref Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao, out Dominio.Entidades.Empresa existeTransportador, out decimal valorViaCreditoEmConta, out decimal valorViaConfirming, out decimal valorTotalAdiantamento, out decimal valorCompensado, out decimal valorCreditoEmConta)
        {
            codigoTransportador = Request.GetIntParam("Transportador");
            dataInicial = Request.GetNullableDateTimeParam("DataInicial");
            dataFinal = Request.GetNullableDateTimeParam("DataFinal");
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacao = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);
            repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
            existeTransportador = repositorioEmpresa.BuscarPorCodigo(codigoTransportador);

            if (existeTransportador == null)
                throw new ControllerException("Precisa informar um transportador");

            List<int> codigosTransportadores = new List<int>() { existeTransportador.Codigo };
            var filiais = existeTransportador?.Filiais.ToList() ?? new List<Dominio.Entidades.Empresa>();
            codigosTransportadores.AddRange(filiais?.Select(x => x.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacoes = repositorioMovimentacao.BuscarMovimentacaoFinanceiraContaPagar(dataInicial, dataFinal, codigosTransportadores);

            valorViaCreditoEmConta = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.Pagoviacreditoemconta).Select(x => x.ValorMonetario).Sum();
            valorViaConfirming = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.PagoviaConfirming).Select(x => x.ValorMonetario).Sum();
            valorTotalAdiantamento = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.TotaldeAdiantamento).Select(x => x.ValorMonetario).Sum();
            valorCompensado = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento).Select(x => x.ValorMonetario).Sum();
            valorCreditoEmConta = (valorViaCreditoEmConta + valorViaConfirming);
        }

        private Models.Grid.Grid ObterGridPesquisa(UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro repositorioAlcadaAprovacaoPendente = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro(unitOfWork);

            var grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número do termo", "NumeroTermo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportador ", "Transportador", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data inicial do termo ", "DataInicialTermo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data final do termo ", "DataFinalTermo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Provisão Pendente", "ProvisaoPendente", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Provisão Pendente", "SituacaoProvisaoPendente", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Termo Quitação", "SituacaoTermoQuitacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Aprovação Transportador", "SituacaoAprovacaoTransportador", 10, Models.Grid.Align.left, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.OcultarCabecalho("Transportador");
                grid.OcultarCabecalho("ProvisaoPendente");
                grid.OcultarCabecalho("SituacaoProvisaoPendente");
            }

            FiltroPesquisaTermoQuitacao filtroPesquisa = ObterFiltroPesquisa();

            var parametros = grid.ObterParametrosConsulta();
            parametros.PropriedadeOrdenar = "Codigo";

            int quantidadeRegistro = repositorioTermoQuitacao.ContarConsulta(filtroPesquisa);
            List<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro> termosQuitacoes = quantidadeRegistro > 0 ? repositorioTermoQuitacao.Consultar(filtroPesquisa, parametros) : new List<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>();
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> aprovacoes = quantidadeRegistro > 0 ? repositorioAlcadaAprovacaoPendente.BuscarPorTermosQuitacao(termosQuitacoes.Select(x => x.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();

            var lista = (from obj in termosQuitacoes
                         select new
                         {
                             Codigo = obj.Codigo,
                             NumeroTermo = obj.NumeroTermo,
                             Transportador = obj.Transportador != null ? $"{obj.Transportador.CodigoIntegracao} - {obj.Transportador.RazaoSocial}" : "",
                             DataInicialTermo = obj.DataInicial.HasValue ? obj.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                             DataFinalTermo = obj.DataFinal.HasValue ? obj.DataFinal.Value.ToString("dd/MM/yyyy") : "",
                             ProvisaoPendente = aprovacoes.Any(x => x.Situacao == SituacaoAlcadaRegra.Pendente && obj.Codigo == x.TermoQuitacaoFinanceiro.Codigo) ? "SIM" : "NÃO",
                             SituacaoProvisaoPendente = aprovacoes.Where(x => x.Situacao == SituacaoAlcadaRegra.Pendente && obj.Codigo == x.TermoQuitacaoFinanceiro.Codigo)?.FirstOrDefault()?.Situacao.ObterDescricao() ?? "",
                             SituacaoAprovacaoTransportador = !string.IsNullOrWhiteSpace(obj.SituacaoAprovacaoTransportador.ObterDescricao()) ? obj.SituacaoAprovacaoTransportador.ObterDescricao() : "Pendente",
                             SituacaoTermoQuitacao = obj.SituacaoTermoQuitacao.ObterDescricao()
                         }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(quantidadeRegistro);

            return grid;
        }

        private FiltroPesquisaTermoQuitacao ObterFiltroPesquisa()
        {
            return new FiltroPesquisaTermoQuitacao()
            {
                DataInicio = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataVigenciaInicial = Request.GetDateTimeParam("DataVigenciaInicial"),
                DataVigenciaFinal = Request.GetDateTimeParam("DataVigenciaFinal"),
                NumeroTermo = Request.GetIntParam("NumeroDeTermo"),
                ProvisaoPendente = Request.GetNullableBoolParam("ProvisaoPendente"),
                SitaucaoAprovacaoProvisao = Request.GetNullableEnumParam<SituacaoAlcadaRegra>("AprovacaoPendente"),
                SitaucaoTermoQuitacao = Request.GetEnumParam<SituacaoTermoQuitacaoFinanceiro>("SituacaoTermoQuitacao"),
                Transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa?.Codigo ?? 0 :  Request.GetIntParam("Transportador"),
                SituacaoAprovacaoTransportador = Request.GetEnumParam<SituacaoAprovacaoTermoQuitacaoTransportador>("SituacaoAprovacao"),
            };
        }

        private Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao ObterDadosTermoQuitacao(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao)
        {
            Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao dsComprovanteEntrega = new Dominio.Relatorios.Embarcador.DataSource.Financeiros.TermoQuitacao()
            {
                BairroTransportador = termoQuitacao.Transportador.Bairro ?? string.Empty,
                CidadeTransportador = termoQuitacao.Transportador.Localidade?.Descricao ?? string.Empty,
                EnderecoTransportador = termoQuitacao.Transportador.Endereco ?? string.Empty,
                NumeroEnderecoTransportador = termoQuitacao.Transportador.Numero ?? string.Empty,
                UFTransportador = termoQuitacao.Transportador.Localidade?.Estado?.Sigla ?? string.Empty,
                EstadoTransportador = termoQuitacao.Transportador.Localidade?.Estado?.Nome ?? string.Empty,
                CNPJFiliais = string.Join(",", termoQuitacao.Transportador.Filiais?.Select(x => x.CNPJ_Formatado ?? string.Empty)),
                CNPJTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa.CNPJ : termoQuitacao.Transportador.CNPJ_Formatado ?? string.Empty,
                DataCriacaoTermo = termoQuitacao.DataCriacao.Value.ToString("dd/MM/yyyy") ?? string.Empty,
                DataFinalTermo = termoQuitacao.DataFinal.Value.ToString("dd/MM/yyyy") ?? string.Empty,
                InscricaoEstadualTransportador = termoQuitacao.Transportador.InscricaoEstadual ?? string.Empty,
                RazaoSocialTransportador = termoQuitacao.Transportador.RazaoSocial ?? string.Empty,
                ComplementoTransportador = !string.IsNullOrEmpty(termoQuitacao.Transportador.Complemento) ? termoQuitacao.Transportador.Complemento : "S/C",

            };

            return dsComprovanteEntrega;
        }
        #endregion
    }
}
