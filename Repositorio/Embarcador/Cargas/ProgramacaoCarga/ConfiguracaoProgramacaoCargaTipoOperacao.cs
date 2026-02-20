using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCargaTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCargaTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao> BuscarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao>()
                .Where(configuracaoTipoOperacao => configuracaoTipoOperacao.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoTipoOperacao
                .Fetch(configuracaoTipoOperacao => configuracaoTipoOperacao.TipoOperacao)
                .ToList();
        }

        public List<int> BuscarPorConfiguracaoParaSugestao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao>()
                .Where(configuracaoTipoOperacao => configuracaoTipoOperacao.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoTipoOperacao
                .Select(configuracaoTipoOperacao => configuracaoTipoOperacao.TipoOperacao.Codigo)
                .ToList();
        }

        public void DeletarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConfiguracaoProgramacaoCargaTipoOperacao configuracaoTipoOperacao where configuracaoTipoOperacao.ConfiguracaoProgramacaoCarga.Codigo = :codigoConfiguracaoProgramacaoCarga")
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

        public bool ExistePorConfiguracaoETipoOperacao(int codigoConfiguracaoProgramacaoCarga, int codigoTipoOperacao)
        {
            var consultaConfiguracaoTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao>()
                .Where(configuracaoTipoOperacao => configuracaoTipoOperacao.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga && configuracaoTipoOperacao.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaConfiguracaoTipoOperacao.Any();
        }

        #endregion Métodos Públicos
    }
}
