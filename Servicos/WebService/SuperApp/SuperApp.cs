using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.WebService.SuperApp
{
    public class SuperApp
    {
        #region Propriedades Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;

        #endregion

        #region Construtores

        public SuperApp(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
        }

        #endregion

        #region Métodos Públicos

        public async Task<Retorno<bool>> ProcessarEventoSuperAppAsync(Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.EventoSuperApp eventoSuperApp, CancellationToken cancellationToken, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(eventoSuperApp);
                Servicos.Log.GravarDebug($"ProcessarEventoSuperApp: {requestJson}", "WebHookEventos");

                if (string.IsNullOrEmpty(eventoSuperApp.Event))
                    throw new ServicoException("Tipo de evento é obrigatório.");

                TipoEventoApp tipoEvento = TipoEventoAppHelper.ObterEnumPelaDescricao(eventoSuperApp.Event);

                List<TipoEventoApp> listaTipoEventos = new List<TipoEventoApp>
                {
                    TipoEventoApp.EventsSubmit,
                    TipoEventoApp.DeliveryReceiptCreate,
                    TipoEventoApp.ChatSendMessage,
                    TipoEventoApp.OccurrenceCreate,
                    TipoEventoApp.DriverReceiptCreate,
                    TipoEventoApp.DriverFreightContactCreate,
                    TipoEventoApp.DriverOccurrenceCreate,
                    TipoEventoApp.NotDelivered,
                    TipoEventoApp.PartialDelivery,
                };

                Servicos.Embarcador.Cargas.CargaOfertaIntegracao servicoCargaOfertaIntegracao = new Servicos.Embarcador.Cargas.CargaOfertaIntegracao(_unitOfWork);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repositorioIntegracaoSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(_unitOfWork, cancellationToken);

                if (!listaTipoEventos.Contains(tipoEvento)) throw new ServicoException($"Tipo de evento não permitido.");

                string IDTrizy = eventoSuperApp.Data.StoppingPoint?._id ?? string.Empty; //Coleta/Entrega.
                string[] codigoExterno = eventoSuperApp.Data.StoppingPoint?.ExternalId?.Split(';');
                int codigoCargaEntrega = codigoExterno != null && codigoExterno.Length > 0 ? codigoExterno[0].ToInt() : 0;  //Codigo entrega
                double identificacaoCliente = codigoExterno != null && codigoExterno.Length > 1 ? codigoExterno[1].ToDouble() : 0; //cpf cnpj cliente
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;

                if (tipoEvento == TipoEventoApp.DriverFreightContactCreate)
                {
                    carga = await repCarga.BuscarPorCodigoAsync(eventoSuperApp.Data.Freight.ExternalId.ToInt(0));

                    Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyOfertarCarga);

                    await servicoCargaOfertaIntegracao.GerarIntegracaoOfertadeCarga(carga, tipoIntegracao, TipoIntegracaoOfertaCarga.Inativar, cancellationToken, auditado);
                }
                else
                {
                    carga = await repCarga.BuscarPorCodigoAsync(eventoSuperApp.Data.Travel.ExternalID.ToInt(0));
                    cargaEntrega = await repCargaEntrega.BuscarPorCodigoAsync(codigoCargaEntrega, cancellationToken) ?? await repCargaEntrega.BuscarPorIdTrizyAsync(IDTrizy, cancellationToken) ?? await repCargaEntrega.BuscarPorCargaEClienteAsync(carga?.Codigo ?? 0, identificacaoCliente);
                }

                Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp = new Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp();

                integracaoSuperApp.DataRecebimento = DateTime.Now;
                integracaoSuperApp.NumeroTentativas = 0;
                integracaoSuperApp.SituacaoProcessamento = SituacaoProcessamentoIntegracao.AguardandoProcessamento;
                integracaoSuperApp.DetalhesProcessamento = "Aguardando Processamento";
                integracaoSuperApp.TipoEvento = tipoEvento;
                integracaoSuperApp.Carga = carga;
                integracaoSuperApp.CargaEntrega = cargaEntrega;
                integracaoSuperApp.StringJsonRequest = requestJson;

                await repositorioIntegracaoSuperApp.InserirAsync(integracaoSuperApp);

                Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);

                integracaoSuperApp.StringJsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                await repositorioIntegracaoSuperApp.AtualizarAsync(integracaoSuperApp);

                return retorno;
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro($"Problemas ProcessarEventoSuperApp: {excecao.Message}", "WebHookEventos");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"Problemas ProcessarEventoSuperApp: {excecao.Message}", "WebHookEventos");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
        }

        public Retorno<bool> ProcessarPosicaoSuperApp(Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.EventoSendPosition eventoSendPosition)
        {
            try
            {
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(eventoSendPosition);
                Servicos.Log.GravarDebug($"ProcessarPosicaoSuperApp: {requestJson}", "WebHookPosicoes");

                if (string.IsNullOrEmpty(eventoSendPosition.Event))
                    throw new ServicoException("Tipo de evento é obrigatório.");

                TipoEventoApp tipoEvento = TipoEventoAppHelper.ObterEnumPelaDescricao(eventoSendPosition.Event);

                List<TipoEventoApp> listaTipoEventos = new List<TipoEventoApp> { TipoEventoApp.SendPosition };

                if (!listaTipoEventos.Contains(tipoEvento))
                    throw new ServicoException($"Tipo de evento ({tipoEvento.ObterDescricao()}) não permitido.");

                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaPorCPF(eventoSendPosition.Data.Driver.Document.Value);
                if (motorista == null) throw new ServicoException("Motorista não encontrado");

                int codigoCarga = eventoSendPosition.Data.Travel.ExternalID.ToInt(valorPadrao: 0);
                int codigoMotorista = motorista.Codigo;

                if (codigoCarga <= 0 || !CargaEmMonitoramento(_unitOfWork, codigoCarga))
                    return Retorno<bool>.CriarRetornoSucesso(true);

                Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramentoOuMotorista(_unitOfWork, codigoCarga, codigoMotorista);
                Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao repPosicaoPendenteIntegracao = new Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao(_unitOfWork);

                if (veiculo == null) throw new ServicoException("Veículo vinculado ao motorista não foi encontrado.");

                foreach (Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento.Position posicao in eventoSendPosition.Data.Positions)
                {
                    if (posicao.Location.Coordinates.Count <= 1) throw new ServicoException("Posição sem coordenadas válidas.");

                    double longitude = posicao.Location.Coordinates[0];
                    double latitude = posicao.Location.Coordinates[1];

                    Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao()
                    {
                        ID = posicao.PositionAt.ToUnixMillseconds(),
                        Data = posicao.PositionAt.ToLocalTime(),
                        DataVeiculo = posicao.PositionAt.ToLocalTime(),
                        DataCadastro = DateTime.Now,
                        IDEquipamento = motorista.Codigo.ToString(),
                        Veiculo = veiculo,
                        Ignicao = 1,
                        Latitude = latitude,
                        Longitude = longitude,
                        Velocidade = (int)Math.Floor(posicao.Speed * 3.6),
                        Temperatura = 0,
                        Descricao = $"{latitude}, {longitude} (M)",
                        NivelBateria = posicao.Battery.Level,
                        NivelSinalGPS = 0
                    };
                    repPosicaoPendenteIntegracao.Inserir(posicaoPendenteIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro($"Problemas ProcessarEventoSuperApp: {excecao.Message}", "WebHookPosicoes");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"Problemas ProcessarEventoSuperApp: {excecao.Message}", "WebHookPosicoes");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        #endregion

        #region Métodos Privados

        private bool CargaEmMonitoramento(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento config = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            if (config?.ValidarCargaEmMonitoramentoAoReceberPosicao ?? false)
                return repMonitoramento.CargaEstaEmMonitoramento(codigoCarga);
            else
                return true;
        }

        private void AtualizarVersaoAppMotoristasPorCPFEVersaoDiferente(Repositorio.UnitOfWork unitOfWork, string cpf, string versaoApp)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
            repositorioMotorista.AtualizarVersaoAppPorCPFMotorista(cpf, versaoApp);
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorCargaNoMonitoramentoOuMotorista(Repositorio.UnitOfWork unitOfWork, int codigoCarga, int codigoMotorista)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCarga(unitOfWork, codigoCarga);
            if (veiculo == null)
            {
                veiculo = ObterVeiculoPorMotorista(unitOfWork, codigoMotorista);
            }
            return veiculo;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorCarga(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                return carga?.Veiculo ?? null;
            }
            return null;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorMotorista(Repositorio.UnitOfWork unitOfWork, int codigoMotorista)
        {
            if (codigoMotorista > 0)
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                return repositorioVeiculo.BuscarPorMotorista(codigoMotorista);
            }
            return null;
        }

        public AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente ObterUsuarioMobileCliente(string CPFMotorista, int codigoCliente)
        {
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(_unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCFP(CPFMotorista);

            AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(_unitOfWorkAdmin);
            var usuarioMobileCliente = usuarioMobile != null ? repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente) : null;

            if (usuarioMobileCliente == null) throw new ServicoException("Sua sessão não permite consultar dados deste cliente");

            return usuarioMobileCliente;
        }

        #endregion

    }
}