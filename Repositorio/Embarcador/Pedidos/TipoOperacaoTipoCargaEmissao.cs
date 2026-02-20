using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoTipoCargaEmissao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>
    {
        public TipoOperacaoTipoCargaEmissao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> BuscarPorTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>();

            query = query.Where(o => o.TipoOperacao == tipoOperacao);

            return query.Fetch(o => o.TipoCarga).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>();

            query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao BuscarPorTipoOperacaoETipoCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>();

            query = query.Where(o => o.TipoOperacao == tipoOperacao && o.TipoCarga == tipoDeCarga);

            return query.FirstOrDefault();
        }
    }
}
