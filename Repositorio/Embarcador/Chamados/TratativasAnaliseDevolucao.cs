using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class TratativasAnaliseDevolucao : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.TratativasAnaliseDevolucao>
    {
        public TratativasAnaliseDevolucao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Chamados.TratativasAnaliseDevolucao> BuscarPorTipos(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> tratativasDevolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.TratativasAnaliseDevolucao>();

            if (tratativasDevolucao != null && tratativasDevolucao.Count > 0)
                query = query.Where(o => tratativasDevolucao.Contains(o.TratativaDevolucao));

            return query.ToList();
        }

    }
}
