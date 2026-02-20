using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.NFS
{
    public class NFSManual
    {
        public int Numero { get; set; }
        public string DataEmissao { get; set; }
        public List<NFSManualServicoPrestado> ServicosPrestados { get; set; }
        public string Imagem { get; set; }

    }
}
