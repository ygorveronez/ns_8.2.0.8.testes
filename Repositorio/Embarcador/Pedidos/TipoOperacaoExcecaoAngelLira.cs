using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoExcecaoAngelLira : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira>
    {
        public TipoOperacaoExcecaoAngelLira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira> BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira>()
                .Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);
            
            return query
                .Fetch(obj => obj.Destino)
                .ToList();
        }
        
        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira BuscarPorTipoOperacaoDestinoValorMinimo(int codigoTipoOperacao, decimal valorMinimo, List<string> estados)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira>()
                .Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);

            query = query.Where(obj => (estados.Contains(obj.Destino.Sigla) && obj.ValorMinimo > 0 && obj.ValorMinimo <= valorMinimo) || (obj.Destino == null && obj.ValorMinimo <= valorMinimo) || (estados.Contains(obj.Destino.Sigla) && obj.ValorMinimo == 0));
            
            return query
                .FirstOrDefault();
        }
    }
}
