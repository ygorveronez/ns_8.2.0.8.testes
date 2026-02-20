using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Cargas
{
    public class CargaOfertaIntegracao : ServicoBase
    {
        #region Atributos


        #endregion Atributos

        #region Construtores

        public CargaOfertaIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        public async Task CriarRegistroIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga tipoIntegracaoOfertaCarga)
        {
            Repositorio.Embarcador.Cargas.CargaOferta repCargaOferta = new(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaOfertaIntegracao repCargaOfertaIntegracao = new(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao integracao = new Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao()
            {
                TipoIntegracao = tipoIntegracao,
                Tipo = tipoIntegracaoOfertaCarga,
                CargaOferta = cargaOferta,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
            };

            cargaOferta.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaOferta.Situacao = SituacaoCargaOfertaHelper.ObterSituacaoCargaOfertaPorTipoIntegracaoCargaOferta(tipoIntegracaoOfertaCarga);

            await repCargaOferta.AtualizarAsync(cargaOferta);
            await repCargaOfertaIntegracao.InserirAsync(integracao);
        }

        public async Task GerarIntegracaoOfertadeCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, TipoIntegracaoOfertaCarga tipoIntegracaoOfertaCarga, CancellationToken cancellationToken, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            if (carga == null)
                return;

            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaOferta repCargaOferta = new Repositorio.Embarcador.Cargas.CargaOferta(_unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta = await repCargaOferta.BuscarPorCargaAsync(carga.Codigo);

            await CriarRegistroIntegracao(cargaOferta, tipoIntegracao, tipoIntegracaoOfertaCarga);

            if (tipoIntegracaoOfertaCarga == TipoIntegracaoOfertaCarga.Inativar)
                cargaOferta.Situacao = SituacaoCargaOferta.EmConfirmacao;

            cargaOferta.DataAceite = DateTime.Now;

            await repCargaOferta.AtualizarAsync(cargaOferta);

            cargaOferta.Descricao = "Inativou a oferta ao receber o aceite do frete.";

            if (auditado != null)
                await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, cargaOferta, null, "Atualizado", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, cancellationToken);
        }
    }
}
