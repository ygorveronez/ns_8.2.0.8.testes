using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class DeliveryReceiptCreate : IntegracaoSuperApp
    {
        #region Construtores
        public DeliveryReceiptCreate(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, unitOfWorkAdmin, clienteMultisoftware)
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

                string jsonRequisicao = integracaoSuperApp.ArquivoRequisicao != null ? obterJsonRequisicao(integracaoSuperApp.ArquivoRequisicao) : integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                EventoDeliveryReceiptCreate eventoDeliveryReceiptCreate = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoDeliveryReceiptCreate>(jsonRequisicao);

                if (eventoDeliveryReceiptCreate == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");

                _codigoEmpresaMultisoftware = eventoDeliveryReceiptCreate.Data.Travel.ExternalInfo.Tags[0].ToInt();
                if (_codigoEmpresaMultisoftware == 0) _codigoEmpresaMultisoftware = _clienteMultisoftware.Codigo;
                _cpfMotorista = eventoDeliveryReceiptCreate.Data.Driver.Document.Value;
                _integracaoSuperApp = integracaoSuperApp;

                string IDTrizy = eventoDeliveryReceiptCreate.Data.Travel._id; //Carga.
                int codigoCarga = eventoDeliveryReceiptCreate.Data.Travel.ExternalID.ToInt(); //Codigo Carga.

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorIDIdentificacaoTrizy(IDTrizy) ?? repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ServicoException($"Carga não encontrada. ID: {IDTrizy}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
                else if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ServicoException($"Carga Cancelada. ID: {IDTrizy}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                ObterDadosEvidencias(eventoDeliveryReceiptCreate.Data.Evidences);

                bool processouCanhoto = false;
                IDTrizy = eventoDeliveryReceiptCreate.Data.StoppingPointDocument?._id ?? string.Empty; //Canhoto.
                int codigoCanhoto = eventoDeliveryReceiptCreate.Data.StoppingPointDocument?.ExternalId.ToInt() ?? 0; //Codigo Canhoto.
                if (!string.IsNullOrEmpty(IDTrizy) || codigoCanhoto > 0)
                {
                    Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto) ?? repCanhoto.BuscarPorIdTrizy(IDTrizy) ?? throw new ServicoException($"Não foi localizado um canhoto compatível com a entrega. ID: {IDTrizy}");
                    AtualizarCargaIntegracaoSuperApp(_integracaoSuperApp, canhoto.Carga, null);

                    string retorno = ProcessarImagemCanhoto(canhoto, repCanhoto, eventoDeliveryReceiptCreate);

                    if (string.IsNullOrEmpty(retorno))
                        processouCanhoto = true;
                    else
                        retornoIntegracaoSuperApp.Mensagem = retorno;
                }

                IDTrizy = eventoDeliveryReceiptCreate.Data.StoppingPoint?._id ?? string.Empty; //Coleta/Entrega.
                string[] codigoExterno = eventoDeliveryReceiptCreate.Data.StoppingPoint?.ExternalId?.Split(';');
                int codigoCargaEntrega = codigoExterno != null && codigoExterno.Length > 0 ? codigoExterno[0].ToInt() : 0;  //Codigo entrega
                double identificacaoCliente = codigoExterno != null && codigoExterno.Length > 1 ? codigoExterno[1].ToDouble() : 0; //cpf cnpj cliente

                if (!string.IsNullOrEmpty(IDTrizy) || codigoCargaEntrega > 0)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(IDTrizy) ?? repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega) ?? repCargaEntrega.BuscarPorCargaECliente(carga.Codigo, identificacaoCliente) ?? throw new ServicoException($"Não foi localizada Coleta/Entrega. ID:{IDTrizy} | Código:{codigoCargaEntrega}");
                    AtualizarCargaIntegracaoSuperApp(_integracaoSuperApp, cargaEntrega.Carga, cargaEntrega);
                    ProcessarDadosCargaEntrega(cargaEntrega, eventoDeliveryReceiptCreate, processouCanhoto, true);
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
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar " + TipoEventoApp.DeliveryReceiptCreate.ObterDescricao();
                Servicos.Log.TratarErro(ex);
            }
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}
