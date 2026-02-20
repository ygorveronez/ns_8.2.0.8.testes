using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    public class InfracaoParcela : RepositorioBase<Dominio.Entidades.Embarcador.Frota.InfracaoParcela>
    {
        public InfracaoParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> BuscarPorInfracao(int codigoInfracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.InfracaoParcela>();

            query = query.Where(o => o.Infracao.Codigo == codigoInfracao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.InfracaoParcela BuscarPorInfracaoEParcela(int codigo, int codigoParcela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.InfracaoParcela>()
                .Where(o => o.Parcela == codigoParcela && o.Infracao.Codigo == codigo)
                .FirstOrDefault();

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> BuscarPorDocumentoAgregado(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, double cnpjAgregado, int codigoPagamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.InfracaoParcela>();
            query = query.Where(obj => obj.Infracao.DescontarLancamentoAgregadoTerceiro);

            if (codigoVeiculo > 0)
                query = query.Where(obj => obj.Infracao.Veiculo.Codigo == codigoVeiculo);

            if (cnpjAgregado > 0)
                query = query.Where(obj => obj.Infracao.Pessoa.CPF_CNPJ == cnpjAgregado);            

            if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Date >= dataInicial && obj.DataVencimento.Date <= dataFinal);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela>();

            if (codigoPagamentoAgregado > 0)            
                queryPagamento = queryPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado);

            if (queryPagamento.Count() > 0)
                query = query.Where(obj => !queryPagamento.Select(o => o.InfracaoParcela).Contains(obj));            

            return query.Distinct().ToList();
        }
    }
}
