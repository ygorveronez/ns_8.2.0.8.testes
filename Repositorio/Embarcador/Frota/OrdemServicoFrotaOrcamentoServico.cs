using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaOrcamentoServico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>
    {
        public OrdemServicoFrotaOrcamentoServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> BuscarPorOrcamento(int codigoOrcamentoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Orcamento.Codigo == codigoOrcamentoOrdemServico);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico BuscarPorManutencao(int codigoManutencao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Manutencao.Codigo == codigoManutencao);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Orcamento.OrdemServico.Codigo == codigoOrdemServico);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico BuscarPorOrdemServicoEManutencao(int codigoOrdemServico, int codigoManutencao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Orcamento.OrdemServico.Codigo == codigoOrdemServico && o.Manutencao.Codigo == codigoManutencao);

            return query.FirstOrDefault();
        }

        public decimal BuscarValorTotalMaoObra(int codigoOrcamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Orcamento.Codigo == codigoOrcamento);

            return query.Sum(o => o.ValorMaoObra);
        }

        public decimal BuscarValorTotalProdutos(int codigoOrcamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>();

            query = query.Where(o => o.Orcamento.Codigo == codigoOrcamento);

            return query.Sum(o => o.ValorProdutos);
        }

        #endregion
    }
}
