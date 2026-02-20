using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class DistanciaTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete>
    {
        public DistanciaTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete> BuscarPorCodigos(int[] codigos)
        {
            const int maxParams = 2000; 
            var result = new List<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete>();

            foreach (var chunk in SplitArray(codigos, maxParams))
            {
                var partial = this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete>()
                    .Where(x => chunk.Contains(x.Codigo))
                    .ToList();

                result.AddRange(partial);
            }

            return result;
        }

        private static IEnumerable<int[]> SplitArray(int[] source, int size)
        {
            for (int i = 0; i < source.Length; i += size)
            {
                yield return source.Skip(i).Take(size).ToArray();
            }
        }


    }
}
