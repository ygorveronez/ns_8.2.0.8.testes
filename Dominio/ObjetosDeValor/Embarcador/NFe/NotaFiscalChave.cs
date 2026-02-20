using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class NotaFiscalChave
    {
        public string ChaveNFe { get; set; }
        public StatusNfe Status { get; set; }
        public string NumeroFatura { get; set; }
        public TipoNotaFiscalIntegrada TipoNotaFiscalIntegrada { get; set; }
    }
}
