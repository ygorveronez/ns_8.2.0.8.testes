using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>
    {
        public OcorrenciaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia select obj.TipoIntegracao.Tipo;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia select obj;
            return resut.ToList();
        }

        public int ContarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>();

            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia select obj;

            return resut.Count();
        }

        public int ContarPorOcorrenciaETipoIntegracao(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>();

            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }
    }
}
