using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaFechamentoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>
    {
        public OrdemServicoFrotaFechamentoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> BuscarPorOrdemServicoEOrigem(int codigoOrdemServico, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento origem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.Origem == origem);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.ToList();
        }

        public bool TodosProdutosConformeOrcado(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProdutoFechamentoOrdemServicoFrota.ConformeOrcado);

            return query.Count() == 0;
        }

        public decimal BuscarValorDocumentoPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Sum(o => (decimal?)o.ValorDocumento) ?? 0m;
        }

        public decimal BuscarValorOrcadoPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Sum(o => (decimal?)o.ValorOrcado) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> Consultar(int codigoOrdemServico, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
