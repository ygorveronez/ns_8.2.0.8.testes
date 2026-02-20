using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class PesoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete>
    {
        public PesoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> BuscarPorTabelaFrete(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigo select obj;

            return result.Fetch(obj => obj.UnidadeMedida).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> BuscarPorCodigos(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

    }
}
