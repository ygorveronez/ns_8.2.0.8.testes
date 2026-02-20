using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AgendamentoEntregaPedido")]
    public class AgendamentoEntregaPedidoContatoClienteController : BaseController
    {
        #region Construtores

        public AgendamentoEntregaPedidoContatoClienteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido svcAgendamentoPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, this.Usuario);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega configuracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(unitOfWork).BuscarConfiguracaoPadrao();
                List<int> codigosCargaEntrega = new();
                if (configuracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = svcAgendamentoPedido.ObterPedidosAgendamento(new List<int>() { codigo }, codigosCargaEntrega);
                if (pedidos.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>(unitOfWork);
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Pedidos.Pedido).Name });

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo> anexos = new();
                if (arquivos.Count <= 0)
                {
                    for (int i = 0; i < descricoes?.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo anexo = new Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo()
                        {
                            Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                            GuidArquivo = "",
                            NomeArquivo = "",
                            DataCadastro = DateTime.Now,
                            UsuarioCadastro = this.Usuario.Nome
                        };
                        anexos.Add(anexo);
                    }
                }
                else
                {
                    for (int i = 0; i < arquivos?.Count(); i++)
                    {
                        Servicos.DTO.CustomFile arquivo = arquivos[i];
                        string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                        string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                        arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));

                        Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo anexo = new Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo()
                        {
                            Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                            GuidArquivo = guidArquivo,
                            NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName))),
                            DataCadastro = DateTime.Now,
                            UsuarioCadastro = this.Usuario.Nome
                        };
                        anexos.Add(anexo);
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo anexo in anexos)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo anexoTemp = anexo.Clonar<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo>();
                        anexoTemp.EntidadeAnexo = pedido;
                        repositorioAnexo.Inserir(anexoTemp, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, $"Adicionou o arquivo {anexoTemp.NomeArquivo}.", unitOfWork);
                    }
                }

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        DataCadastro = anexo.DataCadastro.ToString("dd/MM/yyyy : HH:mm"),
                        anexo.UsuarioCadastro
                    }
                ).ToList();

                svcAgendamentoPedido.GerarIntegracaoDriveIn(unitOfWork, pedidos, Cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoIntegracaoMondelezDrivin.ContatoCliente);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>(unitOfWork);
                byte[] arquivoBinario = servicoAnexo.DownloadAnexo(anexo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.EntidadeAnexo, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>(unitOfWork, Auditado);

                servicoAnexo.ExcluirAnexo(anexo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido svcAgendamentoPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, this.Usuario);

                string codigoPessoa = Request.GetStringParam("CodigoTransportador");
                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega configuracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(unitOfWork).BuscarConfiguracaoPadrao();
                List<int> codigosCargaEntrega = new();
                if (configuracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = svcAgendamentoPedido.ObterPedidosAgendamento(new List<int>() { codigo }, codigosCargaEntrega);

                if (pedidos.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo> anexos;

                int codigoTransportador = codigoPessoa.ToInt();
                double codigoCliente = codigoPessoa.ToDouble();

                if (pedidos.Count > 0)
                {
                    Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>(unitOfWork);
                    anexos = repositorioAnexo.BuscarPorEntidades(pedidos.Select(p => p.Codigo).ToList());
                }
                else
                    anexos = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo>();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                List<Dominio.Entidades.Empresa> empresas = codigoTransportador > 0 ? repEmpresa.BuscarComAnexosPorCodigo(codigoTransportador) : null;
                Dominio.Entidades.Empresa empresa = null;

                List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();
                Dominio.Entidades.Cliente cliente = null;

                List<Dominio.Entidades.EmpresaAnexo> empresaAnexos = new List<Dominio.Entidades.EmpresaAnexo>();
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo> pessoaAnexos = new List<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo>();

                if (empresas?.Count == null || empresas?.Count == 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    clientes = codigoCliente > 0d ? repCliente.BuscarComAnexosPorCPFCNPJ(codigoCliente) : null;
                    cliente = clientes.FirstOrDefault();

                    foreach (var cli in clientes)
                        pessoaAnexos = cli.Anexos.ToList();
                }
                else
                {
                    empresa = empresas.FirstOrDefault();
                    foreach (var emp in empresas)
                        empresaAnexos = emp.Anexos.ToList();
                }

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        DataCadastro = anexo.DataCadastro.ToString("dd/MM/yyyy : HH:mm"),
                        anexo.UsuarioCadastro,
                    }
                ).ToList();

                var listaDinamicaAnexosTransportador = (
                    from anexo in empresaAnexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                    ).ToList();

                var listaDinamicaAnexosTransportadorPessoa = (
                    from anexo in pessoaAnexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                    ).ToList();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos,
                    NomeTransportador = empresa != null ? empresa.NomeFantasia ?? "" : cliente?.Nome ?? "",
                    TelefoneTransportador = empresa != null ? empresa.Telefone ?? "" : cliente?.Telefone1 ?? "",
                    EmailTransportador = empresa != null ? empresa.Email ?? "" : cliente?.Email ?? "",
                    LocalidadeTransportador = empresa != null ? empresa.LocalidadeUF ?? "" : cliente?.Localidade?.DescricaoCidadeEstado ?? "",
                    Telefone2Transportador = empresa != null ? empresa.TelefoneContato ?? "" : cliente?.Telefone2 ?? "",
                    ObservacaoTransportador = empresa != null ? empresa.Observacao ?? "" : cliente?.Observacao ?? "",
                    AnexosTransportador = listaDinamicaAnexosTransportador?.Count > 0 ? listaDinamicaAnexosTransportador : listaDinamicaAnexosTransportadorPessoa
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
