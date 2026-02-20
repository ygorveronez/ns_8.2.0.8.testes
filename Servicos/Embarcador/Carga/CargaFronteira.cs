using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaFronteira
    {
        private Repositorio.UnitOfWork unitOfWork;
        private Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira;

        public CargaFronteira(Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> ObterFronteirasPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return repCargaFronteira.BuscarPorCarga(carga.Codigo);
        }
        
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>> ObterFronteirasPorCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return await repCargaFronteira.BuscarPorCargaAsync(carga.Codigo);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> ObterFronteirasPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras)
        {
            return (from o in fronteiras where o.Carga.Codigo == carga.Codigo select o).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> ObterFronteirasPorCargas(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            return repCargaFronteira.BuscarPorCargas((from o in cargas select o.Codigo).ToList());
        }

        /*
         * A fronteira principal é a fronteira brasileira (ou, se não tiver uma brasileira, a primeira)
         */
        public Dominio.Entidades.Embarcador.Cargas.CargaFronteira ObterFronteiraPrincipal(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var fronteiras = ObterFronteirasPorCarga(carga);

            if(fronteiras.Count == 0)
            {
                return null;
            }

            foreach(var fronteira in fronteiras)
            {
                if(fronteira.Fronteira.Localidade?.Pais?.Sigla == "BR")
                {
                    return fronteira;
                }
            }

            return fronteiras[0];
        }

        /*
         * A fronteira principal é a fronteira brasileira (ou, se não tiver uma brasileira, a primeira)
         */
        public Dominio.Entidades.Embarcador.Cargas.CargaFronteira ObterFronteiraPrincipal(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras)
        {

            var fronteirasDaCarga = (from o in fronteiras where o.Carga.Codigo == carga.Codigo select o).ToList();

            if (fronteirasDaCarga.Count == 0)
            {
                return null;
            }

            foreach (var fronteira in fronteirasDaCarga)
            {
                if (fronteira.Fronteira.Localidade?.Pais?.Sigla == "BR")
                {
                    return fronteira;
                }
            }

            return fronteirasDaCarga[0];
        }

        public bool TemFronteira(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return ObterFronteirasPorCarga(carga).Count > 0;
        }

        public bool TemFronteira(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras)
        {
            return ObterFronteirasPorCarga(carga, fronteiras).Count > 0;
        }

    }
}
