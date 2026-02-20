using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cotacao
{
    public class RegraCotacaoPeso : RepositorioBase<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso>
    {
        public RegraCotacaoPeso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso>();
            var result = from obj in query where obj.RegrasCotacao.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

        public void DeletarTodosPorRegra(int codigoRegra)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM RegrasPeso c WHERE c.RegrasCotacao.Codigo = :codigo")
                    .SetInt32("codigo", codigoRegra)
                    .ExecuteUpdate();
        }
    }
}
