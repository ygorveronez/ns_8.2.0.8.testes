using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoTMSCTe : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe>
    {
        public ChamadoTMSCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe> BuscarPorChamado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe>();
            var result = from obj in query where obj.Chamado.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe> ConsultarPorChamado(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe>();
            var result = from obj in query where obj.Chamado.Codigo == codigo select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsultarPorChamado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe>();
            var result = from obj in query where obj.Chamado.Codigo == codigo select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarCTesPorCarga(int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.CargaCTes.Any(c => c.Carga.Codigo == codigoCarga) select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsultarCTesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var result = from obj in query where obj.CargaCTes.Any(c => c.Carga.Codigo == codigoCarga) select obj;

            return result.Count();
        }
    }
}
