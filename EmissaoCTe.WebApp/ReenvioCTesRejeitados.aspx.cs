using EmissaoCTe.API;
using System;
using System.Collections.Generic;

namespace EmissaoCTe.WebApp
{
    public partial class ReenvioCTesRejeitados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReenviarCTes_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {

                //string configAdicionarCTesFilaConsulta = ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"];
                //if (configAdicionarCTesFilaConsulta == null || configAdicionarCTesFilaConsulta == "")
                //    configAdicionarCTesFilaConsulta = "SIM";            

                //Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                //Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(Session["IdUsuario"] != null ? (int)Session["IdUsuario"] : 0);

                //if (usuario == null)
                //    throw new Exception("Usuário inválido.");

                //Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                //List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarTodosPorStatus("R", usuario.Empresa.Codigo);

                //Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);


                //for (var i = 0; i < ctes.Count; i++)
                //{
                //    if (svcCTe.Emitir(ctes[i].Codigo, usuario.Empresa.Codigo, unidadeDeTrabalho))
                //    {
                //        if (configAdicionarCTesFilaConsulta == "SIM")
                //            FilaConsultaCTe.GetInstance().QueueItem(5, ctes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                //    }
                //}



                //foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                //{
                //    if (svcCTe.Emitir(cte.Codigo, usuario.Empresa.Codigo, unidadeDeTrabalho))
                //    {
                //        FilaConsultaCTe.GetInstance().QueueItem(usuario.Empresa.Codigo, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                //    }
                //}

                //                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                //                List<int> codigos = new List<int>
                //            {
                //3433919,    3433922,    3433923,    3433925,    3433926,    3433927,    3433928,    3433929,    3433930,    3433931
                //            };

                //                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(codigos);

                //                DateTime.TryParseExact("30/07/2018 17:00", "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);

                //                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                //                for (var i = 0; i < ctes.Count; i++)
                //                {
                //                    if (ctes[i].Status == "S" || ctes[i].Status == "R")
                //                    {
                //                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(ctes[i].Codigo);

                //                        if (svcCTe.CalcularFretePorTabelaDeFrete(ref cte, cte.Empresa.Codigo, unidadeDeTrabalho, true))
                //                        {
                //                            cte.DataEmissao = dataEmissaoInicial;
                //                            repCTe.Atualizar(cte);

