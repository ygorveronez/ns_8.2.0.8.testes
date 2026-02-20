using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRelacionada : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRelacionada>
    {
        public CargaRelacionada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaRelacionada BuscarRelacionadaPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRelacionada>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaRelacionada BuscarRegistrosRelacionadosPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRelacionada>()
                .Where(o => o.CargaRelacao.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public string BuscarDescricaoRelacionadaPorCodigo(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRelacionada>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CargaRelacao != null);

            return query.Select(o => o.CargaRelacao.CodigoCargaEmbarcador).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaRelacionada filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaRelacionada filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaRelacionada filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(obj => obj.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.DataInicio.HasValue)
                query = query.Where(obj => obj.DataCriacaoCarga >= filtrosPesquisa.DataInicio.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                query = query.Where(obj => obj.DataCriacaoCarga <= filtrosPesquisa.DataLimite.Value);

            if (filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas)
                query = query.Where(obj => obj.SituacaoCarga == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoFilial > 0)
                query = query.Where(obj => obj.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoDeCarga > 0)
                query = query.Where(obj => obj.TipoDeCarga.Codigo == filtrosPesquisa.CodigoTipoDeCarga);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                query = query.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoCanalEntrega > 0)
            {
                var queryPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

                query = query.Where(obj => queryPedidoStage.Any(ps => obj.Pedidos.Any(p => p.Pedido.Codigo == ps.Pedido.Codigo && ps.Stage.CanalEntrega.Codigo == filtrosPesquisa.CodigoCanalEntrega)));
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.Relacionada.HasValue)
            {
                var queryCargaRelacionada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRelacionada>();

                if (filtrosPesquisa.Relacionada.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RelacionamentoCarga.Relacionada)
                    query = query.Where(obj => queryCargaRelacionada.Any(cr => cr.Carga.Codigo == obj.Codigo));
                else
                    query = query.Where(obj => !queryCargaRelacionada.Any(cr => cr.Carga.Codigo == obj.Codigo));
            }

            query = query.Where(obj => obj.TipoOperacao.ConfiguracaoCarga.PermitirRelacionarOutrasCargas);

            return query;
        }

    }
}
