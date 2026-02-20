using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaOrcamento : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento>
    {
        public OrdemServicoFrotaOrcamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public decimal BuscarValorTotalOrcadoPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Sum(o => (decimal?)o.ValorTotalOrcado) ?? 0m;
        }

        #endregion
    }
}
