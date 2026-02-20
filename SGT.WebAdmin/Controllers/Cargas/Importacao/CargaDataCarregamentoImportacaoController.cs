using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System;
using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Cargas.Importacao
{
    [CustomAuthorize("Cargas/CargaDataCarregamentoImportacao")]
    public class CargaDataCarregamentoImportacaoController : BaseController
    {
		#region Construtores

		public CargaDataCarregamentoImportacaoController(Conexao conexao) : base(conexao) { }

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
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = ImportarCargaDataCarregamento(unitOfWork);

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

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCargaDataCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDataCarregamento repositorioCargaDataCarregamento = new Repositorio.Embarcador.Cargas.CargaDataCarregamento(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.Params("Dados"));
            DateTime dataImportacao = DateTime.Now;
            int numero = repositorioCargaDataCarregamento.BuscarProximoNumero();
            int totalRegistrosImportados = 0;
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento cargaDataCarregamento = new Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento()
                    {
                        CodigoCargaEmbarcador = ObterCodigoCargaEmbarcador(linha),
                        CodigoFilialEmbarcador = ObterValorCampoTexto(linha, "CodigoFilialEmbarcador") ?? throw new ControllerException("Código da filial não informado"),
                        CodigoIntegracaoCliente = ObterValorCampoTexto(linha, "CodigoIntegracaoCliente") ?? throw new ControllerException("Código da loja não informado"),
                        DataCarregamento = ObterValorCampoData(linha, "DataCarregamento"),
                        DataFaturamento = ObterValorCampoData(linha, "DataFaturamento"),
                        DataImportacao = dataImportacao,
                        DataSaidaCentroCarregamento = ObterValorCampoData(linha, "DataSaidaCentroCarregamento"),
                        Numero = numero,
                        Usuario = Usuario
                    };

                    repositorioCargaDataCarregamento.Inserir(cargaDataCarregamento);

                    totalRegistrosImportados++;
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

            retornoImportacao.Importados = totalRegistrosImportados;
            retornoImportacao.Total = linhas.Count;

            return retornoImportacao;
        }

        private string ObterCodigoCargaEmbarcador(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha)
        {
            string codigoCargaEmbarcador = ObterValorCampoTexto(linha, nomeCampo: "CodigoCargaEmbarcador") ?? throw new ControllerException("Número da carga não informado");

            if (!codigoCargaEmbarcador.IsSomenteNumeros())
                throw new ControllerException("Número da carga inválido");

            return codigoCargaEmbarcador;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Data de Carregamento", Propriedade = "DataCarregamento", Tamanho = 160 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Data de Faturamento", Propriedade = "DataFaturamento", Tamanho = 160 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Data de Saída do CD", Propriedade = "DataSaidaCentroCarregamento", Tamanho = 160 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Filial", Propriedade = "CodigoFilialEmbarcador", Tamanho = 140, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Loja", Propriedade = "CodigoIntegracaoCliente", Tamanho = 140, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Número da Carga", Propriedade = "CodigoCargaEmbarcador", Tamanho = 140, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        private DateTime? ObterValorCampoData(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string nomeCampo)
        {
            string valorCampo = ObterValorCampoTexto(linha, nomeCampo);

            return valorCampo.ToNullableDateTime();
        }

        private string ObterValorCampoTexto(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string nomeCampo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna = linha.Colunas?.Where(o => o.NomeCampo == nomeCampo).FirstOrDefault();
            string valorCampo = ((string)coluna?.Valor ?? "").Trim();

            if (string.IsNullOrEmpty(valorCampo))
                return null;

            return valorCampo;
        }

        #endregion
    }
}
