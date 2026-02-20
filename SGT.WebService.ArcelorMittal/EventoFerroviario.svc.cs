using System;
using System.ServiceModel;
using Dominio.ObjetosDeValor.Enumerador;

namespace SGT.WebService.ArcelorMittal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EventoFerroviario" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EventoFerroviario.svc or EventoFerroviario.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(/*Sets the wdl:service name*/ Name = "EventoService", Namespace = "http://tempuri.org/")]
    public class EventoFerroviario : WebServiceBase, SGT.WebService.ArcelorMittal.IEventoFerroviario
    {

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        public TRetornoEnvio receberEventoFerroviario(TEventoFerroviario EventoFerroviario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            ValidarToken();

            try
            {
                //De momento registrar o log do objeto recebido. cada uma das funcionalidades que esse evento vai criar no ME serão descritas em tarefas posteriores;

                Servicos.Log.TratarErro($"Evento Recebido versao 6.14", "FerroviarioMRS");
                //Servicos.Log.TratarErro($"Protocolo: {EventoFerroviario.data.protocoloEnvio} Nº Eventos: {EventoFerroviario.data.eventos.Length}", "FerroviarioMRS");
                //for (int i = 0; i < EventoFerroviario.EventoFerroviariodata.eventos.Length; i++)
                //{
                //    Servicos.Log.TratarErro($"Tipo Evento : {EventoFerroviario.data.eventos[i].tipoEvento} Codigo: {EventoFerroviario.data.eventos[i].codigoEvento}", "FerroviarioMRS");
                //}

                return RetornoOk();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return RetornoNOk(ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }



        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
        }


        #region Metodos Privados

        private TRetornoEnvio RetornoOk()
        {
            TRetornoEnvio retorno = new TRetornoEnvio();
            MRetornoEnvio mRetornoEnvio = new MRetornoEnvio();
            retorno.RetornoEnvio = mRetornoEnvio;
            mRetornoEnvio.protocoloEnvio = "1";
            mRetornoEnvio.versao = "0";
            mRetornoEnvio.statusEnvio = TStatus.OK;
            mRetornoEnvio.CNPJFerrovia = "";
            return retorno;
        }

        private TRetornoEnvio RetornoNOk(string erro)
        {
            var retorno = new TRetornoEnvio();

            return retorno;

        }

        #endregion
    }
}