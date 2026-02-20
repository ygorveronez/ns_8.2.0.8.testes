using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Creditos
{
    public class HierarquiaSolicitacaoCredito : RepositorioBase<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>
    {
         public HierarquiaSolicitacaoCredito(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito BuscarPorCreditorRecebedor(int codCreditor, int codRecebedor)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();
             var result = from obj in query where obj.Solicitante.Codigo == codRecebedor && obj.Creditor.Codigo == codCreditor select obj;
             return result.FirstOrDefault();
         }

         public List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> BuscarPorRecebedor(int codRecebedor)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();
             var result = from obj in query where obj.Solicitante.Codigo == codRecebedor select obj;
             return result
                .Fetch(obj => obj.Creditor)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>> BuscarPorRecebedorAsync(int codRecebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();
            var result = from obj in query where obj.Solicitante.Codigo == codRecebedor select obj;
            return await result
               .Fetch(obj => obj.Creditor)
               .ToListAsync();
        }

        public bool PossuiHierarquiaPorRecebedor(int codRecebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();
            var result = from obj in query where obj.Solicitante.Codigo == codRecebedor select obj;
            return result.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito BuscarPorCodigo(int codigo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();
             var result = from obj in query where obj.Codigo == codigo select obj;
             return result.FirstOrDefault();
         }

         public List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> ConsultarPorCreditor(int codCreditor, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();

             var result = from obj in query select obj;

             if (codCreditor > 0)
                 result = result.Where(obj => obj.Creditor.Codigo == codCreditor);

             return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

         }

         public int ContarPorCreditor(int codCreditor)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>();

             var result = from obj in query select obj;

             if (codCreditor > 0)
                 result = result.Where(obj => obj.Creditor.Codigo == codCreditor);

             return result.Count();
         }

    }
}
