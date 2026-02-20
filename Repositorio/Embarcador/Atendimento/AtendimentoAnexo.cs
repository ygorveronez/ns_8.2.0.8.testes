using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Atendimento
{
    public class AtendimentoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo>
    {
        public AtendimentoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo> BuscarPorAtendimento(int codigoAtendimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo>();

            var result = from obj in query where obj.Atendimento.Codigo == codigoAtendimento select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
    }
}
