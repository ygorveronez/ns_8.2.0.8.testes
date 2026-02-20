using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaXMLNotaFiscalImportacao
    {
        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }
        
        public int NumeroNotaFiscal { get; set; }
    }
}
