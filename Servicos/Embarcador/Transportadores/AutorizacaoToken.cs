using Dominio.Entidades.Embarcador.Transportadores.Alcada;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Transportadores
{
    public sealed class AutorizacaoToken : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken,
        Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken,
        Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public AutorizacaoToken(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public AutorizacaoToken(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken, List<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken repositorioAutorizacaoToken = new Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken aprovacao = new  AprovacaoAlcadaAutorizacaoToken()
                        {
                            OrigemAprovacao = solicitacaoToken,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = DateTime.Now,
                            Data = DateTime.Now,
                        };

                        repositorioAutorizacaoToken.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(solicitacaoToken, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken aprovacao = new Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken
                    {
                        OrigemAprovacao = solicitacaoToken,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = DateTime.Now,
                    };

                    repositorioAutorizacaoToken.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
                solicitacaoToken.Situacao = EtapaAutorizacaoToken.AgAprovacao;
            else
                solicitacaoToken.Situacao = EtapaAutorizacaoToken.SolicitacaoAprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken>(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken>();

            foreach (var regra in listaRegras)
            {
                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos



        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Transportadores.SolicitacaoToken repositorioSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken> regras = ObterRegrasAutorizacao(solicitacaoToken);

            if (regras.Count > 0)
                CriarRegrasAprovacao(solicitacaoToken, regras, tipoServicoMultisoftware);
            else
            {
                solicitacaoToken.Situacao = EtapaAutorizacaoToken.SemRegraAprovacao;
            }

            repositorioSolicitacaoToken.Atualizar(solicitacaoToken);

            if (solicitacaoToken.Situacao != EtapaAutorizacaoToken.SolicitacaoAprovada && solicitacaoToken.Situacao != EtapaAutorizacaoToken.SemRegraAprovacao)
            {
                solicitacaoToken.Situacao = EtapaAutorizacaoToken.AgAprovacao;
                return;
            }

            repositorioSolicitacaoToken.Atualizar(solicitacaoToken);
        }

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken origemAprovacao, AprovacaoAlcadaAutorizacaoToken aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: origemAprovacao.Codigo,
                URLPagina: "Transportadores/AutorizcacaoToken",
                titulo: "Autorização Token",
                nota: MontarNotaNotificacao(origemAprovacao),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        private string MontarNotaNotificacao(Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken origemAprovacao)
        {
            StringBuilder nota = new StringBuilder("Criada solicitação para Autorização do Token para os transportadores: <br />");

            ICollection<Dominio.Entidades.Empresa> transportadores = origemAprovacao.Transportadores;

            if(transportadores != null) {
                foreach(Dominio.Entidades.Empresa transportador in transportadores)
                {
                    nota.Append(transportador.NomeCNPJ + "<br />");
                }
            }

            return nota.ToString();
        }

        #endregion
    }
}
