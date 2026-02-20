using System.Collections.Generic;

namespace SGT.Mobile
{
    public class Paginacao<T>
    {
        public int NumeroTotalDeRegistro { get; set; }
        public List<T> Itens { get; set; }
    }
}