using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public sealed class OcorrenciaColetaEntrega
    {
        public string CpfCnpjCliente { get; set; }

        public string Data { get; set; }

        public int Protocolo { get; set; }

        public List<OcorrenciaColetaEntregaNotaFiscal> NotasFiscais { get; set; }

        public string Tipo { get; set; }
    }
}
