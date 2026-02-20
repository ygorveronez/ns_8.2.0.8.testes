using Dominio.Entidades.Embarcador.Frota;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Newtonsoft.Json.Linq;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/Infracao", "Ocorrencias/Ocorrencia")]
    public class InfracaoController : BaseController
    {
		#region Construtores

		public InfracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.Infracao infracao = new Dominio.Entidades.Embarcador.Frota.Infracao()
                {
                    Situacao = SituacaoInfracao.AguardandoProcessamento
                };

                PreencherDadosInfracao(infracao, unitOfWork);
                infracao.DataLancamento = DateTime.Now;

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(infracao.NumeroAtuacao))
                {
                    if (repositorio.ContemInfracaoMesmoNumeroAtuacao(infracao.NumeroAtuacao, infracao.Codigo))
                        throw new ControllerException("Já existe uma infração cadastrada com este mesmo Número de Autuação.");
                }

                infracao.Numero = repositorio.BuscarProximoNumero();

                repositorio.Inserir(infracao, Auditado);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    infracao.Codigo
                };

                return new JsonpResult(retorno);
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherDadosInfracao(infracao, unitOfWork);

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(infracao.NumeroAtuacao))
                {
                    if (repositorio.ContemInfracaoMesmoNumeroAtuacao(infracao.NumeroAtuacao, infracao.Codigo))
                        throw new ControllerException("Já existe uma infração cadastrada com este mesmo Número de Autuação.");
                }

                repositorio.Atualizar(infracao, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDadosAssinaturaMulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherDadosInfracaoDataAssinaturaMulta(infracao, unitOfWork);

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(infracao.NumeroAtuacao))
                {
                    if (repositorio.ContemInfracaoMesmoNumeroAtuacao(infracao.NumeroAtuacao, infracao.Codigo))
                        throw new ControllerException("Já existe uma infração cadastrada com este mesmo Número de Autuação.");
                }

                repositorio.Atualizar(infracao, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                DateTime data = Request.GetDateTimeParam("Data");
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaVeiculo = repCarga.BuscarUltimaCargaPorVeiculos(codigoVeiculo, data);

                if (cargaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar nenhum registro do motorista vinculado a veículo em uma viagem.");

                Dominio.Entidades.Usuario motorista = repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(cargaVeiculo.Codigo);

                if (motorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar nenhum registro do motorista vinculado a veículo em uma viagem.");

                return new JsonpResult(new { motorista.Codigo, motorista.Nome });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar o motorista do período.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                //if (infracao.Situacao != SituacaoInfracao.AguardandoProcessamento)
                //    return new JsonpResult(false, true, "Situação da infração não permite adicionar histórico.");

                Dominio.Entidades.Embarcador.Frota.InfracaoHistorico infracaoHistorico = new Dominio.Entidades.Embarcador.Frota.InfracaoHistorico();

                try
                {
                    PreencherHistoricoInfracao(infracao, infracaoHistorico, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.InfracaoHistorico repositorioHistorico = new Repositorio.Embarcador.Frota.InfracaoHistorico(unitOfWork);

                repositorioHistorico.Inserir(infracaoHistorico);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, $"Adicionou histórico.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { infracaoHistorico.Codigo });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o histórico da infração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.AguardandoProcessamento)
                    return new JsonpResult(false, true, "Situação da infração não permite adicionar processamento.");

                try
                {
                    PreencherProcessamentoInfracao(infracao, unitOfWork);

                    bool gerarTituloEmpresa = Request.GetBoolParam("GerarTituloEmpresa");
                    int? codigoTipoMovimentoEmpresa = Request.GetNullableIntParam("TipoMovimentoEmpresa");
                    double? cnpjPessoaTituloEmpresa = Request.GetNullableDoubleParam("PessoaTituloEmpresa");

                    infracao.GerarTituloEmpresa = gerarTituloEmpresa;
                    infracao.TipoMovimentoEmpresa = codigoTipoMovimentoEmpresa.HasValue && codigoTipoMovimentoEmpresa.Value > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoEmpresa.Value) : null;
                    infracao.PessoaTituloEmpresa = cnpjPessoaTituloEmpresa.HasValue && cnpjPessoaTituloEmpresa.Value > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjPessoaTituloEmpresa.Value) : null;
                    infracao.DescontarLancamentoAgregadoTerceiro = Request.GetBoolParam("DescontarLancamentoAgregadoTerceiro");
                    infracao.GerarOcorrencia = Request.GetBoolParam("GerarOcorrencia");

                    dynamic dynComissaoMotorista = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ComissaoMotorista"));

                    if (dynComissaoMotorista != null)
                    {
                        bool.TryParse((string)dynComissaoMotorista.LancarDescontoMotorista, out bool lancarDescontoMotorista);
                        decimal.TryParse((string)dynComissaoMotorista.DescontoComissaoMotorista, out decimal descontoComissaoMotorista);
                        int.TryParse((string)dynComissaoMotorista.JustificativaDesconto, out int codigoJustificativa);

                        infracao.LancarDescontoMotorista = lancarDescontoMotorista;
                        infracao.DescontoComissaoMotorista = descontoComissaoMotorista;
                        if (codigoJustificativa > 0)
                            infracao.JustificativaDesconto = repJustificativa.BuscarPorCodigo(codigoJustificativa);
                        else
                            infracao.JustificativaDesconto = null;

                        bool.TryParse((string)dynComissaoMotorista.ReduzirPercentualComissaoMotorista, out bool reduzirPercentualComissaoMotorista);
                        decimal.TryParse((string)dynComissaoMotorista.PercentualReducaoComissaoMotorista, out decimal percentualReducaoComissaoMotorista);

                        infracao.ReduzirPercentualComissaoMotorista = reduzirPercentualComissaoMotorista;
                        infracao.PercentualReducaoComissaoMotorista = percentualReducaoComissaoMotorista;
                    }
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(infracao.NumeroAtuacao))
                {
                    if (repositorio.ContemInfracaoMesmoNumeroAtuacao(infracao.NumeroAtuacao, infracao.Codigo))
                        throw new ControllerException("Já existe uma infração cadastrada com este mesmo Número de Autuação.");
                }

                if (!SalvarParcelas(out string erro, infracao, unitOfWork))
                    throw new ControllerException(erro);

                AtualizarAprovacao(infracao, unitOfWork);

                repositorio.Atualizar(infracao, Auditado);

                unitOfWork.CommitChanges();

                var dynTipoMovimento = new
                {
                    infracao.Codigo,
                    TipoDocumento = "MULTA",
                    NumeroDocumento = infracao.Numero,
                    Valor = infracao.InfracaoTitulo?.Valor.ToString("n2") ?? "0,00",
                    Colaborador = new
                    {
                        Codigo = infracao.Motorista?.Codigo ?? 0,
                        Descricao = infracao.Motorista?.Descricao ?? string.Empty,
                    }
                };
                return new JsonpResult(dynTipoMovimento);
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o processamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    ListaHistorico = ObterHistoricoInfracao(infracao)
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    infracao.Codigo,
                    infracao.Situacao,
                    FaturadoTitulosEmpresa = infracao.FaturadoTitulosEmpresa.HasValue ? infracao.FaturadoTitulosEmpresa.Value : false,
                    infracao.OrigemIntegracao,
                    DadosInfracao = ObterDadosInfracao(infracao),
                    HistoricoInfracao = ObterHistoricoInfracao(infracao),
                    ProcessamentoInfracao = ObterProcessamentoInfracao(infracao),
                    Resumo = ObterResumo(infracao),
                    ResumoAprovacao = ObterResumoAprovacao(infracao, unitOfWork),
                    DadosComissaoMotorista = ObterDadosComissaoMotorista(infracao),
                    TitulosEmpresa = ObterTituloEmpresa(infracao),
                    DadosSinistro = ObterDadosSinistro(infracao),
                    Anexos = ObterAnexos(infracao)
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

        public async Task<IActionResult> CancelarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagemInfracao repAcertoInfracao = new Repositorio.Embarcador.Acerto.AcertoViagemInfracao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.AguardandoProcessamento && infracao.Situacao != SituacaoInfracao.AprovacaoRejeitada)
                    return new JsonpResult(false, true, "A situação da infração não permite realizar o cancelamento.");

                if (repAcertoInfracao.ContemInfracaoEmAcerto(codigo))
                    return new JsonpResult(false, true, "Existe(m) acerto(s) com esta infração vinculada. Favor remova a infração antes de cancelar a mesma.");

                unitOfWork.Start();

                infracao.Situacao = SituacaoInfracao.Cancelada;

                repositorio.RemoverVinculosPorCodigo(codigo);
                repositorio.Atualizar(infracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar a infração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EstornarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.Finalizada)
                    return new JsonpResult(false, true, "A situação da infração não permite realizar o cancelamento.");

                if (infracao.FaturadoTitulosEmpresa.HasValue && infracao.FaturadoTitulosEmpresa.Value)
                    return new JsonpResult(false, true, "Favor estorne os títulos a pagar para a empresa antes de reverter a infração lançada.");

                if (infracao.TituloEmpresa != null && infracao.TituloEmpresa.StatusTitulo == StatusTitulo.Quitada)
                    return new JsonpResult(false, true, "A situação do título gerado a empresa se encontra quitado, favor reverta o mesmo antes de estornar a infração.");

                if (infracao.Titulo != null && infracao.Titulo.StatusTitulo == StatusTitulo.Quitada)
                    return new JsonpResult(false, true, "A situação do título gerado se encontra quitado, favor reverta o mesmo antes de estornar a infração.");

                if (infracao.Parcelas != null && infracao.Parcelas.Count > 0)
                {
                    if (infracao.Parcelas.Where(o => o.Titulo != null).Where(o => o.Titulo != null && o.Titulo.StatusTitulo == StatusTitulo.Quitada).Count() > 0)
                    {
                        return new JsonpResult(false, true, "A situação do título gerado se encontra quitado, favor reverta o mesmo antes de estornar a infração.");
                    }
                }

                unitOfWork.Start();

                string msgRetorno = "";
                if (infracao.TituloEmpresa != null && infracao.TituloEmpresa.StatusTitulo == StatusTitulo.EmAberto)
                {
                    if (!CancelarTitulo(codigo, infracao.TituloEmpresa, unitOfWork, ref msgRetorno))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, msgRetorno);
                    }
                }
                if (infracao.Titulo != null && infracao.Titulo.StatusTitulo == StatusTitulo.EmAberto)
                {
                    if (!CancelarTitulo(codigo, infracao.Titulo, unitOfWork, ref msgRetorno))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, msgRetorno);
                    }
                }
                if (infracao.Parcelas != null && infracao.Parcelas.Count > 0)
                {
                    foreach (var parcela in infracao.Parcelas)
                    {
                        if (parcela != null && parcela.Titulo != null)
                        {
                            if (!CancelarTitulo(codigo, parcela.Titulo, unitOfWork, ref msgRetorno))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, msgRetorno);
                            }
                        }
                    }
                }

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorInfracao(codigo);

                if (rateioDespesaVeiculo != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rateioDespesaVeiculo, null, "Excluido e revertido o rateio de despesa do veículo a partir da infração", unitOfWork);

                    rateioDespesaVeiculo.Veiculos = null;
                    rateioDespesaVeiculo.SegmentosVeiculos = null;
                    rateioDespesaVeiculo.CentroResultados = null;

                    repRateioDespesaVeiculoLancamento.DeletarPorRateioDespesaVeiculo(rateioDespesaVeiculo.Codigo);
                    repRateioDespesaVeiculo.Deletar(rateioDespesaVeiculo);
                }

                infracao.Situacao = SituacaoInfracao.AguardandoProcessamento;

                repositorio.Atualizar(infracao, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, $"Estornou infração.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao estornar a infração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao repositorioAprovacao = new Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

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

        public async Task<IActionResult> DownloadReciboCompleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var arquivo = ReportRequest.WithType(ReportType.ReciboInfracao)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoRecibo", Request.GetIntParam("Codigo").ToString())
                    .CallReport()
                    .GetContentFile();

                return Arquivo(arquivo, "application/pdf", "Recibo.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Falha ao gerar recibo completo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirRecibo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.Finalizada)
                    return new JsonpResult(false, true, "A situação da infração não permite imprimir o recibo.");

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R107_Recibo, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R107_Recibo, TipoServicoMultisoftware, "Relatorio de Recibo", "MovimentoFinanceiro", "ReciboFinanceiro.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> dadosRecibo = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro>();
                dadosRecibo.Add(new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro()
                {
                    Via = 1,
                    Acrescimo = infracao.Titulo?.Acrescimo ?? 0,
                    CNPJEmpresa = this.Empresa.CNPJ_Formatado,
                    CNPJPessoa = infracao.Motorista != null ? infracao.Motorista.CPF_Formatado : infracao.Pessoa.CPF_CNPJ_Formatado,
                    Data = infracao.Titulo?.DataVencimento.Value ?? DateTime.Now,
                    Desconto = infracao.Titulo?.Desconto ?? 0,
                    Documento = infracao.Numero + " / " + infracao.NumeroAtuacao,
                    NomeEmpresa = this.Empresa.RazaoSocial,
                    Observacao = "INFRAÇÃO Nº " + infracao.Numero + " Nº AUTUAÇÃO " + infracao.NumeroAtuacao,
                    Parcela = 1,
                    Pessoa = infracao.Motorista != null ? infracao.Motorista.Nome : infracao.Pessoa.Nome,
                    TipoDocumento = "INFRAÇÃO",
                    ValorPago = infracao.Titulo?.ValorPago ?? 0,
                    ValorTotal = infracao.InfracaoTitulo?.DataVencimento > DateTime.Today.AddDays(-1) ? infracao.InfracaoTitulo?.Valor ?? 0 : infracao.InfracaoTitulo?.ValorAposVencimento ?? 0,
                    ObservacaoUsuario = infracao.Observacao ?? string.Empty
                }
                );
                dadosRecibo.Add(new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro()
                {
                    Via = 2,
                    Acrescimo = dadosRecibo[0].Acrescimo,
                    CNPJEmpresa = dadosRecibo[0].CNPJEmpresa,
                    CNPJPessoa = dadosRecibo[0].CNPJPessoa,
                    Data = dadosRecibo[0].Data,
                    Desconto = dadosRecibo[0].Desconto,
                    Documento = dadosRecibo[0].Documento,
                    NomeEmpresa = dadosRecibo[0].NomeEmpresa,
                    Observacao = dadosRecibo[0].Observacao,
                    Parcela = dadosRecibo[0].Parcela,
                    Pessoa = dadosRecibo[0].Pessoa,
                    TipoDocumento = dadosRecibo[0].TipoDocumento,
                    ValorPago = dadosRecibo[0].ValorPago,
                    ValorTotal = dadosRecibo[0].ValorTotal,
                    ObservacaoUsuario = dadosRecibo[0].ObservacaoUsuario,
                }
                );

                if (dadosRecibo.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioRecibo(codigo, nomeCliente, stringConexao, relatorioControleGeracao, dadosRecibo));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro para regar o relatório.");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao imprimir o recibo.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
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
                Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao repositorioAprovacao = new Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao> listaAutorizacao = repositorioAprovacao.ConsultarAutorizacoes(codigo, parametrosConsulta);
                int totalRegistros = repositorioAprovacao.ContarAutorizacoes(codigo);

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

        public async Task<IActionResult> RemoverHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.InfracaoHistorico repositorio = new Repositorio.Embarcador.Frota.InfracaoHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.InfracaoHistorico infracaoHistorico = repositorio.BuscarPorCodigo(codigo);

                if (infracaoHistorico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                //if (infracaoHistorico.Infracao.Situacao != SituacaoInfracao.AguardandoProcessamento)
                //    return new JsonpResult(false, true, "Situação da infração não permite remover histórico.");

                unitOfWork.Start();

                repositorio.Deletar(infracaoHistorico);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, infracaoHistorico.Infracao, null, $"Removeu histórico.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o histórico da infração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                AtualizarAprovacao(infracao, unitOfWork);

                repositorio.Atualizar(infracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(infracao.Situacao != SituacaoInfracao.SemRegraAprovacao);
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

                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProcessarTituloPagar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.Finalizada)
                    return new JsonpResult(false, true, "Situação da infração não permite processar os títulos a pagar.");

                unitOfWork.Start();

                if (!SalvarParcelasEmpresa(out string erro, infracao, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                infracao.FaturadoTitulosEmpresa = true;
                repositorio.Atualizar(infracao, Auditado);

                unitOfWork.CommitChanges();

                var dynTipoMovimento = new
                {
                    infracao.Codigo
                };
                return new JsonpResult(dynTipoMovimento);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha no processamento dos títulos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EstornarTituloPagar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Repositorio.Embarcador.Frota.InfracaoTituloEmpresa repInfracaoTituloEmpresa = new Repositorio.Embarcador.Frota.InfracaoTituloEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, auditavel: true);
                var servicoInfracao = new Servicos.Embarcador.Frota.Infracao(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servicoProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (infracao.Situacao != SituacaoInfracao.Finalizada)
                    return new JsonpResult(false, true, "Situação da infração não permite estornar os títulos a pagar.");

                if (infracao.FaturadoTitulosEmpresa.HasValue && !infracao.FaturadoTitulosEmpresa.Value)
                    return new JsonpResult(false, true, "Situação da infração não permite estornar os títulos a pagar.");

                unitOfWork.Start();
                DateTime? dataVencimentoTitulo = null;

                string msgRetorno = "";
                if (infracao.TitulosEmpresa != null && infracao.TitulosEmpresa.Count > 0)
                {
                    if (VerificarTituloBaixa(infracao, unitOfWork))
                        return new JsonpResult(false, true, "Há título(s) vinculado(s) a baixa(s), favor cancelar a(s) mesma(s) para efetuar esse procedimento.");

                    foreach (var parcela in infracao.TitulosEmpresa)
                    {
                        if (parcela != null && parcela.Titulo != null)
                        {
                            if (!CancelarTitulo(codigo, parcela.Titulo, unitOfWork, ref msgRetorno))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, msgRetorno);
                            }
                            dataVencimentoTitulo = parcela.Titulo.DataVencimento;
                            parcela.Titulo = null;
                            repInfracaoTituloEmpresa.Atualizar(parcela);
                        }
                    }
                    decimal valorTotalTitulo = 0;
                    if (ConfiguracaoEmbarcador.UtilizarValorDescontatoComissaoMotoristaInfracao)
                        valorTotalTitulo = infracao.DescontoComissaoMotorista;
                    else
                        valorTotalTitulo = infracao.TitulosEmpresa.Sum(o => o.Valor);

                    if (valorTotalTitulo > 0 && infracao.ResponsavelPagamentoInfracao == ResponsavelPagamentoInfracao.Condutor && infracao.TipoInfracao != null && infracao.TipoInfracao.GerarMovimentoFichaMotorista == true && infracao.TipoInfracao.TipoMovimentoFichaMotorista != null)
                    {
                        servicoProcessoMovimento.GerarMovimentacao(
                                null,
                                (dataVencimentoTitulo.HasValue && dataVencimentoTitulo.Value > DateTime.MinValue ? dataVencimentoTitulo.Value : infracao.DataLimiteIndicacaoCondutor.Value),
                                valorTotalTitulo,
                                infracao.Numero.ToString(),
                                $"ESTORNO DA INFRAÇÃO DE NÚMERO {infracao.Numero}",
                                unitOfWork,
                                TipoDocumentoMovimento.Outros,
                                TipoServicoMultisoftware,
                                infracao.Motorista.Codigo,
                                infracao.TipoInfracao.TipoMovimentoFichaMotorista.PlanoDeContaDebito,
                                infracao.TipoInfracao.TipoMovimentoFichaMotorista.PlanoDeContaCredito,
                                0,
                                TipoMovimentoEntidade.Entrada
                            );
                    }
                }

                infracao.FaturadoTitulosEmpresa = false;
                repositorio.Atualizar(infracao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, "Estornou os títulos a pagar gerados para a empresa.", unitOfWork);

                unitOfWork.CommitChanges();

                var dynTipoMovimento = new
                {
                    infracao.Codigo
                };
                return new JsonpResult(dynTipoMovimento);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao estornar o processamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (string.IsNullOrWhiteSpace(infracao.ArquivoIntegracao))
                    return new JsonpResult(false, true, "Arquivo não encontrado.");

                byte[] data = System.Text.Encoding.Default.GetBytes(infracao.ArquivoIntegracao);

                if (data == null)
                    return new JsonpResult(false, true, "Arquivo não convertido.");

                return Arquivo(data, "application/json", string.Concat("Arquivo de integração ", infracao.Numero, ".json"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo, true);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                infracao.Situacao = SituacaoInfracao.AguardandoProcessamento;

                PreencherDadosInfracao(infracao, unitOfWork);

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(infracao.NumeroAtuacao))
                {
                    if (repositorio.ContemInfracaoMesmoNumeroAtuacao(infracao.NumeroAtuacao, infracao.Codigo))
                        throw new ControllerException("Já existe uma infração cadastrada com este mesmo Número de Autuação.");
                }

                repositorio.Atualizar(infracao, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarValorAtualizadoPorInfracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoInfracao = Request.GetIntParam("Codigo");
                int codigoParcela = Request.GetIntParam("CodigoParcela");

                decimal valor = Request.GetDecimalParam("Valor");
                decimal valorVencimento = Request.GetDecimalParam("ValorAposVencimento");

                Repositorio.Embarcador.Frota.InfracaoParcela repositorioInfracaoParcela = new Repositorio.Embarcador.Frota.InfracaoParcela(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.InfracaoParcela infracaoParcela = repositorioInfracaoParcela.BuscarPorInfracaoEParcela(codigoInfracao, codigoParcela);

                if (infracaoParcela == null)
                    throw new ControllerException("Não há parcela na infração, não sendo possível atualizar os valores.");

                infracaoParcela.Valor = valor;
                infracaoParcela.ValorAposVencimento = valorVencimento;

                repositorioInfracaoParcela.Atualizar(infracaoParcela);

                var retorno = new
                {
                    Valor = infracaoParcela.Valor,
                    ValorAposVencimento = infracaoParcela.ValorAposVencimento,
                };

                return new JsonpResult(retorno);
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool SalvarParcelasEmpresa(out string erro, Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.InfracaoTituloEmpresa repInfracaoTituloEmpresa = new Repositorio.Embarcador.Frota.InfracaoTituloEmpresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            var servicoInfracao = new Servicos.Embarcador.Frota.Infracao(unitOfWork);

            int? codigoTipoMovimentoTitulo = Request.GetNullableIntParam("TipoMovimentoTitulo");
            int? codigoEmpresa = Request.GetNullableIntParam("Empresa");
            double? cnpjPessoaTituloEmpresa = Request.GetNullableDoubleParam("PessoaTituloEmpresa");
            dynamic parcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parcelas"));

            if (infracao.TitulosEmpresa != null && infracao.TitulosEmpresa.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var parcela in parcelas)
                {
                    if (parcela.Codigo != null)
                    {
                        int.TryParse((string)parcela.Codigo, out int codigoParcela);
                        if (codigoParcela > 0)
                            codigos.Add(codigoParcela);
                    }
                }
                List<Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa> parcelasDeletar = (from obj in infracao.TitulosEmpresa where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < parcelasDeletar.Count; i++)
                {
                    if (parcelasDeletar[i].Titulo != null && parcelasDeletar[i].Titulo.StatusTitulo == StatusTitulo.EmAberto)
                    {
                        string msgRetorno = "";
                        if (!CancelarTitulo(infracao.Codigo, parcelasDeletar[i].Titulo, unitOfWork, ref msgRetorno))
                        {
                            erro = msgRetorno;
                            return false;
                        }
                    }
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, "Removeu o título a pagar " + parcelasDeletar[i].Descricao + ".", unitOfWork);

                    repInfracaoTituloEmpresa.Deletar(parcelasDeletar[i]);
                }

            }

            if (parcelas == null || parcelas.Count == 0)
            {
                erro = "Favor simule as parcelas antes da geração dos títulos";
                return false;
            }

            foreach (var parcela in parcelas)
            {
                Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa par = null;

                int codigo = 0;

                if (parcela.Codigo != null && int.TryParse((string)parcela.Codigo, out codigo))
                    par = repInfracaoTituloEmpresa.BuscarPorCodigo(codigo, false);

                if (par == null)
                    par = new Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa();
                else
                    par.Initialize();

                DateTime.TryParseExact((string)parcela.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimento);

                par.Pessoa = cnpjPessoaTituloEmpresa.HasValue && cnpjPessoaTituloEmpresa > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjPessoaTituloEmpresa.Value) : null;
                par.Empresa = codigoEmpresa.HasValue && codigoEmpresa.Value > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa.Value) : null;
                par.TipoMovimento = codigoTipoMovimentoTitulo.HasValue && codigoTipoMovimentoTitulo > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoTitulo.Value) : null;
                if (par.TipoMovimento == null && infracao.TipoInfracao != null && infracao.TipoInfracao.TipoMovimentoTituloEmpresa != null)
                    par.TipoMovimento = infracao.TipoInfracao.TipoMovimentoTituloEmpresa;
                par.Infracao = infracao;
                par.DataVencimento = dataVencimento;
                par.Parcela = (int)parcela.Parcela;
                par.CodigoBarras = (string)parcela.CodigoBarras;
                par.Valor = Utilidades.Decimal.Converter((string)parcela.Valor);

                if (par.Codigo > 0)
                {
                    repInfracaoTituloEmpresa.Atualizar(par);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, "Atualizou o título a pagar " + par.Descricao + ".", unitOfWork);
                }
                else
                {
                    repInfracaoTituloEmpresa.Inserir(par);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, "Adicionou o título a pagar " + par.Descricao + ".", unitOfWork);
                }
            }
            servicoInfracao.GerarTituloEmpresa(infracao, TipoServicoMultisoftware, this.Empresa, ConfiguracaoEmbarcador.UtilizarValorDescontatoComissaoMotoristaInfracao);
            erro = string.Empty;
            return true;
        }

        private bool SalvarParcelas(out string erro, Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.InfracaoParcela repInfracaoParcela = new Repositorio.Embarcador.Frota.InfracaoParcela(unitOfWork);

            dynamic parcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parcelas"));

            if (infracao.Parcelas != null && infracao.Parcelas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var parcela in parcelas)
                    if (parcela.Codigo != null)
                        codigos.Add(((string)parcela.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> parcelasDeletar = (from obj in infracao.Parcelas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < parcelasDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, "Removeu a parcela " + parcelasDeletar[i].Descricao + ".", unitOfWork);

                    repInfracaoParcela.Deletar(parcelasDeletar[i]);
                }

            }

            decimal valorTotal = 0m, valorTotalAposVencimento = 0m;

            bool possuiParcela = false;

            foreach (var parcela in parcelas)
            {
                possuiParcela = true;

                Dominio.Entidades.Embarcador.Frota.InfracaoParcela par = null;

                int codigo = 0;

                if (parcela.Codigo != null && int.TryParse((string)parcela.Codigo, out codigo))
                    par = repInfracaoParcela.BuscarPorCodigo(codigo, false);

                if (par == null)
                    par = new Dominio.Entidades.Embarcador.Frota.InfracaoParcela();
                else
                    par.Initialize();

                DateTime.TryParseExact((string)parcela.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimento);

                par.Infracao = infracao;
                par.DataVencimento = dataVencimento;
                par.Parcela = (int)parcela.Parcela;
                par.Valor = Utilidades.Decimal.Converter((string)parcela.Valor);
                par.ValorAposVencimento = Utilidades.Decimal.Converter((string)parcela.ValorAposVencimento);

                valorTotal += par.Valor;
                valorTotalAposVencimento += par.ValorAposVencimento;

                if (par.Codigo > 0)
                {
                    repInfracaoParcela.Atualizar(par);
                    var alteracoes = par.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, alteracoes, "Alterou a parcela " + par.Descricao + ".", unitOfWork);
                }
                else
                {
                    repInfracaoParcela.Inserir(par);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracao, null, "Adicionou a parcela " + par.Descricao + ".", unitOfWork);
                }
            }

            if (possuiParcela)
            {
                if (valorTotal != infracao.InfracaoTitulo.Valor)
                {
                    erro = $"O valor ({infracao.InfracaoTitulo.Valor.ToString("n2")}) difere do valor total das parcelas ({valorTotal.ToString("n2")}).";
                    return false;
                }

                if (valorTotalAposVencimento != infracao.InfracaoTitulo.ValorAposVencimento)
                {
                    erro = $"O valor após o vencimento ({infracao.InfracaoTitulo.ValorAposVencimento.ToString("n2")}) difere do valor após o vencimento das parcelas ({valorTotalAposVencimento.ToString("n2")}).";
                    return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        private void AtualizarAprovacao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            var servicoInfracao = new Servicos.Embarcador.Frota.Infracao(unitOfWork);

            servicoInfracao.EtapaAprovacao(infracao, TipoServicoMultisoftware, this.Empresa, ConfiguracaoEmbarcador.UtilizarValorDescontatoComissaoMotoristaInfracao);
        }

        private void PreencherDadosInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            double codigoEmitente = Request.GetDoubleParam("Emitente");
            double codigoDestinatario = Request.GetDoubleParam("Destinatario");

            infracao.NumeroAtuacao = Request.GetNullableStringParam("NumeroAtuacao");
            infracao.Data = Request.GetNullableDateTimeParam("Data") ?? throw new ControllerException("Data é obrigatória.");
            infracao.TipoInfracao = ObterTipoInfracao(unitOfWork);
            infracao.Local = ObterLocal(infracao.TipoInfracao);
            infracao.Cidade = ObterCidade(unitOfWork, infracao.TipoInfracao);
            infracao.Veiculo = ObterVeiculo(unitOfWork);
            infracao.Observacao = Request.GetNullableStringParam("Observacao");

            infracao.DataAssinaturaMulta = Request.GetNullableDateTimeParam("DataAssinaturaMulta") ?? null;

            infracao.Motorista = ObterMotorista(unitOfWork);
            infracao.Funcionario = ObterFuncionario(unitOfWork);
            infracao.OrgaoEmissor = ObterOrgaoEmissor(unitOfWork);
            infracao.DataEmissaoInfracao = Request.GetNullableDateTimeParam("DataEmissaoInfracao");
            infracao.DataLimiteIndicacaoCondutor = Request.GetNullableDateTimeParam("DataLimiteIndicacaoCondutor");
            infracao.TipoOcorrenciaInfracao = Request.GetEnumParam<TipoOcorrenciaInfracao>("TipoOcorrenciaInfracao");

            //Sinistro
            infracao.DataSinistro = Request.GetNullableDateTimeParam("DataSinistro");
            infracao.DataEmbarque = Request.GetNullableDateTimeParam("DataEmbarque");
            infracao.NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal");
            infracao.CargaCTe = ObterCargaCte(unitOfWork);
            infracao.Segurado = Request.GetBoolParam("Segurado");
            infracao.LimpezaPista = Request.GetBoolParam("LimpezaPista");
            infracao.Seguradora = ObterSeguradora(unitOfWork);
            infracao.Carga = ObterCarga(unitOfWork);
            infracao.Destinatario = ObterCliente(codigoDestinatario, unitOfWork);
            infracao.Emitente = ObterCliente(codigoEmitente, unitOfWork);
            infracao.ProdutoCarga = Request.GetStringParam("ProdutoCarga");
            infracao.ValorNotaFiscal = Request.GetDecimalParam("ValorNotaFiscal");
            infracao.ValorEstimadoPrejuizo = Request.GetDecimalParam("ValorEstimadoPrejuizo");
            infracao.ClassificacaoSinistro = Request.GetEnumParam<ClassificacaoSinistro>("ClassificaoSinsitro");
            infracao.CausaSinistro = Request.GetStringParam("CausaSinistro");

            if (infracao.TipoInfracao != null)
            {
                infracao.LancarDescontoMotorista = infracao.TipoInfracao.LancarDescontoMotorista;
                infracao.DescontoComissaoMotorista = infracao.TipoInfracao.DescontoComissaoMotorista;
                infracao.JustificativaDesconto = infracao.TipoInfracao.JustificativaDesconto;
                infracao.ReduzirPercentualComissaoMotorista = infracao.TipoInfracao.ReduzirPercentualComissaoMotorista;
                infracao.PercentualReducaoComissaoMotorista = infracao.TipoInfracao.PercentualReducaoComissaoMotorista;
            }
        }

        private void PreencherDadosInfracaoDataAssinaturaMulta(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {                        
            infracao.DataAssinaturaMulta = Request.GetNullableDateTimeParam("DataAssinaturaMulta") ?? null;
        }

        private void PreencherHistoricoInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Dominio.Entidades.Embarcador.Frota.InfracaoHistorico infracaoHistorico, Repositorio.UnitOfWork unitOfWork)
        {
            infracaoHistorico.Data = Request.GetNullableDateTimeParam("Data") ?? throw new Exception("Data do histórico da infração é obrigatória.");
            infracaoHistorico.Infracao = infracao;
            infracaoHistorico.Observacao = Request.GetNullableStringParam("Observacao");
            infracaoHistorico.Tipo = Request.GetNullableEnumParam<TipoHistoricoInfracao>("TipoHistoricoInfracao") ?? throw new Exception("Tipo do histórico da infração é obrigatório.");
            infracaoHistorico.Usuario = this.Usuario;
        }

        private void PreencherProcessamentoInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            infracao.ResponsavelPagamentoInfracao = Request.GetNullableEnumParam<ResponsavelPagamentoInfracao>("ResponsavelPagamento") ?? throw new Exception("Responsável pelo pagamento é obrigatório.");
            infracao.Situacao = SituacaoInfracao.AguardandoAprovacao;

            switch (infracao.ResponsavelPagamentoInfracao.Value)
            {
                case ResponsavelPagamentoInfracao.Condutor:
                    infracao.Motorista = ObterMotorista(unitOfWork);
                    break;

                case ResponsavelPagamentoInfracao.Funcionario:
                    infracao.Funcionario = ObterFuncionario(unitOfWork);
                    break;

                case ResponsavelPagamentoInfracao.Outro:
                    infracao.TipoTitulo = Request.GetEnumParam<TipoTitulo>("TipoTitulo");
                    infracao.Pessoa = ObterPessoa(unitOfWork);
                    break;

                case ResponsavelPagamentoInfracao.Empresa:
                case ResponsavelPagamentoInfracao.AgregadoTerceiro:
                    infracao.Pessoa = ObterPessoa(unitOfWork);
                    break;
            }

            infracao.InfracaoTitulo = ObterTituloAdicionar(unitOfWork);
        }

        private Dominio.Entidades.Localidade ObterCidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao)
        {
            Repositorio.Localidade repositorio = new Repositorio.Localidade(unitOfWork);
            int? codigoCidade = null;

            if (!tipoInfracao.NaoObrigarInformarCidade)
            {
                codigoCidade = Request.GetNullableIntParam("Cidade") ?? throw new ControllerException("Cidade é obrigatória.");
                return repositorio.BuscarPorCodigo(codigoCidade.Value) ?? throw new ControllerException("Cidade não encontrada.");
            }

            codigoCidade = Request.GetNullableIntParam("Cidade");
            return repositorio.BuscarPorCodigo(codigoCidade.Value);
        }

        private dynamic ObterAnexos(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return (
                from anexo in infracao.Anexos
                select new
                {
                    anexo.Codigo,
                    anexo.Descricao,
                    anexo.NomeArquivo
                }
            ).ToList();
        }

        private dynamic ObterDadosComissaoMotorista(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return new
            {
                infracao.LancarDescontoMotorista,
                infracao.DescontoComissaoMotorista,
                infracao.ReduzirPercentualComissaoMotorista,
                infracao.PercentualReducaoComissaoMotorista,
                JustificativaDesconto = infracao.JustificativaDesconto != null ? new { infracao.JustificativaDesconto.Codigo, infracao.JustificativaDesconto.Descricao } : null
            };
        }

        private dynamic ObterDadosInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return new
            {
                infracao.Codigo,
                Cidade = new { Codigo = infracao.Cidade?.Codigo ?? 0, Descricao = infracao.Cidade?.Descricao ?? string.Empty },
                Data = infracao.Data.ToString("dd/MM/yyyy HH:mm"),
                infracao.Local,
                infracao.Numero,
                infracao.NumeroAtuacao,
                infracao.Observacao,
                DataEmissaoInfracao = ExtrairDataEmissaoInfracao(infracao),
                DataLimiteIndicacaoCondutor = infracao.DataLimiteIndicacaoCondutor?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                infracao.TipoOcorrenciaInfracao,
                OrgaoEmissor = new { Codigo = infracao.OrgaoEmissor?.CPF_CNPJ_SemFormato ?? "0", Descricao = infracao.OrgaoEmissor?.Descricao ?? string.Empty },
                Funcionario = new { Codigo = infracao.Funcionario?.Codigo ?? 0, Descricao = infracao.Funcionario?.Descricao ?? string.Empty },
                Motorista = new { Codigo = infracao.Motorista?.Codigo ?? 0, Descricao = infracao.Motorista?.Descricao ?? string.Empty },
                TipoInfracao = new { Codigo = infracao.TipoInfracao?.Codigo ?? 0, Descricao = infracao.TipoInfracao?.Descricao ?? string.Empty, AdicionarRenavamVeiculoObservacao = infracao.TipoInfracao?.AdicionarRenavamVeiculoObservacao ?? false },
                TipoInfracaoTipo = infracao.TipoInfracao?.Tipo ?? 0,
                TipoInfracaoRequeridos = new { infracao.TipoInfracao.NaoObrigarInformarCidade, infracao.TipoInfracao.NaoObrigarInformarLocal },
                Veiculo = new { infracao.Veiculo?.Codigo, infracao.Veiculo?.Descricao },
                GerarMovimentoFichaMotorista = infracao.TipoInfracao?.GerarMovimentoFichaMotorista ?? false,
                NaoGerarTituloFinanceiro = infracao.TipoInfracao?.NaoGerarTituloFinanceiro ?? false,
                TipoMovimentoFichaMotorista = new { Codigo = infracao.TipoInfracao?.TipoMovimentoFichaMotorista?.Codigo ?? 0, Descricao = infracao.TipoInfracao?.TipoMovimentoFichaMotorista?.Descricao ?? string.Empty },
                TipoMovimentoTituloEmpresa = new { Codigo = infracao.TipoInfracao?.TipoMovimentoTituloEmpresa?.Codigo ?? 0, Descricao = infracao.TipoInfracao?.TipoMovimentoTituloEmpresa?.Descricao ?? string.Empty },
                DataAssinaturaMulta = infracao.DataAssinaturaMulta?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataVencimento = infracao.InfracaoTitulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
            };
        }

        private dynamic ObterDadosSinistro(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return new
            {
                DataSinistro = infracao.DataSinistro?.ToString("dd/MM/yyyy") ?? string.Empty,
                DataEmbarque = infracao.DataEmbarque?.ToString("dd/MM/yyyy") ?? string.Empty,
                infracao.NumeroNotaFiscal,
                CargaCte = new { Codigo = infracao.CargaCTe?.Codigo ?? 0, Descricao = infracao.CargaCTe?.Descricao ?? string.Empty },
                infracao.Segurado,
                infracao.LimpezaPista,
                Seguradora = new { Codigo = infracao.Seguradora?.Codigo ?? 0, Descricao = infracao.Seguradora?.Nome ?? string.Empty },
                Carga = new { Codigo = infracao.Carga?.Codigo ?? 0, Descricao = infracao.Carga?.Descricao ?? string.Empty },
                infracao.ProdutoCarga,
                infracao.ValorNotaFiscal,
                infracao.ValorEstimadoPrejuizo,
                infracao.ClassificacaoSinistro,
                infracao.CausaSinistro,
                Destinatario = new { Codigo = infracao.Destinatario?.Codigo ?? 0, Descricao = infracao.Destinatario?.Nome },
                Emitente = new { Codigo = infracao.Emitente?.Codigo ?? 0, Descricao = infracao.Emitente?.Nome },
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cidade", false);
                grid.AdicionarCabecalho("CidadeCodigo", false);
                grid.AdicionarCabecalho("Local", false);
                grid.AdicionarCabecalho("OrgaoEmissor", false);
                grid.AdicionarCabecalho("OrgaoEmissorCodigo", false);
                grid.AdicionarCabecalho("VeiculoCodigo", false);
                grid.AdicionarCabecalho("MotoristaCodigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número da Atuação", "NumeroAtuacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data/Hora", "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Funcionário", "Funcionario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Ocorrência", "TipoOcorrenciaInfracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "TipoInfracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracao()
                {
                    CodigoCidade = Request.GetIntParam("Cidade"),
                    CodigoTipoInfracao = Request.GetIntParam("TipoInfracao"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    Numero = Request.GetIntParam("Numero"),
                    NumeroAtuacao = Request.GetStringParam("NumeroAtuacao"),
                    Placa = Request.GetStringParam("Placa"),
                    Situacao = Request.GetNullableEnumParam<SituacaoInfracao>("Situacao"),
                    TipoOcorrenciaInfracao = Request.GetNullableEnumParam<TipoOcorrenciaInfracao>("TipoOcorrenciaInfracao"),
                    OrgaoEmissor = Request.GetDoubleParam("OrgaoEmissor"),
                    Motorista = Request.GetIntParam("Motorista"),
                    Funcionario = Request.GetIntParam("Funcionario"),
                    DataVencimento = Request.GetNullableDateTimeParam("DataVencimento"),
                    InfracoesPendentes = Request.GetBoolParam("InfracoesPendentes"),
                    TipoInfracao = Request.GetNullableEnumParam<TipoInfracaoTransito>("TipoInfracaoTransito"),
                    TipoHistorico = Request.GetNullableEnumParam<TipoHistoricoInfracao>("TipoHistorico")
                };

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.Infracao> listaInfracao = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaInfracaoRetornar = (
                    from infracao in listaInfracao
                    select new
                    {
                        infracao.Codigo,
                        infracao.Numero,
                        infracao.NumeroAtuacao,
                        Data = infracao.Data.ToString("dd/MM/yyyy HH:mm"),
                        Situacao = infracao.Situacao.ObterDescricao(),
                        TipoInfracao = infracao.TipoInfracao.Descricao,
                        TipoInfracaoCodigo = infracao.TipoInfracao?.Codigo ?? 0,
                        OrgaoEmissorCodigo = infracao.OrgaoEmissor?.Codigo ?? 0,
                        OrgaoEmissor = infracao.OrgaoEmissor?.Descricao,
                        Veiculo = infracao.Veiculo?.Descricao,
                        VeiculoCodigo = infracao.Veiculo?.Codigo ?? 0,
                        TipoOcorrenciaInfracao = infracao.TipoOcorrenciaInfracao.ObterDescricao(),
                        Motorista = infracao.Motorista?.Descricao,
                        MotoristaCodigo = infracao.Motorista?.Codigo ?? 0,
                        Funcionario = infracao.Funcionario?.Descricao,
                        DataVencimento = infracao.InfracaoTitulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        CidadeCodigo = infracao.Cidade?.Codigo ?? 0,
                        Cidade = infracao.Cidade?.Descricao,
                        Local = infracao.Local,
                    }
                ).ToList();

                grid.AdicionaRows(listaInfracaoRetornar);
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

        private dynamic ObterHistoricoInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return (
                from historico in infracao.Historicos
                select new
                {
                    historico.Codigo,
                    CodigoAnexo = historico.Anexo?.Codigo,
                    Tipo = historico.Tipo.ObterDescricao(),
                    Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                    Operador = historico.Usuario.Nome,
                    historico.Observacao
                }
            ).ToList();
        }

        private Dominio.Entidades.Usuario ObterMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoMotorista = Request.GetIntParam("Motorista");

            if (codigoMotorista <= 0)
                return null;

            Repositorio.Usuario repositorio = new Repositorio.Usuario(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoMotorista);
        }

        private Dominio.Entidades.Cliente ObterPessoa(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpjPessoa = Request.GetNullableDoubleParam("Pessoa") ?? throw new Exception("Pessoa é obrigatória.");
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unitOfWork);

            return repositorio.BuscarPorCPFCNPJ(cpfCnpjPessoa) ?? throw new Exception("Pessoa não encontrada.");
        }

        private dynamic ObterProcessamentoInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return new
            {
                ResponsavelPagamento = infracao.ResponsavelPagamentoInfracao,
                Motorista = new { Codigo = infracao.Motorista?.Codigo ?? 0, infracao.Motorista?.Descricao },
                Pessoa = new { Codigo = infracao.Pessoa?.Codigo ?? 0, infracao.Pessoa?.Descricao },
                DataCompensacao = infracao.InfracaoTitulo != null && infracao.InfracaoTitulo.DataCompensacao.HasValue ? infracao.InfracaoTitulo?.DataCompensacao.Value.ToString("dd/MM/yyyy") ?? "" : "",
                DataVencimento = infracao.InfracaoTitulo != null && infracao.InfracaoTitulo.DataVencimento.HasValue ? infracao.InfracaoTitulo?.DataVencimento.Value.ToString("dd/MM/yyyy") ?? "" : "",
                TipoMovimentoTitulo = infracao.InfracaoTitulo != null ? new { Codigo = infracao.InfracaoTitulo.TipoMovimento?.Codigo ?? 0, Descricao = infracao.InfracaoTitulo?.TipoMovimento?.Descricao ?? "" } : null,
                infracao.InfracaoTitulo?.Valor,
                infracao.InfracaoTitulo?.ValorAposVencimento,
                Parcelas = infracao.Parcelas != null ? (from obj in infracao.Parcelas
                                                        select new
                                                        {
                                                            obj.Codigo,
                                                            DT_Enable = true,
                                                            DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                                            obj.Parcela,
                                                            Valor = obj.Valor.ToString("n2"),
                                                            ValorAposVencimento = obj.ValorAposVencimento.ToString("n2")
                                                        }).ToList() : null,
                Funcionario = new { Descricao = infracao.Funcionario?.Descricao ?? string.Empty, Codigo = infracao.Funcionario?.Codigo ?? 0 },
                infracao.DescontarLancamentoAgregadoTerceiro,
                infracao.GerarOcorrencia
            };
        }

        private dynamic ObterTituloEmpresa(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            if (infracao.TitulosEmpresa == null || infracao.TitulosEmpresa.Count == 0)
                return null;

            Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa tituloEmpresa = infracao.TitulosEmpresa.FirstOrDefault();

            return new
            {
                PessoaTituloEmpresa = new { Codigo = tituloEmpresa.Pessoa?.CPF_CNPJ ?? 0, Descricao = tituloEmpresa.Pessoa?.Nome },
                TipoMovimentoTitulo = new { Codigo = tituloEmpresa.TipoMovimento?.Codigo ?? 0, tituloEmpresa.TipoMovimento?.Descricao },
                Empresa = new { Codigo = tituloEmpresa.Empresa?.Codigo ?? 0, tituloEmpresa.Empresa?.Descricao },
                Valor = infracao.TitulosEmpresa.Sum(o => o.Valor).ToString("n2"),
                CodigoBarras = tituloEmpresa?.CodigoBarras ?? "",
                Parcelas = (from obj in infracao.TitulosEmpresa
                            select new
                            {
                                obj.CodigoBarras,
                                obj.Codigo,
                                DT_Enable = (!infracao.FaturadoTitulosEmpresa),
                                DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                obj.Parcela,
                                Valor = obj.Valor.ToString("n2")
                            }).ToList()
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoInfracao")
                return "TipoInfracao.Descricao";

            return propriedadeOrdenar;
        }

        private dynamic ObterResumo(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            return new
            {
                infracao.Codigo,
                Data = infracao.Data.ToString("dd/MM/yyyy HH:mm"),
                infracao.Numero,
                infracao.NumeroAtuacao,
                Pessoa = infracao.Pessoa?.Descricao,
                Situacao = infracao.Situacao.ObterDescricao(),
                Veiculo = infracao.Veiculo?.Descricao,
                Funcionario = infracao.Funcionario?.Descricao,
                Motorista = infracao.Motorista?.Descricao,
                infracao.TipoOcorrenciaInfracao
            };
        }

        private dynamic ObterResumoAprovacao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao repositorioAprovacao = new Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(infracao.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(infracao.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(infracao.Codigo);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes
            };
        }

        private Dominio.Entidades.Embarcador.Frota.TipoInfracao ObterTipoInfracao(Repositorio.UnitOfWork unitOfWork)
        {
            int? codigoTipoInfracao = Request.GetNullableIntParam("TipoInfracao") ?? throw new ControllerException("Tipo da infração é obrigatório.");
            Repositorio.Embarcador.Frota.TipoInfracao repositorio = new Repositorio.Embarcador.Frota.TipoInfracao(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoInfracao.Value) ?? throw new ControllerException("Tipo da infração não encontrado.");
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTe ObterCargaCte(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("CargaCte");

            if (codigo <= 0)
                return null;

            Repositorio.Embarcador.Cargas.CargaCTe repositorio = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            return repositorio.BuscarPorCodigo(codigo);
        }

        private Dominio.Entidades.Embarcador.Seguros.Seguradora ObterSeguradora(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Seguradora");

            if (codigo <= 0)
                return null;

            Repositorio.Embarcador.Seguros.Seguradora repositorio = new Repositorio.Embarcador.Seguros.Seguradora(unitOfWork);

            return repositorio.BuscarPorCodigo(codigo);
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga ObterCarga(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Carga");

            if (codigo <= 0)
                return null;

            Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            return repositorio.BuscarPorCodigo(codigo);
        }

        private Dominio.Entidades.Cliente ObterCliente(double cpfCnpj, Repositorio.UnitOfWork unitOfWork)
        {
            if (cpfCnpj <= 0d)
                return null;

            Repositorio.Cliente repositorio = new Repositorio.Cliente(unitOfWork);

            return repositorio.BuscarPorCPFCNPJ(cpfCnpj);
        }

        private Dominio.Entidades.Embarcador.Financeiro.TipoMovimento ObterTipoMovimento(Repositorio.UnitOfWork unitOfWork)
        {
            int? codigoTipoMovimento = Request.GetNullableIntParam("TipoMovimentoTitulo") ?? throw new Exception("Tipo do movimento para título é obrigatório.");
            Repositorio.Embarcador.Financeiro.TipoMovimento repositorio = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoMovimento.Value) ?? null; //throw new Exception("Tipo do movimento para título não encontrado.");
        }

        private Dominio.Entidades.Embarcador.Frota.InfracaoTitulo ObterTituloAdicionar(Repositorio.UnitOfWork unitOfWork)
        {
            if (Request.GetDecimalParam("Valor") > 0)
            {
                Dominio.Entidades.Embarcador.Frota.InfracaoTitulo infracaoTitulo = new Dominio.Entidades.Embarcador.Frota.InfracaoTitulo()
                {
                    DataCompensacao = Request.GetNullableDateTimeParam("DataCompensacao"),
                    DataVencimento = Request.GetNullableDateTimeParam("DataVencimento"),
                    TipoMovimento = ObterTipoMovimento(unitOfWork),
                    Valor = Request.GetDecimalParam("Valor"),
                    ValorAposVencimento = Request.GetDecimalParam("ValorAposVencimento")
                };

                return infracaoTitulo;
            }
            else
                return null;
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Veiculo");
            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(unitOfWork);

            if (codigo <= 0)
                return null;

            return repositorio.BuscarPorCodigo(codigo);
        }

        private Dominio.Entidades.Usuario ObterFuncionario(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Funcionario");
            Repositorio.Usuario repositorio = new Repositorio.Usuario(unitOfWork);

            if (codigo <= 0)
                return null;

            return repositorio.BuscarPorCodigo(codigo);
        }

        private Dominio.Entidades.Cliente ObterOrgaoEmissor(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpj = Request.GetDoubleParam("OrgaoEmissor");
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unitOfWork);

            if (cpfCnpj <= 0D)
                return null;

            return repositorio.BuscarPorCPFCNPJ(cpfCnpj);
        }

        private void GerarRelatorioRecibo(int codigo, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> dadosRecibo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                ReportRequest.WithType(ReportType.ReciboFinanceiro)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("PagamentoMotorista", false)
                    .AddExtraData("MovimentoFinanceiro", false)
                    .AddExtraData("Infracao", true)
                    .AddExtraData("BaixaPagar", false)
                    .AddExtraData("Carga", false)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("DadosRecibo", dadosRecibo.ToJson())
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool CancelarTitulo(int codigoInfracao, Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork, ref string msgRetorno)
        {
            msgRetorno = string.Empty;
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

            if (titulo.TipoMovimento != null)
            {
                if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, null, titulo.DataEmissao.Value.Date, titulo.ValorOriginal, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO DE INFRAÇÃO", unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo))
                {
                    msgRetorno = msgErro;
                    return false;
                }
            }

            titulo.StatusTitulo = StatusTitulo.Cancelado;
            titulo.DataCancelamento = DateTime.Now.Date;
            titulo.DataAlteracao = DateTime.Now;

            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Estornou título.", unitOfWork);
            repTitulo.Atualizar(titulo);

            return true;
        }

        private bool VerificarTituloBaixa(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            foreach (var parcela in infracao.TitulosEmpresa)
            {
                if (parcela != null && parcela.Titulo != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorTitulo(parcela.Titulo.Codigo);

                    if (tituloBaixa != null)
                        return true;
                }
            }

            return false;
        }

        private string ObterLocal(Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao)
        {
            string local = string.Empty;

            if (!tipoInfracao.NaoObrigarInformarLocal)
                local = Request.GetNullableStringParam("Local") ?? throw new ControllerException("Local é obrigatório.");

            if (tipoInfracao.NaoObrigarInformarLocal)
                local = Request.GetStringParam("Local");

            return local;
        }

        private string ExtrairDataEmissaoInfracao(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            if (infracao.DataEmissaoInfracao != null)
                return infracao.DataEmissaoInfracao?.ToString("dd/MM/yyyy HH:mm");

            if (infracao.ArquivoIntegracao == null)
                return string.Empty;

            var jsonObject = JObject.Parse(infracao.ArquivoIntegracao);

            if (jsonObject.TryGetValue("created_at", out var createdAtToken))
                if (DateTime.TryParse(createdAtToken.ToString(), out var createdAt))
                    return createdAt.ToString("dd/MM/yyyy HH:mm");

            return string.Empty;
        }

        #endregion
    }
}
