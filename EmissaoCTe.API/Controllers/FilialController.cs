using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FilialController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("filial.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];

                var repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial()
                {
                    Ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo,
                    Descricao = Request.Params["Descricao"]
                };

                List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFiliais = repFilial.Consultar(filtrosPesquisa, "Codigo", "Desc", 0, 50);
                int countFiliais = repFilial.ContarConsulta(filtrosPesquisa);

                var retorno = from obj in listaFiliais select new { obj.Codigo, obj.Descricao };

                return Json(retorno, true, null, new string[] { "Codigo", "Descricao|90" }, countFiliais);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as Filiais.");
            }
        }

        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                if (!int.TryParse(Request.Params["Codigo"], out codigo))
                    return Json<bool>(false, false, "Código inválido!");

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigo);

                if (filial != null)
                {
                    Dominio.Entidades.Cliente cliente = null;
                    if (!string.IsNullOrWhiteSpace(filial.CNPJ))
                        cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(filial.CNPJ));

                    var retorno = new
                    {
                        filial.Codigo,
                        filial.Descricao,
                        filial.CompraValePedagio,
                        RazaoCliente = cliente != null ? cliente.Nome : string.Empty,
                        CNPJCliente = cliente != null ? cliente.CPF_CNPJ_Formatado : "0",
                        filial.FornecedorValePedagio,
                        filial.UsuarioValePedagio,
                        filial.SenhaValePedagio,
                        filial.IntegradoraValePedagio,
                        filial.URLIntegracaoRest,
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Filial não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das Filiais. Atualize a página e tente novamente.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Enumeradores.OpcaoSimNao compraValePedagio;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["CompraValePedagio"], out compraValePedagio);
                
                Dominio.Enumeradores.IntegradoraValePedagio integradoraValePedagio = Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma;               
                
                if (compraValePedagio == Dominio.Enumeradores.OpcaoSimNao.Sim && Enum.TryParse(Request.Params["IntegradoraValePedagio"], out Dominio.Enumeradores.IntegradoraValePedagio integradoraParsed))
                {
                    integradoraValePedagio = integradoraParsed;
                }

                string descricao = Request.Params["Descricao"];
                string fornecedorValePedagio = Request.Params["FornecedorValePedagio"];
                string usuarioValePedagio = Request.Params["UsuarioValePedagio"];
                string senhaValePedagio = Request.Params["SenhaValePedagio"];
                string cnpjCliente = Utilidades.String.OnlyNumbers(Request.Params["Cliente"]);
                string urlIntegracaoRest = Request.Params["UrlIntegracaoRest"];

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Cliente cliente = null;
                if (!string.IsNullOrWhiteSpace(cnpjCliente))
                    cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjCliente));

                Dominio.Entidades.Embarcador.Filiais.Filial filialValidacao = repFilial.BuscarPorCNPJ(cnpjCliente);
                if (filialValidacao != null)
                {
                    if (codigo == 0 || codigo != filialValidacao.Codigo)
                        return Json<bool>(false, false, "Já existe filial cadastrada para o CNPJ " + cnpjCliente);
                }

                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    filial = repFilial.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inserção negada.");

                    filial = new Dominio.Entidades.Embarcador.Filiais.Filial();
                }

                filial.CNPJ = cliente != null ? cliente.CPF_CNPJ_SemFormato : cnpjCliente;
                filial.Descricao = descricao;
                filial.CompraValePedagio = compraValePedagio;
                filial.CodigoFilialEmbarcador = "0";
                filial.ControlaExpedicao = false;
                filial.ExibirDescricaoRemetente = false;
                filial.Localidade = cliente != null ? cliente.Localidade : this.EmpresaUsuario.Localidade;
                filial.NaoAdicionarValorDescarga = false;
                filial.NumeroUnidadeImpressao = 0;
                filial.TipoFilial = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilial.Filial;
                filial.Ativo = true;
                filial.FornecedorValePedagio = fornecedorValePedagio;
                filial.UsuarioValePedagio = usuarioValePedagio;
                filial.SenhaValePedagio = senhaValePedagio;
                filial.IntegradoraValePedagio = integradoraValePedagio;
                filial.URLIntegracaoRest = urlIntegracaoRest;

                if (filial.Codigo > 0)
                    repFilial.Atualizar(filial);
                else
                    repFilial.Inserir(filial);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Filial");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

    }
}