using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escalas
{
    public class EscalaOrigemDestino : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino>
    {
        #region Construtores

        public EscalaOrigemDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino BuscarPorCodigo(int codigo)
        {
            var consultaEscalaOrigemDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino>()
                .Where(o => o.Codigo == codigo);

            return consultaEscalaOrigemDestino.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> BuscarPorGeracaoEscala(int codigoGeracaoEscala)
        {
            var consultaEscalaOrigemDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino>()
                .Where(o => o.ExpedicaoEscala.GeracaoEscala.Codigo == codigoGeracaoEscala);

            consultaEscalaOrigemDestino = consultaEscalaOrigemDestino
                .Fetch(o => o.CentroCarregamento)
                .Fetch(o => o.ClienteDestino)
                .Fetch(o => o.ExpedicaoEscala).ThenFetch(o => o.ProdutoEmbarcador).ThenFetch(o => o.GrupoProduto)
                .OrderBy(o => o.CentroCarregamento.Descricao).ThenBy(o => o.ClienteDestino.Nome).ThenBy(o => o.ExpedicaoEscala.ProdutoEmbarcador.Descricao);

            return consultaEscalaOrigemDestino.ToList();
        }

        #endregion
    }
}
