# TelegramBotProject

## О проекте

**TelegramBotProject** — это простой, расширяемый Telegram‑бот на C# (.NET 8) с хранением данных в SQL Server через Entity Framework Core. Бот поддерживает базовые команды, загрузку PDF‑файлов от пользователей и хранение API‑ключей для каждого пользователя. ([github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Program.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/BotService.cs?plain=1))

Основные возможности:

* Работа с Telegram Bot API через библиотеку `Telegram.Bot` 20+;
* Приём и сохранение PDF‑документов;
* Хранение пользователей и их состояний (Finite‑State‑Machine) в БД;
* Набор встроенных команд: `/start`, `/help`, `/setapikey`, `/getapikey`, `/version`; ([github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandHandlers/StartCommandHandler.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandHandlers/HelpCommandHandler.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandHandlers/SetApiKeyCommandHandler.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandHandlers/GetApiKeyCommandHandler.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandHandlers/VersionCommandHandler.cs?plain=1))
* Docker‑контейнер, docker‑compose для запуска вместе с SQL Server.

## Быстрый старт

```bash
# Клонируем репозиторий
$ git clone https://github.com/tgr484/TelegramBotProject.git
$ cd TelegramBotProject

# Запуск через Docker‑Compose
$ TELEGRAM_BOT_TOKEN="<ТОКЕН_БОТА>" \
  DB_CONNECTION_STRING="Server=sql,1433;Database=BotDb;User Id=sa;Password=<StrongPassword>;TrustServerCertificate=true" \
  docker compose up --build
```

По умолчанию бот запускается по `dotnet run` или внутри контейнера и начинает получать обновления методом *long‑polling*. ([github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Program.cs?plain=1))

## Конфигурация

| Переменная             | Описание                                                                                                                                   |
| ---------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| `TELEGRAM_BOT_TOKEN`   | Токен, выданный [@BotFather](https://t.me/BotFather).                                                                                      |
| `DB_CONNECTION_STRING` | Строка подключения к SQL Server. Пример: `Server=localhost,1433;Database=BotDb;User Id=sa;Password=Pass_w0rd;TrustServerCertificate=true`. |

Файл `appsettings.json` используется только для примеров и может быть переопределён переменными окружения в Docker. ([github.com](https://github.com/tgr484/TelegramBotProject))

## Структура каталогов

```
Bot/                — логика бота
├── CommandHandlers — классы обработчиков команд
├── FileHandlers    — приём файлов (сейчас только PDF)
├── StateHandlers   — обработчики пошагового ввода
└── CommandRegistry — реестр команд
Data/               — сущности и контекст EF Core
Migrations/         — миграции базы данных
Services/           — бизнес‑логика (UserService и др.)
Program.cs          — точка входа, настройка и запуск бота
Dockerfile, docker-compose.yml
```

## База данных

Модель пользователя:

```csharp
public class TelegramUser
{
    Guid   Id          { get; set; }
    long   TelegramId  { get; set; }
    string ApiKey?     { get; set; }
    UserState State    { get; set; }
}
```

Миграции создаются автоматически при первом запуске (см. `dbContext.Database.Migrate()` в `Program.cs`). ([github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Program.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Data/TelegramUser.cs?plain=1))

## Команды бота

| Команда             | Описание                                                      |
| ------------------- | ------------------------------------------------------------- |
| `/start`            | Регистрация пользователя, перевод в режим ожидания API‑ключа. |
| `/help`             | Справка по доступным командам.                                |
| `/setapikey <ключ>` | Сохранить API‑ключ вручную.                                   |
| `/getapikey`        | Показать текущий API‑ключ.                                    |
| `/version`          | Показать версию приложения.                                   |

Каждый обработчик реализует интерфейс `ICommandHandler` и регистрируется в `CommandRegistry`. Это упрощает добавление новых команд: создайте класс‑обработчик, реализуйте `Command` и `HandleAsync`, затем зарегистрируйте в `Program.cs` или модульно в `CommandRegistry`. ([github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandHandlers/ICommandHandler.cs?plain=1), [github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/CommandRegistry.cs?plain=1))

## Приём PDF‑файлов

Бот принимает документы типа `application/pdf`; валидирует MIME‑тип/расширение и скачивает файл в каталог `Downloads/` рядом с исполняемым файлом. После успешной загрузки пользователь получает подтверждение. ([github.com](https://github.com/tgr484/TelegramBotProject/raw/master/Bot/FileHandlers/PdfFileHandler.cs?plain=1))

## Развёртывание

* **Docker** — самый простой способ. В `docker-compose.yml` уже описаны сервисы `bot` и `sql`.
* **Kubernetes / Helm** — вынесите переменные окружения и секреты в ConfigMap / Secret.
* **Bare‑metal / systemd** — выполните `dotnet publish -c Release`, скопируйте артефакты и создайте unit‑файл.

## Тестирование и разработка

```bash
# Запуск линтера и тестов (если добавите)
$ dotnet format
$ dotnet test
```

Для добавления новой функции рекомендуем придерживаться принципа **Single Responsibility**: отдельная папка/класс для каждой подсистемы (команды, файлы, состояния).

## Как внести вклад

1. Сделайте форк репозитория.
2. Создайте ветку `feature/<имя>`.
3. Откройте Pull Request в `master`.
4. Опишите изменения и свяжите с Issue (если есть).

## Лицензия

Проект распространяется под лицензией MIT. См. файл `LICENSE` (пока отсутствует).

---

> Документация сгенерирована автоматически на основе исходного кода репозитория *tgr484/TelegramBotProject*.
