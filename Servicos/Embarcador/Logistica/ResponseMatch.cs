using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    class ResponseMatch
    {
        public string code { get; set; }
        public List<Tracepoint> tracepoints { get; set; }
    }
}
