using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class FiltroPesquisaSerieDocumentos
    {
        public int CodigoEmpresa { get; set; }
        public int CodigoSerie { get; set; }
        public List<int> ModelosDocumentosFiscais { get; set; }
    }
}
