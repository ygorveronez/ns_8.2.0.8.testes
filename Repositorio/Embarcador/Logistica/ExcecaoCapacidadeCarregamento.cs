using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ExcecaoCapacidadeCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>
    {
        #region Construtores

        public ExcecaoCapacidadeCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private void ExecucaoDeletarPorCodigo(int codigo)
        {
            UnitOfWork.Sessao.CreateQuery(@"DELETE PeriodoCarregamentoTipoOperacaoSimultaneo TipoOperacao 
                                            WHERE TipoOperacao.PeriodoCarregamento.Codigo IN (
                                                SELECT Periodo.Codigo FROM PeriodoCarregamento Periodo WHERE Periodo.ExcecaoCapacidadeCarregamento.Codigo = :Excecao
                                            )")
                             .SetInt32("Excecao", codigo)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE from T_CENTRO_CARREGAMENTO_PREVISAO_CARREGAMENTO_MODELO_VEICULO 
                                            WHERE PRC_CODIGO IN (
                                                SELECT PrevisaoCarregamento.PRC_CODIGO FROM T_CENTRO_CARREGAMENTO_PREVISAO_CARREGAMENTO PrevisaoCarregamento WHERE PrevisaoCarregamento.CEX_CODIGO = :Excecao
                                            )")
                             .SetInt32("Excecao", codigo)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery(@"DELETE FROM PeriodoCarregamento obj WHERE obj.ExcecaoCapacidadeCarregamento.Codigo = :Excecao")
                             .SetInt32("Excecao", codigo)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery(@"DELETE FROM PrevisaoCarregamento obj WHERE obj.ExcecaoCapacidadeCarregamento.Codigo = :Excecao")
                             .SetInt32("Excecao", codigo)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery(@"DELETE FROM ExcecaoCapacidadeCarregamento obj WHERE obj.Codigo = :Excecao")
                             .SetInt32("Excecao", codigo)
                             .ExecuteUpdate();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeCarregamento filtrosPesquisa)
        {
            var consultaExcecaoCapacidadeCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.Data.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.Data.Date <= filtrosPesquisa.DataLimite.Value.Date);

            return consultaExcecaoCapacidadeCarregamento;
        }

        #endregion

        #region Métodos Públicos

        public void DeletarPorCodigo(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    ExecucaoDeletarPorCodigo(codigo);
                    return;
                }

                try
                {
                    UnitOfWork.Start();

                    ExecucaoDeletarPorCodigo(codigo);

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
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

        public Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento BuscarExcecaoIncompativel(int codigoExcecao, int codigoCentroCarregamento, TipoAbrangenciaExcecaoCapacidadeCarregamento tipo, DateTime dataInicial, DateTime? dataFinal, bool disponivelSegunda, bool disponivelTerca, bool disponivelQuarta, bool disponivelQuinta, bool disponivelSexta, bool disponivelSabado, bool disponivelDomingo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>();

            var result = from obj in query
                         where
                           obj.Codigo != codigoExcecao
                           && obj.CentroCarregamento.Codigo == codigoCentroCarregamento
                           && obj.TipoAbrangencia == tipo
                         select obj;

            if (tipo == TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia)
                result = result.Where(o => o.Data.Date == dataInicial.Date);
            else
            {
                result = result.Where(o =>
                    (
                        (o.Data.Date <= dataInicial.Date && o.DataFinal.Value.Date >= dataFinal.Value.Date) ||
                        (o.Data.Date >= dataInicial.Date && o.DataFinal.Value.Date <= dataFinal.Value.Date) ||
                        (o.Data.Date >= dataInicial.Date && o.Data.Date <= dataFinal.Value.Date) ||
                        (o.DataFinal.Value.Date >= dataInicial.Date && o.DataFinal.Value.Date <= dataFinal.Value.Date)
                    ) && (
                        (o.DisponivelSegunda == disponivelSegunda && disponivelSegunda) ||
                        (o.DisponivelTerca == disponivelTerca && disponivelTerca) ||
                        (o.DisponivelQuarta == disponivelQuarta && disponivelQuarta) ||
                        (o.DisponivelQuinta == disponivelQuinta && disponivelQuinta) ||
                        (o.DisponivelSexta == disponivelSexta && disponivelSexta) ||
                        (o.DisponivelSabado == disponivelSabado && disponivelSabado) ||
                        (o.DisponivelDomingo == disponivelDomingo && disponivelDomingo)
                    )
                );
            }

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento BuscarPorCentroCarregamentoEDia(int codigoCentroCarregamento, DateTime data)
        {
            var consultaExcecaoCapacidadeCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>()
                .Where(o => o.TipoAbrangencia == TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia)
                .Where(o => o.Data == data.Date)
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return consultaExcecaoCapacidadeCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento BuscarPorCentroCarregamentoEPeriodo(int codigoCentroCarregamento, DateTime data)
        {
            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(data);

            var consultaExcecaoCapacidadeCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>()
                .Where(o => o.TipoAbrangencia == TipoAbrangenciaExcecaoCapacidadeCarregamento.Periodo)
                .Where(o => o.Data <= data.Date && o.DataFinal.Value.Date >= data.Date)
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            if (diaSemana == DiaSemana.Segunda) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelSegunda);
            else if (diaSemana == DiaSemana.Terca) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelTerca);
            else if (diaSemana == DiaSemana.Quarta) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelQuarta);
            else if (diaSemana == DiaSemana.Quinta) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelQuinta);
            else if (diaSemana == DiaSemana.Sexta) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelSexta);
            else if (diaSemana == DiaSemana.Sabado) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelSabado);
            else if (diaSemana == DiaSemana.Domingo) consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DisponivelDomingo);

            return consultaExcecaoCapacidadeCarregamento.FirstOrDefault();
        }

        public DateTime? BuscarProximaDataPorCentroCarregamentoEDia(int codigoCentroCarregamento, DateTime data)
        {
            var consultaExcecaoCapacidadeCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.TipoAbrangencia == TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia &&
                    o.Data > data.Date
                );

            return consultaExcecaoCapacidadeCarregamento
                .OrderBy(o => o.Data)
                .Select(o => (DateTime?)o.Data)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento> BuscarTodosPorCentroCarregamentoEPeriodo(int codigoCentroCarregamento, DateTime data)
        {
            var consultaExcecaoCapacidadeCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>()
                .Where(o => o.TipoAbrangencia == TipoAbrangenciaExcecaoCapacidadeCarregamento.Periodo)
                .Where(o => o.Data <= data.Date && o.DataFinal.Value.Date >= data.Date)
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return consultaExcecaoCapacidadeCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaExcecaoCapacidadeCarregamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaExcecaoCapacidadeCarregamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeCarregamento filtrosPesquisa)
        {
            var consultaExcecaoCapacidadeCarregamento = Consultar(filtrosPesquisa);

            return consultaExcecaoCapacidadeCarregamento.Count();
        }

        #endregion
    }
}
