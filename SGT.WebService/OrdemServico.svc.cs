using System;
using System.IO;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class OrdemServico(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IOrdemServico
    {
        public Retorno<bool> RecebimentoDeOrdemServico(int? protocolo, string imagemBase64, string dataExecucao, string observacao, List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens> servicosOrdem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            ValidarToken();

            protocolo ??= 0;

            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repPedidoVendaItens = new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo((int)protocolo);

                if (pedidoVenda == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado uma ordem de serviço para o protocolo informado.");

                unitOfWork.Start();

                if (!string.IsNullOrWhiteSpace(dataExecucao))
                {
                    DateTime dataExecucaoConvertida = dataExecucao.ToDateTime();
                    if (dataExecucaoConvertida > DateTime.MinValue)
                        pedidoVenda.DataEntrega = dataExecucaoConvertida;
                }
                if (!string.IsNullOrWhiteSpace(observacao))
                    pedidoVenda.Observacao = observacao;

                pedidoVenda.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.EmAprovacao;

                if (servicosOrdem != null && servicosOrdem.Count > 0)
                {
                    foreach (var servico in servicosOrdem)
                    {
                        if (!string.IsNullOrWhiteSpace(servico.Codigo) || !string.IsNullOrWhiteSpace(servico.CodigoItem))
                        {
                            int codigoItem = servico.Codigo.ToInt();
                            int codigoServico = servico.CodigoItem.ToInt();
                            if (codigoItem > 0 || codigoServico > 0)
                            {
                                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens item = repPedidoVendaItens.BuscarPorCodigo(codigoItem, true);
                                if (item == null)
                                    item = repPedidoVendaItens.BuscarPorPedidoVendaEServico((int)protocolo, codigoServico);

                                if (item != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(servico.Funcionario?.CPF_CNPJ ?? ""))
                                        item.Funcionario = repUsuario.BuscarPorCPF(Utilidades.String.OnlyNumbers(servico.Funcionario.CPF_CNPJ));
                                    if (!string.IsNullOrWhiteSpace(servico.FuncionarioAuxiliar?.CPF_CNPJ ?? ""))
                                        item.FuncionarioAuxiliar = repUsuario.BuscarPorCPF(Utilidades.String.OnlyNumbers(servico.FuncionarioAuxiliar.CPF_CNPJ));

                                    if (servico.KilometroFinal > 0)
                                        item.KMFinal = servico.KilometroFinal;
                                    else
                                        item.KMFinal = 0;

                                    if (servico.KilometroFinal2 > 0)
                                        item.KMFinal2 = servico.KilometroFinal2;
                                    else
                                        item.KMFinal2 = 0;

                                    if (servico.KilometroInicial > 0)
                                        item.KMInicial = servico.KilometroInicial;
                                    else
                                        item.KMInicial = 0;

                                    if (servico.KilometroInicial2 > 0)
                                        item.KMInicial2 = servico.KilometroInicial2;
                                    else
                                        item.KMInicial2 = 0;

                                    if (item.KMFinal > 0 && item.KMInicial > 0)
                                        item.KMTotal = (item.KMFinal - item.KMInicial);
                                    else
                                        item.KMTotal = 0;
                                    if (item.KMFinal2 > 0 && item.KMInicial2 > 0)
                                        item.KMTotal2 = (item.KMFinal2 - item.KMInicial2);
                                    else
                                        item.KMTotal2 = 0;

                                    if ((item.KMTotal > 0 || item.KMTotal2 > 0) && item.ValorKM > 0)
                                    {
                                        item.KMTotal = (item.KMTotal + item.KMTotal2);
                                        item.ValorTotalKM = Math.Round(item.KMTotal * item.ValorKM, 2, MidpointRounding.ToEven);
                                    }

                                    TimeSpan horaInicial = TimeSpan.MinValue, horaFinal = TimeSpan.MinValue;
                                    if (!string.IsNullOrWhiteSpace((string)servico.HoraInicial))
                                        TimeSpan.TryParse((string)servico.HoraInicial, out horaInicial);
                                    if (!string.IsNullOrWhiteSpace((string)servico.HoraFinal))
                                        TimeSpan.TryParse((string)servico.HoraFinal, out horaFinal);

                                    if (horaInicial > TimeSpan.MinValue)
                                        item.HoraInicial = horaInicial;
                                    else
                                        item.HoraInicial = null;
                                    if (horaFinal > TimeSpan.MinValue)
                                        item.HoraFinal = horaFinal;
                                    else
                                        item.HoraFinal = null;

                                    if (item.HoraInicial != null && item.HoraFinal != null)
                                    {
                                        double totalHoraInicial = item.HoraFinal.Value.TotalMinutes - item.HoraInicial.Value.TotalMinutes;
                                        item.HoraTotal = TimeSpan.FromMinutes(totalHoraInicial);
                                    }
                                    else
                                        item.HoraTotal = null;

                                    TimeSpan horaInicial2 = TimeSpan.MinValue, horaFinal2 = TimeSpan.MinValue;
                                    if (!string.IsNullOrWhiteSpace((string)servico.HoraInicial2))
                                        TimeSpan.TryParse((string)servico.HoraInicial2, out horaInicial2);
                                    if (!string.IsNullOrWhiteSpace((string)servico.HoraFinal2))
                                        TimeSpan.TryParse((string)servico.HoraFinal2, out horaFinal2);

                                    if (horaInicial2 > TimeSpan.MinValue)
                                        item.HoraInicial2 = horaInicial2;
                                    else
                                        item.HoraInicial2 = null;
                                    if (horaFinal2 > TimeSpan.MinValue)
                                        item.HoraFinal2 = horaFinal2;
                                    else
                                        item.HoraFinal2 = null;

                                    if (item.HoraInicial2 != null && item.HoraFinal2 != null)
                                    {
                                        double totalHoraInicial = item.HoraFinal2.Value.TotalMinutes - item.HoraInicial2.Value.TotalMinutes;
                                        item.HoraTotal2 = TimeSpan.FromMinutes(totalHoraInicial);
                                    }
                                    else
                                        item.HoraTotal2 = null;

                                    if ((item.HoraTotal.HasValue || item.HoraTotal2.HasValue) && item.ValorHora > 0)
                                    {
                                        double totalHoras = 0;
                                        if (item.HoraTotal.HasValue)
                                            totalHoras += item.HoraTotal.Value.TotalMinutes;
                                        if (item.HoraTotal2.HasValue)
                                            totalHoras += item.HoraTotal2.Value.TotalMinutes;

                                        if (totalHoras > 0)
                                        {
                                            item.HoraTotal = TimeSpan.FromMinutes(totalHoras);
                                            item.ValorTotalHora = Math.Round((decimal)totalHoras * (item.ValorHora / 60), 2, MidpointRounding.ToEven);
                                        }
                                    }

                                    item.ValorUnitario = (item.ValorTotalHora + item.ValorTotalKM);
                                    item.Quantidade = 1;
                                    item.ValorTotal = item.ValorUnitario * item.Quantidade;

                                    repPedidoVendaItens.Atualizar(item);
                                }
                            }
                        }
                    }
                }


                pedidoVenda.ValorTotal = repPedidoVendaItens.BuscarValorTotal(pedidoVenda.Codigo);
                pedidoVenda.ValorServicos = repPedidoVendaItens.BuscarValorTotal(pedidoVenda.Codigo);

                repPedidoVenda.Atualizar(pedidoVenda);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoVenda, "Atualizado informações pelo APP.", unitOfWork);

                if (!string.IsNullOrWhiteSpace(imagemBase64))
                {
                    byte[] data = System.Convert.FromBase64String(imagemBase64);
                    MemoryStream ms = new MemoryStream(data);

                    string extensao = ".jpg";
                    string token = SalvarImagemAssinatura(ms, unitOfWork);
                    string nomeArquivo = "EN_" + pedidoVenda.Codigo + "_" + pedidoVenda.Cliente.Codigo + extensao;

                    Repositorio.Embarcador.PedidoVenda.PedidoVendaAssinatura repPedidoVendaAssinatura = new Repositorio.Embarcador.PedidoVenda.PedidoVendaAssinatura(unitOfWork);
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaAssinatura pedidoVendaAssinatura = new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaAssinatura
                    {
                        Descricao = "OrdemServicoAssinatura_" + DateTime.Today,
                        GuidArquivo = token,
                        NomeArquivo = nomeArquivo,
                        PedidoVenda = pedidoVenda,
                    };
                    repPedidoVendaAssinatura.Inserir(pedidoVendaAssinatura);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoVenda, "Adicionada assinatura via webservice.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ImportacaoException ex)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração da ordem de serviço!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>> ConsultarOrdemServicoPendente(int? inicio, int? limite, int? codigoEmpresa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;
            codigoEmpresa ??= 0;

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100.");

                List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico> listaPedidoVendas = new List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>();
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                int totalRegistros = repPedidoVenda.ContarConsultarOrdemServicoPendente((int)codigoEmpresa);

                if (totalRegistros > 0)
                {
                    List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidos = repPedidoVenda.ConsultarOrdemServicoPendente((int)codigoEmpresa, "Codigo", "desc", (int)inicio, (int)limite);
                    foreach (Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda in pedidos)
                        listaPedidoVendas.Add(ConverterObjetoOrdemServico(pedidoVenda, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>()
                {
                    Itens = listaPedidoVendas,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou ordem de serviço venda pendentes.", unitOfWork);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>>.CriarRetornoSucesso(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as ordem de serviço venda pendentes");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceOrdemServico;
        }

        #endregion

        #region Métodos privados 

        private string SalvarImagemAssinatura(Stream arquivoImagem, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Assinaturas", "PedidoVenda" });
            string guid = "";

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = arquivoImagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;
            string extensao = ".jpg";
            string token = Guid.NewGuid().ToString().Replace("-", "");
            guid = token;
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guid + extensao);

            using (System.Drawing.Image t = System.Drawing.Image.FromStream(ms))
            {
                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);    
            }
            
            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                throw new ImportacaoException("Arquivo de assinatura enviado não foi armazenado!");

            return token;
        }

        private Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico ConverterObjetoOrdemServico(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Usuarios.Usuario serUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);
            Servicos.WebService.Frota.Veiculo serVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);

            Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico ordemServicoRetornar = new Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServico()
            {
                DataEmissao = pedidoVenda.DataEmissao?.ToDateString(),
                DataEntrega = pedidoVenda.DataEntrega?.ToDateString(),
                FormaPagamento = pedidoVenda.FormaPagamento,
                Funcionario = serUsuario.ConverterObjetoUsuario(pedidoVenda.Funcionario, unitOfWork),
                FuncionarioSolicitante = serUsuario.ConverterObjetoUsuario(pedidoVenda.FuncionarioSolicitante, unitOfWork),
                PessoaSolicitante = pedidoVenda.PessoaSolicitante,
                Numero = pedidoVenda.Numero,
                Observacao = pedidoVenda.Observacao,
                Pessoa = serPessoa.ConverterObjetoPessoa(pedidoVenda.Cliente),
                Protocolo = pedidoVenda.Codigo,
                Status = pedidoVenda.Status,
                ValorProdutos = pedidoVenda.ValorProdutos,
                ValorServicos = pedidoVenda.ValorServicos,
                ValorTotal = pedidoVenda.ValorTotal,
                Veiculo = serVeiculo.ConverterObjetoVeiculo(pedidoVenda.Veiculo, unitOfWork),
                Servicos = ConverterObjetoOrdemServicoItens(pedidoVenda.Codigo, unitOfWork)
            };

            return ordemServicoRetornar;
        }

        private List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens> ConverterObjetoOrdemServicoItens(int codigoPedidoVenda, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Usuarios.Usuario serUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

            List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens> servico = new List<Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens>();

            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repPedidoVendaItens = new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unitOfWork);
            List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens> ordemServicoItens = repPedidoVendaItens.BuscarPorPedidoVenda(codigoPedidoVenda);

            foreach (Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens item in ordemServicoItens)
            {
                Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens ordemServicoItensRetornar = new Dominio.ObjetosDeValor.WebService.OrdemServico.OrdemServicoItens()
                {
                    Codigo = item.Codigo.ToString(),
                    CodigoItem = item.CodigoItem,
                    Descricao = item.DescricaoItem,
                    Funcionario = serUsuario.ConverterObjetoUsuario(item.Funcionario, unitOfWork),
                    FuncionarioAuxiliar = serUsuario.ConverterObjetoUsuario(item.FuncionarioAuxiliar, unitOfWork),
                    HoraInicial = item.HoraInicial?.ToTimeString(),
                    HoraFinal = item.HoraFinal?.ToTimeString(),
                    HoraTotal = item.HoraTotal?.ToTimeString(),
                    HoraInicial2 = item.HoraInicial2?.ToTimeString(),
                    HoraFinal2 = item.HoraFinal2?.ToTimeString(),
                    HoraTotal2 = item.HoraTotal2?.ToTimeString(),
                    KilometroInicial = item.KMInicial,
                    KilometroFinal = item.KMFinal,
                    KilometroTotal = item.KMTotal,
                    KilometroInicial2 = item.KMInicial2,
                    KilometroFinal2 = item.KMFinal,
                    KilometroTotal2 = item.KMTotal2,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario,
                    ValorTotal = item.ValorTotal,
                    ValorDesconto = item.ValorDesconto,
                    ValorHora = item.ValorHora,
                    ValorTotalHora = item.ValorTotalHora,
                    ValorKilometro = item.ValorKM,
                    ValorTotalKilometro = item.ValorTotalKM
                };
                servico.Add(ordemServicoItensRetornar);
            }

            return servico;
        }

        #endregion
    }

}
