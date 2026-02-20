using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>
    {
        #region Construtores
        public TipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoOperacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> consultaTipoOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigosTiposOperacao != null && filtrosPesquisa.CodigosTiposOperacao.Count > 0)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => filtrosPesquisa.CodigosTiposOperacao.Contains(o.Codigo));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => !o.Ativo);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas || (o.GrupoPessoas == null && o.Pessoa == null));

            if (filtrosPesquisa.Pessoa != null)
            {
                if (filtrosPesquisa.Pessoa.GrupoPessoas != null)
                    consultaTipoOperacao = consultaTipoOperacao.Where(o => o.Pessoa.CPF_CNPJ == filtrosPesquisa.Pessoa.CPF_CNPJ || (o.GrupoPessoas == null && o.Pessoa == null) || (filtrosPesquisa.Pessoa.GrupoPessoas.Codigo == o.GrupoPessoas.Codigo));
                else
                    consultaTipoOperacao = consultaTipoOperacao.Where(o => o.Pessoa.CPF_CNPJ == filtrosPesquisa.Pessoa.CPF_CNPJ || (o.GrupoPessoas == null && o.Pessoa == null));
            }

            if (filtrosPesquisa.ListaCodigoTipoOperacaoPermitidos?.Count > 0)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => filtrosPesquisa.ListaCodigoTipoOperacaoPermitidos.Contains(o.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (filtrosPesquisa.SomenteTipoOperacaoPermiteGerarRedespacho)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.PermitirGerarRedespacho == true);

            if (filtrosPesquisa.TipoOperacaoPorTransportador && filtrosPesquisa.CodigoTransportadorLogado > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> queryTipoOperacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador>();
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> resultQueryTipoOperacao = from obj in queryTipoOperacaoTransportador select obj;

                consultaTipoOperacao = consultaTipoOperacao.Where(o => resultQueryTipoOperacao.Where(a => a.TipoOperacao.Codigo == o.Codigo
                    && a.Transportador.Codigo == filtrosPesquisa.CodigoTransportadorLogado).Any());
            }

            if (filtrosPesquisa.CodigoTipoCargaEmissao > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> queryTipoOperacaoTipoCargaEmissao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao>();

                consultaTipoOperacao = consultaTipoOperacao.Where(o => (!queryTipoOperacaoTipoCargaEmissao.Any(tcg => tcg.TipoCarga.Codigo == filtrosPesquisa.CodigoTipoCargaEmissao) &&
                                                                        !queryTipoOperacaoTipoCargaEmissao.Any(tcg => tcg.TipoOperacao == o)) ||
                                                                       queryTipoOperacaoTipoCargaEmissao.Any(tcg => tcg.TipoOperacao == o && tcg.TipoCarga.Codigo == filtrosPesquisa.CodigoTipoCargaEmissao));
            }

            if (filtrosPesquisa.FiltrarTipoOperacaoOcultas)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.ConfiguracaoMontagemCarga.OcultarTipoDeOperacaoNaMontagemDaCarga != true);

            if (filtrosPesquisa.FiltrarPorTipoDevolucao)
                consultaTipoOperacao = consultaTipoOperacao.Where(o => o.TipoCarregamento == RetornoCargaTipo.Devolucao);

            return consultaTipoOperacao;
        }

        #endregion

        #region Métodos Públicos

        public List<int> BuscarCodigosTiposOperacoesIntegrarEmbarcador(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.Ativo && o.GrupoPessoas.UtilizaMultiEmbarcador == true && o.IntegrarCargasMultiEmbarcador == true);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                query = query.Where(o => o.CTeEmitidoNoEmbarcador);

            return query.Select(o => o.Codigo).ToList();
        }


        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoGeracaoCargaDevolucao()
        {
            return (from tipoOperacao in SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>()
                    where tipoOperacao.ConfiguracaoGestaoDevolucao.UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta
                    select tipoOperacao).FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarPorCliente(Dominio.Entidades.Cliente cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.GrupoPessoas.Clientes.Contains(cliente) || obj.Pessoa.CPF_CNPJ == cliente.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacoesRetirada()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.SelecionarRetiradaProduto select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTodosAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo select obj;

            return result.Fetch(x => x.ConfiguracaoCarga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarPorGrupoPessoas(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && (obj.GrupoPessoas.Codigo == grupoPessoas.Codigo || obj.Pessoa.GrupoPessoas.Codigo == grupoPessoas.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoTotalmenteSubContradada()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoCargaForTotalmenteSubcontratada select obj;

            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoCargaEntreFiliais()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoOrigemDestinoFiliais select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoCargaEntreFiliaisAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoOrigemDestinoFiliais select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao == null && obj.GrupoPessoasSetarPadraoNaIntegracao == null select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao == null && obj.GrupoPessoasSetarPadraoNaIntegracao == null select obj;

            return result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarPorProcedimentoEmbarque(int Procedimento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.IntegracaoProcedimentoEmbarque == Procedimento select obj;

            return result.ToList();
        }

        public List<int> BuscarCodigosProcedimentoEmbarque()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<int> result = from obj in query where obj.Ativo && obj.IntegracaoProcedimentoEmbarque > 0 select obj.IntegracaoProcedimentoEmbarque;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao(int filial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao.Codigo == filial select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoAsync(int filial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao.Codigo == filial select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoGrupoPessoa(int grupoPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao && obj.GrupoPessoasSetarPadraoNaIntegracao.Codigo == grupoPessoa select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoGrupoPessoaAsync(int grupoPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao && obj.GrupoPessoasSetarPadraoNaIntegracao.Codigo == grupoPessoa select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoObservacaoEntregaInformadoNaIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoObservacaoLocalEntregaForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao == null && obj.GrupoPessoasSetarPadraoNaIntegracao == null select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoObservacaoEntregaInformadoNaIntegracaoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoObservacaoLocalEntregaForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao == null && obj.GrupoPessoasSetarPadraoNaIntegracao == null select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoForadoPais()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoParaFretesForaDoPais select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoForadoPaisAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoParaFretesForaDoPais select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoDentrodoPais()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoParaFretesDentroDoPais select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoDentrodoPaisAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoParaFretesDentroDoPais select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoQuandoObservacaoEntregaInformadoNaIntegracao(int filial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoObservacaoLocalEntregaForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao.Codigo == filial select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPadraoQuandoObservacaoEntregaInformadoNaIntegracaoAsync(int filial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.UsarComoPadraoQuandoObservacaoLocalEntregaForInformadaNaIntegracao && obj.FilialParaSetarPadraoNaIntegracao.Codigo == filial select obj;

            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoTrocaNota()
        {
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoTrocaNota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>()
                .Where(o => o.OperacaoTrocaNota)
                .FirstOrDefault();

            return tipoOperacaoTrocaNota;
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPorTipoDeCarga(int tipoDeCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.TipoDeCargaPadraoOperacao.Codigo == tipoDeCarga select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTipoOperacaoPorTipoDeCargaAsync(int tipoDeCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.TipoDeCargaPadraoOperacao.Codigo == tipoDeCarga select obj;

            return result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTiposOperacaoPorTipoDeCarga(List<int> tiposDeCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>()
                .Where(obj => tiposDeCarga.Contains(obj.TipoDeCargaPadraoOperacao.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> lstTipoOperacao = null)
        {

            if (lstTipoOperacao != null && lstTipoOperacao.Count > 0)
                return lstTipoOperacao.Where(o => o.CodigoIntegracao == codigoIntegracao).FirstOrDefault();


            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where codigosIntegracao.Contains(obj.CodigoIntegracao) && obj.Ativo select obj;

            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarPorDescricao(string descricao, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> lstTipoOperacao = null)
        {
            if (lstTipoOperacao != null && lstTipoOperacao.Count > 0)
                return lstTipoOperacao.Where(o => o.Descricao == descricao).FirstOrDefault();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.Descricao == descricao && o.Ativo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>> BuscarPorCodigosAsync(List<int> codigos)
        {

            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public List<string> BuscarDescricoesPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(obj => obj.Descricao).ToList();
        }

        public string BuscarExpressaoRegularBooking()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(obj => obj.ExpressaoRegularNumeroBookingObservacaoCTe == true);

            return query.Select(obj => obj.ExpressaoBooking)?.FirstOrDefault() ?? "";
        }

        public string BuscarExpressaoRegularContainer()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(obj => obj.ExpressaoRegularNumeroContainerObservacaoCTe == true);

            return query.Select(obj => obj.ExpressaoContainer)?.FirstOrDefault() ?? "";
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Codigo == codigo select obj;

            return result.Fetch(obj => obj.TiposComprovante).Fetch(obj => obj.ConfiguracaoEmissaoDocumento).Fetch(obj => obj.ConfiguracaoDocumentoEmissao).Fetch(obj => obj.ConfiguracaoCalculoFrete).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Codigo == codigo select obj;

            return await result
                .Fetch(obj => obj.TiposComprovante)
                .Fetch(obj => obj.ConfiguracaoEmissaoDocumento)
                .Fetch(obj => obj.ConfiguracaoDocumentoEmissao)
                .Fetch(obj => obj.ConfiguracaoCalculoFrete).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarPorCodigoFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.Fetch(obj => obj.TiposComprovante)
                .Fetch(obj => obj.ConfiguracaoEmissaoDocumento)
                .Fetch(obj => obj.ConfiguracaoDocumentoEmissao)
                .Fetch(obj => obj.ConfiguracaoCalculoFrete)
                .Fetch(obj => obj.ConfiguracaoEmissao)
                .Fetch(obj => obj.ConfiguracaoControleEntrega)
                .Fetch(obj => obj.ConfiguracaoCanhoto)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarPorPrimeiro()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> consultaTipoOperacao = Consultar(filtrosPesquisa);

            consultaTipoOperacao = consultaTipoOperacao
                .Fetch(o => o.Pessoa)
                .Fetch(o => o.GrupoPessoas);

            return ObterLista(consultaTipoOperacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> consultaTipoOperacao = Consultar(filtrosPesquisa);

            return consultaTipoOperacao.Count();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarPorGrupoPessoasEPeso(int codigoGrupoPessoas, decimal peso)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.Ativo && o.GrupoPessoas.Codigo == codigoGrupoPessoas && (o.PesoMinimo > 0m || o.PesoMaximo > 0m) && (o.PesoMinimo == 0m || o.PesoMinimo <= peso) && (o.PesoMaximo == 0m || o.PesoMaximo >= peso));

            return query.FirstOrDefault();
        }


        public bool ExisteGerarOcorrenciaPedidoEntregueForaPrazo()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where obj.Ativo && obj.GerarOcorrenciaPedidoEntregueForaPrazo select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> BuscarTiposDeOperacaoesPendentesDeIntegracao(int quantidade)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            query = query.Where(t => (((bool?)t.ConfiguracaoCarga.IntegradoERP) ?? false) == false);
            return query.Take(quantidade).ToList();
        }

        public int QuantidadeTotalDeRegistrosPendentesDeIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            query = query.Where(t => (((bool?)t.ConfiguracaoCarga.IntegradoERP) ?? false) == false);
            return query.Count();
        }

        public bool ExisteTipoOperacaoConsolidacao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.AutorizacaoEmissao ||
                                    o.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn);

            return query.Any();
        }

        public async Task<bool> ExisteTipoOperacaoConsolidacaoAsync(CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.AutorizacaoEmissao ||
                                    o.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn);

            return await query.AnyAsync(cancellationToken);
        }

        public List<(int, TipoLiberacaoPagamento)> BuscarTipoOperacaoPorCodigo(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = from obj in query where codigos.Contains(obj.Codigo) && obj.ConfiguracaoPagamentos != null select obj;

            return result.Select(x => ValueTuple.Create(x.Codigo, x.ConfiguracaoPagamentos.TipoLiberacaoPagamento)).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoIntegracaoIntercab(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem tipoTomadorCabotagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalPropostaCabotagem tipoModalPropostaCabotagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem tipoPropostaCabotagem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> result = query.Where(o => o.Ativo &&
                                          o.ConfiguracaoIntercab.Tomador == tipoTomadorCabotagem &&
                                          o.ConfiguracaoIntercab.ModalProposta == tipoModalPropostaCabotagem &&
                                          o.ConfiguracaoIntercab.TipoProposta == tipoPropostaCabotagem);

            return result.FirstOrDefault();
        }

        public bool PossuiMotoristaAjudante()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(r => r.ConfiguracaoCarga.PermitirInformarAjudantesNaCarga);

            return query.Any();
        }

        public bool ExisteTipoOperacaoPermitePacoteViaEntregaPacote()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.ConfiguracaoCarga.PermitirIntegrarPacotes);

            return query.Any();
        }

        public bool ExisteTipoDeOperacaoManterCargaNaEtapaUmAntesDataCarregamento()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.ConfiguracaoCarga.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento && o.Ativo);

            return query.Any();
        }

        public Task<bool> ExisteTipoDeOperacaoUtilizarPlanoViagemAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            query = query.Where(o => o.UtilizarPlanoViagem && o.Ativo);

            return query.AnyAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao BuscarTipoOperacaoPadraoDevolucaoTipoColeta()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            query = query.Where(to => to.Ativo && to.ConfiguracaoGestaoDevolucao.UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta);
            return query.FirstOrDefault();
        }

        #endregion
    }
}
