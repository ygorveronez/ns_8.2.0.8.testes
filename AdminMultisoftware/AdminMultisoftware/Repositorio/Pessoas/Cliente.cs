using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace AdminMultisoftware.Repositorio.Pessoas
{
    public class Cliente : RepositorioBase<Dominio.Entidades.Pessoas.Cliente>
    {
        public Cliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Pessoas.Cliente BuscarPorCNPJ(string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Cliente>();
            var result = from obj in query where obj.CNPJ == cnpj select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Pessoas.Cliente> Consultar(string nome, string cnpj, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Cliente>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.RazaoSocial.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cnpj))
                result = result.Where(obj => obj.CNPJ.Contains(cnpj));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string nome, string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Cliente>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.RazaoSocial.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cnpj))
                result = result.Where(obj => obj.CNPJ.Contains(cnpj));

            return result.Count();
        }
    }
}
