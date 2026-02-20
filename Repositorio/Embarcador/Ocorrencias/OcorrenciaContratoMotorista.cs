using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaContratoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista>
    {

        public OcorrenciaContratoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista>();
            var result = from obj in query where obj.Ocorrencia.Codigo == ocorrencia select obj;
            return result.FirstOrDefault();
        }
    }
}
