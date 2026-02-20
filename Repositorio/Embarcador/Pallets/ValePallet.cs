using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Pallets;

namespace Repositorio.Embarcador.Pallets
{
    public class ValePallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.ValePallet>
    {
        #region Construtores

        public ValePallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.ValePallet> Consultar(FiltroPesquisaValePallet filtrosPesquisa)
        {
            var listaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();

            if (filtrosPesquisa.DataInicial.HasValue)
                listaValePallet = listaValePallet.Where(o => o.DataLancamento >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                listaValePallet = listaValePallet.Where(o => o.DataLancamento <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoFilial > 0)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.Numero > 0)
                listaValePallet = listaValePallet.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.NumeroNfe > 0)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNfe);

            if (filtrosPesquisa.CpfCnpjCliente > 0)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.XMLNotaFiscal.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjCliente);

            if (filtrosPesquisa.Situacao.HasValue)
                listaValePallet = listaValePallet.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoChamado > 0)
                listaValePallet = listaValePallet.Where(o => o.Chamado.Codigo == filtrosPesquisa.CodigoChamado);

            return listaValePallet;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.ValePallet> ConsultarDevolucaoValePallet(FiltroPesquisaDevolucaoValePallet filtrosPesquisa)
        {
            var listaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();

            if (filtrosPesquisa.CodigoFilial > 0)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CpfCnpjCliente > 0d)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.XMLNotaFiscal.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjCliente);

            if (filtrosPesquisa.DataInicial.HasValue)
                listaValePallet = listaValePallet.Where(o => o.DataLancamento >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                listaValePallet = listaValePallet.Where(o => o.DataLancamento <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Nfe > 0)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.XMLNotaFiscal.Numero == filtrosPesquisa.Nfe);

            if (filtrosPesquisa.Numero > 0)
                listaValePallet = listaValePallet.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Situacao.HasValue)
                listaValePallet = listaValePallet.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return listaValePallet;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.ValePallet> ConsultarRelatorio(FiltroPesquisaControleValePallet filtrosPesquisa)
        {
            var listaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();

            if (filtrosPesquisa.DataInicial.HasValue)
                listaValePallet = listaValePallet.Where(o => o.DataLancamento >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                listaValePallet = listaValePallet.Where(o => o.DataLancamento <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
                listaValePallet = listaValePallet.Where(o => filtrosPesquisa.ListaCodigoFilial.Contains(o.Devolucao.Filial.Codigo));

            if (filtrosPesquisa.ListaCpfCnpjCliente?.Count > 0)
                listaValePallet = listaValePallet.Where(o => filtrosPesquisa.ListaCpfCnpjCliente.Contains(o.Devolucao.XMLNotaFiscal.Destinatario.CPF_CNPJ));

            if (filtrosPesquisa.NumeroNfe > 0)
                listaValePallet = listaValePallet.Where(o => o.Devolucao.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNfe);

            if (filtrosPesquisa.Situacao.HasValue)
                listaValePallet = listaValePallet.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return listaValePallet;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();
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
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.DataRecolhimento >= dataInicial
                             && obj.DataRecolhimento <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.Recolhido
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Pallets.ValePallet BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> BuscarPorChamado(int chamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();
            var result = from obj in query where obj.Chamado.Codigo == chamado select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pallets.ValePallet BuscarPorDevolucaoComValePalletDevolvido(int codigoDevolucao)
        {
            var consultaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>()
                .Where(devolucao =>
                    (devolucao.ValePallet.Devolucao.Codigo == codigoDevolucao) &&
                    (devolucao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Finalizada)
                )
                .Select(devolucao => devolucao.ValePallet);

            return consultaValePallet.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pallets.ValePallet BuscarPorDevolucaoComValePalletNaoDevolvido(int codigoDevolucao)
        {
            var consultaDevolucaoValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>()
                .Where(devolucao => devolucao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Finalizada);

            var consultaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>()
                .Where(vale => (
                    (vale.Devolucao.Codigo == codigoDevolucao) &&
                    (vale.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.AgRecolhimento) &&
                    (
                        from devolucao in consultaDevolucaoValePallet
                        where devolucao.ValePallet.Codigo == vale.Codigo
                        select devolucao.ValePallet.Codigo
                    ).Count() == 0
                ));

            return consultaValePallet.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pallets.ValePallet BuscarPorCodigoDevolucao(int codigoDevolucao)
        {
            var consultaValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();

            consultaValePallet = consultaValePallet.Where(vale => vale.Devolucao.Codigo == codigoDevolucao);

            return consultaValePallet.FirstOrDefault();
        }
        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.ValePallet>();
            var result = from obj in query select obj.Numero;

            int maiorNumero = 0;
            if (result.Count() > 0)
                maiorNumero = result.Max();

            return maiorNumero + 1;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> Consultar(FiltroPesquisaValePallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaValePallet = Consultar(filtrosPesquisa);

            return ObterLista(consultaValePallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> ConsultarDevolucaoValePallet(FiltroPesquisaDevolucaoValePallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaValePallet = ConsultarDevolucaoValePallet(filtrosPesquisa);

            return ObterLista(consultaValePallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.ValePallet> ConsultarRelatorio(FiltroPesquisaControleValePallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaValePallet = ConsultarRelatorio(filtrosPesquisa);

            return ObterLista(consultaValePallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(FiltroPesquisaValePallet filtrosPesquisa)
        {
            var listaValePallet = Consultar(filtrosPesquisa);

            return listaValePallet.Count();
        }

        public int ContarConsultaControleValePallet(FiltroPesquisaDevolucaoValePallet filtrosPesquisa)
        {
            var listaValePallets = ConsultarDevolucaoValePallet(filtrosPesquisa);

            return listaValePallets.Count();
        }

        public int ContarConsultaRelatorio(FiltroPesquisaControleValePallet filtrosPesquisa)
        {
            var listaValePallet = ConsultarRelatorio(filtrosPesquisa);

            return listaValePallet.Count();
        }

        #endregion
    }
}
