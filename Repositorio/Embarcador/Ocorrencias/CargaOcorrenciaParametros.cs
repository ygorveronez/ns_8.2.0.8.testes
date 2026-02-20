using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaParametros : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros>
    {
        public CargaOcorrenciaParametros(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(obj => obj.ParametroOcorrencia).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros BuscarPorCargaOcorrenciaETipo(int codigoOCorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOCorrencia && obj.ParametroOcorrencia.TipoParametro == tipo select obj;
            return result
                .Fetch(obj => obj.ParametroOcorrencia)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros> BuscarListaPorCargaOcorrenciaETipo(int codigoOCorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOCorrencia && obj.ParametroOcorrencia.TipoParametro == tipo select obj;
            return result.Fetch(obj => obj.ParametroOcorrencia).ToList();
        }
    }
}
