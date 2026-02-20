using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestEnviarNFesParaAnaliseNaoConformidade
    {
        public int clienteMultisoftware { get; set; }

        public int codigoCargaEntrega { get; set; }

        public List<string> chavesNFe { get; set; }
    }
}
