using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaComplementoFreteController : BaseController
    {
		#region Construtores

		public CargaComplementoFreteController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete = await repCargaComplementoFrete.BuscarPorCodigoAsync(codigo);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFreteAnexo, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFreteAnexo, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFreteAnexo> anexos = await repositorioAnexo.BuscarPorEntidadeAsync(codigo);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                var dynCargaComplementoFrete = new
                {
                    PermiteEscolherDestinacaoDoComplementoDeFrete = cargaComplementoFrete.Carga?.TipoOperacao?.ConfiguracaoCalculoFrete?.PermiteEscolherDestinacaoDoComplementoDeFrete ?? false,
                    cargaComplementoFrete.Codigo,
                    CargaEstaNaLogistica = serCarga.VerificarSeCargaEstaNaLogistica(cargaComplementoFrete.Carga, TipoServicoMultisoftware),
                    ComponenteFrete = new { cargaComplementoFrete.ComponenteFrete.Codigo, cargaComplementoFrete.ComponenteFrete.Descricao },
                    DescricaoSituacao = cargaComplementoFrete.DescricaoSituacao,
                    cargaComplementoFrete.SituacaoComplementoFrete,
                    cargaComplementoFrete.Motivo,
                    DestinoComplemento = cargaComplementoFrete.ComponenteFilialEmissora ? DestinoComplemento.FilialEmissora.ObterDescricao() : DestinoComplemento.Subcontratada.ObterDescricao(),
                    MotivoAdicionalFrete = new { Codigo = cargaComplementoFrete.MotivoAdicionalFrete?.Codigo ?? 0, Descricao = cargaComplementoFrete.MotivoAdicionalFrete?.Descricao },
                    ValorComplemento = cargaComplementoFrete.ValorComplemento.ToString("n2"),
                    ValorComplementoOriginal = cargaComplementoFrete.ValorComplementoOriginal.ToString("n2"),
                    DataAlteracao = cargaComplementoFrete.DataAlteracao.ToString("dd/MM/yyyy HH:mm"),
                    Solicitante = new { cargaComplementoFrete.Usuario.Codigo, Descricao = cargaComplementoFrete.Usuario.Nome },
                    SolicitacaoCredito = cargaComplementoFrete.SolicitacaoCredito != null ? new
                    {
                        cargaComplementoFrete.SolicitacaoCredito.RetornoSolicitacao,
                        ValorLiberado = cargaComplementoFrete.SolicitacaoCredito.ValorLiberado.ToString("n2"),
                        ValorSolicitado = cargaComplementoFrete.SolicitacaoCredito.ValorSolicitado.ToString("n2"),
                        Creditor = cargaComplementoFrete.SolicitacaoCredito.Creditor != null ? cargaComplementoFrete.SolicitacaoCredito.Creditor.Nome : "",
                        Solicitado = cargaComplementoFrete.SolicitacaoCredito.Solicitado.Nome,
                        DataRetorno = cargaComplementoFrete.SolicitacaoCredito.DataRetorno.HasValue ? cargaComplementoFrete.SolicitacaoCredito.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    } : null,
                    Anexos = (
                        from anexo in anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                };

                return new JsonpResult(dynCargaComplementoFrete);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExtornarUtilizacaoAsync(CancellationToken cancellation)
        {

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AdicionarComponentes) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork,cancellation);
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork, cancellation);

                if (operadorLogistica != null && operadorLogistica.PermiteAdicionarComplementosDeFrete)
                {
                    int codigo = int.Parse(Request.Params("codigo"));
                    Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete = await repCargaComplementoFrete.BuscarPorCodigoAsync(codigo);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();

                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                    Servicos.Embarcador.Carga.RateioFrete serCargaFreteRateio = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                    Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new Servicos.Embarcador.Carga.ComplementoFrete(unitOfWork);


                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaComplementoFrete.Carga;

                    if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware) || cargaComplementoFrete.SituacaoComplementoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgAprovacao || cargaComplementoFrete.SituacaoComplementoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgConfirmacaoUso)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponente = await repCargaComponentesFrete.BuscarPorComponenteAsync(cargaComplementoFrete.Codigo);
                        if (cargaComponente != null)
                           await repCargaComponentesFrete.DeletarAsync(cargaComponente);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                        serComplementoFrete.ExtornarComplementoDeFrete(cargaComplementoFrete, TipoServicoMultisoftware, unitOfWork);
                        serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, ConfiguracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);

                        carga.AgConfirmacaoUtilizacaoCredito = false;
                        await repCarga.AtualizarAsync(carga);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Extornou complemento de frete " + cargaComplementoFrete.Descricao + ".", unitOfWork);
                        await unitOfWork.CommitChangesAsync();

                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido);
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        return new JsonpResult(false, true, "Não é possível extornar a utilização do componente na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");
                    }
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, "Você não possui permissão para adicionar complementos de fretes nas cargas.");
                }

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao extornar o uso do componente.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarUtilizacaoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AdicionarComponentes) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork);

                if (operadorLogistica != null && operadorLogistica.PermiteAdicionarComplementosDeFrete)
                {
                    int codigo = int.Parse(Request.Params("codigo"));
                    Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork,cancellationToken);

                    Servicos.Embarcador.Carga.ComplementoFrete serCargaComplementoFrete = new Servicos.Embarcador.Carga.ComplementoFrete(unitOfWork);
                    Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);
                    Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();
                    Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete = await repCargaComplementoFrete.BuscarPorCodigoAsync(codigo);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaComplementoFrete.Carga;

                    string retornoVerificacao = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
                    if (string.IsNullOrWhiteSpace(retornoVerificacao))
                    {

                        if (cargaComplementoFrete.SolicitacaoCredito != null)
                        {
                            List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> utilizados = repCreditoDisponivelUtilizado.BuscarPorCreditoComplementoDeFrete(cargaComplementoFrete.Codigo);
                            serCreditoMovimentacao.ConfirmarUtilizacaoCreditos(utilizados, unitOfWork);

                            cargaComplementoFrete.ValorComplemento = cargaComplementoFrete.SolicitacaoCredito.ValorLiberado + utilizados.Sum(obj => obj.ValorUtilizado);
                            await repCargaComplementoFrete.AtualizarAsync(cargaComplementoFrete);
                        }

                        serCargaComplementoFrete.UtilizarCargaComplementoFrete(cargaComplementoFrete, unitOfWork, TipoServicoMultisoftware);

                        carga.AgConfirmacaoUtilizacaoCredito = false;
                        await repCarga.AtualizarAsync(carga);

                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Confirmou utilização de complemento " + cargaComplementoFrete.Descricao + ".", unitOfWork);
                        await unitOfWork.CommitChangesAsync();

                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido);
                        return new JsonpResult(retorno);
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        return new JsonpResult(false, true, retornoVerificacao);
                    }
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, "Você não possui permissão para adicionar complementos de fretes nas cargas.");
                }

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao extornar o uso do componente.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AdicionarAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AdicionarComponentes) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork);

                if (operadorLogistica != null && operadorLogistica.PermiteAdicionarComplementosDeFrete)
                {
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
                    Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork,cancellationToken);
                    Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);

                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork, cancellationToken);
                    Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
                    Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new Servicos.Embarcador.Carga.ComplementoFrete(unitOfWork);
                    Servicos.Embarcador.Credito.SolicitacaoCredito serSolicitacaoCredito = new Servicos.Embarcador.Credito.SolicitacaoCredito(unitOfWork);
                    Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);
                    Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, ConfiguracaoEmbarcador);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(int.Parse(Request.Params("Carga")));
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();

                    if (servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete() && (carga.SituacaoAlteracaoFreteCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.Aprovada))
                        throw new ControllerException("O valor de frete está aprovado e não pode mais ser alterado.");

                    if (!carga.ExigeNotaFiscalParaCalcularFrete && serCarga.RecebeuNumeroCargaEmbarcador(carga, unitOfWork))
                        throw new ControllerException("A carga já recebeu o número de carga do Embarcador e não permite essa alteração.");

                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = await repComponenteFrete.BuscarPorCodigoAsync(int.Parse(Request.Params("ComponenteFrete")));

                    List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquiaCreditos = await repHierarquiaSolicitacaoCredito.BuscarPorRecebedorAsync(this.Usuario.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete complementoFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete();

                    complementoFrete.Carga = carga;
                    complementoFrete.ComponenteFrete = componenteFrete;
                    complementoFrete.DataAlteracao = System.DateTime.Now;
                    complementoFrete.Usuario = this.Usuario;
                    complementoFrete.ValorComplemento = decimal.Parse(Request.Params("ValorComplemento"));
                    complementoFrete.ValorComplementoOriginal = decimal.Parse(Request.Params("ValorComplemento"));
                    complementoFrete.Motivo = Request.Params("Motivo");
                    complementoFrete.ComponenteFilialEmissora = Request.GetBoolParam("DestinoComplemento");
                    int.TryParse(Request.Params("MotivoAdicionalFrete"), out int codigoMotivoAdicional);

                    if (codigoMotivoAdicional > 0)
                        complementoFrete.MotivoAdicionalFrete = repMotivoAdicionalFrete.BuscarPorCodigo(codigoMotivoAdicional);

                    complementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgConfirmacaoUso;
                    await repCargaComplementoFrete.InserirAsync(complementoFrete);
                    string retornoUtilizacao = "";

                    if (hierarquiaCreditos.Count > 0 && !(configuracaoGeralCarga?.DesabilitarUtilizacaoCreditoOperadores ?? false))
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>(Request.Params("CreditosUtilizados"));
                        decimal somaCreditosUtilizados = creditosUtilizados.Sum(obj => obj.ValorUtilizado);


                        if (somaCreditosUtilizados < complementoFrete.ValorComplemento)
                        {
                            decimal valorSolicitar = creditosUtilizados.Sum(obj => obj.ValorUtilizado);
                            retornoUtilizacao = serCreditoMovimentacao.ComprometerCreditos(creditosUtilizados, complementoFrete, unitOfWork);
                            Dominio.Entidades.Usuario solicitado = await repUsuario.BuscarPorCodigoAsync(int.Parse(Request.Params("CodigoCreditorSolicitar")));
                            Dominio.ObjetosDeValor.Embarcador.Creditos.SolicitacaoCreditoGerada solicitacaoGerada = serSolicitacaoCredito.GerarSolicitacaoCredito(carga, this.Usuario, solicitado, complementoFrete.ComponenteFrete, complementoFrete.ValorComplemento - somaCreditosUtilizados, Request.Params("Motivo"), TipoServicoMultisoftware, unitOfWork);

                            if (solicitacaoGerada.GerouSolicitacao)
                            {
                                complementoFrete.SolicitacaoCredito = solicitacaoGerada.SolicitacaoCredito;
                                complementoFrete.SituacaoComplementoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgAprovacao;
                                await repCargaComplementoFrete.InserirAsync(complementoFrete);
                            }
                            else
                                throw new ControllerException(solicitacaoGerada.MensagemRetorno);
                        }
                        else
                        {
                            if (somaCreditosUtilizados == complementoFrete.ValorComplemento)
                                retornoUtilizacao = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, complementoFrete, unitOfWork);
                            else
                                throw new ControllerException("O valor utilizado (" + somaCreditosUtilizados.ToString("n2") + ") não pode ser maior que o valor do complemento do frete (" + complementoFrete.ValorComplemento.ToString("n2") + ")");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(retornoUtilizacao))
                    {
                        if (servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete())
                        {
                            if (complementoFrete.SolicitacaoCredito != null)
                            {
                                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> utilizados = await repCreditoDisponivelUtilizado.BuscarPorCreditoComplementoDeFreteAsync(complementoFrete.Codigo);

                                serCreditoMovimentacao.ConfirmarUtilizacaoCreditos(utilizados, unitOfWork);

                                complementoFrete.ValorComplemento = complementoFrete.SolicitacaoCredito.ValorLiberado + utilizados.Sum(obj => obj.ValorUtilizado);
                            }

                            serComplementoFrete.UtilizarCargaComplementoFrete(complementoFrete, unitOfWork, TipoServicoMultisoftware);
                            servicoCargaAprovacaoFrete.CriarAprovacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoCarga.InformadoManualmente, TipoServicoMultisoftware);
                        }
                        else if (complementoFrete.SituacaoComplementoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgConfirmacaoUso)
                            serComplementoFrete.UtilizarCargaComplementoFrete(complementoFrete, unitOfWork, TipoServicoMultisoftware);

                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Adicionou um complemento de frete " + complementoFrete.Descricao + ".", unitOfWork);

                        await unitOfWork.CommitChangesAsync();

                        Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = serFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido);

                        return new JsonpResult(retorno);
                    }
                    else
                        throw new ControllerException(retornoUtilizacao);
                }
                else
                    throw new ControllerException("Você não possui permissão para adicionar complementos de fretes nas cargas.");
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}
