using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCargaEstadoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCargaEstadoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino> BuscarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino>()
                .Where(configuracaoEstadoDestino => configuracaoEstadoDestino.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoEstadoDestino
                .Fetch(configuracaoEstadoDestino => configuracaoEstadoDestino.Estado)
                .ToList();
        }

        public List<string> BuscarPorConfiguracaoParaSugestao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino>()
                .Where(configuracaoEstadoDestino => configuracaoEstadoDestino.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoEstadoDestino
                .Select(configuracaoEstadoDestino => configuracaoEstadoDestino.Estado.Sigla)
                .ToList();
        }

        public void DeletarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConfiguracaoProgramacaoCargaEstadoDestino configuracaoEstadoDestino where configuracaoEstadoDestino.ConfiguracaoProgramacaoCarga.Codigo = :codigoConfiguracaoProgramacaoCarga")
                    .SetInt32("codigoConfiguracaoProgramacaoCarga", codigoConfiguracaoProgramacaoCarga)
                    .ExecuteUpdate();
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
