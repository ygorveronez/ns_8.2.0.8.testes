using System.Collections.Generic;

namespace Emissao.Integracao.Rest.Class
{
    public class Paginacao<T>
    {
        public int NumeroTotalDeRegistro { get; set; }
        public List<T> Itens { get; set; }
    }
}