using Repositorio;
using System;

namespace Servicos.Embarcador.Carga
{
    public class ISS : ServicoBase
    {        
        public ISS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Empresa empresa, decimal baseCalculo, Dominio.Entidades.Localidade localidadePrestacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Cliente tomador, Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia, string NBS, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = null;

            Servicos.Embarcador.NFSe.NFSe serNFSe = new NFSe.NFSe(unitOfWork);

            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe;

            if (empresa != null)
                transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(empresa.Codigo, localidadePrestacao.Codigo, tomador?.Localidade.Estado.Sigla ?? "", tomador?.GrupoPessoas?.Codigo ?? 0, tomador?.Localidade.Codigo ?? 0, tipoOperacao?.Codigo ?? 0, tomador?.CPF_CNPJ ?? 0, TipoOcorrencia?.Codigo ?? 0, unitOfWork);
            else
                transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSePorLocalidadeEmpresa(localidadePrestacao.Codigo, localidadePrestacao.Estado?.Sigla ?? "", 0, localidadePrestacao.Codigo, tipoOperacao?.Codigo ?? 0, 0, TipoOcorrencia?.Codigo ?? 0, unitOfWork);//se não sabe quem é o transportador busca uma configuração qualquer da localidade para se ter uma prévia do ISS. Após saber quem é o transportador o ISS será ajustado.

            if (transportadorConfiguracaoNFSe != null)
            {
                regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();
                regraISS.AliquotaISS = transportadorConfiguracaoNFSe.AliquotaISS;
                regraISS.PercentualRetencaoISS = transportadorConfiguracaoNFSe.RetencaoISS;
                regraISS.IncluirISSBaseCalculo = transportadorConfiguracaoNFSe.IncluirISSBaseCalculo;
                regraISS.ValorISS = CalcularInclusaoISSNoFrete(ref baseCalculo, regraISS.AliquotaISS, regraISS.IncluirISSBaseCalculo);
                regraISS.ValorBaseCalculoISS = baseCalculo;
                regraISS.ValorRetencaoISS = CalcularRetencaoISSNoFrete(regraISS.ValorISS, regraISS.PercentualRetencaoISS);

                regraISS.ReterIR = transportadorConfiguracaoNFSe.ReterIReDestacarNFs;
                regraISS.AliquotaIR = transportadorConfiguracaoNFSe.AliquotaIR;
                regraISS.BaseCalculoIR = transportadorConfiguracaoNFSe.BaseCalculoIR;
                regraISS.NBS = !string.IsNullOrEmpty(transportadorConfiguracaoNFSe?.NBS ?? "") ? transportadorConfiguracaoNFSe.NBS : !string.IsNullOrEmpty(NBS) ? NBS : transportadorConfiguracaoNFSe?.ServicoNFSe?.NBS ?? "";
                regraISS.ValorIR = CalcularIRNoFrete(baseCalculo, regraISS.AliquotaIR, regraISS.BaseCalculoIR);
            }

            return regraISS;
        }

        public decimal CalcularInclusaoISSNoFrete(ref decimal valorBaseCalculoISS, decimal aliquota, bool incluirISSBase)
        {
            valorBaseCalculoISS += incluirISSBase ? (aliquota > 0 ? ((valorBaseCalculoISS / ((100 - aliquota) / 100)) - valorBaseCalculoISS) : 0) : 0;
            decimal valorICMS = valorBaseCalculoISS * (aliquota / 100);
            valorBaseCalculoISS = decimal.Round(valorBaseCalculoISS, 2, MidpointRounding.AwayFromZero);
            return Math.Round(valorICMS, 2, MidpointRounding.AwayFromZero);
        }

        public decimal CalcularRetencaoISSNoFrete(decimal valorISS, decimal percentualRetencao)
        {
            return percentualRetencao == 100 ? valorISS : valorISS * (percentualRetencao / 100);
        }

        public decimal CalcularIRNoFrete(decimal valorFrete, decimal aliquotaIR, decimal baseIR)
        {
            decimal valorIR = (valorFrete * (baseIR / 100)) * (aliquotaIR / 100);
            return Math.Round(valorIR, 2, MidpointRounding.AwayFromZero);
        }
    }
}