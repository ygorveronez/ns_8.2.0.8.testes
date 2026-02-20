using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/AlteracaoMassa")]
    public class AlteracaoMassaController : BaseController
    {
		#region Construtores

		public AlteracaoMassaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.Entidades.Empresa> transportadores = ObterTransportadores(unitOfWork);
                
                unitOfWork.Start();

                ProcessarReenvio(transportadores, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new {
                    Quantidade = transportadores.Count
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoCarga();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarParaProcessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoCarga();

                List<Dominio.Entidades.Empresa> transportadores = new List<Dominio.Entidades.Empresa>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, transportadores, ((dicionario) =>
                {
                    dicionario.TryGetValue("CNPJ", out dynamic dynNumeroCarga);
                    string cnpjTransportador = (string)dynNumeroCarga;

                    return repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                }));

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoImportarArquivo);

                transportadores = transportadores.Distinct().ToList();

                retorno.Importados = transportadores.Count();
                retorno.Retorno = (from obj in transportadores select FormataDado(obj)).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private List<Dominio.Entidades.Empresa> ObterTransportadores(Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            List<Dominio.Entidades.Empresa> transportadores = new List<Dominio.Entidades.Empresa>();
            List<dynamic> dynTransportadores = Request.GetListParam<dynamic>("Transportadores");
            foreach (dynamic dynTransportador in dynTransportadores)
            {
                int codigoEmpresa = ((string)dynTransportador.Codigo).ToInt();
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (transportador != null)
                    transportadores.Add(transportador);
            }

            return transportadores.Distinct().ToList();
        }

        private void ProcessarReenvio(List<Dominio.Entidades.Empresa>  transportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            bool liberacaoParaPagamentoAutomatico = Request.GetBoolParam("LiberacaoParaPagamentoAutomatico");

            foreach (Dominio.Entidades.Empresa transportador in transportadores)
            {
                transportador.Initialize();

                transportador.LiberacaoParaPagamentoAutomatico = liberacaoParaPagamentoAutomatico;

                repEmpresa.Atualizar(transportador);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, transportador, transportador.GetChanges(), Localization.Resources.Transportadores.Transportador.AtualizouEmMassa, unitOfWork);
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoCarga()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ", Propriedade = "CNPJ", Tamanho = 150, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } }
            };

            return configuracoes;
        }

        private dynamic FormataDado(Dominio.Entidades.Empresa empresa)
        {
            return new
            {
                empresa.Codigo,
                empresa.DescricaoStatus,
                empresa.Telefone,
                empresa.EmissaoDocumentosForaDoSistema,
                empresa.RazaoSocial,
                empresa.Descricao,
                CNPJ = empresa.CNPJ_Formatado,
                Localidade = empresa.Localidade.DescricaoCidadeEstado
            };
        }
        #endregion
    }
}
