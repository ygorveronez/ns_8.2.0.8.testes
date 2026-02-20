using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteRotaTipoCarga : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga>
    {
         public TabelaFreteRotaTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga BuscarPorCodigo(int codigo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga>();
             var result = from obj in query where obj.Codigo == codigo select obj;
             return result.FirstOrDefault();
         }

         public Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga BuscarPorTabelaTipoCarga(int codigoTabelaFrete, int codigoTipoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga>();
             var result = from obj in query select obj;

             result = result.Where(ttc => ttc.TabelaFreteRota.Codigo == codigoTabelaFrete && ttc.TipoDeCarga.Codigo == codigoTipoCarga );

             if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                 result = result.Where(tbf => tbf.Ativo == true);

             if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                 result = result.Where(tbf => tbf.Ativo == false);

             return result.FirstOrDefault();
         }

         public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga> Consultar(int codigoTabelaFreteRota, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttc => ttc.TabelaFreteRota.Codigo == codigoTabelaFreteRota);


             return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

         }

         public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga> Consultar(int codigoTabelaFreteRota)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttc => ttc.TabelaFreteRota.Codigo == codigoTabelaFreteRota);


             return result.ToList();

         }

         public int ContarConsulta(int codigoTabelaFreteRota)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttc => ttc.TabelaFreteRota.Codigo == codigoTabelaFreteRota);


             return result.Count();
         }


         public void DeletarPorTabelaFreteRota(int codigoTabelaFreteRota)
         {
             try
             {
                 if (UnitOfWork.IsActiveTransaction())
                 {
                     UnitOfWork.Sessao.CreateQuery("DELETE TabelaFreteRotaTipoCarga obj WHERE obj.TabelaFreteRota.Codigo = :codigoTabelaFreteRota")
                                      .SetInt32("codigoTabelaFreteRota", codigoTabelaFreteRota)
                                      .ExecuteUpdate();
                 }
                 else
                 {
                     try
                     {
                         UnitOfWork.Start();
                    
                         UnitOfWork.Sessao.CreateQuery("DELETE TabelaFreteRotaTipoCarga obj WHERE obj.TabelaFreteRota.Codigo = :codigoTabelaFreteRota")
                                 .SetInt32("codigoTabelaFreteRota", codigoTabelaFreteRota)
                                 .ExecuteUpdate();
                    
                         UnitOfWork.CommitChanges();
                     }
                     catch
                     {
                         UnitOfWork.Rollback();
                         throw;
                     }
                 }
             }
             catch (NHibernate.Exceptions.GenericADOException ex)
             {
                 if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                 {
                     System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                     if (excecao.Number == 547)
                     {
                         throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                     }
                 }
                 throw;
             }
         }

    }
}
