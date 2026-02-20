using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Globalization;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize(new string[] { "RetornarEtapaTransportador", "ObterInformacoesGerais", "BuscarNFeCarga" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "Logistica/AgendamentoColeta")]
    public class CargaNotasFiscaisController : BaseController
    {
        #region Construtores

        public CargaNotasFiscaisController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> SolicitarNotasFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && !(carga.TipoOperacao?.ConfiguracaoTransportador?.PermitirTransportadorSolicitarNotasFiscais ?? false)))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                servicoCarga.SolicitarNotasFiscais(carga, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, unitOfWork);

                unitOfWork.CommitChanges();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    Servicos.Embarcador.Notificacao.NotificacaoMTrack serNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);
                    serNotificacaoMTrack.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.NovaCargaMotorista);
                }

                if (carga.TipoOperacao?.EnviarEmailPlanoViagemSolicitarNotasCarga ?? false)
                {
                    Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                    servicoImpressao.EnviarPlanoViagemParaDestinatariosPorEmail(carga, "Aviso de Carregamento");
                }

                if (carga.TipoOperacao?.NotificarRemetentePorEmailAoSolicitarNotas ?? false)
                    servicoCarga.NotificarRemetentesPorEmailAoSolicitarNotasFiscais(carga, unitOfWork);

                return new JsonpResult(new
                {
                    PercursoMDFeValido = true
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == CodigoExcecao.PercursoNaoConfigurado)
                    return new JsonpResult(new
                    {
                        PercursoMDFeValido = false
                    });

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar ao solicitar as notas fiscais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarEmissaoSemNF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarEmissaoSemNF))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = int.Parse(Request.Params("Carga"));
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                if (carga.CargaAgrupada)
                {
                    bool cargasPreCarga = repositorioCarga.BuscarCargasOriginaisSaoPrecarga(carga.Codigo);
                    if (!cargasPreCarga)
                    {
                        carga.CargaDePreCarga = false;
                    }
                }

                if (carga.CargaDePreCarga)
                    return new JsonpResult(false, true, "Não é possível avançar a etapa sem todas NF-e de uma pré carga.");

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && !carga.CargaEmitidaParcialmente)
                    return new JsonpResult(false, true, "Não é possível avançar a etapa sem todas NF-e na atual situação da carga.");

                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                Servicos.Embarcador.Hubs.Carga servicoNotificacaoCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                unitOfWork.Start();

                if (carga.CargaAgrupada && carga.CargaDePreCarga)
                    carga.CargaDePreCarga = false;

                if (carga.CargaEmitidaParcialmente)
                {
                    carga.SituacaoCarga = SituacaoCarga.AgNFe;
                    carga.PossuiPendencia = false;
                }

                servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, Auditado);

                unitOfWork.CommitChanges();

                servicoNotificacaoCarga.InformarCargaAlterada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada);

                if (carga.Filial != null)
                {
                    StringBuilder body = new StringBuilder();

                    body.Append("<p>");
                    body.Append($"O usuário {this.Usuario.Nome} avançou a etapa de NF-e da carga {carga.CodigoCargaEmbarcador} sem que todos os pedidos tivessem NF-e");
                    body.Append("</p>");
                    body.Append("<small>Esse e-mail é gerado automaticamente. Não responda.</small>");

                    string[] bcc = (from e in carga.Filial.Email.Split(';') where !string.IsNullOrEmpty(e) select e.Trim()).ToArray();

#if DEBUG
                    bcc = new string[] { "dev.multisoftware@outlook.com" };
#endif
                    if (bcc.Length > 0)
                    {
                        if (!Servicos.Email.EnviarEmailAutenticado("", $"Avançou etapa da carga {carga.CodigoCargaEmbarcador} sem todas NF-e", body.ToString(), unitOfWork, out string msg, "", null, bcc))
                            Servicos.Log.TratarErro(msg);
                    }
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarEmissaoSemIntegracaoEtapaTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = int.Parse(Request.Params("Carga"));
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);

                var configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "Não é possível retornar a situação desta carga.");

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (configuracao.InformaApoliceSeguroMontagemCarga && carga.Carregamento != null && !repositorioCarregamentoApolice.ExistePorCarregamento(carga.Carregamento.Codigo))
                {
                    Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> listaApoliceSeguroAverbacao = repApoliceSeguro.BuscarPorCarga(carga.Codigo);

                    if (listaApoliceSeguroAverbacao.Count == 0)
                        throw new ServicoException("Obrigatório informar apólice de seguro para seguir.");
                }

                unitOfWork.Start();

                carga.AguardarIntegracaoEtapaTransportador = false;
                if (carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    carga.DataInicioCalculoFrete = DateTime.Now;
                    carga.CalculandoFrete = true;
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                }

                repositorioCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Liberou Emissão Sem integração etapa transportador", unitOfWork);

                unitOfWork.CommitChanges();


                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RetornarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = int.Parse(Request.Params("Carga"));

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                var retorno = servicoCarga.RetornarEtapa(codigoCarga, unitOfWork, TipoServicoMultisoftware, Auditado);

                return new JsonpResult(retorno, true, "");
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

        public async Task<IActionResult> BuscarNFeCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                bool? pedidoComNFe = Request.GetNullableBoolParam("PedidoComNFe");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repoDocDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXmlNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotasFiscais;
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeAnterior;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaCargaPedidoXmlNotaFiscal;
                List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)> situacoesNaoConformidadeNotasFiscal;
                bool retornarSituacoesNaoConformidadeNotasFiscais = (carga.TipoOperacao?.ConfiguracaoCarga?.AtivoModuloNaoConformidades ?? false);

                if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    pedidosXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaCancelada(carga.Codigo);
                else
                    pedidosXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                if (cargaPedidos.Any(o => o.TipoContratacaoCarga != TipoContratacaoCarga.Normal))
                    pedidosCTeAnterior = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork).BuscarPorCarga(carga.Codigo);
                else
                    pedidosCTeAnterior = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

                if (retornarSituacoesNaoConformidadeNotasFiscais)
                    situacoesNaoConformidadeNotasFiscal = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).BuscarSituacoesNotasFiscaisPorCarga(carga.Codigo);
                else
                    situacoesNaoConformidadeNotasFiscal = new List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)>();

                listaCargaPedidoXmlNotaFiscal = repCargaPedidoXmlNotaFiscalParcial.BuscarPorCarga(carga.Codigo);

                int quantidadePedidosComNF = 0;
                List<dynamic> pedidosRetornar = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    bool AgValorFreteRedespacho = cargaPedido.AgValorRedespacho;
                    bool AgEmissaoCTeAnteriorTrecho = cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior;
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotasFiscaisPorCargaPedido = pedidosXMLNotasFiscais.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeAnteriorPorCargaPedido = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> listaCargaPedidoXmlNotaFiscalPorCargaPedido = listaCargaPedidoXmlNotaFiscal.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList();

                    if (cargaPedido.TipoContratacaoCarga != TipoContratacaoCarga.Normal)
                        pedidosCTeAnteriorPorCargaPedido = pedidosCTeAnterior.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList();

                    if (pedidosXMLNotasFiscaisPorCargaPedido.Count > 0)
                        quantidadePedidosComNF++;

                    if (pedidoComNFe.HasValue)//necessário testar aqui e não no where, por causa dos totalizadores
                    {
                        if (pedidoComNFe.Value && pedidosXMLNotasFiscaisPorCargaPedido.Count == 0)
                            continue;

                        if (!pedidoComNFe.Value && pedidosXMLNotasFiscaisPorCargaPedido.Count > 0)
                            continue;
                    }

                    List<dynamic> notasFiscaisRetornar = new List<dynamic>();
                    List<dynamic> ctesRetornar = new List<dynamic>();
                    List<dynamic> notasParciaisRetornar = new List<dynamic>();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNF in pedidosXMLNotasFiscaisPorCargaPedido)
                    {
                        string situacaoNaoConformidade = string.Empty;

                        if (retornarSituacoesNaoConformidadeNotasFiscais)
                        {
                            if (situacoesNaoConformidadeNotasFiscal.Any(o => o.CodigoXmlNotaFiscal == pedidoXMLNF.XMLNotaFiscal.Codigo && o.Situacao == SituacaoNaoConformidade.AguardandoTratativa))
                                situacaoNaoConformidade = SituacaoNaoConformidade.AguardandoTratativa.ObterDescricao();
                            else if (situacoesNaoConformidadeNotasFiscal.Any(o => o.CodigoXmlNotaFiscal == pedidoXMLNF.XMLNotaFiscal.Codigo && o.Situacao == SituacaoNaoConformidade.SemRegraAprovacao))
                                situacaoNaoConformidade = SituacaoNaoConformidade.SemRegraAprovacao.ObterDescricao();
                            else if (situacoesNaoConformidadeNotasFiscal.Any(o => o.CodigoXmlNotaFiscal == pedidoXMLNF.XMLNotaFiscal.Codigo && o.Situacao == SituacaoNaoConformidade.Reprovada))
                                situacaoNaoConformidade = SituacaoNaoConformidade.Reprovada.ObterDescricao();
                            else if (situacoesNaoConformidadeNotasFiscal.Count > 0)
                                situacaoNaoConformidade = "Liberada";
                        }

                        dynamic notaFiscalRetornar = new
                        {
                            CargaPedido = pedidoXMLNF.CargaPedido.Codigo,
                            Codigo = pedidoXMLNF.XMLNotaFiscal.Codigo,
                            Chave = pedidoXMLNF.XMLNotaFiscal.Chave,
                            Numero = pedidoXMLNF.XMLNotaFiscal.Numero,
                            DataEmissao = pedidoXMLNF.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy"),
                            ValorTotal = pedidoXMLNF.XMLNotaFiscal.Valor.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                            Peso = pedidoXMLNF.XMLNotaFiscal.Peso.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                            SituacaoNaoConformidade = situacaoNaoConformidade,
                            PossuiCartaCorrecao = pedidoXMLNF.XMLNotaFiscal.Chave == "" ? false : repoDocDestinadoEmpresa.VerificarNotaCartaCorrecaoEmitente(pedidoXMLNF.XMLNotaFiscal.Chave)
                        };

                        notasFiscaisRetornar.Add(notaFiscalRetornar);
                    }

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeAnterior in pedidosCTeAnteriorPorCargaPedido)
                    {
                        dynamic cteRetornar = new
                        {
                            Chave = pedidoCTeAnterior.CTeTerceiro.ChaveAcesso,
                            Numero = pedidoCTeAnterior.CTeTerceiro.Numero,
                            DataEmissao = pedidoCTeAnterior.CTeTerceiro.DataEmissao.ToString("dd/MM/yyyy"),
                            Emissor = pedidoCTeAnterior.CTeTerceiro.Emitente.Cliente.Descricao,
                            ValorReceber = pedidoCTeAnterior.CTeTerceiro.ValorAReceber.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                            SituacaoNaoConformidade = repNaoConformidade.ExisteNaoConformidadePendente(pedidoCTeAnterior.CargaPedido?.Carga?.Codigo ?? 0) ? "Com Não Conformidade" : "Liberado"
                        };

                        ctesRetornar.Add(cteRetornar);
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXmlNotaFiscalParcial in listaCargaPedidoXmlNotaFiscalPorCargaPedido)
                    {
                        dynamic notaParcialRetornar = new
                        {
                            Chave = cargaPedidoXmlNotaFiscalParcial.Chave ?? string.Empty,
                            Numero = cargaPedidoXmlNotaFiscalParcial.NumeroFatura ?? string.Empty,
                            Status = cargaPedidoXmlNotaFiscalParcial.Status.ObterDescricao()
                        };

                        notasParciaisRetornar.Add(notaParcialRetornar);
                    }

                    dynamic pedidoRetornar = new
                    {
                        AgValorFreteRedespacho,
                        AgEmissaoCTeAnteriorTrecho,
                        Codigo = cargaPedido.Codigo,
                        CargaPedidoFilialEmissora = (cargaPedido.CargaPedidoFilialEmissora && !cargaPedido.EmitirComplementarFilialEmissora),
                        cargaPedido.TipoContratacaoCarga,
                        CodigoPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                        ReentregaSolicitada = cargaPedido.Pedido.ReentregaSolicitada,
                        Cliente = cargaPedido.Pedido.Destinatario?.Nome ?? "",
                        NFs = notasFiscaisRetornar,
                        NotasParciais = notasParciaisRetornar,
                        CTes = ctesRetornar,
                    };

                    pedidosRetornar.Add(pedidoRetornar);
                }

                return new JsonpResult(new
                {
                    Pedidos = pedidosRetornar,
                    QuantidadePedidos = cargaPedidos.Count,
                    QuantidadePedidosComNF = quantidadePedidosComNF,
                    PossuiNaoConformidade = retornarSituacoesNaoConformidadeNotasFiscais,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados das NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesNaoConformidades()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades = repositorioNaoConformidade.BuscarPorNotaFiscal(codigo);

                dynamic naoConformidadesRetornar = (
                    from o in naoConformidades
                    select new
                    {
                        o.Codigo,
                        Descricao = o.ItemNaoConformidade?.Descricao ?? string.Empty,
                        Situacao = o.Situacao.ObterDescricao()
                    }
                ).ToList();

                return new JsonpResult(naoConformidadesRetornar);
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
        public async Task<IActionResult> ObterInformacoesGerais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repCargaIntegracaoAvon = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesDisponiveis = repCargaIntegracao.BuscarTiposPorCarga(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> cargaIntegracoesNatura = null;

                if (integracoesDisponiveis.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                    cargaIntegracaoAvon = repCargaIntegracaoAvon.BuscarPorCarga(codigoCarga);

                if (integracoesDisponiveis.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                    cargaIntegracoesNatura = repCargaIntegracaoNatura.BuscarPorCargaPedidoOuCarga(codigoCarga, codigoCargaPedido);

                var retorno = new
                {
                    Integracoes = integracoesDisponiveis,
                    RequerPermissaoParaAvancar = !serCarga.VerificarIntegracaoOrtec(codigoCarga, TipoServicoMultisoftware, unidadeDeTrabalho),
                    IntegracaoAvon = cargaIntegracaoAvon != null ? new
                    {
                        DataConsulta = cargaIntegracaoAvon.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                        cargaIntegracaoAvon.GUID,
                        cargaIntegracaoAvon.Mensagem,
                        cargaIntegracaoAvon.NumeroMinuta,
                        PesoTotalDocumentos = cargaIntegracaoAvon.PesoTotalDocumentos.ToString("n2"),
                        QuantidadeDocumentos = cargaIntegracaoAvon.QuantidadeDocumentos.ToString("n0"),
                        cargaIntegracaoAvon.Situacao,
                        Usuario = cargaIntegracaoAvon.Usuario.Nome,
                        ValorTotalDocumentos = cargaIntegracaoAvon.ValorTotalDocumentos.ToString("n2"),
                        cargaIntegracaoAvon.Manual
                    } : null,
                    IntegracaoNatura = cargaIntegracoesNatura != null ? cargaIntegracoesNatura.Select(obj => new
                    {
                        obj.DocumentoTransporte.Codigo,
                        obj.DocumentoTransporte.Numero
                    }).ToList() : null,
                    DocOSMae = carga != null ? new { Codigo = carga.CargaDocOS?.Codigo ?? 0, Descricao = carga.CargaDocOS?.Descricao ?? string.Empty } : null,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações da etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosNotasFiscaisPreChekin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Repositorio.Embarcador.Pedidos.StageAgrupamento respositorioAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPrechekinIntegracao = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repostorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentoStages = respositorioAgrupamento.BuscarPorCargaDt(codigoCarga);
                dynamic retornaNotaPrechekin = new
                {
                    StageColeta = new List<dynamic>(),
                    StageTransferencia = new List<dynamic>(),
                    StageEntrega = new List<dynamic>()
                };

                List<dynamic> dadosNotasPrechekin = new List<dynamic>();
                Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao existePrechekinIntegracao = repositorioCargaPrechekinIntegracao.BuscarRegistroIntegrado(codigoCarga);

                if (agrupamentoStages.Count > 0)
                    foreach (var agrupamento in agrupamentoStages)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesPorAgrupamento = repositorioStage.BuscarporAgrupamento(agrupamento.Codigo);
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listadePedidos = repositorioPedidoStage.BuscarPorListaStages(stagesPorAgrupamento.Select(s => s.Codigo).ToList());
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorPedidos(listadePedidos.Select(p => p.Codigo).ToList());

                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                        foreach (var cargaPedido in cargasPedidos)
                            foreach (var pedidoNota in cargaPedido.NotasFiscais)
                                if (!notas.Any(x => x.Chave == pedidoNota.XMLNotaFiscal.Chave))
                                    notas.Add(pedidoNota.XMLNotaFiscal);

                        string origensXDestino = "";

                        if (agrupamento.CargaDT != null)
                            origensXDestino = $"{agrupamento?.CargaDT.DadosSumarizados.Origens} X {agrupamento?.CargaDT?.DadosSumarizados?.Destinos}";
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = listadePedidos.FirstOrDefault();
                            origensXDestino = $"{pedido?.Origem.Descricao} - {pedido?.Origem.Estado.Sigla} X {pedido?.Destino.Descricao} - {pedido?.Destino.Estado.Sigla}";
                        }

                        retornaNotaPrechekin.StageColeta.Add(new
                        {
                            agrupamento.Codigo,
                            NumeroStages = string.Join(", ", stagesPorAgrupamento.Select(s => s.NumeroStage).ToList()),
                            DataPreChekin = existePrechekinIntegracao?.DataIntegracao.ToString("dd/MM/yyyy") ?? string.Empty,
                            Placa = !string.IsNullOrEmpty(agrupamento?.Veiculo?.Placa) ? agrupamento?.Veiculo?.Placa : agrupamento.CargaDT?.Veiculo?.Placa ?? string.Empty,
                            NroFRS = string.Join(", ", stagesPorAgrupamento.Select(s => s.NumeroFolha).Distinct().ToList()),
                            ValorFrete = agrupamento?.ValorFreteTotal ?? 0,
                            Perfil = stagesPorAgrupamento.FirstOrDefault() != null ? stagesPorAgrupamento.FirstOrDefault().ModeloVeicularCarga.CodigoIntegracao : string.Empty,
                            OrigemxDestino = origensXDestino,
                            CanalVenda = stagesPorAgrupamento.FirstOrDefault() != null ? stagesPorAgrupamento.FirstOrDefault()?.CanalVenda.Descricao : string.Empty,
                            ListaNotas = (from nota in notas
                                          select new
                                          {
                                              ChaveNota = nota.Chave,
                                              DataEmissao = nota.DataEmissao.ToString("dd/MM/yyyy"),
                                              Status = "Autorizada",
                                              CodigoNota = nota.Codigo
                                          }).ToList()
                        });
                    }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Stage> existeStages = repositorioStage.BuscarStagesPorCarga(codigoCarga);

                    foreach (var stage in existeStages)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listadePedidos = repositorioPedidoStage.BuscarPorListaStages(new List<int>() { stage.Codigo });
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorPedidos(listadePedidos.Select(p => p.Codigo).ToList());

                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                        foreach (var cargaPedido in cargasPedidos)
                            foreach (var pedidoNota in cargaPedido.NotasFiscais)
                                notas.Add(pedidoNota.XMLNotaFiscal);

                        string origensXDestino = "";

                        if (stage.CargaDT != null)
                            origensXDestino = $"{stage?.CargaDT.DadosSumarizados.Origens} X {stage?.CargaDT?.DadosSumarizados?.Destinos}";
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = listadePedidos.FirstOrDefault();
                            origensXDestino = $"{pedido?.Origem.Descricao} - {pedido?.Origem.Estado.Sigla} X {pedido?.Destino.Descricao} - {pedido?.Destino.Estado.Sigla}";
                        }

                        retornaNotaPrechekin.StageEntrega.Add(new
                        {
                            stage.Codigo,
                            NumeroStages = stage.NumeroStage,
                            DataPreChekin = existePrechekinIntegracao?.DataIntegracao.ToString("dd/MM/yyyy") ?? string.Empty,
                            Placa = stage?.Veiculo?.Placa ?? string.Empty,
                            NroFRS = stage.NumeroFolha,
                            ValorFrete = stage?.CargaDT?.ValorFreteLiquido ?? 0,
                            Perfil = stage?.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                            OrigemxDestino = origensXDestino,
                            CanalVenda = stage?.CanalVenda?.Descricao ?? string.Empty,
                            ListaNotas = (from nota in notas
                                          select new
                                          {
                                              ChaveNota = nota.Chave,
                                              DataEmissao = nota.DataEmissao.ToString("dd/MM/yyyy"),
                                              Status = "Autorizada",
                                              CodigoNota = nota.Codigo
                                          }).ToList()
                        });
                    }
                }


                return new JsonpResult(retornaNotaPrechekin);
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro(exception);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações da etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                byte[] xml = new Servicos.Embarcador.NFe.NFe(unitOfWork).DownloadXml(codigoNFe);

                if (xml == null)
                    return new JsonpResult(false, true, "Não foi encontrado o arquivo XML da nota selecionada.");

                return Arquivo(xml, "text/xml", string.Concat("Nota_" + codigoNFe, ".xml"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDANFE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXMLNotaFiscal.BuscarXMLPorCodigo(codigoNFe);

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DANFE Documentos Emitidos", xmlNotaFiscal.Chave + ".pdf");

                bool contemXml = !string.IsNullOrWhiteSpace(xmlNotaFiscal.XML);
                bool existeArquivo = Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE);

                if (contemXml && xmlNotaFiscal.XML.Contains("</nfeProc>"))
                {
                    if (!existeArquivo && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xmlNotaFiscal.XML, caminhoDANFE, false, false))
                        return new JsonpResult(false, true, erro);
                }
                else if (contemXml && xmlNotaFiscal.XML.Contains("</NFe>"))
                {
                    if (!existeArquivo && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xmlNotaFiscal.XML, caminhoDANFE, false, true))
                        return new JsonpResult(false, true, erro);
                }
                else if (contemXml && xmlNotaFiscal.XML.Contains("nfeProc"))
                {
                    if (!existeArquivo && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xmlNotaFiscal.XML, caminhoDANFE, true, false))
                        return new JsonpResult(false, true, erro);
                }
                else
                {
                    string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                    caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", xmlNotaFiscal.Chave + ".pdf");
                    if (existeArquivo)
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", xmlNotaFiscal.Chave + ".pdf");
                    else
                    {
                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", xmlNotaFiscal.Chave + ".xml");
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");

                        var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                        var retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                        if (retorno == "")
                            return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", xmlNotaFiscal.Chave + ".pdf");
                        else
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                    }
                }
                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", xmlNotaFiscal.Chave + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPacotes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Log Key", "LogKey", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Pedido", "Pedido", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Contratante", "Contratante", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Origem Recebida", "OrigemRecebida", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destino Recebido", "DestinoRecebido", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Contratante Recebido", "ContratanteRecebido", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data Recebimento", "DataRecebimento", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Cubagem", "Cubagem", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Peso", "Peso", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("CT-e", "CTe", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? true : false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.left, false, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? true : false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidopacotes = repCargaPedidoPacote.BuscarCargaPedidoPacotePorCarga(codigoCarga);
                IList<Dominio.ObjetosDeValor.Embarcador.CTe.NumeroCTeAnteriorPacote> cteTerceiros = repositorioCTeTerceiro.BuscarCTeAnteriorIndentificacaoPacote(cargaPedidopacotes.Select(o => o.Pacote.Codigo).ToList());

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    cargaPedidopacotes = cargaPedidopacotes.OrderByDescending(o => o.Pacote.SituacaoIntegracao).ToList();


                grid.AdicionaRows((
                    from o in cargaPedidopacotes
                    select new
                    {
                        o.Codigo,
                        LogKey = !string.IsNullOrWhiteSpace(o.Pacote.LogKey) ? o.Pacote.LogKey : string.Empty,
                        Pedido = o.CargaPedido?.Pedido?.Numero ?? 0,
                        Origem = o.Pacote.Origem?.Descricao ?? string.Empty,
                        Destino = o.Pacote.Destino?.Descricao ?? string.Empty,
                        Contratante = o.Pacote.Contratante?.Descricao ?? string.Empty,
                        OrigemRecebida = o.Pacote.CodigoIntegracaoOrigem ?? string.Empty,
                        DestinoRecebido = o.Pacote.CodigoIntegracaoDestino ?? string.Empty,
                        ContratanteRecebido = o.Pacote.CodigoIntegracaoContratante ?? string.Empty,
                        DataRecebimento = o.Pacote.DataRecebimento.HasValue ? o.Pacote.DataRecebimento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        Cubagem = o.Pacote.Cubagem.ToString("n2"),
                        Peso = o.Pacote.Peso.ToString("n2"),
                        CTe = string.Join(" ,", cteTerceiros.Where(p => p.NumeroPacote == o.Pacote.LogKey).Select(obj => obj.NumeroCTeAnterior).ToList()),
                        Situacao = o.Pacote.SituacaoIntegracao.ObterDescricao(),
                        Mensagem = o.Pacote.MensagemIntegracao
                    }).ToList()
                );

                grid.setarQuantidadeTotal(cargaPedidopacotes.Count);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados das NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPacotesAvulsos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Log Key", "LogKey", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Contratante", "Contratante", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Origem Recebida", "OrigemRecebida", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destino Recebido", "DestinoRecebido", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Contratante Recebido", "ContratanteRecebido", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data Recebimento", "DataRecebimento", 12, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Cubagem", "Cubagem", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Peso", "Peso", 8, Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPacotes filtroPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Cargas.Pacote> pacotes = repPacote.BuscarPacotesAvulsos(filtroPesquisa, parametrosConsulta);

                grid.AdicionaRows((
                    from o in pacotes
                    select new
                    {
                        o.Codigo,
                        LogKey = !string.IsNullOrWhiteSpace(o.LogKey) ? o.LogKey : string.Empty,
                        Origem = o.Origem?.Descricao ?? string.Empty,
                        Destino = o.Destino?.Descricao ?? string.Empty,
                        Contratante = o.Contratante?.Descricao ?? string.Empty,
                        OrigemRecebida = o.CodigoIntegracaoOrigem ?? string.Empty,
                        DestinoRecebido = o.CodigoIntegracaoDestino ?? string.Empty,
                        ContratanteRecebido = o.CodigoIntegracaoContratante ?? string.Empty,
                        DataRecebimento = o.DataRecebimento.HasValue ? o.DataRecebimento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        Cubagem = o.Cubagem.ToString("n2"),
                        Peso = o.Peso.ToString("n2"),
                    }).ToList()
                );

                grid.setarQuantidadeTotal(pacotes.Count);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os pacotes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularPacoteAvulso(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic dynPacotes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Codigos"));
                int codigoCarga = Request.GetIntParam("Carga");

                List<int> codigosPacotes = new List<int>();

                foreach (dynamic pacote in dynPacotes)
                {
                    codigosPacotes.Add((int)pacote.Codigo);
                }

                unitOfWork.Start();

                await VincularPedidos(codigoCarga, codigosPacotes, unitOfWork, cancellationToken);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Pacote(s) vinculado(s) com sucesso");
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular os pacotes");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConsultaPacotes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi servicoIntegracaoLoggi = new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(unitOfWork, Auditado);

                servicoIntegracaoLoggi.GerarRegistroIntegracoesCargaPedidoPacote(codigoCarga);
                servicoIntegracaoLoggi.ProcessarIntegracoesPendentes(codigoCarga);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes repositorioCargaPedidoIntegracaoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 30, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> cargaPedidoIntegracaoPacotes = repositorioCargaPedidoIntegracaoPacote.BuscarPorCargaPedidoIntegracaoPacotesPorCarga(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> arquivosIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                foreach (var cargaPedidoIntegracaoPacote in cargaPedidoIntegracaoPacotes)
                {
                    arquivosIntegracao.AddRange(cargaPedidoIntegracaoPacote.ArquivosTransacao.ToList());
                }

                var retorno = (from obj in arquivosIntegracao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data,
                                   Tipo = obj.Tipo.ObterDescricao(),
                                   Mensagem = obj.Mensagem
                               }).ToList();

                grid.setarQuantidadeTotal(arquivosIntegracao.Count);
                grid.AdicionaRows(retorno);

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

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracao = repositorioArquivo.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(true, false, "Arquivo  não localizado");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "ArquivoHistoricoIntegracao" + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do histórico.");
            }
        }

        #region Importações

        public async Task<IActionResult> ImportarPacotes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Pacote.Pacote servPacote = new Servicos.Embarcador.Carga.Pacote.Pacote(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                var parametros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Parametro"));
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                (string Nome, string Guid) arquivoGerador = ValueTuple.Create(Request.GetStringParam("Nome") ?? string.Empty, Request.GetStringParam("ArquivoSalvoComo") ?? string.Empty);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servPacote.Importar(dados, arquivoGerador, this.Usuario, TipoServicoMultisoftware, Auditado, unitOfWork, codigoCargaPedido);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoPacotes()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Carga.Pacote.Pacote servPacote = new Servicos.Embarcador.Carga.Pacote.Pacote(unitOfWork, TipoServicoMultisoftware);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servPacote.ConfiguracaoImportacao(unitOfWork);

                return new JsonpResult(configuracoes.ToList());
            }
        }

        #endregion

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPacotes ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPacotes
            {
                NumeroPacote = Request.GetStringParam("NumeroPacote"),
            };
        }

        private async Task VincularPedidos(int codigoCarga, List<int> codigosPacotes, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork, cancellationToken);
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPrimeiroPedidoPorCargaAsync(codigoCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidosPacotes = repositorioCargaPedidoPacote.BuscarCargaPedidoPacotePorPacotes(codigosPacotes);

            Servicos.Embarcador.Pacote.Pacote servicoPacote = new Servicos.Embarcador.Pacote.Pacote(unitOfWork, TipoServicoMultisoftware, cancellationToken);

            if (cargaPedidosPacotes.Count == 0)
                throw new ControllerException("Pacotes não foram encontrados");

            if (cargaPedido == null)
                throw new ControllerException("Falha ao vincular à carga");

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote in cargaPedidosPacotes)
            {
                cargaPedidoPacote.CargaPedido = cargaPedido;

                await repositorioCargaPedidoPacote.AtualizarAsync(cargaPedidoPacote);

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro = await repositorioCTeTerceiro.BuscarPorIdentificacaoPacoteAsync(cargaPedidoPacote.Pacote.LogKey, cancellationToken);
                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiro)
                {
                    string retorno = await servicoPacote.VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro, cancellationToken: cancellationToken);

                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ServicoException(retorno);
                }

                await servicoPacote.VerificarQuantidadePacotesCtesAvancaAutomaticoAsync(cargaPedido.Carga, Auditado, cancellationToken);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedidoPacote.Pacote, null, "Vinculado manualmente", unitOfWork);
            }
        }

        #endregion
    }
}
