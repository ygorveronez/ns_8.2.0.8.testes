using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaDiariaPeriodo : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>
    {
        public TabelaDiariaPeriodo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo BuscarPorVigencia(List<int> codigosTabelas, string descricao, DateTime dataDiaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>();
            var result = from obj in query
                         where obj.Descricao == descricao && codigosTabelas.Contains(obj.TabelaDiaria.Codigo)
                         select obj;

            if (dataDiaria > DateTime.MinValue)
                result = result.Where(obj => (obj.TabelaDiaria.DataVigenciaInicial == null || obj.TabelaDiaria.DataVigenciaInicial <= dataDiaria) && (obj.TabelaDiaria.DataVigenciaFinal == null || obj.TabelaDiaria.DataVigenciaFinal >= dataDiaria));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> BuscarPorTabela(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>();
            var result = from obj in query where obj.TabelaDiaria.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<int> BuscarItensNaoPesentesNaLista(int parametro, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>();
            var result = from obj in query
                         where
                            obj.TabelaDiaria.Codigo == parametro
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo BuscarPorParametroHoraExtraECodigo(int parametro, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo>();
            var result = from obj in query
                         where
                            obj.TabelaDiaria.Codigo == parametro
                            && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }
    }
}
