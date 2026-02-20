using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/PoliticaSenha")]
    public class PoliticaSenhaController : BaseController
    {
		#region Construtores

		public PoliticaSenhaController(Conexao conexao) : base(conexao) { }

		#endregion


        int contador = 0;

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPoliticaSenha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha> ListaPoliticasRetornar = new List<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha>();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    var ret = RetornarPoliticaSenhaTMS(unitOfWork, repPoliticaSenha, ListaPoliticasRetornar);
                    return new JsonpResult(ret);
                }
                else
                {
                    var ret = RetornarPoliticaSenhaDemaisServicos(unitOfWork, repPoliticaSenha, ListaPoliticasRetornar);
                    return new JsonpResult(ret);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados da politica de senhas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPoliticaSenhaPorServicoMultiSoftware()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha;

                int? tipoServicoPoliticaSenha = Request.GetNullableIntParam("TipoServicoPoliticaSenha");

                if (tipoServicoPoliticaSenha > 0)
                {
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware((AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware)Enum.Parse(typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), tipoServicoPoliticaSenha.ToString()));
                }
                else
                {
                    //Embarcador e MultiTMS fica gravado com enum 1
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                    else
                        politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(TipoServicoMultisoftware);
                }

                if (politicaSenha == null && (!tipoServicoPoliticaSenha.HasValue || tipoServicoPoliticaSenha <= 0))
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                if (politicaSenha == null)
                    politicaSenha = new Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha();

                if (!politicaSenha.HabilitarPoliticaSenha)
                {
                    politicaSenha.ExigirTrocaSenhaPrimeiroAcesso = false;
                    politicaSenha.ExigirSenhaForte = false;
                    politicaSenha.HabilitarCriptografia = false;
                }

                var dynPotitica = new
                {
                    politicaSenha.ExigirSenhaForte,
                    politicaSenha.ExigirTrocaSenhaPrimeiroAcesso,
                    politicaSenha.HabilitarCriptografia,
                    politicaSenha.HabilitarPoliticaSenha,
                    NumeroMinimoCaracteresSenha = politicaSenha.NumeroMinimoCaracteresSenha > 0 ? politicaSenha.NumeroMinimoCaracteresSenha.ToString() : "",
                    politicaSenha.NaoPermitirAcessosSimultaneos,
                    QuantasSenhasAnterioresNaoRepetir = politicaSenha.QuantasSenhasAnterioresNaoRepetir > 0 ? politicaSenha.QuantasSenhasAnterioresNaoRepetir.ToString() : "",
                    TempoEmMinutosBloqueioUsuario = politicaSenha.TempoEmMinutosBloqueioUsuario > 0 ? politicaSenha.TempoEmMinutosBloqueioUsuario.ToString() : "",
                    PrazoExpiraSenha = politicaSenha.PrazoExpiraSenha > 0 ? politicaSenha.PrazoExpiraSenha.ToString() : "",
                    BloquearUsuarioAposQuantidadeTentativas = politicaSenha.BloquearUsuarioAposQuantidadeTentativas > 0 ? politicaSenha.BloquearUsuarioAposQuantidadeTentativas.ToString() : "",
                    politicaSenhaUsarCPFUsuarioComoSenhaPadraoPrimeiroAcesso = false,// este campo não é mais necessário mas para manter compatibilidade do contrato eu mantive

                    InativarUsuarioAposDiasSemAcessarSistema = politicaSenha.InativarUsuarioAposDiasSemAcessarSistema > 0 ? politicaSenha.InativarUsuarioAposDiasSemAcessarSistema.ToString() : "",
                };

                return new JsonpResult(dynPotitica);
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados da politica de senhas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic RetornarPoliticaSenhaDemaisServicos(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha, List<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha> ListaPoliticasRetornar)
        {
            bool TipoServicoMultiSoftwareVisible = true;
            string TipoServicoMultiSoftwareNome = "Embarcador/TMS";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                TipoServicoMultiSoftwareVisible = false; TipoServicoMultiSoftwareNome = "Multi TMS";
            }

            ListaPoliticasRetornar = repPoliticaSenha.BuscarListaPoliticasPadrao();
            Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha PoliticaSenha;

            if (ListaPoliticasRetornar != null && ListaPoliticasRetornar.Count <= 0)
            {
                PoliticaSenha = repPoliticaSenha.BuscarPoliticaPadrao();
                if (PoliticaSenha != null)
                    ListaPoliticasRetornar.Add(PoliticaSenha);
            }

            if (ListaPoliticasRetornar != null && ListaPoliticasRetornar.Count < 3)
            {
                for (int i = ListaPoliticasRetornar.Count; i < 3; i++)
                {
                    PoliticaSenha = new Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha();
                    PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso = true;
                    PoliticaSenha.ExigirSenhaForte = true;
                    PoliticaSenha.HabilitarCriptografia = true;
                    ListaPoliticasRetornar.Add(PoliticaSenha);
                }
            }

            List<dynamic> lista = (
               from politica in ListaPoliticasRetornar
               select ObterDetalhesPoliticaSenha(politica)
           ).ToList();

            var retorno = new
            {
                TipoServicoMultiSoftwareNome = TipoServicoMultiSoftwareNome,
                TipoServicoMultiSoftwareVisible = TipoServicoMultiSoftwareVisible,
                PoliticasSenha = lista
            };

            return retorno;
        }

        private dynamic RetornarPoliticaSenhaTMS(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha, List<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha> ListaPoliticasRetornar)
        {
            bool TipoServicoMultiSoftwareVisible = true;
            string TipoServicoMultiSoftwareNome = "Embarcador/TMS";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                TipoServicoMultiSoftwareVisible = false; TipoServicoMultiSoftwareNome = "Multi TMS";
            }

            List<Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha> politicasSenhaTMS = repPoliticaSenha.BuscarListaPoliticasPadrao().Where(obj => obj.TipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS).ToList();
            
            if (politicasSenhaTMS.Count <= 0)
            {
                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha PoliticaSenhaPadrao = repPoliticaSenha.BuscarPoliticaPadrao();
                if (PoliticaSenhaPadrao != null)
                    ListaPoliticasRetornar.Add(PoliticaSenhaPadrao);
                else
                    ListaPoliticasRetornar.Add(new Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha());
            }
            else
                ListaPoliticasRetornar.AddRange(politicasSenhaTMS);

            while (ListaPoliticasRetornar.Count < 2)
                ListaPoliticasRetornar.Add(new Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha());
            
            List<dynamic> listaTMS = (
                from politica in ListaPoliticasRetornar
                select ObterDetalhesPoliticaSenha(politica)
            ).ToList();

            var retorno = new
            {
                TipoServicoMultiSoftwareNome = TipoServicoMultiSoftwareNome,
                TipoServicoMultiSoftwareVisible = TipoServicoMultiSoftwareVisible,
                PoliticasSenha = listaTMS
            };

            return retorno;
        }

        private dynamic ObterDetalhesPoliticaSenha(Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha)
        {
            var dynPotitica = new
            {
                politicaSenha.Codigo,
                politicaSenha.ExigirSenhaForte,
                politicaSenha.ExigirTrocaSenhaPrimeiroAcesso,
                politicaSenha.HabilitarCriptografia,
                politicaSenha.HabilitarPoliticaSenha,
                NumeroMinimoCaracteresSenha = politicaSenha.NumeroMinimoCaracteresSenha > 0 ? politicaSenha.NumeroMinimoCaracteresSenha.ToString() : "",
                politicaSenha.NaoPermitirAcessosSimultaneos,
                QuantasSenhasAnterioresNaoRepetir = politicaSenha.QuantasSenhasAnterioresNaoRepetir > 0 ? politicaSenha.QuantasSenhasAnterioresNaoRepetir.ToString() : "",
                TempoEmMinutosBloqueioUsuario = politicaSenha.TempoEmMinutosBloqueioUsuario > 0 ? politicaSenha.TempoEmMinutosBloqueioUsuario.ToString() : "",
                PrazoExpiraSenha = politicaSenha.PrazoExpiraSenha > 0 ? politicaSenha.PrazoExpiraSenha.ToString() : "",
                BloquearUsuarioAposQuantidadeTentativas = politicaSenha.BloquearUsuarioAposQuantidadeTentativas > 0 ? politicaSenha.BloquearUsuarioAposQuantidadeTentativas.ToString() : "",
                UsarCPFUsuarioComoSenhaPadraoPrimeiroAcesso = false, // este campo não é mais necessário mas para manter compatibilidade do contrato eu mantive
                TipoServico = ObterTipoServicoMulti(politicaSenha),
                InativarUsuarioAposDiasSemAcessarSistema = politicaSenha.InativarUsuarioAposDiasSemAcessarSistema > 0 ? politicaSenha.InativarUsuarioAposDiasSemAcessarSistema.ToString() : "",
            };

            return dynPotitica;
        }


        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware ObterTipoServicoMulti(Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha Politica)
        {
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware retorno;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;


            if (Politica.TipoServico == null)
            {
                if (contador == 0) // destinado ao Usuario do MultiEmbarcador e TMS
                    retorno = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
                else if (contador == 1) //destinado ao Transportador
                    retorno = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;
                else // destinado ao Fornecedor Pessoas/Pessoa
                    retorno = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor;
            }
            else
                retorno = (AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware)Politica.TipoServico;

            contador++;
            return retorno;

        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

                dynamic ListaPoliticaSenha = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaPoliticaSalvar"));

                bool exigirTrocaSenhaPrimeiroAcesso = false, habilitarCriptografia = false;

                foreach (dynamic PoliticaSenha in ListaPoliticaSenha)
                {
                    Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenhaSalvar = repPoliticaSenha.BuscarPorCodigo((int)PoliticaSenha.Codigo, true);

                    bool inserir = false;
                    if (politicaSenhaSalvar == null)
                    {
                        politicaSenhaSalvar = new Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha();
                        inserir = true;
                    }

                    politicaSenhaSalvar.Codigo = (int)PoliticaSenha.Codigo;
                    politicaSenhaSalvar.HabilitarPoliticaSenha = (bool)PoliticaSenha.HabilitarPoliticaSenha;
                    politicaSenhaSalvar.ExigirTrocaSenhaPrimeiroAcesso = (bool)PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso;                    
                    //politicaSenhaSalvar.UsarCPFUsuarioComoSenhaPadraoPrimeiroAcesso = (bool)PoliticaSenha.UsarCPFUsuarioComoSenhaPadraoPrimeiroAcesso;
                    politicaSenhaSalvar.NumeroMinimoCaracteresSenha = PoliticaSenha.NumeroMinimoCaracteresSenha != null ? (int)PoliticaSenha.NumeroMinimoCaracteresSenha : 0;
                    politicaSenhaSalvar.PrazoExpiraSenha = PoliticaSenha.PrazoExpiraSenha != null ? (int)PoliticaSenha.PrazoExpiraSenha : 0;
                    politicaSenhaSalvar.BloquearUsuarioAposQuantidadeTentativas = PoliticaSenha.BloquearUsuarioAposQuantidadeTentativas != null ? (int)PoliticaSenha.BloquearUsuarioAposQuantidadeTentativas : 0;
                    politicaSenhaSalvar.ExigirSenhaForte = (bool)PoliticaSenha.ExigirSenhaForte;
                    politicaSenhaSalvar.HabilitarCriptografia = (bool)PoliticaSenha.HabilitarCriptografia;
                    politicaSenhaSalvar.NaoPermitirAcessosSimultaneos = (bool)PoliticaSenha.NaoPermitirAcessosSimultaneos;
                    politicaSenhaSalvar.TempoEmMinutosBloqueioUsuario = PoliticaSenha.TempoEmMinutosBloqueioUsuario != null ? (int)PoliticaSenha.TempoEmMinutosBloqueioUsuario : 0;
                    politicaSenhaSalvar.QuantasSenhasAnterioresNaoRepetir = PoliticaSenha.QuantasSenhasAnterioresNaoRepetir != null ? (int)PoliticaSenha.QuantasSenhasAnterioresNaoRepetir : 0;
                    politicaSenhaSalvar.TipoServico = PoliticaSenha.TipoServico;
                    politicaSenhaSalvar.Usuario = this.Usuario;
                    politicaSenhaSalvar.DataUltimaModificacao = DateTime.Now;
                    politicaSenhaSalvar.InativarUsuarioAposDiasSemAcessarSistema = PoliticaSenha.InativarUsuarioAposDiasSemAcessarSistema != null ? (int)PoliticaSenha.InativarUsuarioAposDiasSemAcessarSistema : 0;

                    if (!politicaSenhaSalvar.HabilitarPoliticaSenha)
                    {
                        exigirTrocaSenhaPrimeiroAcesso = false;
                        habilitarCriptografia = false;
                    }

                    if (!politicaSenhaSalvar.ExigirTrocaSenhaPrimeiroAcesso && exigirTrocaSenhaPrimeiroAcesso)//seta todos os usuário para trocar senha no proximo acesso.
                        repUsuario.SetarTodosParaAlterarSenhaProximoLogin();

                    if (politicaSenhaSalvar.ExigirTrocaSenhaPrimeiroAcesso && !exigirTrocaSenhaPrimeiroAcesso)
                        repUsuario.RemoverTodosParaAlterarSenhaProximoLogin();

                    if (habilitarCriptografia && !politicaSenhaSalvar.HabilitarCriptografia)
                    {
                        List<Dominio.Entidades.Usuario> usuarios = repUsuario.BuscarPorTipoAcesso(Dominio.Enumeradores.TipoAcesso.Embarcador, "");
                        for (int i = 0; i < usuarios.Count; i++)
                        {
                            Dominio.Entidades.Usuario usuario = usuarios[i];
                            if (!usuario.SenhaCriptografada)
                            {
                                usuario.Senha = Servicos.Criptografia.GerarHashSHA256(usuario.Senha ?? string.Empty);
                                usuario.SenhaCriptografada = true;
                                repUsuario.Atualizar(usuario);
                            }
                        }
                    }

                    if (inserir)
                        repPoliticaSenha.Inserir(politicaSenhaSalvar, Auditado);
                    else
                        repPoliticaSenha.Atualizar(politicaSenhaSalvar, Auditado);

                    unitOfWork.CommitChanges();
                }


                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao Salvar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
