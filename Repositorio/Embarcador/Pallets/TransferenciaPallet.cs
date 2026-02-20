using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pallets
{
    public class TransferenciaPallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>
    {
        #region Construtores

        public TransferenciaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPallet filtrosPesquisa)
        {
            var consultaTransferenciaPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();

            if (filtrosPesquisa.Numero > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoSetor > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.CodigoTurno > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Turno.Codigo == filtrosPesquisa.CodigoTurno);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Data >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.Todas)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaTransferenciaPallet;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet filtrosPesquisa)
        {
            var consultaTransferenciaPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Solicitacao.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => filtrosPesquisa.ListaCodigoFilial.Contains(o.Solicitacao.Filial.Codigo));

            if (filtrosPesquisa.ListaCodigoSetor?.Count > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => filtrosPesquisa.ListaCodigoSetor.Contains(o.Solicitacao.Setor.Codigo));

            if (filtrosPesquisa.ListaCodigoTurno?.Count > 0)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => filtrosPesquisa.ListaCodigoTurno.Contains(o.Solicitacao.Turno.Codigo));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.Todas)
                consultaTransferenciaPallet = consultaTransferenciaPallet.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaTransferenciaPallet;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();
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
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.DataTransferencia >= dataInicial
                             && obj.DataTransferencia <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.Finalizada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>()
                .Where(transferenciaPallet => transferenciaPallet.Codigo == codigo)
                .FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaTransferencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();
            int? ultimoNumero = consultaTransferencia.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaTransferenciaPallet = Consultar(filtrosPesquisa);

            return ObterLista(consultaTransferenciaPallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaTransferenciaPallet = ConsultarRelatorio(filtrosPesquisa);

            return ObterLista(consultaTransferenciaPallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPallet filtrosPesquisa)
        {
            var listaTransferenciaPallet = Consultar(filtrosPesquisa);

            return listaTransferenciaPallet.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet filtrosPesquisa)
        {
            var listaTransferenciaPallet = ConsultarRelatorio(filtrosPesquisa);

            return listaTransferenciaPallet.Count();
        }

        #endregion
    }
}
