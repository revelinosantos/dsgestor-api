using DsGestor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DsGestor.Infrastructure.Data;

public class DsGestorDbContext : DbContext
{
    public DsGestorDbContext(DbContextOptions<DsGestorDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Vendedor> Vendedores => Set<Vendedor>();
    public DbSet<Supervisor> Supervisores => Set<Supervisor>();
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
    public DbSet<LancamentoDet> LancamentosDet => Set<LancamentoDet>();
    public DbSet<Promocao> Promocoes =>  Set<Promocao>();
    public DbSet<PromocaoItem> PromocaoItens => Set<PromocaoItem>();
    public DbSet<PromocaoCliente> PromocaoClientes => Set<PromocaoCliente>();
    public DbSet<PromocaoOrigemPedido> PromocaoOrigemPedidos => Set<PromocaoOrigemPedido>();
    public DbSet<Conferencia> Conferencias => Set<Conferencia>();
    public DbSet<ConferenciaPedidoView> ConferenciaPedidoView => Set<ConferenciaPedidoView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ECOACRE");

        ConfigureCliente(modelBuilder);
        ConfigureProduto(modelBuilder);
        ConfigureDepartamento(modelBuilder);
        ConfigureUsuario(modelBuilder);
        ConfigureVendedor(modelBuilder);
        ConfigureSupervisor(modelBuilder);
        ConfigureLancamento(modelBuilder);
        ConfigureLancamentoDet(modelBuilder);
        ConfigurePromocao(modelBuilder);
        ConfigurePromcaoItem(modelBuilder);
        ConfigurePromocaoCliente(modelBuilder);
        ConfigurePromocaoOrigemPedido(modelBuilder);
        ConfigureConferencia(modelBuilder);
        ConfigureConferenciaPedidoView(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureConferenciaPedidoView(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ConferenciaPedidoView>(entity =>
      {
            entity.ToView("VIEW_DSGESTOR_CONF_PED");

            entity.HasNoKey();

            entity.Property(x => x.Numped).HasColumnName("NUMPED").HasPrecision(10, 0);
            entity.Property(x => x.Dataped).HasColumnName("DATAPED");
            entity.Property(x => x.Statusped).HasColumnName("STATUSPED").HasMaxLength(2).IsUnicode(false);
            entity.Property(x => x.Codfilial).HasColumnName("CODFILIAL");
            entity.Property(x => x.Codusur).HasColumnName("CODUSUR");
            entity.Property(x => x.Condvenda).HasColumnName("CONDVENDA");
            entity.Property(x => x.Codemitenteped).HasColumnName("CODEMITENTEPED");
            entity.Property(x => x.Codcli).HasColumnName("CODCLI");
            entity.Property(x => x.Cliente).HasColumnName("CLIENTE").HasMaxLength(100).IsUnicode(false);
            entity.Property(x => x.Vltotal).HasColumnName("VLTOTAL").HasPrecision(18, 2);

            entity.Property(x => x.Codprod).HasColumnName("CODPROD");
            entity.Property(x => x.Numseq).HasColumnName("NUMSEQ");
            entity.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(200).IsUnicode(false);

            entity.Property(x => x.Qtoriginal).HasColumnName("QTORIGINAL");
            entity.Property(x => x.Qt).HasColumnName("QT");
            entity.Property(x => x.Qtfalta).HasColumnName("QTFALTA");
            entity.Property(x => x.Qtunit).HasColumnName("QTUNIT");
            entity.Property(x => x.Qtunitcx).HasColumnName("QTUNITCX");

            entity.Property(x => x.Pvenda).HasColumnName("PVENDA").HasPrecision(18, 2);
            entity.Property(x => x.Codauxiliar).HasColumnName("CODAUXILIAR").HasMaxLength(20).IsUnicode(false);

            entity.Property(x => x.Pesobrutopro).HasColumnName("PESOBRUTOPRO").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqpro).HasColumnName("PESOLIQPRO").HasPrecision(18, 4);

            entity.Property(x => x.Codconf).HasColumnName("CODCONF");
            entity.Property(x => x.Volume).HasColumnName("VOLUME");

            entity.Property(x => x.Pesobrutoped).HasColumnName("PESOBRUTOPED").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqped).HasColumnName("PESOLIQPED").HasPrecision(18, 4);

            entity.Property(x => x.Codsep).HasColumnName("CODSEP");
            entity.Property(x => x.Nomeconf).HasColumnName("NOMECONF").HasMaxLength(100).IsUnicode(false);

            entity.Property(x => x.Codauxiliar2).HasColumnName("CODAUXILIAR2").HasMaxLength(20).IsUnicode(false);

            entity.Property(x => x.Numnota).HasColumnName("NUMNOTA");
            entity.Property(x => x.Numtransvenda).HasColumnName("NUMTRANSVENDA");

            entity.Property(x => x.Lpvid).HasColumnName("LPVID");
            entity.Property(x => x.Lpvcod).HasColumnName("LPVCOD").HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.Lpvcodfind).HasColumnName("LPVCODFIND").HasMaxLength(20).IsUnicode(false);
     });
    }
    private void ConfigureConferencia(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Conferencia>(entity =>
      {
            entity.ToTable("DSGESTOR_CONF");

            entity.HasKey(x => x.Confid);

            entity.Property(x => x.Confid).HasColumnName("CONFID")
                  .HasPrecision(10, 0)
                  .ValueGeneratedOnAdd();
         
            entity.Property(x => x.Numped).HasColumnName("NUMPED").HasPrecision(10, 0).IsRequired();
            entity.Property(x => x.Dataped).HasColumnName("DATAPED");
            entity.Property(x => x.Statusped).HasColumnName("STATUSPED").HasMaxLength(2);
            entity.Property(x => x.Codfilial).HasColumnName("CODFILIAL");
            entity.Property(x => x.Codusur).HasColumnName("CODUSUR");
            entity.Property(x => x.Condvenda).HasColumnName("CONDVENDA");
            entity.Property(x => x.Codemitenteped).HasColumnName("CODEMITENTEPED");
            entity.Property(x => x.Codcli).HasColumnName("CODCLI");
            entity.Property(x => x.Cliente).HasColumnName("CLIENTE").HasMaxLength(100);
            entity.Property(x => x.Vltotal).HasColumnName("VLTOTAL").HasPrecision(18, 2);

            entity.Property(x => x.Codprod).HasColumnName("CODPROD").IsRequired();
            entity.Property(x => x.Numseq).HasColumnName("NUMSEQ").IsRequired();
            entity.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(200);

            entity.Property(x => x.Qtoriginal).HasColumnName("QTORIGINAL");
            entity.Property(x => x.Qt).HasColumnName("QT");
            entity.Property(x => x.Qtfalta).HasColumnName("QTFALTA");
            entity.Property(x => x.Qtunit).HasColumnName("QTUNIT");
            entity.Property(x => x.Qtunitcx).HasColumnName("QTUNITCX");

            entity.Property(x => x.Pvenda).HasColumnName("PVENDA").HasPrecision(18, 2);
            entity.Property(x => x.Codauxiliar).HasColumnName("CODAUXILIAR").HasMaxLength(20);

            entity.Property(x => x.Pesobrutopro).HasColumnName("PESOBRUTOPRO").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqpro).HasColumnName("PESOLIQPRO").HasPrecision(18, 4);

            entity.Property(x => x.Codconf).HasColumnName("CODCONF");
            entity.Property(x => x.Datainiconf).HasColumnName("DATAINICONF");
            entity.Property(x => x.Datafimconf).HasColumnName("DATAFIMCONF");

            entity.Property(x => x.Qtconfunid).HasColumnName("QTCONFUNID").HasDefaultValue(0);
            entity.Property(x => x.Qtconf).HasColumnName("QTCONF").HasDefaultValue(0);

            entity.Property(x => x.Conferido).HasColumnName("CONFERIDO").HasMaxLength(1).HasDefaultValue("N");
            entity.Property(x => x.Conferidoitem).HasColumnName("CONFERIDOITEM").HasMaxLength(1).HasDefaultValue("N");

            entity.Property(x => x.Volume).HasColumnName("VOLUME");

            entity.Property(x => x.Pesobrutoped).HasColumnName("PESOBRUTOPED").HasPrecision(18, 4);
            entity.Property(x => x.Pesoliqped).HasColumnName("PESOLIQPED").HasPrecision(18, 4);

            entity.Property(x => x.Fechadodiverg).HasColumnName("FECHADODIVERG").HasMaxLength(1).HasDefaultValue("N");
            entity.Property(x => x.Datafechdiverg).HasColumnName("DATAFECHDIVERG");

            entity.Property(x => x.Codsep).HasColumnName("CODSEP");
            entity.Property(x => x.Dataconfwinthor).HasColumnName("DATACONFWINTHOR");
            entity.Property(x => x.Nomeconf).HasColumnName("NOMECONF").HasMaxLength(100);

            entity.Property(x => x.Codfundiverg).HasColumnName("CODFUNDIVERG").HasDefaultValue(0);

            entity.Property(x => x.Codauxiliar2).HasColumnName("CODAUXILIAR2").HasMaxLength(20);

            entity.Property(x => x.Android).HasColumnName("ANDROID").HasMaxLength(1).HasDefaultValue("N");

            entity.Property(x => x.Numnota).HasColumnName("NUMNOTA").HasDefaultValue(0).IsRequired();
            entity.Property(x => x.Numtransvenda).HasColumnName("NUMTRANSVENDA").HasDefaultValue(0);

            entity.Property(x => x.Lpvid).HasColumnName("LPVID").HasDefaultValue(0);
            entity.Property(x => x.Lpvcod).HasColumnName("LPVCOD").HasMaxLength(20).HasDefaultValue("N");
            entity.Property(x => x.Lpvcodfind).HasColumnName("LPVCODFIND").HasMaxLength(20).HasDefaultValue("N");

            entity.Property(x => x.Statusconf).HasColumnName("STATUSCONF").HasMaxLength(20).HasDefaultValue("ABERTA");
            entity.Property(x => x.Usuarioinicio).HasColumnName("USUARIOINICIO");
            entity.Property(x => x.Usuariofinalizou).HasColumnName("USUARIOFINALIZOU");
            entity.Property(x => x.Dataultleitura).HasColumnName("DATAULTLEITURA");
            entity.Property(x => x.Observacao).HasColumnName("OBSERVACAO").HasMaxLength(500);
            entity.Property(x => x.Finalizadowinthor).HasColumnName("FINALIZADOWINTHOR").HasMaxLength(1).HasDefaultValue("N");
            entity.Property(x => x.Errowinthor).HasColumnName("ERROWINTHOR").HasMaxLength(1000);
                  
      });
        
    }

    private void ConfigurePromocaoCliente(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PromocaoCliente>(entity =>
      {
            entity.ToTable("PCPROMOCAOCLIMED");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

             entity.Property(e => e.CodigoCliente)
                  .HasColumnName("CODCLI")
                  .HasPrecision(10, 0);
      });
    }

    private void ConfigurePromocaoOrigemPedido(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PromocaoOrigemPedido>(entity =>
      {
            entity.ToTable("PCPROMOCAOORIGEMMED");

            entity.HasKey(e => new { e.Id, e.OrigemPed });
            
            entity.Property(e => e.Id)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();

             entity.Property(e => e.OrigemPed)
                  .HasColumnName("ORIGEMPED")
                  .HasMaxLength(1)
                  .IsUnicode(false);
      });
    }

    private void ConfigurePromocao(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Promocao>(entity =>
      {
            entity.ToTable("PCPROMOCAOMED");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0)
                  .ValueGeneratedNever();
                
            entity.Property(e => e.IdentificadorPromocao)
                  .HasColumnName("IDENTIFICADORPROMOCAO")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.TipoPolitica)
                  .HasColumnName("TIPOPOLITICA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.DescricaoResumida)
                  .HasColumnName("DESCRICAORESUMIDA")
                  .HasMaxLength(40)
                  .IsUnicode(false);

            entity.Property(e => e.DescricaoDetalhada)
                  .HasColumnName("DESCRICAODETALHADA")
                  .HasMaxLength(200)
                  .IsUnicode(false);

            entity.Property(e => e.DataInicial)
                  .HasColumnName("DATAINICIAL");

            entity.Property(e => e.DataFinal)
                  .HasColumnName("DATAFINAL");

            entity.Property(e => e.TipoPromocao)
                  .HasColumnName("TIPOPROMOCAO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoRestricao)
                  .HasColumnName("TIPORESTRICAO")
                  .HasPrecision(3, 0);

            entity.Property(e => e.CodFuncLanc)
                  .HasColumnName("CODFUNCLANC")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DataLanc)
                  .HasColumnName("DATALANC");

            entity.Property(e => e.AlteraDesconto)
                  .HasColumnName("ALTERADESCONTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.SyncFv)
                  .HasColumnName("SYNCFV")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CodReferencia)
                  .HasColumnName("CODREFERENCIA")
                  .HasMaxLength(1000)
                  .IsUnicode(false);

            entity.Property(e => e.BaseCredDebBrca)
                  .HasColumnName("BASECREDDEBRCA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CreditaSobrePolitica)
                  .HasColumnName("CREDITASOBREPOLITICA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ConsideraCalcGiroMedic)
                  .HasColumnName("CONSIDERACALCGIROMEDIC")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UtilizaDescRede)
                  .HasColumnName("UTILIZADESCREDE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoPlanoPag)
                  .HasColumnName("TIPOPLANOPAG")
                  .HasMaxLength(1);

            entity.Property(e => e.AplicaFamiliaProdutos)
                  .HasColumnName("APLICAFAMILIAPRODUTOS")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoSimplesNacional)
                  .HasColumnName("TIPOSIMPLESNACIONAL")
                  .HasMaxLength(1);

            entity.Property(e => e.PontuarAcordoParceria)
                  .HasColumnName("PONTUARACORDOPARCERIA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.PrazoPlPagMandatorio)
                  .HasColumnName("PRAZOPLPAGMANDATORIO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Bloqueio)
                  .HasColumnName("BLOQUEIO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UtilizaPercDescBaseRca)
                  .HasColumnName("UTILIZAPERCDESCBASERCA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.OrigemPreco)
                  .HasColumnName("ORIGEMPRECO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.VisualizarPromoEsp)
                  .HasColumnName("VISUALIZARPROMOESP")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UsaNormalizacaoDesc)
                  .HasColumnName("USANORMALIZACAODESC")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ParticipaPremiacaoCli)
                  .HasColumnName("PARTICIPAPREMIACAOCLI")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.MultiplosBrindes)
                  .HasColumnName("MULTIPLOSBRINDES")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.AplicaDesconto)
                  .HasColumnName("APLICADESCONTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoSolicitante)
                  .HasColumnName("TIPOSOLICITANTE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.AceitaDescPromCampanha)
                  .HasColumnName("ACEITADESCPROMCAMPANHA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ValidarMultEmbMaster)
                  .HasColumnName("VALIDARMULTEMBMASTER")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.ParticipaPremiacaoInd)
                  .HasColumnName("PARTICIPAPREMIACAOIND")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.IncluirFiltroTelevendas)
                  .HasColumnName("INCLUIRFILTROTELEVENDAS")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.TipoCotaGeral)
                  .HasColumnName("TIPOCOTAGERAL")
                  .HasPrecision(5, 0);

            entity.Property(e => e.TipoCotaCliente)
                  .HasColumnName("TIPOCOTACLIENTE")
                  .HasPrecision(5, 0);

            entity.Property(e => e.IonSync)
                  .HasColumnName("IONSYNC")
                  .HasMaxLength(1)
                  .IsUnicode(false);
      });
    }

      private void ConfigurePromcaoItem(ModelBuilder modelBuilder)
      {
        modelBuilder.Entity<PromocaoItem>(entity =>
        {
            entity.ToTable("PCDESCONTO");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("CODDESCONTO")
                  .HasPrecision(18, 0)
                  .ValueGeneratedOnAdd();
             
            entity.Property(e => e.CodigoCliente)
                  .HasColumnName("CODCLI")
                  .HasPrecision(10, 0);

            entity.Property(e => e.CodigoProduto)
                  .HasColumnName("CODPROD")
                  .HasPrecision(10, 0);

            entity.Property(e => e.PercDesc)
                  .HasColumnName("PERCDESC")
                  .HasPrecision(18, 6);

            entity.Property(e => e.DtInicio)
                  .HasColumnName("DTINICIO");

            entity.Property(e => e.DtFim)
                  .HasColumnName("DTFIM");

            entity.Property(e => e.CodFuncLanc)
                  .HasColumnName("CODFUNCLANC")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DataLanc)
                  .HasColumnName("DATALANC");

            entity.Property(e => e.CodFuncUltAlter)
                  .HasColumnName("CODFUNCULTALTER")
                  .HasPrecision(10, 0);

            entity.Property(e => e.DataUltAlter)
                  .HasColumnName("DATAULTALTER");

            entity.Property(e => e.AplicaDesconto)
                  .HasColumnName("APLICADESCONTO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.BaseCredDebBrca)
                  .HasColumnName("BASECREDDEBRCA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.UtilizaDescRede)
                  .HasColumnName("UTILIZADESCREDE")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CreditaSobrePolitica)
                  .HasColumnName("CREDITASOBREPOLITICA")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Tipo)
                  .HasColumnName("TIPO")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.OrigemPed)
                  .HasColumnName("ORIGEMPED")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.CodigoPromocao)
                  .HasColumnName("CODPROMOCAOMED")
                  .HasPrecision(10, 0);

            entity.Property(e => e.TipoPoliticaPromocaoMed)
                  .HasColumnName("TIPOPOLITICAPROMOCAOMED")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.SyncFv)
                  .HasColumnName("SYNCFV")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.IonSync)
                  .HasColumnName("IONSYNC")
                  .HasMaxLength(1)
                  .IsUnicode(false);

            entity.Property(e => e.Descricao)
                  .HasColumnName("DESCRICAO")
                  .HasMaxLength(200)
                  .IsUnicode(false);

            entity.Property(e => e.GrupoValidacao)
                  .HasColumnName("GRUPO_VALIDACAO")
                  .HasMaxLength(10)
                  .IsUnicode(false);

            // relacionamento lógico (sem navegação obrigatória)
            entity.HasIndex(e => e.CodigoPromocao);
      });
    }
    private void ConfigureProduto(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Produto>(entity =>
      {
   
      entity.ToView("VIEW_DSGESTOR_PRO_MARGEM"); 
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasColumnName("CODPROD").HasPrecision(6, 0);
   
      entity.Property(e => e.CodigoFilial).HasColumnName("CODFILIAL");
      entity.Property(e => e.CodigoRegiao).HasColumnName("NUMREGIAO").HasPrecision(4, 0);
      entity.Property(e => e.NomeRegiao).HasColumnName("REGIAO");
      entity.Property(e => e.Estado).HasColumnName("UF");
      entity.Property(e => e.Descricao).HasColumnName("DESCRICAO");
      entity.Property(e => e.CodigoFornecedor).HasColumnName("CODFORNEC").HasPrecision(6, 0);
      entity.Property(e => e.Fornecedor).HasColumnName("FORNECEDOR");

      entity.Property(e => e.CodigoDepto).HasColumnName("CODEPTO").HasPrecision(6, 0);
      entity.Property(e => e.DescricaoDepto).HasColumnName("DESCRICAODEPTO");

      entity.Property(e => e.Unidade).HasColumnName("UNIDADE");
      entity.Property(e => e.DataUltimaEntrada).HasColumnName("DTULTENT");

      entity.Property(e => e.CustoFinanceiro).HasColumnName("CUSTO_FIN");
      entity.Property(e => e.CodigoIcmTab).HasColumnName("CODICMTAB");
      entity.Property(e => e.Estoque).HasColumnName("QTESTGER");
      entity.Property(e => e.PrecoTabela).HasColumnName("PTABELA");
      entity.Property(e => e.PrecoVenda).HasColumnName("PVENDA");
      entity.Property(e => e.MargemIdeal).HasColumnName("MARGEM_IDEAL");
      entity.Property(e => e.PrecoMinimo).HasColumnName("PRECOMINIMO");
      });

    }
    private static void ConfigureCliente(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Cliente>();

        entity.ToTable("VIEW_DSGESTOR_CLIENTE");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODCLI");

        entity.Property(e => e.RazaoSocial)
              .HasColumnName("CLIENTE")
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.NomeFantasia)
              .HasColumnName("FANTASIA")
              .HasMaxLength(200);

        entity.Property(e => e.CnpjCpf)
              .HasColumnName("CGCENT")
              .HasMaxLength(20)
              .IsRequired();

        entity.Property(e => e.Endereco)
              .HasColumnName("ENDERENT")
              .HasMaxLength(250);

        entity.Property(e => e.Bairro)
              .HasColumnName("BAIRROENT")
              .HasMaxLength(100);

        entity.Property(e => e.Cidade)
              .HasColumnName("NOMECIDADE")
              .HasMaxLength(100);

        entity.Property(e => e.Estado)
              .HasColumnName("UF")
              .HasMaxLength(2);

        entity.Property(e => e.CodigoFilial)
              .HasColumnName("CODFILIALNF")
              .HasMaxLength(2);
       
        entity.Property(e => e.CodigoVendedor)
              .HasColumnName("CODUSUR");
       
        entity.Property(e => e.NomeVendedor)
              .HasColumnName("NOME");
       
       entity.Property(e => e.CodigoSupervisor)
              .HasColumnName("CODSUPERVISOR");     
  
       entity.Property(e => e.NomeSupervisor)
              .HasColumnName("NOMESUPERVISOR");    
     
       entity.Property(e => e.DataCadastro)
              .HasColumnName("DTCADASTRO");     
       
       entity.Property(e => e.DataUltimaCompra)
              .HasColumnName("DTULTCOMP");                                        

       entity.Property(e => e.Fone)
              .HasColumnName("TELENT");  
   
       entity.Property(e => e.LimiteCredito)
              .HasColumnName("LIMCRED");       
      
       entity.Property(e => e.NumRegiao)
              .HasColumnName("NUMREGIAO");

       entity.Property(e => e.NomeRegiao)
              .HasColumnName("REGIAO");
                  
    }
    private static void ConfigureDepartamento(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Departamento>();

        entity.ToTable("PCDEPTO");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODEPTO");

        entity.Property(e => e.Descricao)
              .HasColumnName("DESCRICAO")
              .HasMaxLength(150)
              .IsRequired();
    }
    private static void ConfigureUsuario(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Usuario>();

        entity.ToTable("DSGESTOR_USUARIO");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("ID");

        entity.Property(e => e.Nome)
              .HasColumnName("NOME")
              .HasMaxLength(150)
              .IsRequired();

        entity.Property(e => e.Email)
              .HasColumnName("EMAIL")
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.Pwd)
              .HasColumnName("PWD")
              .HasMaxLength(200)
              .IsRequired();

        entity.Property(e => e.Perfil)
              .HasColumnName("PERFIL")
              .HasConversion<int>(); // enum -> int

        entity.Property(e => e.Codigo)
              .HasColumnName("MATRICULA")
              .HasMaxLength(50);

        entity.Property(e => e.CodigoVendedor)
              .HasColumnName("CODUSUR");

        entity.Property(e => e.CodigoSupervisor)
              .HasColumnName("CODSUPERVISOR");
        entity.HasOne(e => e.Vendedor)
              .WithMany()
              .HasForeignKey(e => e.CodigoVendedor)
              .HasPrincipalKey(v => v.Id);
        entity.HasOne(e => e.Supervisor)
              .WithMany()
              .HasForeignKey(e => e.CodigoSupervisor)
              .HasPrincipalKey(v => v.Id);
 
    }
    private static void ConfigureVendedor(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Vendedor>();

        entity.ToTable("PCUSUARI");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODUSUR");

        entity.Property(e => e.Nome)
              .HasColumnName("NOME")
              .HasMaxLength(150)
              .IsRequired();

        entity.Property(e => e.DataTermino )
                  .HasColumnName("DTTERMINO");

        entity.Property(e => e.CodigoSupervisor)
              .HasColumnName("CODSUPERVISOR");  
              
        entity.HasOne(e => e.Supervisor)
              .WithMany()
              .HasForeignKey(e => e.CodigoSupervisor)
              .HasPrincipalKey(v => v.Id);
    }
   
    private static void ConfigureSupervisor(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Supervisor>();

        entity.ToTable("PCSUPERV");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("CODSUPERVISOR");

        entity.Property(e => e.Nome)
              .HasColumnName("NOME")
              .HasMaxLength(150)
              .IsRequired();
    }

    private static void ConfigureLancamento(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Lancamento>();

        entity.ToTable("DSGESTOR_LANC");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.Data).HasColumnName("DATA");

        entity.Property(e => e.CodigoUsuario).HasColumnName("ID_USUARIO").IsRequired();
        entity.Property(e => e.CodigoPromocao).HasColumnName("CODPROMOCAOMED");
    
        entity.Property(e => e.CodigoVendedor).HasColumnName("CODUSUR");
        entity.Property(e => e.CodigoCliente).HasColumnName("CODCLI");
        entity.Property(e => e.CodigoFilial).HasColumnName("CODFILIALNF");
        entity.Property(e => e.NumRegiao).HasColumnName("NUMREGIAO");
        entity.Property( e => e.Observacao).HasColumnName("OBS").HasMaxLength(4000);

        entity.Property(e => e.CodigoUsuarioAut).HasColumnName("ID_USUARIO_AUT");
        entity.Property(e => e.DataAutorizacao).HasColumnName("DATA_AUT");

        entity.Property(e => e.Status)
              .HasColumnName("STATUS")
              .HasConversion<int>();

         entity.HasMany(e => e.Itens)
              .WithOne(d => d.Lancamento)
              .HasForeignKey(d => d.CodigoLancamento)
              .HasConstraintName("FK_DSGESTOR_LANC_DET_LANC");
       
         entity.HasOne(e => e.Cliente)
              .WithMany()
              .HasForeignKey(e => new { e.CodigoCliente, e.CodigoVendedor, e.CodigoFilial })
              .HasPrincipalKey(c => new { c.Id, c.CodigoVendedor, c.CodigoFilial });
             
         entity.HasOne(e => e.Vendedor)
              .WithMany()
              .HasForeignKey(e => e.CodigoVendedor)
              .HasPrincipalKey(v => v.Id);
                   
         entity.HasOne(e => e.Usuario)
              .WithMany()
              .HasForeignKey(e => e.CodigoUsuario)
               .HasPrincipalKey(c => c.Id);

         entity.HasOne(e => e.UsuarioAut)
              .WithMany()
              .HasForeignKey(e => e.CodigoUsuarioAut)
              .HasPrincipalKey(c => c.Id);
      }

      private static void ConfigureLancamentoDet(ModelBuilder modelBuilder)
      {
          var entity = modelBuilder.Entity<LancamentoDet>();

          entity.ToTable("DSGESTOR_LANC_DET");

          entity.HasKey(e => e.Id);
          entity.Property(e => e.Id).HasColumnName("ID");

          entity.Property(e => e.CodigoLancamento).HasColumnName("ID_LANC").IsRequired();
          entity.Property(e => e.CodigoProduto).HasColumnName("CODPROD").IsRequired();
      
          entity.Property(e => e.PrecoCustoFin).HasColumnName("PCUSTO_FIN").HasPrecision(18, 6);
          entity.Property(e => e.CodigoIcmTab).HasColumnName("CODICMTAB").HasPrecision(6, 4);
          entity.Property(e => e.PrecoVenda).HasColumnName("PVENDA").HasPrecision(18, 6);

          entity.Property(e => e.MargemIdeal).HasColumnName("MARGEM_IDEAL").HasPrecision(6, 4);
          entity.Property(e => e.Quantidade).HasColumnName("QTD").HasPrecision(12, 4);

          entity.Property(e => e.PrecoUnitario).HasColumnName("PUNIT").HasPrecision(18, 6);
          entity.Property(e => e.PercDesconto).HasColumnName("PERC_DESC").HasPrecision(6, 4);
          entity.Property(e => e.Margem).HasColumnName("MARGEM").HasPrecision(6, 4);

          entity.Property(e => e.QuantidadeAut).HasColumnName("QTD_AUT").HasPrecision(12, 4);
          entity.Property(e => e.PrecoUnitarioAut).HasColumnName("PUNIT_AUT").HasPrecision(18, 6);
          entity.Property(e => e.PercDescontoAut).HasColumnName("PERC_DESC_AUT").HasPrecision(6, 4);
          entity.Property(e => e.MargemAut).HasColumnName("MARGEM_AUT").HasPrecision(6, 4);
          entity.Property(e => e.PrecoMinimo).HasColumnName("PRECOMINIMO").HasPrecision(18, 6);
          entity.HasOne(e => e.Produto)
                  .WithMany()
                  .HasForeignKey(e => e.CodigoProduto)
                  .HasPrincipalKey(p => p.Id);
      } 
  
}
