using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Documentos
{
    public sealed class ControleDocumento
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ControleDocumento(Repositorio.UnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        #endregion Construtores

        #region Métodos Privados

        private bool ValidaRegraTextoObservacaoCTe(string obs)
        {
            Regex regex = new Regex(@"^PO\d{8}.*$");
            return obs.Length >= 10 && regex.IsMatch(obs);
        }

        #endregion

        #region Métodos Públicos

        public SituacaoControleDocumento ObterSituacaoDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa repGrupoTransportadorEmpresa = new Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa> listaGrupoTransportadorEmpresa = repGrupoTransportadorEmpresa.BuscarPorTransportador(cte.Empresa?.Codigo ?? 0);

            if (cte == null)
                return SituacaoControleDocumento.AguardandoValidacao;

            if (cte.Tomador?.GrupoPessoas == null)
                return SituacaoControleDocumento.ParqueadoAutomaticamente;

            if (((cte.Tomador.GrupoPessoas?.ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo ?? false) && listaGrupoTransportadorEmpresa.Any(o => o.GrupoTransportador?.ParquearDoumentosAutomaticamente ?? false)) && ValidaRegraTextoObservacaoCTe(cte?.ObservacoesGerais ?? string.Empty))
                return SituacaoControleDocumento.ParqueadoAutomaticamente;

            if ((cte.ModeloDocumentoFiscal?.Numero ?? string.Empty) == "67")
                return SituacaoControleDocumento.ParqueadoAutomaticamente;

            if (cte.Documentos?.Any(o => o.Descricao == "DTA") ?? false)
                return SituacaoControleDocumento.ParqueadoAutomaticamente;

            return SituacaoControleDocumento.AguardandoValidacao;
        }

        public void GeracaoThreadControleDocumentoPorCte()
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTE.BuscarSemControleDocumentoComDataCorte(new DateTime(2023, 10, 1), 200);

            foreach (var cte in ctes)
                GeracaoControleDocumento(cte);
        }

        public void GeracaoControleDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(_unitOfWork).BuscarConfiguracaoPadrao();
            if (configuracao?.CriarControleDeEmissaoDeDocumentos ?? false)
                GerarRegistro(cte);
        }

        public void GerarRegistro(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            if (cte == null)
                return;

            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumentoExistente = repControleDocumento.BuscarPorCodigoCtE(cte.Codigo);

            if (controleDocumentoExistente != null)
                return;

            SituacaoControleDocumento situacaoDocumento = ObterSituacaoDocumento(cte);

            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = new Dominio.Entidades.Embarcador.Documentos.ControleDocumento()
            {
                CTe = cte,
                CargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo),
                SituacaoControleDocumento = situacaoDocumento,
                SituacaoVerificacao = SituacaoVerificacaoControleDocumento.AgVerificacao
            };

            repControleDocumento.Inserir(controleDocumento);
        }

        public void ReprocessarRegistroControleDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);

            if (cte == null)
                return;

            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumentoExistente = repControleDocumento.BuscarPorCodigoCtE(cte.Codigo);

            if (controleDocumentoExistente == null)
                return;

            SituacaoControleDocumento situacaoDocumento = ObterSituacaoDocumento(cte);
            controleDocumentoExistente.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.AgVerificacao;

            repControleDocumento.Atualizar(controleDocumentoExistente);
        }

        public void GerarControleDocumentoParaAprovacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivoDesacordo)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            if (cte == null)
                return;

            SituacaoControleDocumento situacaoDocumento = SituacaoControleDocumento.AguardandoAprovacao;

            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = new Dominio.Entidades.Embarcador.Documentos.ControleDocumento()
            {
                CTe = cte,
                CargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo),
                SituacaoControleDocumento = situacaoDocumento,
                MotivoDesacordo = motivoDesacordo,
                DataEnvioAprovacao = DateTime.Now,
                SituacaoVerificacao = SituacaoVerificacaoControleDocumento.AgVerificacao
            };

            repControleDocumento.Inserir(controleDocumento);
        }

        public void AtualizarControleDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);

            if (cte == null)
                return;


            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = repControleDocumento.BuscarPorCodigoCtE(cte.Codigo);

            if (controleDocumento == null || controleDocumento.SituacaoControleDocumento == SituacaoControleDocumento.AguardandoAprovacao)
                throw new ServicoException("O documento já está pendente de aprovação");

            controleDocumento.SituacaoControleDocumento = SituacaoControleDocumento.AguardandoAprovacao;
            controleDocumento.DataEnvioAprovacao = DateTime.Now;

            repControleDocumento.Atualizar(controleDocumento);

            return;
        }

        public Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo ControleEmissaoDesacordo(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivoDesacordo)
        {
            Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo retorno = new Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo() { status = true, mensagem = "Documento enviado para aprovação" };
            try
            {
                Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);

                if (cte == null)
                {
                    retorno.status = false;
                    retorno.mensagem = "Documento não encontrado no módulo de controle";
                    return retorno;
                }

                GeracaoControleDocumento(cte);
                AtualizarControleDocumento(cte);
            }
            catch (Exception ex)
            {
                retorno.status = false;
                retorno.mensagem = ex.Message;
                return retorno;
            }
            return retorno;
        }

        public void ProcessarDocumentosPendentesVerificacao()
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> documentosPendentesVerificacao = repControleDocumento.BuscarControleDocumentosPendentesVerificacoes(200);

            foreach (var controleDocumento in documentosPendentesVerificacao)
                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork).ValidarIrregularidadeControleDocumento(controleDocumento);
        }

        #endregion
    }
}
