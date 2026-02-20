using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class RegraTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga>
    {

        public RegraTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> BuscaPorDestino(double cpfcnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga>();
            var result = from obj in query select obj;

            if (cpfcnpj > 0d)
                result = result.Where(o => o.ClienteDestino.CPF_CNPJ == cpfcnpj);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> BuscaPorUFs(string origem, string destino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(origem) && !string.IsNullOrWhiteSpace(destino))
                result = result.Where(o => (o.UFOrigemDiferente == false && o.UFOrigem == origem) || (o.UFOrigemDiferente == true && o.UFOrigem != origem) || (o.UFDestinoDiferente == false && o.UFDestino == destino) || (o.UFDestinoDiferente == true && o.UFDestino != destino));
            else if (!string.IsNullOrWhiteSpace(origem))
                result = result.Where(o => (o.UFOrigemDiferente == false && o.UFOrigem == origem) || (o.UFOrigemDiferente == true && o.UFOrigem != origem));
            else if (!string.IsNullOrWhiteSpace(destino))
                result = result.Where(o => (o.UFDestinoDiferente == false && o.UFDestino == destino) || (o.UFDestinoDiferente == true && o.UFDestino != destino));

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> BuscaPorGrupo(int grupoOrigem, int grupoDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga>();
            var result = from obj in query select obj;

            if(grupoOrigem > 0 && grupoDestino > 0)
                result = result.Where(o => o.GrupoPessoasOrigem.Codigo == grupoOrigem || o.GrupoPessoasDestino.Codigo == grupoDestino);
            if (grupoOrigem > 0)
                result = result.Where(o => o.GrupoPessoasOrigem.Codigo == grupoOrigem);
            if (grupoDestino > 0)
                result = result.Where(o => o.GrupoPessoasDestino.Codigo == grupoDestino);

            return result.ToList();
        }
    }
}
