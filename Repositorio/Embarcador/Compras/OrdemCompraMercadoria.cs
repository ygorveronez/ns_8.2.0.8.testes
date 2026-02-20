using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class OrdemCompraMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>
    {
        public OrdemCompraMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarNaoPesentesNaLista(int codigo, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();
            var result = from obj in query
                         where
                            obj.OrdemCompra.Codigo == codigo
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria BuscarPorOrdemEMercadoria(int ordem, int mercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();
            var result = from obj in query
                         where
                            obj.OrdemCompra.Codigo == ordem
                            && obj.Codigo == mercadoria
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> BuscarPorOrdem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();
            var result = from obj in query where obj.OrdemCompra.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> ContarConsultaPorOrdem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao, int produto, int ordem, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(situacao, produto, ordem);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorOrdem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao, int produto, int ordem)
        {
            var result = _Consultar(situacao, produto, ordem);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> _Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao, int produto, int ordem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();

            var result = from obj in query select obj;

            if (ordem > 0)
                result = result.Where(obj => obj.OrdemCompra.Codigo == ordem);

            if (produto > 0)
                result = result.Where(obj => obj.Produto.Codigo == produto);

            if (situacao.HasValue)
                result = result.Where(obj => obj.OrdemCompra.Situacao == situacao);

            return result;
        }

        #endregion
    }
}
