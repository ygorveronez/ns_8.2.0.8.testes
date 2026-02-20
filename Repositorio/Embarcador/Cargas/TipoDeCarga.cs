using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoDeCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>
    {
        #region Construtores

        public TipoDeCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoDeCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorCliente(Dominio.Entidades.Cliente cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            var result = from obj in query where obj.GrupoPessoas.Clientes.Contains(cliente) || obj.Pessoa.CPF_CNPJ == cliente.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        //TODO: ct default
        public async Task<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken = default)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            var codigosUnicos = codigos.Select(c => c).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>()
                    .Where(t => bloco.Contains(t.Codigo));
                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPorCodigoErp(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.CodigoERP == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<string> BuscarDescricoesPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPrimeira()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPorDescricao(string descricao, bool situacao, List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> lstTipoCarga = null)
        {
            if (lstTipoCarga != null && lstTipoCarga.Count > 0)
                return lstTipoCarga.Where(o => o.Descricao == descricao && o.Ativo == situacao).FirstOrDefault();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            query = query.Where(o => o.Descricao == descricao && o.Ativo == situacao);

            return query.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>> BuscarPorDescricoesAsync(List<string> descricoes, bool situacao, int pageSize = 2000)
        {
            if (descricoes == null || descricoes.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            var descricoesUnicas = descricoes.Select(d => d.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> resultado = new();

            for (int i = 0; i < descricoesUnicas.Count; i += pageSize)
            {
                var bloco = descricoesUnicas.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>()
                    .Where(o => bloco.Contains(o.Descricao) && o.Ativo == situacao);

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }



        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        /// <summary>
        /// Método para consultar os tipos de cargas distintas de uma lista de pedidos.
        /// </summary>
        /// <param name="codigos">Códigos dos pedidos.</param>
        /// <returns>Retorna a lista de tipos de cargas distintas not null da lista de pedidos informada.</returns>
        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorCodigosPedidos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(obj => obj.TipoDeCarga != null && codigos.Contains(obj.Codigo));
            return query
                .Select(x => x.TipoDeCarga)
                .Fetch(x => x.TipoCargaPrincipal)
                .Distinct()
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPorCodigoEmbarcador(string codigoEmbacador, List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> lstTipoCarga = null)
        {
            if (lstTipoCarga != null && lstTipoCarga.Count > 0)
                return lstTipoCarga.Where(obj => obj.CodigoTipoCargaEmbarcador == codigoEmbacador && obj.Ativo).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.CodigoTipoCargaEmbarcador == codigoEmbacador && obj.Ativo select obj;
            return result.FirstOrDefault();
        }
        
        public Task<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorCodigoEmbarcadorAsync(string codigoEmbacador)
        {            
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.CodigoTipoCargaEmbarcador == codigoEmbacador && obj.Ativo select obj;
            return result.FirstOrDefaultAsync();
        }

        public bool ExisteAtivoPorCodigoEmbarcador(string codigoEmbacador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.CodigoTipoCargaEmbarcador == codigoEmbacador && obj.Ativo select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPorDescricao(string codigoEmbacador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.Descricao == codigoEmbacador && obj.Ativo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorTipoCargaPrincipal(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            var result = from obj in query where obj.TipoCargaPrincipal.Codigo == codigo && obj.Ativo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoGrupoPessoas, Dominio.Entidades.Cliente pessoa, List<int> codigosTipoCarga, bool? isFornecedor, int codigoTipoOperacaoEmissao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, string codigoTipoCargaEmbarcador = "", bool filtrarSomenteDispMontagemCarga = false, bool filtrarTiposPrincipais = false, List<int> codigos = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            var result = from obj in query select obj;

            if (codigos != null && codigos.Count > 0)
                result = result.Where(o => codigos.Contains(o.Codigo));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (codigoGrupoPessoas > 0)
                result = result.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || (o.GrupoPessoas == null && o.Pessoa == null));

            if (pessoa != null && pessoa.GrupoPessoas != null)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null) || pessoa.GrupoPessoas.Codigo == obj.GrupoPessoas.Codigo);
            else if (pessoa != null)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null));

            if (codigosTipoCarga?.Count > 0)
                result = result.Where(obj => codigosTipoCarga.Contains(obj.Codigo));

            if (!string.IsNullOrWhiteSpace(codigoTipoCargaEmbarcador))
                result = result.Where(obj => obj.CodigoTipoCargaEmbarcador == codigoTipoCargaEmbarcador);

            if (filtrarSomenteDispMontagemCarga)
                result = result.Where(x => !x.IndisponivelMontagemCarregamento);

            if (filtrarTiposPrincipais)
                result = result.Where(x => x.Principal);

            if (isFornecedor.HasValue && isFornecedor.Value == true)
                result = result.Where(x => !x.NaoPermitirFornecedorEscolherNoAgendamento);

            if (codigoTipoOperacaoEmissao > 0 && PossuiTipoCargaVinculadoAoTipoOperacao(codigoTipoOperacaoEmissao, UnitOfWork))
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> queryTipoCargaEmissao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>();

                result = result.Where(o => !queryTipoCargaEmissao.Any(tcg => tcg.TipoOperacao.Codigo == codigoTipoOperacaoEmissao || tcg.TipoCarga == o) ||
                                           queryTipoCargaEmissao.Any(tcg => tcg.TipoOperacao.Codigo == codigoTipoOperacaoEmissao && tcg.TipoCarga == o));
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoGrupoPessoas, Dominio.Entidades.Cliente pessoa, List<int> codigosTipoCarga, bool? isFornecedor, int codigoTipoOperacaoEmissao, string codigoTipoCargaEmbarcador = "", bool filtrarSomenteDispMontagemCarga = false, bool filtrarTiposPrincipais = false, List<int> codigos = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            var result = from obj in query select obj;

            if (codigos != null && codigos.Count > 0)
                result = result.Where(o => codigos.Contains(o.Codigo));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (codigoGrupoPessoas > 0)
                result = result.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || (o.GrupoPessoas == null && o.Pessoa == null));

            if (pessoa != null && pessoa.GrupoPessoas != null)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null) || pessoa.GrupoPessoas.Codigo == obj.GrupoPessoas.Codigo);
            else if (pessoa != null)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null));

            if (codigosTipoCarga?.Count > 0)
                result = result.Where(obj => codigosTipoCarga.Contains(obj.Codigo));

            if (!string.IsNullOrWhiteSpace(codigoTipoCargaEmbarcador))
                result = result.Where(obj => obj.CodigoTipoCargaEmbarcador == codigoTipoCargaEmbarcador);

            if (filtrarSomenteDispMontagemCarga)
                result = result.Where(x => !x.IndisponivelMontagemCarregamento);

            if (filtrarTiposPrincipais)
                result = result.Where(x => x.Principal);

            if (isFornecedor.HasValue && isFornecedor.Value == true)
                result = result.Where(x => !x.NaoPermitirFornecedorEscolherNoAgendamento);

            if (codigoTipoOperacaoEmissao > 0 && PossuiTipoCargaVinculadoAoTipoOperacao(codigoTipoOperacaoEmissao, UnitOfWork))
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> queryTipoCargaEmissao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>();

                result = result.Where(o => !queryTipoCargaEmissao.Any(tcg => tcg.TipoOperacao.Codigo == codigoTipoOperacaoEmissao || tcg.TipoCarga == o) ||
                                           queryTipoCargaEmissao.Any(tcg => tcg.TipoCarga == o && tcg.TipoOperacao.Codigo == codigoTipoOperacaoEmissao));
            }

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> ConsultarPorFilial(List<int> codigosFilial, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<int> codigosTipoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool filtrarSomenteDispMontagemCarga = false)
        {
            var consultaTipoCarga = ConsultarPorFilial(codigosFilial, descricao, ativo, codigosTipoCarga, filtrarSomenteDispMontagemCarga);

            return ObterLista(consultaTipoCarga, parametrosConsulta);
        }

        public int ContarConsultaPorFilial(List<int> codigosFilial, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<int> codigosTipoCarga, bool filtrarSomenteDispMontagemCarga = false)
        {
            var consultaTipoCarga = ConsultarPorFilial(codigosFilial, descricao, ativo, codigosTipoCarga, filtrarSomenteDispMontagemCarga);

            return consultaTipoCarga.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador> BuscarAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            query = query.Where(o => o.Ativo);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = o.CodigoTipoCargaEmbarcador, Descricao = o.Descricao }).ToList();
        }



        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarPorDescricaoECodigosEmbarcador(List<string> descricao, List<string> CodigosEmbarcador, bool situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            query = query.Where(o => descricao.Contains(o.Descricao) && CodigosEmbarcador.Contains(o.CodigoTipoCargaEmbarcador) && o.Ativo == situacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPorDescricaoOuCodigoIntegracao(string descricaoOuCodigoIntegracao)
        {
            var consultaTipoDeCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            consultaTipoDeCarga = consultaTipoDeCarga.Where(obj => obj.Descricao.Contains(descricaoOuCodigoIntegracao)
            || (obj.CodigoTipoCargaEmbarcador == descricaoOuCodigoIntegracao));

            return consultaTipoDeCarga.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>> BuscarPorCodigosIntegracaoAsync(List<string> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            var codigosUnicos = codigos.Select(c => c.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>()
                    .Where(t => bloco.Contains(t.CodigoTipoCargaEmbarcador));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> ConsultarPorFilial(List<int> codigosFilial, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<int> codigosTipoCarga, bool filtrarSomenteDispMontagemCarga)
        {
            var consultaCentroCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>()
                .Where(o => codigosFilial.Contains(o.Filial.Codigo));

            var consultaTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            if (codigosFilial.Count > 0)
                consultaTipoCarga = consultaTipoCarga.Where(o => consultaCentroCarregamento.Where(v => v.TiposCarga.Contains(o)).Any());

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaTipoCarga = consultaTipoCarga.Where(o => o.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaTipoCarga = consultaTipoCarga.Where(o => o.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaTipoCarga = consultaTipoCarga.Where(o => o.Ativo == false);

            if (codigosTipoCarga?.Count > 0)
                consultaTipoCarga = consultaTipoCarga.Where(o => codigosTipoCarga.Contains(o.Codigo));


            if (filtrarSomenteDispMontagemCarga)
                consultaTipoCarga = consultaTipoCarga.Where(x => !x.IndisponivelMontagemCarregamento);

            return consultaTipoCarga;
        }

        private bool PossuiTipoCargaVinculadoAoTipoOperacao(int codigoTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Pedidos.TipoOperacaoTipoCargaEmissao repTipoCargaEmissaoVinculo = new Pedidos.TipoOperacaoTipoCargaEmissao(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao tipoCargaEmissaoVinculo = repTipoCargaEmissaoVinculo.BuscarPorTipoOperacao(codigoTipoOperacao);
            if (tipoCargaEmissaoVinculo != null)
                return true;

            return false;
        }

        #endregion Métodos Privados
    }
}
