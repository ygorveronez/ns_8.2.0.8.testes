using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.MontagemFeeder
{
    public class MontagemFeeder : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder>
    {
        #region Construtores

        public MontagemFeeder(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder>();

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Planilha))
                query = query.Where(o => o.Planilha.Contains(filtrosPesquisa.Planilha));

            if (filtrosPesquisa.DataImportacaoInicial.HasValue)
                query = query.Where(o => o.DataImportacao >= filtrosPesquisa.DataImportacaoInicial.Value.Date);

            if (filtrosPesquisa.DataImportacaoFinal.HasValue)
                query = query.Where(o => o.DataImportacao <= filtrosPesquisa.DataImportacaoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != SituacaoImportacaoPedido.Todas)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Mensagem))
                query = query.Where(o => o.Mensagem.Contains(filtrosPesquisa.Mensagem));

            if (filtrosPesquisa.Viagem > 0)
                query = query.Where(o => o.PedidoViagemNavio.Codigo == filtrosPesquisa.Viagem);

            if (filtrosPesquisa.PortoDestino > 0)
                query = query.Where(o => o.PortoDestino.Codigo == filtrosPesquisa.PortoDestino);

            if (filtrosPesquisa.PortoOrigem > 0)
                query = query.Where(o => o.PortoOrigem.Codigo == filtrosPesquisa.PortoOrigem);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                query = query.Where(o => o.Bookings.Contains(filtrosPesquisa.NumeroBooking));

            return query;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(c => c.Destinatario)
                .Fetch(c => c.Expedidor)
                .Fetch(c => c.Tomador)
                .Fetch(c => c.Usuario)
                .Fetch(c => c.Bookings)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder BuscarProximaImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder>();
            var result = from obj in query where obj.Situacao == SituacaoImportacaoPedido.Pendente || obj.Situacao == SituacaoImportacaoPedido.Processando select obj;
            return result.Fetch(c => c.Destinatario).ThenFetch(c => c.Localidade).ThenFetch(c => c.Estado)
                .Fetch(c => c.Expedidor).ThenFetch(c => c.Localidade).ThenFetch(c => c.Estado)
                .Fetch(c => c.Tomador).ThenFetch(c => c.Localidade).ThenFetch(c => c.Estado)
                .Fetch(c => c.Usuario).OrderBy(obj => obj.DataImportacao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            query = query.OrderBy($"{parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ascending" : "descending")}");

            if (parametrosConsulta.LimiteRegistros > 0)
                query = query.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            return query
                .Fetch(o => o.Usuario)
                .WithOptions(o => { o.SetTimeout(300); })
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.WithOptions(o => { o.SetTimeout(300); }).Count();
        }

        #endregion
    }
}
