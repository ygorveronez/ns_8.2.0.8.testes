//using System;
//using System.Collections.Generic;
//using System.Linq;
//using NHibernate.Criterion;
//using NHibernate.Linq;
//using System.Linq.Dynamic.Core;

//namespace Repositorio.Embarcador.Pedidos
//{
//    public class CanhotoAvulso : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.CanhotoAvulso>
//    {
//        public CanhotoAvulso(string stringConexao) : base(stringConexao) { }
//        public CanhotoAvulso(UnitOfWork unitOfWork) : base(unitOfWork) { }

//        public Dominio.Entidades.Embarcador.Pedidos.CanhotoAvulso BuscarPorCodigo(int codigo)
//        {
//            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanhotoAvulso>();
//            var result = query.Where(obj => obj.Codigo == codigo);
//            return result.FirstOrDefault();
//        }
//    }
//}
