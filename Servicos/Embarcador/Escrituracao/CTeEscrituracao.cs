//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Servicos.Embarcador.Escrituracao
//{
//    public static class CTeEscrituracao
//    {
//        public static void AdicionarCTeParaEscrituracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
//        {
//            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
//            {
//                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

//                List<int> codigosCtes = repCargaCTe.BuscarCodigosCTePorCarga(carga.Codigo);
//                for (int i = 0; i < codigosCtes.Count(); i++)
//                {
//                    Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
//                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao();
//                    cTeEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoCTe.AgEscrituracao;
//                    cTeEscrituracao.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigosCtes[i] };
//                    cTeEscrituracao.Carga = carga;
//                    repCTeEscrituracao.Inserir(cTeEscrituracao);
//                }

//            }

//        }

//        public static void AdicionarCTeParaEscrituracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
//        {
//            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
//            {
//                // ver para gerar a escrituração para NFS
//                //Repositorio.Embarcador.Escrituracao.CTeEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.CTeEscrituracao(unitOfWork);

//                //Dominio.Entidades.Embarcador.Escrituracao.CTeEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.CTeEscrituracao();
//                //cTeEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoCTe.AgEscrituracao;
//                //cTeEscrituracao.CTe = lancamentoNFSManual.CTe;
//                //cTeEscrituracao.LancamentoNFSManual = lancamentoNFSManual;
//                //repCTeEscrituracao.Inserir(cTeEscrituracao);
//            }
//        }

//        public static void AdicionarCTeParaEscrituracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
//        {
//            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
//            {
//                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplentoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

//                List<int> codigosCtes = repCargaCTeComplentoInfo.BuscarCodigosCTePorOcorrencia(cargaOcorrencia.Codigo);
//                for (int i = 0; i < codigosCtes.Count(); i++)
//                {
//                    Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repCTeEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

//                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao cTeEscrituracao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao();
//                    cTeEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoCTe.AgEscrituracao;
//                    cTeEscrituracao.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigosCtes[i] };
//                    cTeEscrituracao.CargaOcorrencia = cargaOcorrencia;

//                    repCTeEscrituracao.Inserir(cTeEscrituracao);
//                }
//            }
//        }
//    }
//}
