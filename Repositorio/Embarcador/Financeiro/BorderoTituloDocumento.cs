using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BorderoTituloDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>
    {
        public BorderoTituloDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> BuscarPorBorderoTitulo(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();
            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> BuscarPorBordero(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();
            query = query.Where(o => o.BorderoTitulo.Bordero.Codigo == codigoBordero);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> Consultar(int codigoBorderoTitulo, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();

            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);

            return query.ToList();
        }

        public int ContarConsulta(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();

            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);

            return query.Count();
        }

        public decimal ObterTotalDesconto(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();

            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);

            return query.Sum(o => (decimal?)o.ValorTotalDesconto) ?? 0m;
        }

        public decimal ObterTotalAcrescimo(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();

            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);

            return query.Sum(o => (decimal?)o.ValorTotalAcrescimo) ?? 0m;
        }

        public decimal ObterTotalACobrarLiquido(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();

            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);

            return query.Sum(o => (decimal?)o.ValorACobrar) ?? 0m;
        }

        public decimal ObterTotalACobrar(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento>();

            query = query.Where(o => o.BorderoTitulo.Codigo == codigoBorderoTitulo);

            return query.Sum(o => (decimal?)o.ValorTotalACobrar) ?? 0m;
        }
    }
}
