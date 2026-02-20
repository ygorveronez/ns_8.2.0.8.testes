using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Pallets
{
    public class CompraPallets : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.CompraPallets>
    {
        #region Construtores

        public CompraPallets(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CompraPallets(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.CompraPallets> Consultar(DateTime dataInicio, DateTime dataFim, int filial, int numero, double fornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets? situacao, int transportador)
        {
            var consultaCompraPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();

            if (dataInicio != DateTime.MinValue)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.DataFinalizacao.Value.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.DataFinalizacao.Value.Date <= dataFim);

            if (filial > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.Filial.Codigo == filial);

            if (transportador > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.Transportador.Codigo == transportador);

            if (numero > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.Numero == numero);

            if (fornecedor > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.Fornecedor.CPF_CNPJ == fornecedor);

            if (situacao.HasValue)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.Situacao == situacao.Value);

            return consultaCompraPallet;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.CompraPallets> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet filtrosPesquisa)
        {
            var consultaCompraPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.DataCriacao >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.DataCriacao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => filtrosPesquisa.ListaCodigoFilial.Contains(o.Filial.Codigo));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => filtrosPesquisa.ListaCodigoTransportador.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.ListaCpfCnpjFornecedor?.Count > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => filtrosPesquisa.ListaCpfCnpjFornecedor.Contains(o.Fornecedor.CPF_CNPJ));

            if (filtrosPesquisa.NumeroNfe > 0)
                consultaCompraPallet = consultaCompraPallet.Where(o => o.Numero == filtrosPesquisa.NumeroNfe);

            return consultaCompraPallet;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();
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
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.DataFinalizacao >= dataInicial
                             && obj.DataFinalizacao <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets.Finalizado
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pallets.CompraPallets BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pallets.CompraPallets BuscarPorChaveXML(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.CompraPallets>();
            var result = from obj in query where obj.XMLNotaFiscal.Chave == chave select obj;
            return result.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> Consultar(DateTime dataInicio, DateTime dataFim, int filial, int numero, double fornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets? situacao, int transportador, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(dataInicio, dataFim, filial, numero, fornecedor, situacao, transportador);

            return ObterLista(result, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaCompraPallet = ConsultarRelatorio(filtrosPesquisa);

            return ObterLista(consultaCompraPallet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int filial, int numero, double fornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets? situacao, int transportador)
        {
            var result = Consultar(dataInicio, dataFim, filial, numero, fornecedor, situacao, transportador);

            return result.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaEstoqueCompraPallet filtrosPesquisa)
        {
            var consultaCompraPallet = ConsultarRelatorio(filtrosPesquisa);

            return consultaCompraPallet.Count();
        }

        #endregion
    }
}
