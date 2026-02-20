using System;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class ClienteExportacaoCTe : RepositorioBase<Dominio.Entidades.ClienteExportacaoCTe>, Dominio.Interfaces.Repositorios.ClienteExportacaoCTe
    {
        public ClienteExportacaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ClienteExportacaoCTe BuscarPorCTeETipo(int codigoCTe, int codigoEmpresa, Dominio.Enumeradores.TipoTomador tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteExportacaoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo select obj;
            result = result.Fetch(o => o.Pais);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ClienteExportacaoCTe BuscarPorCTeETipo(int codigoCTe, Dominio.Enumeradores.TipoTomador tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteExportacaoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Tipo == tipo select obj;
            result = result.Fetch(o => o.Pais);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ClienteExportacaoCTe BuscarPorCTe(int codigoCTe, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteExportacaoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorCTeETipo(int codigoEmpresa, int codigoCTe, Dominio.Enumeradores.TipoTomador tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteExportacaoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.ClienteExportacaoCTe BuscarPorNomeEEmpresa(int codigoEmpresa, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteExportacaoCTe>();
            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa && obj.Nome.ToLower().Equals(nome) orderby obj.CTe.Codigo descending select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ClienteExportacaoCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ClienteExportacaoCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                    .SetInt32("codigoCTe", codigoCTe)
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
