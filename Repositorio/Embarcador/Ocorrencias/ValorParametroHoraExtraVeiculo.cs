using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroHoraExtraVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo>
    {

        public ValorParametroHoraExtraVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarItensNaoPesentesNaLista(int parametro, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo>();
            var result = from obj in query
                         where
                            obj.ValorParametroHoraExtraOcorrencia.Codigo == parametro
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo BuscarPorParametroHoraExtraECodigo(int parametro, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo>();
            var result = from obj in query
                         where
                            obj.ValorParametroHoraExtraOcorrencia.Codigo == parametro
                            && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorHoraExtra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo>();
            var result = from obj in query
                         where
                            obj.ValorParametroHoraExtraOcorrencia.Codigo == codigo
                         select obj;

            return result.Count();
        }
    }
}
