using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class PostoCombustivelTabelaValores : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>
    {
        public PostoCombustivelTabelaValores(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public PostoCombustivelTabelaValores(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produto BuscarProdutoPorPessoa(string codigoProduto, double cnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where obj.CodigoIntegracao == codigoProduto && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPessoa select obj;
            return result.Select(c => c.Produto)?.FirstOrDefault() ?? null;
        }

        public Task<Dominio.Entidades.Produto> BuscarProdutoPorPessoaAsync(string codigoProduto, double cnpjPessoa, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where obj.CodigoIntegracao == codigoProduto && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPessoa select obj;
            
            return result.Select(c => c.Produto)?.FirstOrDefaultAsync(cancellationToken) ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores> ConsultarEquipamentosPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();

            query = query.Where(obj => obj.Integrado != true);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarEquipamentosPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();

            query = query.Where(obj => obj.Integrado != true);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores BuscarTabelaPorParametros(double cnpjPosto, string codigoIntegracaoProduto, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            if (cnpjPosto > 0)
                query = query.Where(c => c.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPosto);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoProduto))
                query = query.Where(c => c.Produto.CodigoProduto == codigoIntegracaoProduto);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoProduto))
                query = query.Where(c => c.CodigoIntegracao == codigoIntegracaoProduto);

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                query = query.Where(c => c.DataInicial.HasValue && c.DataInicial.Value.Date <= dataInicial.Value.Date);

            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                query = query.Where(c => c.DataFinal.HasValue && c.DataFinal.Value.Date >= dataFinal.Value.Date);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores> BuscarPorModalidades(List<int> modalidadesFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where modalidadesFornecedor.Contains(obj.ModalidadeFornecedorPessoas.Codigo) select obj;
            return result.Fetch(obj => obj.Produto).Fetch(obj => obj.ModalidadeFornecedorPessoas).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores> BuscarCombustiveisPorClientes(List<double> cnpjsCpf, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = (from obj in query
                          where cnpjsCpf.Contains(obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ)
        && obj.Produto.ProdutoCombustivel == true
                          select obj);

            if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
                result = result.Where(obj => obj.Produto.CodigoNCM.StartsWith("271121") || obj.Produto.CodigoNCM.StartsWith("271019") || obj.Produto.CodigoNCM.StartsWith("271012"));
            else
                result = result.Where(obj => obj.Produto.CodigoNCM.StartsWith("310210"));


            return result.Fetch(obj => obj.Produto)
                .Fetch(obj => obj.ModalidadeFornecedorPessoas)
                .ThenFetch(obj => obj.ModalidadePessoas)
                .ToList();
        }


        public decimal BuscarValorCombustivel(int codigoCombustivel, double cnpjPosto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where obj.Produto.Codigo == codigoCombustivel && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPosto select obj;
            return result.FirstOrDefault() != null ? result.FirstOrDefault().ValorFixo : 0;
        }

        public decimal BuscarValorCombustivel(int codigoCombustivel, double cnpjPosto, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where obj.Produto.Codigo == codigoCombustivel && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPosto && obj.ModalidadeFornecedorPessoas.PostoConveniado == true select obj;
            if (data > DateTime.MinValue)
                result = result.Where(o => (o.DataInicial.Value <= data && (o.DataFinal.Value >= data || !o.DataFinal.HasValue)) || (!o.DataInicial.HasValue && !o.DataFinal.HasValue));
            return result.FirstOrDefault() != null ? result.FirstOrDefault().ValorFixo : 0;
        }

        public (decimal ValorDe, decimal ValorAte) BuscarValorCombustivelDeAte(int codigoCombustivel, double cnpjPosto, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>()
                .Where(obj => obj.Produto.Codigo == codigoCombustivel && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPosto && obj.ModalidadeFornecedorPessoas.PostoConveniado == true);

            if (data > DateTime.MinValue)
                query = query.Where(obj => (obj.DataInicial.Value <= data && (obj.DataFinal.Value >= data || !obj.DataFinal.HasValue)) || (!obj.DataInicial.HasValue && !obj.DataFinal.HasValue));

            return query.Select(obj => ValueTuple.Create((obj.ValorFixo), (obj.ValorAte ?? 0m))).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores BuscarModalidadeFornecedor(int codigoCombustivel, double cnpjPosto, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
            var result = from obj in query where obj.Produto.Codigo == codigoCombustivel && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpjPosto && obj.ModalidadeFornecedorPessoas.PostoConveniado == true select obj;
            if (data > DateTime.MinValue)
                result = result.Where(o => ((o.DataInicial.Value <= data && (o.DataFinal.Value >= data || !o.DataFinal.HasValue)) || (!o.DataFinal.HasValue && !o.DataInicial.HasValue)));
            return result.FirstOrDefault();
        }

        public void DeletarRegistroImportacaoTabelaValor(int codigoTabela)
        {
            var querySql = $@"UPDATE T_IMPORTACAO_PRECO_COMBUSTIVEL_LINHA SET MOT_CODIGO = NULL WHERE MOT_CODIGO = {codigoTabela} ";

            var query = this.SessionNHiBernate.CreateSQLQuery(querySql);

            query.ExecuteUpdate();
        }

    }
}
