using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class MDFe : ServicoBase
    {
        public MDFe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public MDFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public void AtualizarANTT(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unitOfWork);

            mdfe.RNTRC = mdfe.Empresa.RegistroANTT;

            Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);
            if (veiculoMDFe != null)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : mdfe.Empresa.Codigo, veiculoMDFe.Placa);
                if (veiculo != null)
                {
                    if (veiculo.Tipo == "T")
                    {
                        veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                        veiculoMDFe.CPFCNPJProprietario = veiculo.Proprietario?.CPF_CNPJ_SemFormato ?? "";
                        veiculoMDFe.IEProprietario = veiculo.Proprietario?.IE_RG ?? "";
                        veiculoMDFe.NomeProprietario = Utilidades.String.Left(veiculo.Proprietario?.Nome, 60) ?? "";
                        veiculoMDFe.TipoProprietario = veiculo.TipoProprietario.ToString("d");
                        veiculoMDFe.UFProprietario = veiculo.Proprietario?.Localidade.Estado ?? null;
                    }
                    else
                    {
                        veiculoMDFe.RNTRC = "";
                        veiculoMDFe.CPFCNPJProprietario = "";
                        veiculoMDFe.IEProprietario = "";
                        veiculoMDFe.NomeProprietario = "";
                        veiculoMDFe.TipoProprietario = "";
                        veiculoMDFe.UFProprietario = null;
                    }
                    veiculoMDFe.RENAVAM = veiculo.Renavam;

                    repVeiculoMDFe.Atualizar(veiculoMDFe);
                }
            }
            List<Dominio.Entidades.ReboqueMDFe> reboqueMDFes = repReboqueMDFe.BuscarPorMDFe(mdfe.Codigo);
            foreach (Dominio.Entidades.ReboqueMDFe reboqueMDFe in reboqueMDFes)
            {
                Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorPlacaVarrendoFiliais(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : mdfe.Empresa.Codigo, reboqueMDFe.Placa);
                if (reboque != null)
                {
                    if (reboque.Tipo == "T")
                    {
                        reboqueMDFe.RNTRC = string.Format("{0:00000000}", reboque.RNTRC);
                        reboqueMDFe.CPFCNPJProprietario = reboque.Proprietario?.CPF_CNPJ_SemFormato ?? "";
                        reboqueMDFe.IEProprietario = reboque.Proprietario?.IE_RG ?? "";
                        reboqueMDFe.NomeProprietario = Utilidades.String.Left(reboque.Proprietario?.Nome, 60) ?? "";
                        reboqueMDFe.TipoProprietario = reboque.TipoProprietario.ToString("d");
                        reboqueMDFe.UFProprietario = reboque.Proprietario?.Localidade.Estado ?? null;
                    }
                    else
                    {
                        reboqueMDFe.RNTRC = "";
                        reboqueMDFe.CPFCNPJProprietario = "";
                        reboqueMDFe.IEProprietario = "";
                        reboqueMDFe.NomeProprietario = "";
                        reboqueMDFe.TipoProprietario = "";
                        reboqueMDFe.UFProprietario = null;
                    }
                    reboqueMDFe.RENAVAM = reboque.Renavam;
                    repReboqueMDFe.Atualizar(reboqueMDFe);
                }
            }
        }

        public string EmitirMDFeAquaviario(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";

            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

                List<Dominio.Entidades.Embarcador.Pedidos.Porto> portosIniciosPrestacao = new List<Dominio.Entidades.Embarcador.Pedidos.Porto>();
                List<Dominio.Entidades.Embarcador.Pedidos.Porto> portosTerminosPrestacao = new List<Dominio.Entidades.Embarcador.Pedidos.Porto>();

                List<Dominio.Entidades.Embarcador.Pedidos.Porto> portosOrigem = new List<Dominio.Entidades.Embarcador.Pedidos.Porto>() { cargaMDFeManual.PortoOrigem };
                List<Dominio.Entidades.Embarcador.Pedidos.Porto> portosDestino = new List<Dominio.Entidades.Embarcador.Pedidos.Porto>();

                IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> cargaMDFeManualDestinos = repCargaMDFeManualDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

                if (cargaMDFeManual.PortoDestino != null)
                    portosDestino.Add(cargaMDFeManual.PortoDestino);
                else
                {
                    foreach (var cte in cargaMDFeManual.CTes)
                    {
                        if (cte.CTe != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestinoCTe = null;
                            if (cte.CTe.PortoPassagemUm != null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, cargaMDFeManual.PortoOrigem.Codigo, cte.CTe.PortoPassagemUm.Codigo))
                                {
                                    portoDestinoCTe = cte.CTe.PortoPassagemUm;
                                }
                            }
                            if (cte.CTe.PortoPassagemDois != null && portoDestinoCTe == null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, cargaMDFeManual.PortoOrigem.Codigo, cte.CTe.PortoPassagemDois.Codigo))
                                {
                                    portoDestinoCTe = cte.CTe.PortoPassagemDois;
                                }
                            }
                            if (cte.CTe.PortoPassagemTres != null && portoDestinoCTe == null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, cargaMDFeManual.PortoOrigem.Codigo, cte.CTe.PortoPassagemTres.Codigo))
                                {
                                    portoDestinoCTe = cte.CTe.PortoPassagemTres;
                                }
                            }
                            if (cte.CTe.PortoPassagemQuatro != null && portoDestinoCTe == null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, cargaMDFeManual.PortoOrigem.Codigo, cte.CTe.PortoPassagemQuatro.Codigo))
                                {
                                    portoDestinoCTe = cte.CTe.PortoPassagemQuatro;
                                }
                            }
                            if (cte.CTe.PortoPassagemCinco != null && portoDestinoCTe == null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, cargaMDFeManual.PortoOrigem.Codigo, cte.CTe.PortoPassagemCinco.Codigo))
                                {
                                    portoDestinoCTe = cte.CTe.PortoPassagemCinco;
                                }
                            }
                            if (cte.CTe.PortoDestino != null && portoDestinoCTe == null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, cargaMDFeManual.PortoOrigem.Codigo, cte.CTe.PortoDestino.Codigo))
                                {
                                    portoDestinoCTe = cte.CTe.PortoDestino;
                                }
                            }
                            if (portoDestinoCTe != null)
                                portosDestino.Add(portoDestinoCTe);
                        }
                    }
                }

                if (portosDestino == null || portosDestino.Count == 0)
                    return "Nenhum porto de destino localizado.";

                portosTerminosPrestacao.AddRange((from obj in portosDestino select obj).Distinct().ToList());
                portosIniciosPrestacao.AddRange((from obj in portosOrigem select obj).Distinct().ToList());

                foreach (Dominio.Entidades.Embarcador.Pedidos.Porto portoInicioPrestacao in portosIniciosPrestacao)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Porto portoTerminoPrestacao in portosTerminosPrestacao)
                    {
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMDFe = ObterCTesParaMDFeAquaviario(cargaMDFeManual, tipoServicoMultisoftware, portoInicioPrestacao, portoTerminoPrestacao, unitOfWork);
                        List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();

                        Dominio.Entidades.Estado estadoTerminoPrestacao = portoTerminoPrestacao.Localidade?.Estado;
                        Dominio.Entidades.Estado estadoInicioPrestacao = portoInicioPrestacao.Localidade?.Estado;

                        if (estadoTerminoPrestacao == null || estadoInicioPrestacao == null)
                            return "Existe(m) porto(s) sem sua localidade informada.";

                        if (ctesDoMDFe.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDescarregamento = null;
                            if (cargaMDFeManual.TerminalDescarregamento == null || cargaMDFeManual.TerminalDescarregamento.Count == 0)
                            {
                                terminalDescarregamento = repTerminal.BuscarPorPorto(portoTerminoPrestacao.Codigo);
                                if (terminalDescarregamento == null)
                                    return "Não foi encontrado nenhum terminal cadastrado para o porto de destino.";
                            }

                            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = GerarMDFe(cargaMDFeManual, tipoServicoMultisoftware, cargaMDFeManual.Empresa, cargaMDFeManual.Veiculo, cargaMDFeManual.Reboques.ToList(), motoristas, ctesDoMDFe, null, unitOfWork, estadoInicioPrestacao, estadoTerminoPrestacao, cargaMDFeManual.Origem, cargaMDFeManual.UsarListaDestinos() ? null : cargaMDFeManual.Destino, portoInicioPrestacao, portoTerminoPrestacao, terminalDescarregamento, null, configuracaoTMS);

                            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe();
                            cargaMDFeManualMDFe.CargaMDFeManual = cargaMDFeManual;
                            cargaMDFeManualMDFe.MDFe = mdfe;

                            repCargaMDFeManualMDFe.Inserir(cargaMDFeManualMDFe);

                            mdfes.Add(mdfe);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(mensagem))
                {
                    if (mdfes.Count > 0)
                    {
                        unitOfWork.CommitChanges();

                        foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                        {
                            if (!serMDFe.Emitir(mdfe, unitOfWork))
                            {
                                mensagem += "O MDF-e (" + mdfe.Numero + ") foi salvo, porém, ocorreram problemas ao enviar para o sefaz. ";
                            }
                        }

                        return string.IsNullOrEmpty(mensagem) ? string.Empty : mensagem;
                    }
                    else
                    {
                        return "NaoPossuiMDFe";
                    }
                }
                else
                {
                    return mensagem;
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return "Ocorreu uma falha ao emitir o MDF-e.";
            }
        }

        public string EmitirMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            return EmitirMDFe(cargaMDFeManual, configuracaoTMS, tipoServicoMultisoftware, webServiceConsultaCTe, unitOfWork, out bool gerouMdfe);
        }

        public string EmitirMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork, out bool gerouMdfe)
        {
            string mensagem = "";
            gerouMdfe = false;

            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unitOfWork);

                Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

                List<Dominio.Entidades.Estado> estadosIniciosPrestacao = new List<Dominio.Entidades.Estado>();
                List<Dominio.Entidades.Estado> terminosPrestacao = new List<Dominio.Entidades.Estado>();


                List<Dominio.Entidades.Localidade> localidadesOrigem = new List<Dominio.Entidades.Localidade>() { cargaMDFeManual.Origem };
                List<Dominio.Entidades.Localidade> localidadesDestino = new List<Dominio.Entidades.Localidade>();

                IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> cargaMDFeManualDestinos = repCargaMDFeManualDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
                if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario && cargaMDFeManual.UsarListaDestinos())
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino cargaMDFeManualDestino in cargaMDFeManualDestinos)
                        localidadesDestino.Add(cargaMDFeManualDestino.Localidade);
                }
                else
                    localidadesDestino.Add(cargaMDFeManual.Destino);

                terminosPrestacao.AddRange((from obj in localidadesDestino select obj.Estado).Distinct().ToList());
                estadosIniciosPrestacao.AddRange((from obj in localidadesOrigem select obj.Estado).Distinct().ToList());

                List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();
                if (cargaMDFeManual.Motoristas != null && cargaMDFeManual.Motoristas.Count > 0)
                    motoristas = cargaMDFeManual.Motoristas.ToList();

                if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario && cargaMDFeManual.Veiculo == null)
                {
                    unitOfWork.Rollback();
                    return "É necessário informar um veículo para emissão do MDF-e.";
                }

                if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario && string.IsNullOrWhiteSpace(cargaMDFeManual.Veiculo.Renavam))
                {
                    unitOfWork.Rollback();
                    return "Para emitir o MDF-e é necessário informar o RENAVAM do veículo " + cargaMDFeManual.Veiculo.Placa + ". Por favor, atualize o cadastro do veículo e tente novamente.";
                }

                Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral;
                if (cargaMDFeManual.Cargas != null && cargaMDFeManual.Cargas.Count > 0 && cargaMDFeManual.Cargas.FirstOrDefault().TipoDeCarga != null && cargaMDFeManual.Cargas.FirstOrDefault().TipoDeCarga.TipoCargaMDFe.HasValue)
                    tipoCargaMDFe = cargaMDFeManual.Cargas.FirstOrDefault().TipoDeCarga.TipoCargaMDFe.Value;

                foreach (Dominio.Entidades.Estado estadoInicioPrestacao in estadosIniciosPrestacao)
                {
                    foreach (Dominio.Entidades.Estado estadoTerminoPrestacao in terminosPrestacao)
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            mdfe = GerarMDFeManualPorNFe(cargaMDFeManual, estadoInicioPrestacao, estadoTerminoPrestacao, motoristas, configuracaoTMS, tipoCargaMDFe, tipoServicoMultisoftware, unitOfWork);
                        else if (!cargaMDFeManual.Empresa.EmpresaPropria)
                            mdfe = GerarMDFeManualPorCTe(cargaMDFeManual, estadoInicioPrestacao, estadoTerminoPrestacao, motoristas, configuracaoTMS, tipoCargaMDFe, tipoServicoMultisoftware, unitOfWork);
                        else
                            mdfe = GerarMDFeManualPorNotas(cargaMDFeManual, estadoInicioPrestacao, estadoTerminoPrestacao, motoristas, configuracaoTMS, tipoCargaMDFe, tipoServicoMultisoftware, unitOfWork);

                        if (mdfe != null)
                            mdfes.Add(mdfe);
                    }
                }

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return mensagem;

                if (mdfes.Count == 0)
                    return "NaoPossuiMDFe";

                unitOfWork.CommitChanges();
                gerouMdfe = true;

                foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                {
                    if (!serMDFe.Emitir(mdfe, unitOfWork))
                        mensagem += "O MDF-e (" + mdfe.Numero + ") foi salvo, porém, ocorreram problemas ao enviar para o sefaz. ";
                }

                return mensagem ?? string.Empty;
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return ex.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return "Ocorreu uma falha ao emitir o MDF-e.";
            }
        }

        public string EmitirMDFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codEmpresaPai, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork = null)
        {
            string mensagem = "";

            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new CargaLocaisPrestacao(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

                Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
                Servicos.Embarcador.Carga.CTe serCargaCte = new CTe(unitOfWork);

                if (repCargaCTe.ContarCTePorListaSituacaoDiff(carga.Codigo, new string[] { "A", "F", "C", "I", "Z" }) > 0) ////if (repCargaCTe.ContarCTePorSituacaoDiff(carga.Codigo, "A") > 0)
                    return "Não é possível emitir MDF-e antes de emitir todos os CTEs";

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = cargaPedidos.First().TipoEmissaoCTeParticipantes;
                Dominio.Entidades.Cliente tomador = cargaPedidos.First().ObterTomador();

                bool emissaoComTransbordo = carga.CargaTransbordo || serCargaCte.VerificarSeEmissaoSeraPorComTransbordo(tipoEmissaoCTeParticipantes);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
                bool emiteComFilialEmissora = false;
                List<Dominio.Entidades.Empresa> empresasOrigem = (from obj in cargaPedidos select obj.CargaOrigem.Empresa).Distinct().ToList();
                Dominio.Entidades.Empresa empresa = carga.Empresa;

                if (carga.EmpresaFilialEmissora != null)
                {
                    if (carga.EmiteMDFeFilialEmissora)
                    {
                        emiteComFilialEmissora = true;
                        empresa = carga.EmpresaFilialEmissora;
                        empresasOrigem = (from obj in cargaPedidos select obj.CargaOrigem.EmpresaFilialEmissora).Distinct().ToList();
                    }
                    else if (!carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)//se emite cte com o CT-e da filial emissora, não gerar MDF-e da filial transportadora.
                        return "NaoPossuiMDFe";
                }

                if (serCargaLocaisPrestacao.ValidarPassagensMDFe(carga, unitOfWork))
                {
                    bool gerarMDFeTransbordoSemConsiderarOrigem = VerificarSeGeraMDFeTransbordoSemConsiderarOrigem(tomador, carga);

                    List<Dominio.Entidades.Estado> estadosInicioPrestacao = new List<Dominio.Entidades.Estado>();
                    List<Dominio.Entidades.Estado> terminosPrestacao = new List<Dominio.Entidades.Estado>();

                    List<Dominio.Entidades.Localidade> localidadesOrigem = cargaPedidos.Where(x => x.CargaOrigem.LocalidadeColetaLiberada != null).Select(x => x.CargaOrigem.LocalidadeColetaLiberada).Distinct().ToList();

                    if (!localidadesOrigem.Any())
                        localidadesOrigem = repCargaLocaisPrestacao.BuscarEstadosOrigemPrestacao(carga.Codigo);


                    bool considerarRecebedorDaCarga = carga?.TipoOperacao?.ConfiguracaoCarga?.GerarMDFeParaRecebedorDaCarga ?? false;

                    List<Dominio.Entidades.Localidade> localidadesDestino;

                    if (considerarRecebedorDaCarga)
                    {
                        List<Dominio.Entidades.Localidade> ListalocalidadeDestinatarios = new List<Dominio.Entidades.Localidade>();

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRecebedor = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);
                        Dominio.Entidades.Localidade localidadeRecebedor = cargaPedidoRecebedor?.Recebedor?.Localidade ?? null;

                        if (localidadeRecebedor == null)
                            localidadesDestino = repCargaLocaisPrestacao.BuscarEstadosDestinoPrestacao(carga.Codigo);
                        else
                        {
                            ListalocalidadeDestinatarios.Add(localidadeRecebedor);
                            localidadesDestino = ListalocalidadeDestinatarios;
                        }

                    }
                    else
                        localidadesDestino = repCargaLocaisPrestacao.BuscarEstadosDestinoPrestacao(carga.Codigo);


                    terminosPrestacao.AddRange((from obj in localidadesDestino select obj.Estado).Distinct().ToList());
                    estadosInicioPrestacao.AddRange((from obj in localidadesOrigem select obj.Estado).Distinct().ToList());

                    bool permitirIncluirCTesDeDiferentesUFsEmMDFeUnico = (carga.TipoOperacao?.ConfiguracaoCarga?.PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico ?? false) && localidadesDestino.Count == 1 && cargaPedidos.Where(x => x.Recebedor != null).Select(x => x.Recebedor).Distinct().Count() == 1;

                    if (estadosInicioPrestacao.Count > 0)
                    {
                        foreach (Dominio.Entidades.Estado estadoTerminoPrestacao in terminosPrestacao)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoEstado = repCargaLocaisPrestacao.BuscarPorCargaEUFDestino(carga.Codigo, estadoTerminoPrestacao.Sigla);

                            if (cargaLocaisPrestacaoEstado.Count <= 0)
                                continue;

                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens> mdfePassagens = serCargaLocaisPrestacao.RetornarPassagensParaLocaisPercursoEntreOsEstados(cargaLocaisPrestacaoEstado, unitOfWork);

                            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens mdfePassagem in mdfePassagens)// está validação é feita para verificar se não existem passagens diferentes para o mesmo estado de origem e destino, porém com localidades diferentes, se exister serão emtididos as quantidades de CT-es correspondentes
                            {
                                if (carga.Veiculo == null)
                                {
                                    unitOfWork.Rollback();
                                    return "É necessário informar um veículo para emissão do MDF-e.";
                                }

                                if (string.IsNullOrWhiteSpace(carga.Veiculo.Renavam))
                                {
                                    unitOfWork.Rollback();
                                    return "Para emitir o MDF-e é necessário informar o RENAVAM do veículo " + carga.Veiculo.Placa + ". Por favor, atualize o cadastro do veículo e tente novamente.";
                                }

                                if (carga.Veiculo.CIOTEmitidoContratanteDiferenteEmbarcador && (carga.Veiculo.DataFinalCIOTTemporario?.Date < DateTime.Now.Date))
                                {
                                    unitOfWork.Rollback();
                                    return "O CIOT Temporário do Veículo " + carga.Veiculo.Placa + " esta vencido. Por favor, atualize o cadastro do veículo e tente novamente.";
                                }

                                if (string.IsNullOrWhiteSpace(carga.Empresa.RegistroANTT))
                                {
                                    unitOfWork.Rollback();
                                    return "Para emitir o MDF-e é necessário informar o RNTRC do transportador " + carga.Empresa.RazaoSocial + ". Por favor, atualize o cadastro do transportador e tente novamente.";
                                }

                                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMDFe = null;
                                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDoMDFe = null;
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro = null;

                                string observacaoMDFe = string.Empty;

                                bool cargaPropria = empresa.EmpresaPropria || (carga.TipoOperacao != null && carga.TipoOperacao.CargaPropria);

                                if (carga.EmiteMDFeFilialEmissora && carga.UtilizarCTesAnterioresComoCTeFilialEmissora)
                                {
                                    int limiteCteTerceiro = 1000;
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiroDestinoPedido = repCTeTerceiro.BuscarPorCargaEDestino(carga.Codigo, mdfePassagem.Destino?.Estado.Sigla ?? mdfePassagem.CargaLocaisPrestacao.LocalidadeTerminoPrestacao.Estado.Sigla, limiteCteTerceiro, apenasComMesmaLocalidadeCarga: true);
                                    if (ctesTerceiroDestinoPedido != null && ctesTerceiroDestinoPedido.Count > 0)
                                    {
                                        ctesTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

                                        ctesTerceiro.AddRange(ctesTerceiroDestinoPedido);

                                        if (ctesTerceiro.Count < limiteCteTerceiro)
                                            ctesTerceiro.AddRange(repCTeTerceiro.BuscarPorCargaEDestino(carga.Codigo, mdfePassagem.Destino?.Estado.Sigla ?? mdfePassagem.CargaLocaisPrestacao.LocalidadeTerminoPrestacao.Estado.Sigla, limite: limiteCteTerceiro - ctesTerceiro.Count, apenasComMesmaLocalidadeCarga: false));
                                    }

                                    if (ctesTerceiro.Count <= 0)
                                        continue;
                                }
                                else if (!cargaPropria)//se for carga própria emite o MDF-e por nota e não por CT-e
                                {
                                    ctesDoMDFe = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                                    if (considerarRecebedorDaCarga)
                                    {

                                        List<Dominio.Entidades.Localidade> listaLocalidadesDestinoCTes = cargaPedidos.Select(x => x.Pedido.Destino).ToList();
                                        List<Dominio.Entidades.Estado> estadosCTes = new List<Dominio.Entidades.Estado>();

                                        estadosCTes.AddRange((from obj in listaLocalidadesDestinoCTes select obj.Estado).Distinct().ToList());

                                        foreach (Dominio.Entidades.Estado estadoCTe in estadosCTes)
                                        {
                                            foreach (Dominio.Entidades.Empresa empresaOrigem in empresasOrigem)
                                                ctesDoMDFe.AddRange(ObterCTesParaMDFe(carga, empresaOrigem, mdfePassagem, cargaLocaisPrestacaoEstado, emissaoComTransbordo, tipoServicoMultisoftware, estadoCTe, tomador, unitOfWork, gerarMDFeTransbordoSemConsiderarOrigem, permitirIncluirCTesDeDiferentesUFsEmMDFeUnico));
                                        }
                                    }
                                    else
                                    {
                                        foreach (Dominio.Entidades.Empresa empresaOrigem in empresasOrigem)
                                            ctesDoMDFe.AddRange(ObterCTesParaMDFe(carga, empresaOrigem, mdfePassagem, cargaLocaisPrestacaoEstado, emissaoComTransbordo, tipoServicoMultisoftware, estadoTerminoPrestacao, tomador, unitOfWork, gerarMDFeTransbordoSemConsiderarOrigem, permitirIncluirCTesDeDiferentesUFsEmMDFeUnico));
                                    }

                                    if (ctesDoMDFe.Count <= 0)
                                        continue;
                                }
                                else
                                {
                                    notasFiscaisDoMDFe = ObterNFsParaMDFe(carga, mdfePassagem, cargaLocaisPrestacaoEstado, emissaoComTransbordo, tipoServicoMultisoftware, estadoTerminoPrestacao, unitOfWork);

                                    if (notasFiscaisDoMDFe.Count <= 0)
                                        continue;

                                    if (cargaPedidos != null && cargaPedidos.Count > 0 && cargaPedidos.FirstOrDefault().Pedido != null && !string.IsNullOrWhiteSpace(cargaPedidos.FirstOrDefault().Pedido.Observacao))
                                        observacaoMDFe = cargaPedidos.FirstOrDefault().Pedido.Observacao;
                                }

                                Dominio.Entidades.Localidade localidadeFronteira = null;

                                if (mdfePassagem.CargaLocaisPrestacao.LocalidadeFronteira != null)
                                {
                                    localidadeFronteira = mdfePassagem.CargaLocaisPrestacao.LocalidadeFronteira;
                                }
                                else
                                {
                                    var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
                                    localidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(carga)?.Fronteira.Localidade;
                                }

                                bool cargaPropriaNotasExportacao = cargaPropria && notasFiscaisDoMDFe != null && notasFiscaisDoMDFe.Count > 0 && notasFiscaisDoMDFe.Any(o => o.Destinatario.Localidade.Estado.Sigla == "EX");

                                Dominio.Entidades.Estado estadoInicioPrestacao = mdfePassagem?.CargaLocaisPrestacao?.LocalidadeInicioPrestacao?.Estado;
                                Dominio.Entidades.Localidade localidadeDestinoMDFe = null;
                                Dominio.Entidades.Localidade localidadeInicioMDFe = mdfePassagem?.CargaLocaisPrestacao?.LocalidadeInicioPrestacao ?? localidadesOrigem.FirstOrDefault(x => x.Estado.Sigla == estadoInicioPrestacao.Sigla);

                                if (considerarRecebedorDaCarga)
                                    localidadeDestinoMDFe = localidadesDestino.FirstOrDefault();
                                else
                                    localidadeDestinoMDFe = permitirIncluirCTesDeDiferentesUFsEmMDFeUnico || (gerarMDFeTransbordoSemConsiderarOrigem && emissaoComTransbordo) || cargaPropriaNotasExportacao || carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.SubCarga ? localidadesDestino.FirstOrDefault() : null;

                                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = GerarMDFe(carga.Codigo, emiteComFilialEmissora, tipoServicoMultisoftware, empresa, carga.Veiculo, carga.VeiculosVinculados.ToList(), carga.Motoristas.ToList(), ctesDoMDFe, notasFiscaisDoMDFe, emissaoComTransbordo, unitOfWork, null, serCargaLocaisPrestacao.RetornaPassagensProntasParaMDFe(estadoInicioPrestacao, estadoTerminoPrestacao, localidadeFronteira, mdfePassagem.Passagem), localidadeInicioMDFe, localidadeDestinoMDFe, carga.Lacres.Select(o => new Dominio.ObjetosDeValor.MDFe.Lacre() { Numero = o.Numero }).ToList(), observacaoMDFe, carga.TipoDeCarga?.TipoCargaMDFe ?? Dominio.Enumeradores.TipoCargaMDFe.CargaGeral, ctesTerceiro);

                                AtualizarANTT(ref mdfe, tipoServicoMultisoftware, unitOfWork);

                                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe();
                                cargaMDFe.Carga = carga;
                                cargaMDFe.MDFe = mdfe;

                                if (configuracaoTMS.ImprimirObservacaoPedidoMDFe || (notasFiscaisDoMDFe != null && notasFiscaisDoMDFe.Count > 0))
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();
                                    string observacaoCTe = cargaPedido.Pedido.ObservacaoCTe;
                                    if (!string.IsNullOrWhiteSpace(observacaoCTe))
                                    {
                                        string placas = carga.Veiculo != null ? carga.Veiculo.Placa : "";
                                        if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                                            placas = string.Concat(placas, " / ", carga.VeiculosVinculados.FirstOrDefault().Placa);

                                        string numeroContainer = cargaPedido.Pedido != null && cargaPedido.Pedido.Container != null ? cargaPedido.Pedido.Container.Numero : cargaPedido.Pedido?.NumeroContainer ?? string.Empty;
                                        if (string.IsNullOrWhiteSpace(numeroContainer))
                                        {
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> listaCargaVeiculoContainers = repCargaVeiculoContainer.BuscarPorCarga(carga.Codigo);
                                            numeroContainer = listaCargaVeiculoContainers != null && listaCargaVeiculoContainers.Count > 0 ? string.Join(", ", listaCargaVeiculoContainers.Select(o => o.NumeroContainer)) : string.Empty;
                                        }

                                        List<Dominio.Entidades.MDFeSeguro> listaMDFeSeguros = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);
                                        string seguradora = listaMDFeSeguros != null && listaMDFeSeguros.Count > 0 ? listaMDFeSeguros.FirstOrDefault().NomeSeguradora : string.Empty;
                                        string apoliceSeguro = listaMDFeSeguros != null && listaMDFeSeguros.Count > 0 ? listaMDFeSeguros.FirstOrDefault().NumeroApolice : string.Empty;
                                        string notas = string.Join(" / ", repPedidoXMLNotaFiscal.ObterNumerosNotasPorCarga(carga.Codigo));
                                        string lacres = carga.Lacres?.Count > 0 ? string.Join(", ", carga.Lacres.Select(o => o.Numero)) : string.Empty;

                                        string observacao = observacaoCTe.Replace("#CNPJTomador", cargaPedido.ObterTomador().CPF_CNPJ_Formatado).
                                                               Replace("#NomeTomador", cargaPedido.ObterTomador().Nome).
                                                               Replace("#CNPJRemetente", cargaPedido.ObterTomador().CPF_CNPJ_Formatado).
                                                               Replace("#NomeRemetente", cargaPedido.ObterTomador().Nome).
                                                               Replace("#CNPJDestinatario", cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado).
                                                               Replace("#NomeDestinatario", cargaPedido.Pedido.Destinatario.Nome).
                                                               Replace("#NumeroPedido", cargaPedido.Pedido.NumeroPedidoEmbarcador).
                                                               Replace("#NumeroBooking", cargaPedido.Pedido?.NumeroBooking ?? "").
                                                               Replace("#NumeroPedidoCliente", cargaPedido.Pedido?.CodigoPedidoCliente ?? "").
                                                               Replace("#NavioViagemDirecao", cargaPedido.Pedido?.PedidoViagemNavio?.Descricao ?? "").
                                                               Replace("#QuantidadeETipoContainer", (cargaPedido.Pedido != null && cargaPedido.Pedido.Container != null && cargaPedido.Pedido.Container.ContainerTipo != null ? "Qtde: 1 container de " + cargaPedido.Pedido.Container.ContainerTipo.Descricao + " pés" : "")).
                                                               Replace("#PortoOrigem", cargaPedido.Pedido?.Porto?.Descricao ?? "").
                                                               Replace("#PortoDestino", cargaPedido.Pedido?.PortoDestino?.Descricao ?? "").
                                                               Replace("#NumeroCTe", mdfe.Numero.ToString()).
                                                               Replace("#NumeroCarga", carga.CodigoCargaEmbarcador).
                                                               Replace("#SerieCTe", mdfe.Serie.Numero.ToString()).
                                                               Replace("#NumeroNotaFiscal", notas).
                                                               Replace("#Placas", placas).
                                                               Replace("#CPFMotorista", carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault().CPF : "").
                                                               Replace("#NomeMotorista", carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault().Nome : "").
                                                               Replace("#RotaPedido", "").
                                                               Replace("#Rota", "").
                                                               Replace("#ValorTotalPrestacao", carga.ValorFreteAPagar.ToString("n2")).
                                                               Replace("#Seguradora", seguradora).
                                                               Replace("#ApoliceSeguro", apoliceSeguro).
                                                               Replace("#NumeroContainer", numeroContainer).
                                                               Replace("#OrdemPedido", cargaPedido.Pedido?.Ordem ?? string.Empty).
                                                               Replace("#TipoOperacao", carga.TipoOperacao?.Descricao ?? string.Empty).
                                                               Replace("#ValorFrete", "").
                                                               Replace("#LacresCarga", lacres);
                                        mdfe.ObservacaoContribuinte += observacao;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(configuracaoTMS.ObservacaoMDFePadraoEmbarcador))
                                {
                                    string observacaoPadraoMDFe = configuracaoTMS.ObservacaoMDFePadraoEmbarcador;

                                    string notas = observacaoPadraoMDFe.Contains("#NumeroNotaFiscal") ? string.Join(" / ", repPedidoXMLNotaFiscal.ObterNumerosNotasPorCarga(carga.Codigo)) : string.Empty;

                                    string observacao = observacaoPadraoMDFe.
                                                           Replace("#NumeroCarga", carga.CodigoCargaEmbarcador).
                                                           Replace("#NumeroNotaFiscal", notas).
                                                           Replace("#TipoOperacao", carga.TipoOperacao?.Descricao ?? string.Empty);
                                    mdfe.ObservacaoContribuinte += observacao;
                                }

                                cargaMDFe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;
                                cargaMDFe.CargaLocaisPrestacao = mdfePassagem.CargaLocaisPrestacao;

                                repCargaMDFe.Inserir(cargaMDFe);

                                ReplicarMDFeParaCargaDT(cargaMDFe, unitOfWork);
                                GerarAverbacoesCargaMDFe(cargaPedidos.Select(o => o.ApoliceSeguroAverbacao).SelectMany(o => o).Distinct().ToList(), unitOfWork, configuracaoTMS, cargaMDFe);

                                mdfes.Add(mdfe);
                            }
                        }
                    }
                }
                else
                {
                    mensagem += "É necessário informar um percurso válido para emissão dos MDFe-(s)";

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(carga?.CodigoCargaEmbarcador, "error", 9999, "Não existe percurso válido cadastrado.", carga?.CodigoCargaEmbarcador, unitOfWork);
                }

                if (string.IsNullOrWhiteSpace(mensagem))
                {
                    if (mdfes.Count > 0)
                    {
                        unitOfWork.CommitChanges();

                        foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                        {
                            if (!serMDFe.Emitir(mdfe, unitOfWork))
                            {
                                mensagem += "O MDF-e (" + mdfe.Numero + ") foi salvo, porém, ocorreram problemas ao enviar para o sefaz. ";
                            }
                        }

                        return string.IsNullOrEmpty(mensagem) ? string.Empty : mensagem;
                    }
                    else
                    {
                        return "NaoPossuiMDFe";
                    }
                }
                else
                {
                    return mensagem;
                }


            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return "Ocorreu uma falha ao emitir o MDF-e.";
            }
        }

        public static string ObterPlacas(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe == null)
                return string.Empty;

            string placas = string.Empty;

            foreach (Dominio.Entidades.VeiculoMDFe veiculo in mdfe.Veiculos)
                placas += veiculo.Placa;

            foreach (Dominio.Entidades.ReboqueMDFe reboque in mdfe.Reboques)
                placas += ", " + reboque.Placa;

            return placas;
        }

        public static string ObterMotoristas(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe == null)
                return string.Empty;

            string motoristas = string.Join(", ", mdfe.Motoristas.Select(o => o.Nome));

            return motoristas;
        }

        public string GerarMDFeManual(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.MDFE.ManifestoEletronicoDeDocumentosFiscaisManual repMdfeManual = new Repositorio.Embarcador.MDFE.ManifestoEletronicoDeDocumentosFiscaisManual(unitOfWork);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);



                Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo);

                if (!cargaCTEs.Exists(obj => obj.CTe.Status != "A"))
                {

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTEsInterEstaduais = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                    CTEsInterEstaduais.AddRange(from p in cargaCTEs where p.CTe.LocalidadeInicioPrestacao.Estado.Sigla != p.CTe.LocalidadeTerminoPrestacao.Estado.Sigla select p.CTe);

                    List<Dominio.Entidades.Estado> estadosDestino = (from p in CTEsInterEstaduais select p.LocalidadeTerminoPrestacao.Estado).Distinct().ToList();


                    List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MDFEs = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
                    foreach (Dominio.Entidades.Estado estado in estadosDestino)
                    {
                        List<Dominio.Entidades.Estado> estadosOrigem = (from p in CTEsInterEstaduais select p.LocalidadeInicioPrestacao.Estado).Distinct().ToList();

                        foreach (Dominio.Entidades.Estado estadoOrigem in estadosOrigem)
                        {
                            Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual mdfeManual = new Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual();
                            mdfeManual.EstadoCarregamento = estadoOrigem;
                            mdfeManual.EstadoDescarregamento = estado;
                            mdfeManual.MDFeInformado = false;
                            mdfeManual.DataInformacaoManual = DateTime.Now;
                            repMdfeManual.Inserir(mdfeManual);
                            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe();
                            cargaMDFe.Carga = carga;
                            cargaMDFe.MDFeManual = mdfeManual;
                            repCargaMDFe.Inserir(cargaMDFe);

                            ReplicarMDFeParaCargaDT(cargaMDFe, unitOfWork);
                        }
                    }
                    unitOfWork.CommitChanges();
                    return "";
                }
                else
                {
                    unitOfWork.Rollback();
                    return "Não é possível informar o MDF-e antes de importar todos os CTEs";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return "Ocorreu uma falha ao emitir o MDF-e.";
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe ObterDadosEncerramento(int codigoCargaMDFe, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorCodigo(codigoCargaMDFe);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercurso = repCargaPercurso.ConsultarPorCargaEEstado(cargaMDFe.Carga.Codigo, cargaMDFe.MDFe.EstadoDescarregamento.Sigla);

            List<Dominio.Entidades.Localidade> localidadesDestino;
            if (cargaPercurso.Count == 0)
            {
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocalidadesPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> localidades = repCargaLocalidadesPrestacao.BuscarPorCargaUFOrigemEUFDestino(cargaMDFe.Carga.Codigo, cargaMDFe.MDFe.EstadoCarregamento.Sigla, cargaMDFe.MDFe.EstadoDescarregamento.Sigla);
                localidadesDestino = (from obj in localidades select obj.LocalidadeTerminoPrestacao).ToList();
            }
            else
            {
                localidadesDestino = (from obj in cargaPercurso select obj.Destino).ToList();
            }

            if (localidadesDestino == null || localidadesDestino.Count <= 0)
                localidadesDestino = cargaMDFe.MDFe.MunicipiosDescarregamento.Select(o => o.Municipio).ToList();

            Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramentoMDF = new Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe();
            dadosEncerramentoMDF.Codigo = cargaMDFe.MDFe.Codigo;
            dadosEncerramentoMDF.Estado = cargaMDFe.MDFe.EstadoDescarregamento;
            dadosEncerramentoMDF.DataEncerramento = DateTime.Now;
            dadosEncerramentoMDF.Localidades = localidadesDestino;

            return dadosEncerramentoMDF;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe ObterDadosEncerramentoMDFe(int codigoMDFe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unidadeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

            List<Dominio.Entidades.Localidade> localidadesDestino = mdfe.MunicipiosDescarregamento.Select(o => o.Municipio).ToList();

            Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramentoMDF = new Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe();
            dadosEncerramentoMDF.Codigo = mdfe.Codigo;
            dadosEncerramentoMDF.Estado = mdfe.EstadoDescarregamento;
            dadosEncerramentoMDF.DataEncerramento = DateTime.Now;
            dadosEncerramentoMDF.Localidades = localidadesDestino;

            return dadosEncerramentoMDF;
        }

        public void EncerrarMDFesCargaEmitidaParcialmente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade localideEncerramento, List<string> ufsDestino, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMdfe in cargaMDFes)
            {
                cargaMdfe.MDFeAnteriorCargaParcial = true;
                if (cargaMdfe.MDFe.Importado != true && cargaMdfe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && ufsDestino.Contains(cargaMdfe.MDFe.EstadoDescarregamento.Sigla))
                {
                    EncerrarMDFe(cargaMdfe.MDFe.Codigo, carga.Codigo, localideEncerramento.Codigo, DateTime.Now, "", usuario, tipoServicoMultisoftware, unidadeTrabalho, Auditado);
                    if (Auditado != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Solicitou encerramento do MDF-e " + cargaMdfe.MDFe.Codigo + ", ao enviar as notas fiscais restantes da carga.", unidadeTrabalho);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMdfe, "Solicitou encerramento do MDF-e ao enviar as notas fiscais restantes da carga " + carga.CodigoCargaEmbarcador + ".", unidadeTrabalho);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMdfe.MDFe, "Solicitou encerramento do MDF-e ao enviar as notas fiscais restantes da carga " + carga.CodigoCargaEmbarcador + ".", unidadeTrabalho);
                    }
                }
                repCargaMDFe.Atualizar(cargaMdfe);
            }
        }

        public string EncerrarMDFe(int codigo, int codCarga, int codMunicipio, DateTime dataEncerramento, string webServiceConsultaCTe, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
            Dominio.Entidades.Localidade municipio = repLocalidade.BuscarPorCodigo(codMunicipio);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFeECarga(codigo, codCarga);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);

            if (((carga.Empresa?.Configuracao.EncerrarCIOTPorViagem ?? false) || (configuracaoIntegracao?.EncerrarTodosCIOTAutomaticamente ?? false)) && !carga.CargaEmitidaParcialmente)
            {
                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(codCarga);

                if (cargaCIOT?.CIOT != null && cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto && (!configuracaoGeralCarga?.NaoPermitirEncerrarCIOTEncerrarCarga ?? false))
                {
                    Servicos.Log.TratarErro("EncerrarMDFe codigoCiot" + cargaCIOT.CIOT.Codigo.ToString(), "QuitacaoCIOTCarga");
                    if (svcCIOT.EncerrarCIOT(cargaCIOT.CIOT, unidadeTrabalho, tipoServicoMultisoftware, out string mensagemErro))
                    {
                        if (auditado != null)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaCIOT.CIOT, $"Encerrou o CIOT à partir do encerramento do MDF-e {mdfe.Descricao}.", unidadeTrabalho);
                    }
                    else
                        Servicos.Log.TratarErro(mensagemErro);
                }
            }

            if (cargaMDFe != null && cargaMDFe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && !configuracaoTMS.PermiteEncerrarMDFeEmitidoNoEmbarcador)
                return "Não é permitido o encerramento de um MDF-e emitido pelo embarcador.";

            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
            {
                mdfe.MunicipioEncerramento = municipio;

                if (usuario != null)
                    mdfe.Log = "Encerrado pelo usuário " + usuario.Nome + " as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                else
                    mdfe.Log = "Encerrado automaticamente as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                repMDFe.Atualizar(mdfe);
                Servicos.MDFe serMDFe = new Servicos.MDFe(unidadeTrabalho);

                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                DateTime dataEvento = DateTime.Now;
                dataEvento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

                Dominio.Entidades.Empresa empresa = carga.Empresa;
                if (carga.EmpresaFilialEmissora != null)
                    empresa = carga.EmpresaFilialEmissora;

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unidadeTrabalho, dataEvento))
                    return $"Falha ao encerrar MDFE {mdfe.Numero}.";

                serMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, mdfe.Log, unidadeTrabalho);

                if (mdfe.Veiculos != null && mdfe.Veiculos.Count > 0 && !carga.CargaEmitidaParcialmente)
                {
                    foreach (var veiculo in mdfe.Veiculos)
                        serCarga.FinalizarViagemVeiculo(veiculo.Placa, mdfe, unidadeTrabalho, municipio, DateTime.Now, auditado);
                }
                return "";
            }
            else
            {
                return "A atual situação do MDF-e (" + mdfe.DescricaoStatus + ") não permite o seu encerramento.";
            }
        }

        public static bool EncerrarMDFePeloETSConfirmado(out string erro, Dominio.Entidades.Usuario usuario, string webServiceConsultaCTe, int codigoViagem, int codigoPortoAtracacao, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            erro = "";
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.Porto portoAtracacao = repPorto.BuscarPorCodigo(codigoPortoAtracacao);

            if (portoAtracacao == null || portoAtracacao.Localidade == null)
            {
                erro = "Porto de atracação não informadou, ou sua localidade não está cadastrada";
                return false;
            }

            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.BuscarMDFesPendentesEncerramento(codigoViagem, codigoPortoAtracacao);
            foreach (var mdfe in mdfes)
            {
                if (mdfe.Importado != true)
                {
                    if (Auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Gerou Encerramento do MDF-e a partir do ETS Confirmado", unidadeTrabalho);

                    if (!Servicos.Embarcador.Carga.MDFe.EncerrarMDFe(out erro, mdfe.Codigo, portoAtracacao.Localidade.Codigo, DateTime.Now, webServiceConsultaCTe, usuario, unidadeTrabalho, stringConexao, Auditado))
                        return false;
                }
            }
            return true;
        }

        public static bool EncerrarMDFe(out string erro, int codigo, int codMunicipio, DateTime dataEncerramento, string webServiceConsultaCTe, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

            Dominio.Entidades.Localidade municipio = repLocalidade.BuscarPorCodigo(codMunicipio);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);

            if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
            {
                erro = "A atual situação do MDF-e (" + mdfe.DescricaoStatus + ") não permite o seu encerramento.";
                return false;
            }

            mdfe.MunicipioEncerramento = municipio;

            if (usuario != null)
                mdfe.Log = "Encerrado pelo usuário " + usuario.Nome + " às " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            else
                mdfe.Log = "Encerrado automaticamente as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            repMDFe.Atualizar(mdfe);
            Servicos.MDFe serMDFe = new Servicos.MDFe(unidadeTrabalho);

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
            DateTime dataEvento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unidadeTrabalho, dataEvento);
            serMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, mdfe.Log, unidadeTrabalho);

            if (mdfe.Veiculos != null && mdfe.Veiculos.Count > 0)
                foreach (var veiculo in mdfe.Veiculos)
                    serCarga.FinalizarViagemVeiculo(veiculo.Placa, mdfe, unidadeTrabalho, municipio, DateTime.Now, Auditado);

            erro = string.Empty;
            return true;
        }

        public static bool EncerrarMDFeEmissorExterno(out string erro, string chave, Dominio.Entidades.Localidade localidadeEncerramento, string protocolo, Dominio.Entidades.Empresa empresa, DateTime dataEncerramento, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            if (string.IsNullOrWhiteSpace(empresa.NomeCertificado))
            {
                erro = "Transportador não possui certificado configurado para encerramento.";
                return false;
            }

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeChave = repMDFe.BuscarPorChave(chave);

            if (mdfeChave != null && mdfeChave.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
            {
                erro = "Não é possível encerrar manualmente MDF-e existente no sistema. Favor efetuar o encerramento acessando o painel do transportador.";
                return false;
            }

            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dataEncerramento);
            string fusoHorario = horarioVerao ? svcMDFe.AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : svcMDFe.AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno = new Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno()
            {
                Ambiente = empresa.TipoAmbiente,
                Chave = chave,
                CodigoMunicipioEncerramento = localidadeEncerramento.CodigoIBGE,
                CodigoUFEncerramento = localidadeEncerramento.Estado.CodigoIBGE,
                DataEncerramento = dataEncerramento,
                DataEvento = dataEncerramento,
                Empresa = empresa,
                Protocolo = protocolo,
                FusoHorario = fusoHorario
            };

            bool retorno = EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFe.EncerrarMdfeEmissorExterno(mdfeEmissorExterno, unitOfWork);

            if (retorno)
            {
                string log = string.Concat(dataEncerramento.ToString("dd/MM/yyyy HH:mm:ss"), " - Encerramento enviado por ", !string.IsNullOrWhiteSpace(usuario?.Nome) ? usuario.Nome : "Sistema");
                svcMDFe.SalvarLogEncerramentoMDFe(mdfeEmissorExterno.Chave, mdfeEmissorExterno.Protocolo, dataEncerramento, mdfeEmissorExterno.Empresa, mdfeEmissorExterno.Empresa.Localidade, log, unitOfWork);
            }

            erro = string.Empty;

            return retorno;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> ObterCargasEncerramentoPorPlacaVeiculo(string placa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasVeiculo = repCarga.BuscaCargaEmAbertoPorVeiculo(placa);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFeEncerramento = new List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaVeiculo in cargasVeiculo)
            {
                if (cargaVeiculo.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || cargaVeiculo.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFe = repCargaMDFe.BuscarPorCarga(cargaVeiculo.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargasMDFe)
                    {
                        if (cargaMDFe.MDFe != null && cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        {
                            cargasMDFeEncerramento.Add(cargaMDFe);
                        }
                    }
                }
            }
            return cargasMDFeEncerramento;
        }

        public void FinalizarCargaPorEncerramentoDeMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                if (!configuracaoGeralCarga.FinalizarCargaAutomaticamenteAposEncerramentoMDFe)
                    return;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCargaMDFe.BuscarCargaPorMDFe(mdfe.Codigo);
                if (carga == null || carga.CargaEmitidaParcialmente || carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || !carga.DataFimViagem.HasValue)
                    return;

                unitOfWork.Start();

                carga.DataEncerramentoCarga = DateTime.Now;
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Solicitado encerramento automático da carga após encerramento de MDF-e", unitOfWork);

                servicoCarga.ValidarCargasFinalizadas(ref carga, tipoServicoMultisoftware, auditado, unitOfWork);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Log.TratarErro(ex.Message);
            }
        }

        public void ReplicarMDFeParaCargaDT(Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaMDFe.Carga.DadosSumarizados?.CargaTrecho != CargaTrechoSumarizada.SubCarga)
                return;

            Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaDT = repositorioStageAgrupamento.BuscarPrimeiraCargaDTPorCargaGerada(cargaMDFe.Carga.Codigo);

            if (cargaDT == null)
                return;

            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFeDuplicado = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe()
            {
                Carga = cargaDT,
                CargaLocaisPrestacao = cargaMDFe.CargaLocaisPrestacao,
                MDFe = cargaMDFe.MDFe,
                MDFeManual = cargaMDFe.MDFeManual,
                SistemaEmissor = cargaMDFe.SistemaEmissor
            };

            repositorioCargaMDFe.Inserir(cargaMDFeDuplicado);
        }

        public async Task<string> SincronizarDocumentoEmProcessamentoAsync(int codigoMDFE, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, CancellationToken cancellation)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = await repMDFe.BuscarPorCodigoAsync(codigoMDFE, codigoEmpresa);

            if (mdfe == null)
                return "O MDF-e informado não foi localizado";

            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = await repCargaMDFe.BuscarPorMDFeAsync(mdfe.Codigo);

            if (!(mdfe.Status == StatusMDFe.Enviado || mdfe.Status == StatusMDFe.EmCancelamento || mdfe.Status == StatusMDFe.EmEncerramento || mdfe.Status == StatusMDFe.EventoInclusaoMotoristaEnviado))
                return $"A atual situação do MDF-e ({mdfe.DescricaoStatus}) não permite sua sincronização.";

            bool sucesso = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ConsultarMdfe(mdfe, Auditado, TipoServicoMultisoftware, _unitOfWork);

            if (!sucesso)
                return "Não foi possível efetuar a sincronização do documento.";

            if (cargaMDFe != null)
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaMDFe.Carga, null, "Documento sincronizado.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellation);

            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, mdfe, null, "Documento sincronizado.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellation);

            return string.Empty;
        }

        #region Averbação

        public bool GerarAverbacoesCargaMDFe(List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = null, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe = null)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoCarga.BuscarPrimeiroRegistro();

            if (cargaMDFe?.MDFe == null && (apolicesSeguro == null || apolicesSeguro.Count == 0))
                return false;

            if (!((configuracaoTMS.AverbarMDFe && !configuracaoGeralCarga.AverbarMDFeSomenteEmCargasComCIOT) || (configuracaoTMS.AverbarMDFe && configuracaoGeralCarga.AverbarMDFeSomenteEmCargasComCIOT && repositorioCargaCIOT.ExisteCIOTPorCarga(cargaMDFe.Carga.Codigo))))
                return false;

            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguroAnterior = null;

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro in apolicesSeguro)
            {
                if (apoliceSeguroAnterior != null && apoliceSeguroAnterior.Codigo == seguro.ApoliceSeguro.Codigo)
                    continue;

                if (cargaMDFe != null && repAverbacaoMDFe.ExistePorMDFeEApoliceSeguro(cargaMDFe.MDFe.Codigo, seguro.Codigo))
                    continue;

                if (seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido)
                    continue;

                if (seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                {
                    Dominio.Entidades.AverbacaoMDFe averbacaoMDFe = new Dominio.Entidades.AverbacaoMDFe()
                    {
                        Carga = cargaMDFe != null ? cargaMDFe.Carga : null,
                        MDFe = cargaMDFe != null ? cargaMDFe.MDFe : MDFe,
                        ApoliceSeguroAverbacao = seguro,
                        Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao,
                        SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido,
                        Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente
                    };

                    repAverbacaoMDFe.Inserir(averbacaoMDFe);

                    apoliceSeguroAnterior = seguro.ApoliceSeguro;
                }
            }

            return false;
        }

        public void AjustarAverbacoesParaAutorizacao(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            List<Dominio.Entidades.AverbacaoMDFe> averbacoes = repAverbacaoMDFe.BuscarPorMDFeESituacao(codigoMDFe, Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente);

            int countAverbacoes = averbacoes.Count;

            for (var i = 0; i < countAverbacoes; i++)
            {
                Dominio.Entidades.AverbacaoMDFe averbacao = averbacoes[i];

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.AgEmissao;

                repAverbacaoMDFe.Atualizar(averbacao);
            }
        }

        public void AjustarAverbacoesParaCancelamento(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            List<Dominio.Entidades.AverbacaoMDFe> averbacoes = repAverbacaoMDFe.BuscarPorMDFeESituacao(codigoMDFe, Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso);

            int countAverbacoes = averbacoes.Count;

            for (var i = 0; i < countAverbacoes; i++)
            {
                Dominio.Entidades.AverbacaoMDFe averbacao = averbacoes[i];

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.AgCancelamento;

                repAverbacaoMDFe.Atualizar(averbacao);
            }
        }

        public void AjustarAverbacoesParaEncerramento(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            List<Dominio.Entidades.AverbacaoMDFe> averbacoes = repAverbacaoMDFe.BuscarPorMDFeESituacao(codigoMDFe, Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso);

            int countAverbacoes = averbacoes.Count;

            for (var i = 0; i < countAverbacoes; i++)
            {
                Dominio.Entidades.AverbacaoMDFe averbacao = averbacoes[i];

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.AgEncerramento;

                repAverbacaoMDFe.Atualizar(averbacao);
            }
        }

        public void EmitirAverbacao(int codigoAverbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            Dominio.Entidades.AverbacaoMDFe averbacao = repAverbacaoMDFe.BuscarPorCodigo(codigoAverbacao, false);

            if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.AgEmissao)
                return;

            if (averbacao.ApoliceSeguroAverbacao == null)
            {
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                averbacao.MensagemRetorno = "Sem Apolice Seguro.";

                repAverbacaoMDFe.Atualizar(averbacao);

                return;
            }


            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = averbacao.ApoliceSeguroAverbacao.ApoliceSeguro;

            switch (apolice.SeguradoraAverbacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM:

                    Servicos.Embarcador.Integracao.ATM.ATMIntegracaoMDFe.AverbarDocumento(apolice, averbacao, unitOfWork);

                    break;
                default:
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                    averbacao.MensagemRetorno = "Seguradora não disponível para integração.";

                    repAverbacaoMDFe.Atualizar(averbacao);

                    break;
            }
        }

        public void CancelarAverbacao(int codigoAverbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            Dominio.Entidades.AverbacaoMDFe averbacao = repAverbacaoMDFe.BuscarPorCodigo(codigoAverbacao, false);

            if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.AgCancelamento)
                return;

            if (averbacao.ApoliceSeguroAverbacao == null)
            {
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                averbacao.MensagemRetorno = "Sem Apolice Seguro.";

                repAverbacaoMDFe.Atualizar(averbacao);

                return;
            }

            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = averbacao.ApoliceSeguroAverbacao.ApoliceSeguro;

            switch (apolice.SeguradoraAverbacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM:

                    Servicos.Embarcador.Integracao.ATM.ATMIntegracaoMDFe.CancelarDocumento(apolice, averbacao, unitOfWork);

                    break;
                default:
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso;
                    averbacao.MensagemRetorno = "Seguradora não disponível para integração.";

                    repAverbacaoMDFe.Atualizar(averbacao);

                    break;
            }
        }

        public void EncerrarAverbacao(int codigoAverbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            Dominio.Entidades.AverbacaoMDFe averbacao = repAverbacaoMDFe.BuscarPorCodigo(codigoAverbacao, false);

            if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.AgEncerramento)
                return;

            if (averbacao.ApoliceSeguroAverbacao == null)
            {
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                averbacao.MensagemRetorno = "Sem Apolice Seguro.";

                repAverbacaoMDFe.Atualizar(averbacao);

                return;
            }

            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = averbacao.ApoliceSeguroAverbacao.ApoliceSeguro;

            switch (apolice.SeguradoraAverbacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM:

                    Servicos.Embarcador.Integracao.ATM.ATMIntegracaoMDFe.EncerrarDocumento(apolice, averbacao, unitOfWork);

                    break;
                default:
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso;
                    averbacao.MensagemRetorno = "Seguradora não disponível para integração.";

                    repAverbacaoMDFe.Atualizar(averbacao);

                    break;
            }
        }

        #endregion

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterNFsParaMDFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens mdfePassagem, List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoEstado, bool emissaoComTransbordo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Estado estadoTerminoPrestacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> nfsParaEmissao = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            if (!emissaoComTransbordo || cargaLocaisPrestacaoEstado.Count > 1)
            {
                if (mdfePassagem.Origem != null && mdfePassagem.Destino != null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> nfsLocalidade = repXMLNotaFiscal.BuscarPorLocalidadeInicioELocalidadeTerminoPrestacao(carga.Codigo, 0, mdfePassagem.Destino.Codigo);
                    nfsParaEmissao.AddRange(nfsLocalidade);
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> nfsLocalidade = repXMLNotaFiscal.BuscarPorEstadoInicioEEstadoTerminoPrestacao(carga.Codigo, "", estadoTerminoPrestacao.Sigla);
                    nfsParaEmissao.AddRange(nfsLocalidade.Except(nfsParaEmissao));
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> nfsCarga = repXMLNotaFiscal.BuscarTodasPorCarga(carga.Codigo);
                nfsParaEmissao.AddRange(nfsCarga);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> nfsDoMDFe = nfsParaEmissao.OrderBy(o => o.Numero).Take(4000).ToList();
            return nfsDoMDFe;
        }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterCTesParaMDFeAquaviario(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Pedidos.Porto portoInicioPrestacao, Dominio.Entidades.Embarcador.Pedidos.Porto portoTerminoPrestacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesParaEmissao = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            //ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoDestino == portoTerminoPrestacao select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoOrigem == portoInicioPrestacao select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoPassagemUm == portoInicioPrestacao select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoPassagemDois == portoInicioPrestacao select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoPassagemTres == portoInicioPrestacao select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoPassagemQuatro == portoInicioPrestacao select obj.CTe).ToList());
            ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.PortoPassagemCinco == portoInicioPrestacao select obj.CTe).ToList());

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMDFe = ctes.Distinct().OrderBy(o => o.Numero).Take(4000).ToList();
            return ctesDoMDFe;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObteNotasParaMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas.ToList())
            {
                List<int> empresasOrigem = null;
                if (carga.EmpresaFilialEmissora != null && carga.EmiteMDFeFilialEmissora)
                    empresasOrigem = repCargaPedido.ObterEmpresasCargaFilialEmissora(carga.Codigo);
                else
                    empresasOrigem = repCargaPedido.ObterEmpresasCarga(carga.Codigo);

                foreach (int empresa in empresasOrigem)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCarga = repXMLNotaFiscal.BuscarPorCargaParaMDFe(carga.Codigo);
                    notas.AddRange(notasCarga);
                }
            }

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasParaMDFeManual = notas.Distinct().OrderBy(o => o.Numero).Take(4000).ToList();

            return notasParaMDFeManual;
        }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterCTesParaMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesParaEmissao = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            if (cargaMDFeManual.UsarDadosCTe)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas.ToList())
                {
                    List<int> empresasOrigem = null;
                    if (carga.EmpresaFilialEmissora != null && carga.EmiteMDFeFilialEmissora)
                        empresasOrigem = repCargaPedido.ObterEmpresasCargaFilialEmissora(carga.Codigo);
                    else
                    {
                        if (!configuracaoTMS.AgruparCargaAutomaticamente)
                            empresasOrigem = repCargaPedido.ObterEmpresasCarga(carga.Codigo);
                        else
                            empresasOrigem = repCargaPedido.ObterEmpresasCargaOriginal(carga.Codigo);
                    }

                    foreach (int empresa in empresasOrigem)
                    {
                        if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
                        {
                            if (!configuracaoTMS.AgruparCargaAutomaticamente)
                                ctes.AddRange(repCargaCTe.BuscarPorEstadoTerminoPrestacao(carga.Codigo, !carga.CargaTransbordo ? empresa : 0, estadoTerminoPrestacao.Sigla, tipoServicoMultisoftware));
                            else
                                ctes.AddRange(repCargaCTe.BuscarPorEstadoTerminoPrestacaoCargaOrigem(carga.Codigo, !carga.CargaTransbordo ? empresa : 0, estadoTerminoPrestacao.Sigla, tipoServicoMultisoftware));
                        }
                        else
                            ctes.AddRange(repCargaCTe.BuscarPorTipoModal(carga.Codigo, empresa, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario));
                    }
                }

                if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
                    ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null && obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla == estadoTerminoPrestacao.Sigla select obj.CTe).ToList());
                else
                    ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null select obj.CTe).ToList());
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas.ToList())
                {
                    List<int> empresasOrigem = null;
                    if (carga.EmpresaFilialEmissora != null && carga.EmiteMDFeFilialEmissora)
                        empresasOrigem = repCargaPedido.ObterEmpresasCargaFilialEmissora(carga.Codigo);
                    else
                        empresasOrigem = repCargaPedido.ObterEmpresasCarga(carga.Codigo);
                    foreach (int empresa in empresasOrigem)
                    {
                        if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCargaParaMDFe(carga.Codigo, empresa, tipoServicoMultisoftware, true, true, true, true);
                            ctes.AddRange((from obj in cargaCTEs select obj.CTe).ToList());
                        }
                        else
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCargaParaMDFePorTipoModal(carga.Codigo, empresa, tipoServicoMultisoftware, true, false, true, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);
                            ctes.AddRange((from obj in cargaCTEs select obj.CTe).ToList());
                        }
                    }
                }
                ctes.AddRange((from obj in cargaMDFeManual.CTes where obj.CTe != null select obj.CTe).ToList());
            }

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMDFe = ctes.Distinct().OrderBy(o => o.Numero).Take(4000).ToList();
            return ctesDoMDFe;
        }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterCTesParaMDFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.Embarcador.Logistica.MDFePassagens mdfePassagem, List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoEstado, bool emissaoComTransbordo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Estado estadoTerminoPrestacao, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork, bool gerarMDFeTransbordoSemConsiderarOrigem, bool permitirIncluirCTesDeDiferentesUFsEmMDFeUnico)
        {
            int codigoEmpresa = empresa.Codigo;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesParaEmissao = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (!permitirIncluirCTesDeDiferentesUFsEmMDFeUnico && (!gerarMDFeTransbordoSemConsiderarOrigem || !emissaoComTransbordo || cargaLocaisPrestacaoEstado.Count > 1) && carga.DadosSumarizados?.CargaTrecho != CargaTrechoSumarizada.SubCarga)
            {
                if (mdfePassagem.Origem != null && mdfePassagem.Destino != null)
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesLocalidade;
                    ctesLocalidade = repCargaCTe.BuscarPorLocalidadeTerminoPrestacao(carga.Codigo, !carga.CargaTransbordo ? codigoEmpresa : 0, mdfePassagem.Destino.Codigo, tipoServicoMultisoftware);
                    ctesParaEmissao.AddRange(ctesLocalidade);
                }
                else
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesLocalidade = repCargaCTe.BuscarPorEstadoTerminoPrestacao(carga.Codigo, !carga.CargaTransbordo ? codigoEmpresa : 0, estadoTerminoPrestacao.Sigla, tipoServicoMultisoftware);
                    ctesParaEmissao.AddRange(ctesLocalidade.Except(ctesParaEmissao));
                }
            }
            else
            {

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCargaParaMDFe(carga.Codigo, codigoEmpresa, tipoServicoMultisoftware, true, false, true, true);
                ctesParaEmissao.AddRange((from obj in cargaCTEs where obj.CTe.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao select obj.CTe).ToList());
            }

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMDFe = ctesParaEmissao.OrderBy(o => o.Numero).Take(4000).ToList();
            return ctesDoMDFe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, List<Dominio.Entidades.Usuario> motoristas, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, Dominio.Entidades.Localidade localidadeCarregamento, Dominio.Entidades.Localidade localidadeDescarregamento = null, Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = null, Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = null, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDescarregamento = null, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalCarregamento = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, Dominio.Enumeradores.TipoCargaMDFe? tipoCargaMDFE = null, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfes = null)
        {

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualSeguro repCargaMDFeManualSeguro = new Repositorio.Embarcador.Cargas.CargaMDFeManualSeguro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repCargaMDFeManualLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT repCargaMDFeManualCIOT = new Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repCargaMDFeManualPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio repCargaMDFeManualValePedagio = new Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio(unitOfWork);
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
            Repositorio.MDFeCIOT repMDFeCIOTe = new Repositorio.MDFeCIOT(unitOfWork);
            Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();

            if (cargaMDFeManual.TipoModalMDFe != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
            {
                IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursos = repCargaMDFeManualPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

                Dominio.Entidades.Estado ultimoEstado = percursos?.LastOrDefault()?.Estado;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso percurso in percursos)
                {
                    if ((percurso.Estado.Sigla == estadoTerminoPrestacao.Sigla && estadoTerminoPrestacao.Sigla != ultimoEstado.Sigla) || (estadoInicioPrestacao.Sigla == estadoTerminoPrestacao.Sigla))
                        break;

                    passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Posicao = percurso.Ordem, Sigla = percurso.Estado.Sigla });
                }
            }

            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio> cargaMDFeManualValePedagios = repCargaMDFeManualValePedagio.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagiosIntegrar = new List<Dominio.ObjetosDeValor.ValePedagioMDFe>();
            if (cargaMDFeManualValePedagios != null && cargaMDFeManualValePedagios.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio valePedagio in cargaMDFeManualValePedagios)
                {
                    Dominio.ObjetosDeValor.ValePedagioMDFe valePedagioMDFe = new Dominio.ObjetosDeValor.ValePedagioMDFe();
                    valePedagioMDFe.CNPJFornecedor = valePedagio.CNPJFornecedor;
                    valePedagioMDFe.CNPJResponsavel = valePedagio.CNPJResponsavel;
                    valePedagioMDFe.CodigoAgendamentoPorto = valePedagio.CodigoAgendamentoPorto;
                    valePedagioMDFe.NumeroComprovante = valePedagio.NumeroComprovante;
                    valePedagioMDFe.ValorValePedagio = valePedagio.ValorValePedagio;
                    valesPedagiosIntegrar.Add(valePedagioMDFe);
                }
            }
            else if (cargaMDFeManual.Cargas.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> listaValePedagioCarga = repCargaValePedagio.BuscarPorCarga(carga.Codigo, tipoServicoMultisoftware);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaValePedagio valePedagioCarga in listaValePedagioCarga)
                    {
                        Dominio.ObjetosDeValor.ValePedagioMDFe valePedagioMDFe = new Dominio.ObjetosDeValor.ValePedagioMDFe();
                        valePedagioMDFe.CNPJFornecedor = valePedagioCarga.Fornecedor?.CPF_CNPJ_SemFormato;
                        valePedagioMDFe.CNPJResponsavel = valePedagioCarga.Responsavel?.CPF_CNPJ_SemFormato;
                        valePedagioMDFe.NumeroComprovante = valePedagioCarga.NumeroComprovante;
                        valePedagioMDFe.ValorValePedagio = valePedagioCarga.Valor;
                        valesPedagiosIntegrar.Add(valePedagioMDFe);
                    }
                }
            }

            List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> segurosMDFe = new List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao>();
            if (!cargaMDFeManual.UsarSeguroCTe)
            {
                IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro> seguros = repCargaMDFeManualSeguro.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro seguro in seguros)
                {
                    Dominio.ObjetosDeValor.SeguroMDFeIntegracao seguroMDFeIntegracao = new Dominio.ObjetosDeValor.SeguroMDFeIntegracao();
                    seguroMDFeIntegracao.CNPJCPFResponsavel = seguro.Responsavel;
                    seguroMDFeIntegracao.CNPJSeguradora = seguro.CNPJSeguradora;
                    seguroMDFeIntegracao.NomeSeguradora = seguro.NomeSeguradora;
                    seguroMDFeIntegracao.NumeroApolice = seguro.NumeroApolice;
                    seguroMDFeIntegracao.NumeroAverbacao = seguro.NumeroAverbacao;
                    seguroMDFeIntegracao.Responsavel = seguro.TipoResponsavel;
                    segurosMDFe.Add(seguroMDFeIntegracao);
                }
            }
            else
            {
                if (cargaMDFeManual.CTes.Count == 0 && cargaMDFeManual.Cargas.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaMDFeManual.Cargas)
                    {
                        segurosMDFe.AddRange(repMDFe.BuscarSegurosParaMDFe(carga.Codigo));
                        SetarSegurosCTeSubcontratacao(ref segurosMDFe, carga.Codigo, tipoServicoMultisoftware, unitOfWork);
                        SetarAverbacaoPedido(ref segurosMDFe, carga.Codigo, unitOfWork);
                        SetarValePedagioPedido(ref valesPedagiosIntegrar, carga.Codigo, unitOfWork);
                    }
                }
            }

            List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = new List<Dominio.ObjetosDeValor.MDFe.Lacre>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre> cargaMDFeManualLacres = repCargaMDFeManualLacre.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre cargaMDFeManualLacre in cargaMDFeManualLacres)
            {
                Dominio.ObjetosDeValor.MDFe.Lacre lacre = new Dominio.ObjetosDeValor.MDFe.Lacre();
                lacre.Numero = cargaMDFeManualLacre.Numero;
                lacres.Add(lacre);
            }

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

            List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristasIntegracao = new List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao>();
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                Dominio.ObjetosDeValor.MotoristaMDFeIntegracao motoristaIntegracao = new Dominio.ObjetosDeValor.MotoristaMDFeIntegracao();
                motoristaIntegracao.CPF = motorista.CPF;
                motoristaIntegracao.Nome = motorista.Nome;
                motoristasIntegracao.Add(motoristaIntegracao);
            }

            Dominio.ObjetosDeValor.PortoMDFeIntegracao portoEmbarque = null;
            if (portoOrigem != null)
            {
                portoEmbarque = new Dominio.ObjetosDeValor.PortoMDFeIntegracao()
                {
                    Codigo = portoOrigem.Codigo,
                    Descricao = portoOrigem.Descricao
                };
            }
            else if (cargaMDFeManual.PortoOrigem != null)
            {
                portoEmbarque = new Dominio.ObjetosDeValor.PortoMDFeIntegracao()
                {
                    Codigo = cargaMDFeManual.PortoOrigem.Codigo,
                    Descricao = cargaMDFeManual.PortoOrigem.Descricao
                };
            }

            Dominio.ObjetosDeValor.PortoMDFeIntegracao portoDesembarque = null;
            if (portoDestino != null)
            {
                portoDesembarque = new Dominio.ObjetosDeValor.PortoMDFeIntegracao()
                {
                    Codigo = portoDestino.Codigo,
                    Descricao = portoDestino.Descricao
                };
            }
            else if (cargaMDFeManual.PortoDestino != null)
            {
                portoDesembarque = new Dominio.ObjetosDeValor.PortoMDFeIntegracao()
                {
                    Codigo = cargaMDFeManual.PortoDestino.Codigo,
                    Descricao = cargaMDFeManual.PortoDestino.Descricao
                };
            }

            Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao viagem = null;
            if (cargaMDFeManual.PedidoViagemNavio != null)
            {
                viagem = new Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao()
                {
                    Codigo = cargaMDFeManual.PedidoViagemNavio.Codigo,
                    Descricao = cargaMDFeManual.PedidoViagemNavio.Descricao
                };
            }

            List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisCarregamento = new List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao>();
            if (terminalCarregamento != null)
            {
                terminaisCarregamento.Add(new Dominio.ObjetosDeValor.TerminalMDFeIntegracao()
                {
                    Codigo = terminalCarregamento.Codigo,
                    Descricao = terminalCarregamento.Descricao
                }
                );
            }
            else if (cargaMDFeManual.TerminalCarregamento != null && cargaMDFeManual.TerminalCarregamento.Count > 0)
            {
                foreach (var terminal in cargaMDFeManual.TerminalCarregamento)
                {
                    terminaisCarregamento.Add(new Dominio.ObjetosDeValor.TerminalMDFeIntegracao()
                    {
                        Codigo = terminal.Codigo,
                        Descricao = terminal.Descricao
                    }
                    );
                }
            }
            List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisDescarregamento = new List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao>();
            if (terminalDescarregamento != null)
            {
                terminaisDescarregamento.Add(new Dominio.ObjetosDeValor.TerminalMDFeIntegracao()
                {
                    Codigo = terminalDescarregamento.Codigo,
                    Descricao = terminalDescarregamento.Descricao
                }
                );
            }
            else if (cargaMDFeManual.TerminalDescarregamento != null && cargaMDFeManual.TerminalDescarregamento.Count > 0)
            {
                foreach (var terminal in cargaMDFeManual.TerminalDescarregamento)
                {
                    terminaisDescarregamento.Add(new Dominio.ObjetosDeValor.TerminalMDFeIntegracao()
                    {
                        Codigo = terminal.Codigo,
                        Descricao = terminal.Descricao
                    }
                    );
                }
            }

            if (nfes?.Count > 0)
                mdfe = serMDFe.GerarMDFePorNFe(empresa, nfes, cargaMDFeManual.Cargas.Select(o => o.TipoOperacao).FirstOrDefault(), unitOfWork, null, passagens, "", lacres, localidadeCarregamento, localidadeDescarregamento, 0, "", veiculo, reboques, motoristasIntegracao, segurosMDFe.Count > 0 ? segurosMDFe : null, false, valesPedagiosIntegrar.Count > 0 ? valesPedagiosIntegrar : null, null, configuracaoTMS);
            else if (notas?.Count > 0)
                mdfe = serMDFe.GerarMDFePorNotasFiscais(empresa, notas, cargaMDFeManual.Cargas.Select(o => o.TipoOperacao).FirstOrDefault(), unitOfWork, null, passagens, "", lacres, localidadeCarregamento, localidadeDescarregamento, 0, "", veiculo, reboques, motoristasIntegracao, segurosMDFe.Count > 0 ? segurosMDFe : null, false, valesPedagiosIntegrar.Count > 0 ? valesPedagiosIntegrar : null, null, configuracaoTMS, null, cargaMDFeManual.Cargas.Select(obj => obj.Codigo).ToList());
            else
                mdfe = serMDFe.GerarMDFePorCTes(empresa, ctes, unitOfWork, null, passagens, "", lacres, localidadeCarregamento, localidadeDescarregamento, 0, "", veiculo, reboques, motoristasIntegracao, segurosMDFe.Count > 0 ? segurosMDFe : null, false, valesPedagiosIntegrar.Count > 0 ? valesPedagiosIntegrar : null, null, portoEmbarque, portoDesembarque, viagem, terminaisCarregamento, terminaisDescarregamento, configuracaoTMS);

            if (cargaMDFeManual.TipoModalMDFe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario)
                mdfe.Modal = repModalTransporte.BuscarPorNumero("03");
            else
                mdfe.Modal = repModalTransporte.BuscarPorNumero("01");

            if (repMDFeSeguro.ContarPorMDFe(mdfe.Codigo) <= 0)
            {

                Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();

                mdfeSeguro.MDFe = mdfe;
                mdfeSeguro.TipoResponsavel = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                             mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                             mdfe.Empresa.EmpresaPai?.Configuracao != null && mdfe.Empresa.EmpresaPai?.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                             mdfe.Empresa.EmpresaPai?.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                mdfeSeguro.Responsavel = mdfe.Empresa.CNPJ;

                mdfeSeguro.CNPJSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai?.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai?.Configuracao.CNPJSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && mdfe.Empresa.EmpresaPai?.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                mdfeSeguro.NomeSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai?.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai?.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                            mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && mdfe.Empresa.EmpresaPai?.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                mdfeSeguro.NumeroApolice = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                           mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai?.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                mdfeSeguro.NumeroAverbacao = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                             mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai?.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai?.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                             mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai?.Configuracao != null && mdfe.Empresa.EmpresaPai?.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(mdfeSeguro.NumeroApolice) ? mdfeSeguro.NumeroApolice : string.Empty;

                repMDFeSeguro.Inserir(mdfeSeguro);
            }

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && lacres.Count > 0)
                mdfe.ObservacaoContribuinte += " Lacres: " + string.Join(", ", lacres.Select(o => o.Numero)) + ".";

            List<Dominio.Entidades.MDFeCIOT> ciots = repMDFeCIOTe.BuscarPorMDFe(mdfe.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT CargaMDFeManualCIOT = repCargaMDFeManualCIOT.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo).OrderByDescending(x => x.ValorAdiantamento).FirstOrDefault();

            if (ciots.Count == 0 && CargaMDFeManualCIOT != null)
            {
                Dominio.Entidades.MDFeCIOT mDFeCIOT = new Dominio.Entidades.MDFeCIOT();
                mDFeCIOT.MDFe = mdfe;
                mDFeCIOT.NumeroCIOT = CargaMDFeManualCIOT.NumeroCIOT;
                mDFeCIOT.Responsavel = CargaMDFeManualCIOT.Responsavel;
                repMDFeCIOTe.Inserir(mDFeCIOT);
            }

            mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
            mdfe.ObservacaoFisco = cargaMDFeManual.ObservacaoFisco;
            mdfe.ObservacaoContribuinte = cargaMDFeManual.ObservacaoContribuinte;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaNcm = cargaMDFeManual.Cargas.FirstOrDefault() ?? cargaMDFeManual.CTes.FirstOrDefault().Carga;
            ObterProdutoPredominanteNCM(mdfe, cargaNcm, unitOfWork);

            new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unitOfWork).GerarInformacoesPagamentoMDFeManual(cargaMDFeManual, cargaNcm.Codigo, mdfe, ctes, valesPedagiosIntegrar, CargaMDFeManualCIOT);

            if (mdfe.ObservacaoContribuinte != null && !mdfe.ObservacaoContribuinte.Contains("Estados"))
            {
                Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unitOfWork);
                List<string> estados = repPercursoMDFe.BuscarSiglaEstadoPorMDFe(mdfe.Codigo);

                if (!string.IsNullOrWhiteSpace(mdfe.ObservacaoContribuinte))
                    mdfe.ObservacaoContribuinte += " / ";

                mdfe.ObservacaoContribuinte += "Estados de Passagem: " + string.Join(", ", estados) + ".";
            }

            repMDFe.Atualizar(mdfe);

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFe(int codigoCarga, bool emitidoPelaFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, List<Dominio.Entidades.Usuario> motoristas, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDoMDFe, bool emissaoComTransbordo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.PercursoEstado percursoEstado = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = null, Dominio.Entidades.Localidade localidadeCarregamento = null, Dominio.Entidades.Localidade localidadeDescarregamento = null, List<Dominio.ObjetosDeValor.MDFe.Lacre> lacres = null, string observacaoMDFe = "", Dominio.Enumeradores.TipoCargaMDFe? tipoCargaMDFE = null, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesAnteriores = null)
        {
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);
            Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unitOfWork);
            Repositorio.MotoristaMDFe repMotoristasMDFe = new Repositorio.MotoristaMDFe(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> segurosMDFe = null;
            if (ctes != null)
                segurosMDFe = repMDFe.BuscarSegurosParaMDFe(codigoCarga).ToList();// TODO: ToList cast
            else
                segurosMDFe = repMDFe.BuscarSegurosPedidosParaMDFe(codigoCarga).ToList();

            SetarSegurosCTeSubcontratacao(ref segurosMDFe, codigoCarga, tipoServicoMultisoftware, unitOfWork);
            SetarAverbacaoPedido(ref segurosMDFe, codigoCarga, unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> valesPedagios = repCargaValePedagio.BuscarPorCarga(codigoCarga, tipoServicoMultisoftware);
            List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagiosIntegrar = valesPedagios?.Select(o => new Dominio.ObjetosDeValor.ValePedagioMDFe()
            {
                CNPJFornecedor = o.Fornecedor?.CPF_CNPJ_SemFormato,
                CNPJResponsavel = o.Responsavel?.CPF_CNPJ_SemFormato,
                CodigoAgendamentoPorto = o.CodigoAgendamentoPorto,
                NumeroComprovante = o.NumeroComprovante,
                ValorValePedagio = o.Valor,
                QuantidadeEixos = o.QuantidadeEixos,
                TipoCompra = o.TipoCompra
            }).ToList();

            SetarValePedagioPedido(ref valesPedagiosIntegrar, codigoCarga, unitOfWork);

            List<Dominio.ObjetosDeValor.MDFe.CIOT> ciotsCarga = SetarCiotMDFe(codigoCarga, empresa, unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;
            if (ctes != null)
                mdfe = serMDFe.GerarMDFePorCTes(empresa, ctes, unitOfWork, percursoEstado, passagens, "", lacres, localidadeCarregamento, localidadeDescarregamento, 0, "", null, null, null, segurosMDFe, false, valesPedagiosIntegrar, null, null, null, null, null, null, configuracaoTMS, null, ciotsCarga);
            else if ((carga.TipoOperacao?.EmiteCTeFilialEmissora ?? false) && carga.UtilizarCTesAnterioresComoCTeFilialEmissora && ctesAnteriores != null)
                mdfe = serMDFe.GerarMDFePorCTesAnterior(empresa, ctesAnteriores, unitOfWork, percursoEstado, passagens, "", lacres, localidadeCarregamento, localidadeDescarregamento, 0, "", veiculo, reboques, motoristas, segurosMDFe, false, valesPedagiosIntegrar, null, null, null, null, null, null, configuracaoTMS, ciotsCarga);
            else
                mdfe = serMDFe.GerarMDFePorNotasFiscais(empresa, notasFiscaisDoMDFe, carga.TipoOperacao, unitOfWork, percursoEstado, passagens, "", lacres, localidadeCarregamento, localidadeDescarregamento, 0, "", null, null, null, segurosMDFe, false, valesPedagiosIntegrar, null, null, ciotsCarga, new List<int> { codigoCarga });

            if (tipoCargaMDFE != null)
                mdfe.TipoCargaMDFe = tipoCargaMDFE.Value;
            else
                mdfe.TipoCargaMDFe = Dominio.Enumeradores.TipoCargaMDFe.CargaGeral;

            ObterProdutoPredominanteNCM(mdfe, carga, unitOfWork);

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (lacres.Count > 0)
                {
                    mdfe.ObservacaoContribuinte += " Lacres: " + string.Join(", ", lacres.Select(o => o.Numero)) + ".";

                    repMDFe.Atualizar(mdfe);
                }
            }

            if (!string.IsNullOrWhiteSpace(observacaoMDFe))
            {
                if (!string.IsNullOrWhiteSpace(mdfe.ObservacaoContribuinte))
                    mdfe.ObservacaoContribuinte = string.Concat(observacaoMDFe, " / ", mdfe.ObservacaoContribuinte);
                else
                    mdfe.ObservacaoContribuinte = observacaoMDFe;
            }

            mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
            repMDFe.Atualizar(mdfe);

            Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);

            if (veiculoMDFe != null)
            {
                setarVeiculoMDFe(ref veiculoMDFe, veiculo, emitidoPelaFilialEmissora, carga.Empresa);
                repVeiculoMDFe.Atualizar(veiculoMDFe);
            }
            else
            {
                veiculoMDFe = new Dominio.Entidades.VeiculoMDFe();
                veiculoMDFe.MDFe = mdfe;
                setarVeiculoMDFe(ref veiculoMDFe, veiculo, emitidoPelaFilialEmissora, carga.Empresa);
                repVeiculoMDFe.Inserir(veiculoMDFe);
            }

            List<Dominio.Entidades.ReboqueMDFe> reboquesMDFe = repReboqueMDFe.BuscarPorMDFe(mdfe.Codigo);

            if (emissaoComTransbordo && reboquesMDFe != null)
            {
                foreach (Dominio.Entidades.ReboqueMDFe reboqueMDFe in reboquesMDFe)
                    repReboqueMDFe.Deletar(reboqueMDFe);

                reboquesMDFe = new List<Dominio.Entidades.ReboqueMDFe>();
            }

            if (reboques != null)
            {
                foreach (Dominio.Entidades.Veiculo veiculoVinculado in reboques)
                {
                    Dominio.Entidades.ReboqueMDFe reboqueMDF = (from obj in reboquesMDFe where obj.Placa == veiculoVinculado.Placa select obj).FirstOrDefault();

                    if (reboqueMDF != null)
                    {
                        setarReboqueMDFe(ref reboqueMDF, veiculoVinculado, emitidoPelaFilialEmissora);
                        repReboqueMDFe.Atualizar(reboqueMDF);
                    }
                    else
                    {
                        reboqueMDF = new Dominio.Entidades.ReboqueMDFe();
                        reboqueMDF.MDFe = mdfe;
                        setarReboqueMDFe(ref reboqueMDF, veiculoVinculado, emitidoPelaFilialEmissora);
                        repReboqueMDFe.Inserir(reboqueMDF);
                    }
                }
            }

            List<Dominio.Entidades.MotoristaMDFe> motoristasMDFe = repMotoristasMDFe.BuscarPorMDFe(mdfe.Codigo);

            if ((emissaoComTransbordo || carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga) && motoristasMDFe != null)
            {
                foreach (Dominio.Entidades.MotoristaMDFe motoristaMDFe in motoristasMDFe)
                    repMotoristasMDFe.Deletar(motoristaMDFe);

                motoristasMDFe = new List<Dominio.Entidades.MotoristaMDFe>();
            }

            if (motoristasMDFe.Count <= 0)
            {
                if (motoristas != null)
                {
                    foreach (Dominio.Entidades.Usuario motorista in motoristas)
                    {
                        Dominio.Entidades.MotoristaMDFe motoristaMDFe = new Dominio.Entidades.MotoristaMDFe();
                        motoristaMDFe.CPF = motorista.CPF;
                        motoristaMDFe.Nome = motorista.Nome;
                        motoristaMDFe.MDFe = mdfe;
                        motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal;
                        repMotoristasMDFe.Inserir(motoristaMDFe);
                    }
                }
            }

            new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unitOfWork).GerarInformacoesBancariasMDFe(carga, mdfe);

            //Servicos.Log.TratarErro("Retornando MDF-e carga: " + codigoCarga.ToString());

            return mdfe;
        }

        private void ObterProdutoPredominanteNCM(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrEmpty(mdfe.ProdutoPredominanteNCM) && carga != null)
            {
                (string codigoNCM, string descricao) = ("", "DIVERSOS");

                if (!string.IsNullOrWhiteSpace(carga?.TipoDeCarga?.NCM ?? "") && carga.TipoDeCarga.NCM != "0")
                    codigoNCM = carga.TipoDeCarga.NCM.ObterSomenteNumeros();
                else
                {
                    (codigoNCM, descricao) = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork).BuscarNCMProdutoMaiorValorPorCarga(carga.Codigo);

                    if (string.IsNullOrWhiteSpace(codigoNCM))
                        (codigoNCM, descricao) = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork).BuscarNCMProdutoMaiorValorPorCarga(carga.Codigo);
                }

                if (!string.IsNullOrWhiteSpace(codigoNCM))
                    mdfe.ProdutoPredominanteNCM = codigoNCM.ObterSomenteNumeros();

                mdfe.ProdutoPredominanteDescricao = descricao.Left(120);
            }
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFeManualPorNFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, List<Dominio.Entidades.Usuario> motoristas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualNFe repCargaMDFeManualNFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualNFe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> notasDoMDFe = repCargaMDFeManualNFe.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (notasDoMDFe == null || notasDoMDFe.Count == 0)
                return null;

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = GerarMDFe(cargaMDFeManual, tipoServicoMultisoftware, cargaMDFeManual.Empresa, cargaMDFeManual.Veiculo, cargaMDFeManual.Reboques.ToList(), motoristas, null, null, unitOfWork, estadoInicioPrestacao, estadoTerminoPrestacao, cargaMDFeManual.Origem, cargaMDFeManual.UsarListaDestinos() ? null : cargaMDFeManual.Destino, null, null, null, null, configuracaoTMS, tipoCargaMDFe, notasDoMDFe);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe()
            {
                CargaMDFeManual = cargaMDFeManual,
                MDFe = mdfe
            };
            repCargaMDFeManualMDFe.Inserir(cargaMDFeManualMDFe);

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFeManualPorNotas(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, List<Dominio.Entidades.Usuario> motoristas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDoMDFe = ObteNotasParaMDFe(cargaMDFeManual, tipoServicoMultisoftware, estadoInicioPrestacao, estadoTerminoPrestacao, configuracaoTMS, unitOfWork);

            if (notasDoMDFe.Count == 0)
                return null;

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = GerarMDFe(cargaMDFeManual, tipoServicoMultisoftware, cargaMDFeManual.Empresa, cargaMDFeManual.Veiculo, cargaMDFeManual.Reboques.ToList(), motoristas, null, notasDoMDFe, unitOfWork, estadoInicioPrestacao, estadoTerminoPrestacao, cargaMDFeManual.Origem, cargaMDFeManual.UsarListaDestinos() ? null : cargaMDFeManual.Destino, null, null, null, null, configuracaoTMS, tipoCargaMDFe);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe()
            {
                CargaMDFeManual = cargaMDFeManual,
                MDFe = mdfe
            };
            repCargaMDFeManualMDFe.Inserir(cargaMDFeManualMDFe);

            return mdfe;
        }

        private Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais GerarMDFeManualPorCTe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Estado estadoInicioPrestacao, Dominio.Entidades.Estado estadoTerminoPrestacao, List<Dominio.Entidades.Usuario> motoristas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMDFe = ObterCTesParaMDFe(cargaMDFeManual, tipoServicoMultisoftware, estadoInicioPrestacao, estadoTerminoPrestacao, configuracaoTMS, unitOfWork);

            if (ctesDoMDFe.Count == 0)
                return null;

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = GerarMDFe(cargaMDFeManual, tipoServicoMultisoftware, cargaMDFeManual.Empresa, cargaMDFeManual.Veiculo, cargaMDFeManual.Reboques.ToList(), motoristas, ctesDoMDFe, null, unitOfWork, estadoInicioPrestacao, estadoTerminoPrestacao, cargaMDFeManual.Origem, cargaMDFeManual.UsarListaDestinos() ? null : cargaMDFeManual.Destino, null, null, null, null, configuracaoTMS, tipoCargaMDFe);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe();
            cargaMDFeManualMDFe.CargaMDFeManual = cargaMDFeManual;
            cargaMDFeManualMDFe.MDFe = mdfe;

            repCargaMDFeManualMDFe.Inserir(cargaMDFeManualMDFe);

            return mdfe;
        }

        private void SetarSegurosCTeSubcontratacao(ref List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> segurosMDFe, int CodigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoDeTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesSubcontratacao = repCargaCte.BuscarCTesAnterioresPorCarga(CodigoCarga);
                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteSuccontratacao in ctesSubcontratacao)
                {
                    if (repAverbacaoCTe.ContarPorCTe(cteSuccontratacao.Codigo) == 0)
                    {
                        List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosAnteriores = repDocumentoDeTransporteAnteriorCTe.BuscarPorCTe(cteSuccontratacao.Codigo);
                        foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoAnterior in documentosAnteriores)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior = repCTe.BuscarPorChave(documentoAnterior.Chave);
                            if (cteAnterior != null)
                            {
                                List<Dominio.Entidades.AverbacaoCTe> averbacoesCte = repAverbacaoCTe.BuscarPorCTe(cteAnterior.Codigo);
                                foreach (Dominio.Entidades.AverbacaoCTe averbacaoCTe in averbacoesCte)
                                {
                                    if (averbacaoCTe.ApoliceSeguroAverbacao != null)
                                    {
                                        Dominio.ObjetosDeValor.SeguroMDFeIntegracao seguroMDFeIntegracao = new Dominio.ObjetosDeValor.SeguroMDFeIntegracao();
                                        seguroMDFeIntegracao.CNPJCPFResponsavel = averbacaoCTe.ApoliceSeguroAverbacao.ApoliceSeguro.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Transportador ? cteAnterior.Empresa.CNPJ_SemFormato : cteAnterior.TomadorPagador.CPF_CNPJ_SemFormato;
                                        seguroMDFeIntegracao.CNPJSeguradora = averbacaoCTe.ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora.ClienteSeguradora.CPF_CNPJ_SemFormato;
                                        seguroMDFeIntegracao.NomeSeguradora = averbacaoCTe.ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora.Nome;
                                        seguroMDFeIntegracao.NumeroApolice = averbacaoCTe.ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice;
                                        seguroMDFeIntegracao.NumeroAverbacao = averbacaoCTe.Averbacao;
                                        seguroMDFeIntegracao.Responsavel = Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante;
                                        segurosMDFe.Add(seguroMDFeIntegracao);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetarAverbacaoPedido(ref List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> segurosMDFe, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(codigoCarga);
            if (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0)
            {
                if (segurosMDFe == null)
                    segurosMDFe = new List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao averbacaoPedido in listaAverbacaoPedidos)
                {
                    Dominio.ObjetosDeValor.SeguroMDFeIntegracao seguroMDFe = new Dominio.ObjetosDeValor.SeguroMDFeIntegracao();
                    seguroMDFe.Responsavel = Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante;
                    seguroMDFe.CNPJCPFResponsavel = averbacaoPedido.CNPJResponsavel;
                    seguroMDFe.CNPJSeguradora = averbacaoPedido.CNPJSeguradora;
                    seguroMDFe.NomeSeguradora = averbacaoPedido.NomeSeguradora;
                    seguroMDFe.NumeroApolice = averbacaoPedido.NumeroApolice;
                    seguroMDFe.NumeroAverbacao = averbacaoPedido.NumeroAverbacao;
                    segurosMDFe.Add(seguroMDFe);
                }
            }
        }

        private void SetarValePedagioPedido(ref List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagioMDFe, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            //Pedagios dos pedidos estão sendo adicionados na Carga no FecharCarga,
            //Criado validação para só inserir os pedagios do pedido se não tiver vales já pedagios adicionados
            if (valesPedagioMDFe == null || valesPedagioMDFe.Count == 0)
            {
                Repositorio.Embarcador.Pedidos.PedidoValePedagio repPedidoValePedagio = new Repositorio.Embarcador.Pedidos.PedidoValePedagio(unitOfWork);
                IList<Dominio.ObjetosDeValor.MDFe.ValePedagio> listaValePedagio = repPedidoValePedagio.BuscarPorCarga(codigoCarga);
                if (listaValePedagio != null && listaValePedagio.Count > 0)
                {
                    if (valesPedagioMDFe == null)
                        valesPedagioMDFe = new List<Dominio.ObjetosDeValor.ValePedagioMDFe>();

                    foreach (Dominio.ObjetosDeValor.MDFe.ValePedagio valePedagio in listaValePedagio)
                    {
                        Dominio.ObjetosDeValor.ValePedagioMDFe valePedagioMDFe = new Dominio.ObjetosDeValor.ValePedagioMDFe();
                        valePedagioMDFe.CNPJResponsavel = valePedagio.CNPJResponsavel;
                        valePedagioMDFe.CNPJFornecedor = valePedagio.CNPJFornecedor;
                        valePedagioMDFe.NumeroComprovante = valePedagio.NumeroComprovante;
                        valePedagioMDFe.ValorValePedagio = valePedagio.ValorValePedagio;
                        valesPedagioMDFe.Add(valePedagioMDFe);
                    }
                }
            }
        }

        private void setarVeiculoMDFe(ref Dominio.Entidades.VeiculoMDFe veiculoMDFe, Dominio.Entidades.Veiculo veiculo, bool emitidoPelaFilialEmissora, Dominio.Entidades.Empresa empresa)
        {
            veiculoMDFe.CapacidadeKG = veiculo.CapacidadeKG;
            veiculoMDFe.CapacidadeM3 = veiculo.CapacidadeM3;
            veiculoMDFe.RENAVAM = veiculo.Renavam;
            veiculoMDFe.Placa = veiculo.Placa;
            veiculoMDFe.Tara = veiculo.Tara;
            veiculoMDFe.TipoCarroceria = veiculo.TipoCarroceria;
            veiculoMDFe.TipoRodado = veiculo.TipoRodado;
            veiculoMDFe.UF = veiculo.Estado;
            if (veiculo.Tipo == "T")
            {
                veiculoMDFe.CPFCNPJProprietario = veiculo.Proprietario?.CPF_CNPJ_SemFormato;
                veiculoMDFe.IEProprietario = veiculo.Proprietario?.IE_RG;

                if (veiculo.Proprietario != null)
                    veiculoMDFe.NomeProprietario = veiculo.Proprietario.Nome.Length > 60 ? veiculo.Proprietario.Nome.Substring(0, 60) : veiculo.Proprietario.Nome;

                veiculoMDFe.TipoProprietario = veiculo.TipoProprietario.ToString("d");
                veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                veiculoMDFe.UFProprietario = veiculo.Proprietario?.Localidade?.Estado;
            }
            else
            {
                veiculoMDFe.TipoProprietario = "";
                if (emitidoPelaFilialEmissora)
                {
                    if (veiculo.Empresa != null)
                    {
                        veiculoMDFe.CPFCNPJProprietario = veiculo.Empresa.CNPJ_SemFormato;
                        veiculoMDFe.IEProprietario = veiculo.Empresa.InscricaoEstadual;
                        veiculoMDFe.TipoProprietario = veiculo.Empresa.CNPJ == empresa.CNPJ ? "0" : "";
                        veiculoMDFe.NomeProprietario = veiculo.Empresa.RazaoSocial.Length > 60 ? veiculo.Empresa.RazaoSocial.Substring(0, 60) : veiculo.Empresa.RazaoSocial;
                        veiculoMDFe.RNTRC = string.Format("{0:00000000}", veiculo.Empresa.RegistroANTT);
                        veiculoMDFe.UFProprietario = veiculo.Empresa.Localidade.Estado;
                    }
                }
            }
        }

        private void setarReboqueMDFe(ref Dominio.Entidades.ReboqueMDFe reboqueMDF, Dominio.Entidades.Veiculo veiculo, bool emitidoPelaFilialEmissora)
        {
            reboqueMDF.CapacidadeKG = veiculo.CapacidadeKG;
            reboqueMDF.CapacidadeM3 = veiculo.CapacidadeM3;
            reboqueMDF.Tara = veiculo.Tara;
            reboqueMDF.TipoCarroceria = veiculo.TipoCarroceria;
            reboqueMDF.TipoProprietario = veiculo.TipoProprietario.ToString("d");
            reboqueMDF.UF = veiculo.Estado;
            reboqueMDF.RENAVAM = veiculo.Renavam;
            reboqueMDF.Placa = veiculo.Placa;

            if (veiculo.Tipo == "T")
            {
                reboqueMDF.RNTRC = string.Format("{0:00000000}", veiculo.RNTRC);
                reboqueMDF.CPFCNPJProprietario = veiculo.Proprietario?.CPF_CNPJ_SemFormato;
                reboqueMDF.IEProprietario = veiculo.Proprietario?.IE_RG;
                reboqueMDF.NomeProprietario = veiculo.Proprietario != null ? (veiculo.Proprietario.Nome.Length > 60 ? veiculo.Proprietario.Nome.Substring(0, 60) : veiculo.Proprietario.Nome) : "";
                reboqueMDF.UFProprietario = veiculo.Proprietario?.Localidade?.Estado;
            }
            else
            {
                if (emitidoPelaFilialEmissora)
                {
                    if (veiculo.Empresa != null)
                    {
                        reboqueMDF.CPFCNPJProprietario = veiculo.Empresa.CNPJ_SemFormato;
                        reboqueMDF.IEProprietario = veiculo.Empresa.InscricaoEstadual;
                        reboqueMDF.NomeProprietario = veiculo.Empresa.RazaoSocial.Length > 60 ? veiculo.Empresa.RazaoSocial.Substring(0, 60) : veiculo.Empresa.RazaoSocial;
                        reboqueMDF.RNTRC = string.Format("{0:00000000}", veiculo.Empresa.RegistroANTT);
                        reboqueMDF.UFProprietario = veiculo.Empresa.Localidade.Estado;
                    }
                }
            }
        }

        private List<Dominio.ObjetosDeValor.MDFe.CIOT> SetarCiotMDFe(int codigoCarga, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repcargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repcargaCIOT.BuscarPorCarga(codigoCarga);

            List<Dominio.ObjetosDeValor.MDFe.CIOT> ciots = new List<Dominio.ObjetosDeValor.MDFe.CIOT>();

            if (cargaCIOT != null &&
                !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Numero) &&
                (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto ||
                 cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem))
            {
                ciots.Add(new Dominio.ObjetosDeValor.MDFe.CIOT()
                {
                    Numero = cargaCIOT.CIOT.Numero,
                    CNPJCPFResponsavel = cargaCIOT.CIOT.Contratante.CNPJ_SemFormato
                });
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repPedido.BuscarPorCarga(codigoCarga);
            listaPedidos = (from obj in listaPedidos where !string.IsNullOrWhiteSpace(obj.CIOT) select obj).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
            {
                ciots.Add(new Dominio.ObjetosDeValor.MDFe.CIOT()
                {
                    Numero = pedido.CIOT,
                    CNPJCPFResponsavel = !string.IsNullOrWhiteSpace(pedido.ResponsavelCIOT) ? pedido.ResponsavelCIOT : pedido.Remetente != null ? pedido.Remetente.CPF_CNPJ_SemFormato : empresa.CNPJ_SemFormato
                });
            }

            return ciots;
        }

        private bool VerificarSeGeraMDFeTransbordoSemConsiderarOrigem(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                return carga.TipoOperacao.GerarMDFeTransbordoSemConsiderarOrigem;
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    return tomador.GerarMDFeTransbordoSemConsiderarOrigem;
                else if (tomador.GrupoPessoas != null)
                    return tomador.GrupoPessoas.GerarMDFeTransbordoSemConsiderarOrigem;
            }

            return false;
        }

        #endregion
    }
}
