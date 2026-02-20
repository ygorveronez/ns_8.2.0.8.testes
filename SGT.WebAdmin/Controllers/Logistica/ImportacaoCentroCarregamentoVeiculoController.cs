using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System;
using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ImportacaoCentroCarregamentoVeiculo")]
    public class ImportacaoCentroCarregamentoVeiculoController : BaseController
    {
		#region Construtores

		public ImportacaoCentroCarregamentoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            return new JsonpResult(configuracoes);
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = ImportarCentroCarregamentoVeiculo(unitOfWork);

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCentroCarregamentoVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.Params("Dados"));
            int totalRegistrosAtualizados = 0;
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                    string placa = ObterPlaca(linha);
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamento(linha, unitOfWork);
                    Dominio.Entidades.Empresa transportador = ObterTransportador(linha, unitOfWork);
                    Dominio.Entidades.Veiculo veiculo = ObterVeiculo(placa, transportador.Codigo, unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> listaCentroCarregamentoPorVeiculo = ObterCentroCarregamentoPorVeiculo(veiculo.Codigo, unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> listaCentroCarregamentoRemoverVeiculo = (from centro in listaCentroCarregamentoPorVeiculo where centro.Codigo != centroCarregamento.Codigo select centro).ToList();

                    foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamentoRemoverVeiculo in listaCentroCarregamentoRemoverVeiculo)
                    {
                        centroCarregamentoRemoverVeiculo.Veiculos.Remove(veiculo);
                        repositorioCentroCarregamento.Atualizar(centroCarregamentoRemoverVeiculo);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamentoRemoverVeiculo, null, $"Removido o veículo {veiculo.Placa_Formatada} do centro de carregamento através da importação", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                    }

                    bool centroCarregamentoNaoPossuiVeiculo = (from centro in listaCentroCarregamentoPorVeiculo where centro.Codigo == centroCarregamento.Codigo select centro).FirstOrDefault() == null;

                    if (centroCarregamentoNaoPossuiVeiculo)
                    {
                        centroCarregamento.Veiculos.Add(veiculo);
                        repositorioCentroCarregamento.Atualizar(centroCarregamento);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, $"Adicionado o veículo {veiculo.Placa_Formatada} ao centro de carregamento através da importação", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                    }

                    totalRegistrosAtualizados++;
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i));
                }
                catch (ControllerException excecao)
                {
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(excecao.Message, i));
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.Importados = totalRegistrosAtualizados;
            retornoImportacao.Total = linhas.Count;

            return retornoImportacao;
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCentroCarregamento = linha.Colunas?.Where(o => o.NomeCampo == "CentroCarregamento").FirstOrDefault();
            string codigoIntegracaoFilial = ((string)colunaCentroCarregamento?.Valor).Trim();

            if (string.IsNullOrEmpty(codigoIntegracaoFilial))
                throw new ControllerException("Centro de carregamento não informado.");

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.buscarPorCodigoEmbarcador(codigoIntegracaoFilial.PadLeft(4, '0'));

            if (filial == null)
                throw new ControllerException("Centro de carregamento não encontrado.");

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(filial.Codigo);

            if (centroCarregamento == null)
                throw new ControllerException("Centro de carregamento não encontrado.");

            return centroCarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> ObterCentroCarregamentoPorVeiculo(int codigoVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> listaCentroCarregamento = repositorioCentroCarregamento.BuscarPorVeiculo(codigoVeiculo);

            return listaCentroCarregamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Placa", Propriedade = "Placa", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Centro de Carregamento", Propriedade = "CentroCarregamento", Tamanho = 180, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CNPJ Transportador", Propriedade = "CNPJTransportador", Tamanho = 155, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        private string ObterPlaca(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPlaca = linha.Colunas?.Where(o => o.NomeCampo == "Placa").FirstOrDefault();
            string placa = ((string)colunaPlaca?.Valor).Trim();

            if (string.IsNullOrEmpty(placa))
                throw new ControllerException("Placa não informada.");

            return placa;
        }

        private Dominio.Entidades.Empresa ObterTransportador(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTransportador = linha.Colunas?.Where(o => o.NomeCampo == "CNPJTransportador").FirstOrDefault();
            long? cnpjTransportador = ((string)colunaTransportador?.Valor).ObterSomenteNumeros().ToNullableLong();

            if (!cnpjTransportador.HasValue)
                throw new ControllerException("CNPJ do transportador não foi informado.");

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCNPJ(cnpjTransportador.Value.ToString("d14"));

            if (transportador == null)
                throw new ControllerException("Transportador não foi encontrado.");

            return transportador;
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(string placa, int codigoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(codigoTransportador, placa);

            if (veiculo == null)
                throw new ControllerException("Veículo não encontrado.");

            return veiculo;
        }

        #endregion
    }
}
