using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.OrdemEmbarque
{
    public sealed class OrdemEmbarqueRetorno
    {
        public string Data { get; set; }

        public List<OrdemEmbarque> OrdensEmbarque { get; set; }

        public int ProtocoloIntegracaoCarga { get; set; }

        public bool Validada { get; set; }

        public List<OrdemEmbarqueRetornoValidacao> Validacoes { get; set; }
    }
}
