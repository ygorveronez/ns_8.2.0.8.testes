using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class LinhaSeparacaoAgrupa : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa>
    {

        public LinhaSeparacaoAgrupa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> BuscarTodos(int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa>();
            
            var result = from obj in query select obj;

            if (codigoFilial > 0)
                result = result.Where(x => x.LinhaSeparacaoUm.Filial.Codigo == codigoFilial &&
                                          x.LinhaSeparacaoDois.Filial.Codigo == codigoFilial);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> BuscarAgrupamentos(int codigoLinhaSeparacao)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> lista = new List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa>();
            var result = from obj in query where obj.LinhaSeparacaoUm.Codigo == codigoLinhaSeparacao select obj.LinhaSeparacaoDois;
            lista = result.ToList();
            var result2 = from obj in query where obj.LinhaSeparacaoDois.Codigo == codigoLinhaSeparacao select obj.LinhaSeparacaoUm;
            lista.AddRange(result2.ToList());
            return lista.OrderBy(x => x.Descricao).ToList();
        }

        public void ExcluirAgrupamentos(int codigoLinhaSeparacao, List<int> codigos)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM LinhaSeparacaoAgrupa WHERE CLS_CODIGO_UM = :codigo AND CLS_CODIGO_DOIS IN ( :codigos )").SetInt32("codigo", codigoLinhaSeparacao).SetParameterList("codigos", codigos).ExecuteUpdate();
            UnitOfWork.Sessao.CreateQuery("DELETE FROM LinhaSeparacaoAgrupa WHERE CLS_CODIGO_DOIS = :codigo AND CLS_CODIGO_UM IN ( :codigos )").SetInt32("codigo", codigoLinhaSeparacao).SetParameterList("codigos", codigos).ExecuteUpdate();
        }
    }
}
