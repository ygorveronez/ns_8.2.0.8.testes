using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class ParametrosContratoFreteTransportadorValorFreteMinimo
    {
        public int CodigoContratoFreteTransportador { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoTipoCarga { get; set; }

        public List<int> ListaCodigoLocalidadeDestino { get; set; }

        public List<int> ListaCodigoLocalidadeOrigem { get; set; }

        public List<double> ListaCpfCnpjClienteDestino { get; set; }

        public List<double> ListaCpfCnpjClienteOrigem { get; set; }

        public List<string> ListaUfDestino { get; set; }

        public List<string> ListaUfOrigem { get; set; }
    }
}
