using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroPernoiteVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo>
    {

        public ValorParametroPernoiteVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarItensNaoPesentesNaLista(int parametro, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo>();
            var result = from obj in query
                         where
                            obj.ValorParametroPernoiteOcorrencia.Codigo == parametro
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo BuscarPorParametroPernoiteECodigo(int parametro, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo>();
            var result = from obj in query
                         where
                            obj.ValorParametroPernoiteOcorrencia.Codigo == parametro
                            && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorPernoite(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo>();
            var result = from obj in query
                         where
                            obj.ValorParametroPernoiteOcorrencia.Codigo == codigo
                         select obj;

            return result.Count();
        }
    }
}
