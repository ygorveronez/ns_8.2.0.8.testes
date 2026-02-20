using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.MDFe
{
    public class MDFe
    {
        public int Protocolo { get; set; }

        public string Chave { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string DataEmissao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa TransportadoraEmitente { get; set; }

        public string UFOrigem { get; set; }

        public string UFDestino { get; set; }

        public string XML { get; set; }

        public string XMLAutorizacao { get; set; }

        public string PDF { get; set; }

        public Dominio.Enumeradores.StatusMDFe StatusMDFe { get; set; }

        public List<int> ProtocolosDeCTe { get; set; }
        public List<string> ChavesDeCTe { get; set; }
        public string ProtocoloAutorizacao { get; set; }
        public string ProtocoloEncerramento { get; set; }
        public string ProtocoloCancelamento { get; set; }
        public DateTime DataAutorizacao { get; set; }
        public string MensagemRetornoSefaz { get; set; }
        public string NumeroCarga { get; set; }
    }
}
