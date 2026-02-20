using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Integracao
{
    public class NotaFiscalDTNatura : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>
    {
        public NotaFiscalDTNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura> BuscarPorDT(int codigoDTNatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Codigo == codigoDTNatura);

            return query.ToList();
        }

        public List<int> BuscarNumerosPorDT(int codigoDTNatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Codigo == codigoDTNatura);

            return query.Select(o => o.Numero).ToList();
        }

        public decimal BuscarPesoTotal(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Cargas.Any(c => c.Codigo == codigoCarga));

            return query.Sum(o => (decimal?)o.Peso) ?? 0m;
        }

        public int BuscarVolumesTotal(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Cargas.Any(c => c.Codigo == codigoCarga));

            return query.Sum(o => (int?)o.Quantidade) ?? 0;
        }

        public decimal BuscarPesoPorDT(IEnumerable<int> codigosDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => codigosDT.Contains(o.DocumentoTransporte.Codigo));

            return query.Sum(o => (decimal?)o.Peso) ?? 0m;
        }

        public int BuscarVolumesPorDT(IEnumerable<int> codigosDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => codigosDT.Contains(o.DocumentoTransporte.Codigo));

            return query.Sum(o => (int?)o.Quantidade) ?? 0;
        }


        public decimal BuscarValorFretePorDT(int codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Codigo == codigoDT);

            return query.Sum(o => (int?)o.ValorFrete) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Cargas.Any(c => c.Codigo == codigoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura> BuscarPorNotaFiscal(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Cargas.Any(c => c.Codigo == codigoCarga));

            return query.ToList();
        }

        public List<string> BuscarChavePorDocumentoTransporte(int codigoDocumentoTransporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();

            query = query.Where(o => o.DocumentoTransporte.Codigo == codigoDocumentoTransporte);

            return query.Select(o => o.Chave).ToList();
        }

        public void DeletarPorDT(IEnumerable<int> codigosDTsNatura)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalDTNatura obj WHERE obj.Codigo in (SELECT Codigo FROM NotaFiscalDTNatura WHERE DocumentoTransporte.Codigo IN(:codigosDTsNatura))")
                                     .SetParameterList("codigosDTsNatura", codigosDTsNatura)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE NotaFiscalDTNatura obj WHERE obj.Codigo in (SELECT Codigo FROM NotaFiscalDTNatura WHERE DocumentoTransporte.Codigo IN(:codigosDTsNatura))")
                                            .SetParameterList("codigosDTsNatura", codigosDTsNatura)
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
