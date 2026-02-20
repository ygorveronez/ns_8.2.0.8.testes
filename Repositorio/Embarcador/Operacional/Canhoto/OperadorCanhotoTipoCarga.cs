using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Operacional.Canhoto
{
    public class OperadorCanhotoTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga>
    {
        public OperadorCanhotoTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga> BuscarPorOperador(int codigoOperador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga>();
            var result = from obj in query where obj.OperadorCanhoto.Codigo == codigoOperador select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga BuscarPorOperadorETipoDeCarga(int codigo, int codigoTipoDeCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga>()
                .Where(obj => obj.OperadorCanhoto.Codigo == codigo && obj.TipoDeCarga.Codigo == codigoTipoDeCarga);

            return query.FirstOrDefault();
        }
    }
}
