using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pallets
{
    public class AvariaPallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>
    {
        #region Construtores

        public AvariaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvaria filtrosPesquisa)
        {
            var consultaAvariaPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();

            if (filtrosPesquisa.Numero > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoMotivoAvaria > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.MotivoAvaria.Codigo == filtrosPesquisa.CodigoMotivoAvaria);

            if (filtrosPesquisa.CodigoSetor > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Data >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.Todas)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaAvariaPallet;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet filtrosPesquisa)
        {
            var consultaAvariaPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => filtrosPesquisa.ListaCodigoFilial.Contains(o.Filial.Codigo));

            if (filtrosPesquisa.ListaCodigoMotivoAvaria?.Count > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => filtrosPesquisa.ListaCodigoMotivoAvaria.Contains(o.MotivoAvaria.Codigo));

            if (filtrosPesquisa.ListaCodigoSetor?.Count > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => filtrosPesquisa.ListaCodigoSetor.Contains(o.Setor.Codigo));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => filtrosPesquisa.ListaCodigoTransportador.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Data >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.Todas)
                consultaAvariaPallet = consultaAvariaPallet.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaAvariaPallet;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorFechamento(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>()
                .Where(avaria => avaria.Codigo == codigo)
                .FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaAvaria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();
            int? ultimoNumero = consultaAvaria.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvaria filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var avarias = Consultar(filtrosPesquisa);

            return ObterLista(avarias, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.Data.Date >= dataInicial
                             && obj.Data.Date <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.Finalizada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var avarias = ConsultarRelatorio(filtrosPesquisa);

            return ObterLista(avarias, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvaria filtrosPesquisa)
        {
            var avarias = Consultar(filtrosPesquisa);

            return avarias.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet filtrosPesquisa)
        {
            var avarias = ConsultarRelatorio(filtrosPesquisa);

            return avarias.Count();
        }

        #endregion
    }
}
