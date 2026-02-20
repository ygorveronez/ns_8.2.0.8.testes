using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pallets.Reforma
{
    public class ReformaPallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>
    {
        #region Construtores

        public ReformaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaReforma filtrosPesquisa)
        {
            var consultaReformaPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Data >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CpfCnpjFornecedor > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Fornecedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjFornecedor);

            if (filtrosPesquisa.Numero > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoReformaPallet.Todas)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaReformaPallet;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet filtrosPesquisa)
        {
            var consultaReformaPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaReformaPallet = consultaReformaPallet.Where(o => o.Envio.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => filtrosPesquisa.ListaCodigoFilial.Contains(o.Envio.Filial.Codigo));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => filtrosPesquisa.ListaCodigoTransportador.Contains(o.Envio.Transportador.Codigo));

            if (filtrosPesquisa.ListaCpfCnpjFornecedor?.Count > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => filtrosPesquisa.ListaCpfCnpjFornecedor.Contains(o.Envio.Fornecedor.CPF_CNPJ));

            if (filtrosPesquisa.NumeroNfe > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => (from nota in o.NotasFiscaisSaida select nota.XmlNotaFiscal.Numero).Contains(filtrosPesquisa.NumeroNfe));

            if (filtrosPesquisa.NumeroNfeRetorno > 0)
                consultaReformaPallet = consultaReformaPallet.Where(o => (from nota in o.NotasFiscaisRetorno select nota.XmlNotaFiscal.Numero).Contains(filtrosPesquisa.NumeroNfeRetorno));

            return consultaReformaPallet;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();
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
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.DataRetorno >= dataInicial
                             && obj.DataRetorno <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoReformaPallet.Finalizada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>()
                .Where(reforma => reforma.Codigo == codigo)
                .FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaReforma = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet>();
            int? ultimoNumero = consultaReforma.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaReforma filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var listaTransferenciaPallet = Consultar(filtrosPesquisa);

            return ObterLista(listaTransferenciaPallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var listaTransferenciaPallet = ConsultarRelatorio(filtrosPesquisa);

            return ObterLista(listaTransferenciaPallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaReforma filtrosPesquisa)
        {
            var listaTransferenciaPallet = Consultar(filtrosPesquisa);

            return listaTransferenciaPallet.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleReformaPallet filtrosPesquisa)
        {
            var listaTransferenciaPallet = ConsultarRelatorio(filtrosPesquisa);

            return listaTransferenciaPallet.Count();
        }

        #endregion
    }
}
