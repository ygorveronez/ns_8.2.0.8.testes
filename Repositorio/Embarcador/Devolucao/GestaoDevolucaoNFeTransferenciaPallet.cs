using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoNFeTransferenciaPallet : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet>
    {
        public GestaoDevolucaoNFeTransferenciaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet> consultaGestaoDevolucaoNFeTransferenciaPallets = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet>();
            consultaGestaoDevolucaoNFeTransferenciaPallets = consultaGestaoDevolucaoNFeTransferenciaPallets.Where(nfTransferencia => nfTransferencia.CargaEntrega.Codigo == codigoCargaEntrega);
            
            return consultaGestaoDevolucaoNFeTransferenciaPallets.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet> consultaGestaoDevolucaoNFeTransferenciaPallets = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet>();
            consultaGestaoDevolucaoNFeTransferenciaPallets = consultaGestaoDevolucaoNFeTransferenciaPallets.Where(nfTransferencia => nfTransferencia.CargaEntrega.Carga.Codigo == codigoCarga);

            return consultaGestaoDevolucaoNFeTransferenciaPallets.ToList();
        }
    }
}
