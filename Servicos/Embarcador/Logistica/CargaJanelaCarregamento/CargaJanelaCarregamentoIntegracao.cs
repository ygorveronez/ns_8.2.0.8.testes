using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoIntegracao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public CargaJanelaCarregamentoIntegracao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarJanelaCarregamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, bool reenviarIntegracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKlios repIntegracaoKlios = new Repositorio.Embarcador.Configuracoes.IntegracaoKlios(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            if (!repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Klios) || (!repIntegracaoKlios.Buscar()?.PossuiIntegracao ?? false))
                return;

            if (!(janelaCarregamento.Carga?.Filial?.GerarIntegracaoKlios ?? false) || !(janelaCarregamento.Carga.TipoOperacao?.ConfiguracaoIntegracao?.GerarIntegracaoKlios ?? false))
                return;

            if (!(janelaCarregamento.Carga.Motoristas.Count > 0) && janelaCarregamento.Carga.Veiculo == null && !(janelaCarregamento.Carga.VeiculosVinculados.Count > 0))
                return;

            IncluirJanelaCarregamentoIntegracao(janelaCarregamento, null, 0, 0, reenviarIntegracao);

            if((janelaCarregamento.RecomendacaoGR != RecomendacaoGR.AguardandoIntegracao && reenviarIntegracao) || janelaCarregamento.RecomendacaoGR == null)
                janelaCarregamento.RecomendacaoGR = RecomendacaoGR.AguardandoIntegracao;

            repJanelaCarregamento.Atualizar(janelaCarregamento);
        }

        public void IncluirJanelaCarregamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem janelaCarregamentoViagem, TipoRetornoRecebimento tipoRetornoRecebimento, TipoEventoIntegracaoJanelaCarregamento tipoEvento)
        {
            IncluirJanelaCarregamentoIntegracao(janelaCarregamento, janelaCarregamentoViagem, tipoRetornoRecebimento, tipoEvento, reenviarIntegracao: false);
        }

        public bool ReenviarIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (janelaCarregamentoIntegracao == null)
            {
                mensagemErro = Localization.Resources.Logistica.JanelaCarregamentoIntegracao.IntegracaoNaoEncontrada;
                return false;
            }

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem servicoJanelaCarregamentoLeilao = new CargaJanelaCarregamentoViagem(_unitOfWork, _tipoServicoMultisoftware);

            if (janelaCarregamentoIntegracao.TipoEvento == 0 && janelaCarregamentoIntegracao.TipoRetornoRecebimento == 0)
                return ReenviarJanelaCarregamentoIntegracao(janelaCarregamentoIntegracao, out mensagemErro);

            switch (janelaCarregamentoIntegracao.TipoEvento)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEventoIntegracaoJanelaCarregamento.ResultadoLeilao:
                    return servicoJanelaCarregamentoLeilao.ReenviarIntegracaoRetorno(janelaCarregamentoIntegracao, out mensagemErro);

                default:
                    mensagemErro = Localization.Resources.Logistica.JanelaCarregamentoIntegracao.NaoFoiPossivelReenviarIntegracao;
                    break;
            }

            return false;
        }

        public void GravarArquivoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, string jsonRequest, string jsonResponse, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo repositorioJanelaCarregamentoIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo janelaCarregamentoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo();

            if (!string.IsNullOrWhiteSpace(jsonRequest))
                janelaCarregamentoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);

            if (!string.IsNullOrWhiteSpace(jsonResponse))
                janelaCarregamentoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

            janelaCarregamentoIntegracaoArquivo.Data = DateTime.Now;
            janelaCarregamentoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            janelaCarregamentoIntegracaoArquivo.Mensagem = mensagem;

            repositorioJanelaCarregamentoIntegracaoArquivo.Inserir(janelaCarregamentoIntegracaoArquivo);

            if (janelaCarregamentoIntegracao.ArquivosTransacao == null)
                janelaCarregamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracaoArquivo>();

            janelaCarregamentoIntegracao.ArquivosTransacao.Add(janelaCarregamentoIntegracaoArquivo);

            if (janelaCarregamentoIntegracao.Codigo > 0)
                repositorioJanelaCarregamentoIntegracao.Atualizar(janelaCarregamentoIntegracao);
            else
                repositorioJanelaCarregamentoIntegracao.Inserir(janelaCarregamentoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private void IncluirJanelaCarregamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem janelaCarregamentoViagem, TipoRetornoRecebimento tipoRetornoRecebimento, TipoEventoIntegracaoJanelaCarregamento tipoEvento, bool reenviarIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);

            // Apenas um dos dois deve ser diferente de nulo
            if (janelaCarregamento != null && janelaCarregamentoViagem != null)
                janelaCarregamento = null;

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao = null;

            if (janelaCarregamento != null)
                janelaCarregamentoIntegracao = repositorioJanelaCarregamentoIntegracao.BuscarPorJanelaCarregamento(janelaCarregamento.Codigo, tipoRetornoRecebimento, tipoEvento);
            else if (janelaCarregamentoViagem != null)
                janelaCarregamentoIntegracao = repositorioJanelaCarregamentoIntegracao.BuscarPorJanelaCarregamentoViagem(janelaCarregamentoViagem.Codigo, tipoRetornoRecebimento, tipoEvento);

            if (janelaCarregamentoIntegracao != null && (janelaCarregamentoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao || reenviarIntegracao))
                janelaCarregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            else if (janelaCarregamentoIntegracao == null)
            {
                janelaCarregamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao()
                {
                    CargaJanelaCarregamento = janelaCarregamento,
                    CargaJanelaCarregamentoViagem = janelaCarregamentoViagem,
                    TipoEvento = tipoEvento,
                    TipoRetornoRecebimento = tipoRetornoRecebimento,
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    DataCriacao = DateTime.Now,
                };
            }

            if (janelaCarregamentoIntegracao.Codigo > 0)
                repositorioJanelaCarregamentoIntegracao.Atualizar(janelaCarregamentoIntegracao);
            else
                repositorioJanelaCarregamentoIntegracao.Inserir(janelaCarregamentoIntegracao);
        }


        private bool ReenviarJanelaCarregamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, out string mensagemErro)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioCargaJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);

            mensagemErro = string.Empty;

            janelaCarregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

            repositorioCargaJanelaCarregamentoIntegracao.Atualizar(janelaCarregamentoIntegracao);

            return true;
        }

        #endregion

    }
}
