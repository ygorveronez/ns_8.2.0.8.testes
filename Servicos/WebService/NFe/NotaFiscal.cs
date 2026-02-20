using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.ObjetosDeValor.WebService;
using Servicos.Embarcador.Integracao;
using Servicos.Embarcador.Pedido;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.WebService.NFe
{
    public class NotaFiscal : ServicoWebServiceBase
    {
        Repositorio.UnitOfWork _unitOfWork;
        TipoServicoMultisoftware _tipoServicoMultisoftware;
        Auditado _auditado;
        AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly Dominio.Entidades.Usuario _usuario;
        private readonly string _webServiceConsultaCTe;

        public NotaFiscal(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
            _clienteMultisoftware = cliente;

        }

        public NotaFiscal(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, string webServiceConsultaCTe) : base(unitOfWork, tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
            _clienteMultisoftware = cliente;
            _webServiceConsultaCTe = webServiceConsultaCTe;
        }

        #region Métodos Públicos

        public static string ExcluirNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool removerEtapaDiferente = false, bool parcialCancelado = false)
        {
            try
            {
                if (cargaPedido.Carga.DataEnvioUltimaNFe.HasValue && !(cargaPedido.Carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && !parcialCancelado)
                    return "Carga já recebeu todos documentos, não é possível a exclusão de notas fiscais.";

                Servicos.Embarcador.Carga.DocumentoEmissao servicoDocumentoEmissao = new Servicos.Embarcador.Carga.DocumentoEmissao(unitOfWork, removerEtapaDiferente);
                Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                servicoDocumentoEmissao.DeletarPorNotaFiscal(cargaPedido, xmlNotaFiscal.Codigo, auditado);
                servicoCargaPedido.AlterarDadosSumarizadosCargaPedido(cargaPedido, xmlNotaFiscal.Volumes, 0);
                repCargaPedido.Atualizar(cargaPedido);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> outrasCargasPedidos = repositorioPedidoXMLNotaFiscal.ObterSeExisteEmOutroPedido(xmlNotaFiscal.Codigo, cargaPedido.Codigo);

                if (outrasCargasPedidos.Count == 0)
                {
                    servicoCanhoto.ExcluirCanhotoDaNotaFiscal(xmlNotaFiscal, unitOfWork);
                }
                else
                {
                    Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAnterior = (from obj in outrasCargasPedidos where obj.Recebedor != null && obj.Expedidor != null select obj).FirstOrDefault();

                    if (cargaPedidoAnterior == null)
                        cargaPedidoAnterior = (from obj in outrasCargasPedidos where obj.Recebedor != null && obj.Expedidor == null select obj).FirstOrDefault();

                    if (cargaPedidoAnterior == null)
                        cargaPedidoAnterior = outrasCargasPedidos.LastOrDefault();

                    List<Dominio.Entidades.Usuario> motoristas = repositorioCargaMotorista.BuscarMotoristasPorCarga(cargaPedidoAnterior.Carga.Codigo);
                    servicoCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedidoAnterior, cargaPedidoAnterior.Carga.Terceiro, motoristas, tipoServicoMultisoftware, configuracao, unitOfWork, configuracaoCanhoto);
                }

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

                if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada)
                {
                    int numeroNotas = repositorioPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo);

                    if (numeroNotas == 0)
                    {
                        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repositorioPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                        Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        if (pedidosCTeParaSubContratacao.Count > 0)
                        {
                            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.SubContratada;
                            cargaPedido.Pedido.PedidoSubContratado = true;
                            cargaPedido.Tomador = pedidosCTeParaSubContratacao.First().CTeTerceiro.TransportadorTerceiro;
                            cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                            cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                            if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                                servicoCarga.AtualizarTipoServicoCargaMultimodal(pedidosCTeParaSubContratacao.First().CTeTerceiro, cargaPedido, unitOfWork, out string msgRetornoTipoServico);

                            repositorioPedido.Atualizar(cargaPedido.Pedido);
                            repositorioCargaPedido.Atualizar(cargaPedido);
                            servicoHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                        }
                        else
                        {
                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;

                            repositorioCargaPedido.Atualizar(cargaPedido);
                        }

                        servicoCarga.SetarTipoContratacaoCarga(cargaPedido.Carga, unitOfWork);
                    }
                }

                Servicos.Embarcador.Pedido.OcorrenciaPedido servicoOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

                servicoOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.FaturamentoCancelado, cargaPedido.Pedido, configuracao, null);
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, "Excluiu a nota fiscal " + xmlNotaFiscal.Numero.ToString() + ".", unitOfWork);
            }
            catch (BaseException excecao)
            {
                return excecao.Message;
            }

            return "";
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarDadosNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Dominio.Entidades.WebService.Integradora integradora, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Global.TempoDeExecucao servicoTempoDeExecucao = new Global.TempoDeExecucao();

            Servicos.Log.TratarErro($"IntegrarDadosNotasFiscais - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | notasFiscais {(notasFiscais != null ? Newtonsoft.Json.JsonConvert.SerializeObject(notasFiscais) : string.Empty)}", "Request");

            bool retornoStatus = true;
            int codigoMensagem;
            string retornoMensagem = "";
            StringBuilder stMensagem = new StringBuilder();

            try
            {
                if (protocolo?.protocoloIntegracaoCarga > 0 || protocolo?.protocoloIntegracaoPedido > 0)
                {
                    Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);

                    Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                    Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                    string mensagemErro = string.Empty;

                    if (configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono && (protocolo.protocoloIntegracaoCarga == 0 || protocolo.protocoloIntegracaoPedido == 0))
                    {
                        mensagemErro = $"Protocolos informados são inválidos. ";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
                    }

                    string existeCampoObrigratorioVazio = "";
                    if (cargaIntegracaoCompleta != null)
                        existeCampoObrigratorioVazio = servicoCarga.ValidacaoInicialDosCamposObrigratorios(cargaIntegracaoCompleta, _unitOfWork);

                    if (!string.IsNullOrEmpty(existeCampoObrigratorioVazio))
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(existeCampoObrigratorioVazio);

                    if (protocolo.protocoloIntegracaoCarga > 0 && !string.IsNullOrEmpty(protocolo.NumeroContainerPedido))
                        cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEContainerPedido(protocolo.protocoloIntegracaoCarga, protocolo.NumeroContainerPedido);
                    if (cargaPedido == null && protocolo.protocoloIntegracaoCarga > 0 && protocolo.protocoloIntegracaoPedido > 0)
                        cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                    if (cargaPedido == null && protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                        cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);
                    else if (cargaPedido == null && protocolo.protocoloIntegracaoCarga > 0 && protocolo.protocoloIntegracaoPedido == 0)
                        cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloCarga(protocolo.protocoloIntegracaoCarga, !string.IsNullOrEmpty(protocolo.NumeroContainerPedido));

                    if (cargaPedido != null && cargaPedido.Pedido != null && !string.IsNullOrEmpty(protocolo.NumeroContainerPedido))
                    {
                        cargaPedido.Pedido.Container = repContainer.BuscarPorNumero(protocolo.NumeroContainerPedido);
                        if (cargaPedido.Pedido.Container == null)
                        {
                            cargaPedido.Pedido.Container = serPedidoWS.SalvarContainer(protocolo.Container, ref stMensagem, _auditado);
                            if (stMensagem.Length > 0)
                            {
                                Servicos.Log.TratarErro($"Falha ao salvar container: {stMensagem.ToString()}");
                                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(stMensagem.ToString());
                            }
                        }
                        cargaPedido.Pedido.TaraContainer = protocolo.TaraContainer;
                        cargaPedido.Pedido.LacreContainerUm = protocolo.LacreContainerUm;
                        cargaPedido.Pedido.LacreContainerDois = protocolo.LacreContainerDois;
                        cargaPedido.Pedido.LacreContainerTres = protocolo.LacreContainerTres;

                        repPedido.Atualizar(cargaPedido.Pedido);
                    }

                    if (notasFiscais == null)
                    {
                        if (cargaPedido?.Carga != null)
                            new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(_unitOfWork, _tipoServicoMultisoftware).AdicionarIntegracaoIndividual(cargaPedido.Carga, EtapaCarga.NotaFiscal, $"Os dados da nota fiscal integrada para o protocolo de pedido {protocolo.protocoloIntegracaoPedido} está vazia.", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });

                        mensagemErro = $"Os dados da nota fiscal integrada para o protocolo de pedido {protocolo.protocoloIntegracaoPedido} está vazia.";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
                    }

                    if (cargaPedido == null && configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocolo.protocoloIntegracaoCarga, false);
                        if (carga != null)
                            new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(_unitOfWork, _tipoServicoMultisoftware).AdicionarIntegracaoIndividual(carga, EtapaCarga.NotaFiscal, $"O protocolo pedido {protocolo.protocoloIntegracaoPedido} não esta na carga.", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });

                        mensagemErro = $"O protocolo pedido {protocolo.protocoloIntegracaoPedido} não esta na carga.";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
                    }

                    if (cargaPedido != null)
                    {
                        if (configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono)
                        {
                            //Nenhuma validaçao agora, retorna sucesso, salva dados em uma tabela com arquivo para processar depois em paralelo

                            codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                            Dominio.ObjetosDeValor.WebService.Retorno<bool> retorno = Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, retornoMensagem);

                            Dominio.ObjetosDeValor.Embarcador.NFe.DadosNotaProcessar dadosNotaProcessar = new Dominio.ObjetosDeValor.Embarcador.NFe.DadosNotaProcessar();
                            dadosNotaProcessar.CodigoIntegradora = integradora.Codigo;
                            dadosNotaProcessar.NotasFiscais = notasFiscais;
                            dadosNotaProcessar.Protocolo = protocolo;

                            SalvarDadosNotaProcessar(cargaPedido.Carga, protocolo, integradora, Newtonsoft.Json.JsonConvert.SerializeObject(dadosNotaProcessar), Newtonsoft.Json.JsonConvert.SerializeObject(retorno));

                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, true, retornoMensagem);

                            return retorno;
                        }

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                        if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && (
                            cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                            (!configuracao.NaoAceitarNotasNaEtapa1 && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova) ||
                            (configuracaoGeralCarga.PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador && ((cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete) || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador)) ||
                            (cargaPedido.Carga.CargaEmitidaParcialmente && (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos)))
                         )
                        {
                            _unitOfWork.Start();

                            string retornoIntegracao = IntegrarNotaFiscal(cargaPedido, notasFiscais, null, null, configuracao, _tipoServicoMultisoftware, _auditado, integradora, _unitOfWork);

                            if (string.IsNullOrWhiteSpace(retornoIntegracao))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido, "Integrou dados de notas fiscais", _unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Integrou dados de notas fiscais", _unitOfWork);

                                retornoStatus = true;
                                codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                _unitOfWork.CommitChanges();
                            }
                            else
                            {
                                if (retornoIntegracao.Contains("já foi enviada"))
                                {
                                    retornoMensagem = retornoIntegracao;
                                    retornoStatus = false;
                                    codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                }
                                else
                                {
                                    retornoMensagem = retornoIntegracao;
                                    retornoStatus = false;
                                    codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                }
                                _unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            if ((configuracao.NaoAceitarNotasNaEtapa1 && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                                || (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete))
                            {
                                retornoStatus = false;

                                if (configuracao.NotaUnicaEmCargas && notasFiscais != null && notasFiscais.Count() == 1 &&
                                    !string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().Chave) &&
                                    (repPedidoXMLNotaFiscal.BuscarPorChave(notasFiscais.FirstOrDefault().Chave) != null))
                                {
                                    codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                    retornoMensagem = "A nota fiscal " + notasFiscais.FirstOrDefault().Chave + " já foi enviada.";
                                }
                                else
                                {
                                    if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                                        codigoMensagem = 313;
                                    else if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
                                        codigoMensagem = 314;
                                    else
                                        codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                                    retornoMensagem += "Não é possível enviar as notas fiscais para a carga em sua atual situação (" + cargaPedido.Carga.DescricaoSituacaoCarga + "). ";
                                    new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(_unitOfWork, _tipoServicoMultisoftware).AdicionarIntegracaoIndividual(cargaPedido.Carga, EtapaCarga.NotaFiscal, retornoMensagem, new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });
                                }
                            }
                            else
                            {
                                if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && cargaPedido.Carga.CargaEmitidaParcialmente)
                                {
                                    retornoMensagem += "Para enviar as demais notas a carga precisa estar na situação em transporte.";
                                    codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retornoStatus = false;
                                }
                                else if (cargaPedido.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                                {
                                    retornoMensagem += "As notas fiscais já foram enviadas para esse pedido.";
                                    codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                    retornoStatus = configuracao.RetornosDuplicidadeWSSubstituirPorSucesso;
                                }
                                else
                                {
                                    retornoMensagem += $"Não é possível enviar as notas fiscais para a carga em sua atual situação ({cargaPedido.Carga.DescricaoSituacaoCarga})";
                                    codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retornoStatus = false;
                                }
                            }
                        }
                    }
                    else if (protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocolo.protocoloIntegracaoPedido);
                        if (pedido != null)
                        {
                            _unitOfWork.Start();
                            Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(pedido, notasFiscais, null, null, configuracao, _tipoServicoMultisoftware, _auditado, integradora, _unitOfWork);
                            retornoStatus = true;
                            codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;

                            if (notasFiscais.Any(o => o.Chave == "00"))
                                retornoMensagem = "Foram integrados dados de notas fiscais com a sua chave zerada.";

                            Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, "Integrou dados de notas fiscais", _unitOfWork);
                            _unitOfWork.CommitChanges();
                        }
                        else
                        {
                            retornoStatus = false;
                            retornoMensagem = "Protocolos informados são inválidos. ";
                            codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        retornoStatus = false;

                        if (configuracao.NotaUnicaEmCargas && notasFiscais != null && notasFiscais.Count() == 1 && !string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().Chave) && (repPedidoXMLNotaFiscal.BuscarPorChave(notasFiscais.FirstOrDefault().Chave) != null))
                        {
                            codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                            retornoMensagem = "A nota fiscal " + notasFiscais.FirstOrDefault().Chave + " já foi enviada.";
                        }
                        else
                        {
                            retornoMensagem = "Protocolos informados são inválidos. ";
                            codigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }

                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;

                    Servicos.Log.TratarErro($"IntegrarDadosNotasFiscais - Protocolo Carga {protocolo?.protocoloIntegracaoCarga ?? 0} | Tempo total levado: {ts.ToString(@"mm\:ss\:fff")}", "TempoExecucao");

                    if (retornoStatus)
                    {
                        servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", ts, true, retornoMensagem);
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, retornoMensagem);
                    }
                    else
                    {
                        if (codigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao)
                        {
                            servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", ts, false, retornoMensagem);
                            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDuplicidadeRequisicao(retornoMensagem);
                        }
                        else
                        {
                            if (cargaPedido?.Carga != null)
                                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(_unitOfWork, _tipoServicoMultisoftware).AdicionarIntegracaoIndividual(cargaPedido.Carga, EtapaCarga.NotaFiscal, retornoMensagem, new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });

                            servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", ts, false, retornoMensagem);
                            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao(retornoMensagem, retornoStatus);
                        }
                    }
                }
                else
                {
                    retornoMensagem = "É obrigatório informar os protocolos de integração.";
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, retornoMensagem);
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(retornoMensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("IntegrarDadosNotasFiscais", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, ex.Message);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();

            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais, Dominio.Entidades.WebService.Integradora integradora)
        {
            Servicos.Log.TratarErro("IntegrarNotasFiscais - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscais - TokensXMLNotasFiscais: " + (TokensXMLNotasFiscais != null ? Newtonsoft.Json.JsonConvert.SerializeObject(TokensXMLNotasFiscais) : string.Empty), "Request");

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            retorno.Mensagem = "";
            try
            {
                _unitOfWork.Start();
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                    VincularNotasFiscais(protocolo, integradora, TokensXMLNotasFiscais, null, null, null, null, ref caminhosXMLTemp, ref retorno, _unitOfWork, _tipoServicoMultisoftware);
                else
                {

                    _unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar os protocolos de integração. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
                if (retorno.Status)
                {
                    foreach (string caminho in caminhosXMLTemp)
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }

                Servicos.Log.TratarErro("IntegrarNotasFiscais - Retorno: " + (!retorno.Status ? retorno.Mensagem : "Sucesso"));

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(TokensXMLNotasFiscais, _unitOfWork);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) NF-e(s)";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            //if (retorno.CodigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
            //{
            //    falar com rafael da pelissari aqui está gerando muito log, fica tentando mandar sem parar para viagens de armazens
            //    Servicos.Log.TratarErro(retorno.Mensagem);
            //    Servicos.Log.TratarErro("Protocolo Pedido: " + protocolo != null ? protocolo.protocoloIntegracaoPedido.ToString() : "nulo" + "Protocolo Carga: " + protocolo != null ? protocolo.protocoloIntegracaoCarga.ToString() : "nulo" + " Retorno: " + retorno.Mensagem + " Status:" + retorno.Status.ToString());
            //}

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarNotasFiscaisComAverbacaoeValePedagio(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.Entidades.WebService.Integradora integradora, Dominio.ObjetosDeValor.MDFe.CIOT Ciot, Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido informacoesPagamentoPedido)
        {
            Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - TokensXMLNotasFiscais: " + (TokensXMLNotasFiscais != null ? Newtonsoft.Json.JsonConvert.SerializeObject(TokensXMLNotasFiscais) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - Averbacao: " + (Averbacao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Averbacao) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - ValePedagio: " + (ValePedagio != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ValePedagio) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - CIOT: " + (Ciot != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Ciot) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - InformacoesPagamento: " + (informacoesPagamentoPedido != null ? Newtonsoft.Json.JsonConvert.SerializeObject(informacoesPagamentoPedido) : string.Empty), "Request");

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                _unitOfWork.Start();
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                    VincularNotasFiscais(protocolo, integradora, TokensXMLNotasFiscais, Averbacao, ValePedagio, Ciot, informacoesPagamentoPedido, ref caminhosXMLTemp, ref retorno, _unitOfWork, _tipoServicoMultisoftware);
                else
                {

                    _unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar os protocolos de integração. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
                if (retorno.Status)
                {
                    foreach (string caminho in caminhosXMLTemp)
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }

                Servicos.Log.TratarErro("IntegrarNotasFiscaisComAverbacaoeValePedagio - Retorno: " + (!retorno.Status ? retorno.Mensagem : "Sucesso"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(TokensXMLNotasFiscais, _unitOfWork);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) NF-e(s)";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            //if (retorno.CodigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
            //{
            //    falar com rafael da pelissari aqui está gerando muito log, fica tentando mandar sem parar para viagens de armazens
            //    Servicos.Log.TratarErro(retorno.Mensagem);
            //    Servicos.Log.TratarErro("Protocolo Pedido: " + protocolo != null ? protocolo.protocoloIntegracaoPedido.ToString() : "nulo" + "Protocolo Carga: " + protocolo != null ? protocolo.protocoloIntegracaoCarga.ToString() : "nulo" + " Retorno: " + retorno.Mensagem + " Status:" + retorno.Status.ToString());
            //}

            return retorno;
        }

        /// <summary>
        /// Método criado apenas para atualizar apenas informações da nota, não deve ter vinculo ou influência com cargaPedido e carga;
        /// </summary>
        /// <param name="notasFiscais"></param>
        /// <param name="tipoServicoMultisoftware"></param>
        /// <param name="Auditado"></param>
        /// <param name="unitOfWork"></param>
        public static void IntegrarNotaFiscal(List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Embarcador.Pedido.NotaFiscal svcNotaFiscal = new Embarcador.Pedido.NotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscais)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                string retorno = "";

                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                    xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

                if (xmlNotaFiscal == null)
                    xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                else
                {
                    xmlNotaFiscal.Initialize();
                    xmlNotaFiscal.NotaJaEstavaNaBase = true;
                }

                notaFiscal.DocumentoRecebidoViaNOTFIS = true;
                notaFiscal.Modelo = "55";
                notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

                xmlNotaFiscal = svcNotaFiscal.PreencherParaXMLNotaFiscal(ref xmlNotaFiscal, notaFiscal, null, null, ref retorno);

                if (!string.IsNullOrWhiteSpace(retorno))
                    throw new ServicoException(retorno);

                if (xmlNotaFiscal.Codigo == 0)
                {
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                    repXmlNotaFiscal.Inserir(xmlNotaFiscal, Auditado);
                }
                else
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal, Auditado);

                new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork, configuracaoTMS).ArmazenarProdutosNotaFiscalPorListaDeProduto(notaFiscal.Produtos, xmlNotaFiscal, null, Auditado, tipoServicoMultisoftware);

                xmlNotaFiscal.DocumentoRecebidoViaFTP = true;

                if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta && (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP))
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
                if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta && xmlNotaFiscal.NotaJaEstavaNaBase)
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColetaManual;
                else if (xmlNotaFiscal.NotaJaEstavaNaBase && (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP))
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPManual;
                else if (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP)
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTP;
                else if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta)
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColeta;
                else
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.Manual;


                svcNotaFiscal.PreencherDadosContabeisXMLNotaFiscal(xmlNotaFiscal, notaFiscal);
            }
        }

        public static string IntegrarNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, string numeroControlePedido = "")
        {

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Documentos.ConsultaDocumento srvConsultaDocumento = new Embarcador.Documentos.ConsultaDocumento(unitOfWork, tipoServicoMultisoftware, Auditado);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            decimal pesoNaNFs = 0;
            int volumes = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscais)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                {
                    int quantidadeRepetida = (from obj in notasFiscais where obj.Chave == notaFiscal.Chave select obj).Count();
                    if (quantidadeRepetida > 1)
                        return "A nota fiscal " + notaFiscal.Chave + " foi enviada mais de uma vez no mesmo request por favor ajuste a integração.";

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFisca.BuscarPorChave(notaFiscal.Chave);
                    if (xmlNotaFiscalExiste != null)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscalExiste.Codigo);
                        if (configuracao.NotaUnicaEmCargas)
                        {
                            if (pedidosExistente.Any(obj => obj.CargaPedido.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado))
                                return "A nota fiscal " + xmlNotaFiscalExiste.Chave + " já foi enviada.";
                        }

                        // #29094
                        if (!configuracao.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                            if (pedidosExistente.Any(obj => obj.CargaPedido.Carga.Codigo == cargaPedido.Carga.Codigo))
                                return "A nota fiscal " + xmlNotaFiscalExiste.Chave + " já foi enviada para outro pedido nesta mesma carga.";
                    }

                    xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscal(notaFiscal.Chave);

                    if (xmlNotaFiscal == null)
                        xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorDocumentoDestinado(notaFiscal.Chave, tipoServicoMultisoftware);

                    if (xmlNotaFiscal == null)
                    {
                        try
                        {
                            if ((notaFiscal?.Emitente != null && notaFiscal?.Destinatario != null) && notaFiscal.Emitente.CPFCNPJ == notaFiscal.Destinatario.CPFCNPJ)
                                return "O emitente e destinatário da nota não podem ter o mesmo CNPJ, por favor verifique pois a nota enviada está com alguma inconsistência entre o emitente e o destinatário informados.";

                            xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorSerpro(notaFiscal.Chave);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    }
                }

                bool modeloOutros = notaFiscal.Modelo == "99";
                bool formaIntegracaoManual = notaFiscal.FormaIntegracao.HasValue && notaFiscal.FormaIntegracao == FormaIntegracao.Manual;
                if (configuracaoTMS.UtilizaEmissaoMultimodal && xmlNotaFiscal == null && (!modeloOutros && !formaIntegracaoManual))
                    return "Não foi possível localizar o XML da nota fiscal enviada.";

                if (xmlNotaFiscal != null && configuracaoTMS.NotaUnicaEmCargas)
                {
                    xmlNotaFiscal.SemCarga = false;
                    repXMLNotaFisca.Atualizar(xmlNotaFiscal);
                }

                notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;

                if (notaFiscal.Emitente == null)
                    notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);

                if (notaFiscal.Destinatario == null)
                    notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario);

                if (xmlNotaFiscal != null && xmlNotaFiscal.ClassificacaoNFe == null && notaFiscal.ClassificacaoNFe != null)
                {
                    xmlNotaFiscal.ClassificacaoNFe = notaFiscal.ClassificacaoNFe;
                    repXMLNotaFisca.Atualizar(xmlNotaFiscal);
                }

                if (xmlNotaFiscal == null)
                {
                    string retornoAddNFe = serCargaNotaFiscal.InformarDadosNotaCarga(notaFiscal, cargaPedido, tipoServicoMultisoftware, configuracao, Auditado, out bool alteradoTipoDeCarga, true, false, true, numeroControlePedido);

                    if (!string.IsNullOrWhiteSpace(retornoAddNFe))
                        return retornoAddNFe;

                    pesoNaNFs += notaFiscal.PesoBruto;
                    volumes += (int)notaFiscal.VolumesTotal;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(numeroControlePedido))
                        xmlNotaFiscal.NumeroControlePedido = numeroControlePedido;

                    new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork, configuracaoTMS).ArmazenarProdutosNotaFiscalPorListaDeProduto(notaFiscal.Produtos, xmlNotaFiscal, cargaPedido.Pedido, Auditado, tipoServicoMultisoftware);
                    serCargaNotaFiscal.InformarDadosNotaCarga(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out string msgAlerta, Auditado);
                    if (!string.IsNullOrWhiteSpace(msgAlerta))
                        return msgAlerta;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, "Adicionado por integração de dados de notas fiscais", unitOfWork);

                    pesoNaNFs += xmlNotaFiscal.Peso;
                    volumes += xmlNotaFiscal.Volumes;
                }
            }

            if (cargaPedido.Carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false)
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
                Servicos.Embarcador.Pedido.Produto serProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xMLNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produtoNotaFiscal in xMLNotaFiscalProdutos)
                {
                    Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto = (from obj in produtos where obj.Codigo == produtoNotaFiscal.Produto.CodigoProdutoEmbarcador select obj).FirstOrDefault();
                    if (produto == null)
                    {
                        produto = new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos();
                        produto.Descricao = produtoNotaFiscal.Produto.Descricao;
                        produto.Codigo = produtoNotaFiscal.Produto.CodigoProdutoEmbarcador;
                        produto.QuantidadeComercial = produtoNotaFiscal.Quantidade;
                        produto.ValorUnitarioComercial = produtoNotaFiscal.ValorProduto;
                        produtos.Add(produto);
                    }
                    else
                    {
                        produto.QuantidadeComercial += produtoNotaFiscal.Quantidade;
                        produto.ValorUnitarioComercial += produtoNotaFiscal.ValorProduto;
                    }
                }

                serProduto.AtualizarProdutosCargaPedidoPorNotaFiscal(produtos, cargaPedido, unitOfWork, Auditado);
            }

            return FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFs, volumes, null, Averbacao, ValePedagio, null, null, configuracao, tipoServicoMultisoftware, Auditado, integradora, unitOfWork);
        }

        public static string FinalizarEnvioDasNotas(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal pesoNaNFs, int volumes, List<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> averbacoesNF, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.ObjetosDeValor.MDFe.CIOT Ciot, Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido informacoesPagamentoPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizado = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

            decimal peso = 0m;
            decimal pesoLiquido = 0m;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (configuracao.NaoAtualizarPesoPedidoPelaNFe)
            {
                pesoNaNFs = cargaPedido.Pedido.PesoTotal;
                //peso = pesoNaNFs;
            }

            serCargaPedido.CriarUnidadesDeMedidaDaCargaPedido(cargaPedido, pesoNaNFs, volumes, unitOfWork);
            cargaPedido.CienciaDoEnvioDaNotaInformado = true;

            //nova regra criada dia 26/02/2020, quando o tipo de operação permite informar documentos manualmente não
            if ((cargaPedido.Carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) == false)
                cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;

            AjustarModalidadePagamentosCargaPedido(ref cargaPedido, unitOfWork);

            serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedido, null, null, tipoServicoMultisoftware, configuracao, ref peso, ref pesoLiquido, configuracao.NaoAtualizarPesoPedidoPelaNFe, unitOfWork);

            // Config SaintGobain.. atualizou o peso ..o do carga pedido ao integrar o produto..
            if ((peso > 0m && !configuracao.AtualizarProdutosCarregamentoPorNota) || configuracao.NaoUsarPesoNotasPallet)
            {
                //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {peso}. NotaFiscal.FinalizarEnvioDasNotas", "PesoCargaPedido");
                cargaPedido.Peso = peso;
            }

            if (pesoLiquido > 0m || configuracao.NaoUsarPesoNotasPallet)
                cargaPedido.PesoLiquido = pesoLiquido;

            repCargaPedido.Atualizar(cargaPedido);

            //Atualizar peso sumarizado
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaPedido.Carga?.Codigo ?? 0);
            serCargaDadosSumarizado.AtualizarPesos(cargaPedido.Carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware);

            //quando a carga possui foi um redespacho e o segundo trecho ainda não recebeu as notas fiscais, aqui vincula as notas proximo trecho.
            if (cargaPedido.CargaPedidoProximoTrecho != null && cargaPedido.CargaPedidoProximoTrecho.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                serNFe.VincularNotasDeRedespachoVinculadas(cargaPedido.CargaPedidoProximoTrecho, cargaPedido, unitOfWork, tipoServicoMultisoftware, Auditado);

            //quando a carga possui foi um redespacho e o primeiro trecho ainda não recebeu as notas fiscais, aqui vincula as notas ao trecho anterior.
            if (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                serNFe.VincularNotasDeRedespachoVinculadas(cargaPedido.CargaPedidoTrechoAnterior, cargaPedido, unitOfWork, tipoServicoMultisoftware, Auditado);

            string validacaoAverbacao = VincularAverbacoesPedido(cargaPedido.Pedido, Averbacao, averbacoesNF, unitOfWork);

            if (!string.IsNullOrWhiteSpace(validacaoAverbacao))
                return validacaoAverbacao;

            string validacaoValePedagio = VincularValePedagioPedido(cargaPedido.Pedido, ValePedagio, unitOfWork);

            if (!string.IsNullOrWhiteSpace(validacaoValePedagio))
                return validacaoValePedagio;

            string validacaoCIOT = VincularCIOTPedido(cargaPedido.Pedido, Ciot, unitOfWork);

            if (!string.IsNullOrWhiteSpace(validacaoCIOT))
                return validacaoCIOT;

            string validacaoInformacoesPagamentoPedido = VincularInformacoesPagamentoPedido(cargaPedido.Pedido, informacoesPagamentoPedido, Ciot, unitOfWork);

            if (!string.IsNullOrWhiteSpace(validacaoInformacoesPagamentoPedido))
                return validacaoInformacoesPagamentoPedido;

            if ((cargaPedido.Carga.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false) && tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
            {
                if (new Embarcador.Pedido.ConferenciaContainer(unitOfWork).PossuiConferenciaSemAprovacao(cargaPedido.Carga))
                    return "";
            }
            else
            {
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(cargaPedido.Carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = retiradaContainer?.ContainerTipo ?? cargaPedido.Carga.ModeloVeicularCarga?.ContainerTipo ?? cargaPedido.Carga.Carregamento?.ModeloVeicularCarga?.ContainerTipo;

                if (containerTipo != null)
                {
                    if (retiradaContainer?.ColetaContainer?.Container == null)
                    {
                        if (!cargaPedido.Carga.LiberadaSemRetiradaContainer)
                            return "";
                    }
                    //Removido para inicio viagem tarefa #55552
                    //else if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgTransportador || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe)
                    //{
                    //    servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                    //    {
                    //        coletaContainer = retiradaContainer.ColetaContainer,
                    //        DataAtualizacao = DateTime.Now,
                    //        Status = StatusColetaContainer.EmDeslocamentoCarregado,
                    //        Usuario = Auditado?.Usuario,
                    //        OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema,
                    //        InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.AutomaticamenteConfirmarDocumentos
                    //    });
                    //}
                }
            }
            cargaPedido.Carga.IntegradoraNFe = integradora;

            if (configuracaoGeralCarga.IniciarConfirmacaoDocumentosFiscaisCargaPorThread)
            {
                cargaPedido.Carga.DataSolicitacaoConfirmacaoDocumentosFiscais = DateTime.Now;
                repCarga.Atualizar(cargaPedido.Carga);
            }
            else
                SolicitarConfirmacaoDocumentosFiscais(cargaPedido, cargaPedidos, configuracao, tipoServicoMultisoftware, Auditado, unitOfWork);

            return "";
        }
        public static void SolicitarConfirmacaoDocumentosFiscais(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            bool todasNotasCargaRecebidas = repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(cargaPedido.Carga.Codigo);
            bool todasNotasOrigemRecebedidas = false;

            if (configuracao.PermiteEmitirCargaDiferentesOrigensParcialmente || (cargaPedido?.Carga?.TipoOperacao?.PermiteEmitirCargaDiferentesOrigensParcialmente ?? false))
            {
                if (cargaPedido.Expedidor != null)
                    todasNotasOrigemRecebedidas = repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCargaExpedidor(cargaPedido.Carga.Codigo, cargaPedido.Expedidor.CPF_CNPJ);
                else
                    todasNotasOrigemRecebedidas = repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCargaRemetente(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Remetente.CPF_CNPJ);
            }

            if (
                (todasNotasCargaRecebidas && (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgTransportador || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe)) ||
                (todasNotasOrigemRecebedidas && (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgTransportador || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe
                || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos))
            )
            {
                if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    if (!(cargaPedido.Carga.TipoOperacao?.NaoIntegrarOpentech ?? false) && !(cargaPedido.Carga.Veiculo?.NaoIntegrarOpentech ?? false))
                        cargaPedido.Carga.AguardarIntegracaoEtapaTransportador = AdicionarIntegracaoSM(cargaPedido.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, cargaPedido.Carga.CargaEmitidaParcialmente, unitOfWork);

                    if (!cargaPedido.Carga.CargaDePreCargaEmFechamento && !configuracao.UtilizaEmissaoMultimodal && !cargaPedido.Carga.AguardarIntegracaoEtapaTransportador)
                    {
                        cargaPedido.Carga.SituacaoCarga = SituacaoCarga.AgNFe;
                        cargaPedido.Carga.ProcessandoDocumentosFiscais = true;
                        cargaPedido.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;

                        if (cargaPedido.Carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            Servicos.Log.TratarErro("ValidarCTeGlobalizadoPorDestinatario carga protocolo " + cargaPedido.Carga.Protocolo);

                            if (!Servicos.Embarcador.Carga.CTeSimplificado.ValidarCTeSimplificado(cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracao))
                                Servicos.Embarcador.Carga.CTeGlobalizado.ValidarCTeGlobalizadoPorDestinatario(cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracao);
                        }
                    }
                }
                else
                {
                    //Servicos.Log.TratarErro("Informou DataEnvioUltimaNFe carga codigo " + cargaPedido.Carga.Codigo, "DataEnvioUltimaNFe");
                    //cargaPedido.Carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    //cargaPedido.Carga.DataEnvioUltimaNFe = DateTime.Now;
                    cargaPedido.Carga.ProcessandoDocumentosFiscais = true;
                    cargaPedido.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;

                    if (!(cargaPedido.Carga.TipoOperacao?.NaoIntegrarOpentech ?? false) && !(cargaPedido.Carga.Veiculo?.NaoIntegrarOpentech ?? false))
                        cargaPedido.Carga.AguardarIntegracaoEtapaTransportador = AdicionarIntegracaoSM(cargaPedido.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, cargaPedido.Carga.CargaEmitidaParcialmente, unitOfWork);
                }

                if (todasNotasCargaRecebidas)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repositorioCargaEntregaPedido.BuscarPorCarga(cargaPedido.Carga.Codigo);
                    if (cargaEntregaPedidos.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(cargaPedido.Carga.Codigo);
                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(cargaPedido.Carga.Codigo);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.VincularNotasNaCargaEntrega(cargaPedido.Carga, cargaEntregaPedidos, pedidosXmlNotaFiscalCarga, xmlNotaFiscalProdutos, tipoServicoMultisoftware, unitOfWork);
                    }
                }

                if (cargaPedido.Carga.CargaAgrupada)
                    cargaPedido.Carga.LocalidadeColetaLiberada = cargaPedido.Origem;

                if (cargaPedido.Carga.CargaEmitidaParcialmente)
                {
                    cargaPedido.Carga.PossuiPendencia = false;
                    cargaPedido.Carga.MotivoPendencia = "";
                    serCarga.LiberarRestanteCargaEmitidaParcialmente(cargaPedido.Carga, cargaPedido, tipoServicoMultisoftware, Auditado, unitOfWork);
                }

                if (!todasNotasCargaRecebidas && todasNotasOrigemRecebedidas)
                    cargaPedido.Carga.CargaEmitidaParcialmente = true;
                else if (cargaPedido.Carga.CargaEmitidaParcialmente)
                    cargaPedido.Carga.CargaEmitidaParcialmente = false;

                serCarga.SetarValePedagioPedidoCarga(cargaPedido.Carga, unitOfWork);

                Log.TratarErro($"Finalizou o envio das notas da carga {cargaPedido.Carga.Codigo}", "ConfirmarEnvioDosDocumentos");
            }
            else if (configuracao.IncluirCargaCanceladaProcessarDT && todasNotasCargaRecebidas && cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Nova)
            {
                //validar se todos dados transporte informados e avançar para agNfe
                bool dadosTransporteInformados = (
                         (cargaPedido.Carga.TipoDeCarga != null) &&
                         (cargaPedido.Carga.ModeloVeicularCarga != null) &&
                         (cargaPedido.Carga.Veiculo != null) &&
                         (cargaPedido.Carga.Motoristas != null && cargaPedido.Carga.Motoristas.Count > 0) &&
                         (!(cargaPedido.Carga.TipoOperacao?.ExigePlacaTracao ?? false) || ((cargaPedido.Carga.VeiculosVinculados?.Count ?? 0) == cargaPedido.Carga.ModeloVeicularCarga.NumeroReboques)));

                if (dadosTransporteInformados)
                {
                    cargaPedido.Carga.SituacaoCarga = SituacaoCarga.AgNFe;
                    cargaPedido.Carga.ProcessandoDocumentosFiscais = true;
                    cargaPedido.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                }

                Log.TratarErro($"Finalizou o envio das notas da carga {cargaPedido.Carga.Codigo}", "ConfirmarEnvioDosDocumentos");

            }
            cargaPedido.Carga.DataSolicitacaoConfirmacaoDocumentosFiscais = null;
            repositorioCarga.Atualizar(cargaPedido.Carga);
        }

        public static void IntegrarNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Documentos.ConsultaDocumento srvConsultaDocumento = new Embarcador.Documentos.ConsultaDocumento(unitOfWork, tipoServicoMultisoftware, Auditado);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Embarcador.Pedido.Pedido(unitOfWork);

            pedido.ValorTotalNotasFiscais = 0; //Zera o valor para atualizar com o valor das notas
            if (notasFiscais != null && notasFiscais.Count > 0 && !string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().NumeroTransporte))
                pedido.NumeroCarregamento = notasFiscais.FirstOrDefault().NumeroTransporte;

            repPedido.Atualizar(pedido);

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscais)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                {
                    xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscal(notaFiscal.Chave);

                    if (xmlNotaFiscal == null)
                        xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorDocumentoDestinado(notaFiscal.Chave, tipoServicoMultisoftware);
                }

                notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;

                if (notaFiscal.Emitente == null)
                    notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(pedido.Remetente);

                if (notaFiscal.Destinatario == null)
                    notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(pedido.Destinatario);

                serPedido.AdicionarNotaFiscal(pedido, notaFiscal, unitOfWork);
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<string> EnviarXMLNotaFiscal(Stream xml)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork).BuscarPrimeiroRegistro();

                string error = string.Empty;

                System.IO.StreamReader stReaderXML = new StreamReader(xml);
                stReaderXML.BaseStream.Position = 0;

                string textoXML = stReaderXML.ReadToEnd();
                int posIni = textoXML.IndexOf("Id=\"NFe");

                if (posIni <= 0)
                    throw new Exception("Arquivo sem tag da numeração da nota.");

                posIni += 7;
                int posFim = posIni + 44;
                string chave = textoXML.Substring(posIni, posFim - posIni);

                Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao arquivo = new Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao()
                {
                    NomeArquivo = Guid.NewGuid().ToString() + ".xml",
                    Situacao = SituacaoProcessamentoRegistro.Pendente,
                    Tentativas = 0,
                    DataRecebimento = DateTime.Now,
                    Mensagem = string.Empty,
                    Integradora = _auditado.Integradora,
                    IP = _auditado.IP,
                    Chave = chave
                };

                if (configuracaoArquivo == null || configuracaoArquivo?.CaminhoArquivosImportacaoXMLNotaFiscal == null)
                    throw new Exception("Caminho de importação de XML não configurado");

                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal, arquivo.NomeArquivo);

                using (Stream fs = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivo))
                {
                    xml.Seek(0, SeekOrigin.Begin);
                    xml.CopyTo(fs);
                    fs.Close();
                }

                Servicos.Log.TratarErro("Recebido XML chave: " + chave, "EnviarXMLNotaFiscal");

                new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(_unitOfWork).Inserir(arquivo, _auditado);

                return Dominio.ObjetosDeValor.WebService.Retorno<string>.CriarRetornoSucesso("XML Processado Com Sucesso", "XML Processado Com Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<string>.CriarRetornoExcecao(ex.Message);
            }

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> VincularNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave> listaNotasFiscaisChaves, Dominio.Entidades.WebService.Integradora integradora)
        {
            try
            {

                Servicos.Log.TratarErro($"VincularNotaFiscal - Protocolo Pedido {(protocolo != null ? protocolo.protocoloIntegracaoPedido.ToString() : string.Empty)}", "VincularNotaFiscal");
                Servicos.Log.TratarErro($"VincularNotaFiscal - Protocolo Carga {(protocolo != null ? protocolo.protocoloIntegracaoCarga.ToString() : string.Empty)}", "VincularNotaFiscal");
                Servicos.Log.TratarErro($"Lista {(listaNotasFiscaisChaves != null ? Newtonsoft.Json.JsonConvert.SerializeObject(listaNotasFiscaisChaves) : string.Empty)}", "VincularNotaFiscal");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidParcialxml = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (listaNotasFiscaisChaves == null || listaNotasFiscaisChaves.Count == 0)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Sem chaves disponives para fazer a vinculação");

                if (cargaPedido == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Carga não encontrada");

                Servicos.Log.TratarErro($"CargaPedido - {(cargaPedido.Codigo)} ", "VincularNotaFiscal");

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave notaChave in listaNotasFiscaisChaves)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial existeNotaParcialComNumeroFatura = repCargaPedidParcialxml.BuscarPorNumeroFatura(notaChave.NumeroFatura);

                    if (existeNotaParcialComNumeroFatura != null)
                    {
                        if (!string.IsNullOrEmpty(notaChave?.ChaveNFe))
                        {
                            if (!Utilidades.Validate.ValidarChaveNFe(notaChave.ChaveNFe))
                                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Chave nota fiscal inválida");

                            existeNotaParcialComNumeroFatura.Chave = notaChave.ChaveNFe;
                        }

                        if (notaChave.Status > 0 && notaChave.Status != existeNotaParcialComNumeroFatura.Status)
                            existeNotaParcialComNumeroFatura.Status = notaChave.Status;

                        if (!string.IsNullOrEmpty(notaChave.NumeroFatura) && !notaChave.NumeroFatura.Equals(existeNotaParcialComNumeroFatura.NumeroFatura))
                            existeNotaParcialComNumeroFatura.NumeroFatura = notaChave.NumeroFatura;

                        existeNotaParcialComNumeroFatura.TipoNotaFiscalIntegrada = notaChave.TipoNotaFiscalIntegrada;
                        existeNotaParcialComNumeroFatura.CargaPedido = cargaPedido;

                        InserirNotaCargaPedido(notaChave, ref existeNotaParcialComNumeroFatura, cargaPedido, configuracaoEmbarcador);

                        if (existeNotaParcialComNumeroFatura.Status != StatusNfe.Cancelado)
                            repCargaPedidParcialxml.Atualizar(existeNotaParcialComNumeroFatura);

                        Servicos.Log.TratarErro($"Atualizou CargaPedidoParcial: - {(existeNotaParcialComNumeroFatura.Codigo)} | Chave: {existeNotaParcialComNumeroFatura.Chave} ", "VincularNotaFiscal");

                        continue;
                    }

                    bool existeNotaIntegrada = cargaPedido.NotasFiscais.Count > 0 ? cargaPedido.NotasFiscais.Any(n => n.XMLNotaFiscal?.Chave == notaChave.ChaveNFe) : false;

                    if (existeNotaIntegrada)
                    {
                        Servicos.Log.TratarErro($"Ja existe Nota Chave: {notaChave.ChaveNFe}", "VincularNotaFiscal");
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial()
                    {
                        Status = notaChave.Status,
                        CargaPedido = cargaPedido,
                        NumeroFatura = notaChave?.NumeroFatura ?? string.Empty,
                        TipoNotaFiscalIntegrada = notaChave.TipoNotaFiscalIntegrada,
                        Chave = notaChave?.ChaveNFe ?? string.Empty,
                    };
                    repCargaPedidParcialxml.Inserir(cargaPedidoParcial);

                    InserirNotaCargaPedido(notaChave, ref cargaPedidoParcial, cargaPedido, configuracaoEmbarcador);
                    repCargaPedidParcialxml.Atualizar(cargaPedidoParcial);

                    Servicos.Log.TratarErro($"Inseriu CargaPedidoParcial: - {(cargaPedidoParcial.Codigo)} | Chave: {cargaPedidoParcial.Chave} ", "VincularNotaFiscal");
                }

                decimal pesoNaNFs = 0;
                int volumes = 0;
                bool enviouTodos = true;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscalParcials = repCargaPedidParcialxml.BuscarPorCargaPedido(cargaPedido.Codigo); //repCargaPedidParcialxml.BuscarPorCarga(protocolo.protocoloIntegracaoCarga);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial in cargaPedidoXMLNotaFiscalParcials)
                {
                    Servicos.Log.TratarErro($"Validando CargaPedidoParcial: - {(cargaPedidoXMLNotaFiscalParcial.Codigo)} ", "VincularNotaFiscal");

                    if (cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal == null && cargaPedidoXMLNotaFiscalParcial.Status == StatusNfe.Autorizado)
                    {
                        enviouTodos = false;
                        break;
                    }
                }

                if (enviouTodos && cargaPedidoXMLNotaFiscalParcials.Count > 0) //&& !repCargaPedido.ExisteCargaPedidoSemNota(protocolo.protocoloIntegracaoCarga))
                {
                    cargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;
                    repCargaPedido.Atualizar(cargaPedido);

                    pesoNaNFs = repPedidoXMLnotaFiscal.BuscarPesoPorCarga(cargaPedido.Carga.Codigo);
                    volumes = repPedidoXMLnotaFiscal.BuscarVolumesPorCarga(cargaPedido.Carga.Codigo);

                    string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFs, volumes, null, null, null, null, null, configuracaoEmbarcador, _tipoServicoMultisoftware, _auditado, integradora, _unitOfWork);
                    if (string.IsNullOrWhiteSpace(retornoFinalizacao))
                    {
                        Servicos.Log.TratarErro($" Finalizou envio ", "VincularNotaFiscal");
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido, "Integrou notas fiscais", _unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Integrou notas fiscais", _unitOfWork);
                    }
                    else
                        Servicos.Log.TratarErro("Problemas ao finalizar envio das notas. Carga: " + cargaPedido.Carga.CodigoCargaEmbarcador + " erro: " + retornoFinalizacao, "VincularNotaFiscal");
                }
                else if (!enviouTodos)
                {
                    cargaPedido.SituacaoEmissao = SituacaoNF.AgEnvioNF;
                    repCargaPedido.Atualizar(cargaPedido);

                    if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                    {
                        cargaPedido.Carga.SituacaoCarga = SituacaoCarga.AgNFe;
                        repCarga.Atualizar(cargaPedido.Carga);

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Retorno da carga para Notas Fiscais ao integrar novo registro parcial (Billing)", _unitOfWork);
                    }
                }

                Servicos.Log.TratarErro($"FIM", "VincularNotaFiscal");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "VincularNotaFiscal");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu um erro ao processar a requisição");
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarRecebimentoCargaAguardandoNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            if (protocolo.protocoloIntegracaoCarga == 0 || protocolo.protocoloIntegracaoPedido > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Os protocolos informados não existem no Multi Embarcador");

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

            if (cargaPedido != null)
            {
                if (cargaPedido.CienciaDoEnvioDaNotaInformado)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDuplicidadeRequisicao("A confirmação do recebimento da carga aguardando Notas Fiscais já foi feito.");

                cargaPedido.CienciaDoEnvioDaNotaInformado = true;
                repCargaPedido.Atualizar(cargaPedido);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido, "Confirmou recebimento de carga aguardando notas fiscais", _unitOfWork);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }

            var cargasPedidos = repCargaPedido.BuscarPorCargaOrigem(protocolo.protocoloIntegracaoCarga);

            if (cargasPedidos == null || cargasPedidos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Os protocolos informados não existem no Multi Embarcador");

            foreach (var cargaPed in cargasPedidos)
            {
                cargaPed.CienciaDoEnvioDaNotaInformado = true;
                repCargaPedido.Atualizar(cargaPed);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPed, "Confirmou recebimento de carga aguardando notas fiscais", _unitOfWork);
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasAguardandoNotasFiscais(int inicio, int limite, string codigoTipoOperacao, Dominio.Entidades.WebService.Integradora integradora)
        {
            if (limite > 100)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100");

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

            bool naoRetornarCargasCanceladas = configuracaoWebService.NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais;

            //todo: fixo para não dar problema na tirol, ver para criar variavel pois a piracanjuba reclama.
            //limite = 0;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPendetesNotasFiscais(configuracaoTMS.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, integradora.GrupoPessoas?.Codigo ?? 0, inicio, limite, codigoTipoOperacao, naoRetornarCargasCanceladas);
            int totalDeRegistros = repCargaPedido.ContarPendetesNotasFiscais(integradora.GrupoPessoas?.Codigo ?? 0, codigoTipoOperacao);
            List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> itensRetorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
            {
                Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos();
                protocolo.protocoloIntegracaoCarga = cargaPedido.CargaOrigem.Protocolo;
                protocolo.protocoloIntegracaoPedido = cargaPedido.Pedido.Protocolo;
                itensRetorno.Add(protocolo);
            }
            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou cargas aguardando notas fiscais", _unitOfWork);

            Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>()
            {
                Itens = itensRetorno,
                NumeroTotalDeRegistro = totalDeRegistros
            };

            return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarCancelamentoNotaFiscal(int protocoloNFe)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork);

                _unitOfWork.Start();
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(protocoloNFe);

                if (xmlNotaFiscal == null)
                    throw new WebServiceException("O protocolo da nota informada não existe na base da Multisoftware.");

                if (!xmlNotaFiscal.nfAtiva)
                    throw new WebServiceException("Já foi informado o cancelamento desta nota fiscal.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);
                if (cargaPedidoXMLNotaFiscal.Count > 0)
                    throw new WebServiceException("Não é possível solicitar o cancelamento desta nota fiscal pois a mesma está vinculada a carga (" + cargaPedidoXMLNotaFiscal.FirstOrDefault().CargaPedido.Carga.CodigoCargaEmbarcador + "), para cancelar desta nota fiscal é necesário solicitar o cancelamento da Carga.");

                xmlNotaFiscal.nfAtiva = false;
                serCanhoto.ExcluirCanhotoDaNotaFiscal(xmlNotaFiscal, _unitOfWork);

                repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                if (xmlNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet && _tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    new Servicos.Embarcador.Pallets.DevolucaoPallets(_unitOfWork).CancelarPallets(xmlNotaFiscal);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, xmlNotaFiscal, "Informou cancelamento de nota fiscal", _unitOfWork);
                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                _unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                {
                    AuditarRetornoDuplicidadeDaRequisicao(_unitOfWork, excecao.Message, _auditado, protocoloNFe.ToString());
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao(excecao.Message);
                }

                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao tentar informar o cancelamento da nota fiscal.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarEtapaNFe(Dominio.ObjetosDeValor.Embarcador.NFe.ConfirmarEtapaNFe confirmarEtapaNFe)
        {
            try
            {
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(confirmarEtapaNFe.CodigoCarga);

                if (carga == null && confirmarEtapaNFe.CodigoEntrega > 0)
                    carga = repositorioCargaEntrega.BuscarPorCodigo(confirmarEtapaNFe.CodigoEntrega)?.Carga;

                if (carga == null)
                    throw new WebServiceException("Carga não encontrada");

                List<ProdutoDivergente> produtosDivergentes = servicoCarga.VerificarDivergenciaProdutosEsperadoVSRecebidos(carga.Codigo);

                if (produtosDivergentes.Count > 0)
                    throw new WebServiceException($"Foram identificados produtos divergentes ({produtosDivergentes.Select(o => o.Produto)})");

                List<PermissaoPersonalizada> permissoesPersonalizadas = new List<PermissaoPersonalizada>
                {
                    PermissaoPersonalizada.Carga_InformarDocumentosFiscais
                };

                carga.AvancouCargaEtapaDocumentoLote = false;

                servicoCarga.ConfirmarEnvioDosDocumentos(
                    carga,
                    confirmarEtapaNFe.NaoValidarApolice,
                    false,
                    _tipoServicoMultisoftware,
                    permissoesPersonalizadas,
                    _auditado,
                    _webServiceConsultaCTe,
                    _usuario,
                    _unitOfWork
                );

                repositorioCarga.Atualizar(carga);

                if (_auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Confirmou o envio automático dos documentos.", _unitOfWork);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Confirmou o envio automático dos documentos.");
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao tentar confirmar nota fiscal.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }


        #endregion Métodos Públicos

        #region Métodos Privados

        private void SalvarDadosNotaProcessar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.Entidades.WebService.Integradora integradora, string request, string response)
        {
            Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repProcessamentoNotaFiscalAssincrono = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(_unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono processamentoNotaFiscalAssincrono = new Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono
            {
                Integradora = integradora,
                Sucesso = true,
                Mensagem = "Recebido com sucesso",
                DataRecebimento = DateTime.Now,
                ArquivoRequisicao = !string.IsNullOrWhiteSpace(request) ? ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", _unitOfWork) : null,
                ArquivoResposta = !string.IsNullOrWhiteSpace(response) ? ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", _unitOfWork) : null,
                Carga = carga,
                Situacao = SituacaoIntegracao.AgIntegracao,
                ProtocoloPedido = protocolo.protocoloIntegracaoPedido
            };

            repProcessamentoNotaFiscalAssincrono.Inserir(processamentoNotaFiscalAssincrono);
        }

        private static string VincularValePedagioPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.MDFe.ValePedagio valePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            if (valePedagio != null)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoValePedagio repPedidoValePedagio = new Repositorio.Embarcador.Pedidos.PedidoValePedagio(unitOfWork);

                if (string.IsNullOrWhiteSpace(valePedagio.CNPJResponsavel) || !Utilidades.Validate.ValidarCNPJ(valePedagio.CNPJResponsavel))
                    return "CNPJ do Responsável pelo vale pedágio inválido (" + valePedagio.CNPJResponsavel + ")";
                if (string.IsNullOrWhiteSpace(valePedagio.CNPJFornecedor) || !Utilidades.Validate.ValidarCNPJ(valePedagio.CNPJFornecedor))
                    return "CNPJ do Fornecedor do vale pedágio inválido (" + valePedagio.CNPJFornecedor + ")";
                if (string.IsNullOrWhiteSpace(valePedagio.NumeroComprovante) || valePedagio.NumeroComprovante.Length > 20)
                    return "Numero comprovante vale pedágio inválido (" + valePedagio.NumeroComprovante + ")";

                Dominio.Entidades.Cliente fornecedorValePedagio = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(valePedagio.CNPJFornecedor)));
                if (fornecedorValePedagio == null)
                    return "Fornecedor vale pedágio (" + valePedagio.CNPJFornecedor + ") não cadastrado no Embarcador.";

                Dominio.Entidades.Cliente responsavelValePedagio = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(valePedagio.CNPJResponsavel)));
                if (responsavelValePedagio == null)
                    return "Respnsável vale pedágio (" + valePedagio.CNPJResponsavel + ") não cadastrado no Embarcador.";

                if (repPedidoValePedagio.BuscarPorPedidoEComprovante(pedido.Codigo, valePedagio.NumeroComprovante) == null) //Se já existe vale pedágio com este número não inserie outro igual
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio pedidoValePedagio = new Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio();
                    pedidoValePedagio.Pedido = pedido;
                    pedidoValePedagio.CNPJResponsavel = Utilidades.String.OnlyNumbers(valePedagio.CNPJResponsavel);
                    pedidoValePedagio.CNPJFornecedor = Utilidades.String.OnlyNumbers(valePedagio.CNPJFornecedor);
                    pedidoValePedagio.NumeroComprovante = valePedagio.NumeroComprovante;
                    pedidoValePedagio.Valor = valePedagio.ValorValePedagio;
                    repPedidoValePedagio.Inserir(pedidoValePedagio);
                }

                return string.Empty;
            }
            else return string.Empty;
        }

        private static string VincularCIOTPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.MDFe.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            if (ciot != null)
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                if (string.IsNullOrWhiteSpace(ciot.CNPJCPFResponsavel) || (!Utilidades.Validate.ValidarCNPJ(ciot.CNPJCPFResponsavel) && !Utilidades.Validate.ValidarCPF(ciot.CNPJCPFResponsavel)))
                    return "CPF/CNPJ do Responsável pelo CIOT inválido (" + ciot.CNPJCPFResponsavel + ")";

                pedido.CIOT = ciot.Numero;
                pedido.ResponsavelCIOT = Utilidades.String.OnlyNumbers(ciot.CNPJCPFResponsavel);
                repPedido.Atualizar(pedido);

                return string.Empty;
            }
            else return string.Empty;
        }


        public static string VincularInformacoesPagamentoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido informacoesPagamentoPedido, Dominio.ObjetosDeValor.MDFe.CIOT dadosCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            if (informacoesPagamentoPedido == null)
                return string.Empty;

            if (informacoesPagamentoPedido.ValorAdiantamento >= informacoesPagamentoPedido.ValorFrete)
                return "O valor do adiantamento não pode ser maior ou igual ao valor do frete.";

            if (!Enum.IsDefined(typeof(TipoPagamentoMDFe), informacoesPagamentoPedido.TipoInformacaoBancaria))
                return "Para integrar o CIOT é necessário informar uma forma de pagamento válida.";

            switch (informacoesPagamentoPedido.TipoInformacaoBancaria)
            {
                case TipoPagamentoMDFe.PIX:
                    if (string.IsNullOrWhiteSpace(informacoesPagamentoPedido.ChavePix))
                        return "O campo 'ChavePIX' não pode ser vazio quando a forma de pagamento for igual a {1 - PIX}.";
                    break;

                case TipoPagamentoMDFe.Banco:
                    if (string.IsNullOrEmpty(informacoesPagamentoPedido.Banco))
                        return "O campo 'Banco' não pode ser vazio quando a forma de pagamento for igual a {2 - Banco}.";

                    if (string.IsNullOrEmpty(informacoesPagamentoPedido.Agencia))
                        return "O campo 'Agencia' não pode ser vazio quando a forma de pagamento for igual a {2 - Banco}.";

                    if (informacoesPagamentoPedido.Banco.Length > 5)
                        return "O campo 'Banco' não pode ter mais que cinco caracteres.";
                    break;

                case TipoPagamentoMDFe.Ipef:
                    if (string.IsNullOrWhiteSpace(informacoesPagamentoPedido.Ipef))
                        return "O campo 'CNPJInstituicaoPagamentoCIOT' não pode ser vazio quando a forma de pagamento for igual a {3 - Ipef}.";

                    if (informacoesPagamentoPedido.Ipef.ObterSomenteNumeros().Length != 14)
                        return "O 'CNPJInstituicaoPagamentoCIOT' está inválido, precisa conter 14 números sem máscara.";
                    break;
            }

            Repositorio.Embarcador.Pedidos.PedidoInformacoesBancarias repositorioPedidoInformacoesBancarias = new Repositorio.Embarcador.Pedidos.PedidoInformacoesBancarias(unitOfWork);
            Dominio.Entidades.Global.PedidoInformacoesBancarias informacoesBancarias = repositorioPedidoInformacoesBancarias.BuscarPorCodigo(pedido.Codigo, false);

            bool atualizar = true;

            if (informacoesBancarias == null)
            {
                informacoesBancarias = new Dominio.Entidades.Global.PedidoInformacoesBancarias();
                atualizar = false;
            }

            informacoesBancarias.Pedido = pedido;
            informacoesBancarias.ChavePIX = informacoesPagamentoPedido.TipoInformacaoBancaria == TipoPagamentoMDFe.PIX ? informacoesPagamentoPedido.ChavePix : null;
            informacoesBancarias.Conta = informacoesPagamentoPedido.TipoInformacaoBancaria == TipoPagamentoMDFe.Banco ? informacoesPagamentoPedido.Banco : null;
            informacoesBancarias.Agencia = informacoesPagamentoPedido.TipoInformacaoBancaria == TipoPagamentoMDFe.Banco ? informacoesPagamentoPedido.Agencia : null;
            informacoesBancarias.Ipef = informacoesPagamentoPedido.Ipef;
            informacoesBancarias.TipoInformacaoBancaria = informacoesPagamentoPedido.TipoInformacaoBancaria;
            informacoesBancarias.TipoPagamento = informacoesPagamentoPedido.IndicadorPagamento;
            informacoesBancarias.ValorAdiantamento = informacoesPagamentoPedido.ValorAdiantamento;
            informacoesBancarias.IndicadorAltoDesempenho = informacoesPagamentoPedido.IndicadorAltoDesempenho;
            informacoesBancarias.ValorFrete = informacoesPagamentoPedido.ValorFrete;
            informacoesBancarias.DataVencimentoCIOT = informacoesPagamentoPedido.DataVencimentoCIOT;

            if (atualizar)
                repositorioPedidoInformacoesBancarias.Atualizar(informacoesBancarias);
            else
                repositorioPedidoInformacoesBancarias.Inserir(informacoesBancarias);

            return string.Empty;
        }

        private static void AjustarModalidadePagamentosCargaPedido(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaPedido.Pedido.UsarTipoPagamentoNF)
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);
                if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
                {
                    cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;

                    repPedido.Atualizar(cargaPedido.Pedido);

                    if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                    else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                        cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                }


            }
        }

        private static string VincularAverbacoesPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao averbacao, List<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> averbacoesNF, Repositorio.UnitOfWork unitOfWork)
        {
            if (averbacao != null)
            {
                if (string.IsNullOrWhiteSpace(averbacao.CNPJResponsavel) || !Utilidades.Validate.ValidarCNPJ(averbacao.CNPJResponsavel))
                    return "CNPJ do Responsável pela Averbação inválido (" + averbacao.CNPJResponsavel + ")";
                if (string.IsNullOrWhiteSpace(averbacao.CNPJSeguradora) || !Utilidades.Validate.ValidarCNPJ(averbacao.CNPJSeguradora))
                    return "CNPJ da Seguradora inválido (" + averbacao.CNPJSeguradora + ")";
                if (string.IsNullOrWhiteSpace(averbacao.NomeSeguradora) || averbacao.NomeSeguradora.Length > 30)
                    return "Nome da Seguradora inválido (" + averbacao.NomeSeguradora + ")";
                if (string.IsNullOrWhiteSpace(averbacao.NumeroApolice) || averbacao.NumeroApolice.Length > 20)
                    return "Número da apólice de seguro inválido (" + averbacao.NumeroApolice + ")";
                if (string.IsNullOrWhiteSpace(averbacao.NumeroAverbacao) || averbacao.NumeroAverbacao.Length > 40)
                    return "Número da averbação de seguro inválido (" + averbacao.NumeroAverbacao + ")";

                Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao pedidoAverbacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao();
                pedidoAverbacao.Pedido = pedido;
                pedidoAverbacao.CNPJResponsavel = Utilidades.String.OnlyNumbers(averbacao.CNPJResponsavel);
                pedidoAverbacao.CNPJSeguradora = Utilidades.String.OnlyNumbers(averbacao.CNPJSeguradora);
                pedidoAverbacao.NomeSeguradora = averbacao.NomeSeguradora;
                pedidoAverbacao.NumeroApolice = averbacao.NumeroApolice;
                pedidoAverbacao.NumeroAverbacao = averbacao.NumeroAverbacao;
                repPedidoAverbacao.Inserir(pedidoAverbacao);

                return string.Empty;
            }
            else if (averbacoesNF != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao averbacaoNF in averbacoesNF)
                {
                    if (string.IsNullOrWhiteSpace(averbacaoNF.CNPJResponsavel) || !Utilidades.Validate.ValidarCNPJ(averbacaoNF.CNPJResponsavel))
                        return "CNPJ do Responsável pela Averbação inválido (" + averbacaoNF.CNPJResponsavel + ")";
                    if (string.IsNullOrWhiteSpace(averbacaoNF.CNPJSeguradora) || !Utilidades.Validate.ValidarCNPJ(averbacaoNF.CNPJSeguradora))
                        return "CNPJ da Seguradora inválido (" + averbacaoNF.CNPJSeguradora + ")";
                    if (string.IsNullOrWhiteSpace(averbacaoNF.NomeSeguradora) || averbacaoNF.NomeSeguradora.Length > 30)
                        return "Nome da Seguradora inválido (" + averbacaoNF.NomeSeguradora + ")";
                    if (string.IsNullOrWhiteSpace(averbacaoNF.NumeroApolice) || averbacaoNF.NumeroApolice.Length > 20)
                        return "Número da apólice de seguro inválido (" + averbacaoNF.NumeroApolice + ")";
                    if (string.IsNullOrWhiteSpace(averbacaoNF.NumeroAverbacao) || averbacaoNF.NumeroAverbacao.Length > 40)
                        return "Número da averbação de seguro inválido (" + averbacaoNF.NumeroAverbacao + ")";

                    Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao pedidoAverbacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao();
                    pedidoAverbacao.Pedido = pedido;
                    pedidoAverbacao.CNPJResponsavel = Utilidades.String.OnlyNumbers(averbacaoNF.CNPJResponsavel);
                    pedidoAverbacao.CNPJSeguradora = Utilidades.String.OnlyNumbers(averbacaoNF.CNPJSeguradora);
                    pedidoAverbacao.NomeSeguradora = averbacaoNF.NomeSeguradora;
                    pedidoAverbacao.NumeroApolice = averbacaoNF.NumeroApolice;
                    pedidoAverbacao.NumeroAverbacao = averbacaoNF.NumeroAverbacao;
                    repPedidoAverbacao.Inserir(pedidoAverbacao);
                }

                return string.Empty;
            }
            else
                return string.Empty;
        }

        public static bool AdicionarIntegracaoSM(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, bool naoAvancarRejeicaoIntegracaoTranspoortador, bool gerarNovaIntegracaoSeJaIntegrado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipo, true);

            if (tipoIntegracao == null)
                return false;

            if (!carga.ExigeNotaFiscalParaCalcularFrete)
            {

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(carga, tipoIntegracao, unitOfWork, gerarNovaIntegracaoSeJaIntegrado, false);

                return (naoAvancarRejeicaoIntegracaoTranspoortador && cargaIntegracao != null && cargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(carga, tipoIntegracao, unitOfWork, gerarNovaIntegracaoSeJaIntegrado, false);

                return (naoAvancarRejeicaoIntegracaoTranspoortador && cargaDadosTransporteIntegracao != null && cargaDadosTransporteIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
            }

        }

        private void VincularNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.Entidades.WebService.Integradora integradora, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.ObjetosDeValor.MDFe.CIOT Ciot, Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido informacoesPagamentoPedido, ref List<string> caminhosXMLTemp, ref Retorno<bool> retorno, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Pedido.Produto serProduto = new Servicos.Embarcador.Pedido.Produto(_unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal servicoNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_unitOfWork);

            try
            {
                if (TokensXMLNotasFiscais == null || TokensXMLNotasFiscais.Count == 0)
                {
                    retorno.Status = false;
                    retorno.Mensagem = "A tag TokensXMLNotasFiscais não possui nenhum valor informado.";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    unitOfWork.Rollback();
                    return;
                }
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargaPedido == null && protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                    cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);

                if (cargaPedido != null)
                {
                    if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada &&
                       (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                       (!configuracao.NaoAceitarNotasNaEtapa1 && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova) ||
                       (cargaPedido.Carga.CargaEmitidaParcialmente && (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos))))
                    {
                        Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                        bool tudoCerto = true;

                        decimal valorTotalNFs = 0;
                        decimal pesoNaNFs = 0;
                        int volumes = 0;

                        if (cargaPedido.CienciaDoEnvioDaNotaInformado || !configuracao.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos)//todo: rever essa validação.
                        {
                            string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao ?? configuracaoArquivo?.CaminhoArquivosIntegracao;

                            if (TokensXMLNotasFiscais.Count > 0)
                            {
                                if (cargaPedido.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                                {
                                    if (!configuracao?.RetornosDuplicidadeWSSubstituirPorSucesso ?? false)
                                    {
                                        tudoCerto = false;
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                        retorno.Mensagem += "As notas fiscais já foram enviadas para esse pedido. ";
                                    }
                                    else
                                    {
                                        tudoCerto = false;

                                        retorno.Objeto = true;
                                        retorno.Status = true;
                                        retorno.Mensagem = "As notas fiscais já foram enviadas para esse pedido. ";
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;

                                        unitOfWork.Rollback();

                                        return;
                                    }
                                }
                            }

                            if (tudoCerto)
                            {
                                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

                                PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new PedidoXMLNotaFiscal(unitOfWork, configuracao, configuracaoGeralCarga);
                                string NumerosNF = "";
                                List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();

                                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF tokenXML in TokensXMLNotasFiscais)
                                {
                                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(tokenXML.Token, ".xml"));

                                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                    {
                                        System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));

                                        Servicos.NFe svcNFe = new Servicos.NFe(_unitOfWork);

                                        Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

                                        try
                                        {
                                            nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork, false);
                                            tudoCerto = true;
                                        }
                                        catch (Exception e)
                                        {
                                            if (e.Message.Contains("é um símbolo inesperado") || e.Message.Contains("Linha 1, posição "))
                                                retorno.Mensagem = e.Message + " Decodifique o Base64 enviado e faça o ajuste no XML.";
                                            else
                                                retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";

                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                                            tudoCerto = false;
                                            Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada: " + caminho + " " + e.Message);

                                            reader.Dispose();
                                        }

                                        if (tudoCerto)
                                        {
                                            if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, _tipoServicoMultisoftware, configuracao.ImportarEmailCliente, configuracao.UtilizarValorFreteNota, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                                            {
                                                tudoCerto = false;
                                                retorno.Mensagem += erro;
                                            }

                                            reader.Dispose();

                                            if (tudoCerto)
                                            {
                                                if (xmlNotaFiscal.Destinatario.Tipo == "E")
                                                {
                                                    if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida)
                                                    {
                                                        xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;
                                                    }
                                                    else
                                                    {
                                                        xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Remetente;
                                                    }
                                                }

                                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFisca.BuscarPorChave(xmlNotaFiscal.Chave);
                                                if (xmlNotaFiscalExiste == null)
                                                {
                                                    xmlNotaFiscal.TipoNotaFiscalIntegrada = tokenXML.TipoNotaFiscalIntegrada;
                                                    xmlNotaFiscal.ClassificacaoNFe = tokenXML.ClassificacaoNFe;
                                                    xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;
                                                    repXMLNotaFisca.Inserir(xmlNotaFiscal);

                                                    if (tokenXML.ClassificacaoNFe.HasValue)
                                                        cargaPedido.IndicadorRemessaVenda = (tokenXML.ClassificacaoNFe == ClassificacaoNFe.Remessa || tokenXML.ClassificacaoNFe == ClassificacaoNFe.Venda);
                                                }
                                                else
                                                {
                                                    xmlNotaFiscal = xmlNotaFiscalExiste;

                                                    if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Chave))
                                                    {
                                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorChaveAtivaCarga(Utilidades.String.OnlyNumbers(xmlNotaFiscal.Chave), cargaPedido.Carga.Codigo);
                                                        if (pedidosExistente?.CargaPedido?.Pedido?.Codigo == cargaPedido.Pedido.Codigo)
                                                        {
                                                            tudoCerto = false;
                                                            retorno.Mensagem = "A nota fiscal " + xmlNotaFiscal.Chave + " já foi enviada para o mesmo pedido nesta mesma carga";
                                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                                        }
                                                        else
                                                            tudoCerto = true;
                                                    }

                                                    if (!configuracao.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                                                    {
                                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosExistente = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);
                                                        if (pedidosExistente.Any(obj => obj.CargaPedido.Carga.Codigo == cargaPedido.Carga.Codigo))
                                                        {
                                                            tudoCerto = false;
                                                            retorno.Mensagem = "A nota fiscal " + xmlNotaFiscal.Chave + " já foi enviada para outro pedido nesta mesma carga";
                                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                                        }
                                                        else
                                                            tudoCerto = true;
                                                    }
                                                }

                                                if (tudoCerto)
                                                {
                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                                                    pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                                                    pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                                                    pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                                                    repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                                                    if (tokenXML.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet)
                                                    {
                                                        repXMLNotaFisca.Atualizar(xmlNotaFiscal);
                                                        List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtosPellet = nfXml.Produtos;
                                                        if (produtosPellet != null)
                                                        {
                                                            decimal quantidadePallets = produtosPellet.Sum(obj => obj.QuantidadeComercial);
                                                            new Servicos.Embarcador.Pallets.DevolucaoPallets(unitOfWork).AdicionarPallets(cargaPedido, xmlNotaFiscal, cargaPedido.Carga.Filial, cargaPedido.Carga.Empresa, (int)quantidadePallets, _tipoServicoMultisoftware);
                                                            new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, _auditado).AdicionarMovimentacaoPalletAutomatico(xmlNotaFiscal, cargaPedido, (int)quantidadePallets);
                                                            if (!configuracao.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso)
                                                                cargaPedido.Pedido.NumeroPaletes = (int)quantidadePallets;
                                                            cargaPedido.Pedido.PesoTotalPaletes = xmlNotaFiscal.Peso;
                                                            cargaPedido.Pedido.ValorTotalPaletes = (xmlNotaFiscal.Valor - xmlNotaFiscal.ValorST);
                                                            repPedido.Atualizar(cargaPedido.Pedido);
                                                        }

                                                        if (configuracaoCanhoto.GerarCanhotoParaNotasTipoPallet)
                                                            serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas.ToList(), _tipoServicoMultisoftware, configuracao, unitOfWork, configuracaoCanhoto);
                                                    }
                                                    else
                                                    {
                                                        serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas.ToList(), _tipoServicoMultisoftware, configuracao, unitOfWork, configuracaoCanhoto);
                                                        if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete || configuracao.AtualizarProdutosPedidoPorIntegracao || (cargaPedido.Carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false))
                                                            produtos.AddRange(nfXml.Produtos);
                                                    }


                                                    if (cargaPedido.Carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false)
                                                        servicoPedidoXMLNotaFiscal.ArmazenarProdutosXML(xmlNotaFiscal.XML, xmlNotaFiscal, _auditado, _tipoServicoMultisoftware);


                                                    NumerosNF += ", " + xmlNotaFiscal.Numero.ToString();

                                                    pesoNaNFs += xmlNotaFiscal.Peso;
                                                    valorTotalNFs += (xmlNotaFiscal.Valor - xmlNotaFiscal.ValorST);
                                                    volumes += xmlNotaFiscal.Volumes;

                                                    servicoNotaFiscal.ValidarTransportadorDivergente(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware);

                                                    //if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal) && !cargaPedido.Redespacho && !cargaPedido.Carga.NaoExigeVeiculoParaEmissao)
                                                    //{

                                                    //    if ((string)xmlNotaFiscal.PlacaVeiculoNotaFiscal.ToUpper() != cargaPedido.Carga.Veiculo.Placa.ToUpper() && !cargaPedido.Redespacho)
                                                    //    {
                                                    //        if (cargaPedido.Carga.VeiculosVinculados == null || !cargaPedido.Carga.VeiculosVinculados.Any(obj => obj.Placa.ToUpper() == (string)xmlNotaFiscal.PlacaVeiculoNotaFiscal.ToUpper()))
                                                    //        {
                                                    //            tudoCerto = false;
                                                    //            retorno.Mensagem += "(nf: " + xmlNotaFiscal.Chave + ") A placa do veículo na nota (" + xmlNotaFiscal.PlacaVeiculoNotaFiscal + ") é diferente da placa informada na carga (" + cargaPedido.Carga.Veiculo.Placa + ")";
                                                    //        }
                                                    //    }
                                                    //}


                                                    if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida)
                                                    {
                                                        if (xmlNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.Faturamento && xmlNotaFiscal.Destinatario.CPF_CNPJ_Formatado != cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado)
                                                        {
                                                            tudoCerto = false;
                                                            retorno.Mensagem += "(nf: " + xmlNotaFiscal.Chave + ") O destinatário na nota nota (" + xmlNotaFiscal.Destinatario.CPF_CNPJ_Formatado + ") é diferente do informado na pedido (" + cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado + ")";
                                                        }

                                                        if (xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Emitente.CPF_CNPJ != cargaPedido.Pedido.Remetente.CPF_CNPJ && (xmlNotaFiscal.Emitente.GrupoPessoas?.Codigo != cargaPedido.Pedido.Remetente.GrupoPessoas?.Codigo || (xmlNotaFiscal.Emitente.GrupoPessoas?.ValidaEmitenteNFe == true)))
                                                        {
                                                            string cnpjNota = xmlNotaFiscal.Emitente != null ? xmlNotaFiscal.Emitente.CPF_CNPJ_Formatado : "Nulo";
                                                            tudoCerto = false;
                                                            retorno.Mensagem += "(nf: " + xmlNotaFiscal.Chave + ") O emitente na nota nota (" + cnpjNota + ") é diferente do informado na pedido (" + cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado + ")";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //if (xmlNotaFiscal.Destinatario.CPF_CNPJ_Formatado != cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado)
                                                        //{
                                                        //    tudoCerto = false;
                                                        //    retorno.Mensagem += "(nf: " + xmlNotaFiscal.Chave + "/ Nota de Entrada) O destinatário na nota nota (" + xmlNotaFiscal.Destinatario.CPF_CNPJ_Formatado + ") é diferente do informado na pedido (" + cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado + ")";
                                                        //}
                                                        //if (xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Emitente.CPF_CNPJ != cargaPedido.Pedido.Destinatario.CPF_CNPJ)
                                                        //{
                                                        //    tudoCerto = false;
                                                        //    retorno.Mensagem += "(nf: " + xmlNotaFiscal.Chave + "/ Nota de Entrada) O emitente na nota nota (" + xmlNotaFiscal.Emitente != null ? xmlNotaFiscal.Emitente.CPF_CNPJ.ToString() : "Nulo" + ") é diferente do informado na pedido (" + cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado + ")";
                                                        //}
                                                    }
                                                }
                                            }
                                        }

                                        caminhosXMLTemp.Add(caminho);
                                    }
                                    else
                                    {
                                        tudoCerto = false;
                                        retorno.Mensagem += "O Token informado não existe mais físicamente no servidor, por favor, envie o XML da nota novamente e receba um novo token";
                                    }
                                }

                                if ((cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete || configuracao.AtualizarProdutosPedidoPorIntegracao || (cargaPedido.Carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false)) && produtos.Count > 0 && tudoCerto)
                                    serProduto.AtualizarProdutosCargaPedidoPorNotaFiscal(produtos, cargaPedido, unitOfWork, _auditado);
                            }
                        }
                        else
                        {
                            tudoCerto = false;
                            retorno.Mensagem += "Antes de enviar as notas fiscais é necessário confirmar que está ciente que a Multisoftware está aguardando as notas fiscais para esse pedido.";
                        }

                        if (tudoCerto)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> averbacoesNF = null;
                            if (TokensXMLNotasFiscais.Any(o => o.AverbacaoNF != null))
                            {
                                averbacoesNF = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao>();
                                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF tokenNF in TokensXMLNotasFiscais)
                                {
                                    if (tokenNF.AverbacaoNF != null)
                                        averbacoesNF.Add(tokenNF.AverbacaoNF);
                                }
                            }


                            string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFs, volumes, averbacoesNF, Averbacao, ValePedagio, Ciot, informacoesPagamentoPedido, configuracao, _tipoServicoMultisoftware, _auditado, integradora, unitOfWork);
                            if (string.IsNullOrWhiteSpace(retornoFinalizacao))
                            {
                                retorno.Objeto = true;
                                retorno.Status = true;
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido, "Integrou notas fiscais", unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Integrou notas fiscais", unitOfWork);
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                unitOfWork.CommitChanges();
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem += retornoFinalizacao;
                                unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            unitOfWork.Rollback();
                        }
                    }
                    else
                    {
                        if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                            || (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete))
                        {
                            unitOfWork.Rollback();
                            retorno.Status = false;

                            if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                                retorno.CodigoMensagem = 313;
                            else if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                    || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                    || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                    || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
                                retorno.CodigoMensagem = 314;
                            else
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                            retorno.Mensagem += "Não é possível enviar as notas fiscais para a carga em sua atual situação (" + cargaPedido.Carga.DescricaoSituacaoCarga + "). ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                        else
                        {
                            unitOfWork.Rollback();

                            if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && cargaPedido.Carga.CargaEmitidaParcialmente)
                            {
                                retorno.Mensagem += "Para enviar as demais notas a carga precisa estar na situação em transporte.";
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Status = false;
                            }
                            else
                            {
                                retorno.Mensagem += "As notas fiscais já foram enviadas para esse pedido.";
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;

                                if (!configuracao?.RetornosDuplicidadeWSSubstituirPorSucesso ?? false)
                                    retorno.Status = false;
                                else
                                {
                                    retorno.Objeto = true;
                                    retorno.Status = true;
                                }
                            }
                        }
                    }
                }
                else if (protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocolo.protocoloIntegracaoPedido);
                    if (pedido != null)
                    {
                        Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);

                        string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                        bool tudoCerto = false;

                        List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
                        foreach (Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF tokenXML in TokensXMLNotasFiscais)
                        {
                            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(tokenXML.Token, ".xml"));

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            {
                                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));

                                Servicos.NFe svcNFe = new Servicos.NFe(_unitOfWork);

                                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

                                try
                                {
                                    nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork, false);
                                    tudoCerto = true;
                                }
                                catch (Exception e)
                                {
                                    retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";
                                    retorno.Status = false;
                                    tudoCerto = false;
                                    Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada: " + caminho + " " + e.Message);

                                    reader.Dispose();
                                }

                                if (tudoCerto)
                                {
                                    if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, _tipoServicoMultisoftware, configuracao.ImportarEmailCliente, configuracao.UtilizarValorFreteNota, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                                    {
                                        retorno.Mensagem = erro;
                                        retorno.Status = false;
                                        tudoCerto = false;
                                    }

                                    reader.Dispose();

                                    if (tudoCerto)
                                    {
                                        Servicos.Embarcador.Pedido.NotaFiscal serNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                                        string validacaoNota = serNotaFiscal.ValidarRegrasNotaVinculoPedido(xmlNotaFiscal, pedido);

                                        if (!string.IsNullOrWhiteSpace(validacaoNota))
                                        {
                                            Servicos.Log.TratarErro("NFe " + xmlNotaFiscal.Chave + " não vinculada ao pedido protocolo " + pedido.Protocolo.ToString() + ": " + validacaoNota);
                                            unitOfWork.Rollback();
                                            retorno.Status = false;
                                            retorno.Mensagem = validacaoNota;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        }
                                        else
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFisca.BuscarPorChave(xmlNotaFiscal.Chave);
                                            bool inseriuNota = false;
                                            if (xmlNotaFiscalExiste == null)
                                            {
                                                inseriuNota = true;
                                                xmlNotaFiscal.TipoNotaFiscalIntegrada = tokenXML.TipoNotaFiscalIntegrada;
                                                xmlNotaFiscal.ClassificacaoNFe = tokenXML.ClassificacaoNFe;
                                                xmlNotaFiscal.Filial = pedido.Filial;

                                                repXMLNotaFisca.Inserir(xmlNotaFiscal);
                                            }
                                            else
                                                xmlNotaFiscal = xmlNotaFiscalExiste;

                                            if (pedido.NotasFiscais != null && pedido.NotasFiscais.Count > 0 && inseriuNota)
                                                pedido.ValorTotalNotasFiscais += xmlNotaFiscal.Valor;
                                            else
                                                pedido.ValorTotalNotasFiscais = xmlNotaFiscal.Valor;

                                            pedido.NotasFiscais.Add(xmlNotaFiscal);
                                            repPedido.Atualizar(pedido);

                                            retorno.Objeto = true;
                                            retorno.Status = true;
                                            Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, "Integrou notas fiscais", unitOfWork);
                                            unitOfWork.CommitChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    unitOfWork.Rollback();
                                }

                                caminhosXMLTemp.Add(caminho);
                            }
                            else
                            {
                                tudoCerto = false;
                                retorno.Mensagem += "O Token informado não existe mais físicamente no servidor, por favor, envie o XML da nota novamente e receba um novo token";
                            }
                        }

                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.Status = false;
                        retorno.Mensagem = "Protocolos informados são inválidos.";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.Mensagem = "Protocolos informados são inválidos. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    //string objetoJson = Newtonsoft.Json.JsonConvert.SerializeObject(TokensXMLNotasFiscais);
                    //ArmazenarLogParametros(objetoJson);
                }
            }
            catch (BaseException excecao)
            {
                retorno.Mensagem = excecao.Message;
                retorno.Status = false;
                retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;

                unitOfWork.Rollback();
            }
        }

        private void InserirNotaCargaPedido(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave notaChave, ref Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoParcial, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedidos = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.CFOP repositorioCFOP = new Repositorio.CFOP(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidParcialxml = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXMLNotaFiscal.BuscarPorChave(notaChave?.ChaveNFe ?? string.Empty);

            if (notaFiscal == null)
                return;

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_unitOfWork);
            bool alteradoTipoCarga = false;

            notaFiscal.TipoNotaFiscalIntegrada = cargaPedidoParcial.TipoNotaFiscalIntegrada;

            repXMLNotaFiscal.Atualizar(notaFiscal);

            if (cargaPedidoParcial.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe.Cancelado)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedico = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repositorioPedidos.BuscarPorNotaFiscal(notaFiscal.Codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> existemCargaEntregaNotaFiscal = repositorioCargaEntregaNotaFiscal.BuscarPorCodigoNotaFiscal(notaFiscal.Codigo);

                if (existemCargaEntregaNotaFiscal != null && existemCargaEntregaNotaFiscal.Count > 0)
                    foreach (var cargaEntregaNotaFiscal in existemCargaEntregaNotaFiscal)
                        repositorioCargaEntregaNotaFiscal.Deletar(cargaEntregaNotaFiscal);

                string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.ExcluirNotaFiscal(notaFiscal, cargaPedido, _auditado, _tipoServicoMultisoftware, _unitOfWork, true, true);

                if (!string.IsNullOrEmpty(retornoIntegracao))
                    throw new ServicoException(retornoIntegracao);

                foreach (var pedido in listaPedidos)
                {
                    pedido.NotasFiscais.Remove(notaFiscal);
                    servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.FaturamentoCancelado, pedido, configuracao, _clienteMultisoftware);
                }

                bool existNotaParcial = repCargaPedidParcialxml.VerificarSeExisteNotaParcialSemNotaParaCargaPedido(cargaPedido.Codigo);

                if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe && existNotaParcial)
                    cargaPedido.SituacaoEmissao = SituacaoNF.AgEnvioNF;
                else if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe && !existNotaParcial && repPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedidoComNota(cargaPedido.Codigo))
                    cargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;
                else if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe && !existNotaParcial && !repPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedidoComNota(cargaPedido.Codigo))
                    cargaPedido.SituacaoEmissao = SituacaoNF.AgEnvioNF;

                repositorioCargaPedico.Atualizar(cargaPedido);

                if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete && cargaPedido.SituacaoEmissao == SituacaoNF.AgEnvioNF)
                {
                    cargaPedido.Carga.SituacaoCarga = SituacaoCarga.AgNFe;
                    cargaPedido.Carga.CalculandoFrete = false;
                    cargaPedido.Carga.DataInicioCalculoFrete = null;
                    cargaPedido.Carga.DataEnvioUltimaNFe = null;

                    repCarga.Atualizar(cargaPedido.Carga);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Retorno da carga para Notas Fiscais ao integrar novo registro parcial (Billing) CANCELADO", _unitOfWork);
                }

                return;
            }

            cargaPedidoParcial.XMLNotaFiscal = notaFiscal;
            //cargaPedidoParcial.CargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;

            serCargaNotaFiscal.InserirNotaCargaPedido(notaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out alteradoTipoCarga, _auditado, notaChave);
            //repCargaPedido.Atualizar(cargaPedidoParcial.CargaPedido);

            ValidarArquivoXMlRecebidoVinculado(notaFiscal);

            if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
            {
                cargaPedido.Carga.CalculandoFrete = true;
                repCarga.Atualizar(cargaPedido.Carga);
            }

        }

        public void ValidarArquivoXMlRecebidoVinculado(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork).BuscarPrimeiroRegistro();
            Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repArquivos = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(_unitOfWork);

            if (configuracaoArquivo == null)
                return;

            try
            {
                if (xmlNotaFiscal != null)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao arquivo = repArquivos.BuscarProcessadoPorChave(xmlNotaFiscal.Chave);

                    if (arquivo != null)
                    {
                        arquivo.Initialize();

                        string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal, arquivo.NomeArquivo);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                            Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);

                        arquivo.Mensagem = $"Xml vinculado em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                        repArquivos.Atualizar(arquivo);
                    }

                }
            }
            catch (Exception)
            { }
        }

        public Retorno<string> EnviarArquivoXMLNFe(Stream arquivo)
        {
            try
            {
                Retorno<string> retorno = new Retorno<string>();
                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao, string.Concat(nomeArquivo, ".xml"));

                System.IO.StreamReader reader = null;

                try
                {
                    reader = new StreamReader(arquivo);

                    if (reader.BaseStream.CanSeek)
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    string conteudoArquivo = reader.ReadToEnd();
                    string conteudoArquivoTratado = Servicos.Arquivo.RemoveTroublesomeCharacters(conteudoArquivo);

                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, conteudoArquivoTratado, Encoding.UTF8);

                    retorno.Status = true;
                    retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Objeto = nomeArquivo;

                    return retorno;
                }
                finally
                {
                    if (!retorno.Status)
                        Utilidades.IO.FileStorageService.Storage.DeleteIfExists(caminho);

                    if (reader != null)
                    {
                        reader.Dispose();
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar o arquivo.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
        }

        #endregion Métodos Privados
    }
}
