using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class VeiculoServicoAutorizacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga>
    {
        public VeiculoServicoAutorizacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga> BuscarNaoExtornadasPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.VeiculoServicoAutorizacaoCarga>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Situacao !=  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoServicoVeiculoCarga.AutorizacaoExtornada);

            return query.ToList();
        }
    }
}
