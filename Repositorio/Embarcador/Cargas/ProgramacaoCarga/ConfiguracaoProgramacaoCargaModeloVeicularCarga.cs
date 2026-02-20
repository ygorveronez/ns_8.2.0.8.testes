using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class ConfiguracaoProgramacaoCargaModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga>
    {
        #region Construtores

        public ConfiguracaoProgramacaoCargaModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga> BuscarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga>()
                .Where(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoModeloVeicularCarga
                .Fetch(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ModeloVeicularCarga)
                .ToList();
        }

        public List<int> BuscarPorConfiguracaoParaSugestao(int codigoConfiguracaoProgramacaoCarga)
        {
            var consultaConfiguracaoModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga>()
                .Where(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga);

            return consultaConfiguracaoModeloVeicularCarga
                .Select(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ModeloVeicularCarga.Codigo)
                .ToList();
        }

        public void DeletarPorConfiguracao(int codigoConfiguracaoProgramacaoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConfiguracaoProgramacaoCargaModeloVeicularCarga configuracaoModeloVeicularCarga where configuracaoModeloVeicularCarga.ConfiguracaoProgramacaoCarga.Codigo = :codigoConfiguracaoProgramacaoCarga")
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

        public bool ExistePorConfiguracaoEModeloVeicularCarga(int codigoConfiguracaoProgramacaoCarga, int codigoModeloVeicularCarga)
        {
            var consultaConfiguracaoModeloVeicularCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga>()
                .Where(configuracaoModeloVeicularCarga => configuracaoModeloVeicularCarga.ConfiguracaoProgramacaoCarga.Codigo == codigoConfiguracaoProgramacaoCarga && configuracaoModeloVeicularCarga.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

            return consultaConfiguracaoModeloVeicularCarga.Any();
        }

        #endregion Métodos Públicos
    }
}
