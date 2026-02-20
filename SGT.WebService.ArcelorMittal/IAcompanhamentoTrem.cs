//using Dominio.Ferroviario.AcompanhamentoTrem;
//using Dominio.Ferroviario;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.ServiceModel;
//using System.Text;

//namespace SGT.WebService.ArcelorMittal
//{
//    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IEventoFerroviario" no arquivo de código e configuração ao mesmo tempo.

//    [ServiceContract(
//        /*Set wsdl targetNamespae*/ Namespace = "http://xmlns.mrs.com.br/iti/",
//        /*Set wsdl portType name*/  Name = "AcompanhamentoTrem")]
//    public interface IAcompanhamentoTrem
//    {
//        [OperationContract(
//           /*Set wsdl request action*/  Action = "http://xmlns.mrs.com.br/iti/AcompanhamentoTrem/ReceberAcompanhamentoTrem",
//           /*Set wsdl response action*/ ReplyAction = "http://xmlns.mrs.com.br/iti/tipos/retorno",
//           /*Set wsdl operation name*/ Name = "ReceberAcompanhamentoTrem")]

//        MRetornoEnvio ReceberAcompanhamentoTrem(MAcompanhamentoTrem EventoFerroviario);
//    }
//}
