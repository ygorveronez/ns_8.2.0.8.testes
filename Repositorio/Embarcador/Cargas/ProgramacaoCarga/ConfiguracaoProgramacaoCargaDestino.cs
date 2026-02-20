using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCargaDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCargaDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino> BuscarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino>()
                .Where(configuracaoDestino => configuracaoDestino.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoDestino
                .Fetch(configuracaoDestino => configuracaoDestino.Localidade)
                .ToList();
        }

        public List<int> BuscarPorConfiguracaoParaSugestao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino>()
                .Where(configuracaoDestino => configuracaoDestino.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoDestino
                .Select(configuracaoDestino => configuracaoDestino.Localidade.Codigo)
                .ToList();
        }

        public void DeletarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConfiguracaoProgramacaoCargaDestino configuracaoDestino where configuracaoDestino.ConfiguracaoProgramacaoCarga.Codigo = :codigoConfiguracaoProgramacaoCarga")
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
