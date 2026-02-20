using System;
using System.IO;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMDFe" in both code and config file together.
    [ServiceContract]
    public interface IMDFe
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFes(int? protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFesPorCarga(int? protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFesPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> BuscarMDFePorProtocolo(int? protocoloMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno);

        [OperationContract]
        Retorno<bool> SolicitarEncerramentoMDFePorChaveCTe(string chaveCTe);

        [OperationContract]
        Retorno<string> EnviarArquivoXMLMDFe(Stream arquivo);

        [OperationContract]
        Retorno<Dominio.Enumeradores.StatusMDFe> ConsultaStatusMDFePorChaveCTe(string chaveCTe);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.MDFe.ProtocoloMDFeManual> SolicitarEmissaoMDFe(Dominio.ObjetosDeValor.WebService.MDFe.MDFeManual mdfeManual);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFesPorMDFeManual(int? protocoloIntegracaoMDFeManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> EnviarMDFeAquaviario(Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario);

        [OperationContract]
        Retorno<bool> AtualizarSituacaoMDFeAquaviario(string chaveMDFe, Dominio.Enumeradores.StatusMDFe statusMDFe, string protocolo, DateTime dataEvento, string mensagemRetornoSefaz, string motivo);
    }
}
