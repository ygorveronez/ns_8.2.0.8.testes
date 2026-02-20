using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoInformacaoFechamento : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento>
    {
        public ChamadoInformacaoFechamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> BuscarPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento>();

            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento>();

            var result = from obj in query where obj.Chamado.CargaEntrega.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public bool PossuiPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento>();

            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj;

            return result.Any();
        }
    }
}
