using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService
{
    public class Paginacao<T>
    {
        public int NumeroTotalDeRegistro { get; set; }
        public List<T> Itens { get; set; }
    }
}
