namespace Servicos.Embarcador.GestaoPatio
{
    public class ConfiguracaoToleranciaPesagem
    {
        #region Atributos Privados

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ConfiguracaoToleranciaPesagem(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public ConfiguracaoToleranciaPesagem(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos publicos

        public void ValidarPesagem(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita)
        {
            Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem repositorioConfiguracaoToleranciaPesagem = new Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracao = repositorioConfiguracaoToleranciaPesagem.ConfiguracoesCompativel(cargaJanelaCarregamentoGuarita.Carga.Filial?.Codigo ?? 0, cargaJanelaCarregamentoGuarita.Carga.ModeloVeicularCarga.Codigo, cargaJanelaCarregamentoGuarita.Carga.TipoDeCarga.Codigo, cargaJanelaCarregamentoGuarita.Carga.TipoOperacao.Codigo);

            if (configuracao == null) return;

            bool dentroTolerancia = false;
            decimal pesoCarga = cargaJanelaCarregamentoGuarita.PesagemFinal - cargaJanelaCarregamentoGuarita.PesagemInicial;
            decimal pesoTotalCarga = cargaJanelaCarregamentoGuarita.Carga.DadosSumarizados.PesoTotal;
            decimal diferenca = pesoCarga - pesoTotalCarga;
            decimal diferencaPercentual = (diferenca / cargaJanelaCarregamentoGuarita.Carga.DadosSumarizados.PesoTotal) * 100;

            if (pesoCarga == 0 || diferenca == 0)
                return;

            if (configuracao.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem.Peso)
            {
                bool dentroToleranciaSuperior = diferenca <= configuracao.ToleranciaPesoSuperior;
                bool dentroToleranciaInferior = diferenca >= configuracao.ToleranciaPesoInferior;

                if (dentroToleranciaSuperior && dentroToleranciaInferior)
                    dentroTolerancia = true;
            }
            else
            {
                bool dentroToleranciaSuperior = diferencaPercentual <= configuracao.PercentualToleranciaPesoSuperior;
                bool dentroToleranciaInferior = diferencaPercentual >= configuracao.PercentualToleranciaPesoInferior;

                if (dentroToleranciaSuperior && dentroToleranciaInferior)
                    dentroTolerancia = true;
            }

            if (!dentroTolerancia)
            {
                Servicos.Embarcador.Carga.CargaAprovacaoPesagem servicoCargaAprovacaoPesagem = new Servicos.Embarcador.Carga.CargaAprovacaoPesagem(_unitOfWork);
                servicoCargaAprovacaoPesagem.CriarAprovacao(cargaJanelaCarregamentoGuarita, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            }
        }

        #endregion
    }
}
