using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.OrdemEmbarque
{
    public sealed class ConfirmacaoOrdemEmbarqueRequest
    {
        public string DataHora { get; set; }
        public string NumeroOrdemEmbarque { get; set; }
        public int ProtocoloTMSCarga { get; set; }
        public bool Validado { get; set; }
        public List<OrdemEmbarqueRetornoValidacao> Validacoes { get; set; }
    }
}
