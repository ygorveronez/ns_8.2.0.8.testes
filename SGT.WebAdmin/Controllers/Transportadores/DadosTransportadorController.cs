using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "BuscarPorCodigo" }, "Transportadores/DadosTransportador")]
    public class DadosTransportadorController : BaseController
    {
		#region Construtores

		public DadosTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                PreencherTransportador(empresa, unitOfWork, permissoesPersonalizadas);

                empresa.DataAtualizacao = DateTime.Now;
                empresa.UsuarioAtualizacao = Auditado.Usuario;

                empresa.Email = Request.Params("Email").ToLower();
                empresa.EmailAdministrativo = Request.Params("EmailAdministrativo").ToLower();
                empresa.EmailContador = Request.Params("EmailContador").ToLower();

                bool enviarEmail = Request.GetBoolParam("EnviarEmail");
                bool enviarEmailAdministrativo = Request.GetBoolParam("EnviarEmailAdministrativo");
                bool enviarEmailContador = Request.GetBoolParam("EnviarEmailContador");
                empresa.OptanteSimplesNacionalComExcessoReceitaBruta = Request.GetBoolParam("OptanteSimplesNacionalComExcessoReceitaBruta");
                empresa.StatusEmail = enviarEmail ? "A" : "I";
                empresa.StatusEmailAdministrativo = enviarEmailAdministrativo ? "A" : "I";
                empresa.StatusEmailContador = enviarEmailContador ? "A" : "I";
                empresa.StatusEmissao = "S";

                empresa.Integrado = false;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    if (empresa.EmpresaPai != null && empresa.TipoAmbiente != empresa.EmpresaPai.TipoAmbiente)
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TipoDeAmbienteInformadoDiferenteDoAmbienteGeral);

                if (string.IsNullOrWhiteSpace(empresa.CEP) && !configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.FavorInformeCEPDaEmpresa);

                Dominio.Entidades.Empresa empresaExiste = empresa.Tipo == "E" ? repEmpresa.BuscarPorCodigoIntegracao(empresa.CodigoIntegracao) : repEmpresa.BuscarPorCNPJ(empresa.CNPJ);

                repEmpresa.Atualizar(empresa, Auditado);
                
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Usuario?.Empresa?.Codigo ?? 0;

                if(codigo == 0)
                    throw new ControllerException("Usuário não tem transportador vinculado!");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);
                var dynEmpresa = BuscarDynEmpresa(empresa, unitOfWork);

                return new JsonpResult(dynEmpresa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            empresa.RazaoSocial = Request.Params("RazaoSocial");
            empresa.NomeFantasia = Request.Params("NomeFantasia");
            empresa.InscricaoEstadual = Utilidades.String.OnlyNumbers(Request.Params("InscricaoEstadual"));
            empresa.InscricaoMunicipal = Request.Params("InscricaoMunicipal");
            empresa.Suframa = Request.Params("Suframa");
            empresa.Setor = Request.Params("Setor");
            empresa.CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
            empresa.Endereco = Request.Params("Endereco");
            empresa.Complemento = Request.Params("Complemento");
            empresa.Numero = Request.Params("Numero");
            empresa.Bairro = Request.Params("Bairro");
            empresa.Telefone = Request.Params("Telefone");
            empresa.Localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Localidade")));
            empresa.Contato = Request.Params("Contato");
            empresa.TelefoneContato = Request.Params("TelefoneContato");
            empresa.NomeContador = Request.Params("NomeContador");
            empresa.CRCContador = Request.Params("CRCContador");
            empresa.TelefoneContador = Request.Params("TelefoneContador");
            empresa.OptanteSimplesNacional = bool.Parse(Request.Params("OptanteSimplesNacional"));
            empresa.RegimeEspecial = (Dominio.Enumeradores.RegimeEspecialEmpresa)int.Parse(Request.Params("RegimeEspecial"));
            empresa.RegistroANTT = Request.Params("RegistroANTT");
            empresa.FusoHorario = Request.Params("FusoHorario");

            if (Request.GetStringParam("Status") == "A")
                empresa.Status = Request.GetStringParam("Status");
            else
            {
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Transportador_PermiteInativarTransportador))
                    empresa.Status = Request.GetStringParam("Status");
                else
                    throw new ControllerException("Você não possui Permissão para Inativar um Transportador");
            }

            empresa.RegimenTributario = Request.GetEnumParam<RegimenTributacao>("RegimenTributario");

            DateTime data;
            if (DateTime.TryParseExact(Request.Params("DataInicioAtividade"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                empresa.DataInicioAtividade = data;
            else
                empresa.DataInicioAtividade = null;
        }

        private dynamic BuscarDynEmpresa(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            var dynEmpresa = new
            {
                empresa.Codigo,
                empresa.RazaoSocial,
                empresa.NomeFantasia,
                TipoEmpresa = !string.IsNullOrWhiteSpace(empresa.Tipo) ? empresa.Tipo : "J",
                empresa.InscricaoEstadual,
                empresa.InscricaoMunicipal,
                empresa.CNAE,
                empresa.Suframa,
                empresa.Setor,
                CEP = !string.IsNullOrWhiteSpace(empresa.CEP) ? string.Format(@"{0:00\.000-000}", int.Parse(Utilidades.String.OnlyNumbers(empresa.CEP))) : string.Empty,
                empresa.Endereco,
                empresa.Complemento,
                empresa.Numero,
                empresa.Bairro,
                empresa.Telefone,
                Localidade = new { empresa.Localidade.Codigo, empresa.Localidade.Descricao },
                empresa.Contato,
                empresa.TelefoneContato,
                empresa.NomeContador,
                empresa.CRCContador,
                empresa.TransportadorFerroviario,
                Contador = new { Codigo = empresa.Contador != null ? empresa.Contador.CPF_CNPJ : 0, Descricao = empresa.Contador != null ? empresa.Contador.Nome : "" },
                empresa.TelefoneContador,
                empresa.OptanteSimplesNacional,
                empresa.OptanteSimplesNacionalComExcessoReceitaBruta,
                EnviarEmail = empresa.StatusEmail == "A" ? true : false,
                EnviarEmailAdministrativo = empresa.StatusEmailAdministrativo == "A" ? true : false,
                EnviarEmailContador = empresa.StatusEmailContador == "A" ? true : false,
                empresa.RegistroANTT,
                empresa.RegimenTributario,
                empresa.FusoHorario,
                empresa.RegimeEspecial,
                empresa.RegimeTributarioCTe,
                empresa.Status,
                empresa.Email,
                empresa.EmailAdministrativo,
                empresa.EmailContador,
                empresa.EmailEnvioCanhoto,                
            };

            return dynEmpresa;
        }

        #endregion
    }
}
