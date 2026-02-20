using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoToleranciaPesoAutorizacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga>
    {
        public VeiculoToleranciaPesoAutorizacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga> BuscarNaoExtornadasPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.AutorizacaoExtornada);

            return query.ToList();
        }
    }
}
