using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FreteController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretes.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string cliente = Request.Params["NomeCliente"];
                string status = Request.Params["Status"];
                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.Frete repFrete = new Repositorio.Frete(unitOfWork);

                List<Dominio.Entidades.Frete> listaFrete = repFrete.Consultar(this.EmpresaUsuario.Codigo, status, cliente, cpfCnpjCliente, inicioRegistros, 50);
                int countFrete = repFrete.ContarConsulta(this.EmpresaUsuario.Codigo, status, cliente, cpfCnpjCliente);

                var retorno = from obj in listaFrete
                              select new
                              {
                                  obj.Codigo,
                                  obj.ClienteOrigem.Nome,
                                  Destino = obj.LocalidadeDestino != null ? string.Concat(obj.LocalidadeDestino.Estado.Sigla, " / ", obj.LocalidadeDestino.Descricao) : string.Empty,
                                  DataInicio = obj.DataInicio.HasValue ? obj.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  DataFim = obj.DataFim.HasValue ? obj.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  ValorFrete = obj.ValorFrete.ToString("n2")
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Cliente Origem|25", "Cidade Destino|20", "Data Início|15", "Data Fim|15", "Valor Frete|15" }, countFrete);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os fretes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.Frete repFrete = new Repositorio.Frete(unitOfWork);
                Dominio.Entidades.Frete frete = repFrete.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                if (frete != null)
                {
                    var retorno = new
                    {
                        frete.Codigo,
                        CPFCNPJClienteOrigem = frete.ClienteOrigem.CPF_CNPJ,
                        NomeClienteOrigem = string.Concat(frete.ClienteOrigem.CPF_CNPJ_Formatado, " - ", frete.ClienteOrigem.Nome),
                        DataFim = frete.DataFim.HasValue ? frete.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataInicio = frete.DataInicio.HasValue ? frete.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                        UFDestino = frete.LocalidadeDestino != null ? frete.LocalidadeDestino.Estado.Sigla : string.Empty,
                        CodigoLocalidadeDestino = frete.LocalidadeDestino != null ? frete.LocalidadeDestino.Codigo : 0,
                        OutrosValores = frete.OutrosValores.ToString("n2"),
                        frete.Status,
                        CodigoUnidadeMedida = frete.UnidadeDeMedida.Codigo,
                        DescricaoUnidadeMedida = string.Concat(frete.UnidadeDeMedida.CodigoDaUnidade, " - ", frete.UnidadeDeMedida.Descricao),
                        ValorFrete = frete.ValorFrete.ToString("n4"),
                        ValorPedagio = frete.ValorPedagio.ToString("n2"),
                        ValorSeguro = frete.ValorSeguro.ToString("n2"),
                        QuantidadeExcedente = frete.QuantidadeExcedente.ToString("n2"),
                        ValorExcedente = frete.ValorExcedente.ToString("n4"),
                        frete.TipoPagamento,
                        ValorMinimo = frete.ValorMinimoFrete.ToString("n2"),
                        ValorDescarga = frete.ValorDescarga.ToString("n2"),
                        PercentualGris = frete.PercentualGris.ToString("n4"),
                        PercentualAdValorem = frete.PercentualAdValorem.ToString("n4"),
                        IncluiICMS = frete.IncluiICMS == Dominio.Enumeradores.IncluiICMSFrete.Sim ? "1" : "0",
                        frete.IncluirPedagioBC,
                        frete.IncluirSeguroBC,
                        frete.IncluirOutrosBC,
                        frete.IncluirDescargaBC,
                        frete.IncluirGrisBC,
                        frete.IncluirAdValoremBC,
                        frete.TipoCliente,
                        frete.TipoMinimo,
                        ValorPedagioPerc = frete.ValorPedagioPerc.ToString("n4")
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Frete não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoCidadeDestino, codigoUnidadeMedida = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoCidadeDestino"], out codigoCidadeDestino);
                int.TryParse(Request.Params["CodigoUnidadeMedida"], out codigoUnidadeMedida);

                decimal valorFrete, valorPedagio, valorSeguro, outrosValores, valorExcedente, quantidadeExcedente,
                    valorMinimo, valorDescarga, percentualGris, percentualAdValorem, valorPedagioPerc = 0;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);
                decimal.TryParse(Request.Params["ValorSeguro"], out valorSeguro);
                decimal.TryParse(Request.Params["OutrosValores"], out outrosValores);
                decimal.TryParse(Request.Params["ValorExcedente"], out valorExcedente);
                decimal.TryParse(Request.Params["QuantidadeExcedente"], out quantidadeExcedente);

                decimal.TryParse(Request.Params["ValorMinimo"], out valorMinimo);
                decimal.TryParse(Request.Params["ValorDescarga"], out valorDescarga);
                decimal.TryParse(Request.Params["PercentualGris"], out percentualGris);
                decimal.TryParse(Request.Params["PercentualAdValorem"], out percentualAdValorem);
                decimal.TryParse(Request.Params["ValorPedagioPerc"], out valorPedagioPerc);

                double codigoClienteOrigem = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOrigem"]), out codigoClienteOrigem);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.Enumeradores.TipoPagamentoFrete tipoPagamento;
                Enum.TryParse<Dominio.Enumeradores.TipoPagamentoFrete>(Request.Params["TipoPagamento"], out tipoPagamento);

                Dominio.Enumeradores.IncluiICMSFrete incluirICMS;
                Enum.TryParse<Dominio.Enumeradores.IncluiICMSFrete>(Request.Params["IncluirICMS"], out incluirICMS);

                Dominio.Enumeradores.OpcaoSimNao incluirPedagioBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirPedagioBC"], out incluirPedagioBC);

                Dominio.Enumeradores.OpcaoSimNao incluirSeguroBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirSeguroBC"], out incluirSeguroBC);

                Dominio.Enumeradores.OpcaoSimNao incluirOutrosBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirOutrosBC"], out incluirOutrosBC);

                Dominio.Enumeradores.OpcaoSimNao incluirDescargaBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirDescargaBC"], out incluirDescargaBC);

                Dominio.Enumeradores.OpcaoSimNao incluirGrisBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirGrisBC"], out incluirGrisBC);

                Dominio.Enumeradores.OpcaoSimNao incluirAdValoremBC;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirAdValoremBC"], out incluirAdValoremBC);

                Dominio.Enumeradores.TipoTomador tipoCliente;
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["TipoCliente"], out tipoCliente);

                Dominio.Enumeradores.TipoFrete tipoMinimo;
                Enum.TryParse<Dominio.Enumeradores.TipoFrete>(Request.Params["TipoMinimo"], out tipoMinimo);

                string status = Request.Params["Status"];

                Repositorio.Frete repFrete = new Repositorio.Frete(unitOfWork);
                Dominio.Entidades.Frete frete;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");
                    frete = repFrete.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                    frete = new Dominio.Entidades.Frete();
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                if (dataInicial != DateTime.MinValue)
                    frete.DataInicio = dataInicial;
                else
                    frete.DataInicio = null;

                if (dataFinal != DateTime.MinValue)
                    frete.DataFim = dataFinal;
                else
                    frete.DataFim = null;

                frete.ClienteOrigem = repCliente.BuscarPorCPFCNPJ(codigoClienteOrigem);
                if (frete.ClienteOrigem == null)
                    return Json<bool>(false, false, "Cliente é obrigatório.");

                frete.LocalidadeDestino = codigoCidadeDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoCidadeDestino) : null;

                frete.UnidadeDeMedida = repUnidadeMedida.BuscarPorCodigo(codigoUnidadeMedida);
                if (frete.UnidadeDeMedida == null)
                    return Json<bool>(false, false, "Unidade de medida é obrigatório.");

                Dominio.Entidades.Frete freteExistente = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoCidadeDestino, false, tipoPagamento, tipoCliente);
                if (freteExistente != null && freteExistente.Codigo != frete.Codigo)
                    return Json<bool>(false, false, "Já existe um frete com a mesma origem e a mesma localidade.");

                frete.Empresa = this.EmpresaUsuario;
                frete.OutrosValores = outrosValores;
                frete.ValorFrete = valorFrete;
                frete.ValorPedagio = valorPedagio;
                frete.ValorSeguro = valorSeguro;
                frete.ValorExcedente = valorExcedente;
                frete.QuantidadeExcedente = quantidadeExcedente;
                frete.TipoPagamento = tipoPagamento;

                frete.ValorMinimoFrete = valorMinimo;
                frete.ValorDescarga = valorDescarga;
                frete.PercentualGris = percentualGris;
                frete.PercentualAdValorem = percentualAdValorem;
                frete.IncluiICMS = incluirICMS;
                frete.IncluirPedagioBC = incluirPedagioBC;
                frete.IncluirSeguroBC = incluirSeguroBC;
                frete.IncluirOutrosBC = incluirOutrosBC;
                frete.IncluirDescargaBC = incluirDescargaBC;
                frete.IncluirGrisBC = incluirGrisBC;
                frete.IncluirAdValoremBC = incluirAdValoremBC;
                frete.TipoCliente = tipoCliente;
                frete.ValorPedagioPerc = valorPedagioPerc;
                frete.TipoMinimo = tipoMinimo;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    frete.Status = status;

                if (codigo > 0)
                    repFrete.Atualizar(frete);
                else
                    repFrete.Inserir(frete);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterFretePorTabelas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoLocalidadeDestino, codigoLocalidadeOrigem;
                int.TryParse(Request.Params["CodigoLocalidadeDestino"], out codigoLocalidadeDestino);
                int.TryParse(Request.Params["CodigoLocalidadeOrigem"], out codigoLocalidadeOrigem);

                Dominio.Enumeradores.TipoPagamento tipoPagamentoCTe;
                Dominio.Enumeradores.TipoPagamentoFrete tipoPagamento;

                if (Enum.TryParse<Dominio.Enumeradores.TipoPagamento>(Request.Params["TipoPagamento"], out tipoPagamentoCTe))
                    tipoPagamento = tipoPagamentoCTe == Dominio.Enumeradores.TipoPagamento.A_Pagar ? Dominio.Enumeradores.TipoPagamentoFrete.A_Pagar :
                                    tipoPagamentoCTe == Dominio.Enumeradores.TipoPagamento.Pago ? Dominio.Enumeradores.TipoPagamentoFrete.Pago :
                                    Dominio.Enumeradores.TipoPagamentoFrete.Todos;
                else
                    tipoPagamento = Dominio.Enumeradores.TipoPagamentoFrete.Todos;

                double codigoClienteOrigem, codigoClienteDestino, codigoClienteExpedidor, codigoClienteRecebedor, codigoClienteOutros;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOrigem"]), out codigoClienteOrigem);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteDestino"]), out codigoClienteDestino);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteExpedidor"]), out codigoClienteExpedidor);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteRecebedor"]), out codigoClienteRecebedor);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoClienteOutros"]), out codigoClienteOutros);

                decimal valorMercadoria = 0;
                decimal.TryParse(Request.Params["ValorMercadoria"], out valorMercadoria);

                Dominio.Enumeradores.TipoTomador tipoTomador;
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["Tomador"], out tipoTomador);
                double codigoClienteTomador = tipoTomador == Dominio.Enumeradores.TipoTomador.Outros && codigoClienteOutros > 0 ? codigoClienteOutros : tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? codigoClienteOrigem : tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? codigoClienteDestino : tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? codigoClienteRecebedor : tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? codigoClienteExpedidor : codigoClienteOrigem;

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Localidade localidadeDestino = repLocalidade.BuscarPorCodigo(codigoLocalidadeDestino);

                List<Dominio.ObjetosDeValor.NotasRemetente> notasJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NotasRemetente>>(Request.Params["Notas"]);
                List<Dominio.ObjetosDeValor.InformacaoCarga> quantidadesJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.InformacaoCarga>>(Request.Params["Quantidades"]);

                Repositorio.FreteFracionadoUnidade repFreteFracionadoUnidade = new Repositorio.FreteFracionadoUnidade(unitOfWork);
                List<Dominio.Entidades.FreteFracionadoUnidade> listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteOrigem, Dominio.Enumeradores.TipoTomador.Remetente, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                if (listaFreteFracionadoUnidade.Count == 0 && codigoClienteDestino > 0)
                    listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteDestino, Dominio.Enumeradores.TipoTomador.Destinatario, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                if (listaFreteFracionadoUnidade.Count == 0 && codigoClienteExpedidor > 0)
                    listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteExpedidor, Dominio.Enumeradores.TipoTomador.Expedidor, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                if (listaFreteFracionadoUnidade.Count == 0 && codigoClienteRecebedor > 0)
                    listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteRecebedor, Dominio.Enumeradores.TipoTomador.Recebedor, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                if (listaFreteFracionadoUnidade.Count == 0 && codigoClienteTomador > 0)
                    listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteTomador, Dominio.Enumeradores.TipoTomador.Outros, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                if (listaFreteFracionadoUnidade.Count == 0)
                    listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(this.EmpresaUsuario.Codigo, "A", 0, null, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);

                if (listaFreteFracionadoUnidade.Count > 0)
                {
                    decimal pesoMaximoTabela, pesoTotalCTe, valorTotalNFe = 0;
                    Dominio.Entidades.UnidadeDeMedida unidadeMedidaTabela = listaFreteFracionadoUnidade.FirstOrDefault().UnidadeDeMedida;

                    pesoMaximoTabela = listaFreteFracionadoUnidade.Max(x => x.PesoAte);
                    valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;
                    pesoTotalCTe = quantidadesJSON != null && quantidadesJSON.Count > 0 ? quantidadesJSON.Where(x => x.UnidadeMedida.Equals(unidadeMedidaTabela.UnidadeMedida)).Sum(x => x.Quantidade) : 0;

                    if (pesoTotalCTe > pesoMaximoTabela)
                    {
                        Dominio.Entidades.FreteFracionadoUnidade frete = listaFreteFracionadoUnidade.LastOrDefault();

                        decimal pesoExcedente = pesoTotalCTe - pesoMaximoTabela;

                        decimal valorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0;
                        decimal valorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0;

                        if (frete.ValorMinimoGris > 0 && valorGris < frete.ValorMinimoGris)
                            valorGris = frete.ValorMinimoGris;
                        if (frete.ValorMinimoAdValorem > 0 && valorAdValorem < frete.ValorMinimoAdValorem)
                            valorAdValorem = frete.ValorMinimoAdValorem;

                        decimal valorFrete = frete.ValorFrete;

                        if (frete.ValorPorUnidadeMedida > 0 && pesoTotalCTe > 0)
                            valorFrete += (frete.ValorPorUnidadeMedida * pesoTotalCTe);

                        var retorno = new
                        {
                            Tabela = Dominio.Enumeradores.TipoTabelaFrete.FracionadaPorUnidade,
                            ValorExcedentePeso = pesoExcedente * frete.ValorExcedente,
                            ValorFrete = valorFrete,
                            ValorAdValorem = valorAdValorem,
                            ValorGris = valorGris,
                            ValorPedagio = frete.ValorPedagio,
                            ValorTAS = frete.ValorTAS,
                            IncluirICMS = frete.IncluiICMS
                        };

                        return Json(retorno, true);
                    }
                    else
                    {
                        for (var i = 0; i < listaFreteFracionadoUnidade.Count; i++)
                        {
                            if (listaFreteFracionadoUnidade[i].PesoDe <= pesoTotalCTe && listaFreteFracionadoUnidade[i].PesoAte >= pesoTotalCTe)
                            {
                                decimal valorGris = listaFreteFracionadoUnidade[i].PercentualGris > 0 ? valorTotalNFe * (listaFreteFracionadoUnidade[i].PercentualGris / 100) : 0;
                                decimal valorAdValorem = listaFreteFracionadoUnidade[i].PercentualAdValorem > 0 ? valorTotalNFe * (listaFreteFracionadoUnidade[i].PercentualAdValorem / 100) : 0;

                                if (listaFreteFracionadoUnidade[i].ValorMinimoGris > 0 && valorGris < listaFreteFracionadoUnidade[i].ValorMinimoGris)
                                    valorGris = listaFreteFracionadoUnidade[i].ValorMinimoGris;
                                if (listaFreteFracionadoUnidade[i].ValorMinimoAdValorem > 0 && valorAdValorem < listaFreteFracionadoUnidade[i].ValorMinimoAdValorem)
                                    valorAdValorem = listaFreteFracionadoUnidade[i].ValorMinimoAdValorem;

                                decimal valorFrete = listaFreteFracionadoUnidade[i].ValorFrete;

                                if (listaFreteFracionadoUnidade[i].ValorPorUnidadeMedida > 0 && pesoTotalCTe > 0)
                                    valorFrete += (listaFreteFracionadoUnidade[i].ValorPorUnidadeMedida * pesoTotalCTe);

                                var retorno = new
                                {
                                    Tabela = Dominio.Enumeradores.TipoTabelaFrete.FracionadaPorUnidade,
                                    ValorExcedentePeso = 0,
                                    ValorFrete = valorFrete,
                                    ValorAdValorem = valorAdValorem,
                                    ValorGris = valorGris,
                                    ValorPedagio = listaFreteFracionadoUnidade[i].ValorPedagio,
                                    ValorTAS = listaFreteFracionadoUnidade[i].ValorTAS,
                                    IncluirICMS = listaFreteFracionadoUnidade[i].IncluiICMS,
                                };
                                return Json(retorno, true);
                            }
                        }
                    }
                }
                else
                {
                    Repositorio.FreteFracionadoValor repFreteFracionadoValor = new Repositorio.FreteFracionadoValor(unitOfWork);
                    List<Dominio.Entidades.FreteFracionadoValor> listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteOrigem, Dominio.Enumeradores.TipoTomador.Remetente, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                    if (listaFreteFracionadoValor.Count == 0 && codigoClienteDestino > 0)
                        listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteDestino, Dominio.Enumeradores.TipoTomador.Destinatario, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                    if (listaFreteFracionadoValor.Count == 0 && codigoClienteExpedidor > 0)
                        listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteExpedidor, Dominio.Enumeradores.TipoTomador.Expedidor, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                    if (listaFreteFracionadoValor.Count == 0 && codigoClienteRecebedor > 0)
                        listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteRecebedor, Dominio.Enumeradores.TipoTomador.Recebedor, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                    if (listaFreteFracionadoValor.Count == 0 && codigoClienteTomador > 0)
                        listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(this.EmpresaUsuario.Codigo, "A", codigoClienteTomador, Dominio.Enumeradores.TipoTomador.Outros, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);
                    if (listaFreteFracionadoValor.Count == 0)
                        listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(this.EmpresaUsuario.Codigo, "A", 0, null, localidadeDestino != null ? localidadeDestino.CodigoIBGE : 0);

                    if (listaFreteFracionadoValor.Count > 0 )
                    {
                        decimal valorMaximoTabela, valorTotalNFe = 0;

                        valorMaximoTabela = listaFreteFracionadoValor.Max(x => x.ValorAte);
                        valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                        if (valorTotalNFe > valorMaximoTabela)
                        {
                            Dominio.Entidades.FreteFracionadoValor frete = listaFreteFracionadoValor.LastOrDefault();

                            decimal valorExcedente = valorTotalNFe - valorMaximoTabela;

                            decimal valorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0;
                            decimal valorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0;

                            if (frete.ValorMinimoGris > 0 && valorGris < frete.ValorMinimoGris)
                                valorGris = frete.ValorMinimoGris;
                            if (frete.ValorMinimoAdValorem > 0 && valorAdValorem < frete.ValorMinimoAdValorem)
                                valorAdValorem = frete.ValorMinimoAdValorem;

                            var retorno = new
                            {
                                Tabela = Dominio.Enumeradores.TipoTabelaFrete.FracionadaPorUnidade,
                                ValorExcedentePeso = valorExcedente * frete.ValorExcedente,
                                ValorFrete = frete.TipoValor == "P" ? valorTotalNFe * (frete.ValorFrete / 100) : frete.ValorFrete,
                                ValorAdValorem = valorAdValorem,
                                ValorGris = valorGris,
                                ValorPedagio = frete.ValorPedagio,
                                ValorTAS = frete.ValorTAS,
                                IncluirICMS = frete.IncluiICMS
                            };

                            return Json(retorno, true);
                        }
                        else
                        {
                            for (var i = 0; i < listaFreteFracionadoValor.Count; i++)
                            {
                                if (listaFreteFracionadoValor[i].ValorDe <= valorTotalNFe && listaFreteFracionadoValor[i].ValorAte >= valorTotalNFe)
                                {
                                    decimal valorGris = listaFreteFracionadoValor[i].PercentualGris > 0 ? valorTotalNFe * (listaFreteFracionadoValor[i].PercentualGris / 100) : 0;
                                    decimal valorAdValorem = listaFreteFracionadoValor[i].PercentualAdValorem > 0 ? valorTotalNFe * (listaFreteFracionadoValor[i].PercentualAdValorem / 100) : 0;

                                    if (listaFreteFracionadoValor[i].ValorMinimoGris > 0 && valorGris < listaFreteFracionadoValor[i].ValorMinimoGris)
                                        valorGris = listaFreteFracionadoValor[i].ValorMinimoGris;
                                    if (listaFreteFracionadoValor[i].ValorMinimoAdValorem > 0 && valorAdValorem < listaFreteFracionadoValor[i].ValorMinimoAdValorem)
                                        valorAdValorem = listaFreteFracionadoValor[i].ValorMinimoAdValorem;

                                    var retorno = new
                                    {
                                        Tabela = Dominio.Enumeradores.TipoTabelaFrete.FracionadaPorUnidade,
                                        ValorExcedentePeso = 0,
                                        ValorFrete = listaFreteFracionadoValor[i].TipoValor == "P" ? valorTotalNFe * (listaFreteFracionadoValor[i].ValorFrete / 100)  : listaFreteFracionadoValor[i].ValorFrete,
                                        ValorAdValorem = valorAdValorem,
                                        ValorGris = valorGris,
                                        ValorPedagio = listaFreteFracionadoValor[i].ValorPedagio,
                                        ValorTAS = listaFreteFracionadoValor[i].ValorTAS,
                                        IncluirICMS = listaFreteFracionadoValor[i].IncluiICMS,
                                    };
                                    return Json(retorno, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        Repositorio.Frete repFrete = new Repositorio.Frete(unitOfWork);

                        Dominio.Entidades.Frete frete = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoLocalidadeDestino, true, tipoPagamento, Dominio.Enumeradores.TipoTomador.Remetente);
                        if (frete == null && codigoClienteDestino > 0)
                            frete = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteDestino, codigoLocalidadeDestino, true, tipoPagamento, Dominio.Enumeradores.TipoTomador.Destinatario);
                        if (frete == null && codigoClienteExpedidor > 0)
                            frete = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteExpedidor, codigoLocalidadeDestino, true, tipoPagamento, Dominio.Enumeradores.TipoTomador.Expedidor);
                        if (frete == null && codigoClienteRecebedor > 0)
                            frete = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteRecebedor, codigoLocalidadeDestino, true, tipoPagamento, Dominio.Enumeradores.TipoTomador.Recebedor);
                        if (frete == null && codigoClienteTomador > 0)
                            frete = repFrete.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, codigoClienteTomador, codigoLocalidadeDestino, true, tipoPagamento, Dominio.Enumeradores.TipoTomador.Outros);

                        if (frete != null)
                        {
                            decimal valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                            var retorno = new
                            {
                                frete.Codigo,
                                CPFCNPJClienteOrigem = frete.ClienteOrigem.CPF_CNPJ,
                                NomeClienteOrigem = string.Concat(frete.ClienteOrigem.CPF_CNPJ_Formatado, " - ", frete.ClienteOrigem.Nome),
                                DataFim = frete.DataFim.HasValue ? frete.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                DataInicio = frete.DataInicio.HasValue ? frete.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                OutrosValores = frete.OutrosValores,
                                frete.Status,
                                CodigoUnidadeMedida = frete.UnidadeDeMedida.Codigo,
                                UnidadeMedida = frete.UnidadeDeMedida.CodigoDaUnidade,
                                DescricaoUnidadeMedida = frete.UnidadeDeMedida.Descricao,
                                ValorFrete = frete.ValorFrete,
                                ValorPedagio = frete.ValorPedagio,
                                ValorSeguro = frete.ValorSeguro,
                                ValorExcedente = frete.ValorExcedente,
                                QuantidadeExcedente = frete.QuantidadeExcedente,
                                Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorPeso,

                                Tipo = frete.TipoMinimo,
                                ValorMinimoFrete = frete.ValorMinimoFrete,
                                FreteMinimoComICMS = frete.FreteMinimoComICMS,
                                ValorDescarga = frete.ValorDescarga,
                                ValorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0,
                                ValorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0,
                                ValorPedagioPerc = frete.ValorPedagioPerc,

                                IncluirICMS = frete.IncluiICMS,
                                AdicionarPedagioBcICMS = frete.IncluirPedagioBC,
                                AdicionarSeguroBcICMS = frete.IncluirSeguroBC,
                                AdicionarOutrosBcICMS = frete.IncluirOutrosBC,
                                AdicionarDescargaBcICMS = frete.IncluirDescargaBC,
                                AdicionarGrisBcICMS = frete.IncluirGrisBC,
                                AdicionarAdValoremBcICMS = frete.IncluirAdValoremBC

                            };

                            return Json(retorno, true);
                        }
                        else
                        {
                            Repositorio.FretePorValor repFretePorValor = new Repositorio.FretePorValor(unitOfWork);
                            Dominio.Entidades.FretePorValor fretePorValor = null;

                            //Busca tabela valor com cliente e com localidade destino
                            fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoLocalidadeDestino, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Remetente);
                            if (fretePorValor == null)
                                fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteDestino, codigoLocalidadeDestino, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Destinatario);
                            if (fretePorValor == null)
                                fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteExpedidor, codigoLocalidadeDestino, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Expedidor);
                            if (fretePorValor == null)
                                fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteRecebedor, codigoLocalidadeDestino, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Recebedor);
                            if (fretePorValor == null)
                                fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteTomador, codigoLocalidadeDestino, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Outros);

                            if (fretePorValor != null)
                            {
                                var retorno = new
                                {
                                    fretePorValor.Codigo,
                                    CPFCNPJClienteOrigem = fretePorValor.ClienteOrigem != null ? fretePorValor.ClienteOrigem.CPF_CNPJ : 0f,
                                    NomeClienteOrigem = fretePorValor.ClienteOrigem != null ? string.Concat(fretePorValor.ClienteOrigem.CPF_CNPJ_Formatado, " - ", fretePorValor.ClienteOrigem.Nome) : "",
                                    DataFim = fretePorValor.DataFim.HasValue ? fretePorValor.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    DataInicio = fretePorValor.DataInicio.HasValue ? fretePorValor.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    UFDestino = fretePorValor.LocalidadeDestino.Estado.Sigla,
                                    CodigoLocalidadeDestino = fretePorValor.LocalidadeDestino.Codigo,
                                    fretePorValor.Status,
                                    ValorMinimoFrete = fretePorValor.ValorMinimoFrete,
                                    PercentualSobreNF = fretePorValor.PercentualSobreNF,
                                    Tipo = fretePorValor.Tipo,
                                    Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorValor,
                                    ValorPedagio = fretePorValor.ValorPedagio,
                                    Rateio = fretePorValor.TipoRateio,
                                    IncluirICMS = fretePorValor.IncluiICMS,
                                    IncluirPedagioBC = fretePorValor.IncluirPedagioBC
                                };

                                return Json(retorno, true);
                            }
                            else
                            {
                                //Busca tabela valor com cliente e sem localidade destino
                                fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteOrigem, 0, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Remetente);
                                if (fretePorValor == null && codigoClienteDestino > 0)
                                    fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteDestino, 0, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Destinatario);
                                if (fretePorValor == null && codigoClienteExpedidor > 0)
                                    fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteExpedidor, 0, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Expedidor);
                                if (fretePorValor == null && codigoClienteRecebedor > 0)
                                    fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteRecebedor, 0, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Recebedor);
                                if (fretePorValor == null && codigoClienteTomador > 0)
                                    fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, codigoClienteTomador, 0, true, tipoPagamento, "", Dominio.Enumeradores.TipoTomador.Outros);

                                if (fretePorValor != null)
                                {
                                    var retorno = new
                                    {
                                        fretePorValor.Codigo,
                                        CPFCNPJClienteOrigem = fretePorValor.ClienteOrigem != null ? fretePorValor.ClienteOrigem.CPF_CNPJ : 0f,
                                        NomeClienteOrigem = fretePorValor.ClienteOrigem != null ? string.Concat(fretePorValor.ClienteOrigem.CPF_CNPJ_Formatado, " - ", fretePorValor.ClienteOrigem.Nome) : "",
                                        DataFim = fretePorValor.DataFim.HasValue ? fretePorValor.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                        DataInicio = fretePorValor.DataInicio.HasValue ? fretePorValor.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                        UFDestino = fretePorValor.LocalidadeDestino != null ? fretePorValor.LocalidadeDestino.Estado.Sigla : string.Empty,
                                        CodigoLocalidadeDestino = fretePorValor.LocalidadeDestino != null ? fretePorValor.LocalidadeDestino.Codigo : 0,
                                        fretePorValor.Status,
                                        ValorMinimoFrete = fretePorValor.ValorMinimoFrete,
                                        PercentualSobreNF = fretePorValor.PercentualSobreNF,
                                        Tipo = fretePorValor.Tipo,
                                        Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorValor,
                                        ValorPedagio = fretePorValor.ValorPedagio,
                                        Rateio = fretePorValor.TipoRateio,
                                        IncluirICMS = fretePorValor.IncluiICMS,
                                        IncluirPedagioBC = fretePorValor.IncluirPedagioBC
                                    };

                                    return Json(retorno, true);
                                }
                                else
                                {
                                    //Busca tabela valor sem cliente e com localidade destino
                                    fretePorValor = repFretePorValor.BuscarParaCalculo(this.EmpresaUsuario.Codigo, 0, codigoLocalidadeDestino, true, tipoPagamento);

                                    if (fretePorValor != null)
                                    {
                                        var retorno = new
                                        {
                                            fretePorValor.Codigo,
                                            CPFCNPJClienteOrigem = fretePorValor.ClienteOrigem != null ? fretePorValor.ClienteOrigem.CPF_CNPJ : 0f,
                                            NomeClienteOrigem = fretePorValor.ClienteOrigem != null ? string.Concat(fretePorValor.ClienteOrigem.CPF_CNPJ_Formatado, " - ", fretePorValor.ClienteOrigem.Nome) : "",
                                            DataFim = fretePorValor.DataFim.HasValue ? fretePorValor.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                            DataInicio = fretePorValor.DataInicio.HasValue ? fretePorValor.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                            UFDestino = fretePorValor.LocalidadeDestino.Estado.Sigla,
                                            CodigoLocalidadeDestino = fretePorValor.LocalidadeDestino.Codigo,
                                            fretePorValor.Status,
                                            ValorMinimoFrete = fretePorValor.ValorMinimoFrete,
                                            PercentualSobreNF = fretePorValor.PercentualSobreNF,
                                            Tipo = fretePorValor.Tipo,
                                            Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorValor,
                                            ValorPedagio = fretePorValor.ValorPedagio,
                                            Rateio = fretePorValor.TipoRateio,
                                            IncluirICMS = fretePorValor.IncluiICMS,
                                            IncluirPedagioBC = fretePorValor.IncluirPedagioBC
                                        };

                                        return Json(retorno, true);
                                    }
                                    else
                                    {
                                        List<Dominio.ObjetosDeValor.Veiculo> veiculosJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Veiculo>>(Request.Params["Veiculos"]);

                                        if (veiculosJSON != null && veiculosJSON.Count() > 0)
                                        {
                                            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                                            List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, (from obj in veiculosJSON select obj.Placa).Distinct().ToArray());

                                            if (veiculos.Count() > 0)
                                            {
                                                var tracao = (from obj in veiculos where obj.TipoVeiculo.Equals("0") select obj).FirstOrDefault();
                                                var reboque = (from obj in veiculos where obj.TipoVeiculo.Equals("1") select obj).FirstOrDefault();

                                                if (tracao == null)
                                                    tracao = (from obj in veiculos select obj).FirstOrDefault();

                                                bool calculouTabelaPorTracao = false;
                                                if (tracao != null && tracao.TipoDoVeiculo != null)
                                                {
                                                    Repositorio.FretePorTipoDeVeiculo repFretePorTipoVeiculo = new Repositorio.FretePorTipoDeVeiculo(unitOfWork);

                                                    Dominio.Entidades.FretePorTipoDeVeiculo fretePorTipoVeiculo = null;

                                                    fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoClienteDestino, tracao.TipoDoVeiculo.Codigo, "A", true, tipoPagamento);

                                                    if (fretePorTipoVeiculo != null)
                                                    {
                                                        var valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                                                        var retorno = new
                                                        {
                                                            fretePorTipoVeiculo.Codigo,
                                                            fretePorTipoVeiculo.ValorFrete,
                                                            fretePorTipoVeiculo.ValorPedagio,
                                                            fretePorTipoVeiculo.ValorDescarga,
                                                            fretePorTipoVeiculo.PercentualGris,
                                                            fretePorTipoVeiculo.PercentualAdValorem,
                                                            ValorGris = fretePorTipoVeiculo.PercentualGris > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualGris / 100) : 0,
                                                            ValorAdValorem = fretePorTipoVeiculo.PercentualAdValorem > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualAdValorem / 100) : fretePorTipoVeiculo.ValorAdValorem,
                                                            IncluirICMS = fretePorTipoVeiculo.IncluiICMS,
                                                            DescricaoTipoVeiculo = fretePorTipoVeiculo.TipoVeiculo.Descricao,
                                                            CPFCNPJDestino = fretePorTipoVeiculo.ClienteDestino.CPF_CNPJ_Formatado,
                                                            CPFCNPJOrigem = fretePorTipoVeiculo.ClienteOrigem.CPF_CNPJ_Formatado,
                                                            Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorTipoDeVeiculo,
                                                            AdicionarGrisBcICMS = fretePorTipoVeiculo.AdicionarGrisBcICMS,
                                                            AdicionarPedagioBcICMS = fretePorTipoVeiculo.AdicionarPedagioBcICMS,
                                                            AdicionarDescargaBcICMS = fretePorTipoVeiculo.AdicionarDescargaBcICMS,
                                                            AdicionarAdValoremBcICMS = fretePorTipoVeiculo.AdicionarAdValoremBcICMS
                                                        };

                                                        calculouTabelaPorTracao = true;
                                                        return Json(retorno, true);
                                                    }
                                                    else
                                                    {
                                                        fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, codigoClienteOrigem, 0, tracao.TipoDoVeiculo.Codigo, "A", true, tipoPagamento);

                                                        if (fretePorTipoVeiculo != null)
                                                        {
                                                            var valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                                                            var retorno = new
                                                            {
                                                                fretePorTipoVeiculo.Codigo,
                                                                fretePorTipoVeiculo.ValorFrete,
                                                                fretePorTipoVeiculo.ValorPedagio,
                                                                fretePorTipoVeiculo.ValorDescarga,
                                                                fretePorTipoVeiculo.PercentualGris,
                                                                fretePorTipoVeiculo.PercentualAdValorem,
                                                                ValorGris = fretePorTipoVeiculo.PercentualGris > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualGris / 100) : 0,
                                                                ValorAdValorem = fretePorTipoVeiculo.PercentualAdValorem > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualAdValorem / 100) : fretePorTipoVeiculo.ValorAdValorem,
                                                                IncluirICMS = fretePorTipoVeiculo.IncluiICMS,
                                                                DescricaoTipoVeiculo = fretePorTipoVeiculo.TipoVeiculo.Descricao,
                                                                CPFCNPJDestino = fretePorTipoVeiculo.ClienteDestino != null && fretePorTipoVeiculo.ClienteDestino.CPF_CNPJ > 0 ? fretePorTipoVeiculo.ClienteDestino.CPF_CNPJ_Formatado : String.Empty,
                                                                CPFCNPJOrigem = fretePorTipoVeiculo.ClienteOrigem.CPF_CNPJ_Formatado,
                                                                Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorTipoDeVeiculo,
                                                                AdicionarGrisBcICMS = fretePorTipoVeiculo.AdicionarGrisBcICMS,
                                                                AdicionarPedagioBcICMS = fretePorTipoVeiculo.AdicionarPedagioBcICMS,
                                                                AdicionarDescargaBcICMS = fretePorTipoVeiculo.AdicionarDescargaBcICMS,
                                                                AdicionarAdValoremBcICMS = fretePorTipoVeiculo.AdicionarAdValoremBcICMS
                                                            };

                                                            calculouTabelaPorTracao = true;
                                                            return Json(retorno, true);
                                                        }
                                                        else
                                                        {
                                                            fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorLocalidadeOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, codigoLocalidadeOrigem, codigoLocalidadeDestino, tracao.TipoDoVeiculo.Codigo, "A", true, tipoPagamento);

                                                            if (fretePorTipoVeiculo != null)
                                                            {
                                                                var valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                                                                var retorno = new
                                                                {
                                                                    fretePorTipoVeiculo.Codigo,
                                                                    fretePorTipoVeiculo.ValorFrete,
                                                                    fretePorTipoVeiculo.ValorPedagio,
                                                                    fretePorTipoVeiculo.ValorDescarga,
                                                                    fretePorTipoVeiculo.PercentualGris,
                                                                    fretePorTipoVeiculo.PercentualAdValorem,
                                                                    ValorGris = fretePorTipoVeiculo.PercentualGris > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualGris / 100) : 0,
                                                                    ValorAdValorem = fretePorTipoVeiculo.PercentualAdValorem > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualAdValorem / 100) : fretePorTipoVeiculo.ValorAdValorem,
                                                                    IncluirICMS = fretePorTipoVeiculo.IncluiICMS,
                                                                    DescricaoTipoVeiculo = fretePorTipoVeiculo.TipoVeiculo.Descricao,
                                                                    LocalidadeDestino = fretePorTipoVeiculo.CidadeDestino.Codigo,
                                                                    LocalidadeOrigem = fretePorTipoVeiculo.CidadeOrigem.Codigo,
                                                                    Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorTipoDeVeiculo,
                                                                    AdicionarPedagioBcICMS = fretePorTipoVeiculo.AdicionarPedagioBcICMS,
                                                                    AdicionarDescargaBcICMS = fretePorTipoVeiculo.AdicionarDescargaBcICMS,
                                                                    AdicionarAdValoremBcICMS = fretePorTipoVeiculo.AdicionarAdValoremBcICMS
                                                                };

                                                                calculouTabelaPorTracao = true;
                                                                return Json(retorno, true);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (!calculouTabelaPorTracao && reboque != null && reboque.TipoDoVeiculo != null)
                                                {
                                                    Repositorio.FretePorTipoDeVeiculo repFretePorTipoVeiculo = new Repositorio.FretePorTipoDeVeiculo(unitOfWork);

                                                    Dominio.Entidades.FretePorTipoDeVeiculo fretePorTipoVeiculo = null;

                                                    fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, codigoClienteOrigem, codigoClienteDestino, reboque.TipoDoVeiculo.Codigo, "A", true, tipoPagamento);

                                                    if (fretePorTipoVeiculo != null)
                                                    {
                                                        var valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                                                        var retorno = new
                                                        {
                                                            fretePorTipoVeiculo.Codigo,
                                                            fretePorTipoVeiculo.ValorFrete,
                                                            fretePorTipoVeiculo.ValorPedagio,
                                                            fretePorTipoVeiculo.ValorDescarga,
                                                            fretePorTipoVeiculo.PercentualGris,
                                                            fretePorTipoVeiculo.PercentualAdValorem,
                                                            ValorGris = fretePorTipoVeiculo.PercentualGris > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualGris / 100) : 0,
                                                            ValorAdValorem = fretePorTipoVeiculo.PercentualAdValorem > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualAdValorem / 100) : fretePorTipoVeiculo.ValorAdValorem,
                                                            IncluirICMS = fretePorTipoVeiculo.IncluiICMS,
                                                            DescricaoTipoVeiculo = fretePorTipoVeiculo.TipoVeiculo.Descricao,
                                                            CPFCNPJDestino = fretePorTipoVeiculo.ClienteDestino != null && fretePorTipoVeiculo.ClienteDestino.CPF_CNPJ > 0 ? fretePorTipoVeiculo.ClienteDestino.CPF_CNPJ_Formatado : string.Empty,
                                                            CPFCNPJOrigem = fretePorTipoVeiculo.ClienteOrigem.CPF_CNPJ_Formatado,
                                                            Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorTipoDeVeiculo,
                                                            AdicionarGrisBcICMS = fretePorTipoVeiculo.AdicionarGrisBcICMS,
                                                            AdicionarPedagioBcICMS = fretePorTipoVeiculo.AdicionarPedagioBcICMS,
                                                            AdicionarDescargaBcICMS = fretePorTipoVeiculo.AdicionarDescargaBcICMS,
                                                            AdicionarAdValoremBcICMS = fretePorTipoVeiculo.AdicionarAdValoremBcICMS
                                                        };

                                                        return Json(retorno, true);
                                                    }
                                                    else
                                                    {
                                                        fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, codigoClienteOrigem, 0, reboque.TipoDoVeiculo.Codigo, "A", true, tipoPagamento);

                                                        if (fretePorTipoVeiculo != null)
                                                        {
                                                            var valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                                                            var retorno = new
                                                            {
                                                                fretePorTipoVeiculo.Codigo,
                                                                fretePorTipoVeiculo.ValorFrete,
                                                                fretePorTipoVeiculo.ValorPedagio,
                                                                fretePorTipoVeiculo.ValorDescarga,
                                                                fretePorTipoVeiculo.PercentualGris,
                                                                fretePorTipoVeiculo.PercentualAdValorem,
                                                                ValorGris = fretePorTipoVeiculo.PercentualGris > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualGris / 100) : 0,
                                                                ValorAdValorem = fretePorTipoVeiculo.PercentualAdValorem > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualAdValorem / 100) : fretePorTipoVeiculo.ValorAdValorem,
                                                                IncluirICMS = fretePorTipoVeiculo.IncluiICMS,
                                                                DescricaoTipoVeiculo = fretePorTipoVeiculo.TipoVeiculo.Descricao,
                                                                CPFCNPJDestino = fretePorTipoVeiculo.ClienteDestino.CPF_CNPJ_Formatado,
                                                                CPFCNPJOrigem = fretePorTipoVeiculo.ClienteOrigem.CPF_CNPJ_Formatado,
                                                                Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorTipoDeVeiculo,
                                                                AdicionarGrisBcICMS = fretePorTipoVeiculo.AdicionarGrisBcICMS,
                                                                AdicionarPedagioBcICMS = fretePorTipoVeiculo.AdicionarPedagioBcICMS,
                                                                AdicionarDescargaBcICMS = fretePorTipoVeiculo.AdicionarDescargaBcICMS,
                                                                AdicionarAdValoremBcICMS = fretePorTipoVeiculo.AdicionarAdValoremBcICMS
                                                            };

                                                            return Json(retorno, true);
                                                        }
                                                        else
                                                        {
                                                            fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorLocalidadeOrigemDestinoETipoVeiculo(this.EmpresaUsuario.Codigo, codigoLocalidadeOrigem, codigoLocalidadeDestino, reboque.TipoDoVeiculo.Codigo, "A", true, tipoPagamento);

                                                            if (fretePorTipoVeiculo != null)
                                                            {
                                                                var valorTotalNFe = valorMercadoria > 0 ? valorMercadoria : notasJSON != null && notasJSON.Count() > 0 ? notasJSON.Sum(x => x.ValorTotal) : 0;

                                                                var retorno = new
                                                                {
                                                                    fretePorTipoVeiculo.Codigo,
                                                                    fretePorTipoVeiculo.ValorFrete,
                                                                    fretePorTipoVeiculo.ValorPedagio,
                                                                    fretePorTipoVeiculo.ValorDescarga,
                                                                    fretePorTipoVeiculo.PercentualGris,
                                                                    fretePorTipoVeiculo.PercentualAdValorem,
                                                                    ValorGris = fretePorTipoVeiculo.PercentualGris > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualGris / 100) : 0,
                                                                    ValorAdValorem = fretePorTipoVeiculo.PercentualAdValorem > 0 ? valorTotalNFe * (fretePorTipoVeiculo.PercentualAdValorem / 100) : fretePorTipoVeiculo.ValorAdValorem,
                                                                    IncluirICMS = fretePorTipoVeiculo.IncluiICMS,
                                                                    DescricaoTipoVeiculo = fretePorTipoVeiculo.TipoVeiculo.Descricao,
                                                                    LocalidadeDestino = fretePorTipoVeiculo.CidadeDestino.Codigo,
                                                                    LocalidadeOrigem = fretePorTipoVeiculo.CidadeOrigem.Codigo,
                                                                    Tabela = Dominio.Enumeradores.TipoTabelaFrete.PorTipoDeVeiculo,
                                                                    AdicionarPedagioBcICMS = fretePorTipoVeiculo.AdicionarPedagioBcICMS,
                                                                    AdicionarDescargaBcICMS = fretePorTipoVeiculo.AdicionarDescargaBcICMS,
                                                                    AdicionarAdValoremBcICMS = fretePorTipoVeiculo.AdicionarAdValoremBcICMS
                                                                };

                                                                return Json(retorno, true);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return Json<bool>(true, false, "Não foram encontrados fretes cadastrados!");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
