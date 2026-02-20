using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeManual
{
    [CustomAuthorize("Cargas/CargaMDFeManual", "Cargas/CargaMDFeAquaviarioManual")]
    public class CargaMDFeManualCargasController : BaseController
    {
		#region Construtores

		public CargaMDFeManualCargasController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarDadosDasCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                List<int> cargas = Request.GetListParam<int>("Cargas");
                List<int> codigosCtes = Request.GetListParam<int>("CTes");
                List<string> chaves = Request.GetListParam<string>("NFes");

                int codigoOrigem = Request.GetIntParam("Origem");

                bool possuiDestinoExterior = false;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repConhecimentoDeTransporteEletronico.BuscarPorCodigo(codigosCtes);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> nfes = repNotaFiscal.BuscarPorChave(chaves);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargasLocaisPrestacao = null;
                if (cargas.Count == 0 && ctes?.Count > 0)
                {
                    carga = repCargaCTe.BuscarPorCTe(ctes.FirstOrDefault()?.Codigo ?? 0).Carga;
                    cargasLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCarga(carga?.Codigo ?? 0);
                }
                else
                {
                    cargasLocaisPrestacao = repCargaLocaisPrestacao.BuscarPorCargas(cargas);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = cargasLocaisPrestacao?.FirstOrDefault();

                if (carga == null)
                {
                    if (cargaLocalPrestacao != null)
                        carga = cargaLocalPrestacao.Carga;
                    else if (ctes?.Count > 0)
                        carga = repCargaCTe.BuscarPorCTe(ctes.FirstOrDefault()?.Codigo ?? 0).Carga;
                }

                List<Dominio.Entidades.Localidade> destinos = cargasLocaisPrestacao?.Count > 0 ? cargasLocaisPrestacao.Select(o => o.LocalidadeTerminoPrestacao).Concat(ctes.Select(o => o.LocalidadeTerminoPrestacao)).Distinct().ToList() : null;

                Dominio.Entidades.Localidade origem = null;
                if (codigoOrigem > 0)
                    origem = repLocalidade.BuscarPorCodigo(codigoOrigem);
                else
                {
                    if (cargas?.Count > 0)
                        origem = cargaLocalPrestacao.LocalidadeInicioPrestacao;
                    else if (ctes?.Count > 0)
                        origem = ctes.FirstOrDefault().LocalidadeInicioPrestacao;
                    else if (nfes?.Count > 0)
                        origem = nfes.FirstOrDefault().Empresa.Localidade;
                }

                if (carga != null && cargaLocalPrestacao == null)
                    cargaLocalPrestacao = repCargaLocaisPrestacao.BuscarPrimeiroRegistroPorCarga(carga.Codigo);

                if (origem != null && origem.Estado?.Sigla == "EX" && cargaLocalPrestacao?.LocalidadeFronteira != null)
                    origem = cargaLocalPrestacao.LocalidadeFronteira;

                if (destinos?.Count > 0 && destinos.Any(o => o.Estado?.Sigla == "EX") && cargaLocalPrestacao?.LocalidadeFronteira != null)
                {
                    possuiDestinoExterior = true;
                    destinos = new List<Dominio.Entidades.Localidade>() { cargaLocalPrestacao.LocalidadeFronteira };
                }

                List<Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao> mdfeManualDestinosPosicao = new List<Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao>();
                if (destinos?.Count > 0)
                {
                    foreach (Dominio.Entidades.Localidade dest in destinos)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorOrigemDestino(origem.Codigo, dest.Codigo);
                        Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao mdfeManualDestinoPosicao = new Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao();
                        mdfeManualDestinoPosicao.destino = dest;
                        if (rota != null)
                            mdfeManualDestinoPosicao.Distancia = rota.DistanciaKM;

                        mdfeManualDestinosPosicao.Add(mdfeManualDestinoPosicao);
                    }
                }

                mdfeManualDestinosPosicao = mdfeManualDestinosPosicao?.Count > 0 ? mdfeManualDestinosPosicao.OrderBy(obj => obj.Distancia).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao>(); 

                List<dynamic> dynDestinos = new List<dynamic>();
                for (int i = 0; i < mdfeManualDestinosPosicao.Count; i++)
                {
                    Dominio.Entidades.Localidade dest = mdfeManualDestinosPosicao[i].destino;
                    var dynDestino = new
                    {
                        Codigo = dest.Codigo,
                        Descricao = dest.DescricaoCidadeEstado,
                        Posicao = i + 1
                    };
                    dynDestinos.Add(dynDestino);
                }
                if (nfes?.Count > 0)
                {
                    for (int i = 0; i < nfes.Count; i++)
                    {
                        Dominio.Entidades.Localidade dest = nfes[i].Cliente.Localidade;
                        var dynDestino = new
                        {
                            Codigo = dest.Codigo,
                            Descricao = dest.DescricaoCidadeEstado,
                            Posicao = i + 1
                        };
                        dynDestinos.Add(dynDestino);
                    }
                }
                List<dynamic> dynListaValePedagio = new List<dynamic>();
                List<dynamic> dynListaCIOT = new List<dynamic>();
                List<dynamic> dynListaPercurso = new List<dynamic>();
                dynamic dynMotorista = null;
                if (carga != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> ListraCargaValePedagio = repCargaValePedagio.BuscarPorCarga(carga.Codigo);

                    if (ListraCargaValePedagio != null && ListraCargaValePedagio.Count > 0)
                    {
                        foreach (var cargaValePedagio in ListraCargaValePedagio)
                        {
                            var dynValePedagio = new
                            {
                                FornecedorValePedagio = cargaValePedagio.Fornecedor?.CPF_CNPJ_Formatado ?? "",
                                ConsultarFornecedorValePedagio = cargaValePedagio.Fornecedor?.CPF_CNPJ_SemFormato ?? "",
                                ResponsavelValePedagio = cargaValePedagio.Responsavel?.CPF_CNPJ_Formatado ?? "",
                                ConsultarResponsavelValePedagio = cargaValePedagio.Responsavel?.CPF_CNPJ_SemFormato ?? "",
                                ComprovanteValePedagio = cargaValePedagio.NumeroComprovante ?? "",
                                ValorValePedagio = cargaValePedagio.Valor.ToString("N2")
                            };
                            dynListaValePedagio.Add(dynValePedagio);
                        }
                    }
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);

                    if (cargaCIOT?.CIOT != null)
                    {
                        var dynCIOT = new
                        {
                            NumeroCIOT = cargaCIOT.CIOT.Numero ?? "",
                            ResponsavelCIOT = cargaCIOT.CIOT.Transportador?.CPF_CNPJ_Formatado ?? "",
                            ConsultarResponsavelCIOT = cargaCIOT.CIOT.Transportador?.CPF_CNPJ_SemFormato ?? "",
                        };
                        dynListaCIOT.Add(dynCIOT);
                        
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaMotorista motoristaCarga = repCargaMotorista.BuscarPorCarga(carga.Codigo).FirstOrDefault();
                    if(motoristaCarga != null)
                    {
                        dynMotorista = new
                        {
                            Codigo = motoristaCarga.Motorista.Codigo,
                            Descricao = motoristaCarga.Descricao ?? "",
                            PercentualExecucao =( motoristaCarga.PercentualExecucao ?? 100).ToString("n2"),
                        };                      
                    }
                }


                var retorno = new
                {
                    Origem = origem != null ? new { Codigo = origem.Codigo, Descricao = origem.DescricaoCidadeEstado } : null,
                    Destinos = dynDestinos,
                    PossuiDestinoExterior = possuiDestinoExterior,
                    Veiculo = carga != null && carga.Veiculo != null ? new { Codigo = carga.Veiculo.Codigo, Descricao = carga.PlacasVeiculos, Placa = carga.Veiculo.Placa } : new { Codigo = 0, Descricao = "", Placa = "" },
                    Reboques = carga != null ? (from v in carga.VeiculosVinculados select new { v.Codigo, v.Descricao }).ToList() : null,
                    Motorista = dynMotorista,
                    ValePedagio = dynListaValePedagio,
                    CIOT = dynListaCIOT
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar as passagens entre os estados.");
            }
        }
    }
}
