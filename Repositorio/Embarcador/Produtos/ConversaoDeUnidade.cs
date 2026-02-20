using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class ConversaoDeUnidade : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade>
    {
        #region Constructor
        public ConversaoDeUnidade(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade BuscarTipoConversaoPorSiglas(string siglaDe, string siglaPara)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade>();
            query = query.Where(c => c.Sigla == siglaDe && c.UnidadeDeMedida.Sigla == siglaPara);
            return query.FirstOrDefault();
        }

        #endregion
    }
}
