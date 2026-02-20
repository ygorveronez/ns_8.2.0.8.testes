using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BorderoTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>
    {
        public BorderoTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> BuscarPorBordero(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();
            query = query.Where(o => o.Bordero.Codigo == codigoBordero);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Bordero BuscarBorderoPorTitulo(int codigoBorderoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();
            query = query.Where(o => o.Codigo == codigoBorderoTitulo);
            return query.Select(o => o.Bordero).FirstOrDefault();
        }

        public bool ExistePorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();
            query = query.Where(o => o.Titulo.Codigo == codigoTitulo && o.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Cancelado);
            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> Consultar(int codigoBordero, int codigoTitulo, int numeroCTe, string numeroCarga, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            if (codigoTitulo > 0)
                query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            if (numeroCTe > 0)
                query = query.Where(o => o.Titulo.Documentos.Any(c => c.CTe.Numero == numeroCTe || c.Carga.CargaCTes.Any(cc => cc.CTe.Numero == numeroCTe)));

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Titulo.Documentos.Any(d => d.Carga.CodigoCargaEmbarcador == numeroCarga || d.CTe.CargaCTes.Any(cc => cc.Carga.CodigoCargaEmbarcador == numeroCarga)));

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoBordero, int codigoTitulo, int numeroCTe, string numeroCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            if (codigoTitulo > 0)
                query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            if (numeroCTe > 0)
                query = query.Where(o => o.Titulo.Documentos.Any(c => c.CTe.Numero == numeroCTe || c.Carga.CargaCTes.Any(cc => cc.CTe.Numero == numeroCTe)));

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Titulo.Documentos.Any(d => d.Carga.CodigoCargaEmbarcador == numeroCarga || d.CTe.CargaCTes.Any(cc => cc.Carga.CodigoCargaEmbarcador == numeroCarga)));

            return query.Count();
        }

        public bool ExistePorBordero(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);
            
            return query.Any();
        }

        public decimal ObterTotalDesconto(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            return query.Sum(o => (decimal?)o.ValorTotalDesconto) ?? 0m;
        }

        public decimal ObterTotalAcrescimo(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            return query.Sum(o => (decimal?)o.ValorTotalAcrescimo) ?? 0m;
        }

        public decimal ObterTotalACobrarLiquido(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            return query.Sum(o => (decimal?)o.ValorACobrar) ?? 0m;
        }

        public decimal ObterTotalACobrar(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            return query.Sum(o => (decimal?)o.ValorTotalACobrar) ?? 0m;
        }
    }
}
