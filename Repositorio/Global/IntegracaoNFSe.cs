using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoNFSe : RepositorioBase<Dominio.Entidades.IntegracaoNFSe>, Dominio.Interfaces.Repositorios.IntegracaoNFSe
    {
        public IntegracaoNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.IntegracaoNFSe> BuscarPorNFSeETipo(int codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Tipo == tipoIntegracao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoNFSe> Buscar(int codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracaoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Tipo == tipoIntegracaoNFSe select obj;

            return result.ToList();
        }

        public List<int> BuscarIntegracaoNFSeProcessadasPendentes(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao && obj.Status == Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoNFSeTemporaria select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).ToList();
        }

        public int ContarIntegracaoNFSeProcessadasPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao && obj.Status == Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoNFSeTemporaria select obj;

            return result.Count();
        }

        public List<int> BuscarIntegracaoNFSeAguardandoGeracao(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query where obj.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao && obj.Status == Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe select obj;

            return result.OrderBy(o => o.NFSe.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.IntegracaoNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public string BuscarStatusIntegracaoEmissao(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao select obj;

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault() != null ? result.OrderByDescending(o => o.Codigo).FirstOrDefault().Status.ToString() : string.Empty;
        }

        public List<int> BuscarPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query
                         where 
                             obj.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao &&
                             obj.GerouCargaEmbarcador == false && 
                             (obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual )
                         select obj;

            //return result.OrderBy(o => o.NFSe.Codigo).ToList();
            return result.OrderBy(o => o.NFSe.Codigo).Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.IntegracaoNFSe BuscarPorNFSeTipoStatus(int codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao, Dominio.Enumeradores.StatusIntegracao status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Status == status && obj.Tipo == tipoIntegracao select obj;

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<int> BuscarPorCargasPendentes(int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
            var result = from obj in query
                         where
                             obj.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao &&
                             obj.NumeroDaCarga == numeroCarga &&
                             obj.NumeroDaUnidade == numeroUnidade &&
                             obj.GerouCargaEmbarcador == false &&
                             (obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual || obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AguardandoAutorizacaoRPS || obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                         select obj;

            //return result.OrderBy(o => o.NFSe.Codigo).ToList();
            return result.OrderBy(o => o.NFSe.Codigo).Select(o => o.Codigo).ToList();
        }

        public void DeletarPorNFSe(int codigoNFSe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE IntegracaoNFSe obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                     .SetInt32("codigoNFSe", codigoNFSe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE IntegracaoNFSe obj WHERE obj.NFSe.Codigo = :codigoNFSe")
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
