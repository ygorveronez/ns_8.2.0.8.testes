using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EmissaoEmail : RepositorioBase<Dominio.Entidades.EmissaoEmail>, Dominio.Interfaces.Repositorios.EmissaoEmail
    {
        public EmissaoEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EmissaoEmail BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmissaoEmail>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EmissaoEmail> Consultar(int numeroCTe, DateTime data, string empresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmissaoEmail>();

            var result = from obj in query select obj;

            if (numeroCTe > 0)
                result = result.Where(o => (o.CTe.Numero == numeroCTe) || (o.NFSe.Numero == numeroCTe));
            
            if (data > DateTime.MinValue)
                result = result.Where(o => (o.CTe.DataEmissao >= data.Date && o.CTe.DataEmissao < data.AddDays(1).Date) || (o.NFSe.DataEmissao >= data.Date && o.NFSe.DataEmissao < data.AddDays(1).Date));

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => (o.CTe.Empresa.RazaoSocial.Contains(empresa)) || (o.NFSe.Empresa.RazaoSocial.Contains(empresa)));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int numeroCTe, DateTime data, string empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmissaoEmail>();

            var result = from obj in query select obj;

            if (numeroCTe > 0)
                result = result.Where(o => (o.CTe.Numero == numeroCTe) || (o.NFSe.Numero == numeroCTe));

            if (data > DateTime.MinValue)
                result = result.Where(o => (o.CTe.DataEmissao >= data.Date && o.CTe.DataEmissao < data.AddDays(1).Date) || (o.NFSe.DataEmissao >= data.Date && o.NFSe.DataEmissao < data.AddDays(1).Date));

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => (o.CTe.Empresa.RazaoSocial.Contains(empresa)) || (o.NFSe.Empresa.RazaoSocial.Contains(empresa)));

            return result.Count();
        }

        public void DeletarPorNFSe(int codigoNFSe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE EmissaoEmail obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                     .SetInt32("codigoNFSe", codigoNFSe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE EmissaoEmail obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                    .SetInt32("codigoNFSe", codigoNFSe)
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
