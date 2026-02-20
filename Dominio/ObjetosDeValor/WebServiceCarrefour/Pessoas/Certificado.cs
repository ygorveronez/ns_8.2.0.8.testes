using System;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas
{
    public sealed class Certificado
    {
        public bool CertificadoAtivo { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }
    }
}