                //                            if (cte.CST != "90" && cte.LocalidadeInicioPrestacao.Estado.Sigla == "RJ")
                //                                Servicos.Log.TratarErro("CTe com tributação diferente de 90:" + cte.Numero, "LSTranslog");
                //                        }
                //                        else
                //                        {
                //                            Servicos.Log.TratarErro("Não foi possível calcular frete do CTe: " + cte.Numero, "LSTranslog");
                //                        }
                //                        unidadeDeTrabalho.FlushAndClear();
                //                    }
                //                    else
                //                    {
                //                        Servicos.Log.TratarErro("CTe já autorizado: " + ctes[i].Numero, "LSTranslog");
                //                    }
                //}
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        protected void btnReiniciarFilas_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            FilaConsultaCTe.GetNewInstance();
            FilaConsultaCTe.GetInstance().LimparListas();

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            List<Dominio.Entidades.NFSe> listaNFSes = repNFSe.BuscarPorStatus(new Dominio.Enumeradores.StatusNFSe[] { Dominio.Enumeradores.StatusNFSe.Enviado, Dominio.Enumeradores.StatusNFSe.EmCancelamento });
            for (var i = 0; i < listaNFSes.Count; i++)
                FilaConsultaCTe.GetInstance().QueueItem(5, listaNFSes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);

            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = repMDFe.BuscarPorStatus(new Dominio.Enumeradores.StatusMDFe[] { Dominio.Enumeradores.StatusMDFe.Enviado, Dominio.Enumeradores.StatusMDFe.EmCancelamento, Dominio.Enumeradores.StatusMDFe.EmEncerramento, Dominio.Enumeradores.StatusMDFe.EmitidoContingencia });
            for (var i = 0; i < listaMDFes.Count; i++)
            {
                if (listaMDFes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(5, listaMDFes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
            }

            List<Dominio.Entidades.CartaDeCorrecaoEletronica> listaCCes = repCCe.BuscarPorStatus(new Dominio.Enumeradores.StatusCCe[] { Dominio.Enumeradores.StatusCCe.Enviado });
            for (var i = 0; i < listaCCes.Count; i++)
            {
                if (listaCCes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(5, listaCCes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CCe, Conexao.StringConexao);
            }

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarTodosPorStatus(new string[] { "E", "K", "L", "X", "V", "B" });
            for (var i = 0; i < listaCTes.Count; i++)
            {
                if (listaCTes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(5, listaCTes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, unidadeDeTrabalho.StringConexao);
            }
        }


        protected void btnGerarCargasCTeGPA_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(Session["IdUsuario"] != null ? (int)Session["IdUsuario"] : 0);

                DateTime data = DateTime.Today.AddDays(-45);

                IList<int> listaCTes = repCTe.ConsultarCTeSemmCargaGPA(data, usuario.Empresa.Codigo);

                for (var i = 0; i < listaCTes.Count; i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i]);

                    if (cte != null)
                    {
                        string tipoVeiculo = "";
                        if (cte.ObservacoesGerais != null && cte.ObservacoesGerais.Contains("TIPO VEICULO:"))
                        {
                            int posicao = cte.ObservacoesGerais.IndexOf("TIPO VEICULO:");
                            int posicaoFim = cte.ObservacoesGerais.IndexOf("FRETE DESPESA:");
                            if (posicao > -1 && posicaoFim > -1)
                                tipoVeiculo = cte.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                        }
                        servicoCTe.GerarCargaCTe(cte.Codigo, "", "", tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, Conexao.StringConexao, unidadeDeTrabalho);
                        unidadeDeTrabalho.Dispose();
                        unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
                        repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    }
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        //protected void btnAjustarNumerosPagamentosCTe_Click(object sender, EventArgs e)
        //{
        //    Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
        //    try
        //    {
        //        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
        //        Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unidadeDeTrabalho);

        //        List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarTodas("");
        //        for (var i = 0; i < listaEmpresas.Count; i++)
        //        {
        //            List<Dominio.Entidades.PagamentoMotorista> listaPagamentoMotoristas = repPagamentoMotorista.BuscarPorEmpresaSemNumero(listaEmpresas[i].Codigo);
        //            for (var j = 0; j < listaPagamentoMotoristas.Count; j++)
        //            {
        //                listaPagamentoMotoristas[j].Numero = j + 1;
        //                repPagamentoMotorista.Atualizar(listaPagamentoMotoristas[j]);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        unidadeDeTrabalho.Dispose();
        //    }
        //}

        protected void btnAjustarNumerosPagamentosCTe_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(Session["IdUsuario"] != null ? (int)Session["IdUsuario"] : 0);

                List<int> codigos = new List<int>
                            {
51457,
51458
                            };

                for (var i = 0; i < codigos.Count; i++)
                {
                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigos[i]);
                    if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                    {
                        nfse.JustificativaCancelamento = "Cancelamento solicitado pela Coopercarga por falha de integração na Vigor.";
                        repNFSe.Atualizar(nfse);

                        if (svcNFSe.Cancelar(nfse.Codigo, unidadeDeTrabalho, true, usuario))
                            Servicos.Log.TratarErro("Solicito cancelamento NFSe " + nfse.Numero, "CancelamentoNFSes");
                        else
                            Servicos.Log.TratarErro("Não foi possível solicitar cancelamento NFSe " + nfse.Numero, "CancelamentoNFSes");

                        FilaConsultaCTe.GetInstance().QueueItem(3, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);

                    }
                    else
                        Servicos.Log.TratarErro("NFSe não esta autorizada: " + nfse.Numero, "CancelamentoNFSes");
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}