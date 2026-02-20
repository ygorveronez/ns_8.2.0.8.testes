using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/DescargaPessoa")]
    public class DescargaPessoaController : BaseController
    {
		#region Construtores

		public DescargaPessoaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                double cnpjPessoa = Double.Parse(Request.Params("Pessoa"));
                double cnpjPessoaOrigem = Double.Parse(Request.Params("PessoaORigem"));
                Dominio.Entidades.Cliente pessoa = null;
                Dominio.Entidades.Cliente pessoaOrigem = null;
                if (cnpjPessoa > 0)
                    pessoa = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjPessoa };
                if (cnpjPessoaOrigem > 0)
                    pessoaOrigem = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjPessoaOrigem };


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa Origem", "PessoaOrigem", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Hora Início", "HoraInicio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Hora Fim", "HoraFim", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Deixar Reboque para Descarga", "DeixarReboqueParaDescarga", 20, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Pessoa")
                    propOrdena = "Cliente.Nome";
                else if (propOrdena == "PessoaOrigem")
                    propOrdena = "ClienteOrigem.Nome";
                if (propOrdena == "HoraInicio")
                    propOrdena = "HoraInicioDescarga";
                else if (propOrdena == "HoraFim")
                    propOrdena = "HoraLimiteDescarga";

                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> clientesDescarga = repClienteDescarga.Consultar(pessoa != null ? pessoa.CPF_CNPJ : 0, pessoaOrigem != null ? pessoaOrigem.CPF_CNPJ : 0, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repClienteDescarga.ContarConsulta(pessoa != null ? pessoa.CPF_CNPJ : 0, pessoaOrigem != null ? pessoaOrigem.CPF_CNPJ : 0));

                dynamic lista = (from p in clientesDescarga
                                 select new
                                 {
                                     Codigo = p.Codigo,
                                     Pessoa = p.Cliente != null ? p.Cliente.Nome + " (" + p.Cliente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                     PessoaOrigem = p.ClienteOrigem != null ? p.ClienteOrigem.Nome + " (" + p.ClienteOrigem.CPF_CNPJ_Formatado + ")" : string.Empty,
                                     HoraInicio = p.HoraInicioDescarga,
                                     HoraFim = p.HoraLimiteDescarga,
                                     DeixarReboqueParaDescarga = p.DeixarReboqueParaDescarga ? "Sim" : "Não"
                                 }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                double cnpjPessoa = Double.Parse(Request.Params("Pessoa"));
                double cnpjPessoaOrigem = Double.Parse(Request.Params("PessoaORigem"));

                decimal.TryParse(Request.Params("ValorPallet"), out decimal valorPorPallet);
                decimal.TryParse(Request.Params("ValorPorVolume"), out decimal valorPorVolume);

                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorOrigemEDestino(cnpjPessoaOrigem, cnpjPessoa);

                if (clienteDescarga != null)
                    return new JsonpResult(false, "Destino já possui Descarga cadastrada, não é possível inserir uma nova.");

                clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga
                {
                    ValorPorPallet = valorPorPallet,
                    ValorPorVolume = valorPorVolume,
                    Cliente = repCliente.BuscarPorCPFCNPJ(cnpjPessoa),
                    ClienteOrigem = repCliente.BuscarPorCPFCNPJ(cnpjPessoaOrigem),
                    HoraInicioDescarga = Request.Params("HoraInicio"),
                    HoraLimiteDescarga = Request.Params("HoraFim"),
                    DeixarReboqueParaDescarga = Request.GetBoolParam("DeixarReboqueParaDescarga"),
                    Domingo = false
                };

                repClienteDescarga.Inserir(clienteDescarga, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                double cnpjPessoa = Double.Parse(Request.Params("Pessoa"));
                double cnpjPessoaOrigem = Double.Parse(Request.Params("PessoaOrigem"));

                decimal.TryParse(Request.Params("ValorPallet"), out decimal valorPorPallet);
                decimal.TryParse(Request.Params("ValorPorVolume"), out decimal valorPorVolume);

                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorCodigo(codigo, true);

                if (clienteDescarga == null)
                    clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga();

                clienteDescarga.ValorPorVolume = valorPorVolume;
                clienteDescarga.ValorPorPallet = valorPorPallet;
                clienteDescarga.Cliente = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                clienteDescarga.ClienteOrigem = repCliente.BuscarPorCPFCNPJ(cnpjPessoaOrigem);
                clienteDescarga.HoraInicioDescarga = Request.Params("HoraInicio");
                clienteDescarga.HoraLimiteDescarga = Request.Params("HoraFim");
                clienteDescarga.DeixarReboqueParaDescarga = Request.GetBoolParam("DeixarReboqueParaDescarga");

                repClienteDescarga.Atualizar(clienteDescarga, Auditado);

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorCodigo(codigo);
                var dynGrupoPessoas = new
                {
                    Codigo = clienteDescarga.Codigo,
                    Pessoa = new { Codigo = clienteDescarga.Cliente.CPF_CNPJ, Descricao = clienteDescarga.Cliente.Nome },
                    PessoaOrigem = clienteDescarga.ClienteOrigem != null ? new { Codigo = clienteDescarga.ClienteOrigem.CPF_CNPJ, Descricao = clienteDescarga.ClienteOrigem.Nome } : null,
                    HoraInicio = clienteDescarga.HoraInicioDescarga,
                    HoraFim = clienteDescarga.HoraLimiteDescarga,
                    clienteDescarga.DeixarReboqueParaDescarga,
                    ValorPallet = clienteDescarga.ValorPorPallet.ToString("n2"),
                    ValorPorVolume = clienteDescarga.ValorPorVolume.ToString("n3")
                };
                return new JsonpResult(dynGrupoPessoas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorCodigo(codigo);

                if (clienteDescarga != null)
                    repClienteDescarga.Deletar(clienteDescarga, Auditado);
                else
                    return new JsonpResult(false, "Registro não encontrado para exclusão.");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoDescargaPessoa();

            return new JsonpResult(configuracoes.ToList());
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoDescargaPessoa()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ Cliente", Propriedade = "CNPJCliente", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Valor por Pallet", Propriedade = "ValorPallet", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Valor por Volume", Propriedade = "ValorPorVolume", Tamanho = tamanho, CampoInformacao = true });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Cód. Integração Cliente", Propriedade = "CodigoIntegracaoCliente", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "CNPJ Cliente Origem", Propriedade = "CNPJOrigem", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Cód. Integração Cliente Origem", Propriedade = "CodigoIntegracaoOrigem", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Hora Início", Propriedade = "HoraInicioDescarga", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Hora Fim", Propriedade = "HoraLimiteDescarga", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoDescargaPessoa();
                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJCliente = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCliente" select obj).FirstOrDefault();
                        Dominio.Entidades.Cliente cliente = null;
                        if (colCNPJCliente != null)
                        {
                            string CNPJCliente = long.Parse(Utilidades.String.OnlyNumbers(colCNPJCliente.Valor)).ToString("d14");
                            cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(CNPJCliente));
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoCliente = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoCliente" select obj).FirstOrDefault();
                        if (colCodigoIntegracaoCliente != null && cliente == null)
                        {
                            string codigoIntegracaoCliente = Utilidades.String.OnlyNumbers(colCodigoIntegracaoCliente.Valor);
                            cliente = repCliente.BuscarPorCodigoIntegracao(codigoIntegracaoCliente);
                        }

                        if (cliente == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O cliente informado não existe na base multisoftware", i));
                            unitOfWork.Rollback();
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CNPJOrigem" select obj).FirstOrDefault();
                        Dominio.Entidades.Cliente clienteOrigem = null;
                        if (colCNPJOrigem != null && !string.IsNullOrWhiteSpace(colCNPJOrigem.Valor))
                        {
                            string cnpj = long.Parse(Utilidades.String.OnlyNumbers(colCNPJOrigem.Valor)).ToString("d14");
                            clienteOrigem = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpj));

                            if (clienteOrigem == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O cliente de origem informado não existe na base multisoftware", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoOrigem" select obj).FirstOrDefault();
                        if (colCodigoIntegracaoOrigem != null && !string.IsNullOrWhiteSpace(colCodigoIntegracaoOrigem.Valor) && clienteOrigem == null)
                        {
                            string codigoIntegracaoCliente = Utilidades.String.OnlyNumbers(colCodigoIntegracaoOrigem.Valor);
                            clienteOrigem = repCliente.BuscarPorCodigoIntegracao(codigoIntegracaoCliente);

                            if (clienteOrigem == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O cliente de origem informado não existe na base multisoftware", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorPallet = (from obj in linha.Colunas where obj.NomeCampo == "ValorPallet" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorPorVolume = (from obj in linha.Colunas where obj.NomeCampo == "ValorPorVolume" select obj).FirstOrDefault();

                        decimal valorPallet = 0m, valorPorVolume = 0m;

                        if (colValorPallet != null)
                            decimal.TryParse((string)colValorPallet.Valor, out valorPallet);

                        if (colValorPorVolume != null)
                            decimal.TryParse((string)colValorPorVolume.Valor, out valorPorVolume);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraInicioDescarga = (from obj in linha.Colunas where obj.NomeCampo == "HoraInicioDescarga" select obj).FirstOrDefault();
                        string horaInicioDescarga = string.Empty;
                        if (colHoraInicioDescarga != null)
                        {
                            DateTime hora;
                            string strHoraInicioDescarga = (string)colHoraInicioDescarga.Valor;
                            if (!string.IsNullOrWhiteSpace(strHoraInicioDescarga) && DateTime.TryParse(strHoraInicioDescarga, out hora))
                                horaInicioDescarga = hora.ToString("HH:mm");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraLimiteDescarga = (from obj in linha.Colunas where obj.NomeCampo == "HoraLimiteDescarga" select obj).FirstOrDefault();
                        string horaLimiteDescarga = string.Empty;
                        if (colHoraLimiteDescarga != null)
                        {
                            DateTime hora;
                            string strHoraLimiteDescarga = (string)colHoraLimiteDescarga.Valor;
                            if (!string.IsNullOrWhiteSpace(strHoraLimiteDescarga) && DateTime.TryParse(strHoraLimiteDescarga, out hora))
                                horaLimiteDescarga = hora.ToString("HH:mm");
                        }

                        //Salva as informações
                        Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga;
                        if (clienteOrigem != null)
                            clienteDescarga = repClienteDescarga.BuscarPorOrigemEDestino(clienteOrigem.CPF_CNPJ, cliente.CPF_CNPJ);
                        else
                            clienteDescarga = repClienteDescarga.BuscarPorPessoa(cliente.CPF_CNPJ);

                        if (clienteDescarga == null)
                        {
                            clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga
                            {
                                Cliente = cliente,
                                ValorPorPallet = valorPallet,
                                ValorPorVolume = valorPorVolume,
                                ClienteOrigem = clienteOrigem,
                                HoraInicioDescarga = horaInicioDescarga,
                                HoraLimiteDescarga = horaLimiteDescarga
                            };

                            repClienteDescarga.Inserir(clienteDescarga, Auditado);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, clienteDescarga, null, "Cliente Inserido por importação.", unitOfWork);
                        }
                        else
                        {
                            clienteDescarga.Initialize();

                            clienteDescarga.ValorPorPallet = valorPallet;
                            clienteDescarga.ValorPorVolume = valorPorVolume;
                            clienteDescarga.HoraInicioDescarga = horaInicioDescarga;
                            clienteDescarga.HoraLimiteDescarga = horaLimiteDescarga;

                            repClienteDescarga.Atualizar(clienteDescarga, Auditado);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, clienteDescarga, null, "Cliente Atualizado por importação.", unitOfWork);
                        }

                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                        continue;
                    }
                }

                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        #endregion
    }
}
