using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using ICSharpCode.SharpZipLib.Zip;
using MultiSoftware.MDFe.v300.ModalRodoviario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos
{
    public class MDFe : ServicoBase
    {
        #region Construtores

        public MDFe() : base() { }

        public MDFe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorCTesEPlacas(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Veiculo veiculoTracao, Dominio.Entidades.Veiculo veiculoReboque, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, ctes, unidadeDeTrabalho, "", "", null, null);

                this.GerarMunicipiosDeCarregamento(ref mdfe, ctes, unidadeDeTrabalho, null);
                this.GerarMunicipiosDeDescarregamento(ref mdfe, ctes, unidadeDeTrabalho, null, true);
                this.GerarVeiculo(mdfe, veiculoTracao, unidadeDeTrabalho);
                this.GerarReboques(mdfe, veiculoReboque, unidadeDeTrabalho);
                this.GerarMotoristas(mdfe, null, unidadeDeTrabalho, ctes.FirstOrDefault());
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                this.GerarValePedagioPorCTes(mdfe, ctes, unidadeDeTrabalho);
                this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarSeguroPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarCIOTPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                }
                else
                {
                    this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                }

                this.GerarPercursos(mdfe, unidadeDeTrabalho, null, null);

                unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorCargaEUnidade(Dominio.Entidades.Empresa empresa, int numeroCarga, int numeroUnidade, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante = null, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT = null, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento = null)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoCTe> integracoes = repIntegracaoCTe.Buscar(empresa.Codigo, numeroCarga, numeroUnidade, Dominio.Enumeradores.TipoIntegracao.Emissao, new Dominio.Enumeradores.StatusIntegracao[] { Dominio.Enumeradores.StatusIntegracao.Integrado, Dominio.Enumeradores.StatusIntegracao.Impresso }, "A");

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, integracoes, unidadeDeTrabalho);

                this.GerarMunicipiosDeCarregamento(ref mdfe, integracoes, unidadeDeTrabalho);
                this.GerarMunicipiosDeDescarregamento(mdfe, integracoes, unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculoUtilizado = null;
                this.GerarVeiculo(mdfe, integracoes, unidadeDeTrabalho, ref veiculoUtilizado);
                this.GerarPercursos(mdfe, unidadeDeTrabalho);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(mdfe, unidadeDeTrabalho);
                    this.GerarSeguroPorCTes(mdfe, unidadeDeTrabalho);
                }
                GerarProdutoPredominante(mdfe, produtoPredominante, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), unidadeDeTrabalho);
                GerarCIOT(mdfe, listaCIOT, unidadeDeTrabalho);

                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(mdfe, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), informacoesPagamento, veiculoUtilizado);

                unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorNFe(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfes, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho = null, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, string usuario = "", List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string observacaoMDFe = "", Dominio.Entidades.Veiculo veiculo = null, List<Dominio.Entidades.Veiculo> reboques = null, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas = null, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros = null, bool gerarProdutosPerigosos = true, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagios = null, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            bool novaTransacao = false;

            if (unidadeDeTrabalho == null)
            {
                novaTransacao = true;
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            }

            try
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, nfes, tipoOperacao, unidadeDeTrabalho, "", observacaoMDFe, localidadeCarregamento, localidadeDescarregamento, codigoUsuario);

                this.GerarMunicipiosDeCarregamento(ref mdfe, unidadeDeTrabalho, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamento(mdfe, nfes, unidadeDeTrabalho, localidadeDescarregamento, gerarProdutosPerigosos);
                this.GerarPercursos(mdfe, unidadeDeTrabalho, percursoEstado, passagens, configuracaoTMS);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                this.GerarSeguro(mdfe, seguros, new List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada>(), unidadeDeTrabalho);
                this.GerarVeiculoNFes(mdfe, unidadeDeTrabalho, veiculo);

                if (valesPedagios != null && valesPedagios.Count > 0)
                    this.GerarValePedagio(mdfe, valesPedagios, unidadeDeTrabalho);

                this.GerarContratantesNFe(mdfe, unidadeDeTrabalho);

                if (lacres != null)
                    this.GerarLacres(mdfe, lacres, unidadeDeTrabalho);

                if (valePedagioCompra != null)
                    this.GerarValePedagioCompra(mdfe, valePedagioCompra, unidadeDeTrabalho);

                if (usuario != "")
                    mdfe.Log = string.Concat("Criado por ", usuario, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                if (novaTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch
            {
                if (novaTransacao)
                {
                    unidadeDeTrabalho.Rollback();
                }
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorNotasFiscais(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho = null, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, string usuario = "", List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string observacaoMDFe = "", Dominio.Entidades.Veiculo veiculo = null, List<Dominio.Entidades.Veiculo> reboques = null, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas = null, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros = null, bool gerarProdutosPerigosos = true, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagios = null, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT = null, List<int> codigosCarga = null)
        {
            bool novaTransacao = false;

            if (unidadeDeTrabalho == null)
            {
                novaTransacao = true;
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            }

            try
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, notasFiscais, tipoOperacao, unidadeDeTrabalho, "", observacaoMDFe, localidadeCarregamento, localidadeDescarregamento, codigoUsuario);

                this.GerarMunicipiosDeCarregamento(ref mdfe, notasFiscais, unidadeDeTrabalho, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamento(mdfe, notasFiscais, unidadeDeTrabalho, localidadeDescarregamento, gerarProdutosPerigosos);
                this.GerarPercursos(mdfe, unidadeDeTrabalho, percursoEstado, passagens, configuracaoTMS);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                this.GerarSeguro(mdfe, seguros, new List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada>(), unidadeDeTrabalho);
                this.GerarVeiculoNFes(mdfe, notasFiscais, unidadeDeTrabalho, veiculo, codigosCarga);

                if (valesPedagios != null && valesPedagios.Count > 0)
                    this.GerarValePedagio(mdfe, valesPedagios, unidadeDeTrabalho);

                this.GerarContratantesNFe(mdfe, unidadeDeTrabalho);

                if (lacres != null)
                    this.GerarLacres(mdfe, lacres, unidadeDeTrabalho);

                if (valePedagioCompra != null)
                    this.GerarValePedagioCompra(mdfe, valePedagioCompra, unidadeDeTrabalho);

                GerarCIOT(mdfe, listaCIOT, unidadeDeTrabalho);

                if (usuario != "")
                    mdfe.Log = string.Concat("Criado por ", usuario, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                if (novaTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch
            {
                if (novaTransacao)
                {
                    unidadeDeTrabalho.Rollback();
                }
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFeImportadoPorCTes(Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho = null, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, string usuario = "", List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string observacaoMDFe = "", Dominio.Entidades.Veiculo veiculo = null, List<Dominio.Entidades.Veiculo> reboques = null, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas = null, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros = null, bool gerarProdutosPerigosos = true, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagios = null, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra = null, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoEmbarque = null, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoDesembarque = null, Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao viagem = null, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisCarregamento = null, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisDescarregamento = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            bool novaTransacao = false;

            if (unidadeDeTrabalho == null)
            {
                novaTransacao = true;
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            }

            try
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFeImportado(mdfeAquaviario, empresa, ctes, unidadeDeTrabalho, "", observacaoMDFe, localidadeCarregamento, localidadeDescarregamento, codigoUsuario);

                this.GerarMunicipiosDeCarregamento(ref mdfe, ctes, unidadeDeTrabalho, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamento(ref mdfe, ctes, unidadeDeTrabalho, localidadeDescarregamento, gerarProdutosPerigosos);
                this.GerarVeiculo(mdfe, ctes, unidadeDeTrabalho, veiculo, reboques, motoristas);
                this.GerarPercursos(mdfe, unidadeDeTrabalho, percursoEstado, passagens, configuracaoTMS);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);

                if (valesPedagios != null && valesPedagios.Count > 0)
                    this.GerarValePedagio(mdfe, valesPedagios, unidadeDeTrabalho);
                else
                    this.GerarValePedagioPorCTes(mdfe, ctes, unidadeDeTrabalho);

                this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                this.GerarInformacoesAquaviario(ref mdfe, portoEmbarque, portoDesembarque, viagem, terminaisCarregamento, terminaisDescarregamento, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(mdfe, ctes, unidadeDeTrabalho);

                    if (seguros != null && seguros.Count > 0)
                        this.GerarSeguro(mdfe, seguros, ctes, unidadeDeTrabalho);
                    else
                        this.GerarSeguroPorCTes(mdfe, ctes, unidadeDeTrabalho);

                    this.GerarCIOTPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                }
                else
                {
                    this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                }

                if (lacres != null)
                    this.GerarLacres(mdfe, lacres, unidadeDeTrabalho);

                if (valePedagioCompra != null)
                    this.GerarValePedagioCompra(mdfe, valePedagioCompra, unidadeDeTrabalho);

                if (usuario != "")
                    mdfe.Log = string.Concat("Gerado por ", usuario, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                if (novaTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch (Exception ex)
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorCTes(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho = null, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, string usuario = "", List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string observacaoMDFe = "", Dominio.Entidades.Veiculo veiculo = null, List<Dominio.Entidades.Veiculo> reboques = null, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas = null, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros = null, bool gerarProdutosPerigosos = true, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagios = null, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra = null, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoEmbarque = null, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoDesembarque = null, Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao viagem = null, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisCarregamento = null, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisDescarregamento = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante = null, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT = null, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento = null)
        {
            bool novaTransacao = false;

            if (unidadeDeTrabalho == null)
            {
                novaTransacao = true;
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            }

            try
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, ctes, unidadeDeTrabalho, "", observacaoMDFe, localidadeCarregamento, localidadeDescarregamento, codigoUsuario);

                this.GerarMunicipiosDeCarregamento(ref mdfe, ctes, unidadeDeTrabalho, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamento(ref mdfe, ctes, unidadeDeTrabalho, localidadeDescarregamento, gerarProdutosPerigosos);
                this.GerarVeiculo(mdfe, ctes, unidadeDeTrabalho, veiculo, reboques, motoristas);
                this.GerarPercursos(mdfe, unidadeDeTrabalho, percursoEstado, passagens, configuracaoTMS);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);

                if (valesPedagios != null && valesPedagios.Count > 0)
                    this.GerarValePedagio(mdfe, valesPedagios, unidadeDeTrabalho);
                else
                    this.GerarValePedagioPorCTes(mdfe, ctes, unidadeDeTrabalho);

                this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                this.GerarInformacoesAquaviario(ref mdfe, portoEmbarque, portoDesembarque, viagem, terminaisCarregamento, terminaisDescarregamento, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(mdfe, ctes, unidadeDeTrabalho);

                    if (seguros != null && seguros.Count > 0)
                        this.GerarSeguro(mdfe, seguros, ctes, unidadeDeTrabalho);
                    else
                        this.GerarSeguroPorCTes(mdfe, ctes, unidadeDeTrabalho);

                    this.GerarCIOTPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                }
                else
                {
                    this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                }

                if (lacres != null)
                    this.GerarLacres(mdfe, lacres, unidadeDeTrabalho);

                if (valePedagioCompra != null)
                    this.GerarValePedagioCompra(mdfe, valePedagioCompra, unidadeDeTrabalho);

                if (usuario != "")
                    mdfe.Log = string.Concat("Gerado por ", usuario, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                GerarProdutoPredominante(mdfe, produtoPredominante, ctes, unidadeDeTrabalho);
                GerarCIOT(mdfe, listaCIOT, unidadeDeTrabalho);
                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(mdfe, ctes, informacoesPagamento, veiculo);

                if (novaTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch (Exception ex)
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorCTesAnterior(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctes, Repositorio.UnitOfWork unitOfWork = null, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, string usuario = "", List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string observacaoMDFe = "", Dominio.Entidades.Veiculo veiculo = null, List<Dominio.Entidades.Veiculo> reboques = null, List<Dominio.Entidades.Usuario> motoristas = null, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros = null, bool gerarProdutosPerigosos = true, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagios = null, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra = null, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoEmbarque = null, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoDesembarque = null, Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao viagem = null, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisCarregamento = null, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisDescarregamento = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT = null)
        {
            bool novaTransacao = false;

            if (unitOfWork == null)
            {
                novaTransacao = true;
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            }

            try
            {
                if (novaTransacao)
                    unitOfWork.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, ctes, unitOfWork, "", observacaoMDFe, localidadeCarregamento, localidadeDescarregamento, codigoUsuario);

                this.GerarMunicipiosDeCarregamento(ref mdfe, ctes, unitOfWork, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamento(ref mdfe, ctes, unitOfWork, localidadeDescarregamento, gerarProdutosPerigosos);
                this.GerarVeiculo(mdfe, unitOfWork, veiculo, reboques, motoristas);
                this.GerarPercursos(mdfe, unitOfWork, percursoEstado, passagens, configuracaoTMS);
                this.GerarObservacoesVeiculos(mdfe, unitOfWork);

                if (valesPedagios != null && valesPedagios.Count > 0)
                    this.GerarValePedagio(mdfe, valesPedagios, unitOfWork);

                this.GerarValePedagioPorVeiculoTracao(mdfe, unitOfWork);

                this.GerarContratantesPorCTesTerceiro(mdfe, ctes, unitOfWork);

                this.GerarSeguroPorEmpresa(mdfe, unitOfWork);

                if (lacres != null)
                    this.GerarLacres(mdfe, lacres, unitOfWork);

                GerarCIOT(mdfe, listaCIOT, unitOfWork);

                if (usuario != "")
                    mdfe.Log = string.Concat("Gerado por ", usuario, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                if (novaTransacao)
                    unitOfWork.CommitChanges();

                return mdfe;
            }
            catch (Exception ex)
            {
                if (novaTransacao)
                    unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                throw;
            }
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> GerarMDFePorCTesDestinosDiferentes(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, string observacaoMDFe, Repositorio.UnitOfWork unidadeDeTrabalho = null, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, string usuario = "", List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null)
        {
            bool novaTransacao = false;

            if (unidadeDeTrabalho == null)
            {
                novaTransacao = true;
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            }

            try
            {
                if (novaTransacao)
                    unidadeDeTrabalho.Start();

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

                List<Dominio.Entidades.Estado> listaUFInicioPrestacao = (from obj in ctes select obj.LocalidadeInicioPrestacao.Estado).Distinct().ToList();

                for (var i = 0; i < listaUFInicioPrestacao.Count(); i++)
                {
                    List<Dominio.Entidades.Estado> listaUFTerminoPrestacao = (from obj in ctes where obj.LocalidadeInicioPrestacao.Estado.Sigla == listaUFInicioPrestacao[i].Sigla select obj.LocalidadeTerminoPrestacao.Estado).Distinct().ToList();

                    for (var j = 0; j < listaUFTerminoPrestacao.Count(); j++)
                    {
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesPorDestino = (from obj in ctes
                                                                                                     where obj.LocalidadeInicioPrestacao.Estado.Sigla == listaUFInicioPrestacao[i].Sigla &&
                                                                                                           obj.LocalidadeTerminoPrestacao.Estado.Sigla == listaUFTerminoPrestacao[j].Sigla
                                                                                                     select obj).ToList();

                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, ctesPorDestino, unidadeDeTrabalho, "", observacaoMDFe, localidadeCarregamento, localidadeDescarregamento);

                        this.GerarMunicipiosDeCarregamento(ref mdfe, ctesPorDestino, unidadeDeTrabalho, localidadeCarregamento);
                        this.GerarMunicipiosDeDescarregamento(ref mdfe, ctesPorDestino, unidadeDeTrabalho, localidadeDescarregamento, true);
                        this.GerarVeiculo(mdfe, ctesPorDestino, unidadeDeTrabalho);
                        this.GerarPercursos(mdfe, unidadeDeTrabalho, percursoEstado, passagens);
                        this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                        this.GerarValePedagioPorCTes(mdfe, ctesPorDestino, unidadeDeTrabalho);
                        this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);

                        if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                        {
                            this.GerarContratantesPorCTes(mdfe, ctesPorDestino, unidadeDeTrabalho);
                            this.GerarSeguroPorCTes(mdfe, ctesPorDestino, unidadeDeTrabalho);
                            this.GerarCIOTPorCTes(mdfe, ctesPorDestino, unidadeDeTrabalho);
                            this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                        }
                        else
                        {
                            this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                        }

                        if (lacres != null)
                            this.GerarLacres(mdfe, lacres, unidadeDeTrabalho);

                        if (usuario != "")
                            mdfe.Log = string.Concat("Criado por ", usuario, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                        mdfes.Add(mdfe);
                    }
                }

                if (novaTransacao)
                    unidadeDeTrabalho.CommitChanges();

                return mdfes;
            }
            catch
            {
                if (novaTransacao)
                {
                    unidadeDeTrabalho.Rollback();
                }
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorCodigoCTes(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> veiculos, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> reboques, string observacaoFisco, string observacaoContribuinte, string numeroCargaMDFeAnterior, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros, List<Dominio.ObjetosDeValor.ValePedagioIntegracao> valesPedagio, Dominio.ObjetosDeValor.MDFe.DadosMDFe dadosMDFe, Dominio.Entidades.Localidade localidadeCarregamento, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, ctes, unidadeDeTrabalho, observacaoFisco, observacaoContribuinte, null, null, 0, Dominio.Enumeradores.TipoCargaMDFe.CargaGeral, dadosMDFe);

                this.GerarMunicipiosDeCarregamento(ref mdfe, ctes, unidadeDeTrabalho, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamento(ref mdfe, ctes, unidadeDeTrabalho, null, true);
                this.GerarVeiculoPorObjeto(mdfe, veiculos, motoristas, reboques, unidadeDeTrabalho);
                this.GerarPercursos(mdfe, unidadeDeTrabalho);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                this.GerarValePedagioMDFe(mdfe, valesPedagio, unidadeDeTrabalho);
                this.GerarValePedagioPorCTes(mdfe, ctes, unidadeDeTrabalho);
                this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                this.GerarSeguro(mdfe, seguros, ctes, unidadeDeTrabalho);
                this.GerarProdutoPredominante(mdfe, produtoPredominante, ctes, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarSeguroPorCTes(mdfe, ctes, unidadeDeTrabalho);
                    this.GerarCIOTPorCTes(mdfe, ctes, unidadeDeTrabalho);
                }
                this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(numeroCargaMDFeAnterior) && numeroCargaMDFeAnterior != "0")
                    this.AdicionarCTesMDFeAnterior(mdfe, numeroCargaMDFeAnterior, empresa.Codigo, Dominio.Enumeradores.TipoCargaMDFe.CargaGeral, unidadeDeTrabalho);

                GerarCIOT(mdfe, listaCIOT, unidadeDeTrabalho);
                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(mdfe, ctes, informacoesPagamento, null);

                unidadeDeTrabalho.CommitChanges();
                return mdfe;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorNFEsGlobalizadas(Dominio.Entidades.Empresa empresa, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> veiculos, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> reboques, string observacaoFisco, string observacaoContribuinte, string numeroCargaMDFeAnterior, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros, List<Dominio.ObjetosDeValor.ValePedagioIntegracao> valesPedagio, Dominio.ObjetosDeValor.MDFe.DadosMDFe dadosMDFe, Dominio.Entidades.Localidade localidadeCarregamento, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                unidadeDeTrabalho.Start();
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, nfesGlobalizadas, unidadeDeTrabalho, observacaoFisco, observacaoContribuinte, null, null, 0, "", Dominio.Enumeradores.TipoCargaMDFe.CargaGeral);

                this.GerarMunicipiosDeCarregamentoPorNFeGlobalizada(ref mdfe, nfesGlobalizadas, unidadeDeTrabalho, localidadeCarregamento);
                this.GerarMunicipiosDeDescarregamentoPorNFeGlobalizada(ref mdfe, nfesGlobalizadas, unidadeDeTrabalho, null, true);
                this.GerarVeiculoPorObjeto(mdfe, veiculos, motoristas, reboques, unidadeDeTrabalho);
                this.GerarPercursos(mdfe, unidadeDeTrabalho);
                this.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);
                this.GerarValePedagioMDFe(mdfe, valesPedagio, unidadeDeTrabalho);
                this.GerarValePedagioPorVeiculoTracao(mdfe, unidadeDeTrabalho);
                this.GerarSeguro(mdfe, seguros, nfesGlobalizadas, unidadeDeTrabalho);
                this.GerarContratantesPorNFeGlobalizada(mdfe, nfesGlobalizadas, unidadeDeTrabalho);
                this.GerarCIOTPorVeiculoTracao(mdfe, unidadeDeTrabalho);

                GerarProdutoPredominante(mdfe, produtoPredominante, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), unidadeDeTrabalho);
                GerarCIOT(mdfe, listaCIOT, unidadeDeTrabalho);
                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(mdfe, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), informacoesPagamento, null);

                unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorObjeto(Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.MDFe.MDFe mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                unidadeDeTrabalho.Start();

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = this.GerarMDFe(empresa, mdfe, 0, unidadeDeTrabalho, false);

                this.GerarMunicipiosDeCarregamento(manifesto, mdfe.MunicipiosDeCarregamento, unidadeDeTrabalho);
                this.GerarMunicipiosDeDescarregamento(manifesto, mdfe.MunicipiosDeDescarregamento, unidadeDeTrabalho);
                this.GerarVeiculo(manifesto, mdfe.Veiculo, unidadeDeTrabalho);
                this.GerarReboques(manifesto, mdfe.Reboques, unidadeDeTrabalho);
                this.GerarMotoristas(manifesto, mdfe.Motoristas, unidadeDeTrabalho);
                this.GerarPercursos(manifesto, mdfe.Percursos, unidadeDeTrabalho);
                this.GerarLacres(manifesto, mdfe.Lacres, unidadeDeTrabalho);
                this.GerarValesPedagios(manifesto, mdfe.ValesPedagio, unidadeDeTrabalho);
                this.GerarObservacoesVeiculos(manifesto, unidadeDeTrabalho);
                this.GerarValePedagioPorVeiculoTracao(manifesto, unidadeDeTrabalho);

                this.GerarValePedagioPorCTes(manifesto, null, unidadeDeTrabalho);

                this.GerarContratantes(manifesto, mdfe.Contratantes, unidadeDeTrabalho);
                this.GerarCIOT(manifesto, mdfe.ListaCIOT, unidadeDeTrabalho);
                this.GerarSeguros(manifesto, mdfe.Seguros, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(manifesto.Versao) && manifesto.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(manifesto, unidadeDeTrabalho);
                    this.GerarSeguroPorCTes(manifesto, unidadeDeTrabalho);
                }

                Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante = new()
                {
                    CEAN = mdfe.ProdutoPredominanteCEAN,
                    DescProduto = mdfe.ProdutoPredominanteDescricao,
                    NCM = mdfe.ProdutoPredominanteNCM,
                    TipoCarga = ((int)(mdfe.TipoCargaMDFe ?? TipoCargaMDFe.CargaGeral)).ToString()
                };

                GerarProdutoPredominante(manifesto, produtoPredominante, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), unidadeDeTrabalho);
                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(manifesto, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), mdfe.InformacoesPagamento, null);

                unidadeDeTrabalho.CommitChanges();

                return manifesto;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorChaveCTe(Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.MDFe.MDFe mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                unidadeDeTrabalho.Start();

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = this.GerarMDFe(empresa, mdfe, 0, unidadeDeTrabalho, true);

                this.GerarMunicipiosDeCarregamento(manifesto, mdfe.MunicipiosDeCarregamento, unidadeDeTrabalho);
                this.GerarMunicipiosDeDescarregamento(manifesto, mdfe.MunicipiosDeDescarregamento, unidadeDeTrabalho, true);
                this.GerarVeiculo(manifesto, mdfe.Veiculo, unidadeDeTrabalho);
                this.GerarReboques(manifesto, mdfe.Reboques, unidadeDeTrabalho);
                this.GerarMotoristas(manifesto, mdfe.Motoristas, unidadeDeTrabalho);
                this.GerarPercursos(manifesto, mdfe.Percursos, unidadeDeTrabalho);
                this.GerarLacres(manifesto, mdfe.Lacres, unidadeDeTrabalho);
                this.GerarValesPedagios(manifesto, mdfe.ValesPedagio, unidadeDeTrabalho);
                this.GerarObservacoesVeiculos(manifesto, unidadeDeTrabalho);
                this.GerarValePedagioPorVeiculoTracao(manifesto, unidadeDeTrabalho);

                this.GerarContratantes(manifesto, mdfe.Contratantes, unidadeDeTrabalho);
                this.GerarCIOT(manifesto, mdfe.ListaCIOT, unidadeDeTrabalho);
                this.GerarSeguros(manifesto, mdfe.Seguros, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(manifesto.Versao) && manifesto.Versao == "3.00")
                {
                    this.GerarContratantesPorCTes(manifesto, unidadeDeTrabalho);
                    this.GerarSeguroPorCTes(manifesto, unidadeDeTrabalho);
                }

                Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante = new()
                {
                    CEAN = mdfe.ProdutoPredominanteCEAN,
                    DescProduto = mdfe.ProdutoPredominanteDescricao,
                    NCM = mdfe.ProdutoPredominanteNCM,
                    TipoCarga = ((int)(mdfe.TipoCargaMDFe ?? TipoCargaMDFe.CargaGeral)).ToString()
                };

                GerarProdutoPredominante(manifesto, produtoPredominante, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), unidadeDeTrabalho);
                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(manifesto, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), mdfe.InformacoesPagamento, null);

                unidadeDeTrabalho.CommitChanges();

                return manifesto;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFePorNotasFiscais(Dominio.Entidades.Empresa empresa, List<object> notasFiscais, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = this.GerarMDFe(empresa, notasFiscais, unidadeDeTrabalho);

                this.GerarMunicipiosDeCarregamento(ref mdfe, notasFiscais, unidadeDeTrabalho);
                this.GerarMunicipiosDeDescarregamento(ref mdfe, notasFiscais, unidadeDeTrabalho);
                Dominio.Entidades.Veiculo veiculo = null;
                this.GerarVeiculo(mdfe, notasFiscais, unidadeDeTrabalho, ref veiculo);
                this.GerarPercursos(mdfe, unidadeDeTrabalho);

                GerarProdutoPredominante(mdfe, produtoPredominante, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), unidadeDeTrabalho);
                GerarCIOT(mdfe, listaCIOT, unidadeDeTrabalho);
                new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeDeTrabalho).GerarDadosPagamento(mdfe, new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(), informacoesPagamento, veiculo);

                unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public bool Emitir(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                return true;

            bool retorno = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFe.EmitirMDFe(mdfe, mdfe.Empresa.Codigo, unitOfWork);

            if (!retorno && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().ReenviarErroIntegracaoMDFe.Value)
            {
                Servicos.Log.TratarErro("MDF-e codigo " + mdfe.Codigo.ToString() + ": Reenviado problema integração", "ReenvioMDFe");

                System.Threading.Thread.Sleep(2000);
                retorno = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFe.EmitirMDFe(mdfe, mdfe.Empresa.Codigo, unitOfWork);

                if (!retorno)
                {
                    System.Threading.Thread.Sleep(2000);
                    retorno = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFe.EmitirMDFe(mdfe, mdfe.Empresa.Codigo, unitOfWork);
                }

                if (!retorno)
                {
                    System.Threading.Thread.Sleep(2000);
                    retorno = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFe.EmitirMDFe(mdfe, mdfe.Empresa.Codigo, unitOfWork);
                }
            }

            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoMDFe(mdfe, unitOfWork);

            this.SalvarIntegracaoRetornoMDFe(mdfe, mdfe.Empresa, unitOfWork);

            return retorno;
        }

        public bool EmitirMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            mdfe = repMDFe.BuscarPorCodigo(mdfe.Codigo);
            mdfe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador;

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            this.EncerrarMDFesAnteriores(mdfe, empresa.Codigo, unitOfWork);

            ServicoMDFe.MDFe mdfeIntegrar = ObterMDFeParaEmissaoAsync(mdfe, empresa).GetAwaiter().GetResult();

            try
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarMDFe(mdfeIntegrar);

                if (retorno.Valor <= 0)
                {
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                    repMDFe.Atualizar(mdfe);

                    Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                    return false;
                }
                else
                {
                    mdfe.CodigoIntegradorAutorizacao = retorno.Valor;
                    mdfe.MensagemRetornoSefaz = "MDF-e em processamento.";
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Enviado;
                    mdfe.DataIntegracao = DateTime.Now;
                    repMDFe.Atualizar(mdfe);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                mdfe.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                repMDFe.Atualizar(mdfe);


                Email svcEmail = new Servicos.Email(unitOfWork);

                string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                string assunto = ambiente + " - Problemas na emissão de MDF-e!";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Atenção, problemas na emissão de MDF-e no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                sb.Append("MDFe ").Append(mdfe.Numero).Append("/").Append(mdfe.Serie.Numero.ToString()).Append(" transportador ").Append(mdfe.Empresa.CNPJ).Append(" ").Append(mdfe.Empresa.RazaoSocial).Append("<br /> <br />");
                sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, null, false);
#else
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "cesar@multisoftware.com.br", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, null, false);
#endif

                return false;
            }
        }

        public bool RemoverPendenciaMDFeCarga(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return false;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFe(mdfe.Codigo);
            if (cargaMDFe == null || cargaMDFe.Carga == null)
                return false;

            if (cargaMDFe.Carga.PossuiPendencia && cargaMDFe.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
            {
                cargaMDFe.Carga.PossuiPendencia = false;
                cargaMDFe.Carga.problemaMDFe = false;
                cargaMDFe.Carga.MotivoPendencia = "";
                repCarga.Atualizar(cargaMDFe.Carga);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaMDFe.Carga, null, "Solicitou Emissão do MDF-e pelo Controle Emissão MDFe", unitOfWork);
            }

            return true;
        }

        public bool EncerrarMDFesAnteriores(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.EncerramentoMDFeAutomatico == Dominio.Enumeradores.EncerramentoMDFeAutomatico.Nenhum)
                    return true;

                bool encerrarMDFeComMesmaData = (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EncerraMDFeAutomaticoComMesmaData.Value || mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.EncerrarMDFeComMesmaData);

                bool configEncerraMDFeAntesDaEmissao = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EncerraMDFeAntesDaEmissao.Value;

                if (configEncerraMDFeAntesDaEmissao)
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                    Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeEncerrado = null;
                    Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);

                    if (veiculoMDFe != null)
                    {
                        DateTime dataReferencia = DateTime.Today;
                        if (encerrarMDFeComMesmaData)
                            dataReferencia = DateTime.Now.AddHours(-1); //Quando configurado para encerrar cargas do mesmo dia utiliza como referência 1 hora a menos que a atual


                        //Encerrar MDFe com mesma placa para mesma UF de Descarregamento: 611 - Rejeição: Existe MDF-e não encerrado para esta placa, tipo de emitente e UF descarregamento
                        List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFesPlaca = repMDFe.BuscarPendentesEncerramentosRaiz(mdfe.Empresa.CNPJ.Substring(0, 8), veiculoMDFe.Placa, dataReferencia, 0, "", mdfe.EstadoDescarregamento.Sigla); //mdfe.EstadoCarregamento.Sigla Removido estado origem
                        for (var i = 0; i < listaMDFesPlaca.Count; i++)
                        {
                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                            DateTime dataEncerramento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                            if (fusoHorarioEmpresa.BaseUtcOffset != TimeZoneInfo.Local.BaseUtcOffset)
                                dataEncerramento = TimeZoneInfo.ConvertTime(dataEncerramento, TimeZoneInfo.Local, fusoHorarioEmpresa);

                            mdfeEncerrado = repMDFe.BuscarPorCodigo(listaMDFesPlaca[i].Codigo);
                            if (mdfeEncerrado != null)
                            {
                                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfeEncerrado.SistemaEmissor).EncerrarMdfe(mdfeEncerrado.Codigo, mdfeEncerrado.Empresa.Codigo, dataEncerramento, unitOfWork, dataEncerramento))
                                {
                                    this.AdicionarMDFeNaFilaDeConsulta(mdfeEncerrado, unitOfWork);
                                    this.SalvarLogEncerramentoMDFe(mdfeEncerrado.Chave, mdfeEncerrado.Protocolo, dataEncerramento, mdfeEncerrado.Empresa, mdfe.Empresa.Localidade, "MDFe encerrado antes da emissão do MDFe com mesma placa e UF descarregamento: " + mdfe.Chave, unitOfWork);

                                    var tentativas = 0;
                                    mdfeEncerrado = repMDFe.BuscarPorCodigo(mdfeEncerrado.Codigo);
                                    while (mdfeEncerrado.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento && tentativas < 5)
                                    {
                                        System.Threading.Thread.Sleep(3000);

                                        unitOfWork.Clear(mdfeEncerrado);
                                        mdfeEncerrado = repMDFe.BuscarPorCodigo(mdfeEncerrado.Codigo);
                                        tentativas = tentativas + 1;
                                    }
                                }
                            }
                        }

                        //Encerrar MDFe com mesma placa e sentido o posto: 662 - Rejeição: Existe MDF-e não encerrado para esta placa, tipo de emitente no sentido oposto da viagem
                        List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFesPlacaDestinoOposto = repMDFe.BuscarPendentesEncerramentosRaiz(mdfe.Empresa.CNPJ.Substring(0, 8), veiculoMDFe.Placa, dataReferencia, 0, mdfe.EstadoDescarregamento.Sigla, mdfe.EstadoCarregamento.Sigla);
                        for (var i = 0; i < listaMDFesPlacaDestinoOposto.Count; i++)
                        {
                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                            DateTime dataEncerramento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                            if (fusoHorarioEmpresa.BaseUtcOffset != TimeZoneInfo.Local.BaseUtcOffset)
                                dataEncerramento = TimeZoneInfo.ConvertTime(dataEncerramento, TimeZoneInfo.Local, fusoHorarioEmpresa);

                            mdfeEncerrado = repMDFe.BuscarPorCodigo(listaMDFesPlacaDestinoOposto[i].Codigo);
                            if (mdfeEncerrado != null)
                            {
                                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfeEncerrado.SistemaEmissor).EncerrarMdfe(mdfeEncerrado.Codigo, mdfeEncerrado.Empresa.Codigo, dataEncerramento, unitOfWork, dataEncerramento))
                                {
                                    this.AdicionarMDFeNaFilaDeConsulta(mdfeEncerrado, unitOfWork);
                                    this.SalvarLogEncerramentoMDFe(mdfeEncerrado.Chave, mdfeEncerrado.Protocolo, dataEncerramento, mdfeEncerrado.Empresa, mdfe.Empresa.Localidade, "MDFe encerrado antes da emissão do MDFe com mesma placa e sentido oposto: " + mdfe.Chave, unitOfWork);

                                    var tentativas = 0;
                                    mdfeEncerrado = repMDFe.BuscarPorCodigo(mdfeEncerrado.Codigo);
                                    while (mdfeEncerrado.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento && tentativas < 10)
                                    {
                                        System.Threading.Thread.Sleep(3000);

                                        unitOfWork.Clear(mdfeEncerrado);
                                        mdfeEncerrado = repMDFe.BuscarPorCodigo(mdfeEncerrado.Codigo);
                                        tentativas = tentativas + 1;
                                    }
                                }
                            }
                        }

                        //Encerrar MDFe para mesma placa com até dois percursos e mais de 5 dias de emissão: 611 - Rejeição: Existe MDF-e não encerrado para esta placa, tipo de emitente e UF descarregamento
                        List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes5dias = repMDFe.BuscarPendentesEncerramentosRaiz(mdfe.Empresa.CNPJ.Substring(0, 8), veiculoMDFe.Placa, DateTime.Now.AddDays(-5), 2, "", "");
                        for (var i = 0; i < listaMDFes5dias.Count; i++)
                        {
                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                            DateTime dataEncerramento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                            if (fusoHorarioEmpresa.BaseUtcOffset != TimeZoneInfo.Local.BaseUtcOffset)
                                dataEncerramento = TimeZoneInfo.ConvertTime(dataEncerramento, TimeZoneInfo.Local, fusoHorarioEmpresa);

                            mdfeEncerrado = repMDFe.BuscarPorCodigo(listaMDFes5dias[i].Codigo);

                            if (mdfeEncerrado != null)
                            {
                                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfeEncerrado.SistemaEmissor).EncerrarMdfe(mdfeEncerrado.Codigo, mdfeEncerrado.Empresa.Codigo, dataEncerramento, unitOfWork, dataEncerramento))
                                {
                                    this.AdicionarMDFeNaFilaDeConsulta(mdfeEncerrado, unitOfWork);
                                    this.SalvarLogEncerramentoMDFe(mdfeEncerrado.Chave, mdfeEncerrado.Protocolo, dataEncerramento, mdfeEncerrado.Empresa, mdfeEncerrado.Empresa.Localidade, "MDFe emitido a mais de 5 dias encerrado antes da emissão do MDFe: " + mdfe.Chave, unitOfWork);

                                    var tentativas = 0;
                                    mdfeEncerrado = repMDFe.BuscarPorCodigo(mdfeEncerrado.Codigo);
                                    while (mdfeEncerrado.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento && tentativas < 10)
                                    {
                                        System.Threading.Thread.Sleep(3000);

                                        unitOfWork.Clear(mdfeEncerrado);
                                        mdfeEncerrado = repMDFe.BuscarPorCodigo(mdfeEncerrado.Codigo);
                                        tentativas = tentativas + 1;
                                    }
                                }
                            }
                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao encerrar mdfes anteriores: " + ex);

                return false;
            }
        }

        public void ConsultarDuplicidade(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);

            string chaveMDFe = this.GerarChaveMDFe(mdfe);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            ServicoMDFe.RetornoMDFe retorno = svcMDFe.ConsultarMDFePorChave(chaveMDFe);

            if (retorno.CodigoRetornoSefaz == 100)
            {
                mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                mdfe.Protocolo = retorno.Protocolo;
                mdfe.Chave = retorno.Chave;
                mdfe.DataAutorizacao = retorno.DataRetornoSefaz != DateTime.MinValue ? retorno.DataRetornoSefaz : DateTime.Now;
                mdfe.CodigoIntegradorAutorizacao = retorno.CodigoInternoMDFe;
            }
            else
            {
                mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
            }

            mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);
        }

        public Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave ConsultarStatusMDFePorChave(string chaveMDFe)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chaveMDFe))
                    return null;

                string stringConexao = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("ControleCTe");
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                Servicos.Embarcador.Integracao.ConfiguracaoWebService configuracaoWebService = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork);
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = configuracaoWebService.ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                ServicoMDFe.RetornoMDFe retorno = svcMDFe.ConsultarMDFePorChave(chaveMDFe);

                if (retorno == null)
                    return null;

                return new Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave
                {
                    CodigoStatus = retorno.CodigoRetornoSefaz.ToString(),
                    MensagemStatus = !string.IsNullOrWhiteSpace(retorno.MensagemRetornoSefaz)
                        ? retorno.MensagemRetornoSefaz
                        : !string.IsNullOrWhiteSpace(retorno.DescricaoStatus) ? retorno.DescricaoStatus : retorno.Info != null ? retorno.Info.Mensagem : string.Empty
                };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
        }

        public void Consultar(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao = "")
        {
            unidadeDeTrabalho = unidadeDeTrabalho != null ? unidadeDeTrabalho : new Repositorio.UnitOfWork(StringConexao);

            bool encerramentoAutomatico = false;
            string chaveMDFe = string.Empty;
            string protocoloMDFe = string.Empty;
            string mensagemRetorno = string.Empty;
            bool duplicidade = false;

            try
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                //Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Pendente)
                {
                    ServicoMDFe.RetornoMDFe retorno = null;

                    if (mdfe.CodigoIntegradorAutorizacao != 0)
                        retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);
                    else
                        retorno = svcMDFe.ConsultarMDFePorChave(GerarChaveMDFe(mdfe));

                    unidadeDeTrabalho.Start();

                    Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (retorno.Status != "I")
                        SalvarRetornoSefaz(mdfe, "A", retorno.CodigoInternoMDFe, retorno.CodigoRetornoSefaz, Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz), unidadeDeTrabalho);

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("R"))
                        {
                            this.TratarRetornoRejeicao(ref mdfe, retorno, ref encerramentoAutomatico, ref chaveMDFe, ref protocoloMDFe, ref duplicidade, ref mensagemRetorno, unidadeDeTrabalho);
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Enviado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else if (retorno.Status == "M" || retorno.Status == "D" || retorno.Status == "P")
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                            mdfe.Protocolo = retorno.Protocolo;
                            mdfe.Chave = retorno.Chave;
                            mdfe.DataAutorizacao = retorno.DataRetornoSefaz != DateTime.MinValue ? retorno.DataRetornoSefaz : DateTime.Now;

                            this.ObterESalvarXMLAutorizacao(mdfe, mdfe.Empresa.Codigo, unidadeDeTrabalho, retorno);
                        }
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                    }

                    if (mdfe.CodigoIntegradorAutorizacao == 0 && retorno.CodigoInternoMDFe > 0)
                        mdfe.CodigoIntegradorAutorizacao = retorno.CodigoInternoMDFe;

                    repMDFe.Atualizar(mdfe);

                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        this.ObterESalvarDAMDFE(mdfe, duplicidade ? null : retorno, unidadeDeTrabalho);

                    unidadeDeTrabalho.CommitChanges();
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia)
                {
                    ServicoMDFe.RetornoMDFe retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        unidadeDeTrabalho.Start();

                        Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                        mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                        if (retorno.Status == "M" || retorno.Status == "D")
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                            mdfe.Protocolo = retorno.Protocolo;
                            mdfe.Chave = retorno.Chave;
                            mdfe.DataAutorizacao = retorno.DataRetornoSefaz != DateTime.MinValue ? retorno.DataRetornoSefaz : DateTime.Now;

                            this.ObterESalvarXMLAutorizacao(mdfe, mdfe.Empresa.Codigo, unidadeDeTrabalho, retorno);

                            repMDFe.Atualizar(mdfe);

                            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                                this.ObterESalvarDAMDFE(mdfe, duplicidade ? null : retorno, unidadeDeTrabalho);
                        }
                        else if (retorno.Status == "R")
                        {
                            this.TratarRetornoRejeicao(ref mdfe, retorno, ref encerramentoAutomatico, ref chaveMDFe, ref protocoloMDFe, ref duplicidade, ref mensagemRetorno, unidadeDeTrabalho);

                            repMDFe.Atualizar(mdfe);
                        }

                        unidadeDeTrabalho.CommitChanges();
                    }
                }

                if (encerramentoAutomatico)
                {
                    Repositorio.UnitOfWork unidadeDeTrabalho2 = new Repositorio.UnitOfWork(stringConexao);
                    try
                    {
                        if (this.SolicitarEncerramentoAutomaticoChave(mdfe.Empresa.Codigo, mdfe.Chave, chaveMDFe, protocoloMDFe, unidadeDeTrabalho2))
                        {
                            this.Emitir(mdfe, unidadeDeTrabalho2);
                        }
                        else
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                            mdfe.MensagemRetornoSefaz = mensagemRetorno;
                            repMDFe.Atualizar(mdfe);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Encerramento automático MDFe: " + ex);
                    }
                    finally
                    {
                        unidadeDeTrabalho2.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public void TratarRetornoRejeicao(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, ServicoMDFe.RetornoMDFe retorno, ref bool encerramentoAutomatico, ref string chaveMDFe, ref string protocoloMDFe, ref bool duplicidade, ref string mensagemRetorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                mensagemRetorno = string.Empty;
                bool configEncerraMDFeAutomatico = !Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EncerraMDFeAutomatico.HasValue || Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EncerraMDFeAutomatico.Value;

                bool configEnviaContingenciaMDFeAutomatico = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EnviaContingenciaMDFeAutomatico.HasValue && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EnviaContingenciaMDFeAutomatico.Value;

                if (configEncerraMDFeAutomatico && mdfe.TentativaEncerramentoAutomatico < 4 && (retorno.CodigoRetornoSefaz == 610 || retorno.CodigoRetornoSefaz == 686 || retorno.CodigoRetornoSefaz == 611 || retorno.CodigoRetornoSefaz == 462 || retorno.CodigoRetornoSefaz == 662))
                {
                    if (ExtrairChaveEProtocoloDoRetornoSefaz(retorno.MensagemRetornoSefaz, ref chaveMDFe, ref protocoloMDFe))
                    {
                        mdfe.TentativaEncerramentoAutomatico = mdfe.TentativaEncerramentoAutomatico + 1;
                        encerramentoAutomatico = true;

                        mensagemRetorno = "MDF-e pendente de encerramento: [Chave: " + chaveMDFe + " Protocolo: " + protocoloMDFe + " CNPJ Emissor: " + chaveMDFe.Substring(6, 14) + " Número: " + chaveMDFe.Substring(25, 9) + " Série: " + chaveMDFe.Substring(22, 3) + "]";
                        SalvarRetornoSefaz(mdfe, "A", retorno.CodigoInternoMDFe, retorno.CodigoRetornoSefaz, "MDF-e pendente de encerramento: [Chave: " + chaveMDFe + " Protocolo: " + protocoloMDFe + " CNPJ Emissor: " + chaveMDFe.Substring(6, 14) + " Número: " + chaveMDFe.Substring(25, 9) + " Série: " + chaveMDFe.Substring(22, 3) + "]", unidadeDeTrabalho);
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                    }
                }
                else
                {
                    if (retorno.CodigoRetornoSefaz == 204)
                    {
                        this.ConsultarDuplicidade(ref mdfe, unidadeDeTrabalho);

                        duplicidade = true;
                    }
                    else if (configEnviaContingenciaMDFeAutomatico && (retorno.CodigoRetornoSefaz == 108 || retorno.CodigoRetornoSefaz == 109 || retorno.CodigoRetornoSefaz == 8888))
                    {
                        if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).SolicitarEmissaoContingencia(mdfe.Codigo, unidadeDeTrabalho))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmitidoContingencia;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));

                            SalvarRetornoSefaz(mdfe, "O", 0, 0, "MDFe emitido em contingência automáticamente, retorno sefaz " + string.Concat(retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz), unidadeDeTrabalho);
                        }
                        else
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public void TratarRetornoRejeicao(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle, ref bool encerramentoAutomatico, ref string chaveMDFe, ref string protocoloMDFe, ref bool duplicidade, ref string mensagemRetorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                bool configEncerraMDFeAutomatico = !Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EncerraMDFeAutomatico.HasValue || Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EncerraMDFeAutomatico.Value;

                bool configEnviaContingenciaMDFeAutomatico = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EnviaContingenciaMDFeAutomatico.HasValue && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().EnviaContingenciaMDFeAutomatico.Value;

                int statusMDFe = 0;
                int.TryParse(string.IsNullOrWhiteSpace(mdfeOracle.CodStatusProtocolo) ? mdfeOracle.CodStatusEnvio : mdfeOracle.CodStatusProtocolo, out statusMDFe);

                if (configEncerraMDFeAutomatico && !mdfe.Importado && mdfe.TentativaEncerramentoAutomatico < 6 && (statusMDFe == 610 || statusMDFe == 686 || statusMDFe == 611 || statusMDFe == 462 || statusMDFe == 662))
                {
                    if (ExtrairChaveEProtocoloDoRetornoSefaz(string.IsNullOrWhiteSpace(mdfeOracle.DescricaoProtocolo) ? mdfeOracle.DescricaoEnvio : mdfeOracle.DescricaoProtocolo, ref chaveMDFe, ref protocoloMDFe))
                    {
                        mdfe.TentativaEncerramentoAutomatico = mdfe.TentativaEncerramentoAutomatico + 1;

                        mensagemRetorno = "MDF-e pendente de encerramento: [Chave: " + chaveMDFe + " Protocolo: " + protocoloMDFe + " CNPJ Emissor: " + chaveMDFe.Substring(6, 14) + " Número: " + chaveMDFe.Substring(25, 9) + " Série: " + chaveMDFe.Substring(22, 3) + "]";

                        encerramentoAutomatico = true;
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                }
                else
                {
                    if (statusMDFe == 204)
                    {
                        this.ConsultarDuplicidade(ref mdfe, unidadeDeTrabalho);

                        duplicidade = true;
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public bool Cancelar(int codigoMDFe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork = null, DateTime? dataCancelamento = null, string cobrarCancelamento = "")
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

            ServicoMDFe.EventoCancelamentoMDFe evento = new ServicoMDFe.EventoCancelamentoMDFe();

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            if (fusoHorarioEmpresa.BaseUtcOffset != TimeZoneInfo.Local.BaseUtcOffset)
                dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            if (dataFuso < mdfe.DataAutorizacao && mdfe.DataAutorizacao.HasValue)
                dataFuso = mdfe.DataAutorizacao.Value.AddMinutes(1);

            evento.Ambiente = mdfe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? ServicoMDFe.TipoAmbiente.Producao : ServicoMDFe.TipoAmbiente.Homologacao;
            evento.CodigoInternoMDFe = mdfe.CodigoIntegradorAutorizacao;
            evento.DataEmissao = dataCancelamento.HasValue ? dataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : dataFuso.ToString("dd/MM/yyyy HH:mm:ss");
            evento.Empresa = this.ObterEmpresaEmitente(empresa);
            evento.Justificativa = justificativa;
            evento.NumeroSequencialEvento = 1;

            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dataCancelamento.HasValue ? dataCancelamento.Value : dataFuso);
            evento.FusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            try
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarEventoCancelamento(evento);

                if (retorno.Valor <= 0)
                {
                    if (retorno.Info.Mensagem == "MDF-e não encontrado")
                    {
                        if (IntegrareMDFeOracleAsync(mdfe.Empresa, mdfe.Codigo).GetAwaiter().GetResult())
                        {
                            mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);
                            evento.CodigoInternoMDFe = mdfe.CodigoIntegradorAutorizacao;

                            ServicoMDFe.ResultadoInteger retorno2 = svcMDFe.ImportarEventoCancelamento(evento);

                            if (retorno2.Valor <= 0)
                            {
                                mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno2.Info.Mensagem);
                                mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                                repMDFe.Atualizar(mdfe);

                                Servicos.Log.TratarErro(retorno2.Info.MensagemOriginal);

                                return false;
                            }
                            else
                            {
                                mdfe.CodigoIntegradorCancelamento = retorno2.Valor;
                                mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                                mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmCancelamento;
                                mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                                repMDFe.Atualizar(mdfe);

                                return true;
                            }

                        }
                        else
                        {
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                            mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                            repMDFe.Atualizar(mdfe);

                            Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                            return false;
                        }
                    }
                    else
                    {
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                        mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                        repMDFe.Atualizar(mdfe);

                        Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                        return false;
                    }
                }
                else
                {
                    mdfe.CodigoIntegradorCancelamento = retorno.Valor;
                    mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmCancelamento;
                    mdfe.CobrarCancelamento = cobrarCancelamento == "Sim";
                    repMDFe.Atualizar(mdfe);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mdfe.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                repMDFe.Atualizar(mdfe);

                return false;
            }
        }

        public async Task<string> CancelarMDFeImportadoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TProcEvento procEvento, Stream xml, string strXML = null, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork, _cancellationToken);
            Repositorio.XMLMDFe repositorioXMLMDFe = new Repositorio.XMLMDFe(_unitOfWork, _cancellationToken);

            if (mdfe == null)
                return $"MDF-e não encontrado.";

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.MDFe.v300.evCancMDFe));
            byte[] data = Encoding.Default.GetBytes(procEvento.eventoMDFe.infEvento.detEvento.Any.OuterXml);

            using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length))
            {
                MultiSoftware.MDFe.v300.evCancMDFe evCancMDFe = (MultiSoftware.MDFe.v300.evCancMDFe)serializer.Deserialize(memStream);

                mdfe.JustificativaCancelamento = evCancMDFe.xJust;
            }

            string dateFormat = "yyyy-MM-ddTHH:mm:ss" + (procEvento.retEventoMDFe.infEvento.dhRegEvento.Length > 19 ? "zzz" : "");
            DateTime.TryParseExact(procEvento.retEventoMDFe.infEvento.dhRegEvento, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dataEvento);

            mdfe.ProtocoloCancelamento = procEvento.retEventoMDFe.infEvento.nProt;
            mdfe.DataCancelamento = dataEvento;
            mdfe.DataIntegracao = dataEvento;
            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Cancelado;
            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(procEvento.retEventoMDFe.infEvento.xMotivo);
            mdfe.Log += $" / Cancelamento importado com sucesso em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.";

            await repositorioMDFe.AtualizarAsync(mdfe);

            if (auditado != null)
                await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, mdfe, "MDF-e cancelado via importação do XML de cancelamento.", _unitOfWork, _cancellationToken);

            Dominio.Entidades.XMLMDFe xmlMDFe = await repositorioXMLMDFe.BuscarPorMDFeAsync(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento, _cancellationToken);

            if (xmlMDFe == null)
            {
                if (xml != null)
                {
                    using (StreamReader reader = new StreamReader(xml))
                    {
                        xml.Position = 0;

                        strXML = await reader.ReadToEndAsync();
                    }
                }

                xmlMDFe = new Dominio.Entidades.XMLMDFe()
                {
                    MDFe = mdfe,
                    Tipo = Dominio.Enumeradores.TipoXMLMDFe.Cancelamento,
                    XML = strXML
                };

                await repositorioXMLMDFe.InserirAsync(xmlMDFe);
            }

            return string.Empty;
        }

        public async Task<string> EncerrarMDFeImportadoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TProcEvento procEvento, Stream xml, string strXML = null, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork, _cancellationToken);
            Repositorio.XMLMDFe repositorioXMLMDFe = new Repositorio.XMLMDFe(_unitOfWork, _cancellationToken);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork, _cancellationToken);

            if (mdfe == null)
                return $"MDF-e não encontrado.";

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.MDFe.v300.evEncMDFe));
            byte[] data = Encoding.Default.GetBytes(procEvento.eventoMDFe.infEvento.detEvento.Any.OuterXml);

            using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length))
            {
                MultiSoftware.MDFe.v300.evEncMDFe evEncMDFe = (MultiSoftware.MDFe.v300.evEncMDFe)serializer.Deserialize(memStream);

                mdfe.MunicipioEncerramento = await repositorioLocalidade.BuscarPorCodigoIBGEAsync(evEncMDFe.cMun.ToInt(), _cancellationToken);
            }

            string dateFormat = "yyyy-MM-ddTHH:mm:ss" + (procEvento.retEventoMDFe.infEvento.dhRegEvento.Length > 19 ? "zzz" : "");
            DateTime.TryParseExact(procEvento.retEventoMDFe.infEvento.dhRegEvento, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dataEvento);

            mdfe.ProtocoloEncerramento = procEvento.retEventoMDFe.infEvento.nProt;
            mdfe.DataEncerramento = dataEvento;
            mdfe.DataIntegracao = dataEvento;
            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Encerrado;
            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(procEvento.retEventoMDFe.infEvento.xMotivo);
            mdfe.Log += $" / Encerramento importado com sucesso em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.";

            await repositorioMDFe.AtualizarAsync(mdfe);

            this.AtualizarIntegracaoMDFeManual(mdfe, _unitOfWork);

            if (auditado != null)
                Servicos.Auditoria.Auditoria.AuditarAsync(auditado, mdfe, "MDF-e encerrado via importação do XML de encerramento.", _unitOfWork, _cancellationToken);

            Dominio.Entidades.XMLMDFe xmlMDFe = await repositorioXMLMDFe.BuscarPorMDFeAsync(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Encerramento, _cancellationToken);

            if (xmlMDFe == null)
            {
                if (xml != null)
                {
                    using (StreamReader reader = new StreamReader(xml))
                    {
                        xml.Position = 0;

                        strXML = await reader.ReadToEndAsync();
                    }
                }

                xmlMDFe = new Dominio.Entidades.XMLMDFe()
                {
                    MDFe = mdfe,
                    Tipo = Dominio.Enumeradores.TipoXMLMDFe.Encerramento,
                    XML = strXML
                };

                await repositorioXMLMDFe.InserirAsync(xmlMDFe);
            }

            return string.Empty;
        }

        public bool EncerrarMDFeEmissorExterno(Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoMDFe.EventoEncerramentoManualMDFe evento = new Servicos.ServicoMDFe.EventoEncerramentoManualMDFe()
            {
                Ambiente = mdfeEmissorExterno.Ambiente == TipoAmbiente.Producao ? Servicos.ServicoMDFe.TipoAmbiente.Producao : Servicos.ServicoMDFe.TipoAmbiente.Homologacao,
                Chave = mdfeEmissorExterno.Chave,
                CodigoMunicipioEncerramento = string.Format("{0:0000000}", mdfeEmissorExterno.CodigoMunicipioEncerramento),
                CodigoUFEncerramento = string.Format("{0:00}", mdfeEmissorExterno.CodigoUFEncerramento),
                DataEncerramento = mdfeEmissorExterno.DataEncerramento.ToDateTimeString(true),
                DataEvento = mdfeEmissorExterno.DataEvento.ToDateTimeString(true),
                Empresa = ObterEmpresaEmitente(mdfeEmissorExterno.Empresa),
                Protocolo = mdfeEmissorExterno.Protocolo,
                FusoHorario = mdfeEmissorExterno.FusoHorario
            };

            using (ServicoMDFe.uMDFeServiceTSSoapClient wsMDFe = new Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.ServicoMDFe.uMDFeServiceTSSoapClient, Servicos.ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS))
            {
                ServicoMDFe.ResultadoInteger retorno = wsMDFe.ImportarEventoEncerramentoManual(evento);

                return retorno.Valor > 0;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarCancelamento(int codigoMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho = unidadeDeTrabalho != null ? unidadeDeTrabalho : new Repositorio.UnitOfWork(StringConexao);

            try
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                {
                    ServicoMDFe.RetornoEventoMDFe retorno = svcMDFe.ConsultarEventoMDFe(mdfe.CodigoIntegradorCancelamento);

                    unidadeDeTrabalho.Start();

                    Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (retorno.Status != "I")
                        SalvarRetornoSefaz(mdfe, "C", 0, retorno.CodigoRetornoSefaz, Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz), unidadeDeTrabalho);

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("R"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmCancelamento;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Cancelado;
                            //mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                            mdfe.MensagemRetornoSefaz = !string.IsNullOrWhiteSpace(Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz)) ? Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz) : Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                            mdfe.ProtocoloCancelamento = retorno.Protocolo;
                            mdfe.DataCancelamento = retorno.DataRetornoSefaz != DateTime.MinValue ? retorno.DataRetornoSefaz : DateTime.Now;

                            this.ObterESalvarXMLCancelamento(mdfe, unidadeDeTrabalho, retorno);
                        }
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                    }

                    repMDFe.Atualizar(mdfe);
                }

                unidadeDeTrabalho.CommitChanges();

                return mdfe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public bool Encerrar(int codigoMDFe, int codigoEmpresa, DateTime dataEncerramento, Repositorio.UnitOfWork unidadeDeTrabalho = null, DateTime? dataEvento = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.MunicipioDescarregamentoMDFe repMunDescMDFe = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
            List<Dominio.Entidades.MunicipioDescarregamentoMDFe> municipioDescarregamentoMDFe = repMunDescMDFe.BuscarPorMDFe(codigoMDFe);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            ServicoMDFe.EventoEncerramentoMDFe evento = new ServicoMDFe.EventoEncerramentoMDFe();

            evento.Ambiente = mdfe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? ServicoMDFe.TipoAmbiente.Producao : ServicoMDFe.TipoAmbiente.Homologacao;
            evento.CodigoInternoMDFe = mdfe.CodigoIntegradorAutorizacao;
            if (mdfe.MunicipioEncerramento != null && mdfe.MunicipioEncerramento.CodigoIBGE > 0)
                evento.CodigoMunicipioEncerramento = mdfe.MunicipioEncerramento.CodigoIBGE;
            else
                evento.CodigoMunicipioEncerramento = municipioDescarregamentoMDFe != null && municipioDescarregamentoMDFe.Count > 0 && municipioDescarregamentoMDFe.FirstOrDefault().Municipio != null ? municipioDescarregamentoMDFe.FirstOrDefault().Municipio.CodigoIBGE : empresa.Localidade.CodigoIBGE;

            if (evento.CodigoMunicipioEncerramento == 9999999)
                evento.CodigoMunicipioEncerramento = empresa.Localidade.CodigoIBGE;

            evento.DataEmissao = dataEvento.HasValue ? dataEvento.Value.ToString("dd/MM/yyyy HH:mm:ss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            evento.DataEncerramento = dataEncerramento.ToString("dd/MM/yyyy HH:mm:ss");
            evento.Empresa = this.ObterEmpresaEmitente(empresa);
            evento.NumeroSequencialEvento = 1;

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dataEvento.HasValue ? dataEvento.Value : DateTime.Now);
            evento.FusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            if (mdfe.MunicipioEncerramento != null && mdfe.MunicipioEncerramento.Estado != null)
                evento.UFEncerramento = string.Format("{0:00}", mdfe.MunicipioEncerramento.Estado.CodigoIBGE);
            else
                evento.UFEncerramento = string.Format("{0:00}", municipioDescarregamentoMDFe.FirstOrDefault().Municipio.Estado.CodigoIBGE);

            if (evento.UFEncerramento == "00" || evento.UFEncerramento == "99")
                evento.UFEncerramento = string.Format("{0:00}", empresa.Localidade.Estado.CodigoIBGE);

            try
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarEventoEncerramento(evento);

                if (retorno.Valor <= 0)
                {
                    if (retorno.Info.Mensagem == "MDF-e não encontrado")
                    {
                        if (IntegrareMDFeOracleAsync(mdfe.Empresa, mdfe.Codigo).GetAwaiter().GetResult())
                        {
                            mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);
                            evento.CodigoInternoMDFe = mdfe.CodigoIntegradorAutorizacao;

                            ServicoMDFe.ResultadoInteger retorno2 = svcMDFe.ImportarEventoEncerramento(evento);

                            if (retorno2.Valor <= 0)
                            {
                                mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno2.Info.Mensagem);
                                repMDFe.Atualizar(mdfe);

                                Servicos.Log.TratarErro(retorno2.Info.MensagemOriginal);

                                return false;
                            }
                            else
                            {
                                mdfe.CodigoIntegradorEncerramento = retorno2.Valor;
                                mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                                mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmEncerramento;
                                repMDFe.Atualizar(mdfe);

                                return true;
                            }

                        }
                        else
                        {
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                            repMDFe.Atualizar(mdfe);

                            Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                            return false;
                        }
                    }
                    else
                    {
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                        repMDFe.Atualizar(mdfe);

                        Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                        return false;
                    }
                }
                else
                {
                    mdfe.CodigoIntegradorEncerramento = retorno.Valor;
                    mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmEncerramento;
                    repMDFe.Atualizar(mdfe);

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoEncerramentoMDFe(mdfe, unidadeDeTrabalho);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mdfe.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                repMDFe.Atualizar(mdfe);

                return false;
            }
        }

        public bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, Repositorio.UnitOfWork unidadeDeTrabalho = null, DateTime? dataEvento = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            ServicoMDFe.EventoInclusaoMotoristaMDFe evento = new ServicoMDFe.EventoInclusaoMotoristaMDFe();

            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeDeTrabalho);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Embarcador.Carga.CargaMotorista(unidadeDeTrabalho);

            Dominio.Entidades.Usuario entidadeMotorista = repositorioUsuario.BuscarPorCPF(cpfMotorista);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repositorioCargaMDFe.BuscarPorMDFe(codigoMDFe);

            evento.Ambiente = mdfe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? ServicoMDFe.TipoAmbiente.Producao : ServicoMDFe.TipoAmbiente.Homologacao;
            evento.CodigoInternoMDFe = mdfe.CodigoIntegradorAutorizacao;
            evento.DataEmissao = dataEvento.HasValue ? dataEvento.Value.ToString("dd/MM/yyyy HH:mm:ss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            evento.Empresa = this.ObterEmpresaEmitente(empresa);
            evento.CPFMotorista = cpfMotorista;
            evento.NomeMotorista = nomeMotorista;
            evento.NumeroSequencialEvento = this.BuscarSequencialEventoMDFe(codigoMDFe, unidadeDeTrabalho);

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dataEvento.HasValue ? dataEvento.Value : DateTime.Now);
            evento.FusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            try
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarEventoInclusaoMorotista(evento);

                if (retorno.Valor <= 0)
                {
                    if (retorno.Info.Mensagem == "MDF-e não encontrado")
                    {
                        if (IntegrareMDFeOracleAsync(mdfe.Empresa, mdfe.Codigo).GetAwaiter().GetResult())
                        {
                            mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);
                            evento.CodigoInternoMDFe = mdfe.CodigoIntegradorAutorizacao;

                            ServicoMDFe.ResultadoInteger retorno2 = svcMDFe.ImportarEventoInclusaoMorotista(evento);

                            if (retorno2.Valor <= 0)
                            {
                                mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno2.Info.Mensagem);
                                repMDFe.Atualizar(mdfe);

                                Servicos.Log.TratarErro(retorno2.Info.MensagemOriginal);

                                return false;
                            }
                            else
                            {
                                mdfe.CodigoIntegradorEncerramento = retorno2.Valor;
                                mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                                mdfe.Status = Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado;
                                repMDFe.Atualizar(mdfe);

                                if (cargaMDFe != null)
                                    servicoCargaMotorista.AdicionarMotorista(cargaMDFe.Carga, entidadeMotorista);

                                return true;
                            }

                        }
                        else
                        {
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                            repMDFe.Atualizar(mdfe);

                            Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                            return false;
                        }
                    }
                    else
                    {
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                        repMDFe.Atualizar(mdfe);

                        Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                        return false;
                    }
                }
                else
                {
                    mdfe.CodigoIntegradorEncerramento = retorno.Valor;
                    mdfe.MensagemRetornoSefaz = "Evento em processamento.";
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado;
                    repMDFe.Atualizar(mdfe);

                    if (cargaMDFe != null)
                        servicoCargaMotorista.AdicionarMotorista(cargaMDFe.Carga, entidadeMotorista);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mdfe.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                repMDFe.Atualizar(mdfe);

                return false;
            }
        }

        public bool Contingencia(int codigoMDFeOracle, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            try
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.SolicitarContingencia(codigoMDFeOracle);

                if (retorno.Valor <= 0)
                {
                    Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return false;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                {
                    ServicoMDFe.RetornoEventoMDFe retorno = svcMDFe.ConsultarEventoMDFe(mdfe.CodigoIntegradorEncerramento);

                    unidadeDeTrabalho.Start();

                    Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (retorno.Status != "I")
                        SalvarRetornoSefaz(mdfe, "E", 0, retorno.CodigoRetornoSefaz, Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz), unidadeDeTrabalho);

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("R"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmEncerramento;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Encerrado;
                            mdfe.MensagemRetornoSefaz = !string.IsNullOrWhiteSpace(Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz)) ? Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz) : Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                            mdfe.ProtocoloEncerramento = retorno.Protocolo;
                            mdfe.DataEncerramento = retorno.DataRetornoSefaz != DateTime.MinValue ? retorno.DataRetornoSefaz : DateTime.Now;

                            this.ObterESalvarXMLEvento(mdfe.Codigo, mdfe.Empresa.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Encerramento, retorno, unidadeDeTrabalho);

                            this.AtualizarIntegracaoRetornoMDFe(mdfe, unidadeDeTrabalho);
                            this.AtualizarIntegracaoMDFeManual(mdfe, unidadeDeTrabalho);
                        }
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                    }

                    repMDFe.Atualizar(mdfe);

                    unidadeDeTrabalho.CommitChanges();
                }


                return mdfe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (!ex.ToString().Contains("Transaction not connected, or was disconnected"))
                    unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoInclusaoMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado)
                {
                    ServicoMDFe.RetornoEventoMDFe retorno = svcMDFe.ConsultarEventoMDFe(mdfe.CodigoIntegradorEncerramento);

                    unidadeDeTrabalho.Start();

                    Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(retorno.CodigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (retorno.Status != "I")
                        SalvarRetornoSefaz(mdfe, "I", 0, retorno.CodigoRetornoSefaz, Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz), unidadeDeTrabalho);

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("R"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));

                            Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorTipo(mdfe.Codigo, Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao);
                            if (motoristaMDFe != null)
                            {
                                motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.EventoInclusaoRejeitado;
                                repMotoristaMDFe.Atualizar(motoristaMDFe);
                            }
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado;
                            mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetornoSefaz, " - ", retorno.MensagemRetornoSefaz));
                        }
                        else
                        {
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            mdfe.MensagemRetornoSefaz = !string.IsNullOrWhiteSpace(Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz)) ? Utilidades.String.ReplaceInvalidCharacters(retorno.MensagemRetornoSefaz) : Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));

                            this.ObterESalvarXMLEvento(mdfe.Codigo, mdfe.Empresa.Codigo, Dominio.Enumeradores.TipoXMLMDFe.InclusaoMorotista, retorno, unidadeDeTrabalho);

                            Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorTipo(mdfe.Codigo, Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao);
                            if (motoristaMDFe != null)
                            {
                                motoristaMDFe.ProtocoloEventoInclusao = retorno.Protocolo;
                                motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Incluido;
                                repMotoristaMDFe.Atualizar(motoristaMDFe);
                            }

                        }
                    }
                    else
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));

                        Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorTipo(mdfe.Codigo, Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao);
                        if (motoristaMDFe != null)
                        {
                            motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.EventoInclusaoRejeitado;
                            repMotoristaMDFe.Atualizar(motoristaMDFe);
                        }
                    }

                    repMDFe.Atualizar(mdfe);

                    unidadeDeTrabalho.CommitChanges();
                }


                return mdfe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (!ex.ToString().Contains("Transaction not connected, or was disconnected"))
                    unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public void SalvarRetornoSefaz(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, string tipo, int codigoMDFeIntegrador, int codigoRetornoSefaz, string mensagemRetornoSefaz, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unidadeDeTrabalho);
                Repositorio.MDFeRetornoSefaz repMDFeRetornoSefaz = new Repositorio.MDFeRetornoSefaz(unidadeDeTrabalho);

                Dominio.Entidades.MDFeRetornoSefaz mdfeRetornoSefaz = new Dominio.Entidades.MDFeRetornoSefaz();

                var mensagemRetorno = codigoRetornoSefaz.ToString() + " - " + mensagemRetornoSefaz;

                mdfeRetornoSefaz.MDFe = mdfe;
                mdfeRetornoSefaz.Tipo = tipo;
                mdfeRetornoSefaz.CodigoMDFeIntegrador = codigoMDFeIntegrador;
                mdfeRetornoSefaz.DataHora = DateTime.Now;
                mdfeRetornoSefaz.MensagemRetorno = mensagemRetorno.Length > 5000 ? mensagemRetorno.Substring(0, 5000) : mensagemRetorno;
                mdfeRetornoSefaz.ErroSefaz = repErroSefaz.BuscarPorCodigoDoErro(codigoRetornoSefaz, Dominio.Enumeradores.TipoErroSefaz.MDFe);
                repMDFeRetornoSefaz.Inserir(mdfeRetornoSefaz);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao salvar retorno sefaz MDFe: " + ex);
            }
        }

        public string EmitirLoteMDFeContigenciado(List<int> codigosMDFe, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string URLWebServiceCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            int sucesso = 0;

            foreach (int codigo in codigosMDFe)
            {
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repositorioMDFe.BuscarPorCodigo(codigo, false);

                if (mdfe == null || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
                    continue;

                if (mdfe.Empresa?.Configuracao != null && mdfe.Empresa.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca && mdfe.Veiculos != null && mdfe.Veiculos.Count() > 0)
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeAnterior = repositorioMDFe.BuscarPorPlacaEStatus(mdfe.Empresa.CNPJ, mdfe.Veiculos.FirstOrDefault().Placa, Dominio.Enumeradores.StatusMDFe.Autorizado);

                    if (mdfeAnterior != null)
                        continue;
                }

                mdfe.Log = string.Concat(mdfe.Log, " / Emitido por ", usuario.CPF, " - ", usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                if (mdfe.DataEmissao < DateTime.Now.AddDays(-1))
                {
                    TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                    mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                }

                if (!string.IsNullOrWhiteSpace(URLWebServiceCTe) && Emitir(mdfe, unitOfWork))
                {
                    AdicionarMDFeNaFilaDeConsulta(mdfe, unitOfWork);
                    sucesso++;
                }

            }

            if ((codigosMDFe.Count - sucesso) > 0)
                return $"{sucesso} MDF-es emitidos com sucesso. Houveram {codigosMDFe.Count - sucesso} rejeitados.";

            return "MDF-es emitidos com sucesso.";
        }

        public System.IO.MemoryStream ObterLoteDeXML(List<int> codigos, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unitOfWork);
            Servicos.MDFe serMDFe = new MDFe();

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            int quantidadeLotes = (int)Math.Ceiling((decimal)codigos.Count / 1000);

            for (int i = 1; i <= quantidadeLotes; i++)
            {
                List<Dominio.Entidades.XMLMDFe> xmls = repXML.BuscarPorMDFe(codigos.Skip((i - 1) * 1000).Take(1000).ToList(), codigoEmpresa, true);

                foreach (Dominio.Entidades.XMLMDFe xml in xmls)
                {
                    byte[] arquivo = serMDFe.ObterXML(xml.MDFe, xml.Tipo, unitOfWork); //serMDFe.ObterXMLAutorizacao(xml.MDFe, unitOfWork); // System.Text.Encoding.Default.GetBytes(xml.XML);

                    string nomeArquivo = $"MDFe{xml.MDFe.Chave}";

                    if (xml.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento)
                        nomeArquivo += "-procCancMDFe";
                    else if (xml.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Encerramento)
                        nomeArquivo += "-procEncMDFe";

                    nomeArquivo += ".xml";

                    ZipEntry entry = new ZipEntry(nomeArquivo);
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }

                xmls = null;
            }

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public System.IO.MemoryStream ObterLoteDeDAMDFE(List<string> chaves, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios))
                throw new Exception("O caminho para os download da DAMDFE não está disponível.");

            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            if (codigoEmpresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                MemoryStream fZip = new MemoryStream();

                using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
                {
                    zipOStream.SetLevel(9);

                    foreach (string chave in chaves)
                    {
                        string caminhoDAMDFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, empresa.CNPJ, chave) + ".pdf";

                        long tamanhoDAMDFE = 0;
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDAMDFE))
                        {
                            tamanhoDAMDFE = Utilidades.IO.FileStorageService.Storage.GetFileInfo(caminhoDAMDFE).Size;
                        }

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDAMDFE) || tamanhoDAMDFE <= 0)
                        {
                            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorChave(chave);

                            if (mdfe != null)
                                ObterESalvarDAMDFE(mdfe, null, unitOfWork);
                        }

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDAMDFE))
                        {
                            byte[] damdfe = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDAMDFE);

                            ZipEntry entry = new ZipEntry(string.Concat(chave, ".pdf"))
                            {
                                DateTime = DateTime.Now
                            };

                            zipOStream.PutNextEntry(entry);
                            zipOStream.Write(damdfe, 0, damdfe.Length);
                            zipOStream.CloseEntry();
                        }
                    }

                    zipOStream.IsStreamOwner = false;
                    zipOStream.Close();
                }

                fZip.Position = 0;

                return fZip;
            }
            else
            {
                MemoryStream fZip = new MemoryStream();

                using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
                {
                    zipOStream.SetLevel(9);

                    foreach (string chave in chaves)
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorChave(chave);

                        if (mdfe != null && mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        {
                            string caminhoDAMDFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, mdfe.Empresa.CNPJ, chave) + ".pdf";

                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDAMDFE))
                                ObterESalvarDAMDFE(mdfe, null, unitOfWork);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDAMDFE))
                            {
                                byte[] damdfe = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDAMDFE);

                                ZipEntry entry = new ZipEntry(string.Concat(chave, ".pdf"))
                                {
                                    DateTime = DateTime.Now
                                };

                                zipOStream.PutNextEntry(entry);
                                zipOStream.Write(damdfe, 0, damdfe.Length);
                                zipOStream.CloseEntry();
                            }
                        }
                    }

                    zipOStream.IsStreamOwner = false;
                    zipOStream.Close();
                }

                fZip.Position = 0;

                return fZip;

            }

        }

        public byte[] ObterXMLAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return null;

            string xml = ObterStringXMLAutorizacao(mdfe, unitOfWork);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(xml);
                return data;
            }

            return null;
        }

        public byte[] ObterXMLCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return null;

            string xml = ObterStringXMLCancelamento(mdfe, unitOfWork);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(xml);
                return data;
            }

            return null;
        }

        public byte[] ObterXMLEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return null;

            string xml = ObterStringXMLEncerramento(mdfe, unitOfWork);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(xml);
                return data;
            }

            return null;
        }

        public System.IO.MemoryStream ObterXMLInclusaoMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unitOfWork);
            Servicos.MDFe serMDFe = new MDFe(unitOfWork);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);


            List<Dominio.Entidades.XMLMDFe> xmls = repXML.BuscarPorMDFeeTipo(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.InclusaoMorotista);

            foreach (Dominio.Entidades.XMLMDFe xml in xmls)
            {
                byte[] arquivo = System.Text.Encoding.Default.GetBytes(xml.XML);
                ZipEntry entry = new ZipEntry(string.Concat(xml.MDFe.Chave, "-evIncCondutorMDFe", ".xml"));
                entry.DateTime = DateTime.Now;
                zipOStream.PutNextEntry(entry);
                zipOStream.Write(arquivo, 0, arquivo.Length);
                zipOStream.CloseEntry();
            }

            xmls = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public byte[] ObterXML(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Enumeradores.TipoXMLMDFe tipo, Repositorio.UnitOfWork unitOfWork)
        {
            switch (tipo)
            {
                case Dominio.Enumeradores.TipoXMLMDFe.Autorizacao:
                    return ObterXMLAutorizacao(mdfe, unitOfWork);
                case Dominio.Enumeradores.TipoXMLMDFe.Cancelamento:
                    return ObterXMLCancelamento(mdfe, unitOfWork);
                case Dominio.Enumeradores.TipoXMLMDFe.Encerramento:
                    return ObterXMLEncerramento(mdfe, unitOfWork);
            }

            return null;
        }

        public string ObterStringXML(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Enumeradores.TipoXMLMDFe tipo, Repositorio.UnitOfWork unitOfWork)
        {
            switch (tipo)
            {
                case Dominio.Enumeradores.TipoXMLMDFe.Autorizacao:
                    return ObterStringXMLAutorizacao(mdfe, unitOfWork);
                case Dominio.Enumeradores.TipoXMLMDFe.Cancelamento:
                    return ObterStringXMLCancelamento(mdfe, unitOfWork);
                case Dominio.Enumeradores.TipoXMLMDFe.Encerramento:
                    return ObterStringXMLEncerramento(mdfe, unitOfWork);
            }

            return string.Empty;
        }

        public byte[] ObterDAMDFE(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MDFeRetornoSefaz repMDFeRetornoSefaz = new Repositorio.MDFeRetornoSefaz(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unidadeDeTrabalho);

            if (mdfe == null || !(mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento))
                return null;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo arquivo = repArquivo.BuscarPrimeiroRegistro();

            if (string.IsNullOrWhiteSpace(arquivo?.CaminhoRelatorios))
                throw new Exception("O caminho para os download da DAMDFE não está disponível.");

            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(arquivo.CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";

            ////Buscar PDF do Oracle
            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) /*&& Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().RegerarDAMDFEOracle != "NAO"*/)
            {
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                if (mdfe.SistemaEmissor != TipoEmissorDocumento.NSTech)
                    servicoMDFe.ObterESalvarDAMDFE(mdfe, null, unidadeDeTrabalho);
            }

            if (repMDFeRetornoSefaz.ExisteEventoInclusaoMotorista(mdfe.Codigo))
                caminhoPDF = string.Empty;

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
            {
                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeDeTrabalho);

                return svcDAMDFE.Gerar(mdfe.Codigo);
            }
            else
                return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
        }

        public string ObterDAMDFE(int codigoMDFe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

            byte[] damdfe = ObterDAMDFE(mdfe, unidadeDeTrabalho);
            if (damdfe == null)
                return null;

            string stringDamdfe;
            if (codificarUTF8)
                stringDamdfe = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, damdfe));
            else
                stringDamdfe = Convert.ToBase64String(damdfe);

            if (!string.IsNullOrWhiteSpace(stringDamdfe))
                return stringDamdfe;

            return null;
        }

        public string ObterDAMDFE(int codigoMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

            if (mdfe != null && (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ||
                                 mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento))
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                var retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);

                if (retorno != null && !string.IsNullOrWhiteSpace(retorno.DAMDFE))
                    return retorno.DAMDFE;
            }

            return null;
        }

        public ServicoMDFe.RetornoMDFe ObterDadosIntegradosMDFe(int codigoMDFe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

            if (mdfe != null)
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

                ServicoMDFe.RetornoMDFe retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);

                return retorno;
            }

            return null;
        }

        public ServicoMDFe.RetornoEventoMDFe ObterDadosIntegradosEventoMDFe(int codigoEventoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            ServicoMDFe.RetornoEventoMDFe retorno = svcMDFe.ConsultarEventoMDFe(codigoEventoMDFe);

            return retorno;
        }

        public async Task<bool> IntegrareMDFeOracleAsync(Dominio.Entidades.Empresa empresa, int codigoMDFe)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork, _cancellationToken);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = await repositorioMDFe.BuscarPorCodigoAsync(codigoMDFe, false);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
            ServicoMDFe.MDFe mdfeIntegrar = await ObterMDFeParaEmissaoAsync(mdfe, empresa != null ? empresa : mdfe.Empresa);

            mdfeIntegrar.ID = mdfe.Chave;
            mdfeIntegrar.Protocolo = mdfe.Protocolo;

            try
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportaMDFeAnterior(mdfeIntegrar);

                if (retorno.Valor > 0)
                {
                    mdfe.CodigoIntegradorAutorizacao = retorno.Valor;
                    await repositorioMDFe.AtualizarAsync(mdfe);

                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                    return false;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return false;
            }
        }

        public string GerarChaveMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe.Chave != null && mdfe.Chave != "")
            {
                return mdfe.Chave;
            }
            else
            {
                var chaveMDFe = string.Empty;
                chaveMDFe = string.Concat(chaveMDFe, mdfe.Empresa.Localidade.Estado.CodigoIBGE);
                chaveMDFe = string.Concat(chaveMDFe, DateTime.Today.ToString("yyMM"));
                chaveMDFe = string.Concat(chaveMDFe, mdfe.Empresa.CNPJ);
                chaveMDFe = string.Concat(chaveMDFe, "58");
                chaveMDFe = string.Concat(chaveMDFe, string.Format("{0:000}", mdfe.Serie.Numero));
                chaveMDFe = string.Concat(chaveMDFe, string.Format("{0:000000000}", mdfe.Numero));
                chaveMDFe = string.Concat(chaveMDFe, "1");
                chaveMDFe = string.Concat(chaveMDFe, string.Format("{0:00000000}", mdfe.Numero));
                chaveMDFe = string.Concat(chaveMDFe, Utilidades.Calc.Modulo11(chaveMDFe));

                return chaveMDFe;
            }
        }

        public async Task<object> GerarMDFeAnteriorAsync(System.IO.Stream xml, int codigoEmpresa, dynamic arquivo = null, bool integrarMDFeOracle = true)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork, _cancellationToken);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Empresa empresa = await repositorioEmpresa.BuscarPorCodigoAsync(codigoEmpresa);

            arquivo = arquivo ?? MultiSoftware.MDFe.Servicos.Leitura.Ler(xml);

            await _unitOfWork.StartAsync();

            try
            {
                if (arquivo != null)
                {
                    Type tipoArquivo = arquivo.GetType();

                    if (tipoArquivo == typeof(MultiSoftware.MDFe.v100a.mdfeProc))
                        return await GerarMDFeAnteriorAsync(empresa, (MultiSoftware.MDFe.v100a.mdfeProc)arquivo, xml, integrarMDFeOracle);
                    else if (tipoArquivo == typeof(MultiSoftware.MDFe.v300.mdfeProc))
                        return this.GerarMDFeAnteriorAsync(empresa, (MultiSoftware.MDFe.v300.mdfeProc)arquivo, xml, integrarMDFeOracle);
                    else
                    {
                        if (tipoArquivo == typeof(MultiSoftware.MDFe.v300.TProcEvento))
                        {
                            MultiSoftware.MDFe.v300.TProcEvento procEvento = (MultiSoftware.MDFe.v300.TProcEvento)arquivo;

                            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeIntegrado = await repositorioMDFe.BuscarPorChaveAsync(procEvento.retEventoMDFe.infEvento.chMDFe);

                            if (mdfeIntegrado != null)
                            {
                                if (procEvento.retEventoMDFe.infEvento.cStat == "135")
                                {
                                    Servicos.MDFe servicoMDFe = new Servicos.MDFe(_unitOfWork, _cancellationToken);

                                    if (procEvento.retEventoMDFe.infEvento.tpEvento == "110111") //cancelamento
                                    {
                                        if (!string.IsNullOrEmpty(await servicoMDFe.CancelarMDFeImportadoAsync(mdfeIntegrado, procEvento, xml, null, null)))
                                        {
                                            await _unitOfWork.CommitChangesAsync();
                                            return mdfeIntegrado;
                                        }
                                    }
                                    else if (procEvento.retEventoMDFe.infEvento.tpEvento == "110112") //encerramento
                                    {
                                        if (!string.IsNullOrEmpty(await servicoMDFe.EncerrarMDFeImportadoAsync(mdfeIntegrado, procEvento, xml, null, null)))
                                        {
                                            await _unitOfWork.CommitChangesAsync();
                                            return mdfeIntegrado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                await _unitOfWork.RollbackAsync();
                return "O arquivo XML é inválido para a leitura.";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public string BuscarAverbacaoCTe(int codigoCTe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            bool novaUnit = false;
            if (unidadeDeTrabalho == null)
            {
                novaUnit = true;
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);
            }
            try
            {

                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);
                List<Dominio.Entidades.AverbacaoCTe> listaAverbacao = repAverbacaoCTe.BuscarPorCTe(codigoEmpresa, codigoCTe);

                string numeroAverbacao = string.Empty;
                if (listaAverbacao.Count > 0)
                {
                    foreach (Dominio.Entidades.AverbacaoCTe averbacao in listaAverbacao)
                    {
                        if (averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao && averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso)
                        {
                            numeroAverbacao = !string.IsNullOrEmpty(averbacao.Averbacao) ? averbacao.Averbacao : averbacao.Protocolo;
                            break;
                        }
                    }

                }

                if (string.IsNullOrWhiteSpace(numeroAverbacao))
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    numeroAverbacao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.AverbacaoSeguro) ? empresa.Configuracao.AverbacaoSeguro.Length > 40 ? empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : empresa.Configuracao.AverbacaoSeguro :
                                      empresa.Configuracao != null && !empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : empresa.EmpresaPai.Configuracao.AverbacaoSeguro : string.Empty;
                }


                return numeroAverbacao.Length > 40 ? numeroAverbacao.Substring(0, 40) : numeroAverbacao;
            }
            finally
            {
                if (novaUnit)
                    unidadeDeTrabalho.Dispose();
            }
        }

        public void NotificarMDFeEnviado(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int quantidade, string tempo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Email svcEmail = new Servicos.Email(unidadeTrabalho);

                string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                string assunto = ambiente + " - MDF-e com status enviado a mais de " + tempo + " minutos";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Atenção, MDF-e com status enviado a mais de " + tempo + " minutos - ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                if (mdfe != null)
                    sb.Append("MDFe ").Append(mdfe.Numero).Append("/").Append(mdfe.Serie.Numero.ToString()).Append(" transportador ").Append(mdfe.Empresa.CNPJ).Append(" ").Append(mdfe.Empresa.RazaoSocial).Append("<br /> <br />");
                else if (quantidade > 0)
                    sb.Append("Existem ").Append(quantidade.ToString()).Append(" MDFes com status Enviado.").Append("<br /> <br />");
                sb.Append("</p><br /> <br />");

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte1@multisoftware.com.br", 0, unidadeTrabalho);
#else
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte1@multisoftware.com.br", 0, unidadeTrabalho);
#endif
            }
            catch (Exception exptEmail)
            {
                Servicos.Log.TratarErro("Erro ao enviar e-mail notificação CT-e enviado:" + exptEmail);
            }
        }

        public void EnviarEmailEncerramentoTransportador(int codigoMDFe, string ambiente, string urlAcesso, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

            string listaEmails = mdfe.Empresa.Email;
            if (!string.IsNullOrWhiteSpace(mdfe.Empresa.EmailAdministrativo))
                listaEmails = !string.IsNullOrWhiteSpace(listaEmails) ? string.Concat(listaEmails, ";", mdfe.Empresa.EmailAdministrativo) : mdfe.Empresa.EmailAdministrativo;

            if (mdfe == null ||
                mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ||
                mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento ||
                mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ||
                string.IsNullOrWhiteSpace(listaEmails))
                return;

            string placa = mdfe.Veiculos.FirstOrDefault()?.Placa ?? string.Empty;
            string replyTo = "cte1@multisoftware.com.br";
            List<string> emailsCopia = new List<string>() { replyTo };

            System.Text.StringBuilder stBuilder = new StringBuilder();

            stBuilder.Append("Olá " + mdfe.Empresa.RazaoSocial + "(" + mdfe.Empresa.CNPJ_Formatado + "),")
                     .AppendLine()
                     .AppendLine()
                     .Append("O MDF-e número ")
                     .Append(mdfe.Numero)
                     .Append(" série ")
                     .Append(mdfe.Serie.Numero)
                     .Append(" do veículo placa ")
                     .Append(placa)
                     .Append(" ainda não foi encerrado no ambiente ")
                     .Append(ambiente)
                     .AppendLine()
                     .Append("Para encerrar o MDF-e (caso a viagem esteja finalizada) acesse o link: " + urlAcesso)
                     .AppendLine()
                     .AppendLine()
                     .AppendLine(ambiente != "Sintravir" ? "Caso não possua acesso ao portal favor entrar em contato com nosso suporte em horário comercial pelo telefone (49)3025-9500 ou e-mail cte2@multisoftware.com.br" : string.Empty)
                     .AppendLine()
                     .Append("E-mail enviado automaticamente para: " + listaEmails);

            string titulo = "MDF-e aguardando o encerramento: Número " + mdfe.Numero + " do veículo " + placa + " transportador " + mdfe.Empresa.CNPJ_Formatado + " (" + ambiente + ")";

            StringBuilder rodape = new StringBuilder();
            rodape.Append("Atenciosamente,")
                  .AppendLine()
                  .Append(ambiente != "Sintravir" ? "MultiCTe" : ambiente);

            string erro = string.Empty;

            List<string> emails = listaEmails.Split(';').ToList();
#if DEBUG
            emails = new List<string>() { "infra@multisoftware.com.br" };
            emailsCopia = new List<string>() { "infra@multisoftware.com.br" };

#endif

            if (!Servicos.Email.EnviarEmail("", "", "", null, emailsCopia.ToArray(), emails.ToArray(), titulo, stBuilder.ToString(), "", out erro, "", null, rodape.ToString(), false, replyTo, 0, unidadeDeTrabalho))
                Servicos.Log.TratarErro(erro);

            mdfe.DataEnvioNotificacao = DateTime.Now.Date;

            repMDFe.Atualizar(mdfe);
        }

        public string ObterStringXMLAutorizacao(Dominio.Entidades.XMLMDFe xml)
        {
            if (xml != null)
            {
                if (!string.IsNullOrWhiteSpace(xml.XML))
                {
                    return xml.XML;
                }
            }
            return null;
        }

        public void SalvarIntegracaoRetornoMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unidadeDeTrabalho);

            if (empresa.EmpresaPai?.TiposIntegracao != null && empresa.EmpresaPai?.TiposIntegracao.Count > 0)
            {
                foreach (var tipoIntegracao in empresa.EmpresaPai?.TiposIntegracao)
                {
                    if (tipoIntegracao.Ativo && tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog)
                    {
                        Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno = new Dominio.Entidades.MDFeIntegracaoRetorno();
                        mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                        mdfeIntegracaoRetorno.MDFe = mdfe;
                        mdfeIntegracaoRetorno.TipoIntegracao = tipoIntegracao;
                        repMDFeIntegracaoRetorno.Inserir(mdfeIntegracaoRetorno);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(empresa.Configuracao?.WsIntegracaoEnvioMDFeEmbarcadorTMS))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                var tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);
                if (tipoIntegracao == null)
                {
                    tipoIntegracao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracao();
                    tipoIntegracao.Descricao = "MultiEmbarcador TMS";
                    tipoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador;
                    tipoIntegracao.Ativo = true;
                    repTipoIntegracao.Inserir(tipoIntegracao);
                }

                Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno = repMDFeIntegracaoRetorno.BuscarUltipoPorPorMDFeTipo(mdfe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);
                if (mdfeIntegracaoRetorno == null)
                {
                    mdfeIntegracaoRetorno = new Dominio.Entidades.MDFeIntegracaoRetorno();
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                    mdfeIntegracaoRetorno.MDFe = mdfe;
                    mdfeIntegracaoRetorno.TipoIntegracao = tipoIntegracao;
                    repMDFeIntegracaoRetorno.Inserir(mdfeIntegracaoRetorno);
                }
                else
                {
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                    repMDFeIntegracaoRetorno.Atualizar(mdfeIntegracaoRetorno);
                }
            }
        }

        public void AtualizarIntegracaoMDFeManual(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe.Modal?.Numero == "03")//apenas para aquavriario
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao repCargaMDFeAquaviarioManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(mdfe.Codigo);

                if (cargaMDFeManual != null && cargaMDFeManual.MDFeRecebidoDeIntegracao != true && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab);
                    if (tipoIntegracao != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao cargaMDFeAquaviarioManual = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao()
                        {
                            CargaMDFeManual = cargaMDFeManual,
                            SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                            TipoIntegracao = tipoIntegracao,
                            DataIntegracao = DateTime.Now,
                            NumeroTentativas = 0,
                            ProblemaIntegracao = "",
                            IntegrarEncerramento = true
                        };
                        repCargaMDFeAquaviarioManualIntegracao.Inserir(cargaMDFeAquaviarioManual);
                    }
                }
            }
        }

        public void AtualizarIntegracaoRetornoMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unidadeDeTrabalho);
            List<Dominio.Entidades.MDFeIntegracaoRetorno> listaIntegracoes = repMDFeIntegracaoRetorno.BuscarPorMDFe(mdfe.Codigo);
            if (listaIntegracoes.Count > 0)
            {
                foreach (var integracao in listaIntegracoes)
                {
                    integracao.NumeroTentativas = 0;
                    integracao.ProblemaIntegracao = string.Empty;
                    integracao.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                    repMDFeIntegracaoRetorno.Inserir(integracao);
                }
            }
            else
            {
                if (mdfe.Empresa.EmpresaPai?.TiposIntegracao != null && mdfe.Empresa.EmpresaPai?.TiposIntegracao.Count > 0)
                {
                    foreach (var tipoIntegracao in mdfe.Empresa.EmpresaPai?.TiposIntegracao)
                    {
                        if (tipoIntegracao.Ativo && tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yamalog && mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                        {
                            Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno = new Dominio.Entidades.MDFeIntegracaoRetorno();
                            mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                            mdfeIntegracaoRetorno.MDFe = mdfe;
                            mdfeIntegracaoRetorno.TipoIntegracao = tipoIntegracao;
                            repMDFeIntegracaoRetorno.Inserir(mdfeIntegracaoRetorno);
                        }
                    }
                }
            }
        }

        public bool ProcessarRetornoMDFeAutorizado(out string mensagemErro, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string acaoProcessamento = null)
        {
            Servicos.Embarcador.Carga.MDFe svcCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repositorioConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoSGT = repConfiguracaoSGT.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento = repositorioConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadrao();
            bool armazenarEmArquivo = configuracaoSGT?.ArmazenarXMLCTeEmArquivo ?? false;

            int statusMDFe = 0;
            int.TryParse(string.IsNullOrWhiteSpace(mdfeOracle.CodStatusProtocolo) ? mdfeOracle.CodStatusEnvio : mdfeOracle.CodStatusProtocolo, out statusMDFe);

            string mensagemRetorno = string.Empty;
            mensagemErro = string.Empty;

            DateTime dataProtocolo;
            DateTime.TryParseExact(mdfeOracle.DataProtocolo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataProtocolo);

            if (mdfeOracle.CodigoMDFeAutorizacao > 0 || acaoProcessamento == "RetornoEmissao")
            {
                if (mdfe == null && mdfeOracle.CodigoMDFeAutorizacao > 0)
                    mdfe = repMDFe.BuscarPorCodigoIntegradorAutorizacao(mdfeOracle.CodigoMDFeAutorizacao);

                if (mdfe == null)
                {
                    mensagemErro = $"MDF-e {mdfeOracle.ChaveMDFe} não localizado na base SqlServer.";
                    return false;
                }

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Pendente || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
                {
                    unitOfWork.Start();

                    mdfe.MensagemStatus = configuracaoIntegracaoEmissorDocumento.TipoEmissorDocumentoMDFe == TipoEmissorDocumento.NSTech ? null : repErroSefaz.BuscarPorCodigoDoErro(statusMDFe, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (mdfeOracle.StatusIntegrador != "I")
                        SalvarRetornoSefaz(mdfe, "A", mdfeOracle.CodigoMDFeAutorizacao, statusMDFe, string.IsNullOrWhiteSpace(mdfeOracle.DescricaoProtocolo) ? mdfeOracle.DescricaoEnvio : mdfeOracle.DescricaoProtocolo, unitOfWork);

                    if (mdfeOracle.StatusIntegrador == "R")
                    {
                        bool encerramentoAutomatico = false, duplicidade = false;
                        string chaveMDFe = string.Empty, protocoloMDFe = string.Empty;

                        TratarRetornoRejeicao(ref mdfe, mdfeOracle, ref encerramentoAutomatico, ref chaveMDFe, ref protocoloMDFe, ref duplicidade, ref mensagemRetorno, unitOfWork);

                        repMDFe.Atualizar(mdfe);

                        unitOfWork.CommitChanges();

                        EnviarEmailManifestoRejeitado(unitOfWork, mdfe, tipoServicoMultisoftware);

                        if (encerramentoAutomatico)
                        {
                            //unitOfWork.Start();

                            if (SolicitarEncerramentoAutomaticoChave(mdfe.Empresa.Codigo, mdfe.Chave, chaveMDFe, protocoloMDFe, unitOfWork))
                                Emitir(mdfe, unitOfWork);
                            else
                            {
                                mdfe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                                mdfe.MensagemRetornoSefaz = mensagemRetorno;
                                repMDFe.Atualizar(mdfe);
                            }
                            //unitOfWork.CommitChanges();
                        }

                        if ((Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarIntegracaoErroMDFeMagalog.Value && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao) || (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarIntegracaoMagalogNoRetorno.Value))
                        {
                            Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarMDFeRetornoWS(mdfe.Codigo, unitOfWork);
                            Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoMDFeMultiCTeWS(mdfe.Codigo, unitOfWork.StringConexao);
                        }
                    }
                    else if (mdfeOracle.StatusIntegrador == "I")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Enviado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));

                        repMDFe.Atualizar(mdfe);

                        unitOfWork.CommitChanges();
                    }
                    else if (mdfeOracle.StatusIntegrador == "M" || mdfeOracle.StatusIntegrador == "D" || mdfeOracle.StatusIntegrador == "P")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.Info.Mensagem, " - ", mdfeOracle.Info.MensagemOriginal));
                        mdfe.Protocolo = mdfeOracle.NumeroProtocolo;
                        mdfe.Chave = mdfeOracle.ChaveMDFe;
                        mdfe.QRCode = mdfeOracle.DigVerificador;

                        DateTime dataAutorizacao = dataProtocolo != DateTime.MinValue ? dataProtocolo : DateTime.Now;

                        mdfe.DataAutorizacao = dataAutorizacao;

                        SalvarXMLAutorizacao(mdfe, mdfeOracle, unitOfWork);
                        ObterESalvarDAMDFEOracle(mdfe, mdfeOracle, unitOfWork);

                        repMDFe.Atualizar(mdfe);

                        ProcessarMDFeMultiCTe(mdfe, unitOfWork);
                        svcCargaMDFe.AjustarAverbacoesParaAutorizacao(mdfe.Codigo, unitOfWork);

                        unitOfWork.CommitChanges();

                        if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarIntegracaoMagalogNoRetorno.Value)
                        {
                            Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarMDFeRetornoWS(mdfe.Codigo, unitOfWork);
                            Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoMDFeMultiCTeWS(mdfe.Codigo, unitOfWork.StringConexao);
                        }

                        if (mdfe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech)
                            ObterESalvarDAMDFE(mdfe, null, unitOfWork);
                    }
                }
            }
            else if (mdfeOracle.CodigoMDFeCancelamento > 0 || acaoProcessamento == "RetornoCancelamento")
            {
                if (mdfe == null && mdfeOracle.CodigoMDFeCancelamento > 0)
                    mdfe = repMDFe.BuscarPorCodigoIntegradorCancelamento(mdfeOracle.CodigoMDFeCancelamento);

                if (mdfe == null)
                {
                    mensagemErro = $"MDF-e {mdfeOracle.ChaveMDFe} não localizado na base SqlServer.";
                    return false;
                }

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                {
                    unitOfWork.Start();

                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(statusMDFe, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (mdfeOracle.StatusIntegrador != "I")
                        SalvarRetornoSefaz(mdfe, "C", 0, statusMDFe, string.IsNullOrWhiteSpace(mdfeOracle.DescricaoProtocolo) ? mdfeOracle.DescricaoEnvio : mdfeOracle.DescricaoProtocolo, unitOfWork);

                    if (mdfeOracle.StatusIntegrador == "R")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                    else if (mdfeOracle.StatusIntegrador == "I")
                    {
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                    else if (mdfeOracle.StatusIntegrador == "C")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Cancelado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.Info.Mensagem, " - ", mdfeOracle.Info.MensagemOriginal));
                        mdfe.ProtocoloCancelamento = mdfeOracle.NumeroProtocolo;
                        mdfe.DataCancelamento = dataProtocolo != DateTime.MinValue ? dataProtocolo : DateTime.Now;

                        SalvarXMLCancelamento(mdfe, mdfeOracle, unitOfWork);

                        svcCargaMDFe.AjustarAverbacoesParaCancelamento(mdfe.Codigo, unitOfWork);
                    }

                    repMDFe.Atualizar(mdfe);

                    unitOfWork.CommitChanges();
                }
            }
            else if (mdfeOracle.CodigoMDFeEncerramento > 0 || acaoProcessamento == "RetornoEncerramento" || acaoProcessamento == "RetornoInclusaoMotorista")
            {
                if (mdfe == null && mdfeOracle.CodigoMDFeEncerramento > 0)
                    mdfe = repMDFe.BuscarPorCodigoIntegradorEncerramento(mdfeOracle.CodigoMDFeEncerramento);

                if (mdfe == null)
                {
                    mensagemErro = $"MDF-e {mdfeOracle.ChaveMDFe} não localizado na base SqlServer.";
                    return false;
                }

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                {
                    unitOfWork.Start();

                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(statusMDFe, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (mdfeOracle.StatusIntegrador != "I")
                        SalvarRetornoSefaz(mdfe, "E", 0, statusMDFe, string.IsNullOrWhiteSpace(mdfeOracle.DescricaoProtocolo) ? mdfeOracle.DescricaoEnvio : mdfeOracle.DescricaoProtocolo, unitOfWork);

                    if (mdfeOracle.StatusIntegrador == "R")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                    else if (mdfeOracle.StatusIntegrador == "I")
                    {
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                    else if (mdfeOracle.StatusIntegrador == "E")
                    {
                        //Comentado pois não estava encerrando os MDFes na Maroni
                        //if(mdfeOracle.Info.Mensagem.Contains("Evento ainda não processado"))
                        //    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        //else

                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Encerrado;

                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.Info.Mensagem, " - ", mdfeOracle.Info.MensagemOriginal));
                        mdfe.ProtocoloEncerramento = mdfeOracle.NumeroProtocolo;
                        mdfe.DataEncerramento = dataProtocolo != DateTime.MinValue ? dataProtocolo : DateTime.Now;

                        SalvarXMLEncerramento(mdfe, Dominio.Enumeradores.TipoXMLMDFe.Encerramento, mdfeOracle, unitOfWork);
                        AtualizarIntegracaoMDFeManual(mdfe, unitOfWork);

                        svcCargaMDFe.AjustarAverbacoesParaEncerramento(mdfe.Codigo, unitOfWork);
                    }

                    repMDFe.Atualizar(mdfe);

                    unitOfWork.CommitChanges();
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado)
                {
                    unitOfWork.Start();

                    mdfe.MensagemStatus = repErroSefaz.BuscarPorCodigoDoErro(statusMDFe, Dominio.Enumeradores.TipoErroSefaz.MDFe);

                    if (mdfeOracle.StatusIntegrador != "I")
                        SalvarRetornoSefaz(mdfe, "I", 0, statusMDFe, string.IsNullOrWhiteSpace(mdfeOracle.DescricaoProtocolo) ? mdfeOracle.DescricaoEnvio : mdfeOracle.DescricaoProtocolo, unitOfWork);

                    if (mdfeOracle.StatusIntegrador == "R")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                    else if (mdfeOracle.StatusIntegrador == "I")
                    {
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.DescricaoStatusIntegrador, " - ", mdfeOracle.CodStatusEnvio, " - ", mdfeOracle.DescricaoEnvio));
                    }
                    else if (mdfeOracle.StatusIntegrador == "E")
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                        mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(string.Concat(mdfeOracle.Info.Mensagem, " - ", mdfeOracle.Info.MensagemOriginal));

                        SalvarXMLEncerramento(mdfe, Dominio.Enumeradores.TipoXMLMDFe.InclusaoMorotista, mdfeOracle, unitOfWork);

                        Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorTipo(mdfe.Codigo, Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao);
                        if (motoristaMDFe != null)
                        {
                            motoristaMDFe.ProtocoloEventoInclusao = mdfeOracle.NumeroProtocolo;
                            motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Incluido;
                            repMotoristaMDFe.Atualizar(motoristaMDFe);
                        }

                    }

                    repMDFe.Atualizar(mdfe);

                    unitOfWork.CommitChanges();
                }
            }

            if ((mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado))
            {
                if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                {
                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoMDFe(mdfe, unitOfWork);
                    else if (mdfeOracle.CodigoMDFeEncerramento > 0 && (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado))
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoEncerramentoMDFe(mdfe, unitOfWork);
                }
                else if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) != null)
                {
                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                        new Servicos.Embarcador.Integracao.Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware).IntegrarMDFeRejeitado(mdfe);
                }
            }

            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
            {
                Servicos.Embarcador.Carga.MDFe servicoMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                servicoMDFe.FinalizarCargaPorEncerramentoDeMDFe(mdfe, tipoServicoMultisoftware, unitOfWork, Auditado);
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, "Integrou o MDF-e.", unitOfWork);

            return true;
        }

        public bool sincronizarDocumentoEmProcessamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string wsOracle = "")
        {
            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            if (!string.IsNullOrWhiteSpace(wsOracle))
            {
                string endpoint = string.Concat(wsOracle, "uMDFeServiceTS.asmx");
                svcMDFe.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);
            }

            try
            {
                Servicos.ServicoMDFe.RetornoMDFe retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);

                Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle = new Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle();
                mdfeOracle.DataRecibo = retorno.DataRetornoSefaz.ToString("dd/MM/yyyy HH:mm:ss");
                mdfeOracle.CodStatusEnvio = retorno.CodigoRetornoSefaz.ToString();
                mdfeOracle.DescricaoEnvio = retorno.MensagemRetornoSefaz;
                mdfeOracle.NumeroRecibo = null;
                mdfeOracle.DataProtocolo = retorno.DataRetornoSefaz.ToString("dd/MM/yyyy HH:mm:ss");
                mdfeOracle.CodStatusProtocolo = retorno.CodigoRetornoSefaz.ToString();
                mdfeOracle.DescricaoProtocolo = retorno.MensagemRetornoSefaz;
                mdfeOracle.NumeroProtocolo = retorno.Protocolo;
                mdfeOracle.ChaveMDFe = retorno.Chave;
                mdfeOracle.DigVerificador = null;
                mdfeOracle.StatusIntegrador = retorno.Status;
                mdfeOracle.DescricaoStatusIntegrador = retorno.DescricaoStatus;
                mdfeOracle.CodigoMDFeAutorizacao = retorno.CodigoInternoMDFe;
                mdfeOracle.PDFDAMDFE = retorno.DAMDFE;
                mdfeOracle.XMLAutorizacao = retorno.XML;

                if (retorno.Info != null)
                {
                    mdfeOracle.Info = new Dominio.ObjetosDeValor.WebService.CTe.Resultado();
                    mdfeOracle.Info.Tipo = retorno.Info.Tipo;
                    mdfeOracle.Info.Mensagem = retorno.Info.Mensagem;
                }

                string mensagemErro = string.Empty;
                if (!this.ProcessarRetornoMDFeAutorizado(out mensagemErro, mdfeOracle, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                    return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Sincronizar Documento");
                Servicos.Log.TratarErro(ex);

                throw;
            }

            return true;
        }

        public void AdicionarMDFeNaCarga(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repositorioCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repositorioCargaMDFe.BuscarPorMDFe(mdfe.Codigo);
            if (cargaMDFe == null)
            {
                cargaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe();
                cargaMDFe.Carga = carga;
                cargaMDFe.MDFe = mdfe;
                cargaMDFe.SistemaEmissor = SistemaEmissor.MultiCTe;
                cargaMDFe.CargaLocaisPrestacao = repositorioCargaLocaisPrestacao.BuscarPrimeiroPorCargaUFOrigemEUFDestino(carga.Codigo, mdfe.EstadoCarregamento.Sigla, mdfe.EstadoDescarregamento.Sigla);
                repositorioCargaMDFe.Inserir(cargaMDFe);
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterStringXMLAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return string.Empty;

            Repositorio.XMLMDFe repositorioXML = new Repositorio.XMLMDFe(unitOfWork);
            Dominio.Entidades.XMLMDFe xml = repositorioXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);

            if (xml == null)
            {
                Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ObterXMLMdfeAutorizacao(mdfe, mdfe.Empresa.Codigo, unitOfWork);
                xml = repositorioXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);
            }

            return xml?.XML ?? string.Empty;
        }

        private string ObterStringXMLCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return string.Empty;

            Repositorio.XMLMDFe repositorioXML = new Repositorio.XMLMDFe(unitOfWork);
            Dominio.Entidades.XMLMDFe xml = repositorioXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);

            if (xml == null)
            {
                Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ObterXMLMdfeCancelamento(mdfe, mdfe.Empresa.Codigo, unitOfWork);
                xml = repositorioXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);
            }

            return xml?.XML ?? string.Empty;
        }

        private string ObterStringXMLEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (mdfe == null)
                return string.Empty;

            Repositorio.XMLMDFe repositorioXML = new Repositorio.XMLMDFe(unitOfWork);
            Dominio.Entidades.XMLMDFe xml = repositorioXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Encerramento);

            return xml?.XML ?? string.Empty;
        }

        private void EnviarEmailManifestoRejeitado(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(TipoConfiguracaoAlerta.MDFEPendenteDeEncerramento);

                if (string.IsNullOrWhiteSpace(configuracaoAlerta?.CodigosRejeicoes))
                    return;

                if (!configuracaoAlerta.CodigosRejeicoes.Contains(mdfe.MensagemStatus?.CodigoDoErro.ToString()))
                    return;

                string veiculosTotais = string.Join(", ", mdfe.Veiculos.Select(o => o.Placa).ToList()) + string.Join(", ", mdfe.Reboques.Select(o => o.Placa).ToList());

                System.Text.StringBuilder mensagemEmail = new System.Text.StringBuilder();

                string assuntoEmail = "MDF-E Pendente de Encerramento";
                mensagemEmail.AppendLine("Por meio deste informamos que o MDF-E abaixo está com rejeição na emissão do mesmo.");
                mensagemEmail.AppendLine($"MDF-E: {mdfe.Descricao}");
                mensagemEmail.AppendLine($"Transportador: {mdfe.Empresa.NomeCNPJ}");
                mensagemEmail.AppendLine($"Placa(s): {veiculosTotais}");
                mensagemEmail.AppendLine($"Mensagem Retorno SEFAZ: {mdfe.MensagemRetornoSefaz}");

                string corpoEmail = mensagemEmail.ToString();

                foreach (Dominio.Entidades.Usuario usuario in configuracaoAlerta.Usuarios)
                    Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, usuario.Email, null, null, assuntoEmail, corpoEmail, configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, null, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);

                if (configuracaoAlerta.AlertarTransportador)
                    Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, mdfe.Empresa?.Email ?? string.Empty, null, null, assuntoEmail, corpoEmail, configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, null, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        public async Task<object> GerarMDFeAnteriorAsync(Dominio.Entidades.Empresa empresa, MultiSoftware.MDFe.v100a.mdfeProc mdfe, Stream xml, bool integrarMDFeOracle = true)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork, _cancellationToken);

            if (empresa == null)
            {
                string cnpjEmitente = mdfe.MDFe.infMDFe.emit.CNPJ;
                empresa = await repositorioEmpresa.BuscarPorCNPJAsync(Utilidades.String.OnlyNumbers(cnpjEmitente));
            }

            if (empresa == null)
                return "Empresa emissora não encontrada na base (" + mdfe.MDFe.infMDFe.emit.CNPJ + ")";

            if (mdfe.protMDFe.infProt.cStat != "100")
            {
                return string.Concat("O status (", mdfe.protMDFe.infProt.cStat, " - ", Utilidades.String.Left(mdfe.protMDFe.infProt.xMotivo, 60), ") do MDF-e (", mdfe.MDFe.infMDFe.ide.nMDF, " - ", mdfe.MDFe.infMDFe.ide.serie, ") é inválido para a importação.");
            }

            if (!Utilidades.String.OnlyNumbers(mdfe.MDFe.infMDFe.emit.CNPJ).Equals(Utilidades.String.OnlyNumbers(empresa.CNPJ)))
            {
                return string.Concat("O CNPJ (", mdfe.MDFe.infMDFe.emit.CNPJ, ") do emitente do MDF-e (", mdfe.MDFe.infMDFe.ide.nMDF, " - ", mdfe.MDFe.infMDFe.ide.serie, ") é diferente do CNPJ da empresa emitente (", empresa.CNPJ, ").");
            }

            Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork, _cancellationToken);
            Repositorio.ModeloDocumentoFiscal repositorioModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork, _cancellationToken);
            Repositorio.ModalTransporte repositorioModalTransporte = new Repositorio.ModalTransporte(_unitOfWork, _cancellationToken);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioManifesto = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = await repositorioManifesto.BuscarPorChaveAsync(mdfe.protMDFe.infProt.chMDFe);

            if (manifesto != null)
            {
                await _unitOfWork.RollbackAsync();
                return manifesto;
            }

            Dominio.Entidades.EmpresaSerie serie = await ObterSerieAsync(empresa, int.Parse(mdfe.MDFe.infMDFe.ide.serie));

            manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();
            manifesto.Modelo = repositorioModelo.BuscarPorModelo("58");
            manifesto.Empresa = empresa;
            manifesto.Versao = "1.00";
            manifesto.MDFeSemCarga = true;
            manifesto.TipoAmbiente = mdfe.protMDFe.infProt.tpAmb == MultiSoftware.MDFe.v100a.TAmb.Item2 ? Dominio.Enumeradores.TipoAmbiente.Homologacao : Dominio.Enumeradores.TipoAmbiente.Producao;
            manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporteApenasChaveCTe;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            manifesto.Chave = mdfe.protMDFe.infProt.chMDFe;
            manifesto.Protocolo = mdfe.protMDFe.infProt.nProt;
            manifesto.Numero = int.Parse(mdfe.MDFe.infMDFe.ide.nMDF);
            manifesto.Serie = serie;
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
            manifesto.MensagemRetornoSefaz = "MDF-e importado.";
            manifesto.Importado = true;
            manifesto.Log = string.Concat("MDF-e importado com sucesso em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
            DateTime dataEmissao;
            DateTime.TryParseExact(mdfe.MDFe.infMDFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            manifesto.DataEmissao = dataEmissao;
            manifesto.DataAutorizacao = mdfe.protMDFe.infProt.dhRecbto;
            manifesto.Modal = await repositorioModalTransporte.BuscarPorCodigoAsync(1, false);
            manifesto.Modal.modalProcCTe = (int)mdfe.MDFe.infMDFe.ide.modal;
            manifesto.EstadoCarregamento = await repositorioEstado.BuscarPorSiglaAsync(mdfe.MDFe.infMDFe.ide.UFIni.ToString());
            manifesto.EstadoDescarregamento = await repositorioEstado.BuscarPorSiglaAsync(mdfe.MDFe.infMDFe.ide.UFFim.ToString());
            manifesto.RNTRC = empresa.RegistroANTT;
            manifesto.ObservacaoContribuinte = mdfe.MDFe.infMDFe.infAdic.infCpl;
            manifesto.ObservacaoFisco = mdfe.MDFe.infMDFe.infAdic.infAdFisco;

            await repositorioManifesto.InserirAsync(manifesto);

            await ObterMunicipiosCarregamentoAsync(manifesto, mdfe.MDFe.infMDFe.ide.infMunCarrega.Select(x => x.cMunCarrega).ToList());
            await ObterMunicipiosDescarregamentoAsync(manifesto, mdfe.MDFe.infMDFe.infDoc);
            await ObterDadosModalRodoviarioMDFeAsync(manifesto, mdfe.MDFe.infMDFe.infModal);
            await ObterLacresAsync(manifesto, mdfe.MDFe.infMDFe.lacres.Select(x => x.nLacre).ToList());
            await ObterPercursoAsync(manifesto, mdfe.MDFe.infMDFe.ide.infPercurso.Select(x => x.UFPer.ToString("G")).ToList());
            await SetarNumeroPedidoObservacaoMDFeAsync(manifesto);

            Repositorio.XMLMDFe repositorioXMLMDFe = new Repositorio.XMLMDFe(_unitOfWork, _cancellationToken);

            Dominio.Entidades.XMLMDFe xmlMDFe = new Dominio.Entidades.XMLMDFe();
            xmlMDFe.MDFe = manifesto;
            xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Autorizacao;

            xml.Position = 0;
            StreamReader reader = new StreamReader(xml);
            xmlMDFe.XML = (await reader.ReadToEndAsync()).Replace(Convert.ToChar(0x00).ToString(), "");

            await repositorioXMLMDFe.InserirAsync(xmlMDFe);

            await _unitOfWork.CommitChangesAsync();

            if (integrarMDFeOracle)
                await IntegrareMDFeOracleAsync(empresa, manifesto.Codigo);

            return manifesto;
        }

        public async Task<object> GerarMDFeAnteriorAsync(Dominio.Entidades.Empresa empresa, MultiSoftware.MDFe.v300.mdfeProc mdfe, Stream xml, bool integrarMDFeOracle = true, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork, _cancellationToken);

            if (empresa == null)
            {
                string cnpjEmitente = mdfe.MDFe.infMDFe.emit.Item;
                empresa = await repositorioEmpresa.BuscarPorCNPJAsync(Utilidades.String.OnlyNumbers(cnpjEmitente));
            }

            if (empresa == null)
                return "Empresa emissora não encontrada na base (" + mdfe.MDFe.infMDFe.emit.Item + ")";

            await _unitOfWork.StartAsync();

            if (mdfe.protMDFe.infProt.cStat != "100")
            {
                await _unitOfWork.RollbackAsync();
                return string.Concat("O status (", mdfe.protMDFe.infProt.cStat, " - ", Utilidades.String.Left(mdfe.protMDFe.infProt.xMotivo, 60), ") do MDF-e (", mdfe.MDFe.infMDFe.ide.nMDF, " - ", mdfe.MDFe.infMDFe.ide.serie, ") é inválido para a importação.");
            }

            if (!Utilidades.String.OnlyNumbers(mdfe.MDFe.infMDFe.emit.Item).Equals(Utilidades.String.OnlyNumbers(empresa.CNPJ)))
            {
                await _unitOfWork.RollbackAsync();
                return string.Concat("O CNPJ (", mdfe.MDFe.infMDFe.emit.Item, ") do emitente do MDF-e (", mdfe.MDFe.infMDFe.ide.nMDF, " - ", mdfe.MDFe.infMDFe.ide.serie, ") é diferente do CNPJ da empresa emitente (", empresa.CNPJ, ").");
            }

            Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork, _cancellationToken);
            Repositorio.ModeloDocumentoFiscal repositorioModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork, _cancellationToken);
            Repositorio.ModalTransporte repositorioModalTransporte = new Repositorio.ModalTransporte(_unitOfWork, _cancellationToken);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioManifesto = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork, _cancellationToken);
            Repositorio.XMLMDFe repositorioXMLMDFe = new Repositorio.XMLMDFe(_unitOfWork, _cancellationToken);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = await repositorioManifesto.BuscarPorChaveAsync(mdfe.protMDFe.infProt.chMDFe);

            if (manifesto != null)
            {
                await _unitOfWork.RollbackAsync();
                return manifesto;
            }

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.EmpresaSerie serie = await ObterSerieAsync(empresa, int.Parse(mdfe.MDFe.infMDFe.ide.serie));

            DateTime.TryParseExact(mdfe.MDFe.infMDFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissao);
            DateTime.TryParseExact(mdfe.protMDFe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTime dataAutorizacao);

            manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();
            manifesto.QRCode = mdfe.MDFe.infMDFeSupl?.qrCodMDFe;
            manifesto.Modelo = repositorioModelo.BuscarPorModelo("58");
            manifesto.Empresa = empresa;
            manifesto.Versao = "3.00";
            manifesto.MDFeSemCarga = true;
            manifesto.TipoAmbiente = mdfe.protMDFe.infProt.tpAmb == MultiSoftware.MDFe.v300.TAmb.Item2 ? Dominio.Enumeradores.TipoAmbiente.Homologacao : Dominio.Enumeradores.TipoAmbiente.Producao;
            manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporteApenasChaveCTe;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            manifesto.Chave = mdfe.protMDFe.infProt.chMDFe;
            manifesto.Protocolo = mdfe.protMDFe.infProt.nProt;
            manifesto.Numero = int.Parse(mdfe.MDFe.infMDFe.ide.nMDF);
            manifesto.Serie = serie;
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
            manifesto.MensagemRetornoSefaz = "MDF-e importado.";
            manifesto.Importado = true;
            manifesto.Log = string.Concat("MDF-e importado com sucesso em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
            manifesto.DataEmissao = dataEmissao;
            manifesto.DataAutorizacao = dataAutorizacao;
            manifesto.Modal = await repositorioModalTransporte.BuscarPorCodigoAsync(1, false);
            manifesto.Modal.modalProcCTe = (int)mdfe.MDFe.infMDFe.ide.modal;
            manifesto.EstadoCarregamento = await repositorioEstado.BuscarPorSiglaAsync(mdfe.MDFe.infMDFe.ide.UFIni.ToString());
            manifesto.EstadoDescarregamento = await repositorioEstado.BuscarPorSiglaAsync(mdfe.MDFe.infMDFe.ide.UFFim.ToString());
            manifesto.RNTRC = empresa.RegistroANTT;
            manifesto.ObservacaoContribuinte = mdfe.MDFe.infMDFe.infAdic?.infCpl;
            manifesto.ObservacaoFisco = mdfe.MDFe.infMDFe.infAdic?.infAdFisco;

            if (mdfe.MDFe.infMDFe.tot.cUnid == MultiSoftware.MDFe.v300.TMDFeInfMDFeTotCUnid.Item01)
                manifesto.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            else
                manifesto.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.TON;

            if (!string.IsNullOrWhiteSpace(mdfe.MDFe.infMDFe.tot.qCarga))
                manifesto.PesoBrutoMercadoria = decimal.Parse(mdfe.MDFe.infMDFe.tot.qCarga, cultura);

            if (!string.IsNullOrWhiteSpace(mdfe.MDFe.infMDFe.tot.vCarga))
                manifesto.ValorTotalMercadoria = decimal.Parse(mdfe.MDFe.infMDFe.tot.vCarga, cultura);

            if (!string.IsNullOrWhiteSpace(mdfe.MDFe.infMDFe.tot.qCTe))
                manifesto.QtdTotalDocumentos = int.Parse(mdfe.MDFe.infMDFe.tot.qCTe);
            else if (!string.IsNullOrWhiteSpace(mdfe.MDFe.infMDFe.tot.qNFe))
                manifesto.QtdTotalDocumentos = int.Parse(mdfe.MDFe.infMDFe.tot.qNFe);

            await repositorioManifesto.InserirAsync(manifesto);

            await ObterMunicipiosCarregamentoAsync(manifesto, mdfe.MDFe.infMDFe.ide.infMunCarrega.Select(x => x.cMunCarrega).ToList());
            await ObterMunicipiosDescarregamentoAsync(manifesto, mdfe.MDFe.infMDFe.infDoc);
            ObterDadosModalRodoviarioMDFe(manifesto, mdfe.MDFe.infMDFe.infModal, _unitOfWork, cargaPedido);
            await ObterLacresAsync(manifesto, mdfe.MDFe.infMDFe.lacres.Select(x => x.nLacre).ToList());
            await ObterPercursoAsync(manifesto, mdfe.MDFe.infMDFe.ide.infPercurso.Select(x => x.UFPer.ToString("G")).ToList());
            await ObterSeguroAsync(manifesto, mdfe.MDFe.infMDFe.seg);
            ObterProdutoPredominante(manifesto, mdfe.MDFe.infMDFe.prodPred);

            await SetarNumeroPedidoObservacaoMDFeAsync(manifesto);

            xml.Position = 0;
            StreamReader reader = new StreamReader(xml);

            Dominio.Entidades.XMLMDFe xmlMDFe = new Dominio.Entidades.XMLMDFe
            {
                MDFe = manifesto,
                Tipo = Dominio.Enumeradores.TipoXMLMDFe.Autorizacao,
                XML = (await reader.ReadToEndAsync()).Replace(Convert.ToChar(0x00).ToString(), "")
            };

            await repositorioXMLMDFe.InserirAsync(xmlMDFe);
            await repositorioManifesto.AtualizarAsync(manifesto);

            await _unitOfWork.CommitChangesAsync();

            if (integrarMDFeOracle)
                await IntegrareMDFeOracleAsync(empresa, manifesto.Codigo);

            return manifesto;
        }

        public async Task SetarNumeroPedidoObservacaoMDFeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioDocumentoMunicipioDescarregamentoMDFe.BuscarPrimeiroCTePorMDFeAsync(mdfe.Codigo);

            if (cte?.TomadorPagador?.GrupoPessoas == null)
                return;

            if (cte.TomadorPagador.GrupoPessoas.LerNumeroPedidoObservacaoMDFe && !string.IsNullOrWhiteSpace(cte.TomadorPagador.GrupoPessoas.RegexNumeroPedidoObservacaoMDFe) && !string.IsNullOrWhiteSpace(mdfe.ObservacaoContribuinte))
            {
                Regex regex = new Regex(cte.TomadorPagador.GrupoPessoas.RegexNumeroPedidoObservacaoMDFe, RegexOptions.IgnoreCase);

                Match match = regex.Match(mdfe.ObservacaoContribuinte);

                if (match.Success)
                    mdfe.NumeroPedido = match.Value.Trim();
            }
        }

        private async Task ObterDadosModalRodoviarioMDFeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v100a.TMDFeInfMDFeInfModal infModal)
        {
            if (infModal != null)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.MDFe.v100a.ModalRodoviario.rodo));
                byte[] data = Encoding.Default.GetBytes(infModal.Any.OuterXml);
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
                MultiSoftware.MDFe.v100a.ModalRodoviario.rodo rodo = (MultiSoftware.MDFe.v100a.ModalRodoviario.rodo)serializer.Deserialize(memStream);

                await ObterVeiculoMDFeAsync(mdfe, rodo);
                await ObterReboquesMDFeAsync(mdfe, rodo);
            }
        }

        private void ObterDadosModalRodoviarioMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeInfModal infModal, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            if (infModal != null)
            {
                MultiSoftware.MDFe.v300.ModalRodoviario.rodo rodo = null;
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.MDFe.v300.ModalRodoviario.rodo));

                try
                {
                    byte[] data = Encoding.Default.GetBytes(infModal.Any.OuterXml);
                    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length))
                        rodo = (MultiSoftware.MDFe.v300.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar modal rodoviário MDFe com encoding Default: {ex.ToString()}", "CatchNoAction");
                }

                if (rodo == null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);
                    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length))
                        rodo = (MultiSoftware.MDFe.v300.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                }

                ObterVeiculoMDFe(mdfe, rodo, unidadeDeTrabalho);
                ObterReboquesMDFe(mdfe, rodo, unidadeDeTrabalho);
                ObterCIOTMDFe(mdfe, rodo, unidadeDeTrabalho);
                ObterValePedagioMDFe(mdfe, rodo, unidadeDeTrabalho, cargaPedido);
            }
        }

        private void ObterValePedagioMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.ModalRodoviario.rodo rodo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            try
            {

                var repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                var repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
                var repCliente = new Repositorio.Cliente(unitOfWork);
                var repMdfeValePedagio = new Repositorio.ValePedagioMDFe(unitOfWork);

                if (repConfiguracaoTMS.ObterConfiguracaoPorNomePropriedade<bool>("ImportarValePedagioMDFECarga") && rodo?.infANTT?.valePed?.disp != null)
                {
                    foreach (var valePedagio in rodo.infANTT.valePed.disp)
                    {
                        var cnpjFornecedor = !string.IsNullOrWhiteSpace(valePedagio.CNPJForn) && double.TryParse(valePedagio.CNPJForn, out double temp) ? temp : 0;
                        var fornecedor = repCliente.BuscarPorCPFCNPJ(cnpjFornecedor);
                        if (fornecedor == null)
                            throw new Exception($"Fornecedor (CNPJ {cnpjFornecedor}) não encontrado");

                        var cnpjResponsavel = !string.IsNullOrWhiteSpace(valePedagio.Item) && double.TryParse(valePedagio.Item, out double tempResp) ? tempResp : 0;
                        var responsavel = repCliente.BuscarPorCPFCNPJ(cnpjResponsavel);
                        if (responsavel == null)
                            throw new Exception($"Responsável de Pagamento (CNPJ {cnpjResponsavel}) não encontrado");

                        if (cargaPedido == null)
                        {
                            // se nao tem carga entao preenche no vale pegadio do mdfe
                            var valePedagioExistente = repMdfeValePedagio.BuscarPorMdfeENroComprovante(mdfe.Codigo, valePedagio.nCompra);
                            if (valePedagioExistente.Count > 0)
                                repMdfeValePedagio.Deletar(valePedagioExistente);

                            Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();
                            valePedagioMDFe.MDFe = mdfe;
                            valePedagioMDFe.CNPJFornecedor = cnpjFornecedor.ToString();
                            valePedagioMDFe.CNPJResponsavel = cnpjResponsavel.ToString();
                            valePedagioMDFe.NumeroComprovante = valePedagio.nCompra;
                            valePedagioMDFe.ValorValePedagio = decimal.TryParse(valePedagio.vValePed.Replace(".", ","), out decimal valorPed) ? valorPed : 0;
                            valePedagioMDFe.TipoCompra = valePedagio.tpValePed switch
                            {
                                rodoInfANTTValePedDispTpValePed.Item01 => TipoCompraValePedagio.Tag,
                                rodoInfANTTValePedDispTpValePed.Item02 => TipoCompraValePedagio.Cupom,
                                rodoInfANTTValePedDispTpValePed.Item03 => TipoCompraValePedagio.Cartao,
                                _ => TipoCompraValePedagio.Nenhum
                            };
                            repMdfeValePedagio.Inserir(valePedagioMDFe);

                        }
                        else
                        {
                            // se tem carga entao preenche no vale pedagio da carga

                            var valePedagioExistente = repCargaValePedagio.BuscarPorCargaENroComprovante(cargaPedido.Carga.Codigo, valePedagio.nCompra);
                            if (valePedagioExistente.Count > 0)
                                repCargaValePedagio.Deletar(valePedagioExistente);

                            Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio();
                            cargaValePedagio.Carga = cargaPedido.Carga;
                            cargaValePedagio.Fornecedor = fornecedor;
                            cargaValePedagio.Responsavel = responsavel;
                            cargaValePedagio.NumeroComprovante = valePedagio.nCompra;
                            cargaValePedagio.Valor = decimal.TryParse(valePedagio.vValePed.Replace(".", ","), out decimal valorPed) ? valorPed : 0;
                            cargaValePedagio.NaoIncluirMDFe = false;
                            cargaValePedagio.TipoCompra = valePedagio.tpValePed switch
                            {
                                rodoInfANTTValePedDispTpValePed.Item01 => TipoCompraValePedagio.Tag,
                                rodoInfANTTValePedDispTpValePed.Item02 => TipoCompraValePedagio.Cupom,
                                rodoInfANTTValePedDispTpValePed.Item03 => TipoCompraValePedagio.Cartao,
                                _ => TipoCompraValePedagio.Nenhum
                            };
                            repCargaValePedagio.Inserir(cargaValePedagio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ObterCIOTMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.ModalRodoviario.rodo rodo, Repositorio.UnitOfWork unitOfWork)
        {
            if (rodo != null)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                mdfe.CIOT = rodo.infANTT?.infCIOT?.Select(o => o.CIOT).FirstOrDefault();

                repMDFe.Atualizar(mdfe);
            }
        }

        private void ObterProdutoPredominante(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeProdPred prodPred)
        {
            if (prodPred != null)
            {
                mdfe.ProdutoPredominanteNCM = prodPred.NCM;
                mdfe.ProdutoPredominanteDescricao = prodPred.xProd;
                mdfe.ProdutoPredominanteCEAN = prodPred.cEAN;

                ObterDadosLotacao(mdfe, prodPred.infLotacao);
            }
        }

        private void ObterDadosLotacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeProdPredInfLotacao infLotacao)
        {
            if (infLotacao != null)
            {
                ObterDadosLotacaoLocalCarregamento(mdfe, infLotacao.infLocalCarrega);
                ObterDadosLotacaoLocalDescarregamento(mdfe, infLotacao.infLocalDescarrega);
            }
        }

        private void ObterDadosLotacaoLocalCarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeProdPredInfLotacaoInfLocalCarrega infLocalCarrega)
        {
            if (infLocalCarrega != null)
            {
                if (infLocalCarrega.ItemsElementName?.Length > 0 &&
                    infLocalCarrega.Items?.Length > 0)
                {
                    for (int i = 0; i < infLocalCarrega.ItemsElementName.Length; i++)
                    {
                        switch (infLocalCarrega.ItemsElementName[i])
                        {
                            case MultiSoftware.MDFe.v300.ItemsChoiceType.CEP:
                                mdfe.CEPCarregamentoLotacao = infLocalCarrega.Items[i];
                                break;
                            case MultiSoftware.MDFe.v300.ItemsChoiceType.latitude:
                                mdfe.LatitudeCarregamentoLotacao = Utilidades.Decimal.Converter(infLocalCarrega.Items[i]);
                                break;
                            case MultiSoftware.MDFe.v300.ItemsChoiceType.longitude:
                                mdfe.LongitudeCarregamentoLotacao = Utilidades.Decimal.Converter(infLocalCarrega.Items[i]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void ObterDadosLotacaoLocalDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeProdPredInfLotacaoInfLocalDescarrega infLocalDescarrega)
        {
            if (infLocalDescarrega != null)
            {
                if (infLocalDescarrega.ItemsElementName?.Length > 0 &&
                    infLocalDescarrega.Items?.Length > 0)
                {
                    for (int i = 0; i < infLocalDescarrega.ItemsElementName.Length; i++)
                    {
                        switch (infLocalDescarrega.ItemsElementName[i])
                        {
                            case MultiSoftware.MDFe.v300.ItemsChoiceType1.CEP:
                                mdfe.CEPDescarregamentoLotacao = infLocalDescarrega.Items[i];
                                break;
                            case MultiSoftware.MDFe.v300.ItemsChoiceType1.latitude:
                                mdfe.LatitudeDescarregamentoLotacao = Utilidades.Decimal.Converter(infLocalDescarrega.Items[i]);
                                break;
                            case MultiSoftware.MDFe.v300.ItemsChoiceType1.longitude:
                                mdfe.LongitudeDescarregamentoLotacao = Utilidades.Decimal.Converter(infLocalDescarrega.Items[i]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private async Task ObterVeiculoMDFeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v100a.ModalRodoviario.rodo rodo)
        {
            if (rodo != null)
            {
                Repositorio.VeiculoMDFe repositorioVeiculoMDFe = new Repositorio.VeiculoMDFe(_unitOfWork, _cancellationToken);
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = rodo.veicTracao.placa;
                veiculoMDFe.RENAVAM = rodo.veicTracao.RENAVAM;
                veiculoMDFe.Tara = int.Parse(rodo.veicTracao.tara);
                veiculoMDFe.TipoCarroceria = ((int)rodo.veicTracao.tpCar).ToString("00");
                veiculoMDFe.TipoProprietario = "P";
                veiculoMDFe.TipoRodado = ((int)rodo.veicTracao.tpRod).ToString("00");
                veiculoMDFe.UF = mdfe.Empresa.Localidade.Estado;
                veiculoMDFe.UFProprietario = mdfe.Empresa.Localidade.Estado;
                veiculoMDFe.RNTRC = rodo.RNTRC;

                await repositorioVeiculoMDFe.InserirAsync(veiculoMDFe);

                await ObterMotoristasMDFeAsync(mdfe, rodo.veicTracao.condutor);
            }
        }

        private void ObterVeiculoMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.ModalRodoviario.rodo rodo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (rodo != null)
            {
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = rodo.veicTracao.placa;
                veiculoMDFe.RENAVAM = rodo.veicTracao.RENAVAM;
                veiculoMDFe.Tara = int.Parse(rodo.veicTracao.tara);
                veiculoMDFe.TipoCarroceria = ((int)rodo.veicTracao.tpCar).ToString("00");
                veiculoMDFe.TipoProprietario = "P";
                veiculoMDFe.TipoRodado = ((int)rodo.veicTracao.tpRod).ToString("00");
                veiculoMDFe.UF = mdfe.Empresa.Localidade.Estado;
                veiculoMDFe.UFProprietario = mdfe.Empresa.Localidade.Estado;
                veiculoMDFe.RNTRC = rodo.infANTT?.RNTRC ?? mdfe.Empresa.RegistroANTT;

                repVeiculoMDFe.Inserir(veiculoMDFe);

                ObterMotoristasMDFe(mdfe, rodo.veicTracao.condutor, unidadeDeTrabalho);
            }
        }

        private async Task ObterReboquesMDFeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v100a.ModalRodoviario.rodo rodo)
        {
            if (rodo != null && rodo.veicReboque != null && rodo.veicReboque.Length > 0)
            {
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(_unitOfWork, _cancellationToken);

                for (var i = 0; i < rodo.veicReboque.Length; i++)
                {
                    MultiSoftware.MDFe.v100a.ModalRodoviario.rodoVeicReboque reboque = rodo.veicReboque[i];

                    Dominio.Entidades.ReboqueMDFe reboqueMDFe = new Dominio.Entidades.ReboqueMDFe();
                    reboqueMDFe.MDFe = mdfe;
                    reboqueMDFe.Placa = reboque.placa;
                    reboqueMDFe.RENAVAM = reboque.RENAVAM;
                    reboqueMDFe.Tara = int.Parse(reboque.tara);
                    reboqueMDFe.TipoCarroceria = ((int)reboque.tpCar).ToString("00");
                    reboqueMDFe.TipoProprietario = "P";
                    reboqueMDFe.UF = mdfe.Empresa.Localidade.Estado;
                    reboqueMDFe.UFProprietario = mdfe.Empresa.Localidade.Estado;
                    reboqueMDFe.RNTRC = rodo.RNTRC;

                    await repReboqueMDFe.InserirAsync(reboqueMDFe);
                }
            }
        }

        private void ObterReboquesMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.ModalRodoviario.rodo rodo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (rodo != null && rodo.veicReboque != null && rodo.veicReboque.Length > 0)
            {
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);

                for (var i = 0; i < rodo.veicReboque.Length; i++)
                {
                    MultiSoftware.MDFe.v300.ModalRodoviario.rodoVeicReboque reboque = rodo.veicReboque[i];

                    Dominio.Entidades.ReboqueMDFe reboqueMDFe = new Dominio.Entidades.ReboqueMDFe();
                    reboqueMDFe.MDFe = mdfe;
                    reboqueMDFe.Placa = reboque.placa;
                    reboqueMDFe.RENAVAM = reboque.RENAVAM;
                    reboqueMDFe.Tara = int.Parse(reboque.tara);
                    reboqueMDFe.TipoCarroceria = ((int)reboque.tpCar).ToString("00");
                    reboqueMDFe.TipoProprietario = "P";
                    reboqueMDFe.UF = mdfe.Empresa.Localidade.Estado;
                    reboqueMDFe.UFProprietario = mdfe.Empresa.Localidade.Estado;
                    reboqueMDFe.RNTRC = rodo.infANTT?.RNTRC ?? mdfe.Empresa.RegistroANTT;

                    repReboqueMDFe.Inserir(reboqueMDFe);
                }
            }
        }

        private async Task ObterMunicipiosCarregamentoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<string> municipiosCarregamento)
        {
            if (municipiosCarregamento != null)
            {
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork, _cancellationToken);
                Repositorio.MunicipioCarregamentoMDFe repositorioMunicipioCarregamentoMDFe = new Repositorio.MunicipioCarregamentoMDFe(_unitOfWork, _cancellationToken);

                foreach (string municipio in municipiosCarregamento)
                {
                    int.TryParse(municipio, out int codigoMunicipioCarregamento);

                    Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamentoMDFe = new Dominio.Entidades.MunicipioCarregamentoMDFe()
                    {
                        MDFe = mdfe,
                        Municipio = await repositorioLocalidade.BuscarPorCodigoIBGEAsync(codigoMunicipioCarregamento, _cancellationToken)
                    };

                    await repositorioMunicipioCarregamentoMDFe.InserirAsync(municipioCarregamentoMDFe);
                }
            }
        }

        private async Task ObterMunicipiosDescarregamentoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v100a.TMDFeInfMDFeInfMunDescarga[] linfMunDescarga)
        {
            if (linfMunDescarga != null)
            {
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork, _cancellationToken);
                Repositorio.MunicipioDescarregamentoMDFe repMunicipioCarregamentoMDFe = new Repositorio.MunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

                foreach (MultiSoftware.MDFe.v100a.TMDFeInfMDFeInfMunDescarga infMunDescarga in linfMunDescarga)
                {
                    int.TryParse(infMunDescarga.cMunDescarga, out int codigoMunicipioDescarregamento);

                    Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoMDFe = new Dominio.Entidades.MunicipioDescarregamentoMDFe()
                    {
                        MDFe = mdfe,
                        Municipio = await repositorioLocalidade.BuscarPorCodigoIBGEAsync(codigoMunicipioDescarregamento, _cancellationToken),
                    };

                    await repMunicipioCarregamentoMDFe.InserirAsync(municipioDescarregamentoMDFe);

                    await ObterCTeMunicipiosDescarregamentoAsync(municipioDescarregamentoMDFe, infMunDescarga.infCTe.Select(x => x.chCTe).ToList());
                }
            }
        }

        private async Task ObterMunicipiosDescarregamentoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeInfMunDescarga[] linfMunDescarga)
        {
            if (linfMunDescarga != null)
            {
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork, _cancellationToken);
                Repositorio.MunicipioDescarregamentoMDFe repositorioMunicipioCarregamentoMDFe = new Repositorio.MunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

                foreach (MultiSoftware.MDFe.v300.TMDFeInfMDFeInfMunDescarga infMunDescarga in linfMunDescarga)
                {
                    int.TryParse(infMunDescarga.cMunDescarga, out int codigoMunicipioDescarregamento);

                    Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoMDFe = new Dominio.Entidades.MunicipioDescarregamentoMDFe()
                    {
                        MDFe = mdfe,
                        Municipio = await repositorioLocalidade.BuscarPorCodigoIBGEAsync(codigoMunicipioDescarregamento, _cancellationToken),
                    };

                    await repositorioMunicipioCarregamentoMDFe.InserirAsync(municipioDescarregamentoMDFe);

                    await ObterCTeMunicipiosDescarregamentoAsync(municipioDescarregamentoMDFe, infMunDescarga.infCTe.Select(x => x.chCTe).ToList());
                }
            }
        }

        private async Task ObterCTeMunicipiosDescarregamentoAsync(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoMDFe, List<string> listaChaveCTe)
        {
            if (listaChaveCTe != null)
            {
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, _cancellationToken);

                foreach (string chave in listaChaveCTe)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioCTe.BuscarPorChaveAsync(municipioDescarregamentoMDFe.MDFe.Empresa.Codigo, chave);

                    Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoMunicipioDescarregamentoMDFe = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe()
                    {
                        CTe = cte,
                        Chave = chave,
                        MunicipioDescarregamento = municipioDescarregamentoMDFe
                    };

                    await repositorioDocumentoMunicipioDescarregamentoMDFe.InserirAsync(documentoMunicipioDescarregamentoMDFe);
                }
            }
        }

        private async Task ObterPercursoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<string> percursos)
        {
            if (percursos != null)
            {
                Repositorio.PercursoMDFe repositorioPercursoMDFe = new Repositorio.PercursoMDFe(_unitOfWork, _cancellationToken);
                Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork, _cancellationToken);

                foreach (string percurso in percursos)
                {
                    Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe()
                    {
                        MDFe = mdfe,
                        Estado = await repositorioEstado.BuscarPorSiglaAsync(percurso)
                    };

                    await repositorioPercursoMDFe.InserirAsync(percursoMDFe);
                }
            }
        }

        private async Task ObterSeguroAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeSeg[] lInfSeguro)
        {
            if (lInfSeguro != null)
            {
                Repositorio.MDFeSeguro repositorioSeguroMDFe = new Repositorio.MDFeSeguro(_unitOfWork, _cancellationToken);

                foreach (MultiSoftware.MDFe.v300.TMDFeInfMDFeSeg inf in lInfSeguro)
                {
                    Dominio.Entidades.MDFeSeguro seguroMDFe = new Dominio.Entidades.MDFeSeguro()
                    {
                        MDFe = mdfe,
                        CNPJSeguradora = inf.infSeg?.CNPJ ?? string.Empty,
                        NomeSeguradora = inf.infSeg?.xSeg ?? string.Empty,
                        NumeroApolice = inf.nApol ?? string.Empty,
                        NumeroAverbacao = inf.nAver.FirstOrDefault() ?? string.Empty,
                        Responsavel = inf.infResp?.Item ?? string.Empty,
                        TipoResponsavel = (Dominio.Enumeradores.TipoResponsavelSeguroMDFe)inf.infResp?.respSeg
                    };

                    await repositorioSeguroMDFe.InserirAsync(seguroMDFe);
                }
            }
        }

        private async Task ObterLacresAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<string> mdfeLacres)
        {
            if (mdfeLacres != null)
            {
                Repositorio.LacreMDFe repositorioLacreMDFe = new Repositorio.LacreMDFe(_unitOfWork, _cancellationToken);

                foreach (string lacre in mdfeLacres)
                {
                    Dominio.Entidades.LacreMDFe percursoMDFe = new Dominio.Entidades.LacreMDFe()
                    {
                        MDFe = mdfe,
                        Numero = lacre
                    };

                    await repositorioLacreMDFe.InserirAsync(percursoMDFe);
                }
            }
        }

        private async Task ObterMotoristasMDFeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v100a.ModalRodoviario.rodoVeicTracaoCondutor[] lRodoVeicTracaoCondutor)
        {
            if (lRodoVeicTracaoCondutor != null)
            {
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(_unitOfWork, _cancellationToken);

                foreach (MultiSoftware.MDFe.v100a.ModalRodoviario.rodoVeicTracaoCondutor rodoVeicTracaoCondutor in lRodoVeicTracaoCondutor)
                {
                    Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe()
                    {
                        CPF = rodoVeicTracaoCondutor.CPF,
                        Nome = rodoVeicTracaoCondutor.xNome,
                        MDFe = mdfe,
                        Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal
                    };

                    await repMotoristaMDFe.InserirAsync(motoristaMDFe);
                }
            }
        }

        private void ObterMotoristasMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, MultiSoftware.MDFe.v300.ModalRodoviario.rodoVeicTracaoCondutor[] lRodoVeicTracaoCondutor, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (lRodoVeicTracaoCondutor != null)
            {
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeTrabalho);

                foreach (MultiSoftware.MDFe.v300.ModalRodoviario.rodoVeicTracaoCondutor rodoVeicTracaoCondutor in lRodoVeicTracaoCondutor)
                {
                    Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe()
                    {
                        CPF = rodoVeicTracaoCondutor.CPF,
                        Nome = rodoVeicTracaoCondutor.xNome,
                        MDFe = mdfe,
                        Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal
                    };

                    repMotoristaMDFe.Inserir(motoristaMDFe);
                }
            }
        }

        private async Task<Dominio.Entidades.EmpresaSerie> ObterSerieAsync(Dominio.Entidades.Empresa empresa, int serie)
        {
            Repositorio.EmpresaSerie repositorioSerie = new Repositorio.EmpresaSerie(_unitOfWork, _cancellationToken);

            Dominio.Entidades.EmpresaSerie empSerie = await repositorioSerie.BuscarPorSerieAsync(empresa.Codigo, serie, Dominio.Enumeradores.TipoSerie.MDFe);

            if (empSerie == null)
            {
                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = serie;
                empSerie.Status = "A";
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;
                await repositorioSerie.InserirAsync(empSerie);
            }
            return empSerie;
        }

        public void AdicionarCTesMDFeAnterior(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, string numeroCargaMDFeAnterior, int codigoEmpresa, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamentoMDFe = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentos = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

            Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

            List<Dominio.Entidades.IntegracaoMDFe> listaIntegracaoMDFe = repIntegracaoMDFe.BuscarPorCarga(numeroCargaMDFeAnterior, Dominio.Enumeradores.StatusMDFe.Autorizado, codigoEmpresa);

            bool adicionouCTes = false;

            foreach (Dominio.Entidades.IntegracaoMDFe integracaoMDFe in listaIntegracaoMDFe)
            {
                if (integracaoMDFe.MDFe != null)
                {
                    adicionouCTes = true;
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeAnterior = repMDFe.BuscarPorCodigo(integracaoMDFe.MDFe.Codigo);

                    if (mdfeAnterior.EstadoDescarregamento.Sigla == mdfe.EstadoDescarregamento.Sigla)
                    {
                        foreach (Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoMDFeAnterior in mdfeAnterior.MunicipiosDescarregamento)
                        {
                            Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = repMunicipioDescarregamentoMDFe.BuscarPorMunicipioDescarregamento(mdfe.Codigo, municipioDescarregamentoMDFeAnterior.Municipio.Codigo); //mdfe.MunicipiosDescarregamento.Where((o => o.Municipio.Codigo == municipioDescarregamentoMDFeAnterior.Municipio.Codigo)).FirstOrDefault();  //

                            if (municipioDescarregamento == null)
                            {
                                municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                                municipioDescarregamento.MDFe = mdfe;
                                municipioDescarregamento.Municipio = municipioDescarregamentoMDFeAnterior.Municipio;
                                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioDescarregamento.Municipio.CEP);
                                mdfe.LatitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Latitude;
                                mdfe.LongitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Longitude;

                                repMunicipioDescarregamentoMDFe.Inserir(municipioDescarregamento);

                                this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamento, municipioDescarregamentoMDFeAnterior.Documentos.ToList(), unidadeDeTrabalho);
                            }
                            else
                            {
                                this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamento, municipioDescarregamentoMDFeAnterior.Documentos.ToList(), unidadeDeTrabalho);
                            }
                        }
                    }
                }
            }

            if (adicionouCTes)
            {
                List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos = new List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();
                documentos = repDocumentos.BuscarPorMDFe(mdfe.Codigo);

                mdfe.PesoBrutoMercadoria = 0;
                for (var i = 0; i < documentos.Count; i += 4000)
                    mdfe.PesoBrutoMercadoria += repInformacaoCargaCTe.ObterPesoTotal((from obj in documentos.Skip(i).Take(i + 4000) select obj.CTe.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);

                mdfe.ValorTotalMercadoria = (from obj in documentos select obj.CTe.ValorTotalMercadoria).Sum();

                mdfe.TipoCargaMDFe = tipoCargaMDFe;
                mdfe.ProdutoPredominanteDescricao = (from obj in documentos select obj.CTe.ProdutoPredominante).FirstOrDefault();

                repMDFe.Atualizar(mdfe);
            }
        }

        public bool SolicitarEmissaoContingencia(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMdfe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMdfe.BuscarPorCodigo(codigoMDFe);

                if (mdfe != null)
                {
                    if (this.Contingencia(mdfe.CodigoIntegradorAutorizacao, unitOfWork))
                    {
                        this.ObterESalvarXMLContingencia(mdfe.Codigo, mdfe.Empresa.Codigo, null);


                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return false;
            }
        }

        public bool SolicitarEncerramentoAutomaticoChave(int codigoEmpresa, string chaveMDFeNovo, string chaveMDFe, string protocoloMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMdfe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMdfe.BuscarPorChave(chaveMDFe);

            if (mdfe != null)
            {
                bool encerrarMDFeComMesmaData = (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EncerraMDFeAutomaticoComMesmaData.Value || (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.EncerrarMDFeComMesmaData));

                //Se empresa não tiver configuração OU empresa configurada para encerrar
                if (mdfe.Empresa.Configuracao == null || mdfe.Empresa.Configuracao.EncerramentoMDFeAutomatico != Dominio.Enumeradores.EncerramentoMDFeAutomatico.Nenhum)
                {
                    if ((mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) && (mdfe.DataEmissao < DateTime.Today || encerrarMDFeComMesmaData))
                    {
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                        DateTime dataEncerramento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

                        dataEncerramento = TimeZoneInfo.ConvertTime(dataEncerramento, TimeZoneInfo.Local, fusoHorarioEmpresa);
                        Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unitOfWork, dataEncerramento);
                        this.AdicionarMDFeNaFilaDeConsulta(mdfe, unitOfWork);

                        // Adiciona o log
                        this.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, "Encerramento Automatico MDF-e existente na base devido para emissão do MDFe " + chaveMDFeNovo, unitOfWork);

                        return true;
                    }
                    else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                    {
                        mdfe = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ConsultarEventoEncerramento(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);

                        if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                            this.AdicionarMDFeNaFilaDeConsulta(mdfe, unitOfWork);

                        return true;
                    }
                    else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {

                string cnpjChave = chaveMDFe.Substring(6, 14);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjChave);

                if (empresa != null)
                {
                    bool encerrarMDFeOutrosSistemas = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EncerraMDFeAutomaticoOutrosSistemas.Value;
                    if (empresa.Configuracao != null)
                    {
                        if (empresa.Configuracao.EncerramentoMDFeAutomatico == Dominio.Enumeradores.EncerramentoMDFeAutomatico.Nenhum)
                            return false;
                        if (empresa.Configuracao.EncerramentoMDFeAutomatico == Dominio.Enumeradores.EncerramentoMDFeAutomatico.Todos)
                            encerrarMDFeOutrosSistemas = true;
                    }

                    if (encerrarMDFeOutrosSistemas)
                    {
                        DateTime dataEncerramento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
                        dataEncerramento = TimeZoneInfo.ConvertTime(dataEncerramento, TimeZoneInfo.Local, fusoHorarioEmpresa);

                        Embarcador.Carga.MDFe.EncerrarMDFeEmissorExterno(out string erroMDFeExterno, chaveMDFe, empresa.Localidade, protocoloMDFe, empresa, dataEncerramento, null, unitOfWork);

                        return true;
                    }
                }
                return false;
            }
        }

        public byte[] ObterESalvarXMLAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, ServicoMDFe.RetornoMDFe retorno = null)
        {
            byte[] retArquivo = null;

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeDeTrabalho);

            if (mdfe != null)
            {
                if (retorno == null)
                {
                    ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                    retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);
                }

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Autorizacao;
                    xmlMDFe.XML = retorno.XML;

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);

                    retArquivo = Encoding.UTF8.GetBytes(retorno.XML);
                }
            }

            return retArquivo;
        }

        public void SalvarXMLAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeTrabalho);

            if (mdfe != null)
            {
                if (!string.IsNullOrWhiteSpace(mdfeOracle.XMLAutorizacao))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Autorizacao;
                    xmlMDFe.XML = mdfeOracle.XMLAutorizacao;

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);
                }
            }
        }

        public void SalvarXMLEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Enumeradores.TipoXMLMDFe tipo, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeTrabalho);

            if (mdfe != null)
            {
                if (!string.IsNullOrWhiteSpace(mdfeOracle.XMLEncerramento))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = tipo == Dominio.Enumeradores.TipoXMLMDFe.Encerramento ? repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Encerramento) : null;

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = tipo;
                    xmlMDFe.XML = Utilidades.String.RemoveAccents(mdfeOracle.XMLEncerramento);

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);
                }
            }
        }

        public void SalvarXMLCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeTrabalho);

            if (mdfe != null)
            {
                if (!string.IsNullOrWhiteSpace(mdfeOracle.XMLCancelamento))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Cancelamento;
                    xmlMDFe.XML = mdfeOracle.XMLCancelamento;

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);
                }
            }
        }

        public void SalvarXMLInclusaoMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle mdfeOracle, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeTrabalho);

            if (mdfe != null)
            {
                if (!string.IsNullOrWhiteSpace(mdfeOracle.XMLEvento))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.InclusaoMorotista);

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.InclusaoMorotista;
                    xmlMDFe.XML = mdfeOracle.XMLEvento;

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);
                }
            }
        }

        public void ObterESalvarXMLContingencia(int codigoMDFe, int codigoEmpresa, ServicoMDFe.RetornoMDFe retorno = null, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

            if (mdfe != null)
            {
                if (retorno == null)
                {
                    ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                    retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);
                }

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Contingencia);

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Contingencia;
                    xmlMDFe.XML = retorno.XML;

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);
                }
            }
        }

        public byte[] ObterESalvarXMLCancelamento(int codigoMDFe, int codigoEmpresa, ServicoMDFe.RetornoEventoMDFe retorno = null, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            byte[] retArquivo = null;

            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

            if (mdfe != null)
            {
                if (retorno == null)
                {
                    ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                    retorno = svcMDFe.ConsultarEventoMDFe(mdfe.CodigoIntegradorCancelamento);
                }

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Cancelamento;
                    xmlMDFe.XML = retorno.XML;

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);

                    retArquivo = Encoding.UTF8.GetBytes(retorno.XML);
                }
            }

            return retArquivo;
        }

        public byte[] ObterESalvarXMLCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho, ServicoMDFe.RetornoEventoMDFe retorno = null)
        {
            byte[] retArquivo = null;

            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeDeTrabalho);

            if (mdfe == null)
                return retArquivo;

            if (retorno == null)
            {
                ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                retorno = svcMDFe.ConsultarEventoMDFe(mdfe.CodigoIntegradorCancelamento);
            }

            if (!string.IsNullOrWhiteSpace(retorno.XML))
            {
                Dominio.Entidades.XMLMDFe xmlMDFe = repXML.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);

                if (xmlMDFe == null)
                    xmlMDFe = new Dominio.Entidades.XMLMDFe();

                xmlMDFe.MDFe = mdfe;
                xmlMDFe.Tipo = Dominio.Enumeradores.TipoXMLMDFe.Cancelamento;
                xmlMDFe.XML = retorno.XML;

                if (xmlMDFe.Codigo > 0)
                    repXML.Atualizar(xmlMDFe);
                else
                    repXML.Inserir(xmlMDFe);

                retArquivo = Encoding.UTF8.GetBytes(retorno.XML);
            }

            return retArquivo;
        }

        public void ObterESalvarXMLEvento(int codigoMDFe, int codigoEmpresa, Dominio.Enumeradores.TipoXMLMDFe tipoXMLMDFe, ServicoMDFe.RetornoEventoMDFe retorno = null, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

            if (mdfe != null)
            {
                if (retorno == null)
                {
                    ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                    retorno = svcMDFe.ConsultarEventoMDFe(mdfe.CodigoIntegradorEncerramento);
                }

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = tipoXMLMDFe == Dominio.Enumeradores.TipoXMLMDFe.Encerramento ? repXML.BuscarPorMDFe(mdfe.Codigo, tipoXMLMDFe) : null;

                    if (xmlMDFe == null)
                        xmlMDFe = new Dominio.Entidades.XMLMDFe();

                    xmlMDFe.MDFe = mdfe;
                    xmlMDFe.Tipo = tipoXMLMDFe;
                    xmlMDFe.XML = Utilidades.String.RemoveAccents(retorno.XML);

                    if (xmlMDFe.Codigo > 0)
                        repXML.Atualizar(xmlMDFe);
                    else
                        repXML.Inserir(xmlMDFe);
                }
            }
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.MDFe.MDFe mdfe, Dominio.Enumeradores.TipoEmitenteMDFe? mdfeTipoEmitente, Repositorio.UnitOfWork unidadeDeTrabalho, bool ctesExternos = false)
        {
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unidadeDeTrabalho);
            Repositorio.ModalTransporte repositorioModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repositorioEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();

            manifesto.CIOT = mdfe.CIOT;

            DateTime dataEmissao;
            DateTime.TryParseExact(mdfe.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

            manifesto.DataEmissao = dataEmissao != DateTime.MinValue ? dataEmissao : DateTime.Now;
            manifesto.Empresa = empresa;
            manifesto.Versao = !string.IsNullOrWhiteSpace(mdfe.Versao) ? mdfe.Versao : empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            manifesto.EstadoCarregamento = repositorioEstado.BuscarPorSigla(mdfe.UFCarregamento);
            manifesto.EstadoDescarregamento = repositorioEstado.BuscarPorSigla(mdfe.UFDescarregamento);
            manifesto.Modal = repositorioModal.BuscarPorCodigo(1, false);
            manifesto.Modelo = repModelo.BuscarPorModelo("58");
            manifesto.ObservacaoContribuinte = mdfe.ObservacaoContribuinte;
            manifesto.ObservacaoFisco = mdfe.ObservacaoFisco;
            manifesto.PesoBrutoMercadoria = mdfe.PesoBrutoMercadoria;
            manifesto.RNTRC = empresa.RegistroANTT;
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            manifesto.TipoAmbiente = empresa.TipoAmbiente;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            if (!string.IsNullOrWhiteSpace(mdfe.MunicipiosDeDescarregamento.FirstOrDefault().Documentos.FirstOrDefault().ChaveNFe))
                manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte;
            else if (ctesExternos)
                manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporteApenasChaveCTe;
            else
                manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporte;
            manifesto.UnidadeMedidaMercadoria = mdfe.UnidadeMedidaMercadoria;
            manifesto.ValorTotalMercadoria = mdfe.ValorTotalMercadoria;
            manifesto.TentativaEncerramentoAutomatico = 0;

            manifesto.TipoCargaMDFe = mdfe.TipoCargaMDFe != null && mdfe.TipoCargaMDFe != Dominio.Enumeradores.TipoCargaMDFe.NaoDefinido ? mdfe.TipoCargaMDFe.Value : Dominio.Enumeradores.TipoCargaMDFe.CargaGeral;
            manifesto.ProdutoPredominanteDescricao = !string.IsNullOrWhiteSpace(mdfe.ProdutoPredominanteDescricao) ? mdfe.ProdutoPredominanteDescricao : "DIVERSOS";
            manifesto.ProdutoPredominanteCEAN = mdfe.ProdutoPredominanteCEAN;
            manifesto.ProdutoPredominanteNCM = mdfe.ProdutoPredominanteNCM;

            if (mdfe.NumeroMDFe > 0 && mdfe.SerieMDFe > 0)
            {
                manifesto.Serie = repositorioEmpresaSerie.BuscarPorSerie(empresa.Codigo, mdfe.SerieMDFe, Dominio.Enumeradores.TipoSerie.MDFe);

                if (manifesto.Serie == null)
                {
                    manifesto.Serie = new Dominio.Entidades.EmpresaSerie { };
                    manifesto.Serie.Numero = mdfe.SerieMDFe;
                    manifesto.Serie.Empresa = empresa;
                    manifesto.Serie.Status = "A";
                    manifesto.Serie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;

                    repositorioEmpresaSerie.Inserir(manifesto.Serie);
                }

                manifesto.Numero = mdfe.NumeroMDFe;
            }
            else
            {
                manifesto.Serie = empresa.Configuracao.SerieMDFe;
                manifesto.Numero = repMDFe.BuscarUltimoNumero(empresa.Codigo, manifesto.Serie.Codigo, empresa.TipoAmbiente) + 1;
            }

            repMDFe.Inserir(manifesto);

            return manifesto;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<object> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Servicos.NFe svcNFe = new NFe(unidadeDeTrabalho);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unidadeDeTrabalho);
            Repositorio.ModalTransporte repositorioModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            string ufCarregamento = null, ufDescarregamento = null;

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();

            for (var i = 0; i < notasFiscais.Count(); i++)
            {
                if (notasFiscais[i] != null)
                {
                    if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                    {
                        MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                        manifesto.PesoBrutoMercadoria += svcNFe.ObterPeso(notaFiscal.NFe.infNFe.transp, unidadeDeTrabalho);
                        manifesto.ValorTotalMercadoria += decimal.Parse(notaFiscal.NFe.infNFe.total.ICMSTot.vNF, cultura);
                        manifesto.ProdutoPredominanteDescricao = "DIVERSOS";
                        manifesto.TipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral;

                        if (string.IsNullOrWhiteSpace(ufCarregamento))
                            ufCarregamento = notaFiscal.NFe.infNFe.emit.enderEmit.UF.ToString("G");

                        if (string.IsNullOrWhiteSpace(ufDescarregamento))
                            ufDescarregamento = notaFiscal.NFe.infNFe.dest.enderDest.UF.ToString("G");
                    }
                    else if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                    {
                        MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                        manifesto.PesoBrutoMercadoria += svcNFe.ObterPeso(notaFiscal.NFe.infNFe.transp);
                        manifesto.ValorTotalMercadoria += decimal.Parse(notaFiscal.NFe.infNFe.total.ICMSTot.vNF, cultura);
                        manifesto.ProdutoPredominanteDescricao = "DIVERSOS";
                        manifesto.TipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral;

                        if (string.IsNullOrWhiteSpace(ufCarregamento))
                            ufCarregamento = notaFiscal.NFe.infNFe.emit.enderEmit.UF.ToString("G");

                        if (string.IsNullOrWhiteSpace(ufDescarregamento))
                            ufDescarregamento = notaFiscal.NFe.infNFe.dest.enderDest.UF.ToString("G");
                    }
                }
            }

            manifesto.DataEmissao = DateTime.Now;
            manifesto.Empresa = empresa;
            manifesto.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            manifesto.EstadoCarregamento = repositorioEstado.BuscarPorSigla(ufCarregamento);
            manifesto.EstadoDescarregamento = repositorioEstado.BuscarPorSigla(ufDescarregamento);
            manifesto.Modal = repositorioModal.BuscarPorCodigo(1, false);
            manifesto.Modelo = repModelo.BuscarPorModelo("58");

            manifesto.RNTRC = empresa.RegistroANTT;
            manifesto.Serie = empresa.Configuracao.SerieMDFe;
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            manifesto.TipoAmbiente = empresa.TipoAmbiente;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte;
            manifesto.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            manifesto.TentativaEncerramentoAutomatico = 0;

            manifesto.Numero = repMDFe.BuscarUltimoNumero(empresa.Codigo, manifesto.Serie.Codigo, empresa.TipoAmbiente) + 1;

            repMDFe.Inserir(manifesto);

            return manifesto;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.IntegracaoCTe> integracoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ModalTransporte repModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();

            mdfe.DataEmissao = DateTime.Now;
            mdfe.Empresa = empresa;
            mdfe.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "1.00";

            mdfe.EstadoCarregamento = integracoes.First().CTe.LocalidadeInicioPrestacao.Estado;
            mdfe.EstadoDescarregamento = integracoes.First().CTe.LocalidadeTerminoPrestacao.Estado;
            mdfe.Modal = integracoes.First().CTe.ModalTransporte;
            mdfe.Modelo = repModelo.BuscarPorModelo("58");
            mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            mdfe.PesoBrutoMercadoria = repInformacaoCargaCTe.ObterPesoTotal((from obj in integracoes select obj.CTe.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);
            if (mdfe.PesoBrutoMercadoria == 0)
            {
                mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.TON;
                mdfe.PesoBrutoMercadoria = repInformacaoCargaCTe.ObterPesoTotal((from obj in integracoes select obj.CTe.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);
            }
            if (mdfe.PesoBrutoMercadoria == 0)
                mdfe.PesoBrutoMercadoria = 1;
            mdfe.RNTRC = empresa.RegistroANTT;
            mdfe.Serie = empresa.Configuracao.SerieMDFe;
            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            mdfe.TipoAmbiente = empresa.TipoAmbiente;
            mdfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            mdfe.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporte;
            mdfe.ValorTotalMercadoria = (from obj in integracoes select obj.CTe.ValorTotalMercadoria).Sum();
            mdfe.TentativaEncerramentoAutomatico = 0;

            mdfe.ProdutoPredominanteDescricao = (from obj in integracoes select obj.CTe.ProdutoPredominante).FirstOrDefault();
            mdfe.TipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral;

            mdfe.Numero = repMDFe.BuscarUltimoNumero(empresa.Codigo, mdfe.Serie.Codigo, empresa.TipoAmbiente) + 1;

            repMDFe.Inserir(mdfe);

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfes, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, string observacaoFisco = "", string observacaoContribuinte = "", Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string produtoPredominante = "DIVERSOS", Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Servicos.NFe svcNFe = new NFe(unidadeDeTrabalho);
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
            Repositorio.ModalTransporte repositorioModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.Usuario repUsuarioSerie = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);

            string ufCarregamento = null, ufDescarregamento = null;

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();

            for (var i = 0; i < nfes.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXMLNotaFiscal.BuscarPorChave(nfes[i].Chave);
                if (notaFiscal != null)
                {
                    manifesto.PesoBrutoMercadoria += notaFiscal.Peso;
                    manifesto.ValorTotalMercadoria += notaFiscal.Valor;

                    if (string.IsNullOrWhiteSpace(ufCarregamento))
                        ufCarregamento = notaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? notaFiscal.Emitente.Localidade.Estado.Sigla : notaFiscal.Destinatario.Localidade.Estado.Sigla;

                    if (string.IsNullOrWhiteSpace(ufDescarregamento))
                        ufDescarregamento = notaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? notaFiscal.Destinatario.Localidade.Estado.Sigla : notaFiscal.Emitente.Localidade.Estado.Sigla;
                }
                else
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notafiscalInterna = repNotaFiscal.BuscarPorChave(nfes[i].Chave);
                    if (notafiscalInterna != null)
                    {
                        manifesto.PesoBrutoMercadoria += notafiscalInterna.TranspPesoBruto;
                        manifesto.ValorTotalMercadoria += notafiscalInterna.ValorTotalNota;

                        if (string.IsNullOrWhiteSpace(ufCarregamento))
                            ufCarregamento = notafiscalInterna.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida ? notafiscalInterna.Empresa.Localidade.Estado.Sigla : notafiscalInterna.Cliente.Localidade.Estado.Sigla;

                        if (string.IsNullOrWhiteSpace(ufDescarregamento))
                            ufDescarregamento = notafiscalInterna.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida ? notafiscalInterna.Empresa.Localidade.Estado.Sigla : notafiscalInterna.Cliente.Localidade.Estado.Sigla;
                    }
                }
            }

            manifesto.DataEmissao = DateTime.Now;
            manifesto.Empresa = empresa;
            manifesto.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            if (localidadeCarregamento != null)
                manifesto.EstadoCarregamento = localidadeCarregamento.Estado;
            else
                manifesto.EstadoCarregamento = repEstado.BuscarPorSigla(ufCarregamento);

            if (localidadeDescarregamento != null)
                manifesto.EstadoDescarregamento = localidadeDescarregamento.Estado;
            else
                manifesto.EstadoDescarregamento = repEstado.BuscarPorSigla(ufDescarregamento);

            manifesto.Modal = repositorioModal.BuscarPorCodigo(1, false);
            manifesto.Modelo = repModelo.BuscarPorModelo("58");

            manifesto.RNTRC = empresa.RegistroANTT;

            Dominio.Entidades.Usuario usuarioSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;
            if (codigoUsuario > 0)
                usuarioSerie = repUsuarioSerie.BuscarPorCodigo(codigoUsuario);
            if (usuarioSerie != null)
                serie = (from obj in usuarioSerie.Series where obj.Status.Equals("A") && obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe select obj).FirstOrDefault();


            manifesto.Serie = serie != null ? serie : repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            manifesto.TipoAmbiente = empresa.TipoAmbiente;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte;//tipoOperacao.ConfiguracaoEmissaoDocumento.TipoDeEmitente.HasValue ? tipoOperacao.ConfiguracaoEmissaoDocumento.TipoDeEmitente.Value : Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte;
            manifesto.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            manifesto.TentativaEncerramentoAutomatico = 0;

            manifesto.ProdutoPredominanteDescricao = produtoPredominante;
            manifesto.TipoCargaMDFe = tipoCargaMDFe;

            if (!string.IsNullOrWhiteSpace(observacaoFisco))
                manifesto.ObservacaoFisco = observacaoFisco;

            if (!string.IsNullOrWhiteSpace(observacaoContribuinte))
                manifesto.ObservacaoContribuinte = observacaoContribuinte;

            manifesto.Numero = repMDFe.BuscarUltimoNumero(empresa.Codigo, manifesto.Serie.Codigo, empresa.TipoAmbiente) + 1;

            repMDFe.Inserir(manifesto);

            return manifesto;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, string observacaoFisco = "", string observacaoContribuinte = "", Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string produtoPredominante = "DIVERSOS", Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Servicos.NFe svcNFe = new NFe(unidadeDeTrabalho);
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
            Repositorio.ModalTransporte repositorio = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.Usuario repUsuarioSerie = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

            string ufCarregamento = null, ufDescarregamento = null;

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();

            bool utilizarValorNotas = notasFiscais.Any(o => o.ValorTotalProdutos == 0);
            for (var i = 0; i < notasFiscais.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = notasFiscais[i];
                if (notasFiscais[i] != null)
                {
                    manifesto.PesoBrutoMercadoria += notaFiscal.Peso;
                    manifesto.ValorTotalMercadoria += utilizarValorNotas ? notaFiscal.Valor : notaFiscal.ValorTotalProdutos;

                    if (string.IsNullOrWhiteSpace(ufCarregamento))
                        ufCarregamento = notaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? notaFiscal.Emitente.Localidade.Estado.Sigla : notaFiscal.Destinatario.Localidade.Estado.Sigla;

                    if (string.IsNullOrWhiteSpace(ufDescarregamento))
                        ufDescarregamento = notaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? notaFiscal.Destinatario.Localidade.Estado.Sigla : notaFiscal.Emitente.Localidade.Estado.Sigla;
                }
            }

            manifesto.DataEmissao = DateTime.Now;
            manifesto.Empresa = empresa;
            manifesto.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            if (localidadeCarregamento != null)
                manifesto.EstadoCarregamento = localidadeCarregamento.Estado;
            else
                manifesto.EstadoCarregamento = repEstado.BuscarPorSigla(ufCarregamento);

            if (localidadeDescarregamento != null)
                manifesto.EstadoDescarregamento = localidadeDescarregamento.Estado;
            else
                manifesto.EstadoDescarregamento = repEstado.BuscarPorSigla(ufDescarregamento);

            manifesto.Modal = repositorio.BuscarPorCodigo(1, false);
            manifesto.Modelo = repModelo.BuscarPorModelo("58");

            manifesto.RNTRC = empresa.RegistroANTT;

            Dominio.Entidades.Usuario usuarioSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;
            if (codigoUsuario > 0)
                usuarioSerie = repUsuarioSerie.BuscarPorCodigo(codigoUsuario);
            if (usuarioSerie != null)
                serie = (from obj in usuarioSerie.Series where obj.Status.Equals("A") && obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe select obj).FirstOrDefault();


            manifesto.Serie = serie != null ? serie : empresa.Configuracao != null && empresa.Configuracao.SerieMDFe != null ? empresa.Configuracao.SerieMDFe : repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            manifesto.TipoAmbiente = empresa.TipoAmbiente;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            manifesto.TipoEmitente = tipoOperacao.ConfiguracaoEmissaoDocumento.TipoDeEmitente.HasValue ? tipoOperacao.ConfiguracaoEmissaoDocumento.TipoDeEmitente.Value : Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte;
            manifesto.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            manifesto.TentativaEncerramentoAutomatico = 0;

            manifesto.ProdutoPredominanteDescricao = produtoPredominante;
            manifesto.TipoCargaMDFe = tipoCargaMDFe;

            if (!string.IsNullOrWhiteSpace(observacaoFisco))
                manifesto.ObservacaoFisco = observacaoFisco;

            if (!string.IsNullOrWhiteSpace(observacaoContribuinte))
                manifesto.ObservacaoContribuinte = observacaoContribuinte;

            manifesto.Numero = repMDFe.BuscarUltimoNumero(empresa.Codigo, manifesto.Serie.Codigo, empresa.TipoAmbiente) + 1;

            repMDFe.Inserir(manifesto);

            return manifesto;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFeImportado(Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, string observacaoFisco = "", string observacaoContribuinte = "", Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral)
        {
            Repositorio.ModalTransporte repModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Usuario repUsuarioSerie = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();
            Dominio.Entidades.Usuario usuarioSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;
            serie = repEmpresaSerie.BuscarPorEmpresaNumeroTipo(empresa.Codigo, mdfeAquaviario.MDFe.Serie, Dominio.Enumeradores.TipoSerie.MDFe);

            if (serie == null)
            {
                if (codigoUsuario > 0)
                    usuarioSerie = repUsuarioSerie.BuscarPorCodigo(codigoUsuario);
                if (usuarioSerie != null)
                    serie = (from obj in usuarioSerie.Series where obj.Status.Equals("A") && obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe select obj).FirstOrDefault();
            }

            mdfe.Numero = mdfeAquaviario.MDFe.Numero;
            mdfe.Chave = mdfeAquaviario.MDFe.Chave;
            mdfe.Protocolo = mdfeAquaviario.MDFe.ProtocoloAutorizacao;
            mdfe.MensagemRetornoSefaz = mdfeAquaviario.MDFe.MensagemRetornoSefaz;
            mdfe.Status = mdfeAquaviario.MDFe.StatusMDFe;
            mdfe.DataAutorizacao = mdfeAquaviario.MDFe.DataAutorizacao;
            mdfe.DataEmissao = mdfeAquaviario.MDFe.DataAutorizacao;

            mdfe.Empresa = empresa;
            mdfe.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            mdfe.DataEmissao = dataFuso;

            if (localidadeCarregamento != null)
                mdfe.EstadoCarregamento = localidadeCarregamento.Estado;
            else
                mdfe.EstadoCarregamento = ctes.First().LocalidadeInicioPrestacao.Estado;

            if (localidadeDescarregamento != null)
                mdfe.EstadoDescarregamento = localidadeDescarregamento.Estado;
            else
                mdfe.EstadoDescarregamento = ctes.First().LocalidadeTerminoPrestacao.Estado;

            mdfe.Modal = ctes.First().ModalTransporte;
            mdfe.Modelo = repModelo.BuscarPorModelo("58");

            mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            mdfe.PesoBrutoMercadoria = ctes.Sum(o => o.Peso);

            if (mdfe.PesoBrutoMercadoria <= 0m)
            {
                for (var i = 0; i < ctes.Count; i += 2000)
                    mdfe.PesoBrutoMercadoria += repInformacaoCargaCTe.ObterPesoTotal((from obj in ctes.Skip(i).Take(i + 2000) select obj.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);
            }

            if (mdfe.PesoBrutoMercadoria <= 0m)
            {
                mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.TON;

                for (var i = 0; i < ctes.Count; i += 2000)
                    mdfe.PesoBrutoMercadoria += repInformacaoCargaCTe.ObterPesoTotal((from obj in ctes.Skip(i).Take(i + 2000) select obj.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);
            }

            mdfe.RNTRC = empresa.RegistroANTT;
            mdfe.Serie = serie != null ? serie : empresa.Configuracao != null && empresa.Configuracao.SerieMDFe != null ? empresa.Configuracao.SerieMDFe : repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);
            mdfe.TipoAmbiente = empresa.TipoAmbiente;
            mdfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            mdfe.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporte;
            mdfe.ValorTotalMercadoria = ctes.Sum(o => o.ValorTotalMercadoria);
            mdfe.TentativaEncerramentoAutomatico = 0;
            mdfe.Importado = true;

            mdfe.ProdutoPredominanteDescricao = ctes.FirstOrDefault().ProdutoPredominante;
            mdfe.TipoCargaMDFe = tipoCargaMDFe;

            if (!string.IsNullOrWhiteSpace(observacaoFisco))
                mdfe.ObservacaoFisco = observacaoFisco;

            if (!string.IsNullOrWhiteSpace(observacaoContribuinte))
                mdfe.ObservacaoContribuinte = observacaoContribuinte;

            repMDFe.Inserir(mdfe);

            if (!string.IsNullOrWhiteSpace(mdfeAquaviario.MDFe.XML))
            {
                Dominio.Entidades.XMLMDFe xmlMDFe = new Dominio.Entidades.XMLMDFe()
                {
                    MDFe = mdfe,
                    Tipo = Dominio.Enumeradores.TipoXMLMDFe.Autorizacao,
                    XML = mdfeAquaviario.MDFe.XML
                };
                repXMLMDFe.Inserir(xmlMDFe);
            }

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, string observacaoFisco = "", string observacaoContribuinte = "", Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral, Dominio.ObjetosDeValor.MDFe.DadosMDFe dadosMDFe = null)
        {
            Repositorio.ModalTransporte repModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Usuario repUsuarioSerie = new Repositorio.Usuario(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();
            Dominio.Entidades.Usuario usuarioSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;
            if (dadosMDFe?.Serie > 0)
                serie = repEmpresaSerie.BuscarPorEmpresaNumeroTipo(empresa.Codigo, dadosMDFe.Serie, Dominio.Enumeradores.TipoSerie.MDFe);

            if (serie == null)
            {
                if (codigoUsuario > 0)
                    usuarioSerie = repUsuarioSerie.BuscarPorCodigo(codigoUsuario);
                if (usuarioSerie != null)
                    serie = (from obj in usuarioSerie.Series where obj.Status.Equals("A") && obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe select obj).FirstOrDefault();
            }

            mdfe.Empresa = empresa;
            mdfe.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            mdfe.DataEmissao = dataFuso;

            if (localidadeCarregamento != null)
                mdfe.EstadoCarregamento = localidadeCarregamento.Estado;
            else
                mdfe.EstadoCarregamento = ctes.First().LocalidadeInicioPrestacao.Estado;

            if (localidadeDescarregamento != null)
                mdfe.EstadoDescarregamento = localidadeDescarregamento.Estado;
            else
                mdfe.EstadoDescarregamento = ctes.First().LocalidadeTerminoPrestacao.Estado;

            mdfe.Modal = ctes.First().ModalTransporte;
            mdfe.Modelo = repModelo.BuscarPorModelo("58");

            //mdfe.UnidadeMedidaMercadoria = (ctes.Where(o => o.QuantidadesCarga.FirstOrDefault().UnidadeMedida == "01").Count() > 0) ? Dominio.Enumeradores.UnidadeMedidaMDFe.KG : Dominio.Enumeradores.UnidadeMedidaMDFe.TON;

            mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            mdfe.PesoBrutoMercadoria = ctes.Sum(o => o.Peso);

            if (mdfe.PesoBrutoMercadoria <= 0m)
            {
                for (var i = 0; i < ctes.Count; i += 2000)
                    mdfe.PesoBrutoMercadoria += repInformacaoCargaCTe.ObterPesoTotal((from obj in ctes.Skip(i).Take(i + 2000) select obj.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);
            }

            if (mdfe.PesoBrutoMercadoria <= 0m)
            {
                mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.TON;

                for (var i = 0; i < ctes.Count; i += 2000)
                    mdfe.PesoBrutoMercadoria += repInformacaoCargaCTe.ObterPesoTotal((from obj in ctes.Skip(i).Take(i + 2000) select obj.Codigo).ToArray(), mdfe.UnidadeMedidaMercadoria);
            }

            mdfe.RNTRC = empresa.RegistroANTT;
            mdfe.Serie = serie != null ? serie : empresa.Configuracao != null && empresa.Configuracao.SerieMDFe != null ? empresa.Configuracao.SerieMDFe : repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);

            if (mdfe.Serie == null)
                throw new ServicoException($"Não foi possível identificar a serie do documento MDF-e.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoEspecificado);

            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            mdfe.TipoAmbiente = empresa.TipoAmbiente;
            mdfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            mdfe.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporte;
            mdfe.ValorTotalMercadoria = ctes.Sum(o => o.ValorTotalMercadoria);
            mdfe.TentativaEncerramentoAutomatico = 0;

            mdfe.ProdutoPredominanteDescricao = ctes.FirstOrDefault().ProdutoPredominante;
            mdfe.TipoCargaMDFe = tipoCargaMDFe;

            if (!string.IsNullOrWhiteSpace(observacaoFisco))
                mdfe.ObservacaoFisco = observacaoFisco;

            if (!string.IsNullOrWhiteSpace(observacaoContribuinte))
                mdfe.ObservacaoContribuinte = observacaoContribuinte;

            if (dadosMDFe?.Numero > 0)
                mdfe.Numero = dadosMDFe.Numero;
            else
                mdfe.Numero = repMDFe.BuscarUltimoNumeroLock(empresa.Codigo, mdfe.Serie.Codigo, empresa.TipoAmbiente) + 1;

            if (!string.IsNullOrWhiteSpace(dadosMDFe?.ChaveMDFe ?? string.Empty))
                mdfe.Chave = dadosMDFe.ChaveMDFe;

            repMDFe.Inserir(mdfe);

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctes, Repositorio.UnitOfWork unitOfWork, string observacaoFisco = "", string observacaoContribuinte = "", Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral, Dominio.ObjetosDeValor.MDFe.DadosMDFe dadosMDFe = null)
        {
            Repositorio.ModalTransporte repositorioModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.EmpresaSerie repositorioEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Usuario repUsuarioSerie = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();
            Dominio.Entidades.Usuario usuarioSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;

            if (dadosMDFe?.Serie > 0)
                serie = repositorioEmpresaSerie.BuscarPorEmpresaNumeroTipo(empresa.Codigo, dadosMDFe.Serie, Dominio.Enumeradores.TipoSerie.MDFe);

            if (serie == null)
            {
                if (codigoUsuario > 0)
                    usuarioSerie = repUsuarioSerie.BuscarPorCodigo(codigoUsuario);
                if (usuarioSerie != null)
                    serie = (from obj in usuarioSerie.Series where obj.Status.Equals("A") && obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe select obj).FirstOrDefault();
            }

            mdfe.Empresa = empresa;
            mdfe.Versao = BuscarVersao(empresa);

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            mdfe.DataEmissao = dataFuso;

            if (localidadeCarregamento != null)
                mdfe.EstadoCarregamento = localidadeCarregamento.Estado;
            else
                mdfe.EstadoCarregamento = ctes.First().LocalidadeInicioPrestacao.Estado;

            if (localidadeDescarregamento != null)
                mdfe.EstadoDescarregamento = localidadeDescarregamento.Estado;
            else
                mdfe.EstadoDescarregamento = ctes.First().LocalidadeTerminoPrestacao.Estado;

            mdfe.Modal = repositorioModalTransporte.BuscarPrimeiroRegistro();
            mdfe.Modelo = repositorioModeloDocumentoFiscal.BuscarPorModelo("58");

            mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            mdfe.PesoBrutoMercadoria = ctes.Sum(o => o.Peso);

            if (mdfe.PesoBrutoMercadoria <= 0m)
            {
                mdfe.PesoBrutoMercadoria = 1;
            }

            mdfe.RNTRC = empresa.RegistroANTT;
            mdfe.Serie = BuscarSerie(serie, empresa, unitOfWork);

            if (mdfe.Serie == null)
                throw new ServicoException($"Não foi possível identificar a serie do documento MDF-e.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoEspecificado);

            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            mdfe.TipoAmbiente = empresa.TipoAmbiente;
            mdfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            mdfe.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporte;
            mdfe.ValorTotalMercadoria = ctes.Sum(o => o.ValorTotalMercadoria);
            mdfe.TentativaEncerramentoAutomatico = 0;

            mdfe.ProdutoPredominanteDescricao = ctes.FirstOrDefault()?.ProdutoPredominante ?? string.Empty;

            if (string.IsNullOrWhiteSpace(mdfe.ProdutoPredominanteDescricao))
                mdfe.ProdutoPredominanteDescricao = "DIVERSOS";

            mdfe.TipoCargaMDFe = tipoCargaMDFe;

            if (!string.IsNullOrWhiteSpace(observacaoFisco))
                mdfe.ObservacaoFisco = observacaoFisco;

            if (!string.IsNullOrWhiteSpace(observacaoContribuinte))
                mdfe.ObservacaoContribuinte = observacaoContribuinte;

            if (dadosMDFe?.Numero > 0)
                mdfe.Numero = dadosMDFe.Numero;
            else
                mdfe.Numero = repositorioMDFe.BuscarUltimoNumeroLock(empresa.Codigo, mdfe.Serie.Codigo, empresa.TipoAmbiente) + 1;

            if (!string.IsNullOrWhiteSpace(dadosMDFe?.ChaveMDFe ?? string.Empty))
                mdfe.Chave = dadosMDFe.ChaveMDFe;

            repositorioMDFe.Inserir(mdfe);

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Empresa empresa, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Repositorio.UnitOfWork unidadeDeTrabalho, string observacaoFisco = "", string observacaoContribuinte = "", Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, int codigoUsuario = 0, string produtoPredominante = "DIVERSOS", Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Servicos.NFe svcNFe = new NFe(unidadeDeTrabalho);
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
            Repositorio.ModalTransporte repModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.Usuario repUsuarioSerie = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();

            manifesto.ProdutoPredominanteDescricao = "DIVERSOS";
            manifesto.PesoBrutoMercadoria = nfesGlobalizadas.Sum(o => o.Peso);
            manifesto.ValorTotalMercadoria = nfesGlobalizadas.Sum(o => o.Valor);
            manifesto.DataEmissao = DateTime.Now;
            manifesto.Empresa = empresa;
            manifesto.Versao = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe) ? empresa.EmpresaPai.Configuracao.VersaoMDFe : "3.00";

            if (localidadeCarregamento != null)
                manifesto.EstadoCarregamento = localidadeCarregamento.Estado;
            else
            {
                var localidade = repLocalidade.BuscarPorCodigoIBGE(nfesGlobalizadas.FirstOrDefault().IBGEOrigem);
                manifesto.EstadoCarregamento = localidade?.Estado;
            }

            if (localidadeDescarregamento != null)
                manifesto.EstadoDescarregamento = localidadeDescarregamento.Estado;
            else
            {
                var localidade = repLocalidade.BuscarPorCodigoIBGE(nfesGlobalizadas.FirstOrDefault().IBGEDestino);
                manifesto.EstadoDescarregamento = localidade?.Estado;
            }

            manifesto.Modal = repModal.BuscarPorCodigo(1, false);
            manifesto.Modelo = repModelo.BuscarPorModelo("58");
            manifesto.RNTRC = empresa.RegistroANTT;

            Dominio.Entidades.Usuario usuarioSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;
            if (codigoUsuario > 0)
                usuarioSerie = repUsuarioSerie.BuscarPorCodigo(codigoUsuario);
            if (usuarioSerie != null)
                serie = (from obj in usuarioSerie.Series where obj.Status.Equals("A") && obj.Tipo == Dominio.Enumeradores.TipoSerie.MDFe select obj).FirstOrDefault();

            manifesto.Serie = serie != null ? serie : empresa.Configuracao != null && empresa.Configuracao.SerieMDFe != null ? empresa.Configuracao.SerieMDFe : repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);
            manifesto.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
            manifesto.TipoAmbiente = empresa.TipoAmbiente;
            manifesto.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
            manifesto.TipoEmitente = Dominio.Enumeradores.TipoEmitenteMDFe.TransporteCTeGlobalizado;
            manifesto.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;
            manifesto.TentativaEncerramentoAutomatico = 0;

            manifesto.ProdutoPredominanteDescricao = produtoPredominante;
            manifesto.TipoCargaMDFe = tipoCargaMDFe;

            if (!string.IsNullOrWhiteSpace(observacaoFisco))
                manifesto.ObservacaoFisco = observacaoFisco;

            if (!string.IsNullOrWhiteSpace(observacaoContribuinte))
                manifesto.ObservacaoContribuinte = observacaoContribuinte;

            manifesto.Numero = repMDFe.BuscarUltimoNumero(empresa.Codigo, manifesto.Serie.Codigo, empresa.TipoAmbiente) + 1;

            repMDFe.Inserir(manifesto);

            return manifesto;
        }

        private void GerarMunicipiosDeCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.IntegracaoCTe> integracoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

            Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

            municipioCarregamento.MDFe = mdfe;
            municipioCarregamento.Municipio = integracoes.First().CTe.LocalidadeInicioPrestacao;

            repMunicipioCarregamento.Inserir(municipioCarregamento);

            mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioCarregamento.Municipio.CEP);
            mdfe.LatitudeCarregamentoLotacao = municipioCarregamento.Municipio.Latitude;
            mdfe.LongitudeCarregamentoLotacao = municipioCarregamento.Municipio.Longitude;
        }

        private void GerarMunicipiosDeCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeCarregamento = null)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

            string estadoPrimeiroCTe = string.Empty;

            if (ctes != null && ctes.Count > 0)
                estadoPrimeiroCTe = (from obj in ctes where obj.Codigo == ctes[0].Codigo select obj.LocalidadeInicioPrestacao.Estado.Sigla).FirstOrDefault();

            if (localidadeCarregamento == null)
            {
                IEnumerable<Dominio.Entidades.Localidade> cidades = (from obj in ctes select obj.LocalidadeInicioPrestacao).Distinct();

                foreach (Dominio.Entidades.Localidade cidade in cidades)
                {
                    if (string.IsNullOrWhiteSpace(estadoPrimeiroCTe) || cidade.Estado.Sigla == estadoPrimeiroCTe)
                    {
                        Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                        municipioCarregamento.MDFe = mdfe;
                        municipioCarregamento.Municipio = cidade;

                        repMunicipioCarregamento.Inserir(municipioCarregamento);

                        mdfe.CEPCarregamentoLotacao = cidade.CEP;
                        mdfe.LatitudeCarregamentoLotacao = cidade.Latitude;
                        mdfe.LongitudeCarregamentoLotacao = cidade.Longitude;
                    }
                }
            }
            else
            {
                Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();
                municipioCarregamento.MDFe = mdfe;
                municipioCarregamento.Municipio = localidadeCarregamento;
                repMunicipioCarregamento.Inserir(municipioCarregamento);

                mdfe.CEPCarregamentoLotacao = localidadeCarregamento.CEP;
                mdfe.LatitudeCarregamentoLotacao = localidadeCarregamento.Latitude;
                mdfe.LongitudeCarregamentoLotacao = localidadeCarregamento.Longitude;
            }
        }

        private void GerarMunicipiosDeCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeCarregamento = null)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

            string estadoPrimeiroCTe = string.Empty;

            if (ctes != null && ctes.Count > 0)
                estadoPrimeiroCTe = (from obj in ctes where obj.Codigo == ctes[0].Codigo select obj.LocalidadeInicioPrestacao.Estado.Sigla).FirstOrDefault();

            if (localidadeCarregamento == null)
            {
                IEnumerable<Dominio.Entidades.Localidade> cidades = (from obj in ctes select obj.LocalidadeInicioPrestacao).Distinct();

                foreach (Dominio.Entidades.Localidade cidade in cidades)
                {
                    if (string.IsNullOrWhiteSpace(estadoPrimeiroCTe) || cidade.Estado.Sigla == estadoPrimeiroCTe)
                    {
                        Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                        municipioCarregamento.MDFe = mdfe;
                        municipioCarregamento.Municipio = cidade;

                        repMunicipioCarregamento.Inserir(municipioCarregamento);

                        mdfe.CEPCarregamentoLotacao = cidade.CEP;
                        mdfe.LatitudeCarregamentoLotacao = cidade.Latitude;
                        mdfe.LongitudeCarregamentoLotacao = cidade.Longitude;
                    }
                }
            }
            else
            {
                Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();
                municipioCarregamento.MDFe = mdfe;
                municipioCarregamento.Municipio = localidadeCarregamento;
                repMunicipioCarregamento.Inserir(municipioCarregamento);

                mdfe.CEPCarregamentoLotacao = localidadeCarregamento.CEP;
                mdfe.LatitudeCarregamentoLotacao = localidadeCarregamento.Latitude;
                mdfe.LongitudeCarregamentoLotacao = localidadeCarregamento.Longitude;
            }
        }

        private void GerarMunicipiosDeCarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.MunicipioCarregamento> municipiosCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (municipiosCarregamento != null && municipiosCarregamento.Count() > 0)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.MDFe.MunicipioCarregamento municipio in municipiosCarregamento)
                {
                    Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                    municipioCarregamento.MDFe = mdfe;
                    municipioCarregamento.Municipio = repLocalidade.BuscarPorCodigoIBGE(municipio.CodigoIBGE);

                    repMunicipioCarregamento.Inserir(municipioCarregamento);
                }
            }
        }

        private void GerarMunicipiosDeCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeCarregamento = null)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

            if (localidadeCarregamento == null)
            {
                List<Dominio.Entidades.Localidade> cidades = (from obj in notasFiscais where obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida select obj.Emitente.Localidade).Distinct().ToList();
                cidades.AddRange((from obj in notasFiscais where obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada select obj.Destinatario.Localidade).Distinct().ToList());
                foreach (Dominio.Entidades.Localidade cidade in cidades)
                {
                    Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                    municipioCarregamento.MDFe = mdfe;
                    municipioCarregamento.Municipio = cidade;

                    repMunicipioCarregamento.Inserir(municipioCarregamento);

                    mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(cidade.CEP);
                    mdfe.LatitudeCarregamentoLotacao = cidade.Latitude;
                    mdfe.LongitudeCarregamentoLotacao = cidade.Longitude;
                }
            }
            else
            {
                Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();
                municipioCarregamento.MDFe = mdfe;
                municipioCarregamento.Municipio = localidadeCarregamento;
                repMunicipioCarregamento.Inserir(municipioCarregamento);

                mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(localidadeCarregamento.CEP);
                mdfe.LatitudeCarregamentoLotacao = localidadeCarregamento.Latitude;
                mdfe.LongitudeCarregamentoLotacao = localidadeCarregamento.Longitude;
            }

        }

        private void GerarMunicipiosDeCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<object> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<string> codigosMunicipios = new List<string>();

            for (var i = 0; i < notasFiscais.Count(); i++)
            {
                if (notasFiscais[i] != null)
                {
                    if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                    {
                        MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                        if (!(from obj in codigosMunicipios where obj == notaFiscal.NFe.infNFe.emit.enderEmit.cMun select obj).Any())
                            codigosMunicipios.Add(notaFiscal.NFe.infNFe.emit.enderEmit.cMun);
                    }
                    else if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                    {
                        MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                        if (!(from obj in codigosMunicipios where obj == notaFiscal.NFe.infNFe.emit.enderEmit.cMun select obj).Any())
                            codigosMunicipios.Add(notaFiscal.NFe.infNFe.emit.enderEmit.cMun);
                    }
                }
            }

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

            foreach (string codigoMunicipio in codigosMunicipios)
            {
                Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                municipioCarregamento.MDFe = mdfe;
                municipioCarregamento.Municipio = repLocalidade.BuscarPorCodigoIBGE(int.Parse(codigoMunicipio));

                repMunicipioCarregamento.Inserir(municipioCarregamento);

                mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioCarregamento.Municipio.CEP);
                mdfe.LatitudeCarregamentoLotacao = municipioCarregamento.Municipio.Latitude;
                mdfe.LongitudeCarregamentoLotacao = municipioCarregamento.Municipio.Longitude;
            }
        }

        private void GerarMunicipiosDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.IntegracaoCTe> integracoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);

            List<Dominio.Entidades.Localidade> municipios = (from obj in integracoes where obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla == mdfe.EstadoDescarregamento.Sigla select obj.CTe.LocalidadeTerminoPrestacao).Distinct().ToList();

            foreach (Dominio.Entidades.Localidade municipio in municipios)
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = municipio;

                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioDescarregamento.Municipio.CEP);
                mdfe.LatitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Longitude;

                this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, integracoes, unidadeDeTrabalho);
            }
            repMDFe.Atualizar(mdfe);
        }

        private void GerarMunicipiosDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeDescarregamento = null, bool gerarProdutosPerigosos = true)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);

            if (localidadeDescarregamento == null)
            {
                List<Dominio.Entidades.Localidade> municipios = (from obj in notasFiscais where obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida select obj.Destinatario.Localidade).Distinct().ToList();
                municipios.AddRange((from obj in notasFiscais where obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada select obj.Emitente.Localidade).Distinct().ToList());
                foreach (Dominio.Entidades.Localidade municipio in municipios)
                {
                    Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                    municipioDescarregamento.MDFe = mdfe;
                    municipioDescarregamento.Municipio = municipio;

                    repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                    mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(municipio.CEP);
                    mdfe.LatitudeDescarregamentoLotacao = municipio.Latitude;
                    mdfe.LongitudeDescarregamentoLotacao = municipio.Longitude;

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiltradas = (from obj in notasFiscais
                                                                                               where
                                                                      (obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida &&
                                                                      obj.Destinatario.Localidade.Codigo == municipio.Codigo) ||
                                                                      (obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada &&
                                                                      obj.Emitente.Localidade.Codigo == municipio.Codigo)
                                                                                               select obj).ToList();
                    this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, notasFiltradas, unidadeDeTrabalho);
                }
            }
            else
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = localidadeDescarregamento;
                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(localidadeDescarregamento.CEP);
                mdfe.LatitudeDescarregamentoLotacao = localidadeDescarregamento.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = localidadeDescarregamento.Longitude;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiltradas = (from obj in notasFiscais
                                                                                           where
                                                                                               (obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida &&
                                                                                               obj.Destinatario.Localidade.Codigo == municipioDescarregamento.Codigo) ||
                                                                                               (obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada &&
                                                                                               obj.Emitente.Localidade.Codigo == municipioDescarregamento.Codigo) ||
                                                                                               (obj.Destinatario.Localidade.Estado.Sigla == "EX")
                                                                                           select obj).ToList();
                this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, notasFiltradas, unidadeDeTrabalho);
            }

            repMDFe.Atualizar(mdfe);
        }

        private void GerarMunicipiosDeCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeCarregamento)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

            Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();
            municipioCarregamento.MDFe = mdfe;
            municipioCarregamento.Municipio = localidadeCarregamento;
            repMunicipioCarregamento.Inserir(municipioCarregamento);

            mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(localidadeCarregamento.CEP);
            mdfe.LatitudeCarregamentoLotacao = localidadeCarregamento.Latitude;
            mdfe.LongitudeCarregamentoLotacao = localidadeCarregamento.Longitude;

        }

        private void GerarMunicipiosDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfes, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeDescarregamento, bool gerarProdutosPerigosos = true)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);

            if (localidadeDescarregamento != null)
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = localidadeDescarregamento;
                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(localidadeDescarregamento.CEP);
                mdfe.LatitudeDescarregamentoLotacao = localidadeDescarregamento.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = localidadeDescarregamento.Longitude;

                this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, nfes, unidadeDeTrabalho);

                repMDFe.Atualizar(mdfe);
            }
            else
            {
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> notasFiscais = repNotaFiscal.BuscarPorChave(nfes.Select(c => c.Chave).ToList());

                List<Dominio.Entidades.Localidade> municipios = (from obj in notasFiscais select obj.Cliente.Localidade).Distinct().ToList();
                int quantidadeMunicipios = 0;
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoAnterior = null;
                foreach (Dominio.Entidades.Localidade municipio in municipios)
                {
                    if (quantidadeMunicipios < 100)
                    {
                        Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                        municipioDescarregamento.MDFe = mdfe;
                        municipioDescarregamento.Municipio = municipio;

                        mdfe.CEPDescarregamentoLotacao = municipio.CEP;
                        mdfe.LatitudeDescarregamentoLotacao = municipio.Latitude;
                        mdfe.LongitudeDescarregamentoLotacao = municipio.Longitude;

                        repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                        this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, nfes, unidadeDeTrabalho);
                        municipioDescarregamentoAnterior = municipioDescarregamento;
                    }
                    else if (municipioDescarregamentoAnterior != null)
                        this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamentoAnterior, nfes, unidadeDeTrabalho);

                    quantidadeMunicipios += 1;
                }
            }
        }

        private void GerarMunicipiosDeDescarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeDescarregamento = null, bool gerarProdutosPerigosos = true)
        {
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);

            if (localidadeDescarregamento == null)
            {
                List<Dominio.Entidades.Localidade> municipios = (from obj in ctes select obj.LocalidadeTerminoPrestacao).Distinct().ToList();
                int quantidadeMunicipios = 0;
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoAnterior = null;
                foreach (Dominio.Entidades.Localidade municipio in municipios)
                {
                    if (quantidadeMunicipios < 100)
                    {
                        Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                        municipioDescarregamento.MDFe = mdfe;
                        municipioDescarregamento.Municipio = municipio;

                        mdfe.CEPDescarregamentoLotacao = municipio.CEP;
                        mdfe.LatitudeDescarregamentoLotacao = municipio.Latitude;
                        mdfe.LongitudeDescarregamentoLotacao = municipio.Longitude;

                        repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                        this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamento, ctes, unidadeDeTrabalho, false, gerarProdutosPerigosos);
                        municipioDescarregamentoAnterior = municipioDescarregamento;
                    }
                    else if (municipioDescarregamentoAnterior != null)
                        this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamentoAnterior, ctes, unidadeDeTrabalho, false, gerarProdutosPerigosos, municipio);

                    quantidadeMunicipios += 1;
                }
            }
            else
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = localidadeDescarregamento;
                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = localidadeDescarregamento.CEP;
                mdfe.LatitudeDescarregamentoLotacao = localidadeDescarregamento.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = localidadeDescarregamento.Longitude;

                this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamento, ctes, unidadeDeTrabalho, true, gerarProdutosPerigosos);
            }

        }

        private void GerarMunicipiosDeDescarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeDescarregamento = null, bool gerarProdutosPerigosos = true)
        {
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);

            if (localidadeDescarregamento == null)
            {
                List<Dominio.Entidades.Localidade> municipios = (from obj in ctesTerceiro select obj.LocalidadeTerminoPrestacao).Distinct().ToList();
                int quantidadeMunicipios = 0;
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoAnterior = null;
                foreach (Dominio.Entidades.Localidade municipio in municipios)
                {
                    if (quantidadeMunicipios < 100 && municipio.Estado.Sigla == mdfe.EstadoDescarregamento.Sigla)
                    {

                        Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                        municipioDescarregamento.MDFe = mdfe;
                        municipioDescarregamento.Municipio = municipio;

                        mdfe.CEPDescarregamentoLotacao = municipio.CEP;
                        mdfe.LatitudeDescarregamentoLotacao = municipio.Latitude;
                        mdfe.LongitudeDescarregamentoLotacao = municipio.Longitude;

                        repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                        this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamento, ctesTerceiro, unidadeDeTrabalho, false, gerarProdutosPerigosos);
                        municipioDescarregamentoAnterior = municipioDescarregamento;
                    }
                    else if (municipioDescarregamentoAnterior != null && municipio.Estado.Sigla == mdfe.EstadoDescarregamento.Sigla)
                        this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamentoAnterior, ctesTerceiro, unidadeDeTrabalho, false, gerarProdutosPerigosos, municipio);
                    else if (municipioDescarregamentoAnterior != null && municipio.Estado.Sigla != mdfe.EstadoDescarregamento.Sigla)
                        this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamentoAnterior, (from obj in ctesTerceiro where obj.LocalidadeTerminoPrestacao.Codigo == municipio.Codigo select obj).ToList(), unidadeDeTrabalho, true, gerarProdutosPerigosos, municipio);

                    quantidadeMunicipios += 1;
                }
            }
            else
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = localidadeDescarregamento;
                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = localidadeDescarregamento.CEP;
                mdfe.LatitudeDescarregamentoLotacao = localidadeDescarregamento.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = localidadeDescarregamento.Longitude;

                this.GerarDocumentosDoMunicipioDeDescarregamento(mdfe, municipioDescarregamento, ctesTerceiro, unidadeDeTrabalho, true, gerarProdutosPerigosos);
            }

        }

        private void GerarMunicipiosDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.MunicipioDescarregamento> municipiosDescarregamento, Repositorio.UnitOfWork unidadeDeTrabalho, bool ctesExternos = false)
        {
            if (municipiosDescarregamento != null && municipiosDescarregamento.Count() > 0)
            {
                Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.MDFe.MunicipioDescarregamento municipio in municipiosDescarregamento)
                {
                    Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                    municipioDescarregamento.MDFe = mdfe;
                    municipioDescarregamento.Municipio = repLocalidade.BuscarPorCodigoIBGE(municipio.CodigoIBGE);

                    repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                    this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, municipio.Documentos, unidadeDeTrabalho, ctesExternos);
                }
            }
        }

        private void GerarMunicipiosDeDescarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<object> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dictionary<string, List<string>> municipios = new Dictionary<string, List<string>>();

            for (var i = 0; i < notasFiscais.Count(); i++)
            {
                if (notasFiscais[i] != null)
                {
                    string codigoMunicipio = null, chave = null;

                    if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                    {
                        MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                        codigoMunicipio = notaFiscal.NFe.infNFe.dest.enderDest.cMun;
                        chave = notaFiscal.protNFe.infProt.chNFe;
                    }
                    else if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                    {
                        MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                        codigoMunicipio = notaFiscal.NFe.infNFe.dest.enderDest.cMun;
                        chave = notaFiscal.protNFe.infProt.chNFe;
                    }

                    if (!string.IsNullOrWhiteSpace(codigoMunicipio) && !string.IsNullOrWhiteSpace(chave))
                    {
                        List<string> chaves = new List<string>();

                        if (municipios.ContainsKey(codigoMunicipio))
                            municipios.TryGetValue(codigoMunicipio, out chaves);

                        municipios.Remove(codigoMunicipio);

                        chaves.Add(chave);

                        municipios.Add(codigoMunicipio, chaves);
                    }
                }
            }

            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            foreach (KeyValuePair<string, List<string>> municipio in municipios)
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = repLocalidade.BuscarPorCodigoIBGE(int.Parse(municipio.Key));
                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioDescarregamento.Municipio.CEP);
                mdfe.LatitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Longitude;

                this.GerarDocumentosDoMunicipioDeDescarregamento(municipioDescarregamento, municipio.Value, unidadeDeTrabalho);
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.Entidades.IntegracaoCTe> integracoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from obj in integracoes where obj.CTe.LocalidadeTerminoPrestacao.Codigo == municipioDescarregamento.Municipio.Codigo select obj.CTe).ToList();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe();

                documento.CTe = cte;
                documento.MunicipioDescarregamento = municipioDescarregamento;

                repDocumento.Inserir(documento);
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, bool inserirTodos, bool gerarProdutosPerigosos, Dominio.Entidades.Localidade localidadeDescarregamento = null)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMunicipio = null;

            if (!inserirTodos)
            {
                if (localidadeDescarregamento != null)
                    ctesDoMunicipio = (from obj in ctes where obj.LocalidadeTerminoPrestacao.Codigo == localidadeDescarregamento.Codigo select obj).ToList();
                else
                    ctesDoMunicipio = (from obj in ctes where obj.LocalidadeTerminoPrestacao.Codigo == municipioDescarregamento.Municipio.Codigo select obj).ToList();
            }
            else
                ctesDoMunicipio = ctes;

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctesDoMunicipio)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe();

                documento.CTe = cte;
                documento.MunicipioDescarregamento = municipioDescarregamento;

                repDocumento.Inserir(documento);

                if (gerarProdutosPerigosos && !string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
                    GerarProdutosPerigosos(documento, unidadeDeTrabalho);
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho, bool inserirTodos, bool gerarProdutosPerigosos, Dominio.Entidades.Localidade localidadeDescarregamento = null)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesDoMunicipio = null;

            if (!inserirTodos)
            {
                if (localidadeDescarregamento != null)
                    ctesDoMunicipio = (from obj in ctesTerceiro where obj.LocalidadeTerminoPrestacao.Codigo == localidadeDescarregamento.Codigo select obj).ToList();
                else
                    ctesDoMunicipio = (from obj in ctesTerceiro where obj.LocalidadeTerminoPrestacao.Codigo == municipioDescarregamento.Municipio.Codigo select obj).ToList();
            }
            else
                ctesDoMunicipio = ctesTerceiro;

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesDoMunicipio)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe();

                documento.CTeTerceiro = cteTerceiro;
                documento.MunicipioDescarregamento = municipioDescarregamento;

                repDocumento.Inserir(documento);
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento> documentos, Repositorio.UnitOfWork unidadeDeTrabalho, bool ctesExternos = false)
        {
            if (documentos != null && documentos.Count() > 0)
            {
                if (!string.IsNullOrWhiteSpace(documentos.FirstOrDefault().ChaveNFe))
                {
                    Repositorio.NotaFiscalEletronicaMDFe repDocumento = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);

                    foreach (Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento documento in documentos)
                    {
                        Dominio.Entidades.NotaFiscalEletronicaMDFe doc = new Dominio.Entidades.NotaFiscalEletronicaMDFe();

                        doc.Chave = documento.ChaveNFe;
                        doc.MunicipioDescarregamento = municipioDescarregamento;

                        repDocumento.Inserir(doc);
                    }
                }
                else
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                    if (!ctesExternos)
                    {
                        Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                        foreach (Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento documento in documentos)
                        {
                            Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe doc = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe();

                            doc.CTe = repCTe.BuscarPorChave(documento.ChaveCTe);
                            doc.MunicipioDescarregamento = municipioDescarregamento;

                            repDocumento.Inserir(doc);
                        }
                    }
                    else
                    {
                        Repositorio.CTeMDFe repDocumento = new Repositorio.CTeMDFe(unidadeDeTrabalho);

                        foreach (Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento documento in documentos)
                        {
                            Dominio.Entidades.CTeMDFe doc = new Dominio.Entidades.CTeMDFe();

                            doc.Chave = documento.ChaveCTe;
                            doc.MunicipioDescarregamento = municipioDescarregamento;

                            repDocumento.Inserir(doc);
                        }
                    }
                }
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (notasFiscais != null && notasFiscais.Count() > 0)
            {
                Repositorio.NotaFiscalEletronicaMDFe repNotasMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);

                foreach (var nota in notasFiscais)
                {
                    Dominio.Entidades.NotaFiscalEletronicaMDFe doc = new Dominio.Entidades.NotaFiscalEletronicaMDFe();

                    doc.MunicipioDescarregamento = municipioDescarregamento;
                    doc.Chave = nota.Chave;
                    if (nota.TipoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal.ContingenciaFSDA || nota.TipoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal.ContingenciaFSIA)
                        doc.SegundoCodigoDeBarra = nota.SegundoCodigoBarras;

                    repNotasMDFe.Inserir(doc);
                }
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (nfes != null && nfes.Count() > 0)
            {
                Repositorio.NotaFiscalEletronicaMDFe repNotasMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);

                foreach (var nota in nfes)
                {
                    Dominio.Entidades.NotaFiscalEletronicaMDFe doc = new Dominio.Entidades.NotaFiscalEletronicaMDFe();

                    doc.MunicipioDescarregamento = municipioDescarregamento;
                    doc.Chave = nota.Chave;
                    //if (nota.TipoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal.ContingenciaFSDA || nota.TipoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal.ContingenciaFSIA)
                    //    doc.SegundoCodigoDeBarra = nota.SegundoCodigoBarras;

                    repNotasMDFe.Inserir(doc);
                }
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<string> chaves, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (chaves != null && chaves.Count() > 0)
            {
                Repositorio.NotaFiscalEletronicaMDFe repNotasMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);

                foreach (string chave in chaves)
                {
                    Dominio.Entidades.NotaFiscalEletronicaMDFe doc = new Dominio.Entidades.NotaFiscalEletronicaMDFe();

                    doc.MunicipioDescarregamento = municipioDescarregamento;
                    doc.Chave = chave;

                    repNotasMDFe.Inserir(doc);
                }
            }
        }

        private void GerarDocumentosDoMunicipioDeDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (documentos != null && documentos.Count() > 0)
            {
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repCTe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento in documentos)
                {
                    if (repDocumento.BuscarPorCTe(mdfe.Codigo, documento.CTe.Codigo) == null)
                    {
                        Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe doc = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe();

                        doc.CTe = documento.CTe;
                        doc.MunicipioDescarregamento = municipioDescarregamento;

                        repDocumento.Inserir(doc);
                    }
                }
            }
        }

        private void GerarVeiculoNFes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculoTracao = null, List<int> codigosCarga = null)
        {
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = ObterPrimeiroVeiculo(notasFiscais, codigosCarga, unitOfWork);

            if (veiculo != null)
            {
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculo.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculo.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculo.Placa;
                veiculoMDFe.Tara = veiculo.Tara;
                veiculoMDFe.TipoCarroceria = veiculo.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculo.TipoRodado;
                veiculoMDFe.UF = veiculo.Estado;
                veiculoMDFe.RENAVAM = veiculo.Renavam;

                if (veiculo.Tipo == "T" && veiculo.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculo.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculo.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculo.Proprietario.Nome.Length > 60 ? veiculo.Proprietario.Nome.Substring(0, 60) : veiculo.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculo.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculo.Proprietario.IE_RG;
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculo.VeiculosVinculados.Any())
                    this.GerarReboques(mdfe, veiculo.VeiculosVinculados.ToList(), unitOfWork);

                this.GerarMotoristas(mdfe, veiculo, unitOfWork);
            }
            else if (veiculoTracao != null)
            {
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculoTracao.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculoTracao.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculoTracao.Placa;
                veiculoMDFe.Tara = veiculoTracao.Tara;
                veiculoMDFe.TipoCarroceria = veiculoTracao.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculoTracao.TipoRodado;
                veiculoMDFe.UF = veiculoTracao.Estado;
                veiculoMDFe.RENAVAM = veiculoTracao.Renavam;

                if (veiculoTracao.Tipo == "T" && veiculoTracao.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculoTracao.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculoTracao.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculoTracao.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculoTracao.Proprietario.Nome.Length > 60 ? veiculoTracao.Proprietario.Nome.Substring(0, 60) : veiculoTracao.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculoTracao.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculoTracao.Proprietario.IE_RG;
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculoTracao.VeiculosVinculados.Any())
                    this.GerarReboques(mdfe, veiculoTracao.VeiculosVinculados.ToList(), unitOfWork);

                this.GerarMotoristas(mdfe, veiculoTracao, unitOfWork);
            }
        }


        private Dominio.Entidades.Veiculo ObterPrimeiroVeiculo(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, List<int> codigosCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            if (notasFiscais.IsNullOrEmpty())
                return null;

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = notasFiscais.FirstOrDefault();

            if (!codigosCarga.IsNullOrEmpty())
                return repositorioPedidoXMLNotaFiscal.BuscarPrimeiroVeiculoPorNotaECargas(notaFiscal.Codigo, codigosCarga);

            return repositorioPedidoXMLNotaFiscal.BuscarPrimeiroVeiculoPorNotaECargas(notaFiscal.Codigo);
        }

        private void GerarVeiculoNFes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculoTracao)
        {
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);
            if (veiculoTracao != null)
            {
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculoTracao.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculoTracao.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculoTracao.Placa;
                veiculoMDFe.Tara = veiculoTracao.Tara;
                veiculoMDFe.TipoCarroceria = veiculoTracao.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculoTracao.TipoRodado;
                veiculoMDFe.UF = veiculoTracao.Estado;
                veiculoMDFe.RENAVAM = veiculoTracao.Renavam;

                if (veiculoTracao.Tipo == "T" && veiculoTracao.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculoTracao.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculoTracao.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculoTracao.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculoTracao.Proprietario.Nome.Length > 60 ? veiculoTracao.Proprietario.Nome.Substring(0, 60) : veiculoTracao.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculoTracao.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculoTracao.Proprietario.IE_RG;
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculoTracao.VeiculosVinculados.Any())
                    this.GerarReboques(mdfe, veiculoTracao.VeiculosVinculados.ToList(), unitOfWork);

                this.GerarMotoristas(mdfe, veiculoTracao, unitOfWork);
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.IntegracaoCTe> integracoes, Repositorio.UnitOfWork unidadeDeTrabalho, ref Dominio.Entidades.Veiculo veiculoUtilizado)
        {
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);

            List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculoCTe.BuscarPorCTe(mdfe.Empresa.Codigo, integracoes.First().CTe.Codigo);

            var veiculo = (from obj in veiculos where obj.Veiculo.TipoVeiculo == "0" select obj.Veiculo).FirstOrDefault();

            if (veiculo != null)
            {
                veiculoUtilizado = veiculo;
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculo.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculo.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculo.Placa;
                veiculoMDFe.Tara = veiculo.Tara;
                veiculoMDFe.TipoCarroceria = veiculo.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculo.TipoRodado;
                veiculoMDFe.UF = veiculo.Estado;
                veiculoMDFe.RENAVAM = veiculo.Renavam;

                if (veiculo.Tipo == "T" && veiculo.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculo.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculo.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculo.Proprietario.Nome.Length > 60 ? veiculo.Proprietario.Nome.Substring(0, 60) : veiculo.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculo.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculo.Proprietario.IE_RG;
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculo.VeiculosVinculados.Count() > 0)
                    this.GerarReboques(mdfe, veiculo.VeiculosVinculados.ToList(), unidadeDeTrabalho);

                this.GerarMotoristas(mdfe, veiculo, unidadeDeTrabalho);
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboquesEmissao = null, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas = null)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            Dominio.Entidades.VeiculoCTE veiculoCTe = null;
            if (veiculo != null || veiculoCTe != null)
            {
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculo != null ? veiculo.CapacidadeKG : veiculoCTe.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculo != null ? veiculo.CapacidadeM3 : veiculoCTe.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculo != null ? veiculo.Placa : veiculoCTe.Placa;
                veiculoMDFe.Tara = veiculo != null ? veiculo.Tara : veiculoCTe.Tara;
                veiculoMDFe.TipoCarroceria = veiculo != null ? veiculo.TipoCarroceria : veiculoCTe.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculo != null ? veiculo.TipoRodado == "00" ? "01" : veiculo.TipoRodado : veiculoCTe.TipoRodado == "00" ? "01" : veiculoCTe.TipoRodado;
                veiculoMDFe.UF = veiculo != null ? veiculo.Estado : veiculoCTe.Estado;
                veiculoMDFe.RENAVAM = veiculo != null ? veiculo.Renavam : veiculoCTe.RENAVAM;

                if (veiculo != null && veiculo.Tipo == "T" && veiculo.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculo.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculo.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculo.Proprietario.Nome.Length > 60 ? veiculo.Proprietario.Nome.Substring(0, 60) : veiculo.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculo.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculo.Proprietario.IE_RG != null && veiculo.Proprietario.IE_RG != "" ? veiculo.Proprietario.IE_RG : "ISENTO";
                }
                else if (veiculoCTe != null && veiculoCTe.TipoPropriedade == "T" && veiculoCTe.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculoCTe.Proprietario.Estado;
                    veiculoMDFe.TipoProprietario = veiculoCTe.Proprietario.Tipo.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculoCTe.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculoCTe.Proprietario.Nome.Length > 60 ? veiculoCTe.Proprietario.Nome.Substring(0, 60) : veiculoCTe.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculoCTe.Proprietario.CPF_CNPJ;
                    veiculoMDFe.IEProprietario = veiculoCTe.Proprietario.IE != null && veiculoCTe.Proprietario.IE != "" ? veiculoCTe.Proprietario.IE : "ISENTO";
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculo != null && veiculo.Tipo == "T" && veiculo.Proprietario != null && veiculo.CIOT != null && veiculo.CIOT != "")
                {
                    mdfe.CIOT = Utilidades.String.Left(veiculo.CIOT, 12);
                    repMDFe.Atualizar(mdfe);
                }
                else if (veiculoCTe != null && veiculoCTe.TipoPropriedade == "T" && veiculoCTe.CTE != null && veiculoCTe.CTE.CIOT != null && veiculoCTe.CTE.CIOT != "")
                {
                    mdfe.CIOT = Utilidades.String.Left(veiculoCTe.CTE.CIOT, 12);
                    repMDFe.Atualizar(mdfe);
                }

                List<Dominio.Entidades.Veiculo> listaReboque = reboquesEmissao;

                if (listaReboque == null)
                    listaReboque = veiculo.VeiculosVinculados.ToList();

                if (listaReboque.Count() > 0)
                {
                    this.GerarReboques(mdfe, listaReboque.ToList(), unidadeDeTrabalho);
                }
                else if (reboquesEmissao == null)
                {
                    if (veiculo != null && veiculo != null && veiculo.VeiculosVinculados.Count() > 0)
                        this.GerarReboques(mdfe, veiculo.VeiculosVinculados.ToList(), unidadeDeTrabalho);
                    else if (veiculoCTe != null && veiculoCTe.Veiculo != null && veiculoCTe.Veiculo.VeiculosVinculados.Count() > 0)
                        this.GerarReboques(mdfe, veiculoCTe.Veiculo.VeiculosVinculados.ToList(), unidadeDeTrabalho);
                }

                this.GerarMotoristasPorObjeto(mdfe, motoristas, unidadeDeTrabalho);
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculoEmissao = null, List<Dominio.Entidades.Veiculo> reboquesEmissao = null, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas = null)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculoCTe.BuscarPorCTe(mdfe.Empresa.Codigo, ctes.First().Codigo);
            Dominio.Entidades.VeiculoCTE veiculoCTe = null;
            Dominio.Entidades.Veiculo veiculo = null;

            if (veiculoEmissao == null)
            {
                bool configMDFeUtilizaDadosVeiculoCadastro = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().MDFeUtilizaDadosVeiculoCadastro.Value;

                bool configMDFeUtilizaVeiculoReboqueComoTracao = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().MDFeUtilizaVeiculoReboqueComoTracao.Value;

                if (configMDFeUtilizaDadosVeiculoCadastro)
                {
                    veiculo = (from obj in veiculos where obj.Veiculo.TipoVeiculo == "0" select obj.Veiculo).FirstOrDefault();
                    if (veiculo == null && configMDFeUtilizaVeiculoReboqueComoTracao)
                        veiculo = (from obj in veiculos where obj.Veiculo.TipoVeiculo == "1" select obj.Veiculo).FirstOrDefault();
                }

                veiculoCTe = (from obj in veiculos where obj.TipoVeiculo == "0" select obj).FirstOrDefault();
                if (veiculoCTe == null && configMDFeUtilizaVeiculoReboqueComoTracao)
                    veiculoCTe = (from obj in veiculos where obj.TipoVeiculo == "1" select obj).FirstOrDefault();
            }
            else
            {
                veiculo = veiculoEmissao;
            }

            if (veiculo != null || veiculoCTe != null)
            {
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculo != null ? veiculo.CapacidadeKG : veiculoCTe.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculo != null ? veiculo.CapacidadeM3 : veiculoCTe.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculo != null ? veiculo.Placa : veiculoCTe.Placa;
                veiculoMDFe.Tara = veiculo != null ? veiculo.Tara : veiculoCTe.Tara;
                veiculoMDFe.TipoCarroceria = veiculo != null ? veiculo.TipoCarroceria : veiculoCTe.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculo != null ? veiculo.TipoRodado == "00" ? "01" : veiculo.TipoRodado : veiculoCTe.TipoRodado == "00" ? "01" : veiculoCTe.TipoRodado;
                veiculoMDFe.UF = veiculo != null ? veiculo.Estado : veiculoCTe.Estado;
                veiculoMDFe.RENAVAM = veiculo != null ? veiculo.Renavam : veiculoCTe.RENAVAM;

                if (veiculo != null && veiculo.Tipo == "T" && veiculo.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculo.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculo.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculo.Proprietario.Nome.Length > 60 ? veiculo.Proprietario.Nome.Substring(0, 60) : veiculo.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculo.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculo.Proprietario.IE_RG != null && veiculo.Proprietario.IE_RG != "" ? veiculo.Proprietario.IE_RG : "ISENTO";
                }
                else if (veiculoCTe != null && veiculoCTe.TipoPropriedade == "T" && veiculoCTe.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculoCTe.Proprietario.Estado;
                    veiculoMDFe.TipoProprietario = veiculoCTe.Proprietario.Tipo.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculoCTe.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculoCTe.Proprietario.Nome.Length > 60 ? veiculoCTe.Proprietario.Nome.Substring(0, 60) : veiculoCTe.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculoCTe.Proprietario.CPF_CNPJ;
                    veiculoMDFe.IEProprietario = veiculoCTe.Proprietario.IE != null && veiculoCTe.Proprietario.IE != "" ? veiculoCTe.Proprietario.IE : "ISENTO";
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculo != null && veiculo.Tipo == "T" && veiculo.Proprietario != null && veiculo.CIOT != null && veiculo.CIOT != "")
                {
                    mdfe.CIOT = Utilidades.String.Left(veiculo.CIOT, 12);
                    repMDFe.Atualizar(mdfe);
                }
                else if (veiculoCTe != null && veiculoCTe.TipoPropriedade == "T" && veiculoCTe.CTE != null && veiculoCTe.CTE.CIOT != null && veiculoCTe.CTE.CIOT != "")
                {
                    mdfe.CIOT = Utilidades.String.Left(veiculoCTe.CTE.CIOT, 12);
                    repMDFe.Atualizar(mdfe);
                }

                List<Dominio.Entidades.Veiculo> listaReboque = reboquesEmissao;

                if (listaReboque == null)
                    listaReboque = (from obj in veiculos where (veiculo != null ? obj.Veiculo.Placa != veiculo.Placa : obj.Veiculo.Placa != veiculoCTe.Placa) select obj.Veiculo).ToList();

                if (listaReboque.Count() > 0)
                {
                    this.GerarReboques(mdfe, listaReboque.ToList(), unidadeDeTrabalho);
                }
                else if (reboquesEmissao == null)
                {
                    if (veiculo != null && veiculo != null && veiculo.VeiculosVinculados.Count() > 0)
                        this.GerarReboques(mdfe, veiculo.VeiculosVinculados.ToList(), unidadeDeTrabalho);
                    else if (veiculoCTe != null && veiculoCTe.Veiculo != null && veiculoCTe.Veiculo.VeiculosVinculados.Count() > 0)
                        this.GerarReboques(mdfe, veiculoCTe.Veiculo.VeiculosVinculados.ToList(), unidadeDeTrabalho);
                }

                if (motoristas == null)
                    this.GerarMotoristas(mdfe, veiculo != null ? veiculo : veiculoCTe != null ? veiculoCTe.Veiculo : null, unidadeDeTrabalho, ctes.First());
                else
                    this.GerarMotoristasPorObjeto(mdfe, motoristas, unidadeDeTrabalho);
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculoEmissao = null, List<Dominio.Entidades.Veiculo> reboquesEmissao = null, List<Dominio.Entidades.Usuario> motoristas = null)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            if (veiculoEmissao != null)
            {
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculoEmissao.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculoEmissao.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculoEmissao.Placa;
                veiculoMDFe.Tara = veiculoEmissao.Tara;
                veiculoMDFe.TipoCarroceria = veiculoEmissao.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculoEmissao.TipoRodado == "00" ? "01" : veiculoEmissao.TipoRodado;
                veiculoMDFe.UF = veiculoEmissao.Estado;
                veiculoMDFe.RENAVAM = veiculoEmissao.Renavam;

                if (veiculoEmissao.Tipo == "T" && veiculoEmissao.Proprietario != null)
                {
                    veiculoMDFe.UFProprietario = veiculoEmissao.Proprietario.Localidade.Estado;
                    veiculoMDFe.TipoProprietario = veiculoEmissao.TipoProprietario.ToString("d");
                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculoEmissao.RNTRC);
                    veiculoMDFe.NomeProprietario = veiculoEmissao.Proprietario.Nome.Length > 60 ? veiculoEmissao.Proprietario.Nome.Substring(0, 60) : veiculoEmissao.Proprietario.Nome;
                    veiculoMDFe.CPFCNPJProprietario = veiculoEmissao.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculoEmissao.Proprietario.IE_RG != null && veiculoEmissao.Proprietario.IE_RG != "" ? veiculoEmissao.Proprietario.IE_RG : "ISENTO";
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);

                if (veiculoEmissao != null && veiculoEmissao.Tipo == "T" && veiculoEmissao.Proprietario != null && veiculoEmissao.CIOT != null && veiculoEmissao.CIOT != "")
                {
                    mdfe.CIOT = Utilidades.String.Left(veiculoEmissao.CIOT, 12);
                    repMDFe.Atualizar(mdfe);
                }

                List<Dominio.Entidades.Veiculo> listaReboque = reboquesEmissao;

                if (listaReboque.Count > 0)
                {
                    this.GerarReboques(mdfe, listaReboque.ToList(), unidadeDeTrabalho);
                }

                this.GerarMotoristasPorObjeto(mdfe, motoristas, unidadeDeTrabalho);
            }
        }

        private void GerarVeiculoPorObjeto(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> veiculos, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> reboques, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            bool configCTeUtilizaProprietarioCadastro = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().CTeUtilizaProprietarioCadastro.Value;

            if (veiculos != null && veiculos.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.VeiculoMDFeIntegracao veiculo in veiculos)
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                    Repositorio.Estado repEstadoMDFe = new Repositorio.Estado(unidadeDeTrabalho);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                    Dominio.Entidades.Veiculo veiculoCadastro = repVeiculo.BuscarPorPlaca(mdfe.Empresa.Codigo, veiculo.Placa);
                    Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                    veiculoMDFe.MDFe = mdfe;
                    veiculoMDFe.Placa = veiculo.Placa;
                    veiculoMDFe.RENAVAM = !string.IsNullOrWhiteSpace(veiculo.RENAVAM) && veiculo.RENAVAM != "0" ? veiculo.RENAVAM : veiculoCadastro != null ? veiculoCadastro.Renavam : "123456789";
                    if (veiculo.CapacidadeKG > 0)
                        veiculoMDFe.CapacidadeKG = veiculo.CapacidadeKG;
                    else
                        veiculoMDFe.CapacidadeKG = veiculoCadastro != null && veiculoCadastro.CapacidadeKG > 0 ? veiculoCadastro.CapacidadeKG : 10000;
                    if (veiculo.CapacidadeM3 > 0)
                        veiculoMDFe.CapacidadeM3 = veiculo.CapacidadeM3;
                    else
                        veiculoMDFe.CapacidadeM3 = 10;
                    if (veiculo.TaraKg > 0)
                        veiculoMDFe.Tara = veiculo.TaraKg;
                    else
                        veiculoMDFe.Tara = veiculoCadastro != null && veiculoCadastro.Tara > 0 ? veiculoCadastro.Tara : 1000;
                    if (veiculo.TipoCarroceria != null && veiculo.TipoCarroceria != "")
                        veiculoMDFe.TipoCarroceria = veiculo.TipoCarroceria;
                    else
                        veiculoMDFe.TipoCarroceria = veiculoCadastro != null && veiculoCadastro.TipoCarroceria != null && veiculoCadastro.TipoCarroceria != "" ? veiculoCadastro.TipoCarroceria : "02";
                    if (veiculo.TipoRodado != null && veiculo.TipoRodado != "")
                        veiculoMDFe.TipoRodado = veiculo.TipoRodado;
                    else
                        veiculoMDFe.TipoRodado = veiculoCadastro != null && veiculoCadastro.TipoRodado != null && veiculoCadastro.TipoRodado != "" ? veiculoCadastro.TipoRodado : "01";
                    if (veiculo.UF != null && veiculo.UF != "")
                        veiculoMDFe.UF = repEstadoMDFe.BuscarPorSigla(veiculo.UF);
                    else
                        veiculoMDFe.UF = veiculoCadastro != null && veiculoCadastro.Estado != null ? veiculoCadastro.Estado : mdfe.Empresa.Localidade.Estado;

                    if (configCTeUtilizaProprietarioCadastro && veiculoCadastro != null && veiculoCadastro.Tipo == "T" && veiculoCadastro.Proprietario != null)
                    {
                        veiculoMDFe.CPFCNPJProprietario = veiculoCadastro.Proprietario.CPF_CNPJ_SemFormato;
                        veiculoMDFe.NomeProprietario = veiculoCadastro.Proprietario.Nome.Length > 60 ? veiculoCadastro.Proprietario.Nome.Substring(0, 60) : veiculoCadastro.Proprietario.Nome;
                        veiculoMDFe.IEProprietario = veiculoCadastro.Proprietario.IE_RG != null && veiculoCadastro.Proprietario.IE_RG != "" ? veiculoCadastro.Proprietario.IE_RG : "ISENTO";
                        veiculoMDFe.UFProprietario = veiculoCadastro.Proprietario.Localidade.Estado;
                        veiculoMDFe.RNTRC = veiculoCadastro.RNTRC > 0 ? string.Format("{0:00000000}", veiculoCadastro.RNTRC) : veiculoCadastro.Empresa.RegistroANTT;
                        veiculoMDFe.TipoProprietario = veiculoCadastro.TipoProprietario.ToString("d");

                        if (veiculoCadastro.CIOT != null && veiculoCadastro.CIOT != "")
                        {
                            mdfe.CIOT = veiculoCadastro.CIOT;
                            repMDFe.Atualizar(mdfe);
                        }
                    }
                    else if (veiculo.ProprietarioTerceiro != null)
                    {
                        Dominio.Entidades.Estado estadoProprietario = null;
                        if (veiculo.ProprietarioTerceiro.CodigoIBGECidade > 0)
                            estadoProprietario = repLocalidade.BuscarPorCodigoIBGE(veiculo.ProprietarioTerceiro.CodigoIBGECidade)?.Estado ?? null;

                        veiculoMDFe.CPFCNPJProprietario = Utilidades.String.OnlyNumbers(veiculo.ProprietarioTerceiro.CPFCNPJ);
                        veiculoMDFe.NomeProprietario = veiculo.ProprietarioTerceiro.RazaoSocial.Left(60);
                        veiculoMDFe.IEProprietario = !string.IsNullOrWhiteSpace(veiculo.ProprietarioTerceiro.RGIE) ? veiculo.ProprietarioTerceiro.RGIE : "ISENTO";
                        veiculoMDFe.UFProprietario = estadoProprietario;
                        veiculoMDFe.RNTRC = veiculo.RNTRC;
                        veiculoMDFe.TipoProprietario = "0";

                        if (veiculoCadastro != null && veiculoCadastro.CIOT != null && veiculoCadastro.CIOT != "")
                        {
                            mdfe.CIOT = veiculoCadastro.CIOT;
                            repMDFe.Atualizar(mdfe);
                        }
                    }

                    repVeiculoMDFe.Inserir(veiculoMDFe);

                    if (reboques != null && reboques.Count > 0)
                        this.GerarReboquesPorObjeto(mdfe, reboques, unidadeDeTrabalho);
                    else
                        if (veiculoCadastro != null && veiculoCadastro.VeiculosVinculados.Count() > 0)
                        this.GerarReboques(mdfe, veiculoCadastro.VeiculosVinculados.ToList(), unidadeDeTrabalho);

                    this.GerarMotoristasPorObjeto(mdfe, motoristas, unidadeDeTrabalho);
                }
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.CTe.Veiculo veiculo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (veiculo != null)
            {
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculo.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculo.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculo.Placa;
                veiculoMDFe.Tara = veiculo.Tara;
                veiculoMDFe.TipoCarroceria = veiculo.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculo.TipoRodado;
                veiculoMDFe.UF = repEstado.BuscarPorSigla(veiculo.UF);
                veiculoMDFe.RENAVAM = veiculo.Renavam;

                if (veiculo.TipoPropriedade == "T" && veiculo.Proprietario != null)
                {
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(veiculo.Proprietario.CodigoIBGECidade);

                    veiculoMDFe.UFProprietario = localidade.Estado;
                    veiculoMDFe.CPFCNPJProprietario = veiculo.TipoPropriedade == "T" && veiculo.Proprietario != null ? veiculo.Proprietario.CPFCNPJ : string.Empty;
                    veiculoMDFe.IEProprietario = veiculo.TipoPropriedade == "T" && veiculo.Proprietario != null ? veiculo.Proprietario.RGIE : string.Empty;
                    veiculoMDFe.NomeProprietario = veiculo.TipoPropriedade == "T" && veiculo.Proprietario != null ? Utilidades.String.Left(veiculo.Proprietario.RazaoSocial, 60) : string.Empty;
                    veiculoMDFe.RNTRC = veiculo.TipoPropriedade == "T" && veiculo.Proprietario != null ? string.Format("{0:00000000}", veiculo.RNTRCProprietario) : string.Empty;
                    veiculoMDFe.TipoProprietario = veiculo.TipoPropriedade == "T" && veiculo.Proprietario != null ? veiculo.TipoProprietario.ToString("d") : string.Empty;
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<object> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho, ref Dominio.Entidades.Veiculo veiculo)
        {
            if (notasFiscais != null && notasFiscais.Count() > 0)
            {
                string placa = null;

                for (var i = 0; i < notasFiscais.Count(); i++)
                {
                    if (notasFiscais[i] != null)
                    {
                        if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                        {
                            MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                            if (notaFiscal.NFe.infNFe.transp.Items != null && notaFiscal.NFe.infNFe.transp.Items.Count() > 0)
                            {
                                foreach (object item in notaFiscal.NFe.infNFe.transp.Items)
                                {
                                    if (item.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TVeiculo))
                                    {
                                        placa = ((MultiSoftware.NFe.v310.NotaFiscal.TVeiculo)item).placa;
                                        break;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(placa))
                                    break;
                            }
                        }
                        else if (notasFiscais[i].GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                        {
                            MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc notaFiscal = (MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)notasFiscais[i];

                            if (notaFiscal.NFe.infNFe.transp.Items != null && notaFiscal.NFe.infNFe.transp.Items.Count() > 0)
                            {
                                foreach (object item in notaFiscal.NFe.infNFe.transp.Items)
                                {
                                    if (item.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TVeiculo))
                                    {
                                        placa = ((MultiSoftware.NFe.NotaFiscal.TVeiculo)item).placa;
                                        break;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(placa))
                                    break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(placa))
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                    Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorPlaca(mdfe.Empresa.Codigo, placa);

                    if (veic != null)
                    {
                        veiculo = veic;
                        Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                        veiculoMDFe.CapacidadeKG = veic.CapacidadeKG;
                        veiculoMDFe.CapacidadeM3 = veic.CapacidadeM3;
                        veiculoMDFe.MDFe = mdfe;
                        veiculoMDFe.Placa = veic.Placa;
                        veiculoMDFe.Tara = veic.Tara;
                        veiculoMDFe.TipoCarroceria = veic.TipoCarroceria;
                        veiculoMDFe.TipoRodado = veic.TipoRodado;
                        veiculoMDFe.UF = veic.Estado;
                        veiculoMDFe.RENAVAM = veic.Renavam;

                        if (veic.Tipo == "T" && veic.Proprietario != null)
                        {
                            veiculoMDFe.UFProprietario = veic.Proprietario.Localidade.Estado;
                            veiculoMDFe.CPFCNPJProprietario = veic.Proprietario.CPF_CNPJ_SemFormato;
                            veiculoMDFe.IEProprietario = veic.Proprietario.IE_RG;
                            veiculoMDFe.NomeProprietario = veic.Proprietario.Nome.Length > 60 ? veic.Proprietario.Nome.Substring(0, 60) : veic.Proprietario.Nome;
                            veiculoMDFe.RNTRC = string.Format("{0:00000000}", veic.RNTRC);
                            veiculoMDFe.TipoProprietario = veic.TipoProprietario.ToString("d");
                        }

                        repVeiculoMDFe.Inserir(veiculoMDFe);

                        this.GerarReboques(mdfe, veic.VeiculosVinculados.ToList(), unidadeDeTrabalho);

                        this.GerarMotoristas(mdfe, veic, unidadeDeTrabalho);
                    }
                }
            }
        }

        private void GerarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Veiculo veiculoTracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (veiculoTracao != null)
            {
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();

                veiculoMDFe.CapacidadeKG = veiculoTracao.CapacidadeKG;
                veiculoMDFe.CapacidadeM3 = veiculoTracao.CapacidadeM3;
                veiculoMDFe.MDFe = mdfe;
                veiculoMDFe.Placa = veiculoTracao.Placa;
                veiculoMDFe.Tara = veiculoTracao.Tara;
                veiculoMDFe.TipoCarroceria = veiculoTracao.TipoCarroceria;
                veiculoMDFe.TipoRodado = veiculoTracao.TipoRodado;
                veiculoMDFe.UF = veiculoTracao.Estado;
                veiculoMDFe.RENAVAM = veiculoTracao.Renavam;

                if (veiculoTracao.Tipo == "T" && veiculoTracao.Proprietario != null)
                {
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(veiculoTracao.Proprietario.Localidade.CodigoIBGE);

                    veiculoMDFe.UFProprietario = localidade.Estado;
                    veiculoMDFe.CPFCNPJProprietario = veiculoTracao.Proprietario.CPF_CNPJ_SemFormato;
                    veiculoMDFe.IEProprietario = veiculoTracao.Proprietario.IE_RG;

                    if (veiculoTracao.Proprietario.Nome.Length > 60)
                        veiculoMDFe.NomeProprietario = veiculoTracao.Proprietario.Nome.Substring(0, 60);
                    else
                        veiculoMDFe.NomeProprietario = veiculoTracao.Proprietario.Nome;

                    veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculoTracao.RNTRC);
                    veiculoMDFe.TipoProprietario = veiculoTracao.TipoProprietario.ToString("d");
                }

                repVeiculoMDFe.Inserir(veiculoMDFe);
            }
        }

        private void GerarReboques(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Veiculo> reboques, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (reboques != null && reboques.Count() > 0)
            {
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);

                foreach (Dominio.Entidades.Veiculo reboque in reboques)
                {
                    Dominio.Entidades.ReboqueMDFe reboqueMDFe = new Dominio.Entidades.ReboqueMDFe();

                    reboqueMDFe.CapacidadeKG = reboque.CapacidadeKG;
                    reboqueMDFe.CapacidadeM3 = reboque.CapacidadeM3;
                    reboqueMDFe.MDFe = mdfe;
                    reboqueMDFe.Placa = reboque.Placa;
                    reboqueMDFe.Tara = reboque.Tara;
                    reboqueMDFe.TipoCarroceria = reboque.TipoCarroceria;
                    reboqueMDFe.TipoProprietario = reboque.Tipo;
                    reboqueMDFe.UF = reboque.Estado;
                    reboqueMDFe.RENAVAM = reboque.Renavam;

                    if (reboque.Tipo == "T" && reboque.Proprietario != null)
                    {
                        reboqueMDFe.UFProprietario = reboque.Proprietario.Localidade.Estado;
                        reboqueMDFe.CPFCNPJProprietario = reboque.Proprietario.CPF_CNPJ_SemFormato;
                        reboqueMDFe.IEProprietario = reboque.Proprietario.IE_RG;
                        reboqueMDFe.NomeProprietario = reboque.Proprietario.Nome.Length > 60 ? reboque.Proprietario.Nome.Substring(0, 60) : reboque.Proprietario.Nome;
                        reboqueMDFe.RNTRC = string.Format("{0:00000000}", reboque.RNTRC);
                    }

                    repReboqueMDFe.Inserir(reboqueMDFe);
                }
            }
        }

        private void GerarReboquesPorObjeto(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> reboques, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            bool configCTeUtilizaProprietarioCadastro = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().CTeUtilizaProprietarioCadastro.Value;

            if (reboques != null && reboques.Count() > 0)
            {
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.VeiculoMDFeIntegracao reboque in reboques)
                {
                    Dominio.Entidades.ReboqueMDFe reboqueMDFe = new Dominio.Entidades.ReboqueMDFe();

                    reboqueMDFe.MDFe = mdfe;
                    reboqueMDFe.Placa = reboque.Placa;
                    reboqueMDFe.RENAVAM = reboque.RENAVAM;
                    if (reboque.CapacidadeKG > 0)
                        reboqueMDFe.CapacidadeKG = reboque.CapacidadeKG;
                    else
                        reboqueMDFe.CapacidadeKG = 10000;
                    if (reboque.CapacidadeM3 > 0)
                        reboqueMDFe.CapacidadeM3 = reboque.CapacidadeM3;
                    else
                        reboqueMDFe.CapacidadeM3 = 10;
                    if (reboque.TaraKg > 0)
                        reboqueMDFe.Tara = reboque.TaraKg;
                    else
                        reboqueMDFe.Tara = 1000;
                    if (reboque.TipoCarroceria != null && reboque.TipoCarroceria != "")
                        reboqueMDFe.TipoCarroceria = reboque.TipoCarroceria;
                    else
                        reboqueMDFe.TipoCarroceria = "02";
                    if (reboque.UF != null && reboque.UF != "")
                        reboqueMDFe.UF = repEstado.BuscarPorSigla(reboque.UF);
                    else
                        reboqueMDFe.UF = mdfe.Empresa.Localidade.Estado;

                    reboqueMDFe.RNTRC = mdfe.Empresa.RegistroANTT;

                    Dominio.Entidades.Veiculo veiculoCadastro = repVeiculo.BuscarPorPlaca(mdfe.Empresa.Codigo, reboque.Placa);
                    if (configCTeUtilizaProprietarioCadastro && veiculoCadastro != null && veiculoCadastro.Tipo == "T" && veiculoCadastro.Proprietario != null)
                    {
                        reboqueMDFe.CPFCNPJProprietario = veiculoCadastro.Proprietario.CPF_CNPJ_SemFormato;
                        reboqueMDFe.NomeProprietario = veiculoCadastro.Proprietario.Nome.Length > 60 ? veiculoCadastro.Proprietario.Nome.Substring(0, 60) : veiculoCadastro.Proprietario.Nome;
                        reboqueMDFe.IEProprietario = veiculoCadastro.Proprietario.IE_RG != null && veiculoCadastro.Proprietario.IE_RG != "" ? veiculoCadastro.Proprietario.IE_RG : "ISENTO";
                        reboqueMDFe.UFProprietario = veiculoCadastro.Proprietario.Localidade.Estado;
                        reboqueMDFe.RNTRC = veiculoCadastro.RNTRC > 0 ? string.Format("{0:00000000}", veiculoCadastro.RNTRC) : veiculoCadastro.Empresa.RegistroANTT;
                        reboqueMDFe.TipoProprietario = veiculoCadastro.TipoProprietario.ToString("d");

                    }

                    repReboqueMDFe.Inserir(reboqueMDFe);
                }
            }
        }

        private void GerarReboques(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.CTe.Veiculo> reboques, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (reboques != null && reboques.Count() > 0)
            {
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.CTe.Veiculo reboque in reboques)
                {
                    Dominio.Entidades.ReboqueMDFe reboqueMDFe = new Dominio.Entidades.ReboqueMDFe();

                    reboqueMDFe.CapacidadeKG = reboque.CapacidadeKG;
                    reboqueMDFe.CapacidadeM3 = reboque.CapacidadeM3;
                    reboqueMDFe.MDFe = mdfe;
                    reboqueMDFe.Placa = reboque.Placa;
                    reboqueMDFe.Tara = reboque.Tara;
                    reboqueMDFe.TipoCarroceria = reboque.TipoCarroceria;
                    reboqueMDFe.UF = repEstado.BuscarPorSigla(reboque.UF);
                    reboqueMDFe.RENAVAM = reboque.Renavam;

                    if (reboque.TipoPropriedade == "T" && reboque.Proprietario != null)
                    {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                        Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(reboque.Proprietario.CodigoIBGECidade);

                        reboqueMDFe.UFProprietario = localidade.Estado;
                        reboqueMDFe.CPFCNPJProprietario = reboque.Proprietario.CPFCNPJ;
                        reboqueMDFe.IEProprietario = reboque.Proprietario.RGIE;
                        reboqueMDFe.NomeProprietario = Utilidades.String.Left(reboque.Proprietario.RazaoSocial, 60);
                        reboqueMDFe.RNTRC = string.Format("{0:00000000}", reboque.RNTRCProprietario);
                        reboqueMDFe.TipoProprietario = reboque.TipoProprietario.ToString("d");
                    }

                    repReboqueMDFe.Inserir(reboqueMDFe);
                }
            }
        }

        private void GerarReboques(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Veiculo veiculoReboque, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (veiculoReboque != null)
            {
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);

                Dominio.Entidades.ReboqueMDFe reboqueMDFe = new Dominio.Entidades.ReboqueMDFe();

                reboqueMDFe.CapacidadeKG = veiculoReboque.CapacidadeKG;
                reboqueMDFe.CapacidadeM3 = veiculoReboque.CapacidadeM3;
                reboqueMDFe.MDFe = mdfe;
                reboqueMDFe.Placa = veiculoReboque.Placa;
                reboqueMDFe.Tara = veiculoReboque.Tara;
                reboqueMDFe.TipoCarroceria = veiculoReboque.TipoCarroceria;
                reboqueMDFe.TipoProprietario = veiculoReboque.Tipo;
                reboqueMDFe.UF = veiculoReboque.Estado;
                reboqueMDFe.RENAVAM = veiculoReboque.Renavam;

                if (veiculoReboque.Tipo == "T" && veiculoReboque.Proprietario != null)
                {
                    reboqueMDFe.UFProprietario = veiculoReboque.Proprietario.Localidade.Estado;
                    reboqueMDFe.CPFCNPJProprietario = veiculoReboque.Proprietario.CPF_CNPJ_SemFormato;
                    reboqueMDFe.IEProprietario = veiculoReboque.Proprietario.IE_RG;
                    reboqueMDFe.NomeProprietario = veiculoReboque.Proprietario.Nome.Length > 60 ? veiculoReboque.Proprietario.Nome.Substring(0, 60) : veiculoReboque.Proprietario.Nome;
                    reboqueMDFe.RNTRC = string.Format("{0:00000000}", veiculoReboque.RNTRC);
                }

                repReboqueMDFe.Inserir(reboqueMDFe);
            }
        }

        private void GerarMotoristas(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            if (cte != null)
            {
                Repositorio.MotoristaCTE repMotoristaCTE = new Repositorio.MotoristaCTE(unidadeDeTrabalho);
                List<Dominio.Entidades.MotoristaCTE> motoristasCTE = repMotoristaCTE.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);
                if (motoristasCTE.Count > 0)
                {
                    Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);
                    foreach (Dominio.Entidades.MotoristaCTE motoristaCTE in motoristasCTE)
                    {
                        string cpfMotorista = motoristaCTE.CPFMotorista.Replace(".", "").Replace("-", "");

                        Dominio.Entidades.MotoristaMDFe motorista = new Dominio.Entidades.MotoristaMDFe();
                        motorista.MDFe = mdfe;
                        motorista.CPF = Utilidades.String.Left(cpfMotorista, 11);
                        motorista.Nome = motoristaCTE.NomeMotorista.Length > 60 ? motoristaCTE.NomeMotorista.Substring(0, 60) : motoristaCTE.NomeMotorista;
                        motorista.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal;
                        repMotoristaMDFe.Inserir(motorista);
                    }
                }
                else
                {
                    if (veiculo != null)
                        AdicionarMotoristaVinculadoAoVeiculo(mdfe, veiculo, unidadeDeTrabalho);
                }
            }
            else
            {
                if (veiculo != null)
                    AdicionarMotoristaVinculadoAoVeiculo(mdfe, veiculo, unidadeDeTrabalho);
            }
        }

        private void GerarMotoristasPorObjeto(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

            foreach (Dominio.ObjetosDeValor.MotoristaMDFeIntegracao motorista in motoristas)
            {
                Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe
                {
                    MDFe = mdfe,
                    CPF = Utilidades.String.OnlyNumbers(motorista.CPF),
                    Nome = Utilidades.String.Left(motorista.Nome, 60),
                    Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal
                };

                repMotoristaMDFe.Inserir(motoristaMDFe);
            }
        }

        private void GerarMotoristasPorObjeto(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Usuario> motoristas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe
                {
                    MDFe = mdfe,
                    CPF = Utilidades.String.OnlyNumbers(motorista.CPF),
                    Nome = Utilidades.String.Left(motorista.Nome, 60),
                    Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal
                };

                repMotoristaMDFe.Inserir(motoristaMDFe);
            }
        }

        private void AdicionarMotoristaVinculadoAoVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            var motoristaPrincipal = veiculo.Motoristas.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

            if (motoristaPrincipal != null)
            {
                Repositorio.MotoristaMDFe repMotorista = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

                Dominio.Entidades.MotoristaMDFe motorista = new Dominio.Entidades.MotoristaMDFe();

                motorista.MDFe = mdfe;
                motorista.CPF = motoristaPrincipal.CPF;
                motorista.Nome = motoristaPrincipal.Nome.Length > 60 ? motoristaPrincipal.Nome.Substring(0, 60) : motoristaPrincipal.Nome;
                motorista.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal;
                repMotorista.Inserir(motorista);
            }

            if (veiculo.VeiculoMotoristas != null && veiculo.VeiculoMotoristas.Count > 0)
            {
                Repositorio.MotoristaMDFe repMotorista = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

                foreach (Dominio.Entidades.VeiculoMotoristas motoristaAdicional in veiculo.VeiculoMotoristas)
                {
                    Dominio.Entidades.MotoristaMDFe motorista = new Dominio.Entidades.MotoristaMDFe();

                    motorista.MDFe = mdfe;
                    motorista.Nome = motoristaAdicional.Motorista.Nome.Length > 60 ? motoristaAdicional.Motorista.Nome.Substring(0, 60) : motoristaAdicional.Motorista.Nome;
                    motorista.CPF = motoristaAdicional.Motorista.CPF;
                    motorista.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal;

                    repMotorista.Inserir(motorista);
                }
            }

        }

        private void GerarMotoristas(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.CTe.Motorista> motoristas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (motoristas != null && motoristas.Count() > 0)
            {
                Repositorio.MotoristaMDFe repMotorista = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.CTe.Motorista motorista in motoristas)
                {
                    Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe();

                    motoristaMDFe.MDFe = mdfe;
                    motoristaMDFe.CPF = motorista.CPF;
                    motoristaMDFe.Nome = motorista.Nome;
                    motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal;

                    repMotorista.Inserir(motoristaMDFe);
                }
            }
        }

        public void GerarObservacoesVeiculos(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
            Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

            if (mdfe == null)
                return;

            Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);
            if (veiculoMDFe == null)
                return;

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(mdfe.Empresa.Codigo, veiculoMDFe.Placa);
            if (veiculo == null)
                veiculo = repVeiculo.BuscarPorPlaca(0, veiculoMDFe.Placa);

            List<Dominio.Entidades.ReboqueMDFe> reboquesMDFe = repReboqueMDFe.BuscarPorMDFe(mdfe.Codigo);
            string observacaoVeiculo = null;

            bool formatarPlacaComHifen = mdfe.Empresa.Configuracao != null ? mdfe.Empresa.Configuracao.FormatarPlacaComHifenNaObservacao : false;

            if (veiculo != null && !string.IsNullOrWhiteSpace(veiculo.ObservacaoCTe))
            {
                try
                {
                    observacaoVeiculo = veiculo.ObservacaoCTe;

                    string placasVinculadas = "";
                    if (reboquesMDFe != null && reboquesMDFe.Count > 0)
                    {
                        List<string> listaPlacas = (from veiculosVinculados in reboquesMDFe where veiculo.Placa != veiculosVinculados.Placa select veiculosVinculados.Placa).ToList();
                        placasVinculadas = listaPlacas.Count() > 0 ? String.Join(", ", listaPlacas) : string.Empty;
                    }

                    observacaoVeiculo = observacaoVeiculo.Replace("#CPFCNPJProprietarioVeiculo#", (veiculoMDFe.CPFCNPJProprietario != null && veiculoMDFe.CPFCNPJProprietario != "" ? veiculoMDFe.CPFCNPJProprietario : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#NomeProprietarioVeiculo#", (veiculoMDFe.NomeProprietario != null && veiculoMDFe.NomeProprietario != "" ? veiculoMDFe.NomeProprietario : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#RNTRCProprietario#", (veiculoMDFe.RNTRC != null && veiculoMDFe.RNTRC != "" ? veiculoMDFe.RNTRC : string.Empty));

                    observacaoVeiculo = observacaoVeiculo.Replace("#PlacaVeiculo#", (veiculoMDFe.Placa != null && veiculoMDFe.Placa != "" ? formatarPlacaComHifen ? veiculoMDFe.Placa_Formatada : veiculoMDFe.Placa : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#RENAVAMVeiculo#", (veiculoMDFe.RENAVAM != null && veiculoMDFe.RENAVAM != "" ? veiculoMDFe.RENAVAM : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#UFVeiculo#", (veiculoMDFe.UF.Sigla != null && veiculoMDFe.UF.Sigla != "" ? veiculoMDFe.UF.Sigla : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#MarcaVeiculo#", (veiculo != null && veiculo.Marca != null ? veiculo.Marca.Descricao : string.Empty));

                    observacaoVeiculo = observacaoVeiculo.Replace("#PlacasVinculadas#", (placasVinculadas != "" ? placasVinculadas : string.Empty));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro buscando observação veiculos: " + ex);
                }
            }

            if (veiculo != null && string.IsNullOrWhiteSpace(observacaoVeiculo) && mdfe.Empresa.Configuracao != null && (!string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.ObservacaoCTeAvancadaProprio) || !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.ObservacaoCTeAvancadaTerceiros)))
            {
                observacaoVeiculo = veiculo.Proprietario != null ? mdfe.Empresa.Configuracao.ObservacaoCTeAvancadaTerceiros : mdfe.Empresa.Configuracao.ObservacaoCTeAvancadaProprio;

                if (!string.IsNullOrWhiteSpace(observacaoVeiculo))
                {
                    string placasVinculadas = "";
                    string placasVinculadasRenavam = "";

                    if (reboquesMDFe != null && reboquesMDFe.Count > 0)
                    {
                        List<string> listaPlacas = (from veiculosVinculados in reboquesMDFe where veiculo == null || veiculo.Placa != veiculosVinculados.Placa select veiculosVinculados.Placa).ToList();
                        placasVinculadas = listaPlacas.Count() > 0 ? String.Join(", ", listaPlacas) : string.Empty;

                        List<string> listaPlacasRenavam = (from veiculosVinculados in reboquesMDFe where veiculo == null || veiculo.Placa != veiculosVinculados.Placa select veiculosVinculados.Placa + "/" + veiculosVinculados.RENAVAM).ToList();
                        placasVinculadasRenavam = listaPlacasRenavam.Count() > 0 ? String.Join(", ", listaPlacasRenavam) : string.Empty;
                    }

                    List<Dominio.Entidades.MotoristaMDFe> motoristasMDFe = repMotoristaMDFe.BuscarPorMDFe(mdfe.Codigo);

                    observacaoVeiculo = observacaoVeiculo.Replace("#NomeMotorista#", motoristasMDFe != null && motoristasMDFe.Count() > 0 ? motoristasMDFe.FirstOrDefault().Nome : string.Empty);
                    observacaoVeiculo = observacaoVeiculo.Replace("#CPFMotorista#", motoristasMDFe != null && motoristasMDFe.Count() > 0 ? motoristasMDFe.FirstOrDefault().CPF : string.Empty);

                    observacaoVeiculo = observacaoVeiculo.Replace("#CPFCNPJProprietario#", (veiculoMDFe.CPFCNPJProprietario != null && veiculoMDFe.CPFCNPJProprietario != "" ? veiculoMDFe.CPFCNPJProprietario : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#NomeProprietario#", (veiculoMDFe.NomeProprietario != null && veiculoMDFe.NomeProprietario != "" ? veiculoMDFe.NomeProprietario : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#RNTRCProprietario#", (veiculoMDFe.RNTRC != null && veiculoMDFe.RNTRC != "" ? veiculoMDFe.RNTRC : string.Empty));

                    observacaoVeiculo = observacaoVeiculo.Replace("#PlacaVeiculo#", (veiculoMDFe.Placa != null && veiculoMDFe.Placa != "" ? formatarPlacaComHifen ? veiculoMDFe.Placa_Formatada : veiculoMDFe.Placa : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#RENAVAMVeiculo#", (veiculoMDFe.RENAVAM != null && veiculoMDFe.RENAVAM != "" ? veiculoMDFe.RENAVAM : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#UFVeiculo#", (veiculoMDFe.UF.Sigla != null && veiculoMDFe.UF.Sigla != "" ? veiculoMDFe.UF.Sigla : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#MarcaVeiculo#", (veiculo != null && veiculo.Marca != null ? veiculo.Marca.Descricao : string.Empty));

                    observacaoVeiculo = observacaoVeiculo.Replace("#PlacasVinculadas#", (placasVinculadas != "" ? placasVinculadas : string.Empty));
                    observacaoVeiculo = observacaoVeiculo.Replace("#PlacasRenavamVinculadas#", (placasVinculadasRenavam != "" ? placasVinculadasRenavam : string.Empty));
                }
            }

            if (veiculo != null)
            {
                if (!string.IsNullOrWhiteSpace(veiculo.NumeroCompraValePedagio) && veiculo.ValorValePedagio > 0)
                    observacaoVeiculo = string.IsNullOrWhiteSpace(observacaoVeiculo) ? "Valor vale pedágio = " + veiculo.ValorValePedagio.ToString() : string.Concat("Valor vale pedágio = " + veiculo.ValorValePedagio.ToString(), " - ", observacaoVeiculo);

                if (!string.IsNullOrWhiteSpace(veiculo.CIOT))
                    observacaoVeiculo = string.IsNullOrWhiteSpace(observacaoVeiculo) ? "CIOT " + veiculo.CIOT : string.Concat("CIOT " + veiculo.CIOT, " - ", observacaoVeiculo);
            }

            if (!string.IsNullOrWhiteSpace(observacaoVeiculo))
            {
                mdfe.ObservacaoContribuinte = string.Concat(mdfe.ObservacaoContribuinte, " - ", observacaoVeiculo);
                repMDFe.Atualizar(mdfe);
            }
        }

        public void GerarPercursos(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unidadeDeTrabalho);
            Repositorio.PassagemPercursoEstado repPassagem = new Repositorio.PassagemPercursoEstado(unidadeDeTrabalho);
            Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unidadeDeTrabalho);

            if (passagens == null || passagens.Count == 0)
            {
                Dominio.Entidades.PercursoEstado percurso = null;
                if (percursoEstado == null)
                    percurso = repPercursoEstado.Buscar(mdfe.Empresa.Codigo, mdfe.EstadoCarregamento.Sigla, mdfe.EstadoDescarregamento.Sigla);
                else
                    percurso = percursoEstado;

                if (percurso == null && mdfe.Empresa.EmpresaPai != null)
                    percurso = repPercursoEstado.Buscar(mdfe.Empresa.EmpresaPai.Codigo, mdfe.EstadoCarregamento.Sigla, mdfe.EstadoDescarregamento.Sigla);

                if (percurso != null)
                {
                    List<Dominio.Entidades.PassagemPercursoEstado> passagemPercursoEstado = repPassagem.Buscar(percurso.Codigo);

                    foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagemPercursoEstado)
                    {
                        Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe();

                        percursoMDFe.Estado = passagem.EstadoDePassagem;
                        percursoMDFe.MDFe = mdfe;
                        repPercursoMDFe.Inserir(percursoMDFe);
                    }
                }
            }
            else
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passagem in passagens)
                {
                    Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe();
                    percursoMDFe.Estado = new Dominio.Entidades.Estado() { Sigla = passagem.Sigla };
                    percursoMDFe.MDFe = mdfe;
                    repPercursoMDFe.Inserir(percursoMDFe);
                }
            }

            if (configuracaoTMS != null && configuracaoTMS.ImprimirPercursoMDFe)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                List<string> estados = repPercursoMDFe.BuscarSiglaEstadoPorMDFe(mdfe.Codigo);

                if (!string.IsNullOrWhiteSpace(mdfe.ObservacaoContribuinte))
                    mdfe.ObservacaoContribuinte += " / ";

                mdfe.ObservacaoContribuinte += "Estados de Passagem: " + string.Join(", ", estados) + ".";

                repMDFe.Atualizar(mdfe);
            }
        }

        private void GerarPercursos(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.Percurso> percursos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (percursos != null && percursos.Count() > 0)
            {
                Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.MDFe.Percurso percurso in percursos)
                {
                    Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe();

                    DateTime data;
                    DateTime.TryParseExact(percurso.DataHoraPrevistaInicioViagem, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data);

                    if (data != DateTime.MinValue)
                        percursoMDFe.DataInicioViagem = data;

                    percursoMDFe.Estado = repEstado.BuscarPorSigla(percurso.UFPercurso);
                    percursoMDFe.MDFe = mdfe;

                    repPercursoMDFe.Inserir(percursoMDFe);
                }
            }
            else if (mdfe.Empresa.EmpresaPai != null)
            {
                Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unidadeDeTrabalho);
                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unidadeDeTrabalho);
                Repositorio.PassagemPercursoEstado repPassagem = new Repositorio.PassagemPercursoEstado(unidadeDeTrabalho);

                Dominio.Entidades.PercursoEstado percurso = repPercursoEstado.Buscar(mdfe.Empresa.EmpresaPai.Codigo, mdfe.EstadoCarregamento.Sigla, mdfe.EstadoDescarregamento.Sigla);

                if (percurso != null)
                {
                    List<Dominio.Entidades.PassagemPercursoEstado> passagemPercursoEstado = repPassagem.Buscar(percurso.Codigo);

                    if (mdfe.EstadoCarregamento.Sigla == "EX")
                    {
                        Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe();
                        percursoMDFe.Estado = percurso.EstadoOrigem;
                        percursoMDFe.MDFe = mdfe;
                        repPercursoMDFe.Inserir(percursoMDFe);
                    }

                    foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagemPercursoEstado)
                    {
                        Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe();

                        percursoMDFe.Estado = passagem.EstadoDePassagem;
                        percursoMDFe.MDFe = mdfe;
                        repPercursoMDFe.Inserir(percursoMDFe);
                    }

                    if (mdfe.EstadoDescarregamento.Sigla == "EX")
                    {
                        Dominio.Entidades.PercursoMDFe percursoMDFe = new Dominio.Entidades.PercursoMDFe();
                        percursoMDFe.Estado = percurso.EstadoDestino;
                        percursoMDFe.MDFe = mdfe;
                        repPercursoMDFe.Inserir(percursoMDFe);
                    }
                }
            }
        }

        private void GerarValePedagioCompra(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (valePedagioCompra != null)
            {
                if ((mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma) ||
                    (mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma))
                {
                    Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                    Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);
                    Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);

                    Dominio.Entidades.ValePedagioMDFeCompra valePedagio = new Dominio.Entidades.ValePedagioMDFeCompra();

                    int.TryParse(valePedagioCompra.IBGEInicio, out int ibgeInicio);
                    int.TryParse(valePedagioCompra.IBGEFim, out int ibgeFim);

                    string cnpjResponsavel = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma ? mdfe.Empresa.Configuracao.ValePedagioResponsavel : !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioResponsavel) ? mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioResponsavel : string.Empty;
                    if (string.IsNullOrWhiteSpace(cnpjResponsavel))
                    {
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentoMunicipioDescarregamentoMDFe.BuscarCTesPorMDFe(mdfe.Codigo);
                        if (listaCTes != null && listaCTes.Count > 0 && listaCTes[0] != null && listaCTes[0].Remetente != null)
                            cnpjResponsavel = listaCTes[0].Remetente.CPF_CNPJ;
                    }

                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(cnpjResponsavel);

                    valePedagio.MDFe = mdfe;
                    valePedagio.CNPJFornecedor = filial != null && !string.IsNullOrWhiteSpace(filial.FornecedorValePedagio) ? filial.FornecedorValePedagio : mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma ? mdfe.Empresa.Configuracao.ValePedagioFornecedor : mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioFornecedor;
                    valePedagio.CNPJResponsavel = cnpjResponsavel;
                    valePedagio.IBGEInicio = ibgeInicio;
                    valePedagio.IBGEFim = ibgeFim;
                    valePedagio.IntegracaoUsuario = filial != null && !string.IsNullOrWhiteSpace(filial.UsuarioValePedagio) ? filial.UsuarioValePedagio : mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma ? mdfe.Empresa.Configuracao.ValePedagioUsuario : mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioUsuario;
                    valePedagio.IntegracaoSenha = filial != null && !string.IsNullOrWhiteSpace(filial.SenhaValePedagio) ? filial.SenhaValePedagio : mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma ? mdfe.Empresa.Configuracao.ValePedagioSenha : mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioSenha;
                    valePedagio.IntegracaoToken = filial != null && !string.IsNullOrWhiteSpace(filial.TokenValePedagio) ? filial.TokenValePedagio : mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma ? mdfe.Empresa.Configuracao.ValePedagioToken : mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioToken;
                    valePedagio.Integradora = filial != null ? filial.IntegradoraValePedagio : mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora != Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma ? mdfe.Empresa.Configuracao.ValePedagioIntegradora : mdfe.Empresa.EmpresaPai.Configuracao.ValePedagioIntegradora;
                    valePedagio.Tipo = Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao;
                    valePedagio.Status = filial != null && filial.CompraValePedagio == Dominio.Enumeradores.OpcaoSimNao.Sim ? Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente : Dominio.Enumeradores.StatusIntegracaoValePedagio.FilialNaoLiberadaParaCompra;
                    valePedagio.Mensagem = filial == null || filial.CompraValePedagio == Dominio.Enumeradores.OpcaoSimNao.Nao ? "Filial CNPJ " + cnpjResponsavel + " não liberada para compra de vale pedágio." : string.Empty;
                    valePedagio.Valor = 0;
                    valePedagio.ComprarRetorno = Dominio.Enumeradores.OpcaoSimNao.Nao; //valePedagioCompra.ComprarRetorno ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                    valePedagio.DescricaoRota = valePedagioCompra.NomeRota ?? string.Empty;
                    valePedagio.UrlIntegracaoRest = filial != null && !string.IsNullOrEmpty(filial.URLIntegracaoRest) ? filial.URLIntegracaoRest : mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ValePedagioIntegraAutomatico == 1 && mdfe.Empresa.Configuracao.ValePedagioIntegradora == Dominio.Enumeradores.IntegradoraValePedagio.SemParar ? mdfe.Empresa.Configuracao?.URLIntegracaoRest : string.Empty;

                    repValePedagioMDFeCompra.Inserir(valePedagio);

                    if (valePedagioCompra.ComprarRetorno) //(valePedagio.ComprarRetorno == Dominio.Enumeradores.OpcaoSimNao.Sim)
                    {
                        Dominio.Entidades.ValePedagioMDFeCompra valePedagioRetorno = new Dominio.Entidades.ValePedagioMDFeCompra();

                        valePedagioRetorno.MDFe = mdfe;
                        valePedagioRetorno.CNPJFornecedor = valePedagio.CNPJFornecedor;
                        valePedagioRetorno.CNPJResponsavel = cnpjResponsavel;
                        valePedagioRetorno.IBGEInicio = ibgeFim;
                        valePedagioRetorno.IBGEFim = ibgeInicio;
                        valePedagioRetorno.IntegracaoUsuario = valePedagio.IntegracaoUsuario;
                        valePedagioRetorno.IntegracaoSenha = valePedagio.IntegracaoSenha;
                        valePedagioRetorno.IntegracaoToken = valePedagio.IntegracaoToken;
                        valePedagioRetorno.Integradora = valePedagio.Integradora;
                        valePedagioRetorno.Tipo = Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao;
                        valePedagioRetorno.Status = valePedagio.Status;
                        valePedagioRetorno.Mensagem = valePedagio.Mensagem;
                        valePedagioRetorno.Valor = 0;
                        valePedagioRetorno.ComprarRetorno = Dominio.Enumeradores.OpcaoSimNao.Sim;

                        repValePedagioMDFeCompra.Inserir(valePedagioRetorno);
                    }
                }
            }
        }

        private void GerarInformacoesAquaviario(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoEmbarque, Dominio.ObjetosDeValor.PortoMDFeIntegracao portoDesembarque, Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao viagem, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisCarregamento, List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisDescarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);

            if (portoEmbarque != null)
                mdfe.PortoOrigem = repPorto.BuscarPorCodigo(portoEmbarque.Codigo);
            else
                mdfe.PortoOrigem = null;

            if (portoDesembarque != null)
                mdfe.PortoDestino = repPorto.BuscarPorCodigo(portoDesembarque.Codigo);
            else
                mdfe.PortoDestino = null;

            if (viagem != null)
                mdfe.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(viagem.Codigo);
            else
                mdfe.PedidoViagemNavio = null;

            mdfe.TerminalCarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            mdfe.TerminalDescarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            if (terminaisCarregamento != null && terminaisCarregamento.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.TerminalMDFeIntegracao terminal in terminaisCarregamento)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao ter = repTipoTerminalImportacao.BuscarPorCodigo(terminal.Codigo);
                    mdfe.TerminalCarregamento.Add(ter);
                }
            }
            if (terminaisDescarregamento != null && terminaisDescarregamento.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.TerminalMDFeIntegracao terminal in terminaisDescarregamento)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao ter = repTipoTerminalImportacao.BuscarPorCodigo(terminal.Codigo);
                    mdfe.TerminalDescarregamento.Add(ter);
                }
            }
        }

        private void GerarLacres(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (lacres != null && lacres.Count() > 0)
            {
                Repositorio.LacreMDFe repLacreMDFe = new Repositorio.LacreMDFe(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.MDFe.Lacre lacre in lacres)
                {
                    if (string.IsNullOrWhiteSpace(lacre.Numero))
                        Servicos.Log.TratarErro(mdfe.Numero.ToString() + "1", "lacrevazio");

                    Dominio.Entidades.LacreMDFe lacreMDFe = new Dominio.Entidades.LacreMDFe();
                    string strLacre = Utilidades.String.RemoveDiacritics(lacre.Numero);
                    strLacre = Utilidades.String.Left(strLacre, 20);

                    if (string.IsNullOrWhiteSpace(strLacre))
                        Servicos.Log.TratarErro(mdfe.Numero.ToString() + "2", "lacrevazio");

                    lacreMDFe.Numero = strLacre;
                    lacreMDFe.MDFe = mdfe;

                    repLacreMDFe.Inserir(lacreMDFe);
                }
            }
        }

        private void GerarValesPedagios(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.ValePedagio> valesPedagios, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (valesPedagios != null && valesPedagios.Count() > 0)
            {
                Repositorio.ValePedagioMDFe repValePedagio = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.MDFe.ValePedagio valePedagio in valesPedagios)
                {
                    Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();

                    valePedagioMDFe.CNPJFornecedor = valePedagio.CNPJFornecedor;
                    valePedagioMDFe.CNPJResponsavel = valePedagio.CNPJResponsavel;
                    valePedagioMDFe.MDFe = mdfe;
                    valePedagioMDFe.NumeroComprovante = valePedagio.NumeroComprovante;
                    valePedagioMDFe.CodigoAgendamentoPorto = valePedagio.CodigoAgendamentoPorto;
                    valePedagioMDFe.QuantidadeEixos = valePedagio.QuantidadeEixos;
                    valePedagioMDFe.TipoCompra = valePedagio.TipoCompra.HasValue ? valePedagio.TipoCompra.Value : Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                    valePedagioMDFe.ValorValePedagio = valePedagio.ValorValePedagio;

                    repValePedagio.Inserir(valePedagioMDFe);
                }
            }
        }

        private void GerarMunicipiosDeCarregamentoPorNFeGlobalizada(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeCarregamento = null)
        {
            Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamento = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            if (localidadeCarregamento == null)
            {
                IEnumerable<int> ibgesOrigem = (from obj in nfesGlobalizadas select obj.IBGEOrigem).Distinct();

                foreach (int ibge in ibgesOrigem)
                {
                    Dominio.Entidades.Localidade cidade = repLocalidade.BuscarPorCodigoIBGE(ibge);
                    Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                    municipioCarregamento.MDFe = mdfe;
                    municipioCarregamento.Municipio = cidade;

                    repMunicipioCarregamento.Inserir(municipioCarregamento);

                    mdfe.CEPCarregamentoLotacao = cidade.CEP;
                    mdfe.LatitudeCarregamentoLotacao = cidade.Latitude;
                    mdfe.LongitudeCarregamentoLotacao = cidade.Longitude;
                }
            }
            else
            {
                Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();
                municipioCarregamento.MDFe = mdfe;
                municipioCarregamento.Municipio = localidadeCarregamento;
                repMunicipioCarregamento.Inserir(municipioCarregamento);

                mdfe.CEPCarregamentoLotacao = localidadeCarregamento.CEP;
                mdfe.LatitudeCarregamentoLotacao = localidadeCarregamento.Latitude;
                mdfe.LongitudeCarregamentoLotacao = localidadeCarregamento.Longitude;
            }
        }

        private void GerarMunicipiosDeDescarregamentoPorNFeGlobalizada(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Localidade localidadeDescarregamento = null, bool gerarProdutosPerigosos = true)
        {
            Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            if (localidadeDescarregamento == null)
            {
                IEnumerable<int> ibgesDestino = (from obj in nfesGlobalizadas select obj.IBGEDestino).Distinct();

                int quantidadeMunicipios = 0;
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoAnterior = null;
                foreach (int ibge in ibgesDestino)
                {
                    Dominio.Entidades.Localidade municipio = repLocalidade.BuscarPorCodigoIBGE(ibge);

                    var notasPorMunicipio = (from obj in nfesGlobalizadas where obj.IBGEDestino == ibge select obj).ToList();
                    if (notasPorMunicipio.Count > 0)
                    {
                        if (quantidadeMunicipios < 100)
                        {
                            Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                            municipioDescarregamento.MDFe = mdfe;
                            municipioDescarregamento.Municipio = municipio;

                            mdfe.CEPDescarregamentoLotacao = municipio.CEP;
                            mdfe.LatitudeDescarregamentoLotacao = municipio.Latitude;
                            mdfe.LongitudeDescarregamentoLotacao = municipio.Longitude;

                            repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                            this.GerarDocumentosDoMunicipioDeDescarregamentoPorNFeGlobalizada(municipioDescarregamento, notasPorMunicipio, unidadeDeTrabalho);
                            municipioDescarregamentoAnterior = municipioDescarregamento;
                        }
                        else if (municipioDescarregamentoAnterior != null)
                            this.GerarDocumentosDoMunicipioDeDescarregamentoPorNFeGlobalizada(municipioDescarregamentoAnterior, notasPorMunicipio, unidadeDeTrabalho);

                        quantidadeMunicipios += 1;
                    }
                }
            }
            else
            {
                Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.MDFe = mdfe;
                municipioDescarregamento.Municipio = localidadeDescarregamento;
                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                mdfe.CEPDescarregamentoLotacao = localidadeDescarregamento.CEP;
                mdfe.LatitudeDescarregamentoLotacao = localidadeDescarregamento.Latitude;
                mdfe.LongitudeDescarregamentoLotacao = localidadeDescarregamento.Longitude;

                this.GerarDocumentosDoMunicipioDeDescarregamentoPorNFeGlobalizada(municipioDescarregamento, nfesGlobalizadas, unidadeDeTrabalho);
            }

        }

        private void GerarDocumentosDoMunicipioDeDescarregamentoPorNFeGlobalizada(Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (nfesGlobalizadas != null && nfesGlobalizadas.Count() > 0)
            {
                Repositorio.NotaFiscalEletronicaMDFe repNotasMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);

                foreach (var nota in nfesGlobalizadas)
                {
                    Dominio.Entidades.NotaFiscalEletronicaMDFe doc = new Dominio.Entidades.NotaFiscalEletronicaMDFe();

                    doc.MunicipioDescarregamento = municipioDescarregamento;
                    doc.Chave = nota.ChaveNFe;

                    repNotasMDFe.Inserir(doc);
                }
            }
        }

        private async Task<ServicoMDFe.MDFe> ObterMDFeParaEmissaoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            ServicoMDFe.MDFe mdfeIntegrar = new ServicoMDFe.MDFe();

            mdfeIntegrar.DataEmissao = mdfe.DataEmissao.HasValue ? mdfe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(mdfe.DataEmissao.HasValue ? mdfe.DataEmissao.Value : DateTime.Today);
            string fusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);
            string observacaoContribuinte = Utilidades.String.RemoveDiacritics(mdfe.ObservacaoContribuinte);
            string observacaoFisco = Utilidades.String.RemoveDiacritics(mdfe.ObservacaoFisco);

            mdfeIntegrar.ID = mdfe.Chave;
            mdfeIntegrar.Emitente = ObterEmpresaEmitente(empresa);
            mdfeIntegrar.Modal = ObterModal(mdfe.Modal);
            mdfeIntegrar.Modelo = int.Parse(mdfe.Modelo.Numero);
            mdfeIntegrar.Numero = mdfe.Numero;
            mdfeIntegrar.FusoHorario = fusoHorario;
            mdfeIntegrar.ObservacaoContribuinte = string.IsNullOrWhiteSpace(observacaoContribuinte) ? string.Empty : observacaoContribuinte.Substring(0, Math.Min(observacaoContribuinte.Length, 4000));
            mdfeIntegrar.ObservacaoFisco = string.IsNullOrWhiteSpace(observacaoFisco) ? string.Empty : observacaoFisco.Substring(0, Math.Min(observacaoFisco.Length, 2000)); ;
            mdfeIntegrar.QuantidadeCarga = mdfe.PesoBrutoMercadoria;
            mdfeIntegrar.Serie = mdfe.Serie.Numero;
            mdfeIntegrar.TipoAmbiente = mdfe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? ServicoMDFe.TipoAmbiente.Producao : ServicoMDFe.TipoAmbiente.Homologacao;
            mdfeIntegrar.TipoEmissao = mdfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoMDFe.Contingencia ? ServicoMDFe.TipoEmissao.Contingencia : ServicoMDFe.TipoEmissao.Normal;
            mdfeIntegrar.TipoEmitente = mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte ? ServicoMDFe.TipoEmitente.NaoPrestadorDeServicoDeTransporte :
                                        mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.TransporteCTeGlobalizado ? ServicoMDFe.TipoEmitente.TransporteCTeGlobalizado : ServicoMDFe.TipoEmitente.PrestadorDeServicoDeTransporte;
            mdfeIntegrar.UFCarregamento = mdfe.EstadoCarregamento.Sigla;
            mdfeIntegrar.UFDescarregamento = mdfe.EstadoDescarregamento.Sigla;
            mdfeIntegrar.UnidadeMedida = string.Format("{0:00}", (int)mdfe.UnidadeMedidaMercadoria);
            mdfeIntegrar.ValorCarga = mdfe.ValorTotalMercadoria;

            mdfeIntegrar.Irin = mdfe.PedidoViagemNavio?.Navio?.Irin ?? "";
            mdfeIntegrar.CodigoTipoEmbarcacao = mdfe.PedidoViagemNavio != null && mdfe.PedidoViagemNavio.Navio != null ? "04" : "";//mdfe.PedidoViagemNavio?.Navio?.TipoEmbarcacao ?? "";
            mdfeIntegrar.CodigoEmbarcacao = mdfe.PedidoViagemNavio?.Navio?.CodigoIMO ?? "";
            mdfeIntegrar.NomeEmbarcacao = mdfe.PedidoViagemNavio?.Navio?.Descricao ?? "";
            mdfeIntegrar.NumeroViagem = mdfe.PedidoViagemNavio?.NumeroViagem.ToString("D") ?? "";
            mdfeIntegrar.CodigoPortoEmbarque = mdfe.PortoOrigem?.CodigoIATA ?? "";
            mdfeIntegrar.CodigoPortoDestino = mdfe.PortoDestino?.CodigoIATA ?? "";
            mdfeIntegrar.TipoNavegacao = "1";

            mdfeIntegrar.TerminaisCarregamento = this.ObterTerminaisCarregamento(mdfe);
            mdfeIntegrar.TerminaisDescarregamento = this.ObterTerminaisDescarregamento(mdfe);

            mdfeIntegrar.Lacres = await ObterLacresAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.MunicipiosCarregamento = await ObterMunicipiosCarregamentoAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.MunicipiosDescarregamento = await ObterMunicipiosDescarregamentoAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.Percursos = await ObterPercursosAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.Reboques = await ObterReboquesAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.ValesPedagios = await ObterValesPedagioAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.Veiculo = await ObterVeiculoAsync(mdfe).ConfigureAwait(false);

            double versao = 3;
            if (!string.IsNullOrWhiteSpace(mdfe.Versao))
                versao = double.Parse(mdfe.Versao, cultura);

            mdfeIntegrar.Versao = versao;

            mdfeIntegrar.CIOT = await ObterCIOTAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.Contratante = await ObterContratanteAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.Seguro = await ObterSeguroAsync(mdfe).ConfigureAwait(false);
            mdfeIntegrar.ProdutoPredominante = this.ObterProdutoPredominante(mdfe, mdfeIntegrar);
            mdfeIntegrar.CnpjAutorizados = string.Join(";", repositorioDocMDFe.BuscarCNPJsAutorizadosDFe(mdfe.Codigo));

            ObterInformacoesPagamento(mdfe, mdfeIntegrar);

            if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().EnviarEmailMDFeClientes.Value)
                mdfeIntegrar.EmailEmbarcador = await ObterEmailsClientesAsync(mdfe).ConfigureAwait(false);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasMDFe = await repositorioDocMDFe.BuscarGrupoPessoasPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail grupoPessoasModeloDocumentoEmail = grupoPessoasMDFe?.EmailsModeloDocumento?.FirstOrDefault(o => o.ModeloDocumentoFiscal == mdfe.Modelo);

            if (!string.IsNullOrWhiteSpace(grupoPessoasModeloDocumentoEmail?.Emails))
                mdfeIntegrar.EmailEmbarcador += grupoPessoasModeloDocumentoEmail.Emails + ";";

            if (mdfeIntegrar.MunicipiosDescarregamento != null)
            {
                mdfeIntegrar.QuantidadeCTe = (from obj in mdfeIntegrar.MunicipiosDescarregamento where obj.Documentos != null select (from x in obj.Documentos where !string.IsNullOrWhiteSpace(x.ChaveCTe) select x).Count()).Sum();
                mdfeIntegrar.QuantidadeNFe = (from obj in mdfeIntegrar.MunicipiosDescarregamento where obj.Documentos != null select (from x in obj.Documentos where !string.IsNullOrWhiteSpace(x.ChaveNFe) select x).Count()).Sum();
            }

            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
            {
                Repositorio.XMLMDFe repositorioXMLMDFe = new Repositorio.XMLMDFe(_unitOfWork, _cancellationToken);
                Dominio.Entidades.XMLMDFe xmlMDFe = await repositorioXMLMDFe.BuscarPorMDFeAsync(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao, _cancellationToken).ConfigureAwait(false);

                if (xmlMDFe != null && !string.IsNullOrWhiteSpace(xmlMDFe.XML))
                    mdfeIntegrar.ArquivoXML = xmlMDFe.XML.Replace(Convert.ToChar(0x00).ToString(), "");
            }

            return mdfeIntegrar;
        }

        private void ObterInformacoesPagamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, ServicoMDFe.MDFe mdfeIntegrar)
        {
            Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias repositorioMDFeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(_unitOfWork);
            Repositorio.Embarcador.MDFE.MDFePagamentoComponente repositorioMDFePagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(_unitOfWork);
            Repositorio.Embarcador.MDFE.MDFePagamentoParcela repositorioMDFePagamentoParcela = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(_unitOfWork);

            Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias = repositorioMDFeInformacoesBancarias.BuscarPorMDFe(mdfe.Codigo);

            if (informacoesBancarias == null)
                return;

            List<Dominio.Entidades.MDFeContratante> contratantes = new Repositorio.MDFeContratante(_unitOfWork).BuscarPorMDFe(mdfe.Codigo);
            if (contratantes == null || contratantes.Count != 1)
                return;

            List<Dominio.Entidades.MDFePagamentoComponente> pagamentosComponente = repositorioMDFePagamentoComponente.BuscarPorInformacoesBancarias(informacoesBancarias.Codigo);
            Dominio.Entidades.MDFePagamentoParcela pagamentoParcela = repositorioMDFePagamentoParcela.BuscarPorInformacoesBancariasENumeroParcela(informacoesBancarias.Codigo, 1);

            mdfeIntegrar.PagCnpjCpfResponsavel = contratantes.FirstOrDefault().Contratante;

            if (mdfeIntegrar.MDFeComponentes == null && pagamentosComponente.Count > 0)
                mdfeIntegrar.MDFeComponentes = new List<ServicoMDFe.MDFeComponente>();

            foreach (var componente in pagamentosComponente)
                mdfeIntegrar.MDFeComponentes.Add(new Servicos.ServicoMDFe.MDFeComponente
                {
                    Tipo = (int)componente.TipoComponente,
                    Valor = componente.ValorComponente ?? 0
                });

            mdfeIntegrar.PagIndicador = (int)informacoesBancarias.TipoPagamento;
            mdfeIntegrar.PagValorAdiantamento = informacoesBancarias.ValorAdiantamento ?? 0;
            mdfeIntegrar.PagTipoInfBancaria = (int)(informacoesBancarias.TipoInformacaoBancaria ?? TipoPagamentoMDFe.Banco);

            if (pagamentoParcela != null)
            {
                mdfeIntegrar.ParcelaNumero = pagamentoParcela.NumeroParcela ?? 1;
                mdfeIntegrar.ParcelaVencimento = pagamentoParcela.DataVencimentoParcela;
                mdfeIntegrar.ParcelaValor = pagamentoParcela.ValorParcela ?? 0;
            }

            mdfeIntegrar.InfBancBanco = informacoesBancarias.Conta;
            mdfeIntegrar.InfBancAgencia = informacoesBancarias.Agencia;
            mdfeIntegrar.InfBancChavePix = informacoesBancarias.ChavePIX;
            mdfeIntegrar.InfBancIpef = informacoesBancarias.Ipef.ObterSomenteNumeros();

            mdfeIntegrar.PagIndAltoDesemp = informacoesBancarias.IndicadorAltoDesempenho.HasValue ? (informacoesBancarias.IndicadorAltoDesempenho.Value ? 1 : 0) : 0;
        }

        private ServicoMDFe.MDFeProdutoPredominante ObterProdutoPredominante(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, ServicoMDFe.MDFe mdfeIntegrar)
        {
            ServicoMDFe.MDFeProdutoPredominante produtoPredominante = new ServicoMDFe.MDFeProdutoPredominante();
            produtoPredominante.CEAN = mdfe.ProdutoPredominanteCEAN;
            produtoPredominante.NCM = mdfe.ProdutoPredominanteNCM;
            produtoPredominante.DescricaoProduto = !string.IsNullOrWhiteSpace(mdfe.ProdutoPredominanteDescricao) ? mdfe.ProdutoPredominanteDescricao : "DIVERSOS";

            if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.GranelSolido)
                produtoPredominante.TipoCarga = "01";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.GranelLiquido)
                produtoPredominante.TipoCarga = "02";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.Frigorificada)
                produtoPredominante.TipoCarga = "03";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.Conteinerizada)
                produtoPredominante.TipoCarga = "04";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.CargaGeral)
                produtoPredominante.TipoCarga = "05";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.Neogranel)
                produtoPredominante.TipoCarga = "06";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaGranelSolido)
                produtoPredominante.TipoCarga = "07";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaGranelLiquido)
                produtoPredominante.TipoCarga = "08";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaFrigorificada)
                produtoPredominante.TipoCarga = "09";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaConteinerizada)
                produtoPredominante.TipoCarga = "10";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.PerigosaCargaGeral)
                produtoPredominante.TipoCarga = "11";
            else if (mdfe.TipoCargaMDFe == Dominio.Enumeradores.TipoCargaMDFe.GranelPressurizada)
                produtoPredominante.TipoCarga = "12";
            else
                produtoPredominante.TipoCarga = "05"; //CargaGeral

            if (mdfeIntegrar.UFCarregamento == "EX" && string.IsNullOrWhiteSpace(mdfe.CEPCarregamentoLotacao) && (!mdfe.LatitudeCarregamentoLotacao.HasValue || mdfe.LatitudeCarregamentoLotacao == 0))
            {
                produtoPredominante.CEPCarregamento = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                produtoPredominante.LatitudeCarregamento = mdfe.Empresa.Localidade.Latitude.HasValue ? mdfe.Empresa.Localidade.Latitude.Value : 0;
                produtoPredominante.LongitudeCarregamento = mdfe.Empresa.Localidade.Longitude.HasValue ? mdfe.Empresa.Localidade.Longitude.Value : 0;

            }
            else
            {
                produtoPredominante.CEPCarregamento = !string.IsNullOrEmpty(mdfe.CEPCarregamentoLotacao) ? Utilidades.String.OnlyNumbers(mdfe.CEPCarregamentoLotacao).PadLeft(8, '0').Left(8) : string.Empty;
                produtoPredominante.LatitudeCarregamento = mdfe.LatitudeCarregamentoLotacao.HasValue ? mdfe.LatitudeCarregamentoLotacao.Value : 0;
                produtoPredominante.LongitudeCarregamento = mdfe.LongitudeCarregamentoLotacao.HasValue ? mdfe.LongitudeCarregamentoLotacao.Value : 0;
            }

            if (mdfeIntegrar.UFDescarregamento == "EX" && (string.IsNullOrWhiteSpace(mdfe.CEPDescarregamentoLotacao)) && (!mdfe.LatitudeDescarregamentoLotacao.HasValue || mdfe.LatitudeDescarregamentoLotacao == 0))
            {
                produtoPredominante.CEPDescarregamento = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                produtoPredominante.LatitudeDescarregamento = mdfe.Empresa.Localidade.Latitude.HasValue ? mdfe.Empresa.Localidade.Latitude.Value : 0;
                produtoPredominante.LongitudeDescarregamento = mdfe.Empresa.Localidade.Longitude.HasValue ? mdfe.Empresa.Localidade.Longitude.Value : 0;

            }
            else
            {
                produtoPredominante.CEPDescarregamento = !string.IsNullOrWhiteSpace(mdfe.CEPDescarregamentoLotacao) ? Utilidades.String.OnlyNumbers(mdfe.CEPDescarregamentoLotacao).PadLeft(8, '0').Left(8) : string.Empty;
                produtoPredominante.LatitudeDescarregamento = mdfe.LatitudeDescarregamentoLotacao.HasValue ? mdfe.LatitudeDescarregamentoLotacao.Value : 0;
                produtoPredominante.LongitudeDescarregamento = mdfe.LongitudeDescarregamentoLotacao.HasValue ? mdfe.LongitudeDescarregamentoLotacao.Value : 0;
            }


            if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte) //Transporte de Nota
            {
                if (string.IsNullOrWhiteSpace(produtoPredominante.CEPCarregamento) && (produtoPredominante.LatitudeCarregamento == 0) && mdfe.MunicipiosCarregamento != null && mdfe.MunicipiosCarregamento.Count > 0)
                {
                    produtoPredominante.CEPCarregamento = Utilidades.String.OnlyNumbers(mdfe.MunicipiosCarregamento.FirstOrDefault().Municipio.CEP).PadLeft(8, '0').Left(8);
                    produtoPredominante.LatitudeCarregamento = mdfe.MunicipiosCarregamento.FirstOrDefault().Municipio.Latitude.HasValue ? mdfe.MunicipiosCarregamento.FirstOrDefault().Municipio.Latitude.Value : 0;
                    produtoPredominante.LongitudeCarregamento = mdfe.MunicipiosCarregamento.FirstOrDefault().Municipio.Longitude.HasValue ? mdfe.MunicipiosCarregamento.FirstOrDefault().Municipio.Longitude.Value : 0;
                }
                if (string.IsNullOrWhiteSpace(produtoPredominante.CEPDescarregamento) && (produtoPredominante.LatitudeDescarregamento == 0) && mdfe.MunicipiosDescarregamento != null && mdfe.MunicipiosDescarregamento.Count > 0)
                {
                    produtoPredominante.CEPDescarregamento = Utilidades.String.OnlyNumbers(mdfe.MunicipiosDescarregamento.FirstOrDefault().Municipio.CEP).PadLeft(8, '0').Left(8);
                    produtoPredominante.LatitudeDescarregamento = mdfe.MunicipiosDescarregamento.FirstOrDefault().Municipio.Latitude.HasValue ? mdfe.MunicipiosDescarregamento.FirstOrDefault().Municipio.Latitude.Value : 0;
                    produtoPredominante.LongitudeDescarregamento = mdfe.MunicipiosDescarregamento.FirstOrDefault().Municipio.Longitude.HasValue ? mdfe.MunicipiosDescarregamento.FirstOrDefault().Municipio.Longitude.Value : 0;
                }
            }

            //Enviar coordenadas do transportador quando ficou sem CEP ou Latitude
            if ((string.IsNullOrWhiteSpace(produtoPredominante.CEPCarregamento) || produtoPredominante.CEPCarregamento == "0" || produtoPredominante.CEPCarregamento == "00000000") && produtoPredominante.LatitudeCarregamento == 0)
            {
                produtoPredominante.CEPCarregamento = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                produtoPredominante.LatitudeCarregamento = mdfe.Empresa.Localidade.Latitude.HasValue ? mdfe.Empresa.Localidade.Latitude.Value : 0;
                produtoPredominante.LongitudeCarregamento = mdfe.Empresa.Localidade.Longitude.HasValue ? mdfe.Empresa.Localidade.Longitude.Value : 0;

            }

            if ((string.IsNullOrWhiteSpace(produtoPredominante.CEPDescarregamento) || produtoPredominante.CEPDescarregamento == "0" || produtoPredominante.CEPDescarregamento == "00000000") && produtoPredominante.LatitudeDescarregamento == 0)
            {
                produtoPredominante.CEPDescarregamento = Utilidades.String.OnlyNumbers(mdfe.Empresa.Localidade.CEP).PadLeft(8, '0').Left(8);
                produtoPredominante.LatitudeDescarregamento = mdfe.Empresa.Localidade.Latitude.HasValue ? mdfe.Empresa.Localidade.Latitude.Value : 0;
                produtoPredominante.LongitudeDescarregamento = mdfe.Empresa.Localidade.Longitude.HasValue ? mdfe.Empresa.Localidade.Longitude.Value : 0;
            }

            return produtoPredominante;
        }

        private async Task<List<ServicoMDFe.MDFeSeguro>> ObterSeguroAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.MDFeSeguro repositorioMDFeSeguro = new Repositorio.MDFeSeguro(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.MDFeSeguro> listaSeguro = await repositorioMDFeSeguro.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (listaSeguro != null && listaSeguro.Count > 0)
            {
                List<ServicoMDFe.MDFeSeguro> listaSeguroIntegrar = new List<ServicoMDFe.MDFeSeguro>();

                foreach (Dominio.Entidades.MDFeSeguro seguro in listaSeguro)
                {
                    listaSeguroIntegrar.Add(new ServicoMDFe.MDFeSeguro()
                    {
                        TipoResponsavel = seguro.TipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante ? "2" : "1",
                        CNPJCPFResponsavel = this.CNPJResponsavelSeguro(seguro.Responsavel, mdfe),
                        CNPJSeguradora = seguro.CNPJSeguradora,
                        NomeSeguradora = seguro.NomeSeguradora,
                        NumeroApolice = seguro.NumeroApolice,
                        NumeroAverbacao = this.NumeroAverbacaoSeguro(seguro.NumeroAverbacao, seguro.NumeroApolice, mdfe)
                    });
                }

                return listaSeguroIntegrar;
            }
            return null;
        }

        private async Task<string> ObterEmailsClientesAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, _cancellationToken);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTEs = await repositorioDocMDFe.BuscarCTesPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            string emails = string.Empty;

            for (var i = 0; i < listaCTEs.Count(); i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioCTe.BuscarPorCodigoAsync(listaCTEs[i].Codigo, false).ConfigureAwait(false);

                if (cte.OutrosTomador != null)
                {
                    if (cte.OutrosTomador.EmailStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.Email))
                        emails += string.Concat(cte.OutrosTomador.Email, ";");

                    if (cte.OutrosTomador.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.EmailContador))
                        emails += string.Concat(cte.OutrosTomador.EmailContador, ";");

                    if (cte.OutrosTomador.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.EmailContato))
                        emails += string.Concat(cte.OutrosTomador.EmailContato, ";");

                    if (cte.OutrosTomador.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.OutrosTomador.EmailTransportador))
                        emails += string.Concat(cte.OutrosTomador.EmailTransportador, ";");
                }
                if (cte.Remetente != null)
                {
                    if (cte.Remetente.EmailStatus && !string.IsNullOrWhiteSpace(cte.Remetente.Email))
                        emails += string.Concat(cte.Remetente.Email, ";");

                    if (cte.Remetente.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Remetente.EmailContador))
                        emails += string.Concat(cte.Remetente.EmailContador, ";");

                    if (cte.Remetente.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Remetente.EmailContato))
                        emails += string.Concat(cte.Remetente.EmailContato, ";");

                    if (cte.Remetente.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Remetente.EmailTransportador))
                        emails += string.Concat(cte.Remetente.EmailTransportador, ";");
                }
                if (cte.Expedidor != null)
                {
                    if (cte.Expedidor.EmailStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.Email))
                        emails += string.Concat(cte.Expedidor.Email, ";");

                    if (cte.Expedidor.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.EmailContador))
                        emails += string.Concat(cte.Expedidor.EmailContador, ";");

                    if (cte.Expedidor.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.EmailContato))
                        emails += string.Concat(cte.Expedidor.EmailContato, ";");

                    if (cte.Expedidor.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Expedidor.EmailTransportador))
                        emails += string.Concat(cte.Expedidor.EmailTransportador, ";");
                }
                if (cte.Destinatario != null)
                {
                    if (cte.Destinatario.EmailStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.Email))
                        emails += string.Concat(cte.Destinatario.Email, ";");

                    if (cte.Destinatario.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.EmailContador))
                        emails += string.Concat(cte.Destinatario.EmailContador, ";");

                    if (cte.Destinatario.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.EmailContato))
                        emails += string.Concat(cte.Destinatario.EmailContato, ";");

                    if (cte.Destinatario.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Destinatario.EmailTransportador))
                        emails += string.Concat(cte.Destinatario.EmailTransportador, ";");
                }
                if (cte.Recebedor != null)
                {
                    if (cte.Recebedor.EmailStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.Email))
                        emails += string.Concat(cte.Recebedor.Email, ";");

                    if (cte.Recebedor.EmailContadorStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.EmailContador))
                        emails += string.Concat(cte.Recebedor.EmailContador, ";");

                    if (cte.Recebedor.EmailContatoStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.EmailContato))
                        emails += string.Concat(cte.Recebedor.EmailContato, ";");

                    if (cte.Recebedor.EmailTransportadorStatus && !string.IsNullOrWhiteSpace(cte.Recebedor.EmailTransportador))
                        emails += string.Concat(cte.Recebedor.EmailTransportador, ";");
                }
            }

            return Utilidades.String.Left(emails, 1000);
        }

        private async Task<List<ServicoMDFe.MDFeContratante>> ObterContratanteAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.MDFeContratante repositorioMDFeContratante = new Repositorio.MDFeContratante(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.MDFeContratante> listaContratante = await repositorioMDFeContratante.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (listaContratante != null && listaContratante.Count > 0)
            {
                List<ServicoMDFe.MDFeContratante> listaContratanteIntegrar = new List<ServicoMDFe.MDFeContratante>();

                foreach (Dominio.Entidades.MDFeContratante contratante in listaContratante)
                {
                    listaContratanteIntegrar.Add(new ServicoMDFe.MDFeContratante()
                    {
                        CPFCNPJContratante = contratante.Contratante
                    });
                }

                return listaContratanteIntegrar;
            }
            return null;
        }

        private async Task<List<ServicoMDFe.MDFeCIOT>> ObterCIOTAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.MDFeCIOT repositorioMDFeCIOT = new Repositorio.MDFeCIOT(_unitOfWork);
            List<Dominio.Entidades.MDFeCIOT> listaCIOT = await repositorioMDFeCIOT.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (listaCIOT != null && listaCIOT.Count > 0)
            {
                List<ServicoMDFe.MDFeCIOT> listaCIOTIntegrar = new List<ServicoMDFe.MDFeCIOT>();

                foreach (Dominio.Entidades.MDFeCIOT ciot in listaCIOT)
                {
                    listaCIOTIntegrar.Add(new ServicoMDFe.MDFeCIOT()
                    {
                        CIOT = ciot.NumeroCIOT,
                        CNPJCPFResponsavel = ciot.Responsavel
                    });
                }

                return listaCIOTIntegrar;
            }
            return null;
        }

        private List<ServicoMDFe.Terminal> ObterTerminaisCarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe.TerminalCarregamento != null && mdfe.TerminalCarregamento.Count() > 0)
            {
                List<ServicoMDFe.Terminal> terminais = new List<ServicoMDFe.Terminal>();

                foreach (var terminal in mdfe.TerminalCarregamento)
                {
                    terminais.Add(new ServicoMDFe.Terminal()
                    {
                        CodigoTerminal = terminal.CodigoTerminal,
                        NomeTerminal = terminal.Descricao
                    });
                }

                return terminais;
            }

            return null;
        }

        private List<ServicoMDFe.Terminal> ObterTerminaisDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe.TerminalDescarregamento != null && mdfe.TerminalDescarregamento.Count() > 0)
            {
                List<ServicoMDFe.Terminal> terminais = new List<ServicoMDFe.Terminal>();

                foreach (var terminal in mdfe.TerminalDescarregamento)
                {
                    terminais.Add(new ServicoMDFe.Terminal()
                    {
                        CodigoTerminal = terminal.CodigoTerminal,
                        NomeTerminal = terminal.Descricao
                    });
                }

                return terminais;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.LacreMDFe>> ObterLacresAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.LacreMDFe repositorioLacre = new Repositorio.LacreMDFe(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.LacreMDFe> lacres = await repositorioLacre.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (lacres != null && lacres.Count() > 0)
            {
                List<ServicoMDFe.LacreMDFe> lacresIntegrar = new List<ServicoMDFe.LacreMDFe>();

                foreach (Dominio.Entidades.LacreMDFe lacre in lacres)
                {
                    lacresIntegrar.Add(new ServicoMDFe.LacreMDFe()
                    {
                        Numero = lacre.Numero
                    });
                }

                return lacresIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.MunicipioCarregamentoMDFe>> ObterMunicipiosCarregamentoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.MunicipioCarregamentoMDFe repositorioMunicipio = new Repositorio.MunicipioCarregamentoMDFe(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.MunicipioCarregamentoMDFe> municipios = await repositorioMunicipio.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (municipios != null && municipios.Count() > 0)
            {
                List<ServicoMDFe.MunicipioCarregamentoMDFe> municipiosIntegrar = new List<ServicoMDFe.MunicipioCarregamentoMDFe>();

                foreach (Dominio.Entidades.MunicipioCarregamentoMDFe municipio in municipios)
                {
                    municipiosIntegrar.Add(new ServicoMDFe.MunicipioCarregamentoMDFe()
                    {
                        CodigoMunicipio = municipio.Municipio.CodigoIBGE,
                        NomeMunicipio = municipio.Municipio.Descricao
                    });
                }

                return municipiosIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.MunicipioDescarregamentoMDFe>> ObterMunicipiosDescarregamentoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.MunicipioDescarregamentoMDFe repositorioMunicipio = new Repositorio.MunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.MunicipioDescarregamentoMDFe> municipios = await repositorioMunicipio.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (municipios != null && municipios.Count > 0)
            {
                List<ServicoMDFe.MunicipioDescarregamentoMDFe> municipiosIntegrar = new List<ServicoMDFe.MunicipioDescarregamentoMDFe>();

                foreach (Dominio.Entidades.MunicipioDescarregamentoMDFe municipio in municipios)
                {
                    municipiosIntegrar.Add(new ServicoMDFe.MunicipioDescarregamentoMDFe()
                    {
                        CodigoMunicipio = municipio.Municipio.CodigoIBGE,
                        NomeMunicipio = municipio.Municipio.Descricao,
                        Documentos = await ObterDocumentosAsync(municipio).ConfigureAwait(false)
                    });
                }

                return municipiosIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.DocumentoMDFe>> ObterDocumentosAsync(Dominio.Entidades.MunicipioDescarregamentoMDFe municipio)
        {
            Repositorio.NotaFiscalEletronicaMDFe repositorioNotaFiscalMDFe = new Repositorio.NotaFiscalEletronicaMDFe(_unitOfWork, _cancellationToken);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork, _cancellationToken);
            Repositorio.CTeMDFe repositorioCTeMDFe = new Repositorio.CTeMDFe(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos = await repositorioDocumento.BuscarPorMunicipioAsync(municipio.Codigo, _cancellationToken).ConfigureAwait(false);
            List<Dominio.Entidades.NotaFiscalEletronicaMDFe> notasFiscais = await repositorioNotaFiscalMDFe.BuscarPorMunicipioAsync(municipio.Codigo, _cancellationToken).ConfigureAwait(false);
            List<Dominio.Entidades.CTeMDFe> chavesCTes = await repositorioCTeMDFe.BuscarPorMunicipioAsync(municipio.Codigo, _cancellationToken).ConfigureAwait(false);

            List<ServicoMDFe.DocumentoMDFe> documentosIntegrar = new List<ServicoMDFe.DocumentoMDFe>();

            if (documentos != null && documentos.Count > 0)
            {
                int contatdorCTeTerceiro = 0;
                foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento in documentos)
                {
                    if (documento.CTe != null)
                    {
                        documentosIntegrar.Add(new ServicoMDFe.DocumentoMDFe()
                        {
                            ChaveCTe = documento.CTe.Chave,
                            CodigoBarrasCTe = documento.CTe.TipoEmissao == "5" ? documento.CTe.ChaveContingencia : String.Empty,
                            ProdutosPerigosos = await ObterProdutosPerigososAsync(documento).ConfigureAwait(false),
                            UnidadesTransporte = await ObterUnidadesTransporteAsync(documento).ConfigureAwait(false),
                        });
                    }
                    else if (documento.CTeTerceiro != null)
                    {
                        contatdorCTeTerceiro++;
                        documentosIntegrar.Add(new ServicoMDFe.DocumentoMDFe()
                        {
                            ChaveCTe = documento.CTeTerceiro.ChaveAcesso,
                            CodigoBarrasCTe = String.Empty
                        });

                        if (contatdorCTeTerceiro == 1000)
                            break;
                    }
                }
            }

            if (notasFiscais != null && notasFiscais.Count > 0)
            {
                foreach (Dominio.Entidades.NotaFiscalEletronicaMDFe notaFiscal in notasFiscais)
                {
                    documentosIntegrar.Add(new ServicoMDFe.DocumentoMDFe()
                    {
                        ChaveNFe = notaFiscal.Chave,
                        CodigoBarrasNFe = !string.IsNullOrWhiteSpace(notaFiscal.SegundoCodigoDeBarra) ? notaFiscal.SegundoCodigoDeBarra : string.Empty
                    });
                }
            }

            if (chavesCTes != null && chavesCTes.Count > 0)
            {
                foreach (Dominio.Entidades.CTeMDFe cte in chavesCTes)
                {
                    documentosIntegrar.Add(new ServicoMDFe.DocumentoMDFe()
                    {
                        ChaveCTe = cte.Chave
                    });
                }
            }

            return documentosIntegrar.Count > 0 ? documentosIntegrar : null;
        }

        private async Task<List<ServicoMDFe.DocumentoMDFeProdPerigosos>> ObterProdutosPerigososAsync(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repositorioProdPerigosos = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(_unitOfWork);
            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos> produtosPerigosos = await repositorioProdPerigosos.BuscarPorDocumentoAsync(documento.Codigo, _cancellationToken).ConfigureAwait(false);

            if (produtosPerigosos != null && produtosPerigosos.Count() > 0)
            {
                List<ServicoMDFe.DocumentoMDFeProdPerigosos> prodPerigosoIntegrar = new List<ServicoMDFe.DocumentoMDFeProdPerigosos>();

                foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos produto in produtosPerigosos)
                {
                    prodPerigosoIntegrar.Add(new ServicoMDFe.DocumentoMDFeProdPerigosos()
                    {
                        ClasseRisco = produto.ClasseRisco,
                        GrupoEmbalagem = produto.GrupoEmbalagem,
                        Nome = produto.Nome,
                        NumeroOnu = produto.NumeroONU,
                        QtdTipoVolumes = produto.QuantidadeTipoVolumes,
                        QtdTotalProduto = produto.QuantidadeTotalProduto
                    });
                }

                return prodPerigosoIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.UnidadeTransporteDocumentoMDFe>> ObterUnidadesTransporteAsync(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento)
        {

            if (documento.CTe != null && documento.CTe.Viagem != null && documento.CTe.Viagem.Navio != null)
            {
                List<ServicoMDFe.UnidadeTransporteDocumentoMDFe> unidadesIntegrar = new List<ServicoMDFe.UnidadeTransporteDocumentoMDFe>();

                unidadesIntegrar.Add(new ServicoMDFe.UnidadeTransporteDocumentoMDFe()
                {
                    Identificacao = documento.CTe?.Viagem?.Navio?.Irin ?? "",
                    QuantidadeRateada = 0,
                    Tipo = "3",
                    Lacres = ObterLacreMDFe(documento),
                    UnidadesCarga = await ObterUnidadesCargaAsync(documento).ConfigureAwait(false),
                });

                return unidadesIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.UnidadeCargaUnidadeTransporteMDFe>> ObterUnidadesCargaAsync(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento)
        {
            Repositorio.ContainerCTE repositorioContainerCTE = new Repositorio.ContainerCTE(_unitOfWork, _cancellationToken);

            if (documento.CTe != null)
            {
                List<Dominio.Entidades.ContainerCTE> listaContainers = await repositorioContainerCTE.BuscarPorCTeAsync(documento.CTe.Codigo, _cancellationToken).ConfigureAwait(false);

                if (listaContainers != null && listaContainers.Count > 0)
                {
                    List<ServicoMDFe.UnidadeCargaUnidadeTransporteMDFe> unidadesIntegrar = new List<ServicoMDFe.UnidadeCargaUnidadeTransporteMDFe>();

                    foreach (var container in listaContainers)
                    {
                        unidadesIntegrar.Add(new ServicoMDFe.UnidadeCargaUnidadeTransporteMDFe()
                        {
                            Identificacao = (Utilidades.String.RemoveDiacritics(container.Container?.Numero ?? container.Numero)?.Replace("-", "").Replace(" ", "") ?? "").Trim(),
                            Lacres = ObterLacreUnidadeCargaMDFe(container),
                            QuantidadeRateada = 0,
                            Tipo = "1"
                        });
                    }

                    return unidadesIntegrar;
                }
            }

            return null;
        }

        private List<ServicoMDFe.LacreUnidadeCargaMDFe> ObterLacreUnidadeCargaMDFe(Dominio.Entidades.ContainerCTE container)
        {
            if (container != null)
            {
                List<ServicoMDFe.LacreUnidadeCargaMDFe> lacresIntegrar = new List<ServicoMDFe.LacreUnidadeCargaMDFe>();

                if (!string.IsNullOrWhiteSpace(container.Lacre1))
                    lacresIntegrar.Add(new ServicoMDFe.LacreUnidadeCargaMDFe()
                    {
                        Numero = Utilidades.String.RemoveDiacritics(container.Lacre1)
                    });
                if (!string.IsNullOrWhiteSpace(container.Lacre2))
                    lacresIntegrar.Add(new ServicoMDFe.LacreUnidadeCargaMDFe()
                    {
                        Numero = Utilidades.String.RemoveDiacritics(container.Lacre2)
                    });
                if (!string.IsNullOrWhiteSpace(container.Lacre3))
                    lacresIntegrar.Add(new ServicoMDFe.LacreUnidadeCargaMDFe()
                    {
                        Numero = Utilidades.String.RemoveDiacritics(container.Lacre3)
                    });

                return lacresIntegrar;
            }

            return null;
        }

        private List<ServicoMDFe.LacreUnidadeTransporteDocumentoMDFe> ObterLacreMDFe(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento)
        {
            if (documento.CTe != null && documento.CTe.Viagem != null && documento.CTe.Viagem.Navio != null)
            {
                List<ServicoMDFe.LacreUnidadeTransporteDocumentoMDFe> lacresIntegrar = new List<ServicoMDFe.LacreUnidadeTransporteDocumentoMDFe>();

                lacresIntegrar.Add(new ServicoMDFe.LacreUnidadeTransporteDocumentoMDFe()
                {
                    Numero = documento.CTe?.Viagem?.Navio?.Irin
                });

                return lacresIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.PercursoMDFe>> ObterPercursosAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.PercursoMDFe repositorioPercurso = new Repositorio.PercursoMDFe(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.PercursoMDFe> percursos = await repositorioPercurso.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (percursos != null && percursos.Count > 0)
            {
                List<ServicoMDFe.PercursoMDFe> percursosIntegrar = new List<ServicoMDFe.PercursoMDFe>();

                foreach (Dominio.Entidades.PercursoMDFe percurso in percursos)
                {
                    percursosIntegrar.Add(new ServicoMDFe.PercursoMDFe()
                    {
                        UF = percurso.Estado.Sigla,
                        DataHoraPrevistaInicioViagem = percurso.DataInicioViagem.HasValue ? percurso.DataInicioViagem.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                    });
                }

                return percursosIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.ReboqueMDFe>> ObterReboquesAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.ReboqueMDFe repositorioReboque = new Repositorio.ReboqueMDFe(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.ReboqueMDFe> reboques = await repositorioReboque.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (reboques != null && reboques.Count > 0)
            {
                List<ServicoMDFe.ReboqueMDFe> reboquesIntegrar = new List<ServicoMDFe.ReboqueMDFe>();

                foreach (Dominio.Entidades.ReboqueMDFe reboque in reboques)
                {
                    reboquesIntegrar.Add(new ServicoMDFe.ReboqueMDFe()
                    {
                        CapacidadeKG = reboque.CapacidadeKG,
                        CapacidadeM3 = reboque.CapacidadeM3,
                        CodigoInterno = reboque.Codigo.ToString(),
                        Placa = reboque.Placa,
                        RENAVAM = !string.IsNullOrWhiteSpace(reboque.RENAVAM) && reboque.RENAVAM.Length > 11 ? Utilidades.String.Right(reboque.RENAVAM, 11) : reboque.RENAVAM,
                        RNTRCProprietario = reboque.RNTRC,
                        Tara = reboque.Tara,
                        CPFCNPJProprietario = reboque.CPFCNPJProprietario,
                        IEProprietario = reboque.IEProprietario,
                        NomeProprietario = reboque.NomeProprietario,
                        TipoCarroceria = reboque.TipoCarroceria,
                        TipoProprietario = reboque.TipoProprietario,
                        UF = reboque.UF != null ? reboque.UF.Sigla : string.Empty,
                        UFProprietario = reboque.UFProprietario != null ? reboque.UFProprietario.Sigla : string.Empty
                    });
                }

                return reboquesIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.ValePedagioMDFe>> ObterValesPedagioAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.ValePedagioMDFe repositorioValePedagio = new Repositorio.ValePedagioMDFe(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.ValePedagioMDFe> valesPedagio = await repositorioValePedagio.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (valesPedagio != null && valesPedagio.Count > 0)
            {
                List<ServicoMDFe.ValePedagioMDFe> valesPedagioIntegrar = new List<ServicoMDFe.ValePedagioMDFe>();

                foreach (Dominio.Entidades.ValePedagioMDFe valePedagio in valesPedagio)
                {
                    string categoriaCombinacaoVeicular = "02";
                    if (valePedagio.QuantidadeEixos == 2)
                        categoriaCombinacaoVeicular = "02";
                    else if (valePedagio.QuantidadeEixos == 3)
                        categoriaCombinacaoVeicular = "04";
                    else if (valePedagio.QuantidadeEixos == 4)
                        categoriaCombinacaoVeicular = "06";
                    else if (valePedagio.QuantidadeEixos == 5)
                        categoriaCombinacaoVeicular = "07";
                    else if (valePedagio.QuantidadeEixos == 6)
                        categoriaCombinacaoVeicular = "08";
                    else if (valePedagio.QuantidadeEixos == 7)
                        categoriaCombinacaoVeicular = "10";
                    else if (valePedagio.QuantidadeEixos == 8)
                        categoriaCombinacaoVeicular = "11";
                    else if (valePedagio.QuantidadeEixos == 9)
                        categoriaCombinacaoVeicular = "12";
                    else if (valePedagio.QuantidadeEixos == 10)
                        categoriaCombinacaoVeicular = "13";
                    else if (valePedagio.QuantidadeEixos > 10)
                        categoriaCombinacaoVeicular = "14";

                    string tipoValePedagio = "01";
                    if (valePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag)
                        tipoValePedagio = "01";
                    else if (valePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Cupom)
                        tipoValePedagio = "02";
                    else if (valePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Cartao)
                        tipoValePedagio = "03";

                    valesPedagioIntegrar.Add(new ServicoMDFe.ValePedagioMDFe()
                    {
                        CNPJFornecedor = valePedagio.CNPJFornecedor,
                        CNPJResponsavel = valePedagio.CNPJResponsavel,
                        NumeroComprovante = Utilidades.String.OnlyNumbers(valePedagio.NumeroComprovante),
                        CodigoAgendamentoPorto = valePedagio.CodigoAgendamentoPorto,
                        ValorValePedagio = valePedagio.ValorValePedagio,
                        CategoriaCombinacaoVeicular = categoriaCombinacaoVeicular,
                        TipoValePedagio = tipoValePedagio
                    });
                }

                return valesPedagioIntegrar;
            }

            return null;
        }

        private async Task<ServicoMDFe.VeiculoMDFe> ObterVeiculoAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.VeiculoMDFe repositorioVeiculo = new Repositorio.VeiculoMDFe(_unitOfWork, _cancellationToken);
            Dominio.Entidades.VeiculoMDFe veiculo = await repositorioVeiculo.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (veiculo != null)
            {
                ServicoMDFe.VeiculoMDFe veiculoIntegrar = new ServicoMDFe.VeiculoMDFe();

                veiculoIntegrar.CapacidadeKG = veiculo.CapacidadeKG;
                veiculoIntegrar.CapacidadeM3 = veiculo.CapacidadeM3;
                veiculoIntegrar.CIOT = mdfe.CIOT;
                veiculoIntegrar.CodigoInterno = veiculo.Codigo.ToString();
                veiculoIntegrar.Placa = veiculo.Placa;
                veiculoIntegrar.RENAVAM = !string.IsNullOrWhiteSpace(veiculo.RENAVAM) && veiculo.RENAVAM.Length > 11 ? Utilidades.String.Right(veiculo.RENAVAM, 11) : veiculo.RENAVAM;
                veiculoIntegrar.RNTRC = mdfe.RNTRC;
                veiculoIntegrar.RNTRCProprietario = veiculo.RNTRC;
                veiculoIntegrar.Tara = veiculo.Tara;
                veiculoIntegrar.CPFCNPJProprietario = veiculo.CPFCNPJProprietario;
                veiculoIntegrar.IEProprietario = veiculo.IEProprietario;
                veiculoIntegrar.NomeProprietario = veiculo.NomeProprietario;
                veiculoIntegrar.TipoCarroceria = veiculo.TipoCarroceria;
                veiculoIntegrar.TipoProprietario = veiculo.TipoProprietario;
                veiculoIntegrar.TipoRodado = veiculo.TipoRodado;
                veiculoIntegrar.UF = veiculo.UF != null ? veiculo.UF.Sigla : string.Empty;
                veiculoIntegrar.UFProprietario = veiculo.UFProprietario != null ? veiculo.UFProprietario.Sigla : string.Empty;

                veiculoIntegrar.Condutores = await ObterCondutoresAsync(mdfe).ConfigureAwait(false);

                return veiculoIntegrar;
            }

            return null;
        }

        private async Task<List<ServicoMDFe.CondutorMDFe>> ObterCondutoresAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.MotoristaMDFe repositorioMotorista = new Repositorio.MotoristaMDFe(_unitOfWork);
            List<Dominio.Entidades.MotoristaMDFe> motoristas = await repositorioMotorista.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken).ConfigureAwait(false);

            if (motoristas != null && motoristas.Count > 0)
            {
                List<ServicoMDFe.CondutorMDFe> motoristasIntegrar = new List<ServicoMDFe.CondutorMDFe>();

                foreach (Dominio.Entidades.MotoristaMDFe motorista in motoristas)
                {
                    motoristasIntegrar.Add(new ServicoMDFe.CondutorMDFe()
                    {
                        CPF = motorista.CPF,
                        Nome = motorista.Nome
                    });
                }

                return motoristasIntegrar;
            }

            return null;
        }

        public ServicoMDFe.Empresa ObterEmpresaEmitente(Dominio.Entidades.Empresa empresa)
        {
            ServicoMDFe.Empresa empresaEmitente = new ServicoMDFe.Empresa();

            empresaEmitente.Bairro = Utilidades.String.Left(empresa.Bairro, 60);
            empresaEmitente.Cep = Utilidades.String.OnlyNumbers(empresa.CEP);
            empresaEmitente.Cidade = Utilidades.String.Left(empresa.Localidade.Descricao, 60);
            empresaEmitente.CNPJ = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            empresaEmitente.CodigoCidadeIBGE = empresa.Localidade.CodigoIBGE;
            empresaEmitente.Complemento = Utilidades.String.Left(empresa.Complemento, 60);
            empresaEmitente.EmailContador = empresa.EmailContador;
            empresaEmitente.EmailEmitente = empresa.Email;
            empresaEmitente.EnviaEmailContador = empresa.StatusEmail;
            empresaEmitente.EnviaEmailEmitente = empresa.StatusEmailContador;
            empresaEmitente.IE = string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) ? "ISENTO" : empresa.InscricaoEstadual;
            empresaEmitente.Logradouro = Utilidades.String.Left(empresa.Endereco, 255);
            empresaEmitente.NomeContador = Utilidades.String.Left(empresa.NomeContador, 60);
            empresaEmitente.NomeFantasia = Utilidades.String.Left(empresa.NomeFantasia, 60);
            empresaEmitente.NomeRazao = Utilidades.String.Left(empresa.RazaoSocial, 60);
            empresaEmitente.Numero = Utilidades.String.Left(empresa.Numero, 60);
            empresaEmitente.Status = empresa.Status;
            empresaEmitente.Telefone = Utilidades.String.OnlyNumbers(empresa.Telefone);
            empresaEmitente.TelefoneContador = Utilidades.String.OnlyNumbers(empresa.TelefoneContador);
            empresaEmitente.UF = empresa.Localidade.Estado.Sigla;

            return empresaEmitente;
        }

        private ServicoMDFe.TipoModal ObterModal(Dominio.Entidades.ModalTransporte modal)
        {
            int numeroModal = int.Parse(modal.Numero);

            if (numeroModal == 1)
                return ServicoMDFe.TipoModal.Rodoviario;
            else if (numeroModal == 2)
                return ServicoMDFe.TipoModal.Aereo;
            else if (numeroModal == 3)
                return ServicoMDFe.TipoModal.Aquaviario;
            else if (numeroModal == 4)
                return ServicoMDFe.TipoModal.Ferroviario;
            else
                return ServicoMDFe.TipoModal.Rodoviario;
        }

        public void ObterESalvarDAMDFE(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, ServicoMDFe.RetornoMDFe retorno = null, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
            {
                if (mdfe != null)
                {
                    if (retorno == null)
                    {
                        ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                        retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);
                    }

                    if (!string.IsNullOrWhiteSpace(retorno.DAMDFE))
                    {
                        string texto = Encoding.UTF8.GetString(Convert.FromBase64String(retorno.DAMDFE));
                        var windows1252 = Encoding.GetEncoding("Windows-1252");
                        byte[] decodedData = windows1252.GetBytes(texto);

                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";

                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                    }
                }
            }
        }

        public void ObterESalvarDAMDFEOracle(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.WebService.MDFe.MDFeOracle retorno, Repositorio.UnitOfWork unidadeDeTrabalho, byte[] damdfe = null)
        {
            if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                if (mdfe != null)
                {
                    if ((!string.IsNullOrWhiteSpace(retorno.PDFDAMDFE) && retorno.PDFDAMDFE != "0") || damdfe != null)
                    {
                        byte[] decodedData = null;
                        if (damdfe != null)
                            decodedData = damdfe;
                        else
                        {
                            string texto = Encoding.UTF8.GetString(Convert.FromBase64String(retorno.PDFDAMDFE));
                            var windows1252 = Encoding.GetEncoding("Windows-1252");
                            decodedData = windows1252.GetBytes(texto);
                        }

                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";

                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                    }
                }
            }
        }

        public void ObterESalvarDAMDFEContingencia(int codigoMDFe, int codigoEmpresa, ServicoMDFe.RetornoMDFe retorno = null, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

                if (mdfe != null)
                {
                    if (retorno == null)
                    {
                        ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);
                        retorno = svcMDFe.ConsultarMDFePorCodigo(mdfe.CodigoIntegradorAutorizacao);
                    }

                    if (!string.IsNullOrWhiteSpace(retorno.DAMDFE_CONTINGENCIA))
                    {
                        string texto = Encoding.UTF8.GetString(Convert.FromBase64String(retorno.DAMDFE_CONTINGENCIA));
                        var windows1252 = Encoding.GetEncoding("Windows-1252");
                        byte[] decodedData = windows1252.GetBytes(texto);

                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Numero.ToString()) + "_CONTINGENCIA_.pdf";

                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                    }
                }
            }
        }

        public bool AdicionarMDFeNaFilaDeConsulta(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (mdfe.SistemaEmissor != TipoEmissorDocumento.Integrador)
                    return true;

                string configWebServiceConsultaCTe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().WebServiceConsultaCTe;
                if (configWebServiceConsultaCTe == null || configWebServiceConsultaCTe == "")
                    configWebServiceConsultaCTe = "http://localhost/CTe/";

                string postData = "CodigoMDFe=" + mdfe.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(configWebServiceConsultaCTe, "IntegracaoMDFe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                var retorno = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private void GerarProdutosPerigosos(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (documento != null && documento.CTe != null)
            {
                Repositorio.ProdutoPerigosoCTE repProdutoPerigosoCTE = new Repositorio.ProdutoPerigosoCTE(unidadeDeTrabalho);
                List<Dominio.Entidades.ProdutoPerigosoCTE> listaProdutoPerigosoCTE = repProdutoPerigosoCTE.BuscarPorCTe(documento.CTe.Codigo);
                if (listaProdutoPerigosoCTE != null && listaProdutoPerigosoCTE.Count() > 0)
                {
                    Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repProdPerigosoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(unidadeDeTrabalho);
                    foreach (Dominio.Entidades.ProdutoPerigosoCTE prodPerigosoCTe in listaProdutoPerigosoCTE)
                    {
                        Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos prodPerigosoMDFe = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos();
                        prodPerigosoMDFe.DocumentoMunicipioDescarregamentoMDFe = documento;
                        prodPerigosoMDFe.ClasseRisco = prodPerigosoCTe.ClasseRisco;
                        prodPerigosoMDFe.GrupoEmbalagem = prodPerigosoCTe.Grupo;
                        prodPerigosoMDFe.Nome = prodPerigosoCTe.NomeApropriado;
                        prodPerigosoMDFe.NumeroONU = prodPerigosoCTe.NumeroONU.ToString();
                        prodPerigosoMDFe.QuantidadeTipoVolumes = prodPerigosoCTe.Volumes;
                        prodPerigosoMDFe.QuantidadeTotalProduto = prodPerigosoCTe.Quantidade;

                        repProdPerigosoMDFe.Inserir(prodPerigosoMDFe);
                    }
                }
            }
        }

        public void GerarSeguro(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null && seguros != null && seguros.Count > 0)
            {
                bool configProcessarCTeMultiCTe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().ProcessarCTeMultiCTe.HasValue ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().ProcessarCTeMultiCTe.Value : false;
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeSeguro> seguroMDFe = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);

                if (seguroMDFe.Count == 0)
                {
                    int totalSegurosInseridos = 0;

                    foreach (Dominio.ObjetosDeValor.SeguroMDFeIntegracao seguro in seguros)
                    {
                        if (configProcessarCTeMultiCTe && string.IsNullOrEmpty(seguro.NumeroApolice) && string.IsNullOrEmpty(seguro.CNPJSeguradora))
                            continue;

                        totalSegurosInseridos++;

                        Dominio.Enumeradores.TipoResponsavelSeguroMDFe tipoResponsavel = seguro.Responsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                                                                         seguro.Responsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente :
                                                                                         mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                                                         mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                                                                         mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                                                         mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                        string cnpjResponsavel = !string.IsNullOrWhiteSpace(seguro.CNPJCPFResponsavel) ? seguro.CNPJCPFResponsavel :
                                                 tipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ :
                                                 ctes != null ? ctes.FirstOrDefault().Tomador.CPF_CNPJ : mdfe.Empresa.CNPJ;

                        string cnpjSeguradora = !string.IsNullOrWhiteSpace(seguro.CNPJSeguradora) ? seguro.CNPJSeguradora :
                                                        mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                        string nomeSeguradora = !string.IsNullOrWhiteSpace(seguro.NomeSeguradora) ? seguro.NomeSeguradora :
                                                       mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                       mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                       mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                        string numeroApolice = !string.IsNullOrWhiteSpace(seguro.NumeroApolice) ? seguro.NumeroApolice :
                                               mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                               mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                        if (!string.IsNullOrWhiteSpace(seguro.NumeroAverbacao) || ctes == null || ctes.Count == 0)
                        {
                            Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();
                            mdfeSeguro.MDFe = mdfe;
                            mdfeSeguro.TipoResponsavel = tipoResponsavel;
                            mdfeSeguro.Responsavel = cnpjResponsavel;
                            mdfeSeguro.CNPJSeguradora = cnpjSeguradora;
                            mdfeSeguro.NomeSeguradora = Utilidades.String.Left(nomeSeguradora, 30);
                            mdfeSeguro.NumeroApolice = numeroApolice;
                            mdfeSeguro.NumeroAverbacao = seguro.NumeroAverbacao;
                            repMDFeSeguro.Inserir(mdfeSeguro);
                        }
                        else if (!string.IsNullOrWhiteSpace(numeroApolice))
                        {
                            for (var i = 0; i < ctes.Count; i++)
                            {
                                string numeroAverbacao = this.BuscarAverbacaoCTe(ctes[i].Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho);
                                if (string.IsNullOrWhiteSpace(numeroAverbacao))
                                    numeroAverbacao = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                                      mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                                      mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(numeroApolice) ? numeroApolice : string.Empty;

                                Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();
                                mdfeSeguro.MDFe = mdfe;
                                mdfeSeguro.TipoResponsavel = tipoResponsavel;
                                mdfeSeguro.Responsavel = cnpjResponsavel;
                                mdfeSeguro.CNPJSeguradora = cnpjSeguradora;
                                mdfeSeguro.NomeSeguradora = Utilidades.String.Left(nomeSeguradora, 30);
                                mdfeSeguro.NumeroApolice = numeroApolice;
                                mdfeSeguro.NumeroAverbacao = numeroAverbacao;
                                repMDFeSeguro.Inserir(mdfeSeguro);
                            }
                        }

                        if (totalSegurosInseridos >= 500)  //SEFAZ não suporta mais de 500kb de arquivo, por isso foi limitada a quantidade de seguros enviada no XML
                            break;
                    }
                }
            }
        }

        public void GerarProdutoPredominante(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe == null)
                return;

            if (produtoPredominante != null)
            {
                mdfe.ProdutoPredominanteDescricao = string.IsNullOrWhiteSpace(produtoPredominante.DescProduto) ? "DIVERSOS" : produtoPredominante.DescProduto.Left(120);
                mdfe.ProdutoPredominanteNCM = produtoPredominante.NCM.Left(8);
                mdfe.ProdutoPredominanteCEAN = produtoPredominante.CEAN.Left(14);

                if (int.TryParse(produtoPredominante.TipoCarga, out int tipo) && Enum.IsDefined(typeof(TipoCargaMDFe), tipo))
                    mdfe.TipoCargaMDFe = (TipoCargaMDFe)tipo;
                else
                    mdfe.TipoCargaMDFe = TipoCargaMDFe.CargaGeral;
            }
            else
            {
                if (ctes.Any())
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes.FirstOrDefault();
                    mdfe.ProdutoPredominanteDescricao = cte.ProdutoPredominante.Left(120);
                    mdfe.ProdutoPredominanteNCM = cte.ProdutoPredominanteNCM.Left(8);
                    mdfe.ProdutoPredominanteCEAN = cte.ProdutoPredominanteCEAN.Left(14);
                    mdfe.TipoCargaMDFe = TipoCargaMDFe.CargaGeral;
                }
            }
        }

        public void GerarSeguro(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null)
            {
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeSeguro> seguroMDFe = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);

                if (seguroMDFe.Count == 0)
                {
                    int totalSegurosInseridos = 0;

                    if (nfesGlobalizadas != null && nfesGlobalizadas.Count > 0 && (seguros == null || string.IsNullOrWhiteSpace(seguros.FirstOrDefault().NumeroAverbacao)))
                    {
                        Repositorio.AverbacaoNFe repAverbacaoNFe = new Repositorio.AverbacaoNFe(unidadeDeTrabalho);

                        for (var i = 0; i < nfesGlobalizadas.Count; i++)
                        {
                            Dominio.Entidades.AverbacaoNFe averbacaoNFe = repAverbacaoNFe.BuscarPorChaveNFe(nfesGlobalizadas[i].ChaveNFe);

                            if (averbacaoNFe != null)
                            {
                                Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();
                                mdfeSeguro.MDFe = mdfe;
                                mdfeSeguro.TipoResponsavel = Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante;
                                mdfeSeguro.Responsavel = seguros != null && seguros.Count > 0 && !string.IsNullOrWhiteSpace(seguros.FirstOrDefault().CNPJCPFResponsavel) ? seguros.FirstOrDefault().CNPJCPFResponsavel : Utilidades.Chave.ObterCNPJEmitente(nfesGlobalizadas[i].ChaveNFe);
                                mdfeSeguro.CNPJSeguradora = seguros != null && seguros.Count > 0 && !string.IsNullOrWhiteSpace(seguros.FirstOrDefault().CNPJSeguradora) ? seguros.FirstOrDefault().CNPJSeguradora : mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro;
                                mdfeSeguro.NomeSeguradora = seguros != null && seguros.Count > 0 && !string.IsNullOrWhiteSpace(seguros.FirstOrDefault().NomeSeguradora) ? seguros.FirstOrDefault().NomeSeguradora : Utilidades.String.Left(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro, 30);
                                mdfeSeguro.NumeroApolice = seguros != null && seguros.Count > 0 && !string.IsNullOrWhiteSpace(seguros.FirstOrDefault().NumeroApolice) ? seguros.FirstOrDefault().NumeroApolice : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro;
                                mdfeSeguro.NumeroAverbacao = averbacaoNFe.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso ? averbacaoNFe.Averbacao : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro;
                                repMDFeSeguro.Inserir(mdfeSeguro);
                            }

                            if (totalSegurosInseridos >= 500)  //SEFAZ não suporta mais de 500kb de arquivo, por isso foi limitada a quantidade de seguros enviada no XML
                                break;
                        }
                    }
                    else if (seguros != null && seguros.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.SeguroMDFeIntegracao seguro in seguros)
                        {
                            totalSegurosInseridos++;

                            Dominio.Enumeradores.TipoResponsavelSeguroMDFe tipoResponsavel = seguro.Responsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                                                                             seguro.Responsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente :
                                                                                             mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                                                             mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                                                                             mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                                                             mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                            string cnpjResponsavel = !string.IsNullOrWhiteSpace(seguro.CNPJCPFResponsavel) ? seguro.CNPJCPFResponsavel :
                                                     tipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : mdfe.Empresa.CNPJ;

                            string cnpjSeguradora = !string.IsNullOrWhiteSpace(seguro.CNPJSeguradora) ? seguro.CNPJSeguradora :
                                                            mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                            string nomeSeguradora = !string.IsNullOrWhiteSpace(seguro.NomeSeguradora) ? seguro.NomeSeguradora :
                                                           mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                           mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                           mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                            string numeroApolice = !string.IsNullOrWhiteSpace(seguro.NumeroApolice) ? seguro.NumeroApolice :
                                                   mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                                   mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;


                            Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();
                            mdfeSeguro.MDFe = mdfe;
                            mdfeSeguro.TipoResponsavel = tipoResponsavel;
                            mdfeSeguro.Responsavel = cnpjResponsavel;
                            mdfeSeguro.CNPJSeguradora = cnpjSeguradora;
                            mdfeSeguro.NomeSeguradora = Utilidades.String.Left(nomeSeguradora, 30);
                            mdfeSeguro.NumeroApolice = numeroApolice;
                            mdfeSeguro.NumeroAverbacao = seguro.NumeroAverbacao;
                            repMDFeSeguro.Inserir(mdfeSeguro);

                            if (totalSegurosInseridos >= 500)  //SEFAZ não suporta mais de 500kb de arquivo, por isso foi limitada a quantidade de seguros enviada no XML
                                break;
                        }
                    }
                }
            }
        }

        public void GerarSeguroPorCTes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            bool inseriuSeguro = false;
            if (mdfe != null && ctes != null && ctes.Count > 0)
            {
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeSeguro> seguroMDFe = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);

                if (seguroMDFe.Count == 0)
                {
                    int totalCTes = ctes.Count;

                    for (var i = 0; i < totalCTes; i++)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                        if (cte.Seguros != null && cte.Seguros.Count() > 0)
                        {
                            foreach (Dominio.Entidades.SeguroCTE seguro in cte.Seguros)
                            {
                                Dominio.Enumeradores.TipoResponsavelSeguroMDFe tipoResponsavel = seguro.Tipo == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante;
                                string cnpjResponsavel = tipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : seguro.CTE.Tomador.CPF_CNPJ;
                                string numeroAverbacao = !string.IsNullOrWhiteSpace(seguro.NumeroAverbacao) ? seguro.NumeroAverbacao : this.BuscarAverbacaoCTe(seguro.CTE.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho);
                                string cnpjSeguradora = !string.IsNullOrWhiteSpace(seguro.CNPJSeguradora) ? seguro.CNPJSeguradora :
                                                        mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;
                                string nomeSeguradora = !string.IsNullOrWhiteSpace(seguro.NomeSeguradora) ? seguro.NomeSeguradora :
                                                        mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;
                                string numeroApolice = !string.IsNullOrWhiteSpace(seguro.NumeroApolice) ? seguro.NumeroApolice :
                                                       mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                                       mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;
                                numeroAverbacao = !string.IsNullOrWhiteSpace(numeroAverbacao) ? numeroAverbacao :
                                                  mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                                  mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                                  mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(numeroApolice) ? numeroApolice : string.Empty;

                                if (!repMDFeSeguro.ValidaSeguroJaInserido(mdfe.Codigo, tipoResponsavel, cnpjResponsavel, cnpjSeguradora, nomeSeguradora, numeroApolice, numeroAverbacao))
                                {
                                    Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();
                                    mdfeSeguro.MDFe = mdfe;
                                    mdfeSeguro.TipoResponsavel = tipoResponsavel;
                                    mdfeSeguro.Responsavel = cnpjResponsavel;
                                    mdfeSeguro.CNPJSeguradora = cnpjSeguradora;
                                    mdfeSeguro.NomeSeguradora = nomeSeguradora;
                                    mdfeSeguro.NumeroApolice = numeroApolice;
                                    mdfeSeguro.NumeroAverbacao = numeroAverbacao;

                                    repMDFeSeguro.Inserir(mdfeSeguro);

                                    inseriuSeguro = true;
                                }
                            }
                        }
                    }
                }
                else
                    inseriuSeguro = true;
            }

            if (mdfe != null && !inseriuSeguro && mdfe.TipoEmitente != Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte)
            {
                //Se não for inserido nenhum seguro primeiro busca configuração da empresa e depois da empresa pai
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();

                mdfeSeguro.MDFe = mdfe;
                mdfeSeguro.TipoResponsavel = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                             mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                             mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                             mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                mdfeSeguro.Responsavel = mdfeSeguro.TipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : ctes.Count > 0 ? ctes.FirstOrDefault().Tomador.CPF_CNPJ : mdfe.Empresa.CNPJ;

                mdfeSeguro.CNPJSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                mdfeSeguro.NomeSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                mdfeSeguro.NumeroApolice = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                           mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                mdfeSeguro.NumeroAverbacao = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                             mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                             mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(mdfeSeguro.NumeroApolice) ? mdfeSeguro.NumeroApolice : string.Empty;

                repMDFeSeguro.Inserir(mdfeSeguro);
            }
        }

        public void GerarSeguroPorCTes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            bool inseriuSeguro = false;
            if (mdfe != null)
            {
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repDocumentosMDFe.BuscarCTesPorMDFe(mdfe.Codigo);

                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeSeguro> seguroMDFe = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);

                if (seguroMDFe.Count == 0)
                {
                    int totalCTes = ctes?.Count ?? 0;

                    if (ctes != null && ctes.Count > 0)
                    {
                        for (var i = 0; i < totalCTes; i++)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                            if (ctes[i].Seguros != null && ctes[i].Seguros.Count > 0)
                            {
                                foreach (Dominio.Entidades.SeguroCTE seguro in ctes[i].Seguros)
                                {
                                    Dominio.Enumeradores.TipoResponsavelSeguroMDFe tipoResponsavel = seguro.Tipo == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante;

                                    string cnpjResponsavel = tipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : cte.Tomador.CPF_CNPJ;

                                    string numeroAverbacao = !string.IsNullOrWhiteSpace(seguro.NumeroAverbacao) ? seguro.NumeroAverbacao : this.BuscarAverbacaoCTe(cte.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho);

                                    string cnpjSeguradora = !string.IsNullOrWhiteSpace(seguro.CNPJSeguradora) ? seguro.CNPJSeguradora :
                                                            mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                            mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                            mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                                    string nomeSeguradora = !string.IsNullOrWhiteSpace(seguro.NomeSeguradora) ? seguro.NomeSeguradora :
                                                            mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                            mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                            mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                                    string numeroApolice = !string.IsNullOrWhiteSpace(seguro.NumeroApolice) ? seguro.NumeroApolice :
                                                           mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                                           mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                                    numeroAverbacao = !string.IsNullOrWhiteSpace(numeroAverbacao) ? numeroAverbacao :
                                                      mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                                      mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                                      mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(numeroApolice) ? numeroApolice : string.Empty;

                                    if (!repMDFeSeguro.ValidaSeguroJaInserido(mdfe.Codigo, tipoResponsavel, cnpjResponsavel, cnpjSeguradora, nomeSeguradora, numeroApolice, numeroAverbacao))
                                    {
                                        Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();
                                        mdfeSeguro.MDFe = mdfe;
                                        mdfeSeguro.TipoResponsavel = tipoResponsavel;
                                        mdfeSeguro.Responsavel = cnpjResponsavel;
                                        mdfeSeguro.CNPJSeguradora = cnpjSeguradora;
                                        mdfeSeguro.NomeSeguradora = nomeSeguradora;
                                        mdfeSeguro.NumeroApolice = numeroApolice;
                                        mdfeSeguro.NumeroAverbacao = numeroAverbacao;

                                        repMDFeSeguro.Inserir(mdfeSeguro);

                                        inseriuSeguro = true;

                                        if (totalCTes > 600)
                                            return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    inseriuSeguro = true;

                if (mdfe != null && !inseriuSeguro && mdfe.TipoEmitente != Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte)
                {
                    //Se não for inserido nenhum seguro primeiro busca configuração da empresa e depois da empresa pai
                    Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();

                    mdfeSeguro.MDFe = mdfe;
                    mdfeSeguro.TipoResponsavel = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                 mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                                 mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                 mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                    mdfeSeguro.Responsavel = mdfeSeguro.TipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : ctes.Count > 0 ? ctes.FirstOrDefault().Tomador.CPF_CNPJ : mdfe.Empresa.CNPJ;

                    mdfeSeguro.CNPJSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                    mdfeSeguro.NomeSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                    mdfeSeguro.NumeroApolice = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                               mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                    mdfeSeguro.NumeroAverbacao = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                                 mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                                 mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(mdfeSeguro.NumeroApolice) ? mdfeSeguro.NumeroApolice : string.Empty;

                    repMDFeSeguro.Inserir(mdfeSeguro);
                }
            }
        }

        public void GerarSeguroPorEmpresa(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null)
            {
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);

                Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();

                mdfeSeguro.MDFe = mdfe;
                mdfeSeguro.TipoResponsavel = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                             mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                             mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                             mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                mdfeSeguro.Responsavel = mdfe.Empresa.CNPJ;

                mdfeSeguro.CNPJSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                            mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                            mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                mdfeSeguro.NomeSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                            mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                            mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                mdfeSeguro.NumeroApolice = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                           mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                mdfeSeguro.NumeroAverbacao = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                             mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                             mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(mdfeSeguro.NumeroApolice) ? mdfeSeguro.NumeroApolice : string.Empty;

                repMDFeSeguro.Inserir(mdfeSeguro);
            }
        }

        public void GerarContratantesPorCTes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null && ctes != null && ctes.Count > 0)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

                List<Dominio.Entidades.ParticipanteCTe> tomadoresCTe = repMDFe.BuscarTomadoresCTes(mdfe.Codigo);

                bool possuiTomadorExterior = repMDFe.ExisteTomadorExterior(mdfe.Codigo);

                if (tomadoresCTe != null && tomadoresCTe.Count > 0)
                {
                    List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome> contratantesBanco = repMDFeContratante.BuscarPorMDFeGroupByCnpjNome(mdfe.Codigo);
                    tomadoresCTe = tomadoresCTe.Where(c => !contratantesBanco.Any(cb => cb.CnpjContratante == c.CPF_CNPJ)).ToList();

                    foreach (var tomador in tomadoresCTe)
                    {
                        Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                        {
                            MDFe = mdfe,
                            Contratante = tomador.CPF_CNPJ,
                            NomeContratante = tomador.Nome.Left(60)
                        };

                        repMDFeContratante.Inserir(mdfeContratante);
                    }
                }

                if (possuiTomadorExterior)
                {
                    Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                    {
                        MDFe = mdfe,
                        Contratante = mdfe.Empresa.CNPJ,
                        NomeContratante = mdfe.Empresa.RazaoSocial.Left(60)
                    };

                    repMDFeContratante.Inserir(mdfeContratante);
                }
            }
        }

        public void GerarContratantesNFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe == null)
                return;

            if (mdfe.TipoEmitente != Dominio.Enumeradores.TipoEmitenteMDFe.TransporteCTeGlobalizado)
                return;

            List<Dominio.Entidades.MDFeContratante> contratantesBanco = new Repositorio.MDFeContratante(unidadeDeTrabalho).BuscarPorMDFeCnpj(mdfe.Codigo, mdfe.Empresa.CNPJ);
            if (contratantesBanco.Any())
                return;

            Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

            Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
            {
                MDFe = mdfe,
                Contratante = mdfe.Empresa.CNPJ,
                NomeContratante = mdfe.Empresa.RazaoSocial.Left(60)
            };

            repMDFeContratante.Inserir(mdfeContratante);
        }

        public void GerarContratantesPorCTes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null)
            {
                Repositorio.MDFeContratante repMDFeContratanteo = new Repositorio.MDFeContratante(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeContratante> contratantesMDFe = repMDFeContratanteo.BuscarPorMDFe(mdfe.Codigo);

                if (contratantesMDFe.Count == 0)
                {
                    Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repDocumentosMDFe.BuscarCTesPorMDFe(mdfe.Codigo);

                    if (ctes != null && ctes.Count > 0)
                    {
                        List<Dominio.Entidades.ParticipanteCTe> listaContratantes = ctes.Select(o => o.Tomador).Distinct().ToList();
                        if (listaContratantes != null && listaContratantes.Count() > 0)
                        {
                            Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

                            bool adicionouEmpresa = false;

                            List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome> contratantesBanco = repMDFeContratante.BuscarPorMDFeGroupByCnpjNome(mdfe.Codigo);
                            listaContratantes = listaContratantes.Where(c => !contratantesBanco.Any(cb => cb.CnpjContratante == c.CPF_CNPJ && !c.Exterior)).ToList();

                            foreach (Dominio.Entidades.ParticipanteCTe contratante in listaContratantes)
                            {
                                if (contratante.Exterior)
                                {
                                    if (!adicionouEmpresa)
                                    {
                                        Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                                        {
                                            MDFe = mdfe,
                                            Contratante = mdfe.Empresa.CNPJ,
                                            NomeContratante = mdfe.Empresa.RazaoSocial.Left(60)
                                        };

                                        repMDFeContratante.Inserir(mdfeContratante);

                                        adicionouEmpresa = true;
                                    }
                                }
                                else
                                {
                                    Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                                    {
                                        MDFe = mdfe,
                                        Contratante = contratante.CPF_CNPJ,
                                        NomeContratante = contratante.Nome.Left(60)
                                    };

                                    repMDFeContratante.Inserir(mdfeContratante);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GerarContratantesPorCTesTerceiro(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null)
            {
                Repositorio.MDFeContratante repMDFeContratanteo = new Repositorio.MDFeContratante(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeContratante> contratantesMDFe = repMDFeContratanteo.BuscarPorMDFe(mdfe.Codigo);

                if (contratantesMDFe.Count == 0)
                {
                    Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                    if (ctes != null && ctes.Count > 0)
                    {
                        List<Dominio.Entidades.ParticipanteCTe> listaContratantes = ctes.Select(o => o.Tomador).Distinct().ToList();
                        if (listaContratantes != null && listaContratantes.Count > 0)
                        {
                            Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

                            bool adicionouEmpresa = false;

                            List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome> contratantesBanco = repMDFeContratante.BuscarPorMDFeGroupByCnpjNome(mdfe.Codigo);
                            listaContratantes = listaContratantes.Where(c => !contratantesBanco.Any(cb => cb.CnpjContratante == c.CPF_CNPJ && !c.Exterior)).ToList();

                            foreach (Dominio.Entidades.ParticipanteCTe contratante in listaContratantes)
                            {
                                if (contratante.Exterior)
                                {
                                    if (!adicionouEmpresa)
                                    {
                                        Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                                        {
                                            MDFe = mdfe,
                                            Contratante = mdfe.Empresa.CNPJ,
                                            NomeContratante = mdfe.Empresa.RazaoSocial.Left(60)
                                        };

                                        repMDFeContratante.Inserir(mdfeContratante);

                                        adicionouEmpresa = true;
                                    }
                                }
                                else
                                {
                                    Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                                    {
                                        MDFe = mdfe,
                                        Contratante = contratante.CPF_CNPJ,
                                        NomeContratante = contratante.Nome.Left(60)
                                    };

                                    repMDFeContratante.Inserir(mdfeContratante);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GerarContratantesPorNFeGlobalizada(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe != null && nfesGlobalizadas != null && nfesGlobalizadas.Count > 0)
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

                var cnpjsEmissores = (from obj in nfesGlobalizadas select Utilidades.Chave.ObterCNPJEmitente(obj.ChaveNFe)).Distinct().ToList();

                foreach (var cnpj in cnpjsEmissores)
                {
                    if (!string.IsNullOrWhiteSpace(cnpj))
                    {
                        Dominio.Entidades.MDFeContratante mdfeContratante = new Dominio.Entidades.MDFeContratante
                        {
                            MDFe = mdfe,
                            Contratante = cnpj,
                            NomeContratante = cnpj
                        };

                        repMDFeContratante.Inserir(mdfeContratante);
                    }
                }
            }
        }

        public void GerarCIOTPorCTes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (mdfe != null)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                    Repositorio.MDFeCIOT repMDFeCIOTe = new Repositorio.MDFeCIOT(unidadeDeTrabalho);

                    List<Dominio.Entidades.MDFeCIOT> ciot = repMDFeCIOTe.BuscarPorMDFe(mdfe.Codigo);

                    if (ciot.Count == 0)
                    {
                        List<string> listaCiots = ctes.Select(o => o.CIOT).Distinct().ToList();

                        if (listaCiots != null && listaCiots.Count() > 0)
                        {
                            foreach (string ciotCTe in listaCiots)
                            {
                                if (!string.IsNullOrWhiteSpace(ciotCTe))
                                {
                                    Dominio.Entidades.MDFeCIOT ciotMDFe = new Dominio.Entidades.MDFeCIOT();

                                    ciotMDFe.MDFe = mdfe;
                                    ciotMDFe.NumeroCIOT = ciotCTe.Length > 12 ? Utilidades.String.Left(ciotCTe, 12) : ciotCTe;
                                    ciotMDFe.Responsavel = mdfe.Empresa.CNPJ;

                                    repMDFeCIOTe.Inserir(ciotMDFe);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao informar CIOT MDFe por CTe: " + ex);
            }
        }

        public void GerarCIOTPorVeiculoTracao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (mdfe != null)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                    Repositorio.MDFeCIOT repMDFeCIOTe = new Repositorio.MDFeCIOT(unidadeDeTrabalho);

                    List<Dominio.Entidades.MDFeCIOT> ciot = repMDFeCIOTe.BuscarPorMDFe(mdfe.Codigo);

                    if (ciot.Count == 0)
                    {
                        Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);
                        if (veiculoMDFe != null)
                        {
                            Dominio.Entidades.Veiculo veiculoCadastro = repVeiculo.BuscarPorPlaca(mdfe.Empresa.Codigo, veiculoMDFe.Placa);
                            if (veiculoCadastro == null)
                                veiculoCadastro = repVeiculo.BuscarPorPlaca(0, veiculoMDFe.Placa);

                            if (veiculoCadastro != null && !string.IsNullOrWhiteSpace(veiculoCadastro.CIOT))
                            {
                                Dominio.Entidades.MDFeCIOT ciotMDFe = new Dominio.Entidades.MDFeCIOT();

                                ciotMDFe.MDFe = mdfe;
                                ciotMDFe.NumeroCIOT = veiculoCadastro.CIOT.Length > 12 ? Utilidades.String.Left(veiculoCadastro.CIOT, 12) : veiculoCadastro.CIOT;
                                ciotMDFe.Responsavel = veiculoCadastro.ResponsavelCIOT != null ? veiculoCadastro.ResponsavelCIOT.CPF_CNPJ_SemFormato : mdfe.Empresa.CNPJ;

                                repMDFeCIOTe.Inserir(ciotMDFe);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao informar CIOT MDFe: " + ex);
            }
        }

        public void GerarValePedagioMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.ValePedagioIntegracao> valesPedagio, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (mdfe != null && valesPedagio != null && valesPedagio.Count() > 0)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);

                    List<Dominio.Entidades.ValePedagioMDFe> valesPedagioMDFe = repValePedagioMDFe.BuscarPorMDFe(mdfe.Codigo);

                    if (valesPedagioMDFe.Count == 0)
                    {
                        foreach (Dominio.ObjetosDeValor.ValePedagioIntegracao valePedagio in valesPedagio)
                        {
                            Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();

                            valePedagioMDFe.MDFe = mdfe;
                            valePedagioMDFe.CNPJFornecedor = valePedagio.CNPJFornecedor;
                            valePedagioMDFe.CNPJResponsavel = valePedagio.CNPJResponsavel;
                            valePedagioMDFe.NumeroComprovante = valePedagio.NumeroComprovante;
                            valePedagioMDFe.ValorValePedagio = valePedagio.ValorValePedagio;
                            valePedagioMDFe.QuantidadeEixos = valePedagio.QuantidadeEixos;
                            valePedagioMDFe.TipoCompra = valePedagio.TipoCompra.HasValue ? valePedagio.TipoCompra.Value : Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                            repValePedagioMDFe.Inserir(valePedagioMDFe);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao informar vale pedágio MDFe: " + ex);
            }
        }

        public void GerarValePedagioPorCTes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (mdfe != null)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);
                    Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                    List<Dominio.Entidades.ValePedagioMDFe> valePedagio = repValePedagioMDFe.BuscarPorMDFe(mdfe.Codigo);

                    if (ctes == null)
                        ctes = repDocumentoMunicipioDescarregamentoMDFe.BuscarCTesPorMDFe(mdfe.Codigo);

                    if (valePedagio.Count == 0 && ctes != null && ctes.Count > 0)
                    {
                        List<Dominio.Entidades.ValePedagioCTe> listaValePedagioCTe = ctes.Select(o => o.ValesPedagio).FirstOrDefault().Distinct().ToList();

                        if (listaValePedagioCTe != null && listaValePedagioCTe.Count() > 0)
                        {
                            foreach (Dominio.Entidades.ValePedagioCTe valePedagioCTe in listaValePedagioCTe)
                            {
                                Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();

                                valePedagioMDFe.MDFe = mdfe;
                                valePedagioMDFe.CNPJFornecedor = valePedagioCTe.CNPJFornecedor;
                                valePedagioMDFe.CNPJResponsavel = valePedagioCTe.CNPJResponsavel;
                                valePedagioMDFe.NumeroComprovante = valePedagioCTe.NumeroComprovante;
                                valePedagioMDFe.ValorValePedagio = valePedagioCTe.ValorValePedagio;
                                valePedagioMDFe.QuantidadeEixos = 2;
                                valePedagioMDFe.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                                repValePedagioMDFe.Inserir(valePedagioMDFe);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao informar vale pedágio MDFe por CTe: " + ex);
            }
        }

        public void GerarValePedagio(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagios, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
            Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeTrabalho);

            List<Dominio.Entidades.ValePedagioMDFe> valePedagioExistente = repValePedagioMDFe.BuscarPorMDFe(mdfe.Codigo);

            if (valePedagioExistente.Count == 0 && valesPedagios != null)
            {
                foreach (Dominio.ObjetosDeValor.ValePedagioMDFe valePedagio in valesPedagios)
                {
                    Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();

                    valePedagioMDFe.MDFe = mdfe;
                    valePedagioMDFe.CNPJFornecedor = valePedagio.CNPJFornecedor;
                    valePedagioMDFe.CNPJResponsavel = valePedagio.CNPJResponsavel;
                    valePedagioMDFe.NumeroComprovante = valePedagio.NumeroComprovante;
                    valePedagioMDFe.ValorValePedagio = valePedagio.ValorValePedagio;
                    valePedagioMDFe.CodigoAgendamentoPorto = valePedagio.CodigoAgendamentoPorto;
                    valePedagioMDFe.QuantidadeEixos = valePedagio.QuantidadeEixos.HasValue ? valePedagio.QuantidadeEixos.Value : 2;
                    valePedagioMDFe.TipoCompra = valePedagio.TipoCompra.HasValue ? valePedagio.TipoCompra.Value : Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                    repValePedagioMDFe.Inserir(valePedagioMDFe);
                }
            }
        }

        private void GerarContratantes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.Contratante> contratantes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (contratantes != null && contratantes.Count() > 0)
            {
                Repositorio.MDFeContratante repContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome> contratantesBanco = repContratante.BuscarPorMDFeGroupByCnpjNome(mdfe.Codigo);
                contratantes = contratantes.Where(c => !contratantesBanco.Any(cb => cb.CnpjContratante == c.CNPJCPFContratante && cb.NomeContratante == c.NomeContratante)).ToList();

                foreach (Dominio.ObjetosDeValor.MDFe.Contratante contratante in contratantes)
                {
                    Dominio.Entidades.MDFeContratante contratanteMDFe = new Dominio.Entidades.MDFeContratante();

                    contratanteMDFe.MDFe = mdfe;
                    contratanteMDFe.Contratante = contratante.CNPJCPFContratante;
                    contratanteMDFe.NomeContratante = !string.IsNullOrWhiteSpace(contratante.NomeContratante) ? contratante.NomeContratante.Left(60) : string.Empty;
                    contratanteMDFe.IDEstrangeiro = !string.IsNullOrWhiteSpace(contratante.IDEstrangeiro) ? contratante.IDEstrangeiro.Left(20) : string.Empty;

                    repContratante.Inserir(contratanteMDFe);
                }
            }
        }

        private void GerarCIOT(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (listaCIOT == null || listaCIOT.Count <= 0)
                return;

            Repositorio.MDFeCIOT repMDFeCIOT = new Repositorio.MDFeCIOT(unidadeDeTrabalho);

            if (repMDFeCIOT.ExistePorMDFe(mdfe.Codigo))
                return;

            foreach (Dominio.ObjetosDeValor.MDFe.CIOT ciot in listaCIOT)
            {
                Dominio.Entidades.MDFeCIOT ciotMDFe = new Dominio.Entidades.MDFeCIOT();

                ciotMDFe.MDFe = mdfe;
                ciotMDFe.NumeroCIOT = ciot.Numero;
                ciotMDFe.Responsavel = ciot.CNPJCPFResponsavel;

                repMDFeCIOT.Inserir(ciotMDFe);
            }

        }

        private void GerarSeguros(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.MDFe.Seguro> seguros, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (seguros != null && seguros.Count() > 0)
            {
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.MDFe.Seguro seguro in seguros)
                {
                    Dominio.Entidades.MDFeSeguro seguroMDFe = new Dominio.Entidades.MDFeSeguro();

                    seguroMDFe.MDFe = mdfe;
                    seguroMDFe.CNPJSeguradora = seguro.CNPJSeguradora;
                    seguroMDFe.NomeSeguradora = seguro.NomeSeguradora;
                    seguroMDFe.NumeroApolice = seguro.NumeroApolice;
                    seguroMDFe.NumeroAverbacao = seguro.NumeroAverbacao;
                    seguroMDFe.Responsavel = seguro.CNPJCPFResponsavel;
                    seguroMDFe.TipoResponsavel = seguro.Responsavel;

                    repMDFeSeguro.Inserir(seguroMDFe);
                }
            }
        }

        public void GerarValePedagioPorVeiculoTracao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (mdfe != null)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                    Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                    Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);

                    List<Dominio.Entidades.ValePedagioMDFe> valePedagio = repValePedagioMDFe.BuscarPorMDFe(mdfe.Codigo);

                    if (valePedagio.Count == 0)
                    {
                        Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);
                        if (veiculoMDFe != null)
                        {
                            Dominio.Entidades.Veiculo veiculoCadastro = repVeiculo.BuscarPorPlaca(mdfe.Empresa.Codigo, veiculoMDFe.Placa);
                            if (veiculoCadastro == null)
                                veiculoCadastro = repVeiculo.BuscarPorPlaca(0, veiculoMDFe.Placa);

                            if (veiculoCadastro != null && !string.IsNullOrWhiteSpace(veiculoCadastro.NumeroCompraValePedagio) && veiculoCadastro.FornecedorValePedagio != null)
                            {
                                Dominio.Entidades.ValePedagioMDFe valePedagioMDFe = new Dominio.Entidades.ValePedagioMDFe();

                                valePedagioMDFe.MDFe = mdfe;
                                valePedagioMDFe.CNPJFornecedor = veiculoCadastro.FornecedorValePedagio != null ? veiculoCadastro.FornecedorValePedagio.CPF_CNPJ_SemFormato : null;
                                valePedagioMDFe.CNPJResponsavel = veiculoCadastro.ResponsavelValePedagio != null ? veiculoCadastro.ResponsavelValePedagio.CPF_CNPJ_SemFormato : null;
                                valePedagioMDFe.NumeroComprovante = veiculoCadastro.NumeroCompraValePedagio;
                                valePedagioMDFe.ValorValePedagio = veiculoCadastro.ValorValePedagio;
                                valePedagioMDFe.QuantidadeEixos = veiculoCadastro.TipoDoVeiculo?.NumeroEixos ?? 2;
                                valePedagioMDFe.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                                repValePedagioMDFe.Inserir(valePedagioMDFe);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao informar vale pedágio MDFe: " + ex);
            }
        }

        private bool ExtrairChaveEProtocoloDoRetornoSefaz(string retornoCompleto, ref string chave, ref string protocolo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(retornoCompleto))
                    return false;

                string pattern = @"(?:\[chMDFe Não Encerrada:([0-9]+)\])(?:\[NroProtocolo:([0-9]+)\])";

                MatchCollection matches = Regex.Matches(retornoCompleto, pattern);

                chave = Utilidades.String.OnlyNumbers(matches[0].Groups[1].Value);
                protocolo = Utilidades.String.OnlyNumbers(matches[0].Groups[2].Value);

                if (string.IsNullOrWhiteSpace(chave) || chave.Length != 44)
                    return false;

                if (string.IsNullOrWhiteSpace(protocolo))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Extrair Chave MDFe pedente (" + retornoCompleto + "): " + ex);
                return false;
            }

        }

        private string CNPJResponsavelSeguro(string CNPJResponsavel, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Recebe valor setado da aplicacao e verifica se e nullo
            // Se for, verifica se a configuracao e para usar o CNPJ da transportadora como CNPJ do seguro
            if (string.IsNullOrWhiteSpace(CNPJResponsavel) && this.UsarCNPJTransportadorComoCNPJSeguradora(mdfe))
            {
                return mdfe.Empresa.CNPJ;
            }

            return CNPJResponsavel;
        }

        private string NumeroAverbacaoSeguro(string NumeroAverbacao, string NumeroApolice, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Recebe valor setado da aplicacao e verifica se e nullo
            // Se for, verifica se a configuracao e para usar o numero da averbacao como numero da apolice
            if (string.IsNullOrWhiteSpace(NumeroAverbacao) && this.UsarNumeroApoliceComoNumeroAverbacao(mdfe))
            {
                return NumeroApolice;
            }

            return NumeroAverbacao;
        }

        private bool UsarCNPJTransportadorComoCNPJSeguradora(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Define retorno padrao
            Dominio.Enumeradores.OpcaoSimNao? config = Dominio.Enumeradores.OpcaoSimNao.Sim;

            // Busca da configuracao local
            if (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora != null)
                config = mdfe.Empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora;
            // Busca da configuracao pai
            else if (mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora != null)
                config = mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora;

            // Compara
            return config == Dominio.Enumeradores.OpcaoSimNao.Sim;
        }

        private bool UsarNumeroApoliceComoNumeroAverbacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            // Define retorno padrao
            Dominio.Enumeradores.OpcaoSimNao? config = Dominio.Enumeradores.OpcaoSimNao.Sim;

            // Busca da configuracao local
            if (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.NumeroApoliceComoNumeroAverbacao != null)
                config = mdfe.Empresa.Configuracao.NumeroApoliceComoNumeroAverbacao;
            // Busca da configuracao pai
            else if (mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao != null)
                config = mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao;

            // Compara
            return config == Dominio.Enumeradores.OpcaoSimNao.Sim;
        }

        public string AjustarFuso(int hora, int minutos)
        {
            string minutosString = minutos.ToString();

            if (minutosString.Length == 1)
                minutosString = string.Concat("0", minutosString);

            switch (hora)
            {
                case -12:
                    return "-12:" + minutosString;
                case -11:
                    return "-11:" + minutosString;
                case -10:
                    return "-10:" + minutosString;
                case -9:
                    return "-09:" + minutosString;
                case -8:
                    return "-08:" + minutosString;
                case -7:
                    return "-07:" + minutosString;
                case -6:
                    return "-06:" + minutosString;
                case -5:
                    return "-05:" + minutosString;
                case -4:
                    return "-04:" + minutosString;
                case -3:
                    return "-03:" + minutosString;
                case -2:
                    return "-02:" + minutosString;
                case -1:
                    return "-01:" + minutosString;
                case 0:
                    return "00:" + minutosString;
                case 1:
                    return "+01:" + minutosString;
                case 2:
                    return "+02:" + minutosString;
                case 3:
                    return "+03:" + minutosString;
                case 4:
                    return "+04:" + minutosString;
                case 5:
                    return "+05:" + minutosString;
                case 6:
                    return "+06:" + minutosString;
                case 7:
                    return "+07:" + minutosString;
                case 8:
                    return "+08:" + minutosString;
                case 9:
                    return "+09:" + minutosString;
                case 10:
                    return "+10:" + minutosString;
                case 11:
                    return "+11:" + minutosString;
                case 12:
                    return "+12:" + minutosString;
                case 13:
                    return "+13:" + minutosString;
                default:
                    return "-03:00";
            }

        }

        public void SalvarLogEncerramentoMDFe(string chaveMDFe, string protocoloMDFe, DateTime dataEncerramento, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Localidade localidade, string mensagemLog, Repositorio.UnitOfWork unidadeDeTrabalh)
        {
            try
            {
                // Adiciona o log
                Repositorio.EncerramentoManualMDFe repEncerramentoManualMDFe = new Repositorio.EncerramentoManualMDFe(unidadeDeTrabalh);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalh);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPrimeiroPorEmpresa(empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao);

                if (usuario != null)
                {
                    Dominio.Entidades.EncerramentoManualMDFe log = new Dominio.Entidades.EncerramentoManualMDFe();

                    log.ChaveMDFe = chaveMDFe;
                    log.Protocolo = protocoloMDFe;
                    log.DataHoraEncerramento = DateTime.Now;
                    log.DataHoraEvento = dataEncerramento > DateTime.MinValue ? dataEncerramento : DateTime.Now;
                    log.Empresa = empresa;
                    log.Localidade = localidade;
                    log.Usuario = usuario;
                    log.Log = !string.IsNullOrWhiteSpace(mensagemLog) && mensagemLog.Length > 200 ? mensagemLog.Substring(0, 200) : mensagemLog;

                    repEncerramentoManualMDFe.Inserir(log);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("SalvarLogEncerramentoMDFe: " + ex);
            }
        }

        public void ProcessarMDFeMultiCTe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.TransportadorAverbacao repositorioTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Carga.MDFe svcCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeDeTrabalho);

            string configWebServiceConsultaCTe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().WebServiceConsultaCTe;
            bool configProcessarCTeMultiCTe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().ProcessarCTeMultiCTe.HasValue ? Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().ProcessarCTeMultiCTe.Value : false;

            if (!configProcessarCTeMultiCTe)
                return;

            if (mdfe.SistemaEmissor != TipoEmissorDocumento.NSTech)
                return;

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apoliceSeguros = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            apoliceSeguros = repositorioTransportadorAverbacao.BuscarApolicePorTransportador(mdfe.Empresa.Codigo);

            if (mdfe.Empresa.UsarTipoOperacaoApolice)
                apoliceSeguros = repositorioTransportadorAverbacao.BuscarApolicePorTransportador(mdfe.Empresa.Codigo);

            if (apoliceSeguros.Count <= 0)
                apoliceSeguros = repositorioApoliceSeguro.BuscarVigentePorEmpresa(mdfe.Empresa.Codigo);

            if (apoliceSeguros.Count <= 0)
                return;

            bool abriuTransacao = false;
            if (!unidadeDeTrabalho.IsActiveTransaction())
            {
                unidadeDeTrabalho.Start();
                abriuTransacao = true;
            }

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesAverbacao = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao>();

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice in apoliceSeguros)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceAverbacao = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao
                {
                    ApoliceSeguro = apolice,
                };

                repositorioApoliceSeguroAverbacao.Inserir(apoliceAverbacao);

                apolicesAverbacao.Add(apoliceAverbacao);
            }

            svcCargaMDFe.GerarAverbacoesCargaMDFe(apolicesAverbacao, unidadeDeTrabalho, configuracaoEmbarcador, null, mdfe);

            if (abriuTransacao)
                unidadeDeTrabalho.CommitChanges();
        }

        private int BuscarSequencialEventoMDFe(int codigoMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.MDFeRetornoSefaz repMDFeRetornoSefaz = new Repositorio.MDFeRetornoSefaz(unidadeDeTrabalho);
            int sequencialEvento = repMDFeRetornoSefaz.BuscarSequencialEventosPorMDFe(codigoMDFe);
            return sequencialEvento + 1;
        }

        private string BuscarVersao(Dominio.Entidades.Empresa empresa)
        {
            if (empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe))
                return empresa.Configuracao.VersaoMDFe;
            if (empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.VersaoMDFe))
                return empresa.EmpresaPai.Configuracao.VersaoMDFe;

            return "3.00";
        }

        private Dominio.Entidades.EmpresaSerie BuscarSerie(Dominio.Entidades.EmpresaSerie serie, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.EmpresaSerie repositorioEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            if (serie != null)
                return serie;
            if (empresa.Configuracao != null && empresa.Configuracao.SerieMDFe != null)
                return empresa.Configuracao.SerieMDFe;

            return repositorioEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);
        }

        #endregion
    }
}

