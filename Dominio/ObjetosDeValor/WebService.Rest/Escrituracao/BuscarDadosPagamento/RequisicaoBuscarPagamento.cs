using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento
{
    public class RequisicaoBuscarDadosPagamento
    {
        public int? ProtocoloDocumento { get; set; }

        public string ChaveDocumento { get; set; }

        public DateTime? dataEmissaoDocumentoInicial { get; set; }

        public DateTime? dataEmissaoDocumentoFinal { get; set; }

        public int Inicio { get; set; }

        public int Limite { get; set; }

    }
}
