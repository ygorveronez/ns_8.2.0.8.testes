using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ColetaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("coletas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string nomeRemetente = Request.Params["NomeCliente"];
                double.TryParse(Request.Params["CPFCNPJCliente"], out double cnpjRemetente);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                dynamic pedidos = repPedido.Consultar(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, nomeRemetente, cnpjRemetente, inicioRegistros, 50);
                int countPedidos = repPedido.ContarConsulta(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, nomeRemetente, cnpjRemetente);

                return Json(pedidos, true, null, new string[] { "Codigo", "Número|10", "Remetente|20", "Origem|15", "Destino|15", "Valor NF|10" }, countPedidos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as coletas.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["CodigoColeta"], out codigo);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (pedido == null)
                    return Json<bool>(false, false, "Coleta não encontrada.");

                var retorno = new
                {
                    pedido.Codigo,
                    CPFCNPJDestinatario = pedido.Destinatario != null && pedido.Destinatario.CPF_CNPJ > 0 ? pedido.Destinatario.CPF_CNPJ_Formatado : string.Empty,
                    NomeDestinatario = pedido.Destinatario != null && pedido.Destinatario.CPF_CNPJ > 0 ? pedido.Destinatario.Nome : string.Empty,
                    CPFCNPJRemetente = pedido.Remetente != null && pedido.Remetente.CPF_CNPJ > 0 ? pedido.Remetente.CPF_CNPJ_Formatado : string.Empty,
                    NomeRemetente = pedido.Remetente != null && pedido.Remetente.CPF_CNPJ > 0 ? pedido.Remetente.Nome : string.Empty,
                    CPFCNPJTomador = pedido.Tomador != null && pedido.Tomador.CPF_CNPJ > 0 ? pedido.Tomador.CPF_CNPJ_Formatado : string.Empty,
                    NomeTomador = pedido.Tomador != null && pedido.Tomador.CPF_CNPJ > 0 ? pedido.Tomador.Nome : string.Empty,
                    CodigoOrigem = pedido.Origem != null ? pedido.Origem.Codigo : 0,
                    DescricaoOrigem = pedido.Origem != null ? pedido.Origem.Descricao + " - " + pedido.Origem.Estado.Sigla : string.Empty,
                    CodigoDestino = pedido.Destino != null ? pedido.Destino.Codigo : 0,
                    DescricaoDestino = pedido.Destino != null ? pedido.Destino.Descricao + " - " + pedido.Destino.Estado.Sigla : string.Empty,
                    pedido.Numero,
                    DataInicial = pedido.DataInicialColeta.HasValue ? pedido.DataInicialColeta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataFinal = pedido.DataFinalColeta.HasValue ? pedido.DataFinalColeta.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataEntrega = pedido.PrevisaoEntrega.HasValue ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Situacao = pedido.SituacaoPedido,
                    CodigoTipoCarga = pedido.TipoCarga != null ? pedido.TipoCarga.Codigo : 0,
                    DescricaoTipoCarga = pedido.TipoCarga != null ? pedido.TipoCarga.Descricao : string.Empty,
                    CodigoTipoColeta = pedido.TipoColeta != null ? pedido.TipoColeta.Codigo : 0,
                    DescricaoTipoColeta = pedido.TipoColeta != null ? pedido.TipoColeta.Descricao : string.Empty,
                    ValorNFs = pedido.ValorTotalNotasFiscais.ToString("n2"),
                    ValorFrete = pedido.ValorFreteNegociado.ToString("n2"),
                    Peso = pedido.PesoTotal.ToString("n2"),
                    pedido.TipoPagamento,
                    pedido.Observacao,
                    pedido.ObservacaoCTe,
                    pedido.CodigoPedidoCliente,
                    pedido.Requisitante,
                    pedido.QtVolumes,
                    pedido.NumeroNotaCliente,
                    Veiculos = (from obj in pedido.Veiculos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Placa,
                                    obj.Renavam,
                                    UF = obj.Estado.Sigla,
                                    obj.DescricaoTipo,
                                    obj.DescricaoTipoRodado,
                                    obj.DescricaoTipoCarroceria,
                                    obj.Tara,
                                    obj.CapacidadeKG,
                                    obj.CapacidadeM3,
                                    obj.TipoVeiculo
                                }).OrderBy(o => o.TipoVeiculo).ToList(),
                    Motoristas = (from obj in pedido.Motoristas
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Nome,
                                      obj.CPF
                                  }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da coleta.");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadEspelho()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["CodigoColeta"], out codigo);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.OpcoesEmpresa repOpcoesEmpresa = new Repositorio.OpcoesEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (pedido == null)
                    return Json<bool>(false, false, "Coleta não encontrada.");

                Dominio.ObjetosDeValor.Relatorios.EspelhoColeta coleta = new Dominio.ObjetosDeValor.Relatorios.EspelhoColeta()
                {
                    Codigo = pedido.Codigo,
                    CPFCNPJDestinatario = pedido.Destinatario != null && pedido.Destinatario.CPF_CNPJ > 0 ? pedido.Destinatario.CPF_CNPJ_Formatado : string.Empty,
                    NomeDestinatario = pedido.Destinatario != null && pedido.Destinatario.CPF_CNPJ > 0 ? pedido.Destinatario.Nome : string.Empty,
                    TelefoneDestinatario = pedido.Destinatario != null && pedido.Destinatario.CPF_CNPJ > 0 ? TelefoneEspelho(pedido.Destinatario) : string.Empty,
                    EnderecoDestinatario = pedido.Destinatario != null && pedido.Destinatario.CPF_CNPJ > 0 ? MontaEnderecoEspelho(pedido.Destinatario) : string.Empty,
                    CPFCNPJRemetente = pedido.Remetente != null && pedido.Remetente.CPF_CNPJ > 0 ? pedido.Remetente.CPF_CNPJ_Formatado : string.Empty,
                    NomeRemetente = pedido.Remetente != null && pedido.Remetente.CPF_CNPJ > 0 ? pedido.Remetente.Nome : string.Empty,
                    TelefoneRemetente = pedido.Remetente != null && pedido.Remetente.CPF_CNPJ > 0 ? TelefoneEspelho(pedido.Remetente) : string.Empty,
                    EnderecoRemetente = pedido.Remetente != null && pedido.Remetente.CPF_CNPJ > 0 ? MontaEnderecoEspelho(pedido.Remetente) : string.Empty,
                    CPFCNPJTomador = pedido.Tomador != null && pedido.Tomador.CPF_CNPJ > 0 ? pedido.Tomador.CPF_CNPJ_Formatado : string.Empty,
                    NomeTomador = pedido.Tomador != null && pedido.Tomador.CPF_CNPJ > 0 ? pedido.Tomador.Nome : string.Empty,
                    Origem = pedido.Origem != null ? pedido.Origem.Descricao + " - " + pedido.Origem.Estado.Sigla : string.Empty,
                    Destino = pedido.Destino != null ? pedido.Destino.Descricao + " - " + pedido.Destino.Estado.Sigla : string.Empty,
                    Numero = pedido.Numero,
                    DataInicial = pedido.DataInicialColeta.HasValue ? pedido.DataInicialColeta : null,
                    DataFinal = pedido.DataFinalColeta.HasValue ? pedido.DataFinalColeta : null,
                    DataEntrega = pedido.PrevisaoEntrega.HasValue ? pedido.PrevisaoEntrega : null,
                    Situacao = pedido.SituacaoPedido,
                    TipoCarga = pedido.TipoCarga != null ? pedido.TipoCarga.Descricao : string.Empty,
                    TipoColeta = pedido.TipoColeta != null ? pedido.TipoColeta.Descricao : string.Empty,
                    ValorNFs = pedido.ValorTotalNotasFiscais,
                    ValorFrete = pedido.ValorFreteNegociado,
                    Peso = pedido.PesoTotal,
                    TipoPagamento = pedido.TipoPagamento,
                    Observacao = pedido.Observacao,
                    ObservacaoCTe = pedido.ObservacaoCTe,
                    QtVolumes = pedido.QtVolumes,
                    NumeroNotaCliente = pedido.NumeroNotaCliente,
                    CodigoPedidoCliente = pedido.CodigoPedidoCliente,
                    Requisitante = pedido.Requisitante
                };

                // Define o titulo do relatorio
                string tituloRelatorio = repOpcoesEmpresa.BuscaOpcao("titulo-visualizacao-coleta", this.EmpresaUsuario.Codigo, "Coleta");

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Coletas", new List<Dominio.ObjetosDeValor.Relatorios.EspelhoColeta>() { coleta }));
                dataSources.Add(new ReportDataSource("Veiculos", pedido.Veiculos.OrderBy(o => o.TipoVeiculo).ToList()));
                dataSources.Add(new ReportDataSource("Motoristas", pedido.Motoristas));

                List<ReportParameter> parametros = new List<ReportParameter>();

                parametros.Add(new ReportParameter("TituloRelatorio", tituloRelatorio.ToUpper()));
                parametros.Add(new ReportParameter("NomeEmpresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("CnpjEmpresa", this.EmpresaUsuario.CNPJ));
                parametros.Add(new ReportParameter("IeEmpresa", this.EmpresaUsuario.InscricaoEstadual));
                parametros.Add(new ReportParameter("CidadeEmpresa", this.EmpresaUsuario.Localidade.Descricao + "/" + this.EmpresaUsuario.Localidade.Estado.Sigla));
                parametros.Add(new ReportParameter("TelefoneEmpresa", this.EmpresaUsuario.Telefone));
                parametros.Add(new ReportParameter("EnderecoEmpresa", this.EmpresaUsuario.Endereco + " - " + this.EmpresaUsuario.Numero));
                parametros.Add(new ReportParameter("BairroEmpresa", this.EmpresaUsuario.Bairro));
                parametros.Add(new ReportParameter("CepEmpresa", this.EmpresaUsuario.CEP));
                parametros.Add(new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/EspelhoColeta.rdlc", "PDF", parametros, dataSources);

                unitOfWork.Dispose();

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Coleta_" + coleta.Numero.ToString() + "." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o espelho da coleta.");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.TipoCarga repTipoCarga = new Repositorio.TipoCarga(unidadeDeTrabalho);
                Repositorio.TipoColeta repTipoColeta = new Repositorio.TipoColeta(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                DateTime dataInicial, dataFinal, dataEntrega;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(Request.Params["DataEntrega"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrega);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta? requisitante = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta requisitanteAux;
                if (Enum.TryParse(Request.Params["Requisitante"], out requisitanteAux))
                    requisitante = requisitanteAux;

                Dominio.Enumeradores.TipoPagamento? tipoPagamento = null;
                Dominio.Enumeradores.TipoPagamento tipoPagamentoAux;
                if (Enum.TryParse(Request.Params["TipoPagamento"], out tipoPagamentoAux))
                    tipoPagamento = tipoPagamentoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoAux;
                if (Enum.TryParse(Request.Params["Situacao"], out situacaoAux))
                    situacao = situacaoAux;

                double codDestinatario, codTomador, codRemetente;
                double.TryParse(Request.Params["Destinatario"], out codDestinatario);
                double.TryParse(Request.Params["Tomador"], out codTomador);
                double.TryParse(Request.Params["Remetente"], out codRemetente);
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(codDestinatario);
                Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(codTomador);
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(codRemetente);

                int codOrigem, codDestino;
                int.TryParse(Request.Params["Origem"], out codOrigem);
                int.TryParse(Request.Params["Destino"], out codDestino);
                Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigo(codOrigem);
                Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigo(codDestino);

                int codMotorista, codVeiculo;
                int.TryParse(Request.Params["Motorista"], out codMotorista);
                int.TryParse(Request.Params["Veiculo"], out codVeiculo);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codMotorista);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codVeiculo);

                int codTipoColeta, codTipoCarga;
                int.TryParse(Request.Params["TipoColeta"], out codTipoColeta);
                int.TryParse(Request.Params["TipoCarga"], out codTipoCarga);
                Dominio.Entidades.TipoColeta tipoColeta = repTipoColeta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codTipoColeta);
                Dominio.Entidades.TipoCarga tipoCarga = repTipoCarga.BuscarPorCodigo(codTipoCarga, this.EmpresaUsuario.Codigo);


                // Listar de pedidos para relatorio
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioColetas> pedidos = repPedido.RelatorioColetas(
                    this.EmpresaUsuario.Codigo,
                    dataInicial, dataFinal, dataEntrega,
                    destinatario != null && destinatario.CPF_CNPJ > 0 ? destinatario.CPF_CNPJ : 0,
                    tomador != null && tomador.CPF_CNPJ > 0 ? tomador.CPF_CNPJ : 0,
                    remetente != null && remetente.CPF_CNPJ > 0 ? remetente.CPF_CNPJ : 0,
                    origem.Codigo, destino.Codigo,
                    motorista != null ? motorista.Codigo : 0, veiculo != null ? veiculo.Codigo : 0,
                    tipoColeta != null ? tipoColeta.Codigo : 0, tipoCarga != null ? tipoCarga.Codigo : 0,
                    requisitante, tipoPagamento, situacao
                );

                for (var i = 0; i < pedidos.Count; i++)
                {
                    //Veiculo = string.Join(", ", o.Veiculos.Select(v => v.Placa)),
                    //Motorista = string.Join(", ", o.Motoristas.Select(m => m.CPF + " " + m.Nome))

                    var pedido = repPedido.BuscarPorCodigo(pedidos[i].CodigoPedido);
                    pedidos[i].Veiculo = string.Join(", ", pedido.Veiculos.Select(v => v.Placa));
                    pedidos[i].Motorista = string.Join(", ", pedido.Motoristas.Select(m => m.CPF + " " + m.Nome));
                }

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                List<ReportParameter> parametros = new List<ReportParameter>();

                dataSources.Add(new ReportDataSource("Pedidos", pedidos));

                parametros.Add(new ReportParameter("DataInicial", dataInicial != DateTime.MinValue ? dataInicial.ToString() : "Todas"));
                parametros.Add(new ReportParameter("DataFinal", dataFinal != DateTime.MinValue ? dataFinal.ToString() : "Todas"));
                parametros.Add(new ReportParameter("DataEntrega", dataEntrega != DateTime.MinValue ? dataEntrega.ToString() : "Todas"));

                parametros.Add(new ReportParameter("Tomador", tomador.Codigo != 0 ? tomador.CPF_CNPJ_Formatado : "Todos"));
                parametros.Add(new ReportParameter("Remetente", remetente.Codigo != 0 ? remetente.CPF_CNPJ_Formatado : "Todos"));
                parametros.Add(new ReportParameter("Destinatario", destinatario.Codigo != 0 ? destinatario.CPF_CNPJ_Formatado : "Todos"));

                parametros.Add(new ReportParameter("Origem", origem.Codigo != 0 ? origem.DescricaoCidadeEstado : "Todos"));
                parametros.Add(new ReportParameter("Destino", destino.Codigo != 0 ? destino.DescricaoCidadeEstado : "Todos"));

                parametros.Add(new ReportParameter("Carga", tipoCarga != null ? tipoCarga.Descricao : "Todas"));
                parametros.Add(new ReportParameter("Coleta", tipoColeta != null ? tipoColeta.Descricao : "Todas"));

                parametros.Add(new ReportParameter("Situacao", situacao.HasValue ? situacao.Value.ToString("G") : "Todos"));
                parametros.Add(new ReportParameter("Requisitante", requisitante.HasValue ? requisitante.Value.ToString("G") : "Todos"));
                parametros.Add(new ReportParameter("Pagamento", tipoPagamento.HasValue ? tipoPagamento.Value.ToString("G").Replace("_", " ") : "Todos"));

                parametros.Add(new ReportParameter("Motorista", motorista != null ? motorista.Nome : "Todos"));
                parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"));


                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                string caminhoRelatorio = "Relatorios/RelatorioColeta.rdlc";
                string nomeArquivoEmpresa = "Relatorios/RelatorioColeta_" + this.EmpresaUsuario.CNPJ_SemFormato + ".rdlc";
                if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivoEmpresa)))
                    caminhoRelatorio = nomeArquivoEmpresa;

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb(caminhoRelatorio, "PDF", parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Relatorio_" + DateTime.Now.ToString("yyyy/MM/dd") + "." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório de coletas.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoOrigem, codigoDestino, codigoTipoCarga, codigoTipoColeta, qtdVolumes, numeroNotaCliente, numero;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoOrigem"], out codigoOrigem);
                int.TryParse(Request.Params["CodigoDestino"], out codigoDestino);
                int.TryParse(Request.Params["CodigoTipoCarga"], out codigoTipoCarga);
                int.TryParse(Request.Params["CodigoTipoColeta"], out codigoTipoColeta);
                int.TryParse(Request.Params["QtVolumes"], out qtdVolumes);
                int.TryParse(Request.Params["NumeroNotaCliente"], out numeroNotaCliente);
                int.TryParse(Request.Params["Numero"], out numero);

                double codigoRemetente, codigoDestinatario, codigoTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoRemetente"]), out codigoRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoDestinatario"]), out codigoDestinatario);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoTomador"]), out codigoTomador);

                decimal peso, valorNFs, valorFrete;
                decimal.TryParse(Request.Params["Peso"], out peso);
                decimal.TryParse(Request.Params["ValorNFs"], out valorNFs);
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);

                DateTime dataInicial, dataFinal, dataEntrega;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(Request.Params["DataEntrega"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntrega);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta requisitante;
                Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta>(Request.Params["Requisitante"], out requisitante);

                Dominio.Enumeradores.TipoPagamento tipoPagamento;
                Enum.TryParse<Dominio.Enumeradores.TipoPagamento>(Request.Params["TipoPagamento"], out tipoPagamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao;
                Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido>(Request.Params["Situacao"], out situacao);

                string codigoPedidoCliente = Request.Params["CodigoPedidoCliente"];
                string observacao = Request.Params["Observacao"];
                string observacaoCTe = Request.Params["ObservacaoCTe"];

                List<int> codigosMotoristas = JsonConvert.DeserializeObject<List<int>>(Request.Params["Motoristas"]);
                List<int> codigosVeiculos = JsonConvert.DeserializeObject<List<int>>(Request.Params["Veiculos"]);

                if (dataInicial == DateTime.MinValue)
                    return Json<bool>(false, false, "Data inicial inválida.");

                if (dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Data final inválida.");

                if (dataInicial == DateTime.MinValue)
                    return Json<bool>(false, false, "Data prevista da entrega inválida.");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    pedido = repPedido.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
                    pedido.Empresa = this.EmpresaUsuario;
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.TipoCarga repTipoCarga = new Repositorio.TipoCarga(unidadeDeTrabalho);
                Repositorio.TipoColeta repTipoColeta = new Repositorio.TipoColeta(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);

                pedido.Destinatario = codigoDestinatario > 0 ? repCliente.BuscarPorCPFCNPJ(codigoDestinatario) : null;
                pedido.CodigoPedidoCliente = codigoPedidoCliente;
                pedido.DataInicialColeta = dataInicial;
                pedido.DataFinalColeta = dataFinal;
                pedido.Destino = repLocalidade.BuscarPorCodigo(codigoDestino);
                pedido.Observacao = observacao;
                pedido.ObservacaoCTe = observacaoCTe;
                pedido.Origem = repLocalidade.BuscarPorCodigo(codigoOrigem);
                pedido.PesoTotal = peso;
                pedido.PesoSaldoRestante = peso;
                pedido.PrevisaoEntrega = dataEntrega;
                pedido.Remetente = codigoRemetente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoRemetente) : null;
                pedido.Requisitante = requisitante;
                pedido.SituacaoPedido = situacao;
                pedido.TipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga, this.EmpresaUsuario.Codigo);
                pedido.TipoColeta = repTipoColeta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoTipoColeta);
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal;
                pedido.TipoPagamento = tipoPagamento;
                pedido.Tomador = codigoTomador > 0 ? repCliente.BuscarPorCPFCNPJ(codigoTomador) : null;
                pedido.UltimaAtualizacao = DateTime.Now;
                pedido.ValorTotalNotasFiscais = valorNFs;
                pedido.QtVolumes = qtdVolumes;
                pedido.SaldoVolumesRestante = qtdVolumes;
                pedido.NumeroNotaCliente = numeroNotaCliente;
                pedido.ValorFreteNegociado = valorFrete;

                if (pedido.Motoristas == null)
                    pedido.Motoristas = new List<Dominio.Entidades.Usuario>();
                else
                    pedido.Motoristas.Clear();

                if (pedido.Veiculos == null)
                    pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
                else
                    pedido.Veiculos.Clear();

                for (var i = 0; i < codigosMotoristas.Count; i++)
                    pedido.Motoristas.Add(repMotorista.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigosMotoristas[i]));

                for (var i = 0; i < codigosVeiculos.Count; i++)
                    pedido.Veiculos.Add(repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigosVeiculos[i]));

                if (pedido.Origem == null)
                    return Json<bool>(false, false, "Localidade de origem inválida.");

                if (pedido.Destino == null)
                    return Json<bool>(false, false, "Localidade de destino inválida.");

                if (pedido.Remetente == null || pedido.Destinatario == null)
                    if (pedido.Veiculos.Count <= 0)
                        return Json<bool>(false, false, "Deve ser adicionado ao menos um veículo à coleta.");

                if (numero == 0)
                    numero = repPedido.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                pedido.Numero = numero;

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repPedido.BuscarPorNumero(this.EmpresaUsuario.Codigo, numero);
                if (listaPedidos != null && listaPedidos.Count > 0)
                {
                    for (var i = 0; i < listaPedidos.Count; i++)
                    {
                        if (codigo != listaPedidos[i].Codigo)
                            return Json<bool>(false, false, "Já existe outra coleta com mesmo número!");
                    }
                }

                unidadeDeTrabalho.Start();

                if (codigo > 0)
                {
                    pedido.Usuario = this.Usuario;
                    repPedido.Atualizar(pedido);
                }
                else
                {
                    pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                    pedido.Usuario = this.Usuario;
                    pedido.Autor = this.Usuario;
                    repPedido.Inserir(pedido);

                    pedido.Protocolo = pedido.Codigo;
                    repPedido.Atualizar(pedido);
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.CommitChanges();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a coleta.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterColetasParaCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                double cpfCnpjRemetente, cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]), out cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]), out cpfCnpjDestinatario);

                int codigoOrigem, codigoDestino;
                int.TryParse(Request.Params["CodigoOrigem"], out codigoOrigem);
                int.TryParse(Request.Params["CodigoDestino"], out codigoDestino);

                List<int> codigosVeiculos = JsonConvert.DeserializeObject<List<int>>(Request.Params["Veiculos"]);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                dynamic retorno = repPedido.Consultar(this.EmpresaUsuario.Codigo, cpfCnpjRemetente, cpfCnpjDestinatario, codigoOrigem, codigoDestino, codigosVeiculos.ToArray());

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter as coletas.");
            }
        }

        public ActionResult ObterProximoNumero()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                int numero = repPedido.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                var retorno = new
                {
                    numero
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os próximo número.");
            }
        }

        #endregion

        #region Métodos Privados
        private string TelefoneEspelho(Dominio.Entidades.Cliente cliente)
        {
            string telefone = string.Empty;

            if (!string.IsNullOrWhiteSpace(cliente.Telefone1))
            {
                telefone = Utilidades.String.OnlyNumbers(cliente.Telefone1);
                double teleAux;
                double.TryParse(telefone, out teleAux);

                if (telefone.Length == 10)
                    telefone = String.Format(@"{0:(00) 0000\-0000}", teleAux);
                else if (telefone.Length == 11)
                    telefone = String.Format(@"{0:(00)\ 00000\-0000}", teleAux);
            }

            return telefone;
        }

        private string MontaEnderecoEspelho(Dominio.Entidades.Cliente cliente)
        {
            string enderecoCliente = string.Empty;
            List<string> endereco = new List<string>();

            if (!string.IsNullOrWhiteSpace(cliente.Endereco))
                endereco.Add(cliente.Endereco);

            if (!string.IsNullOrWhiteSpace(cliente.Numero) && !cliente.Numero.ToLower().Equals("s/n"))
                endereco.Add("N" + cliente.Numero);

            if (!string.IsNullOrWhiteSpace(cliente.Bairro))
                endereco.Add(cliente.Bairro);

            enderecoCliente = String.Join(", ", endereco);

            if (cliente.Localidade != null)
            {
                enderecoCliente = enderecoCliente + (!string.IsNullOrEmpty(enderecoCliente) ? " - " : "") + cliente.CEP;
                enderecoCliente = enderecoCliente + (!string.IsNullOrEmpty(enderecoCliente) ? " - " : "") + cliente.Localidade.DescricaoCidadeEstado;
            }

            return enderecoCliente;
        }
        #endregion
    }
}
