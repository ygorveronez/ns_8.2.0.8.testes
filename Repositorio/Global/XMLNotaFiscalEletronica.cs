using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class XMLNotaFiscalEletronica : RepositorioBase<Dominio.Entidades.XMLNotaFiscalEletronica>, Dominio.Interfaces.Repositorios.XMLNotaFiscalEletronica
    {
        public XMLNotaFiscalEletronica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.Entidades.XMLNotaFiscalEletronica> Consultar(int codigoEmpresa, string numeroNota, double cnpjEmitente, DateTime dataEmissao, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.XMLNotaFiscalEletronica>();
            criteria.CreateAlias("Emitente", "emitente");
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            if (cnpjEmitente > 0)
                criteria.Add(NHibernate.Criterion.Expression.Eq("emitente.CPF_CNPJ", cnpjEmitente));
            if (!string.IsNullOrWhiteSpace(numeroNota))
                criteria.Add(NHibernate.Criterion.Expression.Eq("Numero", numeroNota));
            if (dataEmissao != DateTime.MinValue)
            {
                criteria.Add(NHibernate.Criterion.Expression.Lt("DataEmissao", dataEmissao.AddDays(1)));
                criteria.Add(NHibernate.Criterion.Expression.Ge("DataEmissao", dataEmissao));
            }
            criteria.SetFirstResult(inicioRegistros);
            criteria.SetMaxResults(maximoRegistros);
            criteria.AddOrder(NHibernate.Criterion.Order.Desc("Codigo"));
            return criteria.List<Dominio.Entidades.XMLNotaFiscalEletronica>();
        }

        public int ContarConsulta(int codigoEmpresa, string numeroNota, double cnpjEmitente, DateTime dataEmissao)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.XMLNotaFiscalEletronica>();
            criteria.CreateAlias("Emitente", "emitente");
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            if (cnpjEmitente > 0)
                criteria.Add(NHibernate.Criterion.Expression.Eq("emitente.CPF_CNPJ", cnpjEmitente));
            if (!string.IsNullOrWhiteSpace(numeroNota))
                criteria.Add(NHibernate.Criterion.Expression.Eq("Numero", numeroNota));
            if (dataEmissao != DateTime.MinValue)
            {
                criteria.Add(NHibernate.Criterion.Expression.Lt("DataEmissao", dataEmissao.AddDays(1)));
                criteria.Add(NHibernate.Criterion.Expression.Ge("DataEmissao", dataEmissao));
            }
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.XMLNotaFiscalEletronica> ConsultarNotasImportadas(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, string chave, double cnpjEmitente, string nomeEmitente, bool? notasSemNFSe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial.Date > DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial);

            if (dataFinal.Date > DateTime.MinValue)
                query = query.Where(o => o.DataEmissao <= dataFinal.AddDays(1));

            if (numeroInicial > 0)
                query = query.Where(o => int.Parse(o.Numero) >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(o => int.Parse(o.Numero) <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(chave))
                query = query.Where(o => o.Chave == chave);

            if (cnpjEmitente > 0)
                query = query.Where(o => o.Emitente.CPF_CNPJ == cnpjEmitente);

            if (!string.IsNullOrWhiteSpace(nomeEmitente))
                query = query.Where(o => o.Emitente.Nome.Contains(nomeEmitente) || o.Emitente.NomeFantasia.Contains(nomeEmitente));

            if (notasSemNFSe != null)
                query = query.Where(o => o.GeradoDocumento == !notasSemNFSe);

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.ToList();
        }


        public int ContarNotasImportadas(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, string chave, double cnpjEmitente, string nomeEmitente, bool? notasSemNFSe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial.Date > DateTime.MinValue)
                query = query.Where(o => o.DataEmissao >= dataInicial);

            if (dataFinal.Date > DateTime.MinValue)
                query = query.Where(o => o.DataEmissao <= dataFinal.AddDays(1));

            if (numeroInicial > 0)
                query = query.Where(o => int.Parse(o.Numero) >= numeroInicial);

            if (numeroFinal > 0)
                query = query.Where(o => int.Parse(o.Numero) <= numeroFinal);

            if (!string.IsNullOrWhiteSpace(chave))
                query = query.Where(o => o.Chave == chave);

            if (cnpjEmitente > 0)
                query = query.Where(o => o.Emitente.CPF_CNPJ == cnpjEmitente);

            if (!string.IsNullOrWhiteSpace(nomeEmitente))
                query = query.Where(o => o.Emitente.Nome.Contains(nomeEmitente) || o.Emitente.NomeFantasia.Contains(nomeEmitente));

            if (notasSemNFSe != null)
                query = query.Where(o => o.GeradoDocumento == !notasSemNFSe);

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.Count();
        }

        public Dominio.Entidades.XMLNotaFiscalEletronica BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            var result = from o in query where o.Codigo == codigo select o;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.XMLNotaFiscalEletronica> BuscarPorCodigos(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            var result = from o in query where codigos.Contains(o.Codigo) select o;

            return result.ToList();
        }

        public Dominio.Entidades.XMLNotaFiscalEletronica BuscarPorChaveNFe(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            var result = from o in query where o.Chave == chaveNFe select o;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.XMLNotaFiscalEletronica BuscarPorChaveNFeEmpresa(string chaveNFe, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            var result = from o in query where o.Chave == chaveNFe  && o.Empresa.Codigo == codigoEmpresa select o;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.XMLNotaFiscalEletronica> BuscarPorContratante(double cnpjContratante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();

            var result = from o in query where o.Contratante.CPF_CNPJ == cnpjContratante select o;

            return result.ToList();
        }


    }
}
