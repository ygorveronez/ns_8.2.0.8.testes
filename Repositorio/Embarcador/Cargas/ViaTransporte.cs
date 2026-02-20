using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class ViaTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ViaTransporte>
    {
        public ViaTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ViaTransporte> MontarQuery(string codigoIntegracao, string codigoIntegracaoEnvio, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ViaTransporte>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(x => x.Ativo == (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoEnvio))
                result = result.Where(obj => obj.CodigoIntegracaoEnvio == codigoIntegracaoEnvio);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ViaTransporte> Consultar(string codigoIntegracao, string codigoIntegracaoEnvio, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = MontarQuery(codigoIntegracao, codigoIntegracaoEnvio, descricao, ativo);
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string codigoIntegracao, string codigoIntegracaoEnvio, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = MontarQuery(codigoIntegracao, codigoIntegracaoEnvio, descricao, ativo);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.ViaTransporte BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ViaTransporte>();
            var result = query.Where(x => x.CodigoIntegracao == codigoIntegracao);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ViaTransporte BuscarViaTransportePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Pedido.ViaTransporte != null);
            return result.Select(obj => obj.Pedido.ViaTransporte).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ViaTransporte> BuscarViaTransportePorCargas(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = query.Where(obj => codigoCargas.Contains(obj.Carga.Codigo) && obj.Pedido.ViaTransporte != null);
            return result.Select(obj => obj.Pedido.ViaTransporte).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ViaTransporte BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ViaTransporte>();
            var result = query.Where(x => x.Descricao == descricao);
            return result.FirstOrDefault();
        }
    }
}
