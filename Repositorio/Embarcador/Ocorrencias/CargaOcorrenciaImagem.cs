using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaImagem : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem>
    {
        public CargaOcorrenciaImagem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem> BuscarPorCodigoOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigo select obj;
            return result.Fetch(obj => obj.CargaOcorrencia).ToList();
        }
    }
}
