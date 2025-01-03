# 👻 MediaPlayerBroadcaster.Daemon.CLI
![WindowsTerminal_kFcu67KCrG (Пользовательское)](https://github.com/user-attachments/assets/12c71cbb-6e50-4286-bd85-90cc058062f7)

## Описание

🎵 MediaPlayerBroadcaster.Daemon.CLI — это консольное приложение для Windows, которое отслеживает текущий воспроизводимый медиа-трек и отправляет информацию о нем на удаленный сервер. Приложение использует Windows.Media.Control для получения информации о воспроизводимых медиа-треках и HTTP-запросы для отправки данных на сервер.

## Основные функции

- **Отслеживание воспроизводимых медиа-треков**: Приложение отслеживает текущий воспроизводимый трек и отображает информацию о нем, включая название трека, исполнителя и обложку альбома.
- **Белый список приложений**: Пользователь может добавлять приложения в белый список, чтобы информация о треках из этих приложений отправлялась на сервер.
- **Отправка данных на сервер**: Приложение отправляет информацию о текущем треке и обложку альбома на удаленный сервер с использованием HTTP-запросов.
- **Обновление интерфейса**: Информация о текущем треке обновляется каждые 5 секунд.

## Установка

1. Скачайте исходный код проекта.
2. Откройте проект в Visual Studio.
3. Скомпилируйте и запустите проект.

## Использование

1. Убедитесь, что у вас есть файлы `ip.data` и `port.data` с IP-адресом и портом сервера соответственно.
2. Убедитесь, что у вас есть файл `whitelist.data` с приложениями, которые должны быть в белом списке.
3. Запустите приложение.
4. Приложение автоматически начнет отслеживать воспроизводимые медиа-треки и отправлять информацию о них на сервер.

## Зависимости

- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Windows.Media.Control](https://docs.microsoft.com/en-us/uwp/api/windows.media.control)

---

# 🖥️ MediaPlayerBroadcaster.NativeClient.WPF
![ce3zEbTTWm (Пользовательское)](https://github.com/user-attachments/assets/6bb29d4a-dae0-4777-95ff-21142ab8f351)

## Описание

🎵 MediaPlayerBroadcaster.NativeClient.WPF — это приложение для Windows, которое позволяет отслеживать текущий воспроизводимый медиа-трек и отображать информацию о нем. Приложение использует библиотеку MicaWPF для создания современного интерфейса и Windows.Media.Control для получения информации о воспроизводимых медиа-треках.

## Основные функции

- **Отслеживание воспроизводимых медиа-треков**: Приложение отслеживает текущий воспроизводимый трек и отображает информацию о нем, включая название трека, исполнителя и обложку альбома.
- **Белый список приложений**: Пользователь может добавлять приложения в белый список, чтобы информация о треках из этих приложений отображалась.
- **Обновление интерфейса**: Интерфейс обновляется каждые 5 секунд, чтобы отображать актуальную информацию о воспроизводимом треке.
- **Захват экрана**: Приложение поддерживает захват экрана с возможностью настройки размытия фона и радиуса углов окна.
- **Окно для захвата экрана**: Второе окно (`ForScreenCapture`) отображает информацию о текущем треке и позволяет пользователю перемещать его по экрану, а также настраивать внешний вид окна.

## Установка

1. Скачайте исходный код проекта.
2. Откройте проект в Visual Studio.
3. Скомпилируйте и запустите проект.

## Использование

1. Запустите приложение.
2. Приложение автоматически начнет отслеживать воспроизводимые медиа-треки.
3. Для добавления приложения в белый список нажмите кнопку "Accept".
4. Информация о текущем треке будет отображаться в интерфейсе приложения.
5. Второе окно (`ForScreenCapture`) будет отображать информацию о текущем треке и позволять пользователю перемещать его по экрану.

## Зависимости

- [MicaWPF](https://github.com/Simnico99/MicaWPF)
- [Windows.Media.Control](https://docs.microsoft.com/en-us/uwp/api/windows.media.control)

---

# 🌐 MediaPlayerBroadcaster.Server.CLI

## Описание

🎵 MediaPlayerBroadcaster.Server.CLI — это серверное приложение, которое предоставляет API для управления информацией о воспроизводимых медиа-треках. Приложение использует ASP.NET Core для создания веб-сервиса и Newtonsoft.Json для работы с JSON-данными.

## Основные функции

- **Установка информации о треке**: Приложение предоставляет endpoint для установки информации о текущем воспроизводимом треке (исполнитель, название трека, приложение).
- **Получение информации о треке**: Приложение предоставляет endpoint для получения текущей информации о воспроизводимом треке.
- **Установка изображения обложки**: Приложение предоставляет endpoint для установки изображения обложки текущего трека.
- **Получение изображения обложки**: Приложение предоставляет endpoint для получения текущего изображения обложки трека.

## Установка

1. Скачайте исходный код проекта.
2. Откройте проект в Visual Studio или другой поддерживаемой IDE.
3. Установите необходимые зависимости с помощью NuGet Package Manager.
4. Скомпилируйте и запустите проект.

## Использование

1. Запустите серверное приложение.
2. Используйте следующие endpoints для взаимодействия с сервером:

### Endpoints

- **POST /player/setplayerinfo**
  - Устанавливает информацию о текущем воспроизводимом треке.
  - Пример запроса:
    ```json
    {
      "Artist": "Исполнитель",
      "Track": "Название трека",
      "App": "Приложение"
    }
    ```
  - Пример ответа:
    ```json
    {
      "Status": "Данные плеера обновлены"
    }
    ```

- **GET /player/getplayerinfo**
  - Получает текущую информацию о воспроизводимом треке.
  - Пример ответа:
    ```json
    {
      "Artist": "Исполнитель",
      "Track": "Название трека",
      "App": "Приложение"
    }
    ```

- **POST /player/setplayerimage**
  - Устанавливает изображение обложки текущего трека.
  - Пример ответа:
    ```json
    {
      "Status": "Изображение плеера обновлено"
    }
    ```

- **GET /player/getplayerimage**
  - Получает текущее изображение обложки трека.
  - Пример ответа:
    - Изображение в формате JPEG.

## Зависимости

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)

## Контакты

Если у вас есть вопросы или предложения, пожалуйста, свяжитесь с нами через [GitHub Issues](https://github.com/Liis17/MediaPlayerBroadcaster/issues).
