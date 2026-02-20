using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaOrcamentoServicoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>
    {
        public OrdemServicoFrotaOrcamentoServicoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> BuscarPorOrcamentoServico(int codigoOrcamentoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.OrcamentoServico.Codigo == codigoOrcamentoServico);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.OrcamentoServico.Orcamento.OrdemServico.Codigo == codigoOrdemServico);

            return query.ToList();
        }

        public int ContarConsulta(int codigoOrcamentoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.OrcamentoServico.Codigo == codigoOrcamentoServico);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> Consultar(int codigoOrcamentoServico, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.OrcamentoServico.Codigo == codigoOrcamentoServico);

            return query.Fetch(o => o.Produto)
                        .OrderBy(propOrdena + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public decimal BuscarValorTotalProdutosPorOrcamento(int codigoOrcamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.OrcamentoServico.Orcamento.Codigo == codigoOrcamento);

            if (query.Count() > 0)
                return query.Sum(o => o.Quantidade * o.Valor);
            else
                return 0;
        }

        public decimal BuscarValorTotalProdutosPorOrcamentoServico(int codigoOrcamentoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

            query = query.Where(o => o.OrcamentoServico.Codigo == codigoOrcamentoServico);

            if (query.Count() > 0)
                return query.Sum(o => o.Quantidade * o.Valor);
            else
                return 0;
        }
    }
}
