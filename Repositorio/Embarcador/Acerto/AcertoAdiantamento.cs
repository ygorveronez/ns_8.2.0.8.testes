using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoAdiantamento : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>
    {
        public AcertoAdiantamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento> BuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento> BuscarPorAcerto(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.Count();
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.PagamentoMotoristaTMS.MoedaCotacaoBancoCentral == null || moedas.Contains(obj.PagamentoMotoristaTMS.MoedaCotacaoBancoCentral.Value)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.PagamentoMotoristaTMS.MoedaCotacaoBancoCentral != null && moedas.Contains(obj.PagamentoMotoristaTMS.MoedaCotacaoBancoCentral.Value));

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                return query.Sum(o => (decimal?)o.PagamentoMotoristaTMS.Valor) ?? 0m;
            else
                return query.Sum(o => (decimal?)o.PagamentoMotoristaTMS.ValorOriginalMoedaEstrangeira) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento BuscarPorCodigoPagamentoeAcerto(int codigoPagamento, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.PagamentoMotoristaTMS.Codigo == codigoPagamento select obj;
            return result.FirstOrDefault();
        }

        public bool ContemPagamentoEmAcerto(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento && obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado select obj;
            return result.Count() > 0;
        }

        public decimal BuscarValorPorAcerto(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.PagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoPagamentoMotorista == tipoPagamentoMotorista && obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento select obj;

            return result.Sum(o => ((decimal?)o.PagamentoMotoristaTMS.Valor - (decimal?)o.PagamentoMotoristaTMS.SaldoDescontado)) ?? 0m;
        }

        public decimal BuscarValorTotalPorAcertoETipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query
                         where obj.AcertoViagem.Codigo == codigo && obj.PagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista == tipoMovimento
                         select obj;

            return result.Sum(o => ((decimal?)o.PagamentoMotoristaTMS.Valor)) ?? 0m;
        }

        public decimal BuscarValorTotalPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;

            return result.Sum(o => ((decimal?)o.PagamentoMotoristaTMS.Valor)) ?? 0m;
        }

        public decimal BuscarValorTotalPorAcerto(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.PagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoPagamentoMotorista == tipoPagamentoMotorista && obj.PagamentoMotoristaTMS.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento select obj;

            return result.Sum(o => ((decimal?)o.PagamentoMotoristaTMS.Valor)) ?? 0m;
        }
    }
}
