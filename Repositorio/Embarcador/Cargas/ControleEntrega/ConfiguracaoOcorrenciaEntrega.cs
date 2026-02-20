using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class ConfiguracaoOcorrenciaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>
    {
        public ConfiguracaoOcorrenciaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> BuscarPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            var result = query.Where(obj => obj.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega ValidarSeExiste(bool alvoDoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, bool reentrega, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            var result = query.Where(obj => obj.AlvoDoPedido == alvoDoPedido && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega && obj.EventoColetaEntrega == eventoColetaEntrega && obj.Reentrega == reentrega);

            if (tipoOperacao != null)
                result = result.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao.Codigo);

            if (TipoDeOcorrencia != null)
                result = result.Where(obj => obj.TipoDeOcorrencia.Codigo == TipoDeOcorrencia.Codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> BuscarRegrasPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            var result = query.Where(obj => obj.EventoColetaEntrega == eventoColetaEntrega);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> BuscarRegrasPorEventoETipoAplicacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            var result = query.Where(obj => obj.EventoColetaEntrega == eventoColetaEntrega && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega);
            return result.ToList();
        }

        public bool ExisteConfiguracaoOcorrenciaPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            var result = query.Where(obj => obj.EventoColetaEntrega == eventoColetaEntrega);
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> Consultar(int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> query = ObterQueryConsulta(tipoOcorrencia, tipoAplicacaoColetaEntrega, eventoColetaEntrega);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public int ContarConsulta(int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            return ObterQueryConsulta(tipoOcorrencia, tipoAplicacaoColetaEntrega, eventoColetaEntrega).Count();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> ObterQueryConsulta(int tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();

            if (tipoOcorrencia > 0)
                query = query.Where(o => o.TipoDeOcorrencia.Codigo == tipoOcorrencia);

            if (tipoAplicacaoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Todos)
                query = query.Where(o => o.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega);

            if (eventoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Todos)
                query = query.Where(o => o.EventoColetaEntrega == eventoColetaEntrega);

            return query;
        }

        #endregion

    }
}
