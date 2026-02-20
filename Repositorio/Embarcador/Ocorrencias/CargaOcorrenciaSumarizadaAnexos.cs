using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaSumarizadaAnexos : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos>
    {
        public CargaOcorrenciaSumarizadaAnexos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos> BuscarPorCodigoOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos>();
            var result = from obj in query where obj.CargaSumarizado.CargaOcorrencia.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos> Consultar(int codigoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos>();
            var result = from obj in query where obj.CargaSumarizado.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos>();
            var result = from obj in query where obj.CargaSumarizado.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return result.Count();
        }


    }
}
