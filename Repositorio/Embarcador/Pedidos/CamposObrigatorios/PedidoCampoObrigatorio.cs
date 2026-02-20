using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos.CamposObrigatorios
{
    public class PedidoCampoObrigatorio : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio>
    {
        public PedidoCampoObrigatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> Consultar(int codigoTipoOperacao, bool? ativo, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> query = ObterQueryConsulta(codigoTipoOperacao, ativo, propOrdenar, dirOrdenar, inicio, limite);

            return query.ToList();
        }

        public int ContarConsulta(int codigoTipoOperacao, bool? ativo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> query = ObterQueryConsulta(codigoTipoOperacao, ativo);

            return query.Count();
        }

        public bool ExistePorTipoOperacao(int codigo, int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio>();

            query = query.Where(o => o.Codigo != codigo);

            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);
            else
                query = query.Where(o => o.TipoOperacao == null);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio BuscarParaPedido(int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio>();

            query = query.Where(o => o.Ativo && (o.TipoOperacao == null || o.TipoOperacao.Codigo == codigoTipoOperacao));

            return query.OrderByDescending(o => o.TipoOperacao).FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> ObterQueryConsulta(int codigoTipoOperacao, bool? ativo, string propOrdenar = "", string dirOrdenar = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio>();

            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            if (ativo.HasValue)
                query = query.Where(o => o.Ativo == ativo.Value);

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
