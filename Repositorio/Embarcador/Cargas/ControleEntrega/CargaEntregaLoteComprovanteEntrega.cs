using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaLoteComprovanteEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega>
    {
        public CargaEntregaLoteComprovanteEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> _Consultar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega>();

            var result = from obj in query select obj;

            // Filtros
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> Consultar(string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar();

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega> BuscarPorLoteComprovanteEntrega(int codigoLoteComprovanteEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega>();
            var result = query.Where(o => o.LoteComprovanteEntrega.Codigo == codigoLoteComprovanteEntrega);

            return result
                .Fetch(obj => obj.DadosRecebedor)
                .ToList();
        }

        public int ContarConsulta()
        {
            var result = _Consultar();
            return result.Count();
        }

        public void DeletarPorLoteComprovanteEntrega(int codigoLoteComprovanteEntrega)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaLoteComprovanteEntrega obj WHERE obj.LoteComprovanteEntrega.Codigo = :codigoLoteComprovanteEntrega")
                                     .SetInt32("codigoLoteComprovanteEntrega", codigoLoteComprovanteEntrega)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaLoteComprovanteEntrega obj WHERE obj.LoteComprovanteEntrega.Codigo = :codigoLoteComprovanteEntrega")
                                .SetInt32("codigoLoteComprovanteEntrega", codigoLoteComprovanteEntrega)
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
                        throw new System.Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }

}