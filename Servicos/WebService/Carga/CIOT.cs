using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Carga
{
    public class CIOT : ServicoWebServiceBase
    {
        #region Propriedades Privadas

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly string _adminStringConexao;
        private readonly string _webServiceConsultaCTe;

        #endregion Propriedades Privadas

        #region Construtores

        public CIOT(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CIOT(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
        }
        public CIOT(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao, string webServiceConsultaCTe = "") : base(unitOfWork, tipoServicoMultisoftware, clienteAcesso, clienteMultisoftware)
        {
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
            _webServiceConsultaCTe = webServiceConsultaCTe;
        }

        public CIOT(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
        }

        #endregion Construtores

        #region Metodos Publicos

        public Retorno<bool> IntegrarCIOTCarga(Dominio.ObjetosDeValor.WebService.Carga.CIOT dadosCIOT, Dominio.Entidades.WebService.Integradora integradora, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                if ((dadosCIOT?.ProtocoloCarga ?? 0) > 0)
                    carga = repositorioCarga.BuscarPorProtocolo(dadosCIOT?.ProtocoloCarga ?? 0); 

                if (carga == null && !string.IsNullOrWhiteSpace(dadosCIOT?.ChaveCTE))
                    carga = repositorioCarga.BuscarPorChaveCTE(dadosCIOT?.ChaveCTE);

                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possivel encontrar uma carga");

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork).BuscarPorCarga(carga.Codigo);

                if (cargaMDFes.Any(x => x.MDFe != null || x.MDFeManual != null))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("MDF-e já emitido para essa carga, não é possível integrar CIOT.");

                if (new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork).ExistePorCarga(carga.Codigo))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Só é permitido integrar um CIOT por carga");

                if ((dadosCIOT?.NumeroCIOT?.Length ?? 0) != 12)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O número CIOT deve conter 12 caracteres");

                if (dadosCIOT.ValorAdiantamento >= dadosCIOT.ValorFrete)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O 'ValorAdiantamento' não pode ser maior ou igual que o 'ValorFrete'.");

                if (dadosCIOT.DataAbertura > DateTime.Now)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A 'DataAbertura' não pode ser maior que a data atual.");

                if (dadosCIOT.DataFinalizacao.HasValue && dadosCIOT.DataFinalizacao.Value < DateTime.Now)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A 'DataFinalizacao' não pode ser menor que a data atual.");

                if (!Enum.IsDefined(typeof(TipoPagamentoMDFe), dadosCIOT.TipoPagamentoCIOT))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Para integrar o CIOT é necessário informar uma forma de pagamento, valida.");

                switch (dadosCIOT.TipoPagamentoCIOT)
                {
                    case TipoPagamentoMDFe.PIX:
                        if (string.IsNullOrWhiteSpace(dadosCIOT.ChavePIX))
                            return Retorno<bool>.CriarRetornoDadosInvalidos("O campo 'ChavePIX' não pode ser vazio quando a forma de pagamento for igual a {1 - PIX}.");

                        break;
                    case TipoPagamentoMDFe.Banco:
                        if (string.IsNullOrEmpty(dadosCIOT.Banco))
                            return Retorno<bool>.CriarRetornoDadosInvalidos("O campo 'Banco' não pode ser vazio quando a forma de pagamento for igual a {2 - Banco}.");

                        if (string.IsNullOrEmpty(dadosCIOT.Agencia))
                            return Retorno<bool>.CriarRetornoDadosInvalidos("O campo 'Agencia' não pode ser vazio quando a forma de pagamento for igual a {2 - Banco}.");

                        if (dadosCIOT.Banco.Length > 5)
                            return Retorno<bool>.CriarRetornoDadosInvalidos("O campo 'Banco' não pode ter mais que cinco caracteres.");

                        break;
                    case TipoPagamentoMDFe.Ipef:
                        if (string.IsNullOrWhiteSpace(dadosCIOT.CNPJInstituicaoPagamentoCIOT))
                            return Retorno<bool>.CriarRetornoDadosInvalidos("O campo 'CNPJInstituicaoPagamentoCIOT' não pode ser vazio quando a forma de pagamento for igual a {3 - Ipef}.");

                        if (dadosCIOT.CNPJInstituicaoPagamentoCIOT.ObterSomenteNumeros().Length != 14)
                            return Retorno<bool>.CriarRetornoDadosInvalidos("O 'CNPJInstituicaoPagamentoCIOT' está inválido, precisa conter 14 números sem máscara.");

                        break;
                }

                if (dadosCIOT.DataPagamento == null || dadosCIOT.DataPagamento == DateTime.MinValue)
                {
                    if (dadosCIOT.FormaPagamento == FormasPagamento.Avista)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("Para 'FormaPagamento' {0 - A vista} a 'DataPagamento' é obrígatória e deve corresponder à data efetiva.");
                    
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Para 'FormaPagamento' {1 - A prazo} a 'DataPagamento' é obrígatória e deve corresponder à data prevista/estimada.");
                }

                Repositorio.Embarcador.Cargas.CargaInformacoesBancarias repositorioCargaInformacoesBancarias = new Repositorio.Embarcador.Cargas.CargaInformacoesBancarias(_unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente terceiro = repositorioCliente.BuscarPorCPFCNPJSemFetch(dadosCIOT.CNPJTerceiro.ToDouble());

                if (terceiro == null && (carga.Empresa?.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro ?? false))
                    terceiro = repositorioCliente.BuscarPorCPFCNPJ(carga.Empresa?.CNPJ.ToDouble() ?? 0);

                if (terceiro == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O terceiro não está cadastro no sistema.");

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = new Dominio.Entidades.Embarcador.Documentos.CIOT
                {
                    DataFinalViagem = dadosCIOT.DataFinalizacao ?? DateTime.Now.AddDays(1),
                    DataParaFechamento = dadosCIOT.DataFinalizacao,
                    Transportador = terceiro,
                    Contratante = carga.Empresa,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto,
                    DataAbertura = DateTime.Now,
                    Motorista = carga.Motoristas.FirstOrDefault(),
                    Veiculo = carga.Veiculo,
                    CIOTPorPeriodo = true,
                    CIOTGeradoAutomaticamente = true,
                    TipoPagamentoCIOT = dadosCIOT.TipoPagamentoCIOT == TipoPagamentoMDFe.PIX ? TipoPagamentoCIOT.PIX : TipoPagamentoCIOT.Transferencia,
                    Numero = dadosCIOT.NumeroCIOT,
                };

                repositorioCIOT.Inserir(ciot);

                Dominio.Entidades.Global.CargaInformacoesBancarias cargaInformacoesBancarias = new Dominio.Entidades.Global.CargaInformacoesBancarias
                {
                    Agencia = dadosCIOT.TipoPagamentoCIOT == TipoPagamentoMDFe.Banco ? dadosCIOT.Agencia : null,
                    Conta = dadosCIOT.TipoPagamentoCIOT == TipoPagamentoMDFe.Banco ? dadosCIOT.Banco : null,
                    ChavePIX = dadosCIOT.TipoPagamentoCIOT == TipoPagamentoMDFe.PIX ? dadosCIOT.ChavePIX : null,
                    Carga = carga,
                    Ipef = dadosCIOT.CNPJInstituicaoPagamentoCIOT,
                    TipoInformacaoBancaria = dadosCIOT.TipoPagamentoCIOT,
                    TipoPagamento = dadosCIOT.FormaPagamento,
                    ValorAdiantamento = dadosCIOT.ValorAdiantamento,
                    DataPagamento = dadosCIOT.DataPagamento,
                    RegistradoPeloEmbarcador = false
                };
                repositorioCargaInformacoesBancarias.Inserir(cargaInformacoesBancarias);

                Servicos.Embarcador.CIOT.CIOT.CriarCargaCIOTAutomaticamente(carga, ciot, dadosCIOT.ValorAdiantamento, dadosCIOT.ValorFrete, _unitOfWork);

                carga.PossuiPendencia = false;
                carga.ProblemaIntegracaoCIOT = false;
                carga.MotivoPendencia = "";
                repositorioCarga.Atualizar(carga);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, "Integração realizada com sucesso.");
            }
            catch (ServicoException ex)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao Integrar CIOT");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
