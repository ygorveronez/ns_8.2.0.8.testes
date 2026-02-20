using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class MDFeSeguro : RepositorioBase<Dominio.Entidades.MDFeSeguro>, Dominio.Interfaces.Repositorios.MDFeSeguro
    {
        public MDFeSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MDFeSeguro(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.MDFeSeguro BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.MDFeSeguro> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.MDFeSeguro>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToListAsync(cancellationToken);
        }
        
        public bool ValidaSeguroJaInserido(int codigoMDFe, Dominio.Enumeradores.TipoResponsavelSeguroMDFe? tipoResponsavel, string responsavel, string cnpjSeguradora, string nomeSeguradora, string apolice, string averbacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            if (tipoResponsavel != null)
                result = result.Where(o => o.TipoResponsavel == tipoResponsavel);

            if (!string.IsNullOrWhiteSpace(responsavel))
                result = result.Where(o => o.Responsavel.Equals(responsavel));

            if (!string.IsNullOrWhiteSpace(cnpjSeguradora))
                result = result.Where(o => o.CNPJSeguradora.Equals(cnpjSeguradora));

            if (!string.IsNullOrWhiteSpace(nomeSeguradora))
                result = result.Where(o => o.NomeSeguradora.Equals(nomeSeguradora));

            if (!string.IsNullOrWhiteSpace(apolice))
                result = result.Where(o => o.NumeroApolice.Equals(apolice));

            if (!string.IsNullOrWhiteSpace(averbacao))
                result = result.Where(o => o.NumeroAverbacao.Equals(averbacao));

            return result.Count() > 0;
        }
    }
}
