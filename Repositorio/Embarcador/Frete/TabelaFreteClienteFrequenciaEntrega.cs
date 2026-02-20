using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteFrequenciaEntrega : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega>
    {
        public TabelaFreteClienteFrequenciaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> BuscarPortabelasCliente(List<int> tabelasCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega>();

            var result = from obj in query where tabelasCliente.Contains(obj.TabelaFreteCliente.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> BuscarPorCodigoTabelaFreteCliente(int codigoTabelaFreteCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega>();

            var result = from obj in query where obj.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente select obj;

            return result.ToList();
        }

        public void DeletarPorTabelaFreteCliente(int codigoTabelaFretecliente)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM TabelaFreteClienteFrequenciaEntrega c WHERE c.TabelaFreteCliente.Codigo = :codigoTabelaFretecliente").SetInt32("codigoTabelaFretecliente", codigoTabelaFretecliente).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM TabelaFreteClienteFrequenciaEntrega c WHERE c.TabelaFreteCliente.Codigo = :codigoTabelaFretecliente").SetInt32("codigoTabelaFretecliente", codigoTabelaFretecliente).ExecuteUpdate();

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
