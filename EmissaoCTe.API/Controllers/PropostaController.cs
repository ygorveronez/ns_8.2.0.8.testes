using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class PropostaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("proposta.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                double cpfcnpjCliente = 0;
                double.TryParse(Request.Params["Cliente"], out cpfcnpjCliente);

                string nome = Request.Params["Nome"];

                Repositorio.Proposta repProposta = new Repositorio.Proposta(unitOfWork);

                List<Dominio.Entidades.Proposta> propostas = repProposta.Consultar(this.EmpresaUsuario.Codigo, data, cpfcnpjCliente, nome, inicioRegistros, 50);
                int countLista = repProposta.ContarConsulta(this.EmpresaUsuario.Codigo, data, cpfcnpjCliente, nome);

                var retorno = (from obj in propostas
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy"),
                                   Cliente = obj.Cliente != null ? obj.Cliente.NomeFantasia : string.Empty,
                                   Origem = obj.Origem != null ? obj.Origem.DescricaoCidadeEstado : string.Empty,
                                   Destino = obj.Destino != null ? obj.Destino.DescricaoCidadeEstado : string.Empty
                               }).ToList();

                unitOfWork.Dispose();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|10", "Cliente|25", "Origem|25", "Destino|25" }, countLista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as PROPOSTAS.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Proposta repProposta = new Repositorio.Proposta(unitOfWork);
                Repositorio.PropostaItem repPropostaItem = new Repositorio.PropostaItem(unitOfWork);

                Dominio.Entidades.Proposta proposta = repProposta.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (proposta == null)
                    return Json<bool>(false, false, "Proposta não encontrada.");

                List<Dominio.Entidades.PropostaItem> itensProposta = repPropostaItem.BuscaPorProposta(proposta.Codigo);

                var retorno = new
                {
                    Codigo = proposta.Codigo,
                    Numero = proposta.Numero,
                    DataLancamento = proposta.DataLancamento.ToString("dd/MM/yyyy"),
                    Data = proposta.Data.ToString("dd/MM/yyyy"),
                    Cliente = proposta.Cliente != null ? new { Codigo = proposta.Cliente.Codigo, Descricao = proposta.Cliente.Nome } : null,
                    Nome = proposta.Nome,
                    Telefone = proposta.Telefone != null ? proposta.Telefone : string.Empty,
                    Email = proposta.Email != null ? proposta.Email : string.Empty,
                    TipoColeta = proposta.TipoColeta != null ? new { Codigo = proposta.TipoColeta.Codigo, Descricao = proposta.TipoColeta.Descricao } : null,
                    ModalProposta = proposta.ModalProposta,
                    Peso = proposta.Peso.ToString("n2"),
                    TipoVeiculo = proposta.TipoVeiculo,
                    TipoCarga = proposta.TipoCarga != null ? new { Codigo = proposta.TipoCarga.Codigo, Descricao = proposta.TipoCarga.Descricao } : null,
                    Volumes = proposta.Volumes,
                    Dimensoes = !string.IsNullOrWhiteSpace(proposta.Dimensoes) ? proposta.Dimensoes : string.Empty,
                    TipoCarroceria = proposta.TipoCarroceria,
                    Rastreador = proposta.Rastreador,
                    Origem = proposta.Origem != null ? new { Codigo = proposta.Origem.Codigo, Descricao = proposta.Origem.DescricaoCidadeEstado } : null,
                    Destino = proposta.Destino != null ? new { Codigo = proposta.Destino.Codigo, Descricao = proposta.Destino.DescricaoCidadeEstado } : null,
                    ClienteOrigem = proposta.ClienteOrigem != null ? new { Codigo = proposta.ClienteOrigem.Codigo, Descricao = proposta.ClienteOrigem.Nome } : null,
                    ClienteDestino = proposta.ClienteDestino != null ? new { Codigo = proposta.ClienteDestino.Codigo, Descricao = proposta.ClienteDestino.Nome } : null,
                    DiasValidade = proposta.DiasValidade == null ? 30 : proposta.DiasValidade,
                    TextoCustosAdicionais = proposta.TextoCustosAdicionais,
                    TextoFormaCobranca = proposta.TextoFormaCobranca,
                    TextoCTRN = proposta.TextoCTRN,
                    Observacoes = !string.IsNullOrWhiteSpace(proposta.Observacoes) ? proposta.Observacoes : string.Empty,
                    ValorMercadoria = proposta.ValorMercadoria.ToString("n2"),
                    UnidadeMonetaria = proposta.UnidadeMonetaria != null ? proposta.UnidadeMonetaria : string.Empty,
                    Itens = (from obj in itensProposta
                             select new Dominio.ObjetosDeValor.PropostaItem()
                             {
                                 Id = obj.Codigo,
                                 Descricao = obj.Descricao,
                                 Valor = obj.Valor,
                                 Tipo = obj.Tipo == Dominio.Enumeradores.TipoItemProposta.Valor ? "0" : "1",
                                 Excluir = false
                             }).ToList()
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Proposta repProposta = new Repositorio.Proposta(unitOfWork);
                Dominio.Entidades.Proposta proposta = repProposta.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (proposta == null)
                    return Json<bool>(false, false, "Proposta não encontrada.");

                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para exclusao negada!");

                repProposta.Deletar(proposta);

                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [ValidateInput(false)]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoTipoColeta, codigoTipoCarga, volumes, codigoOrigem, codigoDestino, diasValidade = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["TipoColeta"], out codigoTipoColeta);
                int.TryParse(Request.Params["TipoCarga"], out codigoTipoCarga);
                int.TryParse(Request.Params["Volumes"], out volumes);
                int.TryParse(Request.Params["Origem"], out codigoOrigem);
                int.TryParse(Request.Params["Destino"], out codigoDestino);
                int.TryParse(Request.Params["DiasValidade"], out diasValidade);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                decimal peso;
                decimal.TryParse(Request.Params["Peso"], out peso);

                decimal valorMercadoria;
                decimal.TryParse(Request.Params["ValorMercadoria"], out valorMercadoria);

                Dominio.Enumeradores.ModalProposta modalProposta;
                Enum.TryParse<Dominio.Enumeradores.ModalProposta>(Request.Params["ModalProposta"], out modalProposta);
                Dominio.Enumeradores.OpcaoSimNao rastreador;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["Rastreador"], out rastreador);

                string tipoVeiculo = Request.Params["TipoVeiculo"];
                string tipoCarroceria = Request.Params["TipoCarroceria"];
                string dimensoes = Request.Params["Dimensoes"];
                string textoCustosAdicionais = Request.Params["TextoCustosAdicionais"];
                string textoFormaCobranca = Request.Params["TextoFormaCobranca"];
                string textoCTRN = Request.Params["TextoCTRN"];
                string observacoes = Request.Params["Observacoes"];
                string nome = Request.Params["Nome"];
                string email = Request.Params["Email"];
                string telefone = Request.Params["Telefone"];
                string unidadeMonetaria = Request.Params["UnidadeMonetaria"];

                double cnpjCliente, cnpjClienteOrigem, cnpjClienteDestino = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Cliente"]), out cnpjCliente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["ClienteOrigem"]), out cnpjClienteOrigem);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["ClienteDestino"]), out cnpjClienteDestino);

                Repositorio.Proposta repProposta = new Repositorio.Proposta(unidadeDeTrabalho);
                Dominio.Entidades.Proposta proposta = repProposta.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (proposta == null)
                    proposta = new Dominio.Entidades.Proposta();

                if (proposta.Codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");

                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada!");

                    proposta = new Dominio.Entidades.Proposta();

                    // Valores inicias
                    proposta.DataLancamento = DateTime.Now;
                    proposta.Empresa = this.EmpresaUsuario;
                    proposta.Numero = repProposta.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                }

                // Repositorios
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.TipoColeta repTipoColeta = new Repositorio.TipoColeta(unidadeDeTrabalho);
                Repositorio.TipoCarga repTipoCarga = new Repositorio.TipoCarga(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                // Preenchimento dos dados
                proposta.Data = data;
                proposta.Cliente = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
                proposta.Nome = nome;
                proposta.Email = email;
                proposta.Telefone = telefone;
                proposta.TipoColeta = repTipoColeta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoTipoColeta);
                proposta.ModalProposta = modalProposta;
                proposta.Peso = peso;
                proposta.TipoVeiculo = tipoVeiculo;
                proposta.TipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga, this.EmpresaUsuario.Codigo);
                proposta.Volumes = volumes;
                proposta.Dimensoes = dimensoes;
                proposta.TipoCarroceria = tipoCarroceria;
                proposta.Rastreador = rastreador;
                proposta.Origem = codigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(codigoOrigem) : null;
                proposta.Destino = codigoDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoDestino) : null;
                proposta.ClienteOrigem = cnpjClienteOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjClienteOrigem) : null;
                proposta.ClienteDestino = cnpjClienteDestino > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjClienteDestino) : null;
                proposta.DiasValidade = diasValidade;
                proposta.TextoCustosAdicionais = textoCustosAdicionais;
                proposta.TextoFormaCobranca = textoFormaCobranca;
                proposta.TextoCTRN = textoCTRN;
                proposta.Observacoes = observacoes;
                proposta.ValorMercadoria = valorMercadoria;
                proposta.UnidadeMonetaria = unidadeMonetaria;

                unidadeDeTrabalho.Start();

                if (proposta.Codigo > 0)
                    repProposta.Atualizar(proposta);
                else
                    repProposta.Inserir(proposta);

                this.SalvarItens(proposta, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return Json(new
                {
                    Codigo = proposta.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterProximoNumero()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Proposta repProposta = new Repositorio.Proposta(unidadeDeTrabalho);
                int numero = repProposta.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                var retorno = new
                {
                    Numero = numero
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os próximo número.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult Visualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Proposta repProposta = new Repositorio.Proposta(unitOfWork);
                Repositorio.PropostaItem repPropostaItem = new Repositorio.PropostaItem(unitOfWork);

                Dominio.Entidades.Proposta propostas = repProposta.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoProposta> listaPropostas = repProposta.RelatorioPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoTextosPropostas> listaTextosPropostas = repProposta.TextoRelatorioPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoPropostaItem> listaItens = repPropostaItem.RelatorioPorCodigo(codigo);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Numero", propostas.Numero.ToString()));
                parametros.Add(new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Proposta", listaPropostas));
                dataSources.Add(new ReportDataSource("Textos", listaTextosPropostas));
                dataSources.Add(new ReportDataSource("Itens", listaItens));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = null;

                if ((from o in listaItens where o.Tipo != 0 select o).Count() > 0)
                     arquivo = svcRelatorio.GerarWeb("Relatorios/VisualizacaoPropostaSemTotal.rdlc", "PDF", parametros, dataSources);
                else
                    arquivo = svcRelatorio.GerarWeb("Relatorios/VisualizacaoProposta.rdlc", "PDF", parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Proposta - " + propostas.Data.ToString("dd-MM-yyyy") + "." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar a proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void SalvarItens(Dominio.Entidades.Proposta proposta, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Itens"]))
            {
                List<Dominio.ObjetosDeValor.PropostaItem> itens = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PropostaItem>>(Request.Params["Itens"]);

                if (itens != null)
                {
                    Repositorio.PropostaItem repPropostaItem = new Repositorio.PropostaItem(unidadeDeTrabalho);

                    for (var i = 0; i < itens.Count; i++)
                    {
                        Dominio.Entidades.PropostaItem item = repPropostaItem.BuscaPorCodigo(itens[i].Id, proposta.Codigo);

                        Dominio.Enumeradores.TipoItemProposta tipoItemPropostaAux;
                        Dominio.Enumeradores.TipoItemProposta? tipoItemProposta = Dominio.Enumeradores.TipoItemProposta.Valor;
                        if (Enum.TryParse<Dominio.Enumeradores.TipoItemProposta>(itens[i].Tipo, out tipoItemPropostaAux))
                            tipoItemProposta = tipoItemPropostaAux;

                        if (item == null)
                            item = new Dominio.Entidades.PropostaItem();

                        item.Proposta = proposta;
                        item.Descricao = itens[i].Descricao;
                        item.Valor = itens[i].Valor;
                        item.Tipo = itens[i].Tipo == "0" ? Dominio.Enumeradores.TipoItemProposta.Valor : Dominio.Enumeradores.TipoItemProposta.Percentual;

                        if (itens[i].Excluir)
                            repPropostaItem.Deletar(item);
                        else if (item.Codigo > 0)
                            repPropostaItem.Atualizar(item);
                        else
                            repPropostaItem.Inserir(item);
                    }
                }
            }
        }
        #endregion
    }
}