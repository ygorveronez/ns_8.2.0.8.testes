using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Seguros
{
    public class ApoliceSeguraAutorizacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga>
    {
        public ApoliceSeguraAutorizacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga> BuscarNaoExtornadasPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.SituacaoAutorizacaoApolice != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice.AutorizacaoExtornada select obj;
            return result.Fetch(obj => obj.ApoliceSeguro).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga> BuscarPorCargaESituacao(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.SituacaoAutorizacaoApolice == situacao select obj;
            return result.ToList();
        }
    }
}
