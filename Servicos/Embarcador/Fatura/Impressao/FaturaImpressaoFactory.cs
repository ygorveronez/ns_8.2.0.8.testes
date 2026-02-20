namespace Servicos.Embarcador.Fatura
{
    public sealed class FaturaImpressaoFactory
    {
        public static FaturaImpressao Criar(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            return Criar(fatura, unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador);
        }

        public static FaturaImpressao Criar(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if (configuracaoEmbarcador.TipoImpressaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura.Multimodal)
                return new FaturaImpressaoMultimodal(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, configuracaoEmbarcador.TipoImpressaoFatura);

            if (configuracaoEmbarcador.TipoImpressaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura.PorDocumentos)
                return new FaturaImpressaoPorDocumentos(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador);

            if (configuracaoEmbarcador.TipoImpressaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura.ParcelasSeparadas)
                return new FaturaImpressaoParcelasSeparadas(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador);

            if (configuracaoEmbarcador.TipoImpressaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura.FaturaChaveCTe)
                return new FaturaImpressaoChaveCTe(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador);

            if (fatura.ClienteTomadorFatura?.GrupoPessoas?.GerarFaturaPorCte ?? false)
                return new FaturaImpressaoPorCte(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador);

            return new FaturaImpressaoPadrao(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador);
        }
    }
}
