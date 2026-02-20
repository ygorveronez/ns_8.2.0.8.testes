using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Filiais
{
    public class Filial : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.Filial>
    {
        #region Construtores
        private CancellationToken _token;
        public Filial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Filial(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._token = cancellationToken; }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.Filial> Consultar(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial filtrosPesquisa)
        {
            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoOuCodigoIntegracao))
                consultaFilial = consultaFilial.Where(obj => obj.Descricao.Contains(filtrosPesquisa.DescricaoOuCodigoIntegracao) || (obj.CodigoFilialEmbarcador == filtrosPesquisa.DescricaoOuCodigoIntegracao) || obj.SiglaFilial.Contains(filtrosPesquisa.DescricaoOuCodigoIntegracao));
            else
            {
                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                    consultaFilial = consultaFilial.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao) || obj.SiglaFilial.Contains(filtrosPesquisa.Descricao));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                    consultaFilial = consultaFilial.Where(obj => obj.CodigoFilialEmbarcador == filtrosPesquisa.CodigoIntegracao);
            }

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaFilial = consultaFilial.Where(obj => obj.Ativo == true);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaFilial = consultaFilial.Where(obj => obj.Ativo == false);

            if (filtrosPesquisa.ListaCodigoFilialPermitidas?.Count > 0)
                consultaFilial = consultaFilial.Where(o => filtrosPesquisa.ListaCodigoFilialPermitidas.Contains(o.Codigo));

            if (filtrosPesquisa.SomenteFiliaisComSolicitacaoDeGas)
                consultaFilial = consultaFilial.Where(obj => obj.HabilitarSolicitacaoSuprimentoDeGas == true);

            if (filtrosPesquisa.SomenteLiberadasParaFilaCarregamento)
                consultaFilial = consultaFilial.Where(obj => obj.LiberarParaFilaCarregamento);

            if (filtrosPesquisa.ListaCodigoFiliaisVendasPermitidas?.Count > 0)
                consultaFilial = consultaFilial.Where(o => filtrosPesquisa.ListaCodigoFiliaisVendasPermitidas.Contains(o.Codigo));

            return consultaFilial;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Filiais.Filial buscarPorCodigoEmbarcador(string codigoFilialEmbarcador, List<Dominio.Entidades.Embarcador.Filiais.Filial> lstFiliais = null)
        {
            if (lstFiliais != null && lstFiliais.Count > 0)
                return lstFiliais.Where(obj => obj.CodigoFilialEmbarcador == codigoFilialEmbarcador || obj.OutrosCodigosIntegracao.Contains(codigoFilialEmbarcador)).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where (obj.CodigoFilialEmbarcador == codigoFilialEmbarcador || obj.OutrosCodigosIntegracao.Contains(codigoFilialEmbarcador)) && obj.Ativo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Filiais.Filial> buscarPorCodigoEmbarcadorEOutrosCodigos(List<string> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where (codigos.Contains(obj.CodigoFilialEmbarcador) || obj.OutrosCodigosIntegracao.Any(item => codigos.Contains(item))) && obj.Ativo select obj;
            result = result.Fetch(obj => obj.OutrosCodigosIntegracao);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> buscarPorCodigosEmbarcador(List<string> codigosFilialsEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where codigosFilialsEmbarcador.Contains(obj.CodigoFilialEmbarcador) && obj.Ativo select obj;
            return result.ToList();
        }

        public bool ValidarPorCodigoEmbarcador(string codigoFilialEmbarcador, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query
                         where
                             (obj.CodigoFilialEmbarcador == codigoFilialEmbarcador || obj.OutrosCodigosIntegracao.Contains(codigoFilialEmbarcador))
                             && obj.Ativo
                             && obj.Codigo != codigoFilial
                         select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(o => o.CodigoFilialEmbarcador == codigoIntegracao)
                .FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Filiais.Filial>> BuscarPorCodigosIntegracaoAsync(List<string> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var codigosUnicos = codigos.Select(c => c.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                    .Where(o => bloco.Contains(o.CodigoFilialEmbarcador));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public bool VerificarPorCNPJ(string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.CNPJ == cnpj && obj.Ativo select obj;
            return result.Any();
        }

        public Task<bool> VerificarPorCNPJAsync(string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.CNPJ == cnpj && obj.Ativo select obj;
            return result.AnyAsync();
        }

        public Task<bool> VerificarPorCNPJSAsync(List<string> cnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where cnpjs.Contains(obj.CNPJ) && obj.Ativo select obj;
            return result.AnyAsync();
        }

        public List<string> BuscarTodosCPNJs()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.Ativo && obj.CNPJ != "" select obj.CNPJ;
            return result.ToList();
        }
        
        public Task<List<string>> BuscarTodosCPNJsAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.Ativo && obj.CNPJ != "" select obj.CNPJ;
            return result.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPorCNPJ(string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.CNPJ == cnpj select obj;
            return result.FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarPorCNPJAsync(string cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.CNPJ == cnpj select obj;
            return result.FirstOrDefaultAsync();
        } 
        
        public Task<List<Dominio.Entidades.Embarcador.Filiais.Filial>> BuscarPorCNPJSAsync(List<string> cnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where cnpjs.Contains(obj.CNPJ) && obj.Ativo select obj;
            return result.Distinct().ToListAsync();
        }

        public List<string> BuscarListaCNPJAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.Ativo select obj;
            return result.Select(o => o.CNPJ).Distinct().ToList();
        }

        public Task<List<string>> BuscarListaCNPJAtivasAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.Ativo select obj;
            return result.Select(o => o.CNPJ).Distinct().ToListAsync(CancellationToken);
        }

        public List<double> BuscarListaDoubleCNPJAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var result = from obj in query
                         where obj.Ativo
                         select Convert.ToDouble(obj.CNPJ);

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarPorCNPJ(List<string> cnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(obj => cnpjs.Contains(obj.CNPJ));

            return query
                .Distinct()
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarMatriz()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.TipoFilial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilial.Matriz select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarMatrizAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.TipoFilial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilial.Matriz select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarMatrizes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.TipoFilial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilial.Matriz select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(_token);
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Filiais.Filial>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                    .Where(obj => bloco.Contains(obj.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public List<string> BuscarDescricoesPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Filiais.Filial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public List<string> BuscarCodigoFilialEmbarcadorPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Filiais.Filial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Select(o => o.CodigoFilialEmbarcador).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> ConsultarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var result = from obj in query where obj.Ativo select obj;

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> Consultar(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaFilial = Consultar(filtrosPesquisa);

            consultaFilial = consultaFilial.Fetch(obj => obj.Localidade);

            return ObterLista(consultaFilial, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial filtrosPesquisa)
        {
            var consultaFilial = Consultar(filtrosPesquisa);

            return consultaFilial.Count();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> ConsultarAgrupado(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (!string.IsNullOrWhiteSpace(propGrupo) && propGrupo != propOrdenacao)
                result = result.OrderBy(propGrupo + (dirOrdenacaoGrupo == "asc" ? " ascending" : " descending"));

            if (propGrupo != propOrdenacao)
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));


            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            query = query.Where(o => o.Descricao == descricao);

            return query.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Filiais.Filial>> BuscarPorDescricoesAsync(List<string> descricoes, int pageSize = 2000)
        {
            if (descricoes == null || descricoes.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var descricoesUnicas = descricoes.Select(d => d.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> resultado = new();

            for (int i = 0; i < descricoesUnicas.Count; i += pageSize)
            {
                var bloco = descricoesUnicas.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                    .Where(o => bloco.Contains(o.Descricao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public int TotalFiliaisPendentesIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Filiais.Filial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            query = query.Where(o => ((bool?)o.IntegradoERP ?? false) == false);
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarFiliaisPendenteIntegracao(int quantidade)
        {
            IQueryable<Dominio.Entidades.Embarcador.Filiais.Filial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            query = query.Where(o => ((bool?)o.IntegradoERP ?? false) == false);
            return query.Take(quantidade).ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPorDescricaoParcial(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            query = query.Where(o => o.Descricao.Contains(descricao));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarFiliaisComFilaCarregamentoLiberada()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            var result = from obj in query where obj.LiberarParaFilaCarregamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.Filial> BuscarPorCodigoInternoProduto(string codigoInterno)
        {
            var consultaFiliais = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor>()
                .Where(o => o.CodigoInterno == codigoInterno);

            return consultaFiliais.Select(x => x.Filial).ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial BuscarPorDescricaoOuCodigoIntegracao(string descricaoOuCodigoIntegracao)
        {
            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();

            consultaFilial = consultaFilial.Where(obj => obj.Descricao.Contains(descricaoOuCodigoIntegracao)
                || (obj.CodigoFilialEmbarcador == descricaoOuCodigoIntegracao));

            return consultaFilial.FirstOrDefault();
        }

        #endregion
    }
}
