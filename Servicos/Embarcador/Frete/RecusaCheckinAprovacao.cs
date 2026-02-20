using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public sealed class RecusaCheckinAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.CargaCTe,
        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin,
        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin
    >
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion Atributos

        #region Construtores

        public RecusaCheckinAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public RecusaCheckinAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork)
        {
            _auditado = auditado;
        }

        #endregion Construtores

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, List<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin repositorio = new Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            DateTime dataCriacao = DateTime.Now;
            bool existeRegraSemAprovacao = false;

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin()
                        {
                            OrigemAprovacao = cargaCTe,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = dataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            TipoAprovadorRegra = regra.TipoAprovadorRegra
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(cargaCTe, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin()
                    {
                        OrigemAprovacao = cargaCTe,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = dataCriacao,
                        TipoAprovadorRegra = regra.TipoAprovadorRegra
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            cargaCTe.SituacaoCheckin = existeRegraSemAprovacao ? SituacaoCheckin.AguardandoAprovacao : SituacaoCheckin.RecusaAprovada;
        }

        private List<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin> listaRegras = repositorioRegra.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin>();

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, cargaCTe.Carga.Filial?.Codigo ?? 0))
                    continue;

                if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, cargaCTe.Carga.Empresa?.Codigo ?? 0))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: cargaCTe.Codigo,
                URLPagina: "Cargas/Carga",
                titulo: Localization.Resources.Fretes.RecusaCheckinAprovacao.TituloRecusaCheckin,
                nota: string.Format(Localization.Resources.Fretes.RecusaCheckinAprovacao.RecusaCheckinCTeCarga, cargaCTe.CTe.Numero, cargaCTe.Carga.CodigoCargaEmbarcador),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RemoverAprovacao(cargaCTe);

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin> regras = ObterRegrasAutorizacao(cargaCTe);

            if (regras.Count > 0)
                CriarRegrasAprovacao(cargaCTe, regras, tipoServicoMultisoftware);
            else
                cargaCTe.SituacaoCheckin = SituacaoCheckin.SemRegraAprovacao;

            repositorioCargaCTe.Atualizar(cargaCTe);

            if (cargaCTe.SituacaoCheckin == SituacaoCheckin.RecusaAprovada)
            {
                Auditoria.Auditoria.Auditar(_auditado, cargaCTe, "Recusa do checkin aprovada", _unitOfWork);
                new RecusaCheckin(_unitOfWork).RecusarCheckin(cargaCTe);

                Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                servicoCarga.CriarRegistroIntegracaoCargaFrete(cargaCTe.Carga, _unitOfWork, TipoIntegracao.RejeicaoCte, Servicos.Embarcador.Pedido.Stage.BuscarStagePorCargaCte(cargaCTe.Codigo, _unitOfWork), new List<string>());
                servicoCarga.CriarRegistroIntegracaoConsolidado(cargaCTe.Carga, _unitOfWork, _auditado, tipoServicoMultisoftware, "");
                servicoCarga.AvancarEtapaSubCargasConsolidado(cargaCTe.Carga, cargaCTe, _unitOfWork, _auditado, tipoServicoMultisoftware, "");
            }
        }

        #endregion Métodos Públicos
    }
}
