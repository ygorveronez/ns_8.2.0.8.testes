using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Global
{
    public class EmpresaIntelipostIntegracao : RepositorioBase<Dominio.Entidades.EmpresaIntelipostIntegracao>
    {
        public EmpresaIntelipostIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EmpresaIntelipostIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostIntegracao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.EmpresaIntelipostIntegracao BuscarPorEmpresaeCanalEntrega(int codigoEmpresa, int codigoCanalEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostIntegracao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Empresa.Codigo == codigoEmpresa && ent.CanalEntrega.Codigo == codigoCanalEntrega);

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.EmpresaIntelipostIntegracao BuscarPorEmpresaSemCanalEntrega(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostIntegracao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Empresa.Codigo == codigoEmpresa && ent.CanalEntrega == null);

            return result.FirstOrDefault();
        }


        public bool VerificarEmpresaPossuiIntegracao(int empresa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostIntegracao>();

            consulta = consulta.Where(obj => obj.Empresa.Codigo == empresa);

            return consulta.Any();
        }

        private IQueryable<Dominio.Entidades.EmpresaIntelipostIntegracao> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaIntelipostIntegracao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.EmpresaIntelipostIntegracao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consulta = Consultar(descricao);

            return consulta.Count();
        }


        public void DeletarPorEmpresa(int codigoEmpresas)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM EmpresaIntelipostIntegracao c WHERE c.Empresa.Codigo = :codigoEmpresas").SetInt32("codigoEmpresas", codigoEmpresas).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM EmpresaIntelipostIntegracao c WHERE c.Empresa.Codigo = :codigoEmpresas").SetInt32("codigoEmpresas", codigoEmpresas).ExecuteUpdate();

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





