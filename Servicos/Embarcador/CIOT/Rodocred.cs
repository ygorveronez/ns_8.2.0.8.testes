using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.ServicoRodocred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.CIOT
{
    public class Rodocred
    {
        #region Propriedades Privadas
        private Repositorio.UnitOfWork _unitOfWork;

        private Repositorio.Embarcador.Documentos.CIOT _repCIOT;
        private Repositorio.Embarcador.Cargas.CargaCIOT _repCargaCIOT;
        private Repositorio.Embarcador.CIOT.CIOTRodocred _repConfiguracaoIntegracaoRodocred;

        private Dominio.Entidades.Embarcador.CIOT.CIOTRodocred _configuracao;
        private Dominio.Entidades.Embarcador.Cargas.CargaCIOT _cargaCIOT;
        private Dominio.Entidades.Embarcador.Documentos.CIOT _ciot;
        private ServicoRodocred.IdentificacaoIntegracaoType _IdentificacaoIntegracaoType;
        private Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas _modalidadeTerceiro;
        private Repositorio.Embarcador.Cargas.CargaPedido _repCargaPedido;
        private Dominio.Entidades.Embarcador.Cargas.CargaPedido _cargaPedido;
        private Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo _repCIOTIntegracaoArquivo;
        private InspectorBehavior _inspector = new InspectorBehavior();
        private Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal _repPedidoXMLNotaFiscal;
        private List<(int CodigoCarga, int NumeroNota)> _listaCodigosCargasNumeroNotasFiscais;

        private Dominio.Entidades.Cliente _Terceiro;
        private Dominio.Entidades.Cliente _Remetente;
        private Dominio.Entidades.Cliente _Destinatario;

        private decimal _ValorINSS = 0;
        private decimal _ValorIRRF = 0;
        private decimal _ValorSENAT = 0;
        private decimal _ValorFrete = 0;
        private decimal _ValorSEST = 0;
        private decimal _ValorSaldo = 0;
        private decimal _ValorAdiantamento = 0;
        private decimal _ValorAbastecimento = 0;
        private DateTime _DataVencimentoAdiantamento;
        private decimal _ValorDespesas = 0;
        private decimal _ValorCombustivel = 0;
        private decimal _ValorCarga = 0;
        private decimal _QuantidadeCarga = 0;
        private decimal _PesoCarga = 0;


        // conexoes com serviço 
        private ServicoRodocred.RodocredSoapClient _rodocredSoapClient;
        #endregion

        #region Construtores

        public Rodocred(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            // Inicializa variaveis 
            _unitOfWork = unitOfWork;
            _repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            _repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            _repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(_unitOfWork);

            _repConfiguracaoIntegracaoRodocred = new Repositorio.Embarcador.CIOT.CIOTRodocred(_unitOfWork);
            _ciot = ciot;
            _configuracao = _repConfiguracaoIntegracaoRodocred.BuscarPrimeiroRegistro();
            _cargaCIOT = _repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            _ciot.Operadora = OperadoraCIOT.Rodocred;
            _repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            if (_ciot.Contratante == null)
                _ciot.Contratante = _cargaCIOT.Carga.Empresa;
            if (_ciot.Motorista == null)
            {
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
                Dominio.Entidades.Usuario veiculoMotorista = null;
                if (_cargaCIOT.Carga.Veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(_cargaCIOT.Carga.Veiculo.Codigo);
                _ciot.Motorista = _cargaCIOT.Carga.Motoristas != null && _cargaCIOT.Carga.Motoristas.Count > 0 ? _cargaCIOT.Carga.Motoristas.FirstOrDefault() : veiculoMotorista ?? null;
            }

            _modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(_ciot.Transportador, _unitOfWork);
            _repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            _cargaPedido = _repCargaPedido.BuscarPrimeiroPedidoPorCarga(_cargaCIOT.Carga.Codigo);
            _Terceiro = _cargaCIOT.Carga?.Terceiro ?? null;
            _Remetente = ObterRemetentePedido(_cargaPedido);
            _Destinatario = ObterDestinatarioPedido(_cargaPedido);


            // valores da carga 
            _ValorDespesas = _ciot.CargaCIOT.Sum(x => x.ContratoFrete.ValoresAdicionais.Where(o => o.TipoJustificativa == TipoJustificativa.Desconto).Sum(o => o.Valor));
            _ValorFrete = _ciot.CargaCIOT.Sum(o => (o.ContratoFrete?.ValorFreteSubcontratacao ?? 0m));
            _ValorCombustivel = _ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAbastecimento ?? 0m);
            _ValorCarga = _ciot.CTes.Sum(x => x.CargaCTe?.CTe?.ValorTotalMercadoria ?? 0m);

            List<int> Cargas = _ciot.CargaCIOT.Select(x => x.Carga?.Codigo ?? 0).ToList();
            if (Cargas != null && Cargas.ElementAt(0) != 0)
            {
                _QuantidadeCarga = _repPedidoXMLNotaFiscal.BuscarPesoPorCargas(Cargas);
                _PesoCarga = _repPedidoXMLNotaFiscal.BuscarPesoPorCargas(Cargas);
                _listaCodigosCargasNumeroNotasFiscais = _repPedidoXMLNotaFiscal.BuscarNumeroNotasFiscaisECodigosCargasPorCarga(Cargas);
            }


            // Valores de impostos 
            _ValorINSS = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorINSS ?? 0m);
            _ValorIRRF = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorIRRF ?? 0m);
            _ValorSEST = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorSEST ?? 0m);
            _ValorSENAT = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorSENAT ?? 0m);
            //dsCIOT.Descontos = seguro + descontos + tarifa + pedagioDesconto;
            _ValorSaldo = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.SaldoAReceber ?? 0m);
            _ValorAdiantamento = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAdiantamento ?? 0m);
            _ValorAbastecimento = ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAbastecimento ?? 0m);



            /*
        decimal valorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento;
        decimal valorAbastecimento = cargaCIOT.ContratoFrete.ValorAbastecimento;
        decimal valorFrete = cargaCIOT.ContratoFrete.ValorBruto;
        decimal valorIRRF = cargaCIOT.ContratoFrete.ValorIRRF;
        decimal valorINSS = cargaCIOT.ContratoFrete.ValorINSS;
        decimal valorSESTSENAT = cargaCIOT.ContratoFrete.ValorSEST + cargaCIOT.ContratoFrete.ValorSENAT;
        decimal valorSaldo = cargaCIOT.ContratoFrete.SaldoAReceber;
        decimal tarifaSaque = cargaCIOT.ContratoFrete.TarifaSaque;
        decimal tarifaTransferencia = cargaCIOT.ContratoFrete.TarifaTransferencia;
        */




            // autentica 
            _IdentificacaoIntegracaoType = ObterAutenticacao();

            //ManterTransportador();
        }
        #endregion
        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT()
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;
            sucesso = DeclararOperacaoTransporte(out mensagemErro);
            if (!sucesso)
            {
                _ciot.Situacao = SituacaoCIOT.Pendencia;
                _ciot.Mensagem = mensagemErro;
            }
            if (_ciot.Codigo > 0)
                _repCIOT.Atualizar(_ciot);
            else
                _repCIOT.Inserir(_ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }
        public bool EncerrarCIOT(out string mensagemErro)
        {
            return EncerrarOperacaoTransporte(out mensagemErro);
        }

        public bool CancelarCIOT(out string mensagemErro)
        {
            return CancelarOperacaoTransporte(out mensagemErro);
        }

        private bool CancelarOperacaoTransporte(out string mensagemErro)
        {
            bool ret = true;
            mensagemErro = "";
            try
            {
                if (_IdentificacaoIntegracaoType == null)
                {
                    mensagemErro = "Falha na autenticação";
                    _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                    _cargaCIOT.CIOT.Mensagem = mensagemErro;
                    ret = false;
                }
                else
                {
                    Servicos.ServicoRodocred.CancelarViagemRequest request = ObterRequestCancelarViagem();
                    Servicos.ServicoRodocred.CancelarViagemResponse response = _rodocredSoapClient.CancelarViagem(request);
                    if ((response.RetornoMensagem.StatusRetorno == CancelarViagemResponseRetornoMensagemStatusRetorno.SUCESSO) || response.RetornoMensagem.Excecao != null && response.RetornoMensagem.Excecao.MensagemExcecao == "Viagem já cancelada")
                    {
                        mensagemErro = "Cancelamento realizado com sucesso.";
                        _cargaCIOT.CIOT.ProtocoloCancelamento = ""; //pendencia//retorno.ProtocoloEncerramento;
                        _cargaCIOT.CIOT.Mensagem = mensagemErro;
                        _cargaCIOT.CIOT.DataCancelamento = DateTime.Now;
                        _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Cancelado;
                        ret = true;
                    }
                    else
                    {
                        if (response.RetornoMensagem.Excecao != null)
                            mensagemErro = $"{response.RetornoMensagem.Excecao.CodigoExcecao} - {response.RetornoMensagem.Excecao.MensagemExcecao}";
                        mensagemErro = mensagemErro + " Ocorreu uma falha na Rodocred ao enviar a tentativa de cancelamento do CIOT.";
                        _cargaCIOT.CIOT.Mensagem = mensagemErro;
                        ret = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                _cargaCIOT.CIOT.Mensagem = mensagemErro;
                ret = false;
            }
            _repCIOT.Atualizar(_cargaCIOT.CIOT);
            GravarArquivoIntegracao(_cargaCIOT.CIOT, _inspector.LastRequestXML, _inspector.LastResponseXML, "xml");
            return ret;

        }

        private CancelarViagemRequest ObterRequestCancelarViagem()
        {
            CancelarViagemRequest retorno = new CancelarViagemRequest();
            retorno.IdentificacaoIntegracao = _IdentificacaoIntegracaoType;
            retorno.IdClienteResponsavelSpecified = false;
            retorno.NumeroViagem = ObterNumeroViagem();
            retorno.MotivoCancelamento = string.IsNullOrEmpty(_ciot.MotivoCancelamento) ? "Cancelamento solicitado via Multisoftware" : _ciot.MotivoCancelamento;
            return retorno;
        }

        #endregion

        #region Transformação/Converssao de dados 

        private ManterViagemRequestDadosViagemForo ObterForo()
        {
            ManterViagemRequestDadosViagemForo Foro = new ManterViagemRequestDadosViagemForo();
            return null;
        }

        private ManterViagemRequestDadosViagem ObterDadosViagem()
        {
            ServicoRodocred.ManterViagemRequestDadosViagem DV = new ServicoRodocred.ManterViagemRequestDadosViagem();
            try
            {
                DV.EmbarqueSpecified = true;
                //DV.Embarque = (DateTime)_cargaCIOT.Carga.Pedidos.Max(x => x.Pedido.DataAgendamento);
                DV.Embarque = this._ciot.DataAbertura ?? DateTime.Now;
            }
            catch (Exception ex) 
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao definir data de embarque CIOT Rodocred: {ex.ToString()}", "CatchNoAction");
            }
            if (DV.Embarque < DateTime.Now)
                DV.Embarque = DateTime.Now;

            try
            {
                DV.PrevisaoEntregaSpecified = true;
                DV.PrevisaoEntrega = (DateTime)_cargaCIOT.Carga.Pedidos.Max(x => x.Pedido.PrevisaoEntrega);
            }
            catch (Exception ex) 
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao definir previsão de entrega CIOT Rodocred: {ex.ToString()}", "CatchNoAction");
            }

            if (DV.PrevisaoEntrega < DateTime.Now)
                DV.PrevisaoEntrega = DateTime.Now.AddDays(8);


            DV.DataVigencia = (_cargaCIOT.CIOT.DataAbertura ?? DateTime.Now).AddDays(30);
            DV.DataVigenciaSpecified = true;
            // tem no manual nao tem nos metodos DV.CodigoTipoCarga = ((int)pPef.tipocarga).ToString();

            DV.NumeroViagemContratacao = "";//??
            DV.NumeroViagem = ObterNumeroViagem();

            DV.Foro = ObterForo();//??
            DV.DocumentoRef = this._ciot.Codigo.ToString();//??
            DV.NumeroViagemContratacao = _ciot.CodigoVerificador;//??
            DV.CentrodeCusto = "";//??
            DV.Observacao = "";//??

            DV.GerarCIOT = true;
            DV.SemTransporteCarga = false;
            DV.QtdeViagens = 1;//??
            DV.QtdeViagensSpecified = true;

            //string cDocumentoViagem = null;
            //if (manif.TipoDocumento == Types.Enums.TipoDocumento.ManifestoCarga)
            //    cDocumentoViagem = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", manif.Grupo, manif.Empresa, manif.Filial, manif.Unidade, manif.DiferenciadorNumero, manif.Serie, manif.NumeroSequencia);
            //else
            //    cDocumentoViagem = string.Format("{0}-{1}-{2}-{3}-{4}", "201", manif.Grupo, manif.Empresa, manif.DiferenciadorNumero, manif.NumeroSequencia);
            //tem no manual mas não tem no serviço  DV.FreteRetorno = ObterFreteRetorno(); ??
            return DV;
        }

        #endregion

        #region Privados de Integração
        private string ObterNumeroViagem()
        {
            return _ciot.ProtocoloAutorizacao?.ToString();
        }
        private bool DeclararOperacaoTransporte(out string mensagemErro)
        {
            mensagemErro = null;
            bool sucesso = false;
            try
            {

                if (_IdentificacaoIntegracaoType == null)
                {
                    mensagemErro = "Falha na autenticação";
                    _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                    _cargaCIOT.CIOT.Mensagem = mensagemErro;
                }
                else
                {
                    Servicos.ServicoRodocred.ManterViagemRequest request = ObterManterViagemInclusao();
                    Servicos.ServicoRodocred.ManterViagemResponse response = _rodocredSoapClient.ManterViagem(request);
                    if (response.RetornoMensagem.StatusRetorno == ServicoRodocred.ManterViagemResponseRetornoMensagemStatusRetorno.SUCESSO)
                    {
                        sucesso = true;
                        _cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                        _cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                        _cargaCIOT.CIOT.Mensagem = "CIOT integrado com sucesso.";
                        _cargaCIOT.CIOT.Numero = response.CIOT.CodigoOperacaoTransporte;
                        _cargaCIOT.CIOT.Digito = response.CIOT.CodigoOperacaoTransporteDV;
                        _cargaCIOT.CIOT.ProtocoloAutorizacao = response.NumeroViagem;
                        _repCIOT.Atualizar(_cargaCIOT.CIOT);
                    }
                    else
                    {
                        mensagemErro = response.RetornoMensagem.Excecao.CodigoExcecao + " - " + response.RetornoMensagem.Excecao.MensagemExcecao;
                        _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                        _cargaCIOT.CIOT.Mensagem = mensagemErro;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                _cargaCIOT.CIOT.Mensagem = "Ocorreu uma falha ao integrar o CIOT.";
            }

            _cargaCIOT.CIOT.Mensagem = _cargaCIOT.CIOT.Mensagem.Replace("'", " ");
            if (_cargaCIOT.CIOT.Mensagem.Length > 400)
                _cargaCIOT.CIOT.Mensagem = _cargaCIOT.CIOT.Mensagem.Substring(1, 400);

            _repCIOT.Atualizar(_cargaCIOT.CIOT);
            GravarArquivoIntegracao(_inspector.LastRequestXML, _inspector.LastResponseXML, "xml");
            return sucesso;
        }
        private Dominio.Entidades.Cliente ObterRemetentePedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Expedidor != null)
                return cargaPedido.Expedidor;
            else
                return cargaPedido.Pedido.Remetente;
        }
        private Dominio.Entidades.Cliente ObterDestinatarioPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null)
                return cargaPedido.Recebedor;
            else
                return cargaPedido.Pedido.Destinatario;
        }


        private bool ManterTransportador()
        {

            ManterTransportadorRequest retorno = new ManterTransportadorRequest();

            retorno.IdentificacaoIntegracao = _IdentificacaoIntegracaoType;
            retorno.Operacao = TipoOperacaoType.INC;
            retorno.IdClienteResponsavelSpecified = false;
            retorno.ListaTransportador = ObterTransportadores();

            var teste = _rodocredSoapClient.ManterTransportador(retorno);

            return true;
        }

        private ManterTransportadorRequestListaTransportador ObterTransportadores()
        {
            ManterTransportadorRequestListaTransportador retorno = new ManterTransportadorRequestListaTransportador();
            retorno.ValidaFrotaANTT = true;
            retorno.ValidaTransportadorANTT = true;

            List<DadosTransportadorType> lstDadosTransportador = new List<DadosTransportadorType>();
            DadosTransportadorType transportador = new DadosTransportadorType();

            transportador.Transportador = ObterTransportadorTransportador();

            /*                if (cad.Classificacao != null && cad.Classificacao == 2 && cad.Tipo == 2)
                                transportador.TipoTransportador = WS.PEF.SemPararEmpresas1.TipoTransportadorType.CTC;
                            else
                            {
                                if (cad.Tipo == 2)
                                    transportador.TipoTransportador = WS.PEF.SemPararEmpresas1.TipoTransportadorType.ETC;
                                else
                                    transportador.TipoTransportador = WS.PEF.SemPararEmpresas1.TipoTransportadorType.TAC;
                            }
            */
            transportador.TipoTransportador = TipoTransportadorType.CTC;
            transportador.Endereco = ObterEndereco(_ciot.Transportador);
            //transportador.Contato = this.Contato(cad);
            //if (cad.Tipo == 2)
            transportador.TransportadorPJ = new TransportadorPJType();
            transportador.TransportadorPJ.InscricaoMunicipal = "0";
            transportador.TransportadorPJ.InscricaoEstadual = "0";

            //else if (cad.Tipo == 1)
            //  transportador.TransportadorPF = this.TransportadorPF(cad);
            transportador.NumeroCartao = "";

            lstDadosTransportador.Add(transportador);
            retorno.DadosTransportador = lstDadosTransportador.ToArray();

            return retorno;
        }

        private bool EncerrarOperacaoTransporte(out string mensagemErro)
        {
            bool ret = true;
            mensagemErro = "";
            try
            {
                if (_IdentificacaoIntegracaoType == null)
                {
                    mensagemErro = "Falha na autenticação";
                    _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                    _cargaCIOT.CIOT.Mensagem = mensagemErro;
                    ret = false;
                }
                else
                {
                    Servicos.ServicoRodocred.EncerrarViagemRequest request = ObterRequestEncerrarViagem();
                    Servicos.ServicoRodocred.EncerrarViagemResponse response = _rodocredSoapClient.EncerrarViagem(request);
                    if (response.RetornoMensagem.StatusRetorno == EncerrarViagemResponseRetornoMensagemStatusRetorno.SUCESSO)
                    {
                        mensagemErro = "Encerramento realizado com sucesso.";
                        _cargaCIOT.CIOT.ProtocoloEncerramento = ""; //pendencia//retorno.ProtocoloEncerramento;
                        _cargaCIOT.CIOT.DataEncerramento = DateTime.Now;
                        _cargaCIOT.CIOT.Situacao = SituacaoCIOT.Encerrado;
                        ret = true;
                    }
                    else
                    {
                        if (response.RetornoMensagem.Excecao != null)
                            mensagemErro = $"{response.RetornoMensagem.Excecao.CodigoExcecao} - {response.RetornoMensagem.Excecao.MensagemExcecao}";
                        mensagemErro = mensagemErro + " Ocorreu uma falha na Rodocred ao enviar a tentativa de Encerramento do CIOT.";
                        _cargaCIOT.CIOT.Mensagem = mensagemErro;
                        ret = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                _cargaCIOT.CIOT.Mensagem = mensagemErro;
                ret = false;
            }
            _repCIOT.Atualizar(_cargaCIOT.CIOT);
            GravarArquivoIntegracao(_cargaCIOT.CIOT, _inspector.LastRequestXML, _inspector.LastResponseXML, "xml");
            return ret;
        }

        private void GravarArquivoIntegracao(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, string request, string response, string extensaoArquivo)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request ?? string.Empty, extensaoArquivo, _unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response ?? string.Empty, extensaoArquivo, _unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Mensagem = ciot.Mensagem
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);
        }

        private Servicos.ServicoRodocred.EncerrarViagemRequest ObterRequestEncerrarViagem()
        {
            EncerrarViagemRequest retorno = new EncerrarViagemRequest();

            retorno.IdentificacaoIntegracao = _IdentificacaoIntegracaoType;
            retorno.IdClienteResponsavelSpecified = false;
            retorno.NumeroViagem = ObterNumeroViagem();
            retorno.ValoresViagemEncerrar = ObterValoresViagemEncerrar();
            retorno.ImpostosViagem = ObterImpostosViagemEncerrar();
            retorno.TarifasCIOT = ObterTarifasCIOT();
            return retorno;
        }

        private EncerrarViagemRequestImpostosViagem ObterImpostosViagemEncerrar()
        {
            if (_cargaCIOT.ContratoFrete == null)
                return null;

            EncerrarViagemRequestImpostosViagem retorno = new EncerrarViagemRequestImpostosViagem();
            retorno.ValorTotalImpostos = _cargaCIOT.ContratoFrete.ValorINSS + _cargaCIOT.ContratoFrete.ValorIRRF + _cargaCIOT.ContratoFrete.ValorSENAT;
            if (_ValorINSS > 0 || _ValorIRRF > 0 || _ValorSENAT > 0)
            {
                var Impostos = new List<EncerrarViagemRequestImpostosViagemImpostos>();
                // 209734 - Gerar Tags de impostos zerados conforme orienteção da administradora
                var inss = new EncerrarViagemRequestImpostosViagemImpostos();
                inss.CodigoImposto = EncerrarViagemRequestImpostosViagemImpostosCodigoImposto.INSS;
                inss.BaseCalculoImposto = _ValorFrete;
                inss.ValorImposto = _ValorINSS;
                inss.PercentualImposto = inss.ValorImposto / inss.BaseCalculoImposto * 100;
                inss.RetencaoImposto = true;
                Impostos.Add(inss);

                // 209734 - Gerar Tags de impostos zerados conforme orienteção da administradora
                var irrf = new EncerrarViagemRequestImpostosViagemImpostos();
                irrf.CodigoImposto = EncerrarViagemRequestImpostosViagemImpostosCodigoImposto.IRRF;
                irrf.BaseCalculoImposto = _ValorFrete;
                irrf.ValorImposto = _ValorIRRF;
                irrf.PercentualImposto = irrf.ValorImposto / irrf.BaseCalculoImposto * 100;
                irrf.RetencaoImposto = true;
                Impostos.Add(irrf);

                // 209734 - Gerar Tags de impostos zerados conforme orienteção da administradora
                var sestsenat = new EncerrarViagemRequestImpostosViagemImpostos();
                sestsenat.CodigoImposto = EncerrarViagemRequestImpostosViagemImpostosCodigoImposto.SENAT;
                sestsenat.BaseCalculoImposto = _ValorFrete;
                sestsenat.ValorImposto = _ValorSENAT;
                sestsenat.PercentualImposto = sestsenat.ValorImposto / sestsenat.BaseCalculoImposto * 100;
                sestsenat.RetencaoImposto = true;
                Impostos.Add(sestsenat);
                retorno.Impostos = Impostos.ToArray();
            }
            return retorno;
        }

        private ValoresViagemEncerrarType ObterValoresViagemEncerrar()
        {
            try
            {
                ValoresViagemEncerrarType retorno = new ValoresViagemEncerrarType();

                retorno.ValorFrete = _ValorFrete;
                retorno.ValorFreteSpecified = true;

                retorno.PesoCarga = Math.Round(_PesoCarga, 2);
                retorno.PesoCargaSpecified = true;

                retorno.UnidadeMedidaPeso = ValoresViagemEncerrarTypeUnidadeMedidaPeso.KG;
                retorno.UnidadeMedidaPesoSpecified = true;

                return retorno;

            }
            catch (Exception)
            {
                return new ServicoRodocred.ValoresViagemEncerrarType();
            }

        }

        private Servicos.ServicoRodocred.EncerrarContratoAgregadoRequest ObterRequestEncerrarContratoAgregado()
        {
            EncerrarContratoAgregadoRequest retorno = new EncerrarContratoAgregadoRequest();

            //retorno.IdentificacaoIntegracao = this.IdentificacaoIntegracao("1.01");
            //retorno.IdClienteResponsavelSpecified = false;
            //retorno.NumeroContrato = pPef.IDViagemAdministradora;

            return retorno;
        }


        private Servicos.ServicoRodocred.ManterViagemRequest ObterManterViagemInclusao()
        {
            Servicos.ServicoRodocred.ManterViagemRequest manterViagemInclusao = new Servicos.ServicoRodocred.ManterViagemRequest();

            manterViagemInclusao.IdentificacaoIntegracao = _IdentificacaoIntegracaoType;
            manterViagemInclusao.Operacao = ServicoRodocred.TipoOperacaoType.INC;
            manterViagemInclusao.TipoViagem = ServicoRodocred.TipoViagemType.FRE;
            manterViagemInclusao.DadosViagem = ObterDadosViagem();
            manterViagemInclusao.Transportador = ObterTransportador();
            manterViagemInclusao.RotaViagem = ObterRotaViagem();
            manterViagemInclusao.VeiculosViagem = ObterVeiculosViagem();
            manterViagemInclusao.ValePedagioViagem = ObterValePedagioViagem();
            manterViagemInclusao.MotoristaViagem = ObterMotoristaViagem();
            manterViagemInclusao.EnvolvidosTransporteViagem = ObterEnvolvidosTransporteViagem();
            manterViagemInclusao.ValoresViagem = ObterValoresViagem();
            manterViagemInclusao.ImpostosViagem = ObterImpostosViagem();
            manterViagemInclusao.DadosBancarios = ObterDadosBancarios();
            manterViagemInclusao.DocumentosViagem = ObterDocumentosViagem();
            manterViagemInclusao.ProgramacaoViagem = ObterProgramacaoViagemFRE();
            manterViagemInclusao.ClausulasViagem = ObterClausulasViagem();
            manterViagemInclusao.TarifasCIOT = ObterTarifasCIOT();
            manterViagemInclusao.ListaCentroCusto = ObterListaCentroCusto();
            manterViagemInclusao.RecargaCartaoViagem = ObterRecargaCartaoViagem();


            return manterViagemInclusao;
        }

        private ManterViagemRequestRecargaCartaoViagem ObterRecargaCartaoViagem()
        {
            return null;
        }

        private PercentualCentroCustoType[] ObterListaCentroCusto()
        {
            return null;
        }

        private TarifasCIOTType ObterTarifasCIOT()
        { // pendencia no chamado
            TarifasCIOTType retorno = null;
            /*
            // Tarifa Bancária de Saque
            if (pPef.QtdTransacaoTarifaSaque > 0)
            {
                retorno.QtdeSaquesObrigatorios = pPef.QtdTransacaoTarifaSaque;

                if (pPef.ValorTotalTarifaSaque > 0 && pPef.Integracao == 2)
                {
                    retorno.ValorTaxaSaques = pPef.ValorTotalTarifaSaque;
                    retorno.ValorTaxaSaquesSpecified = true;
                }
            }
            else
            {
                retorno.QtdeSaquesObrigatorios = 0;
                retorno.ValorTaxaSaquesSpecified = false;
            }

            // Tarifa Bancária de Transferência
            if (pPef.QtdTransacaoTarifaTransferencia > 0)
            {
                retorno.QtdeTransferenciaObrigatorias = pPef.QtdTransacaoTarifaTransferencia;

                if (pPef.ValorTotalTarifaTransferencia > 0 && pPef.Integracao == 2)
                {
                    retorno.ValorTaxaTransferencia = pPef.ValorTotalTarifaTransferencia;
                    retorno.ValorTaxaTransferenciaSpecified = true;
                }
            }
            else
            {
                retorno.QtdeTransferenciaObrigatorias = 0;
                retorno.ValorTaxaTransferenciaSpecified = false;
            }
            */

            return retorno;
        }

        private ManterViagemRequestClausulasViagem[] ObterClausulasViagem()
        {
            return null;
        }

        private ArrayOfOperacaoViagemOperacaoViagem[] ObterProgramacaoViagemFRE()
        {
            List<ArrayOfOperacaoViagemOperacaoViagem> retorno = retorno = new List<ArrayOfOperacaoViagemOperacaoViagem>();

            ArrayOfOperacaoViagemOperacaoViagem operacaoViagem = new ArrayOfOperacaoViagemOperacaoViagem();
            operacaoViagem.CodigoOperacao = "ADF";
            operacaoViagem.CondicaoLiberacaoOperacao = "LIB";
            operacaoViagem.DataOperacao = DateTime.Now.AddDays(1);
            operacaoViagem.DataOperacaoSpecified = false;
            operacaoViagem.EventoOperacao = new ArrayOfOperacaoViagemOperacaoViagemEventoOperacao[1];
            ArrayOfOperacaoViagemOperacaoViagemEventoOperacao EventoOperacao = new ArrayOfOperacaoViagemOperacaoViagemEventoOperacao();
            operacaoViagem.EventoOperacao[0] = EventoOperacao;
            EventoOperacao.CodigoEventoOperacao = "TRF";
            EventoOperacao.ValorEvento = Math.Round(_ValorSaldo * 0.8m, 2);
            EventoOperacao.ValorEventoSpecified = true;
            operacaoViagem.Operacao = TipoOperacaoType.INC;
            operacaoViagem.OperacaoSpecified = true;
            operacaoViagem.IDOperacaoViagemSpecified = false;
            retorno.Add(operacaoViagem);



            operacaoViagem = new ArrayOfOperacaoViagemOperacaoViagem();
            operacaoViagem.CodigoOperacao = "LSF"; //LSF
            operacaoViagem.CondicaoLiberacaoOperacao = "FIL";
            operacaoViagem.DataOperacao = _DataVencimentoAdiantamento;
            operacaoViagem.DataOperacaoSpecified = false;
            operacaoViagem.LocalRealizacao = "QFL";
            operacaoViagem.EventoOperacao = new ArrayOfOperacaoViagemOperacaoViagemEventoOperacao[1];
            EventoOperacao = new ArrayOfOperacaoViagemOperacaoViagemEventoOperacao();
            operacaoViagem.EventoOperacao[0] = EventoOperacao;
            EventoOperacao.CodigoEventoOperacao = "TSF";
            EventoOperacao.ValorEvento = Math.Round(_ValorSaldo * 0.2m, 2);
            EventoOperacao.ValorEventoSpecified = true;
            operacaoViagem.Operacao = TipoOperacaoType.INC;
            operacaoViagem.OperacaoSpecified = true;
            operacaoViagem.IDOperacaoViagemSpecified = false;
            retorno.Add(operacaoViagem);
            return retorno.ToArray();
        }

        private ArrayOfDocumentosViagemDocumentoDocumento[] ObterDocumentosViagem()
        {
            List<ArrayOfDocumentosViagemDocumentoDocumento> Docs = new List<ArrayOfDocumentosViagemDocumentoDocumento>();
            foreach (var item in _listaCodigosCargasNumeroNotasFiscais)
            {
                Docs.Add(new ArrayOfDocumentosViagemDocumentoDocumento()
                {
                    CodigoDocumento = "NFF",
                    NumeroDocumento = item.NumeroNota.ToString(),
                    CodigoAuxiliar = null,
                    NaturezaCarga = "1905",
                    DocumentoRequerido = ArrayOfDocumentosViagemDocumentoDocumentoDocumentoRequerido.N
                });
            }




            return Docs.ToArray();



            /*
             
              List<WS.PEF.SemPararEmpresas1.ArrayOfDocumentosViagemDocumentoDocumento> retorno = null;

            foreach (var documento in pPef.Documentos)
            {
                if (retorno == null)
                    retorno = new List<WS.PEF.SemPararEmpresas1.ArrayOfDocumentosViagemDocumentoDocumento>();

                WS.PEF.SemPararEmpresas1.ArrayOfDocumentosViagemDocumentoDocumento doc = new WS.PEF.SemPararEmpresas1.ArrayOfDocumentosViagemDocumentoDocumento();


                switch (documento.TipoDocumento)
                {
                    case AvaCorp.Virtual.Types.Enums.TipoDocumento.Conhecimento:
                        doc.CodigoDocumento = "CTE";
                        break;
                    case Types.Enums.TipoDocumento.ManifestoCarga:
                        doc.CodigoDocumento = "MAN";
                        break;
                    default:
                        doc.CodigoDocumento = "DAC";
                        break;
                }

                doc.NumeroDocumento = documento.NumeroSequenciaDocumento.ToString();
                doc.CodigoAuxiliar = null;
                doc.DocumentoRequerido = WS.PEF.SemPararEmpresas1.ArrayOfDocumentosViagemDocumentoDocumentoDocumentoRequerido.N;
                doc.NaturezaCarga = documento.NaturezaMercadoria;

                retorno.Add(doc);
            }

            return retorno;
             
             */











        }

        private ManterViagemRequestDadosBancarios ObterDadosBancarios()
        {
            return null;
        }

        private ManterViagemRequestImpostosViagem ObterImpostosViagem()
        {
            if (_cargaCIOT.ContratoFrete == null)
                return null;

            ManterViagemRequestImpostosViagem retorno = new ManterViagemRequestImpostosViagem();
            retorno.ValorTotalImpostos = _cargaCIOT.ContratoFrete.ValorINSS + _cargaCIOT.ContratoFrete.ValorIRRF + _cargaCIOT.ContratoFrete.ValorSENAT;
            if (_ValorINSS > 0 || _ValorIRRF > 0 || _ValorSENAT > 0)
            {
                var Impostos = new List<ManterViagemRequestImpostosViagemImpostos>();
                // 209734 - Gerar Tags de impostos zerados conforme orienteção da administradora
                var inss = new ManterViagemRequestImpostosViagemImpostos();
                inss.CodigoImposto = ManterViagemRequestImpostosViagemImpostosCodigoImposto.INSS;
                inss.BaseCalculoImposto = _ValorFrete;
                inss.ValorImposto = _ValorINSS;
                inss.PercentualImposto = inss.ValorImposto / inss.BaseCalculoImposto * 100;
                inss.RetencaoImposto = true;
                Impostos.Add(inss);

                // 209734 - Gerar Tags de impostos zerados conforme orienteção da administradora
                var irrf = new ManterViagemRequestImpostosViagemImpostos();
                irrf.CodigoImposto = ManterViagemRequestImpostosViagemImpostosCodigoImposto.IRRF;
                irrf.BaseCalculoImposto = _ValorFrete;
                irrf.ValorImposto = _ValorIRRF;
                irrf.PercentualImposto = irrf.ValorImposto / irrf.BaseCalculoImposto * 100;
                irrf.RetencaoImposto = true;
                Impostos.Add(irrf);

                // 209734 - Gerar Tags de impostos zerados conforme orienteção da administradora
                var sestsenat = new ManterViagemRequestImpostosViagemImpostos();
                sestsenat.CodigoImposto = ManterViagemRequestImpostosViagemImpostosCodigoImposto.SENAT;
                sestsenat.BaseCalculoImposto = _ValorFrete;
                sestsenat.ValorImposto = _ValorSENAT;
                sestsenat.PercentualImposto = sestsenat.ValorImposto / sestsenat.BaseCalculoImposto * 100;
                sestsenat.RetencaoImposto = true;
                Impostos.Add(sestsenat);
                retorno.Impostos = Impostos.ToArray();
            }
            return retorno;
        }

        private ManterViagemRequestValoresViagem ObterValoresViagem()
        {
            try
            {
                ServicoRodocred.ManterViagemRequestValoresViagem ret = new ServicoRodocred.ManterViagemRequestValoresViagem();

                //Valor de Despesas de Viagem de Frota Própria.
                ret.ValorDespesas = _ValorDespesas;

                ret.TipoPagamentoFrete = ManterViagemRequestValoresViagemTipoPagamentoFrete.VAL;
                ret.TipoPagamentoFreteSpecified = true;

                ret.ValorFrete = _ValorFrete;
                ret.ValorCombustivel = _ValorCombustivel;

                ret.ValorCarga = _ValorCarga;
                ret.ValorCargaSpecified = true;

                ret.QuantidadeCarga = (int)_QuantidadeCarga;
                ret.QuantidadeCargaSpecified = true;

                ret.PesoCarga = Math.Round(_PesoCarga, 2);
                ret.PesoCargaSpecified = true;

                ret.ValorFreteToneladaSpecified = false;
                ret.UnidadeMedidaPeso = ManterViagemRequestValoresViagemUnidadeMedidaPeso.KG;
                ret.UnidadeMedidaPesoSpecified = true;
                return ret;
            }
            catch (Exception)
            {
                return new ServicoRodocred.ManterViagemRequestValoresViagem();
            }
        }
        private EnvolvidosTransporteViagemType ObterEnvolvidosTransporteViagem()
        {
            EnvolvidosTransporteViagemType retorno = new EnvolvidosTransporteViagemType();

            if (_cargaPedido.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                retorno.TipoFrete = TipoFreteType.CIF;
            else
                retorno.TipoFrete = TipoFreteType.FOB;

            var lstEnvolvidosTransporte = EnvolvidosTransporteViagemTypeEnvolvidosTransporte();
            if (lstEnvolvidosTransporte != null)
                retorno.EnvolvidosTransporte = lstEnvolvidosTransporte.ToArray();

            return retorno;
        }

        private EnderecoType ObterEndereco(Dominio.Entidades.Cliente cliente)
        {
            EnderecoType retorno = new EnderecoType();
            ServicoRodocred.Estado UF;
            ServicoRodocred.Estado.TryParse(cliente.Localidade.Estado.Descricao, out UF);
            retorno.TipoEndereco = EnderecoTypeTipoEndereco.C;
            retorno.TipoEnderecoSpecified = true;
            retorno.CEP = cliente.CEP;
            retorno.TipoLogradouro = ObterTipoEndereco(cliente);
            retorno.TipoLogradouroSpecified = true;
            retorno.Logradouro = cliente.Endereco;
            retorno.Numero = cliente.Numero;
            retorno.Complemento = cliente.Complemento;
            retorno.Bairro = cliente.Bairro;
            retorno.Cidade = cliente.Localidade.Descricao;
            retorno.Estado = UF;
            retorno.EstadoSpecified = true;
            return retorno;
        }

        private EnderecoTypeTipoLogradouro ObterTipoEndereco(Dominio.Entidades.Cliente cliente)
        {
            //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro

            if (cliente.TipoLogradouro == null)
                return ServicoRodocred.EnderecoTypeTipoLogradouro.Área;
            switch (cliente.TipoLogradouro)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua: return ServicoRodocred.EnderecoTypeTipoLogradouro.Rua;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Avenida: return ServicoRodocred.EnderecoTypeTipoLogradouro.Avenida;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Estrada: return ServicoRodocred.EnderecoTypeTipoLogradouro.Estrada;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Praca: return ServicoRodocred.EnderecoTypeTipoLogradouro.Praça;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Travessa: return ServicoRodocred.EnderecoTypeTipoLogradouro.Travessa;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros: return ServicoRodocred.EnderecoTypeTipoLogradouro.Área;
                default: return ServicoRodocred.EnderecoTypeTipoLogradouro.Área;
            }
        }


        private List<EnvolvidosTransporteViagemTypeEnvolvidosTransporte> EnvolvidosTransporteViagemTypeEnvolvidosTransporte()
        {
            List<EnvolvidosTransporteViagemTypeEnvolvidosTransporte> retorno = null;
            if (_Remetente != null)
            {
                if (retorno == null)
                    retorno = new List<EnvolvidosTransporteViagemTypeEnvolvidosTransporte>();
                EnvolvidosTransporteViagemTypeEnvolvidosTransporte remetente = new EnvolvidosTransporteViagemTypeEnvolvidosTransporte();
                remetente.TipoEnvolvido = EnvolvidosTransporteViagemTypeEnvolvidosTransporteTipoEnvolvido.REM;
                remetente.CNPJCPF = _Remetente.CPF_CNPJ_SemFormato.ToString();
                remetente.NomeEnvolvido = _Remetente.Nome;
                remetente.Endereco = ObterEndereco(_Remetente);
                retorno.Add(remetente);
            }

            if (_Destinatario != null)
            {
                if (retorno == null)
                    retorno = new List<EnvolvidosTransporteViagemTypeEnvolvidosTransporte>();
                EnvolvidosTransporteViagemTypeEnvolvidosTransporte destinatario = new EnvolvidosTransporteViagemTypeEnvolvidosTransporte();
                destinatario.TipoEnvolvido = EnvolvidosTransporteViagemTypeEnvolvidosTransporteTipoEnvolvido.DES;
                destinatario.CNPJCPF = _Destinatario.CPF_CNPJ_SemFormato.ToString();
                destinatario.NomeEnvolvido = _Destinatario.Nome;
                destinatario.Endereco = ObterEndereco(_Destinatario);
                retorno.Add(destinatario);
            }
            return retorno;
        }

        private MotoristaViagemType[] ObterMotoristaViagem()
        {
            List<ServicoRodocred.MotoristaViagemType> motoristas = new List<ServicoRodocred.MotoristaViagemType>();
            ServicoRodocred.MotoristaViagemType motoristaViagem = new ServicoRodocred.MotoristaViagemType()
            {
                CPFMotorista = _ciot.Motorista.CPF,
                NomeMotorista = _ciot.Motorista.Nome
            };
            motoristas.Add(motoristaViagem);



            /* if (_cargaCIOT.Carga.Motoristas?.Count > 0)
             {
                 foreach (Dominio.Entidades.Usuario motorista in _cargaCIOT.Carga.Motoristas)
                 {
                     ServicoRodocred.MotoristaViagemType motoristaViagem = new ServicoRodocred.MotoristaViagemType()
                     {
                         CPFMotorista = motorista.CPF,
                         NomeMotorista = motorista.Nome
                     };
                     motoristas.Add(motoristaViagem);
                 }
             }*/
            return motoristas.ToArray();
        }

        private ValePedagioViagemType ObterValePedagioViagem()
        {
            return null;
        }

        private ManterViagemRequestVeiculosViagem[] ObterVeiculosViagem()
        {
            List<ManterViagemRequestVeiculosViagem> veiculos = new List<ManterViagemRequestVeiculosViagem>();
            string Placa = "";
            if (_ciot.Veiculo != null)
            {
                Placa = _ciot.Veiculo.Placa;
                veiculos.Add(new ManterViagemRequestVeiculosViagem()
                {
                    PlacaVeiculo = _cargaCIOT.CIOT.Veiculo.Placa,
                    TipoVeiculo = TransformeTipoVeiculo(_cargaCIOT.CIOT.Veiculo),
                    QtdeEixos = (_cargaCIOT.Carga?.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                    TipoRodagem = ObterTipoRodagem(_cargaCIOT.CIOT.Veiculo),
                    RNTRC = _cargaCIOT.CIOT.Veiculo.RNTRC > 0 ? $"{_cargaCIOT.CIOT.Veiculo.RNTRC:00000000}" : null
                });
            }

            foreach (Dominio.Entidades.Veiculo veiculo in _ciot.VeiculosVinculados)
            {
                if (Placa != veiculo.Placa)
                    veiculos.Add(new ManterViagemRequestVeiculosViagem()
                    {
                        PlacaVeiculo = veiculo.Placa,
                        TipoVeiculo = TransformeTipoVeiculo(veiculo),
                        QtdeEixos = (veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                        TipoRodagem = ObterTipoRodagem(veiculo),
                        RNTRC = veiculo.RNTRC > 0 ? $"{veiculo.RNTRC:00000000}" : null
                    });
            }
            return veiculos.ToArray();
        }

        private TipoRodagemType ObterTipoRodagem(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo?.ModeloVeicularCarga != null)
            {
                if (veiculo.ModeloVeicularCarga.PadraoEixos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo.Duplo || veiculo.ModeloVeicularCarga.NumeroEixos > 2)
                    return ServicoRodocred.TipoRodagemType.D;
                else
                    return ServicoRodocred.TipoRodagemType.S;
            }

            if (!string.IsNullOrWhiteSpace(veiculo?.TipoRodado))
            {
                string tipoRodado = veiculo.TipoRodado;
                if (tipoRodado.Equals("01") || tipoRodado.Equals("02") || tipoRodado.Equals("03"))
                    return ServicoRodocred.TipoRodagemType.D;
            }
            return ServicoRodocred.TipoRodagemType.S;
        }

        private ManterViagemRequestRotaViagem ObterRotaViagem()
        {
            ManterViagemRequestRotaViagem retorno = new ManterViagemRequestRotaViagem();
            retorno.CodigoRota = null;
            List<ManterViagemRequestRotaViagemOrigemDestinoRota> lstOrigemDestinoRota = new List<ManterViagemRequestRotaViagemOrigemDestinoRota>();
            ManterViagemRequestRotaViagemOrigemDestinoRota origemDestinoRota = new ManterViagemRequestRotaViagemOrigemDestinoRota();

            origemDestinoRota.CodigoIBGEOrigem = _cargaPedido.Origem.CodigoIBGE.ToString();
            origemDestinoRota.CodigoIBGEDestino = _cargaPedido.Destino.CodigoIBGE.ToString();
            lstOrigemDestinoRota.Add(origemDestinoRota);
            retorno.OrigemDestinoRota = lstOrigemDestinoRota.ToArray();

            ManterViagemRequestRotaViagemOrigemDestinoRotaLocalidade origemDestinoRotaLocalidade = new ManterViagemRequestRotaViagemOrigemDestinoRotaLocalidade();
            origemDestinoRotaLocalidade.CodigoLocalidadeOrigem = _cargaPedido.Origem.CodigoIBGE;
            origemDestinoRotaLocalidade.CodigoLocalidadeDestino = _cargaPedido.Destino.CodigoIBGE;
            retorno.OrigemDestinoRotaLocalidade = origemDestinoRotaLocalidade;
            //retorno.OrigemDestinoRotaLatLon = null;
            //retorno.OrigemDestinoRota = null;
            return retorno;
        }

        private ManterViagemRequestTransportador ObterTransportador()
        {
            ManterViagemRequestTransportador retorno = new ManterViagemRequestTransportador();
            retorno.CNPJCPFTransportador = _ciot.Transportador.CPF_CNPJ_SemFormato.ToString();
            retorno.NomeTransportador = _ciot.Transportador.Nome;
            retorno.RNTRC = _modalidadeTerceiro.RNTRC; // referencia integracao BBC RNTRC = modalidadeTerceiro.RNTRC,
            return retorno;
        }

        private DadosTransportadorTypeTransportador ObterTransportadorTransportador()
        {
            DadosTransportadorTypeTransportador retorno = new DadosTransportadorTypeTransportador();
            retorno.CNPJCPFTransportador = _ciot.Transportador.CPF_CNPJ_SemFormato.ToString();
            retorno.NomeTransportador = _ciot.Transportador.Nome;
            retorno.RNTRC = _modalidadeTerceiro.RNTRC; // referencia integracao BBC RNTRC = modalidadeTerceiro.RNTRC,
            return retorno;
        }

        private void GravarArquivoIntegracao(string request, string response, string extensaoArquivo)
        {
            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request ?? string.Empty, extensaoArquivo, _unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response ?? string.Empty, extensaoArquivo, _unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Mensagem = _ciot.Mensagem
            };

            _repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
            _ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
            if (_ciot.Codigo > 0)
                _repCIOT.Atualizar(_ciot);
            else
                _repCIOT.Inserir(_ciot);
        }


        private int TransformeTipoVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            int Tipo_Rodocred = 3;

            if (veiculo == null || veiculo.ModeloVeicularCarga == null)
                return Tipo_Rodocred;

            List<string> lstModelo6 = new List<string>();
            lstModelo6.Add("90000001");
            lstModelo6.Add("90000045");
            lstModelo6.Add("90000002");
            lstModelo6.Add("90000046");

            if (lstModelo6.Contains(veiculo.ModeloVeicularCarga.CodigoIntegracao))
                Tipo_Rodocred = 6;

            return Tipo_Rodocred;
        }

        #endregion


        #region Métodos Privados - Autenticação
        private ServicoRodocred.IdentificacaoIntegracaoType ObterAutenticacao()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _rodocredSoapClient = ObterClient(_configuracao.URL);
            _rodocredSoapClient.Endpoint.EndpointBehaviors.Add(_inspector);
            string mensagem;
            try
            {
                ServicoRodocred.AutenticarClienteRequest request = new ServicoRodocred.AutenticarClienteRequest
                {
                    IdClienteRodocred = _configuracao.IDCliente.ToInt(),
                    LoginIntegracao = _configuracao.Login,
                    ChaveAutenticacao = _configuracao.ChaveAutenticacao
                };

                ServicoRodocred.AutenticarClienteResponse retorno = _rodocredSoapClient.AutenticarCliente(request);

                if (retorno.RetornoMensagem.StatusRetorno == ServicoRodocred.AutenticarClienteResponseRetornoMensagemStatusRetorno.SUCESSO)
                    return retorno.RetornoMensagem.IdentificacaoIntegracao;
                else
                    mensagem = retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falhar ao autenticar na integração com a DBTrans/Rodocred.";
            }
            return null;
        }
        private ServicoRodocred.RodocredSoapClient ObterClient(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoRodocred.RodocredSoapClient(binding, endpointAddress);
        }



        #endregion
    }
}

