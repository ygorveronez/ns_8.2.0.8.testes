using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/ImportacaoVeiculo")]
    public class ImportacaoVeiculoController : BaseController
    {
		#region Construtores

		public ImportacaoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresaOrigem, codigoEmpresaDestino = 0;
                int.TryParse(Request.Params("TransportadorOrigem"), out codigoEmpresaOrigem);
                int.TryParse(Request.Params("TransportadorDestino"), out codigoEmpresaDestino);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaOrigem = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaOrigem, this.Empresa.Codigo);
                Dominio.Entidades.Empresa empresaDestino = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaDestino, this.Empresa.Codigo);

                if (empresaOrigem == null)
                    return new JsonpResult(false, "Empresa de origem dos veículos não encontrada!");

                if (empresaDestino == null)
                    return new JsonpResult(false, "Empresa de destino dos veículos não encontrada!");

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

                                    repVeiculo.Inserir(veiculoVinculado, Auditado);

                                    veiculosDestino.Add(veiculoVinculado);
                                    veiculo.VeiculosVinculados.Add(veiculoVinculado);
                                }
                            }
                        }

                        repVeiculo.Inserir(veiculo, Auditado);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Veículo importado entre transportadoras.", unidadeDeTrabalho);

                        veiculosDestino.Add(veiculo);

                    }
                    else
                    {
                        veiculo.Initialize();
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

                                    repVeiculo.Inserir(veiculoVinculado, Auditado);

                                    veiculosDestino.Add(veiculoVinculado);
                                }

                                veiculo.VeiculosVinculados.Add(veiculoVinculado);
                            }
                        }

                        repVeiculo.Atualizar(veiculo, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Veículo importado entre transportadoras.", unidadeDeTrabalho);
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true, "Importação realizada com sucesso!");

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao importar os veículos.");
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
            veiculoDestino.PendenteIntegracaoEmbarcador = true;
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

        #endregion
    }
}
