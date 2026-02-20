using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoComponentesFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>
    {
        public CargaPedidoComponentesFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPedidoComponentesFrete(UnitOfWork unitOfWork, CancellationToken cancellation) : base(unitOfWork, cancellation) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedido(int codigoCargaPedido, bool apenasModeloDocumento, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;

            if (apenasModeloDocumento)
            {
                if (modeloDocumentoFiscal != null)
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);
                else
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedido(int codigoCargaPedido, bool apenasModeloDocumento, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS select obj;

            if (apenasModeloDocumento)
            {
                if (modeloDocumentoFiscal != null)
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);
                else
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedidoComPisCofins(int codigoCargaPedido, bool apenasModeloDocumento, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;

            if (apenasModeloDocumento)
            {
                if (modeloDocumentoFiscal != null)
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);
                else
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedidoSemComponenteFreteValor(int codigoCargaPedido, bool apenasModeloDocumento, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS select obj;

            if (apenasModeloDocumento)
            {
                if (modeloDocumentoFiscal != null)
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);
                else
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargasSemComponenteFreteValor(List<int> cargas, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where cargas.Contains(obj.CargaPedido.Carga.Codigo) && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS select obj;

            return result
                .Fetch(obj => obj.ComponenteFrete)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();

            query = query.Where(obj => obj.CargaPedido.Pedido.Codigo == codigoPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorListaCargaPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();

            query = query.Where(obj => codigoCargaPedidos.Contains(obj.CargaPedido.Codigo));

            return query.ToList();
        }

        public decimal BuscarSomaPorCargaPedido(int codigoCargaPedido, bool incluirBCICMS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.IncluirBaseCalculoICMS == incluirBCICMS);

            return query.Sum(o => o.ValorComponente);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedidoOutrosDocumentos(int codigoCargaPedido, int codigoModeloDocumento, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;

            result = result.Where(obj => obj.ModeloDocumentoFiscal != null && obj.ModeloDocumentoFiscal.Numero != "57" && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.ModeloDocumentoFiscal.Numero != "39" && obj.ModeloDocumentoFiscal.Codigo != codigoModeloDocumento);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaPedidoQuePossuiModeloDocumentoSemMovimentacaoConfigurada(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.ModeloDocumentoFiscal != null && !obj.ModeloDocumentoFiscal.GerarMovimentoAutomatico);
            return query.ToList();
        }

        public decimal BuscarTotalCargaPorCompomente(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.CargaPedido.PedidoSemNFe == false select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public decimal BuscarTotalMoedaCargaPorCompomente(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == carga && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.CargaPedido.PedidoSemNFe == false);

            if (componente != null)
                query = query.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return query.Select(obj => (decimal?)obj.ValorTotalMoeda).Sum() ?? 0;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete BuscarPorCompomente(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool compomenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == compomenteFilialEmissora select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCompomenteAsync(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool compomenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == compomenteFilialEmissora select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return await result.FirstOrDefaultAsync();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarPorCargaAgrupado(int carga, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS && obj.CargaPedido.Carga.Codigo == carga group obj by new { obj.ComponenteFrete.Codigo, obj.TipoComponenteFrete, obj.Percentual, obj.IncluirBaseCalculoICMS } into grupo select new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico { ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete() { Codigo = grupo.Key.Codigo }, TipoComponenteFrete = grupo.Key.TipoComponenteFrete, Percentual = grupo.Key.Percentual, IncluirBaseCalculoImposto = grupo.Key.IncluirBaseCalculoICMS, ValorComponente = grupo.Sum(valor => valor.ValorComponente) };
            return result.ToList();
        }

        public decimal BuscarValorComponentesFreteLiquido(int codigoCargaPedido, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>()
                .Where(obj =>
                    (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) &&
                    obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS &&
                    obj.ComponenteFilialEmissora == componenteFilialEmissora &&
                    obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS &&
                    obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS &&
                    obj.CargaPedido.Codigo == codigoCargaPedido)
                .ToList();

            var result = query.Where(obj => obj.ComponenteFrete.SomarComponenteFreteLiquido || DescontarComponenteFreteLiquido(obj));

            //return result.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
            return result.Sum(o => DescontarComponenteFreteLiquido(o) ? (decimal?)o.ValorComponente * -1 : (decimal?)o.ValorComponente) ?? 0m;
        }

        private static bool DescontarComponenteFreteLiquido(Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete)
        {
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = cargaPedidoComponenteFrete.CargaPedido?.Carga?.TabelaFrete;
            bool destacarComponenteTabelaFrete = DestacarComponenteTabelaFrete(tabelaFrete, cargaPedidoComponenteFrete.ComponenteFrete);

            return (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : cargaPedidoComponenteFrete.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false;
        }

        public static bool DestacarComponenteTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteOriginal)
        {
            if (componenteFreteOriginal == null
                || tabelaFrete == null
                || tabelaFrete.ComponenteFreteDestacar == null
                || !tabelaFrete.DestacarComponenteFrete)
                return false;

            if (tabelaFrete.ComponenteFreteDestacar.Codigo != componenteFreteOriginal.Codigo)
                return false;

            return true;
        }

        public decimal BuscarValorComponentes(int codigoCargaPedido, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query
                         where (
                             (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) &&
                             obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS && obj.CargaPedido.Codigo == codigoCargaPedido
                               )
                         select obj;
            return result.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCargaComponentesImpostos(int codigoCarga, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query
                         where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.ComponenteFilialEmissora == componenteFilialEmissora
       && (obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS || obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS || obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS)
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCarga(int codigoCarga, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>> BuscarPorCargaAsync(int codigoCarga, bool componenteFilialEmissora, CancellationToken cancellationToken)
        {            
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            return await result.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public void DeletarPorCargaPedidoETipoComponente(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo = :CargaPedido and TipoComponenteFrete = :tipoComponenteFrete")
                                     .SetInt32("CargaPedido", codigoCargaPedido)
                                     .SetInt32("tipoComponenteFrete", (int)tipoComponente)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo = :CargaPedido and TipoComponenteFrete = :tipoComponenteFrete")
                                    .SetInt32("CargaPedido", codigoCargaPedido)
                                    .SetInt32("tipoComponenteFrete", (int)tipoComponente)
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

        public void DeletarPorCarga(int codigoCarga, bool componenteFilialEmissora)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga) and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                     .SetInt32("codigoCarga", codigoCarga)
                                     .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga) and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                    .SetInt32("codigoCarga", codigoCarga)
                                    .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
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

        public void DeletarPorCargaNaoEmitidos(int codigoCarga, bool componenteFilialEmissora)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.CTesEmitidos = 0 and cargaPedido.Carga.Codigo = :codigoCarga) and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                     .SetInt32("codigoCarga", codigoCarga)
                                     .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.CTesEmitidos = 0 and cargaPedido.Carga.Codigo = :codigoCarga) and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                    .SetInt32("codigoCarga", codigoCarga)
                                    .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
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


        public void DeletarPorCargaETipoComponente(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga) and obj.TipoComponenteFrete = :tipoComponenteFrete")
                                     .SetInt32("codigoCarga", codigoCarga)
                                      .SetInt32("tipoComponenteFrete", (int)tipoComponente)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo in (SELECT cargaPedido.Codigo FROM CargaPedido cargaPedido WHERE cargaPedido.Carga.Codigo = :codigoCarga) and obj.TipoComponenteFrete = :tipoComponenteFrete")
                                    .SetInt32("codigoCarga", codigoCarga)
                                    .SetInt32("tipoComponenteFrete", (int)tipoComponente)
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

        public void DeletarPorCargaPedido(int codigoCargaPedido, bool componenteFilialEmissora)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo = :CargaPedido  and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                     .SetInt32("CargaPedido", codigoCargaPedido)
                                     .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoComponentesFrete obj WHERE obj.CargaPedido.Codigo = :CargaPedido  and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                    .SetInt32("CargaPedido", codigoCargaPedido)
                                    .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
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
    }
}
