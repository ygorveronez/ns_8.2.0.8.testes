using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoVendedores : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores>
    {
        public TipoOperacaoVendedores(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores> BuscarPorCodigoTipoOperacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores>();
            var result = from obj in query where obj.TipoOperacao.Codigo == codigo select obj;
            return result.ToList();
        }

        public void DeletarPorTipoOperacao(int codigo)
        {
            var query = UnitOfWork.Sessao.CreateQuery("DELETE FROM TipoOperacaoVendedores t WHERE t.TipoOperacao.Codigo = :codigo").SetInt32("codigo", codigo);

            query.SetTimeout(240).ExecuteUpdate();

        }



    }
}
