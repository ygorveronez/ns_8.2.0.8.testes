using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoOcorrenciaPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>
    {
        public ConfiguracaoOcorrenciaPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido> BuscarPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();
            var result = query.Where(obj => obj.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido ValidarSeExiste(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();
            var result = query.Where(obj => obj.EventoColetaEntrega == eventoColetaEntrega);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido BuscarRegraPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();
            var result = query.Where(obj => obj.EventoColetaEntrega == eventoColetaEntrega);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido BuscarRegraPorEventoETipoOperacaoDoPedidoEDaOcorrencia(int codigoTipoOperacaoColeta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();
            var result = query.Where(obj => obj.EventoColetaEntrega == eventoColetaEntrega && obj.TipoDeOcorrencia.TipoOperacaoColeta.Codigo == codigoTipoOperacaoColeta);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido BuscarPorTipoOcorrenciaEEvento(int codigoTipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            if (codigoTipoOcorrencia == 0) return null;

            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();
            var result = query.Where(obj => obj.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia && obj.EventoColetaEntrega == eventoColetaEntrega);
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido> Consultar(int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido> query = ObterQueryConsulta(tipoOcorrencia, eventoColetaEntrega);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public int ContarConsulta(int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            return ObterQueryConsulta(tipoOcorrencia, eventoColetaEntrega).Count();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido> ObterQueryConsulta(int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido>();

            if (tipoOcorrencia > 0)
                query = query.Where(o => o.TipoDeOcorrencia.Codigo == tipoOcorrencia);

            if (eventoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Todos)
                query = query.Where(o => o.EventoColetaEntrega == eventoColetaEntrega);

            return query;
        }

        #endregion

    }
}
