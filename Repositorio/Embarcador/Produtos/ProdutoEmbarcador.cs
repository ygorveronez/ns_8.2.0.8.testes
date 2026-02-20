using NHibernate.Linq;
using Repositorio.Embarcador.Produtos.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>
    {
        public ProdutoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ProdutoEmbarcador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarPorCodigoCEAN(string codigoCEAN)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoCEAN == codigoCEAN select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarPorCodigoCEAN(List<string> codigoCEAN)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where codigoCEAN.Contains(obj.CodigoCEAN) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarListaPorCodigo(string codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoProdutoEmbarcador.Equals(codigoProduto) select obj;

            return result.OrderBy("Codigo").ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarPrimeiroAtivo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> ConsultarPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            var result = from obj in query where obj.Descricao == descricao select obj;

            return result.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.DataPreferencialDescargaCategoria> BuscarPorDescricoes(List<string> descricoes)
        {
            string sql = $@"SELECT 
                                  Produto.PRO_CODIGO Codigo, 
                                  Produto.GRP_DESCRICAO Descricao FROM T_PRODUTO_EMBARCADOR Produto
                           WHERE Produto.GRP_DESCRICAO LIKE '%{descricoes[0]}%' ";

            string where = string.Empty;

            for (int i = 0; i < descricoes.Count; i++)
                where = $" OR Produto.GRP_DESCRICAO LIKE '%{descricoes[i]}%'";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql + where);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.DataPreferencialDescargaCategoria)));

            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Logistica.DataPreferencialDescargaCategoria>();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>> BuscarPorCodigoAsync(int[] codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> buscarPorCodigosEmbarcador(List<string> codigosEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where codigosEmbarcador.Contains(obj.CodigoProdutoEmbarcador) select obj;
            return result.
                 Fetch(obj => obj.GrupoProduto)
                .Fetch(obj => obj.Unidade)
                .Fetch(obj => obj.LinhaSeparacao)
                .Fetch(obj => obj.MarcaProduto)
                .Fetch(obj => obj.TipoEmbalagem)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>> BuscarPendentesPorCodigosEmbarcadorAsync(List<string> codigosEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where codigosEmbarcador.Contains(obj.CodigoProdutoEmbarcador) select obj;
            return result.
                 Fetch(obj => obj.GrupoProduto)
                .ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarProdutosPorCodigoEmbarcador(string codigoEmbarcador, int codigoIgnorar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoProdutoEmbarcador == codigoEmbarcador && obj.Codigo != codigoIgnorar select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador buscarPorCodigoEmbarcador(string codigoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoProdutoEmbarcador == codigoEmbarcador select obj;
            return result
                .Fetch(obj => obj.LinhaSeparacao)
                .Fetch(obj => obj.MarcaProduto)
                .Fetch(obj => obj.GrupoProduto)?.FirstOrDefault() ?? null;
        }
        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador buscarPorCodigoDocumentacao(string codigoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoDocumentacao == codigoEmbarcador select obj;
            return result
                .Fetch(obj => obj.LinhaSeparacao)
                .Fetch(obj => obj.MarcaProduto)
                .Fetch(obj => obj.GrupoProduto)?.FirstOrDefault() ?? null;
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador buscarPorCodigoEmbarcador(string codigoEmbarcador, int grupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoProdutoEmbarcador == codigoEmbarcador && obj.GrupoPessoas.Codigo == grupoPessoa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador buscarPorCodigoEmbarcadorComGrupo(string codigoEmbarcador, int grupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.CodigoProdutoEmbarcador == codigoEmbarcador && (obj.GrupoPessoas.Codigo == grupoPessoa || obj.GrupoPessoas == null) select obj;
            return result.OrderByDescending(obj => obj.GrupoPessoas).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarPorGrupoProduto(int codGrupoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var result = from obj in query where obj.GrupoProduto.Codigo == codGrupoProduto && obj.Ativo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> Consultar(string descricao, string codigoEmbarcador, Dominio.Entidades.Cliente pessoa, int codigoGrupoPessoa, int codigoGrupoProduto, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool pessoasNaoObrigatorioProdutoEmbarcador, double codigoClienteBase, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, List<int> produtos)
        {
            var result = _Consultar(descricao, codigoEmbarcador, pessoa, codigoGrupoPessoa, codigoGrupoProduto, codigoIntegracao, ativo, pessoasNaoObrigatorioProdutoEmbarcador, codigoClienteBase, produtos);

            result = result
                .Fetch(o => o.Cliente)
                .Fetch(o => o.GrupoPessoas)
                .Fetch(o => o.GrupoProduto)
                .Fetch(o => o.ClassificacaoRiscoONU)
                .Fetch(o => o.Unidade)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, string codigoEmbarcador, Dominio.Entidades.Cliente pessoa, int codigoGrupoPessoa, int codigoGrupoProduto, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool pessoasNaoObrigatorioProdutoEmbarcador, double codigoClienteBase, List<int> produtos)
        {
            var result = _Consultar(descricao, codigoEmbarcador, pessoa, codigoGrupoPessoa, codigoGrupoProduto, codigoIntegracao, ativo, pessoasNaoObrigatorioProdutoEmbarcador, codigoClienteBase, produtos);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> ConsultarProdutosAvaria(string descricao, string codigoProdutoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<int> codigosProdutos, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarProdutosAvaria(descricao, codigoProdutoEmbarcador, ativo, codigosProdutos);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultaProdutosAvaria(string descricao, string codigoProdutoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<int> codigosProdutos)
        {
            var result = _ConsultarProdutosAvaria(descricao, codigoProdutoEmbarcador, ativo, codigosProdutos);

            return result.Count();
        }

        public bool ValidarProdutoPorIntegracao(string codigoProdutoEmbarcador, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.CodigoProdutoEmbarcador == codigoProdutoEmbarcador &&
                         obj.Codigo != codigo
                         select obj;

            return result.Count() > 0;
        }

        public void DeletarProduto(int codigoProdutoNaoDeletar, string codigoProdutoEmbarcador)
        {
            UnitOfWork.Sessao.CreateSQLQuery("delete from T_PRODUTO_EMBARCADOR where PRO_CODIGO_PRODUTO_EMBARCADOR = :codigoProdutoEmbarcador and PRO_CODIGO <> :codigoProdutoNaoDeletar").SetString("codigoProdutoEmbarcador", codigoProdutoEmbarcador).SetInt32("codigoProdutoNaoDeletar", codigoProdutoNaoDeletar).SetTimeout(6000).ExecuteUpdate();
        }

        public void UnificarProduto(int codigoProdutoNaoDeletar, string codigoProdutoEmbarcador)
        {
            UnitOfWork.Sessao.CreateSQLQuery("update T_PEDIDO set PRO_CODIGO = :codigoProdutoNaoDeletar where PRO_CODIGO in (select PRO_CODIGO from T_PRODUTO_EMBARCADOR where PRO_CODIGO_PRODUTO_EMBARCADOR = :codigoProdutoEmbarcador and PRO_CODIGO <> :codigoProdutoNaoDeletar)").SetInt32("codigoProdutoNaoDeletar", codigoProdutoNaoDeletar).SetString("codigoProdutoEmbarcador", codigoProdutoEmbarcador).SetTimeout(6000).ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarPorCodigosEmbarcadorPadraoLimpo(List<string> codigosEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>()
                .Where(a => codigosEmbarcador.Contains(a.CodigoProdutoEmbarcador));

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> _Consultar(string descricao, string codigoEmbarcador, Dominio.Entidades.Cliente pessoa, int codigoGrupoPessoa, int codigoGrupoProduto, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool pessoasNaoObrigatorioProdutoEmbarcador, double codigoClienteBase, List<int> produtos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            var result = from obj in query select obj;

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(obj => (obj.Ativo == true || obj.Ativo == false));
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                result = result.Where(obj => obj.CodigoProdutoEmbarcador == codigoEmbarcador);

            if (pessoa != null)
            {
                if (pessoasNaoObrigatorioProdutoEmbarcador)
                {
                    if (pessoa != null && pessoa.GrupoPessoas != null)
                        result = result.Where(obj => obj.GrupoPessoas.Codigo == pessoa.GrupoPessoas.Codigo || obj.Cliente.CPF_CNPJ == pessoa.CPF_CNPJ || obj.GrupoPessoas == null || obj.Cliente == null);
                    else if (pessoa != null)
                        result = result.Where(obj => obj.Cliente.CPF_CNPJ == pessoa.CPF_CNPJ || obj.Cliente == null);
                }
                else
                {
                    if (pessoa != null && pessoa.GrupoPessoas != null)
                        result = result.Where(obj => obj.GrupoPessoas.Codigo == pessoa.GrupoPessoas.Codigo || obj.Cliente.CPF_CNPJ == pessoa.CPF_CNPJ);
                    else if (pessoa != null)
                        result = result.Where(obj => obj.Cliente.CPF_CNPJ == pessoa.CPF_CNPJ);
                }
            }

            if (codigoGrupoPessoa > 0)
            {
                if (pessoasNaoObrigatorioProdutoEmbarcador)
                    result = result.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa || obj.GrupoPessoas == null);
                else
                    result = result.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa);
            }

            if (codigoGrupoProduto > 0)
                result = result.Where(obj => obj.GrupoProduto.Codigo == codigoGrupoProduto);

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoProdutoEmbarcador.Contains(codigoIntegracao));

            if (codigoClienteBase > 0)
            {
                List<int> codigosProdutosPermitidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>()
                    .Where(obj => obj.Cliente.CPF_CNPJ == codigoClienteBase)
                    .Select(obj => obj.SuprimentoDeGas.ProdutoPadrao.Codigo)
                    .ToList();

                result = result.Where(obj => codigosProdutosPermitidos.Contains(obj.Codigo));
            }

            if (produtos != null && produtos.Count > 0)
                result = result.Where(obj => produtos.Contains(obj.Codigo));

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> _ConsultarProdutosAvaria(string descricao, string codigoProdutoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<int> codigosProdutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            var queryProdutoAvaria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();

            var resultProdutoAvaria = from obj in queryProdutoAvaria select obj.ProdutoEmbarcador;
            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                result = result.Where(o => o.CodigoProdutoEmbarcador.Contains(codigoProdutoEmbarcador));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Ativo == (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false));

            if (codigosProdutos != null)
                result = result.Where(o => codigosProdutos.Contains(o.Codigo));

            // Busca somente produtos que possuem alguma configuração de avaria
            result = result.Where(o => resultProdutoAvaria.Contains(o));

            return result;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador> ConsultarRelatorioProdutoEmbarcador(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaProdutoEmbarcador().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoEmbarcador>();
        }

        public int ContarConsultaRelatorioProdutoEmbarcador(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaProdutoEmbarcador().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
