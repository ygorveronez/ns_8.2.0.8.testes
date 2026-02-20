using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoProvisao : ServicoBase
    {
        #region Construtores

        public IntegracaoProvisao(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public IntegracaoProvisao(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarIntegracao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao = repositorioProvisaoIntegracao.BuscarPorProvisaoETipoIntegracao(provisao.Codigo, tipoIntegracao.Codigo);

            if (provisaoIntegracao != null)
            {
                if (tipoIntegracao.Tipo != TipoIntegracao.Unilever)
                    return;

                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                provisaoIntegracao.ProblemaIntegracao = "";
                repositorioProvisaoIntegracao.Atualizar(provisaoIntegracao);
                return;
            }

            provisaoIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao
            {
                Provisao = provisao,
                TipoIntegracao = tipoIntegracao,
                SequenciaIntegracao = 1,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
            };

            repositorioProvisaoIntegracao.Inserir(provisaoIntegracao);
        }

        private void AdicionarIntegracaoEDI(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDIIntegracao = repositorioProvisaoEDIIntegracao.BuscarPorProvisaoELayout(provisao.Codigo, layoutEDI.Codigo);

            if (provisaoEDIIntegracao != null)
                return;

            provisaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao
            {
                Provisao = provisao,
                LayoutEDI = layoutEDI,
                TipoIntegracao = tipoIntegracao,
                SequenciaIntegracao = 1,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
            };

            repositorioProvisaoEDIIntegracao.Inserir(provisaoEDIIntegracao);
        }

        private bool AdicionarIntegracoes(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(new List<TipoIntegracao>() { TipoIntegracao.Carrefour, TipoIntegracao.Unilever, TipoIntegracao.Camil, TipoIntegracao.Buntech });

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoAdicionar = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.Carrefour)
                {
                    Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(_unitOfWork);

                    if (repositorioProvisaoEDIIntegracao.BuscarPorProvisao(provisao.Codigo).Count() > 0)
                        tiposIntegracaoAdicionar.Add(tipoIntegracao);
                }
                else
                    tiposIntegracaoAdicionar.Add(tipoIntegracao);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracaoAdicionar)
                AdicionarIntegracao(provisao, tipoIntegracao);

            return tiposIntegracaoAdicionar.Count > 0;
        }

        private bool AdicionarIntegracoesEDI(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Escrituracao.Provisao servicoProvisao = new Escrituracao.Provisao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIPessoa = servicoProvisao.LayoutEDIProvisaoCliente(provisao);
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIProvisaoPessoa = (from o in layoutsEDIPessoa where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV_INTPFAR select o).ToList();

            if (layoutsEDIProvisaoPessoa.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI layoutEDI in layoutsEDIProvisaoPessoa)
                    AdicionarIntegracaoEDI(provisao, layoutEDI.LayoutEDI, layoutEDI.TipoIntegracao);

                return true;
            }

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIGrupoPessoas = servicoProvisao.LayoutEDIProvisao(provisao);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIProvisaoGrupoPessoas = (from o in layoutsEDIGrupoPessoas where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV_INTPFAR select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutEDI in layoutsEDIProvisaoGrupoPessoas)
                AdicionarIntegracaoEDI(provisao, layoutEDI.LayoutEDI, layoutEDI.TipoIntegracao);

            return (layoutsEDIProvisaoGrupoPessoas.Count > 0);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Escrituracao.Provisao>> VerificarIntegracoesAsync()
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> integracoes = repositorioProvisaoIntegracao.BuscarIntegracoesPendentes(numeroTentativas: 5, minutosACadaTentativa: 5, limiteRegistros: 10);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Carrefour:
                        new Carrefour.IntegracaoCarrefour(_unitOfWork).IntegrarProvisao(integracao);
                        break;
                    case TipoIntegracao.Unilever:
                        new Unilever.IntegracaoUnilever(_unitOfWork).IntegrarProvisao(integracao);
                        break;
                    case TipoIntegracao.Camil:
                        await new Camil.IntegracaoCamil(_unitOfWork).IntegrarProvisaoAsync(integracao);
                        break;
                    case TipoIntegracao.Buntech:
                        await new Buntech.IntegracaoBuntech(_unitOfWork).IntegrarProvisaoAsync(integracao);
                        break;
                }
            }

            return integracoes.Select(integracao => integracao.Provisao).ToList();
        }

        private async Task<List<Dominio.Entidades.Embarcador.Escrituracao.Provisao>> VerificarIntegracoesEDIAsync()
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> integracoesEDI = await repositorioProvisaoEDIIntegracao.BuscarIntegracoesPendentesAsync(numeroTentativas: 5, minutosACadaTentativa: 5, limiteRegistros: 10, _cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao integracao in integracoesEDI)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.FTP:
                        await new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).EnviarEDIAsync(integracao);
                        break;
                    case TipoIntegracao.NaoInformada:
                    case TipoIntegracao.NaoPossuiIntegracao:
                        integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        await repositorioProvisaoEDIIntegracao.AtualizarAsync(integracao);
                        break;
                }
            }

            return integracoesEDI.Select(integracao => integracao.Provisao).ToList();
        }

        private void AtualizarSituacaoProvisoes(List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> provisoes)
        {
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(_unitOfWork);
            Hubs.Provisao servicoNotificacaoProvisao = new Hubs.Provisao();
            List<SituacaoIntegracao> situacoesIntegracaoPendente = new List<SituacaoIntegracao>() { SituacaoIntegracao.AgIntegracao, SituacaoIntegracao.AgRetorno };

            foreach (Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao in provisoes)
            {
                if ((repositorioProvisaoIntegracao.ContarPorProvisaoESituacoes(provisao.Codigo, situacoesIntegracaoPendente) > 0) || (repositorioProvisaoEDIIntegracao.ContarPorProvisaoESituacoes(provisao.Codigo, situacoesIntegracaoPendente) > 0))
                    continue;

                if ((repositorioProvisaoIntegracao.ContarPorProvisaoESituacao(provisao.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0) || (repositorioProvisaoEDIIntegracao.ContarPorProvisaoESituacao(provisao.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0))
                {
                    provisao.Situacao = SituacaoProvisao.FalhaIntegracao;
                    repositorioProvisao.Atualizar(provisao);
                    servicoNotificacaoProvisao.InformarProvisaoAtualizada(provisao.Codigo);
                    continue;
                }

                provisao.Situacao = SituacaoProvisao.Finalizado;
                repositorioProvisao.Atualizar(provisao);
                servicoNotificacaoProvisao.InformarProvisaoAtualizada(provisao.Codigo);
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void Adicionar(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Adicionar(provisao, adicionarIntegracoesEDI: true);
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, bool adicionarIntegracoesEDI)
        {
            bool possuiIntegracaoEDI = adicionarIntegracoesEDI ? AdicionarIntegracoesEDI(provisao) : false;
            bool possuiIntegracao = AdicionarIntegracoes(provisao);

            provisao.Situacao = (possuiIntegracaoEDI || possuiIntegracao) ? SituacaoProvisao.AgIntegracao : SituacaoProvisao.Finalizado;
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> provisoes = new List<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();

            provisoes.AddRange(await VerificarIntegracoesAsync());
            provisoes.AddRange(await VerificarIntegracoesEDIAsync());

            AtualizarSituacaoProvisoes(provisoes.Distinct().ToList());
        }

        #endregion Métodos Públicos
    }
}
