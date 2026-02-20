using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SERVICO", EntityName = "Servico", Name = "Dominio.Entidades.Embarcador.NotaFiscal.Servico", NameType = typeof(Servico))]
    public class Servico : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.Servico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SER_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoNFE", Column = "SER_DESCRICAO_NFE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string DescricaoNFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "SER_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTributacao", Column = "SER_CODIGO_TRIBUTACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoTributacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorVenda", Column = "SER_VALOR_VENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "SER_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoServico", Column = "SER_CODIGO_SERVICO", TypeType = typeof(Dominio.Enumeradores.ListaServico), NotNull = false)]
        public virtual Dominio.Enumeradores.ListaServico CodigoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_VENDA_DENTRO_ESTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPVendaDentroEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_VENDA_FORA_ESTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPVendaForaEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_COMPRA_DENTRO_ESTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPCompraDentroEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_COMPRA_FORA_ESTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPCompraForaEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "SER_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NBS", Column = "SER_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNAE", Column = "SER_CNAE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CNAE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO_NFSE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ServicoNFSe ServicoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SER_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoStatus
        {
            get { return Status ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(Servico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoCodigoServico
        {
            get
            {
                switch (CodigoServico)
                {
                    case Enumeradores.ListaServico.LS101:
                        return "Análise e desenvolvimento de sistemas";
                    case Enumeradores.ListaServico.LS102:
                        return "Programação";
                    case Enumeradores.ListaServico.LS103:
                        return "Processamento de dados e congêneres";
                    case Enumeradores.ListaServico.LS104:
                        return "Elaboração de programas de computadores, inclusive de jogos eletrônicos";
                    case Enumeradores.ListaServico.LS105:
                        return "Licenciamento ou cessão de direito de uso de programas de computação";
                    case Enumeradores.ListaServico.LS106:
                        return "Assessoria e consultoria em informática";
                    case Enumeradores.ListaServico.LS107:
                        return "Suporte técnico em informática, inclusive instalação, configuração e manutenção de programas de computação e bancos de dados";
                    case Enumeradores.ListaServico.LS108:
                        return "Planejamento, confecção, manutenção e atualização de páginas eletrônicas";
                    case Enumeradores.ListaServico.LS201:
                        return "Serviços de pesquisas e desenvolvimento de qualquer natureza";
                    case Enumeradores.ListaServico.LS301:
                        return "(VETADO)";
                    case Enumeradores.ListaServico.LS302:
                        return "Cessão de direito de uso de marcas e de sinais de propaganda";
                    case Enumeradores.ListaServico.LS303:
                        return "Exploração de salões de festas, centro de convenções, escritórios virtuais, stands, quadras esportivas, estádios, ginásios, auditórios, casas de espetáculos, parques de diversões, canchas e congêneres, para realização de eventos ou negócios de qualquer natureza";
                    case Enumeradores.ListaServico.LS304:
                        return "Locação, sublocação, arrendamento, direito de passagem ou permissão de uso, compartilhado ou não, de ferrovia, rodovia, postes, cabos, dutos e condutos de qualquer natureza";
                    case Enumeradores.ListaServico.LS305:
                        return "Cessão de andaimes, palcos, coberturas e outras estruturas de uso temporário";
                    case Enumeradores.ListaServico.LS401:
                        return "Medicina e biomedicina";
                    case Enumeradores.ListaServico.LS402:
                        return "Análises clínicas, patologia, eletricidade médica, radioterapia, quimioterapia, ultra-sonografia, ressonância magnética, radiologia, tomografia e congêneres";
                    case Enumeradores.ListaServico.LS403:
                        return "Hospitais, clínicas, laboratórios, sanatórios, manicômios, casas de saúde, prontos-socorros, ambulatórios e congêneres";
                    case Enumeradores.ListaServico.LS404:
                        return "Instrumentação cirúrgica";
                    case Enumeradores.ListaServico.LS405:
                        return "Acupuntura";
                    case Enumeradores.ListaServico.LS406:
                        return "Enfermagem, inclusive serviços auxiliares";
                    case Enumeradores.ListaServico.LS407:
                        return "Serviços farmacêuticos";
                    case Enumeradores.ListaServico.LS408:
                        return "Terapia ocupacional, fisioterapia e fonoaudiologia";
                    case Enumeradores.ListaServico.LS409:
                        return "Terapias de qualquer espécie destinadas ao tratamento físico, orgânico e mental";
                    case Enumeradores.ListaServico.LS410:
                        return "Nutrição";
                    case Enumeradores.ListaServico.LS411:
                        return "Obstetrícia";
                    case Enumeradores.ListaServico.LS412:
                        return "Odontologia";
                    case Enumeradores.ListaServico.LS413:
                        return "Ortóptica";
                    case Enumeradores.ListaServico.LS414:
                        return "Próteses sob encomenda";
                    case Enumeradores.ListaServico.LS415:
                        return "Psicanálise";
                    case Enumeradores.ListaServico.LS416:
                        return "Psicologia";
                    case Enumeradores.ListaServico.LS417:
                        return "Casas de repouso e de recuperação, creches, asilos e congêneres";
                    case Enumeradores.ListaServico.LS418:
                        return "Inseminação artificial, fertilização in vitro e congêneres";
                    case Enumeradores.ListaServico.LS419:
                        return "Bancos de sangue, leite, pele, olhos, óvulos, sêmen e congêneres";
                    case Enumeradores.ListaServico.LS420:
                        return "Coleta de sangue, leite, tecidos, sêmen, órgãos e materiais biológicos de qualquer espécie";
                    case Enumeradores.ListaServico.LS421:
                        return "Unidade de atendimento, assistência ou tratamento móvel e congêneres";
                    case Enumeradores.ListaServico.LS422:
                        return "Planos de medicina de grupo ou individual e convênios para prestação de assistência médica, hospitalar, odontológica e congêneres";
                    case Enumeradores.ListaServico.LS423:
                        return "Outros planos de saúde que se cumpram através de serviços de terceiros contratados, credenciados, cooperados ou apenas pagos pelo operador do plano mediante indicação do beneficiário";
                    case Enumeradores.ListaServico.LS501:
                        return "Medicina veterinária e zootecnia";
                    case Enumeradores.ListaServico.LS502:
                        return "Hospitais, clínicas, ambulatórios, prontos-socorros e congêneres, na área veterinária";
                    case Enumeradores.ListaServico.LS503:
                        return "Laboratórios de análise na área veterinária";
                    case Enumeradores.ListaServico.LS504:
                        return "Inseminação artificial, fertilização in vitro e congêneres";
                    case Enumeradores.ListaServico.LS505:
                        return "Bancos de sangue e de órgãos e congêneres";
                    case Enumeradores.ListaServico.LS506:
                        return "Coleta de sangue, leite, tecidos, sêmen, órgãos e materiais biológicos de qualquer espécie";
                    case Enumeradores.ListaServico.LS507:
                        return "Unidade de atendimento, assistência ou tratamento móvel e congêneres";
                    case Enumeradores.ListaServico.LS508:
                        return "Guarda, tratamento, amestramento, embelezamento, alojamento e congêneres";
                    case Enumeradores.ListaServico.LS509:
                        return "Planos de atendimento e assistência médico-veterinária";
                    case Enumeradores.ListaServico.LS601:
                        return "Barbearia, cabeleireiros, manicuros, pedicuros e congêneres";
                    case Enumeradores.ListaServico.LS602:
                        return "Esteticistas, tratamento de pele, depilação e congêneres";
                    case Enumeradores.ListaServico.LS603:
                        return "Banhos, duchas, sauna, massagens e congêneres";
                    case Enumeradores.ListaServico.LS604:
                        return "Ginástica, dança, esportes, natação, artes marciais e demais atividades físicas";
                    case Enumeradores.ListaServico.LS605:
                        return "Centros de emagrecimento, spa e congêneres";
                    case Enumeradores.ListaServico.LS701:
                        return "Engenharia, agronomia, agrimensura, arquitetura, geologia, urbanismo, paisagismo e congêneres";
                    case Enumeradores.ListaServico.LS702:
                        return "Execução, por administração, empreitada ou subempreitada, de obras de construção civil, hidráulica ou elétrica e de outras obras semelhantes, inclusive sondagem, perfuração de poços, escavação, drenagem e irrigação, terraplanagem, pavimentação, concretagem e a instalação e montagem de produtos, peças e equipamentos (exceto o fornecimento de mercadorias produzidas pelo prestador de serviços fora do local da prestação dos serviços, que fica sujeito ao ICMS)";
                    case Enumeradores.ListaServico.LS703:
                        return "Elaboração de planos diretores, estudos de viabilidade, estudos organizacionais e outros, relacionados com obras e serviços de engenharia; elaboração de anteprojetos, projetos básicos e projetos executivos para trabalhos de engenharia";
                    case Enumeradores.ListaServico.LS704:
                        return "Demolição";
                    case Enumeradores.ListaServico.LS705:
                        return "Reparação, conservação e reforma de edifícios, estradas, pontes, portos e congêneres (exceto o fornecimento de mercadorias produzidas pelo prestador dos serviços, fora do local da prestação dos serviços, que fica sujeito ao ICMS)";
                    case Enumeradores.ListaServico.LS706:
                        return "Colocação e instalação de tapetes, carpetes, assoalhos, cortinas, revestimentos de parede, vidros, divisórias, placas de gesso e congêneres, com material fornecido pelo tomador do serviço";
                    case Enumeradores.ListaServico.LS707:
                        return "Recuperação, raspagem, polimento e lustração de pisos e congêneres";
                    case Enumeradores.ListaServico.LS708:
                        return "Calafetação";
                    case Enumeradores.ListaServico.LS709:
                        return "Varrição, coleta, remoção, incineração, tratamento, reciclagem, separação e destinação final de lixo, rejeitos e outros resíduos quaisquer";
                    case Enumeradores.ListaServico.LS710:
                        return "Limpeza, manutenção e conservação de vias e logradouros públicos, imóveis, chaminés, piscinas, parques, jardins e congêneres";
                    case Enumeradores.ListaServico.LS711:
                        return "Decoração e jardinagem, inclusive corte e poda de árvores";
                    case Enumeradores.ListaServico.LS712:
                        return "Controle e tratamento de efluentes de qualquer natureza e de agentes físicos, químicos e biológicos";
                    case Enumeradores.ListaServico.LS713:
                        return "Dedetização, desinfecção, desinsetização, imunização, higienização, desratização, pulverização e congêneres";
                    case Enumeradores.ListaServico.LS714:
                        return "(VETADO)";
                    case Enumeradores.ListaServico.LS715:
                        return "(VETADO)";
                    case Enumeradores.ListaServico.LS716:
                        return "Florestamento, reflorestamento, semeadura, adubação e congêneres";
                    case Enumeradores.ListaServico.LS717:
                        return "Escoramento, contenção de encostas e serviços congêneres";
                    case Enumeradores.ListaServico.LS718:
                        return "Limpeza e dragagem de rios, portos, canais, baías, lagos, lagoas, represas, açudes e congêneres";
                    case Enumeradores.ListaServico.LS719:
                        return "Acompanhamento e fiscalização da execução de obras de engenharia, arquitetura e urbanismo";
                    case Enumeradores.ListaServico.LS720:
                        return "Aerofotogrametria (inclusive interpretação), cartografia, mapeamento, levantamentos topográficos, batimétricos, geográficos, geodésicos, geológicos, geofísicos e congêneres";
                    case Enumeradores.ListaServico.LS721:
                        return "Pesquisa, perfuração, cimentação, mergulho, perfilagem, concretação, testemunhagem, pescaria, estimulação e outros serviços relacionados com a exploração e explotação de petróleo, gás natural e de outros recursos minerais";
                    case Enumeradores.ListaServico.LS722:
                        return "Nucleação e bombardeamento de nuvens e congêneres";
                    case Enumeradores.ListaServico.LS801:
                        return "Ensino regular pré-escolar, fundamental, médio e superior";
                    case Enumeradores.ListaServico.LS802:
                        return "Instrução, treinamento, orientação pedagógica e educacional, avaliação de conhecimentos de qualquer natureza";
                    case Enumeradores.ListaServico.LS901:
                        return "Hospedagem de qualquer natureza em hotéis, apart-service condominiais, flat, apart-hotéis, hotéis residência, residence-service, suite service, hotelaria marítima, motéis, pensões e congêneres; ocupação por temporada com fornecimento de serviço (o valor da alimentação e gorjeta, quando incluído no preço da diária, fica sujeito ao Imposto Sobre Serviços)";
                    case Enumeradores.ListaServico.LS902:
                        return "Agenciamento, organização, promoção, intermediação e execução de programas de turismo, passeios, viagens, excursões, hospedagens e congêneres";
                    case Enumeradores.ListaServico.LS903:
                        return "Guias de turismo";
                    case Enumeradores.ListaServico.LS1001:
                        return "Agenciamento, corretagem ou intermediação de câmbio, de seguros, de cartões de crédito, de planos de saúde e de planos de previdência privada";
                    case Enumeradores.ListaServico.LS1002:
                        return "Agenciamento, corretagem ou intermediação de títulos em geral, valores mobiliários e contratos quaisquer";
                    case Enumeradores.ListaServico.LS1003:
                        return "Agenciamento, corretagem ou intermediação de direitos de propriedade industrial, artística ou literária";
                    case Enumeradores.ListaServico.LS1004:
                        return "Agenciamento, corretagem ou intermediação de contratos de arrendamento mercantil (leasing), de franquia (franchising) e de faturização (factoring)";
                    case Enumeradores.ListaServico.LS1005:
                        return "Agenciamento, corretagem ou intermediação de bens móveis ou imóveis, não abrangidos em outros itens ou subitens, inclusive aqueles realizados no âmbito de Bolsas de Mercadorias e Futuros, por quaisquer meios";
                    case Enumeradores.ListaServico.LS1006:
                        return "Agenciamento marítimo";
                    case Enumeradores.ListaServico.LS1007:
                        return "Agenciamento de notícias";
                    case Enumeradores.ListaServico.LS1008:
                        return "Agenciamento de publicidade e propaganda, inclusive o agenciamento de veiculação por quaisquer meios";
                    case Enumeradores.ListaServico.LS1009:
                        return "Representação de qualquer natureza, inclusive comercial";
                    case Enumeradores.ListaServico.LS1010:
                        return "Distribuição de bens de terceiros";
                    case Enumeradores.ListaServico.LS1101:
                        return "Guarda e estacionamento de veículos terrestres automotores, de aeronaves e de embarcações";
                    case Enumeradores.ListaServico.LS1102:
                        return "Vigilância, segurança ou monitoramento de bens e pessoas";
                    case Enumeradores.ListaServico.LS1103:
                        return "Escolta, inclusive de veículos e cargas";
                    case Enumeradores.ListaServico.LS1104:
                        return "Armazenamento, depósito, carga, descarga, arrumação e guarda de bens de qualquer espécie";
                    case Enumeradores.ListaServico.LS1201:
                        return "Espetáculos teatrais";
                    case Enumeradores.ListaServico.LS1202:
                        return "Exibições cinematográficas";
                    case Enumeradores.ListaServico.LS1203:
                        return "Espetáculos circenses";
                    case Enumeradores.ListaServico.LS1204:
                        return "Programas de auditório";
                    case Enumeradores.ListaServico.LS1205:
                        return "Parques de diversões, centros de lazer e congêneres";
                    case Enumeradores.ListaServico.LS1206:
                        return "Boates, taxi-dancing e congêneres";
                    case Enumeradores.ListaServico.LS1207:
                        return "Shows, ballet, danças, desfiles, bailes, óperas, concertos, recitais, festivais e congêneres";
                    case Enumeradores.ListaServico.LS1208:
                        return "Feiras, exposições, congressos e congêneres";
                    case Enumeradores.ListaServico.LS1209:
                        return "Bilhares, boliches e diversões eletrônicas ou não";
                    case Enumeradores.ListaServico.LS1210:
                        return "Corridas e competições de animais";
                    case Enumeradores.ListaServico.LS1211:
                        return "Competições esportivas ou de destreza física ou intelectual, com ou sem a participação do espectador";
                    case Enumeradores.ListaServico.LS1212:
                        return "Execução de música";
                    case Enumeradores.ListaServico.LS1213:
                        return "Produção, mediante ou sem encomenda prévia, de eventos, espetáculos, entrevistas, shows, ballet, danças, desfiles, bailes, teatros, óperas, concertos, recitais, festivais e congêneres";
                    case Enumeradores.ListaServico.LS1214:
                        return "Fornecimento de música para ambientes fechados ou não, mediante transmissão por qualquer processo";
                    case Enumeradores.ListaServico.LS1215:
                        return "Desfiles de blocos carnavalescos ou folclóricos, trios elétricos e congêneres";
                    case Enumeradores.ListaServico.LS1216:
                        return "Exibição de filmes, entrevistas, musicais, espetáculos, shows, concertos, desfiles, óperas, competições esportivas, de destreza intelectual ou congêneres";
                    case Enumeradores.ListaServico.LS1217:
                        return "Recreação e animação, inclusive em festas e eventos de qualquer natureza";
                    case Enumeradores.ListaServico.LS1301:
                        return "(VETADO)";
                    case Enumeradores.ListaServico.LS1302:
                        return "Fonografia ou gravação de sons, inclusive trucagem, dublagem, mixagem e congêneres";
                    case Enumeradores.ListaServico.LS1303:
                        return "Fotografia e cinematografia, inclusive revelação, ampliação, cópia, reprodução, trucagem e congêneres";
                    case Enumeradores.ListaServico.LS1304:
                        return "Reprografia, microfilmagem e digitalização";
                    case Enumeradores.ListaServico.LS1305:
                        return "Composição gráfica, fotocomposição, clicheria, zincografia, litografia, fotolitografia";
                    case Enumeradores.ListaServico.LS1401:
                        return "Lubrificação, limpeza, lustração, revisão, carga e recarga, conserto, restauração, blindagem, manutenção e conservação de máquinas, veículos, aparelhos, equipamentos, motores, elevadores ou de qualquer objeto (exceto peças e partes empregadas, que ficam sujeitas ao ICMS)";
                    case Enumeradores.ListaServico.LS1402:
                        return "Assistência técnica";
                    case Enumeradores.ListaServico.LS1403:
                        return "Recondicionamento de motores (exceto peças e partes empregadas, que ficam sujeitas ao ICMS)";
                    case Enumeradores.ListaServico.LS1404:
                        return "Recauchutagem ou regeneração de pneus";
                    case Enumeradores.ListaServico.LS1405:
                        return "Restauração, recondicionamento, acondicionamento, pintura, beneficiamento, lavagem, secagem, tingimento, galvanoplastia, anodização, corte, recorte, polimento, plastificação e congêneres, de objetos quaisquer";
                    case Enumeradores.ListaServico.LS1406:
                        return "Instalação e montagem de aparelhos, máquinas e equipamentos, inclusive montagem industrial, prestados ao usuário final, exclusivamente com material por ele fornecido";
                    case Enumeradores.ListaServico.LS1407:
                        return "Colocação de molduras e congêneres";
                    case Enumeradores.ListaServico.LS1408:
                        return "Encadernação, gravação e douração de livros, revistas e congêneres";
                    case Enumeradores.ListaServico.LS1409:
                        return "Alfaiataria e costura, quando o material for fornecido pelo usuário final, exceto aviamento";
                    case Enumeradores.ListaServico.LS1410:
                        return "Tinturaria e lavanderia";
                    case Enumeradores.ListaServico.LS1411:
                        return "Tapeçaria e reforma de estofamentos em geral";
                    case Enumeradores.ListaServico.LS1412:
                        return "Funilaria e lanternagem";
                    case Enumeradores.ListaServico.LS1413:
                        return "Carpintaria e serralheria";
                    case Enumeradores.ListaServico.LS1501:
                        return "Administração de fundos quaisquer, de consórcio, de cartão de crédito ou débito e congêneres, de carteira de clientes, de cheques pré-datados e congêneres";
                    case Enumeradores.ListaServico.LS1502:
                        return "Abertura de contas em geral, inclusive conta-corrente, conta de investimentos e aplicação e caderneta de poupança, no País e no exterior, bem como a manutenção das referidas contas ativas e inativas";
                    case Enumeradores.ListaServico.LS1503:
                        return "Locação e manutenção de cofres particulares, de terminais eletrônicos, de terminais de atendimento e de bens e equipamentos em geral";
                    case Enumeradores.ListaServico.LS1504:
                        return "Fornecimento ou emissão de atestados em geral, inclusive atestado de idoneidade, atestado de capacidade financeira e congêneres";
                    case Enumeradores.ListaServico.LS1505:
                        return "Cadastro, elaboração de ficha cadastral, renovação cadastral e congêneres, inclusão ou exclusão no Cadastro de Emitentes de Cheques sem Fundos – CCF ou em quaisquer outros bancos cadastrais";
                    case Enumeradores.ListaServico.LS1506:
                        return "Emissão, reemissão e fornecimento de avisos, comprovantes e documentos em geral; abono de firmas; coleta e entrega de documentos, bens e valores; comunicação com outra agência ou com a administração central; licenciamento eletrônico de veículos; transferência de veículos; agenciamento fiduciário ou depositário; devolução de bens em custódia";
                    case Enumeradores.ListaServico.LS1507:
                        return "Acesso, movimentação, atendimento e consulta a contas em geral, por qualquer meio ou processo, inclusive por telefone, fac-símile, internet e telex, acesso a terminais de atendimento, inclusive vinte e quatro horas; acesso a outro banco e a rede compartilhada; fornecimento de saldo, extrato e demais informações relativas a contas em geral, por qualquer meio ou processo";
                    case Enumeradores.ListaServico.LS1508:
                        return "Emissão, reemissão, alteração, cessão, substituição, cancelamento e registro de contrato de crédito; estudo, análise e avaliação de operações de crédito; emissão, concessão, alteração ou contratação de aval, fiança, anuência e congêneres; serviços relativos a abertura de crédito, para quaisquer fins";
                    case Enumeradores.ListaServico.LS1509:
                        return "Arrendamento mercantil (leasing) de quaisquer bens, inclusive cessão de direitos e obrigações, substituição de garantia, alteração, cancelamento e registro de contrato, e demais serviços relacionados ao arrendamento mercantil (leasing)";
                    case Enumeradores.ListaServico.LS1510:
                        return "Serviços relacionados a cobranças, recebimentos ou pagamentos em geral, de títulos quaisquer, de contas ou carnês, de câmbio, de tributos e por conta de terceiros, inclusive os efetuados por meio eletrônico, automático ou por máquinas de atendimento; fornecimento de posição de cobrança, recebimento ou pagamento; emissão de carnês, fichas de compensação, impressos e documentos em geral";
                    case Enumeradores.ListaServico.LS1511:
                        return "Devolução de títulos, protesto de títulos, sustação de protesto, manutenção de títulos, reapresentação de títulos, e demais serviços a eles relacionados";
                    case Enumeradores.ListaServico.LS1512:
                        return "Custódia em geral, inclusive de títulos e valores mobiliários";
                    case Enumeradores.ListaServico.LS1513:
                        return "Serviços relacionados a operações de câmbio em geral, edição, alteração, prorrogação, cancelamento e baixa de contrato de câmbio; emissão de registro de exportação ou de crédito; cobrança ou depósito no exterior; emissão, fornecimento e cancelamento de cheques de viagem; fornecimento, transferência, cancelamento e demais serviços relativos a carta de crédito de importação, exportação e garantias recebidas; envio e recebimento de mensagens em geral relacionadas a operações de câmbio";
                    case Enumeradores.ListaServico.LS1514:
                        return "Fornecimento, emissão, reemissão, renovação e manutenção de cartão magnético, cartão de crédito, cartão de débito, cartão salário e congêneres";
                    case Enumeradores.ListaServico.LS1515:
                        return "Compensação de cheques e títulos quaisquer; serviços relacionados a depósito, inclusive depósito identificado, a saque de contas quaisquer, por qualquer meio ou processo, inclusive em terminais eletrônicos e de atendimento";
                    case Enumeradores.ListaServico.LS1516:
                        return "Emissão, reemissão, liquidação, alteração, cancelamento e baixa de ordens de pagamento, ordens de crédito e similares, por qualquer meio ou processo; serviços relacionados à transferência de valores, dados, fundos, pagamentos e similares, inclusive entre contas em geral";
                    case Enumeradores.ListaServico.LS1517:
                        return "Emissão, fornecimento, devolução, sustação, cancelamento e oposição de cheques quaisquer, avulso ou por talão";
                    case Enumeradores.ListaServico.LS1518:
                        return "Serviços relacionados a crédito imobiliário, avaliação e vistoria de imóvel ou obra, análise técnica e jurídica, emissão, reemissão, alteração, transferência e renegociação de contrato, emissão e reemissão do termo de quitação e demais serviços relacionados a crédito imobiliário";
                    case Enumeradores.ListaServico.LS1601:
                        return "Serviços de transporte de natureza municipal";
                    case Enumeradores.ListaServico.LS1701:
                        return "Assessoria ou consultoria de qualquer natureza, não contida em outros itens desta lista; análise, exame, pesquisa, coleta, compilação e fornecimento de dados e informações de qualquer natureza, inclusive cadastro e similares";
                    case Enumeradores.ListaServico.LS1702:
                        return "Datilografia, digitação, estenografia, expediente, secretaria em geral, resposta audível, redação, edição, interpretação, revisão, tradução, apoio e infra-estrutura administrativa e congêneres";
                    case Enumeradores.ListaServico.LS1703:
                        return "Planejamento, coordenação, programação ou organização técnica, financeira ou administrativa";
                    case Enumeradores.ListaServico.LS1704:
                        return "Recrutamento, agenciamento, seleção e colocação de mão-de-obra";
                    case Enumeradores.ListaServico.LS1705:
                        return "Fornecimento de mão-de-obra, mesmo em caráter temporário, inclusive de empregados ou trabalhadores, avulsos ou temporários, contratados pelo prestador de serviço";
                    case Enumeradores.ListaServico.LS1706:
                        return "Propaganda e publicidade, inclusive promoção de vendas, planejamento de campanhas ou sistemas de publicidade, elaboração de desenhos, textos e demais materiais publicitários";
                    case Enumeradores.ListaServico.LS1707:
                        return "(VETADO)";
                    case Enumeradores.ListaServico.LS1708:
                        return "Franquia (franchising)";
                    case Enumeradores.ListaServico.LS1709:
                        return "Perícias, laudos, exames técnicos e análises técnicas";
                    case Enumeradores.ListaServico.LS1710:
                        return "Planejamento, organização e administração de feiras, exposições, congressos e congêneres";
                    case Enumeradores.ListaServico.LS1711:
                        return "Organização de festas e recepções; bufê (exceto o fornecimento de alimentação e bebidas, que fica sujeito ao ICMS)";
                    case Enumeradores.ListaServico.LS1712:
                        return "Administração em geral, inclusive de bens e negócios de terceiros";
                    case Enumeradores.ListaServico.LS1713:
                        return "Leilão e congêneres";
                    case Enumeradores.ListaServico.LS1714:
                        return "Advocacia";
                    case Enumeradores.ListaServico.LS1715:
                        return "Arbitragem de qualquer espécie, inclusive jurídica";
                    case Enumeradores.ListaServico.LS1716:
                        return "Auditoria";
                    case Enumeradores.ListaServico.LS1717:
                        return "Análise de Organização e Métodos";
                    case Enumeradores.ListaServico.LS1718:
                        return "Atuária e cálculos técnicos de qualquer natureza";
                    case Enumeradores.ListaServico.LS1719:
                        return "Contabilidade, inclusive serviços técnicos e auxiliares";
                    case Enumeradores.ListaServico.LS1720:
                        return "Consultoria e assessoria econômica ou financeira";
                    case Enumeradores.ListaServico.LS1721:
                        return "Estatística";
                    case Enumeradores.ListaServico.LS1722:
                        return "Cobrança em geral";
                    case Enumeradores.ListaServico.LS1723:
                        return "Assessoria, análise, avaliação, atendimento, consulta, cadastro, seleção, gerenciamento de informações, administração de contas a receber ou a pagar e em geral, relacionados a operações de faturização (factoring)";
                    case Enumeradores.ListaServico.LS1724:
                        return "Apresentação de palestras, conferências, seminários e congêneres";
                    case Enumeradores.ListaServico.LS1801:
                        return "Serviços de regulação de sinistros vinculados a contratos de seguros; inspeção e avaliação de riscos para cobertura de contratos de seguros; prevenção e gerência de riscos seguráveis e congêneres";
                    case Enumeradores.ListaServico.LS1901:
                        return "Serviços de distribuição e venda de bilhetes e demais produtos de loteria, bingos, cartões, pules ou cupons de apostas, sorteios, prêmios, inclusive os decorrentes de títulos de capitalização e congêneres";
                    case Enumeradores.ListaServico.LS2001:
                        return "Serviços portuários, ferroportuários, utilização de porto, movimentação de passageiros, reboque de embarcações, rebocador escoteiro, atracação, desatracação, serviços de praticagem, capatazia, armazenagem de qualquer natureza, serviços acessórios, movimentação de mercadorias, serviços de apoio marítimo, de movimentação ao largo, serviços de armadores, estiva, conferência, logística e congêneres";
                    case Enumeradores.ListaServico.LS2002:
                        return "Serviços aeroportuários, utilização de aeroporto, movimentação de passageiros, armazenagem de qualquer natureza, capatazia, movimentação de aeronaves, serviços de apoio aeroportuários, serviços acessórios, movimentação de mercadorias, logística e congêneres";
                    case Enumeradores.ListaServico.LS2003:
                        return "Serviços de terminais rodoviários, ferroviários, metroviários, movimentação de passageiros, mercadorias, inclusive suas operações, logística e congêneres";
                    case Enumeradores.ListaServico.LS2101:
                        return "Serviços de registros públicos, cartorários e notariais";
                    case Enumeradores.ListaServico.LS2201:
                        return "Serviços de exploração de rodovia mediante cobrança de preço ou pedágio dos usuários, envolvendo execução de serviços de conservação, manutenção, melhoramentos para adequação de capacidade e segurança de trânsito, operação, monitoração, assistência aos usuários e outros serviços definidos em contratos, atos de concessão ou de permissão ou em normas oficiais";
                    case Enumeradores.ListaServico.LS2301:
                        return "Serviços de programação e comunicação visual, desenho industrial e congêneres";
                    case Enumeradores.ListaServico.LS2401:
                        return "Serviços de chaveiros, confecção de carimbos, placas, sinalização visual, banners, adesivos e congêneres";
                    case Enumeradores.ListaServico.LS2501:
                        return "Funerais, inclusive fornecimento de caixão, urna ou esquifes; aluguel de capela; transporte do corpo cadavérico; fornecimento de flores, coroas e outros paramentos; desembaraço de certidão de óbito; fornecimento de véu, essa e outros adornos; embalsamento, embelezamento, conservação ou restauração de cadáveres";
                    case Enumeradores.ListaServico.LS2502:
                        return "Cremação de corpos e partes de corpos cadavéricos";
                    case Enumeradores.ListaServico.LS2503:
                        return "Planos ou convênio funerários";
                    case Enumeradores.ListaServico.LS2504:
                        return "Manutenção e conservação de jazigos e cemitérios";
                    case Enumeradores.ListaServico.LS2601:
                        return "Serviços de coleta, remessa ou entrega de correspondências, documentos, objetos, bens ou valores, inclusive pelos correios e suas agências franqueadas; courrier e congêneres";
                    case Enumeradores.ListaServico.LS2701:
                        return "Serviços de assistência social";
                    case Enumeradores.ListaServico.LS2801:
                        return "Serviços de avaliação de bens e serviços de qualquer natureza";
                    case Enumeradores.ListaServico.LS2901:
                        return "Serviços de biblioteconomia";
                    case Enumeradores.ListaServico.LS3001:
                        return "Serviços de biologia, biotecnologia e química";
                    case Enumeradores.ListaServico.LS3101:
                        return "Serviços técnicos em edificações, eletrônica, eletrotécnica, mecânica, telecomunicações e congêneres";
                    case Enumeradores.ListaServico.LS3201:
                        return "Serviços de desenhos técnicos";
                    case Enumeradores.ListaServico.LS3301:
                        return "Serviços de desembaraço aduaneiro, comissários, despachantes e congêneres";
                    case Enumeradores.ListaServico.LS3401:
                        return "Serviços de investigações particulares, detetives e congêneres";
                    case Enumeradores.ListaServico.LS3501:
                        return "Serviços de reportagem, assessoria de imprensa, jornalismo e relações públicas";
                    case Enumeradores.ListaServico.LS3601:
                        return "Serviços de meteorologia";
                    case Enumeradores.ListaServico.LS3701:
                        return "Serviços de artistas, atletas, modelos e manequins";
                    case Enumeradores.ListaServico.LS3801:
                        return "Serviços de museologia";
                    case Enumeradores.ListaServico.LS3901:
                        return "Serviços de ourivesaria e lapidação (quando o material for fornecido pelo tomador do serviço)";
                    case Enumeradores.ListaServico.LS4001:
                        return "Obras de arte sob encomenda";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumeroCodigoServico
        {
            get
            {
                switch (CodigoServico)
                {
                    case Enumeradores.ListaServico.LS101:
                        return "01.01";
                    case Enumeradores.ListaServico.LS102:
                        return "01.02";
                    case Enumeradores.ListaServico.LS103:
                        return "01.03";
                    case Enumeradores.ListaServico.LS104:
                        return "01.04";
                    case Enumeradores.ListaServico.LS105:
                        return "01.05";
                    case Enumeradores.ListaServico.LS106:
                        return "01.06";
                    case Enumeradores.ListaServico.LS107:
                        return "01.07";
                    case Enumeradores.ListaServico.LS108:
                        return "01.08";
                    case Enumeradores.ListaServico.LS201:
                        return "02.01";
                    case Enumeradores.ListaServico.LS301:
                        return "03.01";
                    case Enumeradores.ListaServico.LS302:
                        return "03.02";
                    case Enumeradores.ListaServico.LS303:
                        return "03.03";
                    case Enumeradores.ListaServico.LS304:
                        return "03.04";
                    case Enumeradores.ListaServico.LS305:
                        return "03.05";
                    case Enumeradores.ListaServico.LS401:
                        return "04.01";
                    case Enumeradores.ListaServico.LS402:
                        return "04.02";
                    case Enumeradores.ListaServico.LS403:
                        return "04.03";
                    case Enumeradores.ListaServico.LS404:
                        return "04.04";
                    case Enumeradores.ListaServico.LS405:
                        return "04.05";
                    case Enumeradores.ListaServico.LS406:
                        return "04.06";
                    case Enumeradores.ListaServico.LS407:
                        return "04.07";
                    case Enumeradores.ListaServico.LS408:
                        return "04.08";
                    case Enumeradores.ListaServico.LS409:
                        return "04.09";
                    case Enumeradores.ListaServico.LS410:
                        return "04.10";
                    case Enumeradores.ListaServico.LS411:
                        return "04.11";
                    case Enumeradores.ListaServico.LS412:
                        return "04.12";
                    case Enumeradores.ListaServico.LS413:
                        return "04.13";
                    case Enumeradores.ListaServico.LS414:
                        return "04.14";
                    case Enumeradores.ListaServico.LS415:
                        return "04.15";
                    case Enumeradores.ListaServico.LS416:
                        return "04.16";
                    case Enumeradores.ListaServico.LS417:
                        return "04.17";
                    case Enumeradores.ListaServico.LS418:
                        return "04.18";
                    case Enumeradores.ListaServico.LS419:
                        return "04.19";
                    case Enumeradores.ListaServico.LS420:
                        return "04.20";
                    case Enumeradores.ListaServico.LS421:
                        return "04.21";
                    case Enumeradores.ListaServico.LS422:
                        return "04.22";
                    case Enumeradores.ListaServico.LS423:
                        return "04.23";
                    case Enumeradores.ListaServico.LS501:
                        return "05.01";
                    case Enumeradores.ListaServico.LS502:
                        return "05.02";
                    case Enumeradores.ListaServico.LS503:
                        return "05.03";
                    case Enumeradores.ListaServico.LS504:
                        return "05.04";
                    case Enumeradores.ListaServico.LS505:
                        return "05.05";
                    case Enumeradores.ListaServico.LS506:
                        return "05.06";
                    case Enumeradores.ListaServico.LS507:
                        return "05.07";
                    case Enumeradores.ListaServico.LS508:
                        return "05.08";
                    case Enumeradores.ListaServico.LS509:
                        return "05.09";
                    case Enumeradores.ListaServico.LS601:
                        return "06.01";
                    case Enumeradores.ListaServico.LS602:
                        return "06.02";
                    case Enumeradores.ListaServico.LS603:
                        return "06.03";
                    case Enumeradores.ListaServico.LS604:
                        return "06.04";
                    case Enumeradores.ListaServico.LS605:
                        return "06.05";
                    case Enumeradores.ListaServico.LS701:
                        return "07.01";
                    case Enumeradores.ListaServico.LS702:
                        return "07.02";
                    case Enumeradores.ListaServico.LS703:
                        return "07.03";
                    case Enumeradores.ListaServico.LS704:
                        return "07.04";
                    case Enumeradores.ListaServico.LS705:
                        return "07.05";
                    case Enumeradores.ListaServico.LS706:
                        return "07.06";
                    case Enumeradores.ListaServico.LS707:
                        return "07.07";
                    case Enumeradores.ListaServico.LS708:
                        return "07.08";
                    case Enumeradores.ListaServico.LS709:
                        return "07.09";
                    case Enumeradores.ListaServico.LS710:
                        return "07.10";
                    case Enumeradores.ListaServico.LS711:
                        return "07.11";
                    case Enumeradores.ListaServico.LS712:
                        return "07.12";
                    case Enumeradores.ListaServico.LS713:
                        return "07.13";
                    case Enumeradores.ListaServico.LS714:
                        return "07.14";
                    case Enumeradores.ListaServico.LS715:
                        return "07.15";
                    case Enumeradores.ListaServico.LS716:
                        return "07.16";
                    case Enumeradores.ListaServico.LS717:
                        return "07.17";
                    case Enumeradores.ListaServico.LS718:
                        return "07.18";
                    case Enumeradores.ListaServico.LS719:
                        return "07.19";
                    case Enumeradores.ListaServico.LS720:
                        return "07.20";
                    case Enumeradores.ListaServico.LS721:
                        return "07.21";
                    case Enumeradores.ListaServico.LS722:
                        return "07.22";
                    case Enumeradores.ListaServico.LS801:
                        return "08.01";
                    case Enumeradores.ListaServico.LS802:
                        return "08.02";
                    case Enumeradores.ListaServico.LS901:
                        return "09.01";
                    case Enumeradores.ListaServico.LS902:
                        return "09.02";
                    case Enumeradores.ListaServico.LS903:
                        return "09.03";
                    case Enumeradores.ListaServico.LS1001:
                        return "10.01";
                    case Enumeradores.ListaServico.LS1002:
                        return "10.02";
                    case Enumeradores.ListaServico.LS1003:
                        return "10.03";
                    case Enumeradores.ListaServico.LS1004:
                        return "10.04";
                    case Enumeradores.ListaServico.LS1005:
                        return "10.05";
                    case Enumeradores.ListaServico.LS1006:
                        return "10.06";
                    case Enumeradores.ListaServico.LS1007:
                        return "10.07";
                    case Enumeradores.ListaServico.LS1008:
                        return "10.08";
                    case Enumeradores.ListaServico.LS1009:
                        return "10.09";
                    case Enumeradores.ListaServico.LS1010:
                        return "10.10";
                    case Enumeradores.ListaServico.LS1101:
                        return "11.01";
                    case Enumeradores.ListaServico.LS1102:
                        return "11.02";
                    case Enumeradores.ListaServico.LS1103:
                        return "11.03";
                    case Enumeradores.ListaServico.LS1104:
                        return "11.04";
                    case Enumeradores.ListaServico.LS1201:
                        return "12.01";
                    case Enumeradores.ListaServico.LS1202:
                        return "12.02";
                    case Enumeradores.ListaServico.LS1203:
                        return "12.03";
                    case Enumeradores.ListaServico.LS1204:
                        return "12.04";
                    case Enumeradores.ListaServico.LS1205:
                        return "12.05";
                    case Enumeradores.ListaServico.LS1206:
                        return "12.06";
                    case Enumeradores.ListaServico.LS1207:
                        return "12.07";
                    case Enumeradores.ListaServico.LS1208:
                        return "12.08";
                    case Enumeradores.ListaServico.LS1209:
                        return "12.09";
                    case Enumeradores.ListaServico.LS1210:
                        return "12.10";
                    case Enumeradores.ListaServico.LS1211:
                        return "12.11";
                    case Enumeradores.ListaServico.LS1212:
                        return "12.12";
                    case Enumeradores.ListaServico.LS1213:
                        return "12.13";
                    case Enumeradores.ListaServico.LS1214:
                        return "12.14";
                    case Enumeradores.ListaServico.LS1215:
                        return "12.15";
                    case Enumeradores.ListaServico.LS1216:
                        return "12.16";
                    case Enumeradores.ListaServico.LS1217:
                        return "12.17";
                    case Enumeradores.ListaServico.LS1301:
                        return "13.01";
                    case Enumeradores.ListaServico.LS1302:
                        return "13.02";
                    case Enumeradores.ListaServico.LS1303:
                        return "13.03";
                    case Enumeradores.ListaServico.LS1304:
                        return "13.04";
                    case Enumeradores.ListaServico.LS1305:
                        return "13.05";
                    case Enumeradores.ListaServico.LS1401:
                        return "14.01";
                    case Enumeradores.ListaServico.LS1402:
                        return "14.02";
                    case Enumeradores.ListaServico.LS1403:
                        return "14.03";
                    case Enumeradores.ListaServico.LS1404:
                        return "14.04";
                    case Enumeradores.ListaServico.LS1405:
                        return "14.05";
                    case Enumeradores.ListaServico.LS1406:
                        return "14.06";
                    case Enumeradores.ListaServico.LS1407:
                        return "14.07";
                    case Enumeradores.ListaServico.LS1408:
                        return "14.08";
                    case Enumeradores.ListaServico.LS1409:
                        return "14.09";
                    case Enumeradores.ListaServico.LS1410:
                        return "14.10";
                    case Enumeradores.ListaServico.LS1411:
                        return "14.11";
                    case Enumeradores.ListaServico.LS1412:
                        return "14.12";
                    case Enumeradores.ListaServico.LS1413:
                        return "14.13";
                    case Enumeradores.ListaServico.LS1501:
                        return "15.01";
                    case Enumeradores.ListaServico.LS1502:
                        return "15.02";
                    case Enumeradores.ListaServico.LS1503:
                        return "15.03";
                    case Enumeradores.ListaServico.LS1504:
                        return "15.04";
                    case Enumeradores.ListaServico.LS1505:
                        return "15.05";
                    case Enumeradores.ListaServico.LS1506:
                        return "15.06";
                    case Enumeradores.ListaServico.LS1507:
                        return "15.07";
                    case Enumeradores.ListaServico.LS1508:
                        return "15.08";
                    case Enumeradores.ListaServico.LS1509:
                        return "15.09";
                    case Enumeradores.ListaServico.LS1510:
                        return "15.10";
                    case Enumeradores.ListaServico.LS1511:
                        return "15.11";
                    case Enumeradores.ListaServico.LS1512:
                        return "15.12";
                    case Enumeradores.ListaServico.LS1513:
                        return "15.13";
                    case Enumeradores.ListaServico.LS1514:
                        return "15.14";
                    case Enumeradores.ListaServico.LS1515:
                        return "15.15";
                    case Enumeradores.ListaServico.LS1516:
                        return "15.16";
                    case Enumeradores.ListaServico.LS1517:
                        return "15.17";
                    case Enumeradores.ListaServico.LS1518:
                        return "15.18";
                    case Enumeradores.ListaServico.LS1601:
                        return "16.01";
                    case Enumeradores.ListaServico.LS1701:
                        return "17.01";
                    case Enumeradores.ListaServico.LS1702:
                        return "17.02";
                    case Enumeradores.ListaServico.LS1703:
                        return "17.03";
                    case Enumeradores.ListaServico.LS1704:
                        return "17.04";
                    case Enumeradores.ListaServico.LS1705:
                        return "17.05";
                    case Enumeradores.ListaServico.LS1706:
                        return "17.06";
                    case Enumeradores.ListaServico.LS1707:
                        return "17.07";
                    case Enumeradores.ListaServico.LS1708:
                        return "17.08";
                    case Enumeradores.ListaServico.LS1709:
                        return "17.09";
                    case Enumeradores.ListaServico.LS1710:
                        return "17.10";
                    case Enumeradores.ListaServico.LS1711:
                        return "17.11";
                    case Enumeradores.ListaServico.LS1712:
                        return "17.12";
                    case Enumeradores.ListaServico.LS1713:
                        return "17.13";
                    case Enumeradores.ListaServico.LS1714:
                        return "17.14";
                    case Enumeradores.ListaServico.LS1715:
                        return "17.15";
                    case Enumeradores.ListaServico.LS1716:
                        return "17.16";
                    case Enumeradores.ListaServico.LS1717:
                        return "17.17";
                    case Enumeradores.ListaServico.LS1718:
                        return "17.18";
                    case Enumeradores.ListaServico.LS1719:
                        return "17.19";
                    case Enumeradores.ListaServico.LS1720:
                        return "17.20";
                    case Enumeradores.ListaServico.LS1721:
                        return "17.21";
                    case Enumeradores.ListaServico.LS1722:
                        return "17.22";
                    case Enumeradores.ListaServico.LS1723:
                        return "17.23";
                    case Enumeradores.ListaServico.LS1724:
                        return "17.24";
                    case Enumeradores.ListaServico.LS1801:
                        return "18.01";
                    case Enumeradores.ListaServico.LS1901:
                        return "19.01";
                    case Enumeradores.ListaServico.LS2001:
                        return "20.01";
                    case Enumeradores.ListaServico.LS2002:
                        return "20.02";
                    case Enumeradores.ListaServico.LS2003:
                        return "20.03";
                    case Enumeradores.ListaServico.LS2101:
                        return "21.01";
                    case Enumeradores.ListaServico.LS2201:
                        return "22.01";
                    case Enumeradores.ListaServico.LS2301:
                        return "23.01";
                    case Enumeradores.ListaServico.LS2401:
                        return "24.01";
                    case Enumeradores.ListaServico.LS2501:
                        return "25.01";
                    case Enumeradores.ListaServico.LS2502:
                        return "25.02";
                    case Enumeradores.ListaServico.LS2503:
                        return "25.03";
                    case Enumeradores.ListaServico.LS2504:
                        return "25.04";
                    case Enumeradores.ListaServico.LS2601:
                        return "26.01";
                    case Enumeradores.ListaServico.LS2701:
                        return "27.01";
                    case Enumeradores.ListaServico.LS2801:
                        return "28.01";
                    case Enumeradores.ListaServico.LS2901:
                        return "29.01";
                    case Enumeradores.ListaServico.LS3001:
                        return "30.01";
                    case Enumeradores.ListaServico.LS3101:
                        return "31.01";
                    case Enumeradores.ListaServico.LS3201:
                        return "32.01";
                    case Enumeradores.ListaServico.LS3301:
                        return "33.01";
                    case Enumeradores.ListaServico.LS3401:
                        return "34.01";
                    case Enumeradores.ListaServico.LS3501:
                        return "35.01";
                    case Enumeradores.ListaServico.LS3601:
                        return "36.01";
                    case Enumeradores.ListaServico.LS3701:
                        return "37.01";
                    case Enumeradores.ListaServico.LS3801:
                        return "38.01";
                    case Enumeradores.ListaServico.LS3901:
                        return "39.01";
                    case Enumeradores.ListaServico.LS4001:
                        return "40.01";
                    default:
                        return "";
                }
            }
        }

        public virtual Enumeradores.ListaServico CodigoServicoParaEnum(string codigoLista)
        {

            switch (codigoLista)
            {
                case "01.01":
                    return Enumeradores.ListaServico.LS101;
                case "01.02":
                    return Enumeradores.ListaServico.LS102;
                case "01.03":
                    return Enumeradores.ListaServico.LS103;
                case "01.04":
                    return Enumeradores.ListaServico.LS104;
                case "01.05":
                    return Enumeradores.ListaServico.LS105;
                case "01.06":
                    return Enumeradores.ListaServico.LS106;
                case "01.07":
                    return Enumeradores.ListaServico.LS107;
                case "01.08":
                    return Enumeradores.ListaServico.LS108;
                case "02.01":
                    return Enumeradores.ListaServico.LS201;
                case "03.01":
                    return Enumeradores.ListaServico.LS301;
                case "03.02":
                    return Enumeradores.ListaServico.LS302;
                case "03.03":
                    return Enumeradores.ListaServico.LS303;
                case "03.04":
                    return Enumeradores.ListaServico.LS304;
                case "03.05":
                    return Enumeradores.ListaServico.LS305;
                case "04.01":
                    return Enumeradores.ListaServico.LS401;
                case "04.02":
                    return Enumeradores.ListaServico.LS402;
                case "04.03":
                    return Enumeradores.ListaServico.LS403;
                case "04.04":
                    return Enumeradores.ListaServico.LS404;
                case "04.05":
                    return Enumeradores.ListaServico.LS405;
                case "04.06":
                    return Enumeradores.ListaServico.LS406;
                case "04.07":
                    return Enumeradores.ListaServico.LS407;
                case "04.08":
                    return Enumeradores.ListaServico.LS408;
                case "04.09":
                    return Enumeradores.ListaServico.LS409;
                case "04.10":
                    return Enumeradores.ListaServico.LS410;
                case "04.11":
                    return Enumeradores.ListaServico.LS411;
                case "04.12":
                    return Enumeradores.ListaServico.LS412;
                case "04.13":
                    return Enumeradores.ListaServico.LS413;
                case "04.14":
                    return Enumeradores.ListaServico.LS414;
                case "04.15":
                    return Enumeradores.ListaServico.LS415;
                case "04.16":
                    return Enumeradores.ListaServico.LS416;
                case "04.17":
                    return Enumeradores.ListaServico.LS417;
                case "04.18":
                    return Enumeradores.ListaServico.LS418;
                case "04.19":
                    return Enumeradores.ListaServico.LS419;
                case "04.20":
                    return Enumeradores.ListaServico.LS420;
                case "04.21":
                    return Enumeradores.ListaServico.LS421;
                case "04.22":
                    return Enumeradores.ListaServico.LS422;
                case "04.23":
                    return Enumeradores.ListaServico.LS423;
                case "05.01":
                    return Enumeradores.ListaServico.LS501;
                case "05.02":
                    return Enumeradores.ListaServico.LS502;
                case "05.03":
                    return Enumeradores.ListaServico.LS503;
                case "05.04":
                    return Enumeradores.ListaServico.LS504;
                case "05.05":
                    return Enumeradores.ListaServico.LS505;
                case "05.06":
                    return Enumeradores.ListaServico.LS506;
                case "05.07":
                    return Enumeradores.ListaServico.LS507;
                case "05.08":
                    return Enumeradores.ListaServico.LS508;
                case "05.09":
                    return Enumeradores.ListaServico.LS509;
                case "06.01":
                    return Enumeradores.ListaServico.LS601;
                case "06.02":
                    return Enumeradores.ListaServico.LS602;
                case "06.03":
                    return Enumeradores.ListaServico.LS603;
                case "06.04":
                    return Enumeradores.ListaServico.LS604;
                case "06.05":
                    return Enumeradores.ListaServico.LS605;
                case "07.01":
                    return Enumeradores.ListaServico.LS701;
                case "07.02":
                    return Enumeradores.ListaServico.LS702;
                case "07.03":
                    return Enumeradores.ListaServico.LS703;
                case "07.04":
                    return Enumeradores.ListaServico.LS704;
                case "07.05":
                    return Enumeradores.ListaServico.LS705;
                case "07.06":
                    return Enumeradores.ListaServico.LS706;
                case "07.07":
                    return Enumeradores.ListaServico.LS707;
                case "07.08":
                    return Enumeradores.ListaServico.LS708;
                case "07.09":
                    return Enumeradores.ListaServico.LS709;
                case "07.10":
                    return Enumeradores.ListaServico.LS710;
                case "07.11":
                    return Enumeradores.ListaServico.LS711;
                case "07.12":
                    return Enumeradores.ListaServico.LS712;
                case "07.13":
                    return Enumeradores.ListaServico.LS713;
                case "07.14":
                    return Enumeradores.ListaServico.LS714;
                case "07.15":
                    return Enumeradores.ListaServico.LS715;
                case "07.16":
                    return Enumeradores.ListaServico.LS716;
                case "07.17":
                    return Enumeradores.ListaServico.LS717;
                case "07.18":
                    return Enumeradores.ListaServico.LS718;
                case "07.19":
                    return Enumeradores.ListaServico.LS719;
                case "07.20":
                    return Enumeradores.ListaServico.LS720;
                case "07.21":
                    return Enumeradores.ListaServico.LS721;
                case "07.22":
                    return Enumeradores.ListaServico.LS722;
                case "08.01":
                    return Enumeradores.ListaServico.LS801;
                case "08.02":
                    return Enumeradores.ListaServico.LS802;
                case "09.01":
                    return Enumeradores.ListaServico.LS901;
                case "09.02":
                    return Enumeradores.ListaServico.LS902;
                case "09.03":
                    return Enumeradores.ListaServico.LS903;
                case "10.01":
                    return Enumeradores.ListaServico.LS1001;
                case "10.02":
                    return Enumeradores.ListaServico.LS1002;
                case "10.03":
                    return Enumeradores.ListaServico.LS1003;
                case "10.04":
                    return Enumeradores.ListaServico.LS1004;
                case "10.05":
                    return Enumeradores.ListaServico.LS1005;
                case "10.06":
                    return Enumeradores.ListaServico.LS1006;
                case "10.07":
                    return Enumeradores.ListaServico.LS1007;
                case "10.08":
                    return Enumeradores.ListaServico.LS1008;
                case "10.09":
                    return Enumeradores.ListaServico.LS1009;
                case "10.10":
                    return Enumeradores.ListaServico.LS1010;
                case "11.01":
                    return Enumeradores.ListaServico.LS1101;
                case "11.02":
                    return Enumeradores.ListaServico.LS1102;
                case "11.03":
                    return Enumeradores.ListaServico.LS1103;
                case "11.04":
                    return Enumeradores.ListaServico.LS1104;
                case "12.01":
                    return Enumeradores.ListaServico.LS1201;
                case "12.02":
                    return Enumeradores.ListaServico.LS1202;
                case "12.03":
                    return Enumeradores.ListaServico.LS1203;
                case "12.04":
                    return Enumeradores.ListaServico.LS1204;
                case "12.05":
                    return Enumeradores.ListaServico.LS1205;
                case "12.06":
                    return Enumeradores.ListaServico.LS1206;
                case "12.07":
                    return Enumeradores.ListaServico.LS1207;
                case "12.08":
                    return Enumeradores.ListaServico.LS1208;
                case "12.09":
                    return Enumeradores.ListaServico.LS1209;
                case "12.10":
                    return Enumeradores.ListaServico.LS1210;
                case "12.11":
                    return Enumeradores.ListaServico.LS1211;
                case "12.12":
                    return Enumeradores.ListaServico.LS1212;
                case "12.13":
                    return Enumeradores.ListaServico.LS1213;
                case "12.14":
                    return Enumeradores.ListaServico.LS1214;
                case "12.15":
                    return Enumeradores.ListaServico.LS1215;
                case "12.16":
                    return Enumeradores.ListaServico.LS1216;
                case "12.17":
                    return Enumeradores.ListaServico.LS1217;
                case "13.01":
                    return Enumeradores.ListaServico.LS1301;
                case "13.02":
                    return Enumeradores.ListaServico.LS1302;
                case "13.03":
                    return Enumeradores.ListaServico.LS1303;
                case "13.04":
                    return Enumeradores.ListaServico.LS1304;
                case "13.05":
                    return Enumeradores.ListaServico.LS1305;
                case "14.01":
                    return Enumeradores.ListaServico.LS1401;
                case "14.02":
                    return Enumeradores.ListaServico.LS1402;
                case "14.03":
                    return Enumeradores.ListaServico.LS1403;
                case "14.04":
                    return Enumeradores.ListaServico.LS1404;
                case "14.05":
                    return Enumeradores.ListaServico.LS1405;
                case "14.06":
                    return Enumeradores.ListaServico.LS1406;
                case "14.07":
                    return Enumeradores.ListaServico.LS1407;
                case "14.08":
                    return Enumeradores.ListaServico.LS1408;
                case "14.09":
                    return Enumeradores.ListaServico.LS1409;
                case "14.10":
                    return Enumeradores.ListaServico.LS1410;
                case "14.11":
                    return Enumeradores.ListaServico.LS1411;
                case "14.12":
                    return Enumeradores.ListaServico.LS1412;
                case "14.13":
                    return Enumeradores.ListaServico.LS1413;
                case "15.01":
                    return Enumeradores.ListaServico.LS1501;
                case "15.02":
                    return Enumeradores.ListaServico.LS1502;
                case "15.03":
                    return Enumeradores.ListaServico.LS1503;
                case "15.04":
                    return Enumeradores.ListaServico.LS1504;
                case "15.05":
                    return Enumeradores.ListaServico.LS1505;
                case "15.06":
                    return Enumeradores.ListaServico.LS1506;
                case "15.07":
                    return Enumeradores.ListaServico.LS1507;
                case "15.08":
                    return Enumeradores.ListaServico.LS1508;
                case "15.09":
                    return Enumeradores.ListaServico.LS1509;
                case "15.10":
                    return Enumeradores.ListaServico.LS1510;
                case "15.11":
                    return Enumeradores.ListaServico.LS1511;
                case "15.12":
                    return Enumeradores.ListaServico.LS1512;
                case "15.13":
                    return Enumeradores.ListaServico.LS1513;
                case "15.14":
                    return Enumeradores.ListaServico.LS1514;
                case "15.15":
                    return Enumeradores.ListaServico.LS1515;
                case "15.16":
                    return Enumeradores.ListaServico.LS1516;
                case "15.17":
                    return Enumeradores.ListaServico.LS1517;
                case "15.18":
                    return Enumeradores.ListaServico.LS1518;
                case "16.01":
                    return Enumeradores.ListaServico.LS1601;
                case "17.01":
                    return Enumeradores.ListaServico.LS1701;
                case "17.02":
                    return Enumeradores.ListaServico.LS1702;
                case "17.03":
                    return Enumeradores.ListaServico.LS1703;
                case "17.04":
                    return Enumeradores.ListaServico.LS1704;
                case "17.05":
                    return Enumeradores.ListaServico.LS1705;
                case "17.06":
                    return Enumeradores.ListaServico.LS1706;
                case "17.07":
                    return Enumeradores.ListaServico.LS1707;
                case "17.08":
                    return Enumeradores.ListaServico.LS1708;
                case "17.09":
                    return Enumeradores.ListaServico.LS1709;
                case "17.10":
                    return Enumeradores.ListaServico.LS1710;
                case "17.11":
                    return Enumeradores.ListaServico.LS1711;
                case "17.12":
                    return Enumeradores.ListaServico.LS1712;
                case "17.13":
                    return Enumeradores.ListaServico.LS1713;
                case "17.14":
                    return Enumeradores.ListaServico.LS1714;
                case "17.15":
                    return Enumeradores.ListaServico.LS1715;
                case "17.16":
                    return Enumeradores.ListaServico.LS1716;
                case "17.17":
                    return Enumeradores.ListaServico.LS1717;
                case "17.18":
                    return Enumeradores.ListaServico.LS1718;
                case "17.19":
                    return Enumeradores.ListaServico.LS1719;
                case "17.20":
                    return Enumeradores.ListaServico.LS1720;
                case "17.21":
                    return Enumeradores.ListaServico.LS1721;
                case "17.22":
                    return Enumeradores.ListaServico.LS1722;
                case "17.23":
                    return Enumeradores.ListaServico.LS1723;
                case "17.24":
                    return Enumeradores.ListaServico.LS1724;
                case "18.01":
                    return Enumeradores.ListaServico.LS1801;
                case "19.01":
                    return Enumeradores.ListaServico.LS1901;
                case "20.01":
                    return Enumeradores.ListaServico.LS2001;
                case "20.02":
                    return Enumeradores.ListaServico.LS2002;
                case "20.03":
                    return Enumeradores.ListaServico.LS2003;
                case "21.01":
                    return Enumeradores.ListaServico.LS2101;
                case "22.01":
                    return Enumeradores.ListaServico.LS2201;
                case "23.01":
                    return Enumeradores.ListaServico.LS2301;
                case "24.01":
                    return Enumeradores.ListaServico.LS2401;
                case "25.01":
                    return Enumeradores.ListaServico.LS2501;
                case "25.02":
                    return Enumeradores.ListaServico.LS2502;
                case "25.03":
                    return Enumeradores.ListaServico.LS2503;
                case "25.04":
                    return Enumeradores.ListaServico.LS2504;
                case "26.01":
                    return Enumeradores.ListaServico.LS2601;
                case "27.01":
                    return Enumeradores.ListaServico.LS2701;
                case "28.01":
                    return Enumeradores.ListaServico.LS2801;
                case "29.01":
                    return Enumeradores.ListaServico.LS2901;
                case "30.01":
                    return Enumeradores.ListaServico.LS3001;
                case "31.01":
                    return Enumeradores.ListaServico.LS3101;
                case "32.01":
                    return Enumeradores.ListaServico.LS3201;
                case "33.01":
                    return Enumeradores.ListaServico.LS3301;
                case "34.01":
                    return Enumeradores.ListaServico.LS3401;
                case "35.01":
                    return Enumeradores.ListaServico.LS3501;
                case "36.01":
                    return Enumeradores.ListaServico.LS3601;
                case "37.01":
                    return Enumeradores.ListaServico.LS3701;
                case "38.01":
                    return Enumeradores.ListaServico.LS3801;
                case "39.01":
                    return Enumeradores.ListaServico.LS3901;
                case "40.01":
                    return Enumeradores.ListaServico.LS4001;
                default:
                    return Enumeradores.ListaServico.LS1001;
            }
        }

        #endregion
    }
}
