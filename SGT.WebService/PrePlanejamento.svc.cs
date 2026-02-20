using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
        [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class PrePlanejamento(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IPrePlanejamento
    {
        #region Métodos Globais

        public Retorno<bool> BuscarPorCodigoIntegracao(string CodigoIntegracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();
                return Retorno<bool>.CriarRetornoDadosInvalidos("Metodo não implementado por enquanto");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("BuscarPrePlanejamento: " + ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha genérica ao realizar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarPrePlanejamento(Dominio.ObjetosDeValor.Embarcador.PrePlanejamento.PrePlanejamento prePlanejamento)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                ValidarToken();

                Servicos.Log.TratarErro("SalvarPrePlanejamento: " + Newtonsoft.Json.JsonConvert.SerializeObject(prePlanejamento));

                if (prePlanejamento == null)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Dados do pré planejamento não enviados;");
                }

                if (string.IsNullOrEmpty(prePlanejamento.CodigoIntegracao))
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Código Integração obrigatório;");
                }
                
                if (string.IsNullOrEmpty(prePlanejamento.NomeGrupo))
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Nome do grupo obrigatório;");
                }

                if (ContemPlacasSemInformacoes(prePlanejamento?.Placas))
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Placa e número da frota são obrigatórios;");
                }

                if(ContemPlacasSemCodigoIntegracao(prePlanejamento?.Metas))
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Código Integração Região na meta é obrigatório;");
                }

                if(prePlanejamento?.Vigencia == null)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Vigência é obrigatório;");
                }

                if(DataEstaFormatada(prePlanejamento.Vigencia.DataInicial) || DataEstaFormatada(prePlanejamento.Vigencia.DataFinal))
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data na vigência não está no formato correto;");
                }

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch(Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarPrePlanejamento: " + ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha genérica ao realizar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePrePlanejamento;
        }


        #endregion

        #region Métodos Privados 

        private bool ContemPlacasSemInformacoes(List<Dominio.ObjetosDeValor.Embarcador.PrePlanejamento.Veiculo> placas)
        {
            return placas?.Count > 0 && placas.Any(placa => string.IsNullOrEmpty(placa.Placa) || string.IsNullOrEmpty(placa.NumeroFrota));
        }

        private bool ContemPlacasSemCodigoIntegracao(List<Dominio.ObjetosDeValor.Embarcador.PrePlanejamento.Meta> metas)
        {
            return metas.Count > 0 && metas.Any(meta => string.IsNullOrEmpty(meta.CodigoIntegracaoRegiao));
        }

        private bool DataEstaFormatada(string data)
        {
            string pattern = @"^\d{2}/\d{2}/\d{4}$";
            return Regex.IsMatch(data, pattern);
        }

        #endregion
    }
}
