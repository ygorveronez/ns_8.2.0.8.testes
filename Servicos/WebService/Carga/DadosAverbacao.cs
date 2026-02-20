using Repositorio;
using System;
using System.Threading;

namespace Servicos.WebService.Carga
{
    public class DadosAverbacao : ServicoBase
    {        
        public DadosAverbacao(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region Métodos Públicos

        public bool SalvarDadosAverbacaoCarga(Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosAverbacao repCargaDadosAverbacao = new Repositorio.Embarcador.Cargas.CargaDadosAverbacao(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);

            //Servicos.Log.TratarErro("EnviarDadosAverbacao Apolice: " + (dadosAverbacao.ApoliceSeguroAverbacao?.NumeroApolice ?? "em branco") + " CNPJ " + (dadosAverbacao.ApoliceSeguroAverbacao?.Seguradora?.ClienteSeguradora?.CPFCNPJSemFormato ?? "em branco"));
            Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao cargaDadosAverbacao = null;
            if (dadosAverbacao.DataRetorno.HasValue && dadosAverbacao.DataRetorno.Value > DateTime.MinValue)
                cargaDadosAverbacao = repCargaDadosAverbacao.BuscarPorCargaAverbacaoApolice(cargaPedido.Carga.Codigo, dadosAverbacao.Averbacao, dadosAverbacao.DataRetorno.Value);
            else
                cargaDadosAverbacao = repCargaDadosAverbacao.BuscarPorCargaAverbacaoApolice(cargaPedido.Carga.Codigo, dadosAverbacao.Averbacao, dadosAverbacao.ApoliceSeguroAverbacao?.NumeroApolice ?? "");

            if (cargaDadosAverbacao != null)
                return true;

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);

            cargaDadosAverbacao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao()
            {
                Adicional = dadosAverbacao.Adicional,
                ApoliceSeguroAverbacao = null,
                ApoliceSeguro = dadosAverbacao.ApoliceSeguroAverbacao != null ? repApoliceSeguro.BuscarPorApoliceSeguradora(dadosAverbacao.ApoliceSeguroAverbacao.NumeroApolice) : null,
                Averbacao = dadosAverbacao.Averbacao,
                Carga = cargaPedido.Carga,
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
                return false;

            repCargaDadosAverbacao.Inserir(cargaDadosAverbacao);

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao ConverterDadosAverbacaoCTe(Dominio.Entidades.AverbacaoCTe averbacao, bool enviarCTeApenasParaTomador, Repositorio.UnitOfWork unitOfWork)
        {
            if (averbacao == null)
                return null;

            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao = new Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao()
            {
                Adicional = averbacao.Adicional,
                ApoliceSeguroAverbacao = ConverterApoliceSeguroAverbacao(averbacao.ApoliceSeguroAverbacao, unitOfWork),
                Averbacao = averbacao.Averbacao,
                ChaveCTe = averbacao.CTe.Chave,
                ChaveNFe = "",
                CodigoIntegracao = averbacao.CodigoIntegracao,
                CodigoRetorno = averbacao.CodigoRetorno,
                CTe = serCTe.ConverterEntidadeCTeParaObjeto(averbacao.CTe, enviarCTeApenasParaTomador, unitOfWork),
                DataRetorno = averbacao.DataRetorno,
                Desconto = averbacao.Desconto,
                Forma = averbacao.Forma,
                IOF = averbacao.IOF,
                MensagemRetorno = averbacao.MensagemRetorno,
                NumeroCarga = "",
                NumeroOS = "",
                NummeroBooking = "",
                Percentual = averbacao.Percentual,
                Protocolo = averbacao.Protocolo,
                SeguradoraAverbacao = averbacao.SeguradoraAverbacao,
                SituacaoFechamento = averbacao.SituacaoFechamento,
                Status = averbacao.Status,
                tentativasIntegracao = averbacao.tentativasIntegracao,
                Tipo = averbacao.Tipo,
                            };

            return dadosAverbacao;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.ApoliceSeguro ConverterApoliceSeguroAverbacao(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceSeguroAverbacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (apoliceSeguroAverbacao == null || apoliceSeguroAverbacao.ApoliceSeguro == null)
                return null;

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.ApoliceSeguro apoliceSeguro = new Dominio.ObjetosDeValor.Embarcador.CTe.ApoliceSeguro()
            {
                DataUltimoAlerta = apoliceSeguroAverbacao.ApoliceSeguro.DataUltimoAlerta,
                Empresa = serEmpresa.ConverterObjetoEmpresa(apoliceSeguroAverbacao.ApoliceSeguro.Empresa),
                FimVigencia = apoliceSeguroAverbacao.ApoliceSeguro.FimVigencia,
                GrupoPessoa = serPessoa.ConverterObjetoGrupoPessoa(apoliceSeguroAverbacao.ApoliceSeguro.GrupoPessoas),
                InicioVigencia = apoliceSeguroAverbacao.ApoliceSeguro.InicioVigencia,
                NumeroApolice = apoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice,
                NumeroAverbacao = apoliceSeguroAverbacao.ApoliceSeguro.NumeroAverbacao,
                Observacao = apoliceSeguroAverbacao.ApoliceSeguro.Observacao,
                Pessoa = serPessoa.ConverterObjetoPessoa(apoliceSeguroAverbacao.ApoliceSeguro.Pessoa),
                Responsavel = apoliceSeguroAverbacao.ApoliceSeguro.Responsavel,
                Seguradora = ConverterSeguradoraAverbacao(apoliceSeguroAverbacao.ApoliceSeguro.Seguradora, unitOfWork),
                SeguradoraAverbacao = apoliceSeguroAverbacao.ApoliceSeguro.SeguradoraAverbacao,
                ValorLimiteApolice = apoliceSeguroAverbacao.ApoliceSeguro.ValorLimiteApolice
            };

            return apoliceSeguro;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.Seguradora ConverterSeguradoraAverbacao(Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora, Repositorio.UnitOfWork unitOfWork)
        {
            if (seguradora == null)
                return null;

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.Seguradora seg = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguradora()
            {
                ClienteSeguradora = serPessoa.ConverterObjetoPessoa(seguradora.ClienteSeguradora),
                Nome = seguradora.Nome,
                Observacao = seguradora.Observacao
            };

            return seg;
        }


        #endregion


    }
}
