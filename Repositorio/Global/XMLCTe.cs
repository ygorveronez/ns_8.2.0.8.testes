using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class XMLCTe : RepositorioBase<Dominio.Entidades.XMLCTe>, Dominio.Interfaces.Repositorios.XMLCTe
    {
        public XMLCTe(UnitOfWork unidadeDeTrabalho) : base(unidadeDeTrabalho) { }

        public List<Dominio.Entidades.XMLCTe> BuscarPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public string BuscarXMLPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.Select(xmlCte => xmlCte.XML).FirstOrDefault();
        }

        public string BuscarXMLPorCTe(int codigoCTe, Dominio.Enumeradores.TipoXMLCTe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Tipo == tipo select obj;
            return result.Select(xmlCte => xmlCte.XML).FirstOrDefault();
        }

        public Dominio.Entidades.XMLCTe BuscarPorCTe(int codigoCTe, Dominio.Enumeradores.TipoXMLCTe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.XMLCTe> BuscarPorCTe(int[] codigosCTes, Dominio.Enumeradores.TipoXMLCTe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            var result = from obj in query where codigosCTes.Contains(obj.CTe.Codigo) && obj.Tipo == tipo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.XMLCTe> BuscarPorCTe(List<int> codigosCTes, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.XMLCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo));

            if (codigoEmpresa > 0)
                query = query.Where(o => o.CTe.Empresa.Codigo == codigoEmpresa);

            return query.ToList();
        }

        public List<Dominio.Entidades.XMLCTe> BuscarPorCTe(List<int> codigosCTes, Dominio.Enumeradores.TipoXMLCTe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            var result = from obj in query where codigosCTes.Contains(obj.CTe.Codigo) && obj.Tipo == tipo select obj;

            return result.ToList();
        }

        public bool ExisteXMLPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.XMLCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>()
                .Where(obj => obj.CTe.Codigo == codigoCTe);

            return query.Any();
        }

        /// <summary>
        /// REMOVER APÓS CONCLUIR ATUALIZACOES
        /// </summary>
        public List<int> BuscarCTesAutorizadosSemXML()
        {
            var queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryXMLs = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            var result = from cte in queryCTes where (cte.Status.Equals("A") || cte.Status.Equals("C")) && !(from xml in queryXMLs where xml.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao select xml.CTe.Codigo).Distinct().Contains(cte.Codigo) select cte.Codigo;

            return result.ToList();
        }

        /// <summary>
        /// REMOVER APÓS CONCLUIR ATUALIZACOES
        /// </summary>
        public List<int> BuscarCTesCanceladosSemXML()
        {
            var queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryXMLs = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            var result = from cte in queryCTes where cte.Status.Equals("C") && !(from xml in queryXMLs where xml.Tipo == Dominio.Enumeradores.TipoXMLCTe.Cancelamento select xml.CTe.Codigo).Distinct().Contains(cte.Codigo) select cte.Codigo;

            return result.ToList();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE XMLCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE XMLCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
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
