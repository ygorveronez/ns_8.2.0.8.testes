using System;

namespace Dominio.ObjetosDeValor.WebService.Rest
{
    public class CargaPendenteProcessarDocumentoTransporte
    {
        public int CodigoCarga { get; set; }

        public long CodigoArquivo { get; set; }

        public DateTime Mindata { get; set; }
    }
}
