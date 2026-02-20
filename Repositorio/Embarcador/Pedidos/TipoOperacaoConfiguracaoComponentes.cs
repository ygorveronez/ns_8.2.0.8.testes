using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoConfiguracaoComponentes : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes>
    {
        public TipoOperacaoConfiguracaoComponentes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes> BuscarPorTipoOperacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes>();

            var result = from obj in query where obj.TipoOperacao.Codigo == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes BuscarPorOutraDescricaoCTe(int codigo, string outraDescricaoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes>();

            var result = from obj in query where obj.TipoOperacao.Codigo == codigo && obj.OutraDescricaoCTe == outraDescricaoCTe select obj;

            return result.FirstOrDefault();
        }

    }
}
