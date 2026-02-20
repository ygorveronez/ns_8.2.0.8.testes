using System.Collections.Generic;

namespace SGT.WebService.ArcelorMittal
{
    public class Paginacao<T>
    {
        public int NumeroTotalDeRegistro { get; set; }
        public List<T> Itens { get; set; }

        public static Paginacao<T> CreateFrom(Dominio.ObjetosDeValor.WebService.Paginacao<T> paginacao)
        {
            if (paginacao == null)
                return null;

            return new Paginacao<T>
            {
                NumeroTotalDeRegistro = paginacao.NumeroTotalDeRegistro,
                Itens = paginacao.Itens
            };
        }
    }
}