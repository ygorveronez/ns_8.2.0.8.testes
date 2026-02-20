using Repositorio;

namespace Servicos.WebService.Frota
{
    public class PneuHistorico : ServicoBase
    {
        public PneuHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico ConverterObjetoPneuHistorico(Dominio.Entidades.Embarcador.Frota.PneuHistorico historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.PneuHistorico repHistoricos = new Repositorio.Embarcador.Frota.PneuHistorico(unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico pneuHistorico = new Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico()
            {
                Protocolo = historico.Codigo,
                Data = historico.Data,
                Descricao = historico.Descricao,
                Servicos = historico.Servicos,
                Tipo = historico.Tipo,
                DataHoraMovimentacao = historico.DataHoraMovimentacao,
                CustoEstimado = historico.CustoEstimado,
                KmAtualRodado = historico.KmAtualRodado,
                BandaRodagem = ConverterObjetoBandaRodagemPneu(historico.BandaRodagem),
                Pneu = ConverterObjetoPneu(historico.Pneu),
            };

           
            return pneuHistorico;
        }
        public Dominio.ObjetosDeValor.Embarcador.Frota.BandaRodagemPneu ConverterObjetoBandaRodagemPneu(Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu)
        {
            if (bandaRodagemPneu != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.BandaRodagemPneu retornoBandaRodagemPneu = new Dominio.ObjetosDeValor.Embarcador.Frota.BandaRodagemPneu()
                {
                    Codigo = bandaRodagemPneu.Codigo,
                    Descricao = bandaRodagemPneu.Descricao,
                    Ativo = bandaRodagemPneu.Ativo,
                    Tipo = bandaRodagemPneu.Tipo,
                    Marca = ConverterObjetoMarcaPneu(bandaRodagemPneu.Marca)
                };
                return retornoBandaRodagemPneu;
            }
            else
                return null;
        }
        public Dominio.ObjetosDeValor.Embarcador.Frota.MarcaPneu ConverterObjetoMarcaPneu(Dominio.Entidades.Embarcador.Frota.MarcaPneu marcaPneu)
        {
            if (marcaPneu != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.MarcaPneu retornoMarcaPneu = new Dominio.ObjetosDeValor.Embarcador.Frota.MarcaPneu()
                {
                    Codigo = marcaPneu.Codigo,
                    Descricao = marcaPneu.Descricao,
                    Ativo = marcaPneu.Ativo,
                };
                return retornoMarcaPneu;
            }
            else
                return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Pneu ConverterObjetoPneu(Dominio.Entidades.Embarcador.Frota.Pneu pneu)
        {
            if (pneu != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.Pneu retornoPneu = new Dominio.ObjetosDeValor.Embarcador.Frota.Pneu()
                {
                    DataEntrada = pneu.DataEntrada,
                    DescricaoNota = pneu.DescricaoNota,
                    DTO = pneu.DTO,
                    KmAtualRodado = pneu.KmAtualRodado,
                    KmAnteriorRodado = pneu.KmAnteriorRodado,
                    KmRodadoEntreSulcos = pneu.KmRodadoEntreSulcos,
                    NumeroFogo = pneu.NumeroFogo,
                    Situacao = pneu.Situacao,
                    Sulco = pneu.Sulco,
                    SulcoAnterior = pneu.SulcoAnterior,
                    TipoAquisicao = pneu.TipoAquisicao,
                    ValorAquisicao = pneu.ValorAquisicao,
                    ValorCustoAtualizado = pneu.ValorCustoAtualizado,
                    ValorCustoKmAtualizado = pneu.ValorCustoKmAtualizado,
                    VidaAtual = pneu.VidaAtual,
                    Almoxarifado = ConverterObjetoAlmoxarifado(pneu.Almoxarifado),
                    DataMovimentacaoEstoque = pneu.DataMovimentacaoEstoque,
                    DataMovimentacaoReforma = pneu.DataMovimentacaoReforma,
                    Modelo = ConverterObjetoModeloPneu(pneu.Modelo),
                    BandaRodagem = ConverterObjetoBandaRodagemPneu(pneu.BandaRodagem)
                };

                return retornoPneu;
            }
            else
                return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Almoxarifado ConverterObjetoAlmoxarifado(Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado)
        {
            if (almoxarifado != null)
            {

                Dominio.ObjetosDeValor.Embarcador.Frota.Almoxarifado retornoAlmoxarifado = new Dominio.ObjetosDeValor.Embarcador.Frota.Almoxarifado()
                {
                    Codigo = almoxarifado.Codigo,
                    Descricao = almoxarifado.Descricao,
                    Email = almoxarifado.Email,
                    Ativo = almoxarifado.Ativo,
                };

                return retornoAlmoxarifado;
            }
            else
                return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.ModeloPneu ConverterObjetoModeloPneu(Dominio.Entidades.Embarcador.Frota.ModeloPneu modeloPneu)
        {
            if (modeloPneu != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.ModeloPneu retornoModeloPneu = new Dominio.ObjetosDeValor.Embarcador.Frota.ModeloPneu()
                {
                    Codigo = modeloPneu.Codigo,
                    Descricao = modeloPneu.Descricao,
                    Ativo = modeloPneu.Ativo,
                    Dimensao = ConverterObjetoDimensaoPneu(modeloPneu.Dimensao),
                    Marca = ConverterObjetoMarcaPneu(modeloPneu.Marca)
                };

                return retornoModeloPneu;
            }
            else
                return null;
        }
        public Dominio.ObjetosDeValor.Embarcador.Frota.DimensaoPneu ConverterObjetoDimensaoPneu(Dominio.Entidades.Embarcador.Frota.DimensaoPneu dimensaoPneu)
        {
            if (dimensaoPneu != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.DimensaoPneu retornoDimensaoPneu = new Dominio.ObjetosDeValor.Embarcador.Frota.DimensaoPneu()
                {
                    Codigo = dimensaoPneu.Codigo,
                    Aplicacao = dimensaoPneu.Aplicacao,
                    Aro = dimensaoPneu.Aro,
                    Largura = dimensaoPneu.Largura,
                    Perfil = dimensaoPneu.Perfil,
                    Radial = dimensaoPneu.Radial,
                    Ativo = dimensaoPneu.Ativo,
                };

                return retornoDimensaoPneu;
            }
            else
                return null;
        }
        #endregion
    }
}
