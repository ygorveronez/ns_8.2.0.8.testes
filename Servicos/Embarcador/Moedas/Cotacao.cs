using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Net;
using System.Threading;

namespace Servicos.Embarcador.Moedas
{
    public class Cotacao : ServicoBase
    {        
        public Cotacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public decimal BuscarCotacaoDiaWSBancoCentral(MoedaCotacaoBancoCentral moeda, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (moeda != MoedaCotacaoBancoCentral.DolarCompra && moeda != MoedaCotacaoBancoCentral.DolarVenda)
                    return 0;

                WsBancoCentral.FachadaWSSGSClient wsBancoCentral = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<WsBancoCentral.FachadaWSSGSClient, WsBancoCentral.FachadaWSSGS>(TipoWebServiceIntegracao.BancoCentral_FachadaWSSGS);
                WsBancoCentral.WSSerieVO retorno = wsBancoCentral.getUltimoValorVO((int)moeda);
                decimal valorBC = decimal.Parse(retorno.ultimoValor.svalor.Replace(".", ","));
                return valorBC;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return 0;
            }
        }

        public decimal BuscarValorCotacaoCliente(MoedaCotacaoBancoCentral moeda, Repositorio.UnitOfWork unitOfWork, double cnpjCliente, decimal valorTaxaFeeder)
        {
            if (valorTaxaFeeder > 0)
                return valorTaxaFeeder;

            Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork);
            try
            {
                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacaoDolar = repCotacao.BuscarCotacaoAtiva(cnpjCliente, MoedaCotacaoBancoCentral.DolarCompra);
                if (cotacaoDolar != null)
                {
                    if (cotacaoDolar.CotacaoAutomaticaViaWS)
                    {
                        Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);
                        try
                        {
                            return serCotacao.BuscarCotacaoDiaWSBancoCentral(cotacaoDolar.MoedaCotacaoBancoCentral, unitOfWork);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex.Message);
                            return 0;
                        }
                    }
                    else
                        return cotacaoDolar.ValorMoeda;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return 0;
            }
        }

        public decimal BuscarValorCotacaoCliente(MoedaCotacaoBancoCentral moeda, Repositorio.UnitOfWork unitOfWork, int codigoGrupoPessoa, DateTime dataBase)
        {
            Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork);
            try
            {
                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacaoDolar = repCotacao.BuscarCotacaoAtiva(moeda, dataBase, codigoGrupoPessoa);
                if (cotacaoDolar != null)
                {
                    if (cotacaoDolar.CotacaoAutomaticaViaWS)
                    {
                        Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);
                        try
                        {
                            return serCotacao.BuscarCotacaoDiaWSBancoCentral(cotacaoDolar.MoedaCotacaoBancoCentral, unitOfWork);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex.Message);
                            return 0;
                        }
                    }
                    else
                        return cotacaoDolar.ValorMoeda;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return 0;
            }
        }

        public void AdicionarCotacaoDiaria(MoedaCotacaoBancoCentral moeda, DateTime dataBase, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Moedas.CotacaoMoedaDiaria repositorioCotacaoMoedaDiaria = new Repositorio.Embarcador.Moedas.CotacaoMoedaDiaria(unitOfWork);

            if (repositorioCotacaoMoedaDiaria.ExisteCotacaoDiaria(moeda, dataBase))
                return;

            decimal valorMoeda = BuscarCotacaoDiaWSBancoCentral(moeda, unitOfWork);

            Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria cotacaoDiaria = new Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria
            {
                MoedaCotacaoBancoCentral = moeda,
                ValorMoeda = valorMoeda,
                DataConsulta = dataBase
            };

            repositorioCotacaoMoedaDiaria.Inserir(cotacaoDiaria);
        }

        public decimal ObterValoMoedaDiaria(MoedaCotacaoBancoCentral moeda, DateTime dataBase, Repositorio.UnitOfWork unitOfWork)
        {
            if (dataBase.Date == DateTime.Today)
                return BuscarCotacaoDiaWSBancoCentral(moeda, unitOfWork);

            Repositorio.Embarcador.Moedas.CotacaoMoedaDiaria repositorioCotacaoMoedaDiaria = new Repositorio.Embarcador.Moedas.CotacaoMoedaDiaria(unitOfWork);

            decimal valorMoeda = repositorioCotacaoMoedaDiaria.BuscarValorCotacaoDiaria(moeda, dataBase);

            return valorMoeda;
        }
    }
}
