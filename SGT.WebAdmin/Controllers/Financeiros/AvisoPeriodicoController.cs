using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using Repositorio;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/AvisoPeriodico")]
    public class AvisoPeriodicoController : BaseController
    {
		#region Construtores

		public AvisoPeriodicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repositorioAvisoPeriodico = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa();
                FiltroPesquisaAvisoPeriodico filtroPesquisa = ObterFiltroPesquisa();

                int quantidadeRegistro = repositorioAvisoPeriodico.ContarConsulta(filtroPesquisa);
                List<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao> avisosPeriodicos = quantidadeRegistro > 0 ? repositorioAvisoPeriodico.Consultar(filtroPesquisa, grid.ObterParametrosConsulta()) : new List<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao>();
                var lista = (from obj in avisosPeriodicos
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 NumeroAviso = obj.Numero,
                                 DataCriacao = obj.DataCriacao.HasValue ? obj.DataCriacao.Value.ToString("dd/MM/yyyy") : "",
                                 DataInicialAviso = obj.DataInicial.HasValue ? obj.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                                 DataFinalAviso = obj.DataFinal.HasValue ? obj.DataFinal.Value.ToString("dd/MM/yyyy") : "",
                                 Situacao = obj.SituacaoAvisoPeriodico.ObterDescricao(),

                             }).ToList();
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(quantidadeRegistro);
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

                Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repositorioAvisoPeriodico = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao avisoPeriodico = repositorioAvisoPeriodico.BuscarPorCodigo(codigo, false);

                if (avisoPeriodico == null)
                    return new JsonpResult(false, "Aviso periódico não encontrado!");


                //TOTAIS
                decimal totalPendenciasTransportador = avisoPeriodico.TotalPendenciasVencidoTransportador + avisoPeriodico.TotalPendenciasAVencerTransportador;
                decimal totalPendenciasDesbloqueado = avisoPeriodico.TotalPendenciasVencidoDesbloqueado + avisoPeriodico.TotalPendenciasAVencerDesbloqueado;
                decimal totalPendenciasUnilever = avisoPeriodico.TotalPendenciasVencidoUnilever + avisoPeriodico.TotalPendenciasAVencerUnilever;
                decimal totalPendenciasBloqueioPOD = avisoPeriodico.TotalPendenciasVencidoBloqueioPOD + avisoPeriodico.TotalPendenciasAVencerBloqueioPOD;
                decimal totalPendenciasBloqueioIrregularidade = avisoPeriodico.TotalPendentesAVencerBloqueioIrregularidade + avisoPeriodico.TotalPendentesVencidaBloqueioIrregularidade;
                
                decimal totalCompensacoes = avisoPeriodico.TotalAvariasEmAberto + avisoPeriodico.TotalDebitoBaixa;
                    

                var retorno = new
                {
                    Codigo = avisoPeriodico.Codigo,
                    NumeroAviso = avisoPeriodico.Numero,
                    DataInicial = avisoPeriodico.DataInicial.Value.ToString("dd/MM/yyyy"),
                    DataFinal = avisoPeriodico.DataFinal.Value.ToString("dd/MM/yyyy"),
                    Transportador = avisoPeriodico?.Transportador != null ? new { Codigo = avisoPeriodico?.Transportador?.Codigo ?? 0, Descricao = avisoPeriodico?.Transportador?.Descricao ?? "" } : null,
                    PagamentosEDescontosViaCreditoEmConta = $"R$ {avisoPeriodico.TotalPagamentoEDescontosViaCreditoConta.ToString("n2")}",
                    PagamentosEDescontosViaConfirming = $"R$ {avisoPeriodico.TotalPagamentoEDescontosViaConfirming.ToString("n2")}",
                    CreditoEmConta = $"R$ {avisoPeriodico.TotalPagamentoEDescontosEmConta.ToString("n2")}",
                    TotalAdiantamento = $"R$ {avisoPeriodico.TotalAdiantamento.ToString("n2")}",
                    NotasCompensadasContraAdiantamentos = $"R$ {avisoPeriodico.TotalNotasCompensadasAdiantamento.ToString("n2")}",
                    SaldoAdiantamentoEmAberto = $"R$ {avisoPeriodico.TotalSaldoAdiantamentoEmAberto.ToString("n2")}",
                    TotalGeralPagamentos = $"R$ {avisoPeriodico.TotalGeralPagamentos.ToString("n2")}",
                    AvariasEmAberto = $"R$ {avisoPeriodico.TotalAvariasEmAberto.ToString("n2")}",
                    DebitoBaixaResultado = $"R$ {avisoPeriodico.TotalDebitoBaixa.ToString("n2")}",
                    TotalCompensacoes = $"R$ {totalCompensacoes.ToString("n2")}",
                    TotalVencidoTransportador = $"R$ {avisoPeriodico.TotalPendenciasVencidoTransportador.ToString("n2")}",
                    TotalAVencerTransportador = $"R$ {avisoPeriodico.TotalPendenciasAVencerTransportador.ToString("n2")}",
                    TotalVencidoDesbloqueado = $"R$ {avisoPeriodico.TotalPendenciasVencidoDesbloqueado.ToString("n2")}",
                    TotalAVencerDesbloqueado = $"R$ {avisoPeriodico.TotalPendenciasAVencerDesbloqueado.ToString("n2")}",
                    TotalUnilever = $"R$ {avisoPeriodico.TotalPendenciasVencidoUnilever.ToString("n2")}",
                    TotalAVencerUnilever = $"R$ {avisoPeriodico.TotalPendenciasAVencerUnilever.ToString("n2")}",
                    TotalVencidoBloqueioPOD = $"R$ {avisoPeriodico.TotalPendenciasVencidoBloqueioPOD.ToString("n2")}",
                    TotalAVencerBloqueioPOD = $"R$ {avisoPeriodico.TotalPendenciasAVencerBloqueioPOD.ToString("n2")}",
                    TotalPendentesAVencerBloqueioIrregularidade = $"R$ {avisoPeriodico.TotalPendenciasAVencerBloqueioPOD.ToString("n2")}",
                    TotalPendentesVencidaBloqueioIrregularidade = $"R$ {avisoPeriodico.TotalPendentesVencidaBloqueioIrregularidade.ToString("n2")}",
                    TotalPendentesBloqueioIrregularidade = $"R$ {totalPendenciasBloqueioIrregularidade.ToString("n2")}",
                    TotalPendenciasBloqueioPOD = $"R$ {totalPendenciasBloqueioPOD.ToString("n2")}",
                    TotalPendenciasUnilever = $"R$ {totalPendenciasUnilever.ToString("n2")}",
                    TotalPendenciasDesbloqueado = $"R$ {totalPendenciasDesbloqueado.ToString("n2")}",
                    TotalPendenciasTransportador = $"R$ {totalPendenciasTransportador.ToString("n2")}",
                    TotalPendentes = $"R$ {avisoPeriodico.TotalPendencias.ToString("n2")}",
                    ProjecaoRecebimento = $"R$ {avisoPeriodico.TotalProjecoesRecebimento.ToString("n2")}",
                    Filiais = (from obj in avisoPeriodico?.Transportador?.Filiais
                        select new
                        {
                            CodigoIntegracao = obj?.CodigoIntegracao ?? string.Empty,
                            CNPJ = obj?.CNPJ ?? "",
                            Cidade = obj?.Localidade?.Descricao ?? string.Empty,
                            UF = obj?.LocalidadeUF ?? string.Empty,
                        }).ToList(),

                };

                return new JsonpResult(retorno);
            }
            catch (System.Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Erro ao Tentar Obter Dados");
            }
        }

        public async Task<IActionResult> ConfirmarAviso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repAvisoPeriodico = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(unitOfWork);
                List<int> codigos = Request.GetListParam<int>("Codigos");
                string justificativa = Request.GetStringParam("Justificativa");

                List<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao> avisosPeriodicos = repAvisoPeriodico.BuscarPorCodigos(codigos);
                if (avisosPeriodicos.Count == 0)
                    throw new ControllerException("Nenhum termo selecionado");

                foreach (var avisoPeriodico in avisosPeriodicos)
                {
                    if (avisoPeriodico.SituacaoAvisoPeriodico != SituacaoAvisoPeriodicoQuitacao.AguardandoConfirmacao)
                        throw new ControllerException("O aviso não pode ser confirmado na situação atual");

                    avisoPeriodico.SituacaoAvisoPeriodico = SituacaoAvisoPeriodicoQuitacao.Confirmado;
                    repAvisoPeriodico.Atualizar(avisoPeriodico);
                    unitOfWork.CommitChanges();
                }


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
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarAviso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repAvisoPeriodico = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(unitOfWork);
                List<int> codigos = Request.GetListParam<int>("Codigos");
                string justificativa = Request.GetStringParam("Justificativa");

                if (justificativa == null || justificativa == string.Empty)
                    throw new ControllerException("Justificativa é obrigatória.");

                List<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao> avisosPeriodicos = repAvisoPeriodico.BuscarPorCodigos(codigos);
                if (avisosPeriodicos.Count == 0)
                    throw new ControllerException("Nenhum termo selecionado");

                foreach (var avisoPeriodico in avisosPeriodicos)
                {
                    if (avisoPeriodico.SituacaoAvisoPeriodico != SituacaoAvisoPeriodicoQuitacao.AguardandoConfirmacao)
                        throw new ControllerException("O aviso não pode ser confirmado na situação atual");

                    avisoPeriodico.SituacaoAvisoPeriodico = SituacaoAvisoPeriodicoQuitacao.Rejeitado;
                    avisoPeriodico.JustificativaRejeicao = justificativa;
                    repAvisoPeriodico.Atualizar(avisoPeriodico);
                    unitOfWork.CommitChanges();
                }


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
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
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
                int codigoAviso = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repAvisoPeriodico = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao avisoPeriodico = repAvisoPeriodico.BuscarPorCodigo(codigoAviso, false);
                byte[] arquivo = null;

                arquivo = new Servicos.Embarcador.Financeiro.AvisoPeriodico(unitOfWork).ExportarExcelResumoAvisoPeriodico(avisoPeriodico);
                return Arquivo(arquivo, "application/xls", $"Resumo do aviso periódico {avisoPeriodico.Codigo}.xls");
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

        private void ObterDadosAviso(UnitOfWork unitOfWork, out int codigoTransportador, out DateTime? dataInicial, out DateTime? dataFinal, ref Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao, out Dominio.Entidades.Empresa existeTransportador, out decimal valorViaCreditoEmConta, out decimal valorViaConfirming, out decimal valorTotalAdiantamento, out decimal valorCompensado, out decimal valorCreditoEmConta)
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

            valorViaCreditoEmConta = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.Pagoviacreditoemconta).Select(x => x.ValorTotal).Sum();
            valorViaConfirming = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.PagoviaConfirming).Select(x => x.ValorTotal).Sum();
            valorTotalAdiantamento = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.TotaldeAdiantamento).Select(x => x.ValorTotal).Sum();
            valorCompensado = movimentacoes.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento).Select(x => x.ValorTotal).Sum();
            valorCreditoEmConta = (valorViaCreditoEmConta + valorViaConfirming);
        }

        #endregion

        #region Metodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            var grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número do aviso", "NumeroAviso", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data geração", "DataCriacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data inicial", "DataInicialAviso", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data final", "DataFinalAviso", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);

          

            return grid;
        }

        private FiltroPesquisaAvisoPeriodico ObterFiltroPesquisa()
        {
            return new FiltroPesquisaAvisoPeriodico()
            {
                NumeroAviso = Request.GetIntParam("NumeroAviso"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataGeracaoInicial = Request.GetNullableDateTimeParam("DataGeracaoInicial"),
                DataGeracaoFinal = Request.GetNullableDateTimeParam("DataGeracaoFinal"),
                Situacao = Request.GetNullableEnumParam<SituacaoAvisoPeriodicoQuitacao>("Situacao"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo  : Request.GetIntParam("Transportador"),

            };
        }

        #endregion
    }
}
