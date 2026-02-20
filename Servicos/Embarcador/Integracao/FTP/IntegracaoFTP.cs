using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.FTP
{
    public class IntegracaoFTP : ServicoBase
    {
        public IntegracaoFTP(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #region Métodos Públicos

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repositorioCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            if (cargaCancelamentoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Cliente tomador = cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Pedidos.First().ObterTomador();

                int codigoLayoutEDI = cargaCancelamentoIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.TipoOperacao;

                Dominio.Entidades.Empresa empresa = cargaCancelamentoIntegracaoEDI.Empresa;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
                if (configuracaoTransportador == null)
                {
                    if (tipoOperacao != null)
                        configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);
                }

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;

                if (configuracaoTipoOperacao == null && configuracaoTransportador == null)
                    configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;
                if (configuracaoTipoOperacao == null && configuracaoCliente == null && configuracaoTransportador == null && tomador.GrupoPessoas != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTransportador?.EnderecoFTP ?? (configuracaoTipoOperacao?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP));
                usuario = configuracaoTransportador?.Usuario ?? (configuracaoTipoOperacao?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario));
                senha = configuracaoTransportador?.Senha ?? (configuracaoTipoOperacao?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha));
                diretorio = configuracaoTransportador?.Diretorio ?? (configuracaoTipoOperacao?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio));
                porta = configuracaoTransportador?.Porta ?? (configuracaoTipoOperacao?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta));
                passivo = configuracaoTransportador?.Passivo ?? (configuracaoTipoOperacao?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false));
                utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? (configuracaoTipoOperacao?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false));
                ssl = configuracaoTransportador?.SSL ?? (configuracaoTipoOperacao?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false));
                criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? (configuracaoTipoOperacao?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false));
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;
                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(cargaCancelamentoIntegracaoEDI, _unitOfWork))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(cargaCancelamentoIntegracaoEDI, _unitOfWork);

                    if (cargaCancelamentoIntegracaoEDI.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        cargaCancelamentoIntegracaoEDI.IniciouConexaoExterna = true;
                        await repositorioCargaCancelamentoIntegracaoEDI.AtualizarAsync(cargaCancelamentoIntegracaoEDI);
                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + cargaCancelamentoIntegracaoEDI.LayoutEDI.Descricao + " para o FTP '" + url + "'.";
                cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaCancelamentoIntegracaoEDI.IniciouConexaoExterna = false;
                cargaCancelamentoIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                cargaCancelamentoIntegracaoEDI.DataIntegracao = DateTime.Now;
                cargaCancelamentoIntegracaoEDI.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDIIntegracao)
        {
            Repositorio.Embarcador.Avarias.LoteEDIIntegracao repositorioLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            if (loteEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = Avarias.LoteGrupoPessoasLayoutEDI.LayoutEDILote(loteEDIIntegracao.Lote);

                int codigoLayoutEDI = loteEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = (from o in layouts where o.LayoutEDI.Codigo == codigoLayoutEDI select o).FirstOrDefault();

                url = configuracaoGrupoPessoas?.EnderecoFTP ?? string.Empty;
                usuario = configuracaoGrupoPessoas?.Usuario ?? string.Empty;
                senha = configuracaoGrupoPessoas?.Senha ?? string.Empty;
                diretorio = configuracaoGrupoPessoas?.Diretorio ?? string.Empty;
                porta = configuracaoGrupoPessoas?.Porta ?? string.Empty;
                passivo = configuracaoGrupoPessoas?.Passivo ?? false;
                utilizarSFTP = configuracaoGrupoPessoas?.UtilizarSFTP ?? false;
                ssl = configuracaoGrupoPessoas?.SSL ?? false;
                criarComNomeTemporaraio = configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false;
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;
                string extensao = string.Empty;

                loteEDIIntegracao.DataIntegracao = DateTime.Now;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(loteEDIIntegracao, _unitOfWork))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteEDIIntegracao, _unitOfWork);

                    if (loteEDIIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        loteEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        loteEDIIntegracao.IniciouConexaoExterna = true;
                        await repositorioLoteEDIIntegracao.AtualizarAsync(loteEDIIntegracao);
                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            loteEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            loteEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                loteEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + loteEDIIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";
                loteEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                loteEDIIntegracao.IniciouConexaoExterna = false;
                loteEDIIntegracao.ProblemaIntegracao = mensagemErro;
                loteEDIIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao)
        {
            if (cargaEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                int codigoLayoutEDI = cargaEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Empresa empresa = cargaEDIIntegracao.Empresa;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);


                Dominio.Entidades.Cliente tomador = configuracaoTransportador == null ? cargaEDIIntegracao.Pedidos.First().ObterTomador() : null;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cargaEDIIntegracao.Carga.TipoOperacao;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
                if (tipoOperacao != null)
                    configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;

                if (configuracaoTipoOperacao == null && configuracaoTransportador == null)
                    configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;
                if (configuracaoTipoOperacao == null && configuracaoCliente == null && configuracaoTransportador == null && tomador.GrupoPessoas != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTransportador?.EnderecoFTP ?? (configuracaoTipoOperacao?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP));
                usuario = configuracaoTransportador?.Usuario ?? (configuracaoTipoOperacao?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario));
                senha = configuracaoTransportador?.Senha ?? (configuracaoTipoOperacao?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha));
                diretorio = configuracaoTransportador?.Diretorio ?? (configuracaoTipoOperacao?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio));
                porta = configuracaoTransportador?.Porta ?? (configuracaoTipoOperacao?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta));
                passivo = configuracaoTransportador?.Passivo ?? (configuracaoTipoOperacao?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false));
                utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? (configuracaoTipoOperacao?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false));
                ssl = configuracaoTransportador?.SSL ?? (configuracaoTipoOperacao?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false));
                criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? (configuracaoTipoOperacao?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false));
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;
                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(cargaEDIIntegracao, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(cargaEDIIntegracao, extensao, _unitOfWork);
                    if (cargaEDIIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        cargaEDIIntegracao.IniciouConexaoExterna = true;
                        await repositorioCargaEDIIntegracao.AtualizarAsync(cargaEDIIntegracao);
                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaEDIIntegracao.DestinoEnvio = url + "/" + diretorio;
                        }
                        else
                            cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + cargaEDIIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaEDIIntegracao.IniciouConexaoExterna = false;
                cargaEDIIntegracao.ProblemaIntegracao = mensagemErro;
                cargaEDIIntegracao.DataIntegracao = DateTime.Now;
                cargaEDIIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repositorioOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(_unitOfWork, _cancellationToken);

            if (ocorrenciaEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Cliente tomador = ocorrenciaEDIIntegracao.Pedidos.First().ObterTomador();
                Dominio.Entidades.Empresa empresa = ocorrenciaEDIIntegracao.Empresa;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.TipoOperacao;
                int codigoLayoutEDI = ocorrenciaEDIIntegracao.LayoutEDI.Codigo;

                ObterConfiguracoesConexaoFTP(tomador, empresa, tipoOperacao, codigoLayoutEDI, out url, out usuario, out senha, out diretorio, out porta, out passivo, out utilizarSFTP, out ssl, out criarComNomeTemporaraio, out certificado, _unitOfWork);

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(ocorrenciaEDIIntegracao, _unitOfWork))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(ocorrenciaEDIIntegracao, false, _unitOfWork);
                    if (ocorrenciaEDIIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        ocorrenciaEDIIntegracao.IniciouConexaoExterna = true;
                        await repositorioOcorrenciaEDIIntegracao.AtualizarAsync(ocorrenciaEDIIntegracao);
                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + ocorrenciaEDIIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                ocorrenciaEDIIntegracao.IniciouConexaoExterna = false;
                ocorrenciaEDIIntegracao.ProblemaIntegracao = mensagemErro;
                ocorrenciaEDIIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaEDIIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao)
        {
            if (nfsManualEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repositorioNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(_unitOfWork);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Cliente tomador = nfsManualEDIIntegracao.LancamentoNFSManual.Tomador;

                int codigoLayoutEDI = nfsManualEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportadorLayout = nfsManualEDIIntegracao.LancamentoNFSManual.Transportador.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);
                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null && tomador.GrupoPessoas != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTransportadorLayout?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP);
                usuario = configuracaoTransportadorLayout?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario);
                senha = configuracaoTransportadorLayout?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha);
                diretorio = configuracaoTransportadorLayout?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio);
                porta = configuracaoTransportadorLayout?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta);
                passivo = configuracaoTransportadorLayout?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false);
                utilizarSFTP = configuracaoTransportadorLayout?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false);
                ssl = configuracaoTransportadorLayout?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false);
                criarComNomeTemporaraio = configuracaoTransportadorLayout?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false);
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(nfsManualEDIIntegracao, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(nfsManualEDIIntegracao, extensao, _unitOfWork);
                    if (nfsManualEDIIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        nfsManualEDIIntegracao.IniciouConexaoExterna = true;
                        await repositorioNFSManualEDIIntegracao.AtualizarAsync(nfsManualEDIIntegracao);
                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + nfsManualEDIIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                nfsManualEDIIntegracao.IniciouConexaoExterna = false;
                nfsManualEDIIntegracao.ProblemaIntegracao = mensagemErro;
                nfsManualEDIIntegracao.DataIntegracao = DateTime.Now;
                nfsManualEDIIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI)
        {
            if (nfsManualCancelamentoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repositorioNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(_unitOfWork);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Cliente tomador = nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Tomador;

                int codigoLayoutEDI = nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportadorLayout = nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Transportador.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);
                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null && tomador.GrupoPessoas != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTransportadorLayout?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP);
                usuario = configuracaoTransportadorLayout?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario);
                senha = configuracaoTransportadorLayout?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha);
                diretorio = configuracaoTransportadorLayout?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio);
                porta = configuracaoTransportadorLayout?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta);
                passivo = configuracaoTransportadorLayout?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false);
                utilizarSFTP = configuracaoTransportadorLayout?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false);
                ssl = configuracaoTransportadorLayout?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false);
                criarComNomeTemporaraio = configuracaoTransportadorLayout?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false);
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(nfsManualCancelamentoIntegracaoEDI, _tipoServicoMultisoftware, _unitOfWork, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(nfsManualCancelamentoIntegracaoEDI, extensao, _unitOfWork);

                    if (nfsManualCancelamentoIntegracaoEDI.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        nfsManualCancelamentoIntegracaoEDI.IniciouConexaoExterna = true;

                        await repositorioNFSManualCancelamentoIntegracaoEDI.AtualizarAsync(nfsManualCancelamentoIntegracaoEDI);

                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";

                            nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                nfsManualCancelamentoIntegracaoEDI.IniciouConexaoExterna = false;
                nfsManualCancelamentoIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                nfsManualCancelamentoIntegracaoEDI.DataIntegracao = DateTime.Now;
                nfsManualCancelamentoIntegracaoEDI.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao LoteEscrituracaoEDIIntegracao)
        {
            if (LoteEscrituracaoEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repositorioLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {

                Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repositorioDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(_unitOfWork, _cancellationToken);

                Dominio.Entidades.Cliente tomador = LoteEscrituracaoEDIIntegracao.LoteEscrituracao.Tomador;
                Dominio.Entidades.Empresa empresa = LoteEscrituracaoEDIIntegracao.Empresa;

                int codigoLayoutEDI = LoteEscrituracaoEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;

                if (configuracaoTransportador == null)
                    configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                if (configuracaoCliente == null && configuracaoTransportador == null && tomador.GrupoPessoas != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);


                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = await repositorioDocumentoEscrituracao.ObterTipoOperacaoPadraoEscrituracaoAsync(LoteEscrituracaoEDIIntegracao.LoteEscrituracao.Codigo, _cancellationToken);
                tiposOperacao.AddRange(await repositorioDocumentoEscrituracao.ObterTipoOperacaoPadraoEscrituracaoOcorrenciaAsync(LoteEscrituracaoEDIIntegracao.LoteEscrituracao.Codigo, _cancellationToken));

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
                if (tiposOperacao.Count > 0)
                    configuracaoTipoOperacao = tiposOperacao.FirstOrDefault().LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTipoOperacao?.EnderecoFTP ?? (configuracaoTransportador?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP));
                usuario = configuracaoTipoOperacao?.Usuario ?? (configuracaoTransportador?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario));
                senha = configuracaoTipoOperacao?.Senha ?? (configuracaoTransportador?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha));
                diretorio = configuracaoTipoOperacao?.Diretorio ?? (configuracaoTransportador?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio));
                porta = configuracaoTipoOperacao?.Porta ?? (configuracaoTransportador?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta));
                passivo = configuracaoTipoOperacao?.Passivo ?? (configuracaoTransportador?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false));
                utilizarSFTP = configuracaoTipoOperacao?.UtilizarSFTP ?? (configuracaoTransportador?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false));
                ssl = configuracaoTipoOperacao?.SSL ?? (configuracaoTransportador?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false));
                criarComNomeTemporaraio = configuracaoTipoOperacao?.CriarComNomeTemporaraio ?? (configuracaoTransportador?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false));
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(LoteEscrituracaoEDIIntegracao, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(LoteEscrituracaoEDIIntegracao, extensao, _unitOfWork);

                    if (LoteEscrituracaoEDIIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        LoteEscrituracaoEDIIntegracao.IniciouConexaoExterna = true;
                        await repositorioLoteEscrituracaoEDIIntegracao.AtualizarAsync(LoteEscrituracaoEDIIntegracao);

                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + LoteEscrituracaoEDIIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                LoteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                LoteEscrituracaoEDIIntegracao.IniciouConexaoExterna = false;
                LoteEscrituracaoEDIIntegracao.ProblemaIntegracao = mensagemErro;
                LoteEscrituracaoEDIIntegracao.DataIntegracao = DateTime.Now;
                LoteEscrituracaoEDIIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao)
        {
            if (loteEscrituracaoCancelamentoEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repositorioLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repositorioDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(_unitOfWork, _cancellationToken);

                Dominio.Entidades.Cliente tomador = loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Tomador;
                Dominio.Entidades.Empresa empresa = loteEscrituracaoCancelamentoEDIIntegracao.Empresa;

                int codigoLayoutEDI = loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;
                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;

                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                if (configuracaoTransportador == null)
                    configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                if (configuracaoCliente == null && configuracaoTransportador == null && tomador.GrupoPessoas != null)
                    configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = await repositorioDocumentoEscrituracaoCancelamento.ObterTipoOperacaoPadraoEscrituracaoAsync(loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Codigo, _cancellationToken);
                tiposOperacao.AddRange(await repositorioDocumentoEscrituracaoCancelamento.ObterTipoOperacaoPadraoEscrituracaoOcorrenciaAsync(loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Codigo, _cancellationToken));

                if (tiposOperacao.Count > 0)
                    configuracaoTipoOperacao = tiposOperacao.FirstOrDefault().LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTipoOperacao?.EnderecoFTP ?? configuracaoTransportador?.EnderecoFTP ?? configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP;
                usuario = configuracaoTipoOperacao?.Usuario ?? configuracaoTransportador?.Usuario ?? configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario;
                senha = configuracaoTipoOperacao?.Senha ?? configuracaoTransportador?.Senha ?? configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha;
                diretorio = configuracaoTipoOperacao?.Diretorio ?? configuracaoTransportador?.Diretorio ?? configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio;
                porta = configuracaoTipoOperacao?.Porta ?? configuracaoTransportador?.Porta ?? configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta;
                passivo = configuracaoTipoOperacao?.Passivo ?? configuracaoTransportador?.Passivo ?? configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false;
                utilizarSFTP = configuracaoTipoOperacao?.UtilizarSFTP ?? configuracaoTransportador?.UtilizarSFTP ?? configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false;
                ssl = configuracaoTipoOperacao?.SSL ?? configuracaoTransportador?.SSL ?? configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false;
                criarComNomeTemporaraio = configuracaoTipoOperacao?.CriarComNomeTemporaraio ?? configuracaoTransportador?.CriarComNomeTemporaraio ?? configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false;
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(loteEscrituracaoCancelamentoEDIIntegracao, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteEscrituracaoCancelamentoEDIIntegracao, extensao, _unitOfWork);

                    if (loteEscrituracaoCancelamentoEDIIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        loteEscrituracaoCancelamentoEDIIntegracao.IniciouConexaoExterna = true;

                        await repositorioLoteEscrituracaoCancelamentoEDIIntegracao.AtualizarAsync(loteEscrituracaoCancelamentoEDIIntegracao);

                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                loteEscrituracaoCancelamentoEDIIntegracao.IniciouConexaoExterna = false;
                loteEscrituracaoCancelamentoEDIIntegracao.ProblemaIntegracao = mensagemErro;
                loteEscrituracaoCancelamentoEDIIntegracao.DataIntegracao = DateTime.Now;
                loteEscrituracaoCancelamentoEDIIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            if (faturaIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork, _cancellationToken);            
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = faturaIntegracao.Fatura;

                Dominio.Entidades.Empresa empresa = faturaIntegracao.Empresa;
                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
                if (fatura.TipoOperacao != null && configuracaoTransportador == null)
                    configuracaoTipoOperacao = fatura.TipoOperacao.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                if (fatura.Cliente != null && configuracaoTipoOperacao == null && configuracaoTransportador == null)
                {
                    if (fatura.Cliente.LayoutsEDI != null && fatura.Cliente.LayoutsEDI.Count() > 0)
                        configuracaoCliente = fatura.Cliente.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                    else if (fatura.Cliente.GrupoPessoas != null && fatura.Cliente.GrupoPessoas.LayoutsEDI != null && fatura.Cliente.GrupoPessoas.LayoutsEDI.Count() > 0)
                        configuracaoGrupoPessoas = (from o in fatura.Cliente.GrupoPessoas.LayoutsEDI where o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP select o).FirstOrDefault();
                }

                if (fatura.GrupoPessoas != null && fatura.GrupoPessoas.LayoutsEDI != null && fatura.GrupoPessoas.LayoutsEDI.Count() > 0)
                    configuracaoGrupoPessoas = (from o in fatura.GrupoPessoas.LayoutsEDI where o.LayoutEDI.Codigo == faturaIntegracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP select o).FirstOrDefault();


                url = configuracaoTransportador?.EnderecoFTP ?? (configuracaoTipoOperacao?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP));
                usuario = configuracaoTransportador?.Usuario ?? (configuracaoTipoOperacao?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario));
                senha = configuracaoTransportador?.Senha ?? (configuracaoTipoOperacao?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha));
                diretorio = configuracaoTransportador?.Diretorio ?? (configuracaoTipoOperacao?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio));
                porta = configuracaoTransportador?.Porta ?? (configuracaoTipoOperacao?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta));
                passivo = configuracaoTransportador?.Passivo ?? (configuracaoTipoOperacao?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false));
                utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? (configuracaoTipoOperacao?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false));
                ssl = configuracaoTransportador?.SSL ?? (configuracaoTipoOperacao?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false));
                criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? (configuracaoTipoOperacao?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false));
                certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(faturaIntegracao, _unitOfWork))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(faturaIntegracao, _unitOfWork, configuracaoTMS.UtilizaEmissaoMultimodal);
                    if (faturaIntegracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        faturaIntegracao.IniciouConexaoExterna = true;
                        await repositorioFaturaIntegracao.AtualizarAsync(faturaIntegracao);

                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            servicoCargaDadosSumarizados.AtualizarDadosCTesFaturadosIntegrados(faturaIntegracao.Fatura.Codigo, _unitOfWork);
                        }
                        else
                            faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + faturaIntegracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                faturaIntegracao.IniciouConexaoExterna = false;
                faturaIntegracao.MensagemRetorno = mensagemErro;
                faturaIntegracao.DataEnvio = DateTime.Now;
                faturaIntegracao.Tentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao integracao)
        {
            if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(_unitOfWork, _cancellationToken);
            Escrituracao.Provisao servicoProvisao = new Escrituracao.Provisao(_unitOfWork);
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> grupoPessoas = servicoProvisao.LayoutEDIProvisao(integracao.Provisao);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = grupoPessoas.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> clienteLayouts = servicoProvisao.LayoutEDIProvisaoCliente(integracao.Provisao);
            Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaocliente = clienteLayouts.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            string mensagemErro = "";
            string url = configuracaocliente?.EnderecoFTP ?? (configuracaoGrupoPessoas?.EnderecoFTP ?? string.Empty);
            string usuario = configuracaocliente?.Usuario ?? (configuracaoGrupoPessoas?.Usuario ?? string.Empty);
            string senha = configuracaocliente?.Senha ?? (configuracaoGrupoPessoas?.Senha ?? string.Empty);
            string diretorio = configuracaocliente?.Diretorio ?? (configuracaoGrupoPessoas?.Diretorio ?? string.Empty);
            string porta = configuracaocliente?.Porta ?? (configuracaoGrupoPessoas?.Porta ?? string.Empty);
            bool passivo = configuracaocliente?.Passivo ?? (configuracaoGrupoPessoas?.Passivo ?? false);
            bool utilizarSFTP = configuracaocliente?.UtilizarSFTP ?? (configuracaoGrupoPessoas?.UtilizarSFTP ?? false);
            bool ssl = configuracaocliente?.SSL ?? (configuracaoGrupoPessoas?.SSL ?? false);
            bool criarComNomeTemporaraio = configuracaocliente?.CriarComNomeTemporaraio ?? (configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false);
            string certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

            try
            {
                using (System.IO.MemoryStream edi = IntegracaoEDI.GerarEDI(integracao, _unitOfWork))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(integracao, incrementarSequencia: false, _unitOfWork);

                    if (integracao.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        integracao.IniciouConexaoExterna = true;
                        await repositorioProvisaoEDIIntegracao.AtualizarAsync(integracao);

                        if (Servicos.FTP.EnviarArquivo(edi, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + integracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                integracao.IniciouConexaoExterna = false;
                integracao.ProblemaIntegracao = mensagemErro;
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
            }

            await repositorioProvisaoEDIIntegracao.AtualizarAsync(integracao);
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao integracao)
        {
            if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repositorioCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(_unitOfWork, _cancellationToken);

            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Escrituracao.CancelamentoProvisao servicoCancelamentoProvisao = new Servicos.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> grupoPessoas = await servicoCancelamentoProvisao.LayoutEDICancelamentoProvisaoAsync(integracao.CancelamentoProvisao);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = grupoPessoas.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> cliente = await servicoCancelamentoProvisao.LayoutEDICancelamentoProvisaoClienteAsync(integracao.CancelamentoProvisao);

            Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = cliente.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);


            string mensagemErro = "";
            string url = configuracaoCliente?.EnderecoFTP ?? (configuracaoGrupoPessoas?.EnderecoFTP ?? string.Empty);
            string usuario = configuracaoCliente?.Usuario ?? (configuracaoGrupoPessoas?.Usuario ?? string.Empty);
            string senha = configuracaoCliente?.Senha ?? (configuracaoGrupoPessoas?.Senha ?? string.Empty);
            string diretorio = configuracaoCliente?.Diretorio ?? (configuracaoGrupoPessoas?.Diretorio ?? string.Empty);
            string porta = configuracaoCliente?.Porta ?? (configuracaoGrupoPessoas?.Porta ?? string.Empty);
            bool passivo = configuracaoCliente?.Passivo ?? (configuracaoGrupoPessoas?.Passivo ?? false);
            bool utilizarSFTP = configuracaoCliente?.UtilizarSFTP ?? (configuracaoGrupoPessoas?.UtilizarSFTP ?? false);
            bool ssl = configuracaoCliente?.SSL ?? (configuracaoGrupoPessoas?.SSL ?? false);
            bool criarComNomeTemporaraio = configuracaoCliente?.CriarComNomeTemporaraio ?? (configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false);
            string certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

            try
            {
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, false, _unitOfWork);
                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(integracao, _unitOfWork);
                if (integracao.IniciouConexaoExterna)
                {
                    mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    integracao.IniciouConexaoExterna = true;
                    await repositorioCancelamentoProvisaoEDIIntegracao.AtualizarAsync(integracao);
                    if (Servicos.FTP.EnviarArquivo(edi, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + integracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                integracao.IniciouConexaoExterna = false;
                integracao.ProblemaIntegracao = mensagemErro;
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
            }

            await repositorioCancelamentoProvisaoEDIIntegracao.AtualizarAsync(integracao);
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao integracao)
        {
            if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Escrituracao.Pagamento servicoPagamento = new Servicos.Embarcador.Escrituracao.Pagamento(_unitOfWork, _cancellationToken);

            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao reositoriopPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> grupoPessoas = await servicoPagamento.LayoutEDIPagamentoAsync(integracao.Pagamento);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = grupoPessoas.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> clienteLayoutEDI = await servicoPagamento.LayoutEDIPagamentoClienteAsync(integracao.Pagamento);

            Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoClienteLayoutEDI = clienteLayoutEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsTipoOperacao = Servicos.Embarcador.Escrituracao.Pagamento.ObterLayoutEDIsPagamentoTipoOperacao(integracao.Pagamento, _unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacaoLayoutEDI = layoutsTipoOperacao.FirstOrDefault(o => o.LayoutEDI.Codigo == integracao.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);


            string mensagemErro = "";
            string url = configuracaoClienteLayoutEDI?.EnderecoFTP ?? (configuracaoGrupoPessoas?.EnderecoFTP ?? (configuracaoTipoOperacaoLayoutEDI?.EnderecoFTP ?? string.Empty));
            string usuario = configuracaoClienteLayoutEDI?.Usuario ?? (configuracaoGrupoPessoas?.Usuario ?? (configuracaoTipoOperacaoLayoutEDI?.Usuario ?? string.Empty));
            string senha = configuracaoClienteLayoutEDI?.Senha ?? (configuracaoGrupoPessoas?.Senha ?? (configuracaoTipoOperacaoLayoutEDI?.Senha ?? string.Empty));
            string diretorio = configuracaoClienteLayoutEDI?.Diretorio ?? (configuracaoGrupoPessoas?.Diretorio ?? (configuracaoTipoOperacaoLayoutEDI?.Diretorio ?? string.Empty));
            string porta = configuracaoClienteLayoutEDI?.Porta ?? (configuracaoGrupoPessoas?.Porta ?? (configuracaoTipoOperacaoLayoutEDI?.Porta ?? string.Empty));
            bool passivo = configuracaoClienteLayoutEDI?.Passivo ?? (configuracaoGrupoPessoas?.Passivo ?? (configuracaoTipoOperacaoLayoutEDI?.Passivo ?? false));
            bool utilizarSFTP = configuracaoClienteLayoutEDI?.UtilizarSFTP ?? (configuracaoGrupoPessoas?.UtilizarSFTP ?? (configuracaoTipoOperacaoLayoutEDI?.UtilizarSFTP ?? false));
            bool ssl = configuracaoClienteLayoutEDI?.SSL ?? (configuracaoGrupoPessoas?.SSL ?? (configuracaoTipoOperacaoLayoutEDI?.SSL ?? false));
            bool criarComNomeTemporaraio = configuracaoClienteLayoutEDI?.CriarComNomeTemporaraio ?? (configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? (configuracaoTipoOperacaoLayoutEDI?.CriarComNomeTemporaraio ?? false));
            string certificado = configuracaoGrupoPessoas != null ? await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas) : string.Empty;

            try
            {
                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(integracao, _unitOfWork);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, false, _unitOfWork);

                if (integracao.IniciouConexaoExterna)
                {
                    mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    integracao.IniciouConexaoExterna = true;
                    await reositoriopPagamentoEDIIntegracao.AtualizarAsync(integracao);

                    if (Servicos.FTP.EnviarArquivo(edi, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio, certificado))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + integracao.LayoutEDI.Descricao + " para o FTP '" + url + "'.";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                integracao.IniciouConexaoExterna = false;
                integracao.ProblemaIntegracao = mensagemErro;
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
            }

            await reositoriopPagamentoEDIIntegracao.AtualizarAsync(integracao);
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI)
        {
            if (loteContabilizacaoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repositorioLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(_unitOfWork, _cancellationToken);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Empresa empresa = loteContabilizacaoIntegracaoEDI.Empresa;

                int codigoLayoutEDI = loteContabilizacaoIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;

                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTransportador?.EnderecoFTP;
                usuario = configuracaoTransportador?.Usuario;
                senha = configuracaoTransportador?.Senha;
                diretorio = configuracaoTransportador?.Diretorio;
                porta = configuracaoTransportador?.Porta;
                passivo = configuracaoTransportador?.Passivo ?? false;
                utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? false;
                ssl = configuracaoTransportador?.SSL ?? false;
                criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? false;

                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(loteContabilizacaoIntegracaoEDI, _tipoServicoMultisoftware, _unitOfWork, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteContabilizacaoIntegracaoEDI, extensao, _unitOfWork);

                    if (loteContabilizacaoIntegracaoEDI.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        loteContabilizacaoIntegracaoEDI.IniciouConexaoExterna = true;

                        await repositorioLoteContabilizacaoIntegracaoEDI.AtualizarAsync(loteContabilizacaoIntegracaoEDI);

                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + loteContabilizacaoIntegracaoEDI.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                loteContabilizacaoIntegracaoEDI.IniciouConexaoExterna = false;
                loteContabilizacaoIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                loteContabilizacaoIntegracaoEDI.DataIntegracao = DateTime.Now;
                loteContabilizacaoIntegracaoEDI.NumeroTentativas++;
            }
        }

        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI)
        {
            if (loteClienteIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repositorioLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(_unitOfWork, _cancellationToken);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            try
            {
                Dominio.Entidades.Empresa empresa = loteClienteIntegracaoEDI.Empresa;

                int codigoLayoutEDI = loteClienteIntegracaoEDI.LayoutEDI.Codigo;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;

                if (empresa != null)
                    configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

                url = configuracaoTransportador?.EnderecoFTP;
                usuario = configuracaoTransportador?.Usuario;
                senha = configuracaoTransportador?.Senha;
                diretorio = configuracaoTransportador?.Diretorio;
                porta = configuracaoTransportador?.Porta;
                passivo = configuracaoTransportador?.Passivo ?? false;
                utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? false;
                ssl = configuracaoTransportador?.SSL ?? false;
                criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? false;
                string extensao = string.Empty;

                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(loteClienteIntegracaoEDI, _unitOfWork, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteClienteIntegracaoEDI, extensao, _unitOfWork);

                    if (loteClienteIntegracaoEDI.IniciouConexaoExterna)
                    {
                        mensagemErro = "Não ouve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                        loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        loteClienteIntegracaoEDI.IniciouConexaoExterna = true;

                        await repositorioLoteClienteIntegracaoEDI.AtualizarAsync(loteClienteIntegracaoEDI);

                        if (Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio))
                        {
                            mensagemErro = "Envio realizado com sucesso.";
                            loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                            loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + loteClienteIntegracaoEDI.LayoutEDI.Descricao + " para o FTP '" + url + "'.";

                loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                loteClienteIntegracaoEDI.IniciouConexaoExterna = false;
                loteClienteIntegracaoEDI.ProblemaIntegracao = mensagemErro;
                loteClienteIntegracaoEDI.DataIntegracao = DateTime.Now;
                loteClienteIntegracaoEDI.NumeroTentativas++;
            }
        }

        /// <summary>
        /// Envia a integração FTP do ControleIntegracaoCargaEDI para transportador.
        /// </summary>
        public async Task EnviarEDIAsync(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEdi)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(_unitOfWork, _cancellationToken);

            if (controleIntegracaoCargaEdi.TipoIntegracao?.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP
                || controleIntegracaoCargaEdi.Transportador == null
                || controleIntegracaoCargaEdi.LayoutEDI == null
            )
            {
                controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
                controleIntegracaoCargaEdi.MensagemRetorno = "Não foi configurado nenhum tipo de integração para o arquivo";

                await repositorioControleIntegracaoCargaEDI.AtualizarAsync(controleIntegracaoCargaEdi);
                return;
            }

            Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = controleIntegracaoCargaEdi.Transportador?.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == controleIntegracaoCargaEdi.LayoutEDI.Codigo && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty;

            bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

            url = configuracaoTransportador?.EnderecoFTP;
            usuario = configuracaoTransportador?.Usuario;
            senha = configuracaoTransportador?.Senha;
            diretorio = configuracaoTransportador?.Diretorio;
            porta = configuracaoTransportador?.Porta;
            passivo = configuracaoTransportador?.Passivo ?? false;
            utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? false;
            ssl = configuracaoTransportador?.SSL ?? false;
            criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? false;

            try
            {
                Dominio.Entidades.Empresa empresa = controleIntegracaoCargaEdi.Transportador;
                int codigoLayoutEDI = controleIntegracaoCargaEdi.LayoutEDI.Codigo;

                foreach (var carga in controleIntegracaoCargaEdi.Cargas)
                {
                    Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = Servicos.Embarcador.Carga.CargaIntegracaoEDI.ConverterCargaEmNotFis(carga, controleIntegracaoCargaEdi.LayoutEDI, _unitOfWork);

                    Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(_unitOfWork, controleIntegracaoCargaEdi.LayoutEDI, null);
                    MemoryStream arquivoEDI = serGeracaoEDI.GerarArquivoRecursivo(notfis);

                    if (arquivoEDI == null)
                    {
                        throw new ServicoException("Arquivo EDI não encontrado");
                    }

                    if (Servicos.FTP.EnviarArquivo(arquivoEDI, controleIntegracaoCargaEdi.NomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, criarComNomeTemporaraio))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                    }
                    else
                    {
                        controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
                    }
                }
            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o EDI " + controleIntegracaoCargaEdi.LayoutEDI.Descricao + " para o FTP '" + url + "'.";
                controleIntegracaoCargaEdi.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
            }
            finally
            {
                controleIntegracaoCargaEdi.MensagemRetorno = mensagemErro;
                controleIntegracaoCargaEdi.NumeroTentativas++;
                await repositorioControleIntegracaoCargaEDI.AtualizarAsync(controleIntegracaoCargaEdi);
            }
        }

        public async Task EnviarCTeAsync(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao)
        {
            if (nfsManualCTeIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty;

            bool passivo = false, utilizarSFTP = false, ssl = false;

            try
            {
                Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP repostorioClienteIntegracaoFTP = new Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP repositorioGrupoPessoaIntegracaoFTP = new Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP(_unitOfWork, _cancellationToken);

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

                Dominio.Entidades.Cliente tomador = nfsManualCTeIntegracao.LancamentoNFSManual.Tomador;

                if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP configuracaoCliente = await repostorioClienteIntegracaoFTP.BuscarPorClienteAsync(tomador.CPF_CNPJ, _cancellationToken);

                    if (configuracaoCliente != null)
                    {
                        url = configuracaoCliente.EnderecoFTP;
                        usuario = configuracaoCliente.Usuario;
                        senha = configuracaoCliente.Senha;
                        diretorio = configuracaoCliente.Diretorio;
                        porta = configuracaoCliente.Porta;
                        passivo = configuracaoCliente.Passivo;
                        utilizarSFTP = configuracaoCliente.UtilizarSFTP;
                        ssl = configuracaoCliente.SSL;
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP configuracaoGrupoPessoas = await repositorioGrupoPessoaIntegracaoFTP.BuscarPorGrupoPessoasAsync(tomador.GrupoPessoas?.Codigo ?? 0, _cancellationToken);

                    if (configuracaoGrupoPessoas != null)
                    {
                        url = configuracaoGrupoPessoas.EnderecoFTP;
                        usuario = configuracaoGrupoPessoas.Usuario;
                        senha = configuracaoGrupoPessoas.Senha;
                        diretorio = configuracaoGrupoPessoas.Diretorio;
                        porta = configuracaoGrupoPessoas.Porta;
                        passivo = configuracaoGrupoPessoas.Passivo;
                        utilizarSFTP = configuracaoGrupoPessoas.UtilizarSFTP;
                        ssl = configuracaoGrupoPessoas.SSL;
                    }
                }

                Dominio.Entidades.XMLCTe xml = nfsManualCTeIntegracao.LancamentoNFSManual.CTe.XMLs.FirstOrDefault(o => o.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                byte[] data = servicoCTe.ObterXMLAutorizacao(xml.CTe, _unitOfWork);

                using (System.IO.MemoryStream msXML = new System.IO.MemoryStream(data))
                {
                    if (Servicos.FTP.EnviarArquivo(msXML, nfsManualCTeIntegracao.LancamentoNFSManual.CTe.Chave + "-aut.xml", url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o CT-e para o FTP '" + url + "'.";

                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                nfsManualCTeIntegracao.ProblemaIntegracao = mensagemErro;
                nfsManualCTeIntegracao.DataIntegracao = DateTime.Now;
                nfsManualCTeIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarCTeAsync(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe)
        {
            if (nfsManualCancelamentoIntegracaoCTe.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty;

            bool passivo = false, utilizarSFTP = false, ssl = false;

            try
            {
                Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP repositorioClienteIntegracaoFTP = new Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP repositorioGrupoPessoaIntegracaoFTP = new Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP(_unitOfWork);
                Servicos.CTe serCTe = new Servicos.CTe(_unitOfWork);

                Dominio.Entidades.Cliente tomador = nfsManualCancelamentoIntegracaoCTe.LancamentoNFSManual.Tomador;

                if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP configuracaoCliente = await repositorioClienteIntegracaoFTP.BuscarPorClienteAsync(tomador.CPF_CNPJ, _cancellationToken);
                    if (configuracaoCliente != null)
                    {
                        url = configuracaoCliente.EnderecoFTP;
                        usuario = configuracaoCliente.Usuario;
                        senha = configuracaoCliente.Senha;
                        diretorio = configuracaoCliente.Diretorio;
                        porta = configuracaoCliente.Porta;
                        passivo = configuracaoCliente.Passivo;
                        utilizarSFTP = configuracaoCliente.UtilizarSFTP;
                        ssl = configuracaoCliente.SSL;
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP configuracaoGrupoPessoas = await repositorioGrupoPessoaIntegracaoFTP.BuscarPorGrupoPessoasAsync(tomador.GrupoPessoas?.Codigo ?? 0, _cancellationToken);

                    if (configuracaoGrupoPessoas != null)
                    {
                        url = configuracaoGrupoPessoas.EnderecoFTP;
                        usuario = configuracaoGrupoPessoas.Usuario;
                        senha = configuracaoGrupoPessoas.Senha;
                        diretorio = configuracaoGrupoPessoas.Diretorio;
                        porta = configuracaoGrupoPessoas.Porta;
                        passivo = configuracaoGrupoPessoas.Passivo;
                        utilizarSFTP = configuracaoGrupoPessoas.UtilizarSFTP;
                        ssl = configuracaoGrupoPessoas.SSL;
                    }
                }

                Dominio.Entidades.XMLCTe xml = nfsManualCancelamentoIntegracaoCTe.LancamentoNFSManual.CTe.XMLs.FirstOrDefault(o => o.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                byte[] data = serCTe.ObterXMLAutorizacao(xml.CTe, _unitOfWork);

                using (System.IO.MemoryStream msXML = new System.IO.MemoryStream(data))
                {
                    if (Servicos.FTP.EnviarArquivo(msXML, nfsManualCancelamentoIntegracaoCTe.LancamentoNFSManual.CTe.Chave + "-aut.xml", url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o CT-e para o FTP '" + url + "'.";

                nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao = mensagemErro;
                nfsManualCancelamentoIntegracaoCTe.DataIntegracao = DateTime.Now;
                nfsManualCancelamentoIntegracaoCTe.NumeroTentativas++;
            }
        }

        public async Task EnviarCTeAsync(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            if (cargaCTeIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   nomenclaturaArquivo = string.Empty;

            bool passivo = false, utilizarSFTP = false, ssl = false;

            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP repositorioTipoOperacaoIntegracaoFTP = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP repositorioClienteIntegracaoFTP = new Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP repositorioGrupoPessoaIntegracaoFTP = new Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP(_unitOfWork, _cancellationToken);

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

                if (cargaCTeIntegracao.CargaCTe.Carga.TipoOperacao != null && cargaCTeIntegracao.CargaCTe.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP tipoOperacaoIntegracaoFTP = await repositorioTipoOperacaoIntegracaoFTP.BuscarPorTipoOperacaoAsync(cargaCTeIntegracao.CargaCTe.Carga.TipoOperacao.Codigo, _cancellationToken);

                    if (tipoOperacaoIntegracaoFTP != null)
                    {
                        url = tipoOperacaoIntegracaoFTP.EnderecoFTP;
                        usuario = tipoOperacaoIntegracaoFTP.Usuario;
                        senha = tipoOperacaoIntegracaoFTP.Senha;
                        diretorio = tipoOperacaoIntegracaoFTP.Diretorio;
                        porta = tipoOperacaoIntegracaoFTP.Porta;
                        passivo = tipoOperacaoIntegracaoFTP.Passivo;
                        nomenclaturaArquivo = tipoOperacaoIntegracaoFTP.NomenclaturaArquivo;
                        utilizarSFTP = tipoOperacaoIntegracaoFTP.UtilizarSFTP;
                        ssl = tipoOperacaoIntegracaoFTP.SSL;
                    }
                }
                else
                {
                    Dominio.Entidades.Cliente tomador = cargaCTeIntegracao.CargaCTe.Carga.Pedidos.Select(o => o.ObterTomador()).FirstOrDefault();

                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP configuracaoCliente = await repositorioClienteIntegracaoFTP.BuscarPorClienteAsync(tomador.CPF_CNPJ, _cancellationToken);

                        if (configuracaoCliente != null)
                        {
                            url = configuracaoCliente.EnderecoFTP;
                            usuario = configuracaoCliente.Usuario;
                            senha = configuracaoCliente.Senha;
                            diretorio = configuracaoCliente.Diretorio;
                            porta = configuracaoCliente.Porta;
                            passivo = configuracaoCliente.Passivo;
                            nomenclaturaArquivo = configuracaoCliente.NomenclaturaArquivo;
                            utilizarSFTP = configuracaoCliente.UtilizarSFTP;
                            ssl = configuracaoCliente.SSL;
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP configuracaoGrupoPessoas = await repositorioGrupoPessoaIntegracaoFTP.BuscarPorGrupoPessoasAsync(tomador.GrupoPessoas?.Codigo ?? 0, _cancellationToken);

                        if (configuracaoGrupoPessoas != null)
                        {
                            url = configuracaoGrupoPessoas.EnderecoFTP;
                            usuario = configuracaoGrupoPessoas.Usuario;
                            senha = configuracaoGrupoPessoas.Senha;
                            diretorio = configuracaoGrupoPessoas.Diretorio;
                            porta = configuracaoGrupoPessoas.Porta;
                            passivo = configuracaoGrupoPessoas.Passivo;
                            nomenclaturaArquivo = configuracaoGrupoPessoas.NomenclaturaArquivo;
                            utilizarSFTP = configuracaoGrupoPessoas.UtilizarSFTP;
                            ssl = configuracaoGrupoPessoas.SSL;
                        }
                    }
                }

                Dominio.Entidades.XMLCTe xml = cargaCTeIntegracao.CargaCTe.CTe.XMLs.FirstOrDefault(o => o.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                byte[] data = servicoCTe.ObterXMLAutorizacao(xml.CTe, _unitOfWork);

                using (System.IO.MemoryStream msXML = new System.IO.MemoryStream(data))
                {
                    string nomeArquivo = ObterNomeArquivo(xml.CTe, nomenclaturaArquivo);

                    if (Servicos.FTP.EnviarArquivo(msXML, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                }

            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o CT-e para o FTP '" + url + "'.";

                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaCTeIntegracao.ProblemaIntegracao = mensagemErro;
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;
            }
        }

        public async Task EnviarCTeAsync(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            if (ocorrenciaCTeIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   nomenclaturaArquivo = string.Empty;

            bool passivo = false, utilizarSFTP = false, ssl = false;

            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP repositorioTipoOperacaoIntegracaoFTP = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP repositorioClienteIntegracaoFTP = new Repositorio.Embarcador.Pessoas.ClienteIntegracaoFTP(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP repositorioGrupoPessoaIntegracaoFTP = new Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP(_unitOfWork, _cancellationToken);

                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

                if (ocorrenciaCTeIntegracao.CargaCTe.Carga.TipoOperacao != null && ocorrenciaCTeIntegracao.CargaCTe.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP tipoOperacaoIntegracaoFTP = await repositorioTipoOperacaoIntegracaoFTP.BuscarPorTipoOperacaoAsync(ocorrenciaCTeIntegracao.CargaCTe.Carga.TipoOperacao.Codigo, _cancellationToken);

                    if (tipoOperacaoIntegracaoFTP != null)
                    {
                        url = tipoOperacaoIntegracaoFTP.EnderecoFTP;
                        usuario = tipoOperacaoIntegracaoFTP.Usuario;
                        senha = tipoOperacaoIntegracaoFTP.Senha;
                        diretorio = tipoOperacaoIntegracaoFTP.Diretorio;
                        porta = tipoOperacaoIntegracaoFTP.Porta;
                        passivo = tipoOperacaoIntegracaoFTP.Passivo;
                        nomenclaturaArquivo = tipoOperacaoIntegracaoFTP.NomenclaturaArquivo;
                        utilizarSFTP = tipoOperacaoIntegracaoFTP.UtilizarSFTP;
                        ssl = tipoOperacaoIntegracaoFTP.SSL;
                    }
                }
                else
                {
                    Dominio.Entidades.Cliente tomador = ocorrenciaCTeIntegracao.CargaCTe.Carga.Pedidos.Select(o => o.ObterTomador()).FirstOrDefault();
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP configuracaoCliente = await repositorioClienteIntegracaoFTP.BuscarPorClienteAsync(tomador.CPF_CNPJ, _cancellationToken);

                        if (configuracaoCliente != null)
                        {
                            url = configuracaoCliente.EnderecoFTP;
                            usuario = configuracaoCliente.Usuario;
                            senha = configuracaoCliente.Senha;
                            diretorio = configuracaoCliente.Diretorio;
                            porta = configuracaoCliente.Porta;
                            passivo = configuracaoCliente.Passivo;
                            nomenclaturaArquivo = configuracaoCliente.NomenclaturaArquivo;
                            utilizarSFTP = configuracaoCliente.UtilizarSFTP;
                            ssl = configuracaoCliente.SSL;
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP configuracaoGrupoPessoas = await repositorioGrupoPessoaIntegracaoFTP.BuscarPorGrupoPessoasAsync(tomador.GrupoPessoas?.Codigo ?? 0, _cancellationToken);

                        if (configuracaoGrupoPessoas != null)
                        {
                            url = configuracaoGrupoPessoas.EnderecoFTP;
                            usuario = configuracaoGrupoPessoas.Usuario;
                            senha = configuracaoGrupoPessoas.Senha;
                            diretorio = configuracaoGrupoPessoas.Diretorio;
                            porta = configuracaoGrupoPessoas.Porta;
                            passivo = configuracaoGrupoPessoas.Passivo;
                            nomenclaturaArquivo = configuracaoGrupoPessoas.NomenclaturaArquivo;
                            utilizarSFTP = configuracaoGrupoPessoas.UtilizarSFTP;
                            ssl = configuracaoGrupoPessoas.SSL;
                        }
                    }
                }

                Dominio.Entidades.XMLCTe xml = ocorrenciaCTeIntegracao.CargaCTe.CTe.XMLs.FirstOrDefault(o => o.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                byte[] data = servicoCTe.ObterXMLAutorizacao(xml.CTe, _unitOfWork);

                using (System.IO.MemoryStream msXML = new System.IO.MemoryStream(data))
                {
                    string nomeArquivo = ObterNomeArquivo(ocorrenciaCTeIntegracao.CargaCTe.CTe, nomenclaturaArquivo);

                    if (Servicos.FTP.EnviarArquivo(msXML, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP))
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha genérica ao enviar o CT-e para o FTP '" + url + "'.";

                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                ocorrenciaCTeIntegracao.ProblemaIntegracao = mensagemErro;
                ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCTeIntegracao.NumeroTentativas++;
            }
        }

        public static void ObterConfiguracoesConexaoFTP(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, int codigoLayoutEDI, out string url, out string usuario, out string senha, out string diretorio, out string porta, out bool passivo, out bool utilizarSFTP, out bool ssl, out bool criarComNomeTemporaraio, out string certificado, Repositorio.UnitOfWork _unitOfWork)
        {
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork);

            url = string.Empty;
            usuario = string.Empty;
            senha = string.Empty;
            diretorio = string.Empty;
            porta = string.Empty;
            certificado = string.Empty;

            Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI configuracaoTransportador = null;
            if (empresa != null)
                configuracaoTransportador = empresa.TransportadorLayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI configuracaoTipoOperacao = null;
            if (configuracaoTransportador == null && tipoOperacao != null)
                configuracaoTipoOperacao = tipoOperacao.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI configuracaoCliente = null;
            if (configuracaoTipoOperacao == null && configuracaoTransportador == null)
                configuracaoCliente = tomador.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI configuracaoGrupoPessoas = null;
            if (configuracaoTipoOperacao == null && configuracaoCliente == null && configuracaoTransportador == null && tomador.GrupoPessoas != null)
                configuracaoGrupoPessoas = tomador.GrupoPessoas.LayoutsEDI.FirstOrDefault(o => o.LayoutEDI.Codigo == codigoLayoutEDI && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);

            url = configuracaoTransportador?.EnderecoFTP ?? (configuracaoTipoOperacao?.EnderecoFTP ?? (configuracaoCliente?.EnderecoFTP ?? configuracaoGrupoPessoas?.EnderecoFTP));
            usuario = configuracaoTransportador?.Usuario ?? (configuracaoTipoOperacao?.Usuario ?? (configuracaoCliente?.Usuario ?? configuracaoGrupoPessoas?.Usuario));
            senha = configuracaoTransportador?.Senha ?? (configuracaoTipoOperacao?.Senha ?? (configuracaoCliente?.Senha ?? configuracaoGrupoPessoas?.Senha));
            diretorio = configuracaoTransportador?.Diretorio ?? (configuracaoTipoOperacao?.Diretorio ?? (configuracaoCliente?.Diretorio ?? configuracaoGrupoPessoas?.Diretorio));
            porta = configuracaoTransportador?.Porta ?? (configuracaoTipoOperacao?.Porta ?? (configuracaoCliente?.Porta ?? configuracaoGrupoPessoas?.Porta));
            passivo = configuracaoTransportador?.Passivo ?? (configuracaoTipoOperacao?.Passivo ?? (configuracaoCliente?.Passivo ?? configuracaoGrupoPessoas?.Passivo ?? false));
            utilizarSFTP = configuracaoTransportador?.UtilizarSFTP ?? (configuracaoTipoOperacao?.UtilizarSFTP ?? (configuracaoCliente?.UtilizarSFTP ?? configuracaoGrupoPessoas?.UtilizarSFTP ?? false));
            ssl = configuracaoTransportador?.SSL ?? (configuracaoTipoOperacao?.SSL ?? (configuracaoCliente?.SSL ?? configuracaoGrupoPessoas?.SSL ?? false));
            criarComNomeTemporaraio = configuracaoTransportador?.CriarComNomeTemporaraio ?? (configuracaoTipoOperacao?.CriarComNomeTemporaraio ?? (configuracaoCliente?.CriarComNomeTemporaraio ?? configuracaoGrupoPessoas?.CriarComNomeTemporaraio ?? false));
            certificado = configuracaoGrupoPessoas != null ? servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(configuracaoGrupoPessoas).GetAwaiter().GetResult() : string.Empty;
        }

        public async Task EnviarCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            if (canhotoIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                return;

            if (canhotoIntegracao.Canhoto == null)
                return;

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto repositorioGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork, _cancellationToken);

            string mensagemErro = string.Empty,
                   url = string.Empty,
                   usuario = string.Empty,
                   senha = string.Empty,
                   diretorio = string.Empty,
                   porta = string.Empty,
                   certificado = string.Empty,
                   extensao = string.Empty,
                   extensaoArquivo = string.Empty;

            bool passivo, utilizarSFTP, ssl;

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto grupoPessoasFaturaCanhoto = (await repositorioGrupoPessoasFaturaCanhoto.BuscarPorGrupoPessoasAsync(canhotoIntegracao.Canhoto?.Emitente?.GrupoPessoas?.Codigo ?? 0, _cancellationToken)).FirstOrDefault();

                if (grupoPessoasFaturaCanhoto == null)
                    throw new ServicoException("Não foi possível encontrar configuração de envio de canhoto de fatura para o grupo " + (canhotoIntegracao.Canhoto?.Emitente?.GrupoPessoas.Descricao ?? ""));

                ObterConexaoFTPCanhoto(canhotoIntegracao.Canhoto, out url, out porta, out diretorio, out usuario, out senha, out passivo, out utilizarSFTP, out ssl, out certificado, _unitOfWork);
                if (!Servicos.FTP.TestarConexao(url, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizarSFTP, certificado))
                    throw new ServicoException("Problema na conexão com FTP: " + erro);

                canhotoIntegracao.DataIntegracao = DateTime.Now;
                int codigoCte = canhotoIntegracao.Canhoto.CargaCTe?.CTe.Codigo ?? canhotoIntegracao.Canhoto?.XMLNotaFiscal?.CTEs[0]?.Codigo ?? 0;
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repositorioCargaOcorrencia.BuscarOcorrenciaCanhoto(canhotoIntegracao.Canhoto.XMLNotaFiscal?.Codigo ?? 0,
                                                                                                                                      codigoCte,
                                                                                                                                      canhotoIntegracao.Canhoto.Carga?.Codigo ?? 0).FirstOrDefault();

                if (cargaOcorrencia == null)
                    throw new ServicoException($"Não foi possível localizar uma ocorrência para o canhoto {canhotoIntegracao.Canhoto.Numero}!");

                string nomeArquivo = IntegracaoEDI.ObterNomeArquivoCanhoto(canhotoIntegracao, grupoPessoasFaturaCanhoto.Nomenclatura ?? "",
                                                                            canhotoIntegracao.Canhoto.DataRecebimento ?? canhotoIntegracao.Canhoto.DataEmissao,
                                                                            canhotoIntegracao.Canhoto.Numero.ToString(),
                                                                            canhotoIntegracao.Canhoto.Empresa,
                                                                            canhotoIntegracao.Canhoto.Codigo,
                                                                            canhotoIntegracao.Canhoto.Destinatario,
                                                                            canhotoIntegracao.Canhoto.Carga?.Codigo ?? 0,
                                                                            cargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao ?? "0",
                                                                            cargaOcorrencia?.DataOcorrencia ?? DateTime.Now,
                                                                            canhotoIntegracao.Canhoto.XMLNotaFiscal?.Serie ?? "",
                                                                            canhotoIntegracao.Canhoto.XMLNotaFiscal?.Emitente?.CPF_CNPJ_SemFormato ?? "",
                                                                            canhotoIntegracao.Canhoto.XMLNotaFiscal?.Numero.ToString() ?? "");

                if (canhotoIntegracao.IniciouConexaoExterna)
                {
                    mensagemErro = "Não houve retorno do FTP após a tentativa de envio deste arquivo, por favor, verifique e se necessário reenvie o mesmo.";
                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    canhotoIntegracao.IniciouConexaoExterna = true;
                    await repositorioCanhotoIntegracao.AtualizarAsync(canhotoIntegracao);

                    System.IO.MemoryStream arquivoCanhoto = ObterArquivoCanhoto(canhotoIntegracao.Canhoto, out extensaoArquivo, _unitOfWork);

                    if (Servicos.FTP.EnviarArquivo(arquivoCanhoto, nomeArquivo + extensaoArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP))
                    {
                        mensagemErro = $"Envio realizado com sucesso para o arquivo '{nomeArquivo + extensaoArquivo}'.";
                        canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                        canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

            }
            catch (BaseException ex)
            {
                mensagemErro = ex.Message;

                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Falha ao enviar o canhoto para o FTP '" + url + "'.";

                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                canhotoIntegracao.ProblemaIntegracao = mensagemErro;
                canhotoIntegracao.DataIntegracao = DateTime.Now;
                canhotoIntegracao.NumeroTentativas++;
            }

            await repositorioCanhotoIntegracao.AtualizarAsync(canhotoIntegracao);
        }

        public static void ObterConexaoFTPCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, out string url, out string porta, out string diretorio, out string usuario, out string senha, out bool passivo, out bool utilizarSFTP, out bool ssl, out string certificado, Repositorio.UnitOfWork unitOfWork)
        {
            url = string.Empty;
            usuario = string.Empty;
            senha = string.Empty;
            diretorio = string.Empty;
            porta = string.Empty;
            certificado = string.Empty;

            Servicos.Embarcador.Pessoa.GrupoPessoasFaturaCanhoto servicoGrupoPessoaFaturaCanhoto = new Servicos.Embarcador.Pessoa.GrupoPessoasFaturaCanhoto(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto repGrupoPessoasCanhoto = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto configCanhoto = repGrupoPessoasCanhoto.BuscarPorGrupoPessoasAsync(canhoto.Emitente?.GrupoPessoas?.Codigo ?? 0, default).GetAwaiter().GetResult().FirstOrDefault();

            if ((configCanhoto == null) || !(configCanhoto.HabilitarEnvioCanhoto))
                throw new ServicoException("O envio de canhoto está desabilitado.");

            if (configCanhoto.TipoIntegracaoCanhoto == TipoIntegracaoCanhoto.FTP &&
                 (new[] { configCanhoto.EnderecoFTP, configCanhoto.Porta, configCanhoto.Usuario }.Any(string.IsNullOrWhiteSpace)))
                throw new ServicoException("Os dados de conexão do FTP devem estar preenchidos.");

            url = configCanhoto.EnderecoFTP;
            porta = configCanhoto.Porta;
            diretorio = configCanhoto.Diretorio;
            usuario = configCanhoto.Usuario;
            senha = configCanhoto.Senha;
            passivo = configCanhoto.Passivo;
            ssl = configCanhoto.SSL;
            utilizarSFTP = configCanhoto.UtilizarSFTP;
            certificado = servicoGrupoPessoaFaturaCanhoto.ObtemCertificadoChavePrivada(configCanhoto);
        }

        #endregion

        #region Métodos Privados

        private static string ObterNomeArquivo(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string nomenclatura)
        {
            if (!string.IsNullOrWhiteSpace(nomenclatura))
            {
                return nomenclatura.Replace("#ChaveCTe", cte.Chave)
                                   .Replace("#NumeroCTe", cte.Numero.ToString())
                                   .Replace("#SerieCTe", cte.Serie.Numero.ToString())
                                   .Replace("#CNPJEmissor", cte.Empresa?.CNPJ_SemFormato ?? "")
                                   .Replace("#CNPJTomador", cte.TomadorPagador?.CPF_CNPJ_SemFormato ?? "")
                                   .Replace("#NumeroBooking", cte.NumeroBooking)
                                   + ".xml";

                if (cte?.Status != "A")
                    nomenclatura = "Cancelado_" + nomenclatura;
            }
            else
            {
                return cte.Chave + "-aut.xml";
            }
        }

        private static System.IO.MemoryStream ObterArquivoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, out string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (canhoto == null)
                throw new ServicoException("Registro do canhoto não localizado");

            string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
            extensao = Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                throw new ServicoException("Arquivo não encontrado");

            byte[] bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            MemoryStream file = new MemoryStream(bufferCanhoto);

            return file;
        }

        #endregion
    }
}
