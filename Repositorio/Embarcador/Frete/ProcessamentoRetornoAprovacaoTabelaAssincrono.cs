using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ProcessamentoAprovacaoTabelaAssincrono : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono>
    {
        public ProcessamentoAprovacaoTabelaAssincrono(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono> BuscarProcessamentoPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono>();
            var result = from obj in query
                         where (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoProcessamentoAprovacaoTabelaAssincrono.Pendente ||
                                obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoProcessamentoAprovacaoTabelaAssincrono.FalhaNoProcessamento)
                            && obj.Tentativas <= 2
                         select obj;
            return result.ToList();
        }
    }
}
