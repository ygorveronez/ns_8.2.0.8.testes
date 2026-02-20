using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class GerarFaturaCompletaIntegracao
    {
        public string cnpjCpfCliente { get; set; }
        public string dataFatura { get; set; }
        public string codigoIntegracaoOperador { get; set; }
        public List<GerarFaturaCompletaIntegracaoDocumentos> documentos { get; set; }
    }

    public class GerarFaturaCompletaIntegracaoDocumentos
    {
        public string chaveAcesso { get; set; }
        public string cnpjCpfEmitente { get; set; }
        public string modelo { get; set; }
        public int? serie { get; set; }
        public int? numero { get; set; }
    }
}
