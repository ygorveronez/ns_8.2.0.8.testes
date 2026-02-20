using System.Linq;

namespace Repositorio.Embarcador.Produtos
{
    public class Organizacao : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.Organizacao>
    {
        public Organizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.Organizacao OrganizacaoExistente(Dominio.Entidades.Embarcador.Produtos.Organizacao organizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.Organizacao>()
                .Where(o => o.Descricao == organizacao.Descricao && o.Canal == organizacao.Canal && o.Setor == organizacao.Setor && o.Nivel == organizacao.Nivel && 
                        o.CodigoHierarquia == organizacao.CodigoHierarquia && o.DescricaoHierarquia == organizacao.DescricaoHierarquia);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
