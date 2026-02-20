using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCargaRegiaoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCargaRegiaoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino> BuscarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino>()
                .Where(configuracaoDestino => configuracaoDestino.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoRegiaoDestino
                .Fetch(configuracaoDestino => configuracaoDestino.Regiao)
                .ToList();
        }

        public List<int> BuscarPorConfiguracaoParaSugestao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino>()
                .Where(configuracaoDestino => configuracaoDestino.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoRegiaoDestino
                .Select(configuracaoDestino => configuracaoDestino.Regiao.Codigo)
                .ToList();
        }

        public void DeletarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConfiguracaoProgramacaoCargaRegiaoDestino configuracaoRegiaoDestino where configuracaoRegiaoDestino.ConfiguracaoProgramacaoCarga.Codigo = :codigoConfiguracaoProgramacaoCarga")
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
