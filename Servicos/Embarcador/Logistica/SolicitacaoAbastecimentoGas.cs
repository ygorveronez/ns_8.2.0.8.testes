using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class SolicitacaoAbastecimentoGas : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas,
        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas,
        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas
    >
    {
        #region Construtores

        public SolicitacaoAbastecimentoGas(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private int ObterTempoExcedido(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas)
        {
            Repositorio.Embarcador.Filiais.SuprimentoDeGas repositorioSuprimentoGas = new Repositorio.Embarcador.Filiais.SuprimentoDeGas(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas suprimentoGas = solicitacaoAbastecimentoGas.ClienteBase != null ? repositorioSuprimentoGas.BuscarPorProdutoCliente(solicitacaoAbastecimentoGas.Produto?.Codigo ?? 0, solicitacaoAbastecimentoGas.ClienteBase.CPF_CNPJ) : null;
            
            double minutosLancamento = solicitacaoAbastecimentoGas.DataCriacao.TimeOfDay.TotalMinutes;
            double minutosBloqueio = (suprimentoGas?.HoraBloqueioSolicitacao.HasValue ?? false) ? suprimentoGas.HoraBloqueioSolicitacao.Value.TotalMinutes : 0.0;
            
            return Convert.ToInt32(Math.Floor(minutosLancamento - minutosBloqueio));
        }
        
        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas, List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas repositorio = new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            DateTime dataCriacao = DateTime.Now;

            foreach (Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas aprovacao = new Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas()
                        {
                            OrigemAprovacao = solicitacaoAbastecimentoGas,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = dataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(solicitacaoAbastecimentoGas, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas aprovacao = new Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas()
                    {
                        OrigemAprovacao = solicitacaoAbastecimentoGas,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = dataCriacao,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            solicitacaoAbastecimentoGas.Situacao = existeRegraSemAprovacao ? SituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao : SituacaoAprovacaoSolicitacaoGas.Aprovada;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas> listaRegras = repositorioRegra.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas>();

            int minutosExcedidos = ObterTempoExcedido(solicitacaoAbastecimentoGas);
            
            foreach (Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas regra in listaRegras)
            {
                if (regra.RegraPorTempoExcedido && !ValidarAlcadas<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData, int>(regra.AlcadasSolicitacaoGasData, minutosExcedidos))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas, Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: solicitacaoAbastecimentoGas.Codigo,
                URLPagina: "Logistica/AbastecimentoGas",
                titulo: Localization.Resources.Logistica.SolicitacaoAbastecimentoGas.SolicitacaoGas,
                nota: Localization.Resources.Logistica.SolicitacaoAbastecimentoGas.CriadaSolicitacaoAprovacaoGas,
                icone: IconesNotificacao.confirmado,
                tipoNotificacao: TipoNotificacao.todas,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(solicitacaoAbastecimentoGas);

            List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas> regras = ObterRegrasAutorizacao(solicitacaoAbastecimentoGas);

            if (regras.Count > 0)
                CriarRegrasAprovacao(solicitacaoAbastecimentoGas, regras, tipoServicoMultisoftware);
            else
                solicitacaoAbastecimentoGas.Situacao = SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao;
        }

        #endregion
    }
}
