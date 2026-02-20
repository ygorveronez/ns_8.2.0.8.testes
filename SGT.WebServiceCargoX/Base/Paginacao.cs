using System.Collections.Generic;

namespace SGT.WebServiceCargoX
{
    public class Paginacao<T>
    {
        public int NumeroTotalDeRegistro { get; set; }
        public List<T> Itens { get; set; }
    }
}