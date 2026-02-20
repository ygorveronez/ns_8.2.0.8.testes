using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public sealed class ContratoPrestacaoServico : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico,
        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico,
        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico
    >
    {
        #region Construtores

        public ContratoPrestacaoServico(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico()
                        {
                            OrigemAprovacao = contratoPrestacaoServico,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = System.DateTime.Now,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(contratoPrestacaoServico, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico()
                    {
                        OrigemAprovacao = contratoPrestacaoServico,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = System.DateTime.Now,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            contratoPrestacaoServico.Situacao = existeRegraSemAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.AguardandoAprovacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.Aprovado;
        }

        private List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico> listaRegras = repositorioRegra.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico>();

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico regra in listaRegras)
            {
                if (regra.RegraPorFilial)
                {
                    if (contratoPrestacaoServico.Filiais?.Count > 0)
                    {
                        List<int> listaCodigoFilial = (from filial in contratoPrestacaoServico.Filiais select filial.Codigo).ToList();

                        if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, listaCodigoFilial))
                            continue;
                    }
                    else
                        continue;
                }

                if (regra.RegraPorTransportador)
                {
                    if (contratoPrestacaoServico.Transportadores?.Count > 0)
                    {
                        List<int> listaCodigoTransportador = (from transportador in contratoPrestacaoServico.Transportadores select transportador.Codigo).ToList();

                        if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, listaCodigoTransportador))
                            continue;
                    }
                    else
                        continue;
                }

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: contratoPrestacaoServico.Codigo,
                URLPagina: "Fretes/ContratoPrestacaoServico",
                titulo: Localization.Resources.Fretes.ContratoPrestacaoServico.TituloContratoPrestacaoServico,
                nota: string.Format(Localization.Resources.Fretes.ContratoPrestacaoServico.ContratoPrestacaoServicoAguardandoAprovacao,contratoPrestacaoServico.Descricao),
                icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(contratoPrestacaoServico);

            List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico> regras = ObterRegrasAutorizacao(contratoPrestacaoServico);

            if (regras.Count > 0)
                CriarRegrasAprovacao(contratoPrestacaoServico, regras, tipoServicoMultisoftware);
            else
                contratoPrestacaoServico.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.SemRegraAprovacao;
        }

        #endregion
    }
}
