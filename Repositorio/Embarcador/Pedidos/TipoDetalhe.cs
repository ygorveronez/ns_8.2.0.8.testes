using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoDetalhe : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe>
    {
        public TipoDetalhe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe BuscarPorCodigoIntegracao(string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe tipoDetalhe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Tipo == tipoDetalhe select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe tipoDetalhe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe>();

            var result = from obj in query where obj.Tipo == tipoDetalhe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> Pesquisa(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe tipoDetalhe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe>();

            var result = from obj in query select obj;

            if (tipoDetalhe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe.Todos)
                result = result.Where(obj => obj.Tipo == tipoDetalhe);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            return result.ToList();
        }
    }
}
