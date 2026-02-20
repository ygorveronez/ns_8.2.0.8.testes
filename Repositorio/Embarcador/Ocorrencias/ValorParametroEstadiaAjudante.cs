using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroEstadiaAjudante : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante>
    {

        public ValorParametroEstadiaAjudante(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarItensNaoPesentesNaLista(int parametro, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante>();
            var result = from obj in query
                         where
                            obj.ValorParametroEstadiaOcorrencia.Codigo == parametro
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante BuscarPorParametroEstadiaECodigo(int parametro, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante>();
            var result = from obj in query
                         where
                            obj.ValorParametroEstadiaOcorrencia.Codigo == parametro
                            && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorEstadia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante>();
            var result = from obj in query
                         where
                            obj.ValorParametroEstadiaOcorrencia.Codigo == codigo
                         select obj;

            return result.Count();
        }
    }
}
