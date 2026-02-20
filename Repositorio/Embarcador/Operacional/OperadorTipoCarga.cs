using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Operacional
{
    public class OperadorTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga>
    {
        public OperadorTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga> BuscarPorOperador(int codigoOperador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga>();
            var result = from obj in query where obj.OperadorLogistica.Codigo == codigoOperador select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga BuscarPorOperadorETipoDeCarga(int codigo, int codigoTipoDeCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga>()
                .Where(obj => obj.OperadorLogistica.Codigo == codigo && obj.TipoDeCarga.Codigo == codigoTipoDeCarga);

            return query.FirstOrDefault();
        }
    }
}
