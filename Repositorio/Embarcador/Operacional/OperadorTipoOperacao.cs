//using System;
//using System.Collections.Generic;
//using System.Linq;
//using NHibernate.Criterion;
//using NHibernate.Linq;
//using System.Linq.Dynamic.Core;

//namespace Repositorio.Embarcador.Operacional
//{
//    public class OperadorTipoOperacao: RepositorioBase<Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao>
//    {
//         public OperadorTipoOperacao(string stringConexao) : base(stringConexao) { }
//         public OperadorTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

//         public Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao BuscarPorCodigo(int codigo)
//         {
//             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao>();
//             var result = from obj in query where obj.Codigo == codigo select obj;
//             return result.FirstOrDefault();
//         }

//         public List<Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao> BuscarPorOperador(int codigoOperador)
//         {
//             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao>();
//             var result = from obj in query where obj.OperadorLogistica.Codigo == codigoOperador select obj;
//             return result.ToList();
//         }

//    }
//}
