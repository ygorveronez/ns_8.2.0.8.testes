using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class DriverReceiptCreate : IntegracaoSuperApp
    {
        #region Construtores
        public DriverReceiptCreate(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, unitOfWorkAdmin, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _clienteMultisoftware = clienteMultisoftware;
        }
        #endregion

        #region Métodos Públicos
        public void ProcessarEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, out RetornoIntegracaoSuperApp retornoIntegracaoSuperApp)
        {
            retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();

            try
            {
                Servicos.Log.TratarErro("Inicio Processar canhoto", "IntegracaoSuperAPPOutrosTipos");

                string jsonRequisicao = integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                EventoDriverReceiptCreate eventoDriverReceiptCreate = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoDriverReceiptCreate>(jsonRequisicao);

                if (eventoDriverReceiptCreate == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");

                _codigoEmpresaMultisoftware = eventoDriverReceiptCreate.Data.Travel.ExternalInfo.Tags[0].ToInt();
                if (_codigoEmpresaMultisoftware == 0) _codigoEmpresaMultisoftware = _clienteMultisoftware.Codigo;
                _cpfMotorista = eventoDriverReceiptCreate.Data.Driver.Document.Value;
                _integracaoSuperApp = integracaoSuperApp;

                string IDTrizy = eventoDriverReceiptCreate.Data.Travel._id; //Carga.
                int codigoCarga = eventoDriverReceiptCreate.Data.Travel.ExternalID.ToInt(); //Codigo Carga.

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorIDIdentificacaoTrizy(IDTrizy) ?? repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ServicoException($"Carga não encontrada. ID: {IDTrizy}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
                else if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ServicoException($"Carga Cancelada. ID: {IDTrizy}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                ObterDadosChecklistFlow(eventoDriverReceiptCreate.Data.Response);

                bool processouCanhoto = false;
                IDTrizy = eventoDriverReceiptCreate.Data.StoppingPointDocument?._id ?? string.Empty; //Canhoto.
                int codigoCanhoto = eventoDriverReceiptCreate.Data.StoppingPointDocument?.ExternalId.ToInt() ?? 0; //Codigo Canhoto.
                if (!string.IsNullOrEmpty(IDTrizy) || codigoCanhoto > 0)
                {
                    Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorIdTrizy(IDTrizy) ?? repCanhoto.BuscarPorCodigo(codigoCanhoto) ?? throw new ServicoException($"Não foi localizado um canhoto compatível com a entrega. ID: {IDTrizy}");
                    AtualizarCargaIntegracaoSuperApp(_integracaoSuperApp, canhoto.Carga, null);

                    string retorno = ProcessarImagemCanhoto(canhoto, repCanhoto, eventoDriverReceiptCreate, _dadosEvidencias);

                    if (string.IsNullOrEmpty(retorno))
                        processouCanhoto = true;
                    else
                        retornoIntegracaoSuperApp.Mensagem = retorno;
                }

                IDTrizy = eventoDriverReceiptCreate.Data.StoppingPoint?._id ?? string.Empty;
                string[] codigoExterno = eventoDriverReceiptCreate.Data.StoppingPoint?.ExternalId?.Split(';');
                int codigoCargaEntrega = codigoExterno != null && codigoExterno.Length > 0 ? codigoExterno[0].ToInt() : 0;
                double identificacaoCliente = codigoExterno != null && codigoExterno.Length > 1 ? codigoExterno[1].ToDouble() : 0;

                if (!string.IsNullOrEmpty(IDTrizy) || codigoCargaEntrega > 0)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(IDTrizy) ?? repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega) ?? repCargaEntrega.BuscarPorCargaECliente(carga.Codigo, identificacaoCliente) ?? throw new ServicoException($"Não foi localizada Coleta/Entrega. ID:{IDTrizy} | Código:{codigoCargaEntrega}");
                    AtualizarCargaIntegracaoSuperApp(_integracaoSuperApp, cargaEntrega.Carga, cargaEntrega);
                    ProcessarDadosCargaEntrega(cargaEntrega, eventoDriverReceiptCreate, processouCanhoto, false);
                }

                Servicos.Log.TratarErro($"SuperApp - EnviarCanhoto - Imagem salva com sucesso", "EnviarCanhoto");
                retornoIntegracaoSuperApp.Sucesso = true;

                Servicos.Log.TratarErro("Fim Processar canhoto", "IntegracaoSuperAPPOutrosTipos");

            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex);
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex);
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar " + TipoEventoApp.DriverReceiptCreate.ObterDescricao();
                Servicos.Log.TratarErro(ex);
            }
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}
