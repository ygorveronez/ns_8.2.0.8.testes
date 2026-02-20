using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cotacao
{
    public class RegraCotacao : RepositorioBase<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao>
    {
        #region Construtores

        public RegraCotacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> Consultar(DateTime? dataInicio, DateTime? dataFim, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        #endregion


        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(dataInicio, dataFim, descricao, ativo);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = Consultar(dataInicio, dataFim, descricao, ativo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> ObterRegrasAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao>();
            var result = from obj in query where obj.Ativo select obj;

            return result
                .OrderByDescending(obj => obj.PrioridadeRegra)
                .Fetch(obj => obj.Transportadores)
                .Fetch(obj => obj.RegrasCepDestino)
                .Fetch(obj => obj.RegrasCubagem)
                .Fetch(obj => obj.RegrasDistancia)
                .Fetch(obj => obj.RegrasEstadoDestino)
                .Fetch(obj => obj.RegrasExpedidor)
                .Fetch(obj => obj.RegrasDestinatario)
                .Fetch(obj => obj.RegrasGrupoProduto)
                .Fetch(obj => obj.RegrasLinhaSeparacao)
                .Fetch(obj => obj.RegrasMarcaProduto)
                .Fetch(obj => obj.RegrasPeso)
                .Fetch(obj => obj.RegrasProduto)
                .Fetch(obj => obj.RegrasCotacaoTransportador)
                .Fetch(obj => obj.RegrasValorMercadoria)
                .Fetch(obj => obj.RegrasVolume)
                .Fetch(obj => obj.RegrasArestaProduto)
                .Fetch(obj => obj.RegrasValorCotacao)
                .ToList();
        }

        public bool VerificarSeExisteRegraCotacao()
        {
            var consultaRegraCotacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao>();

            return consultaRegraCotacao.Count() > 0;
        }

        #endregion

    }
}
