using CoreWCF;
using System.IO;
using System.Collections.Generic;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICanhotos" in both code and config file together.
    [ServiceContract]

    public interface ICanhotos
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosNotasFiscaisDigitalizados(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(int? protocolo);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.NFe.ResponseCanhoto>> ConfirmarIntegracoesDigitalizacaoCanhotosNotasFiscais(List<int> protocolos);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosPorCarga(int? protocolo);

        [OperationContract]
        Retorno<string> EnviarImagemCanhoto(Stream arquivo);

        [OperationContract]
        Retorno<bool> EnviarCanhoto(string token, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal);
    }
}
