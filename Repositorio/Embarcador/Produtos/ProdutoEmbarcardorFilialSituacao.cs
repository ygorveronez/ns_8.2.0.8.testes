using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcardorFilialSituacao : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes>
    {
        #region Constructores
        public ProdutoEmbarcardorFilialSituacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes BuscaPorCodigoSituacao(SituacaoFilial situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes>();
            query = query.Where(s => s.SituacaoFilial == situacao);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
