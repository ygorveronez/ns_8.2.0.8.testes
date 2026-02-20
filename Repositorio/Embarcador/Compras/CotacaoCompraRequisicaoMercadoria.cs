using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Compras
{
    public class CotacaoCompraRequisicaoMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria>
    {

        public CotacaoCompraRequisicaoMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria BuscarPorCotacao(int codigoCotacao, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria>();
            var result = from obj in query where obj.Codigo == codigo && obj.Cotacao.Codigo == codigoCotacao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria> BuscarPorCodigoCotacao(int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria>();
            var result = from obj in query where obj.Cotacao.Codigo == codigoCotacao select obj;
            return result.ToList(); 
        }
    }
}
