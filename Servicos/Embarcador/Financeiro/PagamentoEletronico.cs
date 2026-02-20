using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class PagamentoEletronico : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico,
        Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico,
        Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico
    >
    {
        public static object ObterDetalhesAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico repositorioAprovacao = new Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(pagamentoEletronico.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(pagamentoEletronico.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(pagamentoEletronico.Codigo);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                DescricaoSituacao = pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.HasValue ? pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value.ObterDescricao() : "",
                Situacao = pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico,
                pagamentoEletronico.Codigo
            };
        }
        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool bloquearSemRegraAprovacao)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico> regras = ObterRegrasAutorizacao(pagamentoEletronico);

            if (regras.Count > 0)
                CriarRegrasAprovacao(pagamentoEletronico, regras, tipoServicoMultisoftware);
            else if (bloquearSemRegraAprovacao)
                pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico = SituacaoAutorizacaoPagamentoEletronico.SemRegraAprovacao;
            else
                pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico = SituacaoAutorizacaoPagamentoEletronico.Finalizada;
        }

        #region Construtores

        public PagamentoEletronico(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: pagamentoEletronico.Codigo,
                URLPagina: "Financeiros/AutorizacaoPagamentoEletronico",
                titulo: Localization.Resources.Financeiro.PagamentoEletronico.TituloPagamentoEletronico,
                nota: string.Format(Localization.Resources.Financeiro.PagamentoEletronico.CriadaSolicitacaoAprovacaoPagamentoEletronico, pagamentoEletronico.Numero),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico, List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico repositorio = new Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        var aprovacao = new Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico()
                        {
                            OrigemAprovacao = pagamentoEletronico,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = pagamentoEletronico.DataGeracao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(pagamentoEletronico, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico()
                    {
                        OrigemAprovacao = pagamentoEletronico,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = pagamentoEletronico.DataGeracao
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico = existeRegraSemAprovacao ? SituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao : SituacaoAutorizacaoPagamentoEletronico.Finalizada;
        }

        private List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico>(_unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico>();
            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulos = repPagamentoEletronicoTitulo.BuscarPorPagamento(pagamentoEletronico.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico regra in listaRegras)
            {
                if (regra.RegraPorFornecedor && titulos != null && titulos.Count > 0)
                {
                    foreach (var titulo in titulos)
                    {
                        if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaFornecedor, Dominio.Entidades.Cliente>(regra.AlcadasFornecedor, titulo.Titulo?.Fornecedor?.CPF_CNPJ))
                            continue;
                    }
                }

                if (regra.RegraPorBoletoConfiguracao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>(regra.AlcadasBoletoConfiguracao, pagamentoEletronico.BoletoConfiguracao?.Codigo))
                    continue;

                if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaValor, decimal>(regra.AlcadasValor, pagamentoEletronico.ValorTotal))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion
    }
}
