using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Creditos
{
    public class CreditoDisponivel : RepositorioBase<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>
    {
         public CreditoDisponivel(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel BuscarPorCodigo(int codigo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>();
             var result = from obj in query where obj.Codigo == codigo select obj;
             return result.FirstOrDefault();
         }

         public Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel BuscarRecebedorPossuiCreditoAtivo(int codCreditor, int codRecebedor, DateTime inicio, DateTime Fim)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>();
             var result = from obj in query select obj;
             result = result.Where(obj => obj.Creditor.Codigo == codCreditor);
             result = result.Where(obj => obj.Recebedor.Codigo == codRecebedor);
             result = result.Where(obj => obj.DataInicioCredito >= inicio && obj.DataFimCredito < Fim.AddDays(1).Date);

             return result.FirstOrDefault();
         }

         public Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel BuscarRecebedorCredito(int codCreditor, int codRecebedor)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>();
             var result = from obj in query select obj;
             result = result.Where(obj => obj.Creditor.Codigo == codCreditor);
             result = result.Where(obj => obj.Recebedor.Codigo == codRecebedor);
             result = result.Where(obj => obj.DataInicioCredito <= DateTime.Now.Date && obj.DataFimCredito >= DateTime.Now.Date);

             return result
                .Fetch(obj => obj.Creditor)
                .FirstOrDefault();
         }

         public List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel> Consultar(int codCreditor, int codRecebedor, bool somenteAtivos, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>();

             var result = from obj in query select obj;

             if (codCreditor > 0)
                 result = result.Where(obj => obj.Creditor.Codigo == codCreditor);

             if (codRecebedor > 0)
                 result = result.Where(obj => obj.Recebedor.Codigo == codRecebedor);

             if (somenteAtivos)
                 result = result.Where(obj => obj.DataInicioCredito <= DateTime.Now.Date && obj.DataFimCredito >= DateTime.Now.Date);

             return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

         }

         public int ContarConsulta(int codCreditor, int codRecebedor, bool somenteAtivos)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>();

             var result = from obj in query select obj;

             if (codCreditor > 0)
                 result = result.Where(obj => obj.Creditor.Codigo == codCreditor);

             if (codRecebedor > 0)
                 result = result.Where(obj => obj.Recebedor.Codigo == codRecebedor);

             if (somenteAtivos)
                 result = result.Where(obj => obj.DataInicioCredito <= DateTime.Now.Date && obj.DataFimCredito >= DateTime.Now.Date);

             return result.Count();
         }

    }
}
