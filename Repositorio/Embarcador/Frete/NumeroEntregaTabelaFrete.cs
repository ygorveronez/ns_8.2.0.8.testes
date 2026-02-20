using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class NumeroEntregaTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete>
    {
        public NumeroEntregaTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete> BuscarPorCodigos(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }
    }
}
