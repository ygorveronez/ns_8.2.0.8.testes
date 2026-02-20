using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CargaDadosAverbacao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Contrutores

        public CargaDadosAverbacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao> BuscarDadosAverbacao(List<string> chaveCTe, bool enviarCTeApenasParaTomador)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);

            Servicos.WebService.Carga.DadosAverbacao serWSDadosAverbacao = new Servicos.WebService.Carga.DadosAverbacao(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao> listaAverbacoes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao>();

            if (chaveCTe.Count > 0)
            {
                List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacaoCTe.BuscarPorCTes(chaveCTe);

                foreach (var averbacao in averbacoes)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao = serWSDadosAverbacao.ConverterDadosAverbacaoCTe(averbacao, enviarCTeApenasParaTomador, _unitOfWork);
                    listaAverbacoes.Add(dadosAverbacao);
                }
            }

            return listaAverbacoes;

        }

        public void SalvarDadosAverbacaoCarga(Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosAverbacao repCargaDadosAverbacao = new Repositorio.Embarcador.Cargas.CargaDadosAverbacao(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao cargaDadosAverbacao;

            if (dadosAverbacao.DataRetorno.HasValue && dadosAverbacao.DataRetorno.Value > DateTime.MinValue)
                cargaDadosAverbacao = repCargaDadosAverbacao.BuscarPorCargaAverbacaoApolice(carga.Codigo, dadosAverbacao.Averbacao, dadosAverbacao.DataRetorno.Value);
            else
                cargaDadosAverbacao = repCargaDadosAverbacao.BuscarPorCargaAverbacaoApolice(carga.Codigo, dadosAverbacao.Averbacao, dadosAverbacao.ApoliceSeguroAverbacao?.NumeroApolice ?? "");

            if (cargaDadosAverbacao != null)
                return;

            cargaDadosAverbacao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao()
            {
                Adicional = dadosAverbacao.Adicional,
                ApoliceSeguroAverbacao = null,
                ApoliceSeguro = dadosAverbacao.ApoliceSeguroAverbacao != null ? repApoliceSeguro.BuscarPorApoliceSeguradora(dadosAverbacao.ApoliceSeguroAverbacao.NumeroApolice) : null,
                Averbacao = dadosAverbacao.Averbacao,
                Carga = carga,
                ChaveCTe = dadosAverbacao.ChaveCTe,
                ChaveNFe = dadosAverbacao.ChaveNFe,
                CodigoIntegracao = dadosAverbacao.CodigoIntegracao,
                CodigoRetorno = dadosAverbacao.CodigoRetorno,
                DataRetorno = dadosAverbacao.DataRetorno,
                Desconto = dadosAverbacao.Desconto,
                Forma = dadosAverbacao.Forma,
                IOF = dadosAverbacao.IOF,
                MensagemRetorno = dadosAverbacao.MensagemRetorno,
                NumeroCarga = dadosAverbacao.NumeroCarga,
                NumeroOS = dadosAverbacao.NumeroOS,
                NummeroBooking = dadosAverbacao.NummeroBooking,
                Percentual = dadosAverbacao.Percentual,
                Protocolo = dadosAverbacao.Protocolo,
                SeguradoraAverbacao = dadosAverbacao.SeguradoraAverbacao,
                SituacaoFechamento = dadosAverbacao.SituacaoFechamento,
                Status = dadosAverbacao.Status,
                tentativasIntegracao = dadosAverbacao.tentativasIntegracao,
                Tipo = dadosAverbacao.Tipo
            };

            if (cargaDadosAverbacao.ApoliceSeguro == null)
                return;

            repCargaDadosAverbacao.Inserir(cargaDadosAverbacao);

            return;
        }

        #endregion Métodos Públicos


        #region Métodos Privados


        #endregion Métodos Privados


    }
}