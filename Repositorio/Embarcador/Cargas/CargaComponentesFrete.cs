using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaComponentesFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>
    {
        public CargaComponentesFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaComponentesFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarTodosPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarTodosPorCargas(List<int> codigosCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => codigosCarga.Contains(o.Carga.Codigo));

            return query
                .Fetch(obj => obj.ComponenteFrete)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarComplementosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaComplementoFrete != null && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaComponentesFrete obj WHERE obj.Carga.Codigo = :codigoCarga AND obj.TipoComponenteFrete != :tipoComponenteFreteICMS AND obj.TipoComponenteFrete != :tipoComponenteFreteISS")
                           .SetInt32("codigoCarga", codigoCarga)
                           .SetEnum("tipoComponenteFreteICMS", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                           .SetEnum("tipoComponenteFreteISS", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS)
                           .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery("DELETE CargaComponentesFrete obj WHERE obj.Carga.Codigo = :codigoCarga AND obj.TipoComponenteFrete != :tipoComponenteFreteICMS AND obj.TipoComponenteFrete != :tipoComponenteFreteISS")
                            .SetInt32("codigoCarga", codigoCarga)
                            .SetEnum("tipoComponenteFreteICMS", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                            .SetEnum("tipoComponenteFreteISS", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS)
                            .ExecuteUpdate();

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

        public decimal BuscarPorCargaValorOutros(int codigoCarga, bool composicaoFreteFilialEmissora)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete> ignorarTiposComponente = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.FRETE,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga && obj.ComponenteFilialEmissora == composicaoFreteFilialEmissora
                         && !ignorarTiposComponente.Contains(obj.ComponenteFrete.TipoComponenteFrete)
                         select obj;

            return result.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal BuscarValorTotalPorCargaSemImpostos(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga &&
                                       obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS &&
                                       obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }
        public async Task<decimal> BuscarValorTotalPorCargaSemImpostosAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga &&
                                       obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS &&
                                       obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);

            return await query.SumAsync(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal BuscarValorTotalIncluirIntegralContratoFrete(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga &&
                                       obj.IncluirIntegralmenteContratoFreteTerceiro);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarComponentesIncluirIntegralContratoFrete(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.IncluirIntegralmenteContratoFreteTerceiro);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaSemComponenteCompoeFreteValor(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaSempreExtornar(int codigoCarga, bool sempreExtornar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SempreExtornar == sempreExtornar && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaFilialEmissora(int codigoCarga, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.ComponenteFilialEmissora == componenteFilialEmissora && o.Tipo == tipo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaComImpostos(int codigoCarga, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>> BuscarPorCargaPorCompomenteAsync(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return await result.ToListAsync();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaComImpostosSemComponenteFreteLiquido(int codigoCarga, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoComponenteFrete == tipoComponente && o.ComponenteFilialEmissora == componenteFilialEmissora);

            if (componente != null)
                query = query.Where(o => o.ComponenteFrete.Codigo == componente.Codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, bool sempreExtornar, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaComplementoFrete != null && o.TipoComponenteFrete == tipoComponente && o.SempreExtornar == sempreExtornar && o.ComponenteFilialEmissora == componenteFilialEmissora);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCargaPorCompomente(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete BuscarPrimeiroPorCargaPorCompomente(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete BuscarPorComponente(int codigoCargaComplemento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.CargaComplementoFrete.Codigo == codigoCargaComplemento select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorComponenteAsync(int codigoCargaComplemento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.CargaComplementoFrete.Codigo == codigoCargaComplemento select obj;
            return await result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorComponentesFrete(List<int> codigosComponentes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => codigosComponentes.Contains(o.ComponenteFrete.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete BuscarPorComponente(int codigoCarga, int codigoComponente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.ComponenteFrete.Codigo == codigoComponente && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarTotalCargaPorCompomente(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete BuscarPorCodigo(int codigo, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Codigo == codigo && obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCodigoAsync(int codigo, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Codigo == codigo && obj.Carga.Codigo == codigoCarga select obj;
            return await result.FirstOrDefaultAsync();
        }

        public int ContarComponentesInvalidosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.ComponenteFrete == null || obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.INCONSISTENTE) select obj.Codigo;
            return result.Count();
        }

        public Task<int> ContarComponentesInvalidosPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.ComponenteFrete == null || obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.INCONSISTENTE) select obj.Codigo;
            return result.CountAsync(cancellationToken);
        }

        public int ContarComponentesSemConfiguracaoFinanceira(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (!obj.ComponenteFrete.GerarMovimentoAutomatico || obj.ComponenteFrete.TipoMovimentoEmissao == null || obj.ComponenteFrete.TipoMovimentoCancelamento == null || obj.ComponenteFrete.TipoMovimentoAnulacao == null) select obj.Codigo;
            return result.Count();
        }

        public decimal BuscarValorCompomentePorTipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => o.Tipo == tipo && o.Carga.Codigo == codigoCarga && o.ComponenteFilialEmissora == componenteFilialEmissora);

            return query.Sum(obj => (decimal?)obj.ValorComponente) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> BuscarPorCodigosCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(o => codigosCarga.Contains(o.Carga.Codigo));

            return query.ToList();
        }

        public void DeletarPorCarga(int codigoCarga, bool componenteFilialEmissora, int codigoComponente = 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete? tipoComponente = null, bool? sempreExtornar = null)
        {
            try
            {
                string where = "";
                if (codigoComponente > 0)
                    where = " and obj.ComponenteFrete.Codigo = :componenteFrete ";

                if (sempreExtornar.HasValue)
                    where += " and obj.SempreExtornar = :sempreExtornar ";

                if (tipoComponente.HasValue)
                    where += " and obj.TipoComponenteFrete = :tipoComponenteFrete ";

                if (UnitOfWork.IsActiveTransaction())
                {
                    var query = UnitOfWork.Sessao.CreateQuery("DELETE CargaComponentesFrete obj WHERE obj.Carga.Codigo = :codigoCarga and obj.ComponenteFilialEmissora = : componenteFilialEmissora " + where); // SQL-INJECTION-SAFE
                    query.SetInt32("codigoCarga", codigoCarga).SetBoolean("componenteFilialEmissora", componenteFilialEmissora);

                    if (codigoComponente > 0)
                        query.SetInt32("componenteFrete", codigoComponente);

                    if (sempreExtornar.HasValue)
                        query.SetBoolean("sempreExtornar", sempreExtornar.Value);

                    if (tipoComponente.HasValue)
                        query.SetEnum("tipoComponenteFrete", tipoComponente.Value);

                    query.ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        var query = UnitOfWork.Sessao.CreateQuery("DELETE CargaComponentesFrete obj WHERE obj.Carga.Codigo = :codigoCarga and obj.ComponenteFilialEmissora = : componenteFilialEmissora " + where); // SQL-INJECTION-SAFE
                        query.SetInt32("codigoCarga", codigoCarga).SetBoolean("componenteFilialEmissora", componenteFilialEmissora);

                        if (codigoComponente > 0)
                            query.SetInt32("componenteFrete", codigoComponente);

                        if (sempreExtornar.HasValue)
                            query.SetBoolean("sempreExtornar", sempreExtornar.Value);

                        if (tipoComponente.HasValue)
                            query.SetEnum("tipoComponenteFrete", tipoComponente.Value);

                        query.ExecuteUpdate();

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

        public void DeletarPorCargaAgrupamento(int codigoCarga, bool componenteFilialEmissora, int codigoComponente = 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete? tipoComponente = null, bool? sempreExtornar = null)
        {
            try
            {
                string where = "";
                if (codigoComponente > 0)
                    where = " and obj.ComponenteFrete.Codigo = :componenteFrete ";

                if (sempreExtornar.HasValue)
                    where += " and obj.SempreExtornar = :sempreExtornar ";

                if (tipoComponente.HasValue)
                    where += " and obj.TipoComponenteFrete = :tipoComponenteFrete ";

                if (UnitOfWork.IsActiveTransaction())
                {
                    var query = UnitOfWork.Sessao.CreateQuery("DELETE CargaComponentesFrete obj WHERE obj.Carga.Codigo in (SELECT car.Codigo FROM Carga car WHERE car.CargaAgrupamento.Codigo = :codigoCarga) and obj.ComponenteFilialEmissora = : componenteFilialEmissora " + where); // SQL-INJECTION-SAFE
                    query.SetInt32("codigoCarga", codigoCarga).SetBoolean("componenteFilialEmissora", componenteFilialEmissora);

                    if (codigoComponente > 0)
                        query.SetInt32("componenteFrete", codigoComponente);

                    if (sempreExtornar.HasValue)
                        query.SetBoolean("sempreExtornar", sempreExtornar.Value);

                    if (tipoComponente.HasValue)
                        query.SetEnum("tipoComponenteFrete", tipoComponente.Value);

                    query.ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        var query = UnitOfWork.Sessao.CreateQuery("DELETE CargaComponentesFrete obj WHERE obj.Carga.Codigo in (SELECT car.Codigo FROM Carga car WHERE car.CargaAgrupamento.Codigo = :codigoCarga) and obj.ComponenteFilialEmissora = : componenteFilialEmissora " + where); // SQL-INJECTION-SAFE
                        query.SetInt32("codigoCarga", codigoCarga).SetBoolean("componenteFilialEmissora", componenteFilialEmissora);

                        if (codigoComponente > 0)
                            query.SetInt32("componenteFrete", codigoComponente);

                        if (sempreExtornar.HasValue)
                            query.SetBoolean("sempreExtornar", sempreExtornar.Value);

                        if (tipoComponente.HasValue)
                            query.SetEnum("tipoComponenteFrete", tipoComponente.Value);

                        query.ExecuteUpdate();
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
    }
}
