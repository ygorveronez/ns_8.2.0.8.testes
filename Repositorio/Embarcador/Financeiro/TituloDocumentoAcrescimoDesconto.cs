using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloDocumentoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>
    {
        public TituloDocumentoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> BuscarPorDocumento(int codigoTituloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloDocumento.Codigo == codigoTituloDocumento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> BuscarPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.FaturaDocumentoAcrescimoDesconto.FaturaDocumento.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.TituloDocumento).Fetch(o => o.FaturaDocumentoAcrescimoDesconto).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> BuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloBaixaAcrescimo.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> BuscarPorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloDocumento.Titulo.Codigo == codigoTitulo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> BuscarPorTituloETipo(int codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoAcrescimoDescontoTituloDocumento tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.TituloDocumento.Titulo.Codigo == codigoTitulo && o.Tipo == tipo);

            return query.ToList();
        }

        public void DeletarPorTituloBaixaETipo(int codigoTituloBaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoAcrescimoDescontoTituloDocumento tipo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM TituloDocumentoAcrescimoDesconto WHERE Tipo = :tipo AND Codigo IN (SELECT tdad.Codigo FROM TituloDocumentoAcrescimoDesconto tdad WHERE tdad.TituloDocumento.Titulo.Codigo IN (SELECT tba.Titulo.Codigo FROM TituloBaixaAgrupado tba WHERE tba.TituloBaixa.Codigo = :codigoTituloBaixa))").SetInt32("codigoTituloBaixa", codigoTituloBaixa).SetEnum("tipo", tipo).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM TituloDocumentoAcrescimoDesconto WHERE Tipo = :tipo AND Codigo IN (SELECT tdad.Codigo FROM TituloDocumentoAcrescimoDesconto tdad WHERE tdad.TituloDocumento.Titulo.Codigo IN (SELECT tba.Titulo.Codigo FROM TituloBaixaAgrupado tba WHERE tba.TituloBaixa.Codigo = :codigoTituloBaixa))").SetInt32("codigoTituloBaixa", codigoTituloBaixa).SetEnum("tipo", tipo).ExecuteUpdate();

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

        #endregion

        #region Relatório de Acréscimos e Descontos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloAcrescimoDesconto> ConsultarRelatorioTituloAcrescimoDesconto(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaTituloAcrescimoDesconto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloAcrescimoDesconto)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloAcrescimoDesconto>();
        }

        public int ContarConsultaRelatorioTituloAcrescimoDesconto(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaTituloAcrescimoDesconto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
