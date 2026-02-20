using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class PalletTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete>
    {
        public PalletTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete> BuscarPorCodigos(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }
    }
}
