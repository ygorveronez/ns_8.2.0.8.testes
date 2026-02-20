using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class MensagemRetorno
    {
        public string Type { get; set; }
        public string Message { get; set; }

        public BodyErrors BodyErrors { get; set; }
    }

    public class BodyErrors
    {
        public List<Issue> Issues { get; set; }
        public string Name { get; set; }
    }

    public class Issue
    {
        public string Code { get; set; } = "";
        public string Expected { get; set; } = "";
        public string Received { get; set; } = "";
        public List<string> Path { get; set; }
        public string Message { get; set; } = "";
        public string Minimum { get; set; } = "";

    }
}
