using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Creditos
{

    [CustomAuthorize("Creditos/CreditoLiberacao")]
    public class CreditoLiberacaoController : BaseController
    {
		#region Construtores

		public CreditoLiberacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", true);
                grid.AdicionarCabecalho("Data Solicitação", "DataSolicitacao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Solicitante", "Solicitante", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Solicitado", "ValorSolicitado", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº CT-es", "NumeroCTesCarga", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Solicitante")
                    propOrdena += ".Nome";

                if (propOrdena == "ComponenteFrete")
                    propOrdena += ".Descricao";

                if (propOrdena == "Carga")
                    propOrdena += ".CodigoCargaEmbarcador";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito situacaoSolicitacaoCredito = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito)int.Parse(Request.Params("SituacaoSolicitacaoCredito"));

                int codigo = int.Parse(Request.Params("Codigo"));

                int codRecebedor = int.Parse(Request.Params("Recebedor"));

                int codOcorrencia = 0;
                int.TryParse(Request.Params("Ocorrencia"), out codOcorrencia);

                int codSolicitado = this.Usuario.Codigo;

                string codigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");

                int numeroOcorrencia, codSolicitante = 0;
                int.TryParse(Request.Params("NumeroCargaOcorrencia"), out numeroOcorrencia);
                int.TryParse(Request.Params("Solicitante"), out codSolicitante);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);
                List<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito> listaSolicitacaoCredito = repSolicitacaoCredito.Consultar(codigo, codSolicitado, codRecebedor, codigoCargaEmbarcador, situacaoSolicitacaoCredito, codOcorrencia, numeroOcorrencia, codSolicitante, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSolicitacaoCredito.ContarConsulta(codigo, codSolicitado, codRecebedor, codigoCargaEmbarcador, situacaoSolicitacaoCredito, codOcorrencia, numeroOcorrencia, codSolicitante));
                var lista = (from p in listaSolicitacaoCredito
                             select new
                             {
                                 p.Codigo,
                                 DataSolicitacao = p.DataSolicitacao.ToString("dd/MM/yyyy"),
                                 Solicitante = p.Solicitante.Nome,
                                 ComponenteFrete = p.ComponenteFrete.Descricao,
                                 Ocorrencia = repCargaOcorrencia.BuscarPorCodigoSolicitacaoCredito(p.Codigo)?.TipoOcorrencia?.Descricao,
                                 p.DescricaoSituacao,
                                 NumeroCTesCarga = p.Carga?.NumerosCTesOriginal,
                                 Carga = p.Carga.CodigoCargaEmbarcador,
                                 ValorSolicitado = p.ValorSolicitado.ToString("n2")
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

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);
                Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito = repSolicitacaoCredito.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquiaCreditos = repHierarquiaSolicitacaoCredito.BuscarPorRecebedor(this.Usuario.Codigo);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigoSolicitacaoCredito(solicitacaoCredito.Codigo);
                cargaOcorrencia.Initialize();
                cargaOcorrencia.ContaContabil = Request.Params("ContaContabil");
                cargaOcorrencia.CFOP = Request.Params("CFOP");
                cargaOcorrencia.Responsavel = (Dominio.Enumeradores.TipoTomador)int.Parse(Request.Params("Responsavel"));
                repCargaOcorrencia.Atualizar(cargaOcorrencia, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, "Atualizou pela liberação de crédito.", unitOfWork);

                if (solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.AgLiberacao)
                {
                    solicitacaoCredito.SituacaoSolicitacaoCredito = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito)int.Parse(Request.Params("SituacaoSolicitacaoCredito"));
                    solicitacaoCredito.ValorLiberado = decimal.Parse(Request.Params("ValorLiberado"));
                    if (solicitacaoCredito.ValorLiberado <= solicitacaoCredito.ValorSolicitado)
                    {
                        solicitacaoCredito.DataRetorno = DateTime.Now;
                        solicitacaoCredito.RetornoSolicitacao = Request.Params("RetornoSolicitacao");
                        solicitacaoCredito.Creditor = this.Usuario;
                        repSolicitacaoCredito.Atualizar(solicitacaoCredito, Auditado);
                        ValidarSolicitacaoCargaComplementoFrete(solicitacaoCredito, unitOfWork);
                        ValidarSolicitacaoCargaOcorrencia(solicitacaoCredito, unitOfWork);

                        if (solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Liberado || solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Utilizado)
                        {
                            if (hierarquiaCreditos.Count > 0)
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>((string)Request.Params("CreditosUtilizados"));
                                decimal somaCreditosUtilizados = creditosUtilizados.Sum(obj => obj.ValorUtilizado);
                                if (somaCreditosUtilizados == solicitacaoCredito.ValorLiberado)
                                {
                                    string retornoUtilizacao = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, solicitacaoCredito, unitOfWork);
                                    if (!string.IsNullOrWhiteSpace(retornoUtilizacao))
                                    {
                                        unitOfWork.Rollback();
                                        return new JsonpResult(false, true, retornoUtilizacao);

                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, "O valor liberado (" + solicitacaoCredito.ValorLiberado.ToString("n2") + ") não pode ser diferente do valor de crédito disponibilizado (" + somaCreditosUtilizados.ToString("n2") + ")");
                                }
                            }
                        }

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "O valor liberado (" + solicitacaoCredito.ValorLiberado.ToString("n2") + ") não deve ser maior que o valor solicitado (" + solicitacaoCredito.ValorSolicitado.ToString("n2") + ")");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A atual situação da solicitação não permite alteração");
                }
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Creditos.SolicitacaoCredito repSolicitacaoCredito = new Repositorio.Embarcador.Creditos.SolicitacaoCredito(unitOfWork);
                Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito = repSolicitacaoCredito.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigoSolicitacaoCredito(codigo);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroPeriodo = null;
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroBooleano = null;
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros> listaOcorrenciaParametroDatas = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros>();
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroTexto = null;

                if (ocorrencia != null)
                {
                    ocorrenciaParametroPeriodo = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Periodo);
                    ocorrenciaParametroBooleano = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Booleano);
                    listaOcorrenciaParametroDatas = repCargaOcorrenciaParametros.BuscarListaPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Data);
                    ocorrenciaParametroTexto = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Texto);
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                decimal horasPeriodo = 0;
                var domingo = string.Empty;
                if (ocorrenciaParametroPeriodo != null)
                {
                    domingo = "Não";
                    if (ocorrenciaParametroPeriodo.TotalHoras == 0)
                    {
                        TimeSpan diferenca = ocorrenciaParametroPeriodo.DataFim.Value - ocorrenciaParametroPeriodo.DataInicio.Value;
                        horasPeriodo = Convert.ToDecimal(diferenca.TotalHours);
                    }
                    else
                        horasPeriodo = ocorrenciaParametroPeriodo.TotalHoras;

                    DateTime? data = ocorrenciaParametroPeriodo.DataInicio.Value.Date;
                    while (data <= ocorrenciaParametroPeriodo.DataFim.Value.Date)
                    {
                        if (data.Value.DayOfWeek == DayOfWeek.Sunday)
                        {
                            domingo = "Sim";
                            break;
                        }
                        data = data.Value.AddDays(1);
                    }

                }

                string codigoCFOPIntegracao = string.Empty;
                if (solicitacaoCredito.Carga != null && solicitacaoCredito.Carga.CargaCTes != null && solicitacaoCredito.Carga.CargaCTes.Count > 0)
                {
                    if (ocorrencia != null && ocorrencia.TipoOcorrencia != null)
                    {
                        if (solicitacaoCredito.Carga.CargaCTes.FirstOrDefault().CTe.LocalidadeInicioPrestacao.Estado.Sigla == solicitacaoCredito.Carga.CargaCTes.FirstOrDefault().CTe.LocalidadeTerminoPrestacao.Estado.Sigla)
                        {
                            if (solicitacaoCredito.Carga.CargaCTes.FirstOrDefault().CTe.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                                codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                            else
                                codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                        }
                        else
                        {
                            if (solicitacaoCredito.Carga.CargaCTes.FirstOrDefault().CTe.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento))
                                codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento;
                            else
                                codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                        }
                    }
                }

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                var dynSolicitacaoCredito = new
                {
                    solicitacaoCredito.Codigo,
                    NumeroOcorrencia = ocorrencia != null ? ocorrencia.NumeroOcorrencia : 0,
                    Carga = serCarga.ObterDetalhesDaCarga(solicitacaoCredito.Carga, TipoServicoMultisoftware, unitOfWork),
                    ComponenteFrete = new { solicitacaoCredito.ComponenteFrete.Codigo, solicitacaoCredito.ComponenteFrete.Descricao },
                    DataSolicitacao = solicitacaoCredito.DataSolicitacao.ToString("dd/MM/yyyy HH:mm"),
                    DataRetorno = solicitacaoCredito.DataRetorno.HasValue ? solicitacaoCredito.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    ValorSolicitado = solicitacaoCredito.ValorSolicitado.ToString("n2"),
                    Solicitante = new { solicitacaoCredito.Solicitante.Codigo, Descricao = "(" + solicitacaoCredito.Solicitado.CPF_Formatado + ") " + solicitacaoCredito.Solicitante.Nome },
                    Creditor = new { Codigo = solicitacaoCredito.Creditor != null ? solicitacaoCredito.Creditor.Codigo : 0, Descricao = solicitacaoCredito.Creditor != null ? solicitacaoCredito.Creditor.Nome : "" },
                    solicitacaoCredito.MotivoSolicitacao,
                    solicitacaoCredito.RetornoSolicitacao,
                    Solicitado = new { solicitacaoCredito.Solicitado.Codigo, Descricao = solicitacaoCredito.Solicitado.Nome },
                    ValorLiberado = solicitacaoCredito.ValorLiberado.ToString("n2"),
                    solicitacaoCredito.SituacaoSolicitacaoCredito,
                    solicitacaoCredito.DescricaoSituacao,
                    SolicitacaoTerceiro = ocorrencia != null && ocorrencia.Usuario != null ? ocorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro ? true : false : false,
                    SolicitacaoTransportador = ocorrencia != null && ocorrencia.Usuario != null ? ocorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao ? true : false : false,
                    ParametroPeriodoCodigo = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.ParametroOcorrencia.Codigo : 0,
                    ParametroDataInicio = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.DataInicio.ToString() : string.Empty,
                    ParametroDataFim = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.DataFim.ToString() : string.Empty,
                    ParametroBooleanoCodigo = ocorrenciaParametroBooleano != null ? ocorrenciaParametroBooleano.ParametroOcorrencia.Codigo : 0,
                    ParametroApenasReboque = ocorrenciaParametroBooleano != null ? ocorrenciaParametroBooleano.Booleano == true ? "Sim" : "Não" : String.Empty,
                    ParametroPeriodoHoras = horasPeriodo,
                    ParametroData1Codigo = listaOcorrenciaParametroDatas.Count > 0 ? listaOcorrenciaParametroDatas[0].Codigo : 0,
                    ParametroData1Descricao = listaOcorrenciaParametroDatas.Count > 0 ? listaOcorrenciaParametroDatas[0].ParametroOcorrencia.Descricao : string.Empty,
                    ParametroData1 = listaOcorrenciaParametroDatas.Count > 0 ? listaOcorrenciaParametroDatas[0].Data.ToString() : string.Empty,
                    ParametroData2Codigo = listaOcorrenciaParametroDatas.Count > 1 ? listaOcorrenciaParametroDatas[1].Codigo : 0,
                    ParametroData2Descricao = listaOcorrenciaParametroDatas.Count > 0 ? listaOcorrenciaParametroDatas[1].ParametroOcorrencia.Descricao : string.Empty,
                    ParametroData2 = listaOcorrenciaParametroDatas.Count > 1 ? listaOcorrenciaParametroDatas[1].Data.ToString() : string.Empty,
                    ParametroTextoCodigo = ocorrenciaParametroTexto != null ? ocorrenciaParametroTexto.Codigo : 0,
                    ParametroTextoDescricao = ocorrenciaParametroTexto != null ? ocorrenciaParametroTexto.ParametroOcorrencia.Descricao : string.Empty,
                    ParametroTexto = ocorrenciaParametroTexto != null ? ocorrenciaParametroTexto.Texto : string.Empty,
                    ParametroDomingo = domingo,
                    Responsavel = ocorrencia != null && ocorrencia.Responsavel != null ? ocorrencia.Responsavel : ocorrencia.Carga?.CargaCTes.FirstOrDefault()?.CTe?.TipoTomador ?? Dominio.Enumeradores.TipoTomador.Remetente,
                    CFOP = ocorrencia != null && !string.IsNullOrWhiteSpace(ocorrencia.CFOP) ? ocorrencia.CFOP : configuracaoEmbarcador != null && !string.IsNullOrWhiteSpace(codigoCFOPIntegracao) ? codigoCFOPIntegracao : string.Empty,// ocorrencia != null && !string.IsNullOrWhiteSpace(ocorrencia.CFOP) ? ocorrencia.CFOP : configuracaoEmbarcador != null && !string.IsNullOrWhiteSpace(configuracaoEmbarcador.CodigoCFOPOcorrencia) ? configuracaoEmbarcador.CodigoCFOPOcorrencia : string.Empty,
                    ContaContabil = ocorrencia != null ? ocorrencia.ContaContabil : string.Empty,
                    Remetente = ocorrencia?.Carga?.DadosSumarizados?.Remetentes ?? string.Empty,
                    Destinatario = ocorrencia?.Carga?.DadosSumarizados?.Destinatarios ?? string.Empty,
                    CodigoOcorrencia = ocorrencia != null ? ocorrencia.Codigo : 0,
                    NumeroCTesCarga = ocorrencia?.Carga?.NumerosCTesOriginal ?? string.Empty
                };

                return new JsonpResult(dynSolicitacaoCredito);
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

        #endregion

        #region Metodos Privados

        private void ValidarSolicitacaoCargaOcorrencia(Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaComplementoFrete = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaComplementoFrete.BuscarPorCodigoSolicitacaoCredito(solicitacaoCredito.Codigo);

            if (cargaOcorrencia != null)
            {
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);

                if (solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Liberado && solicitacaoCredito.ValorLiberado == solicitacaoCredito.ValorSolicitado)
                {
                    serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(cargaOcorrencia, unitOfWork);
                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
                    serCreditoMovimentacao.ConfirmarUtilizacaoCreditos(creditosUtilizadosDestino, unitOfWork);
                    string nota = string.Format(Localization.Resources.Credito.CreditoLiberacao.CreditorAutorizouUitlizacaoValorOcorrenciaCarga, solicitacaoCredito.Creditor.Nome, solicitacaoCredito.ValorLiberado.ToString("n2"), (cargaOcorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty));
                    serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitante, solicitacaoCredito.Creditor, cargaOcorrencia.Codigo, "Ocorrencias/Ocorrencia", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, "Autorizou liberação de crédito.", unitOfWork);
                }
                else
                {
                    if (solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Rejeitado)
                    {
                        List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
                        serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, TipoServicoMultisoftware, unitOfWork);
                        cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada;
                        serOcorrencia.RemoverCTeOcorrenciaEliberarCTeImportadoGerados(cargaOcorrencia, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, "Rejeitou liberação de crédito.", unitOfWork);

                        string nota = string.Format(Localization.Resources.Credito.CreditoLiberacao.CreditorRejeitouUitlizacaoValorOcorrenciaCarga, solicitacaoCredito.Creditor.Nome, solicitacaoCredito.ValorLiberado.ToString("n2"), (cargaOcorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty));
                        serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitante, solicitacaoCredito.Creditor, cargaOcorrencia.Codigo, "Ocorrencias/Ocorrencia", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);

                    }
                    else
                    {
                        cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgConfirmacaoUso;
                        string nota = string.Format(Localization.Resources.Credito.CreditoLiberacao.CreditorLiberouApenasUtilizadoValorOcorrenciaCarga, solicitacaoCredito.Creditor.Nome, solicitacaoCredito.ValorLiberado.ToString("n2"), (cargaOcorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty));
                        serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitante, solicitacaoCredito.Creditor, cargaOcorrencia.Codigo, "Ocorrencias/Ocorrencia", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.agConfirmacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, "Liberou parcialmente o crédito.", unitOfWork);
                    }
                }

                repCargaComplementoFrete.Atualizar(cargaOcorrencia);
            }
        }


        private void ValidarSolicitacaoCargaComplementoFrete(Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);

            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete = repCargaComplementoFrete.BuscarPorCodigoSolicitacaoCredito(solicitacaoCredito.Codigo);

            if (cargaComplementoFrete != null)
            {
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Carga.ComplementoFrete serCargaComplementoFrete = new Servicos.Embarcador.Carga.ComplementoFrete(unitOfWork);
                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);


                if (solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Liberado && solicitacaoCredito.ValorLiberado == solicitacaoCredito.ValorSolicitado)
                {
                    serCargaComplementoFrete.UtilizarCargaComplementoFrete(cargaComplementoFrete, unitOfWork, TipoServicoMultisoftware);
                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorCreditoComplementoDeFrete(cargaComplementoFrete.Codigo);
                    serCreditoMovimentacao.ConfirmarUtilizacaoCreditos(creditosUtilizadosDestino, unitOfWork);

                    string nota = string.Format(Localization.Resources.Credito.CreditoLiberacao.CreditorAutorizouUtilizacaoValorComplementoFreteCarga, solicitacaoCredito.Creditor.Nome, solicitacaoCredito.ValorLiberado.ToString("n2"), cargaComplementoFrete.Carga.CodigoCargaEmbarcador);
                    serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitante, solicitacaoCredito.Creditor, solicitacaoCredito.Carga.Codigo, "Cargas/Carga", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaComplementoFrete, null, "Autorizou liberação de crédito.", unitOfWork);
                }
                else
                {
                    if (solicitacaoCredito.SituacaoSolicitacaoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Rejeitado)
                    {
                        List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorCreditoComplementoDeFrete(cargaComplementoFrete.Codigo);
                        serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, TipoServicoMultisoftware, unitOfWork);
                        cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Rejeitada;
                        string nota = string.Format(Localization.Resources.Credito.CreditoLiberacao.CreditorRejeitouUtilizacaoValorComplementoFreteCarga, solicitacaoCredito.Creditor.Nome, solicitacaoCredito.ValorLiberado.ToString("n2"), cargaComplementoFrete.Carga.CodigoCargaEmbarcador);
                        serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitante, solicitacaoCredito.Creditor, solicitacaoCredito.Carga.Codigo, "Cargas/Carga", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaComplementoFrete, null, "Rejeitou liberação de crédito.", unitOfWork);
                    }
                    else
                    {
                        cargaComplementoFrete.Carga.AgConfirmacaoUtilizacaoCredito = true;
                        repCarga.Atualizar(cargaComplementoFrete.Carga);
                        cargaComplementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgConfirmacaoUso;

                        string nota = string.Format(Localization.Resources.Credito.CreditoLiberacao.CreditorLiberouApenasUtilizadoValorComplementoFreteCarga, solicitacaoCredito.Creditor.Nome, solicitacaoCredito.ValorLiberado.ToString("n2"), cargaComplementoFrete.Carga.CodigoCargaEmbarcador);
                        serNotificacao.GerarNotificacao(solicitacaoCredito.Solicitante, solicitacaoCredito.Creditor, solicitacaoCredito.Carga.Codigo, "Cargas/Carga", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.agConfirmacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaComplementoFrete, null, "Liberou parcialmente o crédito.", unitOfWork);
                    }
                }

                repCargaComplementoFrete.Atualizar(cargaComplementoFrete);
            }
        }

        #endregion
    }
}
