using Dominio.ObjetosDeValor.WebService.NFS;
using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "INFS" in both code and config file together.
    [ServiceContract]
    public interface INFS
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSs(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorCarga(int protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarNFSsCompletasPorCarga(int protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFSPorProtocolo(int protocoloNFS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>> BuscarNFSesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> BuscarNFSAguardandoIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> EnviarNFSe(List<Dominio.ObjetosDeValor.WebService.NFS.NFS> notasFiscais);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoNFS(int protocoloNFS);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoNFSComplementar(int protocoloNFS);

        [OperationContract]
        Retorno<bool> InformarRejeicaoNFS(int protocoloNFS, string msgRejeicao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorOcocorrencia(RequestNFSOcorrencia dadosRequest);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>> ConsultarDocumentosParaEmissaoDeNotaManual();

        [OperationContract]
        Retorno<int> GerarNotaManual(GerarNotaManual gerarNotaManual);

        [OperationContract]
        Retorno<int> EnviarXMLNFSManual(int protocolo, string nfseBase64);

        [OperationContract]
        Retorno<int> EnviarPDFNFSManual(int protocolo, string pdfBase64);
    }
}
