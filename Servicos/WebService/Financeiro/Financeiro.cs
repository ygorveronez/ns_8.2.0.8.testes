using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Financeiro;
using Dominio.ObjetosDeValor.WebService.NFe;
using Dominio.ObjetosDeValor.WebService.Rest.Financeiro;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.WebService.Financeiro
{
    public class Financeiro : ServicoWebServiceBase
    {
        #region Atributos Globais

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly private protected string _adminStringConexao;

        #endregion

        #region Construtores

        public Financeiro(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { _unitOfWork = unitOfWork; }
        public Financeiro(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarFaturaCompleta(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Servicos.Log.TratarErro($"AdicionarFaturaCompleta: {(faturaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(faturaIntegracao) : string.Empty)}", "Request");
            StringBuilder mensagemErro = new StringBuilder();

            try
            {
                string msgErro = "";

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, null, _tipoServicoMultisoftware, string.Empty);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaLoteCTe repFaturaLoteCTe = new Repositorio.Embarcador.Fatura.FaturaLoteCTe(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(_unitOfWork);


                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                DateTime dataUltimaParcela = DateTime.MinValue;

                _unitOfWork.Start();

                if (faturaIntegracao.Situacao == SituacaoFatura.Cancelado)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorProtocoloIntegracao(faturaIntegracao.Codigo);
                    if (fatura == null)
                    {
                        msgErro = "Fatura não localizada para gerar o seu cancelamento.";
                        _unitOfWork.Rollback();
                        ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                        return Retorno<bool>.CriarRetornoDadosInvalidos(msgErro);
                    }
                    if (fatura.Situacao == SituacaoFatura.Cancelado || fatura.Situacao == SituacaoFatura.EmCancelamento)
                    {
                        msgErro = "Fatura já se encontra cancelada.";
                        _unitOfWork.Rollback();
                        ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                        return Retorno<bool>.CriarRetornoDadosInvalidos(msgErro);
                    }

                    fatura.FaturaRecebidaDeIntegracao = true;
                    fatura.Duplicar = false;
                    fatura.UsuarioCancelamento = repUsuario.BuscarPrimeiro();
                    fatura.NotificadoOperador = false;
                    fatura.SituacaoNoCancelamento = fatura.Situacao;
                    fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento;
                    fatura.DataCancelamentoFatura = faturaIntegracao.DataCancelamentoFatura;
                    fatura.MotivoCancelamento = faturaIntegracao.MotivoCancelamento;

                    servFatura.CancelarTitulosBoletos(fatura.Codigo, _unitOfWork, _auditado, fatura.UsuarioCancelamento.Empresa);

                    servFatura.InserirLog(fatura, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.CancelouFatura, fatura.UsuarioCancelamento);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, fatura, null, "Cancelou a fatura por integração.", _unitOfWork);

                    repFatura.Atualizar(fatura);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura();

                    if (faturaIntegracao.DataInicial.HasValue && faturaIntegracao.DataInicial.Value > DateTime.MinValue)
                        fatura.DataInicial = faturaIntegracao.DataInicial;
                    else
                        fatura.DataInicial = null;

                    if (faturaIntegracao.DataFinal.HasValue && faturaIntegracao.DataFinal.Value > DateTime.MinValue)
                        fatura.DataFinal = faturaIntegracao.DataFinal;
                    else
                        fatura.DataFinal = null;

                    fatura.DataFatura = faturaIntegracao.DataFatura;
                    fatura.NovoModelo = true;
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas;
                    fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento;
                    fatura.Observacao = faturaIntegracao.Observacao;
                    fatura.Usuario = repUsuario.BuscarPrimeiro();
                    fatura.TipoPessoa = faturaIntegracao.TipoPessoa;
                    fatura.GerarDocumentosAutomaticamente = true;
                    fatura.ProtocoloIntegracao = faturaIntegracao.Codigo;

                    fatura.AliquotaICMS = faturaIntegracao.AliquotaICMS;
                    fatura.TipoCarga = faturaIntegracao.TipoCarga != null ? repTipoDeCarga.BuscarPorDescricao(faturaIntegracao.TipoCarga.Descricao) : null;
                    fatura.Cliente = faturaIntegracao.Cliente != null ? repCliente.BuscarPorCPFCNPJ(Utilidades.String.OnlyNumbers(faturaIntegracao.Cliente.CPFCNPJ).ToDouble()) : null;
                    fatura.IETomador = faturaIntegracao.IETomador;
                    fatura.GrupoPessoas = faturaIntegracao.GrupoPessoas != null ? repGrupoPessoas.BuscarPorDescricao(faturaIntegracao.GrupoPessoas.Descricao) : null;
                    fatura.TipoOperacao = faturaIntegracao.TipoOperacao != null ? repTipoOperacao.BuscarPorCodigoIntegracao(faturaIntegracao.TipoOperacao.CodigoIntegracao) : null;
                    fatura.Transportador = null;
                    fatura.MDFe = !string.IsNullOrWhiteSpace(faturaIntegracao.MDFe) ? repMDFe.BuscarPorChave(faturaIntegracao.MDFe) : null;
                    fatura.Container = faturaIntegracao.Container != null ? repContainer.BuscarPorNumero(faturaIntegracao.Container.Numero) : null;
                    fatura.NumeroControleCliente = faturaIntegracao.NumeroControleCliente;
                    fatura.NumeroReferenciaEDI = faturaIntegracao.NumeroReferenciaEDI;
                    fatura.CTe = !string.IsNullOrWhiteSpace(faturaIntegracao.CTe) ? repCTe.BuscarPorChave(faturaIntegracao.CTe) : null;
                    fatura.ClienteTomadorFatura = faturaIntegracao.ClienteTomadorFatura != null ? repCliente.BuscarPorCPFCNPJ(Utilidades.String.OnlyNumbers(faturaIntegracao.ClienteTomadorFatura.CPFCNPJ).ToDouble()) : null;

                    fatura.PedidoViagemNavio = faturaIntegracao.PedidoViagemNavio != null ? repPedidoViagemNavio.BuscarPorDescricao(faturaIntegracao.PedidoViagemNavio.Descricao) : null;
                    fatura.TerminalOrigem = faturaIntegracao.TerminalOrigem != null ? repTerminal.BuscarPorCodigoDocumentacao(faturaIntegracao.TerminalOrigem.CodigoDocumento) : null;
                    fatura.TerminalDestino = faturaIntegracao.TerminalDestino != null ? repTerminal.BuscarPorCodigoDocumentacao(faturaIntegracao.TerminalDestino.CodigoDocumento) : null;
                    fatura.Origem = faturaIntegracao.Origem != null ? repLocalidade.BuscarPorCodigoIBGE(faturaIntegracao.Origem.IBGE) : null;
                    fatura.Destino = faturaIntegracao.Destino != null ? repLocalidade.BuscarPorCodigoIBGE(faturaIntegracao.Destino.IBGE) : null;
                    fatura.NumeroBooking = faturaIntegracao.NumeroBooking;
                    fatura.TipoPropostaMultimodal = faturaIntegracao.TipoPropostaMultimodal;
                    fatura.GeradoPorFaturaLote = false;

                    fatura.ImprimeObservacaoFatura = faturaIntegracao.ImprimeObservacaoFatura;
                    fatura.Total = 0;
                    fatura.Numero = 0;
                    fatura.DataCancelamentoFatura = null;
                    fatura.FaturamentoExclusivo = faturaIntegracao.FaturamentoExclusivo;
                    fatura.ObservacaoFatura = faturaIntegracao.ObservacaoFatura;
                    fatura.FaturaRecebidaDeIntegracao = true;
                    fatura.NumeroFaturaIntegracao = faturaIntegracao.Numero;

                    if (configuracaoTMS.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao(anoAtual);
                        anoAtual = (anoAtual % 100);
                        if (fatura.ControleNumeracao == 1 || (fatura.ControleNumeracao < ((anoAtual * 1000000) + 1)))
                            fatura.ControleNumeracao = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao();

                    if (!ValidarDadosFatura(fatura, faturaIntegracao, out msgErro))
                    {
                        _unitOfWork.Rollback();
                        ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                        return Retorno<bool>.CriarRetornoDadosInvalidos(msgErro);
                    }

                    repFatura.Inserir(fatura, _auditado);

                    if (faturaIntegracao.CTes != null && faturaIntegracao.CTes.Count > 0)
                    {
                        foreach (var cte in faturaIntegracao.CTes)
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe faturaLoteCTe = new Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe()
                            {
                                CTe = repCTe.BuscarPorChave(cte.ChaveCTe),
                                Fatura = fatura
                            };

                            if (faturaLoteCTe.CTe == null)
                            {
                                _unitOfWork.Rollback();
                                ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                                return Retorno<bool>.CriarRetornoDadosInvalidos("CT-e " + cte.ChaveCTe + " não localizado.");
                            }
                            if (!repDocumentoFaturamento.ContemDocumentoPendenteFaturamento(cte.ChaveCTe))
                            {
                                _unitOfWork.Rollback();
                                ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                                return Retorno<bool>.CriarRetornoDadosInvalidos("CT-e " + cte.ChaveCTe + " não se encontra pendente de faturamento.");
                            }

                            repFaturaLoteCTe.Inserir(faturaLoteCTe);

                            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPrimeiroPorChaveCTe(cte.ChaveCTe);

                            if (documentoFaturamento != null)
                            {
                                if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out string erro, ref fatura, documentoFaturamento.Codigo, 0m, _unitOfWork, _auditado))
                                {
                                    _unitOfWork.Rollback();
                                    return Retorno<bool>.CriarRetornoDadosInvalidos(erro);
                                }
                            }
                            else
                            {
                                _unitOfWork.Rollback();
                                ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                                return Retorno<bool>.CriarRetornoDadosInvalidos("CT-e " + cte.ChaveCTe + " não se encontra pendente de faturamento.");
                            }
                        }
                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                        return Retorno<bool>.CriarRetornoDadosInvalidos("CT-es não informados.");
                    }

                    if (faturaIntegracao.Parcelas != null && faturaIntegracao.Parcelas.Count > 0)
                    {
                        foreach (var parcela in faturaIntegracao.Parcelas)
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturaParcela faturaParcela = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela()
                            {
                                Fatura = fatura,
                                Acrescimo = parcela.Acrescimo,
                                CodigoTituloIntegracao = parcela.CodigoTitulo,
                                DataEmissao = parcela.DataEmissao,
                                DataVencimento = parcela.DataVencimento,
                                Desconto = parcela.Desconto,
                                FormaTitulo = parcela.FormaTitulo,
                                Sequencia = parcela.Sequencia,
                                SituacaoFaturaParcela = SituacaoFaturaParcela.EmAberto,
                                TituloGerado = false,
                                Valor = parcela.Valor,
                                ValorTotalMoeda = parcela.ValorTotalMoeda,
                                VencimentoTitulo = parcela.VencimentoTitulo,
                                NossoNumeroIntegrado = parcela.NossoNumeroBoleto
                            };

                            dataUltimaParcela = parcela.DataVencimento;
                            repFaturaParcela.Inserir(faturaParcela);
                        }
                    }

                    Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, _unitOfWork);
                    servFatura.AtualizarValorVencimento(dataUltimaParcela, fatura.Codigo, _unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, fatura, null, "Recebeu a fatura pela integração.", _unitOfWork);

                    if (faturaIntegracao.Integracoes != null && faturaIntegracao.Integracoes.Count > 0)
                    {
                        foreach (var integracao in faturaIntegracao.Integracoes)
                        {
                            if (integracao.TipoIntegracao != TipoIntegracao.Intercab)
                            {
                                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integ = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao()
                                {
                                    DataEnvio = integracao.DataEnvio,
                                    Empresa = null,
                                    Fatura = fatura,
                                    IniciouConexaoExterna = integracao.IniciouConexaoExterna,
                                    LayoutEDI = !string.IsNullOrWhiteSpace(integracao.LayoutEDI) ? repLayoutEDI.BuscarDescricao(integracao.LayoutEDI) : null,
                                    MensagemRetorno = integracao.MensagemRetorno,
                                    ModeloCTe = integracao.ModeloCTe,
                                    NomeArquivo = integracao.NomeArquivo,
                                    ReenviarAutomaticamenteOutraVezAposMinutos = integracao.ReenviarAutomaticamenteOutraVezAposMinutos,
                                    SituacaoIntegracao = integracao.SituacaoIntegracao,
                                    Tentativas = integracao.Tentativas,
                                    TipoImposto = integracao.TipoImposto,
                                    TipoIntegracao = integracao.TipoIntegracao != TipoIntegracao.NaoPossuiIntegracao ? repTipoIntegracao.BuscarPorTipo(integracao.TipoIntegracao) : null,
                                    TipoIntegracaoFatura = integracao.TipoIntegracaoFatura,
                                    UsarCST = integracao.UsarCST
                                };

                                repFaturaIntegracao.Inserir(integ);

                                if (!string.IsNullOrWhiteSpace(integracao.EmailEnvio))
                                {
                                    servFatura.InserirLog(fatura, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.EnviouFatura, fatura.Usuario, integracao.EmailEnvio);
                                }
                            }
                        }
                    }

                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento;
                    fatura.Situacao = SituacaoFatura.EmFechamento;
                    if (fatura.Empresa == null)
                        fatura.Empresa = repFaturaDocumento.ObterPrimeiraEmpresaEmissora(fatura.Codigo);

                    if (fatura.Numero == 0)
                    {
                        if (configuracaoTMS.GerarNumeracaoFaturaAnual)
                        {
                            int anoAtual = DateTime.Now.Year;
                            fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                            anoAtual = (anoAtual % 100);
                            if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                                fatura.Numero = (anoAtual * 1000000) + 1;
                        }
                        else
                            fatura.Numero = repFatura.UltimoNumeracao() + 1;

                        fatura.ControleNumeracao = null;
                    }

                    if (!string.IsNullOrWhiteSpace(faturaIntegracao.PDF))
                    {
                        string caminhoPDF = servFatura.ObterCaminhoPDF(fatura, _unitOfWork, _tipoServicoMultisoftware);

                        if (caminhoPDF != null)
                        {
                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, Convert.FromBase64String(faturaIntegracao.PDF));
                            fatura.CaminhoPDF = caminhoPDF;
                        }
                    }

                    repFatura.Atualizar(fatura);

                    Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repIntegracaoEMP.Buscar();

                    servFatura.GerarIntegracoesFatura(fatura, _unitOfWork, _tipoServicoMultisoftware, _auditado, configuracaoTMS, configuracaoIntegracaoEMP?.AtivarIntegracaoFaturaEMP ?? false);
                    servFatura.SalvarTituloVencimentoDocumentoFaturamento(fatura, _unitOfWork);
                }

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                ArmazenarLogIntegracao(faturaIntegracao, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao ConverterObjetoFaturaIntegracao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Servicos.WebService.Carga.TipoCarga servicoTipoCarga = new Carga.TipoCarga(_unitOfWork);
            Servicos.WebService.Pessoas.Pessoa servicoPessoa = new Pessoas.Pessoa();
            Servicos.WebService.Carga.TipoOperacao servicoTipoOperacao = new Carga.TipoOperacao(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Embarcador.Localidades.Localidade(_unitOfWork);
            Servicos.WebService.Carga.Carga servico = new Carga.Carga(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao objetoFaturaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao()
            {
                Codigo = fatura.Codigo,
                DataCancelamentoFatura = fatura.DataCancelamentoFatura,
                DataInicial = fatura.DataInicial,
                DataFinal = fatura.DataFinal,
                DataFatura = fatura.DataFatura,
                Observacao = fatura.Observacao,
                TipoPessoa = fatura.TipoPessoa,
                AliquotaICMS = fatura.AliquotaICMS,
                TipoCarga = servicoTipoCarga.ConverterObjetoTipoCarga(fatura.TipoCarga),
                Cliente = servicoPessoa.ConverterObjetoPessoa(fatura.Cliente),
                IETomador = fatura.IETomador,
                GrupoPessoas = servicoPessoa.ConverterObjetoGrupoPessoa(fatura.GrupoPessoas),
                TipoOperacao = servicoTipoOperacao.ConverterObjetoTipoOperacao(fatura.TipoOperacao),
                MDFe = fatura.MDFe?.Chave,
                Container = servicoCarga.ConverterObjetoContainer(fatura.Container),
                NumeroControleCliente = fatura.NumeroControleCliente,
                NumeroReferenciaEDI = fatura.NumeroReferenciaEDI,
                CTe = fatura.CTe?.Chave,
                ClienteTomadorFatura = servicoPessoa.ConverterObjetoPessoa(fatura.ClienteTomadorFatura),
                PedidoViagemNavio = servicoCarga.ConverterObjetoViagem(fatura.PedidoViagemNavio),
                TerminalOrigem = servicoCarga.ConverterObjetoTerminalPorto(fatura.TerminalOrigem),
                TerminalDestino = servicoCarga.ConverterObjetoTerminalPorto(fatura.TerminalDestino),
                Origem = servicoLocalidade.ConverterObjetoLocalidade(fatura.Origem),
                Destino = servicoLocalidade.ConverterObjetoLocalidade(fatura.Destino),
                NumeroBooking = fatura.NumeroBooking,
                TipoPropostaMultimodal = fatura.TipoPropostaMultimodal,
                ImprimeObservacaoFatura = fatura.ImprimeObservacaoFatura,
                FaturamentoExclusivo = fatura.FaturamentoExclusivo,
                ObservacaoFatura = fatura.ObservacaoFatura,
                Numero = fatura.Numero,
                Situacao = fatura.Situacao,
                CTes = ConverterObjetoFaturaIntegracaoCTe(fatura.Codigo),
                Parcelas = ConverterObjetoFaturaIntegracaoParcela(fatura.Codigo),
                Integracoes = ConverterObjetoFaturaIntegracaoIntegracao(fatura.Codigo),
                MotivoCancelamento = fatura.MotivoCancelamento,
                PDF = ObterPDFBase64(fatura)
            };

            return objetoFaturaIntegracao;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmacaoMIGO(RequestConfirmacaoMigo dados)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.NotaFiscalMigo repositoNotaFiscalMigo = new Repositorio.Embarcador.Pedidos.NotaFiscalMigo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(dados.ProtocoloIntegracaoCarga, dados.ProtocoloIntegracaoPedido);

            if (cargaPedidos == null || cargaPedidos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Carga pedidos não encontrados");

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotaFiscais = cargaPedido.NotasFiscais.ToList();

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal existePedidoNota = pedidoNotaFiscais.Where(ped => ped.XMLNotaFiscal != null && ped.XMLNotaFiscal.Chave == dados.ChaveNFe).FirstOrDefault();

                if (existePedidoNota == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Não existe nota com a chave {dados.ChaveNFe} vinculada ao carga pedido");

                Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo existeRegistroMigo = repositoNotaFiscalMigo.BuscarPorPedidoXmlNotaFiscal(existePedidoNota.Codigo);

                if (existeRegistroMigo == null)
                    existeRegistroMigo = new Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo();

                existeRegistroMigo.Data = dados.DataEnvio.ToDateTime();
                existeRegistroMigo.ChaveNFe = dados.ChaveNFe;
                existeRegistroMigo.NumeroOcorrencia = dados.NumeroOcorrencia ?? string.Empty;
                existeRegistroMigo.PedidoXMLNotaFiscal = existePedidoNota;
                existeRegistroMigo.CodigoIdentificador = dados.CodigoIdentificador;

                if (existeRegistroMigo.Codigo > 0)
                    repositoNotaFiscalMigo.Atualizar(existeRegistroMigo);
                else
                    repositoNotaFiscalMigo.Inserir(existeRegistroMigo);
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Confirmado com sucesso");
        }
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmacaoMIRO(RequestConfirmacaoMigo dados)
        {
            Repositorio.Embarcador.Pedidos.NotaFiscalMiro repositorioNotaFiscalMiro = new Repositorio.Embarcador.Pedidos.NotaFiscalMiro(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal existePedidoNota = repositorioPedidoXMLNotaFiscal.BuscarPorProtocoloCargaEChave(dados.ProtocoloIntegracaoCarga, dados.ChaveNFe);

            if (existePedidoNota == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Não existe nota com a chave {dados.ChaveNFe} vinculada a carga");

            Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro existeRegistroMiro = repositorioNotaFiscalMiro.BuscarPorPedidoXmlNotaFiscal(existePedidoNota.Codigo);

            if (existeRegistroMiro == null)
                existeRegistroMiro = new Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro();

            existeRegistroMiro.Data = dados.DataEnvio.ToDateTime();
            existeRegistroMiro.ChaveNFe = dados.ChaveNFe;
            existeRegistroMiro.PedidoXMLNotaFiscal = existePedidoNota;
            existeRegistroMiro.CodigoIdentificador = dados.CodigoIdentificador;

            if (existeRegistroMiro.Codigo > 0)
                repositorioNotaFiscalMiro.Atualizar(existeRegistroMiro);
            else
                repositorioNotaFiscalMiro.Inserir(existeRegistroMiro);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Confirmado com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoMIRO(RetornoMiro dados)
        {

            if (!dados.DataMiro.HasValue && string.IsNullOrEmpty(dados.ChaveDocumento) && !dados.Vencimento.HasValue && string.IsNullOrEmpty(dados.CondicaoPagamento) && dados.processamentoSAP == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Formato Requisição incorreto");

            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repCancelamentoPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            Repositorio.Embarcador.Frete.TermosPagamento repositorioTermoPagamento = new Repositorio.Embarcador.Frete.TermosPagamento(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool pagamento = string.IsNullOrEmpty(dados.NumeroEstorno); //Utilizaremos para identificar quando vier,para buscar um cancelamento pagamento
            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;
            Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = null;
            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao cancelamentoPagamentoIntegracao = null;
            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> integracoesPagamento = new List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            if (pagamento)
            {
                pagamentoIntegracao = repositorioPagamentoIntegracao.BuscarPorChaveDocumento(dados.ChaveDocumento);

                if (pagamentoIntegracao == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Não encontrado registro de integração de pagamento pela chave " + dados.ChaveDocumento);

                integracoesPagamento.AddRange(repositorioPagamentoIntegracao.BuscarPorPagamento(pagamentoIntegracao.Pagamento.Codigo));
                documentoFaturamento = pagamentoIntegracao.DocumentoFaturamento;
            }
            else
            {
                cancelamentoPagamentoIntegracao = repCancelamentoPagamentoIntegracao.BuscarPorNumeroMiro(dados.NumeroMiro);
                if (cancelamentoPagamentoIntegracao == null)
                {
                    cancelamentoPagamentoIntegracao = repCancelamentoPagamentoIntegracao.BuscarPorChaveDocumento(dados.ChaveDocumento);
                    if (cancelamentoPagamentoIntegracao == null)
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Não encontrado registro de integração de cancelamento de pagamento numero Miro " + dados.NumeroMiro + " chave " + dados.ChaveDocumento);

                    documentoFaturamento = cancelamentoPagamentoIntegracao.DocumentoFaturamento;

                    if (documentoFaturamento == null)
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Não encontrado documento de faturamento na integração de cancelamento do pagamento numero Miro " + dados.NumeroMiro + " chave " + dados.ChaveDocumento);
                }
                else
                    documentoFaturamento = cancelamentoPagamentoIntegracao.DocumentoFaturamento;
            }

            documentoFaturamento.DataMiro = dados.DataMiro;
            documentoFaturamento.Vencimento = dados.Vencimento;
            documentoFaturamento.NumeroEstorno = dados.NumeroEstorno;
            documentoFaturamento.NumeroMiro = dados.NumeroMiro;
            documentoFaturamento.TermosPagamento = repositorioTermoPagamento.BuscarPorCodigoIntegracao(dados.CondicaoPagamento);
            documentoFaturamento.Bloqueio = !string.IsNullOrEmpty(dados.Bloqueio) ? dados.Bloqueio.StartsWith("R") ? "R" : "P" : "";

            if (pagamento)
            {
                pagamentoIntegracao.ProblemaIntegracao = dados?.processamentoSAP?.FirstOrDefault()?.DescricaoMensagem ?? string.Empty;
                pagamentoIntegracao.SituacaoIntegracao = dados?.processamentoSAP?.FirstOrDefault()?.CodigoMensagem == "200" ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

                List<SituacaoIntegracao> situacaoIntegracao = integracoesPagamento.Select(x => x.SituacaoIntegracao).ToList();
                situacaoIntegracao.Add(pagamentoIntegracao.SituacaoIntegracao);

                pagamentoIntegracao.Pagamento.Situacao = situacaoIntegracao.Any(x => x == SituacaoIntegracao.AgIntegracao) ? SituacaoPagamento.EmIntegracao :
                                                         situacaoIntegracao.Any(x => x == SituacaoIntegracao.ProblemaIntegracao) ? SituacaoPagamento.FalhaIntegracao : SituacaoPagamento.Finalizado;
            }
            else
            {
                if (dados?.processamentoSAP?.FirstOrDefault()?.CodigoMensagem == "200")
                    documentoFaturamento.Situacao = SituacaoDocumentoFaturamento.Cancelado;
                cancelamentoPagamentoIntegracao.ProblemaIntegracao = dados?.processamentoSAP?.FirstOrDefault()?.DescricaoMensagem ?? string.Empty;
                cancelamentoPagamentoIntegracao.SituacaoIntegracao = dados?.processamentoSAP?.FirstOrDefault()?.CodigoMensagem == "200" ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoPagamentoIntegracao.CancelamentoPagamento.Situacao = cancelamentoPagamentoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado ? SituacaoCancelamentoPagamento.Cancelado : SituacaoCancelamentoPagamento.FalhaIntegracao;
            }

            if (string.IsNullOrEmpty(documentoFaturamento.Bloqueio) || documentoFaturamento.Bloqueio.ToUpper().Contains("P"))
            {
                repositorioDocumentoFaturamento.Atualizar(documentoFaturamento);

                if (pagamento)
                {
                    repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
                    servicoArquivo.Adicionar(pagamentoIntegracao, JsonConvert.SerializeObject(dados), JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true)), "json");
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
                }
                else
                {
                    repCancelamentoPagamentoIntegracao.Atualizar(cancelamentoPagamentoIntegracao);
                    servicoArquivo.Adicionar(cancelamentoPagamentoIntegracao, JsonConvert.SerializeObject(dados), JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true)), "json");
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
                }
            }


            new Servicos.Embarcador.Documentos.ControleDocumento(_unitOfWork).ReprocessarRegistroControleDocumento(documentoFaturamento.CTe);

            if (pagamento)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                pagamentoIntegracao.ProblemaIntegracao = "Retorno miro recebido,documento enviado para processar irregularidade e miro aguardando retorno";
                servicoArquivo.Adicionar(pagamentoIntegracao, JsonConvert.SerializeObject(dados), JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true)), "json");

                repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
                repositorioPagamento.Atualizar(pagamentoIntegracao.Pagamento);
            }
            else
            {
                cancelamentoPagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                cancelamentoPagamentoIntegracao.ProblemaIntegracao = "Retorno miro recebido,documento enviado para processar irregularidade e miro aguardando retorno";
                servicoArquivo.Adicionar(cancelamentoPagamentoIntegracao, JsonConvert.SerializeObject(dados), JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true)), "json");

                repCancelamentoPagamentoIntegracao.Atualizar(cancelamentoPagamentoIntegracao);
            }

            repositorioDocumentoFaturamento.Atualizar(documentoFaturamento);
            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);

        }

        public Retorno<bool> IndicarAntecipacaoFreteDocumento(Dominio.ObjetosDeValor.WebService.Financeiro.DocumentoAntecipacao documentoAntecipacao)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documentoAntecipacao.protocoloDocumento);

            if (cte == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos("O protocólo do documento não existe na base da Multisoftware");

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cte.Titulo;
            DateTime dataAntecipacao;
            if (!DateTime.TryParseExact(documentoAntecipacao.dataAntecipacao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAntecipacao))
                return Retorno<bool>.CriarRetornoDadosInvalidos("A data da antecipacação não esta em um formato correto (dd/MM/yyyy)");

            if (titulo == null)
            {
                titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                titulo.DataVencimento = dataAntecipacao;
                titulo.DataProgramacaoPagamento = dataAntecipacao;
                titulo.Empresa = cte.Empresa;
                titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Tomador.CPF_CNPJ));
                titulo.Sequencia = 1;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Antecipado;
                titulo.DataAlteracao = DateTime.Now;
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                titulo.ValorOriginal = cte.ValorAReceber;
            }

            titulo.IntegradoERP = false;

            if (titulo.Codigo == 0)
            {
                titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;
                repTitulo.Inserir(titulo);
                cte.Titulo = titulo;
                repCTe.Atualizar(cte);
            }
            else
                repTitulo.Atualizar(titulo);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, titulo, "Confirmou a antecipacao do documento", _unitOfWork);
            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<bool> ContasPagar(Stream arquivoProcessar, string nomeOriginalArquivo)
        {
            TipoRegistro tipoRegistro = TipoRegistro.SemTipo;

            if (nomeOriginalArquivo.ToLower().Contains("pagoviaconfirming"))
                tipoRegistro = TipoRegistro.PagoviaConfirming;
            else if (nomeOriginalArquivo.ToLower().Contains("baixaresultado"))
                tipoRegistro = TipoRegistro.BaixaResultado;
            else if (nomeOriginalArquivo.ToLower().Contains("pagoviacreditoemconta"))
                tipoRegistro = TipoRegistro.Pagoviacreditoemconta;
            else if (nomeOriginalArquivo.ToLower().Contains("totaldeadiantamento"))
                tipoRegistro = TipoRegistro.TotaldeAdiantamento;
            else if (nomeOriginalArquivo.ToLower().Contains("cockpit"))
                tipoRegistro = TipoRegistro.Cockpit;
            else if (nomeOriginalArquivo.ToLower().Contains("descontos"))
                tipoRegistro = TipoRegistro.Descontos;
            else if (nomeOriginalArquivo.ToLower().Contains("debitoscompensados"))
                tipoRegistro = TipoRegistro.Debitoscompensados;
            else if (nomeOriginalArquivo.ToLower().Contains("pendentesemaberto"))
                tipoRegistro = TipoRegistro.PendentesemAberto;
            else if (nomeOriginalArquivo.ToLower().Contains("nfexadiantamento"))
                tipoRegistro = TipoRegistro.NotasCompensadasXAdiantamento;

            return Retorno<bool>.CriarRetornoSucesso(SalvarArquivoParaProcessamento(arquivoProcessar, nomeOriginalArquivo, tipoRegistro));
        }

        public bool SalvarArquivoParaProcessamento(Stream arquivoProcessar, string nomeOriginalArquivo, TipoRegistro tipoRegistro)
        {
            Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.ContaPagar novoRegistroContaPagar = new Dominio.Entidades.Embarcador.Financeiro.ContaPagar();
            novoRegistroContaPagar.DataIntegracao = DateTime.Now;
            novoRegistroContaPagar.Situacao = SituacaoProcessamentoArquivo.AguardandoProcessamento;
            novoRegistroContaPagar.NomeOriginalArquivo = nomeOriginalArquivo;
            novoRegistroContaPagar.ArquivoAProcessar = ArquivoIntegracao.SalvarArquivoIntegracao(Utilidades.File.ReadToEnd(arquivoProcessar), ".csv", _unitOfWork);
            novoRegistroContaPagar.TipoRegistro = tipoRegistro;
            repositorioContaPagar.Inserir(novoRegistroContaPagar);
            return true;
        }

        public Retorno<bool> RetornoDesbloqueioR(DesbloqueioR dados)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioEscrituracaoMiro = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao existeIntegracao = repositorioEscrituracaoMiro.BuscarIntegracoesPorNumeroMiro(dados.CodigoIdentificadorMIRO);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            if (existeIntegracao == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Não existe registro de lote de escrituração para a miro ({dados.CodigoIdentificadorMIRO})");

            existeIntegracao.ProblemaIntegracao = string.IsNullOrEmpty(dados.Mensagem) ? "Sucesso ao integrar" : dados.Mensagem;
            existeIntegracao.SituacaoIntegracao = string.IsNullOrEmpty(dados.Mensagem) ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

            repositorioEscrituracaoMiro.Atualizar(existeIntegracao);

            servicoArquivo.Adicionar(existeIntegracao, JsonConvert.SerializeObject(dados), JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true)), "json");
            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<bool> RetornoCancelamentoProvisao(RetornoCancelamentoProvisaoRequest requestDados)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao existeIntegracaoCancelamentoProvisa = repositorioCancelamentoProvisaoIntegracao.BuscarPorNumeroFolha(requestDados.NumeroFolha);

            if (existeIntegracaoCancelamentoProvisa == null)
                return Retorno<bool>.CriarRetornoExcecao("Registro não encontrado com o numero da folha informado");

            existeIntegracaoCancelamentoProvisa.DocumentoProvisao.DataFolha = requestDados.DataFolha.ToNullableDateTime();
            existeIntegracaoCancelamentoProvisa.DocumentoProvisao.Cancelado = requestDados.Cancelado;
            existeIntegracaoCancelamentoProvisa.ProblemaIntegracao = requestDados.MensagemRetornoEtapa;
            existeIntegracaoCancelamentoProvisa.SituacaoIntegracao = SituacaoIntegracao.Integrado;

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);
            servicoArquivo.Adicionar(existeIntegracaoCancelamentoProvisa, JsonConvert.SerializeObject(requestDados), JsonConvert.SerializeObject(retorno), "json");

            repositorioProvisao.Atualizar(existeIntegracaoCancelamentoProvisa.CancelamentoProvisao);
            repositorioCancelamentoProvisaoIntegracao.Atualizar(existeIntegracaoCancelamentoProvisa);

            return retorno;
        }

        public Retorno<bool> ConfirmarProvisao(AtualizarSituacaoProvisao retornoProvisao)
        {
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao = repositorioProvisaoIntegracao.BuscarPorCodigo(retornoProvisao.ProtocoloIntegracao);

            if (provisaoIntegracao == null)
                return Retorno<bool>.CriarRetornoExcecao("Protocolo de Integração não encontrado");

            if (provisaoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                return Retorno<bool>.CriarRetornoExcecao("A situação atual da provisão não permite integração");

            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = provisaoIntegracao.Provisao;

            if (retornoProvisao.ProcessadoSucesso)
            {
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                provisao.Situacao = SituacaoProvisao.Finalizado;
            }
            else
            {
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                provisao.Situacao = SituacaoProvisao.FalhaIntegracao;
            }

            provisaoIntegracao.DataIntegracao = DateTime.Now;
            provisaoIntegracao.ProblemaIntegracao = retornoProvisao.MensagemRetorno;

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);
            servicoArquivo.Adicionar(provisaoIntegracao, JsonConvert.SerializeObject(retornoProvisao), JsonConvert.SerializeObject(retorno), "json");

            repositorioProvisao.Atualizar(provisao);
            repositorioProvisaoIntegracao.Atualizar(provisaoIntegracao);

            return retorno;
        }

        public Retorno<bool> ConfirmarCancelamentoPagamento(RetornoCancelamentoPagamentoRequest retornoCancelamentoPagamento)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repositorioCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repositorioCancelamentoPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao cancelamentoPagamentoIntegracao = repositorioCancelamentoPagamentoIntegracao.BuscarPorCodigo(retornoCancelamentoPagamento.ProtocoloIntegracao);

            if (cancelamentoPagamentoIntegracao == null)
                return Retorno<bool>.CriarRetornoExcecao("Protocolo de Integração não encontrado");

            if (cancelamentoPagamentoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                return Retorno<bool>.CriarRetornoExcecao("A situação atual do lote de cancelamento de pagamento não permite integração");

            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamentoPagamento = cancelamentoPagamentoIntegracao.CancelamentoPagamento;

            if (retornoCancelamentoPagamento.ProcessadoSucesso)
            {
                cancelamentoPagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cancelamentoPagamento.Situacao = SituacaoCancelamentoPagamento.Cancelado;
            }
            else
            {
                cancelamentoPagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoPagamento.Situacao = SituacaoCancelamentoPagamento.FalhaIntegracao;
            }

            cancelamentoPagamentoIntegracao.DataIntegracao = DateTime.Now;
            cancelamentoPagamentoIntegracao.ProblemaIntegracao = retornoCancelamentoPagamento.MensagemRetorno;

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);
            servicoArquivo.Adicionar(cancelamentoPagamentoIntegracao, JsonConvert.SerializeObject(retornoCancelamentoPagamento), JsonConvert.SerializeObject(retorno), "json");

            repositorioCancelamentoPagamento.Atualizar(cancelamentoPagamento);
            repositorioCancelamentoPagamentoIntegracao.Atualizar(cancelamentoPagamentoIntegracao);

            return retorno;
        }

        public Retorno<bool> ConfirmarCancelamentoPagamentoCarga(RetornoCancelamentoPagamentoCargaRequest retornoCancelamentoPagamentoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repositorioCagaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cancelamentoCargaCTeIntegracao = repositorioCagaCancelamentoCargaCTeIntegracao.BuscarPorCodigo(retornoCancelamentoPagamentoCarga.ProtocoloIntegracao, true);

            if (cancelamentoCargaCTeIntegracao == null)
                return Retorno<bool>.CriarRetornoExcecao("Protocolo de Integração não encontrado");

            if (cancelamentoCargaCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                return Retorno<bool>.CriarRetornoExcecao("A situação atual do lote de cancelamento de pagamento da carga não permite integração");

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = cancelamentoCargaCTeIntegracao.CargaCancelamento;

            if (retornoCancelamentoPagamentoCarga.ProcessadoSucesso)
            {
                cancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.Cancelada;
            }
            else
            {
                cancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
            }

            cancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cancelamentoCargaCTeIntegracao.ProblemaIntegracao = retornoCancelamentoPagamentoCarga.MensagemRetorno;

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);
            servicoArquivo.Adicionar(cancelamentoCargaCTeIntegracao, JsonConvert.SerializeObject(retornoCancelamentoPagamentoCarga), JsonConvert.SerializeObject(retorno), "json");

            repositorioCargaCancelamento.Atualizar(cargaCancelamento);
            repositorioCagaCancelamentoCargaCTeIntegracao.Atualizar(cancelamentoCargaCTeIntegracao);

            return retorno;
        }

        public Retorno<bool> ConfirmarLoteEscrituracao(RetornoLoteEscrituracaoResult retornoLoteEscrituracao)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracao repositorioLoteEscrituracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repositorioLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao loteEscrituracaoIntegracao = repositorioLoteEscrituracaoIntegracao.BuscarPorCodigo(retornoLoteEscrituracao.ProtocoloIntegracao);

            if (loteEscrituracaoIntegracao == null)
                return Retorno<bool>.CriarRetornoExcecao("Protocolo de Integração não encontrado");

            if (loteEscrituracaoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                return Retorno<bool>.CriarRetornoExcecao("A situação atual do lote de escrituração não permite integração");

            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = loteEscrituracaoIntegracao.LoteEscrituracao;

            if (retornoLoteEscrituracao.ProcessadoSucesso)
            {
                loteEscrituracaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                loteEscrituracao.Situacao = SituacaoLoteEscrituracao.Finalizado;
            }
            else
            {
                loteEscrituracaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                loteEscrituracao.Situacao = SituacaoLoteEscrituracao.FalhaIntegracao;
            }

            loteEscrituracaoIntegracao.DataIntegracao = DateTime.Now;
            loteEscrituracaoIntegracao.ProblemaIntegracao = retornoLoteEscrituracao.MensagemRetorno;

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);
            servicoArquivo.Adicionar(loteEscrituracaoIntegracao, JsonConvert.SerializeObject(retornoLoteEscrituracao), JsonConvert.SerializeObject(retorno), "json");

            repositorioLoteEscrituracao.Atualizar(loteEscrituracao);
            repositorioLoteEscrituracaoIntegracao.Atualizar(loteEscrituracaoIntegracao);

            return retorno;
        }

        public Retorno<bool> ReceberDocumentoEntradaTMS(Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntradaTMS)
        {
            if (documentoEntradaTMS != null)
            {
                if (documentoEntradaTMS.DataEntrada.Date > DateTime.Now.Date)
                    return Retorno<bool>.CriarRetornoExcecao("A data de entrada não pode ser maior que a data atual (" + DateTime.Now.ToString("dd/MM/yyyy") + ").");

                if (documentoEntradaTMS.DataEmissao.Date > DateTime.Now.Date)
                    return Retorno<bool>.CriarRetornoExcecao("A data de emissão não pode ser maior que a data atual (" + DateTime.Now.ToString("dd/MM/yyyy") + ").");

                if (documentoEntradaTMS.DataEmissao.Date > documentoEntradaTMS.DataEntrada.Date)
                    return Retorno<bool>.CriarRetornoExcecao("A data de emissão não pode ser maior que a data de entrada.");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(_unitOfWork);
                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(_unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento = !string.IsNullOrWhiteSpace(documentoEntradaTMS.Chave) ? repDocumentoEntrada.BuscarPorChave(documentoEntradaTMS.Chave) : null;

                if (documento == null)
                    documento = repDocumentoEntrada.BuscarPorFornecedorNumeroESerie(documentoEntradaTMS.Numero, documentoEntradaTMS.Serie, documentoEntradaTMS.Fornecedor.CPFCNPJ, 0);

                if (documento != null)
                    return Retorno<bool>.CriarRetornoExcecao("O documento informado já se encontra cadastrado no sistema. Número do lançamento: " + documento.NumeroLancamento + ".");

                documento = this.PreencherDocumentoEntrada(documentoEntradaTMS);
                documento = this.ObterVeiculos(documento, documentoEntradaTMS);

                ValidarRegrasDocumentoEntrada(documento, _unitOfWork);

                repDocumentoEntrada.Inserir(documento, _auditado);

                if (!SalvarCentrosResultado(out string msgCentrosResultados, documento, documentoEntradaTMS))
                    return Retorno<bool>.CriarRetornoExcecao(msgCentrosResultados);

                if (!SalvarDuplicatas(out string msgDuplicatas, documento, documentoEntradaTMS))
                    return Retorno<bool>.CriarRetornoExcecao(msgDuplicatas);

                if (!SalvarCentrosResultadoTiposDespesa(out string msgCentroResultadoTipoDespesas, documento, documentoEntradaTMS))
                    return Retorno<bool>.CriarRetornoExcecao(msgCentroResultadoTipoDespesas);

                if (!SalvarItensDocumentoEntradaTMS(out string msgItens, documento, documentoEntradaTMS))
                    return Retorno<bool>.CriarRetornoExcecao(msgItens);

                bool realizarRateioDespesaVeiculo = repDocumentoEntradaItem.RealizarRateioDespesaVeiculo(documento.Codigo);
                decimal valorRateioDespesaVeiculo = repDocumentoEntradaItem.ValorRateioDespesaVeiculo(documento.Codigo);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, documento, "Recebeu documento de entrada", _unitOfWork);
                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            else
            {
                return Retorno<bool>.CriarRetornoExcecao("Documento de entrada não existente");
            }
        }

        public Retorno<bool> ConfirmarEstornoProvisao(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.EstornoProvisaoRequest request)
        {
            _unitOfWork.Start();

            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao cancelamentoProvisaoIntegracao = repositorioCancelamentoProvisaoIntegracao.BuscarPorCodigo(request.ProtocoloIntegracao);
            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);

            try
            {
                servicoProvisao.ConfirmarEstornoProvisao(request, cancelamentoProvisaoIntegracao);
            }
            catch (ServicoException e)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(e.Message);

                return Retorno<bool>.CriarRetornoExcecao(e.Message);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(e.Message);

                return Retorno<bool>.CriarRetornoExcecao("Erro ao confirmar estorno da provisão");
            }

            if (cancelamentoProvisaoIntegracao != null)
            {
                servicoArquivo.Adicionar(cancelamentoProvisaoIntegracao, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(retorno), "json");
                repositorioCancelamentoProvisaoIntegracao.Atualizar(cancelamentoProvisaoIntegracao);
            }

            _unitOfWork.CommitChanges();

            return retorno;
        }

        public Retorno<bool> LiberarPagamento(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.LiberacaoPagamentoRequest request)
        {
            _unitOfWork.Start();

            Servicos.Embarcador.Escrituracao.Pagamento servicoPagamento = new Servicos.Embarcador.Escrituracao.Pagamento(_unitOfWork, _cancellationToken);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = repositorioPagamentoIntegracao.BuscarPorCodigo(request.ProtocoloIntegracao);
            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);

            try
            {
                servicoPagamento.LiberarPagamentoEstornandoProvisao(request, pagamentoIntegracao, _unitOfWork);
            }
            catch (ServicoException e)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(e.Message);

                return Retorno<bool>.CriarRetornoExcecao(e.Message);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(e.Message);

                return Retorno<bool>.CriarRetornoExcecao("Erro ao confirmar estorno da provisão");
            }

            if (pagamentoIntegracao != null)
            {
                servicoArquivo.Adicionar(pagamentoIntegracao, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(retorno), "json");
                repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
            }

            _unitOfWork.CommitChanges();

            return retorno;
        }

        public Retorno<bool> SalvarPDF(int codigo, string boleto, string numero)
        {
            Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);

            try
            {
                _unitOfWork.Start();

                var fatura = repositorioFatura.BuscarPorProtocoloIntegracao(codigo);
                if (fatura == null)
                    return Retorno<bool>.CriarRetornoExcecao("Fatura não encontrada");

                if (string.IsNullOrEmpty(boleto))
                    return Retorno<bool>.CriarRetornoExcecao("PDF do boleto não informado");

                if (string.IsNullOrEmpty(numero))
                    return Retorno<bool>.CriarRetornoExcecao("Número do boleto não informado");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repositorioTitulo.BuscarPorFatura(fatura.Codigo)?.FirstOrDefault();

                if (titulo == null)
                    return Retorno<bool>.CriarRetornoExcecao("Título financeiro não encontrado");

                var data = Convert.FromBase64String(boleto);

                string caminhoPDFBoleto = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Boleto" });
                string guid = Guid.NewGuid().ToString();
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoPDFBoleto, $"{guid}.pdf");

                Utilidades.IO.FileStorageService.Storage.CreateIfNotExists(caminhoPDFBoleto);
                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, data);

                titulo.NossoNumero = numero;
                titulo.CaminhoBoletoIntegracao = caminhoPDF;
                titulo.CaminhoBoleto = caminhoPDF;
                titulo.BoletoStatusTitulo = BoletoStatusTitulo.Gerado;

                repositorioTitulo.Atualizar(titulo);
                _unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao salvar o PDF do boleto: {ex.Message}");
                retorno = Retorno<bool>.CriarRetornoExcecao($"Erro ao salvar o PDF do boleto: {ex.Message}");
                _unitOfWork.Rollback();
            }

            return retorno;
        }

        public Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>> BuscarDadosPagamento(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.RequisicaoBuscarDadosPagamento request)
        {
            Servicos.Embarcador.Escrituracao.Pagamento servicoPagamento = new Servicos.Embarcador.Escrituracao.Pagamento(_unitOfWork, _cancellationToken);
            IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento> cargasPagamentos = new List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>();
            Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>> retorno = new Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>>();

            try
            {
                cargasPagamentos = servicoPagamento.BuscarDadosPagamento(request, _unitOfWork);
                retorno = Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>>.CriarRetornoSucesso(cargasPagamentos);
            }
            catch (ServicoException e)
            {
                Servicos.Log.TratarErro(e.Message);

                retorno = Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>>.CriarRetornoExcecao(e.Message);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e.Message);

                retorno = Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>>.CriarRetornoExcecao(e.Message);
            }
            return retorno;
        }
        public Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao>> BuscarDadosProvisao(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.RequisicaoBuscarDadosProvisao request)
        {
            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao>> retorno = null;

            try
            {
                List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao> cargasProvisao = servicoProvisao.BuscarDadosProvisao(request);
                retorno = Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao>>.CriarRetornoSucesso(cargasProvisao);
            }

            catch (ServicoException e)
            {
                Servicos.Log.TratarErro(e.Message);
                retorno = Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao>>.CriarRetornoExcecao(e.Message);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e.Message);
                retorno = Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao>>.CriarRetornoExcecao("Erro ao obter os dados da provisão.");
            }

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoCTe> ConverterObjetoFaturaIntegracaoCTe(int codigoFatura)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
            List<string> chavesCTe = repFaturaDocumento.BuscarChavesCTesPorFatura(codigoFatura);

            List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoCTe> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoCTe>();
            foreach (string chaveCTe in chavesCTe)
            {
                listaRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoCTe()
                {
                    ChaveCTe = chaveCTe
                });
            }
            return listaRetorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela> ConverterObjetoFaturaIntegracaoParcela(int codigoFatura)
        {
            Repositorio.Embarcador.Fatura.FaturaParcela repositorioFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> faturaParcelas = repositorioFaturaParcela.BuscarPorFatura(codigoFatura);

            List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela>();
            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaParcela faturaParcela in faturaParcelas)
            {
                listaRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela()
                {
                    Acrescimo = faturaParcela.Acrescimo,
                    CodigoTitulo = faturaParcela.CodigoTitulo,
                    DataEmissao = faturaParcela.DataEmissao,
                    DataVencimento = faturaParcela.DataVencimento,
                    Desconto = faturaParcela.Desconto,
                    FormaTitulo = faturaParcela.FormaTitulo,
                    Sequencia = faturaParcela.Sequencia,
                    Valor = faturaParcela.Valor,
                    ValorTotalMoeda = faturaParcela.ValorTotalMoeda,
                    VencimentoTitulo = faturaParcela.DataVencimentoTitulo > DateTime.MinValue ? faturaParcela.DataVencimentoTitulo : faturaParcela.DataVencimento,
                    NossoNumeroBoleto = faturaParcela.NossoNumeroTitulo
                });
            }
            return listaRetorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoIntegracao> ConverterObjetoFaturaIntegracaoIntegracao(int codigoFatura)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> faturaIntegracoes = repFaturaIntegracao.BuscarPorFatura(codigoFatura);

            List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoIntegracao> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoIntegracao>();
            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao in faturaIntegracoes)
            {
                listaRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoIntegracao()
                {
                    CodigoFatura = codigoFatura,
                    CodigoIntegracaoIntegradora = integracao.CodigoIntegracaoIntegradora,
                    DataEnvio = integracao.DataEnvio,
                    EmailEnvio = "",
                    IniciouConexaoExterna = integracao.IniciouConexaoExterna,
                    LayoutEDI = integracao.LayoutEDI?.Descricao ?? "",
                    MensagemRetorno = integracao.MensagemRetorno,
                    ModeloCTe = integracao.ModeloCTe,
                    NomeArquivo = integracao.NomeArquivo,
                    ReenviarAutomaticamenteOutraVezAposMinutos = integracao.ReenviarAutomaticamenteOutraVezAposMinutos,
                    SituacaoIntegracao = SituacaoIntegracao.Integrado,
                    Tentativas = integracao.Tentativas,
                    TipoImposto = integracao.TipoImposto,
                    TipoIntegracao = integracao.TipoIntegracao?.Tipo ?? TipoIntegracao.NaoPossuiIntegracao,
                    TipoIntegracaoFatura = integracao.TipoIntegracaoFatura,
                    UsarCST = integracao.UsarCST
                });
            }
            return listaRetorno;
        }

        private bool ValidarDadosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, out string msgRetorno)
        {
            msgRetorno = "";

            if (!string.IsNullOrWhiteSpace(faturaIntegracao.MDFe) && fatura.MDFe == null)
            {
                msgRetorno = "MDF-e " + faturaIntegracao.MDFe + " não localizado.";
                return false;
            }
            if (faturaIntegracao.TipoCarga != null && fatura.TipoCarga == null)
            {
                msgRetorno = "Tipo de carga " + faturaIntegracao.TipoCarga.Descricao + " não localizado.";
                return false;
            }
            if (faturaIntegracao.Cliente != null && fatura.Cliente == null)
            {
                msgRetorno = "Cliente " + faturaIntegracao.Cliente.RazaoSocial + " não localizado.";
                return false;
            }
            if (faturaIntegracao.GrupoPessoas != null && fatura.GrupoPessoas == null)
            {
                msgRetorno = "Grupo de Pessoa " + faturaIntegracao.GrupoPessoas.Descricao + " não localizado.";
                return false;
            }
            if (faturaIntegracao.TipoOperacao != null && fatura.TipoOperacao == null)
            {
                msgRetorno = "Tipo de Operação " + faturaIntegracao.TipoOperacao.Descricao + " não localizado.";
                return false;
            }
            if (faturaIntegracao.Container != null && fatura.Container == null)
            {
                msgRetorno = "Container " + faturaIntegracao.Container.Numero + " não localizado.";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(faturaIntegracao.CTe) && fatura.CTe == null)
            {
                msgRetorno = "CT-e " + faturaIntegracao.CTe + " não localizado.";
                return false;
            }
            if (faturaIntegracao.PedidoViagemNavio != null && fatura.PedidoViagemNavio == null)
            {
                msgRetorno = "Viagem " + faturaIntegracao.PedidoViagemNavio.Descricao + " não localizado.";
                return false;
            }
            if (faturaIntegracao.TerminalOrigem != null && fatura.TerminalOrigem == null)
            {
                msgRetorno = "Terminal de Origem " + faturaIntegracao.TerminalOrigem.Descricao + " não localizado.";
                return false;
            }
            if (faturaIntegracao.TerminalDestino != null && fatura.TerminalDestino == null)
            {
                msgRetorno = "Terminal de Destino " + faturaIntegracao.TerminalDestino.Descricao + " não localizado.";
                return false;
            }
            if (faturaIntegracao.ClienteTomadorFatura != null && fatura.ClienteTomadorFatura == null)
            {
                msgRetorno = "Tomador " + faturaIntegracao.ClienteTomadorFatura.RazaoSocial + " não localizado.";
                return false;
            }

            return true;
        }

        #region Documento Entrada TMS
        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS PreencherDocumentoEntrada(Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS objValorDocumentoEntrada)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS();

            documentoEntrada.EncerrarOrdemServico = objValorDocumentoEntrada.EncerrarOrdemServico;
            documentoEntrada.Chave = objValorDocumentoEntrada.Chave;
            documentoEntrada.DataEmissao = objValorDocumentoEntrada.DataEmissao;
            documentoEntrada.DataEntrada = objValorDocumentoEntrada.DataEntrada;
            documentoEntrada.Fornecedor = repCliente.BuscarPorCPFCNPJ(objValorDocumentoEntrada.Fornecedor.CPFCNPJ);
            documentoEntrada.SituacaoLancamentoDocumentoEntrada = repSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(objValorDocumentoEntrada.SituacaoLancamentoDocumentoEntrada.Codigo);
            documentoEntrada.Numero = objValorDocumentoEntrada.Numero;
            documentoEntrada.NumeroLancamento = repDocumentoEntrada.BuscarUltimoNumeroLancamento() + 1;
            documentoEntrada.Serie = objValorDocumentoEntrada.Serie;
            documentoEntrada.Situacao = SituacaoDocumentoEntrada.Aberto;
            documentoEntrada.DataAbastecimento = objValorDocumentoEntrada.DataAbastecimento != null && objValorDocumentoEntrada.DataAbastecimento > DateTime.MinValue ? objValorDocumentoEntrada.DataAbastecimento : null;
            documentoEntrada.OperadorLancamentoDocumento = documentoEntrada.OperadorLancamentoDocumento;
            documentoEntrada.MoedaCotacaoBancoCentral = objValorDocumentoEntrada.MoedaCotacaoBancoCentral;
            documentoEntrada.DataBaseCRT = objValorDocumentoEntrada.DataBaseCRT;
            documentoEntrada.ValorMoedaCotacao = objValorDocumentoEntrada.ValorMoedaCotacao;
            documentoEntrada.BaseCalculoICMS = objValorDocumentoEntrada.BaseCalculoICMS;
            documentoEntrada.BaseCalculoICMSST = objValorDocumentoEntrada.BaseCalculoICMSST;
            documentoEntrada.Especie = repEspecie.BuscarPorSigla(objValorDocumentoEntrada.Especie.Sigla);
            documentoEntrada.IndicadorPagamento = objValorDocumentoEntrada.IndicadorPagamento;
            documentoEntrada.Modelo = repModelo.BuscarPorModelo(objValorDocumentoEntrada.Modelo.Numero);
            documentoEntrada.ValorTotal = objValorDocumentoEntrada.ValorTotal;
            documentoEntrada.ValorTotalCOFINS = objValorDocumentoEntrada.ValorTotalCOFINS;
            documentoEntrada.ValorTotalDesconto = objValorDocumentoEntrada.ValorTotalDesconto;
            documentoEntrada.ValorTotalFrete = objValorDocumentoEntrada.ValorTotalFrete;
            documentoEntrada.ValorTotalICMS = objValorDocumentoEntrada.ValorTotalICMS;
            documentoEntrada.ValorTotalICMSST = objValorDocumentoEntrada.ValorTotalICMSST;
            documentoEntrada.ValorTotalIPI = objValorDocumentoEntrada.ValorTotalIPI;
            documentoEntrada.ValorTotalOutrasDespesas = objValorDocumentoEntrada.ValorTotalOutrasDespesas;
            documentoEntrada.ValorTotalPIS = objValorDocumentoEntrada.ValorTotalPIS;
            documentoEntrada.ValorTotalCreditoPresumido = objValorDocumentoEntrada.ValorTotalCreditoPresumido;
            documentoEntrada.ValorTotalDiferencial = objValorDocumentoEntrada.ValorTotalDiferencial;
            documentoEntrada.ValorTotalSeguro = objValorDocumentoEntrada.ValorTotalSeguro;
            documentoEntrada.ValorTotalFreteFora = objValorDocumentoEntrada.ValorTotalFreteFora;
            documentoEntrada.ValorTotalOutrasDespesasFora = objValorDocumentoEntrada.ValorTotalOutrasDespesasFora;
            documentoEntrada.ValorTotalDescontoFora = objValorDocumentoEntrada.ValorTotalDescontoFora;
            documentoEntrada.ValorTotalImpostosFora = objValorDocumentoEntrada.ValorTotalImpostosFora;
            documentoEntrada.ValorTotalDiferencialFreteFora = objValorDocumentoEntrada.ValorTotalDiferencialFreteFora;
            documentoEntrada.ValorTotalICMSFreteFora = objValorDocumentoEntrada.ValorTotalICMSFreteFora;
            documentoEntrada.ValorTotalCusto = objValorDocumentoEntrada.ValorTotalCusto;
            documentoEntrada.Veiculo = repVeiculo.BuscarPorPlaca(objValorDocumentoEntrada.Veiculo.Placa);
            documentoEntrada.Equipamento = repEquipamento.BuscarPorDescricao(objValorDocumentoEntrada.Equipamento.Descricao);
            documentoEntrada.Horimetro = objValorDocumentoEntrada.Horimetro;
            documentoEntrada.KMAbastecimento = objValorDocumentoEntrada.KMAbastecimento;
            documentoEntrada.CFOP = repCFOP.BuscarPorCFOP(objValorDocumentoEntrada.CFOP.CodigoCFOP, objValorDocumentoEntrada.CFOP.Tipo);
            documentoEntrada.Destinatario = repEmpresa.BuscarPorCNPJ(objValorDocumentoEntrada.Destinatario.CNPJ);
            documentoEntrada.NaturezaOperacao = repNaturezaOperacao.BuscarPorNumero(objValorDocumentoEntrada.NaturezaOperacao.Numero);
            documentoEntrada.OrdemServico = repOrdemServico.BuscarPorNumero(objValorDocumentoEntrada.OrdemServico.Numero);
            documentoEntrada.OrdemCompra = repOrdemCompra.BuscarPorNumero(objValorDocumentoEntrada.OrdemCompra.Numero);
            documentoEntrada.TipoMovimento = repTipoMovimento.BuscarPorCodigo(objValorDocumentoEntrada.TipoMovimento.Codigo);
            documentoEntrada.ValorTotalRetencaoCOFINS = objValorDocumentoEntrada.ValorTotalRetencaoCOFINS;
            documentoEntrada.ValorTotalRetencaoCSLL = objValorDocumentoEntrada.ValorTotalRetencaoCSLL;
            documentoEntrada.ValorTotalRetencaoISS = objValorDocumentoEntrada.ValorTotalRetencaoISS;
            documentoEntrada.ValorTotalRetencaoIR = objValorDocumentoEntrada.ValorTotalRetencaoIR;
            documentoEntrada.ValorTotalRetencaoINSS = objValorDocumentoEntrada.ValorTotalRetencaoINSS;
            documentoEntrada.ValorTotalRetencaoIPI = objValorDocumentoEntrada.ValorTotalRetencaoIPI;
            documentoEntrada.ValorTotalRetencaoOutras = objValorDocumentoEntrada.ValorTotalRetencaoOutras;
            documentoEntrada.ValorTotalRetencaoPIS = objValorDocumentoEntrada.ValorTotalRetencaoPIS;
            documentoEntrada.ValorProdutos = objValorDocumentoEntrada.ValorProdutos;
            documentoEntrada.ValorBruto = objValorDocumentoEntrada.ValorBruto;
            documentoEntrada.BaseSTRetido = objValorDocumentoEntrada.BaseSTRetido;
            documentoEntrada.ValorSTRetido = objValorDocumentoEntrada.ValorSTRetido;
            documentoEntrada.DataAlteracao = DateTime.Now;
            documentoEntrada.ChaveNotaAnulacao = objValorDocumentoEntrada.ChaveNotaAnulacao;
            documentoEntrada.Observacao = objValorDocumentoEntrada.Observacao;
            documentoEntrada.Expedidor = repCliente.BuscarPorCPFCNPJ(objValorDocumentoEntrada.Expedidor.CPFCNPJ);
            documentoEntrada.Recebedor = repCliente.BuscarPorCPFCNPJ(objValorDocumentoEntrada.Recebedor.CPFCNPJ);
            documentoEntrada.ContratoFinanciamento = repContratoFinanciamento.BuscarPorNumeroDocumento(objValorDocumentoEntrada.ContratoFinanciamento.NumeroDocumento);
            documentoEntrada.TipoFrete = objValorDocumentoEntrada.TipoFrete;
            documentoEntrada.LocalidadeInicioPrestacao = repLocalidade.BuscarPorCodigoIBGE(objValorDocumentoEntrada.LocalidadeInicioPrestacao.IBGE);
            documentoEntrada.LocalidadeTerminoPrestacao = repLocalidade.BuscarPorCodigoIBGE(objValorDocumentoEntrada.LocalidadeTerminoPrestacao.IBGE);
            documentoEntrada.Motivo = objValorDocumentoEntrada.Motivo;
            documentoEntrada.Servico = repServico.BuscarPorCodigoServico(objValorDocumentoEntrada.Servico.CodigoServico);
            documentoEntrada.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigoIBGE(objValorDocumentoEntrada.LocalidadePrestacaoServico.IBGE);
            documentoEntrada.TipoDocumento = objValorDocumentoEntrada.TipoDocumento;
            documentoEntrada.CSTServico = objValorDocumentoEntrada.CSTServico;
            documentoEntrada.AliquotaSimplesNacional = objValorDocumentoEntrada.AliquotaSimplesNacional;
            documentoEntrada.DocumentoFiscalProvenienteSimplesNacional = objValorDocumentoEntrada.DocumentoFiscalProvenienteSimplesNacional;
            documentoEntrada.TributaISSNoMunicipio = objValorDocumentoEntrada.TributaISSNoMunicipio;


            return documentoEntrada;
        }
        private void ValidarRegrasDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.CFOP cfop = documentoEntrada.CFOP;
            if (cfop != null && cfop.BloqueioDocumentoEntrada != BloqueioDocumentoEntrada.SemBloqueio && documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
            {
                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemCompra && documentoEntrada.OrdemCompra == null)
                    throw new ControllerException("É necessário selecionar uma ordem de compra.");

                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemServico && documentoEntrada.OrdemServico == null && documentoEntrada.Itens.Any(i => i.OrdemServico == null) && documentoEntrada.Itens.Any(i => i.OrdensServico.Count == 0))
                    throw new ControllerException("É necessário selecionar uma ordem de serviço.");

                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemServicoESemOrdemCompra && ((documentoEntrada.OrdemServico == null && documentoEntrada.Itens.Any(i => i.OrdemServico == null) && documentoEntrada.Itens.Any(i => i.OrdensServico.Count == 0)) || documentoEntrada.OrdemCompra == null))
                    throw new ControllerException("É necessário selecionar uma ordem de serviço e uma ordem de compra.");

                if (cfop.BloqueioDocumentoEntrada == BloqueioDocumentoEntrada.SemOrdemServicoOuSemOrdemCompra && documentoEntrada.OrdemServico == null && documentoEntrada.Itens.Any(i => i.OrdemServico == null) && documentoEntrada.Itens.Any(i => i.OrdensServico.Count == 0) && documentoEntrada.OrdemCompra == null)
                    throw new ControllerException("É necessário selecionar uma ordem de serviço e uma ordem de compra.");
            }

            if (documentoEntrada.Modelo.Numero.Equals("55") || documentoEntrada.Modelo.Numero.Equals("57"))
            {
                if (string.IsNullOrWhiteSpace(documentoEntrada.Chave))
                    throw new ControllerException("É necessário informar a chave do documento.");
                else if (!Utilidades.Validate.ValidarChave(documentoEntrada.Chave))
                    throw new ControllerException("A chave do documento informada é inválida.");
            }
        }

        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS ObterVeiculos(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS objValorDocumentoEntrada)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            if (objValorDocumentoEntrada.Veiculos != null)
            {
                foreach (var veiculo in objValorDocumentoEntrada.Veiculos)
                {
                    Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorPlaca(veiculo.Placa);

                    documentoEntrada.Veiculos.Add(veic);
                }
            }

            return documentoEntrada;
        }

        private bool SalvarDuplicatas(out string msgRetorno, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS objValorDocumentoEntrada)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);

            if (objValorDocumentoEntrada.Duplicatas != null)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                foreach (var duplicata in objValorDocumentoEntrada.Duplicatas)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicataDoc = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();

                    duplicataDoc.DocumentoEntrada = documentoEntrada;
                    duplicataDoc.DataVencimento = DateTime.ParseExact((string)duplicata.DataVencimento.ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None);
                    duplicataDoc.Numero = (string)duplicata.Numero;
                    duplicataDoc.Sequencia = !string.IsNullOrWhiteSpace(duplicata.Sequencia.ToString()) ? int.Parse(Utilidades.String.OnlyNumbers(duplicata.Sequencia.ToString())) : 0;
                    duplicataDoc.NumeroBoleto = (string)duplicata.NumeroBoleto;
                    duplicataDoc.Valor = (decimal)duplicata.Valor;
                    duplicataDoc.Forma = duplicata.Forma;
                    duplicataDoc.Observacao = (string)duplicata.Observacao;

                    duplicataDoc.Portador = repCliente.BuscarPorCPFCNPJ(duplicata.Portador.CPFCNPJ);

                    if (configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                    {
                        DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                        int result = DateTime.Compare(duplicataDoc.DataVencimento, dataLimiteVencimento);

                        if (result > 0)
                        {
                            msgRetorno = $"A data da duplicata {duplicataDoc.Numero} é maior que a data limite estipulada nas configurações.";
                            return false;
                        }
                    }

                    if (duplicataDoc.Codigo > 0)
                        repDuplicata.Atualizar(duplicataDoc, _auditado);
                    else
                        repDuplicata.Inserir(duplicataDoc, _auditado);
                }
            }
            msgRetorno = "";
            return true;

        }

        private bool SalvarCentrosResultado(out string msgRetorno, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS objValorDocumentoEntrada)
        {
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            if (objValorDocumentoEntrada.CentrosResultados != null)
            {
                decimal valorTotalRateado = 0m, percentual = 0m;
                int countCentroResultado = objValorDocumentoEntrada.CentrosResultados.Count;


                for (int i = 0; i < countCentroResultado; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamento = new Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado();

                    lancamento.TipoDocumento = TipoDocumentoLancamentoCentroResultado.Titulo;
                    lancamento.Ativo = true;
                    lancamento.DocumentoEntrada = documentoEntrada;
                    lancamento.Data = DateTime.Now;
                    lancamento.CentroResultado = repCentroResultado.BuscarPorCodigo(objValorDocumentoEntrada.CentrosResultados[i].CentroResultado.Codigo);
                    lancamento.Percentual = Utilidades.Decimal.Converter((string)objValorDocumentoEntrada.CentrosResultados[i].Percentual.ToString());

                    if ((i + 1) == countCentroResultado)
                        lancamento.Valor = documentoEntrada.ValorTotal - valorTotalRateado;
                    else
                        lancamento.Valor = Math.Round(Math.Floor(documentoEntrada.ValorTotal * lancamento.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += lancamento.Valor;
                    percentual += lancamento.Percentual;

                    if (lancamento.Codigo > 0)
                        repLancamentoCentroResultado.Atualizar(lancamento, _auditado);
                    else
                    {
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, documentoEntrada, "Adicionou o centro de resultados " + lancamento.Descricao, _unitOfWork);
                        repLancamentoCentroResultado.Inserir(lancamento, _auditado);
                    }

                    if (lancamento.CentroResultado != null && lancamento.CentroResultado.Veiculos != null && lancamento.CentroResultado.Veiculos.Count > 0)
                    {
                        if (documentoEntrada.Veiculos == null)
                            documentoEntrada.Veiculos = new List<Dominio.Entidades.Veiculo>();
                        foreach (var veiculo in lancamento.CentroResultado.Veiculos)
                        {
                            if (!documentoEntrada.Veiculos.Any(v => v.Codigo == veiculo.Codigo))
                            {
                                Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo(veiculo.Codigo);

                                documentoEntrada.Veiculos.Add(veic);
                            }
                        }
                    }
                }

                if (countCentroResultado > 0)
                {
                    if (percentual != 100m)
                        throw new ControllerException("O percentual rateado entre os centros de resultado difere de 100%.");

                    if (valorTotalRateado != documentoEntrada.ValorTotal)
                        throw new ControllerException("Ocorreram problemas ao realizar o rateio dos valores entre os centros de resultado.");
                }
            }
            msgRetorno = "";
            return true;
        }

        private bool SalvarCentrosResultadoTiposDespesa(out string msgRetorno, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS objValorDocumentoEntrada)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa repDocumentoEntradaCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(_unitOfWork);

            if (objValorDocumentoEntrada.CentrosResultadosTipoDespesa != null)
            {
                decimal percentual = 0m;
                foreach (var centroResultadoTipoDespesa in objValorDocumentoEntrada.CentrosResultadosTipoDespesa)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa documentoEntradaCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa();

                    documentoEntradaCentroResultadoTipoDespesa.DocumentoEntrada = documentoEntrada;
                    documentoEntradaCentroResultadoTipoDespesa.TipoDespesaFinanceira = repTipoDespesaFinanceira.BuscarPorCodigo(centroResultadoTipoDespesa.TipoDespesaFinanceira.Codigo);
                    documentoEntradaCentroResultadoTipoDespesa.CentroResultado = repCentroResultado.BuscarPorCodigo(centroResultadoTipoDespesa.CentroResultado.Codigo);
                    documentoEntradaCentroResultadoTipoDespesa.Percentual = (centroResultadoTipoDespesa.Percentual.ToString()).ToDecimal();

                    percentual += documentoEntradaCentroResultadoTipoDespesa.Percentual;

                    repDocumentoEntradaCentroResultadoTipoDespesa.Inserir(documentoEntradaCentroResultadoTipoDespesa);
                }

                if (objValorDocumentoEntrada.CentrosResultadosTipoDespesa.Count > 0 && percentual != 100m)
                {
                    msgRetorno = "O percentual rateado entre os Centros de Resultado/Tipos de Despesa difere de 100%.";
                    return false;
                }
            }

            msgRetorno = "";
            return true;
        }

        private bool SalvarItensDocumentoEntradaTMS(out string msgRetorno, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS objValorDocumentoEntrada)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(_unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento repDocumentoEntradaItemAbastecimento = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico repDocumentoEntradaItemOrdemServico = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico(_unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(_unitOfWork);
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(_unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(_unitOfWork);

            if (objValorDocumentoEntrada.Itens != null)
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();


                foreach (var item in objValorDocumentoEntrada.Itens)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem itemDoc = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem();
                    itemDoc.DocumentoEntrada = documentoEntrada;
                    itemDoc.Veiculo = repVeiculo.BuscarPorPlaca(item.Veiculo.Placa);
                    itemDoc.Equipamento = repEquipamento.BuscarPorCodigo(item.Equipamento.Codigo);
                    itemDoc.Horimetro = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(item.Horimetro.ToString())) ? int.Parse(Utilidades.String.OnlyNumbers(item.Horimetro.ToString())) : 0;
                    itemDoc.KMAbastecimento = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(item.KMAbastecimento.ToString())) ? int.Parse(Utilidades.String.OnlyNumbers(item.KMAbastecimento.ToString())) : 0;
                    itemDoc.DataAbastecimento = item.DataAbastecimento != null && item.DataAbastecimento.Value > DateTime.MinValue ? item.DataAbastecimento : null;
                    itemDoc.AliquotaICMS = item.AliquotaICMS != null ? (decimal)item.AliquotaICMS : 0m;
                    itemDoc.AliquotaIPI = item.AliquotaIPI != null ? (decimal)item.AliquotaIPI : 0m;
                    itemDoc.AliquotaCOFINS = item.AliquotaCOFINS != null ? (decimal)item.AliquotaCOFINS : 0m;
                    itemDoc.AliquotaCreditoPresumido = item.AliquotaCreditoPresumido != null ? (decimal)item.AliquotaCreditoPresumido : 0m;
                    itemDoc.AliquotaDiferencial = item.AliquotaDiferencial != null ? (decimal)item.AliquotaDiferencial : 0m;
                    itemDoc.AliquotaICMSST = item.AliquotaICMSST != null ? (decimal)item.AliquotaICMSST : 0m;
                    itemDoc.AliquotaPIS = item.AliquotaPIS != null ? (decimal)item.AliquotaPIS : 0m;
                    itemDoc.BaseCalculoICMS = item.BaseCalculoICMS != null ? (decimal)item.BaseCalculoICMS : 0m;
                    itemDoc.BaseCalculoICMSST = item.BaseCalculoICMSST != null ? (decimal)item.BaseCalculoICMSST : 0m;
                    itemDoc.BaseCalculoIPI = item.BaseCalculoIPI != null ? (decimal)item.BaseCalculoIPI : 0m;
                    itemDoc.BaseCalculoCOFINS = item.BaseCalculoCOFINS != null ? (decimal)item.BaseCalculoCOFINS : 0m;
                    itemDoc.BaseCalculoCreditoPresumido = item.BaseCalculoCreditoPresumido != null ? (decimal)item.BaseCalculoCreditoPresumido : 0m;
                    itemDoc.BaseCalculoDiferencial = item.BaseCalculoDiferencial != null ? (decimal)item.BaseCalculoDiferencial : 0m;
                    itemDoc.BaseCalculoPIS = item.BaseCalculoPIS != null ? (decimal)item.BaseCalculoPIS : 0m;
                    itemDoc.CFOP = repCFOP.BuscarPorCFOP(objValorDocumentoEntrada.CFOP.CodigoCFOP, objValorDocumentoEntrada.CFOP.Tipo);
                    itemDoc.NaturezaOperacao = repNaturezaOperacao.BuscarPorNumero(objValorDocumentoEntrada.NaturezaOperacao.Numero);
                    itemDoc.CSTICMS = (string)item.CSTICMS;
                    itemDoc.CSTCOFINS = (string)item.CSTCOFINS;
                    itemDoc.CSTIPI = (string)item.CSTIPI;
                    itemDoc.CSTPIS = (string)item.CSTPIS;
                    itemDoc.Desconto = item.Desconto != null ? (decimal)item.Desconto : 0m;
                    itemDoc.OrdemServico = repOrdemServico.BuscarPorNumero(objValorDocumentoEntrada.OrdemServico.Numero);
                    itemDoc.OrdemCompraMercadoria = repOrdemCompraMercadoria.BuscarPorCodigo(item.OrdemCompraMercadoria.Codigo);
                    itemDoc.RegraEntradaDocumento = repRegraEntradaDocumento.BuscarPorCodigo(item.CodigoRegraEntradaDocumento);
                    itemDoc.OutrasDespesas = item.OutrasDespesas != null ? (decimal)item.OutrasDespesas : 0m;
                    itemDoc.Produto = repProduto.BuscarPorCodigo((int)item.Produto.Codigo);
                    itemDoc.CodigoProdutoFornecedor = (string)item.CodigoProdutoFornecedor;
                    itemDoc.DescricaoProdutoFornecedor = (string)item.DescricaoProdutoFornecedor;
                    itemDoc.CodigoBarrasEAN = (string)item.CodigoBarrasEAN;
                    itemDoc.NCMProdutoFornecedor = (string)item.NCMProdutoFornecedor;
                    itemDoc.CESTProdutoFornecedor = (string)item.CESTProdutoFornecedor;
                    itemDoc.Quantidade = (decimal)item.Quantidade;
                    itemDoc.Sequencial = (int)item.Sequencial;
                    itemDoc.TipoMovimento = repTipoMovimento.BuscarPorCodigo(item.TipoMovimento.Codigo);
                    itemDoc.Observacao = (string)item.Observacao;
                    itemDoc.UnidadeMedida = (UnidadeDeMedida)item.UnidadeMedida;
                    itemDoc.PercentualReducaoBaseCalculoCOFINS = item.PercentualReducaoBaseCalculoCOFINS != null ? (decimal)item.PercentualReducaoBaseCalculoCOFINS : 0m;
                    itemDoc.PercentualReducaoBaseCalculoIPI = item.PercentualReducaoBaseCalculoIPI != null ? (decimal)item.PercentualReducaoBaseCalculoIPI : 0m;
                    itemDoc.PercentualReducaoBaseCalculoPIS = item.PercentualReducaoBaseCalculoPIS != null ? (decimal)item.PercentualReducaoBaseCalculoPIS : 0m;
                    itemDoc.ValorCOFINS = item.ValorCOFINS != null ? (decimal)item.ValorCOFINS : 0m;
                    itemDoc.ValorFrete = item.ValorFrete != null ? (decimal)item.ValorFrete : 0m;
                    itemDoc.ValorICMS = item.ValorICMS != null ? (decimal)item.ValorICMS : 0m;
                    itemDoc.ValorICMSST = item.ValorICMSST != null ? (decimal)item.ValorICMSST : 0m;
                    itemDoc.BaseSTRetido = item.BaseSTRetido != null ? (decimal)item.BaseSTRetido : 0m;
                    itemDoc.ValorSTRetido = item.ValorSTRetido != null ? (decimal)item.ValorSTRetido : 0m;
                    itemDoc.ValorIPI = item.ValorIPI != null ? (decimal)item.ValorIPI : 0m;
                    itemDoc.ValorPIS = item.ValorPIS != null ? (decimal)item.ValorPIS : 0m;
                    itemDoc.ValorCreditoPresumido = item.ValorCreditoPresumido != null ? (decimal)item.ValorCreditoPresumido : 0m;
                    itemDoc.ValorDiferencial = item.ValorDiferencial != null ? (decimal)item.ValorDiferencial : 0m;
                    itemDoc.ValorTotal = item.ValorTotal != null ? (decimal)item.ValorTotal : 0m;
                    itemDoc.ValorUnitario = (decimal)item.ValorUnitario;
                    itemDoc.ValorSeguro = item.ValorSeguro != null ? (decimal)item.ValorSeguro : 0m;
                    itemDoc.ValorFreteFora = item.ValorFreteFora != null ? (decimal)item.ValorFreteFora : 0m;
                    itemDoc.ValorOutrasDespesasFora = item.ValorOutrasDespesasFora != null ? (decimal)item.ValorOutrasDespesasFora : 0m;
                    itemDoc.ValorDescontoFora = item.ValorDescontoFora != null ? (decimal)item.ValorDescontoFora : 0m;
                    itemDoc.ValorImpostosFora = item.ValorImpostosFora != null ? (decimal)item.ValorImpostosFora : 0m;
                    itemDoc.ValorDiferencialFreteFora = item.ValorDiferencialFreteFora != null ? (decimal)item.ValorDiferencialFreteFora : 0m;
                    itemDoc.ValorICMSFreteFora = item.ValorICMSFreteFora != null ? (decimal)item.ValorICMSFreteFora : 0m;
                    itemDoc.ValorCustoUnitario = item.ValorCustoUnitario != null ? (decimal)item.ValorCustoUnitario : 0m;
                    itemDoc.ValorCustoTotal = item.ValorCustoTotal != null ? (decimal)item.ValorCustoTotal : 0m;
                    itemDoc.CalculoCustoProduto = item.CalculoCustoProduto != null && !string.IsNullOrWhiteSpace((string)item.CalculoCustoProduto) ? (string)item.CalculoCustoProduto : string.Empty;
                    itemDoc.ValorRetencaoCOFINS = item.ValorRetencaoCOFINS != null ? (decimal)item.ValorRetencaoCOFINS : 0m;
                    itemDoc.ValorRetencaoCSLL = item.ValorRetencaoCSLL != null ? (decimal)item.ValorRetencaoCSLL : 0m;
                    itemDoc.ValorRetencaoIR = item.ValorRetencaoIR != null ? (decimal)item.ValorRetencaoIR : 0m;
                    itemDoc.ValorRetencaoISS = item.ValorRetencaoISS != null ? (decimal)item.ValorRetencaoISS : 0m;
                    itemDoc.ValorRetencaoINSS = item.ValorRetencaoINSS != null ? (decimal)item.ValorRetencaoINSS : 0m;
                    itemDoc.ValorRetencaoIPI = item.ValorRetencaoIPI != null ? (decimal)item.ValorRetencaoIPI : 0m;
                    itemDoc.ValorRetencaoOutras = item.ValorRetencaoOutras != null ? (decimal)item.ValorRetencaoOutras : 0m;
                    itemDoc.ValorRetencaoPIS = item.ValorRetencaoPIS != null ? (decimal)item.ValorRetencaoPIS : 0m;
                    itemDoc.NumeroFogoInicial = item.NumeroFogoInicial != null ? ((string)item.NumeroFogoInicial.ToString()).ToInt() : 0;
                    itemDoc.TipoAquisicao = item.TipoAquisicao != null ? ((string)item.TipoAquisicao.ToString()).ToNullableEnum<TipoAquisicaoPneu>() : null;
                    itemDoc.VidaAtual = item.VidaAtual != null ? ((string)item.VidaAtual.ToString()).ToNullableEnum<VidaPneu>() : null;
                    itemDoc.Almoxarifado = item.CodigoAlmoxarifado > 0 ? repAlmoxarifado.BuscarPorCodigo(item.CodigoAlmoxarifado) : null;
                    itemDoc.ProdutoVinculado = repProduto.BuscarPorCodigo(item.ProdutoVinculado.Codigo);
                    itemDoc.QuantidadeProdutoVinculado = item.QuantidadeProdutoVinculado != null ? (decimal)item.QuantidadeProdutoVinculado : 0m;
                    itemDoc.LocalArmazenamento = repLocalArmazenamentoProduto.BuscarPorCodigo(item.CodigoLocalArmazenamento);
                    itemDoc.UnidadeMedidaFornecedor = (string)item.UnidadeMedidaFornecedor;
                    itemDoc.QuantidadeFornecedor = item.QuantidadeFornecedor != null ? (decimal)item.QuantidadeFornecedor : 0m;
                    itemDoc.ValorUnitarioFornecedor = item.ValorUnitarioFornecedor != null ? (decimal)item.ValorUnitarioFornecedor : 0m;
                    itemDoc.CentroResultado = repCentroResultado.BuscarPorCodigo(item.CentroResultado.Codigo);
                    itemDoc.GeraRateioDespesaVeiculo = itemDoc.CFOP?.RealizarRateioDespesaVeiculo ?? false;
                    itemDoc.OrigemMercadoria = item.OrigemMercadoria != null ? ((string)item.OrigemMercadoria.ToString()).ToNullableEnum<OrigemMercadoria>() : null;
                    itemDoc.EncerrarOrdemServico = item.EncerrarOrdemServico;

                    if ((itemDoc.CFOP?.ObrigarInformarLocalArmazenamento ?? false) && itemDoc.LocalArmazenamento == null)
                    {
                        msgRetorno = "É necessário informar local de armazenamento do item " + itemDoc.Sequencial + ", pois existe configuração de CFOP habilitada com obrigatoriedade (Aba outros do item).";
                        return false;
                    }

                    if (itemDoc.Abastecimentos != null && itemDoc.Abastecimentos.Count > 0 && itemDoc.Veiculo != null && itemDoc.Quantidade > itemDoc.Veiculo.CapacidadeTanque && itemDoc.Veiculo.CapacidadeTanque > 0)
                    {
                        msgRetorno = "Litros abastecidos no veículo do item " + itemDoc.Sequencial + " é maior que sua Capacidade de Tanque (" + itemDoc.Veiculo.CapacidadeTanque.ToString() + ").";
                        return false;
                    }

                    repItem.Inserir(itemDoc, _auditado);

                }
            }
            msgRetorno = "";
            return true;
        }
        #endregion

        private string ObterPDFBase64(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_unitOfWork);

            string caminhoPDF = servFatura.ObterCaminhoPDF(fatura, _unitOfWork, _tipoServicoMultisoftware);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
            {
                byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                if (data != null)
                    return Convert.ToBase64String(data);
            }
            return null;
        }

        #endregion
    }
}
