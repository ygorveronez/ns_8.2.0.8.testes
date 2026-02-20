using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Pessoas
{
    public class ClienteConfiguracao : RepositorioBase<Dominio.Entidades.Pessoas.ClienteConfiguracao>
    {
        public ClienteConfiguracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Pessoas.ClienteConfiguracao> Consultar(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteConfiguracao>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteConfiguracao>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            return result.Count();
        }
    }
}
