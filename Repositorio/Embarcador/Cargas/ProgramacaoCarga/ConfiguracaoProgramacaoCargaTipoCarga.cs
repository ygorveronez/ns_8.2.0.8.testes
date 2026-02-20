using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCargaTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCargaTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga> BuscarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga>()
                .Where(configuracaoTipoCarga => configuracaoTipoCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoTipoCarga
                .Fetch(configuracaoTipoCarga => configuracaoTipoCarga.TipoCarga)
                .ToList();
        }

        public List<int> BuscarPorConfiguracaoParaSugestao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga>()
                .Where(configuracaoTipoCarga => configuracaoTipoCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoTipoCarga
                .Select(configuracaoTipoCarga => configuracaoTipoCarga.TipoCarga.Codigo)
                .ToList();
        }

        public void DeletarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConfiguracaoProgramacaoCargaTipoCarga configuracaoTipoCarga where configuracaoTipoCarga.ConfiguracaoProgramacaoCarga.Codigo = :codigoConfiguracaoProgramacaoCarga")
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

        public bool ExistePorConfiguracaoETipoCarga(int codigoConfiguracaoProgramacaoCarga, int codigoTipoCarga)
        {
            var consultaConfiguracaoTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga>()
                .Where(configuracaoTipoCarga => configuracaoTipoCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga && configuracaoTipoCarga.TipoCarga.Codigo == codigoTipoCarga);

            return consultaConfiguracaoTipoCarga.Any();
        }

        #endregion Métodos Públicos
    }
}
