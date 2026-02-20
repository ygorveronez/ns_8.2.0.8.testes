using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraFaturaModeloTipoMovimento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento>
    {
        public ConfiguracaoFinanceiraFaturaModeloTipoMovimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimento BuscarTipoMovimentoPorModeloDocumento(int codigoModeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento>()
                .Where(o => o.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento);

            return query.Select(o => o.TipoMovimento).FirstOrDefault();
        }

        public void DeletarTodos()
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM ConfiguracaoFinanceiraFaturaModeloTipoMovimento")
                                     .ExecuteUpdate();
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

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento> BuscarTodosRrgistros()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento>();

            query = query
                .Fetch(o => o.ModeloDocumentoFiscal)
                .Fetch(o => o.TipoMovimento);

            return query.ToList();
        }
    }
}
