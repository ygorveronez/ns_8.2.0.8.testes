using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoAlvoSubarea : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>
    {
        #region Métodos públicos
        public PosicaoAlvoSubarea(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> BuscarPorPosicaoAlvo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.PosicaoAlvo.Codigo == codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> BuscarPorPosicao(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.PosicaoAlvo.Posicao.Codigo == codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> BuscarPorPosicao(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();

            var result = from obj in query select obj;
            result = result.Where(ent => codigos.Contains(ent.PosicaoAlvo.Posicao.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> BuscarPorPosicaoAlvosESubAreaClientes(List<long> codigosPosicao, List<int> codigosSubAreaClientes)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> retorno = new();

            int take = 500;
            int start = 0;
            while (start < codigosPosicao.Count)
            {
                List<long> codigosPosicaoTmp = codigosPosicao.Skip(start).Take(take).ToList();
                List<int> codigosSubAreaClientesTmp = codigosSubAreaClientes.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();
                query = query.Where(ent => codigosPosicaoTmp.Contains(ent.PosicaoAlvo.Codigo) && codigosSubAreaClientesTmp.Contains(ent.SubareaCliente.Codigo));

                retorno.AddRange(query.Fetch(obj => obj.SubareaCliente).ToList());

                start += take;
            }
            return retorno;
        }

        #endregion

    }

}
