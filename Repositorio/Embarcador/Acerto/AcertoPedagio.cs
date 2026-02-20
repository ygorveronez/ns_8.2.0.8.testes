using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoPedagio : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>
    {
        public AcertoPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoPedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> BuscarPorCodigoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.Pedagio.MoedaCotacaoBancoCentral == null || moedas.Contains(obj.Pedagio.MoedaCotacaoBancoCentral.Value)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.Pedagio.MoedaCotacaoBancoCentral != null && moedas.Contains(obj.Pedagio.MoedaCotacaoBancoCentral.Value));

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                return query.Sum(o => (decimal?)o.Pedagio.Valor) ?? 0m;
            else
                return query.Sum(o => (decimal?)o.Pedagio.ValorOriginalMoedaEstrangeira) ?? 0m;
        }

        public bool ExistePorAcertoESituacaoDiff(int codigoAcertoViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigoAcertoViagem && obj.Pedagio.SituacaoPedagio != situacaoDiff && obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> BuscarPorCodigoAcertoVeiculo(int codigo, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.Pedagio.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public bool ContemPedagioDuplicado(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto &&
                                     queryPedagio.Any(p => p.Codigo != o.Pedagio.Codigo && p.Veiculo.Codigo == o.Pedagio.Veiculo.Codigo && p.Praca == o.Pedagio.Praca && p.Rodovia == o.Pedagio.Rodovia && p.Data == o.Pedagio.Data && p.TipoPedagio == o.Pedagio.TipoPedagio));

            return query.Any();
        }

        public bool PedagioDuplicado(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, string praca, string rodovia, DateTime data, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var result = from obj in query
                         where obj.Veiculo.Codigo == codigoVeiculo
                            && obj.Data == data
                            && obj.TipoPedagio == tipoPedagio
                         select obj;

            if (!string.IsNullOrWhiteSpace(rodovia))
                result = result.Where(obj => obj.Rodovia.Equals(rodovia.ToUpper()));

            if (!string.IsNullOrWhiteSpace(praca))
                result = result.Where(obj => obj.Praca.Equals(praca.ToUpper()));

            return result.Count() > 1;
        }

        public void DeletarPorAcerto(int codigoAcerto)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM AcertoPedagio c WHERE c.AcertoViagem.Codigo = :codigoAcerto").SetInt32("codigoAcerto", codigoAcerto).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM AcertoPedagio c WHERE c.AcertoViagem.Codigo = :codigoAcerto").SetInt32("codigoAcerto", codigoAcerto).ExecuteUpdate();

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