using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class VendaDiretaParcela : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>
    {
        public VendaDiretaParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela> BuscarPorVendaDireta(int codigoVendaDireta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();

            query = from obj in query where obj.VendaDireta.Codigo == codigoVendaDireta select obj;

            return query.ToList();
        }

        public int ContarPorVendaDireta(int codigoVendaDireta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();
            var queryParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();

            queryParcela = queryParcela.Where(o => o.VendaDireta.Codigo == codigoVendaDireta);
            query = query.Where(o => queryParcela.Select(p => p.Titulo).Contains(o.Titulo));

            return query.Count();
        }

        public int ContarPorStatusEVendaDireta(int codigoVendaDireta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var queryParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();

            queryParcela = queryParcela.Where(o => o.VendaDireta.Codigo == codigoVendaDireta);

            query = query.Where(o => o.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada);
            query = query.Where(o => queryParcela.Select(p => p.Titulo.Codigo).Contains(o.Codigo));

            return query.Count();
        }

        public int ContarPorBoletoStatusEVendaDireta(int codigoVendaDireta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var queryParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();

            queryParcela = queryParcela.Where(o => o.VendaDireta.Codigo == codigoVendaDireta);

            query = query.Where(o => o.BoletoStatusTitulo > Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum);
            query = query.Where(o => queryParcela.Select(p => p.Titulo.Codigo).Contains(o.Codigo));

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorVendaDiretaParcela(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Select(o => o.Titulo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarBoletosPorVendaDireta(int codigoVendaDireta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();
            var result = from obj in query where obj.VendaDireta.Codigo == codigoVendaDireta && obj.Titulo != null select obj;
            return result.Select(o => o.Titulo).ToList();
        }
    }
}
