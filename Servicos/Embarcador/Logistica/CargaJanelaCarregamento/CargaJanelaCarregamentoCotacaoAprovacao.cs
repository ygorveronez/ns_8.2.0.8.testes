using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoCotacaoAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao,
        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao
    >
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoCotacaoAprovacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao repositorio = new Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            DateTime dataCriacao = DateTime.Now;
            bool existeRegraSemAprovacao = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao()
                        {
                            OrigemAprovacao = cargaJanelaCarregamento,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = dataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(cargaJanelaCarregamento, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao()
                    {
                        OrigemAprovacao = cargaJanelaCarregamento,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = dataCriacao
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            cargaJanelaCarregamento.SituacaoCotacao = existeRegraSemAprovacao ? SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao : SituacaoCargaJanelaCarregamentoCotacao.Aprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioComponenteFreteTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> listaRegras = repositorioRegra.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao>();
            decimal valorTotalFrete = cargaJanelaCarregamentoTransportador.ValorFreteTransportador + repositorioComponenteFreteTransportador.BuscarValorPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao regra in listaRegras)
            {
                if (regra.RegraPorCentroCarregamento && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaCentroCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>(regra.AlcadasCentroCarregamento, cargaJanelaCarregamento.CentroCarregamento.Codigo))
                    continue;

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, cargaJanelaCarregamento.CentroCarregamento.Filial.Codigo))
                    continue;

                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, cargaJanelaCarregamentoTransportador.Transportador.Codigo))
                    continue;

                if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaValor, decimal>(regra.AlcadasValor, valorTotalFrete))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: cargaJanelaCarregamento.Codigo,
                URLPagina: "Logistica/JanelaCarregamento",
                titulo: "Interesse na Carga",
                nota: $"Interesse do transportador {cargaJanelaCarregamento.TransportadorCotacao.Descricao} na carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} aguardando aprovação",
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaJanelaCarregamentoETransportador(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.TransportadorCotacao.Codigo);

            CriarAprovacao(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, tipoServicoMultisoftware);
        }

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(cargaJanelaCarregamento);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            if (!ObterConfiguracaoJanelaCarregamento().LiberarCargaParaCotacaoAoLiberarParaTransportadores)
            {
                cargaJanelaCarregamento.SituacaoCotacao = SituacaoCargaJanelaCarregamentoCotacao.NaoDefinida;
                cargaJanelaCarregamento.TransportadorCotacao = null;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao> regras = ObterRegrasAutorizacao(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador);

                cargaJanelaCarregamento.TransportadorCotacao = cargaJanelaCarregamentoTransportador.Transportador;

                if (regras.Count > 0)
                    CriarRegrasAprovacao(cargaJanelaCarregamento, regras, tipoServicoMultisoftware);
                else
                    cargaJanelaCarregamento.SituacaoCotacao = SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao;
            }

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        #endregion Métodos Públicos
    }
}
