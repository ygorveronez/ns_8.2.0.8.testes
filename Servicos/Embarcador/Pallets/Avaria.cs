using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pallets
{
    public sealed class Avaria : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria,
        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria
    >
    {
        #region Construtores

        public Avaria(Repositorio.UnitOfWork unitOfWork) : base (unitOfWork) { }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria, List<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var existeRegraSemAprovacao = false;
            var repositorio = new Repositorio.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (var regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria aprovacao = new Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria()
                        {
                            OrigemAprovacao = avaria,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = avaria.Data,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(avaria, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria()
                    {
                        OrigemAprovacao = avaria,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = avaria.Data,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            avaria.Situacao = existeRegraSemAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.AguardandoAprovacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.Finalizada;
        }

        private List<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria)
        {
            var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria>(_unitOfWork);
            var listaRegras = repositorioRegra.BuscarPorAtiva();
            var listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria>();

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, avaria.Filial?.Codigo ?? 0))
                    continue;

                if (regra.RegraPorMotivoAvaria && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaMotivoAvaria, Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>(regra.AlcadasMotivoAvaria, avaria.MotivoAvaria.Codigo))
                    continue;

                if (regra.RegraPorSetor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaSetor, Dominio.Entidades.Setor>(regra.AlcadasSetor, avaria.Setor.Codigo))
                    continue;

                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, avaria.Transportador?.Codigo ?? 0))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria, Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: avaria.Codigo,
                URLPagina: "Pallets/Avaria",
                titulo: Localization.Resources.Avarias.AutorizacaoAvaria.AvariaPallets,
                nota: string.Format(Localization.Resources.Avarias.AutorizacaoAvaria.SolicitanteCriouAvariaPallets, avaria.Solicitante.Descricao, avaria.Codigo),
                icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var regras = ObterRegrasAutorizacao(avaria);

            if (regras.Count > 0)
            {
                CriarRegrasAprovacao(avaria, regras, tipoServicoMultisoftware);
                InserirMovimentacaoEstoque(avaria, tipoServicoMultisoftware);
            }
            else
                avaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.SemRegraAprovacao;
        }

        public void InserirMovimentacaoEstoque(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (avaria.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet.Finalizada)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipo = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorAvaria : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialAvaria;
                var servicoEstoquePallet = new EstoquePallet(_unitOfWork);

                var dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CodigoAvaria = avaria.Codigo,
                    CodigoFilial = avaria.Filial?.Codigo ?? 0,
                    CodigoTransportador = avaria.Transportador?.Codigo ?? 0,
                    CodigoSetor = avaria.Setor?.Codigo ?? 0,
                    Quantidade = avaria.QuantidadesAvariadas.Sum(o => o.Quantidade),
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoOperacaoMovimentacao = tipo,
                };

                servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
            }
        }

        #endregion
    }
}
