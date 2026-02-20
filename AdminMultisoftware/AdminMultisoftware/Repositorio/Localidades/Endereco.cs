using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class Endereco : RepositorioBase<Dominio.Entidades.Localidades.Endereco>
    {
        public Endereco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Localidades.Endereco BuscarCEP(string cep)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Endereco>();
            var result = from obj in query where obj.CEP.Equals(cep) select obj;
            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Bairro)
                .Fetch(obj => obj.Geo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidades.Endereco> BuscarEnderecos(string logradouro, string bairro, string cep, string descricao, string codigoIBGE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Endereco>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Localidade.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(logradouro))
                result = result.Where(obj => obj.Logradouro.Contains(logradouro));

            if (!string.IsNullOrWhiteSpace(bairro))
                result = result.Where(obj => obj.Bairro.Descricao.Contains(bairro));

            if (!string.IsNullOrWhiteSpace(cep))
                result = result.Where(obj => obj.CEP.Equals(cep));

            if (!string.IsNullOrWhiteSpace(codigoIBGE))
                result = result.Where(obj => obj.Localidade.CodigoIBGE.Equals(codigoIBGE));

            return result.ToList();
        }

        public List<Dominio.Entidades.Localidades.Endereco> BuscarEnderecos(string logradouro, string bairro, string cep, string descricao, string codigoIBGE, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Endereco>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Localidade.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(logradouro))
                result = result.Where(obj => obj.Logradouro.Contains(logradouro));

            if (!string.IsNullOrWhiteSpace(bairro))
                result = result.Where(obj => obj.Bairro.Descricao.Contains(bairro));

            if (!string.IsNullOrWhiteSpace(cep))
                result = result.Where(obj => obj.CEP.Equals(cep));

            if (!string.IsNullOrWhiteSpace(codigoIBGE))
                result = result.Where(obj => obj.Localidade.CodigoIBGE.Equals(codigoIBGE));

            //result = result.Where(obj => !obj.Localidade.CodigoIBGE.Equals("0"));

            //return result.ToList();
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarEnderecos(string logradouro, string bairro, string cep, string descricao, string codigoIBGE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Endereco>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Localidade.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(logradouro))
                result = result.Where(obj => obj.Logradouro.Contains(logradouro));

            if (!string.IsNullOrWhiteSpace(bairro))
                result = result.Where(obj => obj.Bairro.Descricao.Contains(bairro));

            if (!string.IsNullOrWhiteSpace(cep))
                result = result.Where(obj => obj.CEP.Equals(cep));

            if (!string.IsNullOrWhiteSpace(codigoIBGE))
                result = result.Where(obj => obj.Localidade.CodigoIBGE.Equals(codigoIBGE));

            //result = result.Where(obj => !obj.Localidade.CodigoIBGE.Equals("0"));

            return result.Count();            
        }
    }
}
