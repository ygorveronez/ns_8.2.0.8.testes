using System;

namespace Dominio.ObjetosDeValor.EDI.OCOREN
{
    public class NotaFiscalOcorrencia
    {
        public string CNPJEmissorNotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public string ChaveNotaFiscal { get; set; }
        public string ProtocoloNotaFiscal { get; set; }
        public string CodigoOcorrenciaEntrega { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public DateTime DataEvento { get; set; }
        public string CodigoObservacaoOcorrenciaEntrada { get; set; }
        public string TextoExplicativo { get; set; }
        public string NumeroRomaneio { get; set; }
        public string NumeroPedido { get; set; }
        public string Filler { get; set; }
        public CTeOcorrencia CTeOcorrencia { get; set; }

        public string CNPJCarga { get; set; }
        public int NumeroCarregamento { get; set; }
    }
}
