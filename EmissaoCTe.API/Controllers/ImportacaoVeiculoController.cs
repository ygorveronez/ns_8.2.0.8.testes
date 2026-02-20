using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ImportacaoVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("importacaodeveiculos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Importar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão de inclusão negada!");

                int codigoEmpresaOrigem, codigoEmpresaDestino = 0;
                int.TryParse(Request.Params["CodigoEmpresaOrigem"], out codigoEmpresaOrigem);
                int.TryParse(Request.Params["CodigoEmpresaDestino"], out codigoEmpresaDestino);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaOrigem = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaOrigem, this.EmpresaUsuario.Codigo);
                Dominio.Entidades.Empresa empresaDestino = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaDestino, this.EmpresaUsuario.Codigo);

                if (empresaOrigem == null)
                    return Json<bool>(false, false, "Empresa de origem dos veículos não encontrada!");

                if (empresaDestino == null)
                    return Json<bool>(false, false, "Empresa de destino dos veículos não encontrada!");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                List<Dominio.Entidades.Veiculo> veiculosOrigem = repVeiculo.BuscarPorEmpresa(empresaOrigem.Codigo, "A");
                List<Dominio.Entidades.Veiculo> veiculosDestino = repVeiculo.BuscarPorEmpresa(empresaDestino.Codigo, "A");

                unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Veiculo veiculoOrigem in veiculosOrigem)
                {
                    Dominio.Entidades.Veiculo veiculo = (from obj in veiculosDestino where obj.Placa == veiculoOrigem.Placa select obj).FirstOrDefault();

                    if (veiculo == null)
                    {

                        veiculo = new Dominio.Entidades.Veiculo();

                        this.ObterVeiculoParaVincular(ref veiculo, veiculoOrigem, empresaDestino);

                        // Vincula motorista (se nao tiver, cadastra)
                        //veiculo.Motorista = this.MotoristaDoVeiculo(veiculo.CPFMotorista, veiculo.NomeMotorista, empresaDestino, unidadeDeTrabalho);

                        repVeiculo.Inserir(veiculo);

                        if (veiculoOrigem.VeiculosVinculados != null && veiculoOrigem.VeiculosVinculados.Count > 0)
                        {

                            veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                            foreach (Dominio.Entidades.Veiculo veiculoVinculadoOrigem in veiculoOrigem.VeiculosVinculados)
                            {
                                Dominio.Entidades.Veiculo veiculoVinculado = (from obj in veiculosDestino where obj.Placa == veiculoVinculadoOrigem.Placa select obj).FirstOrDefault();

                                if (veiculoVinculado == null)
                                {
                                    veiculoVinculado = new Dominio.Entidades.Veiculo();

                                    this.ObterVeiculoParaVincular(ref veiculoVinculado, veiculoVinculadoOrigem, empresaDestino);

                                    // Vincula motorista (se nao tiver, cadastra)
                                    //veiculoVinculado.Motorista = this.MotoristaDoVeiculo(veiculoVinculado.CPFMotorista, veiculoVinculado.NomeMotorista, empresaDestino, unidadeDeTrabalho);

                                    repVeiculo.Inserir(veiculoVinculado);

                                    veiculosDestino.Add(veiculoVinculado);

                                    veiculo.VeiculosVinculados.Add(veiculoVinculado);
                                }
                            }

                            repVeiculo.Atualizar(veiculo);
                        }

                        veiculosDestino.Add(veiculo);

                    }
                    else
                    {
                        this.ObterVeiculoParaVincular(ref veiculo, veiculoOrigem, empresaDestino);

                        if (veiculo.VeiculosVinculados == null)
                            veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                        else
                            veiculo.VeiculosVinculados.Clear();

                        if (veiculoOrigem.VeiculosVinculados != null && veiculoOrigem.VeiculosVinculados.Count > 0)
                        {
                            foreach (Dominio.Entidades.Veiculo veiculoVinculadoOrigem in veiculoOrigem.VeiculosVinculados)
                            {
                                Dominio.Entidades.Veiculo veiculoVinculado = (from obj in veiculosDestino where obj.Placa == veiculoVinculadoOrigem.Placa select obj).FirstOrDefault();

                                if (veiculoVinculado == null)
                                {
                                    veiculoVinculado = new Dominio.Entidades.Veiculo();

                                    this.ObterVeiculoParaVincular(ref veiculoVinculado, veiculoVinculadoOrigem, empresaDestino);

                                    // Vincula motorista (se nao tiver, cadastra)
                                    //veiculoVinculado.Motorista = this.MotoristaDoVeiculo(veiculoVinculado.CPFMotorista, veiculoVinculado.NomeMotorista, empresaDestino, unidadeDeTrabalho);

                                    repVeiculo.Inserir(veiculoVinculado);

                                    veiculosDestino.Add(veiculoVinculado);
                                }

                                veiculo.VeiculosVinculados.Add(veiculoVinculado);
                            }
                        }

                        repVeiculo.Atualizar(veiculo);
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao importar os veículos, atualize a página e tente novamente.");
            }
        }

        #endregion

        #region Métodos Privados

        private void ObterVeiculoParaVincular(ref Dominio.Entidades.Veiculo veiculoDestino, Dominio.Entidades.Veiculo veiculoOrigem, Dominio.Entidades.Empresa empresaDestino)
        {
            veiculoDestino.AnoFabricacao = veiculoOrigem.AnoFabricacao;
            veiculoDestino.AnoModelo = veiculoOrigem.AnoModelo;
            veiculoDestino.CapacidadeKG = veiculoOrigem.CapacidadeKG;
            veiculoDestino.CapacidadeM3 = veiculoOrigem.CapacidadeM3;
            veiculoDestino.CapacidadeTanque = veiculoOrigem.CapacidadeTanque;
            veiculoDestino.Chassi = veiculoOrigem.Chassi;
            veiculoDestino.Contrato = veiculoOrigem.Contrato;
            //veiculoDestino.CPFMotorista = veiculoOrigem.CPFMotorista;
            //veiculoDestino.NomeMotorista = veiculoOrigem.NomeMotorista;
            veiculoDestino.DataCompra = veiculoOrigem.DataCompra;
            veiculoDestino.DataLicenca = veiculoOrigem.DataLicenca;
            veiculoDestino.DataVencimentoGarantiaEscalonada = veiculoOrigem.DataVencimentoGarantiaEscalonada;
            veiculoDestino.DataVencimentoGarantiaPlena = veiculoOrigem.DataVencimentoGarantiaPlena;
            veiculoDestino.Empresa = empresaDestino;
            veiculoDestino.Estado = veiculoOrigem.Estado;
            veiculoDestino.KilometragemAtual = veiculoOrigem.KilometragemAtual;
            veiculoDestino.Marca = veiculoDestino.Codigo > 0 ? veiculoDestino.Marca : null;
            veiculoDestino.MediaPadrao = veiculoOrigem.MediaPadrao;
            veiculoDestino.Modelo = veiculoDestino.Codigo > 0 ? veiculoDestino.Modelo : null;
            veiculoDestino.NumeroMotor = veiculoOrigem.NumeroMotor;
            veiculoDestino.Observacao = veiculoOrigem.Observacao;
            veiculoDestino.ObservacaoCTe = veiculoOrigem.ObservacaoCTe;
            veiculoDestino.Placa = veiculoOrigem.Placa;
            veiculoDestino.Proprietario = veiculoOrigem.Proprietario;
            veiculoDestino.Renavam = veiculoOrigem.Renavam;
            veiculoDestino.RNTRC = veiculoOrigem.RNTRC;
            veiculoDestino.Situacao = veiculoOrigem.Situacao;
            veiculoDestino.Ativo = true;
            veiculoDestino.Tara = veiculoOrigem.Tara;
            veiculoDestino.Tipo = veiculoOrigem.Tipo;
            veiculoDestino.TipoCarroceria = veiculoOrigem.TipoCarroceria;
            veiculoDestino.TipoCombustivel = veiculoOrigem.TipoCombustivel;
            veiculoDestino.TipoDoVeiculo = veiculoDestino.Codigo > 0 ? veiculoDestino.TipoDoVeiculo : null;
            veiculoDestino.TipoProprietario = veiculoOrigem.TipoProprietario;
            veiculoDestino.TipoRodado = veiculoOrigem.TipoRodado;
            veiculoDestino.TipoVeiculo = veiculoOrigem.TipoVeiculo;
            veiculoDestino.ValorAquisicao = veiculoOrigem.ValorAquisicao;
            veiculoDestino.ModeloVeicularCarga = veiculoOrigem.ModeloVeicularCarga;
        }

        private Dominio.Entidades.Usuario MotoristaDoVeiculo(string cpf, string nome, Dominio.Entidades.Empresa empresaDestino, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Setor repSetor = new Repositorio.Setor(unidadeDeTrabalho);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(empresaDestino?.Codigo ?? 0, cpf);

            if (motorista == null && !string.IsNullOrEmpty(cpf) && !string.IsNullOrEmpty(nome))
            {
                motorista = new Dominio.Entidades.Usuario();

                motorista.Tipo = "M";
                motorista.CPF = cpf;
                motorista.Empresa = empresaDestino;
                motorista.Localidade = empresaDestino?.Localidade ?? null;
                motorista.Nome = nome;
                motorista.Status = "A";
                motorista.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                motorista.Setor = repSetor.BuscarPorCodigo(1);

                repUsuario.Inserir(motorista);
            }

            return motorista;
        }
        #endregion

    }
}
