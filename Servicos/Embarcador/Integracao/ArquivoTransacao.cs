using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao
{
    public sealed class ArquivoTransacao<TIntegracaoArquivo> where TIntegracaoArquivo : Dominio.Entidades.Embarcador.Integracao.IntegracaoArquivo, new()
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ArquivoTransacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private TIntegracaoArquivo Adicionar(string conteudoArquivoRequisicao, string conteudoArquivoRetorno, string extensaoArquivo, string mensagem, TipoArquivoIntegracaoCTeCarga tipo, DateTime data)
        {
            if (string.IsNullOrWhiteSpace(conteudoArquivoRequisicao) && string.IsNullOrWhiteSpace(conteudoArquivoRetorno))
                return null;

            Repositorio.RepositorioBase<TIntegracaoArquivo> repositorioIntegracaoArquivo = new Repositorio.RepositorioBase<TIntegracaoArquivo>(_unitOfWork);
            TIntegracaoArquivo integracaoArquivo = new TIntegracaoArquivo()
            {
                Data = data,
                Mensagem = mensagem,
                Tipo = tipo
            };

            if (!string.IsNullOrWhiteSpace(conteudoArquivoRequisicao))
                integracaoArquivo.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(conteudoArquivoRequisicao, extensaoArquivo, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(conteudoArquivoRetorno))
                integracaoArquivo.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(conteudoArquivoRetorno, extensaoArquivo, _unitOfWork);

            repositorioIntegracaoArquivo.Inserir(integracaoArquivo);

            return integracaoArquivo;
        }

        private TIntegracaoArquivo Adicionar(byte[] conteudoArquivoRequisicao, byte[] conteudoArquivoRetorno, string extensaoArquivo, string mensagem, TipoArquivoIntegracaoCTeCarga tipo, DateTime data)
        {
            if (((conteudoArquivoRequisicao == null) || (conteudoArquivoRequisicao.Length == 0)) && ((conteudoArquivoRetorno == null) || (conteudoArquivoRetorno.Length == 0)))
                return null;

            Repositorio.RepositorioBase<TIntegracaoArquivo> repositorioIntegracaoArquivo = new Repositorio.RepositorioBase<TIntegracaoArquivo>(_unitOfWork);
            TIntegracaoArquivo integracaoArquivo = new TIntegracaoArquivo()
            {
                Data = data,
                Mensagem = mensagem,
                Tipo = tipo
            };

            if ((conteudoArquivoRequisicao != null) && (conteudoArquivoRequisicao.Length > 0))
                integracaoArquivo.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(conteudoArquivoRequisicao, extensaoArquivo, _unitOfWork);

            if ((conteudoArquivoRetorno != null) && (conteudoArquivoRetorno.Length > 0))
                integracaoArquivo.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(conteudoArquivoRetorno, extensaoArquivo, _unitOfWork);

            repositorioIntegracaoArquivo.Inserir(integracaoArquivo);

            return integracaoArquivo;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, string conteudoArquivoRequisicao, string conteudoArquivoRetorno, string extensaoArquivo)
        {
            Adicionar(integracao, conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, integracao.ProblemaIntegracao, tipo: TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento);
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, string conteudoArquivoRequisicao, string conteudoArquivoRetorno, string extensaoArquivo, string mensagem)
        {
            Adicionar(integracao, conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, mensagem, tipo: TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento);
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, string conteudoArquivoRequisicao, string conteudoArquivoRetorno, string extensaoArquivo, TipoArquivoIntegracaoCTeCarga tipo)
        {
            Adicionar(integracao, conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, integracao.ProblemaIntegracao, tipo);
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, string conteudoArquivoRequisicao, string conteudoArquivoRetorno, string extensaoArquivo, string mensagem, TipoArquivoIntegracaoCTeCarga tipo)
        {
            TIntegracaoArquivo arquivoIntegracao = Adicionar(conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, mensagem, tipo, integracao.DataIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<TIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, byte[] conteudoArquivoRequisicao, byte[] conteudoArquivoRetorno, string extensaoArquivo)
        {
            Adicionar(integracao, conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, integracao.ProblemaIntegracao, tipo: TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento);
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, byte[] conteudoArquivoRequisicao, byte[] conteudoArquivoRetorno, string extensaoArquivo, string mensagem)
        {
            Adicionar(integracao, conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, mensagem, tipo: TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento);
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, byte[] conteudoArquivoRequisicao, byte[] conteudoArquivoRetorno, string extensaoArquivo, TipoArquivoIntegracaoCTeCarga tipo)
        {
            Adicionar(integracao, conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, integracao.ProblemaIntegracao, tipo);
        }

        public TIntegracaoArquivo Adicionar(string conteudoArquivoRequisicao, string conteudoArquivoRetorno, string extensaoArquivo, Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao)
        {
            TIntegracaoArquivo arquivoIntegracao = Adicionar(conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, integracao.ProblemaIntegracao, tipo: TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento, integracao.DataIntegracao);

            if (arquivoIntegracao == null)
                return null;

            integracao.ArquivosTransacao.Add(arquivoIntegracao);

            return arquivoIntegracao;
        }

        public void Adicionar(Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo> integracao, byte[] conteudoArquivoRequisicao, byte[] conteudoArquivoRetorno, string extensaoArquivo, string mensagem, TipoArquivoIntegracaoCTeCarga tipo)
        {
            TIntegracaoArquivo arquivoIntegracao = Adicionar(conteudoArquivoRequisicao, conteudoArquivoRetorno, extensaoArquivo, mensagem, tipo, integracao.DataIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<TIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion Métodos Públicos
    }
}
