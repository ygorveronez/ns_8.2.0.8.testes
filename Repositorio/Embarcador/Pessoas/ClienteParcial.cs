using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteParcial : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteParcial>
    {
        public ClienteParcial(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Pessoas.ClienteParcial BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteParcial>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteParcial> BuscarPorSituacaoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteParcial>();
            var result = from obj in query where obj.SituacaoIntegracao == situacaoIntegracao select obj;
            return result.Take(limite).ToList();
        }

    }
}
