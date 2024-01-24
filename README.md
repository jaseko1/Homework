# Homework

![SCREEN_GW](https://jaseko.cz/files/screen_gw.png)


## Úvod a Motivace

### Úvod
Vítejte u projektu Homework. Tento projekt byl inspirován reálnými scénáři v softwarovém vývoji, kde je potřeba efektivně pracovat s existujícími, ale omezenými systémy. Cílem projektu je poskytnout robustní, škálovatelnou a flexibilní službu, která rozšiřuje funkčnost staršího webového systému, a to bez potřeby přístupu k původnímu zdrojovému kódu s důrazem na asynchronní zpracování, thread safety a hluboké porozumění Dependency Injection v rámci .NET. Homework představuje využití široké škály technologií a postupů, od SOAP a REST API až po realtime aktualizace pomocí SignalR a plánování úloh s Hangfire.

### Motivace
Výběrová kola pohovorů často zahrnují tvorbu domácího úkolu, který testuje skillset uchazeče. Po účasti v několika takovýchto pohovorech a získání různých úkolů jsem se rozhodl vytvořit komplexnější úlohu. Tento projekt odráží můj skillset, širokou škálu použitých technologií a komplikovanější zadání, které simuluje reálnou situaci z praxe. Celý projekt jsem zde detailně zdekumentoval.

## Zadání

### Problém
Ve firmě existuje již dlouhá léta webová služba, ke které nikdo nemá kód. Bohužel, z různých důvodů není možné službu implementovat znovu od základu, protože obsahuje business logiku, o které nikdo neví, jak přesně funguje. Hlavním problémem služby je její omezení zpracování dotazů. Kvůli chybě v původním kódu dokáže současně zpracovávat pouze 5 dotazů. Při šestém dotazu dojde k výjimce a služba spadne.

### Úkol
Naprogramujte vlastní službu, která umožní aplikacím kontaktovat výše zmíněnou službu libovolným počtem dotazů. Původní služba poskytuje CRUD operace nad zákazníky přes SOAP. API nové služby bude REST a bude zahrnovat následující funkce:
- Seznam zákazníků včetně řazení
- Detail zákazníka
- Vytvoření zákazníka
- Úprava zákazníka
- Smazání zákazníka

### Model Zákazník
- Jméno
- Příjmení
- Email
- Telefon
- ID

### Požadavky
- Příchozí požadavky musí být odbavovány v pořadí, ve kterém přišly (týká se Create, Update, Delete).
- V případě, že vaše služba spadne, bude po spuštění pokračovat tam, kde přestala (týká se Create, Update, Delete).
- Zákazníci mohou mít libovolná jména, emaily atd.
- Kód pište tak, jakoby byste psali reálnou produkční službu.
- Napište unit testy k nové službě
- Starou službu vytvořte v aplikaci - implementujte tak, aby odpovídala zadání.
- Staré službě trvá 0-10 sekund zpracovat jeden dotaz (přidělujte náhodně).
- Implementujte realtime notifikace (SignalR); v Hubu se bude realtime přenášet aktuální stav fronty a notifikace po dokončení requestu.
- Každý den o půlnoci se vytvoří Excel s jednoduchými statistikami - kolik bylo zpracováno requestů, dle typů a dle úspěšnosti dokončení.


## Technologický Stack

.NET, EntityFramework, MSSQL, MongoDb, AutoMapper, Hangfire, Scrutor, SignalR, SOAP, REST, React, NextJS, TailwindCSS

## Komunikace služeb

![Swagger UI Screenshot](https://jaseko.cz/files/diagram.svg)

# Řešení a Implementace

## Implementace 'Zastaralé Služby' `Homework.OldCustomerService`

Pro simulaci staré služby popsané ve zadání, která nemohla být přepsána z důvodu neznámé business logiky, byla vytvořena služba `Homework.OldCustomerService`.

### Využití SoapCore pro SOAP Komunikaci
Použil jsem knihovnu SoapCore, která je dostupná na NuGet, pro vytvoření SOAP komunikace. Díky SoapCore bylo jednoduché specifikovat modely a vytvořit SOAP službu. SoapCore automaticky generuje WSDL z definované služby, což usnadňuje integraci s klienty.

#### SOAP Endpoint
Pro zpřístupnění služby byl v aplikaci implementován následující endpoint:

```csharp
app.UseEndpoints(endpoints => {
    _ = endpoints.UseSoapEndpoint<ICustomerService>("/Customer.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
});
```

#### ICustomerService
Rozhraní ze kerého vychází SOAP služba, toto rozhrání jsem koknrétně implementoval s očekávanou funkčností a s napojením na MongoDB
```csharp
    [ServiceContract]
    public interface ICustomerService
    {

        [OperationContract]
        Task<CustomerDto> GetOne(string id);

        [OperationContract]
        Task<CustomersDto> Get(GetCustomersRequest request);

        [OperationContract]
        Task Create(CreateCustomerRequest request);

        [OperationContract]
        Task Update(UpdateCustomerRequest request);

        [OperationContract]
        Task Delete(DeleteCustomerRequest request);

    }
```

#### Řešení Omezení Počtu Requestů
Pro simulaci omezení staré služby, která dokáže zpracovat maximálně 5 requestů současně, jsem implementoval třídu `RequestCounter` jako singleton. Tato třída používá `Interlocked` operace pro thread-safe inkrementaci a dekrementaci počtu současně zpracovávaných požadavků:


```csharp
private int _currentCount = 0;
private readonly int _maxCount = 5;

public void Increment()
{
    if (Interlocked.Increment(ref _currentCount) > _maxCount)
    {
        throw new InvalidOperationException("Překročen limit současně zpracovávaných požadavků.");
    }
}

public void Decrement()
{
    Interlocked.Decrement(ref _currentCount);
}
```

#### Integrace s MongoDB
Stará služba byla napojena na MongoDB databázi, ze které čerpala a ukládala data o zákaznících. Tato integrace je zde také řešena. To umožňovalo realistickou simulaci práce se skutečnými daty a zajišťovalo věrohodnost testování nově vytvořené služby.

Výsledkem je plně funkční simulace staré služby, která věrně odpovídá zadání a poskytuje realistické prostředí pro testování a integraci nové služby `Homework.Gateway`.



# Implementace nové služby `Homework.Gateway`
## REST API Nové Služby

Pro novou službu `Homework.Gateway` bylo vytvořeno REST API, které umožňuje efektivní interakci s funkcemi služby. API poskytuje řadu endpointů pro CRUD operace a další funkce související s logikou aplikace.

REST API pro každý request používá samostatný model. Toto řešení poskytuje čistou a strukturovanou architekturu, která zvyšuje čitelnost a udržitelnost kódu.

#### Model-View-Controller Architektura
Pro zpracování requestů využíváme architekturu, ve které každý controller volá specifickou service. Tato service pak zajišťuje přemapování modelů requestů z nové REST služby na modely requestů pro starou SOAP službu. Tento přístup může být považován za variaci na Model-View-Controller (MVC) architekturu, se zaměřením na oddělení logiky zpracování dat od logiky prezentace.

#### Použití AutoMapper pro Mapování Modelů
Pro efektivní mapování mezi modely různých služeb byl použit balíček AutoMapper. Byl vytvořen `CustomerProfile`, který definuje mapování související se zákazníkem:

```csharp
public CustomerProfile() 
{       
    CreateMap<OldCustomerService.CreateCustomerRequest, CreateCustomerRequest>().ReverseMap();        
    CreateMap<OldCustomerService.UpdateCustomerRequest, UpdateCustomerRequest>().ReverseMap();                   
    CreateMap<OldCustomerService.GetCustomersRequest, GetCustomersRequest>().ReverseMap();     
    CreateMap<OldCustomerService.CustomersDto, CustomersDto>().ReverseMap();     
    CreateMap<OldCustomerService.CustomerDto, CustomerDto>().ReverseMap();     
}
```

### API Endpoints

Následující seznam uvádí klíčové endpointy dostupné v našem API:

- `GET /Customer` - Získání seznamu zákazníků
- `GET /Customer/{id}` - Získání detailu zákazníka
- `POST /Customer` - Vytvoření nového zákazníka
- `PUT /Customer/{id}` - Úprava zákazníka
- `DELETE /Customer/{id}` - Smazání zákazníka

### Swagger UI

Pro lepší přehlednost a testování API je k dispozici Swagger UI, který poskytuje interaktivní dokumentaci k našemu API. Níže je screenshot Swagger UI, který ilustruje dostupné endpointy a jejich specifikace.

![Swagger UI Screenshot](https://jaseko.cz/files/swagger_gw.png)


## Integrace SOAP Služby přes Visual Studio Connected Services

### Přidání Service Reference

Pro integraci SOAP služby do `Homework.Gateway` byl využit nástroj Visual Studio Connected Services, který umožňuje snadné připojení k webovým službám. Proces začíná přidáním reference na službu do projektu:

1. **Spuštění 'Zastaralé Služby':** Nejprve byla spuštěna  `Homework.OldCustomerService`, která díky SoapCore vygenerovala WSDL na adrese `/Customer.asmx`.

2. **Přidání Reference ve Visual Studio:**
   - V projektu `Homework.Gateway` byla otevřena sekce Connected Services.
   - Byla vybrána možnost přidat novou Service Reference.
   - Do dialogového okna byla vložena adresa WSDL (`/Customer.asmx`).

3. **Generování Modelů a Klienta:**
   - Po potvrzení byly automaticky vygenerovány modely a klient pro SOAP službu přímo z WSDL, vytvářející tak složku s všemi potřebnými třídami pro interakci se službou.

### Výhody Této Metody

- **Automatické Generování Kódu:** Tento přístup eliminuje potřebu ručně psát modely a klientský kód pro interakci se SOAP službou, což výrazně šetří čas a snižuje riziko chyb.
- **Udržování Konzistence:** Jelikož jsou modely generovány přímo z WSDL, je zaručena jejich konzistence s definicemi služby.
- **Snadná Integrace:** Tento postup umožňuje rychlou a efektivní integraci SOAP služeb do .NET projektů bez hluboké znalosti SOAP a WSDL.
- **Flexibilita a Škálovatelnost:** Jakékoli změny ve WSDL lze snadno reflektovat aktualizací service reference, což umožňuje pružně reagovat na změny ve službě.

Díky využití Visual Studio Connected Services je proces integrace a udržování SOAP služby v `Homework.Gateway` výrazně zjednodušen a efektivnější.


### Vytvoření OldCustomerServiceFactory pro Flexibilní Vytváření Klientů

#### Zvýšení Testovatelnosti a Flexibility

Kromě automatické generace klienta SOAP služby pomocí Connected Services jsem také vytvořil `OldCustomerServiceFactory`. Tento návrhový vzor umožňuje větší kontrolu nad procesem vytváření instancí SOAP klientů a zjednodušuje testování.

##### Implementace Factory Třídy

`OldCustomerServiceFactory` je implementována jako třída, která vytváří klienty pro starou SOAP službu. Toto je kód této factory třídy:

```csharp
public class OldCustomerServiceFactory : IOldCustomerServiceFactory
{
    private readonly OldCustomerServiceConfig _oldServiceConfig;

    public OldCustomerServiceFactory(IOptions<OldCustomerServiceConfig> oldServiceConfig)
    {
        _oldServiceConfig = oldServiceConfig.Value;
    }

    public ICustomerService CreateService()
    {
        var client = new CustomerServiceClient();
        return client.ChannelFactory.CreateChannel(new EndpointAddress(_oldServiceConfig.Url));
    }
}
```

Tato třída přijímá konfiguraci pro starou službu (např. URL) a používá ji k vytvoření a konfiguraci klienta. To znamená, že kdykoliv je potřeba komunikovat se starou službou, `OldCustomerServiceFactory` poskytne připravenou instanci klienta.

#### Výhody Této Metody
- **Lepší Testovatelnost:** Díky odstranění přímé závislosti na konkrétní instanci klienta je možné `OldCustomerServiceFactory` jednoduše mockovat nebo nahradit v testech.
- **Flexibilita:** Factory pattern umožňuje snadné změny v konfiguraci nebo v implementaci klienta bez nutnosti zasahovat do ostatních částí aplikace.
- **Oddělení Zodpovědnosti:** Tímto přístupem se odděluje logika vytváření klienta od jeho používání, což vede k čistšímu a udržitelnějšímu designu.

Použitím `OldCustomerServiceFactory` je zajištěno, že proces vytváření a konfigurace klientů pro starou SOAP službu je efektivní, flexibilní a snadno testovatelný.


## Datová Část Služby 'Homework.Gateway'

#### Struktura a Využití MSSQL Databáze

Služba `Homework.Gateway` je navržena tak, aby efektivně využívala MSSQL databázi. Pro usnadnění správy dat a jejich struktury byla vytvořena knihovna tříd `Homework.Gateway.Data`. Tato knihovna obsahuje následující komponenty:

- **DbContext:** Třída pro správu databázového kontextu, která umožňuje efektivní komunikaci s databází.
- **Enums:** Výčtové typy, které definují sadu pojmenovaných konstant a usnadňují čitelnost a správu kódu.
- **Repository:** Třídy repository, které poskytují abstraktní vrstvu mezi business logikou a datovými operacemi.
- **Entities:** Entity třídy, které reprezentují datové modely a jsou mapovány na databázové tabulky.

#### Výhody Této Architektury

- **Modularita a Čistota Kódu:** Oddělením datové vrstvy do samostatné knihovny je dosaženo vyšší úrovně modularizace a organizace kódu. To usnadňuje údržbu a rozšiřování aplikace.
- **Snadná Testovatelnost:** Díky oddělení datové vrstvy může být logika aplikace testována nezávisle na databázi, což zjednodušuje testování a zvyšuje kvalitu kódu.
- **Flexibilita a Rozšiřitelnost:** Tato architektura umožňuje snadné přidávání nových entit a repozitářů, což podporuje budoucí rozšiřování a změny v aplikaci.

#### Implementace Repository Pattern

Pro komunikaci mezi službami a databází byl implementován repository pattern. Tento vzor poskytuje abstraktní vrstvu mezi business logikou a datovými operacemi, čímž odděluje zpracování dat od ostatních částí aplikace. Výhody zahrnují:

- **Lepší Oddělení Zodpovědnosti:** Každá část aplikace má jasně definovanou zodpovědnost, což zlepšuje strukturu a organizaci kódu.
- **Účinnější Správa Dat:** Repository pattern umožňuje centralizovanou správu datových operací, což zvyšuje účinnost a snižuje redundanci kódu.
- **Snadnější Údržba a Rozšiřování:** Díky oddělení datové vrstvy lze aplikaci snadněji udržovat a rozšiřovat bez výrazných zásahů do existujících komponent.

Tato struktura a implementace zajišťují, že `Homework.Gateway` je robustní, udržitelná a připravena pro budoucí rozvoj.


### Asynchronní Zpracování Requestů

Jedním z hlavních problémů projektu bylo omezení staré služby, která nemohla zpracovávat více než 5 requestů současně. Tento problém byl vyřešen implementací asynchronního zpracování requestů, zajišťující thread-safe přístup.

#### Klíčové Komponenty Request Queue Handleru
Pro efektivní řešení byla vytvořena služba `RequestQueueHandler` jako singleton. Tato služba používá `ConcurrentQueue<Func<Task>>` pro ukládání a správu příchozích requestů a `SemaphoreSlim` pro omezení počtu současně zpracovávaných requestů. Navíc byl integrován SignalR Hub pro poskytování realtime aktualizací o stavu fronty requestů.

Níže je uvedený fragment kódu, který ilustruje klíčové části této implementace:

```csharp
public async Task EnqueueRequest(Func<Task> requestFunc)
{
    _requestQueue.Enqueue(requestFunc);
    await _hubContext.Clients.All.SendAsync(NotificationHub.RECEIVE_QUEUE_COUNT, GetQueueCount());
    if (!_isProccessing)
    {
        await ProcessQueue();
    }
}

public int GetQueueCount()
{
    return _requestQueue.Count;
}

private async Task ProcessQueue()
{
    while (_requestQueue.TryDequeue(out var requestAction))
    {
        await _semaphore.WaitAsync();
        try
        {
            _isProccessing = true;
            await requestAction();
        }
        finally
        {
            _semaphore.Release();
            await _hubContext.Clients.All.SendAsync(NotificationHub.RECEIVE_QUEUE_COUNT, GetQueueCount());
        }
    }
    _isProccessing = false;
}
```

### Zasílání Requestů přes Request Queue Manager

V `Homework.Gateway` jsou requesty zasílány do fronty prostřednictvím `RequestQueueManager`. Tento proces zahrnuje asynchronní volání funkce, která provolává starou službu a následně označí request v naší databázi jako úspěšně dokončený:

```csharp
await _requestQueueManager.EnqueueRequest(() => CreateCustomerAsync(request, queueRequest));

// V těle funkce CreateCustomerAsync
await _oldService.CreateAsync(new OldCustomerService.CreateRequest { /* ... */ });
await _queueRequestService.CompleteSuccessfuly(queueRequest);
```

#### Úprava Stavu Requestu v Databázi
Metoda _queueRequestService.CompleteSuccessfuly obsahuje logiku pro aktualizaci stavu requestu. Tato metoda volá repozitář, který je implementován podle repository pattern a aktualizuje stav requestu v databázi:

```csharp
request.Status = QueueRequestStatus.Done;
await _repository.UpdateOutsideScope(request);
```

#### Řešení DbContext Lifecycle
Využití `DbContextFactory` pro Aktualizace Mimo Scope
Vzhledem k asynchronnímu zpracování requestů ve frontě může dojít k situaci, kdy původní scope, ve kterém byl `DbContext` vytvořen, již neexistuje. Abychom předešli problémům s ukládáním změn do databáze, byla implementována logika využívající `DbContextFactory`. Tento přístup umožňuje vytvořit nový kontext pro každou operaci, nezávisle na původním scope:

```csharp
await using var context = _dbContextFactory.CreateDbContext();
context.Update(request);
await context.SaveChangesAsync();
```

Toto řešení zajišťuje, že každá aktualizace stavu requestu je bezpečně uložena do databáze, a to i v případě, že původní scope již skončil. Tím je zaručena konzistence dat a správné fungování aplikace v asynchronním prostředí.

#### Důležitost Porozumění Dependency Injection a Lifecycle v .NET
Tato implementace podtrhuje důležitost porozumění principům Dependency Injection a lifecycle managementu objektů v .NET. Správné pochopení a aplikace těchto principů je klíčové pro vytvoření robustních a spolehlivých aplikací, zejména v asynchronním a distribuovaném prostředí.

## Registrace Scoped Služeb Pomocí Scrutor a Vlastního Atributu

### Použití Scrutor pro Automatickou Registraci

V `Homework.Gateway` jsme využili Scrutor, rozšíření pro .NET Core Dependency Injection, které umožňuje více flexibilní a deklarativní způsob registrace služeb. Speciálně byla využita funkce automatického skenování a registrace služeb s životním cyklem `Scoped`.

#### Implementace v `Program.cs`

Registrace služeb probíhá v souboru `Program.cs` a vypadá následovně:

```csharp
builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(classes => classes.WithAttribute<ScopedAttribute>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

Tento kód efektivně prohledává všechny třídy v závislostech aplikace a hledá ty, které jsou označeny vlastním atributem `Scoped`. Pokud je třída označena tímto atributem, automaticky ji registruje jako službu s `Scoped` životním cyklem.

Pro jednoduchost a čitelnost kódu byl vytvořen vlastní atribut Scoped. Tento atribut lze použít pro označení tříd, které mají být automaticky zaregistrovány jako Scoped služby:

```csharp
[Scoped]
public class CustomerService : ICustomerService
{
    // Implementace služby
}
```

Použití tohoto atributu zvyšuje transparentnost a snižuje potřebu manuální registrace služeb. Také pomáhá udržovat Dependency Injection kontejner organizovaný a jednoduchý na správu.

## Realtime Komunikace Pomocí SignalR

### Integrace SignalR Hub

`Homework.Gateway` využívá SignalR, framework pro ASP.NET, který umožňuje snadnou implementaci realtime webové komunikace. Díky SignalR je možné efektivně komunikovat s klienty v reálném čase.

#### Vytvoření a Mapování SignalR Hub

Integrace SignalR do projektu je relativně jednoduchá. Nejprve je v projektu vytvořen `Hub`, což je třída, která slouží jako centrální bod pro komunikaci:

```csharp
public class NotificationHub : Hub
{
    // Metody pro odesílání zpráv klientům
}
```

Následně je tento Hub namapován v `Program.cs` takto:

```csharp
app.MapHub<NotificationHub>("/notificationHub");
```

Tímto způsobem je vytvořen websocket endpoint, který je dostupný na cestě `/notificationHub`. Klienti se mohou k tomuto endpointu připojit a přijímat zprávy posílané ze serveru.

#### Posílání Zpráv Klientům
V rámci Homework.Gateway je SignalR použit k odesílání notifikací klientům. Tyto notifikace mohou informovat o dokončení zpracování asynchronních requestů nebo o aktuálním stavu fronty. Komunikace probíhá jednosměrně - z serveru na klienta:

- Snadná Integrace: Díky knihovně SignalR je implementace websocketů v .NET jednoduchá a integruje se dobře s ostatními částmi ASP.NET ekosystému.
- Široká Podpora Klientů: SignalR podporuje širokou škálu klientů včetně webových aplikací, mobilních aplikací a desktopových aplikací.


## Projekt Homework.Hangfire pro Správu Cron Jobů

### Účel a Integrace s Homework.Gateway.Data

Projekt `Homework.Hangfire` je navržen s cílem poskytovat robustní řešení pro správu a plánování Cron jobů. Tento projekt je integrován s `Homework.Gateway.Data`, což umožňuje efektivní přístup k datům potřebným pro plánované úlohy.

### Konfigurace a Použití Hangfire

Hangfire je framework, který poskytuje komplexní řešení pro správu pozadí úloh v .NET aplikacích. Jeho hlavními přednostmi jsou podpora pro plánování úloh a přehledný dashboard pro jejich sledování.

#### Konfigurace v `Program.cs`

Konfigurace Hangfire probíhá v souboru `Program.cs`. Zde je příklad základní konfigurace:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Konfigurace Hangfire
        builder.Services.AddHangfire(config => config
            .UseSqlServerStorage("connection_string_to_sql_server"));
        builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Mapování dashboardu Hangfire
        app.UseHangfireDashboard();
        
        // Ostatní konfigurace
    }
}
```

### Použití Hangfire pro Plánování Úloh
Plánování úloh s Hangfire je jednoduché a intuitivní. Příklad plánování úlohy:

```csharp
RecurringJob.AddOrUpdate(() => Console.WriteLine("Pravidelná úloha spuštěna."), Cron.Daily);
```
### Výhody Odděleného Projektu Homework.Hangfire

- **Škálovatelnost:** Oddělením Cron jobů do samostatného projektu je možné lépe škálovat výpočetní zdroje podle aktuálních potřeb úloh. To umožňuje efektivnější správu výkonu a zdrojů.
- **Specializace:** Vzhledem k tomu, že Cron joby často provádějí složitější nebo výpočetně náročnější operace, jejich izolace do samostatného projektu umožňuje lepší optimalizaci a specializaci.
- **Nezávislá Správa a Monitoring:** S Hangfire dashboardem poskytuje `Homework.Hangfire` jednoduchý a efektivní způsob pro monitorování a správu plánovaných úloh.

Integrace Hangfire do samostatného projektu `Homework.Hangfire` přináší výhody v podobě lepší správy, škálovatelnosti a přehlednosti plánovaných úloh v rámci celého systému.

## Testování v Projektu Homework.Gateway.Tests

### Použití xUnit a Moq pro Jednotkové Testování

V rámci projektu `Homework.Gateway.Tests` jsem implementoval unit testy pomocí xUnit testovacího frameworku a Moq, což je populární knihovna pro vytváření mock objektů v .NET. Tyto nástroje mi umožnily efektivně testovat logiku aplikace a zajišťovat její spolehlivost.

#### Test `CreateCustomer_EnqueuesRequestAndHandlesItProperly`

Jedním z klíčových testů, který jsem napsal, je `CreateCustomer_EnqueuesRequestAndHandlesItProperly`. Tento test ověřuje, že metoda pro vytváření nového zákazníka správně zařazuje request do fronty a následně ho zpracovává:

```csharp
        [Fact]
        public async Task CreateCustomer_EnqueuesRequestAndHandlesItProperly()
        {
            // Arrange
            var mockRequestQueueHandler = new Mock<IRequestQueueHandler>();
            var mockOldServiceFactory = new Mock<OldCustomerService.IOldCustomerServiceFactory>();
            var mockOldService = new Mock<OldCustomerService.ICustomerService>();
            var mockLogger = new Mock<ILogger<CustomerService>>();
            var mockMapper = new Mock<IMapper>();
            var mockQueueRequestService = new Mock<IQueueRequestService>();

            var customerService = new CustomerService(
                mockRequestQueueHandler.Object, mockOldServiceFactory.Object,
                mockLogger.Object, mockMapper.Object, mockQueueRequestService.Object);

            var customerRequest = new CreateCustomerRequest();
            var oldServiceCreateRequest = new OldCustomerService.CreateCustomerRequest();

            mockMapper.Setup(m => m.Map<OldCustomerService.CreateCustomerRequest>(It.IsAny<CreateCustomerRequest>()))
                      .Returns(oldServiceCreateRequest);

            // Simulujeme přidání požadavku do fronty
            mockQueueRequestService.Setup(m => m.AddCustomerCreateRequest(It.IsAny<OldCustomerService.CreateCustomerRequest>()))
                                   .ReturnsAsync(new QueueRequest());

            // Act
            await customerService.Create(customerRequest);

            // Assert
            mockQueueRequestService.Verify(m => m.AddCustomerCreateRequest(It.Is<OldCustomerService.CreateCustomerRequest>(req => req == oldServiceCreateRequest)), Times.Once);
            mockRequestQueueHandler.Verify(m => m.EnqueueRequest(It.IsAny<Func<Task>>()), Times.Once);
        }
```
## Možnosti Rozšíření Testování

Projekt `Homework.Gateway.Tests` nabízí prostor pro další rozšíření a zlepšení testovacího pokrytí:

### TestFixture
Pro komplexnější scénáře lze využít TestFixture, což umožní efektivnější inicializaci a uklizení prostředí pro sady testů.

### Integrační Testy s Databází
Implementace testování s použitím SQLite nabízí možnost provádět integrační testy s reálnými daty, ale v izolovaném prostředí.

### Testování Endpointů
Rozšíření o testování HTTP endpointů s použitím testovacího HTTP klienta, což umožní ověřovat funkčnost REST API.

## Todo

Rozšíření testovacího projektu o další typy testů a techniky, což zvýší spolehlivost a robustnost celého projektu Homework.

Další rozšířením by mohlo být:

- **Přidání Autorizace/Autentizace API:** Implementace bezpečnostních opatření pro API, včetně autentizace a autorizace uživatelů, což zajistí, že přístup k API bude možný pouze pro ověřené a oprávněné uživatele.
- **Přidání Hangfire Jobu pro Generování Excelu:** Rozšíření Hangfire o úlohu, která bude v pravidelných intervalech generovat Excelové soubory s použitím knihovny EPPlus. Tato funkcionalita umožní automatizované sestavování a distribuci reportů a statistik.
- 
## Bonus: Vytvoření Uživatelského Rozhraní

### Interaktivní Testování a Pozorování Procesů

Pro lepší demonstraci a testování celého procesu aplikace jsem vytvořil uživatelské rozhraní. Toto rozhraní umožňuje nejenom interaktivní testování aplikace, ale i vizualizaci důležitých aspektů v reálném čase.

![UI_SCREENSHOT](https://jaseko.cz/files/gw_vid.gif)

#### Realtime Zobrazení Počtu Requestů ve Frontě

Jednou z klíčových funkcí tohoto rozhraní je zobrazení aktuálního počtu requestů ve frontě. Díky integraci s SignalR je tento stav aktualizován v reálném čase, což umožňuje sledovat dynamiku zpracování requestů.

#### Command Line Sekce pro Notifikace

Pro zobrazení notifikací o provedených requestech byla vytvořena sekce inspirovaná příkazovým řádkem (Command Line). Tento design poskytuje intuitivní a přehledný způsob, jak sledovat průběh a výsledky jednotlivých requestů.

#### Kontrolní Panel pro Odesílání Requestů

Nad sekci Command Line je umístěn kontrolní panel, který umožňuje odesílání requestů na `Homework.Gateway`. Toto rozhraní poskytuje uživateli jednoduchý způsob, jak testovat různé aspekty aplikace a sledovat, jak jsou requesty zpracovány v reálném čase.

### Výhody Interaktivního Uživatelského Rozhraní

- **Interaktivní Testování:** Uživatelské rozhraní umožňuje interaktivní testování a experimentování s různými aspekty aplikace.
- **Vizualizace Procesů:** Díky vizuálním prvkům, jako je Command Line sekce a kontrolní panel, je proces zpracování requestů snadno pochopitelný a přehledný.

#### Technologie: React, NextJS, TailwindCSS

### Kontakt
hello@jaseko.cz
