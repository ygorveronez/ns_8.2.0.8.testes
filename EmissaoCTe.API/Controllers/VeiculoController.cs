using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class VeiculoController : ApiController
    {
        #region Variaveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("veiculos.aspx") select obj).FirstOrDefault();
        }

        private Dominio.ObjetosDeValor.PaginaUsuario PermissaoVeiculosVinculados()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("veiculosvinculados.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Metodos Publicos

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorPlacaSimples()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string placa = Request.Params["Placa"];
                string tipoVeiculo = Request.Params["TipoVeiculo"];
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, placa);
                if (veiculo == null)
                {
                    return Json<bool>(true, false, "Nenhum veículo com placa " + placa + " encontrado.");
                }
                else if (!string.IsNullOrEmpty(tipoVeiculo) && veiculo.TipoVeiculo != tipoVeiculo)
                {
                    return Json<bool>(true, false, "O veículo deve ser do tipo " + (tipoVeiculo.Equals("0") ? "Tração" : "Reboque") + ".");
                }
                else
                {
                    var retorno = new
                    {
                        veiculo.Codigo,
                        veiculo.Placa,
                        veiculo.Renavam,
                        DescricaoTipoVeiculo = veiculo.DescricaoTipoVeiculo,
                        VeiculosVinculados = veiculo.VeiculosVinculados != null ?
                            (from obj in veiculo.VeiculosVinculados
                             where obj.Ativo
                             select new Dominio.ObjetosDeValor.Veiculo()
                             {
                                 Codigo = obj.Codigo,
                                 Placa = obj.Placa,
                                 Renavam = obj.Renavam,
                                 CapacidadeKG = obj.CapacidadeKG,
                                 CapacidadeM3 = obj.CapacidadeM3,
                                 DescricaoTipo = obj.DescricaoTipo,
                                 DescricaoTipoCarroceria = obj.DescricaoTipoCarroceria,
                                 DescricaoTipoCombustivel = obj.DescricaoTipoCombustivel,
                                 DescricaoTipoRodado = obj.DescricaoTipoRodado,
                                 DescricaoTipoVeiculo = obj.DescricaoTipoVeiculo,
                                 Tara = obj.Tara,
                                 UF = obj.Estado.Sigla,
                                 Excluir = false
                             }).ToList() : null
                    };
                    return Json(retorno, true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorPlaca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string placa = Request.Params["Placa"];
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, placa);
                if (veiculo == null)
                {
                    return Json<bool>(true, false, "Nenhum veículo com placa " + placa + " encontrado.");
                }
                else
                {
                    List<Dominio.Entidades.Veiculo> veiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                    List<Dominio.Entidades.Veiculo> pais = repVeiculo.BuscarVeiculoPai(this.EmpresaUsuario.Codigo, placa);

                    Dominio.Entidades.Veiculo pai = null;
                    if (pais.Count == 1)
                    {
                        pai = pais[0];
                        veiculosVinculados = pai.VeiculosVinculados != null ? (from obj in pai.VeiculosVinculados where obj.Codigo != veiculo.Codigo select obj).ToList() : new List<Dominio.Entidades.Veiculo>();
                        veiculosVinculados.Add(pai);
                    }
                    else
                    {
                        if (pais.Count == 0)
                            veiculosVinculados = veiculo.VeiculosVinculados != null ? veiculo.VeiculosVinculados.ToList() : new List<Dominio.Entidades.Veiculo>();
                    }


                    var proprietario = new object();

                    if (veiculo.Tipo.Equals("T") && veiculo.Proprietario != null && veiculo.RNTRC > 0 && !string.IsNullOrWhiteSpace(veiculo.ObservacaoCTe))
                        proprietario = new { NomeProprietario = veiculo.Proprietario.Nome, CPFCNPJProprietario = veiculo.Proprietario.CPF_CNPJ_Formatado, RNTRC = string.Format("{0:00000000}", veiculo.RNTRC), veiculo.ObservacaoCTe };
                    else
                        proprietario = (from obj in veiculosVinculados where obj.Tipo.Equals("T") && obj.Proprietario != null && obj.RNTRC > 0 && !string.IsNullOrWhiteSpace(obj.ObservacaoCTe) select new { NomeProprietario = obj.Proprietario.Nome, CPFCNPJProprietario = obj.Proprietario.CPF_CNPJ_Formatado, RNTRC = string.Format("{0:00000000}", obj.RNTRC), obj.ObservacaoCTe }).FirstOrDefault();

                    //Repositorio.VeiculoMotoristas repVeiculoMotoristas = new Repositorio.VeiculoMotoristas(unitOfWork);
                    //List<Dominio.Entidades.VeiculoMotoristas> veiculoMotoristas = repVeiculoMotoristas.BuscarPorVeiculo(veiculo.Codigo);

                    var retorno = new
                    {
                        veiculo.Codigo,
                        veiculo.CapacidadeKG,
                        veiculo.CapacidadeM3,
                        NumeroCIOT = veiculo.CIOT,
                        UF = veiculo.Estado.Sigla,
                        veiculo.Placa,
                        veiculo.Renavam,
                        veiculo.Tara,
                        veiculo.DescricaoMarca,
                        veiculo.DescricaoTipo,
                        veiculo.DescricaoTipoCarroceria,
                        veiculo.DescricaoTipoCombustivel,
                        veiculo.DescricaoTipoVeiculo,
                        veiculo.DescricaoTipoRodado,
                        CodigoMotorista = pai != null && pai.Motoristas != null && pai.Motoristas.Count > 0 ? pai.Motoristas.FirstOrDefault().Motorista?.Codigo : veiculo.Motoristas != null && veiculo.Motoristas.Count > 0 ? veiculo.Motoristas.FirstOrDefault().Motorista?.Codigo : veiculosVinculados.FirstOrDefault() != null && veiculosVinculados.FirstOrDefault().Motoristas != null && veiculosVinculados.FirstOrDefault().Motoristas.Count > 0 ? veiculosVinculados.FirstOrDefault().Motoristas.FirstOrDefault().Motorista?.Codigo : 0,
                        NomeMotorista = pai != null && pai.Motoristas != null && pai.Motoristas.Count > 0 ? pai.Motoristas.FirstOrDefault().Nome : veiculo.Motoristas != null && veiculo.Motoristas.Count > 0 ? veiculo.Motoristas.FirstOrDefault().Nome : veiculosVinculados.FirstOrDefault() != null && veiculosVinculados.FirstOrDefault().Motoristas != null && veiculosVinculados.FirstOrDefault().Motoristas.Count > 0 ? veiculosVinculados.FirstOrDefault().Motoristas.FirstOrDefault().Nome : string.Empty,
                        CPFMotorista = pai != null && pai.Motoristas != null && pai.Motoristas.Count > 0 ? pai.Motoristas.FirstOrDefault().CPF : veiculo.Motoristas != null && veiculo.Motoristas.Count > 0 ? veiculo.Motoristas.FirstOrDefault().CPF : veiculosVinculados.FirstOrDefault() != null && veiculosVinculados.FirstOrDefault().Motoristas != null && veiculosVinculados.FirstOrDefault().Motoristas.Count > 0 ? veiculosVinculados.FirstOrDefault().Motoristas.FirstOrDefault().CPF : string.Empty,
                        veiculo.KilometragemAtual,
                        VeiculosVinculados = (from obj in veiculosVinculados
                                              where obj.Ativo
                                              select new Dominio.ObjetosDeValor.Veiculo()
                                              {
                                                  Codigo = obj.Codigo,
                                                  Placa = obj.Placa,
                                                  Renavam = obj.Renavam,
                                                  CapacidadeKG = obj.CapacidadeKG,
                                                  CapacidadeM3 = obj.CapacidadeM3,
                                                  DescricaoTipo = obj.DescricaoTipo,
                                                  DescricaoTipoCarroceria = obj.DescricaoTipoCarroceria,
                                                  DescricaoTipoCombustivel = obj.DescricaoTipoCombustivel,
                                                  DescricaoTipoRodado = obj.DescricaoTipoRodado,
                                                  DescricaoTipoVeiculo = obj.DescricaoTipoVeiculo,
                                                  Tara = obj.Tara,
                                                  UF = obj.Estado.Sigla,
                                                  TipoDoVeiculo = obj.TipoDoVeiculo != null ? obj.TipoDoVeiculo.Descricao : string.Empty,
                                                  Excluir = false
                                              }).ToList(),
                        VeiculosPai = (from obj in pais
                                       select new Dominio.ObjetosDeValor.Veiculo()
                                       {
                                           Codigo = obj.Codigo,
                                           Placa = obj.Placa,
                                           Renavam = obj.Renavam,
                                           CapacidadeKG = obj.CapacidadeKG,
                                           CapacidadeM3 = obj.CapacidadeM3,
                                           DescricaoTipo = obj.DescricaoTipo,
                                           DescricaoTipoCarroceria = obj.DescricaoTipoCarroceria,
                                           DescricaoTipoCombustivel = obj.DescricaoTipoCombustivel,
                                           DescricaoTipoRodado = obj.DescricaoTipoRodado,
                                           DescricaoTipoVeiculo = obj.DescricaoTipoVeiculo,
                                           Tara = obj.Tara,
                                           UF = obj.Estado.Sigla,
                                           TipoDoVeiculo = obj.TipoDoVeiculo != null ? obj.TipoDoVeiculo.Descricao : string.Empty,
                                           Excluir = false
                                       }).ToList(),
                        Proprietario = proprietario,
                        TipoDoVeiculo = veiculo.TipoDoVeiculo != null ? veiculo.TipoDoVeiculo.Descricao : string.Empty,
                        Motoristas = (from obj in veiculo.VeiculoMotoristas
                                      select new Dominio.ObjetosDeValor.MotoristaMDFe()
                                      {
                                          Codigo = obj.Codigo,
                                          CPF = obj.Motorista.CPF,
                                          Nome = obj.Motorista.Nome,
                                          Excluir = false
                                      }).ToList()
                    };
                    return Json(retorno, true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //public ActionResult BuscarPorPlaca()
        //{
        //    try
        //    {
        //        string placa = Request.Params["Placa"];
        //        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(Conexao.StringConexao);
        //        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, placa);
        //        if (veiculo == null)
        //        {
        //            return Json<bool>(true, false, "Nenhum veículo encontrado.");
        //        }
        //        else
        //        {
        //            List<Dominio.Entidades.Veiculo> veiculosVinculados = new List<Dominio.Entidades.Veiculo>();
        //            var pai = repVeiculo.BuscarVeiculoPai(this.EmpresaUsuario.Codigo, placa);
        //            if (pai != null)
        //            {
        //                veiculosVinculados = pai.VeiculosVinculados != null ? (from obj in pai.VeiculosVinculados where obj.Codigo != veiculo.Codigo select obj).ToList() : new List<Dominio.Entidades.Veiculo>();
        //                veiculosVinculados.Add(pai);
        //            }
        //            else
        //            {
        //                veiculosVinculados = veiculo.VeiculosVinculados != null ? veiculo.VeiculosVinculados.ToList() : new List<Dominio.Entidades.Veiculo>();
        //            }


        //            var proprietario = new object();

        //            if (veiculo.Tipo.Equals("T") && veiculo.Proprietario != null && veiculo.RNTRC > 0 && !string.IsNullOrWhiteSpace(veiculo.ObservacaoCTe))
        //                proprietario = new { NomeProprietario = veiculo.Proprietario.Nome, CPFCNPJProprietario = veiculo.Proprietario.CPF_CNPJ_Formatado, RNTRC = string.Format("{0:00000000}", veiculo.RNTRC), veiculo.ObservacaoCTe };
        //            else
        //                proprietario = (from obj in veiculosVinculados where obj.Tipo.Equals("T") && obj.Proprietario != null && obj.RNTRC > 0 && !string.IsNullOrWhiteSpace(obj.ObservacaoCTe) select new { NomeProprietario = obj.Proprietario.Nome, CPFCNPJProprietario = obj.Proprietario.CPF_CNPJ_Formatado, RNTRC = string.Format("{0:00000000}", obj.RNTRC), obj.ObservacaoCTe }).FirstOrDefault();

        //            var retorno = new
        //            {
        //                veiculo.Codigo,
        //                veiculo.CapacidadeKG,
        //                veiculo.CapacidadeM3,
        //                UF = veiculo.Estado.Sigla,
        //                veiculo.Placa,
        //                veiculo.Renavam,
        //                veiculo.Tara,
        //                veiculo.DescricaoTipo,
        //                veiculo.DescricaoTipoCarroceria,
        //                veiculo.DescricaoTipoCombustivel,
        //                veiculo.DescricaoTipoVeiculo,
        //                veiculo.DescricaoTipoRodado,
        //                CodigoMotorista = pai != null && pai.Motorista != null ? pai.Motorista.Codigo : 0,
        //                NomeMotorista = pai != null ? pai.NomeMotorista : veiculo.NomeMotorista,
        //                CPFMotorista = pai != null ? pai.CPFMotorista : veiculo.CPFMotorista,
        //                veiculo.KilometragemAtual,
        //                VeiculosVinculados = (from obj in veiculosVinculados
        //                                      where obj.Status == "A"
        //                                      select new Dominio.ObjetosDeValor.Veiculo()
        //                                      {
        //                                          Codigo = obj.Codigo,
        //                                          Placa = obj.Placa,
        //                                          Renavam = obj.Renavam,
        //                                          CapacidadeKG = obj.CapacidadeKG,
        //                                          CapacidadeM3 = obj.CapacidadeM3,
        //                                          DescricaoTipo = obj.DescricaoTipo,
        //                                          DescricaoTipoCarroceria = obj.DescricaoTipoCarroceria,
        //                                          DescricaoTipoCombustivel = obj.DescricaoTipoCombustivel,
        //                                          DescricaoTipoRodado = obj.DescricaoTipoRodado,
        //                                          DescricaoTipoVeiculo = obj.DescricaoTipoVeiculo,
        //                                          Tara = obj.Tara,
        //                                          UF = obj.Estado.Sigla,
        //                                          Excluir = false
        //                                      }).ToList(),
        //                Proprietario = proprietario
        //            };
        //            return Json(retorno, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações do veículo.");
        //    }
        //}

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorPlacaETipoVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string placa = Request.Params["Placa"];
                string tipo = Request.Params["Tipo"];

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaETipoVeiculo(this.EmpresaUsuario.Codigo, "", placa, tipo);

                if (veiculo == null)
                    return Json<bool>(true, false, "Nenhum veículo com placa " + placa + " encontrado.");

                Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaPrincipal = veiculo.Motoristas != null && veiculo.Motoristas.Count() > 0 ? veiculo.Motoristas.Where(o => o.Principal).FirstOrDefault() : null;

                var retorno = new
                {
                    Veiculo = new Dominio.ObjetosDeValor.VeiculoMDFe()
                    {
                        CapacidadeKG = veiculo.CapacidadeKG,
                        CapacidadeM3 = veiculo.CapacidadeM3,
                        Excluir = false,
                        Placa = veiculo.Placa,
                        RNTRC = veiculo.Tipo == "T" && veiculo.Proprietario != null ? veiculo.RNTRC.ToString("D8") : string.Empty,
                        Tara = veiculo.Tara,
                        CPFCNPJ = veiculo.Tipo == "T" && veiculo.Proprietario != null ? veiculo.Proprietario.CPF_CNPJ_SemFormato : string.Empty,
                        IE = veiculo.Tipo == "T" && veiculo.Proprietario != null ? veiculo.Proprietario.IE_RG : string.Empty,
                        Nome = veiculo.Tipo == "T" && veiculo.Proprietario != null ? veiculo.Proprietario.Nome : string.Empty,
                        TipoCarroceria = veiculo.TipoCarroceria,
                        TipoProprietario = veiculo.Tipo == "T" && veiculo.Proprietario != null ? veiculo.TipoProprietario.ToString("D") : string.Empty,
                        TipoRodado = veiculo.TipoRodado,
                        UF = veiculo.Estado.Sigla,
                        UFProprietario = veiculo.Tipo == "T" && veiculo.Proprietario != null ? veiculo.Proprietario.Localidade.Estado.Sigla : string.Empty,
                        RENAVAM = veiculo.Renavam
                    },
                    Reboques = veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count() > 0 ? (from obj in veiculo.VeiculosVinculados
                                                                                                               where obj.Ativo
                                                                                                               select new Dominio.ObjetosDeValor.VeiculoMDFe()
                                                                                                               {
                                                                                                                   CapacidadeKG = obj.CapacidadeKG,
                                                                                                                   CapacidadeM3 = obj.CapacidadeM3,
                                                                                                                   CPFCNPJ = obj.Tipo == "T" && obj.Proprietario != null ? obj.Proprietario.CPF_CNPJ_SemFormato : string.Empty,
                                                                                                                   Excluir = false,
                                                                                                                   IE = obj.Tipo == "T" && obj.Proprietario != null ? obj.Proprietario.IE_RG : string.Empty,
                                                                                                                   Nome = obj.Tipo == "T" && obj.Proprietario != null ? obj.Proprietario.Nome : string.Empty,
                                                                                                                   Placa = obj.Placa,
                                                                                                                   RNTRC = obj.Tipo == "T" && obj.Proprietario != null ? obj.RNTRC.ToString("D8") : string.Empty,
                                                                                                                   Tara = obj.Tara,
                                                                                                                   TipoCarroceria = obj.TipoCarroceria,
                                                                                                                   TipoProprietario = obj.Tipo == "T" && obj.Proprietario != null ? obj.TipoProprietario.ToString("D") : string.Empty,
                                                                                                                   TipoRodado = obj.TipoRodado,
                                                                                                                   UF = obj.Estado.Sigla,
                                                                                                                   UFProprietario = obj.Tipo == "T" && obj.Proprietario != null ? obj.Proprietario.Localidade.Estado.Sigla : string.Empty,
                                                                                                                   RENAVAM = veiculo.Renavam
                                                                                                               }) : null,
                    NomeMotorista = motoristaPrincipal != null ? motoristaPrincipal.Nome : string.Empty,
                    CPFMotorista = motoristaPrincipal != null ? motoristaPrincipal.CPF : string.Empty,
                    Motoristas = (from obj in veiculo.VeiculoMotoristas
                                  select new Dominio.ObjetosDeValor.MotoristaMDFe()
                                  {
                                      Codigo = obj.Codigo,
                                      CPF = obj.Motorista.CPF,
                                      Nome = obj.Motorista.Nome,
                                      Excluir = false
                                  }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarKilometragemPorPlaca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string placa = Request.Params["Placa"];
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, placa);
                if (veiculo == null)
                {
                    return Json<bool>(true, false, "Nenhum veículo com placa " + placa + " encontrado.");
                }
                else
                {
                    return Json(new { veiculo.KilometragemAtual }, true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter a kilometragem do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarParaVincular()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string placa = Request.Params["Placa"];
                int codigoVeiculo = 0;
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, placa);
                int countVinculos = repVeiculo.ContarVeiculoVinculado(this.EmpresaUsuario.Codigo, placa, codigoVeiculo);
                if (veiculo == null)
                {
                    return Json<bool>(true, false, "Nenhum veículo com placa " + placa + " encontrado.");
                }
                else
                {
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaPrincipal = veiculo.Motoristas != null && veiculo.Motoristas.Count() > 0 ? veiculo.Motoristas.Where(o => o.Principal).FirstOrDefault() : null;

                    return Json(new
                    {
                        Vinculos = countVinculos,
                        veiculo.Codigo,
                        veiculo.CapacidadeKG,
                        veiculo.CapacidadeM3,
                        UF = veiculo.Estado.Sigla,
                        veiculo.Placa,
                        veiculo.Renavam,
                        veiculo.Tara,
                        veiculo.DescricaoTipo,
                        veiculo.DescricaoTipoCarroceria,
                        veiculo.DescricaoTipoCombustivel,
                        veiculo.DescricaoTipoVeiculo,
                        veiculo.DescricaoTipoRodado,
                        NomeMotorista = motoristaPrincipal?.Nome ?? string.Empty,
                        CPFMotorista = motoristaPrincipal?.CPF ?? string.Empty,
                        VeiculosVinculados = veiculo.VeiculosVinculados != null ? (from obj in veiculo.VeiculosVinculados
                                                                                   where obj.Ativo
                                                                                   select new Dominio.ObjetosDeValor.Veiculo()
                                                                                   {
                                                                                       Codigo = obj.Codigo,
                                                                                       Placa = obj.Placa,
                                                                                       Renavam = obj.Renavam,
                                                                                       CapacidadeKG = obj.CapacidadeKG,
                                                                                       CapacidadeM3 = obj.CapacidadeM3,
                                                                                       DescricaoTipo = obj.DescricaoTipo,
                                                                                       DescricaoTipoCarroceria = obj.DescricaoTipoCarroceria,
                                                                                       DescricaoTipoCombustivel = obj.DescricaoTipoCombustivel,
                                                                                       DescricaoTipoRodado = obj.DescricaoTipoRodado,
                                                                                       DescricaoTipoVeiculo = obj.DescricaoTipoVeiculo,
                                                                                       Tara = obj.Tara,
                                                                                       UF = obj.Estado.Sigla,
                                                                                       Excluir = false
                                                                                   }).ToList() : null
                    }, true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações de veículos do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarVeiculoVinculado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.PermissaoVeiculosVinculados() == null || this.PermissaoVeiculosVinculados().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão de veículo vinculado negada!");

                int codigoPai, codigoFilho = 0;
                int.TryParse(Request.Params["CodigoVeiculoPai"], out codigoPai);
                int.TryParse(Request.Params["CodigoVeiculoFilho"], out codigoFilho);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veicPai = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPai);

                if (veicPai == null)
                    return Json<bool>(false, false, "O veículo pai não foi encontrado.");

                if (veicPai.VeiculosVinculados == null)
                    veicPai.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                Dominio.Entidades.Veiculo veicFilho = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoFilho);

                if (veicFilho != null)
                {
                    if (veicFilho.Codigo == veicPai.Codigo)
                        return Json<bool>(false, false, string.Concat("O veículo não pode ser vinculado a ele mesmo."));

                    if (veicFilho.VeiculosVinculados.Count > 0)
                        return Json<bool>(false, false, string.Concat("O veículo de placa '", veicFilho.Placa, "' já possui outros veículos vinculados a ele, não sendo possível vinculá-lo a outros veículos."));

                    List<string> placasVinculadas = repVeiculo.ObterVinculosExistentes(this.EmpresaUsuario.Codigo, veicPai.Codigo, veicFilho.Codigo);

                    if (placasVinculadas.Count() > 0 && !this.EmpresaUsuario.Configuracao.PermiteVincularMesmaPlacaOutrosVeiculos)
                        return Json<bool>(false, false, "Não é possível vincular este veículo pois ele já está vinculado aos seguintes veículos: " + string.Join(", ", placasVinculadas) + ".");

                    Dominio.Entidades.Veiculo veiculoPai = repVeiculo.BuscarVeiculoPai(veicPai.Codigo);
                    if (veiculoPai != null)
                        return Json<bool>(false, false, string.Concat("O veículo de placa '", veicPai.Placa, "' já possui vínculo ao veículo placa '", veiculoPai.Placa, "'."));

                    if (!veicPai.VeiculosVinculados.Contains(veicFilho))
                    {
                        veicPai.VeiculosVinculados.Add(veicFilho);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veicPai, null, "Adicionou o vínculo com " + veicFilho.Placa, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veicFilho, null, "Vinculou ao veículo " + veicPai.Placa, unitOfWork);
                    }

                }

                repVeiculo.Atualizar(veicPai);

                try
                {
                    Servicos.Veiculo svcVeiculo = new Servicos.Veiculo(unitOfWork);
                    svcVeiculo.SalvarViculosMatrizFilial(veicPai, unitOfWork, Auditado);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return Json<bool>(false, false, "Ocorreu uma falha ao salvar veículo nas filiais.");
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o vínculo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ExcluirVeiculoVinculado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.PermissaoVeiculosVinculados() == null || this.PermissaoVeiculosVinculados().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para exclusão de veículo vinculado negada!");

                int codigoPai, codigoFilho = 0;
                int.TryParse(Request.Params["CodigoVeiculoPai"], out codigoPai);
                int.TryParse(Request.Params["CodigoVeiculoFilho"], out codigoFilho);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                var veicPai = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPai);
                var veicFilho = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoFilho);
                if (veicFilho != null)
                {
                    if (veicPai.VeiculosVinculados != null)
                    {
                        veicPai.VeiculosVinculados.Remove(veicFilho);
                        repVeiculo.Atualizar(veicPai);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veicPai, null, "Removeu o vínculo com " + veicFilho.Placa, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veicFilho, null, "Desvinculou do veículo " + veicPai.Placa, unitOfWork);

                        try
                        {
                            Servicos.Veiculo svcVeiculo = new Servicos.Veiculo(unitOfWork);
                            svcVeiculo.SalvarViculosMatrizFilial(veicPai, unitOfWork, Auditado);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            return Json<bool>(false, false, "Ocorreu uma falha ao salvar veículo nas filiais.");
                        }

                        return Json<bool>(true, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, "Nenhum vínculo encontrado para o veículo selecionado.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Veículo vinculado não encontrado.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao excluir o vínculo.");
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
                string numeroMotor = Request.Params["NumeroMotor"];
                string contrato = Request.Params["Contrato"];
                string chassi = Request.Params["Chassi"];
                string placa = (Request.Params["Placa"] ?? string.Empty).ToUpper();
                string renavam = Request.Params["Renavam"];
                string tipoVeiculo = Request.Params["TipoVeiculo"];
                string tipoRodado = Request.Params["TipoRodado"];
                string tipoCarroceria = Request.Params["Carroceria"];
                string tipo = Request.Params["Tipo"];
                string siglaUF = Request.Params["SiglaUF"];
                string cpfMotorista = Utilidades.String.OnlyNumbers(Request.Params["CPFMotorista"]);
                string nomeMotorista = Request.Params["NomeMotorista"];
                string status = Request.Params["Status"];
                string observacao = Request.Params["Observacao"];
                string observacaoProprietario = Request.Params["ObservacaoProprietario"];
                string ciot = Request.Params["CIOTProprietario"];
                string numeroCompraValePedagio = Request.Params["NumeroCompraValePedagio"];
                string taf = Request.Params["TAF"];
                string nroRegEstadual = Request.Params["NroRegEstadual"];
                string xCampo = Request.Params["XCampo"];
                string xTexto = Request.Params["XTexto"];

                DateTime dataCompra, dataLicenca, dataVencimentoGarantiaPlena, dataVencimentoGarantiaEscalonada, dataInicioVigenciaTagValePedagio, dataFimVigenciaTagValePedagio;
                DateTime.TryParseExact(Request.Params["DataCompra"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCompra);
                DateTime.TryParseExact(Request.Params["DataLicenca"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLicenca);
                DateTime.TryParseExact(Request.Params["DataVencimentoGarantiaPlena"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoGarantiaPlena);
                DateTime.TryParseExact(Request.Params["DataVencimentoGarantiaEscalonada"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoGarantiaEscalonada);
                DateTime.TryParseExact(Request.Params["DataInicioVigenciaTagValePedagio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioVigenciaTagValePedagio);
                DateTime.TryParseExact(Request.Params["DataFimVigenciaTagValePedagio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimVigenciaTagValePedagio);

                decimal valorAquisicao, mediaPadrao, valorValePedagio, valorContainerAverbacao = 0;
                decimal.TryParse(Request.Params["ValorAquisicao"].Replace(".", ","), out valorAquisicao);
                decimal.TryParse(Request.Params["MediaPadrao"].Replace(".", ","), out mediaPadrao);
                decimal.TryParse(Request.Params["ValorValePedagio"].Replace(".", ","), out valorValePedagio);
                //decimal.TryParse(Request.Params["ValorContainerAverbacao"].Replace(".", ","), out valorContainerAverbacao);

                bool.TryParse(Request.Params["PossuiTagValePedagio"], out bool possuiTagValePedagio);

                int tara, capacidadeKG, capacidadeM3, codigo, kilometragemAtual, anoFabricacao, anoModelo, capacidadeTanque, codigoTipoVeiculo, codigoMarcaVeiculo, codigoModeloVeiculo, rntrc, codigoModeloVeicularCarga;

                string numeroFrota = Request.Params["NumeroFrota"];
                int.TryParse(Request.Params["AnoFabricacao"], out anoFabricacao);
                int.TryParse(Request.Params["AnoModelo"], out anoModelo);
                int.TryParse(Request.Params["CapacidadeKG"], out capacidadeKG);
                int.TryParse(Request.Params["CapacidadeM3"], out capacidadeM3);
                int.TryParse(Request.Params["TaraKG"], out tara);
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoTipoVeiculo"], out codigoTipoVeiculo);
                int.TryParse(Request.Params["CodigoMarcaVeiculo"], out codigoMarcaVeiculo);
                int.TryParse(Request.Params["CodigoModeloVeiculo"], out codigoModeloVeiculo);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["KilometragemAtual"]), out kilometragemAtual);
                int.TryParse(Request.Params["CapacidadeTanque"], out capacidadeTanque);
                int.TryParse(Request.Params["RNTRCProprietario"], out rntrc);
                int.TryParse(Request.Params["CodigoModeloVeicularCarga"], out codigoModeloVeicularCarga);

                double cpfCnpjProprietario = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoProprietario"]), out cpfCnpjProprietario);

                double cpfCnpjFornecedorValePedagio = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoFornecedorValePedagio"]), out cpfCnpjFornecedorValePedagio);

                double cpfCnpjResponsavelValePedagio = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoResponsavelValePedagio"]), out cpfCnpjResponsavelValePedagio);

                Dominio.Enumeradores.TipoProprietarioVeiculo tipoProprietario;
                Enum.TryParse<Dominio.Enumeradores.TipoProprietarioVeiculo>(Request.Params["TipoProprietario"], out tipoProprietario);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget modoCompraPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioTagViaFacil;
                if (!string.IsNullOrWhiteSpace(Request.Params["TipoTag"]))
                    Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget>(Request.Params["TipoTag"], out modoCompraPedagio);

                if (string.IsNullOrWhiteSpace(placa) || placa.Length != 7)
                    return Json<bool>(false, false, "Placa inválida!");

                if (System.Configuration.ConfigurationManager.AppSettings["VeiculoExigeCadastroTipo"] == "SIM")
                {
                    if (codigoTipoVeiculo == 0)
                        return Json<bool>(false, false, "Obrigatório informar o Tipo do Veículo.");
                }

                if (string.IsNullOrWhiteSpace(renavam))
                    return Json<bool>(false, false, "Renavam inválido!");

                if (!string.IsNullOrWhiteSpace(cpfMotorista) && !Utilidades.Validate.ValidarCPF(cpfMotorista))
                    return Json<bool>(false, false, "O CPF do motorista é inválido.");

                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Veiculo veiculo;
                bool situacaoAnterior = false;
                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de veículo negada!");

                    veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                    situacaoAnterior = veiculo.Ativo;

                    if (status == "I" && veiculo != null && veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                        return Json<bool>(false, false, "Necessário excluir vinculos antes de inativar o veículo!");

                    if (veiculo.TipoVeiculo != tipoVeiculo && veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                        return Json<bool>(false, false, "Necessário excluir vinculos antes de alterar o tipo do veículo!");

                    veiculo.Initialize();
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de veículo negada!");
                    veiculo = new Dominio.Entidades.Veiculo
                    {
                        Ativo = true
                    };
                }

                Dominio.Entidades.Veiculo veiculoAux = repVeiculo.BuscarPorPlacaTodas(this.EmpresaUsuario.Codigo, placa);
                if (veiculoAux != null && veiculoAux.Codigo != codigo)
                    return Json<bool>(false, false, "Existe um veículo cadastrado com a mesma placa " + (veiculoAux.Ativo ? " e status Ativo." : " e status Inativo"));
                veiculoAux = null;

                veiculo.CapacidadeKG = capacidadeKG;
                veiculo.CapacidadeM3 = capacidadeM3;
                veiculo.Empresa = this.EmpresaUsuario;
                veiculo.Estado = repEstado.BuscarPorSigla(siglaUF);
                veiculo.KilometragemAtual = kilometragemAtual;
                veiculo.Placa = placa;
                veiculo.Renavam = renavam;
                veiculo.AnoFabricacao = anoFabricacao;
                veiculo.AnoModelo = anoModelo;
                veiculo.CapacidadeTanque = capacidadeTanque;
                veiculo.Chassi = chassi;
                veiculo.Contrato = contrato;

                if (dataCompra != DateTime.MinValue)
                    veiculo.DataCompra = dataCompra;
                else
                    veiculo.DataCompra = null;

                if (dataLicenca != DateTime.MinValue)
                    veiculo.DataLicenca = dataLicenca;
                else
                    veiculo.DataLicenca = null;

                if (dataVencimentoGarantiaEscalonada != DateTime.MinValue)
                    veiculo.DataVencimentoGarantiaEscalonada = dataVencimentoGarantiaEscalonada;
                else
                    veiculo.DataVencimentoGarantiaEscalonada = null;

                if (dataVencimentoGarantiaPlena != DateTime.MinValue)
                    veiculo.DataVencimentoGarantiaPlena = dataVencimentoGarantiaPlena;
                else
                    veiculo.DataVencimentoGarantiaPlena = null;

                veiculo.MediaPadrao = mediaPadrao;
                veiculo.Marca = repMarcaVeiculo.BuscarPorCodigo(codigoMarcaVeiculo, this.EmpresaUsuario.Codigo);
                veiculo.Modelo = repModeloVeiculo.BuscarPorCodigo(codigoModeloVeiculo, this.EmpresaUsuario.Codigo);
                veiculo.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);
                veiculo.NumeroMotor = numeroMotor;
                veiculo.PendenteIntegracaoEmbarcador = true;
                veiculo.Observacao = observacao;
                veiculo.TipoDoVeiculo = repTipoVeiculo.BuscarPorCodigo(codigoTipoVeiculo, this.EmpresaUsuario.Codigo);
                veiculo.ValorAquisicao = valorAquisicao;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                {
                    veiculo.Ativo = status == "A";
                }


                veiculo.Tara = tara;
                veiculo.Tipo = tipo;
                veiculo.TipoCarroceria = tipoCarroceria;
                veiculo.TipoRodado = tipoRodado;
                veiculo.TipoVeiculo = tipoVeiculo;
                veiculo.NumeroFrota = numeroFrota;
                veiculo.TAF = taf;
                veiculo.NroRegEstadual = nroRegEstadual;
                veiculo.XCampo = xCampo;
                veiculo.XTexto = xTexto;
                //veiculo.ValorContainerAverbacao = valorContainerAverbacao;

                if (veiculo.Tipo == "T")
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    veiculo.TipoProprietario = tipoProprietario;
                    veiculo.ObservacaoCTe = observacaoProprietario;
                    veiculo.RNTRC = rntrc;
                    veiculo.Proprietario = cpfCnpjProprietario > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjProprietario) : null;
                    veiculo.CIOT = ciot;
                }
                else
                {
                    veiculo.ObservacaoCTe = string.Empty;
                    veiculo.RNTRC = rntrc;
                    veiculo.Proprietario = null;
                    veiculo.TipoProprietario = Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    veiculo.CIOT = ciot;
                }

                if (veiculo.Tipo == "T" && veiculo.Proprietario != null)
                {
                    if (string.IsNullOrWhiteSpace(veiculo.Proprietario.IE_RG))
                        return Json<bool>(false, false, "Proprietário do veículo sem Inscrição Estadual, favor verificar cadastro do cliente!");
                    if (veiculo.Proprietario.IE_RG != "ISENTO" && Utilidades.String.OnlyNumbers(veiculo.Proprietario.IE_RG) == "")
                        return Json<bool>(false, false, "Proprietário do veículo sem Inscrição Estadual, favor verificar cadastro do cliente!");
                }

                if (cpfCnpjFornecedorValePedagio > 0f)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    veiculo.FornecedorValePedagio = repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedorValePedagio);
                }
                else
                    veiculo.FornecedorValePedagio = null;

                if (cpfCnpjResponsavelValePedagio > 0f)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    veiculo.ResponsavelValePedagio = repCliente.BuscarPorCPFCNPJ(cpfCnpjResponsavelValePedagio);
                }
                else
                    veiculo.ResponsavelValePedagio = null;

                veiculo.NumeroCompraValePedagio = numeroCompraValePedagio;
                veiculo.ValorValePedagio = valorValePedagio;

                veiculo.PossuiTagValePedagio = possuiTagValePedagio;
                veiculo.ModoCompraValePedagioTarget = modoCompraPedagio;
                //veiculo.DataInicioVigenciaTagValePedagio = dataInicioVigenciaTagValePedagio;
                //veiculo.DataFimVigenciaTagValePedagio = dataFimVigenciaTagValePedagio;

                bool possuiRastreador;
                bool.TryParse(Request.Params["PossuiRastreador"], out possuiRastreador);
                veiculo.PossuiRastreador = possuiRastreador;
                if (veiculo.PossuiRastreador)
                {

                    int codigoTecnologiaRastreador;
                    int.TryParse(Request.Params["CodigoTecnologiaRastreador"], out codigoTecnologiaRastreador);
                    Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
                    veiculo.TecnologiaRastreador = repTecnologiaRastreador.BuscarPorCodigo(codigoTecnologiaRastreador, false);

                    int codigoTipoComunicacaoRastreador;
                    int.TryParse(Request.Params["CodigoTipoComunicacaoRastreador"], out codigoTipoComunicacaoRastreador);
                    Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);
                    veiculo.TipoComunicacaoRastreador = repTipoComunicacaoRastreador.BuscarPorCodigo(codigoTipoComunicacaoRastreador, false);

                    string numeroEquipamentoRastreador = Request.Params["NumeroEquipamentoRastreador"];
                    veiculo.NumeroEquipamentoRastreador = numeroEquipamentoRastreador;

                    if (veiculo.TecnologiaRastreador == null || veiculo.TipoComunicacaoRastreador == null || string.IsNullOrWhiteSpace(veiculo.NumeroEquipamentoRastreador))
                    {
                        return Json<bool>(false, false, "Ao indicar a existência de rastreador, deve ser informada a tecnologia, comunicação e o número do equipamento.");
                    }
                }
                else
                {
                    veiculo.TecnologiaRastreador = null;
                    veiculo.TipoComunicacaoRastreador = null;
                    veiculo.NumeroEquipamentoRastreador = null;
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                if (configuracao.ObrigatorioCadastrarRastreadorNosVeiculos && (
                    !veiculo.PossuiRastreador || veiculo.TecnologiaRastreador == null || veiculo.TipoComunicacaoRastreador == null || string.IsNullOrWhiteSpace(veiculo.NumeroEquipamentoRastreador)
                ))
                {
                    return Json<bool>(false, false, "É obrigatório indicar a existência do rastreador, tecnologia, comunicação e número do equipamento.");
                }

                if (codigo > 0)
                {

                    Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.Salvar_VeiculoControllerEmissaoCTE, this.Usuario, unitOfWork);
                    repVeiculo.Atualizar(veiculo, Auditado);
                }
                else
                    repVeiculo.Inserir(veiculo, Auditado);

                if (!string.IsNullOrWhiteSpace(cpfMotorista) && !string.IsNullOrWhiteSpace(nomeMotorista))
                {
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(this.EmpresaUsuario.Codigo, cpfMotorista, "M");

                    if (motorista == null)
                    {
                        Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                        motorista = new Dominio.Entidades.Usuario
                        {
                            Tipo = "M",
                            CPF = cpfMotorista,
                            Empresa = this.EmpresaUsuario,
                            Localidade = this.EmpresaUsuario.Localidade,
                            Nome = nomeMotorista,
                            Status = "A",
                            TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao,
                            Setor = repSetor.BuscarPorCodigo(1)
                        };

                        repUsuario.Inserir(motorista);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, null, "Criou motorista pelo cadastro de veículo.", unitOfWork);
                    }

                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotoristaPrincipal = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                    if (veiculoMotoristaPrincipal != null)
                    {
                        veiculoMotoristaPrincipal.Motorista = motorista;
                        veiculoMotoristaPrincipal.CPF = motorista.CPF;
                        veiculoMotoristaPrincipal.Nome = motorista.Nome;
                        repVeiculoMotorista.Atualizar(veiculoMotoristaPrincipal);
                    }
                    else
                    {
                        veiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();
                        veiculoMotoristaPrincipal.Motorista = motorista;
                        veiculoMotoristaPrincipal.CPF = motorista.CPF;
                        veiculoMotoristaPrincipal.Nome = motorista.Nome;
                        veiculoMotoristaPrincipal.Principal = true;
                        veiculoMotoristaPrincipal.Veiculo = veiculo;
                        repVeiculoMotorista.Inserir(veiculoMotoristaPrincipal);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotoristaPrincipal = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                    if (veiculoMotoristaPrincipal != null)
                        repVeiculoMotorista.Deletar(veiculoMotoristaPrincipal);
                }

                this.SalvarMotoristasAdicionais(veiculo, unitOfWork);

                try
                {
                    Servicos.Veiculo svcVeiculo = new Servicos.Veiculo(unitOfWork);
                    svcVeiculo.SalvarViculosMatrizFilial(veiculo, unitOfWork, Auditado);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return Json<bool>(false, false, "Ocorreu uma falha ao salvar veículo nas filiais.");
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarVeiculosVinculados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                var retorno = veiculo.VeiculosVinculados != null ? (from obj in veiculo.VeiculosVinculados
                                                                    where obj.Ativo
                                                                    select new
                                                                    {
                                                                        obj.Codigo,
                                                                        obj.Placa,
                                                                        obj.Renavam,
                                                                        obj.DescricaoTipoVeiculo
                                                                    }).ToList() : null;

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do veículo.");
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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                //Repositorio.VeiculoMotoristas repVeiculoMotoristas = new Repositorio.VeiculoMotoristas(unitOfWork);
                //List<Dominio.Entidades.VeiculoMotoristas> veiculoMotoristas = repVeiculoMotoristas.BuscarPorVeiculo(codigo);

                Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaPrincipal = veiculo.Motoristas != null && veiculo.Motoristas.Count() > 0 ? veiculo.Motoristas.Where(o => o.Principal).FirstOrDefault() : null;

                var retorno = new
                {
                    veiculo.Codigo,
                    veiculo.CapacidadeKG,
                    veiculo.CapacidadeM3,
                    CPFMotorista = motoristaPrincipal != null && motoristaPrincipal.Motorista != null ? String.Format(@"{0:000\.000\.000\-00}", long.Parse(motoristaPrincipal.Motorista.CPF)) : string.Empty,
                    SiglaUF = veiculo.Estado.Sigla,
                    NomeMotorista = motoristaPrincipal != null && motoristaPrincipal.Motorista != null ? motoristaPrincipal.Nome : string.Empty,
                    veiculo.Placa,
                    veiculo.Renavam,
                    veiculo.Situacao,
                    TaraKG = veiculo.Tara,
                    veiculo.Tipo,
                    Carroceria = veiculo.TipoCarroceria,
                    veiculo.TipoRodado,
                    veiculo.TipoVeiculo,
                    Status = veiculo.Ativo ? "A" : "I",
                    veiculo.KilometragemAtual,
                    veiculo.AnoFabricacao,
                    veiculo.AnoModelo,
                    veiculo.CapacidadeTanque,
                    veiculo.Chassi,
                    veiculo.Contrato,
                    DataCompra = veiculo.DataCompra != null ? veiculo.DataCompra.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataLicenca = veiculo.DataLicenca != null ? veiculo.DataLicenca.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimentoGarantiaEscalonada = veiculo.DataVencimentoGarantiaEscalonada != null ? veiculo.DataVencimentoGarantiaEscalonada.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimentoGarantiaPlena = veiculo.DataVencimentoGarantiaPlena != null ? veiculo.DataVencimentoGarantiaPlena.Value.ToString("dd/MM/yyyy") : string.Empty,
                    CodigoMarca = veiculo.Marca != null ? veiculo.Marca.Codigo : 0,
                    DescricaoMarca = veiculo.Marca != null ? veiculo.Marca.Descricao : string.Empty,
                    CodigoModelo = veiculo.Modelo != null ? veiculo.Modelo.Codigo : 0,
                    DescricaoModelo = veiculo.Modelo != null ? veiculo.Modelo.Descricao : string.Empty,
                    CodigoTipo = veiculo.TipoDoVeiculo != null ? veiculo.TipoDoVeiculo.Codigo : 0,
                    DescricaoTipo = veiculo.TipoDoVeiculo != null ? veiculo.TipoDoVeiculo.Descricao : string.Empty,
                    veiculo.ValorAquisicao,
                    veiculo.NumeroMotor,
                    veiculo.MediaPadrao,
                    veiculo.Observacao,
                    veiculo.ObservacaoCTe,
                    RNTRC = veiculo.RNTRC > 0 ? string.Format("{0:00000000}", veiculo.RNTRC) : string.Empty,
                    CIOT = veiculo.CIOT,
                    veiculo.TipoProprietario,
                    CodigoProprietario = veiculo.Proprietario != null ? veiculo.Proprietario.CPF_CNPJ : 0f,
                    veiculo.NumeroFrota,
                    DescricaoProprietario = veiculo.Proprietario != null ? string.Concat(veiculo.Proprietario.CPF_CNPJ_Formatado, " - ", veiculo.Proprietario.Nome) : string.Empty,
                    CodigoModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Codigo ?? 0,
                    DescricaoModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    VeiculosVinculados = veiculo.VeiculosVinculados != null ? (from obj in veiculo.VeiculosVinculados
                                                                               where obj.Ativo
                                                                               select new
                                                                               {
                                                                                   obj.Codigo,
                                                                                   obj.Placa,
                                                                                   obj.Renavam,
                                                                                   obj.DescricaoTipoVeiculo
                                                                               }).ToList() : null,
                    CodigoFornecedorValePedagio = veiculo.FornecedorValePedagio != null ? veiculo.FornecedorValePedagio.CPF_CNPJ : 0f,
                    DescricaoFornecedorValePedagio = veiculo.FornecedorValePedagio != null ? string.Concat(veiculo.FornecedorValePedagio.CPF_CNPJ_Formatado, " - ", veiculo.FornecedorValePedagio.Nome) : string.Empty,
                    CodigoResponsavelValePedagio = veiculo.ResponsavelValePedagio != null ? veiculo.ResponsavelValePedagio.CPF_CNPJ : 0f,
                    DescricaoResponsavelValePedagio = veiculo.ResponsavelValePedagio != null ? string.Concat(veiculo.ResponsavelValePedagio.CPF_CNPJ_Formatado, " - ", veiculo.ResponsavelValePedagio.Nome) : string.Empty,
                    veiculo.NumeroCompraValePedagio,
                    veiculo.ValorValePedagio,
                    veiculo.PossuiTagValePedagio,
                    TipoTag = veiculo.ModoCompraValePedagioTarget.HasValue ? veiculo.ModoCompraValePedagioTarget.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioTagViaFacil,
                    DataInicioVigenciaTagValePedagio = veiculo.DataInicioVigenciaTagValePedagio != null ? veiculo.DataInicioVigenciaTagValePedagio.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataFimVigenciaTagValePedagio = veiculo.DataFimVigenciaTagValePedagio != null ? veiculo.DataFimVigenciaTagValePedagio.Value.ToString("dd/MM/yyyy") : string.Empty,
                    veiculo.TAF,
                    veiculo.NroRegEstadual,
                    veiculo.XCampo,
                    veiculo.XTexto,
                    Motoristas = (from obj in veiculo.VeiculoMotoristas
                                  select new Dominio.ObjetosDeValor.MotoristaMDFe()
                                  {
                                      Codigo = obj.Codigo,
                                      CPF = obj.Motorista.CPF,
                                      Nome = obj.Motorista.Nome,
                                      Excluir = false
                                  }).ToList(),
                    veiculo.PossuiRastreador,
                    TecnologiaRastreador = new
                    {
                        Codigo = veiculo.TecnologiaRastreador?.Codigo ?? null,
                        Descricao = veiculo.TecnologiaRastreador?.Descricao ?? null
                    },
                    TipoComunicacaoRastreador = new
                    {
                        Codigo = veiculo.TipoComunicacaoRastreador?.Codigo ?? null,
                        Descricao = veiculo.TipoComunicacaoRastreador?.Descricao ?? null
                    },
                    veiculo.NumeroEquipamentoRastreador
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterMotoristaDoVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoVeiculo;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                if (veiculo == null)
                    return Json<bool>(false, false, "O veículo não foi encontrado.");

                Dominio.Entidades.Usuario motoristaPrincipal = veiculo.Motoristas?.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

                var retorno = new
                {
                    CodigoMotorista = motoristaPrincipal?.Codigo ?? 0,
                    NomeMotorista = motoristaPrincipal?.Nome ?? string.Empty,
                    CPFMotorista = motoristaPrincipal?.CPF ?? string.Empty
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter o motorista do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string placa = Request.Params["Placa"];
                string renavam = Request.Params["Renavam"];
                string tipoVeiculo = Request.Params["TipoVeiculo"];
                string status = Request.Params["Status"];

                if (!string.IsNullOrWhiteSpace(status) && status.Length > 1)
                    status = status.Substring(0, 1).ToUpper();

                // Remove hifen da placa
                placa = placa.Replace("-", "");

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                IList<Dominio.Entidades.Veiculo> listaVeiculos = repVeiculo.Consultar(this.EmpresaUsuario.Codigo, placa, renavam, tipoVeiculo, status, inicioRegistros, 50);
                int countVeiculos = repVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, placa, renavam, tipoVeiculo, status);

                var retorno = from obj in listaVeiculos
                              select new
                              {
                                  obj.Codigo,
                                  TipoVeiculo = obj.TipoDoVeiculo?.Codigo ?? 0,
                                  obj.Placa,
                                  obj.Renavam,
                                  obj.Estado.Sigla,
                                  obj.DescricaoTipoVeiculo,
                                  obj.DescricaoTipoRodado,
                                  obj.DescricaoTipoCarroceria
                              };

                return Json(retorno, true, null, new string[] { "Código", "TipoVeiculo", "Placa|10", "Renavam|15", "UF|5", "Tipo Veículo|20", "Tipo Rodado|20", "Tipo Carroceria|20" }, countVeiculos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os veículos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarEixosVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoVeiculo;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return Json<bool>(false, false, "Veículo não encontrado.");

                if (veiculo.TipoDoVeiculo == null)
                    return Json<bool>(false, false, "O veículo não possui um tipo de veículo configurado.");

                List<Dominio.Entidades.Pneu> pneus = repPneu.BuscarPorVeiculo(veiculo.Codigo);

                var retorno = (from obj in veiculo.TipoDoVeiculo.EixosDoVeiculo
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.Interno_Externo,
                                   obj.OrdemEixo,
                                   obj.Posicao,
                                   obj.Tipo,
                                   Pneu = (from p in pneus
                                           where p.Eixo.Codigo == obj.Codigo
                                           select new
                                           {
                                               p.Codigo,
                                               p.Serie,
                                               p.MarcaPneu.Descricao
                                           }).FirstOrDefault()
                               });

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os eixos do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private void SalvarMotoristasAdicionais(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Motoristas"]))
            {
                List<Dominio.ObjetosDeValor.MotoristaMDFe> motoristas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.MotoristaMDFe>>(Request.Params["Motoristas"]);

                if (motoristas != null)
                {
                    Repositorio.VeiculoMotoristas repVeiculoMotoristas = new Repositorio.VeiculoMotoristas(unidadeDeTrabalho);

                    for (var i = 0; i < motoristas.Count; i++)
                    {
                        Dominio.Entidades.VeiculoMotoristas veiculoMotorista = repVeiculoMotoristas.BuscarPorCodigo(motoristas[i].Codigo);

                        if (!motoristas[i].Excluir)
                        {
                            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                            Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCPF(veiculo.Empresa.Codigo, motoristas[i].CPF, "M");

                            if (motorista != null)
                            {
                                if (veiculoMotorista == null)
                                    veiculoMotorista = new Dominio.Entidades.VeiculoMotoristas();
                                else
                                    veiculoMotorista.Initialize();

                                veiculoMotorista.Veiculo = veiculo;
                                veiculoMotorista.Motorista = motorista;

                                if (veiculoMotorista.Codigo > 0)
                                {
                                    repVeiculoMotoristas.Atualizar(veiculoMotorista, Auditado);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, veiculoMotorista.GetChanges(), "Atualizou motorista adicional.", unidadeDeTrabalho);
                                }
                                else
                                {
                                    repVeiculoMotoristas.Inserir(veiculoMotorista, Auditado);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Adicionou um motorista adicional.", unidadeDeTrabalho);
                                }
                            }
                        }
                        else if (veiculoMotorista != null && veiculoMotorista.Codigo > 0)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Removeu motorista adicional.", unidadeDeTrabalho);
                            repVeiculoMotoristas.Deletar(veiculoMotorista);
                        }
                    }
                }
            }
        }
    }
}
