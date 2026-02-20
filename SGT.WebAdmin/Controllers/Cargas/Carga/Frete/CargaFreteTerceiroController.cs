using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaFreteTerceiroController : BaseController
    {
        #region Construtores

        public CargaFreteTerceiroController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> InformarValorSubContratacaoFreteManualAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork,cancellationToken);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, ConfiguracaoEmbarcador);

                int codigo = Request.GetIntParam("Carga");
                decimal novoValorFrete = Request.GetDecimalParam("ValorFreteSubcontratacaoManual");
                decimal valorPedagio = Request.GetDecimalParam("ValorPedagioSubcontratacaoManual");
                decimal percentualAdiantamento = Request.GetDecimalParam("PercentualAdiantamentoSubcontratacaoManual");
                string observacaoManual = Request.GetStringParam("ObservacaoManual");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
                if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                    throw new ControllerException(retornoVerificarOperador);

                if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                    throw new ControllerException("Não é possível re-calcular o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

                if (!carga.FreteDeTerceiro)
                    throw new ControllerException("Não é possível informar o valor de subcontração em uma carga que foi subcontratada");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiro = await repContratoFreteTerceiro.BuscarPorCargaAsync(carga.Codigo);

                contratoFreteTerceiro.Initialize();

                if (contratoFreteTerceiro.ValorFreteSubcontratacao != novoValorFrete && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteInformarValorFrete))
                    throw new ControllerException("Você não possui permissões para alterar o valor do frete do terceiro.");

                if (contratoFreteTerceiro.ValorPedagio != valorPedagio && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteInformarValorPedagio))
                    throw new ControllerException("Você não possui permissões para alterar o valor do pedágio do terceiro.");

                if ((contratoFreteTerceiro.Observacao ?? "") != observacaoManual && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAlterarObservacaoContratoFreteTerceiro))
                    throw new ControllerException("Você não possui permissões para alterar o valor da Observação no Contrato de Frete.");

                if (ConfiguracaoEmbarcador.InformarPercentualAdiantamentoCarga)
                {
                    if (contratoFreteTerceiro.PercentualAdiantamento != percentualAdiantamento && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteInformarPercentualAdiantamento))
                        throw new ControllerException("Você não possui permissões para alterar o percentual de adiantamento do terceiro.");

                    if (percentualAdiantamento > 100m)
                        throw new ControllerException("Percentual de adiantamento inválido.");
                }

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfigEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador.NaoPermiteInformarValorMaiorTerceiroTabelaFrete)
                {
                    if (contratoFreteTerceiro.ValorFreteSubContratacaoTabelaFrete < novoValorFrete)
                        throw new ControllerException("O valor a ser pago não pode ser maior que " + contratoFreteTerceiro.ValorFreteSubContratacaoTabelaFrete + ".");
                }

                contratoFreteTerceiro.ValorFreteSubcontratacao = novoValorFrete;
                contratoFreteTerceiro.ValorPedagio = valorPedagio;
                contratoFreteTerceiro.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
                if ((contratoFreteTerceiro.Observacao ?? "") != observacaoManual)
                {
                    contratoFreteTerceiro.Observacao = observacaoManual;
                    contratoFreteTerceiro.AlterouObservacaoManualmente = true;
                }

                if (ConfiguracaoEmbarcador.InformarPercentualAdiantamentoCarga)
                    contratoFreteTerceiro.PercentualAdiantamento = percentualAdiantamento;

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> acrescimosDescontos = await repContratoFreteValor.BuscarPorContratoFreteAsync(contratoFreteTerceiro.Codigo);
                foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor in acrescimosDescontos)
                {
                    if (contratoFreteValor.AplicacaoValor == AplicacaoValorJustificativaContratoFrete.NoTotal)
                    {
                        if (contratoFreteValor.TipoJustificativa == TipoJustificativa.Acrescimo)
                            contratoFreteTerceiro.ValorFreteSubcontratacao += contratoFreteValor.Valor;
                        else
                            contratoFreteTerceiro.ValorFreteSubcontratacao -= contratoFreteValor.Valor;
                    }
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFreteTerceiro, unitOfWork, TipoServicoMultisoftware);

                await repContratoFreteTerceiro.AtualizarAsync(contratoFreteTerceiro, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao freteSubContratacao = serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFreteTerceiro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Alterou os dados do contrato de frete.", unitOfWork);

                if (servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete())
                    servicoCargaAprovacaoFrete.CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.InformadoManualmente, TipoServicoMultisoftware);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(freteSubContratacao);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o valor da subcontração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RecalcularValorFreteSubContratacaoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codigo = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, $"A situação da carga ({carga.SituacaoCarga.ObterDescricao()}) não permite que a mesma seja atualizada.");

                if (carga.CalculandoFrete)
                    return new JsonpResult(false, true, $"O frete da carga está sendo calculado, não sendo possível realizar a operação.");

                if (!carga.FreteDeTerceiro)
                    return new JsonpResult(false, true, "Não é possível calcular o valor de subcontratação em uma carga que não foi subcontratada.");

                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                    return new JsonpResult(false, true, retornoVerificarOperador);

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiro = serCargaFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, TipoServicoMultisoftware, _conexao.StringConexao);

                Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao freteSubContratacao = serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFreteTerceiro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Recalculou os valores do contrato de frete.", unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(freteSubContratacao);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o valor da subcontração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        public async Task<IActionResult> RecalcularValorFreteSubContratacaoEtapaContainer(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codigo = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                if (!carga.FreteDeTerceiro)
                    return new JsonpResult(false, true, "Não é possível calcular o valor de subcontratação em uma carga que não foi subcontratada.");

                if (!carga.RealizandoOperacaoContainer)
                    return new JsonpResult(false, true, "Não é possível realizar a operação, carga já avançou da etapa de container");

                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                    return new JsonpResult(false, true, retornoVerificarOperador);

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiro = serCargaFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, TipoServicoMultisoftware, _conexao.StringConexao);

                Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao freteSubContratacao = serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFreteTerceiro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Recalculou os valores do contrato de frete.", unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(freteSubContratacao);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o valor da subcontração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConsultarValor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoJustificativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Aplicação", "DescricaoAplicacaoValor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Justificativa")
                    propOrdena += ".Descricao";
                else if (propOrdena == "DescricaoTipoJustificativa")
                    propOrdena = "TipoJustificativa";
                else if (propOrdena == "DescricaoAplicacaoValor")
                    propOrdena = "AplicacaoValor";

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> listaContratoFreteValor = repContratoFreteValor.ConsultarPorCarga(codigoCarga, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContratoFreteValor.ContarConsultaPorCarga(codigoCarga));

                var lista = (from p in listaContratoFreteValor
                             select new
                             {
                                 p.Codigo,
                                 Justificativa = p.Justificativa.Descricao,
                                 p.DescricaoTipoJustificativa,
                                 p.DescricaoAplicacaoValor,
                                 Valor = p.Valor.ToString("n2")
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

        public async Task<IActionResult> AdicionarValor()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto))
                    return new JsonpResult(false, true, "Você não possui permissões para adicionar um acréscimo/desconto.");

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoJustificativa = Request.GetIntParam("Justificativa");
                int codigo = Request.GetIntParam("Codigo");
                int codigoTaxaTerceiro = Request.GetIntParam("TaxaTerceiro");

                decimal valor = Request.GetDecimalParam("Valor");

                string observacao = Request.GetStringParam("Observacao");

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unidadeTrabalho);
                Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho, TipoServicoMultisoftware);

                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

                if (cargaCIOT != null && cargaCIOT.CIOT != null &&
                    (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto ||
                     cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem ||
                     cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado) &&
                    (contratoFrete.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                     contratoFrete.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada))
                {
                    string mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                    if (!justificativa.GerarMovimentoAutomatico)
                        return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");

                    if (justificativa.AplicacaoValorContratoFrete != AplicacaoValorJustificativaContratoFrete.NoTotal)
                        return new JsonpResult(false, true, "A justificativa não está configurada para aplicação no total do contrato, não sendo possível adicioná-la.");

                    if (Servicos.Embarcador.CIOT.CIOT.IntegrarMovimentoFinanceiro(out mensagemErro, cargaCIOT, justificativa, valor, TipoServicoMultisoftware, unidadeTrabalho))
                    {
                        Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor()
                        {
                            ContratoFrete = contratoFrete,
                            Justificativa = justificativa,
                            Valor = valor,
                            TipoJustificativa = justificativa.TipoJustificativa,
                            AplicacaoValor = justificativa.AplicacaoValorContratoFrete.HasValue ? justificativa.AplicacaoValorContratoFrete.Value : AplicacaoValorJustificativaContratoFrete.NoAdiantamento,
                            TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                            TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                            Observacao = observacao
                        };

                        repContratoFreteValor.Inserir(contratoFreteValor, Auditado);

                        if (contratoFreteValor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                            contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao + valor;
                        else
                            contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao - valor;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete.Carga, null, "Informou o valor " + valor.ToString("n2") + " de acréscimo/desconto ao contrato do terceiro.", unidadeTrabalho);

                        return new JsonpResult(true);
                    }
                    else
                        return new JsonpResult(false, true, mensagemErro);
                }

                if (contratoFrete.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual do contrato de frete.");

                contratoFrete.Initialize();

                unidadeTrabalho.Start();

                if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, valor, codigoJustificativa, observacao, codigoTaxaTerceiro, Auditado))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unidadeTrabalho, TipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete.Carga, null, "Informou o valor " + valor.ToString("n2") + " de acréscimo/desconto ao contrato do terceiro.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFrete));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarValor()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto))
                    return new JsonpResult(false, true, "Você não possui permissões para alterar um acréscimo/desconto.");

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoJustificativa = Request.GetIntParam("Justificativa");
                int codigo = Request.GetIntParam("Codigo");
                int codigoTaxaTerceiro = Request.GetIntParam("TaxaTerceiro");

                decimal valor = Request.GetDecimalParam("Valor");

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unidadeTrabalho);
                Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho, TipoServicoMultisoftware);

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(codigoCarga);

                if (contratoFrete.Carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível atualizar um acréscimo/desconto na situação atual do contrato de frete.");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorCodigo(codigo, true);

                if (contratoFreteValor.PendenciaContratoFrete != null)
                    return new JsonpResult(false, true, "Não é possível atualizar um acréscimo/desconto gerado automaticamente por pendência de um contrato de frete anterior.");

                contratoFrete.Initialize();

                unidadeTrabalho.Start();

                if (!servicoContratoFrete.AtualizarValorDoContrato(out string erro, ref contratoFrete, ref contratoFreteValor, valor, codigoJustificativa, codigoTaxaTerceiro, Auditado))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unidadeTrabalho, TipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete.Carga, null, "Alterou o valor do(a) " + contratoFreteValor.Justificativa.Descricao + " do contrato do terceiro.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFrete));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirValor()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ContratoFrete_PermiteInformarAcrescimoDesconto))
                    return new JsonpResult(false, true, "Você não possui permissões para excluir um acréscimo/desconto.");

                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unidadeTrabalho);

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorCodigo(codigo);
                if (contratoFreteValor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = contratoFreteValor.ContratoFrete;

                if (contratoFrete.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").");

                if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível excluir um acréscimo/desconto na situação atual do contrato de frete.");

                unidadeTrabalho.Start();

                contratoFrete.Initialize();

                Servicos.Embarcador.Terceiros.ContratoFrete.RemoverVinculoPendenciaContratoFrete(contratoFreteValor, unidadeTrabalho);

                if (!Servicos.Embarcador.Terceiros.ContratoFrete.RemoverValorDoContrato(out string erro, ref contratoFrete, ref contratoFreteValor, unidadeTrabalho, Auditado))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unidadeTrabalho, TipoServicoMultisoftware);

                repContratoFrete.Atualizar(contratoFrete, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoFrete.Carga, null, "Removeu o valor do(a) " + contratoFreteValor.Justificativa.Descricao + " do contrato do terceiro.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFrete));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarValorPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = repContratoFreteValor.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    contratoFreteValor.Codigo,
                    ContratoFrete = contratoFreteValor.ContratoFrete.Codigo,
                    Valor = contratoFreteValor.Valor.ToString("n2"),
                    contratoFreteValor.Observacao,
                    Justificativa = new
                    {
                        Codigo = contratoFreteValor.Justificativa.Codigo,
                        Descricao = contratoFreteValor.Justificativa.Descricao
                    },
                    TaxaTerceiro = new
                    {
                        Codigo = contratoFreteValor.TaxaTerceiro?.Codigo ?? 0,
                        Descricao = contratoFreteValor.TaxaTerceiro?.Descricao ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar valor por código.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaHistoricoIntegracaoContratoFreteValores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao repContratoFreteValoresIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao(unitOfWork);
                int codigo = Request.GetIntParam("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao> ListContratoFreteValoresIntegracao = repContratoFreteValoresIntegracao.BuscarPorCarga(codigo);
                grid.setarQuantidadeTotal(ListContratoFreteValoresIntegracao.Count());

                var retorno = (from obj in ListContratoFreteValoresIntegracao
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                   DescricaoTipo = obj.DescricaoSituacaoIntegracao,
                                   Mensagem = obj.ProblemaIntegracao
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        public async Task<IActionResult> GerarAdiantamentoTerceiroContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
                int codigoCarga = Request.GetIntParam("Carga");

                decimal valor = Request.GetDecimalParam("ValorAdiantamento");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (repComprovante.BuscarPorCodigoETipoRecebido(carga.Codigo, carga.TipoOperacao?.ConfiguracaoContainer?.TipoComprovanteColetaContainer?.Codigo ?? 0) == null && (carga.TipoOperacao?.ConfiguracaoContainer?.ExigirComprovanteColetaContainerParaSeguir ?? false))
                    return new JsonpResult(false, true, "É exigido com comprovante de coleta do container para seguir com o pagamento do adiantamento");

                if (carga.GerouAdiantamentoTerceiroContainer)
                    return new JsonpResult(false, true, "Adiantamento ao motorista já gerado anteriormente");

                if (carga.TipoOperacao?.ConfiguracaoContainer?.PagamentoMotoristaTipo == null)
                    return new JsonpResult(false, true, "O Tipo de pagamento ao motorista não foi configurado no tipo de operação");

                if (valor <= 0)
                    return new JsonpResult(false, true, "Necessário informar um valor válido para geração do adiantamento");

                if (!carga.RealizandoOperacaoContainer)
                    return new JsonpResult(false, true, "Não é possível realizar a operação, carga já avançou da etapa de container");

                unitOfWork.Start();

                string mensagem = "";
                string observacao = "GERADO A PARTIR DA ETAPA DE CONTAINER DA CARGA Nº " + carga.CodigoCargaEmbarcador.ToString() + " REFERENTE AO ADIANTAMENTO";

                if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarPagamentoMotoristaAdiantamento(ref mensagem, observacao, carga.TipoOperacao.ConfiguracaoContainer.PagamentoMotoristaTipo, valor, carga.Motoristas?.FirstOrDefault(), carga, Usuario, unitOfWork, Auditado, TipoServicoMultisoftware, unitOfWork.StringConexao))
                    return new JsonpResult(false, true, mensagem);

                carga.GerouAdiantamentoTerceiroContainer = true;
                repCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar adiantamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> ObterValorAdiantamentoEtapaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");


                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                decimal valorAdiantamento = 0;
                if (carga.GerouAdiantamentoTerceiroContainer)
                    valorAdiantamento = repPagamentoMotoristaTMS.BuscarPorCargaETipoPagamento(carga.Codigo, carga.TipoOperacao?.ConfiguracaoContainer?.PagamentoMotoristaTipo?.Codigo ?? 0)?.Valor ?? 0;

                return new JsonpResult(new
                {
                    ValorAdiantamento = valorAdiantamento.ToString("n2")
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar adiantamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion


    }
}
