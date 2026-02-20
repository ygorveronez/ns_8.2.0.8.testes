using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public sealed class RotaFreteEmpresaExclusiva : RepositorioBase<Dominio.Entidades.RotaFreteEmpresaExclusiva>
    {
        #region Construtores

        public RotaFreteEmpresaExclusiva(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.RotaFreteEmpresaExclusiva> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteEmpresaExclusiva = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteEmpresaExclusiva.ToList();
        }

        public List<Dominio.Entidades.RotaFreteEmpresaExclusiva> BuscarPorRotasFretes(List<int> codigosRotaFrete)
        {
            var consultaRotaFreteEmpresaExclusiva = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                .Where(o => codigosRotaFrete.Contains(o.RotaFrete.Codigo));

            return consultaRotaFreteEmpresaExclusiva.ToList();
        }

        public List<Dominio.Entidades.RotaFreteEmpresaExclusiva> BuscarPorRegiaoExclusivaRegiaoDestino(int codigoRegiaoDestino)
        {
            var consultaRotaFreteEmpresaExclusiva = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                .Where(o => o.RotaFrete.RegiaoDestino.Codigo == codigoRegiaoDestino);

            return consultaRotaFreteEmpresaExclusiva
                .Fetch(obj => obj.RotaFrete).ToList();
        }

        public List<Dominio.Entidades.RotaFreteEmpresaExclusiva> BuscarPorRegiaoExclusivaRegiaoDestinos(List<int> codigosRegiaoDestinos)
        {
            var consultaRotaFreteEmpresaExclusiva = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteEmpresaExclusiva>()
                .Where(o => codigosRegiaoDestinos.Contains(o.RotaFrete.RegiaoDestino.Codigo));

            return consultaRotaFreteEmpresaExclusiva
                .Fetch(obj => obj.RotaFrete)
                .ThenFetch(x => x.TipoOperacao)
                .Fetch(x => x.RotaFrete)
                .ThenFetch(x => x.RegiaoDestino)
                .Fetch(obj => obj.Empresa).ToList();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {
                UnitOfWork.Sessao.CreateQuery("delete from RotaFreteEmpresaExclusiva e where e.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion Métodos Públicos
    }
}
