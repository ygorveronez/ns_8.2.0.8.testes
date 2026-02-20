using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoElectroluxDocumentoTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>
    {
        public IntegracaoElectroluxDocumentoTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> BuscarPorDT(string codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            query = query.Where(o => o.NumeroNotfis == codigoDT);

            return query.ToList();
        }
        public Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte BuscarPorNumero(int codigoEmpresa, string numero, bool? geradoPorNOTFIS)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa && obj.NumeroNotfis == numero && obj.GeradoPorNOTFIS);

            if (geradoPorNOTFIS.HasValue)
                query = query.Where(o => o.GeradoPorNOTFIS == geradoPorNOTFIS);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> BuscarPorNotaFiscal(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> Consultar(string numeroDocumentoTransporte, int numeroNotaFiscal, DateTime dataInicial, DateTime dataFinal, bool semCarga, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            if (!string.IsNullOrEmpty(numeroDocumentoTransporte))
                query = query.Where(o => o.NumeroNotfis == numeroDocumentoTransporte);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.Data >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.Data <= dataFinal.AddDays(1).Date);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.Numero == numeroNotaFiscal));

            if (semCarga)
                query = query.Where(o => !o.Cargas.Any(c => c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                            c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada));

            if (!string.IsNullOrWhiteSpace(propOrdena) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propOrdena + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Fetch(o => o.Recebedor).Fetch(o => o.Empresa).ToList();
        }
        public int ContarConsulta(string numeroDocumentoTransporte, int numeroNotaFiscal, DateTime dataInicial, DateTime dataFinal, bool semCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte>();

            if (!string.IsNullOrEmpty(numeroDocumentoTransporte))
                query = query.Where(o => o.NumeroNotfis == numeroDocumentoTransporte);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.Data >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.Data <= dataFinal.AddDays(1).Date);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.Numero == numeroNotaFiscal));

            if (semCarga)
                query = query.Where(o => !o.Cargas.Any(c => c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                            c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada));

            return query.Count();
        }

        public void DeletarPorDT(IEnumerable<int> codigosDT)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE IntegracaoElectroluxDocumentoTransporteNotaFiscal obj WHERE obj.Codigo in (SELECT Codigo FROM IntegracaoElectroluxDocumentoTransporteNotaFiscal WHERE DocumentoTransporte.Codigo IN(:codigosDTs))")
                                     .SetParameterList("codigosDTs", codigosDT)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE IntegracaoElectroluxDocumentoTransporteNotaFiscal obj WHERE obj.Codigo in (SELECT Codigo FROM IntegracaoElectroluxDocumentoTransporteNotaFiscal WHERE DocumentoTransporte.Codigo IN(:codigosDTs))")
                                            .SetParameterList("codigosDTs", codigosDT)
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

    }
}
