using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public sealed class EstornoProvisaoAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public EstornoProvisaoAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public EstornoProvisaoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoProvisao, List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorio = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;
                    List<Dominio.Entidades.Usuario> aprovadores;
                    aprovadores = regra.Aprovadores.ToList();

                    foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao()
                        {
                            OrigemAprovacao = estornoProvisao.EstornoProvisao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = DateTime.Now,
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(estornoProvisao.EstornoProvisao, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao()
                    {
                        OrigemAprovacao = estornoProvisao.EstornoProvisao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = DateTime.Now,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
                estornoProvisao.Situacao = SituacaoEstornoProvisaoSolicitacao.AguardandoAprovacao;
            else
                estornoProvisao.Situacao = SituacaoEstornoProvisaoSolicitacao.Aprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoSolicitacao)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = estornoSolicitacao.EstornoProvisao.Carga;

            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>();

            decimal valorEstorno = estornoSolicitacao.EstornoProvisao?.ValorCancelamentoProvisao ?? 0;

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, carga.Filial?.Codigo))
                    continue;

                if (regra.RegraPorValorProvisao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaValorProvisao, decimal>(regra.AlcadaValorProvisao, valorEstorno))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private void ValidarAprovacao(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoProvisaoSolicitacao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork);
            repCancelamentoProvisao.Atualizar(estornoProvisaoSolicitacao.EstornoProvisao);
            return;
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamento, Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: cancelamento.Codigo,
                URLPagina: "Escrituracao/CancelamentoProvisao",
                titulo: "Estorno de Provisão",
                nota: "Criada solicitação para cancelamento de provisão",
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao origem, TipoGeracaoRegraProvisao TipoRegra, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoSolicitacao = repositorioEstornoProvisaoSolicitacao.BuscarPendentePorEstornoProvisao(origem.Codigo);

            if (!(repositorioEstornoProvisaoSolicitacao.ExisteRegraAprovacao()))
                return;

            if (estornoSolicitacao != null)
                throw new ServicoException($"Já existe uma solicitação de cancelamento {estornoSolicitacao.Situacao.ObterDescricao().ToLower()}");

            estornoSolicitacao = new Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao()
            {
                EstornoProvisao = origem,
                Numero = repositorioEstornoProvisaoSolicitacao.BuscarProximoNumeroPorEstornoProvisao(origem.Codigo),
                Situacao = SituacaoEstornoProvisaoSolicitacao.AguardandoAprovacao
            };

            repositorioEstornoProvisaoSolicitacao.Inserir(estornoSolicitacao);

            CriarAprovacao(estornoSolicitacao, tipoServicoMultisoftware);
        }

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoProvisaoSolicitacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> regras = ObterRegrasAutorizacao(estornoProvisaoSolicitacao);

            if (regras.Count > 0)
                CriarRegrasAprovacao(estornoProvisaoSolicitacao, regras, tipoServicoMultisoftware);
            else
            {
                estornoProvisaoSolicitacao.EstornoProvisao.Situacao = SituacaoCancelamentoProvisao.SemRegraAprovacao;
                estornoProvisaoSolicitacao.Situacao = SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao;
            }

            repositorioEstornoProvisaoSolicitacao.Atualizar(estornoProvisaoSolicitacao);

            if (estornoProvisaoSolicitacao.Situacao != SituacaoEstornoProvisaoSolicitacao.Aprovada && estornoProvisaoSolicitacao.Situacao != SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao)
            {
                estornoProvisaoSolicitacao.EstornoProvisao.Situacao = SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao;
                return;
            }

            ValidarAprovacao(estornoProvisaoSolicitacao);
        }

        #endregion
    }
}
