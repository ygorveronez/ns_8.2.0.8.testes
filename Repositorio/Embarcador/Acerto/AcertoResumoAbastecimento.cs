using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoResumoAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>
    {
        public AcertoResumoAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> BuscarPorCodigoAcertoTipo(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == tipoAbastecimento select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> BuscarPorCodigoAcertoTipo(int codigoAcerto, int codioResumo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();
            var result = from obj in query where obj.Codigo == codioResumo && obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == tipoAbastecimento select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento BuscarPorCodigoAcertoVeiculoTipo(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoAbastecimento == tipoAbastecimento select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> BuscarPorCodigoAcertoVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public int BuscarKMInicialPorCodigoAcertoVeiculoTipo(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoAbastecimento == tipoAbastecimento);

            return query.Select(o => (int?)o.KMInicial).FirstOrDefault() ?? 0;

            //var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoAbastecimento == tipoAbastecimento select obj;
            //if (result.Count() > 0)
            //    return result.FirstOrDefault().KMInicial;
            //else
            //    return 0;
        }

        public int BuscarKMTotalVeiculo(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoAbastecimento == tipoAbastecimento);

            return query.Select(o => (int?)o.KMTotalAjustado).FirstOrDefault() ?? 0;

            //var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoAbastecimento == tipoAbastecimento select obj;
            //if (result.Count() > 0)
            //    return result.FirstOrDefault().KMTotalAjustado;
            //else
            //    return 0;
        }

        public bool ResumoAutorizado(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoAbastecimento == tipoAbastecimento);

            return query.Select(o => o.ResumoAprovado).FirstOrDefault();

        }

        public int BuscarKMTotalAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);

            return query.Sum(o => (int?)o.KMTotalAjustado) ?? 1;
        }

        public int BuscarKMTotalAcerto(int codigoAcerto, string tipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel && obj.Veiculo.TipoVeiculo == tipoVeiculo);

            return query.Sum(o => (int?)o.KMTotalAjustado) ?? 1;
        }

        public decimal BuscarMediaDoAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel && obj.Media > 0 && obj.Veiculo.TipoVeiculo == "0");

            return query.Average(o => (decimal?)o.Media) ?? 1m;
        }

        public decimal BuscarMediaDoAcertoPorVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel && obj.Media > 0 && obj.Veiculo.Codigo == codigoVeiculo);

            return query.Average(o => (decimal?)o.Media) ?? 0m;
        }

        public decimal BuscarParametroDoAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel && obj.MediaIdeal > 0 && obj.Veiculo.TipoVeiculo == "0");

            return query.Average(o => (decimal?)o.MediaIdeal) ?? 1m;
        }

        public void DeletarResumoSemVeiculo(int codigoAcerto)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE AcertoResumoAbastecimento obj WHERE obj.AcertoViagem.Codigo = :codigoAcerto AND obj.Veiculo.Codigo not in (SELECT veic.Veiculo.Codigo FROM AcertoVeiculo veic WHERE veic.AcertoViagem.Codigo = :codigoAcerto)")
                                     .SetInt32("codigoAcerto", codigoAcerto)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE AcertoResumoAbastecimento obj WHERE obj.AcertoViagem.Codigo = :codigoAcerto AND obj.Veiculo.Codigo not in (SELECT veic.Veiculo.Codigo FROM AcertoVeiculo veic WHERE veic.AcertoViagem.Codigo = :codigoAcerto)")
                                .SetInt32("codigoAcerto", codigoAcerto)
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