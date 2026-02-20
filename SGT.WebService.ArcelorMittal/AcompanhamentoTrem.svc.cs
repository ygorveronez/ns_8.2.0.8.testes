//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.ServiceModel;
//using System.Text;
//using Dominio.Ferroviario.AcompanhamentoTrem;
//using Dominio.Ferroviario;
//using Dominio.ObjetosDeValor.Enumerador;
//using System.ServiceModel.Channels;

//namespace SGT.WebService.ArcelorMittal
//{
//    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EventoFerroviario" in code, svc and config file together.
//    // NOTE: In order to launch WCF Test Client for testing this service, please select EventoFerroviario.svc or EventoFerroviario.svc.cs at the Solution Explorer and start debugging.
//    [ServiceBehavior(/*Sets the wdl:service name*/ Name = "AcompanhamentoTrem", Namespace = "http://xmlns.mrs.com.br/iti/")]
//    public class AcompanhamentoTrem : WebServiceBase, IAcompanhamentoTrem
//    {

//        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

//        public MRetornoEnvio ReceberAcompanhamentoTrem(MAcompanhamentoTrem acompanhamentoTrem)
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

//            Dominio.Entidades.WebService.Integradora integradora = ValidarTokenMRS(unitOfWork);

//            try
//            {
//                //De momento registrar o log do objeto recebido. cada uma das funcionalidades que esse evento vai criar no ME serão descritas em tarefas posteriores;

//                Servicos.Log.TratarErro($"Acompanhamento trem Recebido", "FerroviarioMRS");
//                Servicos.Log.TratarErro($"Processo Envio: {acompanhamentoTrem.data?.NomeProcessoEnvio   } trem: {acompanhamentoTrem.data?.Trem?.IdentificadorTrem}", "FerroviarioMRS");
                

//                return RetornoOk();
//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                Servicos.Log.TratarErro(ex);

//                return RetornoNOk(ex.Message);
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }


//        protected override OrigemAuditado ObterOrigemAuditado()
//        {
//            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
//        }


//        #region Metodos Privados

//        private Dominio.Entidades.WebService.Integradora ValidarTokenMRS(Repositorio.UnitOfWork unitOfWork = null)
//        {
//            bool controlarUnitOfWork = false;

//            if (unitOfWork == null)
//            {
//                unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
//                controlarUnitOfWork = true;
//            }

//            OperationContext context = OperationContext.Current;
//            MessageProperties properties = context.IncomingMessageProperties;
//            MessageHeaders headers = context.IncomingMessageHeaders;

//            try
//            {
//                //esse metodo é usado pela Ferrovia MRS eles nao podem enviar o token. entao para usar deve criar na t_integradora esse token direto.
//                string token = "rO0ABXehACZ3ZWJsb2dpYy5kaWFnbm9zdGljcy5EaWFnbm9zd8";  //Convert.ToString(headers.GetHeader<string>("Token", "Token"));

//                Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
//                Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorToken(token);

//                if (integradora != null && integradora.Ativo)
//                {
//                    RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

//                    string ip = endpoint.Address;

//                    SetarIntegradora(integradora, ip);
//                    return integradora;
//                }
//                else
//                {
//                    Servicos.Log.TratarErro("Token " + token + " inválido.");

//                    throw new FaultException("Token inválido. Verifique se o token informado é o mesmo autorizado para a integração.");
//                }
//            }
//            catch (Exception)
//            {
//#if DEBUG
//                return null;
//#endif
//                throw;
//            }
//            finally
//            {
//                if (controlarUnitOfWork)
//                    unitOfWork.Dispose();
//            }
//        }

//        private void SetarIntegradora(Dominio.Entidades.WebService.Integradora integradora, string ipOrigem)
//        {
//            if (_auditado == null)
//            {
//                _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
//                {
//                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
//                    OrigemAuditado = ObterOrigemAuditado()
//                };
//            }

//            _auditado.Integradora = integradora;
//            _auditado.IP = Utilidades.String.Left(ipOrigem, 50);
//        }

//        private MRetornoEnvio RetornoOk()
//        {
//            Dominio.Ferroviario.MRetornoEnvio retorno = new MRetornoEnvio();
//            retorno.data.statusEnvio = TStatus.OK;

//            return retorno;

//        }

//        private MRetornoEnvio RetornoNOk(string erro)
//        {
//            Dominio.Ferroviario.MRetornoEnvio retorno = new MRetornoEnvio();
//            retorno.data.statusEnvio = TStatus.NOK;
//            retorno.data.Item.Erro = new MRetornoEnvioErrosErro[1];
//            retorno.data.Item.Erro[0].mensagemErro = erro;

//            return retorno;

//        }

//        #endregion
//    }
//}
