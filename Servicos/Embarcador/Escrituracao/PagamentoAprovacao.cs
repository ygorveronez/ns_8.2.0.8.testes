using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Escrituracao
{
    public sealed class PagamentoAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Escrituracao.Pagamento,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly string _urlAcesso;

        #endregion

        #region Construtores

        public PagamentoAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, urlAcesso: null) { }

        public PagamentoAprovacao(Repositorio.UnitOfWork unitOfWork, string urlAcesso) : this(unitOfWork, configuracaoEmbarcador: null, urlAcesso) { }

        public PagamentoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, string urlAcesso) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _urlAcesso = urlAcesso;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorio = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento()
                        {
                            OrigemAprovacao = pagamento,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = pagamento.DataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(pagamento, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento()
                    {
                        OrigemAprovacao = pagamento,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = pagamento.DataCriacao,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            pagamento.Situacao = existeRegraSemAprovacao ? SituacaoPagamento.AguardandoAprovacao : SituacaoPagamento.EmFechamento;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento>();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, pagamento.Filial?.Codigo))
                    continue;

                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, pagamento.Empresa?.Codigo))
                    continue;

                if (regra.RegraPorValorPagamento && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaValorPagamento, decimal>(regra.AlcadasValorPagamento, pagamento.ValorPagamento))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            StringBuilder nota = new StringBuilder(string.Format(Localization.Resources.Escrituracao.PagamentoAprovacao.CriadaSolicitacaoLiberacaoPagamento, pagamento.Numero));

            if (!string.IsNullOrWhiteSpace(_urlAcesso)) 
                nota.AppendLine(".").AppendLine().Append($"{(Configuracoes.Ambiente.Seguro(_unitOfWork) ? "https" : "http")}://{_urlAcesso}");

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: pagamento.Codigo,
                URLPagina: "Escrituracao/Pagamento",
                titulo: Localization.Resources.Escrituracao.PagamentoAprovacao.Pagamento,
                nota: nota.ToString(),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(pagamento);

            if (!UtilizarAlcadaAprovacaoPagamento())
                return;

            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento> regras = ObterRegrasAutorizacao(pagamento);

            if (regras.Count > 0)
                CriarRegrasAprovacao(pagamento, regras, tipoServicoMultisoftware);
            else
                pagamento.Situacao = SituacaoPagamento.SemRegraAprovacao;

            if (pagamento.Situacao == SituacaoPagamento.EmFechamento)
                Pagamento.FinalizarPagamento(pagamento, _unitOfWork);
        }

        public bool UtilizarAlcadaAprovacaoPagamento()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UtilizarAlcadaAprovacaoPagamento ?? false;
        }

        #endregion
    }
}
