using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCargaCancelamentoDadosCancelamento : ServicoBase
    {
        #region Construtores        

        public IntegracaoCargaCancelamentoDadosCancelamento() : base() { }

        public IntegracaoCargaCancelamentoDadosCancelamento(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public bool AdicionarIntegracoesDadosCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarAtivos();

            int integracoes_incluidas = 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                        if (repCargaCargaIntegracao.ExisteIntegradoOuAguardandoRetornoPorCargaETipo(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST) ||
                                repCargaCargaIntegracao.ExisteIntegradoOuAguardandoRetornoPorCargaETipo(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP))
                            AdicionarIntegracaoCargaCancelamentoDadosCancelamento(cargaCancelamento, tipoIntegracao, unidadeDeTrabalho);
                        integracoes_incluidas++;

                        break;
                }
            }
            return integracoes_incluidas > 0 ? true : false;
        }

        public void IniciarIntegracoesDadosCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao> cargaCancelamentosIntegracao = repositorioCargaCancelamentoIntegracao.BuscarPorCargaCancelamento(cargaCancelamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao in cargaCancelamentosIntegracao)
            {
                switch (cargaCancelamentoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        CriarCargaCancelamentoDadosCancelamentoCarga(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentesCarga()
        {
            Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento repIntegracaoDadosCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento> integracoesPendentes = repIntegracaoDadosCancelamento.BuscarIntegracoesPendentes(3, 5);

            foreach (Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento integracao in integracoesPendentes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(_unitOfWork).IntegrarDadosCancelamentoCarga(integracao);
                        break;
                }

                VerificarSituacaoIntegracoesDados(integracao.CargaCancelamento, _unitOfWork);
            }
        }

        public void VerificarIntegracoesPendentesCTe()
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados repIntegracaoDadosCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados> integracoesPendentes = repIntegracaoDadosCancelamento.BuscarIntegracoesPendentes(3, 5);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados integracao in integracoesPendentes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(_unitOfWork).IntegrarDadosCancelamentoCTe(integracao);
                        break;
                }

                VerificarSituacaoIntegracoesDados(integracao.CargaCancelamento, _unitOfWork);
            }
        }
        public void VerificarSituacaoIntegracoesDados(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento repIntegracaoDadosCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados repCargaCancelamentoCargaCTeIntegracaoDados = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados(unitOfWork);

            bool possuiIntegracoesPendentesDadosCarga = repIntegracaoDadosCancelamento.PossuiIntegracoesPendentesPorCargaCancelamento(cargaCancelamento.Codigo);
            bool possuiIntegracoesPendentesDadosCTe = repCargaCancelamentoCargaCTeIntegracaoDados.PossuiIntegracoesPendentesPorCargaCancelamento(cargaCancelamento.Codigo);

            if (!possuiIntegracoesPendentesDadosCarga && !possuiIntegracoesPendentesDadosCTe)
            {
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT;
                repCargaCancelamento.Atualizar(cargaCancelamento);
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarIntegracaoCargaCancelamentoDadosCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = tipoIntegracao
            };
            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
        }

        private void CriarCargaCancelamentoDadosCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento repCargaCancelamentoDadosIntegracao = new Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados repCargaCancelamentoCargaCTeIntegracaoDados = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados(unitOfWork);
            Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento repIntegracaoDadosCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repositorioIntegracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP = repositorioIntegracaoSAP.Buscar();

            if (!string.IsNullOrWhiteSpace(integracaoSAP.URLSolicitacaoCancelamento))
            {
                if (repCargaCargaIntegracao.ExisteIntegradoOuAguardandoRetornoPorCargaETipo(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST))
                {
                    if (!repCargaCancelamentoDadosIntegracao.PossuiIntegracoesPorCargaCancelamento(cargaCancelamento.Codigo, tipoIntegracao.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento cargaCancelamentoDadosIntegracao = new Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento();
                        cargaCancelamentoDadosIntegracao.CargaCancelamento = cargaCancelamento;
                        cargaCancelamentoDadosIntegracao.DataIntegracao = DateTime.Now;
                        cargaCancelamentoDadosIntegracao.NumeroTentativas = 0;
                        cargaCancelamentoDadosIntegracao.ProblemaIntegracao = "";
                        cargaCancelamentoDadosIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        cargaCancelamentoDadosIntegracao.TipoIntegracao = tipoIntegracao;

                        repIntegracaoDadosCancelamento.Inserir(cargaCancelamentoDadosIntegracao);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(integracaoSAP.URLSolicitacaoCancelamentoCTe))
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados> JaInseridosCargaCancelamentoCargaCTeIntegracaoDados = repCargaCancelamentoCargaCTeIntegracaoDados.BuscarIntegracoesPorCargaCancelamentoCte(cargaCancelamento.Codigo, tipoIntegracao.Codigo);

                string[] status = new string[] { "A", "C", "Z" }; ;
                cargaCTes = repCargaCTe.BuscarPorCargaEStatusSemComplementares(cargaCancelamento.Carga.Codigo, status);

                if (repCargaCargaIntegracao.ExisteIntegradoOuAguardandoRetornoPorCargaETipo(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP))
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                    {
                        if (!JaInseridosCargaCancelamentoCargaCTeIntegracaoDados.Where(x => x.CargaCTe.Codigo == cargaCTe.Codigo).Any())
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados cargaCancelamentoCargaCTeIntegracaoDados = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracaoDados()
                            {
                                CargaCancelamento = cargaCancelamento,
                                CargaCTe = cargaCTe,
                                DataIntegracao = DateTime.Now,
                                NumeroTentativas = 0,
                                ProblemaIntegracao = string.Empty,
                                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                                TipoIntegracao = tipoIntegracao
                            };
                            repCargaCancelamentoCargaCTeIntegracaoDados.Inserir(cargaCancelamentoCargaCTeIntegracaoDados);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
